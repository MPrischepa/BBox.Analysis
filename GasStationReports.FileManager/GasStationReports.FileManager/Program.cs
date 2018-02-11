using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GasStationReports.FileManager.Configuration;

namespace GasStationReports.FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var __config = ConfigurationManager.GetSection("fileManagerConfiguration") as FileManagerConfiguration;
            Debug.Assert(__config != null, "__config != null");
            if (!Directory.Exists(__config.OutputDirectory.Value))
                Directory.CreateDirectory(__config.OutputDirectory.Value);
            IList<TextTemplate> __templates =
                (from ReplacementTemplateElement __configTemplate in __config.ReplacementTextTemplates
                        select __configTemplate)
                    .OrderBy(x => x.Index.Ind)
                    .Select(x => new TextTemplate(x)).ToList();
            foreach (InputDirectoryElement __inputDirectoryElement in __config.InputDirectories)
            {
                var __fileDirectory = Path.Combine(__config.InputDirectories.BaseDirectory,
                    __inputDirectoryElement.Directory);
                var __files = Directory.GetFiles(__fileDirectory, __config.ArchiveFilePattern.Value);
                foreach (var __file in __files)
                {
                    Console.WriteLine(__file);
                    foreach (var __fileContent in new ArchiveEnumerable(__file))
                    {
                        var __outPut = __templates.Aggregate(__fileContent.Content,
                            (current, textTemplate) => textTemplate.Replace(current));
                        var __fileName = __fileContent.FileName;
                        __fileName = Regex.Replace(__fileName,
                            "(\\d{2}_\\d{2}_\\d{4}\\s\\d{1,2}_\\d{2}_\\d{2})\\s(-)\\s(\\d{2}_\\d{2}_\\d{4}\\s\\d{1,2}_\\d{2}_\\d{2})", "$1 _ $3");
                        __fileName = Regex.Replace(__fileName,
                            "(\\d{2})_(\\d{2})_(\\d{4})\\s(\\d{1,2})_(\\d{2})_(\\d{2})", "$3_$2_$1 $4_$5_$6");
                        __fileName = Regex.Replace(__fileName,
                           "(\\d{4})_(\\d{2}).(\\d{2})\\s(\\d{1})_(\\d{2})_(\\d{2})", "$1_$2_$3 0$4_$5_$6");
                        using (var __fileStream = new StreamWriter(Path.Combine(__config.OutputDirectory.Value, __fileName),false, Encoding.GetEncoding(1251)))
                        {
                            __fileStream.Write(__outPut);
                            __fileStream.Close();
                        }
                    }
                    File.Delete(__file);
                }
            }

            foreach (RemoveFile __configRemoveFile in __config.RemoveFiles)
            {
                var __removeFiles = Directory.GetFiles(__config.OutputDirectory.Value, __configRemoveFile.FillePattern);
                foreach (var __removeFile in __removeFiles)
                {
                    File.Delete(__removeFile);
                }
            }
            
        }
    }
}
