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
        /// Orig: escape_html(inp, preserve_entities)
        /// </summary>
        private static string EscapeHtml(string inp, bool preserveEntities)
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
                        if (preserveEntities && 0 != (match = Scanner.scan_entity(s, pos)))
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
        /// Orig: cr
        /// </summary>
        private static void EnsureNewlineEnding(HtmlTextWriter writer)
        {
            if (!writer.EndsWithNewline)
                writer.WriteLine();
        }

        // Convert a block list to HTML.  Returns 0 on success, and sets result.
        /// <summary>
        /// Orig: blocks_to_html
        /// </summary>
        public static void BlocksToHtml(System.IO.TextWriter writer, Block b, bool tight)
        {
            using (var wrapper = new HtmlTextWriter(writer))
                BlocksToHtmlInner(wrapper, b, tight);
        }

        // Convert a block list to HTML.  Returns 0 on success, and sets result.
        /// <summary>
        /// Orig: 
        /// </summary>
        private static void BlocksToHtmlInner(HtmlTextWriter writer, Block b, bool tight)
        {
            string tag;
            while (b != null)
            {
                switch (b.tag)
                {
                    case BlockTag.document:
                        BlocksToHtmlInner(writer, b.children, false);
                        break;

                    case BlockTag.paragraph:
                        if (tight)
                        {
                            InlinesToHtml(writer, b.inline_content);
                        }
                        else
                        {
                            EnsureNewlineEnding(writer);
                            writer.Write("<p>");
                            InlinesToHtml(writer, b.inline_content);
                            writer.WriteLine("</p>");
                        }
                        break;

                    case BlockTag.block_quote:
                        EnsureNewlineEnding(writer);
                        writer.WriteLine("<blockquote>");
                        BlocksToHtmlInner(writer, b.children, false);
                        writer.WriteLine("</blockquote>");
                        break;

                    case BlockTag.list_item:
                        EnsureNewlineEnding(writer);
                        writer.Write("<li>");
                        using (var sb = new System.IO.StringWriter())
                        using (var sbw = new HtmlTextWriter(sb))
                        {
                            BlocksToHtmlInner(sbw, b.children, tight);
                            sbw.Flush();
                            writer.Write(sb.ToString().TrimEnd());
                        }
                        writer.WriteLine("</li>");
                        break;

                    case BlockTag.list:
                        // make sure a list starts at the beginning of the line:
                        EnsureNewlineEnding(writer);
                        var data = b.attributes.list_data;
                        tag = data.ListType == ListType.Bullet ? "ul" : "ol";
                        writer.Write("<" + tag);
                        if (data.start != 1)
                            writer.Write(" start=\"" + data.start.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\"");
                        writer.WriteLine(">");
                        BlocksToHtmlInner(writer, b.children, data.tight);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.atx_header:
                    case BlockTag.setext_header:
                        tag = "h" + b.attributes.header_level.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        EnsureNewlineEnding(writer);
                        writer.Write("<" + tag + ">");
                        InlinesToHtml(writer, b.inline_content);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.indented_code:
                        EnsureNewlineEnding(writer);
                        writer.Write("<pre><code>");
                        writer.Write(EscapeHtml(b.string_content, false));
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.fenced_code:
                        EnsureNewlineEnding(writer);
                        writer.Write("<pre><code");
                        if (b.attributes.fenced_code_data.info.Length > 0)
                        {
                            string[] info_words = EscapeHtml(b.attributes.fenced_code_data.info, true).Split(new[] { ' ' }, 2);
                            writer.Write(" class=\"language-" + info_words[0] + "\"");
                        }
                        writer.Write(">");
                        writer.Write(EscapeHtml(b.string_content, false));
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
        /// <summary>
        /// Orig: inlines_to_html
        /// </summary>
        public static void InlinesToHtml(HtmlTextWriter writer, Inline ils)
        {
            while (ils != null)
            {
                switch (ils.tag)
                {
                    case InlineTag.str:
                        writer.Write(EscapeHtml(ils.content.Literal, false));
                        break;

                    case InlineTag.linebreak:
                        writer.WriteLine("<br />");
                        break;

                    case InlineTag.softbreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.code:
                        writer.Write("<code>");
                        writer.Write(EscapeHtml(ils.content.Literal, false));
                        writer.Write("</code>");
                        break;

                    case InlineTag.raw_html:
                    case InlineTag.entity:
                        writer.Write(ils.content.Literal);
                        break;

                    case InlineTag.link:
                        string mbtitle;
                        if (ils.content.linkable.title.Length > 0)
                            mbtitle = " title=\"" + EscapeHtml(ils.content.linkable.title, true) + "\"";
                        else
                            mbtitle = "";

                        writer.Write("<a href=\"{0}\"{1}>", EscapeHtml(ils.content.linkable.url, true), mbtitle);
                        InlinesToHtml(writer, ils.content.linkable.label);
                        writer.Write("</a>");
                        break;

                    case InlineTag.image:
                        writer.Write("<img src=\"");
                        writer.Write(EscapeHtml(ils.content.linkable.url, true));
                        writer.Write("\" alt=\"");
                        using (var sb = new System.IO.StringWriter())
                        using (var sbw = new HtmlTextWriter(sb))
                        {
                            InlinesToHtml(sbw, ils.content.linkable.label);
                            sbw.Flush();
                            writer.Write(EscapeHtml(sb.ToString(), false));
                        }
                        writer.Write("\"");
                        if (ils.content.linkable.title.Length > 0)
                        {
                            writer.Write(" title=\"");
                            writer.Write(EscapeHtml(ils.content.linkable.title, true));
                            writer.Write("\"");
                        }
                        writer.Write(" />");
                        break;

                    case InlineTag.strong:
                        writer.Write("<strong>");
                        InlinesToHtml(writer, ils.content.inlines);
                        writer.Write("</strong>");
                        break;

                    case InlineTag.emph:
                        writer.Write("<em>");
                        InlinesToHtml(writer, ils.content.inlines);
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
