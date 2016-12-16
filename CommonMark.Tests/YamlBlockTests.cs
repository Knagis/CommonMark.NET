using Microsoft.VisualStudio.TestTools.UnitTesting;

using CommonMark.Syntax;

namespace CommonMark.Tests
{
    [TestClass]
    public class YamlBlockTests
    {
        private const string Category = "Container blocks - YAML";

        private static CommonMarkSettings GetSettings(bool frontMatterOnly = false)
        {
            var settings = CommonMarkSettings.Default.Clone();
            settings.AdditionalFeatures = frontMatterOnly
                ? CommonMarkAdditionalFeatures.YamlFrontMatterOnly
                : CommonMarkAdditionalFeatures.YamlBlocks;
            return settings;
        }

        [TestMethod]
        [TestCategory(Category)]
        public void YamlDisabled()
        {
            Helpers.ExecuteTest(
                "---\nparagraph\n...",
                "<hr />\n<p>paragraph\n...</p>");
        }

        [TestMethod]
        [TestCategory(Category)]
        public void YamlEmpty()
        {
            Helpers.ExecuteTest(
                "---\n\n...",
                "<pre><code class=\"language-yaml\">\n</code></pre>",
                GetSettings());
        }

        [TestMethod]
        [TestCategory(Category)]
        public void YamlFrontMatterOnly()
        {
            Helpers.ExecuteTest(
                "---\nfrontmatter\n...\n\nparagraph\n\n---\nline 1\nline 2\n...\n",
                "<pre><code class=\"language-yaml\">frontmatter\n</code></pre>\n" +
                "<p>paragraph</p>\n" +
                "<hr />\n" +
                "<p>line 1\nline 2\n...</p>",
                GetSettings(true));
        }

        [TestMethod]
        [TestCategory(Category)]
        public void YamlMultipleBlocks()
        {
            Helpers.ExecuteTest(
                "---\nfrontmatter\n...\n\nparagraph\n\n---\nline 1\nline 2\n...\n",
                "<pre><code class=\"language-yaml\">frontmatter\n</code></pre>\n" +
                "<p>paragraph</p>\n" +
                "<pre><code class=\"language-yaml\">line 1\nline 2\n</code></pre>",
                GetSettings());
        }

        [TestMethod]
        [TestCategory(Category)]
        public void YamlAndThematicBreak()
        {
            Helpers.ExecuteTest(
                "----\n\nnot yaml\n---\nalso not yaml\n\n---\nbut this\nis\nyaml\n...\npara",
                "<hr />\n" +
                "<h2>not yaml</h2>\n" +
                "<p>also not yaml</p>\n" +
                "<pre><code class=\"language-yaml\">but this\nis\nyaml\n</code></pre>\n" +
                "<p>para</p>",
                GetSettings());
        }

        [TestMethod]
        [TestCategory(Category)]
        public void YamlClosingFenceDash()
        {
            AssertYamlClosingFenceAndAst("---");
        }

        [TestMethod]
        [TestCategory(Category)]
        public void YamlClosingFenceDot()
        {
            AssertYamlClosingFenceAndAst("...");
        }

        private static void AssertYamlClosingFenceAndAst(string fence)
        {
            var markdown = "---\nyaml\n" + fence;
            Helpers.ExecuteTest(
                markdown,
                "<pre><code class=\"language-yaml\">yaml\n</code></pre>",
                GetSettings());

            var doc = CommonMarkConverter.Parse(markdown, GetSettings());

            Assert.IsNotNull(doc.FirstChild);
            Assert.AreEqual(BlockTag.YamlBlock, doc.FirstChild.Tag);
            Assert.IsNotNull(doc.FirstChild.FencedCodeData);
            Assert.AreEqual(0, doc.FirstChild.FencedCodeData.FenceOffset);
            Assert.AreEqual(-1, doc.FirstChild.FencedCodeData.FenceLength);
            Assert.AreEqual(fence[0], doc.FirstChild.FencedCodeData.FenceChar);
            Assert.IsNull(doc.FirstChild.FencedCodeData.Info);
        }
    }
}