using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark
{
    public class Reference
    {
        /// <summary>
        /// Gets or sets the label (the key by which it is referenced in the mapping) of the reference.
        /// </summary>
        public string Label;
        public string Url;
        public string Title;
    }
}
