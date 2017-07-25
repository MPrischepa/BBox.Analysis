using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.RecordTemplates.FuelColumnTemplates
{
    internal class TransactionFinishedTemplate : RecordTemplate<FuelColumn>
    {
        public static TransactionFinishedTemplate Instance { get; } = new TransactionFinishedTemplate();

        private TransactionFinishedTemplate()
        {
        }

        #region Overrides of RecordTemplate<FuelColumn>

        public override bool IsMatch(FuelColumn entity, Record record)
        {
            return new Regex("Конец транзакции: Рукав: \\d+").IsMatch(record.Entry[1]);
        }

        public override bool Process(FuelColumn entity, Record record)
        {
            var __hoseName = Int16.Parse(new Regex("\\d+").Match(record.Entry[1]).Value);
            var __hose = entity.GetFuelHouse(__hoseName);
            if (new Regex("Счетчик: NAN").IsMatch(record.Entry[2])) return true;
            var __value = Double.Parse(new Regex("\\d+,\\d{2}").Match(record.Entry[2]).Value);
            __hose.SetValue(__value);
            return true;
        }

        #endregion


    }
}
