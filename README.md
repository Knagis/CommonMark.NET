# CommonMark.NET

Implementation of [CommonMark] [1] specification (passes tests from version 0.15) in C# for converting Markdown documents to HTML.

The current version of the library is also [available on NuGet] [nuget].

## Usage

To convert Markdown data in a stream or file:
```C#
using (var reader = new System.IO.StreamReader("source.md"))
using (var writer = new System.IO.StreamWriter("result.html"))
{
  CommonMark.CommonMarkConverter.Convert(reader, writer);
}
```

To convert data stored in a string:
```C#
var result = CommonMark.CommonMarkConverter.Convert("**Hello world!**");
```

See [issue #4] [issue4] for an example how the parser can be extended with additional logic.

**Important:** The converter [does not include any HTML sanitizing][XSS].

## Compatibility

The library uses no references except for `System` - it has no external dependencies. It is cross compiled to
both .NET 2.0 and .NET 4.0 Portable Class Library that targets .NET Framework 4.0, Silverlight 5, Windows 8,
Windows Phone 8.1, Windows Phone Silverlight 8, Xamarin.Android and Xamarin.iOS. A special .NET 4.5 build is
also created that hints certain optimizations to the runtime that were not available in earlier versions.

For working with the source code you will need Visual Studio 2013.3 or newer (Community edition is supported).

## Performance

Using a [simple tool][3] to compare the performance of various Markdown implementations for .NET yields the
following results:

     CommonMark.NET 0.4.1      4 ms   7%      

    CommonMarkSharp 0.3.2     30 ms   46%
       MarkdownSharp 1.13     55 ms   84%     (might not conform to CommonMark specification)
         MarkdownDeep 1.5      7 ms   11%     (might not conform to CommonMark specification)
	  MoonShine (sundown)      3 ms    6%     (wrapper for a native x86 .dll)
                 Baseline     65 ms   100%    (used to compare results on different machines)

This benchmark is very simple and tests the processing of the CommonMark specification document itself (a 
115 KB file). The results are provided just for a high-level comparison.

## Reliability

The parser uses algorithms that completely avoid recursion so even specifically crafted input documents
will not cause exceptions and will be rendered correctly. This is important because a `StackOverflowException`
cannot be caught and will bring down the entire process.

## References

This library is based on a port of the reference implementation in C, available on [jgm/stdm repo] [2]. 
It follows the same approach - the source is parsed into a syntax tree that can be used to add custom 
processing if the application needs it and then formatted into HTML.

## Running tests

All tests from `spec.txt` are merged into the unit testing project and can be executed from within Visual Studio.

The project also includes a slightly modified version of `runtests.pl` for compatibility with the original
implementation.

Note that if you run the `runtests.pl` script on a Windows machine it will incorrectly determine that some tests
have failed due to Unicode symbols that are not supported by the approach used in the perl script.

[1]: http://spec.commonmark.org/
[2]: https://github.com/jgm/stmd/commit/2cf0750a7a507eded4cf3c9a48fd1f924d0ce538
[3]: https://github.com/Knagis/CommonMarkBenchmark
[issue4]: https://github.com/Knagis/CommonMark.NET/issues/4
[XSS]: http://talk.commonmark.org/t/cross-site-scripting-issue-in-standard-markdown-example-at-try-standardmarkdown-com/55
[nuget]: https://www.nuget.org/packages/CommonMark.NET/