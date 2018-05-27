using System.IO;
using GzipAssessment.DataFlow;
using Xunit;

namespace GzipAssessment.Tests
{
    public class GzipTests
    {
        private static string TestFile = "../../TestData/Test.csv";

        [Fact]
        public void Compress_decompress_should_create_correct_zip_file()
        {
            var compressArguments = new CommandLineArguments(new[] {"compress", TestFile, "./result.zip"});
            new DataFlowBuilder(compressArguments, 1).StartFlow();

            var decompressArguments = new CommandLineArguments(new[] {"decompress", "result.zip", "./result.csv"});
            new DataFlowBuilder(decompressArguments, 1).StartFlow();

            Assert.NotNull(new FileInfo("result.zip"));
            Assert.NotNull(new FileInfo("result.csv"));
            Assert.Equal(new FileInfo("result.csv").Length, new FileInfo(TestFile).Length);

            File.Delete("result.csv");
            File.Delete("result.zip");
        }

        [Fact]
        public void Invalid_input_should_throw_user_exception()
        {
            Assert.Throws<UserReadableException>(() => new CommandLineArguments(new[] { "compress", "../../TestData/Invalid.csv", "./result.zip" }));
            Assert.Throws<UserReadableException>(() => new CommandLineArguments(new[] { "compress", TestFile, "./InvalidDirectory/result.zip" }));
            Assert.Throws<UserReadableException>(() => new CommandLineArguments(new[] { "invalid", TestFile, "./result.zip" }));
        }
    }
}
