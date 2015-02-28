using CommonMark.Syntax;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CommonMark.Formatter
{
    public class ExtensibleHtmlBlockWriter : IBlockWriter
    {
        private bool _trackPosition;
        private HtmlTextWriter _htmlTextWriter;
        private Stack<HtmlBlockWriter.BlockStackEntry> _stack = new Stack<HtmlBlockWriter.BlockStackEntry>();
        private Stack<HtmlBlockWriter.InlineStackEntry> _inlineStack = new Stack<HtmlBlockWriter.InlineStackEntry>();
        private bool _tight = false;
        private bool _stackTight = false;
        private bool _visitChildren;
        private string _stackLiteral = null;

        public CommonMarkSettings Settings { get; private set; }

        protected void WriteLine()
        {
            _htmlTextWriter.WriteLine();
        }

        protected void WriteLine(char value)
        {
            _htmlTextWriter.WriteLine(value);
        }

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

        protected void WriteEscapeUrl(string input)
        {
            HtmlBlockWriter.EscapeUrl(input, _htmlTextWriter);
        }

        protected void WriteEscapeHtml(StringContent input)
        {
            HtmlBlockWriter.EscapeHtml(input, _htmlTextWriter);
        }

        protected void WriteEscapeHtml(string input)
        {
            WriteEscapeHtml(input, 0, input.Length);
        }

        protected void WriteEscapeHtml(string input, int startIndex, int length)
        {
            HtmlBlockWriter.EscapeHtml(new StringPart(input, startIndex, length), _htmlTextWriter);
        }

        public void Write(TextWriter writer, Block block, CommonMarkSettings settings)
        {
            _trackPosition = settings.TrackSourcePosition;
            _htmlTextWriter = new HtmlTextWriter(writer);
            Settings = settings;
            BlocksToHtml(block);
        }

        private void WritePosition(Block block)
        {
            WriteConstant(" data-sourcepos=\"");
            WriteConstant(block.SourcePosition.ToString(CultureInfo.InvariantCulture));
            Write('-');
            WriteConstant(block.SourceLastPosition.ToString(CultureInfo.InvariantCulture));
            WriteConstant("\"");
        }

        private void WritePosition(Inline inline)
        {
            WriteConstant(" data-sourcepos=\"");
            WriteConstant(inline.SourcePosition.ToString(CultureInfo.InvariantCulture));
            Write('-');
            WriteConstant(inline.SourceLastPosition.ToString(CultureInfo.InvariantCulture));
            WriteConstant("\"");
        }

        private void BlocksToHtml(Block block)
        {
            while (block != null)
            {
                _visitChildren = false;

                switch (block.Tag)
                {
                    case BlockTag.Document:
                        WriteDocument();
                        break;

                    case BlockTag.Paragraph:
                        WriteParagraph(block);
                        break;

                    case BlockTag.BlockQuote:
                        WriteBlockQuotes(block);
                        break;

                    case BlockTag.ListItem:
                        WriteListItem(block);
                        break;

                    case BlockTag.List:
                        WriteList(block);
                        break;

                    case BlockTag.AtxHeader:
                    case BlockTag.SETextHeader:
                        WriteHeader(block);
                        break;

                    case BlockTag.IndentedCode:
                    case BlockTag.FencedCode:
                        WriteCode(block);
                        break;

                    case BlockTag.HtmlBlock:
                        WriteHtmlBlock(block);
                        break;

                    case BlockTag.HorizontalRuler:
                        WriteHorizontalRuler(block);
                        break;

                    case BlockTag.ReferenceDefinition:
                        break;

                    default:
                        throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                }

                if (_visitChildren)
                {
                    _stack.Push(new HtmlBlockWriter.BlockStackEntry(_stackLiteral, block.NextSibling, _tight));

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

        protected virtual void WriteHorizontalRuler(Block block)
        {
            if (_trackPosition)
            {
                WriteConstant("<hr");
                WritePosition(block);
                WriteLine();
            }
            else
            {
                WriteLineConstant("<hr />");
            }
        }

        protected virtual void WriteHtmlBlock(Block block)
        {
            // cannot output source position for HTML blocks
            block.StringContent.WriteTo(_htmlTextWriter);
        }

        protected virtual void WriteCode(Block block)
        {
            int x;
            EnsureLine();
            WriteConstant("<pre><code");
            if (_trackPosition) WritePosition(block);

            var info = block.FencedCodeData == null ? null : block.FencedCodeData.Info;
            if (info != null && info.Length > 0)
            {
                x = info.IndexOf(' ');
                if (x == -1)
                    x = info.Length;

                WriteConstant(" class=\"language-");
                WriteEscapeHtml(info, 0, x);
                Write('\"');
            }
            Write('>');
            WriteEscapeHtml(block.StringContent);
            WriteLineConstant("</code></pre>");
        }

        protected virtual void WriteHeader(Block block)
        {
            int x;
            EnsureLine();

            x = block.HeaderLevel;

            if (_trackPosition)
            {
                WriteConstant("<h" + x.ToString(CultureInfo.InvariantCulture));
                WritePosition(block);
                InlinesToHtml(block.InlineContent);
                WriteLineConstant(x > 0 && x < 7 ? HtmlBlockWriter.HeaderCloserTags[x - 1] : "</h" + x.ToString(CultureInfo.InvariantCulture) + ">");
            }
            else
            {
                WriteConstant(x > 0 && x < 7 ? HtmlBlockWriter.HeaderOpenerTags[x - 1] : "<h" + x.ToString(CultureInfo.InvariantCulture) + ">");
                InlinesToHtml(block.InlineContent);
                WriteLineConstant(x > 0 && x < 7 ? HtmlBlockWriter.HeaderCloserTags[x - 1] : "</h" + x.ToString(CultureInfo.InvariantCulture) + ">");
            }
        }

        protected virtual void WriteList(Block block)
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
            if (_trackPosition) WritePosition(block);
            WriteLine('>');

            _stackLiteral = data.ListType == ListType.Bullet ? "</ul>" : "</ol>";
            _stackTight = data.IsTight;
            _visitChildren = true;
        }

        protected virtual void WriteListItem(Block block)
        {
            EnsureLine();
            WriteConstant("<li");
            if (_trackPosition) WritePosition(block);
            Write('>');

            _stackLiteral = "</li>";
            _stackTight = _tight;
            _visitChildren = true;
        }

        protected virtual void WriteBlockQuotes(Block block)
        {
            EnsureLine();
            WriteConstant("<blockquote");
            if (_trackPosition) WritePosition(block);
            WriteLine('>');

            _stackLiteral = "</blockquote>";
            _stackTight = false;
            _visitChildren = true;
        }

        protected virtual void WriteDocument()
        {
            _stackLiteral = null;
            _stackTight = false;
            _visitChildren = true;
        }

        protected virtual void WriteParagraph(Block block)
        {
            if (_tight)
            {
                InlinesToHtml(block.InlineContent);
            }
            else
            {
                EnsureLine();
                WriteConstant("<p");
                if (_trackPosition) WritePosition(block);
                this.Write('>');
                InlinesToHtml(block.InlineContent);
                WriteLineConstant("</p>");
            }
        }


        /// <summary>
        /// Writes the inline list to the given writer as plain text (without any HTML tags).
        /// </summary>
        /// <seealso href="https://github.com/jgm/CommonMark/issues/145"/>
        private void InlinesToPlainText(Inline inline)
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
                        HtmlBlockWriter.EscapeHtml(inline.LiteralContentValue, _htmlTextWriter);
                        break;

                    case InlineTag.LineBreak:
                    case InlineTag.SoftBreak:
                        WriteLine();
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
                    _inlineStack.Push(new HtmlBlockWriter.InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

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
        protected void InlinesToHtml(Inline inline)
        {
            var uriResolver = Settings.UriResolver;
            bool withinLink = false;
            bool stackWithinLink = false;
            bool visitChildren;
            bool trackPositions = Settings.TrackSourcePosition;
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
                            WritePosition(inline);
                            Write('>');
                            HtmlBlockWriter.EscapeHtml(inline.LiteralContentValue, _htmlTextWriter);
                            WriteConstant("</span>");
                        }
                        else
                        {
                            HtmlBlockWriter.EscapeHtml(inline.LiteralContentValue, _htmlTextWriter);
                        }

                        break;

                    case InlineTag.LineBreak:
                        WriteLineConstant("<br />");
                        break;

                    case InlineTag.SoftBreak:
                        if (Settings.RenderSoftLineBreaksAsLineBreaks)
                            WriteLineConstant("<br />");
                        else
                            WriteLine();
                        break;

                    case InlineTag.Code:
                        WriteConstant("<code");
                        if (trackPositions) WritePosition(inline);
                        Write('>');
                        HtmlBlockWriter.EscapeHtml(inline.LiteralContentValue, _htmlTextWriter);
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
                                WriteEscapeUrl(uriResolver(inline.TargetUrl));
                            else
                                WriteEscapeUrl(inline.TargetUrl);

                            Write('\"');
                            if (inline.LiteralContentValue.Length > 0)
                            {
                                WriteConstant(" title=\"");
                                HtmlBlockWriter.EscapeHtml(inline.LiteralContentValue, _htmlTextWriter);
                                Write('\"');
                            }

                            if (trackPositions) WritePosition(inline);

                            Write('>');

                            visitChildren = true;
                            stackWithinLink = true;
                            stackLiteral = "</a>";
                        }
                        break;

                    case InlineTag.Image:
                        WriteConstant("<img src=\"");
                        if (uriResolver != null)
                            WriteEscapeUrl(uriResolver(inline.TargetUrl));
                        else
                            WriteEscapeUrl(inline.TargetUrl);

                        WriteConstant("\" alt=\"");
                        InlinesToPlainText(inline.FirstChild);
                        Write('\"');
                        if (inline.LiteralContentValue.Length > 0)
                        {
                            WriteConstant(" title=\"");
                            HtmlBlockWriter.EscapeHtml(inline.LiteralContentValue, _htmlTextWriter);
                            Write('\"');
                        }

                        if (trackPositions) WritePosition( inline);
                        WriteConstant(" />");

                        break;

                    case InlineTag.Strong:
                        WriteConstant("<strong");
                        if (trackPositions) WritePosition(inline);
                        Write('>');
                        stackLiteral = "</strong>";
                        stackWithinLink = withinLink;
                        visitChildren = true;
                        break;

                    case InlineTag.Emphasis:
                        WriteConstant("<em");
                        if (trackPositions) WritePosition(inline);
                        Write('>');
                        stackLiteral = "</em>";
                        visitChildren = true;
                        stackWithinLink = withinLink;
                        break;

                    case InlineTag.Strikethrough:
                        WriteConstant("<del");
                        if (trackPositions) WritePosition( inline);
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
                    _inlineStack.Push(new HtmlBlockWriter.InlineStackEntry(stackLiteral, inline.NextSibling, withinLink));

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
