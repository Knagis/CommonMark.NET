using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class SourcePositionTests
    {
        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionStringNodes()
        {
            var data = "**foo** bar";
            var doc = Helpers.ParseDocument(data);

            var foo = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "foo");
            var bar = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == " bar");

            Assert.IsNotNull(foo);
            Assert.AreEqual("foo", data.Substring(foo.Inline.SourcePosition, foo.Inline.SourceLength));

            Assert.IsNotNull(bar);
            Assert.AreEqual(" bar", data.Substring(bar.Inline.SourcePosition, bar.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis1()
        {
            var data = "**foo**";
            var doc = Helpers.ParseDocument(data);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Strong);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis2()
        {
            var data = "***foo***";
            var doc = Helpers.ParseDocument(data);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Strong);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Emphasis);
            Assert.IsNotNull(inline);
            Assert.AreEqual("*foo*", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis3()
        {
            var data = @"**[foo* bar";
            var doc = Helpers.ParseDocument(data);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Emphasis);
            Assert.IsNotNull(inline);
            Assert.AreEqual("*[foo*", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            var str = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "*");
            Assert.IsNotNull(str);
            Assert.AreEqual("*", data.Substring(str.Inline.SourcePosition, str.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasis4()
        {
            var data = @"*[foo*** bar";
            var doc = Helpers.ParseDocument(data);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Emphasis);
            Assert.IsNotNull(inline);
            Assert.AreEqual("*[foo*", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            var str = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "**");
            Assert.IsNotNull(str);
            Assert.AreEqual("**", data.Substring(str.Inline.SourcePosition, str.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionEmphasisStringNodes()
        {
            var data = @"**[foo* bar
***double***
*[inlink*
**[inlink2*
****many***
*foo**
*[bar***
**first**
*second*
third";
            data = data.Replace("\r", "");
            var doc = Helpers.ParseDocument(data);

            var res = doc.AsEnumerable()
                .Where(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.String)
                .Select(o => new { o.Inline.LiteralContent, Source = data.Substring(o.Inline.SourcePosition, o.Inline.SourceLength) })
                .Where(o => o.Source != o.LiteralContent)
                .FirstOrDefault();

            Assert.IsNull(res, "String inline '" + (res == null ? string.Empty : res.LiteralContent) + "' has invalid position or length.");
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionBackticksMatch()
        {
            var data = @"``foo``";
            var doc = Helpers.ParseDocument(data);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Code);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }

        [TestMethod]
        [TestCategory("SourcePosition")]
        public void SourcePositionBackticksNoMatch()
        {
            var data = @"```foo``";
            var doc = Helpers.ParseDocument(data);

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "```");
            Assert.IsNotNull(inline);
            Assert.AreEqual("```", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));

            inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.LiteralContent == "``");
            Assert.IsNotNull(inline);
            Assert.AreEqual("``", data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }
    }
}
