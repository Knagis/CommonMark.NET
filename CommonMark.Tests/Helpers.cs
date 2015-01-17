/*
The MIT License (MIT)

Copyright (c) 2014 Morten Houston Ludvigsen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonMark.Tests
{
    internal static class Helpers
    {
        public static Syntax.Block ParseDocument(string commonMark, CommonMarkSettings settings = null)
        {
            using (var reader = new System.IO.StringReader(Helpers.Normalize(commonMark)))
            {
                var doc = CommonMarkConverter.ProcessStage1(reader, settings);
                CommonMarkConverter.ProcessStage2(doc, settings);
                return doc;
            }
        }

        public static void ExecuteTest(string commonMark, string html, CommonMarkSettings settings = null)
        {
            // Arrange
            commonMark = Helpers.Normalize(commonMark);
            html = Helpers.Normalize(html);

            Helpers.LogValue("CommonMark", commonMark);
            Helpers.LogValue("Expected", html);

            // Act
            var actual = CommonMarkConverter.Convert(commonMark, settings);

            // Assert
            Helpers.LogValue("Actual", actual);
            Assert.AreEqual(Helpers.Tidy(html), Helpers.Tidy(actual));
        }

        public static string Normalize(string value)
        {
            value = value.Replace('→', '\t');
            value = value.Replace('␣', ' ');
            return value;
        }

        public static string Tidy(string html)
        {
            html = html.Replace("\r", "").Trim();

            // collapse spaces and newlines before </li> and after <li>
            html = Regex.Replace(html, @"\s+</li>", "</li>");
            html = Regex.Replace(html, @"<li>\s+", "<li>");

            // needed to compare UTF-32 characters
            html = html.Normalize(NormalizationForm.FormKD);
            return html;

            ////var result = new StringBuilder();
            ////var inPre = false;
            ////foreach (var line in html.Split('\n'))
            ////{
            ////    if (Regex.IsMatch(line, @"<pre"))
            ////        inPre = true;
            ////    if (Regex.IsMatch(line, @"</pre"))
            ////        inPre = false;
            ////    if (inPre)
            ////        result.AppendLine(line);
            ////    else
            ////        result.AppendLine(line.Trim());
            ////}
            ////return result.ToString();
        }

        public static void Log()
        {
            Console.WriteLine();
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        public static void LogValue(string caption, string text)
        {
            Log("{0}:", caption);
            Log();
            Log(string.Join("\n", text.Replace("\r", "").Split('\n', '\r').Select(s => "    " + s)));
            Log();
        }
    }
}
