using System.IO;
using System.IO.Compression;

namespace GzipAssessment
{
    public static class GZipHelper
    {
        public static byte[] Decompress(byte[] input)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(input))
                {
                    using (Stream csStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        csStream.CopyTo(outputStream);
                    }
                }
                return outputStream.ToArray();
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (var compressedMemoryStream = new MemoryStream())
            {
                using (GZipStream compressedStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress))
                {
                    compressedStream.Write(data, 0, data.Length);
                }

                return compressedMemoryStream.ToArray();
            }
        }

    }
}