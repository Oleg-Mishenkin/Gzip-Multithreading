using System;
using System.Collections.Generic;

namespace GzipAssessment.Commands
{
    public class DecompressCommandContext : BaseCommandContext
    {
        public DecompressCommandContext(int blockSize, int threadCount, string readFile, string writeFile) : base(blockSize, threadCount, readFile, writeFile)
        {
        }

        public override void StartProducer()
        {
            var fs = ReadFile.Stream;
            var gzipHeaderMatches = 0;
            List<byte> currentDataBlockData = new List<byte>();
            int blockNumber = 0;

            byte[] initBuffer = new byte[Constants.GZipDefaultHeader.Length + 1];
            int currentStreamPosition = fs.Read(initBuffer, 0, Constants.GZipDefaultHeader.Length + 1); // read first file gzip header plus additional data byte

            currentDataBlockData.AddRange(initBuffer);

            while (currentStreamPosition < ReadFile.FileLength)
            {
                var currentByte = fs.ReadByte();
                currentDataBlockData.Add((byte) currentByte);

                if (currentStreamPosition == ReadFile.FileLength - 1) // last block of data
                {
                    BlockQueue.Enqueue(new ProcessingBlock(blockNumber, currentDataBlockData.ToArray(), true));
                    OnProgressChanged(new ProgressChangedEventArgs(100));
                    break;
                }

                if (currentByte == Constants.GZipDefaultHeader[gzipHeaderMatches])
                {
                    if (gzipHeaderMatches == Constants.GZipDefaultHeader.Length - 1)
                    {
                        var nextBlockHeader = currentDataBlockData.GetRange(
                            currentDataBlockData.Count - Constants.GZipDefaultHeader.Length,
                            Constants.GZipDefaultHeader.Length);
                        currentDataBlockData.RemoveRange(currentDataBlockData.Count - Constants.GZipDefaultHeader.Length,
                            Constants.GZipDefaultHeader.Length); // we've found the beginning of the next block
                        BlockQueue.Enqueue(new ProcessingBlock(blockNumber, currentDataBlockData.ToArray(), false));

                        if (currentStreamPosition % 100 == 0)
                            OnProgressChanged(new ProgressChangedEventArgs((int)(currentStreamPosition * 100.0 / ReadFile.FileLength)));

                        currentDataBlockData = nextBlockHeader;
                        blockNumber++;
                        gzipHeaderMatches = 0;
                    }
                    else
                    {
                        gzipHeaderMatches++;
                    }
                }
                else
                {
                    gzipHeaderMatches = 0;
                }

                currentStreamPosition++;
            }
        }
    }
}
