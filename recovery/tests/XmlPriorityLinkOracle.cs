using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

// Combined assembly smoke. Group differential matrices remain mandatory.
class XmlPriorityLinkOracle
{
    const string AssetNamespace = "uri:ea.com:eala:asset";
    sealed class Root
    {
        public readonly string Name;
        public readonly uint TypeId;
        public readonly string Compiler;
        public Root(string name, uint typeId, bool ai)
        {
            Name = name; TypeId = typeId;
            Compiler = ai ? "Recovered.XmlaiCompiler" : "Recovered." + name + "Compiler";
        }
    }
    static readonly Root[] Roots = new Root[] {
        new Root("AttributeModifier", 3319822471u, false),
        new Root("FXList", 2254974584u, false),
        new Root("SpecialPowerTemplate", 2178440954u, false),
        new Root("ObjectCreationList", 3899542881u, false),
        new Root("LocomotorTemplate", 3972178387u, false),
        new Root("LogicCommandSet", 3959844197u, false),
        new Root("FXParticleSystemTemplate", 686292351u, false),
        new Root("UpgradeTemplate", 3786401627u, false),
        new Root("AIPersonalityDefinition", 3604279694u, true),
        new Root("AIStrategicStateDefinition", 607123156u, true),
        new Root("AIMicroManagerData", 2423780215u, true),
        new Root("AIBudgetStateDefinition", 2893598307u, true),
        new Root("AIStateDefinition", 3239935619u, true),
        new Root("AITargetingHeuristic", 926814458u, true),
        new Root("LogicCommand", 2101756272u, false),
        new Root("PlayerTech", 2131237081u, false),
        new Root("PlayerTechRank", 1802643394u, false),
        new Root("PlayerTechUpgradeBinding", 2140592493u, false),
        new Root("ExperienceLevelTemplate", 1874610847u, false),
        new Root("Weather", 2430161325u, false),
        new Root("WaterTransparency", 1479531562u, false),
        new Root("WaterSetting", 2728605516u, false),
        new Root("Environment", 680780553u, false),
        new Root("LightingSettings", 3169690211u, false),
        new Root("WaterTextureList", 2995571269u, false),
        new Root("AIData", 3827723072u, false),
        new Root("UnitAISettings", 2964891936u, false),
        new Root("PlayerTemplate", 4063352333u, false),
        new Root("MultiplayerSettings", 191082224u, false),
        new Root("AIBuildStateDefinition", 1644972043u, false),
        new Root("AIStrategicStateName", 2010562192u, false),
    };
    static readonly int[] Platforms = new int[] { 0, 1, 2, 99 };
    static readonly Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
    static readonly Dictionary<string, int> Coverage = new Dictionary<string, int>();
    static int fixtures, comparisons, completionFixtures, completionComparisons;
    static string current = "metadata preflight";

    static string Json(string value)
    {
        if (value == null) return "null";
        return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t") + "\"";
    }
    static void WriteResult(string path, bool succeeded, string sourceCommit, string runId, string failure)
    {
        StringBuilder text = new StringBuilder();
        text.Append("{\"succeeded\":").Append(succeeded ? "true" : "false");
        text.Append(",\"sourceCommit\":").Append(Json(sourceCommit)).Append(",\"runId\":").Append(Json(runId));
        text.Append(",\"scope\":\"combined link/default-root and completion smoke only\"");
        text.Append(",\"types\":").Append(Methods.Count).Append(",\"fixtures\":").Append(fixtures);
        text.Append(",\"comparisons\":").Append(comparisons).Append(",\"platforms\":[0,1,2,99]");
        text.Append(",\"completionFixtures\":").Append(completionFixtures).Append(",\"completionComparisons\":").Append(completionComparisons);
        text.Append(",\"typeCoverage\":{");
        for (int i = 0; i < Roots.Length; ++i) {
            if (i != 0) text.Append(',');
            text.Append(Json(Roots[i].Name)).Append(':').Append(Coverage.ContainsKey(Roots[i].Name) ? Coverage[Roots[i].Name] : 0);
        }
        text.Append("},\"rootTypeIds\":{");
        for (int i = 0; i < Roots.Length; ++i) {
            if (i != 0) text.Append(',');
            text.Append(Json(Roots[i].Name)).Append(':').Append(Roots[i].TypeId.ToString(CultureInfo.InvariantCulture));
        }
        text.Append("},\"compilerClasses\":{");
        for (int i = 0; i < Roots.Length; ++i) {
            if (i != 0) text.Append(',');
            text.Append(Json(Roots[i].Name)).Append(':').Append(Json(Roots[i].Compiler));
        }
        text.Append("},\"failure\":").Append(Json(failure)).Append('}');
        File.WriteAllText(path, text.ToString());
    }
    static void Manifest(string path)
    {
        Dictionary<string, uint> supplied = new Dictionary<string, uint>();
        foreach (string line in File.ReadAllLines(path)) {
            if (line.Trim().Length == 0) continue;
            string[] pair = line.Split('\t');
            if (pair.Length != 2) throw new Exception("Invalid expected-root-types manifest row");
            supplied.Add(pair[0], UInt32.Parse(pair[1], CultureInfo.InvariantCulture));
        }
        if (Roots.Length != 31 || supplied.Count != 31) throw new Exception("Exactly 31 recovered roots are required");
        foreach (Root root in Roots) {
            uint value;
            if (!supplied.TryGetValue(root.Name, out value) || value != root.TypeId)
                throw new Exception("Expected TypeId map mismatch: " + root.Name);
            Coverage.Add(root.Name, 0);
        }
    }
    static void Preflight(Assembly recovered)
    {
        // Force CLR metadata loading for the entire combined image, not only the first route.
        Type[] exported = recovered.GetExportedTypes();
        HashSet<string> publicNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (Type type in exported)
            if (!publicNames.Add(type.FullName)) throw new Exception("Duplicate exported type: " + type.FullName);
        foreach (Root root in Roots) {
            Type compiler = recovered.GetType(root.Compiler, false, false);
            if (compiler == null || !compiler.IsVisible)
                throw new Exception("Missing required formal compiler class: " + root.Compiler + " for " + root.Name + "; Draft/#if0 snapshots do not count");
            MethodInfo method = compiler.GetMethod("ProcessInstance", BindingFlags.Public | BindingFlags.Static,
                null, new Type[] { typeof(InstanceDeclaration), typeof(TargetPlatform) }, null);
            if (method == null || method.ContainsGenericParameters || method.ReturnType != typeof(AssetBuffer))
                throw new Exception("Required exact signature absent: AssetBuffer " + root.Compiler + ".ProcessInstance(InstanceDeclaration, TargetPlatform)");
            if (method.DeclaringType != compiler) throw new Exception("Inherited compiler route is not an exact declared interface: " + root.Compiler);
            Methods.Add(root.Name, method);
            Console.WriteLine("ROUTE root={0} TypeId={1} compiler={2}", root.Name, root.TypeId, root.Compiler);
        }
        if (Methods.Count != 31) throw new Exception("Metadata preflight did not resolve all 31 roots");
    }
    static InstanceDeclaration Declaration(XmlDocument xml, XmlNamespaceManager namespaces)
    {
        AssetDeclarationDocument document = new AssetDeclarationDocument();
        Type stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state, namespaces);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        InstanceDeclaration declaration = new InstanceDeclaration(document);
        declaration.XmlNode = xml.DocumentElement;
        return declaration;
    }
    static void Equal(string buffer, byte[] original, byte[] recovered, bool completion)
    {
        if (original == null || recovered == null || original.Length != recovered.Length)
            throw new Exception(current + "/" + buffer + " lengths original=" + (original == null ? -1 : original.Length) + " recovered=" + (recovered == null ? -1 : recovered.Length));
        for (int i = 0; i < original.Length; ++i)
            if (original[i] != recovered[i])
                throw new Exception(current + "/" + buffer + " byte=" + i + " original=" + original[i] + " recovered=" + recovered[i]);
        if (completion) ++completionComparisons; else ++comparisons;
    }
    static void Test(IAssetBuilderPlugin original, Root root, int platform, string body = null)
    {
        bool completion = body != null;
        current = root.Name + (completion ? " completion" : " defaults") + " platform=" + platform;
        XmlDocument xml = new XmlDocument();
        // Unprefixed canonical root is essential: InstanceDeclaration may hash root.Name.
        xml.LoadXml("<" + root.Name + " xmlns='" + AssetNamespace + "' id='PriorityLinkSmoke'>" + (body ?? "") + "</" + root.Name + ">");
        XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable);
        namespaces.AddNamespace("ea", AssetNamespace);
        InstanceDeclaration declaration = Declaration(xml, namespaces);
        if (declaration.Handle.TypeId != root.TypeId)
            throw new Exception(current + " canonical declaration Handle.TypeId=" + declaration.Handle.TypeId + " expected=" + root.TypeId);
        original.Initialize(null, (TargetPlatform)platform);
        AssetBuffer expected = original.ProcessInstance(declaration);
        AssetBuffer actual = (AssetBuffer)Methods[root.Name].Invoke(null, new object[] { declaration, (TargetPlatform)platform });
        if (expected == null || actual == null) throw new Exception(current + " null AssetBuffer");
        // Reject an original unknown-TypeId empty fallback masquerading as a successful route.
        if (expected.InstanceData == null || expected.InstanceData.Length == 0)
            throw new Exception(current + " original route returned an empty instance");
        Equal("InstanceData", expected.InstanceData, actual.InstanceData, completion);
        Equal("RelocationData", expected.RelocationData, actual.RelocationData, completion);
        Equal("ImportsData", expected.ImportsData, actual.ImportsData, completion);
        if (completion) ++completionFixtures; else { ++fixtures; ++Coverage[root.Name]; }
        Console.WriteLine("PASS root={0} TypeId={1} compiler={2} platform={3} fixtures=1 comparisons=3 instance={4} relocations={5} imports={6}",
            root.Name, root.TypeId, root.Compiler, platform, actual.InstanceData.Length, actual.RelocationData.Length, actual.ImportsData.Length);
    }
    static void Completion(IAssetBuilderPlugin original, int platform)
    {
        Root particle = Roots[6], effects = Roots[1];
        // All eight newly restored module TypeIds must resolve in the combined image.
        foreach (uint id in new uint[] { 2693838876u, 2933826200u, 234581578u, 2248017887u,
            3036920908u, 3408220251u, 1075723135u, 3465979181u })
            Test(original, particle, platform, "<Physics><Anything TypeId='" + id.ToString(CultureInfo.InvariantCulture) + "'/></Physics>");
        Test(original, effects, platform,
            "<NuggetList><Anything TypeId='99'><SecondaryObjectFilter/><SourceObjectFilter/></Anything>" +
            "<Anything TypeId='503892219' Value='Asset\\17'><SourceObjectFilter/></Anything></NuggetList>");
        Test(original, particle, platform,
            "<Physics><Anything TypeId='1075723135' EventFX='Physics\\2'/></Physics>" +
            "<Draw><Anything TypeId='2248017887'/></Draw>" +
            "<Event><Anything TypeId='2693838876' EventFX='Event\\3'/>" +
            "<Anything TypeId='3465979181'><EventTime Low='1' High='5'/></Anything></Event>" +
            "<Update><Anything TypeId='234581578'/></Update>");
    }
    static int Main(string[] args)
    {
        bool succeeded = false;
        string failure = null;
        try {
            if (args.Length != 6) throw new Exception("Usage: XmlPriorityLinkOracle original.dll recovered.dll result.json expected.tsv sourceCommit runId");
            if (IntPtr.Size != 4) throw new Exception("Priority link oracle requires an x86 process");
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Manifest(args[3]);
            Assembly recovered = Assembly.LoadFrom(args[1]);
            Preflight(recovered);
            Assembly source = Assembly.LoadFrom(args[0]);
            Type pluginType = source.GetType("BinaryAssetBuilder.XmlCompiler.Plugin", true, false);
            IAssetBuilderPlugin original = (IAssetBuilderPlugin)Activator.CreateInstance(pluginType);
            foreach (int platform in Platforms) {
                int beforeFixtures = fixtures, beforeComparisons = comparisons;
                foreach (Root root in Roots) Test(original, root, platform);
                Completion(original, platform);
                Console.WriteLine("PASS platform={0} types=31 fixtures={1} comparisons={2}", platform, fixtures-beforeFixtures, comparisons-beforeComparisons);
            }
            if (fixtures != 124 || comparisons != 372 || completionFixtures != 40 || completionComparisons != 120 || Methods.Count != 31) throw new Exception("Incomplete priority smoke matrix");
            foreach (Root root in Roots) if (Coverage[root.Name] != 4) throw new Exception("Incomplete root coverage: " + root.Name);
            succeeded = true;
            Console.WriteLine("PASS XmlPriorityLink types=31 fixtures=124 comparisons=372 completionFixtures=40 completionComparisons=120 platforms=0/1/2/99; link smoke only");
            return 0;
        } catch (Exception error) {
            failure = current + ": " + error.ToString();
            Console.Error.WriteLine("FAIL " + failure);
            ReflectionTypeLoadException metadata = error as ReflectionTypeLoadException;
            if (metadata != null) foreach (Exception loader in metadata.LoaderExceptions) Console.Error.WriteLine(loader);
            TargetInvocationException invocation = error as TargetInvocationException;
            if (invocation != null && invocation.InnerException != null) Console.Error.WriteLine(invocation.InnerException);
            return 1;
        } finally {
            if (args.Length >= 3) WriteResult(args[2], succeeded, args.Length > 4 ? args[4] : "", args.Length > 5 ? args[5] : "", failure);
        }
    }
}


