using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        [TestCategory("Container blocks - Lists")]
        public void Example210WithPositionTracking()
        {
            // Example 210 handles case that relies on Block.SourcePosition property (everything else just sets it)

            var s = CommonMarkSettings.Default.Clone();
            s.TrackSourcePosition = true;

            Helpers.Log("Example {0}" + Environment.NewLine + "Section: {0}" + Environment.NewLine, 210, "Container blocks - Lists");
            Helpers.ExecuteTest("* a\n*\n\n* c", "<ul>\n<li>\n<p>a</p>\n</li>\n<li></li>\n<li>\n<p>c</p>\n</li>\n</ul>", s);
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UnicodeBulletEscape()
        {
            Helpers.ExecuteTest("\\• foo\n\n\\* bar", "<p>• foo</p>\n<p>* bar</p>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void UnicodeBulletList()
        {
            Helpers.ExecuteTest("• foo\n• bar", "<ul>\n<li>foo</li>\n<li>bar</li>\n</ul>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void EmptyList1()
        {
            Helpers.ExecuteTest("1.\n2.", "<ol>\n<li></li>\n<li></li>\n</ol>");
        }

        [TestMethod]
        [TestCategory("Container blocks - List items")]
        public void EmptyList2()
        {
            Helpers.ExecuteTest("+\n+", "<ul>\n<li></li>\n<li></li>\n</ul>");
        }
    }
}
