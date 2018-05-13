using System.Threading;
using GzipAssessment.Commands;

namespace GzipAssessment
{
    public class SyncThread
    {
        private readonly Thread _thread;

        public SyncThread(ICommand command)
        {
            _thread = new Thread(command.Execute);
        }

        public void Start()
        {
            _thread.Start();
        }
    }
}
