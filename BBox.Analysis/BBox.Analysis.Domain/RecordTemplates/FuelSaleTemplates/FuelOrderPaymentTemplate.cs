using System.Text.RegularExpressions;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class FuelOrderPaymentTemplate : RecordTemplate<FuelSale>
    {
        public static FuelOrderPaymentTemplate Instance { get; } = new FuelOrderPaymentTemplate();

        private FuelOrderPaymentTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelSale>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            var __pattern = "обработка\\s\"+(\\w*\\s*)*\"\\s-\\s\"Оплата\":\\sуспешно";
            var __reg = new Regex(__pattern);
            return __reg.IsMatch(record.Entry[1]);
        }

        public override ProcessingResult Process(FuelSale entity, Record record)
        {
            entity.Payment?.PaymentAccepted(entity);
            return ProcessingResult.SelfProcessing;
        }
    }

    #endregion

}
