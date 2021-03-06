﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class FuelSaleCanceledTemplate : RecordTemplate<FuelSale>
    {
        public static FuelSaleCanceledTemplate Instance { get; } = new FuelSaleCanceledTemplate();

        private FuelSaleCanceledTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelSale>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            return record.Entry[1].Equals("Отмена транзакции");
        }

        public override ProcessingResult Process(FuelSale entity, Record record)
        {
            entity.SaleState = FuelSaleState.Canceled;
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
