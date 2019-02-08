using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BBox.Analysis.Domain;
using BBox.Analysis.Domain.PaymentTypes;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BBox.Analysis.Processing
{
    internal class StatementRegistrar
    {
        private readonly String _outputPath;
        public StatementRegistrar(String outputPath)
        {
            _outputPath = outputPath;
        }

        private void FillRow(ISheet sheet, int rowNum, Shift shift, String product, Decimal volume)
        {
            var __row = RegistrarHelper.GetRow(sheet, rowNum);
            var __cell = RegistrarHelper.GetCell(__row, 0);
            
            __cell.SetCellValue($"{shift.BeginDate:dd.MM.yyyy HH:mm} - {shift.EndDate:dd.MM.yyyy HH:mm}");

            __cell = RegistrarHelper.GetCell(__row, 1);
            __cell.SetCellValue(product);

            __cell = RegistrarHelper.GetCell(__row, 2);
            __cell.SetCellValue(Convert.ToDouble(volume));
        }

        public void FillStatementReports(ICollection<FuelStation> stations)
        {
            var __path = Path.Combine(".\\Resources", "Объем пролива Ведомостям.xlsx");
            IWorkbook __book;
            using (var __templateStream = File.OpenRead(__path))
            {
                __book = new XSSFWorkbook(__templateStream);
                var __sheetInd = 0;
                var __results = stations.SelectMany(x => x.Shifts.Select(y => new {Station = x, Shift = y}))
                    .SelectMany(
                        x =>
                            x.Shift.FuelsSales.Where(
                                    y =>
                                        y != null &&
                                        y.SaleState == FuelSaleState.Approved &&
                                        y.Payment != null && 
                                        y.Payment.PaymentType.HasFlag(PaymentType.Statement))
                                .Select(y => new {x.Station, x.Shift, Sale = y}))
                    .GroupBy(x => x.Station,
                        (station, data) =>
                            new
                            {
                                Station = station,
                                Shifts =
                                data.GroupBy(x => new {x.Shift, Product = x.Sale.FactSale.ProductName},
                                    (x, y) =>
                                        new
                                        {
                                            x.Shift, x.Product,
                                            FactVolume = y.Sum(z => z.Sale.FactSale.Volume)
                                        })
                            }).OrderBy(x => x.Station.Name);
                foreach (var __stationInfo in __results)
                {
                    Console.WriteLine($"Анализируем {__stationInfo.Station.Name}");
                    __book.CloneSheet(__sheetInd);
                    var __sheet = __book.GetSheetAt(__sheetInd);
                    var __fuelStationName = __stationInfo.Station.Name;
                    __book.SetSheetName(__sheetInd,
                        $"{__fuelStationName}");
                    var __rowNum = 3;
                    __sheetInd++;
                    foreach (var __shiftInfo in __stationInfo.Shifts.OrderBy(x => x.Shift.BeginDate).ThenBy(x => x.Product))
                    {
                        FillRow(__sheet,__rowNum,__shiftInfo.Shift,__shiftInfo.Product,__shiftInfo.FactVolume);
                        __rowNum++;
                    }
                }
            }

            using (
                var __resultStream =
                    File.OpenWrite(Path.Combine(_outputPath, "Объем пролива Ведомостям.xlsx"))
            )
            {
                __book.Write(__resultStream);
                __book.Close();
            }

        }
    }
}
