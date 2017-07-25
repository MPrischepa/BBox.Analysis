using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Domain;
using BBox.Analysis.Domain.PaymentTypes;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BBox.Analysis.Processing
{
    internal class ShiftRegistrar
    {
        private string _outputPath;

        public ShiftRegistrar(string outputFile)
        {
            _outputPath = outputFile;
        }




        private void FillShiftRaport(ISheet sheet, Shift shift)
        {
            var __row = RegistrarHelper.GetRow(sheet, 1);
            var __cell = RegistrarHelper.GetCell(__row, 5);
            __cell.SetCellValue(shift.FuelStation.Name);
            __row = RegistrarHelper.GetRow(sheet, 2);
            __cell = RegistrarHelper.GetCell(__row, 5);
            __cell.SetCellValue($"{shift.BeginDate:dd.MM.yyyy HH:mm:ss} - {shift.EndDate:dd.MM.yyyy HH:mm:ss}");
            var __i = 7;
            foreach (
                var __stationFuelsSale in
                shift.FuelsSales.Where(x => x.PreOrder != null || x.FactPour != null || x.FactSale != null).OrderBy(x => x.Date))
            {
                __row = RegistrarHelper.GetRow(sheet, __i);
                __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__stationFuelsSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{__stationFuelsSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__stationFuelsSale.ID);

                var __order = __stationFuelsSale.PreOrder;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 3, __order, true);
                __order = __stationFuelsSale.FactPour;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 10, __order, false);
                __order = __stationFuelsSale.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 16, __order, false);
                if (__stationFuelsSale.IsCheckPrinted)
                {
                    __cell = RegistrarHelper.GetCell(__row, 22);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.CheckAmount));
                }

                if (__stationFuelsSale.PourState >= FuelPourState.PourStart && __stationFuelsSale.FuelHouse != null)
                {
                    var __column = __stationFuelsSale.FactPour?.FuelColumn ?? __stationFuelsSale.PreOrder.FuelColumn;
                   __cell = RegistrarHelper.GetCell(__row, 23);
                   __cell.SetCellValue(__column.ID);
                    

                    __cell = RegistrarHelper.GetCell(__row, 24);
                    __cell.SetCellValue(__stationFuelsSale.FuelHouse.Name);

                    __cell = RegistrarHelper.GetCell(__row, 25);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.StartCounterValue));

                    __cell = RegistrarHelper.GetCell(__row, 26);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FinishedCounterValue));

                    __cell = RegistrarHelper.GetCell(__row, 27);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FinishedCounterValue - __stationFuelsSale.StartCounterValue));
                }
                __i++;
            }
        }

        private void FillDivergenceCounter(ISheet sheet, Shift shift)
        {
            var __rowNum = 4;
            var __data =
                shift.FuelsSales.Where(
                        x =>
                            x.FuelHouse != null)
                    .Where(
                        x =>
                            Math.Abs((x.FactSale?.Volume ?? 0) - (x.FinishedCounterValue - x.StartCounterValue)) >= 1);
            foreach (var __fuelSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__fuelSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{__fuelSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__fuelSale.ID);

                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(Convert.ToDouble(__fuelSale.FinishedCounterValue - __fuelSale.StartCounterValue));

                var __order = __fuelSale.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 4, __order, false);

                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(
                    Convert.ToDouble((__order?.Volume ?? 0) -
                                     (__fuelSale.FinishedCounterValue - __fuelSale.StartCounterValue)));
                __rowNum++;
            }
        }

        private void AdditionalPrintedCheck(ISheet sheet, Shift shift)
        {
            var __rowNum = 4;
            var __data =
                shift.FuelsSales.Where(
                        x => x.IsCheckPrinted && x.FactSale == null && x.FactPour == null && x.PreOrder != null)
                    .OrderBy(x => x.Date);
            foreach (var __fuelSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__fuelSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{ __fuelSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__fuelSale.ID);

                var __order = __fuelSale.PreOrder;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 3, __order, true);

                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(Convert.ToDouble(__fuelSale.CheckAmount));
                __rowNum++;
            }
        }

        private void FillFuelSummaryTable(ISheet sheet, Shift shift)
        {
            var __rowNum = 3;
            var __dataPour = shift.FuelsSales.Where(x => x.FactPour != null)
                .GroupBy(x => x.FactPour.ProductName, (product, sales) => new
                {
                    Product = product,
                    Volume = sales.Sum(y => y.FactPour.Volume),
                    Amount = sales.Sum(y => y.FactPour.Amount),
                    Count = sales.Count()
                });
            var __dataFact = shift.FuelsSales.Where(x => x.FactSale != null)
                .GroupBy(x => x.FactSale.ProductName, (product, sales) => new
                {
                    Product = product,
                    Volume = sales.Sum(y => y.FactSale.Volume),
                    Amount = sales.Sum(y => y.FactSale.Amount),
                    Count = sales.Count(),
                    CheckAmount = sales.Sum(y => y.IsCheckPrinted ? y.FactSale.Amount : 0)
                });

            var __printedChecks =
                shift.FuelsSales.Where(
                        x => x.IsCheckPrinted && x.FactSale == null && x.FactPour == null && x.PreOrder != null)
                    .GroupBy(x => x.PreOrder.ProductName,
                        (product, sales) => new {Product = product, PrintedCheck = sales.Sum(x => x.CheckAmount)});

            var __result = (from __pour in __dataPour
                join __sale in __dataFact on __pour.Product equals __sale.Product into __fact
                from __f in __fact.DefaultIfEmpty(new {Product = __pour.Product, Volume = 0M, Amount = 0M, Count = 0,CheckAmount = 0M})
                join __check in __printedChecks on __pour.Product equals  __check.Product into __printedCheck
                from __pc in __printedCheck.DefaultIfEmpty(new {Product = __pour.Product, PrintedCheck = 0M})
                select new
                {
                    Product = __pour.Product,
                    Pour = new
                    {
                        Volume = __pour.Volume,
                        Amount = __pour.Amount,
                        Count = __pour.Count
                    },
                    Fact = new
                    {
                        Volume = __f.Volume,
                        Amount = __f.Amount,
                        Count = __f.Count,
                        CheckAmount = __f.CheckAmount
                    },
                    PrintedCheck = __pc.PrintedCheck
                }).OrderBy(x => x.Product);

            foreach (var __r in __result)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__r.Product);
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Volume));

                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Amount));

                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Count));

                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Volume));

                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Amount));

               // __cell = RegistrarHelper.GetCell(__row, 6);
               // __cell.SetCellValue(Convert.ToDouble(__r.Fact.CheckAmount));

                __cell = RegistrarHelper.GetCell(__row, 6);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Count));

               // __cell = RegistrarHelper.GetCell(__row, 8);
               // __cell.SetCellValue(Convert.ToDouble(__r.PrintedCheck));

                __cell = RegistrarHelper.GetCell(__row, 7);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Volume - __r.Fact.Volume ));

                __cell = RegistrarHelper.GetCell(__row, 8);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Amount - __r.Fact.Amount /*- __r.PrintedCheck*/));

                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Count - __r.Fact.Count));
                __rowNum++;
            }
        }

        private void FillCashSummaryTable(ISheet sheet, Shift shift)
        {
            var __rowNum = 3;
           
            var __dataFact = shift.FuelsSales.Where(x => x.FactSale != null)
                .GroupBy(x => new {
                    x.FactSale.ProductName,x.FactSale.Payment.PaymentTypeName}, (product, sales) => new
                {
                        Sale = product,
                        Amount = sales.Sum(y => y.FactSale.Amount),
                    CheckAmount = sales.Sum(y => y.IsCheckPrinted ? y.CheckAmount : 0)
                });

            var __printedChecks =
                shift.FuelsSales.Where(
                        x => x.IsCheckPrinted && x.FactSale == null && x.FactPour == null && x.PreOrder != null)
                    .GroupBy(x => new { x.PreOrder.ProductName, x.Payment.PaymentTypeName},
                        (product, sales) => new { Sale = product, PrintedCheck = sales.Sum(x => x.CheckAmount) });

            var __result = (from  __sale in __dataFact 
                join __check in __printedChecks on __sale.Sale equals __check.Sale into __printedCheck
                from __pc in __printedCheck.DefaultIfEmpty(new {Sale = __sale.Sale, PrintedCheck = 0M})
                select new
                {
                    Sale = __sale.Sale,
                    Fact = new
                    {
                        Amount = __sale.Amount,
                        CheckAmount = __sale.CheckAmount
                    },
                    PrintedCheck = __pc.PrintedCheck
                }).OrderBy(x => x.Sale.ProductName).ThenBy(x => x.Sale.PaymentTypeName);

            foreach (var __r in __result)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__r.Sale.ProductName);
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__r.Sale.PaymentTypeName);

                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Amount));

                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.CheckAmount));


                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(Convert.ToDouble(__r.PrintedCheck));

                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Amount - __r.Fact.CheckAmount - __r.PrintedCheck));

                __rowNum++;
            }
        }

        private void FillCanceledOrder(ISheet sheet, Shift shift)
        {
            var __rowNum = 4;
            var __data =
                shift.FuelsSales.Where(x => x.SaleState == FuelSaleState.Canceled)
                    .Where(x => x.FactPour != null)
                    .Where(x => x.FactSale == null).OrderBy(x => x.Date);
            foreach (var __stationFuelsSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__stationFuelsSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{ __stationFuelsSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__stationFuelsSale.ID);

                var __order = __stationFuelsSale.FactPour;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 3, __order, false);
                if (__stationFuelsSale.IsCheckPrinted)
                {
                    __cell = RegistrarHelper.GetCell(__row, 9);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.CheckAmount));
                }

                if (__stationFuelsSale.FuelHouse != null)
                {
                    __cell = RegistrarHelper.GetCell(__row, 10);
                    __cell.SetCellValue(__stationFuelsSale.FactPour.FuelColumn.ID);

                    __cell = RegistrarHelper.GetCell(__row, 11);
                    __cell.SetCellValue(__stationFuelsSale.FuelHouse.Name);

                    __cell = RegistrarHelper.GetCell(__row, 12);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.StartCounterValue));

                    __cell = RegistrarHelper.GetCell(__row, 13);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FinishedCounterValue));

                    __cell = RegistrarHelper.GetCell(__row, 14);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FinishedCounterValue - __stationFuelsSale.StartCounterValue));
                }
                __rowNum++;
            }
        }

        private void FillPaymentCange(ISheet sheet, Shift shift)
        {
            var __rowNum = 4;

            var __data =
                shift.FuelsSales.Where(x => x.SaleState == FuelSaleState.Approved)
                    .Where(x => x.FactPour != null && x.FactPour.Payment != null)
                    .Where(x => x.FactSale != null && x.FactSale.Payment != null)
                    .Where(x => !x.FactSale.Payment.Equals(x.FactPour.Payment)).OrderBy(x => x.Date);
            foreach (var __stationFuelsSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__stationFuelsSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{ __stationFuelsSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__stationFuelsSale.ID);

                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(__stationFuelsSale.FactPour.Payment.PaymentTypeName);

                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(__stationFuelsSale.FactPour.Payment.AdditionalInfo);
                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FactPour.Amount));

                __cell = RegistrarHelper.GetCell(__row, 6);
                __cell.SetCellValue(__stationFuelsSale.FactSale.Payment.PaymentTypeName);
                __cell = RegistrarHelper.GetCell(__row, 7);
                __cell.SetCellValue(__stationFuelsSale.FactSale.Payment.AdditionalInfo);
                __cell = RegistrarHelper.GetCell(__row, 8);
                __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FactSale.Amount));

                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(
                    Convert.ToDouble(__stationFuelsSale.FactPour.Amount - __stationFuelsSale.FactSale.Amount));
                if (__stationFuelsSale.IsCheckPrinted)
                {
                    __cell = RegistrarHelper.GetCell(__row,10);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.CheckAmount));
                }
                __rowNum++;
            }
        }

        public void FillChargeBonuses(ISheet sheet, Shift shift)
        {
            var __rowNum = 4;
            var __data = new List<Tuple<FuelSale,BonusCardPayment>>();
            foreach (var __stationFuelsSale in shift.FuelsSales)
            {
                if (__stationFuelsSale.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment) __stationFuelsSale.Payment));
                }
                if (__stationFuelsSale.PreOrder?.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment) __stationFuelsSale.PreOrder.Payment));
                }
                if (__stationFuelsSale.FactPour?.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment) __stationFuelsSale.FactPour.Payment));
                }

                if (__stationFuelsSale.FactSale?.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment) __stationFuelsSale.FactSale.Payment));
                }

            }
            __data =
                __data.Distinct(new PaymentComparer())
                    .Where(x => x.Item2.ChargeBonuses != 0)
                    .OrderBy(x => x.Item1.Date)
                    .ToList();
            foreach (var __tuple in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__tuple.Item1.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{ __tuple.Item1.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__tuple.Item1.ID);

                var __order = __tuple.Item1.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 3, __order, false);
                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(__tuple.Item2.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(Convert.ToDouble(__tuple.Item2.ChargeBonuses));
                if (__tuple.Item1.IsCheckPrinted)
                {
                    __cell = RegistrarHelper.GetCell(__row, 11);
                    __cell.SetCellValue(Convert.ToDouble(__tuple.Item1.CheckAmount));
                }
                __rowNum++;
            }
        }

        public void FillCancellationBonuses(ISheet sheet, Shift shift)
        {
            var __rowNum = 4;
            var __data = new List<Tuple<FuelSale, BonusCardPayment>>();
            foreach (var __stationFuelsSale in shift.FuelsSales)
            {
                if (__stationFuelsSale.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment)__stationFuelsSale.Payment));
                }
                if (__stationFuelsSale.PreOrder?.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment)__stationFuelsSale.PreOrder.Payment));
                }
                if (__stationFuelsSale.FactPour?.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment)__stationFuelsSale.FactPour.Payment));
                }

                if (__stationFuelsSale.FactSale?.Payment is BonusCardPayment)
                {
                    __data.Add(new Tuple<FuelSale, BonusCardPayment>(__stationFuelsSale,
                        (BonusCardPayment)__stationFuelsSale.FactSale.Payment));
                }
            }
            __data =
                __data.Distinct(new PaymentComparer())
                    .Where(x => x.Item2.CancellationBonuses != 0)
                    .OrderBy(x => x.Item1.Date)
                    .ToList();
            foreach (var __tuple in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__tuple.Item1.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue($"{ __tuple.Item1.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__tuple.Item1.ID);

                var __order = __tuple.Item1.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 3, __order, false);
                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(__tuple.Item2.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(Convert.ToDouble(__tuple.Item2.CancellationBonuses));
                if (__tuple.Item1.IsCheckPrinted)
                {
                    __cell = RegistrarHelper.GetCell(__row, 11);
                    __cell.SetCellValue(Convert.ToDouble(__tuple.Item1.CheckAmount));
                }
                __rowNum++;
            }
        }

        
        public void RegisterShiftSales(Shift shift)
        {
            var __path = Path.Combine(".\\Resources", "Шаблон.xlsx");
            IWorkbook __book;
            using (var __templateStream = File.OpenRead(__path))
            {
                __book = new XSSFWorkbook(__templateStream);
                var __sheet = __book.GetSheetAt(0);
                FillShiftRaport(__sheet, shift);
                FillFuelSummaryTable(__book.GetSheetAt(1), shift);
                FillCashSummaryTable(__book.GetSheetAt(2), shift);
                AdditionalPrintedCheck(__book.GetSheetAt(3), shift);
                FillCanceledOrder(__book.GetSheetAt(4), shift);
                FillPaymentCange(__book.GetSheetAt(5), shift);
                FillChargeBonuses(__book.GetSheetAt(6), shift);
                FillCancellationBonuses(__book.GetSheetAt(7), shift);
                FillDivergenceCounter(__book.GetSheetAt(8), shift);
            }

            if (!Directory.Exists(Path.Combine(_outputPath, shift.FuelStation.Name)))
            {
                Directory.CreateDirectory(Path.Combine(_outputPath, shift.FuelStation.Name));
            }
           
            using (
                var __resultStream =
                    File.OpenWrite(Path.Combine(_outputPath, shift.FuelStation.Name,
                        $"BBOX_{shift.FuelStation.Name.Substring(6)} {shift.BeginDate:yyyy_MM_dd HH_mm_ss} _ {shift.EndDate:yyyy_MM_dd HH_mm_ss}.xlsx"))
            )
            {
                __book.Write(__resultStream);
                __book.Close();
            }
        }
    }

    internal class PaymentComparer : IEqualityComparer<Tuple<FuelSale, BonusCardPayment>>
    {
        #region Implementation of IEqualityComparer<in Tuple<FuelSale,BonusPayment>>

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        public bool Equals(Tuple<FuelSale, BonusCardPayment> x, Tuple<FuelSale, BonusCardPayment> y)
        {
            return x.Item1.Equals(y.Item1) && x.Item2.Equals(y.Item2);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
        public int GetHashCode(Tuple<FuelSale, BonusCardPayment> obj)
        {
            return (obj.Item1.GetHashCode() * 397) ^ obj.Item2.GetHashCode();
        }

        #endregion
    }
}
