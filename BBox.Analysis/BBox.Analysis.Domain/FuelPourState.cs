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
        NewOrder = 0,
        /// <summary>
        /// Заказ расчитан
        /// </summary>
        PreOrder = 1,
        /// <summary>
        /// Налив начался
        /// </summary>
        PourStart = 2,
        /// <summary>
        /// Налив закончился
        /// </summary>
        PourFinished = 3,
        ///// <summary>
        ///// Заказ отменен
        ///// </summary>
        //OrderCanceled,
        /// <summary>
        /// Заказа зафиксирован
        /// </summary>
        OrderFinished = 4
    }
}
