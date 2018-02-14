using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BBox.Analysis.Domain;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BBox.Analysis.Processing.OneSComparer
{
    internal class OneSComparerRegistrar
    {
        private IDataReader _reader;
        private String _outputPath;
        public OneSComparerRegistrar(IDataReader reader, String outputPath)
        {
            _reader = reader;
            _outputPath = outputPath;
        }

        private void FillRow(ISheet sheet, int rowNum, DataRecord record, Decimal startCounter, Decimal finishedCounter)
        {
            var __row = RegistrarHelper.GetRow(sheet, rowNum);
            var __cell = RegistrarHelper.GetCell(__row, 0);
            __cell.SetCellValue(record.FuelStationName);

            __cell = RegistrarHelper.GetCell(__row, 1);
            __cell.SetCellValue($"{record.ShiftBeginDate:dd.MM.yyyy HH:mm} - {record.ShiftEndDate:dd.MM.yyyy HH:mm}");

            __cell = RegistrarHelper.GetCell(__row, 2);
            __cell.SetCellValue(record.Operator);

            __cell = RegistrarHelper.GetCell(__row, 3);
            __cell.SetCellValue(record.FuelColumnID);

            __cell = RegistrarHelper.GetCell(__row, 4);
            __cell.SetCellValue(record.FuelHoseID);
            __cell = RegistrarHelper.GetCell(__row, 5);
            __cell.SetCellValue(record.Product);
            __cell = RegistrarHelper.GetCell(__row, 6);
            __cell.SetCellValue(Convert.ToDouble(record.Volume));

            __cell = RegistrarHelper.GetCell(__row, 7);
            __cell.SetCellValue(Convert.ToDouble(finishedCounter - startCounter));

            __cell = RegistrarHelper.GetCell(__row, 8);
            __cell.SetCellValue(Convert.ToDouble(finishedCounter - startCounter - record.Volume));

            __cell = RegistrarHelper.GetCell(__row, 9);
            __cell.SetCellValue(Convert.ToDouble(startCounter));
            __cell = RegistrarHelper.GetCell(__row, 10);
            __cell.SetCellValue(Convert.ToDouble(finishedCounter));
           
        }
        public void FillCompareReports(IDictionary<String,FuelStation> stations)
        {

            var __rowNum = 3;
            var __path = Path.Combine(".\\Resources", "Сравнение 1С.xlsx");
            IWorkbook __book;
            using (var __templateStream = File.OpenRead(__path))
            {
                __book = new XSSFWorkbook(__templateStream);
                ISheet __sheet = null;
                var __sheetInd = 0;
                var __fuelStationName = String.Empty;
                var __output = String.Empty;
                foreach (var __dataRecord in _reader)
                {
                    if (
                        !__output.Equals(
                            $"Анализируем {__dataRecord.FuelStationName} {__dataRecord.ShiftBeginDate.Year}"))
                    {
                        __output = $"Анализируем {__dataRecord.FuelStationName} {__dataRecord.ShiftBeginDate.Year}";
                        Console.WriteLine(__output);
                    }
                    FuelStation __station;
                    if (!stations.TryGetValue(__dataRecord.FuelStationName, out __station))
                        continue;
                    var __shifts =
                        __station.Shifts.Where(
                                x =>
                                    x.BeginDate >= __dataRecord.ShiftBeginDate &&
                                    x.EndDate <= __dataRecord.ShiftEndDate)
                            .ToList();
                    if (!__shifts.Any()) continue;
                    var __startCounters =
                        __shifts.SelectMany(x => x.StartShiftHoses)
                            .Where(
                                x => x.FuelHose == __dataRecord.FuelHoseID && x.FuelColumn == __dataRecord.FuelColumnID)
                            .ToList();
                    var __finishCounters = __shifts.SelectMany(x => x.FinishedShiftHoses)
                        .Where(x => x.FuelHose == __dataRecord.FuelHoseID && x.FuelColumn == __dataRecord.FuelColumnID)
                        .ToList();

                    var __startCounter = __startCounters.Any() ? __startCounters.Min(x => x.Value) : 0;
                    var __finishedCounter = __finishCounters.Any() ? __finishCounters.Max(x => x.Value) : 0;
                    //if (__dataRecord.Volume == __finishedCounter - __startCounter) continue;
                    
                    if (!__dataRecord.FuelStationName.Equals(__fuelStationName))
                    {
                        __book.CloneSheet(__sheetInd);
                        __sheet = __book.GetSheetAt(__sheetInd);
                        __fuelStationName = __dataRecord.FuelStationName;
                        __book.SetSheetName(__sheetInd,
                            $"{__fuelStationName}");
                        __rowNum = 3;
                        __sheetInd++;
                        
                    }
                    FillRow(__sheet, __rowNum, __dataRecord, __startCounter, __finishedCounter);
                    __rowNum++;
                    
                }
            }

            using (
                var __resultStream =
                    File.OpenWrite(Path.Combine(_outputPath, "Сравнение с 1С.xlsx"))
            )
            {
                __book.Write(__resultStream);
                __book.Close();
            }
            
        }
    }
}
