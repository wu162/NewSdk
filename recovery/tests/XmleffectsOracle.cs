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
        InstanceDeclaration originalDeclaration = declaration;
        bool prefixed = d.DocumentElement.Prefix.Length != 0;
        if (prefixed) {
            XmlDocument canonical = new XmlDocument();
            XmlElement root = canonical.CreateElement(d.DocumentElement.LocalName, Ns);
            foreach (XmlAttribute attr in d.DocumentElement.Attributes) root.Attributes.Append((XmlAttribute)canonical.ImportNode(attr, true));
            foreach (XmlNode child in d.DocumentElement.ChildNodes) root.AppendChild(canonical.ImportNode(child, true));
            canonical.AppendChild(root);
            originalDeclaration = Declaration(canonical, Namespaces(canonical));
            if (declaration.Handle.TypeId != 2254974584u) {
                try { FXListCompiler.ProcessInstance(declaration, platform); throw new Exception("Prefixed wrong TypeId was accepted"); }
                catch (NotSupportedException) { ++rejections; }
            }
        }
        original.Initialize(null, platform);
        AssetBuffer a = original.ProcessInstance(originalDeclaration);
        AssetBuffer b = prefixed ? FXListCompiler.CompileXml(d.DocumentElement, ns, platform != TargetPlatform.Win32) : FXListCompiler.ProcessInstance(declaration, platform);
        Equal(label + "/instance", a.InstanceData, b.InstanceData);
        Equal(label + "/relocations", a.RelocationData, b.RelocationData);
        Equal(label + "/imports", a.ImportsData, b.ImportsData);
        AssetBuffer c = FXListCompiler.CompileXml(d.DocumentElement, ns, platform != TargetPlatform.Win32);
        Equal(label + "/direct-instance", b.InstanceData, c.InstanceData);
        Equal(label + "/direct-relocations", b.RelocationData, c.RelocationData);
        Equal(label + "/direct-imports", b.ImportsData, c.ImportsData);
        bool big = platform != TargetPlatform.Win32;
        int expectedCount = 0;
        XmlNode listNode = null;
        foreach (XmlNode child in d.DocumentElement.ChildNodes) if (child.NodeType == XmlNodeType.Element && child.LocalName == "NuggetList") listNode = child;
        if (listNode != null) foreach (XmlNode child in listNode.ChildNodes) if (child.NodeType == XmlNodeType.Element) ++expectedCount;
        if (b.InstanceData.Length < 28 || Word(b.InstanceData, 0, big) != 0 ||
            Word(b.InstanceData, 16, big) != expectedCount || Word(b.InstanceData, 20, big) != (expectedCount == 0 ? 0u : 28u) ||
            b.InstanceData[26] != 0 || b.InstanceData[27] != 0)
            throw new Exception(label + " independent FXList root layout failed");
        if (expectedCount == 0 && (b.InstanceData.Length != 28 || b.RelocationData.Length != 0 || b.ImportsData.Length != 0))
            throw new Exception(label + " independent empty FXList layout failed");
        if (expectedCount > 0) {
            for (int n=0;n<expectedCount;++n) {
                uint at=Word(b.InstanceData,28+4*n,big);
                if (at < 28+4*expectedCount || at > b.InstanceData.Length-44 || Word(b.InstanceData,(int)at,big)==0)
                    throw new Exception(label + " independent nugget pointer/type layout failed");
            }
        }
        Tables(label,b,big);
        ++fixtures; ++layouts; return b;
    }

    static void Tables(string label, AssetBuffer buffer, bool big) {
        foreach (byte[] table in new byte[][] {buffer.RelocationData,buffer.ImportsData}) {
            if (table.Length == 0) continue;
            if ((table.Length & 3) != 0 || Word(table,table.Length-4,big) != UInt32.MaxValue) throw new Exception(label + " table terminator");
            for(int i=0;i<table.Length-4;i+=4) {
                uint at=Word(table,i,big);
                if (at > buffer.InstanceData.Length-4) throw new Exception(label + " table location");
                if (Object.ReferenceEquals(table,buffer.RelocationData) && Word(buffer.InstanceData,(int)at,big)>=buffer.InstanceData.Length)
                    throw new Exception(label + " relocation target");
            }
        }
    }


    static string Nugget(uint typeId, string attrs, string children) {
        return "<Nugget TypeId='"+typeId.ToString(CultureInfo.InvariantCulture)+"' "+attrs+">"+children+"</Nugget>";
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
            FXListCompiler.CompileXml(d.DocumentElement, Namespaces(d), false);
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

                uint[] nuggetIds={503892219u,638391350u,4011332978u,2424288100u,4234415617u,2263385183u,904475169u,2386148390u,4293352609u,2106748504u,3768725989u,138194854u};
                string[] nuggetAttrs={
                    "Value='Audio\\0'",
                    "Event='Force\\17'",
                    "Type='STRONG'",
                    "EvaEventOwner='Eva\\0' EvaEventAlly='Eva\\1' EvaEventEnemy='Eva\\2'",
                    "FX='Fx\\-1' Bone='Bone_A'",
                    "Model='Model\\5' RandomlyRotate='true' ExpireTimer='1500ms'",
                    "LaserTemplate='MiXeDObject' LaserBackwards='true'",
                    "Thing='RayObject'",
                    "Radius='12.5' RadiusAsPercentOfObjectSize='33.3' IncreaseTime='0x10' DecreaseTime='-1'",
                    "PreColorTime='250ms' PostColorTime='1.5s' SustainedColorTime='2.25s' Frequency='3.5' Amplitude='4.75'",
                    "Radius='32.5' Duration='200ms' Amplitude='90d'",
                    "Type='SCORCH_13' Radius='42.5'"
                };
                string[] nuggetChildren={"","","","","","","<Offset x='1.25' y='-2.5' z='3.75'/>","<PrimaryOffset x='1' y='2' z='3'/><SecondaryOffset x='4' y='5' z='6'/>","<Color R='0.1' G='0.2' B='0.3'/>","<Color R='0.4' G='0.5' B='0.6'/>","","<RandomRange Low='SCORCH_2' High='SCORCH_15'/>"};
                for(int i=0;i<nuggetIds.Length;++i) {
                    Test(original,"nugget-default-"+i,FX("","<NuggetList>"+Nugget(nuggetIds[i],"","")+"</NuggetList>"),platform);
                    Test(original,"nugget-fields-"+i,FX("","<NuggetList>"+Nugget(nuggetIds[i],nuggetAttrs[i]+" StopIfPlayed='true' Weather='RAINY' OnlyIfOnLand='1' PlayIfSourceIsStealthed='T'",nuggetChildren[i])+"</NuggetList>"),platform);
                    Test(original,"nugget-disabled-"+i,FX("","<NuggetList>"+Nugget(nuggetIds[i],"Weather='unknown'","<SourceMustNotHaveBeenDisabledThisFrameByType>EMP</SourceMustNotHaveBeenDisabledThisFrameByType><SourceMustNotHaveBeenDisabledThisFrameByType>unknown</SourceMustNotHaveBeenDisabledThisFrameByType>")+"</NuggetList>"),platform);
                }
                foreach(string value in values) {
                    Test(original,"bone:"+value,FX("","<NuggetList>"+Nugget(4234415617u,"Bone='"+value+"'","")+"</NuggetList>"),platform);
                    Test(original,"camera:"+value,FX("","<NuggetList>"+Nugget(3768725989u,"Radius='"+value+"' Duration='"+value+"' Amplitude='"+value+"'","")+"</NuggetList>"),platform);
                    Test(original,"typed:"+value,FX("","<NuggetList>"+Nugget(904475169u,"LaserTemplate='"+value+"' LaserBackwards='"+value+"'","")+"</NuggetList>"),platform);
                }
                foreach(string value in new string[]{"","unknown"," CINE_EXTREME","CINE_EXTREME ","cine_extreme","SUBTLE","NORMAL","STRONG","SEVERE","CINE_EXTREME","CINE_INSANE"})
                    Test(original,"shake:"+value,FX("","<NuggetList>"+Nugget(4011332978u,"Type='"+value+"'","")+"</NuggetList>"),platform);
                foreach(string value in new string[]{"CLOUDY","CLOUDYRAINY","INVALID","NORMAL","RAINY","SNOWY","SUNNY","rainy","","unknown"})
                    Test(original,"weather:"+value,FX("","<NuggetList>"+Nugget(503892219u,"Weather='"+value+"'","")+"</NuggetList>"),platform);
                foreach(int count in new int[]{2,27,28,29,64,257}) {
                    string children="";
                    for(int i=0;i<count;++i) children+=Nugget(nuggetIds[i%nuggetIds.Length],nuggetAttrs[i%nuggetIds.Length],nuggetChildren[i%nuggetIds.Length]);
                    Test(original,"nugget-list-"+count,FX("","<NuggetList>"+children+"</NuggetList>"),platform);
                }
                foreach(uint id in new uint[]{0u,1u,751572338u,3121289073u,866573550u,UInt32.MaxValue})
                    Reject(FX("","<NuggetList>"+Nugget(id,"","")+"</NuggetList>"),false);
                Reject(FX("","<NuggetList>"+Nugget(503892219u,"Unknown='x'","")+"</NuggetList>"),false);
                Reject(FX("","<NuggetList>"+Nugget(503892219u,"RequiredSourceModelConditions='MOVING'","")+"</NuggetList>"),false);
                Reject(FX("","<NuggetList>"+Nugget(503892219u,"","<SourceObjectFilter/>")+"</NuggetList>"),false);
                Console.WriteLine("PASS platform={0} fixtures={1} threeArrayComparisons={2} directArrayComparisons={3} rejections={4} independentLayouts={5}",
                    platformId, fixtures-startFixtures, (comparisons-startComparisons)/2, (comparisons-startComparisons)/2, rejections-startRejections, layouts-startLayouts);
            }
            Console.WriteLine("RESULT fixtures={0} arrayComparisons={1} rejections={2} independentLayouts={3}", fixtures, comparisons, rejections, layouts);
            return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
