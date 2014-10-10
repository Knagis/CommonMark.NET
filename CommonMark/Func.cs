using System;

namespace CommonMark
{
#if OptimizeFor20
    /// <summary>An alternative to <c>System.Func</c> which is not present in .NET 2.0.</summary>
    public delegate TResult Func<in T, out TResult>(T arg);
#endif
}
