using System;
using System.Collections.Specialized;

namespace BBox.Analysis.Domain.PaymentTypes
{
    public class PaymentTypeDescription
    {
        public String Name { get; set; }
        public ClientType ClientType { get; set; }

        public PaymentType PaymentType { get; set; }

        //public ClientCardType ClientCardType { get; set; }
    }
}
