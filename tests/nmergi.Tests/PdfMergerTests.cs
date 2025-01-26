using Moq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace nmergi.Tests;

[TestFixture]
public class PdfMergerTests
{
    [Test]
    public void MergePdfs_ShouldMergeAndSavePdf()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var mockOutputDocument = new Mock<IPdfDocumentWrapper>();
        var mockPdfReader = new Mock<IPdfReader>();

        var inputDocument1 = new PdfDocument();
        inputDocument1.AddPage();
        var inputDocument2 = new PdfDocument();
        inputDocument2.AddPage();

        var pdfPaths = new[] { "path1" };
        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns(new List<string> { "path1/file1.pdf", "path1/file2.pdf" });
        mockFileUtilities
            .Setup(fu => fu.GetTempPdfFullFileName(It.IsAny<string>()))
            .Returns("output.pdf");

        mockPdfReader
            .SetupSequence(reader =>
                reader.Open(It.IsAny<string>(), It.IsAny<PdfDocumentOpenMode>())
            )
            .Returns(new PdfDocumentWrapper(inputDocument1))
            .Returns(new PdfDocumentWrapper(inputDocument2));

        var pdfMerger = new PdfMerger(
            mockOutputDocument.Object,
            mockPdfReader.Object,
            mockFileUtilities.Object
        );

        pdfMerger.MergePdfs(pdfPaths);

        mockOutputDocument.Verify(doc => doc.AddPage(It.IsAny<PdfPage>()), Times.Exactly(2));
        mockOutputDocument.Verify(doc => doc.Save("output.pdf"), Times.Once);
        mockFileUtilities.Verify(util => util.ShowDocument("output.pdf"), Times.Once);
    }

    [Test]
    public void MergePdfs_ShouldThrowException_WhenPdfPathsIsNull()
    {
        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            Mock.Of<IFileUtilities>()
        );

        var ex = Assert.Throws<ArgumentException>(() => pdfMerger.MergePdfs(null));
        Assert.That(ex.Message, Does.Contain("PDF paths cannot be null. (Parameter 'pdfPaths')"));
    }

    [Test]
    public void MergePdfs_ShouldThrowException_WhenPdfPathsIsEmpty()
    {
        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            Mock.Of<IFileUtilities>()
        );

        var ex = Assert.Throws<ArgumentException>(() => pdfMerger.MergePdfs([]));
        Assert.That(ex.Message, Does.Contain("PDF paths cannot be null or empty"));
    }

    [Test]
    public void MergePdfs_ShouldThrowException_WhenFilePathIsInvalid()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            mockFileUtilities.Object
        );

        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns(new List<string> { " " });

        var ex = Assert.Throws<ArgumentException>(() => pdfMerger.MergePdfs(["path1"]));
        Assert.That(ex.Message, Does.Contain("A file path is null or empty"));
    }

    [Test]
    public void MergePdfs_ShouldThrowException_WhenInputDocumentIsNull()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var mockPdfReader = new Mock<IPdfReader>();

        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns(new List<string> { "path1/file1.pdf" });

        mockPdfReader
            .Setup(reader => reader.Open(It.IsAny<string>(), It.IsAny<PdfDocumentOpenMode>()))
            .Returns((IPdfDocumentWrapper)null!);

        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            mockPdfReader.Object,
            mockFileUtilities.Object
        );

        var ex = Assert.Throws<InvalidOperationException>(() => pdfMerger.MergePdfs(["path1"]));
        Assert.That(ex.Message, Does.Contain("Input PDF document at 'path1/file1.pdf' is null"));
    }
}
