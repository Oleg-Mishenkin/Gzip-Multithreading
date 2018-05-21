﻿using System.IO;
using GzipAssessment.Commands;
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
            using (var executionContext =
                CommandFactory.CreateCommandContext(Constants.BlockSize, 1, compressArguments))
            {
                executionContext.Proceed();
            }

            var decompressArguments = new CommandLineArguments(new[] {"decompress", "result.zip", "./result.csv"});
            using (var executionContext =
                CommandFactory.CreateCommandContext(Constants.BlockSize, 1, decompressArguments))
            {
                executionContext.Proceed();
            }

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