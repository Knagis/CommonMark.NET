using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;


namespace CommonMark
{
    class Program
    {
        static void PrintUsage(string invalidArg = null)
        {
            if (invalidArg != null)
            {
                Console.WriteLine("Invalid argument: " + invalidArg);
                Console.WriteLine();
            }

            Console.WriteLine("Usage:   CommonMark.Console [FILE*] [--out FILE]");
            Console.WriteLine("Options: --help, -h    Print usage information");
            Console.WriteLine("         --ast         Print AST instead of HTML");
            Console.WriteLine("         --bench 20    Execute a benchmark on the given input, optionally");
            Console.WriteLine("                       specify the number of iterations, default is 20");
            Console.WriteLine("         --perltest    Changes console input handling for runtests.pl");
            Console.WriteLine("         --version     Print version");
            Console.WriteLine("         --out         The output file; if not specified, stdout is used");
        }

        static int Main(string[] args)
        {
            var sources = new List<System.IO.TextReader>();
            var ast = false;
            var benchmark = false;
            var benchmarkIterations = 20;
            var target = Console.Out;
            var runPerlTests = false;

            try
            {
                for (var i = 0; i < args.Length; i++)
                {
                    if (string.Equals(args[i], "--version", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("CommonMark.NET {0}", CommonMarkConverter.Version);
                        Console.WriteLine(" - (c) 2014 Kārlis Gaņģis");
                        return 0;
                    }
                    else if ((string.Equals(args[i], "--help", StringComparison.OrdinalIgnoreCase)) ||
                             (string.Equals(args[i], "-h", StringComparison.OrdinalIgnoreCase)))
                    {
                        PrintUsage();
                        return 0;
                    }
                    else if (string.Equals(args[i], "--perltest", StringComparison.OrdinalIgnoreCase))
                    {
                        runPerlTests = true;
                    }
                    else if (string.Equals(args[i], "--ast", StringComparison.OrdinalIgnoreCase))
                    {
                        ast = true;
                    }
                    else if (string.Equals(args[i], "--bench", StringComparison.OrdinalIgnoreCase))
                    {
                        benchmark = true;
                        if (i != args.Length - 1)
                        {
                            if (!int.TryParse(args[i + 1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out benchmarkIterations))
                                benchmarkIterations = 20;
                            else
                                i++;
                        }
                    }
                    else if (string.Equals(args[i], "--out", StringComparison.OrdinalIgnoreCase))
                    {
                        if (i == args.Length - 1 || args[i + 1].StartsWith("-"))
                        {
                            PrintUsage();
                            return 1;
                        }

                        i++;
                        target = new System.IO.StreamWriter(args[i]);
                    }
                    else if (args[i].StartsWith("-"))
                    {
                        PrintUsage(args[i]);
                        return 1;
                    }
                    else
                    {
                        // treat as file argument
                        sources.Add(new System.IO.StreamReader(args[i]));
                    }
                }

                if (sources.Count == 0)
                {
                    if (runPerlTests)
                    {
                        Console.InputEncoding = Encoding.UTF8;
                        Console.OutputEncoding = Encoding.UTF8;

                        // runtests.pl writes directly to STDIN but will not send Ctrl+C and also
                        // will get confused by the additional information written to STDOUT
                        var sb = new StringBuilder();
                        while (Console.In.Peek() != -1)
                            sb.AppendLine(Console.In.ReadLine());

                        sources.Add(new System.IO.StringReader(sb.ToString()));
                    }
                    else if (!Console.IsInputRedirected)
                    {
                        Console.InputEncoding = Encoding.Unicode;
                        Console.OutputEncoding = Encoding.Unicode;

                        Console.WriteLine("Enter the source. Press Enter after the last line and" + Environment.NewLine + "then press Ctrl+C to run parser.");
                        Console.WriteLine();

                        Console.CancelKeyPress += (s, a) =>
                        {
                            a.Cancel = true;
                            Console.WriteLine("Output:");
                            Console.WriteLine();
                        };

                        sources.Add(Console.In);
                    }
                    else
                    {
                        Console.InputEncoding = Encoding.UTF8;
                        Console.OutputEncoding = Encoding.UTF8;

                        sources.Add(Console.In);
                    }
                }

                var settings = new CommonMarkSettings();
                settings.OutputFormat = ast ? OutputFormat.SyntaxTree : OutputFormat.Html;

                if (benchmark)
                {
                    Console.WriteLine("Running the benchmark...");
                    foreach (var source in sources)
                    {
                        // by using a in-memory source, the disparity of results is reduced.
                        var data = source.ReadToEnd();
                        
                        // in-memory source that gets reused further reduces the disparity.
                        var builder = new StringBuilder(2 * 1024 * 1024);

                        var sw = new System.Diagnostics.Stopwatch();
                        var mem = GC.GetTotalMemory(true);
                        long mem2 = 0;

                        for (var x = -1; x < benchmarkIterations; x++)
                        {
                            if (x == 0)
                                sw.Start();

                            builder.Length = 0;
                            using (var reader = new System.IO.StringReader(data))
                            using (var twriter = new System.IO.StringWriter(builder))
                                CommonMarkConverter.Convert(reader, twriter, settings);

                            if (mem2 == 0)
                                mem2 = GC.GetTotalMemory(false);

                            GC.Collect();
                        }

                        sw.Stop();
                        target.WriteLine("Time spent: {0:0.#}ms    Approximate memory usage: {1:0.000}MB",
                            (decimal)sw.ElapsedMilliseconds / benchmarkIterations,
                            (mem2 - mem) / 1024M / 1024M);
                    }
                }
                else
                {
                    foreach (var source in sources)
                        CommonMarkConverter.Convert(source, target, settings);
                }

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return -1;
            }
            finally
            {
                foreach (var s in sources)
                    if (s != Console.In)
                        s.Close();

                if (target != Console.Out)
                    target.Close();
            }
        }
    }
}
