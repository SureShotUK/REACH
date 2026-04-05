using OpenMcdf;
using System.Text;

// Outlook Template Cleaner (CFBF Direct Stream Modification)
// Removes soft hyphens (U+00AD) from .oft files by directly modifying CFBF streams

if (args.Length == 0)
{
    Console.WriteLine("Outlook Template Cleaner - CFBF Edition");
    Console.WriteLine("========================================");
    Console.WriteLine();
    Console.WriteLine("Usage: OutlookTemplateCleaner <path-to-oft-file> [--inspect]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --inspect    Inspect file structure without modifying (default: clean)");
    Console.WriteLine("  --scan       Comprehensive scan for soft hyphens in ALL streams/formats");
    Console.WriteLine();
    Console.WriteLine("Example:");
    Console.WriteLine("  OutlookTemplateCleaner.exe Template.oft");
    Console.WriteLine("  OutlookTemplateCleaner.exe Template.oft --inspect");
    Console.WriteLine("  OutlookTemplateCleaner.exe Template.oft --scan");
    return 1;
}

string oftFilePath = args[0];
string mode = args.Length > 1 ? args[1] : "";

if (!File.Exists(oftFilePath))
{
    Console.WriteLine($"ERROR: File not found: {oftFilePath}");
    return 1;
}

Console.WriteLine($"Processing: {oftFilePath}");
Console.WriteLine($"Mode: {(mode == "--inspect" ? "INSPECT ONLY" : mode == "--scan" ? "COMPREHENSIVE SCAN" : "CLEAN")}");
Console.WriteLine(new string('=', 80));

try
{
    if (mode == "--scan")
    {
        StreamScanner.ScanAllStreams(oftFilePath);
        return 0;
    }
    else if (mode == "--extract")
    {
        // Extract text body stream for debugging
        using var cf = new CompoundFile(oftFilePath);
        var stream = cf.RootStorage.GetStream("__substg1.0_1000001F");
        byte[] data = stream.GetData();

        string outputFile = oftFilePath + ".text_stream.bin";
        File.WriteAllBytes(outputFile, data);
        Console.WriteLine($"\nExtracted {data.Length} bytes to: {outputFile}");

        // Check for soft hyphens
        int count = 0;
        for (int i = 0; i < data.Length - 1; i++)
        {
            if (data[i] == 0xAD && data[i + 1] == 0x00)
            {
                count++;
                Console.WriteLine($"Soft hyphen found at byte offset {i}");
            }
        }
        Console.WriteLine($"\nTotal soft hyphens in extracted stream: {count}");

        // Also check RTF stream
        Console.WriteLine("\n--- Checking RTF Stream ---");
        ExtractRTFStream(oftFilePath);

        return 0;
    }
    else if (mode == "--inspect")
    {
        return InspectFile(oftFilePath);
    }
    else
    {
        return CleanFile(oftFilePath);
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nERROR: {ex.Message}");
    Console.ResetColor();
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    return 1;
}

// Clean soft hyphens from the OFT file
int CleanFile(string filePath)
{
    Console.WriteLine("\nCreating backup...");
    string backupPath = filePath + ".backup";
    File.Copy(filePath, backupPath, overwrite: true);
    Console.WriteLine($"Backup created: {backupPath}");

    Console.WriteLine("\nOpening CFBF file...");
    var cf = new CompoundFile(filePath, CFSUpdateMode.Update, CFSConfiguration.SectorRecycle);

    int totalRemoved = 0;
    bool modified = false;

    // Process plain text body stream (__substg1.0_1000001F)
    string textBodyStreamName = "__substg1.0_1000001F";
    if (StreamExists(cf.RootStorage, textBodyStreamName))
    {
        Console.WriteLine($"\nProcessing stream: {textBodyStreamName}");
        var stream = cf.RootStorage.GetStream(textBodyStreamName);
        byte[] data = stream.GetData();

        string originalText = Encoding.Unicode.GetString(data);
        int originalCount = originalText.Count(c => c == '\u00AD');

        Console.WriteLine($"  Original text length: {originalText.Length} characters");
        Console.WriteLine($"  Soft hyphens found: {originalCount}");

        if (originalCount > 0)
        {
            string cleanedText = originalText.Replace("\u00AD", "");
            byte[] cleanedData = Encoding.Unicode.GetBytes(cleanedText);

            stream.SetData(cleanedData);
            totalRemoved += originalCount;
            modified = true;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ Removed {originalCount} soft hyphen(s)");
            Console.ResetColor();
            Console.WriteLine($"  New text length: {cleanedText.Length} characters");
        }
        else
        {
            Console.WriteLine("  No soft hyphens found in this stream");
        }
    }
    else
    {
        Console.WriteLine($"\nStream {textBodyStreamName} not found (this may be normal for some templates)");
    }

    // Save changes
    if (modified)
    {
        Console.WriteLine("\nCommitting and saving changes to CFBF file...");
        cf.Commit();
        cf.Close();  // Explicitly close to flush changes to disk

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✓ SUCCESS: Removed {totalRemoved} soft hyphen(s) from {Path.GetFileName(filePath)}");
        Console.ResetColor();
        Console.WriteLine($"\nBackup saved to: {backupPath}");
        Console.WriteLine("If the cleaned template works correctly, you can delete the backup.");
    }
    else
    {
        Console.WriteLine("\n✓ No modifications needed - file is already clean");
        cf.Close();  // Close without saving

        // Delete backup since we didn't modify anything
        File.Delete(backupPath);
        Console.WriteLine("Backup deleted (not needed)");
    }

    return 0;
}

// Inspect the OFT file structure
int InspectFile(string filePath)
{
    using var cf = new CompoundFile(filePath);

    Console.WriteLine($"\nRoot Storage: {cf.RootStorage.Name}");
    Console.WriteLine("\nStreams and Storages:");
    Console.WriteLine(new string('-', 80));

    // Visit all entries
    cf.RootStorage.VisitEntries(item =>
    {
        if (item.IsStream)
        {
            Console.WriteLine($"[STREAM] {item.Name} ({item.Size} bytes)");

            // Check text body streams for soft hyphens
            if (item.Name == "__substg1.0_1000001F" || // Plain text body (Unicode)
                item.Name == "__substg1.0_1000001E" || // Plain text body (ANSI)
                item.Name == "__substg1.0_1013001F" || // HTML body (Unicode)
                item.Name == "__substg1.0_1013001E")   // HTML body (ANSI)
            {
                var stream = cf.RootStorage.GetStream(item.Name);
                InspectTextStream(stream, "  ");
            }
        }
        else if (item.IsStorage)
        {
            Console.WriteLine($"[STORAGE] {item.Name}");
        }
    }, false);

    Console.WriteLine(new string('-', 80));
    return 0;
}

// Check if a stream exists
bool StreamExists(CFStorage storage, string streamName)
{
    try
    {
        storage.GetStream(streamName);
        return true;
    }
    catch
    {
        return false;
    }
}

// Inspect a text stream for soft hyphens
void InspectTextStream(CFStream stream, string indent)
{
    try
    {
        byte[] data = stream.GetData();

        if (data.Length == 0)
        {
            Console.WriteLine($"{indent}-> Empty stream");
            return;
        }

        // Check Unicode (UTF-16 LE) - most common for Outlook
        if (data.Length >= 2 && data.Length % 2 == 0)
        {
            string unicodeText = Encoding.Unicode.GetString(data);
            int softHyphenCount = unicodeText.Count(c => c == '\u00AD');

            if (softHyphenCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{indent}-> Contains {softHyphenCount} soft hyphen(s) (U+00AD)");
                Console.ResetColor();

                // Show a preview (first 150 chars)
                string preview = unicodeText.Length > 150 ? unicodeText[..150] + "..." : unicodeText;
                preview = preview.Replace('\r', ' ').Replace('\n', ' ');

                // Highlight soft hyphens in preview
                preview = preview.Replace("\u00AD", "[­]");

                Console.WriteLine($"{indent}-> Preview: {preview}");
            }
            else
            {
                Console.WriteLine($"{indent}-> Unicode text, no soft hyphens detected");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{indent}-> Error reading stream: {ex.Message}");
    }
}

// Add RTF stream extraction at the end of the file
static void ExtractRTFStream(string oftFile)
{
    using var cf = new CompoundFile(oftFile);
    try
    {
        var rtfStream = cf.RootStorage.GetStream("__substg1.0_10090102");
        byte[] data = rtfStream.GetData();

        // Search for RTF soft hyphen code (\~) in the data
        string ascii = Encoding.ASCII.GetString(data);
        int count = 0;
        int idx = 0;
        while ((idx = ascii.IndexOf("\\~", idx)) != -1)
        {
            Console.WriteLine($"RTF soft hyphen code found at byte offset {idx}");
            count++;
            idx += 2;
        }

        Console.WriteLine($"Total RTF soft hyphen codes found: {count}");

        // Also check for raw UTF-16 soft hyphens in the compressed data
        int rawCount = 0;
        for (int i = 0; i < data.Length - 1; i++)
        {
            if (data[i] == 0xAD && data[i + 1] == 0x00)
            {
                Console.WriteLine($"Raw UTF-16 soft hyphen found at RTF byte offset {i}");
                rawCount++;
                i++; // Skip next byte
            }
        }
        Console.WriteLine($"Total raw UTF-16 soft hyphens in RTF stream: {rawCount}");

        // Save RTF stream for inspection
        string rtfOutput = oftFile + ".rtf_stream.bin";
        File.WriteAllBytes(rtfOutput, data);
        Console.WriteLine($"RTF stream saved to: {rtfOutput}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading RTF stream: {ex.Message}");
    }
}
