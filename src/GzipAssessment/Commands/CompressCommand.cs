namespace GzipAssessment.Commands
{
    public class CompressCommand : ICommand
    {
        private readonly CompressCommandContext _context;

        public CompressCommand(ICommandContext context)
        {
            _context = (CompressCommandContext)context;
        }

        public void Execute()
        {
            /*for (var threadCurrentBlock = _context.SetNextCurrentBlock(); threadCurrentBlock * _context.BlockSize <= _context.ReadFile.FileLength; threadCurrentBlock = _context.SetNextCurrentBlock())
            {
                byte[] data = _context.ReadFile.ReadBytes(threadCurrentBlock * _context.BlockSize, _context.BlockSize);
                var compressedBytes = GZipHelper.Compress(data);

                _context.GetCurrentThreadEvent(threadCurrentBlock).WaitOne();
                _context.WriteFile.WriteToFile(compressedBytes);

                _context.GetNextThreadEvent(threadCurrentBlock).Set();
                CheckWorkDone(threadCurrentBlock);
            }*/

            while (_context.BlockQueue.TryDequeue(out var currentProcessedBlock))
            {
                var compressedBytes = GZipHelper.Compress(currentProcessedBlock.BlockData);

                _context.GetCurrentThreadEvent(currentProcessedBlock.BlockIndex).WaitOne();
                _context.WriteFile.WriteToFile(compressedBytes);

                _context.GetNextThreadEvent(currentProcessedBlock.BlockIndex).Set();
                CheckWorkDone(currentProcessedBlock);
            }
        }

        private void CheckWorkDone(ProcessingBlock dataBlock)
        {
            if (dataBlock.IsLastBlock)
                _context.OnWorkDone();
        }
    }
}
