using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

// Pure IL differential: horizontal strip slicing and logical volume resizing.
// No native compressor, D3D, loader, filtered mip generation or SDK acceptance.
internal static unsafe class TextureVolumeOracle
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate IntPtr MallocFunction(UIntPtr bytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void FreeFunction(IntPtr address);
    static MallocFunction malloc; static FreeFunction free;
    static MethodInfo resize, destroy, strip;
    static long checks, fixtures, pixels;
    static string label;
    static void Equal<T>(T expected, T actual, string name)
    {
        ++checks;
        if (!Object.Equals(expected, actual)) throw new Exception(label + "/" + name + " expected=" + expected + " actual=" + actual);
    }
    static IntPtr Allocate(int size)
    {
        IntPtr memory = malloc((UIntPtr)(uint)Math.Max(size, 1));
        if (memory == IntPtr.Zero) throw new OutOfMemoryException();
        for (int i = 0; i < size; ++i) ((byte*)memory)[i] = 0;
        return memory;
    }
    static object Ptr(MethodInfo method, int parameter, void* pointer)
    {
        return Pointer.Box(pointer, method.GetParameters()[parameter].ParameterType);
    }
    static void Resize(IntPtr volume, uint width, uint height, uint depth, uint levels)
    {
        resize.Invoke(null, new object[] { Ptr(resize, 0, volume.ToPointer()), width, height, depth, levels });
    }
    static void Destroy(IntPtr volume)
    {
        destroy.Invoke(null, new object[] { Ptr(destroy, 0, volume.ToPointer()) });
        free(volume);
    }
    static uint* Level(IntPtr volume, int level) { return (uint*)((uint*)volume)[0] + level * 3; }
    static uint* Slice(IntPtr volume, int level, int slice) { return (uint*)Level(volume, level)[0] + slice * 6; }
    static void CompareShape(IntPtr native, TextureVolume recovered)
    {
        Equal(((uint*)native)[1], (uint)recovered.Levels.Length, "levelCount");
        Equal(((uint*)native)[2], recovered.LevelCapacity, "levelCapacity");
        for (int l = 0; l < recovered.Levels.Length; ++l)
        {
            TextureVolume.Level level = recovered.Levels[l];
            Equal(Level(native, l)[1], (uint)level.Slices.Length, "sliceCount/" + l);
            Equal(Level(native, l)[2], level.SliceCapacity, "sliceCapacity/" + l);
            for (int z = 0; z < level.Slices.Length; ++z)
            {
                uint* image = Slice(native, l, z);
                TextureVolume.Slice slice = level.Slices[z];
                Equal(image[0], slice.Width, "width/" + l + "/" + z);
                Equal(image[1], slice.Height, "height/" + l + "/" + z);
                Equal(image[3], slice.PixelCount, "pixelCount/" + l + "/" + z);
                Equal(image[4], slice.PixelCapacity, "pixelCapacity/" + l + "/" + z);
            }
        }
    }
    static void CompareLogicalPixels(IntPtr native, TextureVolume recovered)
    {
        for (int l = 0; l < recovered.Levels.Length; ++l)
            for (int z = 0; z < recovered.Levels[l].Slices.Length; ++z)
            {
                TextureVolume.Slice image = recovered.Levels[l].Slices[z];
                uint* data = (uint*)Slice(native, l, z)[2];
                for (int p = 0; p < image.PixelCount; ++p)
                {
                    Equal(data[p], image.Pixels[p], "rgba/" + l + "/" + z + "/" + p);
                    ++pixels;
                }
            }
    }
    static uint Pattern(int seed, int level, int slice, int pixel)
    {
        return unchecked((uint)(seed * 123457 + level * 500009 + slice * 700001 + pixel * 31337));
    }
    static void Stamp(IntPtr native, TextureVolume recovered, int seed)
    {
        for (int l = 0; l < recovered.Levels.Length; ++l)
            for (int z = 0; z < recovered.Levels[l].Slices.Length; ++z)
            {
                TextureVolume.Slice image = recovered.Levels[l].Slices[z];
                uint* data = (uint*)Slice(native, l, z)[2];
                for (int p = 0; p < image.PixelCapacity; ++p)
                    image.Pixels[p] = data[p] = Pattern(seed, l, z, p);
            }
    }
    static uint[][][] Snapshot(TextureVolume recovered)
    {
        uint[][][] snapshot = new uint[recovered.Levels.Length][][];
        for (int l = 0; l < snapshot.Length; ++l)
        {
            snapshot[l] = new uint[recovered.Levels[l].Slices.Length][];
            for (int z = 0; z < snapshot[l].Length; ++z)
                snapshot[l][z] = (uint[])recovered.Levels[l].Slices[z].Pixels.Clone();
        }
        return snapshot;
    }
    static void ResizeSequence()
    {
        uint[][] cases = {
            new uint[] {8,8,8,0}, new uint[] {4,4,4,0}, new uint[] {2,2,2,0}, new uint[] {8,8,8,0},
            new uint[] {7,5,9,0}, new uint[] {3,5,7,6}, new uint[] {1,1,1,12}, new uint[] {1,1,1,1},
            new uint[] {1,1,1,9}, new uint[] {0,0,0,0}, new uint[] {0,5,3,0}, new uint[] {5,0,3,0},
            new uint[] {5,3,0,0}, new uint[] {0,0,0,5}, new uint[] {0,0,1,1}, new uint[] {0,7,1,1},
            new uint[] {0x80000000u,2,1,1}, new uint[] {0x80000001u,2,1,1}, new uint[] {0xFFFFFFFFu,0,1,1},
            new uint[] {3,4,5,1}, new uint[] {3,4,5,0}, new uint[] {1,1,1,3}, new uint[] {3,4,5,0}
        };
        IntPtr native = Allocate(12); TextureVolume recovered = new TextureVolume();
        try
        {
            for (int i = 0; i < cases.Length; ++i)
            {
                uint[] c = cases[i]; label = "resize/" + i;
                uint[][][] before = Snapshot(recovered);
                Resize(native, c[0], c[1], c[2], c[3]);
                recovered.Resize(c[0], c[1], c[2], c[3]);
                CompareShape(native, recovered);
                // Fresh native allocation is uninitialized. Compare only the
                // bytes which were explicitly stamped and retained by realloc.
                for (int l = 0; l < Math.Min(before.Length, recovered.Levels.Length); ++l)
                    for (int z = 0; z < Math.Min(before[l].Length, recovered.Levels[l].Slices.Length); ++z)
                    {
                        uint[] expected = before[l][z]; TextureVolume.Slice actual = recovered.Levels[l].Slices[z];
                        uint* originalPixels = (uint*)Slice(native, l, z)[2];
                        for (int p = 0; p < Math.Min(expected.Length, actual.Pixels.Length); ++p)
                        {
                            Equal(expected[p], originalPixels[p], "originalRetainedRGBA/" + l + "/" + z + "/" + p);
                            Equal(expected[p], actual.Pixels[p], "recoveredRetainedRGBA/" + l + "/" + z + "/" + p);
                            ++pixels;
                        }
                    }
                Stamp(native, recovered, i + 1); ++fixtures;
            }
        }
        finally { Destroy(native); }
    }
    static Exception Root(Exception error)
    {
        while (error is TargetInvocationException && error.InnerException != null) error = error.InnerException;
        return error;
    }
    static void StripCase(uint width, uint height, int seed)
    {
        label = "strip/" + width + "x" + height + "/" + seed;
        uint count = checked(width * height);
        uint[] rgba = new uint[(int)count];
        for (int i = 0; i < rgba.Length; ++i) rgba[i] = Pattern(seed, 0, 0, i);
        IntPtr native = Allocate(12); TextureVolume recovered = new TextureVolume();
        try
        {
            // Reuse a nonempty destination, covering resize destruction and
            // unchanged destination state when the modulo check rejects.
            Resize(native, 3u, 2u, 4u, 0u); recovered.Resize(3u, 2u, 4u, 0u); Stamp(native, recovered, seed);
            Exception expectedError = null, actualError = null;
            fixed (uint* data = rgba)
            {
                uint* image = stackalloc uint[6];
                image[0] = width; image[1] = height; image[2] = (uint)data;
                image[3] = count; image[4] = count; image[5] = 0;
                try { strip.Invoke(null, new object[] { Ptr(strip, 0, native.ToPointer()), Ptr(strip, 1, image) }); }
                catch (Exception error) { expectedError = Root(error); }
            }
            try { recovered.CreateVolumeRGBAImage(width, height, rgba); } catch (Exception error) { actualError = Root(error); }
            Equal(expectedError == null, actualError == null, "success");
            if (expectedError != null)
            {
                Equal(expectedError.GetType().FullName, actualError.GetType().FullName, "exceptionType");
                Equal(expectedError.Message, actualError.Message, "exceptionMessage");
            }
            CompareShape(native, recovered); CompareLogicalPixels(native, recovered); ++fixtures;
        }
        finally { Destroy(native); }
    }
    static void SafeBoundaryCases()
    {
        label = "managedUnsafeMemoryBoundary";
        foreach (uint[] dimensions in new uint[][] { new uint[] {0x40000000u,1}, new uint[] {0x80000000u,2}, new uint[] {65536,65536} })
        {
            try { TextureVolume.FromStrip(dimensions[0], dimensions[1], new uint[0]); throw new Exception("Expected explicit unsafe extent rejection"); }
            catch (NotSupportedException) { ++checks; }
        }
        try { TextureVolume.FromStrip(8, 4, new uint[1]); throw new Exception("Expected short storage rejection"); }
        catch (ArgumentException) { ++checks; }
        try { TextureVolume.FromStrip(8, 4, null); throw new Exception("Expected null storage rejection"); }
        catch (ArgumentNullException) { ++checks; }
        Equal(32u, TextureVolume.MipCount(UInt32.MaxValue,0,0), "maxUintMipCount");
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 1 || IntPtr.Size != 4) throw new Exception("Usage (x86): TextureVolumeOracle original.dll");
            string input = Path.GetFullPath(args[0]); string directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e) {
                string candidate = Path.Combine(directory,new AssemblyName(e.Name).Name + ".dll");
                return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
            };
            string hash;
            using (SHA256 sha = SHA256.Create()) hash = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(input))).Replace("-", "").ToLowerInvariant();
            if (hash != "4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e") throw new Exception("Original SHA256 differs from guarded thunk/token input");
            Assembly original = Assembly.LoadFrom(input); IntPtr imageBase = Marshal.GetHINSTANCE(original.ManifestModule);
            if (imageBase == IntPtr.Zero || imageBase == new IntPtr(-1)) throw new Exception("Original native module base unavailable");
            malloc = (MallocFunction)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(imageBase,0x4e74c),typeof(MallocFunction));
            free = (FreeFunction)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(imageBase,0x4e758),typeof(FreeFunction));
            resize = (MethodInfo)original.ManifestModule.ResolveMethod(0x06000003);
            destroy = (MethodInfo)original.ManifestModule.ResolveMethod(0x06000004);
            strip = (MethodInfo)original.ManifestModule.ResolveMethod(0x0600001d);
            ResizeSequence();
            foreach (uint h in new uint[] {0,1,2,3,4,5,7,8,16,31,32,63,64})
                foreach (uint multiple in new uint[] {0,1,2,3,5,7})
                    for (int seed = 1; seed <= 2; ++seed)
                    {
                        StripCase(h == 0 ? multiple : h * multiple, h, seed);
                        if (h > 1 && multiple > 0) StripCase(h * multiple + 1u, h, seed);
                    }
            StripCase(0u, UInt32.MaxValue, 3);
            SafeBoundaryCases();
            Console.WriteLine("PASS TextureVolume original differential fixtures=" + fixtures + " checks=" + checks + " pixels=" + pixels + "; resize/capacities/retained RGBA/strip slices; native compression and SDK not validated");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(label); Console.Error.WriteLine(error); return 1; }
    }
}
