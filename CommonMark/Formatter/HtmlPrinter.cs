using CommonMark.Parser;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatter
{
    internal static class HtmlPrinter
    {
        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        private static string escape_html(string inp, bool preserve_entities)
        {
            int pos = 0;
            int match;
            char c;
            string escapable = "&<>\"";
            string ent;
            string s = inp;
            while ((pos = BString.binchr(s, pos, escapable)) != -1)
            {
                c = s[pos];
                switch (c)
                {
                    case '<':
                        s = s.Remove(pos, 1);
                        ent = "&lt;";
                        BString.binsert(ref s, pos, ent, ' ');
                        pos += 4;
                        break;
                    case '>':
                        s = s.Remove(pos, 1);
                        ent = "&gt;";
                        BString.binsert(ref s, pos, ent, ' ');
                        pos += 4;
                        break;
                    case '&':
                        if (preserve_entities && 0 != (match = Scanner.scan_entity(s, pos)))
                        {
                            pos += match;
                        }
                        else
                        {
                            s = s.Remove(pos, 1);
                            ent = "&amp;";
                            BString.binsert(ref s, pos, ent, ' ');
                            pos += 5;
                        }
                        break;
                    case '"':
                        s = s.Remove(pos, 1);
                        ent = "&quot;";
                        BString.binsert(ref s, pos, ent, ' ');
                        pos += 6;
                        break;
                    default:
                        s = s.Remove(pos, 1);
                        throw new CommonMarkException(string.Format("Unexpected character '{0}' ({1}). Source string: '{2}', position {3}", c, (int)c, inp, pos));
                }
            }
            return s;
        }

        /// <summary>
        /// Adds a newline if the writer does not currently end with a newline.
        /// </summary>
        private static void cr(HtmlTextWriter writer)
        {
            if (!writer.EndsWithNewline)
                writer.WriteLine();
        }

        // Convert a block list to HTML.  Returns 0 on success, and sets result.
        public static void blocks_to_html(System.IO.TextWriter writer, Block b, bool tight)
        {
            using (var wrapper = new HtmlTextWriter(writer))
                blocks_to_html_inner(wrapper, b, tight);
        }

        // Convert a block list to HTML.  Returns 0 on success, and sets result.
        private static void blocks_to_html_inner(HtmlTextWriter writer, Block b, bool tight)
        {
            string tag;
            while (b != null)
            {
                switch (b.tag)
                {
                    case BlockTag.document:
                        blocks_to_html_inner(writer, b.children, false);
                        break;

                    case BlockTag.paragraph:
                        if (tight)
                        {
                            inlines_to_html(writer, b.inline_content);
                        }
                        else
                        {
                            cr(writer);
                            writer.Write("<p>");
                            inlines_to_html(writer, b.inline_content);
                            writer.WriteLine("</p>");
                        }
                        break;

                    case BlockTag.block_quote:
                        cr(writer);
                        writer.WriteLine("<blockquote>");
                        blocks_to_html_inner(writer, b.children, false);
                        writer.WriteLine("</blockquote>");
                        break;

                    case BlockTag.list_item:
                        cr(writer);
                        writer.Write("<li>");
                        using (var sb = new System.IO.StringWriter())
                        using (var sbw = new HtmlTextWriter(sb))
                        {
                            blocks_to_html_inner(sbw, b.children, tight);
                            sbw.Flush();
                            writer.Write(sb.ToString().TrimEnd());
                        }
                        writer.WriteLine("</li>");
                        break;

                    case BlockTag.list:
                        // make sure a list starts at the beginning of the line:
                        cr(writer);
                        var data = b.attributes.list_data;
                        tag = data.ListType == ListType.Bullet ? "ul" : "ol";
                        writer.Write("<" + tag);
                        if (data.start != 1)
                            writer.Write(" start=\"" + data.start.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\"");
                        writer.WriteLine(">");
                        blocks_to_html_inner(writer, b.children, data.tight);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.atx_header:
                    case BlockTag.setext_header:
                        tag = "h" + b.attributes.header_level.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        cr(writer);
                        writer.Write("<" + tag + ">");
                        inlines_to_html(writer, b.inline_content);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.indented_code:
                        cr(writer);
                        writer.Write("<pre><code>");
                        writer.Write(escape_html(b.string_content, false));
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.fenced_code:
                        cr(writer);
                        writer.Write("<pre><code");
                        if (b.attributes.fenced_code_data.info.Length > 0)
                        {
                            var info_words = escape_html(b.attributes.fenced_code_data.info, true).Split(new[] { ' ' });
                            writer.Write(" class=\"language-" + info_words[0] + "\"");
                        }
                        writer.Write(">");
                        writer.Write(escape_html(b.string_content, false));
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.html_block:
                        writer.Write(b.string_content);
                        break;

                    case BlockTag.hrule:
                        writer.WriteLine("<hr />");
                        break;

                    case BlockTag.reference_def:
                        break;

                    default:
                        throw new CommonMarkException("Block type " + b.tag + " is not supported.", b);
                }
                b = b.next;
            }
        }

        // Convert an inline list to HTML.  Returns 0 on success, and sets result.
        public static void inlines_to_html(HtmlTextWriter writer, Inline ils)
        {
            while (ils != null)
            {
                switch (ils.tag)
                {
                    case InlineTag.str:
                        writer.Write(escape_html(ils.content.Literal, false));
                        break;

                    case InlineTag.linebreak:
                        writer.WriteLine("<br />");
                        break;

                    case InlineTag.softbreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.code:
                        writer.Write("<code>");
                        writer.Write(escape_html(ils.content.Literal, false));
                        writer.Write("</code>");
                        break;

                    case InlineTag.raw_html:
                    case InlineTag.entity:
                        writer.Write(ils.content.Literal);
                        break;

                    case InlineTag.link:
                        string mbtitle;
                        if (ils.content.linkable.title.Length > 0)
                            mbtitle = " title=\"" + escape_html(ils.content.linkable.title, true) + "\"";
                        else
                            mbtitle = "";

                        writer.Write("<a href=\"{0}\"{1}>", escape_html(ils.content.linkable.url, true), mbtitle);
                        inlines_to_html(writer, ils.content.linkable.label);
                        writer.Write("</a>");
                        break;

                    case InlineTag.image:
                        writer.Write("<img src=\"");
                        writer.Write(escape_html(ils.content.linkable.url, true));
                        writer.Write("\" alt=\"");
                        using (var sb = new System.IO.StringWriter())
                        using (var sbw = new HtmlTextWriter(sb))
                        {
                            inlines_to_html(sbw, ils.content.linkable.label);
                            sbw.Flush();
                            writer.Write(escape_html(sb.ToString(), false));
                        }
                        writer.Write("\"");
                        if (ils.content.linkable.title.Length > 0)
                        {
                            writer.Write(" title=\"");
                            writer.Write(escape_html(ils.content.linkable.title, true));
                            writer.Write("\"");
                        }
                        writer.Write(" />");
                        break;

                    case InlineTag.strong:
                        writer.Write("<strong>");
                        inlines_to_html(writer, ils.content.inlines);
                        writer.Write("</strong>");
                        break;

                    case InlineTag.emph:
                        writer.Write("<em>");
                        inlines_to_html(writer, ils.content.inlines);
                        writer.Write("</em>");
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + ils.tag + " is not supported.", ils);
                }
                ils = ils.next;
            }
        }

    }
}
