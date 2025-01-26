using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Quality;

namespace nmergi;

public class PdfMerger
{
    public string OutputFileName { get; }
    private IPdfDocumentWrapper OutputDocumentWrapper { get; }
    private IFileUtilities FileUtilities { get; }
    private IPdfReader PdfReader { get; }

    public PdfMerger(
        IPdfDocumentWrapper outputDocumentWrapper,
        IPdfReader pdfReader,
        IFileUtilities fileUtilities,
        string? outputFileName
    )
    {
        OutputDocumentWrapper = outputDocumentWrapper;
        PdfReader = pdfReader;
        FileUtilities = fileUtilities;
        OutputFileName = outputFileName ?? FileUtilities.GetTempPdfFullFileName("merged");
    }

    public void MergePdfs(string[] pdfPaths)
    {
        foreach (var path in pdfPaths)
        {
            var filePaths = FileUtilities.GetPdfFilePaths(path);
            AddFileContentToPdf(OutputDocumentWrapper, filePaths);
        }

        OutputDocumentWrapper.Save(OutputFileName);
        FileUtilities.ShowDocument(OutputFileName);
    }

    private void AddFileContentToPdf(IPdfDocumentWrapper outputDocument, IList<string> pdfFilePaths)
    {
        foreach (var filePath in pdfFilePaths)
        {
            var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
            for (var i = 0; i < inputDocument.PageCount; i++)
            {
                outputDocument.AddPage(inputDocument.Pages?[i]);
            }
        }
    }
}
