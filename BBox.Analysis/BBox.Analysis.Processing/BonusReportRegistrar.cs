using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Domain;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BBox.Analysis.Processing
{
    internal class BonusReportRegistrar
    {
        private string _outputPath;

        public BonusReportRegistrar(string outputFile)
        {
            _outputPath = outputFile;
        }


        private void FillBonusReport(ISheet sheet, BonusCard card)
        {
            var __rowNum = 5;
            var __bonusesRest = 0M;
            var __data = card.Movements.OrderBy(x => x.OperationDate);
            foreach (var __movement in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__movement.Sale.FuelStation.Name);

                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{__movement.Sale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__movement.Sale.Shift.EndDate:dd.MM.yyyy HH:mm}");

                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__movement.Sale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue($"{ __movement.Sale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(__movement.Sale.ID);

                var __order = __movement.Sale.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 5, __order, false);

                
                var __chargedBonuses = __movement.Operation == BonusesOperation.ChargeBonuses ? __movement.Bonuses : 0;
                var __cancelationBonuses = __movement.Operation == BonusesOperation.CancellationBonuses
                    ? __movement.Bonuses
                    : 0;
                __bonusesRest = __bonusesRest + __chargedBonuses - __cancelationBonuses;
                __cell = RegistrarHelper.GetCell(__row, 11);
                __cell.SetCellValue(Convert.ToDouble(__chargedBonuses));

                __cell = RegistrarHelper.GetCell(__row, 12);
                __cell.SetCellValue(Convert.ToDouble(__cancelationBonuses));

                __cell = RegistrarHelper.GetCell(__row, 13);
                __cell.SetCellValue(Convert.ToDouble(__bonusesRest));
                __rowNum++;

            }
        }
        public void RegisterBonusReport(BonusCard card)
        {
            var __path = Path.Combine(".\\Resources", "Бонусы Шаблон.xlsx");
            IWorkbook __book;
            using (var __templateStream = File.OpenRead(__path))
            {
                __book = new XSSFWorkbook(__templateStream);

                FillBonusReport(__book.GetSheetAt(0), card);
            }

            if (!Directory.Exists(Path.Combine(_outputPath, "Бонусы")))
            {
                Directory.CreateDirectory(Path.Combine(_outputPath, "Бонусы"));
            }

            using (
                var __resultStream =
                    File.OpenWrite(Path.Combine(_outputPath, "Бонусы",
                        $"Бонусы карта № {card.CardNo}.xlsx"))
            )
            {
                __book.Write(__resultStream);
                __book.Close();
            }
        }
    }
}
