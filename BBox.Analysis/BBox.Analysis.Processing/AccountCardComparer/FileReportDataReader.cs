﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BBox.Analysis.Processing.AccountCardComparer
{
    public class FileReportDataReader : IDataReader
    {
        private readonly String _filePath;
        public FileReportDataReader(string filePath)
        {
            _filePath = filePath;
        }
        #region Implementation of IEnumerable

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<DataRecord> GetEnumerator()
        {
            return new FileReportDataReaderEnumerator(_filePath);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private sealed class FileReportDataReaderEnumerator : IEnumerator<DataRecord>
        {
            private readonly StreamReader _sr;
            public FileReportDataReaderEnumerator(string filePath)
            {
                _sr = new StreamReader(filePath);
            }

            private DataRecord _current;
            // Implement the IEnumerator(T).Current publicly, but implement 
            // IEnumerator.Current, which is also required, privately.
            public DataRecord Current
            {

                get
                {
                    if (_sr == null || _current == null)
                    {
                        throw new InvalidOperationException();
                    }

                    return _current;
                }
            }

            private object Current1 => Current;

            object IEnumerator.Current => Current1;

            private static DataRecord ParseRecord(String record)
            {
                var __str = record.Split(';');
                var __result = new DataRecord
                {
                    FuelStationID = __str[0],
                    FuelStationName = __str[1].TrimEnd()
                };
                __result.SetShiftBeginDate(
                    () => DateTime.ParseExact(__str[2], "yyyy-MM-dd HH:mm:ss.fff", new CultureInfo("ru-RU")));
                __result.SetShiftEndDate(
                    () => DateTime.ParseExact(__str[3], "yyyy-MM-dd HH:mm:ss.fff", new CultureInfo("ru-RU")));
                __result.SetOperator(() => __str[4].TrimEnd());
                __result.SetSaleDate(
                    () => DateTime.ParseExact(__str[5], "yyyy-MM-dd HH:mm:ss", new CultureInfo("ru-RU")));
                __result.SetProduct(() => __str[6].TrimEnd());
                __result.SetFuelColumnID(() => Int16.Parse(__str[7]));
                __result.SetFuelHoseID(() => Int16.Parse(__str[8]));
                __result.SetVolume(() => Decimal.Parse(__str[9], new NumberFormatInfo {NumberDecimalSeparator = "."}));
                __result.SetCardNo(() => __str[10].TrimEnd());
                __result.SetCustomer(() => __str[11].TrimEnd());
                return __result;
            }

            // Implement MoveNext and Reset, which are required by IEnumerator.
            public bool MoveNext()
            {
                var __line = _sr.ReadLine();
                if (__line == null)
                    return false;
                _current = ParseRecord(__line);
                return _current != null;
            }

            public void Reset()
            {
                _sr.DiscardBufferedData();
                _sr.BaseStream.Seek(0, SeekOrigin.Begin);
                _current = null;
            }

            // Implement IDisposable, which is also implemented by IEnumerator(T).
            private bool _disposedValue;
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        // Dispose of managed resources.
                    }
                    _current = null;
                    if (_sr != null)
                    {
                        _sr.Close();
                        _sr.Dispose();
                    }
                }

                _disposedValue = true;
            }

            ~FileReportDataReaderEnumerator()
            {
                Dispose(false);
            }
        }
    }
}
