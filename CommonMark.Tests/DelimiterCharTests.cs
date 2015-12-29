using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class DelimiterCharTests
    {
        public DelimiterCharTests()
        {
        }

        [TestMethod]
        public void EmphasisDelimiterChar()
        {
            var md = "*foo*";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('*', doc.FirstChild.InlineContent.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void EmphasisDelimiterChar2()
        {
            var md = "_foo_";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('_', doc.FirstChild.InlineContent.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void EmphasisDelimiterChar3()
        {
            var md = "_foo[bar_";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('_', doc.FirstChild.InlineContent.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void EmphasisDelimiterChar4()
        {
            var md = "*foo[bar*";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('*', doc.FirstChild.InlineContent.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void DoubleEmphasisDelimiterChar()
        {
            var md = "_*foo*_";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('*', doc.FirstChild.InlineContent.FirstChild.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void DoubleEmphasisDelimiterChar2()
        {
            var md = "*_foo_*";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('_', doc.FirstChild.InlineContent.FirstChild.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void DoubleEmphasisDelimiterChar3()
        {
            var md = "_*foo[bar*_";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('*', doc.FirstChild.InlineContent.FirstChild.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void DoubleEmphasisDelimiterChar4()
        {
            var md = "*_foo[bar_*";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('_', doc.FirstChild.InlineContent.FirstChild.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void DoubleEmphasisDelimiterChar5()
        {
            var md = "_*foo[bar*baz_";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('*', doc.FirstChild.InlineContent.FirstChild.Emphasis.DelimiterCharacter);
        }

        [TestMethod]
        public void DoubleEmphasisDelimiterChar6()
        {
            var md = "*_foo[bar_baz*";
            var doc = Helpers.ParseDocument(md);
            Assert.AreEqual('_', doc.FirstChild.InlineContent.FirstChild.Emphasis.DelimiterCharacter);
        }
    }
}
