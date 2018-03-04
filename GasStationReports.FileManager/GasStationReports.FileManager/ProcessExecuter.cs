using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GasStationReports.FileManager.Configuration;

namespace GasStationReports.FileManager
{
    public class ProcessExecuter
    {
        static ProcessExecuter _instance;

        private ProcessExecuter()
        {

        }

        public static ProcessExecuter Instance => _instance ?? (_instance = new ProcessExecuter());

        public void Execute(ApplicationElement application)
        {
            var __process = new Process();
            try
            {
                __process.StartInfo.FileName = application.FileName.Value;
                if (application.Arguments != null && application.Arguments.Count > 0)
                    __process.StartInfo.Arguments = application.Arguments.Cast<StringElement>()
                        .Aggregate("", (result, value) =>
                        {
                            if (result == null) throw new ArgumentNullException(nameof(result));
                            return $"{result} {value.Value}";
                        });
                Console.WriteLine($"Выполняется {__process.StartInfo.FileName} {__process.StartInfo.Arguments}");
                __process.Start();
                __process.WaitForExit();
            }
            catch (Exception __ex)
            {
                Console.WriteLine(__ex);
                throw;
            }
        }
    }
}
