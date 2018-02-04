using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class FuelOrderMoveTemplate : RecordTemplate<FuelSale>
    {
        public static FuelOrderMoveTemplate Instance { get; } = new FuelOrderMoveTemplate();

        private FuelOrderMoveTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelSale>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            return record.Entry[1].StartsWith("Топливный заказ перемещен");
        }

        public override ProcessingResult Process(FuelSale entity, Record record)
        {
            var __pattern = "ТРК:\\s*\\d+";
            var __reg = new Regex(__pattern);
            var __matches = __reg.Matches(record.Entry[1]);
            var __trkCurrent =
                entity.FuelStation.GetFuelColumn(Int16.Parse(__matches[0].Value.Replace("ТРК:", String.Empty)));
            var __trkMove = entity.FuelStation.GetFuelColumn(Int16.Parse(__matches[1].Value.Replace("ТРК:", String.Empty)));
           // if (!entity.FuelColumn.Equals(__trkCurrent))
           //     throw new ArgumentException(
           //         $"Что то пошло не так при перемещении заказа запись: {record.Entry[1]} по факту c ТРК:{entity.FuelColumn.ID} на {__trkMove.ID}");
            entity.MoveFuelOrder(__trkMove);
            return ProcessingResult.SelfProcessing;
        }

        #endregion
    }
}
