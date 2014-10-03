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

        /// <summary>
        /// Search for the first position in b0 starting from pos or after, in which 
        /// one of the characters in b1 is found.  This function has an execution 
        /// time of O(b0->slen + b1->slen).  If such a position does not exist in b0, 
        /// then BSTR_ERR is returned.
        /// </summary>
        public static int binchr (string b0, int pos, string b1)
        {
            return b0.IndexOfAny(b1.ToCharArray(), pos);
        }

        /// <summary>
        /// Create a bstring which is the substring of b starting from position left 
        /// and running for a length len (clamped by the end of the bstring b.)  If 
        /// there was no error, the value of this constructed bstring is returned 
        /// otherwise NULL is returned.
        /// </summary>
        public static string bmidstr(string b, int left, int len)
        {
            if (left + len >= b.Length)
                return b.Substring(left);

            return b.Substring(left, len);
        }
    }
}
