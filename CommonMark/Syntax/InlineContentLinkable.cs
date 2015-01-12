using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    [Obsolete("These properties have been moved directly into the Inline element.")]
    public sealed class InlineContentLinkable
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
