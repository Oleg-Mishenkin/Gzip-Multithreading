﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace GzipAssessment.Commands
{
    public class CommandContext
    {
        private readonly int _blockSize;
        private int _currentBlockIndex = -1;
        private SyncThread _currentInOrderThread;
        private ReadOnlyCollection<AutoResetEvent> _threadEvents;

        private AutoResetEvent _workDoneEvent = new AutoResetEvent(false);

        public CommandContext(int blockSize, int threadCount)
        {
            _blockSize = blockSize;
            InitThreadEvents(threadCount);
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

        public int SetCurrentBlock()
        {
            return Interlocked.Increment(ref _currentBlockIndex);
        }

        public int BlockSize
        {
            get { return _blockSize; }
        }

        public void OnWorkDone()
        {
            _workDoneEvent.Set();
        }

        public void WaitWorkDone()
        {
            _workDoneEvent.WaitOne();
        }

        public AutoResetEvent GetCurrentThreadEvent(int order)
        {
            return _threadEvents[order % _threadEvents.Count];
        }

        public AutoResetEvent GetNextThreadEvent(int order)
        {
            return _threadEvents[(order + 1) % _threadEvents.Count];
        }
    }
}
