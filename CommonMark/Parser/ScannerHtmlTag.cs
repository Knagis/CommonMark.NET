using System;
using System.Text;

namespace CommonMark.Parser
{
    internal static partial class Scanner
    {
        private static int _scanHtmlTagCloseTag(string s, int pos, int sourceLength)
        {
            // Orignal regexp: "[/]" + tagname + @"\s*[>]"

            if (pos + 2 >= sourceLength)
                return 0;

            var nextChar = s[pos + 1];
            if ((nextChar < 'A' || nextChar > 'Z') && (nextChar < 'a' || nextChar > 'z'))
                return 0;

            var tagNameEnded = false;
            for (var i = pos + 2; i < sourceLength; i++)
            {
                nextChar = s[i];
                if (nextChar == '>')
                    return i - pos + 1;

                if (nextChar == ' ' || nextChar == '\n')
                {
                    tagNameEnded = true;
                    continue;
                }

                if (tagNameEnded || ((nextChar < 'A' || nextChar > 'Z') 
                                  && (nextChar < 'a' || nextChar > 'z') 
                                  && (nextChar < '0' || nextChar > '9')))
                    return 0;
            }

            return 0;
        }

        private static int _scanHtmlTagProcessingInstruction(string s, int pos, int sourceLength)
        {
            // Original regexp: "\\?(([^?>\\x00]+)|([?][^>\\x00]))*\\?>"
            // note the original regexp is invalid since it does not allow '>' within the tag.

            char nextChar;
            char lastChar = '\0';
            for (var i = pos + 1; i < sourceLength; i++)
            {
                nextChar = s[i];

                if (nextChar == '>' && lastChar == '?')
                    return i - pos + 1;

                lastChar = nextChar;
            }

            return 0;
        }

        private static int _scanHtmlTagHtmlComment(string s, int pos, int sourceLength)
        {
            // we know that the initial "!-" has already been verified
            if (pos + 5 >= sourceLength)
                return 0;

            if (s[pos + 2] != '-')
                return 0;

            char nextChar = s[pos + 3];
            if (nextChar == '>' || (nextChar == '-' && s[pos + 4] == '>'))
                return 0;

            byte hyphenCount = 0;
            for (var i = pos + 3; i < sourceLength ; i++)
            {
                nextChar = s[i];

                if (hyphenCount == 2)
                    return nextChar == '>' ? i - pos + 1 : 0;

                if (nextChar == '-')
                    hyphenCount++;
                else
                    hyphenCount = 0;
            }

            return 0;
        }

        private static int _scanHtmlTagCData(string s, int pos, int sourceLength)
        {
            // Original regexp: "\\!\\[CDATA\\[(([^\\]\\x00]+)|(\\][^\\]\\x00])|(\\]\\][^>\\x00]))*\\]\\]>"

            if (pos + 10 >= sourceLength)
                return 0;

            if (!string.Equals(s.Substring(pos, 8), "![CDATA[", StringComparison.Ordinal))
                return 0;

            var bracketCount = 0;
            char nextChar;
            for (var i = pos + 8; i < sourceLength; i++ )
            {
                nextChar = s[i];

                if (nextChar == '>' && bracketCount >= 2)
                    return i - pos + 1;

                if (nextChar == ']')
                    bracketCount++;
                else
                    bracketCount = 0;
            }

            return 0;
        }

        private static int _scanHtmlTagDeclaration(string s, int pos, int sourceLength)
        {
            // Original regexp: "\\![A-Z]+\\s+[^>\\x00]*>"

            // minimum value: "!A >"
            if (pos + 4 >= sourceLength)
                return 0;

            var spaceFound = false;
            char nextChar = s[pos + 2];
            if (nextChar < 'A' || nextChar > 'Z')
                return 0;

            for (var i = pos + 3; i < sourceLength; i++)
            {
                nextChar = s[i];

                if (nextChar == '>')
                    return spaceFound ? i - pos + 1 : 0;

                if (nextChar == ' ' || nextChar == '\n')
                    spaceFound = true;
            }

            return 0;
        }

        private static int _scanHtmlTagOpenTag(string s, int pos, int sourceLength)
        {
            var lastPosition = sourceLength - 1;

            // the minimum length valid tag is "a>"
            if (lastPosition < pos + 1)
                return 0;

            // currentPosition - positioned after the last character matched by that any particular part
            var currentPosition = pos;

            // stores the character at the current position
            char currentChar = s[currentPosition];
            
            // stores if the previous character was a whitespace
            bool hadWhitespace = false;

            // stores if an attribute name has been parsed
            bool hadAttribute = false;

            // some additional variables used in the process
            char c1;

            // The tag name must start with an ASCII letter
            if (!ScannerCharacterMatcher.MatchAsciiLetter(s, ref currentChar, ref currentPosition, lastPosition))
                return 0;

            // Move past any other characters that make up the tag name
            ScannerCharacterMatcher.MatchAsciiLetterOrDigit(s, ref currentChar, ref currentPosition, lastPosition);

            // loop while the end of string is reached or the tag is closed
            while (currentPosition <= lastPosition)
            {
                // Move past any whitespaces
                hadWhitespace = ScannerCharacterMatcher.MatchWhitespaces(s, ref currentChar, ref currentPosition, lastPosition);

                // check if the end of the tag has been reached
                if (currentChar == '>')
                    return currentPosition - pos + 1;

                if (currentChar == '/')
                {
                    if (currentPosition == lastPosition) return 0;
                    currentChar = s[++currentPosition];
                    return (currentChar == '>') ? currentPosition - pos + 1 : 0;
                }

                // check if arrived at the attribute value
                if (currentChar == '=')
                {
                    if (!hadAttribute || currentPosition == lastPosition)
                        return 0;

                    // move past the '=' symbol and any whitespaces
                    currentChar = s[++currentPosition];
                    ScannerCharacterMatcher.MatchWhitespaces(s, ref currentChar, ref currentPosition, lastPosition);

                    if (currentChar == '\'' || currentChar == '\"')
                    {
                        c1 = currentChar;

                        currentChar = s[++currentPosition];
                        ScannerCharacterMatcher.MatchAnythingExcept(s, ref currentChar, ref currentPosition, lastPosition, c1);

                        if (currentChar != c1 || currentPosition == lastPosition)
                            return 0;

                        currentChar = s[++currentPosition];
                    }
                    else
                    {
                        // an unquoted value must have at least one character
                        if (!ScannerCharacterMatcher.MatchAnythingExceptWhitespaces(s, ref currentChar, ref currentPosition, lastPosition, '\"', '\'', '=', '<', '>', '`'))
                            return 0;
                    }

                    hadAttribute = false;
                    continue;
                }

                // the attribute must be preceded by a whitespace
                if (!hadWhitespace)
                    return 0;

                // if the end has not been found then there is just one possible alternative - an attribute
                // validate that the attribute name starts with a correct character
                if (!ScannerCharacterMatcher.MatchAsciiLetter(s, ref currentChar, ref currentPosition, lastPosition, '_', ':'))
                    return 0;

                // match any remaining characters in the attribute name
                ScannerCharacterMatcher.MatchAsciiLetterOrDigit(s, ref currentChar, ref currentPosition, lastPosition, '_', ':', '.', '-');

                hadAttribute = true;
            }

            return 0;
        }

        /// <summary>
        /// Try to match an HTML tag after first &lt;, returning number of chars matched.
        /// </summary>
        public static int scan_html_tag(string s, int pos, int sourceLength)
        {
            if (pos + 2 >= sourceLength)
                return 0;

            var firstChar = s[pos];

            if (firstChar == '/')
                return _scanHtmlTagCloseTag(s, pos, sourceLength);

            if (firstChar == '?')
                return _scanHtmlTagProcessingInstruction(s, pos, sourceLength);

            if (firstChar == '!')
            {
                var nextChar = s[pos + 1];
                if (nextChar == '-')
                    return _scanHtmlTagHtmlComment(s, pos, sourceLength);

                if (nextChar == '[')
                    return _scanHtmlTagCData(s, pos, sourceLength);

                return _scanHtmlTagDeclaration(s, pos, sourceLength);
            }

            return _scanHtmlTagOpenTag(s, pos, sourceLength);
        }
    }
}
