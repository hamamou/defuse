using System.CommandLine;
using Defuse.pdf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Defuse.CommandParser
{
    public class Parser
    {
        private readonly IServiceProvider _serviceProvider;

        public Parser(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Command CreateMergeCommand()
        {
            var inputOption = new Option<string[]>("--input", "Input files to merge")
            {
                IsRequired = true,
                AllowMultipleArgumentsPerToken = true,
            };

            var outputOption = new Option<string>("--output", "Output file name");

            var mergeCommand = new Command("merge", "Merge multiple PDF files into one")
            {
                inputOption,
                outputOption,
            };

            mergeCommand.SetHandler(
                (string[] input, string output) =>
                {
                    var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
                    var pdfMerger = _serviceProvider.GetRequiredService<PdfMerger>();
                    logger.LogInformation("Starting PDF merge process...");

                    var result = pdfMerger.MergePdfs(input);
                    if (result.IsFailure)
                    {
                        logger.LogError(
                            "An error occurred during the PDF merge process: {ErrorMessage}",
                            result.Error?.Message
                        );
                        Console.Error.WriteLine($"Error: {result.Error?.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"Merged PDF saved as: {pdfMerger.OutputPath}");
                        logger.LogInformation(
                            "Merged PDF saved as: {OutputFileName}",
                            pdfMerger.OutputPath
                        );
                    }
                },
                inputOption,
                outputOption
            );

            return mergeCommand;
        }
    }
}
