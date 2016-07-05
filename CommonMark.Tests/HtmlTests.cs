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

        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        public void HtmlTagAttributes()
        {
            Helpers.ExecuteTest(
                "foo <a href=\"`~1!@#$%^&*()-_=+{}[];:'\\|/.,><čā\uD83D\uDF13\">asd</a> bar",
                "<p>foo <a href=\"`~1!@#$%^&*()-_=+{}[];:'\\|/.,><čā\uD83D\uDF13\">asd</a> bar</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        public void HtmlTagAttributesLink()
        {
            Helpers.ExecuteTest(
                "foo <a href=\"http://foo.baz\">asd</a> bar",
                "<p>foo <a href=\"http://foo.baz\">asd</a> bar</p>");
        }

        [TestMethod]
        [TestCategory("Inlines - Raw HTML")]
        public void HtmlSingleOpenBrace()
        {
            // https://github.com/Knagis/CommonMark.NET/issues/85
            Helpers.ExecuteTest(
                "bar\r\nfoo<",
                "<p>bar\r\nfoo&lt;</p>");
        }

        /// <summary>
        /// Tests HTML block tag names of various lengths (see https://github.com/Knagis/CommonMark.NET/issues/16)
        /// </summary>
        [TestMethod]
        [TestCategory("Leaf blocks - HTML blocks")]
        public void HtmlTagNameLengths()
        {
            var source = "";
            var result = "";
            foreach (var tag in new[] { "p", "h1", "map", "form", "style", "object", "section", "progress", "progress2", "blockquote", "blockquoteX" })
            {
                source += "<" + tag + ">\n\t*" + tag + "*\n</" + tag + ">\n\n";
                result += "<" + tag + ">\n\t*" + tag + "*\n</" + tag + ">\n";
            }

            Helpers.ExecuteTest(source, result);
        }
    }
}
