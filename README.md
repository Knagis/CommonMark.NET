# CommonMark.NET

Implementation of [CommonMark] [1] specification in C#.

This is a port of the reference implementation in C, available on [jgm/stdm repo] [2]. The implementation keeps the
same approach - the source is parsed into a syntax tree that can be used to add custom processing if the 
application needs it and then formatted into HTML.

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

**Important:** The converter [does not include any HTML sanitizing][XSS].

## Running tests

The project includes a slightly modified version of `runtests.pl` since the original did not work on Windows.
The main change is that it now ignores differences in newlines (`\n` and `\r\n` will be considered as equal).

Note that if you run the `runtests.pl` script on a Windows machine it will incorrectly determine that
4 tests have failed - if the same test is run manually by specifying both input and output files
(instead of using STDIN and STDOUT) it will provide the correct result.

[1]: http://commonmark.org/
[2]: https://github.com/jgm/stmd/commit/2cf0750a7a507eded4cf3c9a48fd1f924d0ce538
[XSS]: http://talk.commonmark.org/t/cross-site-scripting-issue-in-standard-markdown-example-at-try-standardmarkdown-com/55
