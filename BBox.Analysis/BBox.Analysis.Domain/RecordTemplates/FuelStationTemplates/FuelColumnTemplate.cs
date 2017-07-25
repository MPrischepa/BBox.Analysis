using System;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class FuelColumnTemplate:RecordTemplate<FuelStation>
    {
        public static FuelColumnTemplate Instance { get; } = new FuelColumnTemplate();

        private FuelColumnTemplate()
        {
            
        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return record.Entry[0].StartsWith("ТРК");
        }

        private Int16 GetTrkNo(String str)
        {
            var __spl = str.Split(':');
            return Int16.Parse(__spl[1]);
        }
        public override bool Process(FuelStation entity, Record record)
        {
            var __trkNo = GetTrkNo(record.Entry[0]);
            var __trk = entity.GetFuelColumn(__trkNo);
            return BlackBoxObject.ProcessRecord(__trk,record);
        }

        #endregion
    }
}
