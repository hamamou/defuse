﻿using Cocona;
using Defuse.Merge;
using Defuse.pdf;
using Defuse.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;

var builder = CoconaApp.CreateBuilder();
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddTransient<IPdfDocumentWrapper, PdfDocumentWrapper>();
builder.Services.AddTransient<PdfDocument>();
builder.Services.AddTransient<PdfMerger>();
builder.Services.AddTransient<IFileUtilities, FileUtilities>();
builder.Services.AddTransient<IPdfReader, PdfReader>();
var app = builder.Build();

app.AddCommand(
    "merge",
    (string[] files, string? output) =>
    {
        var merge = app.Services.GetRequiredService<PdfMerger>();
        merge.MergePdfs(files, output);
    }
);
