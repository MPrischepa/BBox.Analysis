using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class IgnoreRecordTemplate: RecordTemplate<FuelStation>
    {
        private readonly IList<String> _ignoreString;
        public static IgnoreRecordTemplate Instance { get; } = new IgnoreRecordTemplate();
        private IgnoreRecordTemplate()
        {
            _ignoreString = new List<string>
            {
                "Начало автоматической передачи данных",
                "Передача данных в локальный каталог",
                "Окончание автоматической передачи данных",
                "Начало автоматического приема данных",
                "Не дождались окончания приема/передачи SBDelivery",
                "Окончание автоматического приема данных",
                "Начато редактирование данных смены",
                "Ввод пароля с правами:",
                "Принятие смены",
                "Рабочее место открыто",
                "Редактирование данных смены завершено",
                "Закрытие окна смены",
                "Вход в окно управления"
            };
        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return _ignoreString.Any(x => record.Entry[0].StartsWith(x));
        }

        public override ProcessingResult Process(FuelStation entity, Record record)
        {
            return ProcessingResult.DoesntMetter;
        }

        #endregion
    }
}
