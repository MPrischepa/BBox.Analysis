using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ionic.Zip;

namespace GasStationReports.FileManager
{
    internal class ArchiveEnumerable : IEnumerable<FileContent>
    {
        private String _fileName;

        public ArchiveEnumerable(String fileName)
        {
            _fileName = fileName;
        }

        #region Implementation of IEnumerable

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<FileContent> GetEnumerator()
        {
           return new ArchiveEnumerator(_fileName);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        class ArchiveEnumerator : IEnumerator<FileContent>
        {
            private ZipFile _zipFile;
            private IEnumerator<ZipEntry> _entryEnum;
            private bool _isDisposed;

            public ArchiveEnumerator(String fileName)
            {
                _zipFile = ZipFile.Read(fileName);
                _entryEnum = _zipFile.GetEnumerator();
            }

            #region Implementation of IDisposable

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _entryEnum?.Dispose();
                    _zipFile?.Dispose();
                    _isDisposed = true;
                }
                _entryEnum = null;
                _zipFile = null;
            }


            #endregion

            #region Implementation of IEnumerator

            /// <summary>Advances the enumerator to the next element of the collection.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                return _entryEnum.MoveNext();
            }

            /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                _entryEnum.Reset();
            }

            /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public FileContent Current
            {
                get
                {
                    var __memoryStream = new MemoryStream();
                    //var __writeStream = File.Create(Path.Combine(outputDir,__entry.FileName));
                    Debug.Assert(_entryEnum.Current != null, "_entryEnum.Current != null");
                    _entryEnum.Current.Extract(__memoryStream);
                    var __stream = new StreamReader(__memoryStream, Encoding.GetEncoding(1251));
                    __stream.BaseStream.Position = 0;
                    return new FileContent
                    {
                        FileName = _entryEnum.Current.FileName,
                        Content = __stream.ReadToEnd(),
                    };
                }
            }

            /// <summary>Gets the current element in the collection.</summary>
            /// <returns>The current element in the collection.</returns>
            object IEnumerator.Current => Current;

            #endregion
        }
    }

    internal class FileContent
    {
        public String FileName { get; set; }

        public String Content { get; set; }
    }
}
