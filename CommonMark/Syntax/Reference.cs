using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class Reference
    {
        /// <summary>
        /// Gets or sets the label (the key by which it is referenced in the mapping) of the reference.
        /// </summary>
        public string Label { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
