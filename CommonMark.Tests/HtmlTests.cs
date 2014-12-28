using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class HtmlTests
    {
        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        public void HtmlTagsWithDigits()
        {
            Helpers.ExecuteTest("foo <h1>asd</h1> bar", "<p>foo <h1>asd</h1> bar</p>");
        }
    }
}
