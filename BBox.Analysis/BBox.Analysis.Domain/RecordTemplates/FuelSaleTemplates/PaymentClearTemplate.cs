using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal class PaymentClearTemplate : RecordTemplate<FuelSale>
    {
        public static PaymentClearTemplate Instance { get; } = new PaymentClearTemplate();
        private PaymentClearTemplate()
        {

        }

        #region Overrides of RecordTemplate<FuelSale>

        public override bool IsMatch(FuelSale entity, Record record)
        {
            var __pattern = "Основание\\s\"+(\\w*\\s*)*\"\\sсброшено";
            var __reg = new Regex(__pattern);
            return __reg.IsMatch(record.Entry[1]);
        }

        public override bool Process(FuelSale entity, Record record)
        {
            entity.ClearPayment();
            return true;
        }

        #endregion
    }
}
