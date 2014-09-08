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
            e.tag = t;
            e.content.linkable.label = label;
            e.content.linkable.url = url;
            e.content.linkable.title = title;
            e.next = null;
            return e;
        }

        private static Inline make_inlines(InlineTag t, Inline contents)
        {
            Inline e = new Inline();
            e.tag = t;
            e.content.inlines = contents;
            e.next = null;
            return e;
        }


        // Create an inline with a literal string value.
        private static Inline make_literal(InlineTag t, string s)
        {
            Inline e = new Inline();
            e.tag = t;
            e.content.Literal = s;
            e.next = null;
            return e;
        }

        // Create an inline with no value.
        private static Inline make_simple(InlineTag t)
        {
            Inline e = new Inline();
            e.tag = t;
            e.next = null;
            return e;
        }

        // Macros for creating various kinds of inlines.
        private static Inline make_str(string s) { return make_literal(InlineTag.str, s); }
        private static Inline make_code(string s) { return make_literal(InlineTag.code, s); }
        private static Inline make_raw_html(string s) { return make_literal(InlineTag.raw_html, s); }
        private static Inline make_entity(string s) { return make_literal(InlineTag.entity, s); }
        private static Inline make_linebreak() { return make_simple(InlineTag.linebreak); }
        private static Inline make_softbreak() { return make_simple(InlineTag.softbreak); }
        private static Inline make_link(Inline label, string url, string title) { return make_linkable(InlineTag.link, label, url, title); }
        private static Inline make_image(Inline alt, string url, string title) { return make_linkable(InlineTag.image, alt, url, title); }
        private static Inline make_emph(Inline contents) { return make_inlines(InlineTag.emph, contents); }
        private static Inline make_strong(Inline contents) { return make_inlines(InlineTag.strong, contents); }

        // Append inline list b to the end of inline list a.
        // Return pointer to head of new list.
        private static Inline append_inlines(Inline a, Inline b)
        {
            if (a == null)
            {  // null acts like an empty list
                return b;
            }
            Inline cur = a;
            while (cur.next != null)
            {
                cur = cur.next;
            }
            cur.next = b;
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

        private static bool isbacktick(char c)
        {
            return (c == '`');
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
        private static void advance(Subject subj)
        {
            subj.Position += 1;
        }


        // Take characters while a predicate holds, and return a string.
        private static string take_while(Subject subj, Predicate<char> predicate)
        {
            char? c;
            int startpos = subj.Position;
            int len = 0;
            while (null != (c = peek_char(subj)) && predicate(c.Value))
            {
                advance(subj);
                len++;
            }
            return BString.bmidstr(subj.Buffer, startpos, len);
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
            string openticks = take_while(subj, isbacktick);
            string result;
            int ticklength = openticks.Length;
            int startpos = subj.Position;
            int endpos = scan_to_closing_backticks(subj, ticklength);
            if (endpos == 0)
            { // not found
                subj.Position = startpos; // rewind
                return make_str(openticks);
            }
            else
            {
                result = BString.bmidstr(subj.Buffer, startpos, endpos - startpos - ticklength);
                result = result.Trim();
                normalize_whitespace(ref result);
                return make_code(result);
            }
        }

        // Scan ***, **, or * and return number scanned, or 0.
        // Don't advance position.
        static int scan_delims(Subject subj, char c, ref bool can_open, ref bool can_close)
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
            subj.Position = startpos;
            return numdelims;
        }

        // Parse strong/emph or a fallback.
        // Assumes the subject has '_' or '*' at the current position.
        private static Inline handle_strong_emph(Subject subj, char c)
        {
            bool can_open = false, can_close = false;
            Inline result = null;
            Inline last;
            Inline inew;
            Inline il;
            Inline first_head = null;
            Inline first_close = null;
            int first_close_delims = 0;
            int numdelims;

            last = null;

            numdelims = scan_delims(subj, c, ref can_open, ref can_close);
            subj.Position += numdelims;

            inew = make_str(BString.bmidstr(subj.Buffer, subj.Position - numdelims, numdelims));
            last = inew;
            first_head = inew;
            result = inew;

            if (!can_open || numdelims == 0)
            {
                goto done;
            }

            switch (numdelims)
            {
                case 1:
                    while (true)
                    {
                        numdelims = scan_delims(subj, c, ref can_open, ref can_close);
                        if (numdelims >= 1 && can_close)
                        {
                            subj.Position += 1;
                            first_head.tag = InlineTag.emph;
                            first_head.content.Literal = null;
                            first_head.content.inlines = first_head.next;
                            first_head.next = null;
                            goto done;
                        }
                        else
                        {
                            if (!parse_inline(subj, ref last))
                            {
                                goto done;
                            }
                        }
                    }
                    break;
                case 2:
                    while (true)
                    {
                        numdelims = scan_delims(subj, c, ref can_open, ref can_close);
                        if (numdelims >= 2 && can_close)
                        {
                            subj.Position += 2;
                            first_head.tag = InlineTag.strong;
                            first_head.content.Literal = null;
                            first_head.content.inlines = first_head.next;
                            first_head.next = null;
                            goto done;
                        }
                        else
                        {
                            if (!parse_inline(subj, ref last))
                            {
                                goto done;
                            }
                        }
                    }
                    break;
                case 3:
                    while (true)
                    {
                        numdelims = scan_delims(subj, c, ref can_open, ref can_close);
                        if (can_close && numdelims >= 1 && numdelims <= 3 &&
                            numdelims != first_close_delims)
                        {
                            inew = make_str(BString.bmidstr(subj.Buffer, subj.Position, numdelims));
                            append_inlines(last, inew);
                            last = inew;

                            if (first_close_delims == 1 && numdelims > 2)
                            {
                                numdelims = 2;
                            }
                            else if (first_close_delims == 2)
                            {
                                numdelims = 1;
                            }
                            else if (numdelims == 3)
                            {
                                // If we opened with ***, we interpret it as ** followed by *
                                // giving us <strong><em>
                                numdelims = 1;
                            }

                            subj.Position += numdelims;
                            if (first_close != null)
                            {
                                first_head.tag = first_close_delims == 1 ? InlineTag.strong : InlineTag.emph;
                                first_head.content.Literal = null;
                                first_head.content.inlines =
                                  make_inlines(first_close_delims == 1 ? InlineTag.emph : InlineTag.strong,
                                               first_head.next);

                                il = first_head.next;
                                while (il.next != null && il.next != first_close)
                                {
                                    il = il.next;
                                }
                                il.next = null;

                                first_head.content.inlines.next = first_close.next;

                                il = first_head.content.inlines;
                                while (il.next != null && il.next != last)
                                {
                                    il = il.next;
                                }
                                il.next = null;

                                first_close.next = null;
                                first_head.next = null;
                                goto done;
                            }
                            else
                            {
                                first_close = last;
                                first_close_delims = numdelims;
                            }
                        }
                        else
                        {
                            if (!parse_inline(subj, ref last))
                            {
                                goto done;
                            }
                        }
                    }
                    break;
                default:
                    goto done;
            }

        done:
            return result;
        }

        // Parse backslash-escape or just a backslash, returning an inline.
        private static Inline handle_backslash(Subject subj)
        {
            advance(subj);
            char? nextchar = peek_char(subj);
            if (nextchar != null && (char.IsPunctuation(nextchar.Value) || char.IsSymbol(nextchar.Value)))
            {  // only ascii symbols and newline can be escaped
                advance(subj);
                return make_str(nextchar.ToString());
            }
            else if (nextchar == '\n')
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

        // Destructively unescape a string: remove backslashes before punctuation chars.
        public static void unescape(ref string url)
        {
            // remove backslashes before punctuation chars:
            int searchpos = 0;
            while ((searchpos = BString.bstrchrp(url, '\\', searchpos)) != -1)
            {
                var c = BString.bchar(url, searchpos + 1);
                if (c != null && (char.IsPunctuation(c.Value) || char.IsSymbol(c.Value)))
                {
                    url = url.Remove(searchpos, 1);
                }
                else
                {
                    searchpos++;
                }
            }
        }

        // Clean a URL: remove surrounding whitespace and surrounding <>,
        // and remove \ that escape punctuation.
        private static void clean_url(ref string url)
        {
            if (url.Length == 0)
                return;

            // remove surrounding <> if any:
            url = url.Trim(); ;
            if (url[0] == '<' && url[url.Length - 1] == '>')
            {
                url = url.Substring(1, url.Length - 2);
            }
            unescape(ref url);
        }

        // Clean a title: remove surrounding quotes and remove \ that escape punctuation.
        static void clean_title(ref string title)
        {
            // remove surrounding quotes if any:
            int titlelength = title.Length;
            if (titlelength == 0)
                return;

            var a = title[0];
            var b = title[title.Length - 1];
            if ((a == '\'' && b == '\'') ||
                (a == '(' && b == ')') ||
                (a == '"' && b == '"'))
            {
                title = title.Substring(1, title.Length - 2);
            }
            unescape(ref title);
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
                contents = BString.bmidstr(subj.Buffer, subj.Position, matchlen);
                BString.binsertch(ref contents, 0, 1, '<');
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
                        clean_url(ref url);
                        title = BString.bmidstr(subj.Buffer, starttitle, endtitle - starttitle);
                        clean_title(ref title);
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

        private static bool not_eof(Subject subj)
        {
            return !is_eof(subj);
        }

        // Parse inlines while a predicate is satisfied.  Return inlines.
        public static Inline parse_inlines_while(Subject subj, Predicate<Subject> predicate)
        {
            Inline last = null;
            while (predicate(subj) && parse_inline(subj, ref last))
            {
            }
            return last;
        }

        // Parse an inline, advancing subject, and add it to last element.
        // Adjust tail to point to new last element of list.
        // Return 0 if no inline can be parsed, 1 otherwise.
        public static bool parse_inline(Subject subj, ref Inline last)
        {
            Inline inew = null;
            string contents;
            string special_chars;
            char? c;
            int endpos;
            c = peek_char(subj);
            if (c == null)
                return false;

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
                    if (subj.Position > 0 && (char.IsLetterOrDigit(BString.bchar(subj.Buffer, subj.Position - 1).Value) ||
                                          BString.bchar(subj.Buffer, subj.Position - 1) == '_'))
                    {
                        inew = make_str(take_one(subj));
                    }
                    else
                    {
                        inew = handle_strong_emph(subj, '_');
                    }
                    break;
                case '*':
                    inew = handle_strong_emph(subj, '*');
                    break;
                case '[':
                    inew = handle_left_bracket(subj);
                    break;
                case '!':
                    advance(subj);
                    if (peek_char(subj) == '[')
                    {
                        inew = handle_left_bracket(subj);
                        if (inew != null && inew.tag == InlineTag.link)
                        {
                            inew.tag = InlineTag.image;
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
            if (last == null)
            {
                last = inew;
            }
            else
            {
                append_inlines(last, inew);
            }
            return true;
        }

        public static Inline parse_inlines(string input, Dictionary<string, Reference> refmap)
        {
            Subject subj = make_subject(input, refmap);
            return parse_inlines_while(subj, not_eof);
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
                clean_url(ref url);
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
                clean_title(ref title);
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
