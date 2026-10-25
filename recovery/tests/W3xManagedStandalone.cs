using System;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core;
using Recovered.W3x;
using Plugin = Recovered.Compiler.W3x.Plugin;

internal static class W3xManagedStandalone
{
    private static int Main()
    {
        try
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BinaryAssetBuilder.W3XCompiler.dll")))
                throw new Exception("Original W3X DLL must be absent.");
            var plugin = new Plugin();
            foreach (int platform in new int[] { 0, 1, 2, 99 })
            {
                plugin.Initialize(null, (TargetPlatform)platform);
                var info = plugin.GetExtendedTypeInformation(W3xCatalog.AnimationTypeId);
                if (info.TypeName != "W3DAnimation" || info.TypeHash != 1430609896u ||
                    info.ProcessingHash != (plugin.GetVersionNumber() ^ 0xD6505CE1u))
                    throw new Exception("Standalone managed metadata mismatch.");
            }
            plugin.Dispose();
            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "BinaryAssetBuilder.W3XCompiler"))
                throw new Exception("Original W3X DLL was loaded.");
            Console.WriteLine("PASS W3X managed standalone: original W3X DLL absent; metadata only, no animation/mesh acceptance");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
