using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class ShiftClosedTemplate : RecordTemplate<FuelStation>
    {
        public static ShiftClosedTemplate Instance { get; } = new ShiftClosedTemplate();

        private ShiftClosedTemplate()
        {

        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return record.Entry[0].Equals("Смена закрыта");
        }

        public override bool Process(FuelStation entity, Record record)
        {
            var __shift = entity.CurrentShift;
            __shift?.FinishedShift(record.TimeRecord);
            return true;
        }

        #endregion
    }
}
