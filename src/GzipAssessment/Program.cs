using System;
using System.Diagnostics;
using GzipAssessment.DataFlow;

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

                using (var builder = new DataFlowBuilder(commandLineArguments, Environment.ProcessorCount))
                {
                    builder.BuildContext()
                        .BuildConsumers()
                        .BuildProducer()
                        .StartFlow();
                }
                
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
        }
    }
}
