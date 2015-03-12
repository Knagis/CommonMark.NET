using System.IO;
using CommonMark.Syntax;

namespace CommonMark.Formatter
{
	public interface IHtmlPrinterVisitor
	{
		string OnEnter(Inline inline);
		string OnExit(Inline inline);
	}
}