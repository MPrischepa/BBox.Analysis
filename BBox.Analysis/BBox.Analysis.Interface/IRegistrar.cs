using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBox.Analysis.Domain;

namespace BBox.Analysis.Interface
{
    public interface IRegistrar: IBonusCardFactory,IAccountCardFactory
    {
        //Task RegisterShift(Shift shift);

        FuelStation GetFuelStation(String stationName);

        void AfterProcessDataFinished();

        //void RegisterSummaryReport();

        //void RegisterBonusReport();

        //void RegisterSummaryBonusReport();

        bool IsProcessedRecord(FuelStation station, Record record);

        void UnProcessedRecord(FuelStation station, Record record);

        void SetInvalidInfo(String fileName, Int64 position, String reason);

        //void RegisterInvalidRecordReport();

        //void RegisterGapCounterReport();

        //void RegisterOneSCompareReport();

        //void RegisterStatementReport();

        //void RegisterAccountCardReport();
    }
}
