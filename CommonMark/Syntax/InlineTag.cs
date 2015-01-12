using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public enum InlineTag
    {
        /// <remarks>Original: str</remarks>
        String = 0,

        SoftBreak,

        LineBreak,

        Code,

        /// <remarks>Original: raw_html</remarks>
        RawHtml,

        /// <remarks>Original: emph</remarks>
        Emphasis,

        Strong,

        Link,

        Image,

        /// <summary>
        /// Represents an inline element that has been "removed" (visually represented as strikethrough).
        /// Only present if the <see cref="CommonMarkAdditionalFeatures.StrikethroughTilde"/> is enabled.
        /// </summary>
        Strikethrough
    }
}
