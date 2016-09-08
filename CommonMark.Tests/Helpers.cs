using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonMark.Tests
{
    internal static class Helpers
    {
        public static Syntax.Block ParseDocument(string commonMark, CommonMarkSettings settings = null)
        {
            using (var reader = new System.IO.StringReader(Helpers.Normalize(commonMark)))
            {
                return CommonMarkConverter.Parse(reader, settings);
            }
        }

        public static void ExecuteTest(string commonMark, string html, CommonMarkSettings settings = null, Func<System.IO.TextWriter, CommonMarkSettings, Formatters.HtmlFormatter> htmlFormatterFactory = null)
        {
            if (settings == null)
                settings = CommonMarkSettings.Default.Clone();

            Helpers.LogValue("CommonMark", Denormalize(commonMark));
            Helpers.LogValue("Expected", Denormalize(html));

            // Arrange
            commonMark = Helpers.Normalize(commonMark);
            html = Helpers.Normalize(html);
            
            string actual;
            Syntax.Block document;

            // Act
            using (var reader = new System.IO.StringReader(commonMark))
            using (var writer = new System.IO.StringWriter())
            {
                if (htmlFormatterFactory != null)
                {
                    settings = settings.Clone();
                    settings.OutputFormat = OutputFormat.CustomDelegate;
                    settings.OutputDelegate = (doc, target, stngs) => htmlFormatterFactory(target, stngs).WriteDocument(doc);
                }

                document = CommonMarkConverter.ProcessStage1(reader, settings);
                CommonMarkConverter.ProcessStage2(document, settings);
                CommonMarkConverter.ProcessStage3(document, writer, settings);
                actual = writer.ToString();
            }

            // Assert
            Helpers.LogValue("Actual", Denormalize(actual));
            actual = Helpers.Tidy(actual);
            Assert.AreEqual(Helpers.Tidy(html), actual);

            if (htmlFormatterFactory == null)
            {
                // Verify that the extendable HTML formatter returns the same result, unless
                // the test was run with a custom extendable HTML formatter factory. In the
                // latter case, only the output of the custom extendable HTML formatter is
                // verified.
                var settingsHtmlFormatter = settings.Clone();
                settingsHtmlFormatter.OutputDelegate = (doc, target, stngs) => new Formatters.HtmlFormatter(target, stngs).WriteDocument(doc);
                var actual2 = CommonMarkConverter.Convert(commonMark, settingsHtmlFormatter);
                Assert.AreEqual(actual, Helpers.Tidy(actual2), "HtmlFormatter returned a different result than HtmlFormatterSlim.");
            }

            // Additionally verify that the parser included source position information.
            // This is done here to catch cases during specification tests that might not be 
            // covered in SourcePositionTests.cs.
            var firstFail = document.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.IsOpening && o.Inline.SourceLength <= 0);
            if (firstFail != null)
            {
                Assert.Fail("Incorrect source position: " + firstFail);
            }
        }

        private static string Denormalize(string value)
        {
            value = value.Replace('\t', '→');
            value = value.Replace(' ', '␣');
            return value;
        }

        private static string Normalize(string value)
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
        }

        public static void Log(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
                Console.WriteLine(format, args);
            else
                Console.WriteLine(format);
        }

        public static void LogValue(string caption, string text)
        {
            Console.Write(caption);
            Console.WriteLine(":");
            Console.WriteLine();

            Console.Write("    ");
            Console.WriteLine(text.Replace("\n", "\n    "));

            Console.WriteLine();
        }
    }
}
