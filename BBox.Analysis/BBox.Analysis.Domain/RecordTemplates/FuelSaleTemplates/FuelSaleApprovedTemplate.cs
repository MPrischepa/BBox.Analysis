using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class FuelSaleApprovedTemplate : RecordTemplate<FuelSale>
    {
        public static FuelSaleApprovedTemplate Instance { get; } = new FuelSaleApprovedTemplate();

        private FuelSaleApprovedTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelSale>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            return record.Entry[1].Equals("Запись результатов транзакции");
        }

        public override bool Process(FuelSale entity, Record record)
        {
            if (entity.SaleState != FuelSaleState.Canceled)
            {
                entity.SaleState = FuelSaleState.Approved;
                var __order = entity.GetCurrentOrder();
            }
            return true;
        }

        #endregion
    }
}
