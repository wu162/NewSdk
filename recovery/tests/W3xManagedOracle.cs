using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered.W3x;
using OriginalPlugin = BinaryAssetBuilder.W3XCompiler.Plugin;
using RecoveredPlugin = Recovered.Compiler.W3x.Plugin;

internal static class W3xManagedOracle
{
    private static int checks, records, dispatches;
    private static void Check(bool value, string message)
    {
        if (!value) throw new Exception(message);
        checks++;
    }
    private static void Compare(IAssetBuilderPlugin original, IAssetBuilderPlugin recovered, uint id)
    {
        var expected = original.GetExtendedTypeInformation(id);
        var actual = recovered.GetExtendedTypeInformation(id);
        foreach (var field in typeof(ExtendedTypeInformation).GetFields())
            Check(Object.Equals(field.GetValue(expected), field.GetValue(actual)), "type " + id + " field " + field.Name);
        records++;
    }
    private static InstanceDeclaration Declaration(string type)
    {
        var xml = new XmlDocument();
        xml.LoadXml("<" + type + " id='ManagedOracle' xmlns='uri:ea.com:eala:asset'/>");
        var result = new InstanceDeclaration((AssetDeclarationDocument)null);
        result.XmlNode = xml.DocumentElement;
        return result;
    }
    private static string Failure(Action action)
    {
        try { action(); return "none"; }
        catch (Exception ex) { return ex.GetType().FullName; }
    }
    private sealed class CaptureBackend : IW3xAssetCompiler
    {
        public string Route;
        public InstanceDeclaration Input;
        public TargetPlatform Platform;
        public Exception Error;
        public readonly AssetBuffer Output = new AssetBuffer {
            InstanceData = new byte[] { 1, 2 }, RelocationData = new byte[] { 3 }, ImportsData = new byte[] { 4, 5 }
        };
        private AssetBuffer Run(string route, InstanceDeclaration input, TargetPlatform platform)
        {
            Route = route; Input = input; Platform = platform;
            if (Error != null) throw Error;
            return Output;
        }
        public AssetBuffer CompileAnimation(InstanceDeclaration input, TargetPlatform platform) { return Run("animation", input, platform); }
        public AssetBuffer CompileMesh(InstanceDeclaration input, TargetPlatform platform) { return Run("mesh", input, platform); }
    }
    private sealed class VersionOriginal : OriginalPlugin
    {
        public int Calls;
        public override uint GetVersionNumber() { Calls++; return 0xFEDCBA98u; }
    }
    private sealed class VersionRecovered : RecoveredPlugin
    {
        public int Calls;
        public override uint GetVersionNumber() { Calls++; return 0xFEDCBA98u; }
    }
    private sealed class InitializeOriginal : OriginalPlugin
    {
        public int Calls;
        public override void Initialize(object config, TargetPlatform platform) { Calls++; base.Initialize(config, platform); }
    }
    private sealed class InitializeRecovered : RecoveredPlugin
    {
        public int Calls;
        public override void Initialize(object config, TargetPlatform platform) { Calls++; base.Initialize(config, platform); }
    }
    private static void HashState(object plugin)
    {
        var field = plugin.GetType().GetField("m_HashCode", BindingFlags.Instance | BindingFlags.NonPublic);
        Check(field != null, "private hash field");
        Check((uint)field.GetValue(plugin) == HashProvider.GetTypeHash(plugin.GetType()), "runtime type/module hash algorithm");
    }
    private static int Main()
    {
        try
        {
            Check(IntPtr.Size == 4, "x86 host required");
            var original = new OriginalPlugin();
            var recovered = new RecoveredPlugin();
            Check(original.GetVersionNumber() == recovered.GetVersionNumber(), "constructor default platform");
            foreach (uint id in new uint[] { W3xCatalog.MeshTypeId, W3xCatalog.AnimationTypeId, 0u, UInt32.MaxValue })
                Compare(original, recovered, id);
            uint[] ids = Enumerable.Range(0, 4096).Select(i => unchecked((uint)i * 2654435761u))
                .Concat(new uint[] { W3xCatalog.MeshTypeId, W3xCatalog.AnimationTypeId, UInt32.MaxValue }).ToArray();
            foreach (int target in new int[] { 0, 1, 2, 99, -1, Int32.MinValue, Int32.MaxValue })
            {
                var platform = (TargetPlatform)target;
                original.Initialize(new object(), platform);
                recovered.Initialize(new object(), platform);
                HashState(original); HashState(recovered);
                Check(original.GetVersionNumber() == recovered.GetVersionNumber(), "version " + target);
                Check(original.GetAllTypesHash() == recovered.GetAllTypesHash(), "all types " + target);
                foreach (uint id in ids) Compare(original, recovered, id);
                var info = recovered.GetExtendedTypeInformation(W3xCatalog.MeshTypeId);
                info.TypeName = "mutated";
                Compare(original, recovered, W3xCatalog.MeshTypeId);
                original.ReInitialize(null, TargetPlatform.Win32);
                recovered.ReInitialize(null, TargetPlatform.Win32);
                Check(original.GetVersionNumber() == recovered.GetVersionNumber(), "ReInitialize resets platform");
                Console.WriteLine("PASS managed metadata platform " + target);
            }
            var vo = new VersionOriginal(); var vr = new VersionRecovered();
            foreach (uint id in new uint[] { 0u, W3xCatalog.MeshTypeId, W3xCatalog.AnimationTypeId, UInt32.MaxValue }) Compare(vo, vr, id);
            Check(vo.Calls == 2 && vr.Calls == vo.Calls, "virtual version getter only for known IDs");
            var io = new InitializeOriginal(); var ir = new InitializeRecovered();
            io.ReInitialize(null, TargetPlatform.Xbox360); ir.ReInitialize(null, TargetPlatform.Xbox360);
            Check(io.Calls == 1 && ir.Calls == io.Calls, "ReInitialize virtual dispatch");
            original.Dispose(); original.Dispose(); recovered.Dispose(); recovered.Dispose();
            original.Initialize(null, TargetPlatform.PlayStation3); recovered.Initialize(null, TargetPlatform.PlayStation3);
            Compare(original, recovered, W3xCatalog.AnimationTypeId);
            Check(Failure(() => original.ProcessInstance(null)) == Failure(() => recovered.ProcessInstance(null)), "null declaration rejection");
            var uninitialized = new InstanceDeclaration();
            Check(Failure(() => original.ProcessInstance(uninitialized)) == Failure(() => recovered.ProcessInstance(uninitialized)), "uninitialized declaration rejection");
            var unknown = Declaration("UnknownW3xType");
            TextWriter console = Console.Out;
            string expectedWarning, actualWarning;
            try
            {
                var writer = new StringWriter(); Console.SetOut(writer);
                Check(original.ProcessInstance(unknown) == null, "original unknown result"); expectedWarning = writer.ToString();
                writer = new StringWriter(); Console.SetOut(writer);
                Check(recovered.ProcessInstance(unknown) == null, "recovered unknown result"); actualWarning = writer.ToString();
            }
            finally { Console.SetOut(console); }
            Check(expectedWarning == actualWarning && expectedWarning.Length > 0, "exact unknown-type warning");
            // Known dispatch calls are independently tied to the original IL targets.
            byte[] processIl = typeof(OriginalPlugin).GetMethod("ProcessInstance").GetMethodBody().GetILAsByteArray();
            foreach (string name in new string[] { "HandleMesh", "HandleAnimation" })
            {
                byte[] token = BitConverter.GetBytes(typeof(OriginalPlugin).GetMethod(name).MetadataToken);
                Check(Enumerable.Range(0, processIl.Length - 4).Any(i => processIl[i] == 0x28 && token.SequenceEqual(processIl.Skip(i + 1).Take(4))), "original ProcessInstance calls " + name);
            }
            foreach (int target in new int[] { 0, 1, 2, 99 })
            {
                var backend = new CaptureBackend(); var plugin = new RecoveredPlugin(backend);
                plugin.Initialize(null, (TargetPlatform)target);
                foreach (string type in new string[] { "W3DMesh", "W3DAnimation" })
                {
                    var input = Declaration(type);
                    Check(Object.ReferenceEquals(plugin.ProcessInstance(input), backend.Output), "backend buffer identity");
                    Check(backend.Route == (type == "W3DMesh" ? "mesh" : "animation"), "backend route");
                    Check(Object.ReferenceEquals(input, backend.Input) && (int)backend.Platform == target, "backend input/platform identity");
                    dispatches++;
                }
                var error = new InvalidOperationException("backend-marker"); backend.Error = error;
                Exception observed = null;
                try { plugin.ProcessInstance(Declaration("W3DMesh")); } catch (Exception ex) { observed = ex; }
                Check(Object.ReferenceEquals(error, observed), "preserve backend exception");
            }
            var bare = new RecoveredPlugin();
            Check(Failure(() => bare.ProcessInstance(Declaration("W3DMesh"))) == typeof(NotSupportedException).FullName, "unrecovered mesh explicitly rejected");
            Check(Failure(() => bare.ProcessInstance(Declaration("W3DAnimation"))) == typeof(NotSupportedException).FullName, "unrecovered animation explicitly rejected");
            Check(!typeof(RecoveredPlugin).Assembly.GetReferencedAssemblies().Any(a => a.Name == "BinaryAssetBuilder.W3XCompiler"), "no original W3X assembly reference");
            Console.WriteLine("PASS W3X managed: {0} type records, {1} assertions, {2} backend dispatch checks; native outputs and SDK not validated", records, checks, dispatches);
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
