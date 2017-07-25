//using System;

//namespace BBox.Analysis.Domain.PaymentTypes
//{
//    /// <summary>
//    /// служебные виды оплаты
//    /// </summary>
//    internal sealed class ServicePayment : Payment
//    {
        
//        public ServicePayment(string paymentName) : base(paymentName)
//        {

//        }

//        #region Overrides of Payment

//        public override string AdditionalInfo => String.Empty;
//        public override ClientType ClientType => ClientType.Service;
//        public override PaymentType PaymentType => PaymentType.WithoutPayment;

//        #endregion
//    }
//}
