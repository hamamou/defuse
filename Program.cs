namespace nmergi;

public static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: nmergi <pdf1> <pdf2> ... <pdfN>");
            return;
        }
        var pdfMerger = new PdfMerger();
        pdfMerger.MergePdfs(args);

        Console.WriteLine($"Merged PDF saved as: {pdfMerger.OutputFileName}");
    }
}
