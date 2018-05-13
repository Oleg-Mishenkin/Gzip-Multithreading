using System;

namespace GzipAssessment.Commands
{
    public interface ICommand : IDisposable
    {
        void Execute();
    }
}
