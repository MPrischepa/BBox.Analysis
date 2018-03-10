using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BBox.Analysis.Domain;
using BBox.Analysis.Domain.PaymentTypes;
using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BBox.Analysis.Processing.AccountCardComparer
{
    internal class AccountCardRegistrar
    {
        private readonly String _outputPath;
        private IDataReader _reader;
        public AccountCardRegistrar(IDataReader reader, String outputPath)
        {
            _outputPath = outputPath;
            _reader = reader;
        }

        private void FillRow(ISheet sheet, int rowNum, ResultRecord record)
        {
            var __row = RegistrarHelper.GetRow(sheet, rowNum);
            var __cell = RegistrarHelper.GetCell(__row, 0);

            __cell.SetCellValue($"{record.Shift.BeginDate:dd.MM.yyyy HH:mm} - {record.Shift.EndDate:dd.MM.yyyy HH:mm}");
            if (record.OneC != null)
            {
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(record.OneC.Operator);
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue($"{record.OneC.SaleDate:dd.MM.yy HH:mm:ss}");

                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(record.OneC.FuelColumnID);
                __cell = RegistrarHelper.GetCell(__row, 6);
                __cell.SetCellValue(record.OneC.FuelHoseID);
                __cell = RegistrarHelper.GetCell(__row, 7);
                __cell.SetCellValue(record.OneC.Product);

                __cell = RegistrarHelper.GetCell(__row, 8);
                __cell.SetCellValue(record.OneC.CardNo);

                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(record.OneC.Customer);

                __cell = RegistrarHelper.GetCell(__row, 15);
                __cell.SetCellValue(Convert.ToDouble(record.OneC.Volume));


            }

            if (record.Sale != null)
            {
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(record.Sale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue($"{record.Sale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(record.Sale.ID);

                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(record.Sale.FactSale.FuelColumn.ID);
                __cell = RegistrarHelper.GetCell(__row, 6);
                __cell.SetCellValue(record.Sale.FuelHouse.Name);
                __cell = RegistrarHelper.GetCell(__row, 7);
                __cell.SetCellValue(record.Sale.FactSale.ProductName);

                __cell = RegistrarHelper.GetCell(__row, 8);
                __cell.SetCellValue(record.Sale.Payment.CardNo);

                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(Convert.ToDouble(record.Sale.FactSale.Volume));

                __cell = RegistrarHelper.GetCell(__row, 11);
                __cell.SetCellValue(Convert.ToDouble(record.Sale.StartCounterValue));
                __cell = RegistrarHelper.GetCell(__row, 12);
                __cell.SetCellValue(Convert.ToDouble(record.Sale.FinishedCounterValue));

                __cell = RegistrarHelper.GetCell(__row, 13);
                __cell.SetCellValue(Convert.ToDouble(record.Sale.FinishedCounterValue - record.Sale.StartCounterValue));

                __cell = RegistrarHelper.GetCell(__row, 14);
                __cell.SetCellValue(Convert.ToDouble(record.Sale.FinishedCounterValue - record.Sale.StartCounterValue - record.Sale.FactSale.Volume));
            }

            if (record.OneC != null && record.Sale != null)
            {
                __cell = RegistrarHelper.GetCell(__row, 16);
                __cell.SetCellValue(Convert.ToDouble(record.Sale.FactSale.Volume - record.OneC.Volume));
            }

            

        }

        public void FillAccountCardReport(ICollection<FuelStation> stations)
        {
            var __path = Path.Combine(".\\Resources", "Диалог сводная ведомость.xlsx");
            IWorkbook __book;
            using (var __templateStream = File.OpenRead(__path))
            {
                __book = new XSSFWorkbook(__templateStream);
                var __sheetInd = 0;
                var __bBoxData = stations.SelectMany(x => x.Shifts.Select(y => new {Station = x, Shift = y}))
                    .SelectMany(
                        x =>
                            x.Shift.FuelsSales.Where(
                                    y =>
                                        y != null &&
                                        y.SaleState == FuelSaleState.Approved &&
                                        y.PourState == FuelPourState.OrderFinished &&
                                        y.Payment != null &&
                                        y.Payment.PaymentType.HasFlag(PaymentType.CashlessSettlement) &&
                                        y.Payment.ClientType == ClientType.LegalEntity &&
                                        y.Payment.ClientCard != null &&
                                        y.Payment.ClientCardType == ClientCardType.AccountCard)
                                .Select(y => new {x.Station, x.Shift, Sale = y}))
                    .GroupBy(
                        x =>
                            new ReportGroup
                            {
                                StationName = x.Station.Name,
                                ShiftBeginDate = x.Shift.BeginDate,
                                ShiftEndDate = x.Shift.EndDate
                            },new ReportGroupComparer());
                var __oneCSelect = _reader.Where(x => stations.Select(y => y.Name).Contains(x.FuelStationName)).ToList();
                var __oneCData = __oneCSelect.GroupBy(x => new ReportGroup
                {
                    StationName = x.FuelStationName,
                    ShiftEndDate = x.ShiftEndDate,
                    ShiftBeginDate = x.ShiftBeginDate
                }, new ReportGroupComparer()).ToList();

                var __resultData =
                    __bBoxData.Join(__oneCData, x => x.Key, y => y.Key,
                            (bBox, oneC) => new {ReportGroup = bBox.Key, bBox = bBox.ToList(), OneC = oneC.ToList()},
                            new ReportGroupComparer())
                        .OrderBy(x => x.ReportGroup.StationName).ThenBy(x => x.ReportGroup.ShiftBeginDate).ToList();
                var __output = String.Empty;
                var __stationName = String.Empty;
                ISheet __sheet = null;
                var __rowNum = 0;
                foreach (var __shiftInfo in __resultData)
                {
                    if (
                       !__output.Equals(
                           $"Анализируем {__shiftInfo.ReportGroup.StationName}"))
                    {
                        __output = $"Анализируем {__shiftInfo.ReportGroup.StationName}";
                        Console.WriteLine(__output);
                    }
                    if (!__shiftInfo.ReportGroup.StationName.Equals(__stationName))
                    {

                        __book.CloneSheet(__sheetInd);
                        __sheet = __book.GetSheetAt(__sheetInd);
                        __stationName = __shiftInfo.ReportGroup.StationName;
                        __book.SetSheetName(__sheetInd, $"{__stationName}");
                        __rowNum = 3;
                        __sheetInd++;
                    }

                    var __row = RegistrarHelper.GetRow(__sheet, __rowNum);
                    var __cell = RegistrarHelper.GetCell(__row, 0);
                    __cell.SetCellValue(
                        $"Смена: {__shiftInfo.ReportGroup.ShiftBeginDate:dd.MM.yyyy HH:mm} - {__shiftInfo.ReportGroup.ShiftEndDate:dd.MM.yyyy HH:mm}");
                    __rowNum++;
                    __rowNum++;

                    var __leftOneC = from __bData in __shiftInfo.bBox
                        join __oData in __shiftInfo.OneC on __bData.Sale.Payment.CardNo equals __oData.CardNo into __o
                        from __oD in __o.DefaultIfEmpty()
                        where __bData.Sale.Date == (__oD?.SaleDate ?? __bData.Sale.Date)
                        select new ResultRecord
                        {
                            Shift = new ShiftReport
                            {
                                EndDate = __bData.Shift.EndDate,
                                BeginDate = __bData.Shift.BeginDate
                            },
                            Sale = __bData.Sale,
                            OneC = __oD
                        };

                    var __leftBBox = from __oData in __shiftInfo.OneC
                        join __bData in __shiftInfo.bBox on __oData.CardNo equals __bData.Sale.Payment.CardNo into __b
                        from __bD in __b.DefaultIfEmpty()
                        where __oData.SaleDate == (__bD?.Sale.Date ?? __oData.SaleDate)
                        select
                        new ResultRecord
                        {
                            Shift = new ShiftReport {BeginDate = __oData.ShiftBeginDate, EndDate = __oData.ShiftEndDate},
                            Sale = __bD?.Sale,
                            OneC = __oData
                        };
                    var __sales = __leftOneC.Union(__leftBBox, new ResultRecordComparer()).ToList();
                    foreach (var __sale in __sales)
                    {
                        FillRow(__sheet, __rowNum, __sale);
                        __rowNum++;
                    }
                    __rowNum++;
                   
                }
            }

            using (
                var __resultStream =
                    File.OpenWrite(Path.Combine(_outputPath, "Объем пролива по Диалогу.xlsx"))
            )
            {
                __book.Write(__resultStream);
                __book.Close();
            }

        }


        class ResultRecord
        {
            public ShiftReport Shift { get; set; }

            public FuelSale Sale { get; set; }

            public DataRecord OneC { get; set; }
        }

        class ResultRecordComparer : IEqualityComparer<ResultRecord>
        {
            #region Implementation of IEqualityComparer<in ResultRecord>

            /// <summary>Determines whether the specified objects are equal.</summary>
            /// <returns>true if the specified objects are equal; otherwise, false.</returns>
            /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
            /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
            public bool Equals(ResultRecord x, ResultRecord y)
            {
                return x.Shift.BeginDate == y.Shift.BeginDate && x.Shift.EndDate == y.Shift.EndDate &&
                       ((x.Sale == null && y.Sale == null) || x.Sale == y.Sale)
                       && ((x.OneC == null && y.OneC == null) || x.OneC == y.OneC);
            }

            /// <summary>Returns a hash code for the specified object.</summary>
            /// <returns>A hash code for the specified object.</returns>
            /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
            /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
            public int GetHashCode(ResultRecord obj)
            {
                return obj.Shift.BeginDate.GetHashCode() ^ obj.Shift.EndDate.GetHashCode() ^
                       (obj.Sale?.Payment.CardNo ?? obj.OneC.CardNo).GetHashCode() ^
                       (obj.Sale?.Date ?? obj.OneC.SaleDate).GetHashCode();
            }

            #endregion
        }
        class ReportGroup
        {
            public String StationName { get; set; }

            public DateTime ShiftBeginDate { get; set; }

            public DateTime ShiftEndDate { get; set; }
        }

        class ShiftReport
        {
            public DateTime BeginDate { get; set; }

            public DateTime EndDate { get; set; }
        }
        class ReportGroupComparer : IEqualityComparer<ReportGroup>
        {
            #region Implementation of IEqualityComparer<in ReportGroup>

            /// <summary>Determines whether the specified objects are equal.</summary>
            /// <returns>true if the specified objects are equal; otherwise, false.</returns>
            /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
            /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
            public bool Equals(ReportGroup x, ReportGroup y)
            {
                return y != null && x != null &&
                       (x.StationName.Equals(y.StationName) && x.ShiftBeginDate == y.ShiftBeginDate &&
                        x.ShiftEndDate == y.ShiftEndDate);
            }

            /// <summary>Returns a hash code for the specified object.</summary>
            /// <returns>A hash code for the specified object.</returns>
            /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
            /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
            public int GetHashCode(ReportGroup obj)
            {
                return obj.StationName.GetHashCode() ^ obj.ShiftBeginDate.GetHashCode() ^ obj.ShiftEndDate.GetHashCode();
            }

            #endregion
        }
    }

    public static class LinQExtension
    {
        public static JoinComparerProvider<T, TKey> WithComparer<T, TKey>(this IEnumerable<T> inner, IEqualityComparer<TKey> comparer)
        {
            return new JoinComparerProvider<T, TKey>(inner, comparer);
        }
    }

    public sealed class JoinComparerProvider<T, TKey>
    {
        internal JoinComparerProvider(IEnumerable<T> inner, IEqualityComparer<TKey> comparer)
        {
            Inner = inner;
            Comparer = comparer;
        }

        public IEqualityComparer<TKey> Comparer { get; private set; }
        public IEnumerable<T> Inner { get; private set; }
    }
}
