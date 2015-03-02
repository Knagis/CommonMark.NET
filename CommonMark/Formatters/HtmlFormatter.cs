using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CommonMark.Formatters
{
    /// <summary>
    /// An extendable implementation for writing CommonMark data as HTML.
    /// </summary>
    public class HtmlFormatter
    {
        private readonly HtmlTextWriter _target;
        private readonly CommonMarkSettings _settings;
        private readonly Stack<bool> _renderTightParagraphs = new Stack<bool>(new bool[] { false });
        private readonly Stack<bool> _renderPlainTextInlines = new Stack<bool>(new bool[] { false });

        /// <summary>
        /// Gets a stack of values indicating whether the paragraph tags should be ommitted.
        /// Every element that impacts this setting has to push a value when opening and pop it when closing.
        /// The most recent value is used to determine the current state.
        /// </summary>
        protected Stack<bool> RenderTightParagraphs { get { return this._renderTightParagraphs; } }

        /// <summary>
        /// Gets a stack of values indicating whether the inline elements should be rendered as plain text
        /// (without formatting). This usually is done within image description attributes that do not support
        /// HTML tags.
        /// Every element that impacts this setting has to push a value when opening and pop it when closing.
        /// The most recent value is used to determine the current state.
        /// </summary>
        protected Stack<bool> RenderPlainTextInlines { get { return this._renderPlainTextInlines; } }

        /// <summary>Initializes a new instance of the <see cref="HtmlFormatter" /> class.</summary>
        /// <param name="target">The target text writer.</param>
        /// <param name="settings">The settings used when formatting the data.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="target"/> is <c>null</c></exception>
        public HtmlFormatter(TextWriter target, CommonMarkSettings settings)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (settings == null)
                settings = CommonMarkSettings.Default;

            this._target = new HtmlTextWriter(target);
            this._settings = settings;
        }

        /// <summary>
        /// Gets the settings used for formatting data.
        /// </summary>
        protected CommonMarkSettings Settings { get { return this._settings; } }

        /// <summary>
        /// Writes the given CommonMark document to the output stream as HTML.
        /// </summary>
        public void WriteDocument(Block document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            bool ignoreChildNodes;
            Block ignoreUntilBlockCloses = null;
            Inline ignoreUntilInlineCloses = null;

            foreach (var node in document.AsEnumerable())
            {
                if (node.Block != null)
                {
                    if (ignoreUntilBlockCloses != null)
                    {
                        if (ignoreUntilBlockCloses != node.Block)
                            continue;

                        ignoreUntilBlockCloses = null;
                    }

                    this.WriteBlock(node.Block, node.IsOpening, node.IsClosing, out ignoreChildNodes);
                    if (ignoreChildNodes && !node.IsClosing)
                        ignoreUntilBlockCloses = node.Block;
                }
                else if (ignoreUntilBlockCloses == null && node.Inline != null)
                {
                    if (ignoreUntilInlineCloses != null)
                    {
                        if (ignoreUntilInlineCloses != node.Inline)
                            continue;

                        ignoreUntilInlineCloses = null;
                    }

                    this.WriteInline(node.Inline, node.IsOpening, node.IsClosing, out ignoreChildNodes);
                    if (ignoreChildNodes && !node.IsClosing)
                        ignoreUntilBlockCloses = node.Block;
                }
            }
        }

        /// <summary>
        /// Writes the specified block element to the output stream. Does not write the child nodes, instead
        /// the <paramref name="ignoreChildNodes"/> is used to notify the caller whether it should recurse
        /// into the child nodes.
        /// </summary>
        /// <param name="block">The block element to be written to the output stream.</param>
        /// <param name="isOpening">Specifies whether the block element is being opened (or started).</param>
        /// <param name="isClosing">Specifies whether the block element is being closed. If the block does not
        /// have child nodes, then both <paramref name="isClosing"/> and <paramref name="isOpening"/> can be
        /// <c>true</c> at the same time.</param>
        /// <param name="ignoreChildNodes">Instructs the caller whether to skip processing of child nodes or not.</param>
        protected virtual void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;
            int x;

            switch (block.Tag)
            {
                case BlockTag.Document:
                    break;

                case BlockTag.Paragraph:
                    if (this.RenderTightParagraphs.Peek())
                        break;

                    if (isOpening)
                    {
                        this.EnsureNewLine();
                        this.Write("<p");
                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(block);
                        this.Write('>');
                    }

                    if (isClosing)
                        this.WriteLine("</p>");

                    break;

                case BlockTag.BlockQuote:
                    if (isOpening)
                    {
                        this.EnsureNewLine();
                        this.Write("<blockquote");
                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(block);
                        this.WriteLine(">");

                        this.RenderTightParagraphs.Push(false);
                    }

                    if (isClosing)
                    {
                        this.RenderTightParagraphs.Pop();
                        this.WriteLine("</blockquote>");
                    }

                    break;

                case BlockTag.ListItem:
                    if (isOpening)
                    {
                        this.EnsureNewLine();
                        this.Write("<li");
                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(block);
                        this.Write('>');
                    }

                    if (isClosing)
                        this.WriteLine("</li>");

                    break;

                case BlockTag.List:
                    var data = block.ListData;

                    if (isOpening)
                    {
                        this.EnsureNewLine();
                        this.Write(data.ListType == ListType.Bullet ? "<ul" : "<ol");
                        if (data.Start != 1)
                        {
                            this.Write(" start=\"");
                            this.Write(data.Start.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            this.Write('\"');
                        }
                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(block);
                        this.WriteLine(">");

                        this.RenderTightParagraphs.Push(data.IsTight);
                    }

                    if (isClosing)
                    {
                        this.WriteLine(data.ListType == ListType.Bullet ? "</ul>" : "</ol>");
                        this.RenderTightParagraphs.Pop();
                    }

                    break;

                case BlockTag.AtxHeader:
                case BlockTag.SETextHeader:

                    x = block.HeaderLevel;
                    if (isOpening)
                    {
                        this.EnsureNewLine();

                        this.Write("<h" + x.ToString(CultureInfo.InvariantCulture));
                        if (this.Settings.TrackSourcePosition)
                            this.WritePositionAttribute(block);

                        this.Write('>');
                    }

                    if (isClosing)
                        this.WriteLine("</h" + x.ToString(CultureInfo.InvariantCulture) + ">");

                    break;

                case BlockTag.IndentedCode:
                case BlockTag.FencedCode:

                    ignoreChildNodes = true;

                    this.EnsureNewLine();
                    this.Write("<pre><code");
                    if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(block);

                    var info = block.FencedCodeData == null ? null : block.FencedCodeData.Info;
                    if (info != null && info.Length > 0)
                    {
                        x = info.IndexOf(' ');
                        if (x == -1)
                            x = info.Length;

                        this.Write(" class=\"language-");
                        this.WriteEncodedHtml(new StringPart(info, 0, x));
                        this.Write('\"');
                    }
                    this.Write('>');
                    this.WriteEncodedHtml(block.StringContent);
                    this.WriteLine("</code></pre>");
                    break;

                case BlockTag.HtmlBlock:
                    ignoreChildNodes = true;
                    // cannot output source position for HTML blocks
                    this.Write(block.StringContent);

                    break;

                case BlockTag.HorizontalRuler:
                    ignoreChildNodes = true;
                    if (this.Settings.TrackSourcePosition)
                    {
                        this.Write("<hr");
                        this.WritePositionAttribute(block);
                        this.WriteLine();
                    }
                    else
                    {
                        this.WriteLine("<hr />");
                    }

                    break;

                case BlockTag.ReferenceDefinition:
                    break;

                default:
                    throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
            }

            if (ignoreChildNodes && !isClosing)
                throw new InvalidOperationException("Block of type " + block.Tag + " cannot contain child nodes.");
        }

        /// <summary>
        /// Writes the specified inline element to the output stream. Does not write the child nodes, instead
        /// the <paramref name="ignoreChildNodes"/> is used to notify the caller whether it should recurse
        /// into the child nodes.
        /// </summary>
        /// <param name="inline">The inline element to be written to the output stream.</param>
        /// <param name="isOpening">Specifies whether the inline element is being opened (or started).</param>
        /// <param name="isClosing">Specifies whether the inline element is being closed. If the inline does not
        /// have child nodes, then both <paramref name="isClosing"/> and <paramref name="isOpening"/> can be
        /// <c>true</c> at the same time.</param>
        /// <param name="ignoreChildNodes">Instructs the caller whether to skip processing of child nodes or not.</param>
        protected virtual void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            if (this.RenderPlainTextInlines.Peek())
            {
                switch (inline.Tag)
                {
                    case InlineTag.String:
                    case InlineTag.Code:
                    case InlineTag.RawHtml:
                        this.WriteEncodedHtml(inline.LiteralContentValue);
                        break;

                    case InlineTag.LineBreak:
                    case InlineTag.SoftBreak:
                        this.WriteLine();
                        break;

                    case InlineTag.Image:
                        if (isOpening)
                            this.RenderPlainTextInlines.Push(true);

                        if (isClosing)
                        {
                            this.RenderPlainTextInlines.Pop();

                            if (!this.RenderPlainTextInlines.Peek())
                                goto useFullRendering;
                        }

                        break;

                    case InlineTag.Link:
                    case InlineTag.Strong:
                    case InlineTag.Emphasis:
                    case InlineTag.Strikethrough:
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                ignoreChildNodes = false;
                return;
            }

            useFullRendering:

            switch (inline.Tag)
            {
                case InlineTag.String:
                    ignoreChildNodes = true;
                    if (this.Settings.TrackSourcePosition)
                    {
                        this.Write("<span");
                        this.WritePositionAttribute(inline);
                        this.Write('>');
                        this.WriteEncodedHtml(inline.LiteralContentValue);
                        this.Write("</span>");
                    }
                    else
                    {
                        this.WriteEncodedHtml(inline.LiteralContentValue);
                    }

                    break;

                case InlineTag.LineBreak:
                    ignoreChildNodes = true;
                    this.WriteLine("<br />");
                    break;

                case InlineTag.SoftBreak:
                    ignoreChildNodes = true;
                    if (this.Settings.RenderSoftLineBreaksAsLineBreaks)
                        this.WriteLine("<br />");
                    else
                        this.WriteLine();
                    break;

                case InlineTag.Code:
                    ignoreChildNodes = true;
                    this.Write("<code");
                    if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(inline);
                    this.Write('>');
                    this.WriteEncodedHtml(inline.LiteralContentValue);
                    this.Write("</code>");
                    break;

                case InlineTag.RawHtml:
                    ignoreChildNodes = true;
                    // cannot output source position for HTML blocks
                    this.Write(inline.LiteralContentValue);
                    break;

                case InlineTag.Link:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        this.Write("<a href=\"");
                        var uriResolver = this.Settings.UriResolver;
                        if (uriResolver != null)
                            this.WriteEncodedUrl(uriResolver(inline.TargetUrl));
                        else
                            this.WriteEncodedUrl(inline.TargetUrl);

                        this.Write('\"');
                        if (inline.LiteralContentValue.Length > 0)
                        {
                            this.Write(" title=\"");
                            this.WriteEncodedHtml(inline.LiteralContentValue);
                            this.Write('\"');
                        }

                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(inline);

                        this.Write('>');
                    }

                    if (isClosing)
                    {
                        this.Write("</a>");
                    }

                    break;

                case InlineTag.Image:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        this.Write("<img src=\"");
                        var uriResolver = this.Settings.UriResolver;
                        if (uriResolver != null)
                            this.WriteEncodedUrl(uriResolver(inline.TargetUrl));
                        else
                            this.WriteEncodedUrl(inline.TargetUrl);

                        this.Write("\" alt=\"");

                        if (!isClosing)
                            this.RenderPlainTextInlines.Push(true);
                    }

                    if (isClosing)
                    {
                        // this.RenderPlainTextInlines.Pop() is done by the plain text renderer above.

                        this.Write('\"');
                        if (inline.LiteralContentValue.Length > 0)
                        {
                            this.Write(" title=\"");
                            this.WriteEncodedHtml(inline.LiteralContentValue);
                            this.Write('\"');
                        }

                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(inline);
                        this.Write(" />");
                    }

                    break;

                case InlineTag.Strong:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        this.Write("<strong");
                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(inline);
                        this.Write('>');
                    }

                    if (isClosing)
                    {
                        this.Write("</strong>");
                    }
                    break;

                case InlineTag.Emphasis:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        this.Write("<em");
                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(inline);
                        this.Write('>');
                    }

                    if (isClosing)
                    {
                        this.Write("</em>");
                    }
                    break;

                case InlineTag.Strikethrough:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        this.Write("<del");
                        if (this.Settings.TrackSourcePosition) this.WritePositionAttribute(inline);
                        this.Write('>');
                    }

                    if (isClosing)
                    {
                        this.Write("</del>");
                    }
                    break;

                default:
                    throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
            }
        }

        /// <summary>
        /// Writes the specified text to the target writer.
        /// </summary>
        protected void Write(string text)
        {
            if (text == null)
                return;
            this._target.Write(new StringPart(text, 0, text.Length));
        }

        private void Write(StringPart text)
        {
            this._target.Write(text);
        }

        /// <summary>
        /// Writes the specified text to the target writer.
        /// </summary>
        protected void Write(StringContent text)
        {
            if (text == null)
                return;

            text.WriteTo(this._target);
        }

        /// <summary>
        /// Writes the specified character to the target writer.
        /// </summary>
        protected void Write(char c)
        {
            this._target.Write(c);
        }

        /// <summary>
        /// Ensures that the output ends with a newline. This means that newline character will be written
        /// only if the writer does not currently end with a newline.
        /// </summary>
        protected void EnsureNewLine()
        {
            this._target.EnsureLine();
        }

        /// <summary>
        /// Writes a newline to the target writer.
        /// </summary>
        protected void WriteLine()
        {
            this._target.WriteLine();
        }

        /// <summary>
        /// Writes the specified text and a newline to the target writer.
        /// </summary>
        protected void WriteLine(string text)
        {
            this._target.Write(new StringPart(text, 0, text.Length));
            this._target.WriteLine();
        }

        /// <summary>
        /// Encodes the given text with HTML encoding (ampersand-encoding) and writes the result to the target writer.
        /// </summary>
        protected void WriteEncodedHtml(StringContent text)
        {
            if (text == null)
                return;

            HtmlFormatterSlim.EscapeHtml(text, this._target);
        }

        /// <summary>
        /// Encodes the given text with HTML encoding (ampersand-encoding) and writes the result to the target writer.
        /// </summary>
        protected void WriteEncodedHtml(string text)
        {
            if (text == null)
                return;

            HtmlFormatterSlim.EscapeHtml(new StringPart(text, 0, text.Length), this._target);
        }

        private void WriteEncodedHtml(StringPart text)
        {
            HtmlFormatterSlim.EscapeHtml(text, this._target);
        }

        /// <summary>
        /// Encodes the given text with URL encoding (percent-encoding) and writes the result to the target writer.
        /// Note that the result is intended to be written to HTML attribute so this also encodes <c>&amp;</c> character
        /// as <c>&amp;amp;</c>.
        /// </summary>
        protected void WriteEncodedUrl(string url)
        {
            HtmlFormatterSlim.EscapeUrl(url, this._target);
        }

        /// <summary>
        /// Writes a <c>data-sourcepos="start-end"</c> attribute to the target writer. 
        /// This method should only be called if <see cref="CommonMarkSettings.TrackSourcePosition"/> is set to <c>true</c>.
        /// Note that the attribute is preceded (but not succeeded) by a single space.
        /// </summary>
        protected void WritePositionAttribute(Block block)
        {
            HtmlFormatterSlim.PrintPosition(this._target, block);
        }

        /// <summary>
        /// Writes a <c>data-sourcepos="start-end"</c> attribute to the target writer. 
        /// This method should only be called if <see cref="CommonMarkSettings.TrackSourcePosition"/> is set to <c>true</c>.
        /// Note that the attribute is preceded (but not succeeded) by a single space.
        /// </summary>
        protected void WritePositionAttribute(Inline inline)
        {
            HtmlFormatterSlim.PrintPosition(this._target, inline);
        }
    }
}
