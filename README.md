CommonMark.NET
==============

Implementation of [CommonMark] [1] specification in C#.

This is a port of the reference implementation in C, available on [GitHub] [2]. The current version 
passes all tests available in the specification version 2cf0750a7a. The implementation follows the
same approach - the source is parsed into a syntax tree that can be used to add custom processing if
the application needs it and then formatted into HTML.

Note that if you run the `runtests.pl` script on a Windows machine it will incorrectly determine that
5 tests have failed - if the same test is run manually by specifying both input and output files
(instead of using STDIN and STDOUT) it will provide the correct result.

[1]: http://commonmark.org/
[2]: https://github.com/jgm/stmd
