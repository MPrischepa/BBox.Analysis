using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Core;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class SelectedPaymentTemplate : RecordTemplate<FuelSale>
    {
        public static SelectedPaymentTemplate Instance { get; } = new SelectedPaymentTemplate();
        private const string SelectedPayment = "Выбрано основание";
        private SelectedPaymentTemplate()
        {

        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            return record.Entry[1].StartsWith(SelectedPayment);
        }

        public override ProcessingResult Process(FuelSale entity, Record record)
        {
            var __paymentType = record.Entry[1].Substring(SelectedPayment.Length).Trim().Trim('\"').Trim();
            var __payment = PaymentFactory.Instance.CreatePayment(__paymentType);
            entity.SetPayment(__payment);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
