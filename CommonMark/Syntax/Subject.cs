using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    internal class Subject
    {
        /// <summary>
        /// Gets or sets the whole buffer this instance is created over.
        /// </summary>
        public string Buffer;

        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Position;

        public int LabelNestingLevel;

        public Dictionary<string, Reference> ReferenceMap;
    }
}
