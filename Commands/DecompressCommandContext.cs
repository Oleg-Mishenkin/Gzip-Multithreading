using System.Collections.Generic;

namespace GzipAssessment.Commands
{
    public class DecompressCommandContext : BaseCommandContext
    {
        public DecompressCommandContext(int blockSize, int threadCount, string readFile, string writeFile) : base(blockSize, threadCount, readFile, writeFile)
        {
        }

        public override void Proceed()
        {
            StartProducer();

            WorkDoneEvent.WaitOne();
            BlockQueue.Close();
        }

        private void StartProducer()
        {
            var fs = ReadFile.Stream;
            var gzipHeaderMatches = 0;
            List<byte> currentDataBlock = new List<byte>();
            int blockNumber = 0;

            byte[] initBuffer = new byte[Constants.GZipDefaultHeader.Length + 1];
            int currentStreamPosition = fs.Read(initBuffer, 0, Constants.GZipDefaultHeader.Length + 1); // read first file gzip header plus additional data byte

            currentDataBlock.AddRange(initBuffer);

            while (currentStreamPosition < ReadFile.FileLength)
            {
                var currentByte = fs.ReadByte();
                currentDataBlock.Add((byte) currentByte);

                if (currentStreamPosition == ReadFile.FileLength - 1) // last block of data
                {
                    BlockQueue.Enqueue(new ProcessingBlock(blockNumber, currentDataBlock.ToArray(), true));
                    break;
                }

                if (currentByte == Constants.GZipDefaultHeader[gzipHeaderMatches])
                {
                    if (gzipHeaderMatches == Constants.GZipDefaultHeader.Length - 1)
                    {
                        var nextBlockHeader = currentDataBlock.GetRange(
                            currentDataBlock.Count - Constants.GZipDefaultHeader.Length,
                            Constants.GZipDefaultHeader.Length);
                        currentDataBlock.RemoveRange(currentDataBlock.Count - Constants.GZipDefaultHeader.Length,
                            Constants.GZipDefaultHeader.Length); // we've found the beginning of the next block
                        BlockQueue.Enqueue(new ProcessingBlock(blockNumber, currentDataBlock.ToArray(), false));

                        currentDataBlock = nextBlockHeader;
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
