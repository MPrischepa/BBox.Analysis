namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class PaymentAddedTemplate: RecordTemplate<FuelSale>
    {
        public static PaymentAddedTemplate Instance { get; } = new PaymentAddedTemplate();
        private PaymentAddedTemplate()
        {

        }
        #region Overrides of RecordTemplate<FuelSale>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            return entity.Payment != null && BlackBoxObject.IsMatch(entity.Payment, record);
        }

        public override bool Process(FuelSale entity, Record record)
        {
            return BlackBoxObject.ProcessRecord(entity.Payment, record);
        }

        #endregion
    }
}
