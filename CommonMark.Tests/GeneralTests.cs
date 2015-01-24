using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
    [TestClass]
    public class GeneralTests
    {
        /// <summary>
        /// Verifies that the U+0000 characters are removed from the source data.
        /// </summary>
        [TestMethod]
        [TestCategory("Security")]
        public void TestZeroCharRemoval()
        {
            Helpers.ExecuteTest("\u0000*foo*\0", "<p><em>foo</em></p>");
        }

        /// <summary>
        /// Verifies that the U+0000 characters are removed from the source data.
        /// </summary>
        [TestMethod]
        [TestCategory("Other")]
        public void Version()
        {
            var version = CommonMarkConverter.Version;

            Assert.AreEqual(0, version.Major, "The version number is incorrect: {0}", version);
            Assert.IsTrue(version.Minor > 5, "The version number is incorrect: {0}", version);
        }
    }
}
