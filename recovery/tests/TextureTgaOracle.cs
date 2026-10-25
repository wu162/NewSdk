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
internal static unsafe class TextureTgaOracle
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr MallocFunction(UIntPtr bytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void FreeFunction(IntPtr memory);
    private static MallocFunction malloc;
    private static FreeFunction free;
    private static MethodInfo construct, destroy, load;
    private static int fixtures, mismatches, assertions, unsupportedCount, originalFailures;
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
    private static uint Pixel(int index, int alphaMode)
    {
        if (alphaMode == 4) return 0x7F2B7913u;
        uint alpha = alphaMode == 0 ? 0u : alphaMode == 1 ? 255u : alphaMode == 2 ? (uint)((index & 1) * 255) : (uint)((index * 61 + 17) & 255);
        // Repeated groups force RLE runs, with distinct rows/channels to detect
        // orientation, channel order and packet-boundary errors independently.
        int color = index / 3;
        return (uint)((color * 47 + 13) & 255) | (uint)((color * 29 + 5) & 255) << 8 |
            (uint)((color * 17 + 79) & 255) << 16 | alpha << 24;
    }
    private static void WritePixel(BinaryWriter writer, uint value, int depth)
    {
        writer.Write((byte)(value >> 16)); writer.Write((byte)(value >> 8)); writer.Write((byte)value);
        if (depth == 32) writer.Write((byte)(value >> 24));
    }
    private static byte[] Fixture(int width, int height, int depth, bool rle, int descriptor, int idLength, int alphaMode)
    {
        uint[] filePixels = new uint[width * height];
        for (int position = 0; position < filePixels.Length; position++)
        {
            int fy = position / width, fx = position % width;
            int x = (descriptor & 16) != 0 ? width - 1 - fx : fx;
            int y = (descriptor & 32) != 0 ? fy : height - 1 - fy;
            filePixels[position] = Pixel(y * width + x, alphaMode);
        }
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            byte[] header = new byte[18]; header[0] = (byte)idLength; header[2] = (byte)(rle ? 10 : 2);
            header[12] = (byte)width; header[13] = (byte)(width >> 8); header[14] = (byte)height; header[15] = (byte)(height >> 8);
            header[16] = (byte)depth; header[17] = (byte)descriptor; writer.Write(header);
            for (int i = 0; i < idLength; i++) writer.Write((byte)(i * 17 + 23));
            if (!rle) foreach (uint value in filePixels) WritePixel(writer, value, depth);
            else
            {
                int position = 0;
                while (position < filePixels.Length)
                {
                    int repeated = 1;
                    while (position + repeated < filePixels.Length && repeated < 128 &&
                        (depth == 32 ? filePixels[position + repeated] == filePixels[position] : (filePixels[position + repeated] & 0xFFFFFFu) == (filePixels[position] & 0xFFFFFFu))) repeated++;
                    if (repeated > 1)
                    {
                        writer.Write((byte)(128 | (repeated - 1))); WritePixel(writer, filePixels[position], depth); position += repeated;
                    }
                    else
                    {
                        // A one-pixel raw packet also probes alternating raw/run packets.
                        writer.Write((byte)0); WritePixel(writer, filePixels[position++], depth);
                    }
                }
            }
            writer.Flush(); return stream.ToArray();
        }
    }
    private static string Pixels(TexturePacker.Image image)
    {
        StringBuilder b = new StringBuilder();
        foreach (uint value in image.Pixels) { if (b.Length > 0) b.Append(','); b.Append(value.ToString("X8")); }
        return b.ToString();
    }
    private static void Compare(StreamWriter transcript, string path, string label, byte[] bytes, bool probe)
    {
        File.WriteAllBytes(path, bytes);
        fixtures++;
        TexturePacker.Image expected;
        try { expected = Original(path); }
        catch (Exception ex)
        {
            transcript.WriteLine("{0}|originalFailure={1}", label, ex.ToString().Replace('\r', ' ').Replace('\n', ' '));
            originalFailures++;
            mismatches++;
            if (!probe) { transcript.Flush(); throw; }
            Console.WriteLine("PROBE originalFailure " + label + " " + ex.GetType().Name);
            return;
        }
        string originalPixels = Pixels(expected);
        TexturePacker.Image actual;
        try { actual = TextureTgaDecoder.Decode(bytes); }
        catch (Exception ex)
        {
            transcript.WriteLine("{0}|original={1}x{2},alpha={3}|candidateFailure={4}|originalPixels={5}", label, expected.Width, expected.Height, expected.HasAlpha, ex.GetType().FullName + ": " + ex.Message, originalPixels);
            if (ex is NotSupportedException) unsupportedCount++;
            mismatches++;
            if (!probe) { transcript.Flush(); throw; }
            Console.WriteLine("PROBE candidateUnsupported " + label + " original=" + expected.Width + "x" + expected.Height + " alpha=" + expected.HasAlpha + " reason=" + ex.Message);
            return;
        }
        string recoveredPixels = Pixels(actual);
        bool equal = expected.Width == actual.Width && expected.Height == actual.Height && expected.HasAlpha == actual.HasAlpha && originalPixels == recoveredPixels;
        transcript.WriteLine("{0}|equal={1}|original={2}x{3},alpha={4}|recovered={5}x{6},alpha={7}|originalPixels={8}|recoveredPixels={9}", label, equal, expected.Width, expected.Height, expected.HasAlpha, actual.Width, actual.Height, actual.HasAlpha, originalPixels, recoveredPixels);
        if (!equal)
        {
            mismatches++;
            Console.WriteLine("DIFFERENCE " + label + " original alpha=" + expected.HasAlpha + " candidate alpha=" + actual.HasAlpha + " original first=" + expected.Pixels[0].ToString("X8") + " candidate first=" + actual.Pixels[0].ToString("X8"));
            if (!probe) { transcript.Flush(); throw new InvalidDataException("TGA decoder differs from original LoadImageA: " + label); }
        }
    }

    private static int Main(string[] args)
    {
        try
        {
            if (args.Length < 3 || args.Length > 5 || (args.Length >= 4 && args[3] != "--probe" && args[3] != "--strict"))
                throw new ArgumentException("Usage: TextureTgaOracle original.dll transcript.txt fixtures-directory [--probe|--strict] [real-fixtures-directory]");
            Check(IntPtr.Size == 4, "x86 process required");
            bool probe = args.Length >= 4 && args[3] == "--probe";
            string input = Path.GetFullPath(args[0]), directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string candidate = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll");
                return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
            };
            Assembly original = Assembly.LoadFrom(input); Bind(original, input);
            Type type = original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
            object plugin = Activator.CreateInstance(type);
            MethodInfo initialize = type.GetMethod("Initialize");
            Type platform = initialize.GetParameters()[1].ParameterType;
            initialize.Invoke(plugin, new object[] { null, Enum.ToObject(platform, 0) });
            string fixturesDirectory = Path.GetFullPath(args[2]); Directory.CreateDirectory(fixturesDirectory);
            try
            {
                using (StreamWriter transcript = new StreamWriter(args[1], false, Encoding.UTF8))
                {
                    foreach (int width in new int[] { 1, 2, 4, 8 })
                    foreach (int height in new int[] { 1, 2, 4 })
                    foreach (int depth in new int[] { 24, 32 })
                    foreach (bool rle in new bool[] { false, true })
                    foreach (int direction in new int[] { 0, 16, 32, 48 })
                    foreach (int alpha in new int[] { 0, 1, 2, 3 })
                    foreach (int id in new int[] { 0, 7 })
                    {
                        int descriptor = direction | (depth == 32 ? 8 : 0);
                        string label = width + "x" + height + "-" + depth + "-rle" + rle + "-descriptor" + descriptor + "-alpha" + alpha + "-id" + id;
                        Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), label, Fixture(width, height, depth, rle, descriptor, id, alpha), probe);
                    }
                    // Runs at the 128-pixel packet limit and across scanlines.
                    foreach (int width in new int[] { 64, 128, 256 })
                    foreach (int depth in new int[] { 24, 32 })
                    foreach (int direction in new int[] { 0, 16, 32, 48 })
                        Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "long-run-" + width + "-" + depth + "-direction" + direction, Fixture(width, 2, depth, true, direction | (depth == 32 ? 8 : 0), 0, 4), probe);
                    // Exact standard footer with no extension/developer area.
                    foreach (int depth in new int[] { 24, 32 })
                    foreach (bool rle in new bool[] { false, true })
                    foreach (int direction in new int[] { 0, 16, 32, 48 })
                    {
                        byte[] content = Fixture(4, 4, depth, rle, direction | (depth == 32 ? 8 : 0), 7, 3);
                        byte[] footer = new byte[content.Length + 26];
                        Buffer.BlockCopy(content, 0, footer, 0, content.Length);
                        byte[] signature = Encoding.ASCII.GetBytes("TRUEVISION-XFILE.\0");
                        Buffer.BlockCopy(signature, 0, footer, content.Length + 8, signature.Length);
                        Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "empty-footer-" + depth + "-rle" + rle + "-direction" + direction, footer, probe);
                    }
                    // 32-bit descriptor alpha-count zero is separately visible.
                    foreach (int direction in new int[] { 0, 16, 32, 48 })
                    foreach (bool rle in new bool[] { false, true })
                    foreach (int alpha in new int[] { 0, 1, 2, 3 })
                        Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "descriptor0-" + direction + "-rle" + rle + "-alpha" + alpha, Fixture(4, 4, 32, rle, direction, 0, alpha), probe);
                    if (args.Length == 5)
                    {
                        string realDirectory = Path.GetFullPath(args[4]);
                        string[] realFiles = Directory.GetFiles(realDirectory, "*.tga", SearchOption.AllDirectories);
                        Array.Sort(realFiles, StringComparer.Ordinal);
                        Check(realFiles.Length > 0, "Real TGA fixture directory must not be empty");
                        foreach (string realFile in realFiles)
                            Compare(transcript, Path.Combine(fixturesDirectory, "current.tga"), "real:" + Path.GetFileName(realFile), File.ReadAllBytes(realFile), probe);
                    }
                }
            }
            finally { ((IDisposable)plugin).Dispose(); }
            if (probe)
            {
                Console.WriteLine("PROBE Texture TGA original loading: fixtures=" + fixtures + "; mismatches=" + mismatches + "; unsupportedCount=" + unsupportedCount + "; originalFailures=" + originalFailures + "; no acceptance claim");
                return 0;
            }
            Console.WriteLine("PASS Texture TGA original LoadImageA differential: fixtures=" + fixtures + "; assertions=" + assertions + "; unsupportedCount=" + unsupportedCount + "; complete pixels and hasAlpha; raw/RLE24/32; four orientations");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
