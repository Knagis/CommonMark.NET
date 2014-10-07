using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal static class InlineMethods
    {
        private static readonly char[] SpecialCharacters = new[] { '\n', '\\', '`', '&', '_', '*', '[', ']', '<', '!' };

        /// <summary>
        /// Collapses internal whitespace to single space, removes leading/trailing whitespace, folds case.
        /// </summary>
        private static string NormalizeReference(string s)
        {
            if (s == null || s.Length == 0)
                return string.Empty;

            return NormalizeWhitespace(s, 0, s.Length).ToUpperInvariant();
        }

        // Returns reference if refmap contains a reference with matching
        // label, otherwise null.
        public static Reference lookup_reference(Dictionary<string, Reference> refmap, string lab)
        {
            if (refmap == null)
                return null;

            string label = NormalizeReference(lab);

            Reference r;
            if (refmap.TryGetValue(label, out r))
                return r;

            return null;
        }

        public static Reference make_reference(string label, string url, string title)
        {
            Reference r = new Reference();
            r.Label = NormalizeReference(label);
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
            e.Linkable.Label = label;
            e.Linkable.Url = url;
            e.Linkable.Title = title;
            return e;
        }

        private static Inline make_inlines(InlineTag t, Inline contents)
        {
            Inline e = new Inline();
            e.Tag = t;
            e.FirstChild = contents;
            return e;
        }

        // Create an inline with a literal string value.
        private static Inline make_literal(InlineTag t, string s)
        {
            Inline e = new Inline();
            e.Tag = t;
            e.LiteralContent = s;
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

            a.LastSibling.NextSibling = b;
            return a;
        }

        // Make a 'subject' from an input string.
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static Subject make_subject(string s, Dictionary<string, Reference> refmap)
        {
            return new Subject(s == null ? string.Empty : s.TrimEnd(), refmap);
        }

        // Return the next character in the subject, without advancing.
        // Return 0 if at the end of the subject.
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static char? peek_char(Subject subj)
        {
            return BString.bchar(subj.Buffer, subj.Position);
        }

        // Advance the subject.  Doesn't check for eof.
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static void advance(Subject subj)
        {
            subj.Position += 1;
        }

        // Try to process a backtick code span that began with a
        // span of ticks of length openticklength length (already
        // parsed).  Return 0 if you don't find matching closing
        // backticks, otherwise return the position in the subject
        // after the closing backticks.
        static int scan_to_closing_backticks(Subject subj, int openticklength)
        {
            // read non backticks
            var buf = subj.Buffer;
            var len = buf.Length;
            var cc = 0;
            var pos = subj.Position;

            for (var i = subj.Position; i < len; i++)
            {
                if (buf[i] == '`')
                {
                    cc++;
                }
                else
                {
                    if (cc == openticklength)
                    {
                        subj.Position = i;
                        return i;
                    }

                    i = buf.IndexOf('`', i) - 1;
                    if (i == -2)
                        return 0;

                    cc = 0;
                }
            }

            if (cc == openticklength)
            {
                subj.Position = len;
                return len;
            }

            return 0;
        }

        /// <summary>
        /// Collapses consecutive space and newline characters into a single space.
        /// Additionaly removes leading and trailing spaces.
        /// </summary>
        private static string NormalizeWhitespace(string s, int startIndex, int count)
        {
            char c;

            // count will actually be the lastIndex. The method argument is count only because other similar methods have startIndex/count
            count = startIndex + count - 1;

            // trim leading and trailing spaces.
            while (startIndex < count)
            {
                c = s[startIndex];
                if (c != ' ' && c != '\n') break;
                startIndex++;
            }

            while (count >= startIndex)
            {
                c = s[count];
                if (c != ' ' && c != '\n') break;
                count--;
            }

            if (count < startIndex)
                return string.Empty;

            // collapse inner whitespace
            // the complexity of this method is mainly so that the use of StringBuilder could be avoided if it is not needed
            StringBuilder sb = null;
            int pos = startIndex;
            int lastPos = startIndex;
            while (-1 != (pos = s.IndexOfAny(new[] { ' ', '\n' }, pos, count - pos)))
            {
                if (s[pos] == '\n')
                {
                    if (sb == null)
                        sb = new StringBuilder(s.Length);

                    // newline has to be replaced with ' '
                    sb.Append(s, lastPos, pos - lastPos);
                    sb.Append(' ');

                    // move past consecutive spaces
                    do
                    {
                        c = s[++pos];
                        if (c != ' ' && c != '\n')
                            break;
                    } while (pos < count);

                    lastPos = pos;
                }
                else
                {
                    c = s[++pos];

                    if (c == ' ' || c == '\n')
                    {
                        // multiple consecutive whitespaces
                        if (sb == null)
                            sb = new StringBuilder(s.Length);

                        sb.Append(s, lastPos, pos - lastPos);

                        // move past consecutive spaces
                        do
                        {
                            c = s[++pos];
                            if (c != ' ' && c != '\n')
                                break;
                        } while (pos < count);

                        lastPos = pos;
                    }
                }
            }

            if (sb == null)
                return s.Substring(startIndex, count - startIndex + 1);

            sb.Append(s, lastPos, count - lastPos + 1);
            return sb.ToString();
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
                return make_code(NormalizeWhitespace(subj.Buffer, startpos, endpos - startpos - ticklength));
            }
        }

        /// <summary>
        /// Scan ***, **, or * and return number scanned, or 0.
        /// </summary>
        static int scan_delims(Subject subj, char c, out bool can_open, out bool can_close)
        {
            int numdelims = 0;
            char char_before, char_after;
            int startpos = subj.Position;
            int len = subj.Buffer.Length;

            char_before = startpos == 0 ? '\n' : subj.Buffer[startpos - 1];
            while (startpos + numdelims < len && subj.Buffer[startpos + numdelims] == c)
                numdelims++;

            subj.Position = (startpos += numdelims);

            if (numdelims == 0 || numdelims > 3)
            {
                can_open = false;
                can_close = false;
                return numdelims;
            }

            char_after = len == startpos ? '\n' : subj.Buffer[startpos];

            can_open = char_after != ' ' && char_after != '\n';
            can_close = char_before != ' ' && char_before != '\n';

            if (c == '_')
            {
                can_open = can_open && !char.IsLetterOrDigit(char_before);
                can_close = can_close && !char.IsLetterOrDigit(char_after);
            }

            return numdelims;
        }

        private static Inline HandleEmphasis(Subject subj, char c)
        {
            bool can_open, can_close;
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
                    inl.LiteralContent = null;
                    inl.FirstChild = inl.NextSibling;
                    inl.NextSibling = null;

                    subj.EmphasisStack = istack.Previous;
                    istack.Previous = null;
                    subj.LastInline = inl;
                }
                else
                {
                    // the opener will only partially be used - stack entry remains (truncated) and a new inline is added.
                    var inl = istack.StartingInline;
                    istack.DelimeterCount -= useDelims;
                    inl.LiteralContent = istack.StartingInline.LiteralContent.Substring(0, istack.DelimeterCount);

                    var emph = useDelims == 1 ? make_emph(inl.NextSibling) : make_strong(inl.NextSibling);
                    inl.NextSibling = emph;
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
            var inlText = make_str(subj.Buffer.Substring(subj.Position - numdelims, numdelims));

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
            match = Scanner.scan_entity(subj.Buffer, subj.Position, subj.Buffer.Length - subj.Position);
            if (match > 0)
            {
                result = make_entity(subj.Buffer.Substring(subj.Position, match));
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
                if (c == '&')
                {
                    inew = handle_entity(subj);
                }
                else
                {
                    searchpos = subj.Buffer.IndexOf('&', subj.Position);
                    if (searchpos == -1)
                        searchpos = subj.Buffer.Length;

                    inew = make_str(subj.Buffer.Substring(subj.Position, searchpos - subj.Position));
                    subj.Position = searchpos;
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
                contents = subj.Buffer.Substring(subj.Position, matchlen - 1);
                subj.Position += matchlen;
                result = make_link(make_str_with_entities(contents), contents, "");
                return result;
            }
            // next try to match an email autolink
            matchlen = Scanner.scan_autolink_email(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                contents = subj.Buffer.Substring(subj.Position, matchlen - 1);
                subj.Position += matchlen;
                result = make_link(make_str_with_entities(contents), "mailto:" + contents, "");
                return result;
            }
            // finally, try to match an html tag
            matchlen = Scanner.scan_html_tag(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                contents = subj.Buffer.Substring(subj.Position - 1, matchlen + 1);
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

            var len = subj.Buffer.Length;
            char c = '\0';
            while (subj.Position < len && ((c = subj.Buffer[subj.Position]) != ']' || nestlevel > 0))
            {
                switch (c)
                {
                    case '`':
                        handle_backticks(subj);
                        break;
                    case '<':
                        handle_pointy_brace(subj);
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
                        if (char.IsPunctuation(subj.Buffer[subj.Position]))
                            advance(subj);
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
                    raw = subj.Buffer.Substring(startpos + 1, subj.Position - startpos - 1);
                    raw_label = raw;
                }
                subj.LabelNestingLevel = 0;
                advance(subj);  // advance past ]
                return true;
            }
            else
            {
                if (c == '\0')
                    subj.LabelNestingLevel = nestlevel;

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
                        url = subj.Buffer.Substring(starturl, endurl - starturl);
                        url = CleanUrl(url);
                        title = subj.Buffer.Substring(starttitle, endtitle - starttitle);
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
            var len = subj.Buffer.Length;
            while (subj.Position < len && subj.Buffer[subj.Position] == ' ')
                advance(subj);

            if (nlpos > 1 && subj.Buffer[nlpos - 1] == ' ' && subj.Buffer[nlpos - 2] == ' ')
                return make_linebreak();
            else
                return make_softbreak();
        }

        // Parse an inline, advancing subject, and add it to last element.
        // Adjust tail to point to new last element of list.
        // Return 0 if no inline can be parsed, 1 otherwise.
        public static Inline parse_inline(Subject subj)
        {
            Inline inew = null;
            string contents;
            int endpos;

            var c = subj.Buffer[subj.Position];

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
                    endpos = subj.Buffer.IndexOfAny(SpecialCharacters, subj.Position);

                    if (endpos == subj.Position)
                    {
                        // current char is special: read a 1-character str
                        contents = subj.Buffer[endpos].ToString();
                        advance(subj);
                    }
                    else if (endpos == -1)
                    {
                        // special char not found, take whole rest of buffer:
                        endpos = subj.Buffer.Length;
                        contents = subj.Buffer.Substring(subj.Position);
                        subj.Position = endpos;
                    }
                    else
                    {
                        // take buffer from subj.pos to endpos to str.
                        contents = subj.Buffer.Substring(subj.Position, endpos - subj.Position);

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

            var len = subj.Buffer.Length;

            if (len == 0)
                return null;

            var first = parse_inline(subj);
            subj.LastInline = first.LastSibling;

            Inline cur;
            while (subj.Position < len)
            {
                cur = parse_inline(subj);
                subj.LastInline.NextSibling = cur;
                subj.LastInline = cur.LastSibling;
            }

            return first;
        }

        // Parse zero or more space characters, including at most one newline.
        private static void spnl(Subject subj)
        {
            bool seen_newline = false;
            var len = subj.Buffer.Length;
            char c;
            while (subj.Position < len)
            {
                c = subj.Buffer[subj.Position];
                if (c == ' ' || (!seen_newline && (seen_newline = c == '\n')))
                    advance(subj);
                else
                    return;
            }
        }

        // Parse reference.  Assumes string begins with '[' character.
        // Modify refmap if a reference is encountered.
        // Return 0 if no reference found, otherwise position of subject
        // after reference is parsed.
        public static int ParseReference(Syntax.StringContent input, Dictionary<string, Reference> refmap)
        {
            Subject subj = make_subject(input.ToString(), null);
            string lab = string.Empty;
            string url = null;
            string title = null;
            int matchlen = 0;
            int beforetitle;

            // parse label:
            if (!link_label(subj, ref lab))
                return 0;

            // colon:
            if (peek_char(subj) == ':')
                advance(subj);
            else
                return 0;

            // parse link url:
            spnl(subj);
            matchlen = Scanner.scan_link_url(subj.Buffer, subj.Position);
            if (matchlen == 0)
                return 0;

            url = subj.Buffer.Substring(subj.Position, matchlen);
            url = CleanUrl(url);
            subj.Position += matchlen;
            
            // parse optional link_title
            beforetitle = subj.Position;
            spnl(subj);
            matchlen = Scanner.scan_link_title(subj.Buffer, subj.Position);
            if (matchlen > 0)
            {
                title = subj.Buffer.Substring(subj.Position, matchlen);
                title = CleanTitle(title);
                subj.Position += matchlen;
            }
            else
            {
                subj.Position = beforetitle;
                title = string.Empty;
            }

            // parse final spaces and newline:
            while (peek_char(subj) == ' ')
                advance(subj);

            if (peek_char(subj) == '\n')
                advance(subj);
            else if (peek_char(subj) != null)
                return 0;

            // insert reference into refmap
            add_reference(refmap, make_reference(lab, url, title));

            return subj.Position;
        }
    }
}
