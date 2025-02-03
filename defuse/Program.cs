using Cocona;
using Cocona.Builder;
using Defuse.Merge;
using Defuse.pdf;
using Defuse.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;

var builder = CoconaApp.CreateBuilder();
AddLogging(builder);
ConfigureServices(builder);

var app = builder.Build();
app.AddCommands<PdfMerger>();

app.Run();
return;

static void ConfigureServices(CoconaAppBuilder appBuilder)
{
    appBuilder.Services.AddTransient<IPdfDocumentWrapper, PdfDocumentWrapper>();
    appBuilder.Services.AddTransient<PdfDocument>();
    appBuilder.Services.AddTransient<PdfMerger>();
    appBuilder.Services.AddTransient<IFileUtilities, FileUtilities>();
    appBuilder.Services.AddTransient<IPdfReader, PdfReader>();
}

static void AddLogging(CoconaAppBuilder appBuilder)
{
    appBuilder.Services.AddLogging(config =>
    {
        config.AddConsole();
        config.SetMinimumLevel(LogLevel.Information);
    });
}
