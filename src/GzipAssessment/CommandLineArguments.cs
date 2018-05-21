using System;
using System.IO;
using System.Security;
using GzipAssessment.Commands;

namespace GzipAssessment
{
    public class CommandLineArguments
    {
        public CommandLineArguments(string[] args)
        {
            Validate(args);
        }

        private void Validate(string[] args)
        {
            if (args.Length < 3)
            {
                throw new UserReadableException("Unable to parse arguments. Please, specify them in the next form: compress/decompress path_to_source_file path_to_destination_file");
            }

            var command = args[0];
            switch (command)
            {
                case "compress":
                    Command = CommandType.Compress;
                    break;
                case "decompress":
                    Command = CommandType.Decompress;
                    break;
                default: throw new UserReadableException("Command should be either 'compress' or 'decompress'");
            }

            var inputFile = args[1];
            if (File.Exists(inputFile))
                InputFile = inputFile;
            else
                throw new UserReadableException("Can't find input file or user doesn't have read permissions on it");
            var outputFile = args[2];

            try
            {
                var resultFileDirectory = Path.GetDirectoryName(outputFile);
                if (resultFileDirectory == null) throw new UserReadableException("Output file directory not found");
                var resultFilePath = Path.GetFullPath(resultFileDirectory);
                if (string.IsNullOrEmpty(resultFilePath) || !Directory.Exists(resultFilePath))
                    throw new UserReadableException("Output file directory not found");
            }
            catch (ArgumentException)
            {
                throw new UserReadableException("Output file directory not found");
            }
            catch (SecurityException)
            {
                throw new UserReadableException("User doesn't have permissions for a path to output file");
            }

            if (!Path.HasExtension(outputFile))
                outputFile += ".gz";

            OutputFile = outputFile;
        }

        public CommandType Command { get; set; }

        public string InputFile { get; set; }

        public string OutputFile { get; set; }
    }
}
