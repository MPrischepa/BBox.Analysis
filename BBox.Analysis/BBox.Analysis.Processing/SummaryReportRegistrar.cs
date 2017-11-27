using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BBox.Analysis.Core.Logger;
using BBox.Analysis.Domain;
using BBox.Analysis.Domain.PaymentTypes;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace BBox.Analysis.Processing
{
    internal class SummaryReportRegistrar
    {
        private string _outputPath;

        public SummaryReportRegistrar(string outputFile)
        {
            _outputPath = outputFile;
        }

        public void RegisterSummaryReport(FuelStation station)
        {

            var __path = Path.Combine(".\\Resources", "Сводный Шаблон.xlsx");
            IWorkbook __book;
            using (var __templateStream = File.OpenRead(__path))
            {
                __book = new XSSFWorkbook(__templateStream);

                FillFuelSummaryTable(__book.GetSheetAt(0), station);
                FillCashSummaryTable(__book.GetSheetAt(1), station);
                AdditionalPrintedCheck(__book.GetSheetAt(2), station);
                FillCanceledOrder(__book.GetSheetAt(3), station);
                FillPaymentCange(__book.GetSheetAt(4), station);
                FillVisaPayment(__book.GetSheetAt(5), station);
                FillChargeBonuses(__book.GetSheetAt(6), station);
                FillCancellationBonuses(__book.GetSheetAt(7), station);
                FillTopDiscountCard(__book.GetSheetAt(8), station);
                FillTopBonusCard(__book.GetSheetAt(9), station);
                FillTopAccountCard(__book.GetSheetAt(10), station);
                FillDivergenceCounter(__book.GetSheetAt(11), station);
                FillGapCounter(__book.GetSheetAt(12), station);
                FillCanceledOrderSummary(__book.GetSheetAt(13), station);
                FillSummaryIncorrectConclusion(__book.GetSheetAt(14), station);
                FillIncorrectConclusion(__book.GetSheetAt(15), station);

            }

            using (
                var __resultStream =
                    File.OpenWrite(Path.Combine(_outputPath,
                        $"Сводный отчет {station.Name}.xlsx"))
            )
            {
                __book.Write(__resultStream);
                __book.Close();
            }
            Thread.Sleep(1);
        }

        private void FillCashSummaryTable(ISheet sheet, FuelStation station)
        {
            var __rowNum = 3;
            //var __dataPour = station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.FactPour != null)
            //    .GroupBy(x => new { x.FactPour.ProductName, x.FactPour.Payment.PaymentTypeName, x.Shift }, (product, sales) => new
            //    {
            //        Sale = product,
            //        Amount = sales.Sum(y => y.FactPour.Amount),

            //    });
            var __dataFact = station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.FactSale != null)
                .GroupBy(x => new {
                    x.FactSale.ProductName,
                    (x.FactSale.Payment ?? new Payment(new PaymentTypeDescription
                    {
                        Name = "Не определено",
                        PaymentType = PaymentType.NotDefined,
                        ClientType = ClientType.Other
                    })).PaymentTypeName,
                    x.Shift
                }, (product, sales) =>
                {
                    var __fuelSales = sales as IList<FuelSale> ?? sales.ToList();
                    return new
                    {
                        Sale = product,
                        Amount = __fuelSales.Sum(y => y.FactSale.Amount),
                        CheckAmount = __fuelSales.Sum(y => y.IsCheckPrinted ? y.CheckAmount : 0)
                    };
                });

            var __printedChecks =
                station.Shifts.SelectMany(x => x.FuelsSales).Where(
                        x => x.IsCheckPrinted && x.FactSale == null && x.FactPour == null && x.PreOrder != null)
                    .GroupBy(x => new { x.PreOrder.ProductName, x.Payment.PaymentTypeName,x.Shift },
                        (product, sales) => new { Sale = product, PrintedCheck = sales.Sum(x => x.CheckAmount) });

            var __result = (from __sale in __dataFact 
                    join __check in __printedChecks on __sale.Sale equals __check.Sale into __printedCheck
                    from __pc in __printedCheck.DefaultIfEmpty(new { __sale.Sale, PrintedCheck = 0M})
                    select new
                    {
                        __sale.Sale,
                        Fact = new
                        {
                            __sale.Amount,
                            __sale.CheckAmount
                        },
                        __pc.PrintedCheck
                    }).OrderBy(x => x.Sale.Shift.BeginDate)
                .ThenBy(x => x.Sale.Shift.EndDate)
                .ThenBy(x => x.Sale.ProductName)
                .ThenBy(x => x.Sale.PaymentTypeName);

            foreach (var __r in __result)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__r.Sale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__r.Sale.Shift.EndDate:dd.MM.yyyy HH:mm}");
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__r.Sale.ProductName);
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(__r.Sale.PaymentTypeName);

                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Amount));

                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.CheckAmount));


                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(Convert.ToDouble(__r.PrintedCheck));

                __cell = RegistrarHelper.GetCell(__row, 6);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Amount - __r.Fact.CheckAmount - __r.PrintedCheck));

                __rowNum++;
            }
        }

        private void FillGapCounter(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __data =
                station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.FuelHouse != null).GroupBy(x => x.FuelHouse);
            foreach (var __grouping in __data)
            {
                FuelSale __priorSale = null;
                var __sales = __grouping.OrderBy(x => x.Date);
                foreach (var __fuelSale in __sales)
                {
                    if (__priorSale == null)
                    {
                        __priorSale = __fuelSale;
                        continue;
                    }
                    if (__priorSale.FinishedCounterValue != __fuelSale.StartCounterValue)
                    {
                        var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                        var __cell = RegistrarHelper.GetCell(__row, 0);
                        __cell.SetCellValue(__grouping.Key.FuelColumn.ID);

                        __cell = RegistrarHelper.GetCell(__row, 1);
                        __cell.SetCellValue(__grouping.Key.Name);

                        __cell = RegistrarHelper.GetCell(__row, 2);
                        __cell.SetCellValue(__priorSale.FactSale != null
                            ? __priorSale.FactSale.ProductName
                            : (__priorSale.FactPour != null
                                ? __priorSale.FactPour.ProductName
                                : __priorSale.PreOrder.ProductName));

                        __cell = RegistrarHelper.GetCell(__row, 3);
                        __cell.SetCellValue($"{__priorSale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__priorSale.Shift.EndDate:dd.MM.yyyy HH:mm}");

                        __cell = RegistrarHelper.GetCell(__row, 4);
                        __cell.SetCellValue(RegistrarHelper.GetSaleState(__priorSale.SaleState));
                        __cell = RegistrarHelper.GetCell(__row, 5);
                        __cell.SetCellValue($"{__priorSale.Date:dd.MM.yy HH:mm:ss}");
                        __cell = RegistrarHelper.GetCell(__row, 6);
                        __cell.SetCellValue(__priorSale.ID);

                        __cell = RegistrarHelper.GetCell(__row, 7);
                        __cell.SetCellValue(Convert.ToDouble(__priorSale.FinishedCounterValue));

                        __cell = RegistrarHelper.GetCell(__row, 8);
                        __cell.SetCellValue($"{__fuelSale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__fuelSale.Shift.EndDate:dd.MM.yyyy HH:mm}");

                        __cell = RegistrarHelper.GetCell(__row, 9);
                        __cell.SetCellValue(RegistrarHelper.GetSaleState(__fuelSale.SaleState));
                        __cell = RegistrarHelper.GetCell(__row, 10);
                        __cell.SetCellValue($"{__fuelSale.Date:dd.MM.yy HH:mm:ss}");
                        __cell = RegistrarHelper.GetCell(__row, 11);
                        __cell.SetCellValue(__fuelSale.ID);

                        __cell = RegistrarHelper.GetCell(__row, 12);
                        __cell.SetCellValue(Convert.ToDouble(__fuelSale.StartCounterValue));

                        __cell = RegistrarHelper.GetCell(__row, 13);
                        __cell.SetCellValue(Convert.ToDouble(__fuelSale.StartCounterValue - __priorSale.FinishedCounterValue));
                        __rowNum++;
                    }

                    __priorSale = __fuelSale;
                }
            }
        }
        private void FillDivergenceCounter(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __data =
                station.Shifts.SelectMany(x => x.FuelsSales).Where(
                        x =>
                            x.FuelHouse != null)
                    .Where(
                        x =>
                            Math.Abs((x.FactSale?.Volume ?? 0) - (x.FinishedCounterValue - x.StartCounterValue)) >= 1)
                    .OrderBy(x => x.Shift.BeginDate)
                    .ThenBy(x => x.Shift.EndDate).ThenBy(x => x.Date);
            foreach (var __fuelSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__fuelSale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__fuelSale.Shift.EndDate:dd.MM.yyyy HH:mm}");
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__fuelSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue($"{__fuelSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(__fuelSale.ID);

                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(Convert.ToDouble(__fuelSale.FinishedCounterValue - __fuelSale.StartCounterValue));

                var __order = __fuelSale.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 5, __order, false);

                __cell = RegistrarHelper.GetCell(__row, 11);
                __cell.SetCellValue(
                    Convert.ToDouble((__order?.Volume ?? 0) -
                                     (__fuelSale.FinishedCounterValue - __fuelSale.StartCounterValue)));
                __rowNum++;
            }
        }
        private void AdditionalPrintedCheck(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __data =
                station.Shifts.SelectMany(x => x.FuelsSales).Where(
                        x => x.IsCheckPrinted && x.FactSale == null && x.FactPour == null && x.PreOrder != null)
                    .OrderBy(x => x.Shift.BeginDate)
                    .ThenBy(x => x.Shift.EndDate).ThenBy(x => x.Date);
            foreach (var __fuelSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__fuelSale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__fuelSale.Shift.EndDate:dd.MM.yyyy HH:mm}");
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__fuelSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue($"{ __fuelSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(__fuelSale.ID);

                var __order = __fuelSale.PreOrder;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 4, __order, true);

                __cell = RegistrarHelper.GetCell(__row, 11);
                __cell.SetCellValue(Convert.ToDouble(__fuelSale.CheckAmount));
                __rowNum++;
            }
        }
        private void FillTopBonusCard(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __cards =
                station.Shifts.SelectMany(x => x.FuelsSales)
                    .Where(x => x.SaleState == FuelSaleState.Approved)
                    .Where(x => x.FactSale != null)
                    .Where(x => x.FactSale.Payment is BonusCardPayment)
                    .Select(x => x.FactSale.Payment as BonusCardPayment)
                    .GroupBy(x => x.CardNo,
                        (s, sales) =>
                        {
                            var __bonusCardPayments = sales as IList<BonusCardPayment> ?? sales.ToList();
                            return new
                            {
                                CardNo = s,
                                Count = __bonusCardPayments.Count(),
                                ChargedBonuses = __bonusCardPayments.Sum(x => x.ChargeBonuses),
                                CalculationBonuses = __bonusCardPayments.Sum(x => x.CancellationBonuses)
                            };
                        })
                    .OrderByDescending(x => x.Count)
                    /*.Take(20)*/;
            foreach (var __r in __cards)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__r.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__r.Count);

                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(Convert.ToDouble(__r.ChargedBonuses));

                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(Convert.ToDouble(__r.CalculationBonuses));
                __rowNum++;
            }

        }

        private void FillTopDiscountCard(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __cards =
                station.Shifts.SelectMany(x => x.FuelsSales)
                    .Where(x => x.SaleState == FuelSaleState.Approved)
                    .Where(x => x.FactSale != null)
                    .Where(
                        x =>
                            (x.FactSale.Payment ?? new Payment(new PaymentTypeDescription
                            {
                                Name = "Не определено",
                                PaymentType = PaymentType.NotDefined,
                                ClientType = ClientType.Other
                            })).ClientCardType == ClientCardType.DiscountCard)
                    .Select(x => new {Payment = x.FactSale.Payment, Amount = x.FactSale.Amount})
                    .GroupBy(x => x.Payment.CardNo,
                        (s, sales) => new {CardNo = s, Count = sales.Count(), Amount = sales.Sum(x => x.Amount)})
                    .OrderByDescending(x => x.Count)
                    /*.Take(20)*/;
            foreach (var __r in __cards)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__r.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__r.Count);
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(Convert.ToDouble(__r.Amount));
                __rowNum++;
            }

        }

        private void FillTopAccountCard(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __cards =
                station.Shifts.SelectMany(x => x.FuelsSales)
                    .Where(x => x.SaleState == FuelSaleState.Approved)
                    .Where(x => x.FactSale != null)
                    .Where(
                        x =>
                            (x.FactSale.Payment ?? new Payment(new PaymentTypeDescription
                            {
                                Name = "Не определено",
                                PaymentType = PaymentType.NotDefined,
                                ClientType = ClientType.Other
                            })).ClientCardType == ClientCardType.AccountCard)
                    .Select(x => new { Payment = x.FactSale.Payment, Amount = x.FactSale.Amount })
                    .GroupBy(x => x.Payment.CardNo,
                        (s, sales) => new { CardNo = s, Count = sales.Count(), Amount = sales.Sum(x => x.Amount) })
                    .OrderByDescending(x => x.Count)
                    /*.Take(20)*/;
            foreach (var __r in __cards)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue(__r.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__r.Count);
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(Convert.ToDouble(__r.Amount));
                __rowNum++;
            }

        }
        private void FillFuelSummaryTable(ISheet sheet, FuelStation station)
        {
            var __rowNum = 3;
            var __dataPour = station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.FactPour != null)
                .GroupBy(x => new {x.Shift, Product = x.FactPour.ProductName}, (group, sales) =>
                {
                    var __fuelSales = sales as IList<FuelSale> ?? sales.ToList();
                    return new
                    {
                        @group.Shift,
                        @group.Product,
                        Volume = __fuelSales.Sum(y => y.FactPour.Volume),
                        Amount = __fuelSales.Sum(y => y.FactPour.Amount),
                        Count = __fuelSales.Count()
                    };
                });

            var __dataFact = station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.FactSale != null)
                .GroupBy(x => new {x.Shift, Product = x.FactSale.ProductName }, (group, sales) =>
                {
                    var __enumerable = sales as IList<FuelSale> ?? sales.ToList();
                    return new
                    {
                        @group.Shift,
                        @group.Product,
                        Volume = __enumerable.Sum(y => y.FactSale.Volume),
                        Amount = __enumerable.Sum(y => y.FactSale.Amount),
                        Count = __enumerable.Count()
                    };
                });

            var __result = (from __pour in __dataPour
                join __sale in __dataFact on __pour.Product equals __sale.Product into __fact
                from __f in
                __fact.DefaultIfEmpty(
                    new {__pour.Shift, __pour.Product, Volume = 0M, Amount = 0M, Count = 0})
                where __f.Shift == __pour.Shift
                select new
                {
                    __pour.Shift, __pour.Product,
                    Pour = new
                    {
                        __pour.Volume, __pour.Amount, __pour.Count
                    },
                    Fact = new
                    {
                        __f.Volume, __f.Amount, __f.Count
                    }
                }).OrderBy(x => x.Shift.BeginDate).ThenBy(x => x.Shift.EndDate).ThenBy(x => x.Product);

            foreach (var __r in __result)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__r.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__r.Shift.EndDate:dd.MM.yyyy HH:mm}");
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__r.Product);
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Volume));

                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Amount));

                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Count));

                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Volume));

                __cell = RegistrarHelper.GetCell(__row, 6);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Amount));

                __cell = RegistrarHelper.GetCell(__row, 7);
                __cell.SetCellValue(Convert.ToDouble(__r.Fact.Count));

                __cell = RegistrarHelper.GetCell(__row, 8);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Volume - __r.Fact.Volume));

                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Amount - __r.Fact.Amount));

                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(Convert.ToDouble(__r.Pour.Count - __r.Fact.Count));
                __rowNum++;
            }
        }

        private void FillVisaPayment(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __dataFact = station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.FactSale != null)
                .Where(
                    x =>
                        (x.FactSale.Payment ?? new Payment(new PaymentTypeDescription
                        {
                            Name = "Не определено",
                            PaymentType = PaymentType.NotDefined,
                            ClientType = ClientType.Other
                        })).ClientType == ClientType.PhysicalPerson &&
                        (x.FactSale.Payment ?? new Payment(new PaymentTypeDescription
                        {
                            Name = "Не определено",
                            PaymentType = PaymentType.NotDefined,
                            ClientType = ClientType.Other
                        })).PaymentType.HasFlag(PaymentType.CashlessSettlement))
                .GroupBy(x => x.Shift, (group, sales) =>
                {
                    var __enumerable = sales as IList<FuelSale> ?? sales.ToList();
                    return new
                    {
                        Shift = @group,
                        Amount = __enumerable.Sum(y => y.IsCheckPrinted ? y.CheckAmount : y.FactSale.Amount),
                    };
                });

            foreach (var __r in __dataFact)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__r.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__r.Shift.EndDate:dd.MM.yyyy HH:mm}");
                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(Convert.ToDouble(__r.Amount));
                __rowNum++;
            }
        }

        private void FillCanceledOrder(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __data =
                station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.SaleState == FuelSaleState.Canceled)
                    .Where(x => x.FactPour != null)
                    .Where(x => x.FactSale == null)
                    .GroupBy(x => x.Shift)
                    .OrderBy(x => x.Key.BeginDate)
                    .ThenBy(x => x.Key.EndDate);
                    //.ThenBy(x => x.Date);
            foreach (var __shift in __data)
            {
                IRow __row;
                ICell __cell;
                foreach (var __stationFuelsSale in __shift)
                {
                    __row = RegistrarHelper.GetRow(sheet, __rowNum);
                    __cell = RegistrarHelper.GetCell(__row, 0);
                    __cell.SetCellValue($"{__stationFuelsSale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__stationFuelsSale.Shift.EndDate:dd.MM.yyyy HH:mm}");

                    __cell = RegistrarHelper.GetCell(__row, 1);
                    __cell.SetCellValue(RegistrarHelper.GetSaleState(__stationFuelsSale.SaleState));
                    __cell = RegistrarHelper.GetCell(__row, 2);
                    __cell.SetCellValue($"{ __stationFuelsSale.Date:dd.MM.yy HH:mm:ss}");
                    __cell = RegistrarHelper.GetCell(__row, 3);
                    __cell.SetCellValue(__stationFuelsSale.ID);

                    var __order = __stationFuelsSale.FactPour;
                    if (__order != null)
                        RegistrarHelper.FillOrderData(__row, 4, __order, false);

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
                var __region = new CellRangeAddress(__rowNum, __rowNum + 1, 0, 7);
                sheet.AddMergedRegion(__region);
                __region = new CellRangeAddress(__rowNum, __rowNum + 1, 8, 8);
                sheet.AddMergedRegion(__region);
                __region = new CellRangeAddress(__rowNum, __rowNum + 1, 9, 9);
                sheet.AddMergedRegion(__region);
                __row = RegistrarHelper.GetRow(sheet, __rowNum);
                __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"Итого по смене: {__shift.Key.BeginDate:dd.MM.yyyy HH:mm} - {__shift.Key.EndDate:dd.MM.yyyy HH:mm}");
                __cell = RegistrarHelper.GetCell(__row,8);
                __cell.SetCellValue(__shift.Count());
                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(Convert.ToDouble(__shift.Sum(x => x.FactPour.Amount)));
                __rowNum = __rowNum+2;

            }
        }

        private void FillCanceledOrderSummary(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __data =
                station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.SaleState == FuelSaleState.Canceled)
                    .Where(x => x.FactPour != null)
                    .Where(x => x.FactSale == null)
                    .GroupBy(x => x.Shift,(shift, sales) => new {Shift = shift,Count = sales.Count(),Amount = sales.Sum(x => x.FactPour.Amount)})
                    .OrderBy(x => x.Shift.BeginDate)
                    .ThenBy(x => x.Shift.EndDate);
            foreach (var __stationFuelsSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__stationFuelsSale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__stationFuelsSale.Shift.EndDate:dd.MM.yyyy HH:mm}");

                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__stationFuelsSale.Count);

                    __cell = RegistrarHelper.GetCell(__row,2);
                    __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.Amount));
                __rowNum++;
            }
        }

        private void FillPaymentCange(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;

            var __data =
                station.Shifts.SelectMany(x => x.FuelsSales).Where(x => x.SaleState == FuelSaleState.Approved)
                    .Where(x => x.FactPour != null && x.FactPour.Payment != null)
                    .Where(x => x.FactSale != null && x.FactSale.Payment != null)
                    .Where(x => !x.FactSale.Payment.Equals(x.FactPour.Payment))
                    .OrderBy(x => x.Shift.BeginDate).ThenBy(x => x.Shift.EndDate).ThenBy(x => x.Date);
            foreach (var __stationFuelsSale in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);

                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__stationFuelsSale.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__stationFuelsSale.Shift.EndDate:dd.MM.yyyy HH:mm}");

                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__stationFuelsSale.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue($"{ __stationFuelsSale.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(__stationFuelsSale.ID);

                __cell = RegistrarHelper.GetCell(__row, 4);
                __cell.SetCellValue(__stationFuelsSale.FactPour.Payment.PaymentTypeName);

                __cell = RegistrarHelper.GetCell(__row, 5);
                __cell.SetCellValue(__stationFuelsSale.FactPour.Payment.AdditionalInfo);
                __cell = RegistrarHelper.GetCell(__row, 6);
                __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FactPour.Amount));

                __cell = RegistrarHelper.GetCell(__row, 7);
                __cell.SetCellValue(__stationFuelsSale.FactSale.Payment.PaymentTypeName);
                __cell = RegistrarHelper.GetCell(__row, 8);
                __cell.SetCellValue(__stationFuelsSale.FactSale.Payment.AdditionalInfo);
                __cell = RegistrarHelper.GetCell(__row, 9);
                __cell.SetCellValue(Convert.ToDouble(__stationFuelsSale.FactSale.Amount));

                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(
                    Convert.ToDouble(__stationFuelsSale.FactPour.Amount - __stationFuelsSale.FactSale.Amount));
                __rowNum++;
            }
        }

        public void FillChargeBonuses(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __data = new List<Tuple<FuelSale, BonusCardPayment>>();
            foreach (var __stationFuelsSale in station.Shifts.SelectMany(x => x.FuelsSales).OrderBy(x => x.Shift.BeginDate).ThenBy(x => x.Shift.EndDate))
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
                    .Where(x => x.Item2.ChargeBonuses != 0)
                    .OrderBy(x => x.Item1.Date)
                    .ToList();
            foreach (var __tuple in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__tuple.Item1.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__tuple.Item1.Shift.EndDate:dd.MM.yyyy HH:mm}");

                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__tuple.Item1.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue($"{ __tuple.Item1.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(__tuple.Item1.ID);

                var __order = __tuple.Item1.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row,4, __order, false);
                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(__tuple.Item2.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 11);
                __cell.SetCellValue(Convert.ToDouble(__tuple.Item2.ChargeBonuses));

                __rowNum++;
            }
        }

        public void FillCancellationBonuses(ISheet sheet, FuelStation station)
        {
            var __rowNum = 4;
            var __data = new List<Tuple<FuelSale, BonusCardPayment>>();
            foreach (var __stationFuelsSale in station.Shifts.SelectMany(x => x.FuelsSales).OrderBy(x => x.Shift.BeginDate).ThenBy(x => x.Shift.EndDate))
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
                __cell.SetCellValue($"{__tuple.Item1.Shift.BeginDate:dd.MM.yyyy HH:mm} - {__tuple.Item1.Shift.EndDate:dd.MM.yyyy HH:mm}");

                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(RegistrarHelper.GetSaleState(__tuple.Item1.SaleState));
                __cell = RegistrarHelper.GetCell(__row, 2);
                __cell.SetCellValue($"{ __tuple.Item1.Date:dd.MM.yy HH:mm:ss}");
                __cell = RegistrarHelper.GetCell(__row, 3);
                __cell.SetCellValue(__tuple.Item1.ID);

                var __order = __tuple.Item1.FactSale;
                if (__order != null)
                    RegistrarHelper.FillOrderData(__row, 4, __order, false);
                __cell = RegistrarHelper.GetCell(__row, 10);
                __cell.SetCellValue(__tuple.Item2.CardNo);
                __cell = RegistrarHelper.GetCell(__row, 11);
                __cell.SetCellValue(Convert.ToDouble(__tuple.Item2.CancellationBonuses));

                __rowNum++;
            }
        }

        private void FillIncorrectConclusion(ISheet sheet, FuelStation station)
        {
            var __rowNum = 3;
            var __data =
                station.Shifts.Where(x => x.Incorrect.Any())
                    .OrderBy(x => x.BeginDate)
                    .ThenBy(x => x.EndDate);
            foreach (var __shift in __data)
            {
                
                
                foreach (var __incorrectConclusionse in __shift.Incorrect)
                {
                    var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                    var __cell = RegistrarHelper.GetCell(__row, 0);
                    __cell.SetCellValue($"{__shift.BeginDate:dd.MM.yyyy HH:mm} - {__shift.EndDate:dd.MM.yyyy HH:mm}");

                    __cell = RegistrarHelper.GetCell(__row, 1);
                    __cell.SetCellValue($"{__incorrectConclusionse.Date:dd.MM.yyyy HH:mm:ss}");

                    __cell = RegistrarHelper.GetCell(__row, 2);
                    __cell.SetCellValue(Convert.ToDouble(__incorrectConclusionse.PositionNo));
                    __rowNum++;
                }
                
                
            }
        }

        private void FillSummaryIncorrectConclusion(ISheet sheet, FuelStation station)
        {
            var __rowNum = 3;
            var __data =
                station.Shifts.Where(x => x.Incorrect.Any())
                    .OrderBy(x => x.BeginDate)
                    .ThenBy(x => x.EndDate);
            foreach (var __shift in __data)
            {
                var __row = RegistrarHelper.GetRow(sheet, __rowNum);
                var __cell = RegistrarHelper.GetCell(__row, 0);
                __cell.SetCellValue($"{__shift.BeginDate:dd.MM.yyyy HH:mm} - {__shift.EndDate:dd.MM.yyyy HH:mm}");

                __cell = RegistrarHelper.GetCell(__row, 1);
                __cell.SetCellValue(__shift.Incorrect.Count());
                __rowNum++;
            }
        }

    }
}
