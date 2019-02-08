using System;
using System.Collections.Generic;
using BBox.Analysis.Domain;
using BBox.Analysis.Interface;

namespace BBox.Analysis.Processing
{
    public class Registrar : IRegistrar
    {
       
        private readonly IDictionary<String, FuelStation> _stations;
        private readonly IDictionary<String, BonusCard> _bonusCards;
        private readonly IDictionary<String, AccountCard> _accountCards;
        private readonly ISet<Tuple<FuelStation, DateTime, Int64>> _processedRecord;
        private readonly IList<Tuple<String, Int64, String>> _invalidRecords;
        private readonly IPostProcessing _postProcessor;


        public Registrar(IPostProcessing postProcessor)
        {
            _stations = new Dictionary<string, FuelStation>();
            _bonusCards = new Dictionary<string, BonusCard>();
            _accountCards = new Dictionary<string, AccountCard>();
            _processedRecord = new HashSet<Tuple<FuelStation, DateTime, long>>();
            _invalidRecords = new List<Tuple<string, long, string>>();
            _postProcessor = postProcessor;
        }

        #region Implementation of IRegistrar

        

        public FuelStation GetFuelStation(String stationName)
        {
            if (_stations.TryGetValue(stationName, out var __fuelStation))
                return __fuelStation;
            __fuelStation = new FuelStation {Name = stationName};
            _stations.Add(stationName, __fuelStation);
            return __fuelStation;
        }

        public void AfterProcessDataFinished()
        {
            _postProcessor.AfterProcessFinished(_stations,_bonusCards,_accountCards,_invalidRecords);
        }
        
        public bool IsProcessedRecord(FuelStation station, Record record)
        {
            var __result =
                _processedRecord.Contains(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            if (!__result)
            {
                _processedRecord.Add(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            }
            return __result;
        }

        public void UnProcessedRecord(FuelStation station, Record record)
        {
            var __result =
                _processedRecord.Contains(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            if (__result)
            {
                _processedRecord.Remove(new Tuple<FuelStation, DateTime, long>(station, record.TimeRecord, record.ID));
            }
        }

        public void SetInvalidInfo(string fileName, long position, string reason)
        {
            _invalidRecords.Add(new Tuple<string, long, string>(fileName,position,reason));
        }

        public BonusCard GetBonusCard(String cardNo)
        {
            if (_bonusCards.TryGetValue(cardNo, out var __card))
                return __card;
            __card = new BonusCard(cardNo);
            _bonusCards.Add(cardNo, __card);
            return __card;
        }

        public AccountCard GetAccountCard(String cardNo)
        {
            if (_accountCards.TryGetValue(cardNo, out var __card))
                return __card;
            __card = new AccountCard(cardNo);
            _accountCards.Add(cardNo, __card);
            return __card;
        }
        #endregion
    }
}
