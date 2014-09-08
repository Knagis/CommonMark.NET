using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public enum InlineTag
    {
        str,
        softbreak,
        linebreak,
        code,
        raw_html,
        entity,
        emph,
        strong,
        link,
        image
    }
}
