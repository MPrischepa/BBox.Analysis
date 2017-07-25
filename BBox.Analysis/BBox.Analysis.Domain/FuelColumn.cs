using System;
using System.Collections.Generic;
using System.Linq;
using BBox.Analysis.Domain.RecordTemplates.FuelColumnTemplates;

namespace BBox.Analysis.Domain
{
    public class FuelColumn
    {
        static FuelColumn()
        {
            BlackBoxObject.AddTemplate(IgnoreRecordTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelPourFinishedTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelPourStartedTemplate.Instance);
            BlackBoxObject.AddTemplate(SetDoseTemplate.Instance);
            BlackBoxObject.AddTemplate(TransactionFinishedTemplate.Instance);
        }
        public Int16 ID { get; set; }

        public IEnumerable<FuelHose> FuelHouses => _fuelHoses.Values;

        public FuelStation FuelStation { get; private set; }

        private readonly LinkedList<FuelSale> _sales;

        private readonly IDictionary<Int16, FuelHose> _fuelHoses;
        public FuelColumn(FuelStation fuelStation)
        {
            FuelStation = fuelStation;
            _sales = new LinkedList<FuelSale>();
            _fuelHoses = new Dictionary<short, FuelHose>();
        }

        public void AddFuelOrder(FuelSale sale)
        {
            if (!_sales.Any() || !_sales.Last.Value.Equals(sale))
                _sales.AddLast(sale);
        }

        public void Start(Int16 hoseName)
        {
            if (!_sales.Any() || _sales.Last.Value.SaleState != FuelSaleState.Active) return;
            var __sale = _sales.Last.Value;
            if (__sale.PourState > FuelPourState.PourStart) return;
            __sale.PourState = FuelPourState.PourStart;
            var __hose = GetFuelHouse(hoseName);
           // __sale.StartCounterValue = __hose.Value;
            __sale.FuelHouse = __hose;
        }

        public void Finished()
        {
            var __sale = _sales.Last.Value;
            if (__sale.PourState > FuelPourState.PourFinished) return;
            __sale.PourState = FuelPourState.PourFinished;
            __sale.GetCurrentOrder();
        }

        public void RemoveSale(FuelSale sale)
        {
            if (_sales.Any())
                _sales.Remove(sale);
        }

        public FuelHose GetFuelHouse(Int16 name)
        {
            FuelHose __hose;
            if (_fuelHoses.TryGetValue(name, out __hose)) return __hose;
            __hose = new FuelHose(this,name);
            _fuelHoses.Add(name,__hose);
            return __hose;
        }
    }

   
   
}
