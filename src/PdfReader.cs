using Bogoware.Monads;
using PdfSharp.Pdf.IO;

namespace nmergi;

public interface IPdfReader
{
    Result<IPdfDocumentWrapper> Open(string filePath, PdfDocumentOpenMode mode);
}

public class PdfReader : IPdfReader
{
    public Result<IPdfDocumentWrapper> Open(string filePath, PdfDocumentOpenMode mode)
    {
        try
        {
            return new PdfDocumentWrapper(PdfSharp.Pdf.IO.PdfReader.Open(filePath, mode));
        }
        catch (Exception e)
        {
            return Result.Failure<IPdfDocumentWrapper>(
                $"An error occurred while opening the PDF file: {e.Message}"
            );
        }
    }
}
