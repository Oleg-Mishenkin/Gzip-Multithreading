using System;

namespace GzipAssessment.DataFlow
{
    public class DataProcessingExceptionEventArgs : EventArgs
    {
        public DataProcessingExceptionEventArgs(Exception e)
        {
            Exception = e;
        }

        public Exception Exception { get; private set; }
    }
}
