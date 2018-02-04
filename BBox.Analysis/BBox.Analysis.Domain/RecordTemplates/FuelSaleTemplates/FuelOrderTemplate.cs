using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBox.Analysis.Core;

namespace BBox.Analysis.Domain.RecordTemplates.FuelSaleTemplates
{
    internal abstract class FuelOrderTemplate : RecordTemplate<FuelSale>
    {
        protected readonly IList<String> _addedString = new List<string>();

        public sealed override bool IsMatch(FuelSale entity, Record record)
        {
            return _addedString.Any(x => record.Entry[1].StartsWith(x));
        }

        private String GetValueStr(string template)
        {
            var __spl = template.Split(':');
            return __spl[1];
        }

        protected virtual void BeforeProcessing(FuelSale entity) { }

        protected virtual void AfterProcessed(FuelSale entity)
        {
            
        } 
        public sealed override ProcessingResult Process(FuelSale entity, Record record)
        {
            BeforeProcessing(entity);
            var __order = entity.GetCurrentOrder();
            var __trkNo = Int16.Parse(GetValueStr(record.Entry[1]));
            var __column = entity.FuelStation.GetFuelColumn(__trkNo);
            //if (entity.FuelColumn == null)
                entity.SetFuelColumn(__column);
            //else if (!entity.FuelColumn.Equals(__column))
            //{
            //    entity.FuelColumn.RemoveSale(entity);
            //    entity.SetFuelColumn(__column);
            //}

            __order.ProductName = GetValueStr(record.Entry[2]);
            var __numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "," };
            __order.Price = Decimal.Parse(GetValueStr(record.Entry[3]),__numberFormatInfo);
            __order.DoseString = GetValueStr(record.Entry[4].Substring(0, record.Entry[4].Length-1));
            AfterProcessed(entity);
            return ProcessingResult.SelfProcessing;
        }
    }

    internal class FuelOrderAddedTemplate : FuelOrderTemplate
    {
        public static FuelOrderAddedTemplate Instance { get; } = new FuelOrderAddedTemplate();
        private FuelOrderAddedTemplate()
        {
            _addedString.Add("Добавлен топливный заказ");
            _addedString.Add("Доза рассчитана");
        }

        #region Overrides of FuelOrderTemplate

        protected override void BeforeProcessing(FuelSale entity)
        {
            base.BeforeProcessing(entity);
            entity.PourState = FuelPourState.PreOrder;
        }

        #endregion
    }

    internal class FuelOrderToFuelStationTemplate : FuelOrderTemplate
    {
        public static FuelOrderToFuelStationTemplate Instance { get; } = new FuelOrderToFuelStationTemplate();
        private FuelOrderToFuelStationTemplate()
        {
            _addedString.Add("Доза установлена");
            _addedString.Add("Доза восстановлена");
        }

        #region Overrides of FuelOrderTemplate

        protected override void BeforeProcessing(FuelSale entity)
        {
            base.BeforeProcessing(entity);
            if (entity.PourState <= FuelPourState.PreOrder)
                entity.PourState = FuelPourState.PreOrder;
        }

        #endregion
    }

    internal class FuelOrderPourFixedTemplate : RecordTemplate<FuelSale>
    {
        protected readonly IList<String> _addedString = new List<string>();
        public static FuelOrderPourFixedTemplate Instance { get; } = new FuelOrderPourFixedTemplate();
        private FuelOrderPourFixedTemplate()
        {
            _addedString.Add("Налив зафиксирован");
        }
        private String GetValueStr(string template)
        {
            var __spl = template.Split(':');
            return __spl[1];
        }
        public sealed override bool IsMatch(FuelSale entity, Record record)
        {
            return _addedString.Any(x => record.Entry[1].StartsWith(x));
        }

        public sealed override ProcessingResult Process(FuelSale entity, Record record)
        {
            var __order = entity.GetCurrentOrder();
            var __trkNo = Int16.Parse(GetValueStr(record.Entry[1]));
            var __column = entity.FuelStation.GetFuelColumn(__trkNo);
            if (__order == null && entity.PourState == FuelPourState.NewOrder)
            {
                __column.AddFuelOrder(entity);
                __column.Finished();
                __order = entity.GetCurrentOrder();
            }
            //if (entity.FuelColumn == null)
            entity.SetFuelColumn(__column);
            //else if (!entity.FuelColumn.Equals(__column))
            //{
            //    entity.FuelColumn.RemoveSale(entity);
            //    entity.SetFuelColumn(__column);
            //}

            __order.ProductName = GetValueStr(record.Entry[2]);
            var __numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "," };
            __order.Price = Decimal.Parse(GetValueStr(record.Entry[3]),__numberFormatInfo);
            __order.Volume = Decimal.Parse(new Regex("\\d*,\\d{2}").Match(record.Entry[4]).Value,__numberFormatInfo);
            __order.Amount = Decimal.Parse(new Regex("\\d*,\\d{2}").Match(record.Entry[5]).Value,__numberFormatInfo);
            entity.PourState = FuelPourState.OrderFinished;
            return ProcessingResult.SelfProcessing;
        }
    }
}
