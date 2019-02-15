using System;

namespace BBox.Analysis.Domain.PaymentTypes
{
    public class PaymentConfig
    {
        public ClientCardType ClientCardType { get; set; }

        public PaymentType PaymentType { get; set; }

        public String Name { get; set; }

        public ClientType ClientType { get; set; }
    }
}
