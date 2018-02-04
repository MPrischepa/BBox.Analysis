using System;
using System.Collections.Generic;
using BBox.Analysis.Domain.PaymentTypes;
using BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates;

namespace BBox.Analysis.Domain
{
    public class FuelSale: Entity
    {
        static FuelSale()
        {
            BlackBoxObject.AddTemplate(PaymentAddedTemplate.Instance);
            BlackBoxObject.AddTemplate(PaymentClearTemplate.Instance);
            BlackBoxObject.AddTemplate(SelectedPaymentTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelOrderAddedTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelOrderToFuelStationTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelOrderPourFixedTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelSaleApprovedTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelSaleCanceledTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelOrderMoveTemplate.Instance);
            BlackBoxObject.AddTemplate(FuelOrderCalculatedTemplate.Instance);
            BlackBoxObject.AddTemplate(PrintCheck.Instance);
        }
        public Int64 ID { get; set; }

        public Decimal StartCounterValue { get; private set; }

        public Decimal FinishedCounterValue { get; private set; }
        public DateTime Date { get; set; }

       // public FuelColumn FuelColumn { get; private set; }
        public FuelStation FuelStation { get; private set; }

        public Payment Payment { get; private set; }

        /// <summary>
        /// состояние налива
        /// </summary>
        public FuelPourState PourState { get; set; }
        /// <summary>
        /// состояние заказа
        /// </summary>
        public FuelSaleState SaleState { get; set; }

        private FuelHose _fuelHose;

        public FuelHose FuelHouse
        {
            get
            {
                return _fuelHose;
            }
            set
            {
                if (_fuelHose != null)
                    _fuelHose.ValueChanged -= FuelHoseOnValueChanged;
                _fuelHose = value;
                StartCounterValue = _fuelHose.Value;
                _fuelHose.ValueChanged +=FuelHoseOnValueChanged;
            }
        }

        private void FuelHoseOnValueChanged(object sender, decimal @decimal)
        {
            FinishedCounterValue = @decimal;
            if (_fuelHose != null)
                _fuelHose.ValueChanged -= FuelHoseOnValueChanged;
        }

        public Order PreOrder { get; private set; }

        public Order FactPour { get; private set; }

        public Order FactSale { get; private set; }

        public Boolean IsCheckPrinted { get; private set; }

        public Decimal CheckAmount { get; private set; }

        internal Order GetCurrentOrder()
        {
            switch (PourState)
            {
                case FuelPourState.NewOrder:
                    return null;
                case FuelPourState.PreOrder:
                    return PreOrder ?? (PreOrder = new Order(Payment));
                
                case FuelPourState.PourStart:
                case FuelPourState.PourFinished:
                    if (FactPour == null)
                    {
                        var __order = new Order(Payment);
                        var __cloneOrder = PreOrder;
                        if (__cloneOrder == null)
                        {
                            PreOrder = __order;
                            return __order;
                        }
                        __order.Amount = __cloneOrder.Amount;
                        __order.SetFuelColumn(__cloneOrder.FuelColumn);
                        __order.Price = __cloneOrder.Price;
                        __order.ProductName = __cloneOrder.ProductName;
                        __order.Volume = __cloneOrder.Volume;
                        FactPour = __order;
                    }
                    return FactPour;
                case FuelPourState.OrderFinished:
                    if (FactSale == null)
                    {
                        var __order = new Order(Payment);
                        var __cloneOrder = FactPour ?? PreOrder;
                        if (__cloneOrder == null) return null;
                        __order.Amount = __cloneOrder.Amount;
                        __order.SetFuelColumn(__cloneOrder.FuelColumn);
                        __order.Price = __cloneOrder.Price;
                        __order.ProductName = __cloneOrder.ProductName;
                        __order.Volume = __cloneOrder.Volume;
                        FactSale = __order;
                    }
                    return FactSale;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal void ClearPayment()
        {
            Payment = null;
        }
        internal void SetPayment(Payment payment)
        {
            Payment = payment;
            var __order = GetCurrentOrder();
            __order?.SetPayment(payment);
        }

       
        public FuelSale(FuelStation fuelsStation, Shift shift)
        {
            FuelStation = fuelsStation;
            Shift = shift;
        }

        public Shift Shift { get; }

        internal void SetFuelColumn(FuelColumn column)
        {
            var __order = GetCurrentOrder();
            if (!column.Equals(__order.FuelColumn))
                __order.SetFuelColumn(column);
            column.AddFuelOrder(this);
        }

        internal void MoveFuelOrder(FuelColumn column)
        {
            var __order = GetCurrentOrder();
            if (__order.FuelColumn.Equals(column)) return;
            __order.FuelColumn.RemoveSale(this);
            SetFuelColumn(column);
            if (_fuelHose == null) return;
            _fuelHose.ValueChanged -= FuelHoseOnValueChanged;
            _fuelHose = null;
        }

        internal void SalePrintCheck(Decimal amount)
        {
            IsCheckPrinted = true;
            CheckAmount = amount;
        }

        #region Overrides of Entity

        private readonly IList<Record> _records = new List<Record>();
        public override IEnumerable<Record> Records => _records;
        public override void AddRecord(Record record)
        {
            _records.Add(record);
        }

        #endregion
    }

}
