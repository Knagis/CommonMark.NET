using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class FencedCodeData
    {
        /// <remarks>Original: fence_length</remarks>
        public int FenceLength { get; set; }

        /// <remarks>Original: fence_offset</remarks>
        public int FenceOffset { get; set; }

        /// <remarks>Original: fence_char</remarks>
        public char FenceChar { get; set; }

        public string Info { get; set; }
    }
}
