using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace nmergi;

public interface IPdfReader
{
    IPdfDocumentWrapper Open(string filePath, PdfDocumentOpenMode mode);
}

public class PdfReader : IPdfReader
{
    public IPdfDocumentWrapper Open(string filePath, PdfDocumentOpenMode mode)
    {
        return new PdfDocumentWrapper(PdfSharp.Pdf.IO.PdfReader.Open(filePath, mode));
    }
}
