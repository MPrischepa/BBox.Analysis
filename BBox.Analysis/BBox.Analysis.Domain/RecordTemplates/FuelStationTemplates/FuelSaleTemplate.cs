using System;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelStationTemplates
{
    internal class FuelSaleTemplate : RecordTemplate<FuelStation>
    {
        public static FuelSaleTemplate Instance { get; } = new FuelSaleTemplate();

        private FuelSaleTemplate()
        {

        }
        #region Overrides of RecordTemplate<FuelStation>

        public override bool IsMatch(FuelStation entity, Record record)
        {
            return record.Entry[0].StartsWith("Тр:");
        }

        private Int64 GetSaleNo(String str)
        {
            var __spl = str.Split(':');
            return Int64.Parse(__spl[1]);
        }

        public override ProcessingResult Process(FuelStation entity, Record record)
        {
            if (entity.CurrentShift == null) return ProcessingResult.DontProcessing;
            var __saleNo = GetSaleNo(record.Entry[0]);
            var __sale = entity.CurrentShift.GetFuelSale(__saleNo, record);
            BlackBoxObject.ProcessRecord(__sale,record);
            return ProcessingResult.PostProcessing;
        }

        #endregion
    }
}
