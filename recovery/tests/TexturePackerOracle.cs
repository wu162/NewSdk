using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Recovered.Compiler.Texture;

// x86 CLR4 /unsafe. Reflection invokes original managed IL using native image
// structures allocated by the SAME VC8 CRT as the original DLL. This exercises
// pack/padding/sort, without D3D, file decoding, nvDXT or SDK compilation.
internal unsafe static class TexturePackerOracle
{
    [DllImport("msvcr80.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "malloc")]
    private static extern IntPtr Malloc(UIntPtr size);
    [DllImport("msvcr80.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free")]
    private static extern void Free(IntPtr memory);
    static MethodInfo packOriginal, destroyOriginal;
    static int assertions, fixtures;
    static IntPtr Allocate(int bytes)
    {
        IntPtr memory = Malloc((UIntPtr)(uint)Math.Max(bytes, 1));
        if (memory == IntPtr.Zero) throw new OutOfMemoryException();
        for (int i = 0; i < bytes; ++i) ((byte*)memory)[i] = 0;
        return memory;
    }
    static object InvokePointer(MethodInfo method, IntPtr pointer)
    {
        return method.Invoke(null, new object[] { Pointer.Box(pointer.ToPointer(), method.GetParameters()[0].ParameterType) });
    }
    static void Equal<T>(T expected, T actual, string label)
    {
        if (!Object.Equals(expected, actual)) throw new Exception(label + " expected=" + expected + " actual=" + actual);
        ++assertions;
    }
    static IntPtr NativeImage(TexturePacker.Image image, bool normal)
    {
        IntPtr native = Allocate(24);
        IntPtr pixels = Allocate(image.Pixels.Length * 4);
        ((int*)native)[0] = image.Width; ((int*)native)[1] = image.Height;
        ((IntPtr*)native)[2] = pixels;
        ((int*)native)[3] = image.Pixels.Length; ((int*)native)[4] = image.Pixels.Length;
        for (int i = 0; i < image.Pixels.Length; ++i)
        {
            uint value = image.Pixels[i];
            if (normal) value |= 0xFF000000u;
            else if (!image.HasAlpha) value &= 0x00FFFFFFu;
            ((uint*)pixels)[i] = value;
        }
        return native;
    }
    static TexturePacker.Image Image(int seed, bool alpha)
    {
        uint[] pixels = new uint[256 * 256];
        for (int y = 0; y < 256; ++y)
            for (int x = 0; x < 256; ++x)
                pixels[y * 256 + x] = unchecked((uint)((x + seed * 17) & 255) | (uint)((y + seed * 23) & 255) << 8 |
                    (uint)((x ^ y ^ seed * 31) & 255) << 16 | (uint)((x * 7 + y * 13 + seed) & 255) << 24);
        return new TexturePacker.Image(256, 256, pixels, alpha);
    }
    static void CompareImage(IntPtr native, TexturePacker.Image recovered, string label)
    {
        Equal(native == IntPtr.Zero, recovered == null, label + "/present");
        if (native == IntPtr.Zero) return;
        int width = ((int*)native)[0], height = ((int*)native)[1];
        Equal(width, recovered.Width, label + "/width"); Equal(height, recovered.Height, label + "/height");
        uint* pixels = (uint*)((IntPtr*)native)[2];
        for (int i = 0; i < width * height; ++i)
            if (pixels[i] != recovered.Pixels[i])
                throw new Exception(label + "/pixel=" + i + " expected=" + pixels[i] + " actual=" + recovered.Pixels[i]);
        ++assertions;
    }
    static uint Id(int index, int count, int order)
    {
        if (order == 0) return unchecked((uint)(count - index) * 31337u);
        if (order == 1) return (uint)(index % 4);
        if (order == 2) return UInt32.MaxValue;
        return index % 3 == 0 ? UInt32.MaxValue : index % 3 == 1 ? 0u : 2147483648u;
    }
    static Exception Root(Exception exception)
    {
        while (exception is TargetInvocationException && exception.InnerException != null) exception = exception.InnerException;
        return exception;
    }
    static void Fixture(int count, int order, bool normals)
    {
        string label = "count=" + count + "/order=" + order + "/normals=" + normals;
        IntPtr original = Allocate(28);
        IntPtr entries = Allocate(count * 4);
        ((IntPtr*)original)[2] = entries;
        ((IntPtr*)original)[3] = IntPtr.Add(entries, count * 4);
        ((IntPtr*)original)[4] = IntPtr.Add(entries, count * 4);
        // Original constructor leaves byte25 uninitialized unless a normal is
        // loaded; initialize the controlled no-normal fixture to false.
        ((byte*)original)[25] = normals && count != 0 ? (byte)1 : (byte)0;
        TexturePacker recovered = new TexturePacker();
        try
        {
            for (int i = 0; i < count; ++i)
            {
                uint id = Id(i, count, order);
                TexturePacker.Image image = Image(i + 1, i % 2 == 0);
                TexturePacker.Image normal = normals && i % 2 == 0 ? Image(i + 101, false) : null;
                recovered.AddImage(id, image, normal);
                IntPtr tile = Allocate(20);
                ((IntPtr*)entries)[i] = tile;
                ((IntPtr*)tile)[0] = NativeImage(image, false);
                ((IntPtr*)tile)[1] = normal == null ? IntPtr.Zero : NativeImage(normal, true);
                ((uint*)tile)[2] = id;
            }
            Exception expectedError = null, actualError = null;
            object expectedResult = null;
            bool actualResult = false;
            try { expectedResult = InvokePointer(packOriginal, original); } catch (Exception error) { expectedError = Root(error); }
            try { actualResult = recovered.PackTextures(); } catch (Exception error) { actualError = Root(error); }
            Equal(expectedError == null, actualError == null, label + "/success");
            Equal(((byte*)original)[24] != 0, recovered.IsPacked, label + "/packedState");
            if (expectedError != null)
            {
                Equal(expectedError.GetType().FullName, actualError.GetType().FullName, label + "/exceptionType");
                Equal(expectedError.Message, actualError.Message, label + "/exceptionMessage");
            }
            else
            {
                Equal(Convert.ToBoolean(expectedResult), actualResult, label + "/return");
                CompareImage(((IntPtr*)original)[0], recovered.BaseAtlas, label + "/base");
                CompareImage(((IntPtr*)original)[1], recovered.NormalAtlas, label + "/normal");
                TexturePacker.Tile[] tiles = recovered.Tiles;
                Equal(count, tiles.Length, label + "/count");
                for (int i = 0; i < count; ++i)
                {
                    IntPtr tile = ((IntPtr*)entries)[i];
                    Equal(((uint*)tile)[2], tiles[i].Id, label + "/tileId/" + i);
                    Equal(((ushort*)tile)[6], tiles[i].UpperLeftX, label + "/upperX/" + i);
                    Equal(((ushort*)tile)[7], tiles[i].UpperLeftY, label + "/upperY/" + i);
                    Equal(((ushort*)tile)[8], tiles[i].BottomRightX, label + "/bottomX/" + i);
                    Equal(((ushort*)tile)[9], tiles[i].BottomRightY, label + "/bottomY/" + i);
                    Equal(IntPtr.Zero, ((IntPtr*)tile)[0], label + "/sourceBaseFreed/" + i);
                    Equal(IntPtr.Zero, ((IntPtr*)tile)[1], label + "/sourceNormalFreed/" + i);
                }
            }
            int previous = recovered.Tiles.Length;
            recovered.AddImage(99u, null, null);
            Equal(previous, recovered.Tiles.Length, label + "/addAfterPackedNoOp");
            ++fixtures;
        }
        finally
        {
            InvokePointer(destroyOriginal, original);
            Free(original);
        }
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || IntPtr.Size != 4) throw new Exception("Usage (x86): TexturePackerOracle original.dll");
            string input = Path.GetFullPath(args[0]);
            string directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string file = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll");
                return File.Exists(file) ? Assembly.LoadFrom(file) : null;
            };
            Assembly assembly = Assembly.LoadFrom(input);
            // Resolve known original MethodDefs: reflection may hide the
            // special global <Module> type from Assembly.GetType.
            packOriginal = (MethodInfo)assembly.ManifestModule.ResolveMethod(0x06000024);
            destroyOriginal = (MethodInfo)assembly.ManifestModule.ResolveMethod(0x06000021);
            Equal("BinaryAssetBuilder.TextureCompiler.TexturePacker.PackTextures", packOriginal.Name, "original PackTextures token identity");
            Equal("BinaryAssetBuilder.TextureCompiler.TexturePacker.{dtor}", destroyOriginal.Name, "original destructor token identity");
            foreach (int count in new int[] { 0, 1, 2, 3, 4, 7, 8, 9, 10, 21, 22, 28, 29, 32, 49, 50 })
                for (int order = 0; order < 4; ++order)
                    Fixture(count, order, order % 2 == 0);
            Console.WriteLine("PASS TexturePacker original differential fixtures=" + fixtures + " assertions=" + assertions + "; all atlas pixels, metadata order and coordinates; no codec/nvDXT/SDK acceptance");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
