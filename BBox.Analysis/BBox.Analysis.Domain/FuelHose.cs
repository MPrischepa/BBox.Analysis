using System;

namespace BBox.Analysis.Domain
{
    public class FuelHose
    {
        public FuelColumn FuelColumn { get; private set; }

        public Decimal Value { get; private set; }

        public Int16 Name { get; private set; }

        public FuelHose(FuelColumn column, Int16 name)
        {
            FuelColumn = column;
            Name = name;
            Value = 0;
        }

        public void SetValue(Double value)
        {
            if (value.Equals(Double.NaN)) return;
            Value = Convert.ToDecimal(value);
            OnValueChanged(Value);
        }

        public event EventHandler<Decimal> ValueChanged;

        protected virtual void OnValueChanged(decimal e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
