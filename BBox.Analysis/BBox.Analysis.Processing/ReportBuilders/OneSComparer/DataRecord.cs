using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBox.Analysis.Processing.OneSComparer
{
    public class DataRecord
    {
        public string FuelStationID { get; set; }

        public string FuelStationName { get; set; }

        public DateTime ShiftBeginDate { get; set; }

        public DateTime ShiftEndDate { get; set; }

        public string Operator { get; set; }

        public String Product { get; set; }

        public Int16 FuelColumnID { get; set; }

        public Int16 FuelHoseID { get; set; }

        public Decimal Volume { get; set; }

        public Decimal Amount { get; set; }

    }
}
