using System;

namespace GzipAssessment.DataFlow
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public ProgressChangedEventArgs(int percentageCompleted)
        {
            PercentageCompleted = percentageCompleted;
        }

        public int PercentageCompleted { get; private set; }
    }
}