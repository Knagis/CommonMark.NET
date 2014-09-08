using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    public class Block
    {
        public BlockTag tag;
        public int start_line;
        public int start_column;
        public int end_line;
        public bool open;
        public bool last_line_blank;
        public Block children;
        public Block last_child;
        public Block parent;
        public Block top;
        public string string_content;
        public Inline inline_content;
        public BlockAttributes attributes = new BlockAttributes();
        public Block next;
        public Block prev;
    }
}
