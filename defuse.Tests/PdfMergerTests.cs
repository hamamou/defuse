using Moq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace defuse.Tests;

[TestFixture]
public class PdfMergerTests
{
    [Test]
    public void MergePdfs_ShouldMergeAndSavePdf()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var mockDocumentWrapper = new Mock<IPdfDocumentWrapper>();
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
            mockDocumentWrapper.Object,
            mockPdfReader.Object,
            mockFileUtilities.Object,
            "output"
        );

        var result = pdfMerger.MergePdfs(pdfPaths);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.EqualTo(true));
            mockDocumentWrapper.Verify(doc => doc.AddPage(It.IsAny<PdfPage>()), Times.Exactly(2));
            mockDocumentWrapper.Verify(doc => doc.Save(It.IsAny<string>()), Times.Once);
            mockDocumentWrapper.Verify(doc => doc.ShowDocument(It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public void MergePdfs_ShouldReturnError_WhenPdfPathsIsNull()
    {
        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            Mock.Of<IFileUtilities>(),
            "output"
        );
        var result = pdfMerger.MergePdfs(null);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.EqualTo(true));
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error!.Message, Does.Contain("PDF paths cannot be null. $pdfPaths"));
        });
    }

    [Test]
    public void MergePdfs_ShouldReturnError_WhenPdfPathsIsEmpty()
    {
        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            Mock.Of<IFileUtilities>(),
            "output"
        );
        var result = pdfMerger.MergePdfs([]);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.EqualTo(true));
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(
                result.Error!.Message,
                Does.Contain("PDF paths cannot be empty. $pdfPaths")
            );
        });
    }

    [Test]
    public void MergePdfs_ShouldReturnError_WhenFilePathIsInvalid()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            mockFileUtilities.Object,
            "output"
        );

        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns(new List<string> { " " });

        var result = pdfMerger.MergePdfs(["path1"]);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.EqualTo(true));
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error!.Message, Does.Contain("A file path is null or empty"));
        });
    }

    [Test]
    public void MergePdfs_ShouldReturnError_WhenInputDocumentIsNull()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var mockPdfReader = new Mock<IPdfReader>();

        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns(new List<string> { "path1/file1.pdf" });

        mockPdfReader
            .Setup(reader => reader.Open(It.IsAny<string>(), It.IsAny<PdfDocumentOpenMode>()))
            .Returns(Bogoware.Monads.Result.Failure<IPdfDocumentWrapper>("message"));

        var pdfMerger = new PdfMerger(
            Mock.Of<IPdfDocumentWrapper>(),
            mockPdfReader.Object,
            mockFileUtilities.Object,
            "output"
        );

        var result = pdfMerger.MergePdfs(["path1"]);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.EqualTo(true));
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error!.Message, Does.Contain("message"));
        });
    }
}
