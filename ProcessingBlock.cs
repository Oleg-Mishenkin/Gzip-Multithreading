namespace GzipAssessment
{
    public class ProcessingBlock
    {
        private readonly int _blockIndex;
        private readonly byte[] _blockData;
        private readonly bool _isLast;

        public ProcessingBlock(int blockIndex, byte[] blockData, bool isLast)
        {
            _blockIndex = blockIndex;
            _blockData = blockData;
            _isLast = isLast;
        }

        public int BlockIndex
        {
            get { return _blockIndex; }
        }

        public byte[] BlockData
        {
            get { return _blockData; }
        }

        public bool IsLastBlock
        {
            get { return _isLast; }
        }
    }
}
