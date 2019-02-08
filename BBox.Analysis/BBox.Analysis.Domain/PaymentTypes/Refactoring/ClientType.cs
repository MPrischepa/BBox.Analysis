namespace BBox.Analysis.Domain.PaymentTypes
{
    public enum ClientType
    {
        /// <summary>
        /// юрлицо
        /// </summary>
        LegalEntity = 0,
        /// <summary>
        /// Физлицо
        /// </summary>
        PhysicalPerson = 1,
        /// <summary>
        /// служебный
        /// </summary>
        Service = 2,
        /// <summary>
        /// Прочие
        /// </summary>
        Other = 3
    }
}
