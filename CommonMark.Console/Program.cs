using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;


namespace CommonMark
{
    class Program
    {
        static void print_usage()
        {
            Console.WriteLine("Usage:   CommonMark.Console [FILE*] [--out FILE]");
            Console.WriteLine("Options: --help, -h    Print usage information");
            Console.WriteLine("         --ast         Print AST instead of HTML");
            Console.WriteLine("         --version     Print version");
            Console.WriteLine("         --out         The output file; if not specified, stdout is used");
        }

        static int Main(string[] args)
        {
            try
            {
                var argc = args.Length;
                int i;
                bool ast = false;
                List<string> files = new List<string>();
                string target = null;

                for (i = 0; i < argc; i++)
                {
                    if (string.Equals(args[i], "--version", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("CommonMark.NET {0}", CommonMarkConverter.AssemblyVersion);
                        Console.WriteLine(" - (c) 2014 Kārlis Gaņģis");
                        return 0;
                    }
                    else if ((string.Equals(args[i], "--help", StringComparison.OrdinalIgnoreCase)) ||
                             (string.Equals(args[i], "-h", StringComparison.OrdinalIgnoreCase)))
                    {
                        print_usage();
                        return 0;
                    }
                    else if (string.Equals(args[i], "--ast", StringComparison.OrdinalIgnoreCase))
                    {
                        ast = true;
                    }
                    else if (string.Equals(args[i], "--out", StringComparison.OrdinalIgnoreCase))
                    {
                        if (i == argc - 1 || args[i + 1].StartsWith("-"))
                        {
                            print_usage();
                            return 1;
                        }

                        i++;
                        target = args[i];
                    }
                    else if (args[i].StartsWith("-"))
                    {
                        print_usage();
                        return 1;
                    }
                    else
                    {
                        // treat as file argument
                        files.Add(args[i]);
                    }
                }

                var settings = new CommonMarkSettings();
                settings.OutputFormat = ast ? OutputFormat.SyntaxTree : OutputFormat.Html;

                System.IO.TextWriter writer = target == null ? Console.Out : new System.IO.StreamWriter(target);

                try
                {
                    if (files.Count == 0)
                    {
                        // read from stdin
                        if (!Console.IsInputRedirected)
                        {
                            Console.WriteLine("Only redirected STDIN is supported.");
                            print_usage();
                            return 2;
                        }

                        CommonMarkConverter.Convert(Console.In, writer, settings);
                    }
                    else
                    {
                        // iterate over input file pointers
                        for (var g = 0; g < files.Count; g++)
                        {
                            using (var reader = new System.IO.StreamReader(files[g]))
                            {
                                CommonMarkConverter.Convert(reader, writer, settings);
                            }
                        }
                    }
                }
                finally
                {
                    if (writer != Console.Out)
                        writer.Close();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return -1;
            }
        }
    }
}
