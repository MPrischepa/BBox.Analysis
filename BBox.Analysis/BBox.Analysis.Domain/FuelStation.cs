using System;
using System.Collections.Generic;
using System.Linq;
using BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates;

namespace BBox.Analysis.Domain
{
    public class FuelStation 
    {
        static FuelStation()
        {
            BlackBoxObject.AddTemplate(FuelSaleTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelColumnTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelColumnStartTemplate.Instance);
            BlackBoxObject.AddTemplate(IgnoreRecordTemplate.Instance);
            BlackBoxObject.AddTemplate(ShiftOpenedTemplate.Instance);
            BlackBoxObject.AddTemplate(ShiftClosedTemplate.Instance);
            BlackBoxObject.AddTemplate(BonusesChargeTemplate.Instance);
            BlackBoxObject.AddTemplate(CancelBonusesChargeTemplate.Instance);
            BlackBoxObject.AddTemplate(BonusesCancellationTemplate.Instance);
            BlackBoxObject.AddTemplate(CancelBonusesCancellationTemplate.Instance);
            BlackBoxObject.AddTemplate(DbUtilCommitTransactionTemplate.Instance);
            BlackBoxObject.AddTemplate(IncorrectConclusionTemplate.Instance);
        }
        public String Name { get; set; }

        private readonly LinkedList<Shift> _shifts;
        public IEnumerable<Shift> Shifts => _shifts;

        private readonly IDictionary<Int16, FuelColumn> _fuelColumns;

        public IEnumerable<FuelColumn> FuelColumns => _fuelColumns.Values;

        public FuelColumn GetFuelColumn(Int16 columnID)
        {
            FuelColumn __column;
            if (_fuelColumns.TryGetValue(columnID, out __column)) return __column;
            __column = new FuelColumn(this)
            {
                ID = columnID
            };
            _fuelColumns.Add(columnID, __column);
            return __column;
        }

        //public FuelSale GetFuelSale(Int64 saleID, Record record)
        //{
        //    if (!_shifts.Any()) throw new Exception("Не создана ниодна смена");
        //    var __currentShift = _shifts.Last.Value;
        //    return __currentShift.GetFuelSale(saleID, record);
        //}

       
        public FuelStation()
        {
            _fuelColumns = new Dictionary<short, FuelColumn>();
            _shifts = new LinkedList<Shift>();
        }

        public Shift CurrentShift => !_shifts.Any() ? null : _shifts.Last.Value;

        public void AddShift(Shift shift)
        {
            _shifts.AddLast(shift);
        }
    }

   
}
