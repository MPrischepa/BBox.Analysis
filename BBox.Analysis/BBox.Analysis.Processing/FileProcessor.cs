using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using BBox.Analysis.Core;
using BBox.Analysis.Core.Logger;
using BBox.Analysis.Domain;
using BBox.Analysis.Interface;
using ILogger = BBox.Analysis.Interface.ILogger;

namespace BBox.Analysis.Processing
{
    public class FileProcessor
    {
        private readonly ILogger _logger;
        private readonly IRegistrar _registrar;

        private void WriteLog(String text)
        {
            _logger?.Write(text);
        }



        public FileProcessor(ILogger logger, IRegistrar registrar)
        {
            _logger = logger;
            _registrar = registrar;
        }

        public Record ProcessRecord(String record)
        {
            var __spl = record.Split(';');
            for (var __i = 0; __i < __spl.Length; __i++)
            {
                __spl[__i] = __spl[__i].Trim();
            }
            if (__spl.Count(x => !String.IsNullOrWhiteSpace(x)) < 3) return null;
            var __record = new Record
            {
                FuelStationName = __spl[0],
                TimeRecord = DateTime.Parse(__spl[1]),
                ID = Int64.Parse(__spl[2]),
                Entry = new string[__spl.Length - 3]
            };
            Array.Copy(__spl, 3, __record.Entry, 0, __record.Entry.Length);
            return __record;
        }


        private DateTime TranslateDate(String date)
        {
            var __year = Int16.Parse(date.Substring(1, 4));
            var __month = Int16.Parse(date.Substring(6, 2));
            var __day = Int16.Parse(date.Substring(9, 2));
            var __hour = Int16.Parse(date.Substring(12, 2));
            var __min = Int16.Parse(date.Substring(15, 2));
            var __sec = Int16.Parse(date.Substring(18, 2));

            return new DateTime(__year, __month, __day, __hour, __min, __sec);
                //DateTime.ParseExact(date.Trim(), "yyyy_MM_dd HH_mm_ss", new CultureInfo("ru-RU"));
        }

        public void ProcessFile(String fileName)
        {
            var __filePattern =
                "BBOX_\\d+\\s\\d{4}\\D\\d{2}\\D\\d{2}\\s\\d{2}\\D\\d{2}\\D\\d{2}\\s\\D\\s\\d{4}\\D\\d{2}\\D\\d{2}\\s\\d{2}\\D\\d{2}\\D\\d{2}";
            var __isMatch = new Regex(__filePattern).IsMatch(fileName);
            if (!__isMatch)
            {
                WriteLog($"Файл {fileName} не обработан. Причина: не верный формат заголовка");
            }
            var __fileName = new Regex(__filePattern).Match(fileName).Value;
            var __fuelStationReg = new Regex("BBOX_\\d+\\s");
            var __fuelStationName = Int16.Parse(__fuelStationReg.Match(fileName).Value.Substring(5));
            var __shiftDatePattern = "\\s\\d{4}\\D\\d{2}\\D\\d{2}\\s\\d{2}\\D\\d{2}\\D\\d{2}";
            var __shiftPeriodMatches = new Regex(__shiftDatePattern).Matches(__fileName);
            //var __startDate = TranslateDate(__shiftPeriodMatches[0].Value);
            var __endDate = TranslateDate(__shiftPeriodMatches[1].Value);
            var __fuelStation = GetFuelStation($"АЗС № {__fuelStationName}");
            using (var __reader = new StreamReader(fileName, Encoding.GetEncoding(1251)))
            {

                if (__reader.EndOfStream) return;
                string __line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                var __processedLine = String.Empty;
                Int64? __positionNo = null;
                while ((__line = __reader.ReadLine()) != null)
                {
                    var __unProcessed = false;
                    if (!new Regex(".+;\\d{4}-\\d{2}-\\d{2}.+;").IsMatch(__line))
                    {
                        __processedLine = $"{__processedLine}{__line}";
                        __unProcessed = true;
                    }
                    else
                    {
                        __processedLine = __line;
                    }
                    try
                    {
                        var __record = ProcessRecord(__processedLine);
                        if (__unProcessed && __record != null) UnProcessedRecord(__fuelStation,__record);
                        if (__record == null || IsProcessedRecord(__fuelStation, __record)) continue;
                        if (__positionNo.HasValue && __positionNo.Value != __record.ID - 1)
                        {
                            SetInvalidRecord(__fileName, __record.ID,
                                $"Нарушение порядка следования записей: Предыдущая запись: {__positionNo}, текущая запись {__record.ID}");
                        }
                        __positionNo = __record.ID;
                        if (BlackBoxObject.ProcessRecord(__fuelStation, __record) == ProcessingResult.DontProcessing)
                            WriteLog($"{DateTime.Now}: Обработка: {__processedLine} : Не обработанно");
                    }
                    catch (Exception __ex)
                    {
                        LogManager.GetInstance().GetLogger("BBox.Analysis").Error($"Ошибка обработки файла: {fileName}. \r\n Не обработана строка: {__processedLine}. \r\n Данные отчета не корректны. \r\n",__ex);
                        WriteLog($"Ошибка обработки файла: {fileName}.");
                        WriteLog($"Не обработана строка: { __processedLine}.");
                        WriteLog("Данные отчета не корректны.");
                        WriteLog($"{__ex}");
                        File.Copy(fileName,Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Errors",__fileName+".TXT"),true);
                        break;
                    }
                }

                if (BlackBoxObject.ProcessRecord(__fuelStation, new Record
                {
                    Entry = new[] {"Смена закрыта"},
                    TimeRecord = __endDate,
                    ID = Int64.MaxValue,
                    FuelStationName = __fuelStation.Name
                }) == ProcessingResult.DontProcessing)
                    WriteLog($"{DateTime.Now}: Обработка: {__line} : Не обработанно");


            }
            //var __task = _registrar.RegisterShift(__fuelStation.CurrentShift);
            // __task.Wait();
        }

        private void UnProcessedRecord(FuelStation station, Record record)
        {
            _registrar.UnProcessedRecord(station,record);
        }
        private bool IsProcessedRecord(FuelStation station, Record record)
        {
            return _registrar.IsProcessedRecord(station, record);
        }
        private FuelStation GetFuelStation(string s)
        {
            return _registrar.GetFuelStation(s);
        }

        private void SetInvalidRecord(String fileName, Int64 positionNo, String reason)
        {
            _registrar.SetInvalidInfo(fileName,positionNo,reason);
        }
    }
}
