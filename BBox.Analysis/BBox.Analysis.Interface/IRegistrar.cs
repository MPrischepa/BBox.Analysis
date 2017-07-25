using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Domain;

namespace BBox.Analysis.Interface
{
    public interface IRegistrar: IBonusCardFactory
    {
        Task RegisterShift(Shift shift);

        FuelStation GetFuelStation(String stationName);

        void RegisterSummaryReport();

        void RegisterBonusReport();

        void RegisterSummaryBonusReport();

        bool IsProcessedRecord(FuelStation station, Record record);
    }
}
