using PdfSharp.Pdf;

namespace nmergi;

public interface IPdfDocumentWrapper
{
    void AddPage(PdfPage? page);
    void Save(string fileName);
    int PageCount { get; }

    public PdfPages? Pages { get; }
}

public class PdfDocumentWrapper(PdfDocument pdfDocument) : IPdfDocumentWrapper
{
    public void AddPage(PdfPage? page)
    {
        if (page != null)
            pdfDocument.AddPage(page);
    }

    public void Save(string fileName)
    {
        pdfDocument.Save(fileName);
    }

    public int PageCount => pdfDocument.PageCount;
    public PdfPages Pages => pdfDocument.Pages;
}
