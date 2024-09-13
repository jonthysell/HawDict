// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text;
using System.Threading.Tasks;

namespace HawDict
{
    class Program
    {
        static DateTime StartTime
        {
            get
            {
                return _startTime ?? (_startTime = DateTime.Now).Value;
            }
        }
        private static DateTime? _startTime = null;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("{0} v{1}", AppInfo.Name, AppInfo.Version);

            string rootDir = Environment.CurrentDirectory;
            OutputFormats outputFormats = OutputFormats.All;

            if (args is not null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-f":
                        case "--format"
                            if (i + 1 > args.Length)
                            {
                                throw new Exception("Missing argument.");
                            }
                            outputFormats = Enum.TryParse<OutputFormats>(args[++i], out var result) ? result : outputFormats;
                            break;
                        default:
                            rootDir = args[i];
                            break;
                    }
                }
            }

            try
            {
                InputDictBase[] dictionaries = new InputDictBase[]
                {
                    new PukuiElbertInputDict(TranslationType.HawToEng, PrintLine),
                    new PukuiElbertInputDict(TranslationType.EngToHaw, PrintLine),
                    new MamakaKaiaoInputDict(TranslationType.HawToEng, PrintLine),
                    new MamakaKaiaoInputDict(TranslationType.EngToHaw, PrintLine),
                    new PlaceNamesInputDict(PrintLine),
                };

                Parallel.ForEach(dictionaries, (dict) =>
                {
                    try
                    {
                        dict.Process(rootDir, outputFormats);
                    }
                    catch (AggregateException ex)
                    {
                        PrintException(ex);
                        foreach (Exception innerEx in ex.InnerExceptions)
                        {
                            PrintException(innerEx);
                        }
                    }
                    catch (Exception ex)
                    {
                        PrintException(ex);
                    }
                });

            }
            catch (AggregateException ex)
            {
                PrintException(ex);
                foreach (Exception innerEx in ex.InnerExceptions)
                {
                    PrintException(innerEx);
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }

        static void PrintLine(string format, params object[] args)
        {
            TimeSpan elapsedTime = DateTime.Now - StartTime;
            Console.Out.WriteLine(string.Format("{0} > {1}", elapsedTime.ToString(@"hh\:mm\:ss"), string.Format(format, args)));
        }

        static void PrintException(Exception ex)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine();
            Console.Error.WriteLine("Error: {0}", ex.Message);
            Console.Error.WriteLine(ex.StackTrace);

            Console.ForegroundColor = oldColor;

            if (ex.InnerException is not null)
            {
                PrintException(ex.InnerException);
            }
        }
    }
}
