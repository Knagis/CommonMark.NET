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

        public class HelloPrinter : IPrinter
        {
            public void Print(TextWriter writer, Block block, CommonMarkSettings settings)
            {
                writer.Write("hello");
            }
        }
    }
}
