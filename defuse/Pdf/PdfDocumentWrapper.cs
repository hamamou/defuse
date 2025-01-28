using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using PdfSharp.Quality;

namespace Defuse.pdf;

public interface IPdfDocumentWrapper
{
    void AddPage(PdfPage? page);
    void Save(string fileName);
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

    public virtual void Save(string fileName)
    {
        pdfDocument.Save(fileName);
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
