using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonMark.Formatter
{
    internal class MarkdownPrinter
    {
        /// <summary>
        /// Write the block data to the given writer.
        /// </summary>
        public static void PrintBlocks(System.IO.TextWriter writer, Block block, CommonMarkSettings settings)
        {
            var stack = new Stack<BlockStackEntry>();
            var inlineStack = new Stack<InlineStackEntry>();
            var buffer = new StringBuilder();
            string prefix = string.Empty;
            string stackLiteral = null;
            string stackPrefix = null;
            bool visitChildren;

            while (block != null)
            {
                visitChildren = false;

                switch (block.Tag)
                {
                    case BlockTag.Document:
                    case BlockTag.ReferenceDefinition:
                        visitChildren = true;
                        stackPrefix = null;
                        stackLiteral = null;
                        break;

                    case BlockTag.Paragraph:
                        PrintInlines(writer, block.InlineContent, prefix, inlineStack, buffer);
                        writer.WriteLine();
                        writer.WriteLine();
                        break;

                    case BlockTag.BlockQuote:
                        writer.Write("block_quote");
                        break;

                    case BlockTag.ListItem:
                        writer.Write("list_item");
                        break;

                    case BlockTag.List:
                        writer.Write("list");

                        var data = block.ListData;
                        if (data.ListType == ListType.Ordered)
                        {
                            writer.Write(" (type=ordered tight={0} start={1} delim={2})",
                                 data.IsTight,
                                 data.Start,
                                 data.Delimiter);
                        }
                        else
                        {
                            writer.Write("(type=bullet tight={0} bullet_char={1})",
                                 data.IsTight,
                                 data.BulletChar);
                        }
                        break;

                    case BlockTag.AtxHeader:
                        writer.Write(new string('#', block.HeaderLevel));
                        writer.Write(" ");
                        PrintInlines(writer, block.InlineContent, prefix, inlineStack, buffer);
                        writer.WriteLine();
                        writer.WriteLine();
                        break;

                    case BlockTag.SETextHeader:
                        PrintInlines(writer, block.InlineContent, prefix, inlineStack, buffer);
                        writer.WriteLine();
                        writer.WriteLine(block.HeaderLevel == 1 ? "===" : "---");
                        writer.WriteLine();
                        writer.WriteLine();
                        break;

                    case BlockTag.HorizontalRuler:
                        writer.WriteLine();
                        writer.WriteLine();
                        writer.Write("---");
                        writer.WriteLine();
                        writer.WriteLine();
                        break;

                    case BlockTag.IndentedCode:
                        stackPrefix = prefix;
                        prefix += "    ";
                        writer.WriteLine(block.StringContent.ToString(buffer));

                        prefix = stackPrefix;

                        break;

                    case BlockTag.FencedCode:
                        writer.Write("~~~");
                        writer.WriteLine(block.FencedCodeData.Info);
                        writer.WriteLine(block.StringContent.ToString(buffer));
                        writer.Write("~~~");
                        break;

                    case BlockTag.HtmlBlock:
                        writer.WriteLine(block.StringContent.ToString(buffer));
                        writer.WriteLine();
                        break;

                    default:
                        throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                }

                if (visitChildren)
                {
                    stack.Push(new BlockStackEntry(stackPrefix, stackLiteral, block.NextSibling));
                    block = block.FirstChild;
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

                    stackLiteral = entry.Literal;
                    if (stackLiteral != null)
                    {
                        writer.WriteLine();
                        writer.Write(entry.Literal);
                    }

                    writer.WriteLine();
                    writer.WriteLine();

                    prefix = entry.Prefix;
                    block = entry.Target;
                }
            }

            writer.WriteLine();
        }

        private static void PrintInlines(System.IO.TextWriter writer, Inline inline, string prefix, Stack<InlineStackEntry> stack, StringBuilder buffer)
        {
            string stackLiteral = null;
            bool visitChildren;

            while (inline != null)
            {
                visitChildren = false;

                switch (inline.Tag)
                {
                    case InlineTag.String:
                        writer.Write(inline.LiteralContent);
                        break;

                    case InlineTag.LineBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.SoftBreak:
                        writer.Write(" ");
                        break;

                    case InlineTag.Code:
                        writer.Write("`");
                        writer.Write(inline.LiteralContent);
                        writer.Write("`");
                        break;

                    case InlineTag.RawHtml:
                        writer.Write(inline.LiteralContent);
                        break;

                    case InlineTag.Link:
                        writer.Write("[");
                        visitChildren = true;
                        stackLiteral = "](" + inline.TargetUrl + ")";
                        break;

                    case InlineTag.Image:
                        writer.Write("[");
                        visitChildren = true;
                        stackLiteral = "](" + inline.TargetUrl + ")";
                        break;

                    case InlineTag.Strong:
                        writer.Write("**");
                        visitChildren = true;
                        stackLiteral = "**";
                        break;

                    case InlineTag.Emphasis:
                        writer.Write("*");
                        visitChildren = true;
                        stackLiteral = "*";
                        break;

                    case InlineTag.Strikethrough:
                        writer.Write("~~");
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
                    writer.Write(entry.Literal);
                    inline = entry.Target;
                }
            }
        }

        private struct BlockStackEntry
        {
            public readonly Block Target;
            public readonly string Prefix;
            public readonly string Literal;
            public BlockStackEntry(string prefix, string literal, Block target)
            {
                this.Literal = literal;
                this.Target = target;
                this.Prefix = prefix;
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
