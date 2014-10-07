using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class Block
    {
        public Block(BlockTag tag, int startLine, int startColumn)
        {
            this.Tag = tag;
            this.StartLine = startLine;
            this.EndLine = startLine;
            this.StartColumn = startColumn;
            this.IsOpen = true;
        }

        public BlockTag Tag { get; set; }

        /// <remarks>Original: start_line</remarks>
        public int StartLine { get; set; }

        /// <remarks>Original: start_column</remarks>
        public int StartColumn { get; set; }

        /// <remarks>Original: end_line</remarks>
        public int EndLine { get; set; }

        /// <remarks>Original: open</remarks>
        public bool IsOpen { get; set; }

        /// <remarks>Original: last_line_blank</remarks>
        public bool IsLastLineBlank { get; set; }

        /// <remarks>Original: children</remarks>
        public Block FirstChild { get; set; }

        /// <remarks>Original: last_child</remarks>
        public Block LastChild { get; set; }

        public Block Parent { get; set; }

        public Block Top { get; set; }

        /// <summary>
        /// Gets or sets the string content of this block. The content consists of multiple string parts to avoid string concatenation.
        /// Note that some parts of the parser (for example, <see cref="Formatter.HtmlPrinter.EscapeHtml(StringContent, bool, System.IO.TextWriter)"/>) might assume that
        /// the parts are not split within certain objects, so it is advised that the parts are split on newline.
        /// </summary>
        /// <remarks>Original: string_content</remarks>
        public StringContent StringContent { get; set; }

        /// <remarks>Original: inline_content</remarks>
        public Inline InlineContent { get; set; }

        private ListData _listData = new ListData();
        /// <remarks>Original: list_data</remarks>
        public ListData ListData { get { return this._listData; } set { this._listData = value; } }

        private readonly FencedCodeData _fencedCodeData = new FencedCodeData();
        /// <remarks>Original: fenced_code_data</remarks>
        public FencedCodeData FencedCodeData { get { return this._fencedCodeData; } }

        /// <remarks>Original: header_level</remarks>
        public int HeaderLevel { get; set; }

        /// <remarks>Original: refmap</remarks>
        public Dictionary<string, Reference> ReferenceMap { get; set; }

        public Block NextSibling { get; set; }

        /// <remarks>Original: prev</remarks>
        public Block Previous { get; set; }
    }
}
