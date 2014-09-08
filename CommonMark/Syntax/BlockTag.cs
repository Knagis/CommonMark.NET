using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public enum BlockTag
    {
        document,
        block_quote,
        list,
        list_item,
        fenced_code,
        indented_code,
        html_block,
        paragraph,
        atx_header,
        setext_header,
        hrule,
        reference_def
    }
}
