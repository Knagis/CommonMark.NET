using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class Reference
    {
        /// <summary>
        /// A special constant reference that represents an collapsed reference link: [foo][]
        /// </summary>
        internal static readonly Reference SelfReference = new Reference();

        /// <summary>
        /// A special constant reference that signifies that the reference label was not found: [foo][bar]
        /// </summary>
        internal static readonly Reference InvalidReference = new Reference();

        /// <summary>
        /// Gets or sets the label (the key by which it is referenced in the mapping) of the reference.
        /// </summary>
        public string Label { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
