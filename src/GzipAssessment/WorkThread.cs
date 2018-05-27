using System;
using System.Threading;
using GzipAssessment.DataFlow;

namespace GzipAssessment
{
    public class WorkThread
    {
        public event EventHandler CommandException;

        private readonly Action _action;
        private readonly Thread _thread;

        public WorkThread(Action action)
        {
            _action = action;
            _thread = new Thread(SafeExecute);
        }

        public void Start()
        {
            _thread.Start();
        }

        private void SafeExecute()
        {
            try
            {
                _action();
            }
            catch (Exception e)
            {
                OnCommandException(new DataProcessingExceptionEventArgs(e));
            }
        }

        protected void OnCommandException(DataProcessingExceptionEventArgs exception)
        {
            if (CommandException != null)
            {
                CommandException(this, exception);
            }
        }
    }
}
