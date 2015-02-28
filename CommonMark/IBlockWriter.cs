using CommonMark.Syntax;
using System.IO;

namespace CommonMark
{
    /// <summary>
    /// Printer for a CommonMark document
    /// </summary>
    public interface IBlockWriter
    {
        /// <summary>
        /// Prints the block in the writer
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="block">Block to print</param>
        /// <param name="settings">Settings</param>
        void Write(TextWriter writer, Block block, CommonMarkSettings settings);
    }
}
