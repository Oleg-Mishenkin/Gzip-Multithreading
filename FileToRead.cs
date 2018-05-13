using System;
using System.IO;

namespace GzipAssessment
{
    public class FileToRead : IDisposable
    {
        private FileStream _fileStream;
        private readonly long _fileLength;

        private static object _fileLock = new object();

        public FileToRead(string path)
        {
            _fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _fileLength = _fileStream.Length;
        }

        public byte[] ReadBytes(long offset, int blockSize)
        {
            var readSize = Math.Min(blockSize, _fileLength - offset);
            byte[] data = new byte[readSize];
            lock (_fileLock)
            {
                _fileStream.Seek(offset, SeekOrigin.Begin);
                _fileStream.Read(data, 0, (int)readSize);
            }

            return data;
        }

        public long FileLength
        {
            get { return _fileLength; }
        }

        public Stream GetStream
        {
            get { return _fileStream; }
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
        }
    }
}
