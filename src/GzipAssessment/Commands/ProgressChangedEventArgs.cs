using System;

namespace GzipAssessment.Commands
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