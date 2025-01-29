using System.CommandLine;
using Defuse.CommandParser;
using Defuse.Merge;
using Defuse.pdf;
using Defuse.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;

namespace Defuse
{
    public class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var commandParser = new Parser(serviceProvider);

            var rootCommand = new RootCommand("defuse: A simple PDF utility tool")
            {
                commandParser.CreateMergeCommand(),
            };

            return await rootCommand.InvokeAsync(args);
        }

        private static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(config =>
            {
                config.AddConsole();
                config.SetMinimumLevel(LogLevel.Information);
            });

            serviceCollection.AddTransient<IPdfDocumentWrapper, PdfDocumentWrapper>();
            serviceCollection.AddTransient<PdfDocument>();
            serviceCollection.AddTransient<PdfMerger>();
            serviceCollection.AddTransient<IFileUtilities, FileUtilities>();
            serviceCollection.AddTransient<IPdfReader, PdfReader>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
