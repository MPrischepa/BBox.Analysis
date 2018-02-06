using System;
using System.Collections.Generic;

namespace BBox.Analysis.Processing.OneSComparer
{
    public class RepositoryKeeper
    {
        private static RepositoryKeeper _keeper;

        public static RepositoryKeeper Instance => _keeper ?? (_keeper = new RepositoryKeeper());

        private readonly IDictionary<String, String> _settings;
        private RepositoryKeeper()
        {
            _settings = new Dictionary<string, string>
            {
                {"     C   ", "АЗС № 1"},
                {"     K   ", "АЗС № 2"},
                {"     E   ", "АЗС № 3"},
                {"     F   ", "АЗС № 4"},
                {"     G   ", "АЗС № 5"},
                {"     L   ", "АЗС № 6"},
                {"     J   ", "АЗС № 7"},
                {"     H   ", "АЗС № 8"},
                {"     I   ", "АЗС № 9"},
                {"     D   ", "АЗС № 10"},
                {"    17   ", "АЗС № 11"},
                {"    1C   ", "АЗС № 12"},
                {"    1L   ", "АЗС № 13"},
                {"    1N   ", "АЗС № 14"},
                {"    1K   ", "АЗС № 15"},
                {"    1H   ", "АЗС № 16"},
                {"    1I   ", "АЗС № 17"},
                {"    1P   ", "АЗС № 18"},
                {"    1Q   ", "АЗС № 19"},
                {"    1S   ", "АЗС № 20"},
                {"    1V   ", "АЗС № 21"},
                {"    1T   ", "АЗС № 22"},
                {"    1U   ", "АЗС № 23"},
                {"    20   ", "АЗС № 24"},
                {"    2F   ", "АЗС № 25"},
                {"    1W   ", "АЗС № 26"},
                {"    1Y   ", "АЗС № 27"},
                {"    2E   ", "АЗС № 28"},
                {"    2G   ", "АЗС № 29"},
                {"    2J   ", "АЗС № 30"},
                {"    2K   ", "АЗС № 31"},
                {"    2L   ", "АЗС № 32"},
                {"    2I   ", "АЗС № 33"},
                {"    2N   ", "АЗС № 34"},
                {"    2O   ", "АЗС № 35"},
                {"    2P   ", "АЗС № 36"},
                {"    2R   ", "АЗС № 37"}
            };
        }

        public String GetFuelStationName(String id)
        {
            return _settings[id];
        }
    }
}
