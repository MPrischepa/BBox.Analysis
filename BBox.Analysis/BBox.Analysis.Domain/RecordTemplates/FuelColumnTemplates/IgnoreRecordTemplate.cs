using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelColumnTemplates
{
    internal class IgnoreRecordTemplate:RecordTemplate<FuelColumn>
    {
        private static IgnoreRecordTemplate _instance;
        private static readonly object _lockObject = new object();
        private readonly IList<String> _ignoreString;

        public static IgnoreRecordTemplate Instance
        {
            get
            {
                lock (_lockObject)
                {
                    return _instance ?? (_instance = new IgnoreRecordTemplate());
                }
            }
        }
        private IgnoreRecordTemplate()
        {
            _ignoreString = new List<string>
            {
                "На ТРК снят пистолет"
            };
        }
        #region Overrides of RecordTemplate<FuelColumn>

        public override bool IsMatch(FuelColumn entity, Record record)
        {
            return _ignoreString.Any(x => record.Entry[1].TrimStart().StartsWith(x));
        }

        public override ProcessingResult Process(FuelColumn entity, Record record)
        {
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
