using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonMark.Formatter
{
    public class HtmlFormatter
    {
        private readonly HtmlTextWriter _target;
        private readonly CommonMarkSettings _settings;

        public HtmlFormatter(TextWriter target, CommonMarkSettings settings)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (settings == null)
                settings = CommonMarkSettings.Default;

            this._target = new HtmlTextWriter(target);
            this._settings = settings;
        }

        protected CommonMarkSettings Settings { get { return this._settings; } }

        /// <summary>
        /// Writes the given CommonMark document to the output stream as HTML.
        /// </summary>
        public void Write(Block document)
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
        /// the <paramref name="ignoreChildBlocks"/> is used to notify the caller whether it should recurse
        /// into the child nodes.
        /// </summary>
        /// <param name="block">The block element to be written to the output stream.</param>
        /// <param name="isOpening">Specifies whether the block element is being opened (or started).</param>
        /// <param name="isClosing">Specifies whether the block element is being closed. If the block does not
        /// have child nodes, then both <paramref name="isClosing"/> and <paramref name="IsOpening"/> can be
        /// <c>true</c> at the same time.</param>
        /// <param name="ignoreChildBlocks">Instructs the caller whether to skip processing of child nodes or not.</param>
        protected virtual void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildBlocks)
        {
            ignoreChildBlocks = false;
        }

        // See WriteBlock for comment. On release, the comment should be copied here as well.
        protected virtual void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildInlines)
        {
            ignoreChildInlines = false;
        }

        protected void Write(string text)
        {
            this._target.Write(new StringPart(text, 0, text.Length));
        }

        protected void WriteEncodedHtml(StringContent text)
        {
            HtmlFormatterSlim.EscapeHtml(text, this._target);
        }

        protected void WriteEncodedUrl(string url)
        {
            HtmlFormatterSlim.EscapeUrl(url, this._target);
        }
    }
}
