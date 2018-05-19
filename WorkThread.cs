using System.Threading;
using GzipAssessment.Commands;

namespace GzipAssessment
{
    public class WorkThread
    {
        private readonly Thread _thread;

        public WorkThread(ICommand command)
        {
            _thread = new Thread(command.Execute);
        }

        public void Start()
        {
            _thread.Start();
        }
    }
}
