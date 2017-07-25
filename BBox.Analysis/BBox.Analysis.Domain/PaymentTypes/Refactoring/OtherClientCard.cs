namespace BBox.Analysis.Domain.PaymentTypes
{
    internal class OtherClientCard: ClientCard
    {
        public OtherClientCard(string cardNo, ClientCardType cardType) : base(cardNo)
        {
            ClientCardType = cardType;
        }

        #region Overrides of ClientCard

        public override ClientCardType ClientCardType { get; }

        #endregion
    }
}
