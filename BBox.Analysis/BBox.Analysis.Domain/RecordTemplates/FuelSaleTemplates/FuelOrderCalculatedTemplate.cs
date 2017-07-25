using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class FuelOrderCalculatedTemplate : RecordTemplate<FuelSale>
    {
        public static FuelOrderCalculatedTemplate Instance { get; } = new FuelOrderCalculatedTemplate();

        private FuelOrderCalculatedTemplate()
        {

        }

        public override bool IsMatch(FuelSale entity, Record record)
        {
            return record.Entry[1].StartsWith("Заказ расчитан");
        }

        public override bool Process(FuelSale entity, Record record)
        {
            var __order = entity.GetCurrentOrder();
            if (__order == null) return true;
            var __amountStrPattern = "(Сумма заказа: \\d+,\\d{2}р)";
            if (!new Regex(__amountStrPattern).IsMatch(record.Entry[1])) return true;
            __order.Amount = Decimal.Parse(new Regex("\\d+,\\d{2}").Match(record.Entry[1]).Value);
            return true;
        }
    }
}
