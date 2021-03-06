﻿using System;
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


        public IEnumerable<IncorrectConclusions> Incorrect => _incorrect;

        private readonly IList<IncorrectConclusions> _incorrect;

        public void AddIncorrectConclusion(DateTime date, long positionNo)
        {
            _incorrect.Add(new IncorrectConclusions
            {
                Date = date,
                PositionNo = positionNo
            });
        }

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
            _startShiftHoses = station.FuelColumns.SelectMany(x => x.FuelHoses.Select(y => new ShiftHose
            {
                FuelColumn = x.ID,
                FuelHose = y.Name,
                Value = y.Value

            })).ToList();
            _finishedShiftHoses = new List<ShiftHose>();
            _incorrect = new List<IncorrectConclusions>();
        }

        public void StartShift(DateTime beginDate)
        {
            BeginDate = beginDate;
            _startShiftHoses = FuelStation.FuelColumns.SelectMany(x => x.FuelHoses.Select(y => new ShiftHose
            {
                FuelColumn = x.ID,
                FuelHose = y.Name,
                Value = y.Value

            })).ToList();
        }

        public void FinishedShift(DateTime endDate)
        {
            _finishedShiftHoses = FuelStation.FuelColumns.SelectMany(x => x.FuelHoses.Select(y => new ShiftHose
            {
                FuelColumn = x.ID,
                FuelHose = y.Name,
                Value = y.Value

            })).ToList();
            EndDate = endDate;
        }
    }

    public class ShiftHose
    {
        public Int16 FuelColumn { get; set; }
        public Int16 FuelHose { get; set; }

        public Decimal Value { get; set; }
    }

    public class IncorrectConclusions
    {
        public DateTime Date { get; set; }

        public long PositionNo { get; set; }
    }
}
