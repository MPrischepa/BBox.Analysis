using System;

namespace BBox.Analysis.Domain
{
    /// <summary>
    /// Запись
    /// </summary>
    public class Record
    {
        public String FuelStationName { get; set; }

        public DateTime TimeRecord { get; set; }

        public Int64 ID { get; set; }

        public String[] Entry { get; set; }
    }
}