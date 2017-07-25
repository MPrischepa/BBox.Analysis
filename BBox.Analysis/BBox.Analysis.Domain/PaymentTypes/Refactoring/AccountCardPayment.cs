using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

namespace BBox.Analysis.Domain.PaymentTypes
{
    internal class AccountCardPayment:Payment
    {
        static AccountCardPayment()
        {
            BlackBoxObject.AddTemplate(SetCardTemplate<AccountCardPayment>.Instance);
        }

       
        public AccountCardPayment(PaymentTypeDescription descr) : base(descr)
        {
            
        }

        #region Overrides of Payment

       
       
        #endregion

        #region Implementation of IContainsClientCard

        
        public override void SetCardNo(string cardNo)
        {
            ClientCard = new OtherClientCard(cardNo,ClientCardType.AccountCard);
        }

        #endregion
    }
}
