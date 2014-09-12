using CommonMark.Parser;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatter
{
    internal static class HtmlPrinter
    {
        private static readonly char[] EscapeHtmlCharacters = new[] { '&', '<', '>', '\"' };

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        private static string EscapeHtml(string inp, bool preserveEntities)
        {
            int pos = 0;
            int lastPos = 0;
            int match;

            StringBuilder result = null;
            while ((pos = inp.IndexOfAny(EscapeHtmlCharacters, lastPos)) != -1)
            {
                if (result == null)
                    result = new StringBuilder(inp.Length + 10);

                result.Append(inp, lastPos, pos - lastPos);
                lastPos = pos + 1;

                switch (inp[pos])
                {
                    case '<':
                        result.Append("&lt;");
                        break;
                    case '>':
                        result.Append("&gt;");
                        break;
                    case '&':
                        if (preserveEntities && 0 != (match = Scanner.scan_entity(inp, pos)))
                            result.Append('&');
                        else
                            result.Append("&amp;");
                        break;
                    case '"':
                        result.Append("&quot;");
                        break;
                }
            }

            if (result == null)
                return inp;

            result.Append(inp, lastPos, inp.Length - lastPos);
            return result.ToString();
        }

        /// <summary>
        /// Adds a newline if the writer does not currently end with a newline.
        /// </summary>
        /// <remarks>Orig: cr</remarks>
        private static void EnsureNewlineEnding(HtmlTextWriter writer)
        {
            if (!writer.EndsWithNewline)
                writer.WriteLine();
        }

        /// <summary>
        /// Convert a block list to HTML.  Returns 0 on success, and sets result.
        /// </summary>
        /// <remarks>Orig: blocks_to_html</remarks>
        public static void BlocksToHtml(System.IO.TextWriter writer, Block b, bool tight)
        {
            using (var wrapper = new HtmlTextWriter(writer))
                BlocksToHtmlInner(wrapper, b, tight);
        }

        /// <remarks>Orig: blocks_to_html_inner</remarks>
        private static void BlocksToHtmlInner(HtmlTextWriter writer, Block b, bool tight)
        {
            string tag;
            while (b != null)
            {
                switch (b.Tag)
                {
                    case BlockTag.Document:
                        BlocksToHtmlInner(writer, b.FirstChild, false);
                        break;

                    case BlockTag.Paragraph:
                        if (tight)
                        {
                            InlinesToHtml(writer, b.InlineContent);
                        }
                        else
                        {
                            EnsureNewlineEnding(writer);
                            writer.Write("<p>");
                            InlinesToHtml(writer, b.InlineContent);
                            writer.WriteLine("</p>");
                        }
                        break;

                    case BlockTag.BlockQuote:
                        EnsureNewlineEnding(writer);
                        writer.WriteLine("<blockquote>");
                        BlocksToHtmlInner(writer, b.FirstChild, false);
                        writer.WriteLine("</blockquote>");
                        break;

                    case BlockTag.ListItem:
                        EnsureNewlineEnding(writer);
                        writer.Write("<li>");
                        using (var sb = new System.IO.StringWriter())
                        using (var sbw = new HtmlTextWriter(sb))
                        {
                            BlocksToHtmlInner(sbw, b.FirstChild, tight);
                            sbw.Flush();
                            writer.Write(sb.ToString().TrimEnd());
                        }
                        writer.WriteLine("</li>");
                        break;

                    case BlockTag.List:
                        // make sure a list starts at the beginning of the line:
                        EnsureNewlineEnding(writer);
                        var data = b.Attributes.ListData;
                        tag = data.ListType == ListType.Bullet ? "ul" : "ol";
                        writer.Write("<" + tag);
                        if (data.Start != 1)
                            writer.Write(" start=\"" + data.Start.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\"");
                        writer.WriteLine(">");
                        BlocksToHtmlInner(writer, b.FirstChild, data.IsTight);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.AtxHeader:
                    case BlockTag.SETextHeader:
                        tag = "h" + b.Attributes.HeaderLevel.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        EnsureNewlineEnding(writer);
                        writer.Write("<" + tag + ">");
                        InlinesToHtml(writer, b.InlineContent);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.IndentedCode:
                        EnsureNewlineEnding(writer);
                        writer.Write("<pre><code>");
                        writer.Write(EscapeHtml(b.StringContent, false));
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.FencedCode:
                        EnsureNewlineEnding(writer);
                        writer.Write("<pre><code");
                        if (b.Attributes.FencedCodeData.Info.Length > 0)
                        {
                            string[] info_words = EscapeHtml(b.Attributes.FencedCodeData.Info, true).Split(new[] { ' ' });
                            writer.Write(" class=\"language-" + info_words[0] + "\"");
                        }
                        writer.Write(">");
                        writer.Write(EscapeHtml(b.StringContent, false));
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.HtmlBlock:
                        writer.Write(b.StringContent);
                        break;

                    case BlockTag.HorizontalRuler:
                        writer.WriteLine("<hr />");
                        break;

                    case BlockTag.ReferenceDefinition:
                        break;

                    default:
                        throw new CommonMarkException("Block type " + b.Tag + " is not supported.", b);
                }
                b = b.Next;
            }
        }

        // Convert an inline list to HTML.  Returns 0 on success, and sets result.
        /// <summary>
        /// </summary>
        /// <remarks>Orig: inlines_to_html</remarks>
        public static void InlinesToHtml(HtmlTextWriter writer, Inline ils)
        {
            while (ils != null)
            {
                switch (ils.Tag)
                {
                    case InlineTag.String:
                        writer.Write(EscapeHtml(ils.Content.Literal, false));
                        break;

                    case InlineTag.LineBreak:
                        writer.WriteLine("<br />");
                        break;

                    case InlineTag.SoftBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.Code:
                        writer.Write("<code>");
                        writer.Write(EscapeHtml(ils.Content.Literal, false));
                        writer.Write("</code>");
                        break;

                    case InlineTag.RawHtml:
                    case InlineTag.Entity:
                        writer.Write(ils.Content.Literal);
                        break;

                    case InlineTag.Link:
                        string mbtitle;
                        if (ils.Content.Linkable.Title.Length > 0)
                            mbtitle = " title=\"" + EscapeHtml(ils.Content.Linkable.Title, true) + "\"";
                        else
                            mbtitle = "";

                        writer.Write("<a href=\"{0}\"{1}>", EscapeHtml(ils.Content.Linkable.Url, true), mbtitle);
                        InlinesToHtml(writer, ils.Content.Linkable.Label);
                        writer.Write("</a>");
                        break;

                    case InlineTag.Image:
                        writer.Write("<img src=\"");
                        writer.Write(EscapeHtml(ils.Content.Linkable.Url, true));
                        writer.Write("\" alt=\"");
                        using (var sb = new System.IO.StringWriter())
                        using (var sbw = new HtmlTextWriter(sb))
                        {
                            InlinesToHtml(sbw, ils.Content.Linkable.Label);
                            sbw.Flush();
                            writer.Write(EscapeHtml(sb.ToString(), false));
                        }
                        writer.Write("\"");
                        if (ils.Content.Linkable.Title.Length > 0)
                        {
                            writer.Write(" title=\"");
                            writer.Write(EscapeHtml(ils.Content.Linkable.Title, true));
                            writer.Write("\"");
                        }
                        writer.Write(" />");
                        break;

                    case InlineTag.Strong:
                        writer.Write("<strong>");
                        InlinesToHtml(writer, ils.Content.Inlines);
                        writer.Write("</strong>");
                        break;

                    case InlineTag.Emphasis:
                        writer.Write("<em>");
                        InlinesToHtml(writer, ils.Content.Inlines);
                        writer.Write("</em>");
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + ils.Tag + " is not supported.", ils);
                }
                ils = ils.Next;
            }
        }

    }
}
