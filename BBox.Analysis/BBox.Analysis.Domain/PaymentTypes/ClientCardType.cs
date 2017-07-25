using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain.PaymentTypes
{
    public enum ClientCardType
    {
        None,
        /// <summary>
        /// счет (юрлица)
        /// </summary>
        AccountCard,

        /// <summary>
        /// скидачная карта
        /// </summary>
        DiscountCard,

        /// <summary>
        /// Бонусная карта
        /// </summary>
        BonusCard

    }
}
