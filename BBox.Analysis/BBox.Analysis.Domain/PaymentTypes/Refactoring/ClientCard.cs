using System;

namespace BBox.Analysis.Domain.PaymentTypes
{
    public abstract class ClientCard
    {

        public string CardNo { get; }

        protected ClientCard(String cardNo)
        {
            CardNo = cardNo;
        }

        public abstract ClientCardType ClientCardType { get; }
    }
}
