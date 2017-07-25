using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BBox.Analysis.Domain;
using BBox.Analysis.Interface;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BBox.Analysis.Processing
{
    public class Registrar: IRegistrar
    {
        private string _outputPath;
        private IDictionary<String, FuelStation> _stations;
        private IDictionary<String, BonusCard> _bonusCards;
        private ISet<Tuple<FuelStation, DateTime, Int64>> _processedRecord;
        private ILogger _logger;

        public Registrar(String outputPath, ILogger logger)
        {
            _outputPath = outputPath;
            _stations = new Dictionary<string, FuelStation>();
            _bonusCards = new Dictionary<string, BonusCard>();
            _logger = logger;
            _processedRecord = new HashSet<Tuple<FuelStation, DateTime, long>>();
        }
        #region Implementation of IRegistrar

        public async Task RegisterShift(Shift shift)
        {
            var __shiftRegistrar = new ShiftRegistrar(_outputPath);
            var __task = Task.Run(() => {
                __shiftRegistrar.RegisterShiftSales(shift); });
            await __task;
        }

        public FuelStation GetFuelStation(String stationName)
        {
            FuelStation __fuelStation;
            if (_stations.TryGetValue(stationName, out __fuelStation))
                return __fuelStation;
            __fuelStation = new FuelStation {Name = stationName};
            _stations.Add(stationName,__fuelStation);
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
                    _logger.Write($"BBOX_{__fuelStationShift.FuelStation.Name.Substring(6)} {__fuelStationShift.BeginDate:yyyy_MM_dd HH_mm_ss} _ {__fuelStationShift.EndDate:yyyy_MM_dd HH_mm_ss}.xlsx");
                    var __task = RegisterShift(__fuelStationShift);
                    __task.Wait();
                }
                _logger.Write(__fuelStation.Name);
                __report.RegisterSummaryReport(__fuelStation);
                Thread.Sleep(1);
            }
        }

        private void FillBonusesSummaryReport(ISheet sheet)
        {
            var __cards = _bonusCards.Values.Select(x => new
            {
                x.CardNo,
                Charged = x.Movements.Where(z => z.Operation == BonusesOperation.ChargeBonuses).Sum(y => y.Bonuses),
                Cancellation = x.Movements.Where(z => z.Operation == BonusesOperation.CancellationBonuses).Sum(y => y.Bonuses),
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

        public void RegisterBonusReport()
        {
            
            var __report = new BonusReportRegistrar(_outputPath);
            var __cards = _bonusCards.Values.ToArray();
            _logger.Write($"Формирование ведомостей по бонусам. Количество {__cards.Length}");
            for (var __i = 0; __i < __cards.Length; __i++)
            {
                _logger.Write($"Формирование ведомости № {__cards[__i].CardNo}. {__i} из {__cards.Length}");
                __report.RegisterBonusReport(__cards[__i]);
                Thread.Sleep(1);
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
