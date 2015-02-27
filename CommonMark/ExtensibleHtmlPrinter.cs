using CommonMark.Syntax;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CommonMark.Formatter
{
    public class ExtensibleHtmlPrinter : IPrinter
    {
        private static readonly char[] EscapeHtmlCharacters = new[] { '&', '<', '>', '"' };
        private const string HexCharacters = "0123456789ABCDEF";

        private static readonly char[] EscapeHtmlLessThan = "&lt;".ToCharArray();
        private static readonly char[] EscapeHtmlGreaterThan = "&gt;".ToCharArray();
        private static readonly char[] EscapeHtmlAmpersand = "&amp;".ToCharArray();
        private static readonly char[] EscapeHtmlQuote = "&quot;".ToCharArray();

        private static readonly string[] HeaderOpenerTags = new[] { "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>" };
        private static readonly string[] HeaderCloserTags = new[] { "</h1>", "</h2>", "</h3>", "</h4>", "</h5>", "</h6>" };

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

        private bool _trackPosition;
        private HtmlTextWriter _htmlTextWriter;
        private Stack<HtmlPrinter.BlockStackEntry> _stack = new Stack<HtmlPrinter.BlockStackEntry>();
        private Stack<HtmlPrinter.InlineStackEntry> _inlineStack = new Stack<HtmlPrinter.InlineStackEntry>();
        private bool _tight = false;
        private bool _stackTight = false;
        private bool _visitChildren;
        private string _stackLiteral = null;

        protected void Write(char value)
        {
            _htmlTextWriter.Write(value);
        }

        protected void Write(char[] value, int index, int count)
        {
            _htmlTextWriter.Write(value, index, count);
        }

        private void Write(StringPart value)
        {
            _htmlTextWriter.Write(value);
        }

        protected void WriteConstant(string value)
        {
            _htmlTextWriter.WriteConstant(value);
        }

        protected void WriteConstant(char[] value, int index, int count)
        {
            _htmlTextWriter.WriteConstant(value, index, count);
        }

        protected void WriteLineConstant(string value)
        {
            _htmlTextWriter.WriteLineConstant(value);
        }

        protected void EnsureLine()
        {
            _htmlTextWriter.EnsureLine();
        }

        /// <summary>
        /// Escapes special URL characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        protected void EscapeUrl(string input)
        {
            if (input == null)
                return;

            char c;
            int lastPos = 0;
            int len = input.Length;
            char[] buffer;

            if (_htmlTextWriter.Buffer.Length < len)
                buffer = _htmlTextWriter.Buffer = input.ToCharArray();
            else
            {
                buffer = _htmlTextWriter.Buffer;
                input.CopyTo(0, buffer, 0, len);
            }

            // since both \r and \n are not url-safe characters and will be encoded, all calls are
            // made to WriteConstant.
            for (var pos = 0; pos < len; pos++)
            {
                c = buffer[pos];

                if (c == '&')
                {
                    _htmlTextWriter.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;
                    _htmlTextWriter.WriteConstant(EscapeHtmlAmpersand);
                }
                else if (c < 128 && !UrlSafeCharacters[c])
                {
                    _htmlTextWriter.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    _htmlTextWriter.WriteConstant(new[] { '%', HexCharacters[c / 16], HexCharacters[c % 16] });
                }
                else if (c > 127)
                {
                    _htmlTextWriter.WriteConstant(buffer, lastPos, pos - lastPos);
                    lastPos = pos + 1;

                    byte[] bytes;
                    if (c >= '\ud800' && c <= '\udfff' && len != lastPos)
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
                        _htmlTextWriter.WriteConstant(new[] { '%', HexCharacters[bytes[i] / 16], HexCharacters[bytes[i] % 16] });
                }
            }

            _htmlTextWriter.WriteConstant(buffer, lastPos, len - lastPos);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        private void EscapeHtml(StringPart input)
        {
            if (input.Length == 0)
                return;

            int pos;
            int lastPos = input.StartIndex;
            char[] buffer;

            if (_htmlTextWriter.Buffer.Length < input.Length)
                buffer = _htmlTextWriter.Buffer = new char[input.Length];
            else
                buffer = _htmlTextWriter.Buffer;

            input.Source.CopyTo(input.StartIndex, buffer, 0, input.Length);

            while ((pos = input.Source.IndexOfAny(EscapeHtmlCharacters, lastPos, input.Length - lastPos + input.StartIndex)) != -1)
            {
                _htmlTextWriter.Write(buffer, lastPos - input.StartIndex, pos - lastPos);
                lastPos = pos + 1;

                switch (input.Source[pos])
                {
                    case '<':
                        _htmlTextWriter.WriteConstant(EscapeHtmlLessThan);
                        break;

                    case '>':
                        _htmlTextWriter.WriteConstant(EscapeHtmlGreaterThan);
                        break;

                    case '&':
                        _htmlTextWriter.WriteConstant(EscapeHtmlAmpersand);
                        break;

                    case '"':
                        _htmlTextWriter.WriteConstant(EscapeHtmlQuote);
                        break;
                }
            }

            _htmlTextWriter.Write(buffer, lastPos - input.StartIndex, input.Length - lastPos + input.StartIndex);
        }

        /// <summary>
        /// Escapes special HTML characters.
        /// </summary>
        /// <remarks>Orig: escape_html(inp, preserve_entities)</remarks>
        protected void EscapeHtml(StringContent inp)
        {
            int pos;
            int lastPos;
            char[] buffer = _htmlTextWriter.Buffer;

            var parts = inp.RetrieveParts();
            for (var i = parts.Offset; i < parts.Offset + parts.Count; i++)
            {
                var part = parts.Array[i];

                if (buffer.Length < part.Length)
                    buffer = _htmlTextWriter.Buffer = new char[part.Length];

                part.Source.CopyTo(part.StartIndex, buffer, 0, part.Length);

                lastPos = pos = part.StartIndex;
                while ((pos = part.Source.IndexOfAny(EscapeHtmlCharacters, lastPos, part.Length - lastPos + part.StartIndex)) != -1)
                {
                    _htmlTextWriter.Write(buffer, lastPos - part.StartIndex, pos - lastPos);
                    lastPos = pos + 1;

                    switch (part.Source[pos])
                    {
                        case '<':
                            _htmlTextWriter.WriteConstant(EscapeHtmlLessThan);
                            break;

                        case '>':
                            _htmlTextWriter.WriteConstant(EscapeHtmlGreaterThan);
                            break;

                        case '&':
                            _htmlTextWriter.WriteConstant(EscapeHtmlAmpersand);
                            break;

                        case '"':
                            _htmlTextWriter.WriteConstant(EscapeHtmlQuote);
                            break;
                    }
                }

                _htmlTextWriter.Write(buffer, lastPos - part.StartIndex, part.Length - lastPos + part.StartIndex);
            }
        }

        public void Print(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            _trackPosition = settings.TrackSourcePosition;
            _htmlTextWriter = new HtmlTextWriter(writer);
            BlocksToHtml(writer, block, settings);
        }

        private void PrintPosition(Block block)
        {
            WriteConstant(" data-sourcepos=\"");
            WriteConstant(block.SourcePosition.ToString(CultureInfo.InvariantCulture));
            Write('-');
            WriteConstant(block.SourceLastPosition.ToString(CultureInfo.InvariantCulture));
            WriteConstant("\"");
        }

        private void PrintPosition(Inline inline)
        {
            WriteConstant(" data-sourcepos=\"");
            WriteConstant(inline.SourcePosition.ToString(CultureInfo.InvariantCulture));
            Write('-');
            WriteConstant(inline.SourceLastPosition.ToString(CultureInfo.InvariantCulture));
            WriteConstant("\"");
        }

        private void BlocksToHtml(TextWriter writer, Block block, CommonMarkSettings settings)
        {

            int x;

            while (block != null)
            {
                _visitChildren = false;

                switch (block.Tag)
                {
                    case BlockTag.Document:
                        PrintDocument();
                        break;

                    case BlockTag.Paragraph:
                        PrintParagraph(writer, block, settings);
                        break;

                    case BlockTag.BlockQuote:
                        PrintBlockQuotes(writer, block);
                        break;

                    case BlockTag.ListItem:
                        PrintListItem(block);
                        break;

                    case BlockTag.List:
                        PrintList(writer, block);
                        break;

                    case BlockTag.AtxHeader:
                    case BlockTag.SETextHeader:
                        PrintHeader(writer, block, settings);
                        break;

                    case BlockTag.IndentedCode:
                    case BlockTag.FencedCode:
                        PrintCode(writer, block);
                        break;

                    case BlockTag.HtmlBlock:
                        PrintHtmlBlock(writer, block);
                        break;

                    case BlockTag.HorizontalRuler:
                        PrintHorizontalRuler(writer, block);
                        break;

                    case BlockTag.ReferenceDefinition:
                        break;

                    default:
                        throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                }

                if (_visitChildren)
                {
                    _stack.Push(new HtmlPrinter.BlockStackEntry(_stackLiteral, block.NextSibling, _tight));

                    _tight = _stackTight;
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

                while (block == null && _stack.Count > 0)
                {
                    var entry = _stack.Pop();

                    WriteLineConstant(entry.Literal);
                    _tight = entry.IsTight;
                    block = entry.Target;
                }
            }
        }

        protected virtual void PrintHorizontalRuler(TextWriter writer, Block block)
        {
            if (_trackPosition)
            {
                WriteConstant("<hr");
                PrintPosition(block);
                writer.WriteLine();
            }
            else
            {
                WriteLineConstant("<hr />");
            }
        }

        protected virtual void PrintHtmlBlock(TextWriter writer, Block block)
        {
            // cannot output source position for HTML blocks
            block.StringContent.WriteTo(writer);
        }

        protected virtual void PrintCode(TextWriter writer, Block block)
        {
            int x;
            EnsureLine();
            WriteConstant("<pre><code");
            if (_trackPosition) PrintPosition(block);

            var info = block.FencedCodeData == null ? null : block.FencedCodeData.Info;
            if (info != null && info.Length > 0)
            {
                x = info.IndexOf(' ');
                if (x == -1)
                    x = info.Length;

                WriteConstant(" class=\"language-");
                EscapeHtml(new StringPart(info, 0, x));
                writer.Write('\"');
            }
            Write('>');
            EscapeHtml(block.StringContent);
            WriteLineConstant("</code></pre>");
        }

        protected virtual void PrintHeader(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            int x;
            EnsureLine();

            x = block.HeaderLevel;

            if (_trackPosition)
            {
                WriteConstant("<h" + x.ToString(CultureInfo.InvariantCulture));
                PrintPosition(block);
                InlinesToHtml(writer, block.InlineContent, settings);
                WriteLineConstant(x > 0 && x < 7 ? HeaderCloserTags[x - 1] : "</h" + x.ToString(CultureInfo.InvariantCulture) + ">");
            }
            else
            {
                WriteConstant(x > 0 && x < 7 ? HeaderOpenerTags[x - 1] : "<h" + x.ToString(CultureInfo.InvariantCulture) + ">");
                InlinesToHtml(writer, block.InlineContent, settings);
                WriteLineConstant(x > 0 && x < 7 ? HeaderCloserTags[x - 1] : "</h" + x.ToString(CultureInfo.InvariantCulture) + ">");
            }
        }

        protected virtual void PrintList(TextWriter writer, Block block)
        {
            // make sure a list starts at the beginning of the line:
            EnsureLine();
            var data = block.ListData;
            WriteConstant(data.ListType == ListType.Bullet ? "<ul" : "<ol");
            if (data.Start != 1)
            {
                WriteConstant(" start=\"");
                WriteConstant(data.Start.ToString(CultureInfo.InvariantCulture));
                Write('\"');
            }
            if (_trackPosition) PrintPosition(block);
            writer.WriteLine('>');

            _stackLiteral = data.ListType == ListType.Bullet ? "</ul>" : "</ol>";
            _stackTight = data.IsTight;
            _visitChildren = true;
        }

        protected virtual void PrintListItem(Block block)
        {
            EnsureLine();
            WriteConstant("<li");
            if (_trackPosition) PrintPosition(block);
            Write('>');

            _stackLiteral = "</li>";
            _stackTight = _tight;
            _visitChildren = true;
        }

        protected virtual void PrintBlockQuotes(TextWriter writer, Block block)
        {
            EnsureLine();
            WriteConstant("<blockquote");
            if (_trackPosition) PrintPosition(block);
            writer.WriteLine('>');

            _stackLiteral = "</blockquote>";
            _stackTight = false;
            _visitChildren = true;
        }

        protected virtual void PrintDocument()
        {
            _stackLiteral = null;
            _stackTight = false;
            _visitChildren = true;
        }

        protected virtual void PrintParagraph(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            if (_tight)
            {
                InlinesToHtml(writer, block.InlineContent, settings);
            }
            else
            {
                EnsureLine();
                WriteConstant("<p");
                if (_trackPosition) PrintPosition(block);
                this.Write('>');
                InlinesToHtml(writer, block.InlineContent, settings);
                WriteLineConstant("</p>");
            }
        }


        /// <summary>
        /// Writes the inline list to the given writer as plain text (without any HTML tags).
        /// </summary>
        /// <seealso href="https://github.com/jgm/CommonMark/issues/145"/>
        protected void InlinesToPlainText(TextWriter writer, Inline inline)
        {
            bool withinLink = false;
            bool stackWithinLink = false;
            bool visitChildren;
            string stackLiteral = null;
            var origStackCount = _inlineStack.Count;

            while (inline != null)
            {
                visitChildren = false;

                switch (inline.Tag)
                {
                    case InlineTag.String:
                    case InlineTag.Code:
                    case InlineTag.RawHtml:
                        EscapeHtml(inline.LiteralContentValue);
                        break;

                    case InlineTag.LineBreak:
                    case InlineTag.SoftBreak:
                        writer.WriteLine();
                        break;

                    case InlineTag.Link:
                        if (withinLink)
                        {
                            Write('[');
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
                    _inlineStack.Push(new HtmlPrinter.InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

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

                while (inline == null && _inlineStack.Count > origStackCount)
                {
                    var entry = _inlineStack.Pop();
                    WriteConstant(entry.Literal);
                    inline = entry.Target;
                    withinLink = entry.IsWithinLink;
                }
            }
        }

        /// <summary>
        /// Writes the inline list to the given writer as HTML code.
        /// </summary>
        protected void InlinesToHtml(TextWriter writer, Inline inline, CommonMarkSettings settings)
        {
            var uriResolver = settings.UriResolver;
            bool withinLink = false;
            bool stackWithinLink = false;
            bool visitChildren;
            bool trackPositions = settings.TrackSourcePosition;
            string stackLiteral = null;

            while (inline != null)
            {
                visitChildren = false;

                switch (inline.Tag)
                {
                    case InlineTag.String:
                        if (trackPositions)
                        {
                            WriteConstant("<span");
                            PrintPosition(inline);
                            Write('>');
                            EscapeHtml(inline.LiteralContentValue);
                            WriteConstant("</span>");
                        }
                        else
                        {
                            EscapeHtml(inline.LiteralContentValue);
                        }

                        break;

                    case InlineTag.LineBreak:
                        WriteLineConstant("<br />");
                        break;

                    case InlineTag.SoftBreak:
                        if (settings.RenderSoftLineBreaksAsLineBreaks)
                            WriteLineConstant("<br />");
                        else
                            writer.WriteLine();
                        break;

                    case InlineTag.Code:
                        WriteConstant("<code");
                        if (trackPositions) PrintPosition(inline);
                        Write('>');
                        EscapeHtml(inline.LiteralContentValue);
                        WriteConstant("</code>");
                        break;

                    case InlineTag.RawHtml:
                        // cannot output source position for HTML blocks
                        Write(inline.LiteralContentValue);
                        break;

                    case InlineTag.Link:
                        if (withinLink)
                        {
                            Write('[');
                            stackLiteral = "]";
                            stackWithinLink = withinLink;
                            visitChildren = true;
                        }
                        else
                        {
                            WriteConstant("<a href=\"");
                            if (uriResolver != null)
                                EscapeUrl(uriResolver(inline.TargetUrl));
                            else
                                EscapeUrl(inline.TargetUrl);

                            Write('\"');
                            if (inline.LiteralContentValue.Length > 0)
                            {
                                WriteConstant(" title=\"");
                                EscapeHtml(inline.LiteralContentValue);
                                Write('\"');
                            }

                            if (trackPositions) PrintPosition(inline);

                            Write('>');

                            visitChildren = true;
                            stackWithinLink = true;
                            stackLiteral = "</a>";
                        }
                        break;

                    case InlineTag.Image:
                        WriteConstant("<img src=\"");
                        if (uriResolver != null)
                            EscapeUrl(uriResolver(inline.TargetUrl));
                        else
                            EscapeUrl(inline.TargetUrl);

                        WriteConstant("\" alt=\"");
                        InlinesToPlainText(writer, inline.FirstChild);
                        Write('\"');
                        if (inline.LiteralContentValue.Length > 0)
                        {
                            WriteConstant(" title=\"");
                            EscapeHtml(inline.LiteralContentValue);
                            Write('\"');
                        }

                        if (trackPositions) PrintPosition( inline);
                        WriteConstant(" />");

                        break;

                    case InlineTag.Strong:
                        WriteConstant("<strong");
                        if (trackPositions) PrintPosition(inline);
                        Write('>');
                        stackLiteral = "</strong>";
                        stackWithinLink = withinLink;
                        visitChildren = true;
                        break;

                    case InlineTag.Emphasis:
                        WriteConstant("<em");
                        if (trackPositions) PrintPosition(inline);
                        Write('>');
                        stackLiteral = "</em>";
                        visitChildren = true;
                        stackWithinLink = withinLink;
                        break;

                    case InlineTag.Strikethrough:
                        WriteConstant("<del");
                        if (trackPositions) PrintPosition( inline);
                        Write('>');
                        stackLiteral = "</del>";
                        visitChildren = true;
                        stackWithinLink = withinLink;
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                if (visitChildren)
                {
                    _inlineStack.Push(new HtmlPrinter.InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

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

                while (inline == null && _inlineStack.Count > 0)
                {
                    var entry = _inlineStack.Pop();
                    WriteConstant(entry.Literal);
                    inline = entry.Target;
                    withinLink = entry.IsWithinLink;
                }
            }
        }
    }
}
