namespace GzipAssessment.Commands
{
    public class CompressCommandContext : BaseCommandContext
    {
        public CompressCommandContext(int blockSize, int threadCount, string readFile, string writeFile) : base(blockSize, threadCount, readFile, writeFile)
        {
        }
        
        public override void Proceed()
        {
            WorkDoneEvent.WaitOne();
        }
    }
}
