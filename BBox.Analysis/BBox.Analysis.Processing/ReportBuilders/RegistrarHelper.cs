using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Domain;
using NPOI.SS.UserModel;

namespace BBox.Analysis.Processing
{
    static class RegistrarHelper
    {
        public static IRow GetRow(ISheet sheet, int rownum)
        {
            return sheet.GetRow(rownum) ?? sheet.CreateRow(rownum);
        }

        public static ICell GetCell(IRow row, int cellnum)
        {
            return row.GetCell(cellnum) ?? row.CreateCell(cellnum);
        }

        public static String GetSaleState(FuelSaleState state)
        {
            switch (state)
            {
                case FuelSaleState.Active:
                    return "Создана";
                case FuelSaleState.Canceled:
                    return "Отменена";
                case FuelSaleState.Approved:
                    return "Проведена";
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public static void FillOrderData(IRow row, int startCellNum, Order order, bool includeDoseStr)
        {
            var __ind = startCellNum;
            ICell __cell;
            if (order.Payment != null)
            {
                __cell = RegistrarHelper.GetCell(row, __ind);
                __cell.SetCellValue(order.Payment.PaymentTypeName);
            }
            else
            {
                __cell = RegistrarHelper.GetCell(row, __ind);
                __cell.SetCellValue("Не определено");
            }
            __ind++;
            if (order.FuelColumn != null)
            {
                __cell = RegistrarHelper.GetCell(row, __ind);
                __cell.SetCellValue(order.FuelColumn.ID);
            }
            __ind++;
            __cell = RegistrarHelper.GetCell(row, __ind);
            __cell.SetCellValue(order.ProductName);
            __ind++;
            __cell = RegistrarHelper.GetCell(row, __ind);
            __cell.SetCellValue(Convert.ToDouble(order.Price));
            __ind++;
            if (includeDoseStr)
            {
                __cell = RegistrarHelper.GetCell(row, __ind);
                __cell.SetCellValue(order.DoseString);
                __ind++;
            }
            __cell = RegistrarHelper.GetCell(row, __ind);
            __cell.SetCellValue(Convert.ToDouble(order.Volume));
            __ind++;
            __cell = RegistrarHelper.GetCell(row, __ind);
            __cell.SetCellValue(Convert.ToDouble(order.Amount));
        }
    }
}
