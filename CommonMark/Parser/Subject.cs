using CommonMark.Syntax;
using System.Collections.Generic;

namespace CommonMark.Parser
{
    [System.Diagnostics.DebuggerDisplay("{DebugToString()}")]
    internal sealed class Subject
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

        /// <summary>
        /// The last top-level inline parsed from this subject.
        /// </summary>
        public Inline LastInline;

        /// <summary>
        /// The current stack of possible emphasis openers. Can be <c>null</c>.
        /// </summary>
        public InlineStack EmphasisStack;

        public Dictionary<string, Reference> ReferenceMap;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by [DebuggerDisplay]")]
        private string DebugToString()
        {
            if (this.Position > this.Buffer.Length)
                return this.Buffer;

            return this.Buffer.Insert(this.Position, "⁞");
        }
    }
}
