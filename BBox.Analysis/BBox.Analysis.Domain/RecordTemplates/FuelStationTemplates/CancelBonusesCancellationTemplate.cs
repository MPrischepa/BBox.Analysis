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
    internal class CancelBonusesCancellationTemplate : RecordTemplate<FuelStation>
    {
        public static CancelBonusesCancellationTemplate Instance { get; } = new CancelBonusesCancellationTemplate();

        private CancelBonusesCancellationTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            var __pattern = "тр. \\d+, Отмена списания бонусов:";
            return new Regex(__pattern).IsMatch(record.Entry[0]);
        }

        public override ProcessingResult Process(FuelStation entity, Record record)
        {
            var __pattern = "тр. \\d+, Отмена списания бонусов:";
            var __str = new Regex(__pattern).Match(record.Entry[0]).Value;
            var __saleNo = Int64.Parse(new Regex("\\d+").Match(__str).Value);
            var __sale = entity.CurrentShift.GetFuelSale(__saleNo, record);
            var __payment = __sale.Payment as BonusCardPayment;
            if (__payment == null) throw new Exception("Ожидается основание Бонусы");
            __pattern = "Отмена списания бонусов: \\d+,\\d{2}";
            __str = new Regex(__pattern).Match(record.Entry[0]).Value;
            var __bonuses = Decimal.Parse(new Regex("\\d+,\\d{2}").Match(__str).Value);
            __payment.RemoveCancellationBonuses(__sale,__bonuses);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
