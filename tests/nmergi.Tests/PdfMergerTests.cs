using Moq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace nmergi.Tests;

public class PdfMergerTests
{
    [Test]
    public void MergePdfs_ShouldCallFileHandlerAndSavePdf()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var outputDocumentWrapper = new Mock<IPdfDocumentWrapper>();
        var inputDocument1 = new PdfDocument();
        inputDocument1.AddPage();
        var inputDocument2 = new PdfDocument();
        inputDocument2.AddPage();
        var pdfReader = new Mock<IPdfReader>();

        var pdfPaths = new[] { "path1" };
        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns<string>(path => new List<string> { $"{path}/file1.pdf", $"{path}/file2.pdf" });

        pdfReader
            .SetupSequence(reader =>
                reader.Open(It.IsAny<string>(), It.IsAny<PdfDocumentOpenMode>())
            )
            .Returns(new PdfDocumentWrapper(inputDocument1))
            .Returns(new PdfDocumentWrapper(inputDocument2));

        var pdfMerger = new PdfMerger(
            outputDocumentWrapper.Object,
            pdfReader.Object,
            mockFileUtilities.Object,
            "output.pdf"
        );

        pdfMerger.MergePdfs(pdfPaths);

        outputDocumentWrapper.Verify(odw => odw.AddPage(It.IsAny<PdfPage>()), Times.Exactly(2));
        outputDocumentWrapper.Verify(odw => odw.Save(It.IsAny<string>()), Times.Once);
        mockFileUtilities.Verify(fh => fh.ShowDocument(It.IsAny<string>()), Times.Once);
    }
}
