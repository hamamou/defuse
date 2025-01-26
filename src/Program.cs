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

        try
        {
            logger.LogInformation("Starting PDF merge process...");
            var pdfMerger = serviceProvider.GetRequiredService<PdfMerger>();
            pdfMerger.MergePdfs(options.InputFiles);

            Console.WriteLine($"Merged PDF saved as: {pdfMerger.OutputFileName}");
            logger.LogInformation($"Merged PDF saved as: {pdfMerger.OutputFileName}");

            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during the PDF merge process.");
            Console.Error.WriteLine($"Error: {ex.Message}");
            return -1;
        }
        finally
        {
            if (serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }

    private static int HandleParseError(IEnumerable<Error> errors)
    {
        var errorArray = errors as Error[] ?? errors.ToArray();
        if (errorArray.Any(e => e is HelpRequestedError or VersionRequestedError))
        {
            return 0; // Help or version requested - no error.
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

        return serviceCollection.BuildServiceProvider();
    }
}
