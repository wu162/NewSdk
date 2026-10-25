using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered.Compiler.Texture;

// x86 CLR4 host with useLegacyV2RuntimeActivationPolicy=true. Calls original pure
// IL methods only; it never calls Initialize, D3D, compression, or image decoding.
internal static unsafe class TextureImageRulesOracle
{
    private static Assembly original;
    private static object plugin;
    private static Type pluginType;
    private static MethodInfo constant, oneBit, valid, guess, options;
    private static int checks, records;
    private static StreamWriter transcript;

    private static void Check(bool value, string message)
    {
        checks++;
        if (!value) throw new InvalidOperationException(message);
    }
    private static object PointerArgument(MethodInfo method, int index, void* address)
    {
        return Pointer.Box(address, method.GetParameters()[index].ParameterType);
    }
    private static object InvokeImage(MethodInfo method, uint width, uint height, byte[] pixels)
    {
        fixed (byte* p = pixels)
        {
            uint* image = stackalloc uint[6];
            image[0] = width; image[1] = height; image[2] = (uint)p;
            image[3] = unchecked(width * height); image[4] = image[3]; image[5] = 0;
            return method.Invoke(null, new object[] { PointerArgument(method, 0, image) });
        }
    }
    private static InstanceDeclaration Declaration()
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml("<Texture id='ImageRulesOracle' xmlns='uri:ea.com:eala:asset'/>");
        InstanceDeclaration result = new InstanceDeclaration((AssetDeclarationDocument)null);
        result.XmlNode = doc.DocumentElement;
        return result;
    }
    private static object InvokePluginImage(MethodInfo method, uint width, uint height, byte[] pixels, bool alpha, string format)
    {
        GCHandle root = GCHandle.Alloc(Declaration());
        int rootSlot = (int)GCHandle.ToIntPtr(root);
        fixed (byte* p = pixels)
        {
            uint* image = stackalloc uint[6];
            image[0] = width; image[1] = height; image[2] = (uint)p;
            image[3] = unchecked(width * height); image[4] = image[3]; image[5] = 0;
            try
            {
                object rootArgument = PointerArgument(method, 0, &rootSlot);
                object imageArgument = PointerArgument(method, method == valid ? 1 : 2, image);
                object result = method == valid
                    ? method.Invoke(plugin, new object[] { rootArgument, imageArgument })
                    : method.Invoke(plugin, new object[] { rootArgument, alpha, imageArgument, format });
                Check(rootSlot == 0, method.Name + " must destroy by-value gcroot");
                return result;
            }
            finally
            {
                // The original consumes and zeroes the gcroot on both normal and fault paths.
                if (rootSlot != 0) GCHandle.FromIntPtr(new IntPtr(rootSlot)).Free();
            }
        }
    }
    private static void AlphaCase(int index, uint width, uint height, byte[] pixels)
    {
        bool a = (bool)InvokeImage(constant, width, height, pixels);
        bool b = TextureImageRules.HasConstantAlphaChannel(width, height, pixels);
        Check(a == b, "constant alpha " + index);
        bool c = (bool)InvokeImage(oneBit, width, height, pixels);
        bool d = TextureImageRules.Has1BitAlphaChannel(width, height, pixels);
        Check(c == d, "one-bit alpha " + index);
        transcript.WriteLine("alpha|{0}|{1}|{2}|{3}|{4}", index, width, height, a, c);
        records++;
        if (pixels.Length == 0) return; // Original format inference's zero-pixel read is undefined.
        string[] formats = { "D3DFMT_DXT1", "D3DFMT_DXT1A", "D3DFMT_DXT3", "D3DFMT_DXT5", "d3dfmt_dxt1", "unknown", null };
        foreach (string format in formats)
        foreach (bool alpha in new bool[] { false, true })
        {
            string expected = (string)InvokePluginImage(guess, width, height, pixels, alpha, format);
            int messages = 0;
            string actual = TextureImageRules.GuessProperFormat(width, height, pixels, alpha, format, "ImageRulesOracle", delegate(string ignored) { messages++; });
            Check(expected == actual, "format " + index + "/" + format + "/" + alpha);
            Check(messages == (expected == format ? 0 : 1), "format trace count");
            transcript.WriteLine("format|{0}|{1}|{2}|{3}", index, format ?? "<null>", alpha, actual ?? "<null>");
            records++;
        }
    }
    private static void ValidCase(uint width, uint height)
    {
        bool expected = (bool)InvokePluginImage(valid, width, height, new byte[0], false, null);
        int warnings = 0;
        bool actual = TextureImageRules.IsValidTexture(width, height, "ImageRulesOracle", delegate(string message) { warnings++; });
        Check(expected == actual, "valid dimensions " + width + "x" + height);
        Check(warnings == (actual ? 0 : 1), "valid warning count");
        transcript.WriteLine("valid|{0}|{1}|{2}", width, height, actual);
        records++;
    }
    private static void OptionsCase(int platform, string format, bool mipmaps, int seed)
    {
        byte[] initial = new byte[TextureImageRules.CompressionOptionsSize];
        Random random = new Random(seed);
        if (seed == 1) for (int i = 0; i < initial.Length; i++) initial[i] = 0xA5;
        else if (seed == 2) random.NextBytes(initial);
        byte[] expected = (byte[])initial.Clone();
        fixed (byte* address = expected)
        {
            options.Invoke(plugin, new object[] { PointerArgument(options, 0, address), format, mipmaps });
        }
        byte[] actual = TextureImageRules.GetNVCompressionOptions(format, mipmaps, platform, initial);
        for (int i = 0; i < actual.Length; i++)
            Check(expected[i] == actual[i], "compression option byte " + i + " platform=" + platform + " format=" + format + " mipmaps=" + mipmaps + " seed=" + seed);
        transcript.WriteLine("options|{0}|{1}|{2}|{3}|{4}", platform, format ?? "<null>", mipmaps, seed, Convert.ToBase64String(actual));
        records++;
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new ArgumentException("Usage: TextureImageRulesOracle original.dll transcript.txt");
            Check(IntPtr.Size == 4, "x86 process required");
            string input = Path.GetFullPath(args[0]);
            string directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string candidate = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll");
                return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
            };
            original = Assembly.LoadFrom(input);
            pluginType = original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
            plugin = Activator.CreateInstance(pluginType);
            constant = (MethodInfo)original.ManifestModule.ResolveMethod(0x06000019);
            oneBit = (MethodInfo)original.ManifestModule.ResolveMethod(0x06000018);
            valid = pluginType.GetMethod("IsValidTexture", BindingFlags.NonPublic | BindingFlags.Instance);
            guess = pluginType.GetMethod("GuessProperFormat", BindingFlags.NonPublic | BindingFlags.Instance);
            options = pluginType.GetMethod("GetNVCompressionOptions", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo platformField = pluginType.GetField("_Platform", BindingFlags.NonPublic | BindingFlags.Instance);
            using (transcript = new StreamWriter(args[1], false, System.Text.Encoding.UTF8))
            {
                int index = 0;
                AlphaCase(index++, 0, 0, new byte[0]);
                AlphaCase(index++, 0x10000u, 0x10000u, new byte[0]);
                // All possible constant alpha values; RGB bytes deliberately vary.
                for (int alpha = 0; alpha < 256; alpha++)
                    AlphaCase(index++, 2, 2, new byte[] { 1,2,3,(byte)alpha, 9,8,7,(byte)alpha, 4,5,6,(byte)alpha, 22,11,0,(byte)alpha });
                // Every possible nonconstant alpha value paired with each binary endpoint.
                for (int alpha = 0; alpha < 256; alpha++)
                {
                    AlphaCase(index++, 2, 1, new byte[] { 0,0,0,0, 0,0,0,(byte)alpha });
                    AlphaCase(index++, 1, 2, new byte[] { 0,0,0,255, 0,0,0,(byte)alpha });
                }
                Random random = new Random(67041);
                for (int i = 0; i < 128; i++)
                {
                    byte[] pixels = new byte[64]; random.NextBytes(pixels);
                    AlphaCase(index++, 4, 4, pixels);
                }
                uint[] sizes = { 0u, 1u, 2u, 3u, 4u, 63u, 64u, 1024u, 2048u, 4095u, 4096u, 8192u, 16384u, 65536u, 0x40000000u, 0x80000000u, uint.MaxValue };
                foreach (uint width in sizes) foreach (uint height in sizes) ValidCase(width, height);
                string[] formats = { "D3DFMT_A8R8G8B8", "D3DFMT_DXT1", "D3DFMT_DXT1A", "D3DFMT_DXT3", "D3DFMT_DXT5", "D3DFMT_X8R8G8B8", "D3DFMT_R5G6B5", "D3DFMT_UNKNOWN", "d3dfmt_dxt5", "", null };
                foreach (int platform in new int[] { 0, 1, 2, 99 })
                {
                    platformField.SetValue(plugin, Enum.ToObject(platformField.FieldType, platform));
                    foreach (string format in formats) foreach (bool mipmaps in new bool[] { false, true })
                        for (int seed = 0; seed < 3; seed++) OptionsCase(platform, format, mipmaps, seed);
                }
                // Adapter guards intentionally replace native out-of-bounds reads.
                bool rejected = false;
                try { TextureImageRules.Has1BitAlphaChannel(2, 2, new byte[4]); }
                catch (ArgumentException) { rejected = true; }
                Check(rejected, "managed short-buffer guard");
                rejected = false;
                try { TextureImageRules.GetNVCompressionOptions("D3DFMT_DXT1", false, 0, new byte[1]); }
                catch (ArgumentException) { rejected = true; }
                Check(rejected, "managed native-storage guard");
            }
            Console.WriteLine("PASS Texture pure image rules: records=" + records + "; checks=" + checks + "; platforms=0,1,2,99; no D3D/compression/SDK claims");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}
