namespace GzipAssessment.DataFlow
{
    public class DecompressConsumer : IConsumer
    {
        private readonly DataFlowContext _context;

        public DecompressConsumer(DataFlowContext context)
        {
            _context = context;
        }

        public void StartConsuming()
        {
            ProcessingBlock currentProcessedBlock;
            while (_context.BlockQueue.TryDequeue(out currentProcessedBlock) && !_context.IsExecutionStopped())
            {
                var decompressedBytes = GZipHelper.Decompress(currentProcessedBlock.BlockData);
                _context.ConsumerEventsManager.GetCurrentThreadEvent(currentProcessedBlock.BlockIndex).WaitOne();
                _context.WriteFile.WriteToFile(decompressedBytes);

                _context.ConsumerEventsManager.GetNextThreadEvent(currentProcessedBlock.BlockIndex).Set();
                CheckWorkDone(currentProcessedBlock);
            }
        }

        private void CheckWorkDone(ProcessingBlock dataBlock)
        {
            if (dataBlock.IsLastBlock)
                _context.ConsumerEventsManager.OnWorkDone();
        }
    }
}
