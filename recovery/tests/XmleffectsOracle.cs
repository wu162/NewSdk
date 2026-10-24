using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;
class XmleffectsOracle {
    const string Ns = "uri:ea.com:eala:asset";
    static int fixtures, comparisons, rejections, layouts;
    static InstanceDeclaration Declaration(XmlDocument xml, XmlNamespaceManager ns) {
        AssetDeclarationDocument document = new AssetDeclarationDocument();
        Type stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state, ns);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        InstanceDeclaration declaration = new InstanceDeclaration(document);
        declaration.XmlNode = xml.DocumentElement;
        return declaration;
    }
    static XmlDocument Xml(string text) { XmlDocument d = new XmlDocument(); d.LoadXml(text); return d; }
    static XmlNamespaceManager Namespaces(XmlDocument d) {
        XmlNamespaceManager ns = new XmlNamespaceManager(d.NameTable); ns.AddNamespace("ea", Ns); return ns;
    }
    static string FX(string attrs, string children) {
        return "<FXList xmlns='" + Ns + "' id='Oracle' " + attrs + ">" + children + "</FXList>";
    }
    static void Equal(string label, byte[] a, byte[] b) {
        if (a == null || b == null || a.Length != b.Length) throw new Exception(label + " length mismatch");
        for (int i = 0; i < a.Length; ++i) if (a[i] != b[i])
            throw new Exception(label + " byte=" + i + " original=" + a[i] + " recovered=" + b[i]);
        ++comparisons;
    }
    static uint Word(byte[] b, int at, bool big) {
        return big ? ((uint)b[at] << 24) | ((uint)b[at+1] << 16) | ((uint)b[at+2] << 8) | b[at+3] : BitConverter.ToUInt32(b, at);
    }
    static uint FloatBits(float x) { return BitConverter.ToUInt32(BitConverter.GetBytes(x), 0); }
    static AssetBuffer Test(IAssetBuilderPlugin original, string label, string text, TargetPlatform platform) {
        XmlDocument d = Xml(text); XmlNamespaceManager ns = Namespaces(d);
        InstanceDeclaration declaration = Declaration(d, ns);
        original.Initialize(null, platform);
        AssetBuffer a = original.ProcessInstance(declaration);
        AssetBuffer b = FXListCompiler.ProcessInstance(declaration, platform);
        Equal(label + "/instance", a.InstanceData, b.InstanceData);
        Equal(label + "/relocations", a.RelocationData, b.RelocationData);
        Equal(label + "/imports", a.ImportsData, b.ImportsData);
        AssetBuffer c = FXListCompiler.CompileXml(d.DocumentElement, ns, platform != TargetPlatform.Win32);
        Equal(label + "/direct-instance", b.InstanceData, c.InstanceData);
        Equal(label + "/direct-relocations", b.RelocationData, c.RelocationData);
        Equal(label + "/direct-imports", b.ImportsData, c.ImportsData);
        bool big = platform != TargetPlatform.Win32;
        if (b.InstanceData.Length != 28 || Word(b.InstanceData, 0, big) != 0 ||
            Word(b.InstanceData, 16, big) != 0 || Word(b.InstanceData, 20, big) != 0 ||
            b.InstanceData[26] != 0 || b.InstanceData[27] != 0 ||
            b.RelocationData.Length != 0 || b.ImportsData.Length != 0)
            throw new Exception(label + " independent root/empty-list layout failed");
        ++fixtures; ++layouts; return b;
    }
    static void Layout(AssetBuffer b, bool big, float cull, int min, int max, byte play, byte tail) {
        if (Word(b.InstanceData, 4, big) != FloatBits(cull) ||
            Word(b.InstanceData, 8, big) != unchecked((uint)min) ||
            Word(b.InstanceData, 12, big) != unchecked((uint)max) ||
            b.InstanceData[24] != play || b.InstanceData[25] != tail)
            throw new Exception("Independent scalar/endian layout mismatch");
        ++layouts;
    }
    static void Reject(string text, bool particle) {
        XmlDocument d = Xml(text);
        try {
            if (particle) FXParticleSystemTemplateCompiler.CompileXml(d.DocumentElement, Namespaces(d), false);
            else FXListCompiler.CompileXml(d.DocumentElement, Namespaces(d), false);
        } catch (NotSupportedException) { ++rejections; return; }
        throw new Exception("Expected explicit refusal: " + text);
    }
    static int Main(string[] args) {
        try {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Assembly assembly = Assembly.LoadFrom(args[0]);
            IAssetBuilderPlugin original = (IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.XmlCompiler.Plugin"));
            foreach (int platformId in new int[] {0,1,2,99}) {
                TargetPlatform platform = (TargetPlatform)platformId;
                int startFixtures=fixtures, startComparisons=comparisons, startRejections=rejections, startLayouts=layouts;
                bool big = platformId != 0;
                Layout(Test(original, "default", FX("", ""), platform), big, 0, 0, 0, 0, 0);
                Layout(Test(original, "distinct", FX("CullTracking='1500ms' CullTrackingMin='-17' CullTrackingMax='0x23' PlayEvenIfShrouded='true' Tailorable='1'", "<NuggetList/>"), platform), big,
                    (float)((double)1500.0f * (double)0.001f), -17, 35, 1, 1);
                Layout(Test(original, "seconds", FX("CullTracking='2.25s' CullTrackingMin='010' CullTrackingMax='-2147483648'", ""), platform), big, 2.25f, 8, Int32.MinValue, 0, 0);
                string[] values = {"", "0", "-0", "1", "-17.25", "33.3333333333", "1e-30", "1e30", "2d1", "2D-1", "garbage", "25,5", " 25.75junk", "1.5s", "1500ms", "-25ms", "ms2", "1s2ms", "0x10", "010", "2147483648", "-2147483649"};
                foreach (string value in values) {
                    Test(original, "time:" + value, FX("CullTracking='" + value + "'", ""), platform);
                    Test(original, "min:" + value, FX("CullTrackingMin='" + value + "'", ""), platform);
                    Test(original, "max:" + value, FX("CullTrackingMax='" + value + "'", ""), platform);
                }
                foreach (string value in new string[] {"", "1", "0", "true", "True", "TRUE", "t", "T", "false", "yes", " true", "10", "trailing"}) {
                    Test(original, "play:" + value, FX("PlayEvenIfShrouded='" + value + "'", ""), platform);
                    Test(original, "tail:" + value, FX("Tailorable='" + value + "'", ""), platform);
                }
                Test(original, "prefix", "<e:FXList xmlns:e='" + Ns + "' id='Oracle' CullTracking='3s'><e:NuggetList/></e:FXList>", platform);
                Test(original, "comments", FX("", "<!-- comment -->\n<NuggetList> </NuggetList>"), platform);
                Reject(FX("Unknown='1'", ""), false);
                Reject(FX("tailorable='true'", ""), false);
                Reject(FX("xmlns:x='other' x:CullTracking='1'", ""), false);
                Reject(FX("", "<Unknown/>"), false);
                Reject(FX("", "<NuggetList><Sound Type='SoundFXNugget'/></NuggetList>"), false);
                Reject(FX("", "<NuggetList/><NuggetList/>"), false);
                Reject(FX("", "<NuggetList Unknown='1'/>"), false);
                Reject(FX("", "<NuggetList xmlns=''/>"), false);
                Reject(FX("", "<NuggetList>text</NuggetList>"), false);
                Reject(FX("", "text"), false);
                Reject("<FXList xmlns='other'/>", false);
                Reject("<Wrong xmlns='" + Ns + "'/>", false);
                Reject("<FXParticleSystemTemplate xmlns='" + Ns + "'/>", true);
                Console.WriteLine("PASS platform={0} fixtures={1} threeArrayComparisons={2} directArrayComparisons={3} rejections={4} independentLayouts={5}",
                    platformId, fixtures-startFixtures, (comparisons-startComparisons)/2, (comparisons-startComparisons)/2, rejections-startRejections, layouts-startLayouts);
            }
            Console.WriteLine("RESULT fixtures={0} arrayComparisons={1} rejections={2} independentLayouts={3}", fixtures, comparisons, rejections, layouts);
            return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
