using System;
using System.Collections.Generic;
using GzipAssessment.Managers;

namespace GzipAssessment.DataFlow
{
    public class DataFlowBuilder : IDisposable
    {
        private readonly CommandLineArguments _arguments;
        private readonly int _consumerThreadsCount;
        private DataFlowContext _executionContext;
        private List<WorkThread> _threads = new List<WorkThread>();

        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public DataFlowBuilder(CommandLineArguments arguments, int consumerThreadsCount)
        {
            _arguments = arguments;
            _consumerThreadsCount = consumerThreadsCount;
        }

        public DataFlowBuilder BuildContext()
        {
            _executionContext = new DataFlowContext(_arguments, new ConsumerThreadEventsManager(_consumerThreadsCount));
            return this;
        }

        public void StartFlow()
        {
            _threads.ForEach(x => x.Start());

            _executionContext.ConsumerEventsManager.WaitWorkDone();
            _executionContext.BlockQueue.Close();
            Console.WriteLine();
        }

        public DataFlowBuilder BuildConsumers()
        {
            for (int i = 0; i < _consumerThreadsCount; i++)
            {
                var consumer = DataFlowFactory.CreateConsumer(_executionContext, _arguments.DataFlow);
                var workThread = new WorkThread(consumer.StartConsuming);
                workThread.CommandException += (sender, args) =>
                {
                    _logger.Fatal(((DataProcessingExceptionEventArgs)args).Exception);
                    Console.WriteLine("\r\nAn exception occured while processing data. Check logs for details");
                    _executionContext.StopExecution();
                };

                _threads.Add(workThread);
            }

            return this;
        }

        public DataFlowBuilder BuildProducer()
        {
            var producer = DataFlowFactory.CreateProducer(_executionContext, _arguments.DataFlow);

            var workThread = new WorkThread(producer.StartProducing);
            workThread.CommandException += (sender, args) =>
            {
                _logger.Fatal(((DataProcessingExceptionEventArgs)args).Exception);
                Console.WriteLine("\r\nAn exception occured while reading data. Check logs for details");
                _executionContext.StopExecution();
            };

            producer.ProgressChanged += (sender, e) =>
            {
                Console.Write("\rCompleted " + ((ProgressChangedEventArgs)e).PercentageCompleted + "%");
            };

            _threads.Add(workThread);

            return this;
        }

        public void Dispose()
        {
            _executionContext?.Dispose();
        }
    }
}
