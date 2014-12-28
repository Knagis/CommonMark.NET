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
    }
}
