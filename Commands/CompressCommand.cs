using System.IO;
using System.IO.Compression;

namespace GzipAssessment.Commands
{
    public class CompressCommand : ICommand
    {
        private readonly CompressCommandContext _context;
        private readonly FileToRead _readFile;
        private readonly FileToWrite _writeFile;

        public CompressCommand(ICommandContext context, string readFile, string writeFile)
        {
            _context = (CompressCommandContext)context;
            _readFile = new FileToRead(readFile);
            _writeFile = new FileToWrite(writeFile);
        }

        public void Execute()
        {
            for (var threadCurrentBlock = _context.SetNextCurrentBlock(); threadCurrentBlock * _context.BlockSize <= _readFile.FileLength; threadCurrentBlock = _context.SetNextCurrentBlock())
            {
                using (var compressedMemoryStream = new MemoryStream())
                {
                    byte[] data = _readFile.ReadBytes(threadCurrentBlock * _context.BlockSize, _context.BlockSize);

                    using (GZipStream compressedStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress))
                    {
                        compressedStream.Write(data, 0, data.Length);
                    }

                    var compressedBytes = compressedMemoryStream.ToArray();
                    _context.GetCurrentThreadEvent(threadCurrentBlock).WaitOne();
                    _writeFile.WriteToFile(compressedBytes);
                }

                _context.GetNextThreadEvent(threadCurrentBlock).Set();
                CheckWorkDone(threadCurrentBlock);
            }
        }

        private void CheckWorkDone(int threadCurrentBlock)
        {
            // For the last block we should notify Main thread about completion. We can use Barrier class here for that, but it's in .NET 4.0
            if ((threadCurrentBlock + 1) * _context.BlockSize > _readFile.FileLength)
                _context.OnWorkDone();
        }

        public void Dispose()
        {
            _readFile.Dispose();
            _writeFile.Dispose();
        }
    }
}
