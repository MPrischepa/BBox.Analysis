using System;

namespace BBox.Analysis.Domain.PaymentTypes
{
    [Flags]
    public enum PaymentType
    {
        None = 0,
        /// <summary>
        /// Наличный расчет
        /// </summary>
        CashSettlement = 1,
        /// <summary>
        /// безнал
        /// </summary>
        CashlessSettlement = 2,
        /// <summary>
        /// Бонусы
        /// </summary>
        BonusesSettlement = 4,
        /// <summary>
        /// безоплаты
        /// </summary>
        WithoutPayment = 8,
        /// <summary>
        /// Не определено
        /// </summary>
        NotDefined = 16,
        /// <summary>
        /// Ведамость
        /// </summary>
        Statement = 32,
    }
}
