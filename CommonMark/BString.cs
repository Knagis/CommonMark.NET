using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark
{
    /// <summary>
    /// Methods from BString library that are used to ease the conversion from C code.
    /// These are mainly used because the naming of the methods are easy to confuse and errors might 
    /// be introduced if the calls would be modified right away during the porting process.
    /// </summary>
    internal static class BString
    {
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static char? bchar(string data, int pos)
        {
            if (data.Length <= pos)
                return null;
            return data[pos];
        }
    }
}
