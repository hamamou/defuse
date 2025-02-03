using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using PdfSharp.Quality;

namespace Defuse.pdf;

public interface IPdfDocumentWrapper
{
    void AddPage(PdfPage? page);
    Task SaveAsync(string fileName);
    int PageCount { get; }
    public PdfPages? Pages { get; }
    void ShowDocument(string filePath);
    public PdfDocument GetPdfDocument();
}

public class PdfDocumentWrapper(PdfDocument pdfDocument) : IPdfDocumentWrapper
{
    public void AddPage(PdfPage? page)
    {
        if (page != null)
            pdfDocument.AddPage(page);
    }

    public virtual async Task SaveAsync(string fileName)
    {
        await pdfDocument.SaveAsync(fileName);
    }

    public virtual void ShowDocument(string filePath)
    {
        Console.WriteLine($"Document opened: {filePath}");
        PdfFileUtility.ShowDocument(filePath);
    }

    public int PageCount => pdfDocument.PageCount;
    public PdfPages Pages => pdfDocument.Pages;

    public PdfDocument GetPdfDocument()
    {
        return pdfDocument;
    }
}
