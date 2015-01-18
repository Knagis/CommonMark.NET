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
        public void SourcePositionEmphasis1()
        {
            var data = "**foo**";
            var doc = Helpers.ParseDocument("**foo**");

            var inline = doc.AsEnumerable().FirstOrDefault(o => o.Inline != null && o.Inline.Tag == Syntax.InlineTag.Strong);
            Assert.IsNotNull(inline);
            Assert.AreEqual(data, data.Substring(inline.Inline.SourcePosition, inline.Inline.SourceLength));
        }
    }
}
