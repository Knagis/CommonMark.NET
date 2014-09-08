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
        public static char? bchar(string data, int pos)
        {
            if (data.Length <= pos)
                return null;
            return data[pos];
        }

        /// <summary>
        /// Inserts the bstring s2 into s1 at position pos.  If the position pos is 
        /// past the end of s1, then the character "fill" is appended as necessary to 
        /// make up the gap between the end of s1 and pos.  The value BSTR_OK is 
        /// returned if the operation is successful, otherwise BSTR_ERR is returned.
        /// </summary>
        public static void binsert (ref string s1, int pos, string s2, char fill)
        {
            while (s1.Length < pos)
                s1 += fill;
            s1 = s1.Insert(pos, s2);
        }

        public static void binsertch(ref string s, int pos, int len, char fill)
        {
            while (s.Length < pos)
                s += fill;
            s = s.Insert(pos, new string(fill, len));
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
        /// Search for the last position in b0 no greater than pos, in which none of 
        /// the characters in b1 is found and return it.  This function has an 
        /// execution time of O(b0->slen + b1->slen).  If such a position does not 
        /// exist in b0, then -1 is returned.
        /// </summary>
        public static int bninchrr(string s, int pos, string invalidchars)
        {
            if (s == null)
                return -1;

            if (invalidchars == null || invalidchars.Length == 0)
                return pos;

            char c;
            bool match;
            for (var i = pos; i >= 0; i--)
            {
                c = s[i];
                match = false;
                for (var j = 0; j < invalidchars.Length; j++)
                    if (c == invalidchars[j])
                    {
                        match = true;
                        break;
                    }

                if (!match)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Search for the character c in b forwards from the position pos 
        /// (inclusive).  Returns the position of the found character or -1 if 
        /// it is not found.
        /// </summary>
        public static int bstrchrp(string b, char c, int pos)
        {
            return b.IndexOf(c, pos);
        }

        /// <summary>
        /// Search for the character c in b backwards from the position pos in bstring 
        /// (inclusive).  Returns the position of the found character or -1 if 
        /// it is not found.
        /// </summary>
        public static int bstrrchrp(string b, char c, int pos)
        {
            return b.LastIndexOf(c, pos);
        }

        /// <summary>
        /// Search for the character c in the bstring b forwards from the start of 
        /// the bstring.  Returns the position of the found character or BSTR_ERR if 
        /// it is not found.
        /// </summary>
        public static int bstrchr(string b, char c)
        {
            return b.IndexOf(c);
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
