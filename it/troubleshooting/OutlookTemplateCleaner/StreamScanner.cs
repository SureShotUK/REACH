using OpenMcdf;
using System.Text;

public static class StreamScanner
{
    public static void ScanAllStreams(string oftFilePath)
    {
        using var cf = new CompoundFile(oftFilePath);

        Console.WriteLine("Scanning ALL streams for soft hyphens in any format...");
        Console.WriteLine(new string('=', 80));

        cf.RootStorage.VisitEntries(item =>
        {
            if (item.IsStream)
            {
                var stream = cf.RootStorage.GetStream(item.Name);
                byte[] data = stream.GetData();

                if (data.Length == 0) return;

                bool found = false;
                var results = new List<string>();

                // Check 1: UTF-16 LE (most common for Outlook)
                if (data.Length % 2 == 0)
                {
                    string unicode = Encoding.Unicode.GetString(data);
                    int count = unicode.Count(c => c == '\u00AD');
                    if (count > 0)
                    {
                        results.Add($"UTF-16 Unicode: {count} soft hyphen(s)");
                        found = true;
                    }
                }

                // Check 2: UTF-8
                try
                {
                    string utf8 = Encoding.UTF8.GetString(data);
                    int count = utf8.Count(c => c == '\u00AD');
                    if (count > 0)
                    {
                        results.Add($"UTF-8: {count} soft hyphen(s)");
                        found = true;
                    }
                }
                catch { }

                // Check 3: ASCII/ANSI text
                string ascii = Encoding.ASCII.GetString(data);

                // Check for HTML entities
                if (ascii.Contains("&shy;") || ascii.Contains("&#173;") || ascii.Contains("&#xAD;"))
                {
                    int shyCount = CountOccurrences(ascii, "&shy;");
                    int dec173Count = CountOccurrences(ascii, "&#173;");
                    int hexADCount = CountOccurrences(ascii, "&#xAD;");
                    int total = shyCount + dec173Count + hexADCount;

                    if (total > 0)
                    {
                        results.Add($"HTML entities: {total} (&shy;={shyCount}, &#173;={dec173Count}, &#xAD;={hexADCount})");
                        found = true;
                    }
                }

                // Check for RTF soft hyphen code (\~)
                if (ascii.Contains("\\~"))
                {
                    int count = CountOccurrences(ascii, "\\~");
                    results.Add($"RTF code (\\~): {count} occurrence(s)");
                    found = true;
                }

                // Check for raw byte pattern (0xAD 0x00 for UTF-16 LE)
                int bytePattern = 0;
                for (int i = 0; i < data.Length - 1; i++)
                {
                    if (data[i] == 0xAD && data[i + 1] == 0x00)
                    {
                        bytePattern++;
                        i++; // Skip next byte
                    }
                }
                if (bytePattern > 0 && !results.Any(r => r.Contains("UTF-16")))
                {
                    results.Add($"Raw byte pattern (AD 00): {bytePattern} occurrence(s)");
                    found = true;
                }

                if (found)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n[STREAM] {item.Name} ({item.Size} bytes)");
                    Console.ResetColor();

                    foreach (var result in results)
                    {
                        Console.WriteLine($"  ✓ {result}");
                    }

                    // Show preview
                    if (ascii.Length > 200)
                    {
                        string preview = ascii[..200].Replace('\r', ' ').Replace('\n', ' ');
                        Console.WriteLine($"  Preview: {preview}...");
                    }
                }
            }
        }, false);

        Console.WriteLine(new string('=', 80));
    }

    private static int CountOccurrences(string text, string pattern)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(pattern, index)) != -1)
        {
            count++;
            index += pattern.Length;
        }
        return count;
    }
}
