using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BBox.Analysis.Core.Logger;
using BBox.Analysis.Domain;
using BBox.Analysis.Interface;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ILogger = BBox.Analysis.Interface.ILogger;

namespace BBox.Analysis.Processing
{
    public class Registrar : IRegistrar
    {
        private string _outputPath;
        private IDictionary<String, FuelStation> _stations;
        private IDictionary<String, BonusCard> _bonusCards;
        private ISet<Tuple<FuelStation, DateTime, Int64>> _processedRecord;
        private IList<Tuple<String, Int64, String>> _invalidRecords;
        private ILogger _logger;

        public Registrar(String outputPath, ILogger logger)
        {
            _outputPath = outputPath;
            _stations = new Dictionary<string, FuelStation>();
            _bonusCards = new Dictionary<string, BonusCard>();
            _logger = logger;
            _processedRecord = new HashSet<Tuple<FuelStation, DateTime, long>>();
            _invalidRecords = new List<Tuple<string, long, string>>();
        }

        #region Implementation of IRegistrar

        public async Task RegisterShift(Shift shift)
        {
            var __shiftRegistrar = new ShiftRegistrar(_outputPath);
            var __task = Task.Run(() => {
                __shiftRegistrar.RegisterShiftSales(shift);
            });
            await __task;
        }

        public FuelStation GetFuelStation(String stationName)
        {
            FuelStation __fuelStation;
            if (_stations.TryGetValue(stationName, out __fuelStation))
                return __fuelStation;
            __fuelStation = new FuelStation {Name = stationName};
            _stations.Add(stationName, __fuelStation);
            return __fuelStation;
        }

        public void RegisterSummaryReport()
        {

            _logger.Write("Формирование сводных ведомостей");
            var __report = new SummaryReportRegistrar(_outputPath);
            var __stations = _stations.Values.ToArray();
            foreach (var __fuelStation in __stations)
            {
                foreach (var __fuelStationShift in __fuelStation.Shifts)
                {
                    try
                    {
                        _logger.Write(
                            $"BBOX_{__fuelStationShift.FuelStation.Name.Substring(6)} {__fuelStationShift.BeginDate:yyyy_MM_dd HH_mm_ss} _ {__fuelStationShift.EndDate:yyyy_MM_dd HH_mm_ss}.xlsx");
                        var __task = RegisterShift(__fuelStationShift);
                        __task.Wait();
                    }
                    catch (Exception __ex)
                    {
                        var __fileName =
                            $"BBOX_{__fuelStationShift.FuelStation.Name.Substring(6)} {__fuelStationShift.BeginDate:yyyy_MM_dd HH_mm_ss} _ {__fuelStationShift.EndDate:yyyy_MM_dd HH_mm_ss}.xlsx";
                        LogManager.GetInstance()
                            .GetLogger("BBox.Analysis")
                            .Error($"Ошибка формирования файла: {__fileName}. \r\n Данные отчета не корректны. \r\n",
                                __ex);
                        _logger.Write($"Ошибка формирования файла: {__fileName}.");
                        _logger.Write("Данные отчета не корректны.");
                        _logger.Write($"{__ex}");
                    }
                }
                _logger.Write(__fuelStation.Name);
                try
                {
                    __report.RegisterSummaryReport(__fuelStation);
                    Thread.Sleep(1);
                }
                catch (Exception __ex)
                {
                    var __fileName = $"Сводный отчет {__fuelStation.Name}.xlsx";
                    LogManager.GetInstance()
                        .GetLogger("BBox.Analysis")
                        .Error($"Ошибка формирования файла: {__fileName}. \r\n Данные отчета не корректны. \r\n", __ex);
                    _logger.Write($"Ошибка обработки файла: {__fileName}.");
                    _logger.Write("Данные отчета не корректны.");
                    _logger.Write($"{__ex}");
                }

            }
        }

        private void FillBonusesSummaryReport(ISheet sheet)
        {
            var __cards = _bonusCards.Values.Select(x => new
            {
                x.CardNo,
                Charged = x.Movements.Where(z => z.Operation == BonusesOperation.ChargeBonuses).Sum(y => y.Bonuses),
                Cancellation =
                x.Movements.Where(z => z.Operation == BonusesOperation.CancellationBonuses).Sum(y => y.Bonuses),
            }).OrderBy(x => x.CardNo).ToList();
            var __rowNum = 3;
            foreach (var __card in __cards)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__card.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(Convert.ToDouble(__card.Charged));
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(Convert.ToDouble(__card.Cancellation));
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(Convert.ToDouble(__card.Charged - __card.Cancellation));
                __rowNum++;
            }

        }

        public void RegisterSummaryBonusReport()
        {
            _logger.Write("Формирование сводной ведомости по бонусам");
            try
            {
                var __path = Path.Combine(".\\Resources", "Бонусы сводный Шаблон.xlsx");
                IWorkbook __book;
                using (var __templateStream = File.OpenRead(__path))
                {
                    __book = new XSSFWorkbook(__templateStream);

                    FillBonusesSummaryReport(__book.GetSheetAt(0));
                }

                using (
                    var __resultStream =
                        File.OpenWrite(Path.Combine(_outputPath,
                            $"Бонусы сводный отчет.xlsx"))
                )
                {
                    __book.Write(__resultStream);
                    __book.Close();
                }
            }
            catch (Exception __ex)
            {
                var __fileName = $"Бонусы сводный отчет.xlsx";
                LogManager.GetInstance()
                    .GetLogger("BBox.Analysis")
                    .Error($"Ошибка формирования файла: {__fileName}. \r\n Данные отчета не корректны. \r\n", __ex);
                _logger.Write($"Ошибка обработки файла: {__fileName}.");
                _logger.Write("Данные отчета не корректны.");
                _logger.Write($"{__ex}");
            }

        }

        public bool IsProcessedRecord(FuelStation station, Record record)
        {
            var __result =
                _processedRecord.Contains(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            if (!__result)
            {
                _processedRecord.Add(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            }
            return __result;
        }

        public void UnProcessedRecord(FuelStation station, Record record)
        {
            var __result =
                _processedRecord.Contains(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            if (__result)
            {
                _processedRecord.Remove(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            }
        }

        public void SetInvalidInfo(string fileName, long position, string reason)
        {
            _invalidRecords.Add(new Tuple<string, long, string>(fileName,position,reason));
        }

        public void RegisterInvalidRecordReport()
        {
            _logger.Write("Формирование отчета по валидности данных");
            try
            {
                var __path = Path.Combine(".\\Resources", "Шаблон валидности.xlsx");
                IWorkbook __book;
                using (var __templateStream = File.OpenRead(__path))
                {
                    __book = new XSSFWorkbook(__templateStream);

                    FillInvalidRecordReport(__book.GetSheetAt(0));
                }

                using (
                    var __resultStream =
                        File.OpenWrite(Path.Combine(_outputPath,
                            $"Отчет по валидности данных.xlsx"))
                )
                {
                    __book.Write(__resultStream);
                    __book.Close();
                }
            }
            catch (Exception __ex)
            {
                var __fileName = $"Отчет по валидности данных.xlsx";
                LogManager.GetInstance()
                    .GetLogger("BBox.Analysis")
                    .Error($"Ошибка формирования файла: {__fileName}. \r\n Данные отчета не корректны. \r\n", __ex);
                _logger.Write($"Ошибка обработки файла: {__fileName}.");
                _logger.Write("Данные отчета не корректны.");
                _logger.Write($"{__ex}");
            }
        }

        private void FillInvalidRecordReport(ISheet sheet)
        {
            var __rowNum = 2;
            foreach (var __record in _invalidRecords)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__record.Item1);
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__record.Item2);
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__record.Item3);
                __rowNum++;
            }
        }

        public void RegisterBonusReport()
        {

            var __report = new BonusReportRegistrar(_outputPath);
            var __cards = _bonusCards.Values.ToArray();
            _logger.Write($"Формирование ведомостей по бонусам. Количество {__cards.Length}");
            for (var __i = 0; __i < __cards.Length; __i++)
            {
                try
                {
                    _logger.Write($"Формирование ведомости № {__cards[__i].CardNo}. {__i} из {__cards.Length}");
                    __report.RegisterBonusReport(__cards[__i]);
                    Thread.Sleep(1);
                }
                catch (Exception __ex)
                {
                    var __fileName = $"Бонусы карта № {__cards[__i].CardNo}.xlsx";
                    LogManager.GetInstance().GetLogger("BBox.Analysis").Error($"Ошибка формирования файла: {__fileName}. \r\n Данные отчета не корректны. \r\n", __ex);
                    _logger.Write($"Ошибка обработки файла: {__fileName}.");
                    _logger.Write("Данные отчета не корректны.");
                    _logger.Write($"{__ex}");
                }
               
            }
            //Parallel.For(0, __cards.Length, (ind) =>
            //{
            //    _logger.Write($"Формирование ведомости № {__cards[ind].CardNo}. {ind} из {__cards.Length}");
            //    __report.RegisterBonusReport(__cards[ind]);
            //    Thread.Sleep(1);
            //});
            
        }

        public BonusCard GetBonusCard(String cardNo)
        {
            BonusCard __card;
            if (_bonusCards.TryGetValue(cardNo, out __card))
                return __card;
            __card = new BonusCard(cardNo);
            _bonusCards.Add(cardNo, __card);
            return __card;
        }
        #endregion
    }
}
