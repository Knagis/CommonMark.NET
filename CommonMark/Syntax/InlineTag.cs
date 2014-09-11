using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public enum InlineTag
    {
        /// <remarks>Original: str</remarks>
        String,

        SoftBreak,

        LineBreak,

        Code,

        /// <remarks>Original: raw_html</remarks>
        RawHtml,

        Entity,

        /// <remarks>Original: emph</remarks>
        Emphasis,

        Strong,

        Link,

        Image
    }
}
