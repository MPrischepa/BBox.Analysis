using System;

namespace BBox.Analysis.Processing.AccountCardComparer
{
    public class DataRecord
    {
        public string FuelStationID { get; set; }

        public string FuelStationName { get; set; }

        public DateTime ShiftBeginDate => _shiftBeginDate.Value;

        private Lazy<DateTime> _shiftBeginDate = new Lazy<DateTime>(() => DateTime.MinValue);
        public void SetShiftBeginDate(Func<DateTime> func)
        {
            _shiftBeginDate = new Lazy<DateTime>(func);
        }

        public DateTime ShiftEndDate => _shiftEndDate.Value;

        private Lazy<DateTime> _shiftEndDate = new Lazy<DateTime>(() => DateTime.MaxValue);
        public void SetShiftEndDate(Func<DateTime> func)
        {
            _shiftEndDate = new Lazy<DateTime>(func);
        }

        public string Operator => _operator.Value;

        private Lazy<string> _operator = new Lazy<string>(() => String.Empty);
        public void SetOperator(Func<String> func)
        {
            _operator = new Lazy<String>(func);
        }

        public DateTime SaleDate => _saleDate.Value;

        private Lazy<DateTime> _saleDate = new Lazy<DateTime>(() => DateTime.MaxValue);
        public void SetSaleDate(Func<DateTime> func)
        {
            _saleDate = new Lazy<DateTime>(func);
        }

        public String Product => _product.Value;

        private Lazy<string> _product = new Lazy<string>(() => String.Empty);
        public void SetProduct(Func<String> func)
        {
            _product = new Lazy<String>(func);
        }

        public Int16 FuelColumnID => _fuelColumnID.Value;

        private Lazy<Int16> _fuelColumnID = new Lazy<Int16>(() => Int16.MinValue);
        public void SetFuelColumnID(Func<Int16> func)
        {
            _fuelColumnID = new Lazy<Int16>(func);
        }

        public Int16 FuelHoseID => _fuelHoseID.Value;

        private Lazy<Int16> _fuelHoseID = new Lazy<Int16>(() => Int16.MinValue);
        public void SetFuelHoseID(Func<Int16> func)
        {
            _fuelHoseID = new Lazy<Int16>(func);
        }

        public Decimal Volume => _volume.Value;

        private Lazy<Decimal> _volume = new Lazy<Decimal>(() => Decimal.MaxValue);
        public void SetVolume(Func<Decimal> func)
        {
            _volume = new Lazy<Decimal>(func);
        }

        public String CardNo => _cardNo.Value;

        private Lazy<string> _cardNo = new Lazy<string>(() => String.Empty);
        public void SetCardNo(Func<String> func)
        {
            _cardNo = new Lazy<String>(func);
        }

        public String Customer => _customer.Value;

        private Lazy<string> _customer = new Lazy<string>(() => String.Empty);
        public void SetCustomer(Func<String> func)
        {
            _customer = new Lazy<String>(func);
        }
    }
}
