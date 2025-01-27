using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;

namespace nmergi;

public class Program
{
    public static void Main(string[] args)
    {
        var result = Parser
            .Default.ParseArguments<Options>(args)
            .MapResult(RunOptionsAndReturnExitCode, HandleParseError);

        Environment.Exit(result);
    }

    private static int RunOptionsAndReturnExitCode(Options options)
    {
        var serviceProvider = ConfigureServices();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Starting PDF merge process...");
        var pdfMerger = serviceProvider.GetRequiredService<PdfMerger>();
        var result = pdfMerger.MergePdfs(options.InputFiles);
        if (result.IsFailure)
        {
            logger.LogError(
                "An error occurred during the PDF merge process: {ErrorMessage}",
                result.Error
            );
            Console.Error.WriteLine($"Error: {result.Error}");
            return -1;
        }

        Console.WriteLine($"Merged PDF saved as: {pdfMerger.OutputPath}");
        logger.LogInformation("Merged PDF saved as: {OutputFileName}", pdfMerger.OutputPath);

        return 0;
    }

    private static int HandleParseError(IEnumerable<Error> errors)
    {
        var errorArray = errors as Error[] ?? errors.ToArray();
        if (errorArray.Any(e => e is HelpRequestedError or VersionRequestedError))
        {
            return 0;
        }

        return -1;
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        // Add logging
        serviceCollection.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });

        // Register application services
        serviceCollection.AddTransient<PdfDocument>();
        serviceCollection.AddTransient<PdfMerger>();
        serviceCollection.AddTransient<IFileUtilities, FileUtilities>();
        serviceCollection.AddTransient<IPdfReader, PdfReader>();
        serviceCollection.AddTransient<IPdfDocumentWrapper, PdfDocumentWrapper>();
        serviceCollection.AddSingleton(DateTime.Now.ToString("yyyyMMddHHmmss"));

        return serviceCollection.BuildServiceProvider();
    }
}
