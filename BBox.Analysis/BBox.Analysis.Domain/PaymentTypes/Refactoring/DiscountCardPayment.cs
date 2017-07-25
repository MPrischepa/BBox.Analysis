using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

namespace BBox.Analysis.Domain.PaymentTypes
{
    internal class DiscountCardPayment : Payment
    {
        static DiscountCardPayment()
        {
            BlackBoxObject.AddTemplate(SetCardTemplate<DiscountCardPayment>.Instance);
        }

        
        public DiscountCardPayment(PaymentTypeDescription descr) : base(descr)
        {
           
        }

       

        #region Implementation of IContainsClientCard

        
        public override void SetCardNo(string cardNo)
        {
            ClientCard = new OtherClientCard(cardNo,ClientCardType.DiscountCard);
        }

        #endregion
    }
}
