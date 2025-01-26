using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Quality;

namespace nmergi;

public class PdfMerger
{
    public string OutputFileName { get; } = FileUtilities.GetTempPdfFullFileName("merged");

    public void MergePdfs(string[] pdfPaths)
    {
        using var outputDocument = new PdfDocument();

        foreach (var path in pdfPaths)
        {
            var filePaths = FileHandler.GetPdfFilePaths(path);
            AddFileContentToPdf(outputDocument, filePaths);
        }

        outputDocument.Save(OutputFileName);
        FileUtilities.ShowDocument(OutputFileName);
    }

    private static void AddFileContentToPdf(PdfDocument outputDocument, IList<string> pdfFilePaths)
    {
        foreach (var filePath in pdfFilePaths)
        {
            var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
            for (var i = 0; i < inputDocument.PageCount; i++)
            {
                outputDocument.AddPage(inputDocument.Pages[i]);
            }
        }
    }
}
