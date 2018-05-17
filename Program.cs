using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using GzipAssessment.Commands;

namespace GzipAssessment
{
    class Program
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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
                //DoWork(commandLineArguments, Environment.ProcessorCount);
                /*using (Stream fs = File.OpenRead("../../Test.csv"))
                using (Stream fd = File.Create("../../gj.zip"))
                using (Stream csStream = new GZipStream(fd, CompressionMode.Compress))
                {
                    byte[] buffer = new byte[1024];
                    int nRead;
                    while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        csStream.Write(buffer, 0, nRead);
                    }
                }*/
                var fileLength = new FileInfo("../../Results/Compressed.zip").Length;

                using (Stream fd = File.Create("../../Results/Result.csv"))
                using (Stream fs = File.OpenRead("../../Results/Compressed.zip"))
                {
                    int currentStreamPosition = 0;
                    var gzipHeaderMatches = 0;
                    List<byte> currentDataBlock = new List<byte>();
                    int blockNumber = 0;
                    while (currentStreamPosition < fileLength)
                    {
                        var currentByte = fs.ReadByte();
                        currentStreamPosition++;
                        currentDataBlock.Add((byte)currentByte);

                        if (currentStreamPosition == fileLength - 1) // last block of data
                        {
                            using (var memoryStream = new MemoryStream(currentDataBlock.ToArray()))
                            {
                                using (Stream csStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                                {
                                    byte[] buffer = new byte[Constants.BlockSize];
                                    int nRead;
                                    while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        fd.Write(buffer, 0, nRead);
                                    }
                                }
                            }

                            break;
                        }

                        if (currentByte == Constants.GZipDefaultHeader[gzipHeaderMatches])
                        {
                            if (gzipHeaderMatches == Constants.GZipDefaultHeader.Length - 1 && blockNumber > 0)
                            {
                                var nextBlockHeader = currentDataBlock.GetRange(currentDataBlock.Count - Constants.GZipDefaultHeader.Length, Constants.GZipDefaultHeader.Length);
                                currentDataBlock.RemoveRange(currentDataBlock.Count - Constants.GZipDefaultHeader.Length, Constants.GZipDefaultHeader.Length); // we've found the beginning of the next block
                                using (var memoryStream = new MemoryStream(currentDataBlock.ToArray()))
                                {
                                    using (Stream csStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                                    {
                                        byte[] buffer = new byte[Constants.BlockSize];
                                        int nRead;
                                        while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            fd.Write(buffer, 0, nRead);
                                        }
                                    }
                                }

                                currentDataBlock = nextBlockHeader;
                                blockNumber++;
                                gzipHeaderMatches = 0;
                            }
                            else if (gzipHeaderMatches == Constants.GZipDefaultHeader.Length - 1 && blockNumber == 0)
                            {
                                gzipHeaderMatches = 0;
                                blockNumber++;
                            }
                            else
                            {
                                gzipHeaderMatches++;
                            }
                        }
                        else
                        {
                            gzipHeaderMatches = 0;
                        }
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
            var executionContext = CommandFactory.CreateCommandContext(Constants.BlockSize, threadCount, arguments);
            using (var command = CommandFactory.CreateCommand(executionContext, arguments))
            {
                for (int i = 0; i < threadCount; i++)
                {
                    var zipThread = new SyncThread(command);
                    zipThread.Start();
                }

                executionContext.Proceed();
            }
        }
    }
}
