using CommonMark.Formatter;
using CommonMark.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CommonMark.Tests
{
    [TestClass]
    public class CustomPrinterTests
    {
        [TestMethod]
        [TestCategory("Inlines - Custom Printer")]
        public void CustomHelloPrinter()
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.CustomOutputPrinter = new HelloPrinter();

            Helpers.ExecuteTest("foo <h1>asd</h1> bar", "hello", settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Custom Printer")]
        public void CustomPrinterBaseOnExtensibleHtmlPrinter_Paragraph()
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.CustomOutputPrinter = new OverriddenHtmlPrinter();
            
            Helpers.ExecuteTest("foo <h1>asd</h1> bar", "<span>foo <h1>asd</h1> bar</span>", settings);
        }

        [TestMethod]
        [TestCategory("Inlines - Custom Printer")]
        public void CustomPrinterBaseOnExtensibleHtmlPrinter_List()
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.CustomOutputPrinter = new OverriddenHtmlPrinter();

            Helpers.ExecuteTest(" 1. summary\r\n 1. summary2\r\n 2. summary3", @"<ul class=""srn-priority"">
<li>summary</li>
<li>summary2</li>
<li>summary3</li>
</ul>", settings);
        }

        public class HelloPrinter : IBlockWriter
        {
            public void Write(TextWriter writer, Block block, CommonMarkSettings settings)
            {
                writer.Write("hello");
            }
        }

        public class OverriddenHtmlPrinter : ExtensibleHtmlBlockWriter
        {
            protected override void WriteParagraph(Block block)
            {
                WriteConstant("<span>");
                InlinesToHtml(block.InlineContent);
                WriteLineConstant("</span>");
            }

            protected override void WriteList(Block block)
            {
                EnsureLine();
                WriteLineConstant("<ul class=\"srn-priority\">");
                var child = block.FirstChild;

                while(child != null)
                {
                    EnsureLine();
                    WriteLineConstant("<li>" + child.FirstChild.InlineContent.LiteralContent + "</li>");
                    child = child.NextSibling;
                }
                WriteLineConstant("</ul>");
            }
        }
    }
}
