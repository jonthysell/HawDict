// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Text;

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

            string rootDir = (null != args && args.Length > 0) ? args[0] : Environment.CurrentDirectory;

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

                foreach (InputDictBase dictionary in dictionaries)
                {
                    dictionary.Process(rootDir);
                }
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

            if (null != ex.InnerException)
            {
                PrintException(ex.InnerException);
            }
        }
    }
}
