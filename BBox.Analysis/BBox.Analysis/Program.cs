using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BBox.Analysis.Core.Logger;
using BBox.Analysis.Processing;
using ILogger = BBox.Analysis.Interface.ILogger;

namespace BBox.Analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var __inDirectory = ReadSetting("InFileDirectory");
                if (String.IsNullOrWhiteSpace(__inDirectory))
                {
                    Thread.Sleep(5000);
                    return;
                }
                if (!Directory.Exists(__inDirectory))
                {
                    Console.WriteLine($"Каталог входящих файлов {Path.GetFullPath(__inDirectory)}: не существует");
                    Thread.Sleep(5000);
                    return;
                }
                __inDirectory = Path.GetFullPath(__inDirectory);
                Console.WriteLine($"Каталог входящих файлов {__inDirectory}");
                var __searchPattern = ReadSetting("FileSearchPattern") ?? "BBOX*.txt";
                Console.WriteLine($"Строка поиска файлов: {__searchPattern}");
                var __outDirectory = ReadSetting("OutFileDirectory");
                if (String.IsNullOrWhiteSpace(__outDirectory))
                    __outDirectory = __inDirectory;
                if (!Directory.Exists(__outDirectory))
                    Directory.CreateDirectory(__outDirectory);
                Console.WriteLine($"Каталог исходящих файлов {Path.GetFullPath(__outDirectory)}");
                var __logger = new ConsoleLogger();
                var __processing =
                    new BlackBoxProcessingManager(__inDirectory).WithSearchTemplate(__searchPattern)
                        .WithLogger(__logger)
                        .WithRegistrar(new Registrar(__outDirectory, __logger));
                __processing.Process();
                Thread.Sleep(5000);
            }
            catch (Exception __ex)
            {
                LogManager.GetInstance().GetLogger("Application").Fatal("Не обработанное исключение",__ex);
                throw;
            }
           

        }

        static String ReadSetting(string key)
        {
            try
            {
                var __appSettings = ConfigurationManager.AppSettings;
                var __result = __appSettings[key] ?? String.Empty;
                Console.WriteLine(__result);
                return __result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return String.Empty;
        }

        class ConsoleLogger : ILogger
        {
            #region Implementation of ILogger

            public void Write(string text)
            {
                Console.WriteLine(text);
            }

            #endregion
        }
    }
}
