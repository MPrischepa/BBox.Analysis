using System;

namespace BBox.Analysis.Core
{
    public class ProcessingSettings
    {
        private static ProcessingSettings _settings;

        public static ProcessingSettings Instatnce => _settings ?? (_settings = new ProcessingSettings());

        private ProcessingSettings()
        {
            
        }

        public Boolean BuildShiftReports { get; set; }

        public Boolean BuildSummaryReports { get; set; }

        public Boolean BuildBonusesReports { get; set; }

        public Boolean BuildGapCounterReports { get; set; }

        public Boolean Build1SCompareReports { get; set; }

        public Boolean BuildStatementReports { get; set; }
    }
}
