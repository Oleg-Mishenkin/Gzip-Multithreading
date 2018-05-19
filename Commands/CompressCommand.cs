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
            for (var threadCurrentBlock = _context.SetNextCurrentBlock(); threadCurrentBlock * _context.BlockSize <= _context.ReadFile.FileLength; threadCurrentBlock = _context.SetNextCurrentBlock())
            {
                byte[] data = _context.ReadFile.ReadBytes(threadCurrentBlock * _context.BlockSize, _context.BlockSize);
                var compressedBytes = GZipHelper.Compress(data);

                _context.GetCurrentThreadEvent(threadCurrentBlock).WaitOne();
                _context.WriteFile.WriteToFile(compressedBytes);

                _context.GetNextThreadEvent(threadCurrentBlock).Set();
                CheckWorkDone(threadCurrentBlock);
            }
        }

        private void CheckWorkDone(int threadCurrentBlock)
        {
            // For the last block we should notify Main thread about completion. We can use Barrier class here for that, but it's in .NET 4.0
            if ((threadCurrentBlock + 1) * _context.BlockSize > _context.ReadFile.FileLength)
                _context.OnWorkDone();
        }
    }
}
