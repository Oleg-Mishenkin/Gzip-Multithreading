using System;

namespace GzipAssessment.DataFlow
{
    public class CompressProducer : IProducer
    {
        private readonly DataFlowContext _context;

        public CompressProducer(DataFlowContext context)
        {
            _context = context;
        }

        public void StartProducing()
        {
            int currentBlockIndex = 0;

            while (currentBlockIndex * Constants.BlockSize <= _context.ReadFile.FileLength && !_context.IsExecutionStopped())
            {
                byte[] data = _context.ReadFile.ReadBytes(currentBlockIndex * Constants.BlockSize, Constants.BlockSize);
                var isLastBlock = (currentBlockIndex + 1) * Constants.BlockSize > _context.ReadFile.FileLength;

                _context.BlockQueue.Enqueue(new ProcessingBlock(currentBlockIndex, data, isLastBlock));

                OnProgressChanged(new ProgressChangedEventArgs((int)(currentBlockIndex * Constants.BlockSize * 100.0 / _context.ReadFile.FileLength)));
                currentBlockIndex++;
            }

            OnProgressChanged(new ProgressChangedEventArgs(100));
        }

        public event EventHandler ProgressChanged;

        private void OnProgressChanged(ProgressChangedEventArgs eventArgs)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, eventArgs);
            }
        }
    }
}
