using PdfSharp.Quality;

namespace Defuse.Utilities;

public interface IFileUtilities
{
    string GetTempPdfFullFileName(string baseFileName);
    Result<IList<string>> GetPdfFilePaths(string path);
}

public class FileUtilities : IFileUtilities
{
    public string GetTempPdfFullFileName(string baseFileName)
    {
        return PdfFileUtility.GetTempPdfFullFileName("merged");
    }

    public Result<IList<string>> GetPdfFilePaths(string path)
    {
        var filePaths = new List<string>();

        if (File.Exists(path))
        {
            filePaths.Add(path);
            return filePaths;
        }
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, "*.pdf");
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    filePaths.Add(Path.GetFullPath(file));
                }
            }
            return filePaths;
        }

        return $"Invalid file or directory path: {path}";
    }
}
