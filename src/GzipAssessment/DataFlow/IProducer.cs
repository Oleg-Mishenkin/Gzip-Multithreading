using System;

namespace GzipAssessment.DataFlow
{
    public interface IProducer
    {
        void StartProducing();

        event EventHandler ProgressChanged;
    }
}
