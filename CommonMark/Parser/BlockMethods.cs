using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal static class BlockMethods
    {
        private const int CODE_INDENT = 4;

        private static Block make_block(BlockTag tag, int start_line, int start_column)
        {
            Block e = new Block();
            e.tag = tag;
            e.open = true;
            e.last_line_blank = false;
            e.start_line = start_line;
            e.start_column = start_column;
            e.end_line = start_line;
            e.children = null;
            e.last_child = null;
            e.parent = null;
            e.top = null;
            e.attributes.refmap = null;
            e.string_content = string.Empty;
            e.inline_content = null;
            e.next = null;
            e.prev = null;
            return e;
        }

        // Create a root document block.
        public static Block make_document()
        {
            Block e = make_block(BlockTag.document, 1, 1);
            e.attributes.refmap = new Dictionary<string, Reference>();
            e.top = e;
            return e;
        }

        // Returns true if line has only space characters, else false.
        private static bool is_blank(string s, int offset)
        {
            char? c;
            while (null != (c = BString.bchar(s, offset)))
            {
                if (c == '\n')
                {
                    return true;
                }
                else if (c == ' ')
                {
                    offset++;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static bool can_contain(BlockTag parent_type, BlockTag child_type)
        {
            return (parent_type == BlockTag.document ||
                     parent_type == BlockTag.block_quote ||
                     parent_type == BlockTag.list_item ||
                     (parent_type == BlockTag.list && child_type == BlockTag.list_item));
        }

        private static bool accepts_lines(BlockTag block_type)
        {
            return (block_type == BlockTag.paragraph ||
                    block_type == BlockTag.atx_header ||
                    block_type == BlockTag.indented_code ||
                    block_type == BlockTag.fenced_code);
        }


        static void add_line(Block block, string ln, int offset)
        {
            string s;
            var len = ln.Length - offset;
            if (len < 0)
                s = string.Empty;
            else
                s = BString.bmidstr(ln, offset, len);

            if (!block.open)
                throw new CommonMarkException(string.Format("Attempted to add line '{0}' to closed container ({1}).", ln, block.tag));

            block.string_content += s;
        }

        private static void remove_trailing_blank_lines(ref string ln)
        {
            string tofind = " \t\r\n";
            int pos;
            // find last nonspace:
            pos = BString.bninchrr(ln, ln.Length - 1, tofind);
            if (pos == -1)
            { // all spaces
                ln = "";
            }
            else
            {
                // find next newline after it
                pos = BString.bstrchrp(ln, '\n', pos);
                if (pos != -1)
                {
                    ln = ln.Remove(pos, ln.Length - pos);
                }
            }
        }
        // Check to see if a block ends with a blank line, descending
        // if needed into lists and sublists.
        static bool ends_with_blank_line(Block block)
        {
            if (block.last_line_blank)
            {
                return true;
            }
            if ((block.tag == BlockTag.list || block.tag == BlockTag.list_item) && block.last_child != null)
            {
                return ends_with_blank_line(block.last_child);
            }
            else
            {
                return false;
            }
        }

        // Break out of all containing lists
        private static void break_out_of_lists(ref Block bptr, int line_number)
        {
            Block container = bptr;
            Block b = container.top;
            // find first containing list:
            while (b != null && b.tag != BlockTag.list)
            {
                b = b.last_child;
            }
            if (b != null)
            {
                while (container != null && container != b)
                {
                    finalize(container, line_number);
                    container = container.parent;
                }
                finalize(b, line_number);
                bptr = b.parent;
            }
        }


        public static void finalize(Block b, int line_number)
        {
            int firstlinelen;
            int pos;
            Block item;
            Block subitem;

            if (b == null)
                throw new ArgumentNullException("b");

            if (!b.open)
            {
                // don't do anything if the block is already closed
                return; 
            }

            b.open = false;
            if (line_number > b.start_line)
            {
                b.end_line = line_number - 1;
            }
            else
            {
                b.end_line = line_number;
            }

            switch (b.tag)
            {

                case BlockTag.paragraph:
                    pos = 0;
                    while (BString.bchar(b.string_content, 0) == '[' &&
                           0 != (pos = InlineMethods.parse_reference(b.string_content,
                                                  b.top.attributes.refmap)))
                    {
                        b.string_content = b.string_content.Remove(0, pos);
                    }
                    if (is_blank(b.string_content, 0))
                    {
                        b.tag = BlockTag.reference_def;
                    }
                    break;

                case BlockTag.indented_code:
                    remove_trailing_blank_lines(ref b.string_content);
                    b.string_content += "\n";
                    break;

                case BlockTag.fenced_code:
                    // first line of contents becomes info
                    firstlinelen = BString.bstrchr(b.string_content, '\n');
                    b.attributes.fenced_code_data.info = BString.bmidstr(b.string_content, 0, firstlinelen);
                    b.string_content = b.string_content.Remove(0, firstlinelen + 1); // +1 for \n
                    b.attributes.fenced_code_data.info = b.attributes.fenced_code_data.info.Trim();
                    InlineMethods.unescape(ref b.attributes.fenced_code_data.info);
                    break;

                case BlockTag.list: // determine tight/loose status
                    b.attributes.list_data.tight = true; // tight by default
                    item = b.children;

                    while (item != null)
                    {
                        // check for non-final non-empty list item ending with blank line:
                        if (item.last_line_blank && item.next != null)
                        {
                            b.attributes.list_data.tight = false;
                            break;
                        }
                        // recurse into children of list item, to see if there are
                        // spaces between them:
                        subitem = item.children;
                        while (subitem != null)
                        {
                            if (ends_with_blank_line(subitem) &&
                                (item.next != null || subitem.next != null))
                            {
                                b.attributes.list_data.tight = false;
                                break;
                            }
                            subitem = subitem.next;
                        }
                        if (!(b.attributes.list_data.tight))
                        {
                            break;
                        }
                        item = item.next;
                    }

                    break;

                default:
                    break;
            }
        }

        // Add a block as child of another.  Return pointer to child.
        public static Block add_child(Block parent, BlockTag block_type, int start_line, int start_column)
        {
            // if 'parent' isn't the kind of block that can accept this child,
            // then back up til we hit a block that can.
            while (!can_contain(parent.tag, block_type))
            {
                finalize(parent, start_line);
                parent = parent.parent;
            }

            if (parent == null)
                throw new ArgumentNullException("parent");

            Block child = make_block(block_type, start_line, start_column);
            child.parent = parent;
            child.top = parent.top;

            if (parent.last_child != null)
            {
                parent.last_child.next = child;
                child.prev = parent.last_child;
            }
            else
            {
                parent.children = child;
                child.prev = null;
            }
            parent.last_child = child;
            return child;
        }


        // Walk through block and all children, recursively, parsing
        // string content into inline content where appropriate.
        public static void process_inlines(Block cur, Dictionary<string, Reference> refmap)
        {
            switch (cur.tag)
            {

                case BlockTag.paragraph:
                case BlockTag.atx_header:
                case BlockTag.setext_header:
                    if (cur.string_content == null)
                        throw new CommonMarkException("The block does not contain string content.", cur);

                    cur.inline_content = InlineMethods.parse_inlines(cur.string_content, refmap);
                    cur.string_content = null;
                    break;

                default:
                    break;
            }

            Block child = cur.children;
            while (child != null)
            {
                process_inlines(child, refmap);
                child = child.next;
            }
        }

        // Attempts to parse a list item marker (bullet or enumerated).
        // On success, returns length of the marker, and populates
        // data with the details.  On failure, returns 0.
        static int parse_list_marker(string ln, int pos, ref ListData dataptr)
        {
            char? c;
            int startpos;
            int start = 1;
            ListData data;

            startpos = pos;
            c = BString.bchar(ln, pos);

            if ((c == '*' || c == '-' || c == '+') && 0 == Scanner.scan_hrule(ln, pos))
            {
                pos++;
                if (pos == ln.Length || !char.IsWhiteSpace(BString.bchar(ln, pos).Value))
                {
                    return 0;
                }
                data = new ListData();
                data.marker_offset = 0; // will be adjusted later
                data.ListType = ListType.Bullet;
                data.BulletChar = c.Value;
                data.start = 1;
                data.delimiter = ListDelimiter.Period;
                data.tight = false;
            }
            else if (c != null && char.IsDigit(c.Value))
            {

                pos++;
                while (char.IsDigit(BString.bchar(ln, pos).Value))
                {
                    pos++;
                }

                if (!int.TryParse(ln.Substring(startpos, pos - startpos), 
                    System.Globalization.NumberStyles.Integer, 
                    System.Globalization.CultureInfo.InvariantCulture, out start))
                {
                    // the only reasonable explanation why this case would occur is if the number is larger than int.MaxValue.
                    return 0;
                }

                c = BString.bchar(ln, pos);
                if (c == '.' || c == ')')
                {
                    pos++;
                    if (!char.IsWhiteSpace(BString.bchar(ln, pos).Value))
                        return 0;

                    data = new ListData();
                    data.marker_offset = 0; // will be adjusted later
                    data.ListType = ListType.Ordered;
                    data.BulletChar = '\0';
                    data.start = start;
                    data.delimiter = (c == '.' ? ListDelimiter.Period : ListDelimiter.Parenthesis);
                    data.tight = false;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                return 0;
            }

            dataptr = data;
            return (pos - startpos);
        }

        // Return 1 if list item belongs in list, else 0.
        private static bool lists_match(ListData list_data, ListData item_data)
        {
            return (list_data.ListType == item_data.ListType &&
                    list_data.delimiter == item_data.delimiter &&
                // list_data.marker_offset == item_data.marker_offset &&
                    list_data.BulletChar == item_data.BulletChar);
        }

        // Process one line at a time, modifying a block.
        // Returns 0 if successful.  curptr is changed to point to
        // the currently open block.
        public static int incorporate_line(string ln, int line_number, ref Block curptr)
        {
            // the original C code terminates each code with '\n'. TextReader.ReadLine() does not do so - we need to add it manually.
            ln += "\n";

            Block last_matched_container;
            int offset = 0;
            int matched = 0;
            int lev = 0;
            int i;
            ListData data = null;
            bool all_matched = true;
            Block container;
            Block cur = curptr;
            bool blank = false;
            int first_nonspace;
            int indent;

            // detab input line
            ln = Utilities.Untabify(ln);

            // container starts at the document root.
            container = cur.top;

            // for each containing block, try to parse the associated line start.
            // bail out on failure:  container will point to the last matching block.

            while (container.last_child != null && container.last_child.open)
            {
                container = container.last_child;

                first_nonspace = offset;
                while (BString.bchar(ln, first_nonspace) == ' ')
                    first_nonspace++;

                indent = first_nonspace - offset;
                blank = BString.bchar(ln, first_nonspace) == '\n';

                if (container.tag == BlockTag.block_quote)
                {

                    matched = (indent <= 3 && BString.bchar(ln, first_nonspace) == '>') ? 1 : 0;
                    if (matched != 0)
                    {
                        offset = first_nonspace + 1;
                        if (BString.bchar(ln, offset) == ' ')
                            offset++;
                    }
                    else
                    {
                        all_matched = false;
                    }

                }
                else if (container.tag == BlockTag.list_item)
                {

                    if (indent >= container.attributes.list_data.marker_offset +
                        container.attributes.list_data.padding)
                    {
                        offset += container.attributes.list_data.marker_offset +
                          container.attributes.list_data.padding;
                    }
                    else if (blank)
                    {
                        offset = first_nonspace;
                    }
                    else
                    {
                        all_matched = false;
                    }

                }
                else if (container.tag == BlockTag.indented_code)
                {

                    if (indent >= CODE_INDENT)
                    {
                        offset += CODE_INDENT;
                    }
                    else if (blank)
                    {
                        offset = first_nonspace;
                    }
                    else
                    {
                        all_matched = false;
                    }

                }
                else if (container.tag == BlockTag.atx_header ||
                         container.tag == BlockTag.setext_header)
                {

                    // a header can never contain more than one line
                    all_matched = false;

                }
                else if (container.tag == BlockTag.fenced_code)
                {

                    // skip optional spaces of fence offset
                    i = container.attributes.fenced_code_data.fence_offset;
                    while (i > 0 && BString.bchar(ln, offset) == ' ')
                    {
                        offset++;
                        i--;
                    }

                }
                else if (container.tag == BlockTag.html_block)
                {

                    if (blank)
                    {
                        all_matched = false;
                    }

                }
                else if (container.tag == BlockTag.paragraph)
                {

                    if (blank)
                    {
                        container.last_line_blank = true;
                        all_matched = false;
                    }

                }

                if (!all_matched)
                {
                    container = container.parent;  // back up to last matching block
                    break;
                }
            }

            last_matched_container = container;

            // check to see if we've hit 2nd blank line, break out of list:
            if (blank && container.last_line_blank)
            {
                break_out_of_lists(ref container, line_number);
            }

            // unless last matched container is code block, try new container starts:
            while (container.tag != BlockTag.fenced_code && container.tag != BlockTag.indented_code &&
                   container.tag != BlockTag.html_block)
            {

                first_nonspace = offset;
                while (BString.bchar(ln, first_nonspace) == ' ')
                    first_nonspace++;

                indent = first_nonspace - offset;
                blank = BString.bchar(ln, first_nonspace) == '\n';

                if (indent >= CODE_INDENT)
                {

                    if (cur.tag != BlockTag.paragraph && !blank)
                    {
                        offset += CODE_INDENT;
                        container = add_child(container, BlockTag.indented_code, line_number, offset + 1);
                    }
                    else
                    { // indent > 4 in lazy line
                        break;
                    }

                }
                else if (BString.bchar(ln, first_nonspace) == '>')
                {

                    offset = first_nonspace + 1;
                    // optional following character
                    if (BString.bchar(ln, offset) == ' ')
                    {
                        offset++;
                    }
                    container = add_child(container, BlockTag.block_quote, line_number, offset + 1);

                }
                else if (0 != (matched = Scanner.scan_atx_header_start(ln, first_nonspace)))
                {

                    offset = first_nonspace + matched;
                    container = add_child(container, BlockTag.atx_header, line_number, offset + 1);
                    int hashpos = BString.bstrchrp(ln, '#', first_nonspace);

                    if (hashpos == -1)
                        throw new CommonMarkException("ATX header parsing with regular expression returned incorrect results.", curptr);

                    int level = 0;
                    while (BString.bchar(ln, hashpos) == '#')
                    {
                        level++;
                        hashpos++;
                    }
                    container.attributes.header_level = level;

                }
                else if (0 != (matched = Scanner.scan_open_code_fence(ln, first_nonspace)))
                {

                    container = add_child(container, BlockTag.fenced_code, line_number, first_nonspace + 1);
                    container.attributes.fenced_code_data.fence_char = ln[first_nonspace];
                    container.attributes.fenced_code_data.fence_length = matched;
                    container.attributes.fenced_code_data.fence_offset = first_nonspace - offset;
                    offset = first_nonspace + matched;

                }
                else if (0 != (matched = Scanner.scan_html_block_tag(ln, first_nonspace)))
                {

                    container = add_child(container, BlockTag.html_block, line_number,
                                        first_nonspace + 1);
                    // note, we don't adjust offset because the tag is part of the text

                }
                else if (container.tag == BlockTag.paragraph &&
                        0 != (lev = Scanner.scan_setext_header_line(ln, first_nonspace)) &&
                    // check that there is only one line in the paragraph:
                         BString.bstrrchrp(container.string_content, '\n',
                                   container.string_content.Length - 2) == -1)
                {

                    container.tag = BlockTag.setext_header;
                    container.attributes.header_level = lev;
                    offset = ln.Length - 1;

                }
                else if (!(container.tag == BlockTag.paragraph && !all_matched) &&
                         0 != (matched = Scanner.scan_hrule(ln, first_nonspace)))
                {

                    // it's only now that we know the line is not part of a setext header:
                    container = add_child(container, BlockTag.hrule, line_number, first_nonspace + 1);
                    finalize(container, line_number);
                    container = container.parent;
                    offset = ln.Length - 1;

                }
                else if (0 != (matched = parse_list_marker(ln, first_nonspace, ref data)))
                {

                    // compute padding:
                    offset = first_nonspace + matched;
                    i = 0;
                    while (i <= 5 && BString.bchar(ln, offset + i) == ' ')
                    {
                        i++;
                    }
                    // i = number of spaces after marker, up to 5
                    if (i >= 5 || i < 1 || BString.bchar(ln, offset) == '\n')
                    {
                        data.padding = matched + 1;
                        if (i > 0)
                        {
                            offset += 1;
                        }
                    }
                    else
                    {
                        data.padding = matched + i;
                        offset += i;
                    }

                    // check container; if it's a list, see if this list item
                    // can continue the list; otherwise, create a list container.

                    data.marker_offset = indent;

                    if (container.tag != BlockTag.list ||
                        !lists_match(container.attributes.list_data, data))
                    {
                        container = add_child(container, BlockTag.list, line_number,
                      first_nonspace + 1);
                        container.attributes.list_data = data;
                    }

                    // add the list item
                    container = add_child(container, BlockTag.list_item, line_number,
                        first_nonspace + 1);
                    container.attributes.list_data = data;
                }
                else
                {
                    break;
                }

                if (accepts_lines(container.tag))
                {
                    // if it's a line container, it can't contain other containers
                    break;
                }
            }

            // what remains at offset is a text line.  add the text to the
            // appropriate container.

            first_nonspace = offset;
            while (BString.bchar(ln, first_nonspace) == ' ')
            {
                first_nonspace++;
            }

            indent = first_nonspace - offset;
            blank = BString.bchar(ln, first_nonspace) == '\n';

            // block quote lines are never blank as they start with >
            // and we don't count blanks in fenced code for purposes of tight/loose
            // lists or breaking out of lists.  we also don't set last_line_blank
            // on an empty list item.
            container.last_line_blank = (blank &&
                                          container.tag != BlockTag.block_quote &&
                                          container.tag != BlockTag.fenced_code &&
                                          !(container.tag == BlockTag.list_item &&
                                            container.children == null &&
                                            container.start_line == line_number));

            Block cont = container;
            while (cont.parent != null)
            {
                cont.parent.last_line_blank = false;
                cont = cont.parent;
            }

            if (cur != last_matched_container &&
                container == last_matched_container &&
                !blank &&
                cur.tag == BlockTag.paragraph &&
                cur.string_content.Length > 0)
            {

                add_line(cur, ln, offset);

            }
            else
            { // not a lazy continuation

                // finalize any blocks that were not matched and set cur to container:
                while (cur != last_matched_container)
                {

                    finalize(cur, line_number);
                    cur = cur.parent;

                    if (cur == null)
                        throw new CommonMarkException("Cannot finalize container block. Last matched container tag = " + last_matched_container.tag);

                }

                if (container.tag == BlockTag.indented_code)
                {

                    add_line(container, ln, offset);

                }
                else if (container.tag == BlockTag.fenced_code)
                {

                    matched = (indent <= 3
                      && BString.bchar(ln, first_nonspace) == container.attributes.fenced_code_data.fence_char)
                      && (0 != Scanner.scan_close_code_fence(ln, first_nonspace, container.attributes.fenced_code_data.fence_length))
                      ? 1 : 0;
                    if (matched != 0)
                    {
                        // if closing fence, don't add line to container; instead, close it:
                        finalize(container, line_number);
                        container = container.parent; // back up to parent
                    }
                    else
                    {
                        add_line(container, ln, offset);
                    }

                }
                else if (container.tag == BlockTag.html_block)
                {

                    add_line(container, ln, offset);

                }
                else if (blank)
                {

                    // ??? do nothing

                }
                else if (container.tag == BlockTag.atx_header)
                {

                    // chop off trailing ###s...use a scanner?
                    ln = ln.TrimEnd();
                    int p = ln.Length - 1;
                    int numhashes = 0;
                    // if string ends in #s, remove these:
                    while (p >= 0 && BString.bchar(ln, p) == '#')
                    {
                        p--;
                        numhashes++;
                    }
                    if (p >= 0 && BString.bchar(ln, p) == '\\')
                    {
                        // the last # was escaped, so we include it.
                        p++;
                        numhashes--;
                    }
                    ln = ln.Remove(p + 1, numhashes);
                    add_line(container, ln, first_nonspace);
                    finalize(container, line_number);
                    container = container.parent;

                }
                else if (accepts_lines(container.tag))
                {

                    add_line(container, ln, first_nonspace);

                }
                else if (container.tag != BlockTag.hrule && container.tag != BlockTag.setext_header)
                {

                    // create paragraph container for line
                    container = add_child(container, BlockTag.paragraph, line_number, first_nonspace + 1);
                    add_line(container, ln, first_nonspace);

                }
                else
                {

                    Utilities.Warning("Line {0} with container type {1} did not match any condition:\n\"{2}\"", line_number, container.tag, ln);

                }

                curptr = container;
            }

            return 0;
        }
    }
}
