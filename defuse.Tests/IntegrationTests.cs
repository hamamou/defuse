using Defuse.Commands;
using Defuse.pdf;
using Defuse.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using Console = System.Console;

namespace defuse.Tests;

public class IntegrationTests
{
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void Setup()
    {
        _serviceProvider = ConfigureTestServices();
    }

    [Test]
    public async Task Run()
    {
        var pdfMerger = _serviceProvider.GetRequiredService<Merge>();
        var filePaths = new[] { "Assets/Doc1.pdf", "Assets/Doc2.pdf" };

        var mergeResult = await pdfMerger.MergePdfs(filePaths);

        Assert.That(mergeResult.IsSuccess, Is.True, "PDF merging failed!");

        var pdfDocumentWrapper = _serviceProvider.GetRequiredService<IPdfDocumentWrapper>();
        var pdfDocument = pdfDocumentWrapper.GetPdfDocument();
        Assert.That(pdfDocument, Is.Not.Null, "PDF document is null!");
        Assert.That(pdfDocument.PageCount, Is.EqualTo(4), "Page count is incorrect!");
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    private ServiceProvider ConfigureTestServices()
    {
        var serviceCollection = new ServiceCollection();

        // Add Logging
        serviceCollection.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Debug);
        });

        // Test-specific PdfDocument
        var pdfDocument = new PdfDocument();
        serviceCollection.AddTransient(_ => pdfDocument);

        // Add application services
        serviceCollection.AddTransient<Merge>();
        serviceCollection.AddTransient<IFileUtilities, FileUtilities>();
        serviceCollection.AddTransient<IPdfReader, PdfReader>();

        // Add Test PdfDocumentWrapper
        serviceCollection.AddTransient<IPdfDocumentWrapper>(_ => new PdfDocumentWrapperTest(
            pdfDocument
        ));

        // Add Output Path as Singleton
        var outputPath = Path.Combine(Utils.GetProjectRootDirectory(), "Assets", "output.pdf");
        serviceCollection.AddSingleton(outputPath);

        return serviceCollection.BuildServiceProvider();
    }

    // Custom Test PdfDocumentWrapper Implementation
    private class PdfDocumentWrapperTest(PdfDocument pdfDocument) : PdfDocumentWrapper(pdfDocument)
    {
        public override Task SaveAsync(string path)
        {
            Console.Out.WriteLine($"Simulated save: {path}");
            return Task.CompletedTask;
        }

        public override void ShowDocument(string filePath)
        {
            Console.Out.WriteLine($"Simulated open: {filePath}");
        }
    }
}
