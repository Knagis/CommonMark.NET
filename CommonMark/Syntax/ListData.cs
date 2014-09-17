using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public sealed class ListData
    {
        public ListType ListType { get; set; }

        /// <remarks>Original: marker_offset</remarks>
        public int MarkerOffset { get; set; }

        public int Padding { get; set; }

        public int Start { get; set; }

        public ListDelimiter Delimiter { get; set; }

        public char BulletChar { get; set; }

        /// <remarks>Original: tight</remarks>
        public bool IsTight { get; set; }
    }
}
