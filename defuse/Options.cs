using CommandLine;
using CommandLine.Text;

namespace defuse;

public class Options
{
    [Option(
        'i',
        "input",
        Required = true,
        HelpText = "Paths to the input PDF files or directories containing PDFs to merge."
    )]
    public required IEnumerable<string> InputFiles { get; init; }

    [Usage(ApplicationAlias = "defuse")]
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<Example> Examples =>
        new List<Example>
        {
            new(
                "Merge PDFs from a list of files",
                new Options { InputFiles = ["file1.pdf", "file2.pdf"] }
            ),
            new("Merge PDFs from a directory", new Options { InputFiles = ["./pdfs"] }),
        };
}
