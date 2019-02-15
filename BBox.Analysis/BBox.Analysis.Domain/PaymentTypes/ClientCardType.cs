namespace BBox.Analysis.Domain.PaymentTypes
{
    public enum ClientCardType
    {
        None = 0,
        /// <summary>
        /// счет (юрлица)
        /// </summary>
        AccountCard = 1,

        /// <summary>
        /// скидачная карта
        /// </summary>
        DiscountCard = 2,

        /// <summary>
        /// Бонусная карта
        /// </summary>
        BonusCard = 3

    }
}
