using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Represents a part of <see cref="StringContent"/>.
    /// </summary>
    internal struct StringPart
    {
        /// <summary>
        /// Gets or sets the string object this part is created from.
        /// </summary>
        public string Source;

        /// <summary>
        /// Gets or sets the index at which this part starts.
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// Gets or sets the length of the part.
        /// </summary>
        public int Length;
    }
}
