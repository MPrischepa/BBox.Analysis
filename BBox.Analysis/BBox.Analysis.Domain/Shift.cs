using System;
using System.Collections.Generic;
using System.Linq;

namespace BBox.Analysis.Domain
{
    public class Shift
    {
        private IList<ShiftHose> _startShiftHoses;
        private IList<ShiftHose> _finishedShiftHoses;
        public IEnumerable<ShiftHose> StartShiftHoses => _startShiftHoses;
        public IEnumerable<ShiftHose> FinishedShiftHoses => _finishedShiftHoses;
        public FuelStation FuelStation { get; }
        public DateTime BeginDate { get; private set; }

        public DateTime EndDate { get; private set; }

        private readonly IDictionary<Int64, FuelSale> _fuelSales;

        public IEnumerable<FuelSale> FuelsSales => _fuelSales.Values;


        public FuelSale GetFuelSale(Int64 saleID, Record record)
        {
            FuelSale __sale;
            if (_fuelSales.TryGetValue(saleID, out __sale)) return __sale;
            __sale = new FuelSale(FuelStation, this)
            {
                ID = saleID,
                PourState = FuelPourState.NewOrder,
                SaleState = FuelSaleState.Active,
                Date = record.TimeRecord
            };
            _fuelSales.Add(saleID, __sale);
            return __sale;
        }
        public Shift(FuelStation station)
        {
            FuelStation = station;
            _fuelSales = new Dictionary<long, FuelSale>();
            _startShiftHoses = station.FuelColumns.SelectMany(x => x.FuelHouses).Select(x => new ShiftHose
            {
                Value = x.Value,
                FuelHose = x.Name
            }).ToList();
            _finishedShiftHoses = new List<ShiftHose>();
        }

        public void StartShift(DateTime beginDate)
        {
            BeginDate = beginDate;
            _startShiftHoses = FuelStation.FuelColumns.SelectMany(x => x.FuelHouses).Select(x => new ShiftHose
            {
                Value = x.Value,
                FuelHose = x.Name
            }).ToList();
        }

        public void FinishedShift(DateTime endDate)
        {
            _finishedShiftHoses = FuelStation.FuelColumns.SelectMany(x => x.FuelHouses).Select(x => new ShiftHose
            {
                Value = x.Value,
                FuelHose = x.Name
            }).ToList();
            EndDate = endDate;
        }
    }

    public class ShiftHose
    {
        public Int16 FuelHose { get; set; }

        public Decimal Value { get; set; }
    }
}
