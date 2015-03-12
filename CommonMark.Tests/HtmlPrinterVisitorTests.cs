using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Formatter;
using CommonMark.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonMark.Tests
{
	[TestClass]
	public class HtmlPrinterVisitorTests
	{
		[TestCleanup]
		public void TestCleanup()
		{
			CommonMarkSettings.Default.HtmlPrinterVisitor = null; // for the other tests.
		}

		[TestMethod]
		[TestCategory("Inlines - visitor")]
		public void ShouldCallOnEnter()
		{
			// Arrange
			string markdown = "[![moon](moon.jpg)](/uri)";
			string expected = "<p>Enter text<a href=\"link-onenter-url\">Enter text<img src=\"image-onenter-url\" alt=\"moon\" /></a></p>";

			var htmlPrinterVisitorMock = new HtmlPrinterVisitorMock();
			htmlPrinterVisitorMock.EnterText = "Enter text";
			var settings = new CommonMarkSettings() { HtmlPrinterVisitor = htmlPrinterVisitorMock };

			// Act
			string actual = CommonMarkConverter.Convert(markdown, settings);

			// Assert
			Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
		}

		[TestMethod]
		[TestCategory("Inlines - visitor")]
		public void ShouldCallOnExit()
		{
			// Arrange
			string markdown = "[![moon](moon.jpg)](/uri)";
			string expected = "<p><a href=\"/uri\">Exit text<img src=\"moon.jpg\" alt=\"moon\" />Exit text</a></p>";

			var htmlPrinterVisitorMock = new HtmlPrinterVisitorMock();
			htmlPrinterVisitorMock.ExitText = "Exit text";
			var settings = new CommonMarkSettings() { HtmlPrinterVisitor = htmlPrinterVisitorMock };

			// Act
			string actual = CommonMarkConverter.Convert(markdown, settings);

			// Assert
			Assert.AreEqual(Helpers.Tidy(expected), Helpers.Tidy(actual));
		}
	}

	public class HtmlPrinterVisitorMock : IHtmlPrinterVisitor
	{
		// The text before every tag
		public string EnterText { get; set; }

		// The text after every tag
		public string ExitText { get; set; }

		public string OnEnter(Inline inline)
		{
			if (!string.IsNullOrEmpty(EnterText))
			{
				switch (inline.Tag)
				{
					case InlineTag.Image:
						inline.Linkable.Url = "image-onenter-url";
						break;

					case InlineTag.Link:
						inline.Linkable.Url = "link-onenter-url";
						break;
				}
			}

			return EnterText;
		}

		public string OnExit(Inline inline)
		{
			return ExitText;
		}
	}
}
