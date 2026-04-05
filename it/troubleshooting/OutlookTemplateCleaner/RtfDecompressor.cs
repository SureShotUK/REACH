using System.Text;
using System.IO.Compression;

public static class RtfDecompressor
{
    // Decompress Outlook's compressed RTF format
    // Based on MS-OXRTFCP specification
    public static string Decompress(byte[] compressedRtf)
    {
        if (compressedRtf == null || compressedRtf.Length < 16)
        {
            return string.Empty;
        }

        // Check magic number (should be 0x414C454D for "MELA" in little-endian)
        int magic = BitConverter.ToInt32(compressedRtf, 4);
        if (magic != 0x414C454D && magic != 0x75465A4C) // MELA or LZFu
        {
            // Not compressed, might be plain RTF
            return Encoding.ASCII.GetString(compressedRtf);
        }

        try
        {
            using var input = new MemoryStream(compressedRtf);
            using var output = new MemoryStream();

            input.Seek(16, SeekOrigin.Begin); // Skip header

            while (input.Position < input.Length)
            {
                int controlByte = input.ReadByte();
                if (controlByte == -1) break;

                for (int bit = 0; bit < 8 && input.Position < input.Length; bit++)
                {
                    bool isRef = ((controlByte >> bit) & 1) == 1;

                    if (isRef)
                    {
                        // Reference to dictionary
                        int b1 = input.ReadByte();
                        int b2 = input.ReadByte();
                        if (b1 == -1 || b2 == -1) break;

                        int offset = ((b1 << 8) | b2) >> 4;
                        int length = (b2 & 0x0F) + 2;

                        long dictPos = output.Length % 4096;
                        for (int i = 0; i < length; i++)
                        {
                            long readPos = (dictPos + offset) % 4096;
                            output.Seek(output.Length - (dictPos - readPos), SeekOrigin.Begin);
                            int b = output.ReadByte();
                            output.Seek(0, SeekOrigin.End);
                            output.WriteByte((byte)b);
                            dictPos = (dictPos + 1) % 4096;
                        }
                    }
                    else
                    {
                        // Literal byte
                        int b = input.ReadByte();
                        if (b == -1) break;
                        output.WriteByte((byte)b);
                    }
                }
            }

            return Encoding.ASCII.GetString(output.ToArray());
        }
        catch
        {
            // Decompression failed, try as plain RTF
            return Encoding.ASCII.GetString(compressedRtf);
        }
    }
}
