//using System;
//using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

//namespace BBox.Analysis.Domain.PaymentTypes
//{
//    /// <summary>
//    /// прочие виды оплаты (неклассифицированные)
//    /// </summary>
//    internal sealed class OtherPayment : Payment
//    {
//        public OtherPayment(string paymentName) : base(paymentName)
//        {

//        }

//        #region Overrides of Payment

//        public override string AdditionalInfo => String.Empty;
//        public override ClientType ClientType => ClientType.Other;
//        public override PaymentType PaymentType =>PaymentType.WithoutPayment;

//        #endregion
//    }
//}
