
namespace GzipAssessment.Commands
{
    public class DecompressCommand : ICommand
    {
        private readonly DecompressCommandContext _context;

        public DecompressCommand(ICommandContext context)
        {
            _context = (DecompressCommandContext)context;
        }

        public void Execute()
        {
            while (_context.BlockQueue.TryDequeue(out var currentProcessedBlock))
            {
                var decompressedBytes = GZipHelper.Decompress(currentProcessedBlock.BlockData);
                _context.GetCurrentThreadEvent(currentProcessedBlock.BlockIndex).WaitOne();
                _context.WriteFile.WriteToFile(decompressedBytes);

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
