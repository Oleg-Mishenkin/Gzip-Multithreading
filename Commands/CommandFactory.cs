namespace GzipAssessment.Commands
{
    public class CommandFactory
    {
        public static ICommand CreateCommand(CommandContext context, CommandLineArguments args)
        {
            if (args.Command == CommandType.Decompress) return new DecompressCommand(context, args.InputFile, args.OutputFile);
            return new CompressCommand(context, args.InputFile, args.OutputFile);
        }
    }
}
