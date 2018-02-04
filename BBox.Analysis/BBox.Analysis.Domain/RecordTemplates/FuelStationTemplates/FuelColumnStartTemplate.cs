using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class FuelColumnStartTemplate: RecordTemplate<FuelStation>
    {
        public static FuelColumnStartTemplate Instance { get; } = new FuelColumnStartTemplate();

        private FuelColumnStartTemplate()
        {

        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return record.Entry[0].StartsWith("Ручной пуск");
        }

        public override ProcessingResult Process(FuelStation entity, Record record)
        {
            //var __trkPattern = @"ТРК\s\d*";
            //var __trkNo = Int16.Parse(new Regex(__trkPattern).Match(record.Entry[0]).Value.Replace("ТРК",String.Empty));
            //var __column = entity.GetFuelColumn(__trkNo);
            //__column.Start();
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
