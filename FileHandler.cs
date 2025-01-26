namespace nmergi;

public static class FileHandler
{
    public static IList<string> GetPdfFilePaths(string path)
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
