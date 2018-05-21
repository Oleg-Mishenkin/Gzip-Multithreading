namespace GzipAssessment.Commands
{
    public class CompressCommandContext : BaseCommandContext
    {
        public CompressCommandContext(int blockSize, int threadCount, CommandLineArguments args) : base(blockSize, threadCount, args)
        {
        }

        public override void StartProducer()
        {
            int currentBlockIndex = 0;

            while (currentBlockIndex * BlockSize <= ReadFile.FileLength)
            {
                byte[] data = ReadFile.ReadBytes(currentBlockIndex * BlockSize, BlockSize);
                var isLastBlock = (currentBlockIndex + 1) * BlockSize > ReadFile.FileLength;

                BlockQueue.Enqueue(new ProcessingBlock(currentBlockIndex, data, isLastBlock));

                OnProgressChanged(new ProgressChangedEventArgs((int)(currentBlockIndex * BlockSize * 100.0 / ReadFile.FileLength)));
                currentBlockIndex++;
            }

            OnProgressChanged(new ProgressChangedEventArgs(100));
        }
    }
}
