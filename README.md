[![Build status](https://ci.appveyor.com/api/projects/status/9jo20450utqy1p0o/branch/master?svg=true)](https://ci.appveyor.com/project/Knagis/commonmark-net/branch/master)

# CommonMark.NET

Implementation of [CommonMark] [1] specification (passes tests from version 0.23) in C# for converting Markdown documents to HTML.

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

See [wiki] [extensibility] for an example how the parser can be extended with additional logic.

**Important:** The converter [does not include any HTML sanitizing][XSS].

## Compatibility

The library uses no references except for `System` - it has no external dependencies. It is cross compiled to:

* .NET Framework 2.0 
* .NET Framework 3.5 Client Profile
* .NET Framework 4.0 Client Profile
* .NET 4.0 Portable Class Library
    * .NET Framework 4.0 Client Profile
    * Silverlight 5
    * Windows 8
    * Windows Phone 8.1
    * Windows Phone Silverlight 8
    * Xamarin.Android
    * Xamarin.iOS
* .NET 4.5 Portable Class Library (Optimized)
    * .NET Framework 4.5
    * Windows 8
    * Windows Phone 8.1
    * Windows Phone Silverlight 8
    * Xamarin.Android
    * Xamarin.iOS
* .NET Framework 4.5 (Optimized)
* .NET Framework 5.0 Core (also known as ASP.NET vNext Core CLR)

For working with the source code you will need Visual Studio 2015 or newer (Community edition is supported).

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

This library is based on a port of the reference implementation in C, available on [jgm/cmark repo] [2]. 
It follows the same approach - the source is parsed into a syntax tree that can be used to add custom 
processing if the application needs it and then formatted into HTML.

## Running tests

All tests from `spec.txt` are merged into the unit testing project and can be executed from within Visual Studio.

The project also includes a slightly modified version of `runtests.pl` for compatibility with the original
implementation. Use `CommonMark.Console.exe --perltest` as the argument to `runtests.pl` so that the
application can correctly handle input from the Perl script.

[1]: http://spec.commonmark.org/
[2]: https://github.com/jgm/cmark
[3]: https://github.com/Knagis/CommonMarkBenchmark
[extensibility]: https://github.com/Knagis/CommonMark.NET/wiki
[XSS]: http://talk.commonmark.org/t/cross-site-scripting-issue-in-standard-markdown-example-at-try-standardmarkdown-com/55
[nuget]: https://www.nuget.org/packages/CommonMark.NET/
