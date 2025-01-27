namespace nmergi.Tests;

public class Utils
{
    public static string GetProjectRootDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        // Traverse upwards until we find the .csproj or .sln file
        while (
            !File.Exists(Path.Combine(currentDirectory, "nmergi.csproj"))
            && !File.Exists(Path.Combine(currentDirectory, "nmergi.sln"))
        )
        {
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;

            // If we've reached the root directory (e.g., C:\ or /), stop
            if (currentDirectory == null)
                throw new InvalidOperationException("Project root directory not found.");
        }

        return currentDirectory;
    }
}
