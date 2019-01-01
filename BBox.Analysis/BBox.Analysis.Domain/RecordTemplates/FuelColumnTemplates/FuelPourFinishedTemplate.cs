using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelColumnTemplates
{
    internal class FuelPourFinishedTemplate : RecordTemplate<FuelColumn>
    {
        public static FuelPourFinishedTemplate Instance { get; } = new FuelPourFinishedTemplate();
        private FuelPourFinishedTemplate()
        {
        }
        #region Overrides of RecordTemplate<FuelColumn>

        public override bool IsMatch(FuelColumn entity, Record record)
        {
            return record.Entry[1].Equals("На ТРК закончен отпуск топлива");
        }

        public override ProcessingResult Process(FuelColumn entity, Record record)
        {
            var __hoseName = record.Entry.Length > 2 ? Int16.Parse(new Regex("\\d+").Match(record.Entry[2]).Value) : (short)1;
            entity.Finished(__hoseName);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }

    internal class FuelPourStartedTemplate : RecordTemplate<FuelColumn>
    {
        public static FuelPourStartedTemplate Instance { get; } = new FuelPourStartedTemplate();
        private FuelPourStartedTemplate()
        {
        }
        #region Overrides of RecordTemplate<FuelColumn>

        public override bool IsMatch(FuelColumn entity, Record record)
        {
            return record.Entry[1].Equals("На ТРК идет отпуск топлива");
        }

        public override ProcessingResult Process(FuelColumn entity, Record record)
        {
            var __hoseName = record.Entry.Length > 2
                ? Int16.Parse(new Regex("\\d+").Match(record.Entry[2]).Value)
                : (short) 1;
            entity.Start(__hoseName);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }

    internal class FuelPourStopedTemplate : RecordTemplate<FuelColumn>
    {
        public static FuelPourStopedTemplate Instance { get; } = new FuelPourStopedTemplate();
        private FuelPourStopedTemplate()
        {
        }
        #region Overrides of RecordTemplate<FuelColumn>

        public override bool IsMatch(FuelColumn entity, Record record)
        {
            return record.Entry[1].Equals("ТРК остановлена");
        }

        public override ProcessingResult Process(FuelColumn entity, Record record)
        {
            var __hoseName = Int16.Parse(new Regex("\\d+").Match(record.Entry[2]).Value);
            entity.Start(__hoseName);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
