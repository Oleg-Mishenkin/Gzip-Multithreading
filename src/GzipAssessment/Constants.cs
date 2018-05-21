namespace GzipAssessment
{
    public static class Constants
    {
        public const int BlockSize = 1024 * 64;

        public static readonly byte[] GZipDefaultHeader = { 0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00 };
    }
}
