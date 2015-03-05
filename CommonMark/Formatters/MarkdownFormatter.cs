using System;
using System.Collections.Generic;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    internal class MarkdownFormatter
    {
        /// <summary>
        /// Write the block data to the given writer.
        /// </summary>
        public static void PrintBlocks(System.IO.TextWriter writer, Block block, CommonMarkSettings settings)
        {
            PrintBlocksInner(new MarkdownTextWriter(writer), block, settings);
        }

        /// <summary>
        /// Write the block data to the given writer.
        /// </summary>
        public static void PrintBlocksInner(MarkdownTextWriter writer, Block block, CommonMarkSettings settings)
        {
            var stack = new Stack<BlockStackEntry>();
            var inlineStack = new Stack<InlineStackEntry>();

            // TODO: use a better approach for keeping track of list items already rendered.
            var listItemNumbering = new Dictionary<ListData, int>();
            string stackPrefix = null;
            bool visitChildren;
            bool tight = false;
            bool stackTight = false;

            while (block != null)
            {
                visitChildren = false;
                stackTight = tight;
                stackPrefix = null;

                switch (block.Tag)
                {
                    case BlockTag.Document:
                    case BlockTag.ReferenceDefinition:
                        visitChildren = true;
                        break;

                    case BlockTag.Paragraph:
                        PrintInlines(writer, block.InlineContent, inlineStack);
                        writer.EnsureLine();
                        if (!tight) writer.WriteLine();
                        break;

                    case BlockTag.BlockQuote:
                        stackPrefix = "> ";
                        visitChildren = true;
                        break;

                    case BlockTag.ListItem:
                        visitChildren = true;
                        var data = block.Parent.ListData;
                        if (data == null)
                            throw new CommonMarkException("ListItem block node is not nested within a List node.");

                        if (data.ListType == ListType.Bullet)
                        {
                            writer.Write(data.BulletChar);
                            writer.Write(' ');
                            stackPrefix = "  ";
                        }
                        else
                        {
                            int delta;
                            if (!listItemNumbering.TryGetValue(data, out delta))
                                listItemNumbering.Add(data, 1);
                            else
                                listItemNumbering[data]++;

                            var deltastr = (delta + data.Start).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            writer.WriteConstant(deltastr);
                            writer.Write(data.Delimiter == ListDelimiter.Parenthesis ? ')' : '.');
                            writer.Write(' ');

                            stackPrefix = new string(' ', 2 + deltastr.Length);
                        }

                        break;

                    case BlockTag.List:
                        stackTight = block.ListData.IsTight;
                        visitChildren = true;
                        break;

                    case BlockTag.AtxHeader:
                        writer.WriteConstant(new string('#', block.HeaderLevel));
                        writer.Write(' ');
                        PrintInlines(writer, block.InlineContent, inlineStack);
                        writer.EnsureLine();
                        if (!tight) writer.WriteLine();
                        break;

                    case BlockTag.SETextHeader:
                        PrintInlines(writer, block.InlineContent, inlineStack);
                        writer.EnsureLine();
                        writer.WriteLineConstant(block.HeaderLevel == 1 ? "===" : "---");
                        if (!tight) writer.WriteLine();
                        break;

                    case BlockTag.HorizontalRuler:
                        writer.WriteLineConstant("---");
                        if (!tight) writer.WriteLine();
                        break;

                    case BlockTag.IndentedCode:
                        stackPrefix = writer.Prefix;
                        writer.Prefix += "    ";
                        writer.Write(block.StringContent);
                        writer.EnsureLine();
                        if (!tight) writer.WriteLine();

                        writer.Prefix = stackPrefix;

                        break;

                    case BlockTag.FencedCode:
                        writer.WriteConstant("~~~");
                        writer.WriteLineConstant(block.FencedCodeData.Info);
                        writer.Write(block.StringContent);
                        writer.EnsureLine();
                        writer.WriteLineConstant("~~~");
                        if (!tight) writer.WriteLine();
                        break;

                    case BlockTag.HtmlBlock:
                        writer.Write(block.StringContent);
                        writer.EnsureLine();
                        if (!tight) writer.WriteLine();
                        break;

                    default:
                        throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                }

                if (visitChildren)
                {
                    stack.Push(new BlockStackEntry(writer.Prefix, tight, block.NextSibling));
                    block = block.FirstChild;
                    writer.Prefix = writer.Prefix + stackPrefix;
                    tight = stackTight;
                }
                else if (block.NextSibling != null)
                {
                    block = block.NextSibling;
                }
                else
                {
                    block = null;
                }

                while (block == null && stack.Count > 0)
                {
                    var entry = stack.Pop();

                    writer.Prefix = entry.Prefix;
                    tight = entry.IsTight;
                    block = entry.Target;
                }
            }

            writer.WriteLine();
        }

        private static void PrintInlines(MarkdownTextWriter writer, Inline inline, Stack<InlineStackEntry> stack)
        {
            string stackLiteral = null;
            bool visitChildren;

            while (inline != null)
            {
                visitChildren = false;

                switch (inline.Tag)
                {
                    case InlineTag.String:
                        writer.Write(inline.LiteralContentValue);
                        break;

                    case InlineTag.LineBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.SoftBreak:
                        writer.Write(' ');
                        break;

                    case InlineTag.Code:
                        writer.Write('`');
                        writer.Write(inline.LiteralContentValue);
                        writer.Write('`');
                        break;

                    case InlineTag.RawHtml:
                        writer.Write(inline.LiteralContentValue);
                        break;

                    case InlineTag.Link:
                        writer.Write('[');
                        visitChildren = true;
                        stackLiteral = "](" + inline.TargetUrl + ")";
                        break;

                    case InlineTag.Image:
                        writer.Write('[');
                        visitChildren = true;
                        stackLiteral = "](" + inline.TargetUrl + ")";
                        break;

                    case InlineTag.Strong:
                        writer.WriteConstant("**");
                        visitChildren = true;
                        stackLiteral = "**";
                        break;

                    case InlineTag.Emphasis:
                        writer.Write('*');
                        visitChildren = true;
                        stackLiteral = "*";
                        break;

                    case InlineTag.Strikethrough:
                        writer.WriteConstant("~~");
                        visitChildren = true;
                        stackLiteral = "~~";
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                if (visitChildren)
                {
                    stack.Push(new InlineStackEntry(stackLiteral, inline.NextSibling));
                    inline = inline.FirstChild;
                }
                else if (inline.NextSibling != null)
                {
                    inline = inline.NextSibling;
                }
                else
                {
                    inline = null;
                }

                while (inline == null && stack.Count > 0)
                {
                    var entry = stack.Pop();
                    writer.WriteConstant(entry.Literal);
                    inline = entry.Target;
                }
            }
        }

        private struct BlockStackEntry
        {
            public readonly Block Target;
            public readonly string Prefix;
            public readonly bool IsTight;
            public BlockStackEntry(string prefix, bool tight, Block target)
            {
                this.Target = target;
                this.Prefix = prefix;
                this.IsTight = tight;
            }
        }
        private struct InlineStackEntry
        {
            public readonly Inline Target;
            public readonly string Literal;
            public InlineStackEntry(string literal, Inline target)
            {
                this.Literal = literal;
                this.Target = target;
            }
        }
    }
}
