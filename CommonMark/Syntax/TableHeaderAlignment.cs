namespace CommonMark.Syntax
{
    /// <summary>
    /// Defines the alignment specified in a header column for a GithubStyleTable
    /// </summary>
    public enum TableHeaderAlignment
    {
        /// <summary>
        /// No alignment specified
        /// </summary>
        None = 0,

        /// <summary>
        /// Left alignment
        /// </summary>
        Left = 1,

        /// <summary>
        /// Right alignment
        /// </summary>
        Right = 2,

        /// <summary>
        /// Center alignment
        /// </summary>
        Center = 3
    }
}