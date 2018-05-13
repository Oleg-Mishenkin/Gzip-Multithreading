using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using GzipAssessment.Commands;

namespace GzipAssessment
{
    class Program
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const int BlockSize = 1024 * 64;

        static void Main(string[] args)
        {
            _logger.Info("Starting...");

            try
            {
                var commandLineArguments = new CommandLineArguments(args);
                int iterations = 10;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var outputFile = commandLineArguments.OutputFile;

                using (Stream fs = File.OpenRead("../../Test.csv"))
                using (Stream fd = File.Create("../../gj.zip"))
                using (Stream csStream = new GZipStream(fd, CompressionMode.Compress))
                {
                    byte[] buffer = new byte[1024];
                    int nRead;
                    while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        csStream.Write(buffer, 0, nRead);
                    }
                }

                using (Stream fd = File.Create(outputFile))
                using (Stream fs = File.OpenRead("../../gj.zip"))
                using (Stream csStream = new GZipStream(fs, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[BlockSize];
                    int nRead;
                    while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fd.Write(buffer, 0, nRead);
                    }
                }
                /*for (int i = 0; i < iterations; i++)
                {
                    commandLineArguments.OutputFile = $"{outputFile + i}";
                    DoWork(commandLineArguments, Environment.ProcessorCount);
                }*/

                stopWatch.Stop();
                _logger.Info(
                    $"Finished in {stopWatch.ElapsedMilliseconds}ms. Avg compression time is {stopWatch.ElapsedMilliseconds / iterations}ms");
            }
            catch (UserReadableException e)
            {
                _logger.Info(e.ErrorMessage);
            }
            catch (Exception e)
            {
                _logger.Info("Application has crashed. Check logs for details.");
                _logger.Fatal(e);
            }

            Console.ReadKey();
        }

        private static void DoWork(CommandLineArguments arguments, int threadCount)
        {
            var executionContext = new CommandContext(BlockSize, threadCount);
            using (var command = CommandFactory.CreateCommand(executionContext, arguments))
            {
                for (int i = 0; i < threadCount; i++)
                {
                    var zipThread = new SyncThread(command);
                    zipThread.Start();
                }

                executionContext.WaitWorkDone();
            }
        }
    }
}
