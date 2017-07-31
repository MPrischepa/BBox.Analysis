using System;
using System.Text.RegularExpressions;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain
{
    public class Order
    {
        public FuelColumn FuelColumn { get; private set; }

        public Payment Payment { get; private set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        private string _doseString = String.Empty;

        public String DoseString
        {
            get
            {
                return _doseString;
            }
            set
            {
                _doseString = value;
                if (String.IsNullOrWhiteSpace(_doseString) || _doseString.Equals("Полный бак")) return;
                var __volumePattern = @"\d*\,\d{2}л";
                var __volumeReg = new Regex(__volumePattern);
                if (__volumeReg.IsMatch(_doseString))
                {
                    var __volumeStr = __volumeReg.Match(_doseString).Value.TrimEnd('л');
                    Volume = Decimal.Parse(__volumeStr);
                }
                var __amountPattern = @"\d*\,\d{2}р";
                var __amountReg = new Regex(__amountPattern);
                if (__amountReg.IsMatch(_doseString))
                {
                    var __amountStr = __amountReg.Match(_doseString).Value.TrimEnd('р');
                    Amount = Decimal.Parse(__amountStr);
                }
            }
        }

        public Decimal Volume { get; set; }

        public decimal Amount { get; set; }

        public Order(Payment payment)
        {
            //if (payment == null)
            //    throw new ArgumentNullException(nameof(payment));
            Payment = payment;
        }

        internal void SetFuelColumn(FuelColumn column)
        {
            FuelColumn = column;
            
        }

        internal void SetPayment(Payment payment)
        {
            Payment = payment;
        }

    }
}
