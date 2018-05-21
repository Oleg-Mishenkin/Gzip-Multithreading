﻿using System;
using System.Diagnostics;
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
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                DoWork(commandLineArguments, Environment.ProcessorCount);
                
                stopWatch.Stop();
                _logger.Info($"Finished in {stopWatch.ElapsedMilliseconds}ms");
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
            using (var executionContext = CommandFactory.CreateCommandContext(Constants.BlockSize, threadCount, arguments))
            {
                executionContext.ProgressChanged += (sender, e) =>
                {
                    Console.Out.Write("\rCompleted " + ((ProgressChangedEventArgs)e).PercentageCompleted + "%");
                };
                
                executionContext.Proceed();

                Console.Out.WriteLine();
            }
        }
    }
}