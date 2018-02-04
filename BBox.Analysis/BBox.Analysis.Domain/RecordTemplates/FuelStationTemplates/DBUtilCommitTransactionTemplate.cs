using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBox.Analysis.Core;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class DbUtilCommitTransactionTemplate : RecordTemplate<FuelStation>
    {
        public static DbUtilCommitTransactionTemplate Instance { get; } = new DbUtilCommitTransactionTemplate();

        private DbUtilCommitTransactionTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return new Regex("Применение транзакции через DBUtils. НомерТранзакции: \\d+").IsMatch(record.Entry[0]);
        }

        private String GetValueStr(string template)
        {
            var __spl = template.Split(':');
            return __spl[1];
        }

        public override ProcessingResult Process(FuelStation entity, Record record)
        {
            var __saleNo = Int64.Parse(new Regex("\\d+").Match(record.Entry[0]).Value);
            var __sale = entity.CurrentShift.GetFuelSale(__saleNo, record);
            if (__sale.PourState == FuelPourState.PourStart)
            {
                var __order = __sale.GetCurrentOrder();
                __order.Price = Decimal.Parse(GetValueStr(record.Entry[1]));
                __order.Volume = Decimal.Parse(GetValueStr(record.Entry[2]));
                __order.Amount = Decimal.Parse(GetValueStr(record.Entry[3]));
                __sale.PourState = FuelPourState.OrderFinished;
            }
            if (__sale.PourState == FuelPourState.OrderFinished)
            {
                var __order = __sale.GetCurrentOrder();
                __order.Price = Decimal.Parse(GetValueStr(record.Entry[1]));
                __order.Volume = Decimal.Parse(GetValueStr(record.Entry[2]));
                __order.Amount = Decimal.Parse(GetValueStr(record.Entry[3]));
                __sale.SaleState = FuelSaleState.Approved;
                return ProcessingResult.SelfProcessing;
            }

            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
