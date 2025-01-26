using PdfSharp.Quality;

namespace nmergi;

public static class FileUtilities
{
    public static string GetTempPdfFullFileName(string baseFileName)
    {
        return PdfFileUtility.GetTempPdfFullFileName("merged");
    }

    public static void ShowDocument(string filePath)
    {
        Console.WriteLine($"Document opened: {filePath}");
        PdfFileUtility.ShowDocument(filePath);
    }
}
