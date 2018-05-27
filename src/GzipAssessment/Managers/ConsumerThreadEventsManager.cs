using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace GzipAssessment.Managers
{
    public class ConsumerThreadEventsManager
    {
        protected AutoResetEvent WorkDoneEvent = new AutoResetEvent(false);
        private ReadOnlyCollection<AutoResetEvent> _threadEvents;

        public ConsumerThreadEventsManager(int threadCount)
        {
            InitThreadEvents(threadCount);
        }

        public void OnWorkDone()
        {
            WorkDoneEvent.Set();
        }

        public void WaitWorkDone()
        {
            WorkDoneEvent.WaitOne();
        }

        public AutoResetEvent GetCurrentThreadEvent(int order)
        {
            return _threadEvents[order % _threadEvents.Count];
        }

        public AutoResetEvent GetNextThreadEvent(int order)
        {
            return _threadEvents[(order + 1) % _threadEvents.Count];
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
