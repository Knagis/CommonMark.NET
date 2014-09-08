using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class ListData
    {
        public ListType ListType;
        public int marker_offset;
        public int padding;
        public int start;
        public ListDelimiter delimiter;
        public char BulletChar;
        public bool tight;
    }
}
