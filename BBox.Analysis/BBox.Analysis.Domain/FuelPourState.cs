using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBox.Analysis.Domain
{
    public enum FuelPourState
    {
        /// <summary>
        /// Новый заказ
        /// </summary>
        NewOrder,
        /// <summary>
        /// Заказ расчитан
        /// </summary>
        PreOrder,
        /// <summary>
        /// Налив начался
        /// </summary>
        PourStart,
        /// <summary>
        /// Налив закончился
        /// </summary>
        PourFinished,
        ///// <summary>
        ///// Заказ отменен
        ///// </summary>
        //OrderCanceled,
        /// <summary>
        /// Заказа зафиксирован
        /// </summary>
        OrderFinished
    }
}
