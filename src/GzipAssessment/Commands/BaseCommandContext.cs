using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace GzipAssessment.Commands
{
    public abstract class BaseCommandContext : ICommandContext
    {
        private readonly CommandLineArguments _args;
        private int _currentBlockIndex = -1;
        private ReadOnlyCollection<AutoResetEvent> _threadEvents;
        protected AutoResetEvent WorkDoneEvent = new AutoResetEvent(false);

        public BaseCommandContext(int blockSize, int threadCount, CommandLineArguments args)
        {
            _args = args;
            ReadFile = new FileToRead(args.InputFile);
            WriteFile = new FileToWrite(args.OutputFile);
            BlockSize = blockSize;
            BlockQueue = new BlockingQueue<ProcessingBlock>(Environment.ProcessorCount * 2); // let it be twice the number of threads
            InitThreadEvents(threadCount);
        }

        public event EventHandler ProgressChanged;

        public FileToWrite WriteFile { get; private set; }

        public FileToRead ReadFile { get; private set; }

        public int SetNextCurrentBlock()
        {
            return Interlocked.Increment(ref _currentBlockIndex);
        }

        public int BlockSize { get; private set; }

        public void OnWorkDone()
        {
            WorkDoneEvent.Set();
        }
        
        public BlockingQueue<ProcessingBlock> BlockQueue { get; private set; }

        public AutoResetEvent GetCurrentThreadEvent(int order)
        {
            return _threadEvents[order % _threadEvents.Count];
        }

        public AutoResetEvent GetNextThreadEvent(int order)
        {
            return _threadEvents[(order + 1) % _threadEvents.Count];
        }

        public void Dispose()
        {
            ReadFile.Dispose();
            WriteFile.Dispose();
        }

        public abstract void StartProducer();

        public void Proceed()
        {
            InitCommands();
            StartProducer();

            WorkDoneEvent.WaitOne();
            BlockQueue.Close();
        }

        private void InitCommands()
        {
            var command = CommandFactory.CreateCommand(this, _args);
            for (int i = 0; i < _threadEvents.Count; i++)
            {
                var zipThread = new WorkThread(command);
                zipThread.Start();
            }

        }

        protected void OnProgressChanged(ProgressChangedEventArgs eventArgs)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, eventArgs);
            }
        }

        private void InitThreadEvents(int threadCount)
        {
            List<AutoResetEvent> threadEvents = new List<AutoResetEvent>();
            threadEvents.Add(new AutoResetEvent(true));

            for (int i = 1; i < threadCount; i++)
            {
                threadEvents.Add(new AutoResetEvent(false));
            }

            _threadEvents = new ReadOnlyCollection<AutoResetEvent>(threadEvents);
        }

    }
}