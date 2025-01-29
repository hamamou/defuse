using Defuse.Utilities;
using PdfSharp.Pdf.IO;

namespace Defuse.pdf;

public class PdfMerger
{
    private IPdfDocumentWrapper DocumentWrapper { get; }
    private IFileUtilities FileUtilities { get; }
    private IPdfReader PdfReader { get; }

    public string OutputPath { get; }

    public PdfMerger(
        IPdfDocumentWrapper documentWrapper,
        IPdfReader pdfReader,
        IFileUtilities fileUtilities
    )
    {
        DocumentWrapper =
            documentWrapper ?? throw new ArgumentNullException(nameof(documentWrapper));
        PdfReader = pdfReader ?? throw new ArgumentNullException(nameof(pdfReader));
        FileUtilities = fileUtilities ?? throw new ArgumentNullException(nameof(fileUtilities));

        if (!Path.IsPathRooted(OutputPath))
        {
            OutputPath = Path.Combine(Path.GetTempPath(), OutputPath ?? "output.pdf");
        }
    }

    public Result<bool> MergePdfs(IEnumerable<string>? pdfPaths)
    {
        if (pdfPaths == null)
            return Result.Failure<bool>($"PDF paths cannot be null. ${nameof(pdfPaths)}");
        var paths = pdfPaths.ToArray();
        if (paths.Length == 0)
            return Result.Failure<bool>($"PDF paths cannot be empty. ${nameof(pdfPaths)}");

        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Result.Failure<bool>($"A file path is null or empty. ${nameof(pdfPaths)}");

            var pdfFilePathsResult = FileUtilities.GetPdfFilePaths(path);
            if (pdfFilePathsResult.IsFailure)
                return Result.Failure<bool>(pdfFilePathsResult.Error!);

            var result = AddFileContentToPdf(DocumentWrapper, pdfFilePathsResult.Value!);
            if (result.IsFailure)
            {
                return Result.Failure<bool>(result.Error!);
            }
        }

        DocumentWrapper.Save(OutputPath);
        DocumentWrapper.ShowDocument(OutputPath);
        return true;
    }

    private Result<bool> AddFileContentToPdf(
        IPdfDocumentWrapper outputDocument,
        IList<string> pdfFilePaths
    )
    {
        if (pdfFilePaths.Count == 0)
        {
            return Result.Failure<bool>("No PDF file paths found.");
        }

        foreach (var filePath in pdfFilePaths)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Failure<bool>(
                    $"A file path is null or empty. ${nameof(pdfFilePaths)}"
                );
            }

            var result = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
            if (result.IsFailure)
            {
                return Result.Failure<bool>(result.Error!);
            }
            var inputDocument = result.Value;
            if (inputDocument == null || inputDocument.PageCount == 0)
            {
                return Result.Failure<bool>($"No pages found in document '{filePath}'.");
            }

            for (var i = 0; i < inputDocument.PageCount; i++)
            {
                var page = inputDocument.Pages?[i];
                if (page == null)
                {
                    return Result.Failure<bool>($"Page {i} not found in document '{filePath}'.");
                }

                outputDocument.AddPage(page);
            }
        }

        return true;
    }
}
