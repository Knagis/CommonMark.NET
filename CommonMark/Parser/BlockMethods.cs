using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Parser
{
    internal static class BlockMethods
    {
        private const int CODE_INDENT = 4;
        private const int TabSize = 4;
        private const string Spaces = "         ";

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool CanContain(BlockTag parent_type, BlockTag child_type)
        {
            return (parent_type == BlockTag.Document ||
                     parent_type == BlockTag.BlockQuote ||
                     parent_type == BlockTag.ListItem ||
                     (parent_type == BlockTag.List && child_type == BlockTag.ListItem));
        }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static bool AcceptsLines(BlockTag block_type)
        {
            return (block_type == BlockTag.Paragraph ||
                    block_type == BlockTag.AtxHeading ||
                    block_type == BlockTag.IndentedCode ||
                    block_type == BlockTag.FencedCode);
        }

        private static void AddLine(Block block, LineInfo lineInfo, string ln, int offset, int remainingSpaces, int length = -1)
        {
            if (!block.IsOpen)
                throw new CommonMarkException(string.Format(CultureInfo.InvariantCulture, "Attempted to add line '{0}' to closed container ({1}).", ln, block.Tag));

            var len = length == -1 ? ln.Length - offset : length;
            if (len <= 0)
                return;

            var curSC = block.StringContent;
            if (curSC == null)
            {
                block.StringContent = curSC = new StringContent();
                if (lineInfo.IsTrackingPositions)
                    curSC.PositionTracker = new PositionTracker(lineInfo.LineOffset);
            }

            if (lineInfo.IsTrackingPositions)
                curSC.PositionTracker.AddOffset(lineInfo, offset, len);

            curSC.Append(Spaces, 0, remainingSpaces);
            curSC.Append(ln, offset, len);
        }

        /// <summary>
        /// Check to see if a block ends with a blank line, descending if needed into lists and sublists.
        /// </summary>
        private static bool EndsWithBlankLine(Block block)
        {
            while (true)
            {
                if (block.IsLastLineBlank)
                    return true;

                if (block.Tag != BlockTag.List && block.Tag != BlockTag.ListItem)
                    return false;

                block = block.LastChild;

                if (block == null)
                    return false;
            }
        }

        /// <summary>
        /// Break out of all containing lists
        /// </summary>
        private static void BreakOutOfLists(ref Block blockRef, LineInfo line, CommonMarkSettings settings)
        {
            Block container = blockRef;
            Block b = container.Top;

            // find first containing list:
            while (b != null && b.Tag != BlockTag.List)
                b = b.LastChild;

            if (b != null)
            {
                while (container != null && container != b)
                {
                    Finalize(container, line, settings);
                    container = container.Parent;
                }

                Finalize(b, line, settings);
                blockRef = b.Parent;
            }
        }

        static List<string> ParseTableLine(string line, StringBuilder sb)
        {
            var ret = new List<string>();

            var i = 0;

            if (i < line.Length && line[i] == '|') i++;

            while (i < line.Length && char.IsWhiteSpace(line[i])) i++;

            for (; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '\\')
                {
                    i++;
                    continue;
                }

                if (c == '|')
                {
                    ret.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length != 0)
            {
                ret.Add(sb.ToString());
                sb.Clear();
            }

            return ret;
        }

        static void MakeTableCells(Block row, StringBuilder sb)
        {
            var asStr = row.StringContent.ToString();

            var offset = 0;

            for (var i = 0; i < asStr.Length; i++)
            {
                var c = asStr[i];

                if (c == '|')
                {
                    var text = sb.ToString();
                    sb.Clear();

                    if (text.Length > 0)
                    {
                        var leadingWhiteSpace = 0;
                        while (leadingWhiteSpace < text.Length && char.IsWhiteSpace(text[leadingWhiteSpace])) leadingWhiteSpace++;
                        var trailingWhiteSpace = 0;
                        while (trailingWhiteSpace < text.Length && char.IsWhiteSpace(text[text.Length - trailingWhiteSpace - 1])) trailingWhiteSpace++;

                        var cell = new Block(BlockTag.TableCell, row.SourcePosition + offset + leadingWhiteSpace);
                        cell.SourceLastPosition = cell.SourcePosition + text.Length - trailingWhiteSpace - leadingWhiteSpace;
                        cell.StringContent = new StringContent();
                        cell.StringContent.Append(text, leadingWhiteSpace, text.Length - leadingWhiteSpace - trailingWhiteSpace);

                        if (row.LastChild == null)
                        {
                            row.FirstChild = row.LastChild = cell;
                        }
                        else
                        {
                            row.LastChild.NextSibling = cell;
                            row.LastChild = cell;
                        }

                        cell.IsOpen = false;
                    }

                    offset += text.Length;

                    // skip the |
                    offset++;
                    continue;
                }

                if (c == '\\')
                {
                    sb.Append(c);
                    if (i + 1 < asStr.Length)
                    {
                        sb.Append(asStr[i + 1]);
                    }
                    i++;
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
            {
                var text = sb.ToString();
                sb.Clear();

                if (text.Length > 0)
                {
                    var leadingWhiteSpace = 0;
                    while (leadingWhiteSpace < text.Length && char.IsWhiteSpace(text[leadingWhiteSpace])) leadingWhiteSpace++;
                    var trailingWhiteSpace = 0;
                    while (trailingWhiteSpace < text.Length && char.IsWhiteSpace(text[text.Length - trailingWhiteSpace - 1])) trailingWhiteSpace++;

                    if (text.Length - leadingWhiteSpace - trailingWhiteSpace > 0)
                    {
                        var cell = new Block(BlockTag.TableCell, row.SourcePosition + offset + leadingWhiteSpace);
                        cell.SourceLastPosition = cell.SourcePosition + text.Length - trailingWhiteSpace - leadingWhiteSpace;
                        cell.StringContent = new StringContent();
                        cell.StringContent.Append(text, leadingWhiteSpace, text.Length - leadingWhiteSpace - trailingWhiteSpace);

                        if (row.LastChild == null)
                        {
                            row.FirstChild = row.LastChild = cell;
                        }
                        else
                        {
                            row.LastChild.NextSibling = cell;
                            row.LastChild = cell;
                        }

                        cell.IsOpen = false;
                    }
                }
            }
        }

        static void MakeTableRows(Block table, StringBuilder sb)
        {
            var asStr = table.StringContent.ToString();
            var lines = asStr.Split('\n');

            var offset = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                var lineLength = line.Length;
                var hasLineBreak = offset + lineLength < asStr.Length && asStr[offset + lineLength] == '\n';
                if (hasLineBreak) lineLength++;

                // skip the header row
                if (i != 1 && !string.IsNullOrWhiteSpace(line))
                {
                    var rowStartsInDocument = table.SourcePosition + offset;
                    var row = new Block(BlockTag.TableRow, rowStartsInDocument);
                    row.SourceLastPosition = rowStartsInDocument + lineLength;

                    row.StringContent = new StringContent();
                    row.StringContent.Append(asStr, offset, row.SourceLength);

                    if (table.LastChild == null)
                    {
                        table.FirstChild = row;
                        table.LastChild = row;
                    }
                    else
                    {
                        table.LastChild.NextSibling = row;
                        table.LastChild = row;
                    }

                    MakeTableCells(row, sb);
                    row.IsOpen = false;
                }

                offset += lineLength;
            }
        }

        static bool TryMakeTable(Block b, LineInfo line, CommonMarkSettings settings)
        {
            if ((settings.AdditionalFeatures & CommonMarkAdditionalFeatures.GithubStyleTables) == 0) return false;

            var asStr = b.StringContent.ToString();
            var lines = asStr.Split('\n');

            if (lines.Length < 2) return false;

            var sb = new StringBuilder();

            var columnsLine = ParseTableLine(lines[0], sb);
            if (columnsLine.Count == 1) return false;

            var headerLine = ParseTableLine(lines[1], sb);
            if (headerLine.Count == 1) return false;

            var headerAlignment = new List<TableHeaderAlignment>();

            foreach (var headerPart in headerLine)
            {
                var trimmed = headerPart.Trim();
                if (trimmed.Length < 3) return false;

                var validateFrom = 0;
                var startsWithColon = trimmed[validateFrom] == ':';
                if (startsWithColon) validateFrom++;

                var validateTo = trimmed.Length - 1;
                var endsWithColon = trimmed[validateTo] == ':';
                if (endsWithColon) validateTo--;

                for (var i = validateFrom; i <= validateTo; i++)
                {
                    // don't check for escapes, they don't count in header
                    if (trimmed[i] != '-') return false;
                }

                if (!startsWithColon && !endsWithColon)
                {
                    headerAlignment.Add(TableHeaderAlignment.None);
                    continue;
                }

                if (startsWithColon && endsWithColon)
                {
                    headerAlignment.Add(TableHeaderAlignment.Center);
                    continue;
                }

                if (startsWithColon)
                {
                    headerAlignment.Add(TableHeaderAlignment.Left);
                }

                if (endsWithColon)
                {
                    headerAlignment.Add(TableHeaderAlignment.Right);
                }
            }

            while (columnsLine.Count > 0 && string.IsNullOrWhiteSpace(columnsLine[0])) columnsLine.RemoveAt(0);
            while (columnsLine.Count > 0 && string.IsNullOrWhiteSpace(columnsLine[columnsLine.Count - 1])) columnsLine.RemoveAt(columnsLine.Count - 1);
            while (headerLine.Count > 0 && string.IsNullOrWhiteSpace(headerLine[0])) headerLine.RemoveAt(0);
            while (headerLine.Count > 0 && string.IsNullOrWhiteSpace(headerLine[headerLine.Count - 1])) headerLine.RemoveAt(headerLine.Count - 1);

            if (columnsLine.Count < 2) return false;
            if (headerLine.Count < columnsLine.Count) return false;

            var lastTableLine = 1;

            // it's a table!
            for (var i = 2; i < lines.Length; i++)
            {
                var hasPipe = false;
                for (var j = 0; j < lines[i].Length; j++)
                {
                    var c = lines[i][j];
                    if (c == '\\')
                    {
                        j++;
                        continue;
                    }

                    if (c == '|')
                    {
                        hasPipe = true;
                        break;
                    }
                }
                if (!hasPipe) break;

                lastTableLine = i;
            }

            if (lastTableLine + 1 < lines.Length && string.IsNullOrWhiteSpace(lines[lastTableLine + 1]))
            {
                lastTableLine++;
            }

            var wholeBlockIsTable = lastTableLine == (lines.Length - 1);

            // No need to break, the whole block is a table now
            if (wholeBlockIsTable)
            {
                b.Tag = BlockTag.Table;
                b.TableHeaderAlignments = headerAlignment;

                // create table rows
                MakeTableRows(b, sb);
                return true;
            }

            var takingCharsForTable = 0;
            for (var i = 0; i <= lastTableLine; i++)
            {
                takingCharsForTable += lines[i].Length;
                var hasFollowingLineBreak = takingCharsForTable < asStr.Length && asStr[takingCharsForTable] == '\n';
                if (hasFollowingLineBreak)
                {
                    takingCharsForTable++;
                }
            }

            // get the text of the table separate
            var tableBlockString = b.StringContent.TakeFromStart(takingCharsForTable, trim: true);
            var newBlock = new Block(BlockTag.Paragraph, b.SourcePosition + tableBlockString.Length);

            // create the trailing paragraph, and set it's text and source positions
            var newParagraph = b.Clone();
            newParagraph.StringContent = b.StringContent;
            if (settings.TrackSourcePosition)
            {
                newParagraph.SourcePosition = b.SourcePosition + tableBlockString.Length;
                newParagraph.SourceLastPosition = newParagraph.SourcePosition + (asStr.Length - tableBlockString.Length);
            }

            // update the text of the table block
            b.Tag = BlockTag.Table;
            b.TableHeaderAlignments = headerAlignment;
            b.StringContent = new StringContent();
            b.StringContent.Append(tableBlockString, 0, tableBlockString.Length);
            if (settings.TrackSourcePosition)
            {
                b.SourceLastPosition = b.SourcePosition + tableBlockString.Length;
            }

            // create table rows
            MakeTableRows(b, sb);

            // put the new paragraph after the table
            newParagraph.NextSibling = b.NextSibling;
            b.NextSibling = newParagraph;

            Finalize(newParagraph, line, settings);

            return true;
        }

        public static void Finalize(Block b, LineInfo line, CommonMarkSettings settings)
        {
            // don't do anything if the block is already closed
            if (!b.IsOpen)
                return;

            b.IsOpen = false;

            if (line.IsTrackingPositions)
            {
                // HTML Blocks other than type 7 call Finalize when the last line is encountered.
                // Block types 6 and 7 calls Finalize once it finds the next empty row but that empty row is no longer considered to be part of the block.
                var includesThisLine = b.HtmlBlockType != HtmlBlockType.None && b.HtmlBlockType != HtmlBlockType.InterruptingBlock && b.HtmlBlockType != HtmlBlockType.NonInterruptingBlock;

                // (b.SourcePosition >= line.LineOffset) determines if the block started on this line.
                includesThisLine = includesThisLine || b.SourcePosition >= line.LineOffset;

                if (includesThisLine && line.Line != null)
                    b.SourceLastPosition = line.CalculateOrigin(line.Line.Length, false);
                else
                    b.SourceLastPosition = line.CalculateOrigin(0, false);
            }

            switch (b.Tag)
            {
                case BlockTag.Paragraph:
                    var sc = b.StringContent;

                    if (TryMakeTable(b, line, settings))
                    {
                        break;
                    }

                    if (!sc.StartsWith('['))
                        break;

                    var subj = new Subject(b.Top.Document);
                    sc.FillSubject(subj);
                    var origPos = subj.Position;
                    while (subj.Position < subj.Buffer.Length
                        && subj.Buffer[subj.Position] == '['
                        && 0 != InlineMethods.ParseReference(subj))
                    {
                    }

                    if (subj.Position != origPos)
                    {
                        sc.Replace(subj.Buffer, subj.Position, subj.Buffer.Length - subj.Position);

                        if (sc.PositionTracker != null)
                            sc.PositionTracker.AddBlockOffset(subj.Position - origPos);

                        if (Utilities.IsFirstLineBlank(subj.Buffer, subj.Position))
                            b.Tag = BlockTag.ReferenceDefinition;
                    }

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

                        // recurse into children of list item, to see if there are spaces between them:
                        subitem = item.FirstChild;
                        while (subitem != null)
                        {
                            if (EndsWithBlankLine(subitem) && (item.NextSibling != null || subitem.NextSibling != null))
                            {
                                b.ListData.IsTight = false;
                                break;
                            }

                            subitem = subitem.NextSibling;
                        }

                        if (!b.ListData.IsTight)
                            break;

                        item = item.NextSibling;
                    }

                    break;
            }
        }

        /// <summary>
        /// Adds a new block as child of another. Return the child.
        /// </summary>
        /// <remarks>Original: add_child</remarks>
        public static Block CreateChildBlock(Block parent, LineInfo line, CommonMarkSettings settings, BlockTag blockType, int startColumn)
        {
            // if 'parent' isn't the kind of block that can accept this child,
            // then back up til we hit a block that can.
            while (!CanContain(parent.Tag, blockType))
            {
                Finalize(parent, line, settings);
                parent = parent.Parent;
            }

            var startPosition = line.IsTrackingPositions ? line.CalculateOrigin(startColumn, true) : line.LineOffset;
#pragma warning disable 0618
            Block child = new Block(blockType, line.LineNumber, startColumn + 1, startPosition);
#pragma warning restore 0618
            child.Parent = parent;
            child.Top = parent.Top;

            var lastChild = parent.LastChild;
            if (lastChild != null)
            {
                lastChild.NextSibling = child;
            }
            else
            {
                parent.FirstChild = child;
            }

            parent.LastChild = child;
            return child;
        }

        private static void AdjustInlineSourcePosition(Inline inline, PositionTracker tracker, ref Stack<Inline> stack)
        {
            if (stack == null)
                stack = new Stack<Inline>();

            while (inline != null)
            {
                inline.SourcePosition = tracker.CalculateInlineOrigin(inline.SourcePosition, true);
                inline.SourceLastPosition = tracker.CalculateInlineOrigin(inline.SourceLastPosition, false);

                if (inline.FirstChild != null)
                {
                    if (inline.NextSibling != null)
                        stack.Push(inline.NextSibling);

                    inline = inline.FirstChild;
                }
                else if (inline.NextSibling != null)
                {
                    inline = inline.NextSibling;
                }
                else if (stack.Count > 0)
                {
                    inline = stack.Pop();
                }
                else
                {
                    inline = null;
                }
            }

        }

        /// <summary>
        /// Walk through the block, its children and siblings, parsing string content into inline content where appropriate.
        /// </summary>
        /// <param name="block">The document level block from which to start the processing.</param>
        /// <param name="data">Document data.</param>
        /// <param name="settings">The settings that influence how the inline parsing is performed.</param>
        public static void ProcessInlines(Block block, DocumentData data, CommonMarkSettings settings)
        {
            Stack<Inline> inlineStack = null;
            var stack = new Stack<Block>();
            var parsers = settings.InlineParsers;
            var specialCharacters = settings.InlineParserSpecialCharacters;
            var subj = new Subject(data);

            StringContent sc;
            int delta;

            while (block != null)
            {
                var tag = block.Tag;
                if (tag == BlockTag.Paragraph || tag == BlockTag.AtxHeading || tag == BlockTag.SetextHeading || tag == BlockTag.TableCell)
                {
                    sc = block.StringContent;
                    if (sc != null)
                    {
                        sc.FillSubject(subj);
                        delta = subj.Position;

                        block.InlineContent = InlineMethods.parse_inlines(subj, parsers, specialCharacters);
                        block.StringContent = null;

                        if (sc.PositionTracker != null)
                        {
                            sc.PositionTracker.AddBlockOffset(-delta);
                            AdjustInlineSourcePosition(block.InlineContent, sc.PositionTracker, ref inlineStack);
                        }
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

            if (c == '+' || c == '•' || ((c == '*' || c == '-') && 0 == Scanner.scan_thematic_break(ln, pos, len)))
            {
                pos++;
                if (pos == len || !Utilities.IsWhitespace(ln[pos]))
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
                    // We limit to 9 digits to avoid overflow, This also seems to be the limit for 'start' in some browsers. 
                    if (c >= '0' && c <= '9' && start < 100000000)
                        start = start * 10 + (c - '0');
                    else
                        break;
                }

                if (pos >= len - 1 || (c != '.' && c != ')'))
                    return 0;

                pos++;
                if (pos == len || !Utilities.IsWhitespace(ln[pos]))
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

        private static bool ListsMatch(ListData listData, ListData itemData)
        {
            return (listData.ListType == itemData.ListType &&
                    listData.Delimiter == itemData.Delimiter &&
                    // list_data.marker_offset == item_data.marker_offset &&
                    listData.BulletChar == itemData.BulletChar);
        }

        private static bool AdvanceOptionalSpace(string line, ref int offset, ref int column, ref int remainingSpaces)
        {
            if (remainingSpaces > 0)
            {
                remainingSpaces--;
                return true;
            }

            var c = line[offset];
            if (c == ' ')
            {
                offset++;
                column++;
                return true;
            }
            else if (c == '\t')
            {
                offset++;
                var chars_to_tab = 4 - (column % TabSize);
                column += chars_to_tab;
                remainingSpaces = chars_to_tab - 1;
                return true;
            }

            return false;
        }

        private static void AdvanceOffset(string line, int count, bool columns, ref int offset, ref int column, ref int remainingSpaces)
        {
            if (columns)
            {
                if (remainingSpaces > count)
                {
                    remainingSpaces -= count;
                    count = 0;
                }
                else
                {
                    count -= remainingSpaces;
                    remainingSpaces = 0;
                }
            }
            else
            {
                remainingSpaces = 0;
            }

            char c;
            while (count > 0 && (c = line[offset]) != '\n')
            {
                if (c == '\t')
                {
                    var chars_to_tab = 4 - (column % TabSize);
                    column += chars_to_tab;
                    offset += 1;
                    count -= columns ? chars_to_tab : 1;

                    if (count < 0)
                    {
                        remainingSpaces = 0 - count;
                    }
                }
                else
                {
                    offset += 1;
                    column += 1; // assume ascii; block starts are ascii  
                    count -= 1;
                }
            }
        }

        // Process one line at a time, modifying a block.
        // Returns 0 if successful.  curptr is changed to point to
        // the currently open block.
        public static void IncorporateLine(LineInfo line, ref Block curptr, CommonMarkSettings settings)
        {
            var ln = line.Line;

            Block last_matched_container;

            // offset is the char position in the line
            var offset = 0;

            // column is the virtual position in the line that takes TAB expansion into account
            var column = 0;

            // the adjustment to the virtual position `column` that points to the number of spaces from the TAB that have not been included in any indent.
            var remainingSpaces = 0;

            // the char position of the first non-space char
            int first_nonspace;

            // the virtual position of the first non-space chart, that includes TAB expansion
            int first_nonspace_column;

            int matched;
            int i;
            ListData data;
            bool all_matched = true;
            Block cur = curptr;
            var blank = false;
            char curChar;
            int indent;

            // container starts at the document root.
            var container = cur.Top;

            // for each containing block, try to parse the associated line start.
            // bail out on failure:  container will point to the last matching block.

            while (container.LastChild != null && container.LastChild.IsOpen)
            {
                container = container.LastChild;

                FindFirstNonspace(ln, offset, column, out first_nonspace, out first_nonspace_column, out curChar);

                indent = first_nonspace_column - column + remainingSpaces;
                blank = curChar == '\n';

                switch (container.Tag)
                {
                    case BlockTag.BlockQuote:
                        {
                            if (indent <= 3 && curChar == '>')
                            {
                                AdvanceOffset(ln, indent + 1, true, ref offset, ref column, ref remainingSpaces);
                                AdvanceOptionalSpace(ln, ref offset, ref column, ref remainingSpaces);
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
                            {
                                AdvanceOffset(ln, container.ListData.MarkerOffset + container.ListData.Padding, true, ref offset, ref column, ref remainingSpaces);
                            }
                            else if (blank && container.FirstChild != null)
                            {
                                // if container->first_child is NULL, then the opening line
                                // of the list item was blank after the list marker; in this
                                // case, we are done with the list item.
                                AdvanceOffset(ln, first_nonspace - offset, false, ref offset, ref column, ref remainingSpaces);
                            }
                            else
                            {
                                all_matched = false;
                            }

                            break;
                        }

                    case BlockTag.IndentedCode:
                        {
                            if (indent >= CODE_INDENT)
                                AdvanceOffset(ln, CODE_INDENT, true, ref offset, ref column, ref remainingSpaces);
                            else if (blank)
                                AdvanceOffset(ln, first_nonspace - offset, false, ref offset, ref column, ref remainingSpaces);
                            else
                                all_matched = false;

                            break;
                        }

                    case BlockTag.AtxHeading:
                    case BlockTag.SetextHeading:
                        {
                            // a heading can never contain more than one line
                            all_matched = false;
                            if (blank)
                                container.IsLastLineBlank = true;

                            break;
                        }

                    case BlockTag.FencedCode:
                        {
                            // -1 means we've seen closer 
                            if (container.FencedCodeData.FenceLength == -1)
                            {
                                all_matched = false;
                                if (blank)
                                    container.IsLastLineBlank = true;
                            }
                            else
                            {
                                // skip optional spaces of fence offset
                                i = container.FencedCodeData.FenceOffset;
                                while (i > 0 && ln[offset] == ' ')
                                {
                                    offset++;
                                    column++;
                                    i--;
                                }
                            }

                            break;
                        }

                    case BlockTag.HtmlBlock:
                        {
                            // all other block types can accept blanks
                            if (blank && container.HtmlBlockType >= HtmlBlockType.InterruptingBlock)
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
                BreakOutOfLists(ref container, line, settings);

            var maybeLazy = cur.Tag == BlockTag.Paragraph;

            // unless last matched container is code block, try new container starts:
            while (container.Tag != BlockTag.FencedCode &&
                   container.Tag != BlockTag.IndentedCode &&
                   container.Tag != BlockTag.HtmlBlock)
            {

                FindFirstNonspace(ln, offset, column, out first_nonspace, out first_nonspace_column, out curChar);

                indent = first_nonspace_column - column;
                blank = curChar == '\n';

                var indented = indent >= CODE_INDENT;

                if (!indented && curChar == '>')
                {

                    AdvanceOffset(ln, first_nonspace + 1 - offset, false, ref offset, ref column, ref remainingSpaces);
                    AdvanceOptionalSpace(ln, ref offset, ref column, ref remainingSpaces);

                    container = CreateChildBlock(container, line, settings, BlockTag.BlockQuote, first_nonspace);

                }
                else if (!indented && curChar == '#' && 0 != (matched = Scanner.scan_atx_heading_start(ln, first_nonspace, ln.Length, out i)))
                {

                    AdvanceOffset(ln, first_nonspace + matched - offset, false, ref offset, ref column, ref remainingSpaces);
                    container = CreateChildBlock(container, line, settings, BlockTag.AtxHeading, first_nonspace);
                    container.Heading = new HeadingData(i);

                }
                else if (!indented && (curChar == '`' || curChar == '~') && 0 != (matched = Scanner.scan_open_code_fence(ln, first_nonspace, ln.Length)))
                {

                    container = CreateChildBlock(container, line, settings, BlockTag.FencedCode, first_nonspace);
                    container.FencedCodeData = new FencedCodeData();
                    container.FencedCodeData.FenceChar = curChar;
                    container.FencedCodeData.FenceLength = matched;
                    container.FencedCodeData.FenceOffset = first_nonspace - offset;

                    AdvanceOffset(ln, first_nonspace + matched - offset, false, ref offset, ref column, ref remainingSpaces);

                }
                else if (!indented && curChar == '<' &&
                    (0 != (matched = (int)Scanner.scan_html_block_start(ln, first_nonspace, ln.Length))
                    || (container.Tag != BlockTag.Paragraph && 0 != (matched = (int)Scanner.scan_html_block_start_7(ln, first_nonspace, ln.Length)))
                    ))
                {

                    container = CreateChildBlock(container, line, settings, BlockTag.HtmlBlock, first_nonspace);
                    container.HtmlBlockType = (HtmlBlockType)matched;
                    // note, we don't adjust offset because the tag is part of the text

                }
                else if (!indented && container.Tag == BlockTag.Paragraph && (curChar == '=' || curChar == '-')
                        && 0 != (matched = Scanner.scan_setext_heading_line(ln, first_nonspace, ln.Length)))
                {

                    container.Tag = BlockTag.SetextHeading;
                    container.Heading = new HeadingData(matched);
                    AdvanceOffset(ln, ln.Length - 1 - offset, false, ref offset, ref column, ref remainingSpaces);

                }
                else if (!indented
                    && !(container.Tag == BlockTag.Paragraph && !all_matched)
                    && 0 != (Scanner.scan_thematic_break(ln, first_nonspace, ln.Length)))
                {

                    // it's only now that we know the line is not part of a setext heading:
                    container = CreateChildBlock(container, line, settings, BlockTag.ThematicBreak, first_nonspace);
                    Finalize(container, line, settings);
                    container = container.Parent;
                    AdvanceOffset(ln, ln.Length - 1 - offset, false, ref offset, ref column, ref remainingSpaces);

                }
                else if ((!indented || container.Tag == BlockTag.List)
                    && 0 != (matched = ParseListMarker(ln, first_nonspace, out data)))
                {

                    // compute padding:
                    AdvanceOffset(ln, first_nonspace + matched - offset, false, ref offset, ref column, ref remainingSpaces);

                    var prevOffset = offset;
                    var prevColumn = column;
                    var prevRemainingSpaces = remainingSpaces;

                    while (column - prevColumn <= CODE_INDENT)
                    {
                        if (!AdvanceOptionalSpace(ln, ref offset, ref column, ref remainingSpaces))
                            break;
                    }

                    // i = number of spaces after marker, up to 5
                    if (column == prevColumn)
                    {
                        // no spaces at all
                        data.Padding = matched + 1;
                    }
                    else if (column - prevColumn > CODE_INDENT || ln[offset] == '\n')
                    {
                        data.Padding = matched + 1;

                        // too many (or none) spaces, ignoring everything but the first one
                        offset = prevOffset;
                        column = prevColumn;
                        remainingSpaces = prevRemainingSpaces;
                        AdvanceOptionalSpace(ln, ref offset, ref column, ref remainingSpaces);
                    }
                    else
                    {
                        data.Padding = matched + column - prevColumn;
                    }

                    // check container; if it's a list, see if this list item
                    // can continue the list; otherwise, create a list container.

                    data.MarkerOffset = indent;

                    if (container.Tag != BlockTag.List || !ListsMatch(container.ListData, data))
                    {
                        container = CreateChildBlock(container, line, settings, BlockTag.List, first_nonspace);
                        container.ListData = data;
                    }

                    // add the list item
                    container = CreateChildBlock(container, line, settings, BlockTag.ListItem, first_nonspace);
                    container.ListData = data;
                }
                else if (indented && !maybeLazy && !blank)
                {
                    AdvanceOffset(ln, CODE_INDENT, true, ref offset, ref column, ref remainingSpaces);
                    container = CreateChildBlock(container, line, settings, BlockTag.IndentedCode, offset);
                }
                else
                {
                    break;
                }

                if (AcceptsLines(container.Tag))
                {
                    // if it's a line container, it can't contain other containers
                    break;
                }

                maybeLazy = false;
            }

            // what remains at offset is a text line.  add the text to the
            // appropriate container.

            FindFirstNonspace(ln, offset, column, out first_nonspace, out first_nonspace_column, out curChar);
            indent = first_nonspace_column - column;
            blank = curChar == '\n';

            if (blank && container.LastChild != null)
            {
                container.LastChild.IsLastLineBlank = true;
            }

            // block quote lines are never blank as they start with >
            // and we don't count blanks in fenced code for purposes of tight/loose
            // lists or breaking out of lists.  we also don't set last_line_blank
            // on an empty list item.
            container.IsLastLineBlank = (blank &&
                                          container.Tag != BlockTag.BlockQuote &&
                                          container.Tag != BlockTag.SetextHeading &&
                                          container.Tag != BlockTag.FencedCode &&
                                          !(container.Tag == BlockTag.ListItem &&
                                            container.FirstChild == null &&
                                            container.SourcePosition >= line.LineOffset));

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

                AddLine(cur, line, ln, offset, remainingSpaces);

            }
            else
            { // not a lazy continuation

                // finalize any blocks that were not matched and set cur to container:
                while (cur != last_matched_container)
                {

                    Finalize(cur, line, settings);
                    cur = cur.Parent;

                    if (cur == null)
                        throw new CommonMarkException("Cannot finalize container block. Last matched container tag = " + last_matched_container.Tag);

                }

                if (container.Tag == BlockTag.IndentedCode)
                {
                    AddLine(container, line, ln, offset, remainingSpaces);

                }
                else if (container.Tag == BlockTag.FencedCode)
                {

                    if ((indent <= 3
                      && curChar == container.FencedCodeData.FenceChar)
                      && (0 != Scanner.scan_close_code_fence(ln, first_nonspace, container.FencedCodeData.FenceLength, ln.Length)))
                    {
                        // if closing fence, set fence length to -1. it will be closed when the next line is processed. 
                        container.FencedCodeData.FenceLength = -1;
                    }
                    else
                    {
                        AddLine(container, line, ln, offset, remainingSpaces);
                    }

                }
                else if (container.Tag == BlockTag.HtmlBlock)
                {

                    AddLine(container, line, ln, offset, remainingSpaces);

                    if (Scanner.scan_html_block_end(container.HtmlBlockType, ln, first_nonspace, ln.Length))
                    {
                        Finalize(container, line, settings);
                        container = container.Parent;
                    }

                }
                else if (blank)
                {

                    // ??? do nothing

                }
                else if (container.Tag == BlockTag.AtxHeading)
                {

                    int p = ln.Length - 1;

                    // trim trailing spaces
                    while (p >= 0 && (ln[p] == ' ' || ln[p] == '\t' || ln[p] == '\n'))
                        p--;

                    int px = p;

                    // if string ends in #s, remove these:
                    while (p >= 0 && ln[p] == '#')
                        p--;

                    // there must be a space before the last hashtag
                    if (p < 0 || (ln[p] != ' ' && ln[p] != '\t'))
                        p = px;

                    // trim trailing spaces that are before the closing #s
                    while (p >= first_nonspace && (ln[p] == ' ' || ln[p] == '\t'))
                        p--;

                    AddLine(container, line, ln, first_nonspace, remainingSpaces, p - first_nonspace + 1);
                    Finalize(container, line, settings);
                    container = container.Parent;

                }
                else if (AcceptsLines(container.Tag))
                {

                    AddLine(container, line, ln, first_nonspace, remainingSpaces);

                }
                else if (container.Tag != BlockTag.ThematicBreak && container.Tag != BlockTag.SetextHeading)
                {

                    // create paragraph container for line
                    container = CreateChildBlock(container, line, settings, BlockTag.Paragraph, first_nonspace);
                    AddLine(container, line, ln, first_nonspace, remainingSpaces);

                }
                else
                {

                    Utilities.Warning("Line {0} with container type {1} did not match any condition:\n\"{2}\"", line.LineNumber, container.Tag, ln);

                }

                curptr = container;
            }
        }

        private static void FindFirstNonspace(string ln, int offset, int column, out int first_nonspace,
            out int first_nonspace_column, out char curChar)
        {
            var chars_to_tab = TabSize - (column % TabSize);
            first_nonspace = offset;
            first_nonspace_column = column;
            while ((curChar = ln[first_nonspace]) != '\n')
            {
                if (curChar == ' ')
                {
                    first_nonspace++;
                    first_nonspace_column++;
                    chars_to_tab--;
                    if (chars_to_tab == 0) chars_to_tab = TabSize;
                }
                else if (curChar == '\t')
                {
                    first_nonspace++;
                    first_nonspace_column += chars_to_tab;
                    chars_to_tab = TabSize;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
