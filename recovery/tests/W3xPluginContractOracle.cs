using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

// Contract inventory only: this host does not claim to test recovered behavior.
// Run as x86 CLR4 with legacy CLR2 mixed-mode activation enabled.
internal static class W3xPluginContractOracle
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new ArgumentException("Usage: W3xPluginContractOracle original.dll output.json");
            string input = Path.GetFullPath(args[0]);
            string directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string candidate = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll");
                return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
            };
            if (IntPtr.Size != 4) throw new InvalidOperationException("The original compiler requires an x86 oracle host.");
            Assembly original = Assembly.LoadFrom(input);
            Type plugin = original.GetType("BinaryAssetBuilder.W3XCompiler.Plugin", true);
            string[] interfaces = plugin.GetInterfaces().Select(t => t.FullName).OrderBy(n => n).ToArray();
            if (!interfaces.Contains("BinaryAssetBuilder.Core.IAssetBuilderPlugin"))
                throw new InvalidOperationException("Expected BAB plugin interface was not found.");
            var methods = plugin.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .OrderBy(m => m.MetadataToken).Select(m => new
                {
                    token = "0x" + m.MetadataToken.ToString("X8"), name = m.Name,
                    returns = m.ReturnType.FullName, isStatic = m.IsStatic,
                    parameters = m.GetParameters().Select(p => new { name = p.Name, type = p.ParameterType.FullName }).ToArray()
                }).ToArray();
            string hash;
            using (var sha = SHA256.Create())
                hash = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(input))).Replace("-", "").ToLowerInvariant();
            var result = new
            {
                scope = "original-plugin-contract-inventory; no recovered implementation or differential acceptance",
                sha256 = hash, assembly = original.FullName, runtime = original.ImageRuntimeVersion,
                pointerSize = IntPtr.Size, plugin = plugin.FullName, interfaces = interfaces,
                constructors = plugin.GetConstructors().Select(c => c.ToString()).ToArray(), methods = methods
            };
            File.WriteAllText(args[1], new JavaScriptSerializer().Serialize(result));
            Console.WriteLine("PASS original W3X plugin contract inventory: " + methods.Length + " public declared methods");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
