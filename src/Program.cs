using Microsoft.Extensions.DependencyInjection;
using PdfSharp.Pdf;

namespace nmergi;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: nmergi <pdf1> <pdf2> ... <pdfN>");
            return;
        }

        var serviceProvider = ConfigureServices();

        var pdfMerger = serviceProvider.GetRequiredService<PdfMerger>();
        pdfMerger.MergePdfs(args);

        Console.WriteLine($"Merged PDF saved as: {pdfMerger.OutputFileName}");
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient<PdfDocument>();
        serviceCollection.AddTransient<PdfMerger>();
        serviceCollection.AddTransient<IFileUtilities, FileUtilities>();
        serviceCollection.AddTransient<IPdfReader, PdfReader>();
        serviceCollection.AddTransient<IPdfDocumentWrapper, PdfDocumentWrapper>();

        return serviceCollection.BuildServiceProvider();
    }
}
