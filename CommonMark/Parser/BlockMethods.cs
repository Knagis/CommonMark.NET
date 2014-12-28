using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal static class BlockMethods
    {
        private const int CODE_INDENT = 4;

        // Create a root document block.
        public static Block make_document()
        {
            Block e = new Block(BlockTag.Document, 1, 1);
            e.ReferenceMap = new Dictionary<string, Reference>();
            e.Top = e;
            return e;
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool can_contain(BlockTag parent_type, BlockTag child_type)
        {
            return (parent_type == BlockTag.Document ||
                     parent_type == BlockTag.BlockQuote ||
                     parent_type == BlockTag.ListItem ||
                     (parent_type == BlockTag.List && child_type == BlockTag.ListItem));
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool accepts_lines(BlockTag block_type)
        {
            return (block_type == BlockTag.Paragraph ||
                    block_type == BlockTag.AtxHeader ||
                    block_type == BlockTag.IndentedCode ||
                    block_type == BlockTag.FencedCode);
        }


        static void add_line(Block block, string ln, int offset, int length = -1)
        {
            if (!block.IsOpen)
                throw new CommonMarkException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Attempted to add line '{0}' to closed container ({1}).", ln, block.Tag));

            var len = length == -1 ? ln.Length - offset : length;
            if (len <= 0)
                return;

            var curSC = block.StringContent;
            if (curSC == null)
                block.StringContent = curSC = new StringContent();

                block.StringContent.Append(ln, offset, len);
        }

        // Check to see if a block ends with a blank line, descending
        // if needed into lists and sublists.
        static bool ends_with_blank_line(Block block)
        {
            if (block.IsLastLineBlank)
            {
                return true;
            }
            if ((block.Tag == BlockTag.List || block.Tag == BlockTag.ListItem) && block.LastChild != null)
            {
                return ends_with_blank_line(block.LastChild);
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
            Block b = container.Top;
            // find first containing list:
            while (b != null && b.Tag != BlockTag.List)
            {
                b = b.LastChild;
            }
            if (b != null)
            {
                while (container != null && container != b)
                {
                    finalize(container, line_number);
                    container = container.Parent;
                }
                finalize(b, line_number);
                bptr = b.Parent;
            }
        }


        public static void finalize(Block b, int line_number)
        {
            if (!b.IsOpen)
            {
                // don't do anything if the block is already closed
                return; 
            }

            b.IsOpen = false;
            if (line_number > b.StartLine)
                b.EndLine = line_number - 1;
            else
                b.EndLine = line_number;

            switch (b.Tag)
            {

                case BlockTag.Paragraph:
                    var pos = 0;
                    while (b.StringContent.StartsWith('[') && 0 != (pos = InlineMethods.ParseReference(b.StringContent, b.Top.ReferenceMap)))
                        b.StringContent.TrimStart(pos);

                    if (b.StringContent.IsFirstLineBlank())
                        b.Tag = BlockTag.ReferenceDefinition;

                    break;

                case BlockTag.IndentedCode:
                    b.StringContent.RemoveTrailingBlankLines();
                    break;

                case BlockTag.FencedCode:
                    // first line of contents becomes info
                    var firstlinelen = b.StringContent.IndexOf('\n') + 1;
                    b.FencedCodeData.Info = InlineMethods.Unescape(b.StringContent.TakeFromStart(firstlinelen, true).Trim());
                    break;

                case BlockTag.List: // determine tight/loose status
                    b.ListData.IsTight = true; // tight by default
                    var item = b.FirstChild;
                    Block subitem;

                    while (item != null)
                    {
                        // check for non-final non-empty list item ending with blank line:
                        if (item.IsLastLineBlank && item.NextSibling != null)
                        {
                            b.ListData.IsTight = false;
                            break;
                        }
                        // recurse into children of list item, to see if there are
                        // spaces between them:
                        subitem = item.FirstChild;
                        while (subitem != null)
                        {
                            if (ends_with_blank_line(subitem) &&
                                (item.NextSibling != null || subitem.NextSibling != null))
                            {
                                b.ListData.IsTight = false;
                                break;
                            }
                            subitem = subitem.NextSibling;
                        }
                        if (!(b.ListData.IsTight))
                        {
                            break;
                        }
                        item = item.NextSibling;
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Adds a new block as child of another. Return the child.
        /// </summary>
        /// <remarks>Original: add_child</remarks>
        public static Block CreateChildBlock(Block parent, BlockTag blockType, int startLine, int startColumn)
        {
            // if 'parent' isn't the kind of block that can accept this child,
            // then back up til we hit a block that can.
            while (!can_contain(parent.Tag, blockType))
            {
                finalize(parent, startLine);
                parent = parent.Parent;
            }

            Block child = new Block(blockType, startLine, startColumn);
            child.Parent = parent;
            child.Top = parent.Top;

            if (parent.LastChild != null)
            {
                parent.LastChild.NextSibling = child;
                child.Previous = parent.LastChild;
            }
            else
            {
                parent.FirstChild = child;
                child.Previous = null;
            }

            parent.LastChild = child;
            return child;
        }


        /// <summary>
        /// Walk through the block, its children and siblings, parsing string content into inline content where appropriate.
        /// </summary>
        /// <param name="block">The document level block from which to start the processing.</param>
        /// <param name="refmap">The reference mapping used when parsing links.</param>
        /// <param name="settings">The settings that influence how the inline parsing is performed.</param>
        public static void ProcessInlines(Block block, Dictionary<string, Reference> refmap, CommonMarkSettings settings)
        {
            var stack = new Stack<Block>();
            var parsers = settings.InlineParsers;
            var specialCharacters = settings.InlineParserSpecialCharacters;

            while (block != null)
            {
                var tag = block.Tag;
                if (tag == BlockTag.Paragraph || tag == BlockTag.AtxHeader || tag == BlockTag.SETextHeader)
                {
                    if (block.StringContent != null)
                    {
                        block.InlineContent = InlineMethods.parse_inlines(block.StringContent.ToString(), refmap, parsers, specialCharacters);
                        block.StringContent = null;
                    }
                }

                if (block.FirstChild != null)
                {
                    if (block.NextSibling != null)
                        stack.Push(block.NextSibling);

                    block = block.FirstChild;
                }
                else if (block.NextSibling != null)
                {
                    block = block.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    block = stack.Pop();
                }
                else
                {
                    block = null;
                }
            }
        }

        /// <summary>
        /// Attempts to parse a list item marker (bullet or enumerated).
        /// On success, returns length of the marker, and populates
        /// data with the details.  On failure, returns 0.
        /// </summary>
        /// <remarks>Original: int parse_list_marker(string ln, int pos, ref ListData dataptr)</remarks>
        private static int ParseListMarker(string ln, int pos, out ListData data)
        {
            char c;
            int startpos;
            data = null;
            var len = ln.Length;

            startpos = pos;
            c = ln[pos];

            if (c == '+' || c == '•' || ((c == '*' || c == '-') && 0 == Scanner.scan_hrule(ln, pos)))
            {
                pos++;
                if (pos == len || (ln[pos] != ' ' && ln[pos] != '\n'))
                    return 0;

                data = new ListData();
                data.BulletChar = c;
                data.Start = 1;
            }
            else if (c >= '0' && c <= '9')
            {

                int start = c - '0';

                while (pos < len - 1)
                {
                    c = ln[++pos];
                    if (c >= '0' && c <= '9')
                        start = start * 10 + (c - '0');
                    else
                        break;
                }

                if (pos >= len - 1 || (c != '.' && c != ')'))
                    return 0;

                pos++;
                if (pos == len || (ln[pos] != ' ' && ln[pos] != '\n'))
                    return 0;

                data = new ListData();
                data.ListType = ListType.Ordered;
                data.BulletChar = '\0';
                data.Start = start;
                data.Delimiter = (c == '.' ? ListDelimiter.Period : ListDelimiter.Parenthesis);

            }
            else
            {
                return 0;
            }

            return (pos - startpos);
        }

        private static bool ContainsSingleLine(StringContent content)
        {
            if (content == null)
                return true;
            var i = content.IndexOf('\n');
            return (i == -1 || i == content.Length - 1);
        }

        // Return 1 if list item belongs in list, else 0.
        private static bool lists_match(ListData list_data, ListData item_data)
        {
            return (list_data.ListType == item_data.ListType &&
                    list_data.Delimiter == item_data.Delimiter &&
                // list_data.marker_offset == item_data.marker_offset &&
                    list_data.BulletChar == item_data.BulletChar);
        }

        // Process one line at a time, modifying a block.
        // Returns 0 if successful.  curptr is changed to point to
        // the currently open block.
        public static void incorporate_line(string ln, int line_number, ref Block curptr)
        {
            Block last_matched_container;
            int offset = 0;
            int matched = 0;
            int i;
            ListData data;
            bool all_matched = true;
            Block container;
            Block cur = curptr;
            bool blank = false;
            int first_nonspace;
            char curChar;
            int indent;

            // container starts at the document root.
            container = cur.Top;

            // for each containing block, try to parse the associated line start.
            // bail out on failure:  container will point to the last matching block.

            while (container.LastChild != null && container.LastChild.IsOpen)
            {
                container = container.LastChild;

                first_nonspace = offset;
                while ((curChar = ln[first_nonspace]) == ' ')
                    first_nonspace++;

                indent = first_nonspace - offset;
                blank = curChar == '\n';

                switch (container.Tag)
                {
                    case BlockTag.BlockQuote:
                        {
                            if (indent <= 3 && curChar == '>')
                            {
                                offset = first_nonspace + 1;
                                if (ln[offset] == ' ')
                                    offset++;
                            }
                            else
                            {
                                all_matched = false;
                            }

                            break;
                        }

                    case BlockTag.ListItem:
                        {
                            if (indent >= container.ListData.MarkerOffset + container.ListData.Padding)
                                offset += container.ListData.MarkerOffset + container.ListData.Padding;
                            else if (blank)
                                offset = first_nonspace;
                            else
                                all_matched = false;

                            break;
                        }

                    case BlockTag.IndentedCode:
                        {
                            if (indent >= CODE_INDENT)
                                offset += CODE_INDENT;
                            else if (blank)
                                offset = first_nonspace;
                            else
                                all_matched = false;

                            break;
                        }

                    case BlockTag.AtxHeader:
                    case BlockTag.SETextHeader:
                        {
                            // a header can never contain more than one line
                            all_matched = false;
                            if (blank)
                                container.IsLastLineBlank = true;

                            break;
                        }

                    case BlockTag.FencedCode:
                        {
                            // skip optional spaces of fence offset
                            i = container.FencedCodeData.FenceOffset;
                            while (i > 0 && ln[offset] == ' ')
                            {
                                offset++;
                                i--;
                            }

                            break;
                        }

                    case BlockTag.HtmlBlock:
                        {
                            if (blank)
                            {
                                container.IsLastLineBlank = true;
                                all_matched = false;
                            }

                            break;
                        }

                    case BlockTag.Paragraph:
                        {
                            if (blank)
                            {
                                container.IsLastLineBlank = true;
                                all_matched = false;
                            }

                            break;
                        }
                }

                if (!all_matched)
                {
                    container = container.Parent;  // back up to last matching block
                    break;
                }
            }

            last_matched_container = container;

            // check to see if we've hit 2nd blank line, break out of list:
            if (blank && container.IsLastLineBlank)
            {
                break_out_of_lists(ref container, line_number);
            }

            // unless last matched container is code block, try new container starts:
            while (container.Tag != BlockTag.FencedCode && 
                   container.Tag != BlockTag.IndentedCode &&
                   container.Tag != BlockTag.HtmlBlock)
            {

                first_nonspace = offset;
                while ((curChar = ln[first_nonspace]) == ' ')
                    first_nonspace++;

                indent = first_nonspace - offset;
                blank = curChar == '\n';

                if (indent >= CODE_INDENT)
                {

                    if (cur.Tag != BlockTag.Paragraph && !blank)
                    {
                        offset += CODE_INDENT;
                        container = CreateChildBlock(container, BlockTag.IndentedCode, line_number, offset + 1);
                    }
                    else
                    {
                        // indent > 4 in lazy line
                        break;
                    }

                }
                else if (curChar == '>')
                {

                    offset = first_nonspace + 1;
                    // optional following character
                    if (ln[offset] == ' ')
                        offset++;

                    container = CreateChildBlock(container, BlockTag.BlockQuote, line_number, offset + 1);

                }
                else if (curChar == '#' && 0 != (matched = Scanner.scan_atx_header_start(ln, first_nonspace, out i)))
                {

                    offset = first_nonspace + matched;
                    container = CreateChildBlock(container, BlockTag.AtxHeader, line_number, offset + 1);
                    container.HeaderLevel = i;

                }
                else if ((curChar == '`' || curChar == '~') && 0 != (matched = Scanner.scan_open_code_fence(ln, first_nonspace)))
                {

                    container = CreateChildBlock(container, BlockTag.FencedCode, line_number, first_nonspace + 1);
                    container.FencedCodeData = new FencedCodeData();
                    container.FencedCodeData.FenceChar = curChar;
                    container.FencedCodeData.FenceLength = matched;
                    container.FencedCodeData.FenceOffset = first_nonspace - offset;
                    offset = first_nonspace + matched;

                }
                else if (curChar == '<' && Scanner.scan_html_block_tag(ln, first_nonspace))
                {

                    container = CreateChildBlock(container, BlockTag.HtmlBlock, line_number, first_nonspace + 1);
                    // note, we don't adjust offset because the tag is part of the text

                }
                else if (container.Tag == BlockTag.Paragraph 
                        && 0 != (matched = Scanner.scan_setext_header_line(ln, first_nonspace))
                        && ContainsSingleLine(container.StringContent))
                {

                    container.Tag = BlockTag.SETextHeader;
                    container.HeaderLevel = matched;
                    offset = ln.Length - 1;

                }
                else if (!(container.Tag == BlockTag.Paragraph && !all_matched) && 0 != (matched = Scanner.scan_hrule(ln, first_nonspace)))
                {

                    // it's only now that we know the line is not part of a setext header:
                    container = CreateChildBlock(container, BlockTag.HorizontalRuler, line_number, first_nonspace + 1);
                    finalize(container, line_number);
                    container = container.Parent;
                    offset = ln.Length - 1;

                }
                else if (0 != (matched = ParseListMarker(ln, first_nonspace, out data)))
                {

                    // compute padding:
                    offset = first_nonspace + matched;
                    i = 0;
                    while (i <= 5 && ln[offset + i] == ' ')
                    {
                        i++;
                    }
                    // i = number of spaces after marker, up to 5
                    if (i >= 5 || i < 1 || ln[offset] == '\n')
                    {
                        data.Padding = matched + 1;
                        if (i > 0)
                        {
                            offset += 1;
                        }
                    }
                    else
                    {
                        data.Padding = matched + i;
                        offset += i;
                    }

                    // check container; if it's a list, see if this list item
                    // can continue the list; otherwise, create a list container.

                    data.MarkerOffset = indent;

                    if (container.Tag != BlockTag.List || !lists_match(container.ListData, data))
                    {
                        container = CreateChildBlock(container, BlockTag.List, line_number, first_nonspace + 1);
                        container.ListData = data;
                    }

                    // add the list item
                    container = CreateChildBlock(container, BlockTag.ListItem, line_number, first_nonspace + 1);
                    container.ListData = data;
                }
                else
                {
                    break;
                }

                if (accepts_lines(container.Tag))
                {
                    // if it's a line container, it can't contain other containers
                    break;
                }
            }

            // what remains at offset is a text line.  add the text to the
            // appropriate container.

            first_nonspace = offset;
            if (offset >= ln.Length)
                curChar = '\0';
            else
                while ((curChar = ln[first_nonspace]) == ' ')
                    first_nonspace++;

            indent = first_nonspace - offset;
            blank = curChar == '\n';

            // block quote lines are never blank as they start with >
            // and we don't count blanks in fenced code for purposes of tight/loose
            // lists or breaking out of lists.  we also don't set last_line_blank
            // on an empty list item.
            container.IsLastLineBlank = (blank &&
                                          container.Tag != BlockTag.BlockQuote &&
                                          container.Tag != BlockTag.SETextHeader &&
                                          container.Tag != BlockTag.FencedCode &&
                                          !(container.Tag == BlockTag.ListItem &&
                                            container.FirstChild == null &&
                                            container.StartLine == line_number));

            Block cont = container;
            while (cont.Parent != null)
            {
                cont.Parent.IsLastLineBlank = false;
                cont = cont.Parent;
            }

            if (cur != last_matched_container &&
                container == last_matched_container &&
                !blank &&
                cur.Tag == BlockTag.Paragraph &&
                cur.StringContent.Length > 0)
            {

                add_line(cur, ln, offset);

            }
            else
            { // not a lazy continuation

                // finalize any blocks that were not matched and set cur to container:
                while (cur != last_matched_container)
                {

                    finalize(cur, line_number);
                    cur = cur.Parent;

                    if (cur == null)
                        throw new CommonMarkException("Cannot finalize container block. Last matched container tag = " + last_matched_container.Tag);

                }

                if (container.Tag == BlockTag.IndentedCode)
                {

                    add_line(container, ln, offset);

                }
                else if (container.Tag == BlockTag.FencedCode)
                {

                    matched = (indent <= 3
                      && curChar == container.FencedCodeData.FenceChar)
                      && (0 != Scanner.scan_close_code_fence(ln, first_nonspace, container.FencedCodeData.FenceLength))
                      ? 1 : 0;
                    if (matched != 0)
                    {
                        // if closing fence, don't add line to container; instead, close it:
                        finalize(container, line_number);
                        container = container.Parent; // back up to parent
                    }
                    else
                    {
                        add_line(container, ln, offset);
                    }

                }
                else if (container.Tag == BlockTag.HtmlBlock)
                {

                    add_line(container, ln, offset);

                }
                else if (blank)
                {

                    // ??? do nothing

                }
                else if (container.Tag == BlockTag.AtxHeader)
                {

                    int p = ln.Length - 1;

                    // trim trailing spaces
                    while (p >= 0 && (ln[p] == ' ' || ln[p] == '\n'))
                        p--;

                    // if string ends in #s, remove these:
                    while (p >= 0 && ln[p] == '#')
                        p--;

                    // there must be a space before the last hashtag
                    if (p < 0 || ln[p] != ' ')
                        p = ln.Length - 1;

                    add_line(container, ln, first_nonspace, p - first_nonspace + 1);
                    finalize(container, line_number);
                    container = container.Parent;

                }
                else if (accepts_lines(container.Tag))
                {

                    add_line(container, ln, first_nonspace);

                }
                else if (container.Tag != BlockTag.HorizontalRuler && container.Tag != BlockTag.SETextHeader)
                {

                    // create paragraph container for line
                    container = CreateChildBlock(container, BlockTag.Paragraph, line_number, first_nonspace + 1);
                    add_line(container, ln, first_nonspace);

                }
                else
                {

                    Utilities.Warning("Line {0} with container type {1} did not match any condition:\n\"{2}\"", line_number, container.Tag, ln);

                }

                curptr = container;
            }
        }
    }
}
