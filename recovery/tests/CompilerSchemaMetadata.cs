using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

public static class CompilerSchemaMetadata
{
    const string Switch = "NewSdk.Compiler.DisableSharedSchemaMetadata";
    const string Ns = "uri:ea.com:eala:asset";
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> measurements = new List<object>();
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer();
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static string Shape(SchemaSet schemas)
    {
        var map = (IDictionary<string, StringCollection>)typeof(SchemaSet).GetField("_InheritanceMap", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(schemas);
        return Json.Serialize(new {
            inheritance = map.Select(item => new { name = item.Key, children = item.Value.Cast<string>().ToArray() }).ToArray(),
            dependencies = schemas.AssetDependencies == null ? null : schemas.AssetDependencies.Select(item => new { id = item.Key, count = item.Value }).ToArray()
        });
    }
    static SchemaSet Create(SchemaSet.MetadataCache cache, bool dependencies, bool disabled = false)
    {
        AppContext.SetSwitch(Switch, disabled);
        try { return new SchemaSet(dependencies, cache); }
        finally { AppContext.SetSwitch(Switch, false); }
    }
    static void Built(SchemaSet schemas, string context)
    {
        Require(schemas.MetadataBuildCount == 1 && schemas.MetadataHitCount == 0, context + " must build its metadata");
        Require(schemas.GetPreparationMetrics().Any(item => item.stage == "schemaMetadataBuild" && item.calls == 1 && item.failedCalls == 0), context + " build interval missing");
        Require(!schemas.GetPreparationMetrics().Any(item => item.stage == "schemaMetadataRestore"), context + " must not count a lookup miss as restore");
    }
    static void Hit(SchemaSet schemas, string context)
    {
        Require(schemas.MetadataBuildCount == 0 && schemas.MetadataHitCount == 1 && schemas.SharedMetadataEnabled, context + " must restore its metadata");
        Require(schemas.GetPreparationMetrics().Any(item => item.stage == "schemaMetadataRestore" && item.calls == 1 && item.failedCalls == 0), context + " restore interval missing");
        Require(!schemas.GetPreparationMetrics().Any(item => item.stage == "schemaMetadataBuild"), context + " repeated metadata traversal");
    }
    static string Types(string parent)
    {
        return "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:a='" + Ns + "' xmlns:xas='uri:ea.com:eala:asset:schema' targetNamespace='" + Ns + "' elementFormDefault='qualified'>" +
            "<xs:complexType name='BaseAssetType'/><xs:complexType name='BaseInheritableAsset'><xs:complexContent><xs:extension base='a:BaseAssetType'/></xs:complexContent></xs:complexType>" +
            "<xs:simpleType name='AssetReference'><xs:restriction base='xs:string'/></xs:simpleType><xs:simpleType name='WeakReference'><xs:restriction base='a:AssetReference'/></xs:simpleType><xs:simpleType name='FileReference'><xs:restriction base='xs:string'/></xs:simpleType>" +
            "<xs:complexType name='TargetA'><xs:complexContent><xs:extension base='a:BaseInheritableAsset'/></xs:complexContent></xs:complexType><xs:complexType name='TargetB'><xs:complexContent><xs:extension base='a:BaseInheritableAsset'/></xs:complexContent></xs:complexType>" +
            "<xs:complexType name='Child'><xs:complexContent><xs:extension base='a:" + parent + "'><xs:sequence><xs:element name='Ref' type='a:AssetReference' xas:refType='TargetA'/></xs:sequence></xs:extension></xs:complexContent></xs:complexType></xs:schema>";
    }
    static string Root(string include)
    {
        return "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='" + Ns + "'><xs:include schemaLocation='" + include + "'/></xs:schema>";
    }
    static void SmallCases(string work)
    {
        string root = Path.Combine(work, "root.xsd"), include = Path.Combine(work, "types.xsd");
        File.WriteAllText(root, Root("types.xsd"), new UTF8Encoding(false));
        File.WriteAllText(include, Types("TargetA"), new UTF8Encoding(false));
        Settings.Current = new Settings { SchemaPath = root, StringHashBinDescriptors = new StringHashBinDescriptor[0] };
        var cache = new SchemaSet.MetadataCache();
        var cold = Create(cache, true); var warm = Create(cache, true); var fallback = Create(cache, true, true);
        Built(cold, "small cold"); Hit(warm, "small warm"); Built(fallback, "small fallback");
        Require(!fallback.SharedMetadataEnabled, "Fallback must disable sharing");
        string original = Shape(cold);
        Require(original == Shape(warm) && original == Shape(fallback), "Cold/warm/fallback tables or ordering differ");
        Require(!Object.ReferenceEquals(cold.Schemas, warm.Schemas) && !Object.ReferenceEquals(cold.XmlBaseAssetType, warm.XmlBaseAssetType), "Workers share Framework schema objects");
        Require(!Object.ReferenceEquals(cold.GetDerivedTypes("BaseAssetType"), warm.GetDerivedTypes("BaseAssetType")) && !Object.ReferenceEquals(cold.AssetDependencies, warm.AssetDependencies), "Workers share mutable metadata collections");
        cold.GetDerivedTypes("BaseAssetType").Clear(); cold.AssetDependencies.Clear();
        warm.GetDerivedTypes("BaseAssetType").Add("Injected"); warm.AssetDependencies.Clear();
        var isolated = Create(cache, true); Hit(isolated, "after worker mutation");
        Require(Shape(isolated) == original, "Mutable worker maps contaminated the cached snapshot");
        DateTime time = File.GetLastWriteTimeUtc(include); long length = new FileInfo(include).Length;
        File.WriteAllText(include, Types("TargetB"), new UTF8Encoding(false)); File.SetLastWriteTimeUtc(include, time);
        Require(File.GetLastWriteTimeUtc(include) == time && new FileInfo(include).Length == length, "Include mutation must preserve exact mtime and length");
        var changed = Create(cache, true); Built(changed, "equal-mtime changed include");
        Require(Shape(changed) != original && Shape(changed) == Shape(Create(cache, true, true)), "Full include content identity did not invalidate metadata");
        Hit(Create(cache, true), "changed generation warm");
        File.WriteAllText(include, "<broken", new UTF8Encoding(false));
        bool rejected = false;
        try { Create(cache, true); } catch (BinaryAssetBuilderException error) { rejected = error.ErrorCode == ErrorCode.ReadingSchema; }
        Require(rejected, "Invalid schema was accepted");
        File.WriteAllText(include, Types("TargetB"), new UTF8Encoding(false));
        Hit(Create(cache, true), "repair preserves last successful generation");
        Built(Create(cache, false), "dependency mode change");
        var noDependencies = Create(cache, false); Hit(noDependencies, "dependency mode warm");
        Require(noDependencies.AssetDependencies == null && Shape(noDependencies) == Shape(Create(cache, false, true)), "Dependency-disabled table differs from fallback");
        Built(Create(cache, true), "single-entry eviction");
        CultureInfo culture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture.Name == "tr-TR" ? "en-US" : "tr-TR");
            var differentCulture = Create(cache, true); Built(differentCulture, "culture change");
            Require(Shape(differentCulture) == Shape(Create(cache, true, true)), "Culture-specific metadata differs from fallback");
        }
        finally { Thread.CurrentThread.CurrentCulture = culture; }
        Built(Create(cache, true), "restored culture");
        string root2 = Path.Combine(work, "root2.xsd"); File.WriteAllText(root2, Root("types.xsd"), new UTF8Encoding(false));
        Settings.Current.SchemaPath = root2; Built(Create(cache, true), "different root");
        Settings.Current.SchemaPath = root;
        cache.Clear(); Built(Create(cache, true), "cleared cache");
        Settings.Current.StringHashBinDescriptors = new[] { new StringHashBinDescriptor { SchemaTypeName = "AssetReference", BinName = "Changed", CaseSensitivity = true } };
        Built(Create(cache, true), "hashable configuration change");
        tests.Add("full-content-include-key-mtime-size-countDependencies-culture-root-hashable-settings-and-single-entry-clear");
        tests.Add("worker-Framework-and-mutable-map-isolation-fallback-ordering-failure-does-not-publish");
    }
    static void SdkCases(string sdk)
    {
        var config = new XmlDocument(); config.Load(Path.Combine(sdk, "tools/BinaryAssetBuilder.exe.config"));
        Settings.Current = (Settings)new SerializedSectionHandler().Create(null, null, config.SelectSingleNode("/configuration/assetbuilder"));
        Settings.Current.SchemaPath = Path.Combine(sdk, "Schemas/xsd/CnC3Types.xsd");
        var cache = new SchemaSet.MetadataCache(); string expected = null;
        for (int repeat = 0; repeat < 3; repeat++)
        {
            cache.Clear();
            // Alternate fallback placement to reduce a fixed-order timing bias.
            foreach (string mode in repeat % 2 == 0 ? new[] { "cold", "warm", "fallback" } : new[] { "fallback", "cold", "warm" })
            {
                var timer = Stopwatch.StartNew(); var schemas = Create(cache, true, mode == "fallback"); timer.Stop();
                if (mode == "warm") Hit(schemas, "SDK warm"); else Built(schemas, "SDK " + mode);
                string shape = Shape(schemas); if (expected == null) expected = shape;
                Require(shape == expected, "SDK complete inherited/dependency table differs across cold/warm/fallback");
                measurements.Add(new { repeat, mode, elapsedMs = timer.Elapsed.TotalMilliseconds, stages = schemas.GetPreparationMetrics(),
                    filesRead = schemas.SchemaFileReadCount, bytesRead = schemas.SchemaContentBytesRead, builds = schemas.MetadataBuildCount, hits = schemas.MetadataHitCount });
            }
        }
        tests.Add("real-SDK-complete-metadata-tables-cold-warm-fallback-and-three-interleaved-preparation-measurements");
    }
    public static int Main(string[] args)
    {
        int code = 0; object result; Settings previous = Settings.Current;
        bool previousSwitch; AppContext.TryGetSwitch(Switch, out previousSwitch);
        try
        {
            if (args.Length != 5 || IntPtr.Size != 4) throw new Exception("Expected work result sourceCommit runId sdk (x86)");
            Directory.CreateDirectory(args[0]); SmallCases(Path.GetFullPath(args[0])); SdkCases(Path.GetFullPath(args[4]));
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), measurements = measurements.ToArray(),
                scope = "Build-owned immutable inheritance/dependency metadata only; Framework schemas still read and compile separately; logical reads and preparation timing are not full-project speedup" };
            Console.WriteLine("PASS SchemaMetadata: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, tests = tests.ToArray(), measurements = measurements.ToArray(), error = error.ToString() };
        }
        finally { Settings.Current = previous; AppContext.SetSwitch(Switch, previousSwitch); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result));
        return code;
    }
}
