using PdfSharp.Quality;

namespace nmergi;

public interface IFileUtilities
{
    string GetTempPdfFullFileName(string baseFileName);
    void ShowDocument(string filePath);
    IList<string> GetPdfFilePaths(string path);
}

public class FileUtilities : IFileUtilities
{
    public string GetTempPdfFullFileName(string baseFileName)
    {
        return PdfFileUtility.GetTempPdfFullFileName("merged");
    }

    public void ShowDocument(string filePath)
    {
        Console.WriteLine($"Document opened: {filePath}");
        PdfFileUtility.ShowDocument(filePath);
    }

    public IList<string> GetPdfFilePaths(string path)
    {
        var filePaths = new List<string>();

        if (File.Exists(path))
        {
            filePaths.Add(path);
        }
        else if (Directory.Exists(path))
        {
            filePaths.AddRange(Directory.GetFiles(path, "*.pdf"));
        }
        else
        {
            Console.WriteLine($"Invalid file or directory path: {path}");
        }

        return filePaths;
    }
}
