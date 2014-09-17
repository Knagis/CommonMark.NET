using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal static class InlineMethods
    {

        // normalize reference:  collapse internal whitespace to single space,
        // remove leading/trailing whitespace, case fold
        static string normalize_reference(string s)
        {
            string normalized = s == null ? string.Empty : s.ToLower();
            int pos = 0;
            int startpos;
            char? c;
            while (null != (c = BString.bchar(normalized, pos)))
            {
                if (char.IsWhiteSpace(c.Value))
                {
                    startpos = pos;
                    // skip til next non-space
                    pos++;
                    while (char.IsWhiteSpace(BString.bchar(s, pos).Value))
                    {
                        pos++;
                    }
                    normalized = normalized.Remove(startpos, pos - startpos);
                    BString.binsertch(ref normalized, startpos, 1, ' ');
                    pos = startpos + 1;
                }
                pos++;
            }
            return normalized.Trim();
        }


        // Returns reference if refmap contains a reference with matching
        // label, otherwise null.
        public static Reference lookup_reference(Dictionary<string, Reference> refmap, string lab)
        {
            string label = normalize_reference(lab);
            if (refmap == null)
            {
                return null;
            }
            Reference r;
            if (refmap.TryGetValue(label, out r))
                return r;

            return null;
        }

        public static Reference make_reference(string label, string url, string title)
        {
            Reference r = new Reference();
            r.Label = normalize_reference(label);
            r.Url = url;
            r.Title = title;
            return r;
        }

        public static void add_reference(Dictionary<string, Reference> refmap, Reference refer)
        {
            if (refmap.ContainsKey(refer.Label))
                return;

            refmap.Add(refer.Label, refer);
        }

        // Create an inline with a linkable string value.
        private static Inline make_linkable(InlineTag t, Inline label, string url, string title)
        {
            Inline e = new Inline();
            e.Tag = t;
            e.Content.Linkable.Label = label;
            e.Content.Linkable.Url = url;
            e.Content.Linkable.Title = title;
            return e;
        }

        private static Inline make_inlines(InlineTag t, Inline contents)
        {
            Inline e = new Inline();
            e.Tag = t;
            e.Content.Inlines = contents;
            return e;
        }

        // Create an inline with a literal string value.
        private static Inline make_literal(InlineTag t, string s)
        {
            Inline e = new Inline();
            e.Tag = t;
            e.Content.Literal = s;
            return e;
        }

        // Create an inline with no value.
        private static Inline make_simple(InlineTag t)
        {
            Inline e = new Inline();
            e.Tag = t;
            return e;
        }

        // Macros for creating various kinds of inlines.
        private static Inline make_str(string s) { return make_literal(InlineTag.String, s); }
        private static Inline make_code(string s) { return make_literal(InlineTag.Code, s); }
        private static Inline make_raw_html(string s) { return make_literal(InlineTag.RawHtml, s); }
        private static Inline make_entity(string s) { return make_literal(InlineTag.Entity, s); }
        private static Inline make_linebreak() { return make_simple(InlineTag.LineBreak); }
        private static Inline make_softbreak() { return make_simple(InlineTag.SoftBreak); }
        private static Inline make_link(Inline label, string url, string title) { return make_linkable(InlineTag.Link, label, url, title); }
        private static Inline make_image(Inline alt, string url, string title) { return make_linkable(InlineTag.Image, alt, url, title); }
        private static Inline make_emph(Inline contents) { return make_inlines(InlineTag.Emphasis, contents); }
        private static Inline make_strong(Inline contents) { return make_inlines(InlineTag.Strong, contents); }

        // Append inline list b to the end of inline list a.
        // Return pointer to head of new list.
        private static Inline append_inlines(Inline a, Inline b)
        {
            if (a == null)
            {  // null acts like an empty list
                return b;
            }

            a.LastSibling.Next = b;
            return a;
        }

        // Make a 'subject' from an input string.
        private static Subject make_subject(string s, Dictionary<string, Reference> refmap)
        {
            Subject e = new Subject();
            // remove final whitespace
            e.Buffer = s == null ? string.Empty : s.TrimEnd();
            e.Position = 0;
            e.LabelNestingLevel = 0;
            e.ReferenceMap = refmap;
            return e;
        }

        // Return the next character in the subject, without advancing.
        // Return 0 if at the end of the subject.
        private static char? peek_char(Subject subj)
        {
            return BString.bchar(subj.Buffer, subj.Position);
        }

        // Return true if there are more characters in the subject.
        private static bool is_eof(Subject subj)
        {
            return (subj.Position >= subj.Buffer.Length);
        }

        // Advance the subject.  Doesn't check for eof.
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static void advance(Subject subj)
        {
            subj.Position += 1;
        }

        // Take one character and return a string, or null if eof.
        private static string take_one(Subject subj)
        {
            int startpos = subj.Position;
            if (is_eof(subj))
            {
                return null;
            }
            else
            {
                advance(subj);
                return BString.bmidstr(subj.Buffer, startpos, 1);
            }
        }

        // Try to process a backtick code span that began with a
        // span of ticks of length openticklength length (already
        // parsed).  Return 0 if you don't find matching closing
        // backticks, otherwise return the position in the subject
        // after the closing backticks.
        static int scan_to_closing_backticks(Subject subj, int openticklength)
        {
            // read non backticks
            char? c;
            while (null != (c = peek_char(subj)) && c != '`')
            {
                advance(subj);
            }
            if (is_eof(subj))
            {
                return 0;  // did not find closing ticks, return 0
            }
            int numticks = 0;
            while (peek_char(subj) == '`')
            {
                advance(subj);
                numticks++;
            }
            if (numticks != openticklength)
            {
                return (scan_to_closing_backticks(subj, openticklength));
            }
            return (subj.Position);
        }

        // Destructively modify string, collapsing consecutive
        // space and newline characters into a single space.
        static void normalize_whitespace(ref string s)
        {
            bool last_char_was_space = false;
            int pos = 0;
            char? c;
            while (null != (c = BString.bchar(s, pos)))
            {
                switch (c)
                {
                    case ' ':
                        if (last_char_was_space)
                        {
                            s = s.Remove(pos, 1);
                        }
                        else
                        {
                            pos++;
                        }
                        last_char_was_space = true;
                        break;
                    case '\n':
                        if (last_char_was_space)
                        {
                            s = s.Remove(pos, 1);
                        }
                        else
                        {
                            s = s.Remove(pos, 1);
                            BString.binsertch(ref s, pos, 1, ' ');
                            pos++;
                        }
                        last_char_was_space = true;
                        break;
                    default:
                        pos++;
                        last_char_was_space = false;
                        break;
                }
            }
        }

        // Parse backtick code section or raw backticks, return an inline.
        // Assumes that the subject has a backtick at the current position.
        static Inline handle_backticks(Subject subj)
        {
            int ticklength = 0;
            var bl = subj.Buffer.Length;
            while (subj.Position < bl && (subj.Buffer[subj.Position] == '`'))
            {
                ticklength++;
                subj.Position++;
            }

            int startpos = subj.Position;
            int endpos = scan_to_closing_backticks(subj, ticklength);
            if (endpos == 0)
            { 
                // closing not found
                subj.Position = startpos; // rewind to right after the opening ticks
                return make_str(new string('`', ticklength));
            }
            else
            {
                var result = BString.bmidstr(subj.Buffer, startpos, endpos - startpos - ticklength);
                result = result.Trim();
                normalize_whitespace(ref result);
                return make_code(result);
            }
        }

        // Scan ***, **, or * and return number scanned, or 0.
        // Don't advance position.
        static int scan_delims(Subject subj, char c, out bool can_open, out bool can_close)
        {
            int numdelims = 0;
            char? char_before, char_after;
            int startpos = subj.Position;

            char_before = subj.Position == 0 ? '\n' : BString.bchar(subj.Buffer, subj.Position - 1);
            while (peek_char(subj) == c)
            {
                numdelims++;
                advance(subj);
            }
            
            char_after = peek_char(subj);
            can_open = numdelims > 0 && numdelims <= 3 && char_after != null && !char.IsWhiteSpace(char_after.Value);
            can_close = numdelims > 0 && numdelims <= 3 && char_before != null && !char.IsWhiteSpace(char_before.Value);
            
            if (c == '_')
            {
                can_open = can_open && (char_before == null || !char.IsLetterOrDigit(char_before.Value));
                can_close = can_close && (char_after == null || !char.IsLetterOrDigit(char_after.Value));
            }

            return numdelims;
        }

        private static Inline HandleEmphasis(Subject subj, char c)
        {
            bool can_open = false, can_close = false;
            var numdelims = scan_delims(subj, c, out can_open, out can_close);

            if (can_close)
            {
                // walk the stack and find a matching opener, if there is one
                var istack = subj.EmphasisStack;
                while (true)
                {
                    if (istack == null)
                        goto cannotClose;

                    // the only combination that is not processed is **foo*
                    if ((istack.DelimeterCount != 2 || numdelims != 1) && istack.Delimeter == c)
                        break;

                    istack = istack.Previous;
                }

                // calculate the actual number of delimeters used from this closer
                var useDelims = istack.DelimeterCount;
                if (useDelims == 3) useDelims = numdelims == 3 ? 1 : numdelims;
                else if (useDelims > numdelims) useDelims = 1;

                if (istack.DelimeterCount == useDelims)
                {
                    // the opener is completely used up - remove the stack entry and reuse the inline element
                    var inl = istack.StartingInline;
                    inl.Tag = useDelims == 1 ? InlineTag.Emphasis : InlineTag.Strong;
                    inl.Content.Literal = null;
                    inl.Content.Inlines = inl.Next;
                    inl.Next = null;

                    subj.EmphasisStack = istack.Previous;
                    istack.Previous = null;
                    subj.LastInline = inl;
                }
                else
                {
                    // the opener will only partially be used - stack entry remains (truncated) and a new inline is added.
                    var inl = istack.StartingInline;
                    istack.DelimeterCount -= useDelims;
                    inl.Content.Literal = istack.StartingInline.Content.Literal.Substring(0, istack.DelimeterCount);

                    var emph = useDelims == 1 ? make_emph(inl.Next) : make_strong(inl.Next);
                    inl.Next = emph;
                    subj.LastInline = emph;
                }

                // if the closer was not fully used, move back a char or two and try again.
                if (useDelims < numdelims)
                {
                    subj.Position = subj.Position - numdelims + useDelims;
                    return HandleEmphasis(subj, c);
                }

                return make_str(string.Empty);
            }

            cannotClose:
            var inlText = make_str(BString.bmidstr(subj.Buffer, subj.Position - numdelims, numdelims));
            
            if (can_open)
            {
                var istack = new InlineStack();
                istack.DelimeterCount = numdelims;
                istack.Delimeter = c;
                istack.StartingInline = inlText;
                istack.Previous = subj.EmphasisStack;
                subj.EmphasisStack = istack;
            }

            return inlText;
        }

        // Parse backslash-escape or just a backslash, returning an inline.
        private static Inline handle_backslash(Subject subj)
        {
            advance(subj);

            if (subj.Position >= subj.Buffer.Length)
                return make_str("\\");

            var nextChar = subj.Buffer[subj.Position];

            if (Utilities.IsAsciiSymbol(nextChar))
            {  
                // only ascii symbols and newline can be escaped
                advance(subj);
                return make_str(nextChar.ToString());
            }
            else if (nextChar == '\n')
            {
                advance(subj);
                return make_linebreak();
            }
            else
            {
                return make_str("\\");
            }
        }

        // Parse an entity or a regular "&" string.
        // Assumes the subject has an '&' character at the current position.
        private static Inline handle_entity(Subject subj)
        {
            int match;
            Inline result;
            match = Scanner.scan_entity(subj.Buffer, subj.Position);
            if (match > 0)
            {
                result = make_entity(BString.bmidstr(subj.Buffer, subj.Position, match));
                subj.Position += match;
            }
            else
            {
                advance(subj);
                result = make_str("&");
            }
            return result;
        }

        // Like make_str, but parses entities.
        // Returns an inline sequence consisting of str and entity elements.
        static Inline make_str_with_entities(string s)
        {
            Inline result = null;
            Inline inew;
            int searchpos;
            char? c;
            Subject subj = make_subject(s, null);

            while (null != (c = peek_char(subj)))
            {
                switch (c)
                {
                    case '&':
                        inew = handle_entity(subj);
                        break;
                    default:
                        searchpos = BString.bstrchrp(subj.Buffer, '&', subj.Position);
                        if (searchpos == -1)
                        {
                            searchpos = subj.Buffer.Length;
                        }
                        inew = make_str(BString.bmidstr(subj.Buffer, subj.Position, searchpos - subj.Position));
                        subj.Position = searchpos;
                        break;
                }
                result = append_inlines(result, inew);
            }
            return result;
        }

        /// <summary>
        /// Destructively unescape a string: remove backslashes before punctuation or symbol characters.
        /// </summary>
        /// <param name="url">The string data that will be changed by unescaping any punctuation or symbol characters.</param>
        public static string Unescape(string url)
        {
            // remove backslashes before punctuation chars:
            int searchpos = 0;
            char c;
            while ((searchpos = url.IndexOf('\\', searchpos)) != -1)
            {
                searchpos++;
                if (url.Length == searchpos)
                    break;

                c = url[searchpos];
                if (Utilities.IsAsciiSymbol(c))
                    url = url.Remove(searchpos - 1, 1);
            }

            return url;
        }

        /// <summary>
        /// Clean a URL: remove surrounding whitespace and surrounding &lt; &gt; and remove \ that escape punctuation and other symbols.
        /// </summary>
        /// <remarks>Original: clean_url(ref string)</remarks>
        private static string CleanUrl(string url)
        {
            if (url.Length == 0)
                return url;

            // remove surrounding <> if any:
            url = url.Trim();

            if (url[0] == '<' && url[url.Length - 1] == '>')
                url = url.Substring(1, url.Length - 2);

            return Unescape(url);
        }

        /// <summary>
        /// Clean a title: remove surrounding quotes and remove \ that escape punctuation.
        /// </summary>
        /// <remarks>Original: clean_title(ref string)</remarks>
        private static string CleanTitle(string title)
        {
            // remove surrounding quotes if any:
            int titlelength = title.Length;
            if (titlelength == 0)
                return title;

            var a = title[0];
            var b = title[titlelength - 1];
            if ((a == '\'' && b == '\'') || (a == '(' && b == ')') || (a == '"' && b == '"'))
                title = title.Substring(1, titlelength - 2);

            return Unescape(title);
        }

        // Parse an autolink or HTML tag.
        // Assumes the subject has a '<' character at the current position.
        static Inline handle_pointy_brace(Subject subj)
        {
            int matchlen = 0;
            string contents;
            Inline result;

            advance(subj);  // advance past first <
            // first try to match a URL autolink
            matchlen = Scanner.scan_autolink_uri(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                contents = BString.bmidstr(subj.Buffer, subj.Position, matchlen - 1);
                subj.Position += matchlen;
                result = make_link(make_str_with_entities(contents), contents, "");
                return result;
            }
            // next try to match an email autolink
            matchlen = Scanner.scan_autolink_email(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                contents = BString.bmidstr(subj.Buffer, subj.Position, matchlen - 1);
                subj.Position += matchlen;
                result = make_link(make_str_with_entities(contents),
                                   "mailto:" + contents, "");
                return result;
            }
            // finally, try to match an html tag
            matchlen = Scanner.scan_html_tag(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                contents = BString.bmidstr(subj.Buffer, subj.Position - 1, matchlen + 1);
                subj.Position += matchlen;
                return make_raw_html(contents);
            }
            else
            {// if nothing matches, just return the opening <:
                return make_str("<");
            }
        }

        // Parse a link label.  Returns 1 if successful.
        // Unless raw_label is null, it is set to point to the raw contents of the [].
        // Assumes the subject has a '[' character at the current position.
        // Returns 0 and does not advance if no matching ] is found.
        // Note the precedence:  code backticks have precedence over label bracket
        // markers, which have precedence over *, _, and other inline formatting
        // markers. So, 2 below contains a link while 1 does not:
        // 1. [a link `with a ](/url)` character
        // 2. [a link *with emphasized ](/url) text*
        static bool link_label(Subject subj, ref string raw_label)
        {
            int nestlevel = 0;
            Inline tmp = null;
            string raw;
            int startpos = subj.Position;
            if (subj.LabelNestingLevel > 0)
            {
                // if we've already checked to the end of the subject
                // for a label, even with a different starting [, we
                // know we won't find one here and we can just return.
                // Note:  nestlevel 1 would be: [foo [bar]
                // nestlevel 2 would be: [foo [bar [baz]
                subj.LabelNestingLevel--;
                return false;
            }
            advance(subj);  // advance past [
            char? c;
            while (null != (c = peek_char(subj)) && (c != ']' || nestlevel > 0))
            {
                switch (c)
                {
                    case '`':
                        tmp = handle_backticks(subj);
                        break;
                    case '<':
                        tmp = handle_pointy_brace(subj);
                        break;
                    case '[':  // nested []
                        nestlevel++;
                        advance(subj);
                        break;
                    case ']':  // nested []
                        nestlevel--;
                        advance(subj);
                        break;
                    case '\\':
                        advance(subj);
                        if (char.IsPunctuation(peek_char(subj).Value))
                        {
                            advance(subj);
                        }
                        break;
                    default:
                        advance(subj);
                        break;
                }
            }
            if (c == ']')
            {
                if (raw_label != null)
                {
                    raw = BString.bmidstr(subj.Buffer, startpos + 1, subj.Position - (startpos + 1));
                    raw_label = raw;
                }
                subj.LabelNestingLevel = 0;
                advance(subj);  // advance past ]
                return true;
            }
            else
            {
                if (c == 0)
                {
                    subj.LabelNestingLevel = nestlevel;
                }
                subj.Position = startpos; // rewind
                return false;
            }
        }

        // Parse a link or the link portion of an image, or return a fallback.
        static Inline handle_left_bracket(Subject subj)
        {
            Inline lab = null;
            Inline result = null;
            Reference refer;
            int n;
            int sps;
            bool found_label;
            int endlabel, starturl, endurl, starttitle, endtitle, endall;
            string url, title, reflabel;
            string rawlabel = "";
            string rawlabel2 = "";
            found_label = link_label(subj, ref rawlabel);
            endlabel = subj.Position;
            if (found_label)
            {
                if (peek_char(subj) == '(' &&
                    ((sps = Scanner.scan_spacechars(subj.Buffer, subj.Position + 1)) > -1) &&
                    ((n = Scanner.scan_link_url(subj.Buffer, subj.Position + 1 + sps)) > -1))
                {
                    // try to parse an explicit link:
                    starturl = subj.Position + 1 + sps; // after (
                    endurl = starturl + n;
                    starttitle = endurl + Scanner.scan_spacechars(subj.Buffer, endurl);
                    // ensure there are spaces btw url and title
                    endtitle = (starttitle == endurl) ? starttitle :
                               starttitle + Scanner.scan_link_title(subj.Buffer, starttitle);
                    endall = endtitle + Scanner.scan_spacechars(subj.Buffer, endtitle);
                    if (BString.bchar(subj.Buffer, endall) == ')')
                    {
                        subj.Position = endall + 1;
                        url = BString.bmidstr(subj.Buffer, starturl, endurl - starturl);
                        url = CleanUrl(url);
                        title = BString.bmidstr(subj.Buffer, starttitle, endtitle - starttitle);
                        title = CleanTitle(title);
                        lab = parse_inlines(rawlabel, null);
                        return make_link(lab, url, title);
                    }
                    else
                    {
                        // if we get here, we matched a label but didn't get further:
                        subj.Position = endlabel;
                        lab = parse_inlines(rawlabel, subj.ReferenceMap);
                        result = append_inlines(make_str("["),
                                                append_inlines(lab, make_str("]")));
                        return result;
                    }
                }
                else
                {
                    // Check for reference link.
                    // First, see if there's another label:
                    subj.Position = subj.Position + Scanner.scan_spacechars(subj.Buffer, endlabel);
                    reflabel = rawlabel;
                    // if followed by a nonempty link label, we change reflabel to it:
                    if (peek_char(subj) == '[' &&
                        link_label(subj, ref rawlabel2))
                    {
                        if (rawlabel2 != null && rawlabel2.Length > 0)
                        {
                            reflabel = rawlabel2;
                        }
                    }
                    else
                    {
                        subj.Position = endlabel;
                    }
                    // lookup rawlabel in subject.reference_map:
                    refer = lookup_reference(subj.ReferenceMap, reflabel);
                    if (refer != null)
                    { // found
                        lab = parse_inlines(rawlabel, null);
                        result = make_link(lab, refer.Url, refer.Title);
                    }
                    else
                    {
                        subj.Position = endlabel;
                        lab = parse_inlines(rawlabel, subj.ReferenceMap);
                        result = append_inlines(make_str("["),
                                               append_inlines(lab, make_str("]")));
                    }
                    return result;
                }
            }
            // If we fall through to here, it means we didn't match a link:
            advance(subj);  // advance past [
            return make_str("[");
        }

        // Parse a hard or soft linebreak, returning an inline.
        // Assumes the subject has a newline at the current position.
        static Inline handle_newline(Subject subj)
        {
            int nlpos = subj.Position;
            // skip over newline
            advance(subj);
            // skip spaces at beginning of line
            while (peek_char(subj) == ' ')
            {
                advance(subj);
            }
            if (nlpos > 1 &&
                BString.bchar(subj.Buffer, nlpos - 1) == ' ' &&
                BString.bchar(subj.Buffer, nlpos - 2) == ' ')
            {
                return make_linebreak();
            }
            else
            {
                return make_softbreak();
            }
        }

        // Parse inlines while a predicate is satisfied.  Return inlines.
        public static Inline parse_inlines_while(Subject subj)
        {
            Inline first = null;
            Inline cur;
            while (!is_eof(subj))
            {
                cur = parse_inline(subj);
                if (first == null)
                {
                    first = cur;
                    subj.LastInline = cur.LastSibling;
                }
                else
                {
                    subj.LastInline.Next = cur;
                    subj.LastInline = cur.LastSibling;
                }
            }

            return first;
        }

        // Parse an inline, advancing subject, and add it to last element.
        // Adjust tail to point to new last element of list.
        // Return 0 if no inline can be parsed, 1 otherwise.
        public static Inline parse_inline(Subject subj)
        {
            Inline inew = null;
            string contents;
            string special_chars;
            char? c;
            int endpos;
            c = peek_char(subj);
            if (c == null)
                return null;

            switch (c)
            {
                case '\n':
                    inew = handle_newline(subj);
                    break;
                case '`':
                    inew = handle_backticks(subj);
                    break;
                case '\\':
                    inew = handle_backslash(subj);
                    break;
                case '&':
                    inew = handle_entity(subj);
                    break;
                case '<':
                    inew = handle_pointy_brace(subj);
                    break;
                case '_':
                    inew = HandleEmphasis(subj, '_');
                    break;
                case '*':
                    inew = HandleEmphasis(subj, '*');
                    break;
                case '[':
                    inew = handle_left_bracket(subj);
                    break;
                case '!':
                    advance(subj);
                    if (peek_char(subj) == '[')
                    {
                        inew = handle_left_bracket(subj);
                        if (inew != null && inew.Tag == InlineTag.Link)
                        {
                            inew.Tag = InlineTag.Image;
                        }
                        else
                        {
                            inew = append_inlines(make_str("!"), inew);
                        }
                    }
                    else
                    {
                        inew = make_str("!");
                    }
                    break;
                default:
                    // we read until we hit a special character
                    special_chars = "\n\\`&_*[]<!";
                    endpos = BString.binchr(subj.Buffer, subj.Position, special_chars);
                    if (endpos == subj.Position)
                    {
                        // current char is special: read a 1-character str
                        contents = take_one(subj);
                    }
                    else if (endpos == -1)
                    {
                        // special char not found, take whole rest of buffer:
                        endpos = subj.Buffer.Length;
                        contents = BString.bmidstr(subj.Buffer, subj.Position, endpos - subj.Position);
                        subj.Position = endpos;
                    }
                    else
                    {
                        // take buffer from subj.pos to endpos to str.
                        contents = BString.bmidstr(subj.Buffer, subj.Position, endpos - subj.Position);
                        subj.Position = endpos;
                        // if we're at a newline, strip trailing spaces.
                        if (peek_char(subj) == '\n')
                            contents = contents.TrimEnd();
                    }
                    inew = make_str(contents);
                    break;
            }

            return inew;
        }

        public static Inline parse_inlines(string input, Dictionary<string, Reference> refmap)
        {
            Subject subj = make_subject(input, refmap);
            return parse_inlines_while(subj);
        }

        // Parse zero or more space characters, including at most one newline.
        private static void spnl(Subject subj)
        {
            bool seen_newline = false;
            while (peek_char(subj) == ' ' ||
                   (!seen_newline &&
                    (seen_newline = peek_char(subj) == '\n')))
            {
                advance(subj);
            }
        }

        // Parse reference.  Assumes string begins with '[' character.
        // Modify refmap if a reference is encountered.
        // Return 0 if no reference found, otherwise position of subject
        // after reference is parsed.
        public static int parse_reference(string input, Dictionary<string, Reference> refmap)
        {
            Subject subj = make_subject(input, null);
            string lab = "";
            string url = null;
            string title = null;
            int matchlen = 0;
            int beforetitle;
            Reference inew = null;
            int newpos;

            // parse label:
            if (!link_label(subj, ref lab))
            {
                return 0;
            }
            // colon:
            if (peek_char(subj) == ':')
            {
                advance(subj);
            }
            else
            {
                return 0;
            }
            // parse link url:
            spnl(subj);
            matchlen = Scanner.scan_link_url(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                url = BString.bmidstr(subj.Buffer, subj.Position, matchlen);
                url = CleanUrl(url);
                subj.Position += matchlen;
            }
            else
            {
                return 0;
            }
            // parse optional link_title
            beforetitle = subj.Position;
            spnl(subj);
            matchlen = Scanner.scan_link_title(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                title = BString.bmidstr(subj.Buffer, subj.Position, matchlen);
                title = CleanTitle(title);
                subj.Position += matchlen;
            }
            else
            {
                subj.Position = beforetitle;
                title = "";
            }
            // parse final spaces and newline:
            while (peek_char(subj) == ' ')
            {
                advance(subj);
            }
            if (peek_char(subj) == '\n')
            {
                advance(subj);
            }
            else if (peek_char(subj) != null)
            {
                return 0;
            }
            // insert reference into refmap
            inew = make_reference(lab, url, title);
            add_reference(refmap, inew);

            newpos = subj.Position;
            return newpos;
        }
    }
}
