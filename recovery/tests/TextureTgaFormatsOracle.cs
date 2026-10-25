using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

// x86 CLR4 /unsafe, legacy v2 activation. Original LoadImageA is invoked through
// its managed IL entry; all original image storage uses that module's VC8 CRT.
internal static unsafe class TextureTgaFormatsOracle
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr MallocFunction(UIntPtr bytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void FreeFunction(IntPtr memory);
    private static MallocFunction malloc;
    private static FreeFunction free;
    private static MethodInfo construct, destroy, load;
    private static int fixtures, mismatches, assertions, originalFailures, rejectionMatches, decodedMatches;
    private static object PointerArg(MethodInfo method, int index, void* value)
    { return Pointer.Box(value, method.GetParameters()[index].ParameterType); }
    private static void Check(bool condition, string message)
    { assertions++; if (!condition) throw new InvalidOperationException(message); }
    private static void Bind(Assembly original, string input)
    {
        string hash;
        using (SHA256 sha = SHA256.Create())
            hash = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(input))).Replace("-", "").ToLowerInvariant();
        if (hash != "4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")
            throw new InvalidDataException("Original TextureCompiler changed; review allocator RVAs and managed method tokens.");
        IntPtr module = Marshal.GetHINSTANCE(original.ManifestModule);
        Check(module != IntPtr.Zero && module != new IntPtr(-1), "Native module handle");
        IntPtr m = IntPtr.Add(module, 0x4e74c), f = IntPtr.Add(module, 0x4e758);
        Check(Marshal.ReadByte(m) == 0xFF && Marshal.ReadByte(m, 1) == 0x25, "Original malloc thunk");
        Check(Marshal.ReadByte(f) == 0xFF && Marshal.ReadByte(f, 1) == 0x25, "Original free thunk");
        malloc = (MallocFunction)Marshal.GetDelegateForFunctionPointer(m, typeof(MallocFunction));
        free = (FreeFunction)Marshal.GetDelegateForFunctionPointer(f, typeof(FreeFunction));
        load = (MethodInfo)original.ManifestModule.ResolveMethod(0x06000017);
        construct = (MethodInfo)original.ManifestModule.ResolveMethod(0x0600002c);
        destroy = (MethodInfo)original.ManifestModule.ResolveMethod(0x0600002d);
        Check(load.Name == "BinaryAssetBuilder.TextureCompiler.LoadImageA", "Original LoadImageA token");
        Check(construct.Name == "nvImage<rgba_t>.{ctor}" && destroy.Name == "nvImage<rgba_t>.{dtor}", "Original nvImage tokens");
    }
    private static TexturePacker.Image Original(string path)
    {
        IntPtr image = malloc((UIntPtr)24u);
        if (image == IntPtr.Zero) throw new OutOfMemoryException();
        bool constructed = false;
        try
        {
            for (int i = 0; i < 24; i++) ((byte*)image)[i] = 0xA5;
            construct.Invoke(null, new object[] { PointerArg(construct, 0, image.ToPointer()) });
            constructed = true;
            bool hasAlpha = false;
            byte[] pathBytes = Encoding.Default.GetBytes(path + "\0");
            fixed (byte* pathPointer = pathBytes)
                load.Invoke(null, new object[] { PointerArg(load, 0, image.ToPointer()), PointerArg(load, 1, &hasAlpha), PointerArg(load, 2, pathPointer) });
            int width = ((int*)image)[0], height = ((int*)image)[1];
            Check(width > 0 && height > 0 && (long)width * height < 16777216L, "Original loaded dimensions");
            int count = checked(width * height);
            Check(((int*)image)[3] == count && ((int*)image)[4] >= count, "Original nvVector size/capacity");
            uint* data = (uint*)((IntPtr*)image)[2];
            Check(data != null, "Original loaded pixel pointer");
            uint[] pixels = new uint[count];
            for (int i = 0; i < count; i++) pixels[i] = data[i];
            return new TexturePacker.Image(width, height, pixels, hasAlpha);
        }
        finally
        {
            try
            {
                if (constructed) destroy.Invoke(null, new object[] { PointerArg(destroy, 0, image.ToPointer()) });
            }
            finally { free(image); }
        }
    }
    private static Exception Unwrap(Exception e)
    { while (e is TargetInvocationException && e.InnerException != null) e = e.InnerException; return e; }
    private static string Failure(Exception failure)
    {
        Exception e = Unwrap(failure); PropertyInfo code = e.GetType().GetProperty("ErrorCode");
        return e.GetType().FullName + ": " + e.Message + "; errorCode=" + (code == null ? "<none>" : Convert.ToString(code.GetValue(e, null)));
    }
    private static string Pixels(TexturePacker.Image image)
    {
        StringBuilder text = new StringBuilder();
        foreach (uint value in image.Pixels) { if (text.Length > 0) text.Append(','); text.Append(value.ToString("X8")); }
        return text.ToString();
    }
    private static uint ColorValue(int index, int alphaMode)
    {
        uint alpha = alphaMode == 0 ? 0u : alphaMode == 1 ? 255u : alphaMode == 2 ? (uint)((index & 1) * 255) : (uint)((index * 61 + 17) & 255);
        return (uint)((index * 47 + 13) & 255) | (uint)((index * 29 + 5) & 255) << 8 | (uint)((index * 17 + 79) & 255) << 16 | alpha << 24;
    }
    private static byte[] StoredColor(uint color, int bits)
    {
        if (bits == 15 || bits == 16)
        {
            int value = (int)((color & 255) >> 3) << 10 | (int)(((color >> 8) & 255) >> 3) << 5 |
                (int)(((color >> 16) & 255) >> 3) | (bits == 16 && (color >> 24) >= 128 ? 0x8000 : 0);
            return new byte[] { (byte)value, (byte)(value >> 8) };
        }
        return bits == 24 ? new byte[] { (byte)(color >> 16), (byte)(color >> 8), (byte)color } :
            new byte[] { (byte)(color >> 16), (byte)(color >> 8), (byte)color, (byte)(color >> 24) };
    }
    private static byte[] Fixture(int width, int height, int family, int depth, int paletteDepth, int first, int entries, bool rle, int direction, int attributes, int idLength, int alphaMode)
    {
        byte[] header = new byte[18]; header[0] = (byte)idLength; header[1] = (byte)(family == 1 ? 1 : 0); header[2] = (byte)(family + (rle ? 8 : 0));
        header[3] = (byte)first; header[4] = (byte)(first >> 8); header[5] = (byte)entries; header[6] = (byte)(entries >> 8); header[7] = (byte)paletteDepth;
        header[12] = (byte)width; header[13] = (byte)(width >> 8); header[14] = (byte)height; header[15] = (byte)(height >> 8); header[16] = (byte)depth; header[17] = (byte)(direction | attributes);
        byte[][] stored = new byte[width * height][];
        for (int position = 0; position < stored.Length; position++)
        {
            int fy = position / width, fx = position % width;
            int x = (direction & 16) != 0 ? width - 1 - fx : fx;
            int y = (direction & 32) != 0 ? fy : height - 1 - fy;
            int index = y * width + x, colorIndex = index / 2;
            uint color = ColorValue(colorIndex, alphaMode);
            if (family == 1)
            {
                int mapIndex = first + (colorIndex % entries);
                stored[position] = depth == 8 ? new byte[] { (byte)mapIndex } : new byte[] { (byte)mapIndex, (byte)(mapIndex >> 8) };
            }
            else if (family == 3)
                stored[position] = depth == 8 ? new byte[] { (byte)color } : new byte[] { (byte)color, (byte)(color >> 24) };
            else stored[position] = StoredColor(color, depth);
        }
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(header); for (int i = 0; i < idLength; i++) writer.Write((byte)(i * 17 + 23));
            if (family == 1) for (int i = 0; i < entries; i++) writer.Write(StoredColor(ColorValue(i, alphaMode), paletteDepth));
            for (int y = 0; y < height; y++)
            {
                if (!rle) for (int x = 0; x < width; x++) writer.Write(stored[y * width + x]);
                else
                {
                    int x = 0;
                    while (x < width)
                    {
                        byte[] value = stored[y * width + x]; int run = 1;
                        while (x + run < width && run < 128 && Equal(value, stored[y * width + x + run])) run++;
                        if (run > 1) { writer.Write((byte)(128 | run - 1)); writer.Write(value); x += run; }
                        else { writer.Write((byte)0); writer.Write(value); x++; }
                    }
                }
            }
            writer.Flush(); return stream.ToArray();
        }
    }
    private static byte[] Every16BitPattern(int direction, int attributes)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            byte[] header = new byte[18]; header[2] = 2;
            header[13] = 1; header[15] = 1; header[16] = 16; header[17] = (byte)(direction | attributes);
            writer.Write(header);
            for (int fy = 0; fy < 256; fy++) for (int fx = 0; fx < 256; fx++)
            {
                int x = (direction & 16) != 0 ? 255 - fx : fx;
                int y = (direction & 32) != 0 ? fy : 255 - fy;
                writer.Write((ushort)(y * 256 + x));
            }
            writer.Flush(); return stream.ToArray();
        }
    }
    private static byte[] EveryGrayValue(int depth, int direction, int attributes)
    {
        int width = 256, height = depth == 8 ? 1 : 256;
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            byte[] header = new byte[18]; header[2] = 3; header[13] = 1;
            header[14] = (byte)height; header[15] = (byte)(height >> 8); header[16] = (byte)depth; header[17] = (byte)(direction | attributes);
            writer.Write(header);
            for (int fy = 0; fy < height; fy++) for (int fx = 0; fx < width; fx++)
            {
                int x = (direction & 16) != 0 ? width - 1 - fx : fx;
                int y = (direction & 32) != 0 ? fy : height - 1 - fy;
                int word = y * width + x;
                writer.Write((byte)word); if (depth == 16) writer.Write((byte)(word >> 8));
            }
            writer.Flush(); return stream.ToArray();
        }
    }
    private static byte[] ExhaustivePalette(int paletteDepth, int block)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            byte[] header = new byte[18]; header[1] = 1; header[2] = 1;
            header[6] = 1; header[7] = (byte)paletteDepth; header[13] = 1; header[14] = 1; header[16] = 8;
            writer.Write(header);
            for (int i = 0; i < 256; i++)
            {
                if (paletteDepth == 15 || paletteDepth == 16) writer.Write((ushort)(block * 256 + i));
                else
                {
                    uint color = (uint)i | (uint)(255 - i) << 8 | (uint)((i * 73) & 255) << 16 | (uint)i << 24;
                    writer.Write(StoredColor(color, paletteDepth));
                }
            }
            for (int i = 0; i < 256; i++) writer.Write((byte)i);
            writer.Flush(); return stream.ToArray();
        }
    }
    private static bool Equal(byte[] a, byte[] b)
    { if (a.Length != b.Length) return false; for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false; return true; }
    private static void Compare(StreamWriter transcript, string path, string label, byte[] bytes, bool probe)
    {
        File.WriteAllBytes(path, bytes); fixtures++;
        TexturePacker.Image original = null, candidate = null; Exception originalError = null, candidateError = null;
        try { original = Original(path); } catch (Exception ex) { originalError = ex; }
        try { candidate = TextureTgaAdditionalFormats.Decode(bytes); } catch (Exception ex) { candidateError = ex; }
        if (originalError != null)
        {
            originalFailures++; string expected = Failure(originalError), actual = candidateError == null ? "<success>" : Failure(candidateError);
            bool same = expected == actual;
            transcript.WriteLine("{0}|negativeMatched={1}|originalFailure={2}|candidateFailure={3}", label, same, expected, actual);
            if (same) { rejectionMatches++; return; }
        }
        else if (candidateError == null)
        {
            bool same = original.Width == candidate.Width && original.Height == candidate.Height && original.HasAlpha == candidate.HasAlpha && Pixels(original) == Pixels(candidate);
            transcript.WriteLine("{0}|equal={1}|original={2}x{3},alpha={4}|recovered={5}x{6},alpha={7}|originalPixels={8}|recoveredPixels={9}", label, same, original.Width, original.Height, original.HasAlpha, candidate.Width, candidate.Height, candidate.HasAlpha, Pixels(original), Pixels(candidate));
            if (same) { decodedMatches++; return; }
        }
        else transcript.WriteLine("{0}|candidateFailure={1}|original={2}x{3},alpha={4}|originalPixels={5}", label, Failure(candidateError), original.Width, original.Height, original.HasAlpha, Pixels(original));
        mismatches++;
        if (!probe) { transcript.Flush(); throw new InvalidDataException("Additional TGA format differs: " + label); }
    }
    private static void Case(StreamWriter transcript, string directory, bool probe, int width, int height, int family, int depth, int paletteDepth, int first, int entries, bool rle, int direction, int attributes, int id, int alpha)
    {
        string label = "family" + family + "-" + width + "x" + height + "-depth" + depth + "-palette" + paletteDepth + "-first" + first + "-entries" + entries + "-rle" + rle + "-direction" + direction + "-attributes" + attributes + "-id" + id + "-alpha" + alpha;
        Compare(transcript, Path.Combine(directory, "current.tga"), label, Fixture(width, height, family, depth, paletteDepth, first, entries, rle, direction, attributes, id, alpha), probe);
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 4 || (args[3] != "--probe" && args[3] != "--strict")) throw new ArgumentException("Usage: TextureTgaFormatsOracle original.dll transcript.txt fixtures-dir --probe|--strict");
            Check(IntPtr.Size == 4, "x86 host"); bool probe = args[3] == "--probe";
            string input = Path.GetFullPath(args[0]), directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) { string candidate = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll"); return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null; };
            Assembly assembly = Assembly.LoadFrom(input); Bind(assembly, input);
            Type type = assembly.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true); object plugin = Activator.CreateInstance(type);
            MethodInfo initialize = type.GetMethod("Initialize"); initialize.Invoke(plugin, new object[] { null, Enum.ToObject(initialize.GetParameters()[1].ParameterType, 0) });
            string fixturesDirectory = Path.GetFullPath(args[2]); Directory.CreateDirectory(fixturesDirectory);
            try
            {
                using (StreamWriter transcript = new StreamWriter(args[1], false, Encoding.UTF8))
                {
                    foreach (int size in new int[] { 0, 1, 2 })
                    {
                        int width = size == 0 ? 1 : size == 1 ? 4 : 8, height = size == 0 ? 1 : size == 1 ? 2 : 4;
                        foreach (bool rle in new bool[] { false, true }) foreach (int direction in new int[] { 0, 16, 32, 48 })
                        foreach (int alpha in new int[] { 0, 1, 2, 3 }) foreach (int id in new int[] { 0, 7 })
                        {
                            foreach (int attribute in new int[] { 0, 1 }) Case(transcript, fixturesDirectory, probe, width, height, 2, 16, 0, 0, 0, rle, direction, attribute, id, alpha);
                            Case(transcript, fixturesDirectory, probe, width, height, 3, 8, 0, 0, 0, rle, direction, 0, id, alpha);
                            foreach (int attribute in new int[] { 0, 8 }) Case(transcript, fixturesDirectory, probe, width, height, 3, 16, 0, 0, 0, rle, direction, attribute, id, alpha);
                            foreach (int paletteDepth in new int[] { 15, 16, 24, 32 }) foreach (int indexDepth in new int[] { 8, 16 }) foreach (int first in new int[] { 0, 5 })
                                Case(transcript, fixturesDirectory, probe, width, height, 1, indexDepth, paletteDepth, first, 4, rle, direction, 0, id, alpha);
                        }
                    }
                    foreach (int direction in new int[] { 0, 16, 32, 48 })
                    foreach (int attributes in new int[] { 0, 1 })
                        Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "every16bit-pattern-direction" + direction + "-attributes" + attributes, Every16BitPattern(direction, attributes), probe);
                    foreach (int direction in new int[] { 0, 16, 32, 48 })
                    {
                        Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "every-gray8-direction" + direction, EveryGrayValue(8, direction, 0), probe);
                        foreach (int attributes in new int[] { 0, 8 })
                            Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "every-gray16-direction" + direction + "-attributes" + attributes, EveryGrayValue(16, direction, attributes), probe);
                    }
                    foreach (int paletteDepth in new int[] { 15, 16, 24, 32 })
                    {
                        int blocks = paletteDepth == 15 ? 128 : paletteDepth == 16 ? 256 : 1;
                        for (int block = 0; block < blocks; block++)
                            Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "exhaustive-palette" + paletteDepth + "-block" + block, ExhaustivePalette(paletteDepth, block), probe);
                        foreach (int edge in new int[] { 0, 1, 2 })
                        foreach (bool rle in new bool[] { false, true }) foreach (int direction in new int[] { 0, 16, 32, 48 })
                            Case(transcript, fixturesDirectory, probe, 8, 4, 1, 8, paletteDepth, edge == 0 ? 250 : edge == 1 ? 253 : 255, edge == 0 ? 4 : edge == 1 ? 3 : 1, rle, direction, 0, 0, 3);
                    }
                }
            }
            finally { ((IDisposable)plugin).Dispose(); }
            Check(fixtures == 4534, "Full new-format matrix count");
            Console.WriteLine((probe ? "PROBE" : "PASS") + " Texture TGA additional formats: fixtures=" + fixtures + "; decodedMatches=" + decodedMatches + "; rejectionMatches=" + rejectionMatches + "; originalFailures=" + originalFailures + "; mismatches=" + mismatches + "; no old-format rerun; no SDK claim");
            Check(probe || mismatches == 0, "Strict differences");
            if (!probe)
            {
                Check(decodedMatches == 2606 && rejectionMatches == 1928 && originalFailures == 1928, "Exact accepted format/negative accounting");
            }
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
