using System;
using GzipAssessment.Managers;

namespace GzipAssessment.DataFlow
{
    public class DataFlowContext : IDisposable
    {
        private bool _isExecutionCancelled;
        public DataFlowContext(CommandLineArguments args, ConsumerThreadEventsManager consumerThreadEventsManager)
        {
            ConsumerEventsManager = consumerThreadEventsManager;
            ReadFile = new FileToRead(args.InputFile);
            WriteFile = new FileToWrite(args.OutputFile);
            BlockQueue = new BlockingQueue<ProcessingBlock>(Environment.ProcessorCount * 2); // let it be twice the number of threads
        }

        public ConsumerThreadEventsManager ConsumerEventsManager { get; private set; }

        public FileToWrite WriteFile { get; private set; }

        public FileToRead ReadFile { get; private set; }

        public BlockingQueue<ProcessingBlock> BlockQueue { get; private set; }

        public void StopExecution()
        {
            _isExecutionCancelled = true;
            ConsumerEventsManager.OnWorkDone();
        }

        public bool IsExecutionStopped()
        {
            return _isExecutionCancelled;
        }

        public void Dispose()
        {
            ReadFile.Dispose();
            WriteFile.Dispose();
        }
    }
}