using Defuse.pdf;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf.IO;

namespace Defuse.pdf;

public interface IPdfReader
{
    Result<IPdfDocumentWrapper> Open(string filePath, PdfDocumentOpenMode mode);
}

public class PdfReader() : IPdfReader
{
    public Result<IPdfDocumentWrapper> Open(string filePath, PdfDocumentOpenMode mode)
    {
        try
        {
            return new PdfDocumentWrapper(PdfSharp.Pdf.IO.PdfReader.Open(filePath, mode));
        }
        catch (Exception e)
        {
            return $"An error occurred while opening the PDF file: {e.Message}";
        }
    }
}
