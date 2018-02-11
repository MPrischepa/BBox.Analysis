using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace GasStationReports.FileManager
{
    public static class ArchiveExtensions
    {
        public static void Read(String fileName, string outputDir)
        {
            using (var __archive = ZipFile.Read(fileName))
            {
                foreach (var __entry in __archive.Entries)
                {
                    var __stream = new MemoryStream();
                    //var __writeStream = File.Create(Path.Combine(outputDir,__entry.FileName));
                    __entry.Extract(__stream);
                    var __str = new StreamReader(__stream, Encoding.GetEncoding(1251));
                    __str.BaseStream.Position = 0;
                    var __text = __str.ReadToEnd();
                    Console.Write(__text);
                   
                   
                }
            }

        }
    }
}
