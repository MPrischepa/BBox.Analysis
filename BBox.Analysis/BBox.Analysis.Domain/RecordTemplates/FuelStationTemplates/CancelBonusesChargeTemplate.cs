using System;
using System.Text.RegularExpressions;
using BBox.Analysis.Core;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class CancelBonusesChargeTemplate : RecordTemplate<FuelStation>
    {
        public static CancelBonusesChargeTemplate Instance { get; } = new CancelBonusesChargeTemplate();

        private CancelBonusesChargeTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            var __pattern = "тр. \\d+, Отмена начисления бонусов:";
            return new Regex(__pattern).IsMatch(record.Entry[0]);
        }

        public override ProcessingResult Process(FuelStation entity, Record record)
        {
            var __pattern = "тр. \\d+, Отмена начисления бонусов:";
            var __str = new Regex(__pattern).Match(record.Entry[0]).Value;
            var __saleNo = Int64.Parse(new Regex("\\d+").Match(__str).Value);
            var __sale = entity.CurrentShift.GetFuelSale(__saleNo, record);
            var __payment = __sale.Payment as BonusCardPayment;
            if (__payment == null) throw new Exception("Ожидается основание Бонусы");
            __pattern = "Отмена начисления бонусов: \\d+,\\d{2}";
            __str = new Regex(__pattern).Match(record.Entry[0]).Value;
            var __bonuses = Decimal.Parse(new Regex("\\d+,\\d{2}").Match(__str).Value);

            __pattern = "Отмена начисления экстрабонусов: \\d+,\\d{2}";
            __str = new Regex(__pattern).Match(record.Entry[1]).Value;
            var __ekstra = Decimal.Parse(new Regex("\\d+,\\d{2}").Match(__str).Value);

            __payment.RemoveChargeBonuses(__sale,__bonuses + __ekstra);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
