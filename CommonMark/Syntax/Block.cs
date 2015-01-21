using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Represents a block-level element of the parsed document.
    /// </summary>
    public sealed class Block
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="tag">The type of the element this instance represents.</param>
        /// <param name="startLine">The number of the first line in the source text that contains this element.</param>
        /// <param name="startColumn">The number of the first column (within the first line) in the source text that contains this element.</param>
        public Block(BlockTag tag, int startLine, int startColumn)
        {
            this.Tag = tag;
            this.SourceStartLine = startLine;
            this.SourceEndLine = startLine;
            this.SourceStartColumn = startColumn;
            this.IsOpen = true;
        }

        /// <summary>
        /// Returns an enumerable that allows the iteration over all block and inline elements within this
        /// instance. Note that the enumerator should not be used if optimal performance is desired and instead
        /// a custom implementation should be written.
        /// </summary>
        public IEnumerable<EnumeratorEntry> AsEnumerable()
        {
            return new Enumerable(this);
        }

        /// <summary>
        /// Creates a new top-level document block.
        /// </summary>
        internal static Block CreateDocument()
        {
            Block e = new Block(BlockTag.Document, 1, 1);
            e.ReferenceMap = new Dictionary<string, Reference>();
            e.Top = e;
            return e;
        }

        /// <summary>
        /// Gets or sets the type of the element this instance represents.
        /// </summary>
        public BlockTag Tag { get; set; }

        /// <summary>
        /// Gets or sets the number of the first line in the source text that contains this element.
        /// </summary>
        [Obsolete("Use SourceStartLine instead", false)]
        public int StartLine { get { return this.SourceStartLine; } }

        /// <summary>
        /// Gets or sets the number of the first column (within the first line) in the source text that contains this element.
        /// </summary>
        [Obsolete("Use SourceStartColumn instead", false)]
        public int StartColumn { get { return this.SourceStartColumn; } }

        /// <summary>
        /// Gets or sets the number of the last line in the source text that contains this element.
        /// </summary>
        [Obsolete("Use SourceEndLine instead", false)]
        public int EndLine { get { return this.SourceEndLine; } }

        /// <summary>
        /// Gets or sets the number of the first line in the source text that contains this element.
        /// </summary>
        public int SourceStartLine { get; set; }

        /// <summary>
        /// Gets or sets the number of the first column (within the first line) in the source text that contains this element.
        /// </summary>
        public int SourceStartColumn { get; set; }

        /// <summary>
        /// Gets or sets the number of the last line in the source text that contains this element.
        /// </summary>
        public int SourceEndLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this block element has been completed (and thus new lines cannot be added
        /// to it) or is still open. By default all elements are created as open and are closed when the parser detects it.
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the last line parsed for this block element was blank (containing only
        /// whitespaces).
        /// </summary>
        public bool IsLastLineBlank { get; set; }

        /// <summary>
        /// Gets or sets the first child element of this instance. <c>null</c> if there are no children.
        /// </summary>
        public Block FirstChild { get; set; }

        /// <summary>
        /// Gets or sets the last child element (the last sibling of <see cref="FirstChild"/>) of this instance. 
        /// <c>null</c> if there are no children.
        /// </summary>
        public Block LastChild { get; set; }

        /// <summary>
        /// Gets or sets the parent element of this block.
        /// </summary>
        public Block Parent { get; set; }

        /// <summary>
        /// Gets or sets the root element (that represents the document itself).
        /// </summary>
        public Block Top { get; set; }

        /// <summary>
        /// Gets or sets the string content of this block. The content consists of multiple string parts to avoid string concatenation.
        /// Note that some parts of the parser (for example, <see cref="Formatter.HtmlPrinter.EscapeHtml(StringContent, System.IO.TextWriter)"/>) might assume that
        /// the parts are not split within certain objects, so it is advised that the parts are split on newline.
        /// </summary>
        public StringContent StringContent { get; set; }

        /// <summary>
        /// Gets or sets the first inline element that was parsed from <see cref="StringContent"/> property.
        /// Note that once the inlines are parsed, <see cref="StringContent"/> will be set to <c>null</c>.
        /// </summary>
        public Inline InlineContent { get; set; }

        /// <summary>
        /// Gets or sets the additional properties that apply to list elements.
        /// </summary>
        public ListData ListData { get; set; }

        /// <summary>
        /// Gets or sets the additional properties that apply to fenced code blocks.
        /// </summary>
        public FencedCodeData FencedCodeData { get; set; }

        /// <summary>
        /// Gets or sets the heading level (as in <c>&lt;h1&gt;</c> or <c>&lt;h2&gt;</c>).
        /// </summary>
        public int HeaderLevel { get; set; }

        /// <summary>
        /// Gets or sets the dictionary containing resolved link references. Only set on the document node, <c>null</c>
        /// and not used for all other elements.
        /// </summary>
        public Dictionary<string, Reference> ReferenceMap { get; set; }

        /// <summary>
        /// Gets or sets the next sibling of this block element. <c>null</c> if this is the last element.
        /// </summary>
        public Block NextSibling { get; set; }

        /// <summary>
        /// Gets or sets the previous sibling of this block element. <c>null</c> if this is the first element.
        /// </summary>
        public Block Previous { get; set; }

        internal Parser.PositionTracker PositionTracker;
    }
}
