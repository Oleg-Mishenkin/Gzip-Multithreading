namespace GzipAssessment.DataFlow
{
    public class DataFlowFactory
    {
        public static IConsumer CreateConsumer(DataFlowContext context, DataFlowType flowType)
        {
            if (flowType == DataFlowType.Decompress) return new DecompressConsumer(context);
            return new CompressConsumer(context);
        }

        public static IProducer CreateProducer(DataFlowContext context, DataFlowType flowType)
        {
            if (flowType == DataFlowType.Decompress) return new DecompressProducer(context);
            return new CompressProducer(context);
        }
    }
}
