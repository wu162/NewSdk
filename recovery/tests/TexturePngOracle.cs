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
internal static unsafe class TexturePngOracle
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr MallocFunction(UIntPtr bytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void FreeFunction(IntPtr memory);
    private static MallocFunction malloc;
    private static FreeFunction free;
    private static MethodInfo construct, destroy, load;
    private static IDisposable owner;
    private static int fixtures, mismatches, assertions, unsupportedCount, originalFailures, rejectionMatches, decodedMatches;
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
        Type pluginType = original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
        object plugin = Activator.CreateInstance(pluginType);
        MethodInfo initialize = pluginType.GetMethod("Initialize");
        Type platform = initialize.GetParameters()[1].ParameterType;
        initialize.Invoke(plugin, new object[] { null, Enum.ToObject(platform, 0) });
        owner = (IDisposable)plugin;
        byte[] sanity = new byte[34]; sanity[2]=2;sanity[12]=2;sanity[14]=2;sanity[16]=32;sanity[17]=40;
        string sanityPath=Path.Combine(Path.GetDirectoryName(input),"sanity.tga");File.WriteAllBytes(sanityPath,sanity);
        Check(Original(sanityPath).Width==2,"Initialized native TGA positive control");
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
    private static void Big(BinaryWriter w, uint n) { w.Write((byte)(n >> 24)); w.Write((byte)(n >> 16)); w.Write((byte)(n >> 8)); w.Write((byte)n); }
    private static uint Crc(byte[] bytes)
    {
        uint result = 0xffffffff;
        foreach (byte value in bytes)
        {
            result ^= value;
            for (int i = 0; i < 8; i++) result = (result & 1) != 0 ? (result >> 1) ^ 0xedb88320u : result >> 1;
        }
        return ~result;
    }
    private static void Chunk(BinaryWriter w, string name, byte[] bytes)
    {
        byte[] tag = Encoding.ASCII.GetBytes(name), crcBytes = new byte[bytes.Length + 4];
        Buffer.BlockCopy(tag, 0, crcBytes, 0, 4); Buffer.BlockCopy(bytes, 0, crcBytes, 4, bytes.Length);
        Big(w, (uint)bytes.Length); w.Write(crcBytes); Big(w, Crc(crcBytes));
    }
    private static byte[] Header(int width, int height, int depth, int color, int interlace)
    {
        using (MemoryStream s = new MemoryStream()) using (BinaryWriter w = new BinaryWriter(s))
        { Big(w, (uint)width); Big(w, (uint)height); w.Write((byte)depth); w.Write((byte)color); w.Write((byte)0); w.Write((byte)0); w.Write((byte)interlace); return s.ToArray(); }
    }
    private static int Value(int x, int y, int channel, int depth, int pattern)
    {
        int mask = (1 << depth) - 1;
        if (pattern == 1) return 0;
        if (pattern == 2) return mask;
        if (pattern == 3) return ((x + y + channel) & 1) * mask;
        return (x * 1271 + y * 395 + channel * 739 + 17) & mask;
    }
    private static int Predict(int a, int b, int c)
    {
        int p = a + b - c;
        return Math.Abs(p - a) <= Math.Abs(p - b) && Math.Abs(p - a) <= Math.Abs(p - c) ? a : Math.Abs(p - b) <= Math.Abs(p - c) ? b : c;
    }
    // Fixture generation is independent of the decoder; forward PNG filters,
    // a stored-block zlib stream and scatter of source samples into Adam7 passes.
    public static byte[] Fixture(int width, int height, int depth, int color, int interlace, int filterMode, int metadata, int pattern)
    {
        int channels = color == 0 || color == 3 ? 1 : color == 2 ? 3 : color == 4 ? 2 : 4;
        byte[] raw;
        using (MemoryStream samples = new MemoryStream())
        {
            int[,] passes = interlace == 0 ? new int[,] { { 0, 0, 1, 1 } } : new int[,] { { 0, 0, 8, 8 }, { 4, 0, 8, 8 }, { 0, 4, 4, 8 }, { 2, 0, 4, 4 }, { 0, 2, 2, 4 }, { 1, 0, 2, 2 }, { 0, 1, 1, 2 } };
            for (int p = 0; p < passes.GetLength(0); p++)
            {
                List<int> xs = new List<int>(); for (int x = passes[p, 0]; x < width; x += passes[p, 2]) xs.Add(x);
                if (xs.Count == 0) continue;
                byte[] previous = new byte[(xs.Count * channels * depth + 7) / 8];
                for (int y = passes[p, 1]; y < height; y += passes[p, 3])
                {
                    byte[] row = new byte[previous.Length]; int sample = 0;
                    foreach (int x in xs) for (int c = 0; c < channels; c++, sample++)
                    {
                        int v = Value(x, y, c, depth, pattern);
                        if (depth == 16) { row[2 * sample] = (byte)(v >> 8); row[2 * sample + 1] = (byte)v; }
                        else if (depth == 8) row[sample] = (byte)v;
                        else row[sample * depth / 8] |= (byte)(v << (8 - depth - sample * depth % 8));
                    }
                    int filter = filterMode == 5 ? (y + p) % 5 : filterMode, stride = Math.Max(1, channels * depth / 8);
                    samples.WriteByte((byte)filter);
                    for (int i = 0; i < row.Length; i++)
                    {
                        int a = i >= stride ? row[i - stride] : 0, b = previous[i], c = i >= stride ? previous[i - stride] : 0;
                        int predict = filter == 0 ? 0 : filter == 1 ? a : filter == 2 ? b : filter == 3 ? (a + b) / 2 : Predict(a, b, c);
                        samples.WriteByte(unchecked((byte)(row[i] - predict)));
                    }
                    previous = row;
                }
            }
            raw = samples.ToArray();
        }
        byte[] zlib;
        using (MemoryStream s = new MemoryStream()) using (BinaryWriter w = new BinaryWriter(s))
        {
            w.Write((byte)0x78); w.Write((byte)0x01);
            int offset = 0;
            do
            {
                int n = Math.Min(65535, raw.Length - offset); bool last = offset + n == raw.Length;
                w.Write((byte)(last ? 1 : 0)); w.Write((ushort)n); w.Write((ushort)~n); w.Write(raw, offset, n); offset += n;
            } while (offset < raw.Length);
            uint a = 1, b = 0; foreach (byte v in raw) { a = (a + v) % 65521; b = (b + a) % 65521; }
            Big(w, b * 65536 + a); zlib = s.ToArray();
        }
        using (MemoryStream s = new MemoryStream()) using (BinaryWriter w = new BinaryWriter(s))
        {
            w.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }); Chunk(w, "IHDR", Header(width, height, depth, color, interlace));
            if (color == 3)
            {
                int entries = 1 << depth; byte[] palette = new byte[entries * 3];
                for (int e = 0; e < entries; e++) { palette[e * 3] = (byte)(e * 73 + 11); palette[e * 3 + 1] = (byte)(e * 29 + 17); palette[e * 3 + 2] = (byte)(e * 43 + 37); }
                Chunk(w, "PLTE", palette);
            }
            if (metadata >= 1 && metadata <= 3 && (color == 0 || color == 2 || color == 3))
            {
                byte[] trns = color == 0 ? new byte[2] : color == 2 ? new byte[6] : new byte[Math.Max(1, (1 << depth) / 2)];
                if (color == 3) for (int i = 0; i < trns.Length; i++) trns[i] = (byte)(metadata == 1 ? i * 61 : metadata == 2 ? 255 : 0);
                else for (int c = 0; c < trns.Length / 2; c++) { int v = Value(0, 0, c, depth, pattern); trns[c * 2] = (byte)(v >> 8); trns[c * 2 + 1] = (byte)v; }
                Chunk(w, "tRNS", trns);
            }
            if (metadata == 4) Chunk(w, "gAMA", new byte[] { 0, 0, 0xb1, 0x8f });
            if (metadata == 5) Chunk(w, "gAMA", new byte[] { 0, 1, 0x86, 0xa0 });
            if (metadata == 6) Chunk(w, "sRGB", new byte[] { 0 });
            if (metadata == 7) { byte[] significant = new byte[color == 3 ? 3 : channels]; for (int i = 0; i < significant.Length; i++) significant[i] = (byte)Math.Min(depth, 5); Chunk(w, "sBIT", significant); }
            if(metadata>=9){uint[] gammas={10000,20000,43000,45000,46000,47000,50000,70000,180000,220000,1000};uint value=gammas[metadata-9];Chunk(w,"gAMA",new byte[]{(byte)(value>>24),(byte)(value>>16),(byte)(value>>8),(byte)value});}
            if (metadata == 8) { Chunk(w, "tEXt", Encoding.ASCII.GetBytes("Software\0TextureCompiler recovery test")); Chunk(w, "pHYs", new byte[] { 0, 0, 11, 19, 0, 0, 11, 19, 1 }); }
            // Non-byte-aligned split exercises concatenation across arbitrary IDAT boundaries.
            int split = zlib.Length / 3; byte[] first = new byte[split], rest = new byte[zlib.Length - split];
            Buffer.BlockCopy(zlib, 0, first, 0, split); Buffer.BlockCopy(zlib, split, rest, 0, rest.Length);
            Chunk(w, "IDAT", first); Chunk(w, "IDAT", new byte[0]); Chunk(w, "IDAT", rest); Chunk(w, "IEND", new byte[0]);
            return s.ToArray();
        }
    }
    private static string Hash(uint[] pixels)
    {
        byte[] bytes = new byte[pixels.Length * 4]; Buffer.BlockCopy(pixels, 0, bytes, 0, bytes.Length);
        using (SHA256 sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "");
    }
    private static string Failure(Exception e)
    {
        if(e==null)return null;while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;
        var property=e.GetType().GetProperty("ErrorCode");return e.GetType().FullName+"/"+e.Message+"/"+(property==null?"":Convert.ToString(property.GetValue(e,null)));
    }
    private static void Compare(StreamWriter transcript, string path, string label, byte[] bytes, bool probe)
    {
        File.WriteAllBytes(path, bytes); fixtures++;
        TexturePacker.Image expected = null, actual = null; Exception error = null, candidateError = null;
        try { expected = Original(path); } catch (Exception e) { error = e; }
        try { actual = TexturePngDecoder.Decode(bytes); } catch (Exception e) { candidateError = e; }
        if(error!=null&&candidateError!=null&&Failure(error)==Failure(candidateError))
        {originalFailures++;rejectionMatches++;transcript.WriteLine(label+"|rejectionMatched=True|failure="+Failure(error));return;}
        if (error != null || candidateError != null)
        {
            originalFailures += error != null ? 1 : 0; mismatches++;
            transcript.WriteLine(label + "|originalError=" + error + "|candidateError=" + candidateError);
            if(mismatches<=12)Console.WriteLine("DIFFERENCE " + label + " original=" + (error == null ? "success" : error.ToString()) + " candidate=" + candidateError);
            if (!probe) throw new InvalidDataException("PNG load failed: " + label);
            return;
        }
        bool equal = expected.Width == actual.Width && expected.Height == actual.Height && expected.HasAlpha == actual.HasAlpha && Hash(expected.Pixels) == Hash(actual.Pixels);
        int first = -1; for (int i = 0; i < Math.Min(expected.Pixels.Length, actual.Pixels.Length); i++) if (expected.Pixels[i] != actual.Pixels[i]) { first = i; break; }
        transcript.WriteLine("{0}|equal={1}|original={2}x{3},alpha={4},hash={5}|candidate={6}x{7},alpha={8},hash={9}|firstDiff={10},expected={11},actual={12}", label, equal, expected.Width, expected.Height, expected.HasAlpha, Hash(expected.Pixels), actual.Width, actual.Height, actual.HasAlpha, Hash(actual.Pixels), first, first < 0 ? "" : expected.Pixels[first].ToString("X8"), first < 0 ? "" : actual.Pixels[first].ToString("X8"));
        if (equal) decodedMatches++; else
        {
            mismatches++; Console.WriteLine("DIFFERENCE " + label + " alpha=" + expected.HasAlpha + "/" + actual.HasAlpha + " first=" + first + (first < 0 ? "" : " pixels=" + expected.Pixels[first].ToString("X8") + "/" + actual.Pixels[first].ToString("X8")));
            if (!probe) throw new InvalidDataException("PNG pixels/alpha differ: " + label);
        }
    }
    private static void FormatControls(string work)
    {
        Directory.CreateDirectory(work);
        using (var bitmap = new System.Drawing.Bitmap(16, 8))
        {
            for (int y=0;y<8;y++)for(int x=0;x<16;x++)bitmap.SetPixel(x,y,System.Drawing.Color.FromArgb((x*37+y*17)&255,x*17,y*31,71));
            bitmap.Save(Path.Combine(work,"system.png"),System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Save(Path.Combine(work,"system.bmp"),System.Drawing.Imaging.ImageFormat.Bmp);
            bitmap.Save(Path.Combine(work,"system.jpg"),System.Drawing.Imaging.ImageFormat.Jpeg);
            bitmap.Save(Path.Combine(work,"system.gif"),System.Drawing.Imaging.ImageFormat.Gif);
        }
        File.WriteAllBytes(Path.Combine(work,"generated.png"),Fixture(16,8,8,6,0,0,0,0));
        File.WriteAllBytes(Path.Combine(work,"generated.tga"),Fixture(16,8,8,6,0,0,0,0));
        byte[] tga=new byte[18+16*8*4];tga[2]=2;tga[12]=16;tga[14]=8;tga[16]=32;tga[17]=40;
        for(int n=18;n<tga.Length;n++)tga[n]=(byte)(n*13);
        File.WriteAllBytes(Path.Combine(work,"control.tga"),tga);
        foreach(string file in Directory.GetFiles(work))
        {
            try { var result=Original(file); Console.WriteLine("FORMAT "+Path.GetFileName(file)+" accepted="+result.Width+"x"+result.Height+" alpha="+result.HasAlpha+" pixels="+Hash(result.Pixels)); }
            catch(Exception e){while(e is TargetInvocationException&&e.InnerException!=null)e=e.InnerException;Console.WriteLine("FORMAT "+Path.GetFileName(file)+" rejected="+e.GetType().FullName+"/"+e.Message);}
        }
        File.Copy(Path.Combine(work,"system.png"),Path.Combine(Path.GetDirectoryName(work),"system.png"),true);
        Console.WriteLine("PROBE Texture native format controls complete");
    }
    private static int Main(string[] args)
    {
        try
        {
            bool probe = args.Length > 3 && args[3] == "--probe";
            string input = Path.GetFullPath(args[0]), directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) { string p = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll"); return File.Exists(p) ? Assembly.LoadFrom(p) : null; };
            Check(IntPtr.Size == 4, "x86 process required"); Bind(Assembly.LoadFrom(input), input);
            Directory.CreateDirectory(args[2]);
            if(args.Length>3 && args[3]=="--formats"){FormatControls(args[2]);return 0;}
            using (StreamWriter transcript = new StreamWriter(args[1]))
            {
                foreach (int color in new int[] { 0, 2, 3, 4, 6 }) foreach (int depth in new int[] { 1, 2, 4, 8, 16 })
                {
                    if (color == 3 ? depth == 16 : color != 0 && depth < 8) continue;
                    foreach (int interlace in new int[] { 0, 1 }) foreach (int filter in new int[] { 0, 1, 2, 3, 4, 5 })
                    foreach (int size in new int[] { 1, 2, 4, 8, 16 }) foreach (int pattern in new int[] { 0, 1, 2, 3 })
                    {
                        string label = "c" + color + "-d" + depth + "-i" + interlace + "-f" + filter + "-s" + size + "-p" + pattern;
                        Compare(transcript, Path.Combine(args[2], "test.png"), label, Fixture(size, size == 1 ? 1 : size / 2, depth, color, interlace, filter, 0, pattern), probe);
                    }
                    foreach (int interlace in new int[] { 0, 1 }) for (int meta = 1; meta <= 19; meta++) foreach (int pattern in new int[] { 0, 1, 2, 3 })
                    {
                        string label = "c" + color + "-d" + depth + "-i" + interlace + "-meta" + meta + "-p" + pattern;
                        Compare(transcript, Path.Combine(args[2], "test.png"), label, Fixture(16, 8, depth, color, interlace, 5, meta, pattern), probe);
                    }
                }
                foreach(bool rgba in new[]{false,true})
                using(var bitmap=new System.Drawing.Bitmap(16,8,rgba?System.Drawing.Imaging.PixelFormat.Format32bppArgb:System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    for(int y=0;y<8;y++)for(int x=0;x<16;x++)bitmap.SetPixel(x,y,System.Drawing.Color.FromArgb(rgba?((x*37+y*17)&255):255,x*17,y*31,71));
                    string path=Path.Combine(args[2],"gdi.png");bitmap.Save(path,System.Drawing.Imaging.ImageFormat.Png);
                    Compare(transcript,path,"gdi-"+rgba,File.ReadAllBytes(path),probe);
                }
            }
            Check(fixtures==decodedMatches+rejectionMatches+unsupportedCount+mismatches,"Complete PNG fixture accounting");
            Console.WriteLine("{0} Texture PNG fixtures={1} decodedMatches={2} originalFailures={3} mismatches={4} assertions={5} rejectionMatches={6} unsupportedCount={7}", probe ? "PROBE" : "PASS", fixtures, decodedMatches, originalFailures, mismatches, assertions, rejectionMatches, unsupportedCount);
            return !probe && mismatches != 0 ? 1 : 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
        finally { if(owner!=null)owner.Dispose(); }
    }
}
