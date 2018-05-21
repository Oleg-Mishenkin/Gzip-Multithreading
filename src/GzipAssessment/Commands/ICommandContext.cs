using System;

namespace GzipAssessment.Commands
{
    public interface ICommandContext : IDisposable
    {
        void Proceed();

        event EventHandler ProgressChanged;
    }
}
