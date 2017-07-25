using System;
using System.Text.RegularExpressions;
using BBox.Analysis.Domain.PaymentTypes;

namespace BBox.Analysis.Domain.RecordTemplates.PaymentTemplates
{
    internal class SetCardTemplate<T>: RecordTemplate<T> where T : Payment//, IContainsClientCard
    {
        
        public static SetCardTemplate<T> Instance { get; } = new SetCardTemplate<T>();

        private SetCardTemplate()
        {

        }
        #region Overrides of RecordTemplate<Payment>

        public override bool IsMatch(T entity, Record record)
        {
            return new Regex("Предъявлена карта/идентификатор № \\d{20}").IsMatch(record.Entry[1]);
        }

        public override bool Process(T entity, Record record)
        {
            var __payment = entity;
            if (__payment == null)
                throw  new ArgumentException($"Тип оплаты {entity.PaymentTypeName} должен содержать клиентскую карту");

            var __cardNo = new Regex("\\d{20}").Match(record.Entry[1]).Value;
            __payment.SetCardNo(__cardNo.TrimStart('0'));
            return true;
        }

        #endregion
    }
}
