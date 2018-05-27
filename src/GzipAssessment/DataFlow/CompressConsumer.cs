using System;

namespace GzipAssessment.DataFlow
{
    public class CompressConsumer : IConsumer
    {
        private readonly DataFlowContext _context;

        public CompressConsumer(DataFlowContext context)
        {
            _context = context;
        }

        public void StartConsuming()
        {
            ProcessingBlock currentProcessedBlock;
            while (_context.BlockQueue.TryDequeue(out currentProcessedBlock) && !_context.IsExecutionStopped())
            {
                var compressedBytes = GZipHelper.Compress(currentProcessedBlock.BlockData);

                _context.ConsumerEventsManager.GetCurrentThreadEvent(currentProcessedBlock.BlockIndex).WaitOne();
                _context.WriteFile.WriteToFile(compressedBytes);

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
