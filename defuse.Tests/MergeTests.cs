using Defuse.Commands;
using Defuse.pdf;
using Defuse.Utilities;
using Moq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace defuse.Tests;

[TestFixture]
public class MergeTests
{
    [Test]
    public async Task MergePdfs_ShouldMergeAndSavePdf()
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

        var pdfMerger = new Merge(
            mockDocumentWrapper.Object,
            mockPdfReader.Object,
            mockFileUtilities.Object
        );

        var result = await pdfMerger.MergePdfs(pdfPaths);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.EqualTo(true));
            mockDocumentWrapper.Verify(doc => doc.AddPage(It.IsAny<PdfPage>()), Times.Exactly(2));
            mockDocumentWrapper.Verify(doc => doc.SaveAsync(It.IsAny<string>()), Times.Once);
            mockDocumentWrapper.Verify(doc => doc.ShowDocument(It.IsAny<string>()), Times.Once);
        });
    }

    [Test]
    public async Task MergePdfs_ShouldReturnError_WhenPdfPathsIsEmpty()
    {
        var pdfMerger = new Merge(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            Mock.Of<IFileUtilities>()
        );
        var result = await pdfMerger.MergePdfs([]);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.EqualTo(true));
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error, Does.Contain("PDF paths cannot be empty. $input"));
        });
    }

    [Test]
    public async Task MergePdfs_ShouldReturnError_WhenFilePathIsInvalid()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var pdfMerger = new Merge(
            Mock.Of<IPdfDocumentWrapper>(),
            Mock.Of<IPdfReader>(),
            mockFileUtilities.Object
        );

        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns(new List<string> { " " });

        var result = await pdfMerger.MergePdfs(["path1"]);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.EqualTo(true));
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error, Does.Contain("A file path is null or empty"));
        });
    }

    [Test]
    public async Task MergePdfs_ShouldReturnError_WhenInputDocumentIsNull()
    {
        var mockFileUtilities = new Mock<IFileUtilities>();
        var mockPdfReader = new Mock<IPdfReader>();

        mockFileUtilities
            .Setup(fh => fh.GetPdfFilePaths(It.IsAny<string>()))
            .Returns(new List<string> { "path1/file1.pdf" });

        mockPdfReader
            .Setup(reader => reader.Open(It.IsAny<string>(), It.IsAny<PdfDocumentOpenMode>()))
            .Returns(Defuse.Monads.Result.Failure<IPdfDocumentWrapper>("message")!);

        var pdfMerger = new Merge(
            Mock.Of<IPdfDocumentWrapper>(),
            mockPdfReader.Object,
            mockFileUtilities.Object
        );

        var result = await pdfMerger.MergePdfs(["path1"]);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.EqualTo(true));
            Assert.That(result.Error, Is.Not.Null);
            Assert.That(result.Error, Does.Contain("message"));
        });
    }
}
