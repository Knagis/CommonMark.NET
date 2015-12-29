using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class HeadingTests
    {
        [TestMethod]
        [TestCategory("Leaf blocks - ATX headings")]
        public void HeadingsAndThematicBreaks()
        {
            // see https://github.com/Knagis/CommonMark.NET/issues/60
            Helpers.ExecuteTest("##### A\n---\n\n##### B\n---\n\n##### C\n---", "<h5>A</h5>\n<hr />\n<h5>B</h5>\n<hr />\n<h5>C</h5>\n<hr />\n");
        }
    }
}
