using System;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class ShiftOpenedTemplate:RecordTemplate<FuelStation>
    {
        public static ShiftOpenedTemplate Instance { get; } = new ShiftOpenedTemplate();

        private ShiftOpenedTemplate()
        {
            
        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return record.Entry[0].Equals("Смена открыта");
        }

        public override ProcessingResult Process(FuelStation entity, Record record)
        {
            var __shift = new Shift(entity);
            __shift.StartShift(record.TimeRecord);
            entity.AddShift(__shift);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
