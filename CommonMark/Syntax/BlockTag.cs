using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public enum BlockTag
    {
        Document,

        /// <remarks>Original: block_quote</remarks>
        BlockQuote,

        List,

        /// <remarks>Original: list_item</remarks>
        ListItem,

        /// <remarks>Original: fenced_code</remarks>
        FencedCode,

        /// <remarks>Original: indented_code</remarks>
        IndentedCode,

        /// <remarks>Original: html_block</remarks>
        HtmlBlock,

        Paragraph,

        /// <remarks>Original: atx_header</remarks>
        AtxHeader,

        /// <remarks>Original: setext_header</remarks>
        SETextHeader,

        HorizontalRuler,

        /// <remarks>Original: reference_def</remarks>
        ReferenceDefinition
    }
}
