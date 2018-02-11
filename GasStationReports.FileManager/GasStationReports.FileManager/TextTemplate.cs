using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GasStationReports.FileManager.Configuration;

namespace GasStationReports.FileManager
{
    internal class TextTemplate
    {
        private readonly String _searchPattern;
        private readonly String _replacePattern;

        public TextTemplate(ReplacementTemplateElement template)
        {
            _searchPattern = template.SearchPattern.Value;
            _replacePattern = template.ReplacementPattern.Value;
        }

        public String Replace(String input)
        {
            return Regex.Replace(input, _searchPattern, _replacePattern);
        }
    }
}
