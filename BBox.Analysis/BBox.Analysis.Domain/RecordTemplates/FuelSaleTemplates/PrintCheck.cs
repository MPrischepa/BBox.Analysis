using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class PrintCheck : RecordTemplate<FuelSale>
    {
        readonly IList<String> _addedString = new List<string>();
        public static PrintCheck Instance { get; } = new PrintCheck();

        private PrintCheck()
        {
            _addedString.Add("Печать чека \"Чек заказа\"");
            _addedString.Add("Печать чека \"Чек по факту\"");
        }

        #region Overrides of RecordTemplate<FuelSale>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            return _addedString.Any(x => record.Entry[1].StartsWith(x));
        }

        public override bool Process(FuelSale entity, Record record)
        {
            entity.SalePrintCheck(Decimal.Parse(new Regex("\\d+,\\d{2}").Match(record.Entry[1]).Value));
            return true;
        }

        #endregion
    }
}
