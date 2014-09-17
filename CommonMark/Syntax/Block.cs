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

        /// <remarks>Original: string_content</remarks>
        public string StringContent { get; set; }

        /// <remarks>Original: inline_content</remarks>
        public Inline InlineContent { get; set; }

        private readonly BlockAttributes _attributes = new BlockAttributes();
        public BlockAttributes Attributes { get { return this._attributes; } }

        public Block Next { get; set; }

        /// <remarks>Original: prev</remarks>
        public Block Previous { get; set; }
    }
}
