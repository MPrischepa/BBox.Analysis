using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Core;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain.RecordTemplates.PaymentTemplates
{
    internal class IgnoreRecordTemplate<T> : RecordTemplate<T> where T : Payment
    {
        private readonly IList<String> _ignoreString;
        public static IgnoreRecordTemplate<T> Instance { get; } = new IgnoreRecordTemplate<T>();

        private IgnoreRecordTemplate()
        {
            _ignoreString = new List<string>
            {
                "обработка \"Диалог\" - \"Авторизация\": успешно",
                "обработка \"SBRF\" - \"Авторизация\": успешно",
                "обработка \"Диалог\" - \"Расчет заказа\": успешно",
            };
        }

        #region Overrides of RecordTemplate<Payment>

        public override bool IsMatch(T entity, Record record)
        {
            return _ignoreString.Any(x => record.Entry[1].StartsWith(x));
        }

        public override ProcessingResult Process(T entity, Record record)
        {
            return ProcessingResult.DoesntMetter;
        }

        #endregion
    }
}
