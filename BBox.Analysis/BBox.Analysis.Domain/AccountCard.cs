using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain
{
    public class AccountCard : ClientCard
    {
        private IList<PaymentAccepted> _acceptedPayments;
        public AccountCard(String cardNo) : base(cardNo)
        {
            _acceptedPayments = new List<PaymentAccepted>();
        }


        #region Overrides of ClientCard

        public override ClientCardType ClientCardType => ClientCardType.AccountCard;

        #endregion

        public IEnumerable<PaymentAccepted> AcceptedPayments => _acceptedPayments;

        public void AcceptPayment(FuelSale sale)
        {
            var __order = sale.GetCurrentOrder();
            _acceptedPayments.Add(new PaymentAccepted
            {
                OperationDate = sale.Date,
                Sale = sale,
                Amount = __order.Amount,
                Price = __order.Price,
                ProductName = __order.ProductName,
                Volume = __order.Volume
            });
        }
    }

    public class PaymentAccepted
    {
        public DateTime OperationDate { get; set; }
        public FuelSale Sale { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public decimal Amount { get; set; }
    }
}
