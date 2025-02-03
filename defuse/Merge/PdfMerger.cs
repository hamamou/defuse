using Cocona;
using Defuse.pdf;
using Defuse.Utilities;
using PdfSharp.Pdf.IO;

namespace Defuse.Merge;

public class PdfMerger(
    IPdfDocumentWrapper documentWrapper,
    IPdfReader pdfReader,
    IFileUtilities fileUtilities
)
{
    private IPdfDocumentWrapper DocumentWrapper { get; } =
        documentWrapper ?? throw new ArgumentNullException(nameof(documentWrapper));
    private IFileUtilities FileUtilities { get; } =
        fileUtilities ?? throw new ArgumentNullException(nameof(fileUtilities));
    private IPdfReader PdfReader { get; } =
        pdfReader ?? throw new ArgumentNullException(nameof(pdfReader));

    private string? OutputPath { get; set; }

    [Command("merge")]
    public async Task<Result<bool>> MergePdfs(IEnumerable<string> input, string? output = null)
    {
        OutputPath = output ?? Path.Combine(Path.GetTempPath(), OutputPath ?? "output.pdf");
        var paths = input.ToArray();
        if (paths.Length == 0)
            return Result.Failure<bool>($"PDF paths cannot be empty. ${nameof(input)}");

        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Result.Failure<bool>($"A file path is null or empty. ${nameof(input)}");

            var pdfFilePathsResult = FileUtilities.GetPdfFilePaths(path);
            if (pdfFilePathsResult.IsFailure)
                return Result.Failure<bool>(pdfFilePathsResult.Error!);

            var result = AddFileContentToPdf(DocumentWrapper, pdfFilePathsResult.Value!);
            if (result.IsFailure)
            {
                return Result.Failure<bool>(result.Error!);
            }
        }

        await DocumentWrapper.SaveAsync(OutputPath);
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
