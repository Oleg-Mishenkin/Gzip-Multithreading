using System;
using System.IO;

namespace GzipAssessment
{
    public class FileToWrite : IDisposable
    {
        private FileStream _fileStream;

        private static object _fileLock = new object();

        public FileToWrite(string path)
        {
            _fileStream = File.OpenWrite(path);
        }

        public void WriteToFile(byte[] data)
        {
            // If the hard drive is slow it's better to have some cache and write on disk less often, when cache is full.
            // For simplicity I write to file itself, without a cache. But we should keep it in mind
            lock (_fileLock)
            {
                _fileStream.Write(data, 0, data.Length);
            }
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
        }
    }
}
