using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Specifies the element type of a <see cref="Block"/> instance.
    /// </summary>
    public enum BlockTag
    {
        /// <summary>
        /// The root element that represents the document itself. There should only be one in the tree.
        /// </summary>
        Document,

        /// <summary>
        /// A block-quote element.
        /// </summary>
        BlockQuote,

        /// <summary>
        /// A list element. Will contain nested blocks with type of <see cref="BlockTag.ListItem"/>.
        /// </summary>
        List,

        /// <summary>
        /// An item in a block element of type <see cref="BlockTag.List"/>.
        /// </summary>
        ListItem,

        /// <summary>
        /// A code block element that was formatted with fences (for example, <c>~~~\nfoo\n~~~</c>).
        /// </summary>
        FencedCode,

        /// <summary>
        /// A code block element that was formatted by indenting the lines with at least 4 spaces.
        /// </summary>
        IndentedCode,

        /// <summary>
        /// A raw HTML code block element.
        /// </summary>
        HtmlBlock,

        /// <summary>
        /// A paragraph block element.
        /// </summary>
        Paragraph,

        /// <summary>
        /// A header element that was parsed from an ATX style markup (<c>## heading 2</c>).
        /// </summary>
        AtxHeader,

        /// <summary>
        /// A header element that was parsed from a Setext style markup (<c>heading\n========</c>).
        /// </summary>
        SETextHeader,

        /// <summary>
        /// A horizontal ruler element.
        /// </summary>
        HorizontalRuler,

        /// <summary>
        /// A text block that contains only link reference definitions.
        /// </summary>
        ReferenceDefinition
    }
}
