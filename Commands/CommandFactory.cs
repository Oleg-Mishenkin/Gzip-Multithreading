namespace GzipAssessment.Commands
{
    public class CommandFactory
    {
        public static ICommand CreateCommand(ICommandContext context, CommandLineArguments args)
        {
            if (args.Command == CommandType.Decompress) return new DecompressCommand(context, args.InputFile, args.OutputFile);
            return new CompressCommand(context, args.InputFile, args.OutputFile);
        }

        public static ICommandContext CreateCommandContext(int blockSize, int threadCount, CommandLineArguments args)
        {
            if (args.Command == CommandType.Decompress) return new DecompressCommandContext(Constants.BlockSize, threadCount);
            return new CompressCommandContext(Constants.BlockSize, threadCount);
        }
    }
}
