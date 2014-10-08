using CommonMark.Parser;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Formatter
{
    internal static class HtmlPrinter
    {
        private static readonly char[] EscapeHtmlCharacters = new[] { '&', '<', '>', '"' };
        private static readonly string HexCharacters = "0123456789ABCDEF";

        private static readonly bool[] UrlSafeCharacters = new[] {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, true,  false, true,  true,  true,  false, false, true,  true,  true,  true,  true,  true,  true,  true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, true,  false, true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, true, 
            false, true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true, 
            true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  true,  false, false, false, false, false,
        };

        /// <summary>
        /// Escapes special URL characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        private static void EscapeUrl(string input, System.IO.TextWriter target)
        {
            char c;
            int lastPos = 0;
            char[] buffer = input.ToCharArray();
            for (var pos = 0; pos < buffer.Length; pos++)
            {
                c = buffer[pos];

                if (c == '&')
                {
                    target.Write(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;
                    target.Write("&amp;");
                }
                else if (c < 128 && !UrlSafeCharacters[c])
                {
                    target.Write(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    target.Write(new[] { '%', HexCharacters[c / 16], HexCharacters[c % 16] });
                }
                else if (c > 127)
                {
                    target.Write(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && buffer.Length != lastPos)
                    {
                        // this char is the first of UTF-32 character pair
                        bytes = Encoding.UTF8.GetBytes(new[] { c, buffer[lastPos] });
                        lastPos = ++pos + 1;
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(new[] { c });
                    }

                    for (var i = 0; i < bytes.Length; i++)
                        target.Write(new[] { '%', HexCharacters[bytes[i] / 16], HexCharacters[bytes[i] % 16] });
                }
            }

            target.Write(buffer, lastPos, buffer.Length - lastPos);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        private static void EscapeHtml(string input, System.IO.TextWriter target)
        {
            int pos = 0;
            int lastPos = 0;
            char[] buffer = null;

            while ((pos = input.IndexOfAny(EscapeHtmlCharacters, lastPos)) != -1)
            {
                if (buffer == null)
                    buffer = input.ToCharArray();

                target.Write(buffer, lastPos, pos - lastPos);
                lastPos = pos + 1;

                switch (buffer[pos])
                {
                    case '<':
                        target.Write("&lt;");
                        break;
                    case '>':
                        target.Write("&gt;");
                        break;
                    case '&':
                        target.Write("&amp;");
                        break;
                    case '"':
                        target.Write("&quot;");
                        break;
                }
            }

            if (buffer == null)
                target.Write(input);
            else
                target.Write(buffer, lastPos, input.Length - lastPos);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        private static void EscapeHtml(StringContent inp, System.IO.TextWriter target)
        {
            int pos;
            int lastPos;
            char[] buffer = null;

            var parts = inp.RetrieveParts();
            for (var i = parts.Offset; i < parts.Offset + parts.Count; i++)
            {
                var part = parts.Array[i];

                if (buffer == null || buffer.Length < part.Length)
                    buffer = new char[part.Length];
                
                part.Source.CopyTo(part.StartIndex, buffer, 0, part.Length);

                lastPos = pos = part.StartIndex;
                while ((pos = part.Source.IndexOfAny(EscapeHtmlCharacters, lastPos, part.Length - lastPos + part.StartIndex)) != -1)
                {
                    target.Write(buffer, lastPos - part.StartIndex, pos - lastPos);
                    lastPos = pos + 1;

                    switch (part.Source[pos])
                    {
                        case '<':
                            target.Write("&lt;");
                            break;
                        case '>':
                            target.Write("&gt;");
                            break;
                        case '&':
                            target.Write("&amp;");
                            break;
                        case '"':
                            target.Write("&quot;");
                            break;
                    }
                }

                target.Write(buffer, lastPos - part.StartIndex, part.Length - lastPos + part.StartIndex);
            }
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
                        var data = b.ListData;
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
                        tag = "h" + b.HeaderLevel.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        EnsureNewlineEnding(writer);
                        writer.Write("<" + tag + ">");
                        InlinesToHtml(writer, b.InlineContent);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.IndentedCode:
                        EnsureNewlineEnding(writer);
                        writer.Write("<pre><code>");
                        EscapeHtml(b.StringContent, writer);
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.FencedCode:
                        EnsureNewlineEnding(writer);
                        writer.Write("<pre><code");
                        if (b.FencedCodeData.Info.Length > 0)
                        {
                            string[] info_words = b.FencedCodeData.Info.Split(new[] { ' ' });
                            writer.Write(" class=\"language-");
                            EscapeHtml(info_words[0], writer);
                            writer.Write("\"");
                        }
                        writer.Write(">");
                        EscapeHtml(b.StringContent, writer);
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.HtmlBlock:
                        b.StringContent.WriteTo(writer);
                        break;

                    case BlockTag.HorizontalRuler:
                        writer.WriteLine("<hr />");
                        break;

                    case BlockTag.ReferenceDefinition:
                        break;

                    default:
                        throw new CommonMarkException("Block type " + b.Tag + " is not supported.", b);
                }
                b = b.NextSibling;
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
                        EscapeHtml(ils.LiteralContent, writer);
                        break;

                    case InlineTag.LineBreak:
                        writer.WriteLine("<br />");
                        break;

                    case InlineTag.SoftBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.Code:
                        writer.Write("<code>");
                        EscapeHtml(ils.LiteralContent, writer);
                        writer.Write("</code>");
                        break;

                    case InlineTag.RawHtml:
                        writer.Write(ils.LiteralContent);
                        break;

                    case InlineTag.Link:
                        writer.Write("<a href=\"");
                        EscapeUrl(ils.Linkable.Url, writer);
                        writer.Write('\"');
                        if (ils.Linkable.Title.Length > 0)
                        {
                            writer.Write(" title=\"");
                            EscapeHtml(ils.Linkable.Title, writer);
                            writer.Write('\"');
                        }
                        
                        writer.Write('>');
                        InlinesToHtml(writer, ils.Linkable.Label);
                        writer.Write("</a>");
                        break;

                    case InlineTag.Image:
                        writer.Write("<img src=\"");
                        EscapeUrl(ils.Linkable.Url, writer);
                        writer.Write("\" alt=\"");
                        using (var sb = new System.IO.StringWriter())
                        using (var sbw = new HtmlTextWriter(sb))
                        {
                            InlinesToHtml(sbw, ils.Linkable.Label);
                            sbw.Flush();
                            EscapeHtml(sb.ToString(), writer);
                        }
                        writer.Write("\"");
                        if (ils.Linkable.Title.Length > 0)
                        {
                            writer.Write(" title=\"");
                            EscapeHtml(ils.Linkable.Title, writer);
                            writer.Write("\"");
                        }
                        writer.Write(" />");
                        break;

                    case InlineTag.Strong:
                        writer.Write("<strong>");
                        InlinesToHtml(writer, ils.FirstChild);
                        writer.Write("</strong>");
                        break;

                    case InlineTag.Emphasis:
                        writer.Write("<em>");
                        InlinesToHtml(writer, ils.FirstChild);
                        writer.Write("</em>");
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + ils.Tag + " is not supported.", ils);
                }
                ils = ils.NextSibling;
            }
        }

    }
}
