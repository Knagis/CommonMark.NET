using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatter
{
    internal static class Printer
    {
        private static string format_str(string s)
        {
            int pos = 0;
            int len = s.Length;
            StringBuilder result = new StringBuilder();
            char c;
            result.Append("\"");
            while (pos < len)
            {
                c = s[pos];
                switch (c)
                {
                    case '\n':
                        result.Append("\\n");
                        break;
                    case '"':
                        result.Append("\\\"");
                        break;
                    case '\\':
                        result.Append("\\\\");
                        break;
                    default:
                        result.Append(c);
                        break;
                }
                pos++;
            }
            result.Append("\"");
            return result.ToString();
        }

        // Functions to pretty-print inline and block lists, for debugging.
        // Prettyprint an inline list, for debugging.
        public static void print_blocks(System.IO.TextWriter writer, Block b, int indent)
        {
            ListData data;
            while (b != null)
            {
                // printf("%3d %3d %3d| ", b.start_line, b.start_column, b.end_line);
                for (int i = 0; i < indent; i++)
                {
                    writer.Write(' ');
                }
                switch (b.Tag)
                {
                    case BlockTag.Document:
                        writer.WriteLine("document");
                        print_blocks(writer, b.FirstChild, indent + 2);
                        break;
                    case BlockTag.BlockQuote:
                        writer.WriteLine("block_quote");
                        print_blocks(writer, b.FirstChild, indent + 2);
                        break;
                    case BlockTag.ListItem:
                        data = b.Attributes.ListData;
                        writer.WriteLine("list_item");
                        print_blocks(writer, b.FirstChild, indent + 2);
                        break;
                    case BlockTag.List:
                        data = b.Attributes.ListData;
                        if (data.ListType == ListType.Ordered)
                        {
                            writer.WriteLine("list (type=ordered tight={0} start={1} delim={2})",
                                 data.IsTight,
                                 data.Start,
                                 data.Delimiter);
                        }
                        else
                        {
                            writer.WriteLine("list (type=bullet tight={0} bullet_char={1})",
                                 data.IsTight,
                                 data.BulletChar);
                        }
                        print_blocks(writer, b.FirstChild, indent + 2);
                        break;
                    case BlockTag.AtxHeader:
                        writer.WriteLine("atx_header (level={0})", b.Attributes.HeaderLevel);
                        print_inlines(writer, b.InlineContent, indent + 2);
                        break;
                    case BlockTag.SETextHeader:
                        writer.WriteLine("setext_header (level={0})", b.Attributes.HeaderLevel);
                        print_inlines(writer, b.InlineContent, indent + 2);
                        break;
                    case BlockTag.Paragraph:
                        writer.WriteLine("paragraph");
                        print_inlines(writer, b.InlineContent, indent + 2);
                        break;
                    case BlockTag.HorizontalRuler:
                        writer.WriteLine("hrule");
                        break;
                    case BlockTag.IndentedCode:
                        writer.WriteLine("indented_code {0}", format_str(b.StringContent.ToString()));
                        break;
                    case BlockTag.FencedCode:
                        writer.WriteLine("fenced_code length={0} info={1} {2}",
                               b.Attributes.FencedCodeData.FenceLength,
                               format_str(b.Attributes.FencedCodeData.Info),
                               format_str(b.StringContent.ToString()));
                        break;
                    case BlockTag.HtmlBlock:
                        writer.WriteLine("html_block {0}", format_str(b.StringContent.ToString()));
                        break;
                    case BlockTag.ReferenceDefinition:
                        writer.WriteLine("reference_def");
                        break;
                    default:
                        throw new CommonMarkException("Block type " + b.Tag + " is not supported.", b);
                }
                b = b.Next;
            }
        }

        // Prettyprint an inline list, for debugging.
        public static void print_inlines(System.IO.TextWriter writer, Inline ils, int indent)
        {
            while (ils != null)
            {
                /*
                // we add 11 extra spaces for the line/column info
                for (int i=0; i < 11; i++) {
                  putchar(' ');
                }
                putchar('|');
                putchar(' ');
                */
                for (int i = 0; i < indent; i++)
                {
                    writer.Write(' ');
                }
                switch (ils.Tag)
                {
                    case InlineTag.String:
                        writer.WriteLine("str {0}", format_str(ils.LiteralContent));
                        break;
                    case InlineTag.LineBreak:
                        writer.WriteLine("linebreak");
                        break;
                    case InlineTag.SoftBreak:
                        writer.WriteLine("softbreak");
                        break;
                    case InlineTag.Code:
                        writer.WriteLine("code {0}", format_str(ils.LiteralContent));
                        break;
                    case InlineTag.RawHtml:
                        writer.WriteLine("html {0}", format_str(ils.LiteralContent));
                        break;
                    case InlineTag.Entity:
                        writer.WriteLine("entity {0}", format_str(ils.LiteralContent));
                        break;
                    case InlineTag.Link:
                        writer.WriteLine("link url={0} title={1}",
                               format_str(ils.Linkable.Url),
                               format_str(ils.Linkable.Title));
                        print_inlines(writer, ils.Linkable.Label, indent + 2);
                        break;
                    case InlineTag.Image:
                        writer.WriteLine("image url={0} title={1}",
                               format_str(ils.Linkable.Url),
                               format_str(ils.Linkable.Title));
                        print_inlines(writer, ils.Linkable.Label, indent + 2);
                        break;
                    case InlineTag.Strong:
                        writer.WriteLine("strong");
                        print_inlines(writer, ils.Linkable.Label, indent + 2);
                        break;
                    case InlineTag.Emphasis:
                        writer.WriteLine("emph");
                        print_inlines(writer, ils.Linkable.Label, indent + 2);
                        break;
                }
                ils = ils.NextSibling;
            }
        }
    }
}
