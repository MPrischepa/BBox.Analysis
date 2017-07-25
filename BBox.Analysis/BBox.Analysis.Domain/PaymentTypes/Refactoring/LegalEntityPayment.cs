//using System;
//using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

//namespace BBox.Analysis.Domain.PaymentTypes
//{
//    /// <summary>
//    /// Юр лицо (безнал)
//    /// </summary>
//    internal class LegalEntityPayment : Payment
//    {
//        public LegalEntityPayment(string paymentName) : base(paymentName)
//        {

//        }

//        #region Overrides of Payment

//        public override string AdditionalInfo => String.Empty;
//        public override ClientType ClientType => ClientType.LegalEntity;
//        public override PaymentType PaymentType => PaymentType.CashlessSettlement;

//        #endregion
//    }
//}
