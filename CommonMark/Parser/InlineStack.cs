using System;
using System.Collections.Generic;
using System.Text;
using CommonMark.Syntax;

namespace CommonMark.Parser
{
    /// <summary>
    /// Describes an element in a stack of possible inline openers.
    /// </summary>
    internal sealed class InlineStack
    {
        /// <summary>
        /// Previous entry in the stack. <c>null</c> if this is the last one.
        /// </summary>
        public InlineStack Previous;

        /// <summary>
        /// The at-the-moment text inline that could be transformed into the opener.
        /// </summary>
        public Inline StartingInline;

        /// <summary>
        /// The number of delimeter characters found for this opener.
        /// </summary>
        public int DelimeterCount;

        /// <summary>
        /// The character that was used in the opener.
        /// </summary>
        public char Delimeter;
    }
}
