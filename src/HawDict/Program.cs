// 
// Program.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2018 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
                return _startTime.HasValue ? _startTime.Value : (_startTime = DateTime.Now).Value;
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
