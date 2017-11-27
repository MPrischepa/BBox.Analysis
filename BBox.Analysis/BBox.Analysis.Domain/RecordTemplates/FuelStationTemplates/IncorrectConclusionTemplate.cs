using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class IncorrectConclusionTemplate : RecordTemplate<FuelStation>
    {

        public static IncorrectConclusionTemplate Instance { get; } = new IncorrectConclusionTemplate();

        private IncorrectConclusionTemplate()
        {

        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return record.Entry[0].Equals("ЗАПУСК ПРОГРАММЫ ПОСЛЕ НЕКОРРЕКТНОГО ЗАВЕРШЕНИЯ");
        }

        public override bool Process(FuelStation entity, Record record)
        {
            entity.CurrentShift.AddIncorrectConclusion(record.TimeRecord,record.ID);
            return true;
        }

        #endregion
    }
}
