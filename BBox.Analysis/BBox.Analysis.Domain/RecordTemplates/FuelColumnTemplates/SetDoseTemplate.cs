using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelColumnTemplates
{
    internal class SetDoseTemplate : RecordTemplate<FuelColumn>
    {
        public static SetDoseTemplate Instance { get; } = new SetDoseTemplate();

        private SetDoseTemplate()
        {
        }

        #region Overrides of RecordTemplate<FuelColumn>

        public override bool IsMatch(FuelColumn entity, Record record)
        {
            return new Regex("Уст. доза: Рукав: \\d+").IsMatch(record.Entry[1]);
        }

        public override ProcessingResult Process(FuelColumn entity, Record record)
        {
            var __hoseName = Int16.Parse(new Regex("\\d+").Match(record.Entry[1]).Value);
            var __hose = entity.GetFuelHouse(__hoseName);
            if (new Regex("Счетчик: NAN").IsMatch(record.Entry[3]))
            {
                __hose.SetValue(__hose.Value);
                return ProcessingResult.SelfProcessing;
            }
            var __value = Double.Parse(new Regex("\\d+,\\d{2}").Match(record.Entry[3]).Value);
            __hose.SetValue(__value);
            return ProcessingResult.SelfProcessing;
        }

        #endregion

    }
}
