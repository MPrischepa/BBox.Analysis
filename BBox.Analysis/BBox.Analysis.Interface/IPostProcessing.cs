using System;
using System.Collections.Generic;
using BBox.Analysis.Domain;

namespace BBox.Analysis.Interface
{
    public interface IPostProcessing
    {
        void AfterProcessFinished(IDictionary<String, FuelStation> stations, IDictionary<String, BonusCard> bonusCards,
            IDictionary<string, AccountCard> accountCards, IList<Tuple<String, Int64, String>> invalidRecords);
    }
}
