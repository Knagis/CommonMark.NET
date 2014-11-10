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
        private const string HexCharacters = "0123456789ABCDEF";

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
            if (input == null)
                return;

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
            if (input == null)
                return;

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
        /// Convert a block list to HTML.  Returns 0 on success, and sets result.
        /// </summary>
        /// <remarks>Orig: blocks_to_html</remarks>
        public static void BlocksToHtml(System.IO.TextWriter writer, Block block, CommonMarkSettings settings)
        {
            using (var wrapper = new HtmlTextWriter(writer))
                BlocksToHtmlInner(wrapper, block, settings);
        }

        private static void BlocksToHtmlInner(HtmlTextWriter writer, Block block, CommonMarkSettings settings)
        {
            var stack = new Stack<BlockStackEntry>();
            var inlineStack = new Stack<InlineStackEntry>();
            bool visitChildren;
            string stackLiteral = null;
            bool stackTight = false;
            bool tight = false;

            string tag;
            while (block != null)
            {
                visitChildren = false;

                switch (block.Tag)
                {
                    case BlockTag.Document:
                        stackLiteral = null;
                        stackTight = false;
                        visitChildren = true;
                        break;

                    case BlockTag.Paragraph:
                        if (tight)
                        {
                            InlinesToHtml(writer, block.InlineContent, settings, inlineStack);
                        }
                        else
                        {
                            writer.EnsureLine();
                            writer.Write("<p>");
                            InlinesToHtml(writer, block.InlineContent, settings, inlineStack);
                            writer.WriteLine("</p>");
                        }
                        break;

                    case BlockTag.BlockQuote:
                        writer.EnsureLine();
                        writer.WriteLine("<blockquote>");

                        stackLiteral = "</blockquote>" + Environment.NewLine;
                        stackTight = false;
                        visitChildren = true;
                        break;

                    case BlockTag.ListItem:
                        writer.EnsureLine();
                        writer.Write("<li>");

                        stackLiteral = "</li>" + Environment.NewLine;
                        stackTight = tight;
                        visitChildren = true;
                        break;

                    case BlockTag.List:
                        // make sure a list starts at the beginning of the line:
                        writer.EnsureLine();
                        var data = block.ListData;
                        tag = data.ListType == ListType.Bullet ? "ul" : "ol";
                        writer.Write("<" + tag);
                        if (data.Start != 1)
                            writer.Write(" start=\"" + data.Start.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\"");
                        writer.WriteLine(">");

                        stackLiteral = "</" + tag + ">" + Environment.NewLine;
                        stackTight = data.IsTight;
                        visitChildren = true;
                        break;

                    case BlockTag.AtxHeader:
                    case BlockTag.SETextHeader:
                        tag = "h" + block.HeaderLevel.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        writer.EnsureLine();
                        writer.Write("<" + tag + ">");
                        InlinesToHtml(writer, block.InlineContent, settings, inlineStack);
                        writer.WriteLine("</" + tag + ">");
                        break;

                    case BlockTag.IndentedCode:
                        writer.EnsureLine();
                        writer.Write("<pre><code>");
                        EscapeHtml(block.StringContent, writer);
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.FencedCode:
                        writer.EnsureLine();
                        writer.Write("<pre><code");
                        if (block.FencedCodeData.Info.Length > 0)
                        {
                            string[] info_words = block.FencedCodeData.Info.Split(new[] { ' ' });
                            writer.Write(" class=\"language-");
                            EscapeHtml(info_words[0], writer);
                            writer.Write("\"");
                        }
                        writer.Write(">");
                        EscapeHtml(block.StringContent, writer);
                        writer.WriteLine("</code></pre>");
                        break;

                    case BlockTag.HtmlBlock:
                        block.StringContent.WriteTo(writer);
                        break;

                    case BlockTag.HorizontalRuler:
                        writer.WriteLine("<hr />");
                        break;

                    case BlockTag.ReferenceDefinition:
                        break;

                    default:
                        throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                }

                if (visitChildren)
                {
                    stack.Push(new BlockStackEntry(stackLiteral, block.NextSibling, tight));

                    tight = stackTight;
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

                    writer.Write(entry.Literal);
                    tight = entry.IsTight;
                    block = entry.Target;
                }
            }
        }

        /// <summary>
        /// Writes the inline list to the given writer as plain text (without any HTML tags).
        /// </summary>
        /// <seealso cref="https://github.com/jgm/CommonMark/issues/145"/>
        private static void InlinesToPlainText(HtmlTextWriter writer, Inline inline, CommonMarkSettings settings, Stack<InlineStackEntry> stack)
        {
            bool withinLink = false;
            bool stackWithinLink = false; 
            bool visitChildren;
            string stackLiteral = null;
            var origStackCount = stack.Count;

            while (inline != null)
            {
                visitChildren = false;

                switch (inline.Tag)
                {
                    case InlineTag.String:
                    case InlineTag.Code:
                    case InlineTag.RawHtml:
                        EscapeHtml(inline.LiteralContent, writer);
                        break;

                    case InlineTag.LineBreak:
                    case InlineTag.SoftBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.Link:
                        if (withinLink)
                        {
                            writer.Write('[');
                            stackLiteral = "]";
                            visitChildren = true;
                            stackWithinLink = withinLink;
                        }
                        else
                        {
                            visitChildren = true;
                            stackWithinLink = true;
                            stackLiteral = string.Empty;
                        }
                        break;

                    case InlineTag.Image:
                        visitChildren = true;
                        stackWithinLink = true;
                        stackLiteral = string.Empty;
                        break;

                    case InlineTag.Strong:
                    case InlineTag.Emphasis:
                        stackLiteral = string.Empty;
                        stackWithinLink = withinLink;
                        visitChildren = true;
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                if (visitChildren)
                {
                    stack.Push(new InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

                    withinLink = stackWithinLink;
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

                while (inline == null && stack.Count > origStackCount)
                {
                    var entry = stack.Pop();
                    writer.Write(entry.Literal);
                    inline = entry.Target;
                    withinLink = entry.IsWithinLink;
                }
            }
        }

        /// <summary>
        /// Writes the inline list to the given writer as HTML code. 
        /// </summary>
        private static void InlinesToHtml(HtmlTextWriter writer, Inline inline, CommonMarkSettings settings, Stack<InlineStackEntry> stack)
        {
            var uriResolver = settings.UriResolver;
            bool withinLink = false;
            bool stackWithinLink = false;
            bool visitChildren;
            string stackLiteral = null;

            while (inline != null)
            {
                visitChildren = false;

                switch (inline.Tag)
                {
                    case InlineTag.String:
                        EscapeHtml(inline.LiteralContent, writer);
                        break;

                    case InlineTag.LineBreak:
                        writer.WriteLine("<br />");
                        break;

                    case InlineTag.SoftBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.Code:
                        writer.Write("<code>");
                        EscapeHtml(inline.LiteralContent, writer);
                        writer.Write("</code>");
                        break;

                    case InlineTag.RawHtml:
                        writer.Write(inline.LiteralContent);
                        break;

                    case InlineTag.Link:
                        if (withinLink)
                        {
                            writer.Write('[');
                            stackLiteral = "]";
                            stackWithinLink = withinLink;
                            visitChildren = true;
                        }
                        else
                        {
                            writer.Write("<a href=\"");
                            if (uriResolver != null)
                                EscapeUrl(uriResolver(inline.Linkable.Url), writer);
                            else
                                EscapeUrl(inline.Linkable.Url, writer);

                            writer.Write('\"');
                            if (!string.IsNullOrEmpty(inline.Linkable.Title))
                            {
                                writer.Write(" title=\"");
                                EscapeHtml(inline.Linkable.Title, writer);
                                writer.Write('\"');
                            }

                            writer.Write('>');

                            visitChildren = true;
                            stackWithinLink = true;
                            stackLiteral = "</a>";
                        }
                        break;

                    case InlineTag.Image:
                        writer.Write("<img src=\"");
                        if (uriResolver != null)
                            EscapeUrl(uriResolver(inline.Linkable.Url), writer);
                        else
                            EscapeUrl(inline.Linkable.Url, writer);

                        writer.Write("\" alt=\"");
                        InlinesToPlainText(writer, inline.FirstChild, settings, stack);
                        writer.Write("\"");
                        if (inline.Linkable.Title.Length > 0)
                        {
                            writer.Write(" title=\"");
                            EscapeHtml(inline.Linkable.Title, writer);
                            writer.Write("\"");
                        }
                        writer.Write(" />");
                        break;

                    case InlineTag.Strong:
                        writer.Write("<strong>");
                        stackLiteral = "</strong>";
                        stackWithinLink = withinLink;
                        visitChildren = true;
                        break;

                    case InlineTag.Emphasis:
                        writer.Write("<em>");
                        stackLiteral = "</em>";
                        visitChildren = true;
                        stackWithinLink = withinLink;
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                if (visitChildren)
                {
                    stack.Push(new InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

                    withinLink = stackWithinLink;
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
                    withinLink = entry.IsWithinLink;
                }
            }
        }

        private struct BlockStackEntry
        {
            public readonly string Literal;
            public readonly Block Target;
            public readonly bool IsTight;
            public BlockStackEntry(string literal, Block target, bool isTight)
            {
                this.Literal = literal;
                this.Target = target;
                this.IsTight = isTight;
            }
        }
        private struct InlineStackEntry
        {
            public readonly string Literal;
            public readonly Inline Target;
            public readonly bool IsWithinLink;
            public InlineStackEntry(string literal, Inline target, bool isWithinLink)
            {
                this.Literal = literal;
                this.Target = target;
                this.IsWithinLink = isWithinLink;
            }
        }
    }
}
