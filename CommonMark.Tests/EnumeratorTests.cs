using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class EnumeratorTests
    {
        [TestMethod]
        [TestCategory("Enumerable iterator")]
        public void EnumeratorSimple()
        {
            var doc = Helpers.ParseDocument("> **foo**");

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Strong);
            Assert.IsNotNull(inline);
            Assert.IsTrue(inline.IsOpening);
            Assert.IsFalse(inline.IsClosing);
        }

        [TestMethod]
        [TestCategory("Enumerable iterator")]
        public void EnumeratorSimple2()
        {
            var doc = Helpers.ParseDocument("* > **foo** *bar*");

            var inlines = doc.AsEnumerable().Count(o => o.Inline != null && o.IsOpening);
            Assert.AreEqual(5, inlines);
        }

        [TestMethod]
        [TestCategory("Enumerable iterator")]
        public void EnumeratorNoEmptyNodes()
        {
            var doc = Helpers.ParseDocument("* > *[*foo** *bar*");

            var empty = doc.AsEnumerable().FirstOrDefault(o => 
                o.Inline != null 
                && o.Inline.Tag == Syntax.InlineTag.String 
                && string.IsNullOrEmpty(o.Inline.LiteralContent));
            
            Assert.IsNull(empty);
        }

        [TestMethod]
        [TestCategory("Enumerable iterator")]
        public void EnumeratorForChildElement()
        {
            // tests that the sibling elements for the enumarable root are not returned.
            var doc = Helpers.ParseDocument("foo\n\nbar");

            var fooNodes = doc.FirstChild.AsEnumerable();
            var foostring = fooNodes.FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.String && o.Inline.LiteralContent == "foo");

            // 3: open para, string foo, close para
            Assert.AreEqual(3, fooNodes.Count());
            Assert.IsNotNull(foostring);
        }

        [TestMethod]
        [TestCategory("Enumerable iterator")]
        public void EnumeratorSyncOpenClose()
        {
            var doc = Helpers.ParseDocument("* > **foo** *bar*\n\n\tqwe\n\n* * [asd](/url)");

            var openInl = doc.AsEnumerable().Count(o => o.Inline != null && o.IsOpening);
            var openBlo = doc.AsEnumerable().Count(o => o.Block != null && o.IsOpening);
            var closeInl = doc.AsEnumerable().Count(o => o.Inline != null && o.IsClosing);
            var closeBlo = doc.AsEnumerable().Count(o => o.Block != null && o.IsClosing);

            Assert.AreEqual(openInl, closeInl, "Inlines - opener and closer cound do not match.");
            Assert.AreEqual(openBlo, closeBlo, "Blocks - opener and closer cound do not match.");
        }
    }
}
