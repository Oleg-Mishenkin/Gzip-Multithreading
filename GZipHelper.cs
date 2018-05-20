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
                        byte[] buffer = new byte[64 * 1024];
                        int bytesRead;

                        while ((bytesRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, bytesRead);
                        }
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