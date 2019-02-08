using BBox.Analysis.Domain.RecordTemplates.PaymentTemplates;

namespace BBox.Analysis.Domain.PaymentTypes
{
    internal class AccountCardPayment:Payment
    {

        static AccountCardPayment()
        {
            BlackBoxObject.AddTemplate(SetCardTemplate<AccountCardPayment>.Instance);
        }

        private readonly IAccountCardFactory _factory;

        public AccountCardPayment(PaymentTypeDescription descr, IAccountCardFactory factory) : base(descr)
        {
            _factory = factory;
        }

        #region Overrides of Payment

        internal override void PaymentAccepted(FuelSale sale)
        {
            ((AccountCard)ClientCard).AcceptPayment(sale);
        }

        #endregion

        #region Implementation of IContainsClientCard


        public override void SetCardNo(string cardNo)
        {
            ClientCard = _factory.GetAccountCard(cardNo);
        }

        #endregion
    }
}
