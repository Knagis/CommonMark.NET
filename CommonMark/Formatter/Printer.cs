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
                switch (b.tag)
                {
                    case BlockTag.document:
                        writer.WriteLine("document");
                        print_blocks(writer, b.children, indent + 2);
                        break;
                    case BlockTag.block_quote:
                        writer.WriteLine("block_quote");
                        print_blocks(writer, b.children, indent + 2);
                        break;
                    case BlockTag.list_item:
                        data = b.attributes.list_data;
                        writer.WriteLine("list_item");
                        print_blocks(writer, b.children, indent + 2);
                        break;
                    case BlockTag.list:
                        data = b.attributes.list_data;
                        if (data.ListType == ListType.Ordered)
                        {
                            writer.WriteLine("list (type=ordered tight={0} start={1} delim={2})",
                                 data.tight,
                                 data.start,
                                 data.delimiter);
                        }
                        else
                        {
                            writer.WriteLine("list (type=bullet tight={0} bullet_char={1})",
                                 data.tight,
                                 data.BulletChar);
                        }
                        print_blocks(writer, b.children, indent + 2);
                        break;
                    case BlockTag.atx_header:
                        writer.WriteLine("atx_header (level={0})", b.attributes.header_level);
                        print_inlines(writer, b.inline_content, indent + 2);
                        break;
                    case BlockTag.setext_header:
                        writer.WriteLine("setext_header (level={0})", b.attributes.header_level);
                        print_inlines(writer, b.inline_content, indent + 2);
                        break;
                    case BlockTag.paragraph:
                        writer.WriteLine("paragraph");
                        print_inlines(writer, b.inline_content, indent + 2);
                        break;
                    case BlockTag.hrule:
                        writer.WriteLine("hrule");
                        break;
                    case BlockTag.indented_code:
                        writer.WriteLine("indented_code {0}", format_str(b.string_content));
                        break;
                    case BlockTag.fenced_code:
                        writer.WriteLine("fenced_code length={0} info={1} {2}",
                               b.attributes.fenced_code_data.fence_length,
                               format_str(b.attributes.fenced_code_data.info),
                               format_str(b.string_content));
                        break;
                    case BlockTag.html_block:
                        writer.WriteLine("html_block {0}", format_str(b.string_content));
                        break;
                    case BlockTag.reference_def:
                        writer.WriteLine("reference_def");
                        break;
                    default:
                        throw new CommonMarkException("Block type " + b.tag + " is not supported.", b);
                }
                b = b.next;
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
                switch (ils.tag)
                {
                    case InlineTag.str:
                        writer.WriteLine("str {0}", format_str(ils.content.Literal));
                        break;
                    case InlineTag.linebreak:
                        writer.WriteLine("linebreak");
                        break;
                    case InlineTag.softbreak:
                        writer.WriteLine("softbreak");
                        break;
                    case InlineTag.code:
                        writer.WriteLine("code {0}", format_str(ils.content.Literal));
                        break;
                    case InlineTag.raw_html:
                        writer.WriteLine("html {0}", format_str(ils.content.Literal));
                        break;
                    case InlineTag.entity:
                        writer.WriteLine("entity {0}", format_str(ils.content.Literal));
                        break;
                    case InlineTag.link:
                        writer.WriteLine("link url={0} title={1}",
                               format_str(ils.content.linkable.url),
                               format_str(ils.content.linkable.title));
                        print_inlines(writer, ils.content.linkable.label, indent + 2);
                        break;
                    case InlineTag.image:
                        writer.WriteLine("image url={0} title={1}",
                               format_str(ils.content.linkable.url),
                               format_str(ils.content.linkable.title));
                        print_inlines(writer, ils.content.linkable.label, indent + 2);
                        break;
                    case InlineTag.strong:
                        writer.WriteLine("strong");
                        print_inlines(writer, ils.content.linkable.label, indent + 2);
                        break;
                    case InlineTag.emph:
                        writer.WriteLine("emph");
                        print_inlines(writer, ils.content.linkable.label, indent + 2);
                        break;
                }
                ils = ils.next;
            }
        }
    }
}
