using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

class XmlPrioritySdkOracle {
    const string NS="uri:ea.com:eala:asset";
    static readonly Dictionary<string,string> Required=new Dictionary<string,string>(StringComparer.Ordinal) {
        {"RecoveryXmlPriority_FX_Defaults","FXList"},
        {"RecoveryXmlPriority_FX_Filters","FXList"},
        {"RecoveryXmlPriority_Particle_Defaults","FXParticleSystemTemplate"},
        {"RecoveryXmlPriority_Particle_Gpu","FXParticleSystemTemplate"},
        {"RecoveryXmlPriority_Particle_Lightning","FXParticleSystemTemplate"},
        {"RecoveryXmlPriority_Particle_RenderObject","FXParticleSystemTemplate"},
        {"RecoveryXmlPriority_AttributeModifier_Defaults","AttributeModifier"},
        {"RecoveryXmlPriority_SpecialPower_Defaults","SpecialPowerTemplate"},
        {"RecoveryXmlPriority_OCL_Defaults","ObjectCreationList"},
        {"RecoveryXmlPriority_Locomotor_Defaults","LocomotorTemplate"},
        {"RecoveryXmlPriority_CommandSet_Defaults","LogicCommandSet"},
        {"RecoveryXmlPriority_Upgrade_Defaults","UpgradeTemplate"},
        {"RecoveryXmlPriority_Personality_Defaults","AIPersonalityDefinition"},
        {"RecoveryXmlPriority_Strategic_Defaults","AIStrategicStateDefinition"},
        {"RecoveryXmlPriority_Micro_Defaults","AIMicroManagerData"},
        {"RecoveryXmlPriority_Budget_Defaults","AIBudgetStateDefinition"},
        {"RecoveryXmlPriority_State_Defaults","AIStateDefinition"},
        {"RecoveryXmlPriority_Target_Defaults","AITargetingHeuristic"},
        {"RecoveryXmlAdditional_LogicCommand_Defaults","LogicCommand"},
        {"RecoveryXmlAdditional_PlayerTech_Defaults","PlayerTech"},
        {"RecoveryXmlAdditional_PlayerTechRank_Defaults","PlayerTechRank"},
        {"RecoveryXmlAdditional_UpgradeTemplate_Support","UpgradeTemplate"},
        {"RecoveryXmlAdditional_PlayerTechUpgradeBinding_Defaults","PlayerTechUpgradeBinding"},
        {"RecoveryXmlAdditional_ExperienceLevelTemplate_Defaults","ExperienceLevelTemplate"},
        {"RecoveryXmlAdditional_ButtonSingleStateData_Defaults","ButtonSingleStateData"},
        {"RecoveryXmlAdditional_PurchasePlayerTechButtonTemplate_Defaults","PurchasePlayerTechButtonTemplate"},
        {"RecoveryXmlAdditional_PlayerTechStoreTemplate_Defaults","PlayerTechStoreTemplate"},
        {"RecoveryXmlAdditional_UICommandButton_Defaults","UICommandButton"},
        {"RecoveryXmlAdditional_FXList_Support","FXList"},
        {"RecoveryXmlAdditional_ObjectCreationList_Support","ObjectCreationList"},
        {"RecoveryXmlAdditional_AttributeModifier_Support","AttributeModifier"},
        {"RecoveryXmlAdditional_LogicCommand_Combined","LogicCommand"},
        {"RecoveryXmlAdditional_LogicCommand_Inherited","LogicCommand"},
        {"RecoveryXmlAdditional_PlayerTech_Combined","PlayerTech"},
        {"RecoveryXmlAdditional_PlayerTech_Inherited","PlayerTech"},
        {"RecoveryXmlAdditional_PlayerTechRank_Combined","PlayerTechRank"},
        {"RecoveryXmlAdditional_PlayerTechRank_Inherited","PlayerTechRank"},
        {"RecoveryXmlAdditional_PlayerTechUpgradeBinding_Combined","PlayerTechUpgradeBinding"},
        {"RecoveryXmlAdditional_PlayerTechUpgradeBinding_Inherited","PlayerTechUpgradeBinding"},
        {"RecoveryXmlAdditional_ExperienceLevelTemplate_Combined","ExperienceLevelTemplate"},
        {"RecoveryXmlAdditional_ExperienceLevelTemplate_Inherited","ExperienceLevelTemplate"},
        {"RecoveryXmlAdditional_ButtonSingleStateData_Combined","ButtonSingleStateData"},
        {"RecoveryXmlAdditional_PurchasePlayerTechButtonTemplate_Combined","PurchasePlayerTechButtonTemplate"},
        {"RecoveryXmlAdditional_PlayerTechStoreTemplate_Combined","PlayerTechStoreTemplate"},
        {"RecoveryXmlAdditional_UICommandButton_Combined","UICommandButton"},
        {"RecoveryXmlAdditional_FontDefaultSetting_Defaults","FontDefaultSetting"},
        {"RecoveryXmlAdditional_FontTranslation_Defaults","FontTranslation"},
        {"RecoveryXmlAdditional_FontSubstitution_Defaults","FontSubstitution"},
        {"RecoveryXmlAdditional_FontDefaultSetting_Combined","FontDefaultSetting"},
        {"RecoveryXmlAdditional_FontTranslation_Combined","FontTranslation"},
        {"RecoveryXmlAdditional_FontSubstitution_Combined","FontSubstitution"},
        {"RecoveryXmlAdditional_FontDefaultSetting_Inherited","FontDefaultSetting"},
        {"RecoveryXmlAdditional_FontSubstitution_Inherited","FontSubstitution"},
        {"RecoveryXmlAdditional_Weather_Defaults","Weather"},
        {"RecoveryXmlAdditional_Weather_Combined","Weather"},
        {"RecoveryXmlAdditional_WaterTransparency_Defaults","WaterTransparency"},
        {"RecoveryXmlAdditional_WaterTransparency_Combined","WaterTransparency"},
        {"RecoveryXmlAdditional_WaterSetting_Defaults","WaterSetting"},
        {"RecoveryXmlAdditional_WaterSetting_Combined","WaterSetting"},
        {"RecoveryXmlAdditional_WaterTextureList_Defaults","WaterTextureList"},
        {"RecoveryXmlAdditional_WaterTextureList_Combined","WaterTextureList"},
        {"RecoveryXmlAdditional_Environment_Defaults","Environment"},
        {"RecoveryXmlAdditional_LightingSettings_Defaults","LightingSettings"},
        {"RecoveryXmlAdditional_LightingSettings_Combined","LightingSettings"},
        {"RecoveryXmlAdditional_Environment_Combined","Environment"},
        {"RecoveryXmlAdditional_TerrainParticleMapping_Defaults","TerrainParticleMapping"},
        {"RecoveryXmlAdditional_TerrainParticleMapping_Combined","TerrainParticleMapping"},
        {"RecoveryXmlAdditional_BounceKickTerrainMapping_Defaults","BounceKickTerrainMapping"},
        {"RecoveryXmlAdditional_BounceKickTerrainMapping_Combined","BounceKickTerrainMapping"},
        {"RecoveryXmlAdditional_ShadowMap_Defaults","ShadowMap"},
        {"RecoveryXmlAdditional_ShadowMap_Combined","ShadowMap"},
        {"RecoveryXmlAdditional_OrdersManagerSettings_Defaults","OrdersManagerSettings"},
        {"RecoveryXmlAdditional_OrdersManagerSettings_Combined","OrdersManagerSettings"},
        {"RecoveryXmlAdditional_Weather_Inherited","Weather"},
        {"RecoveryXmlAdditional_TerrainParticleMapping_Inherited","TerrainParticleMapping"},
        {"RecoveryXmlAdditional_BounceKickTerrainMapping_Inherited","BounceKickTerrainMapping"},
        {"RecoveryXmlAdditional_ShadowMap_Inherited","ShadowMap"},
        {"RecoveryXmlAdditional_TerrainVisualSettings_Defaults","TerrainVisualSettings"},
        {"RecoveryXmlAdditional_CameraSettings_Defaults","CameraSettings"},
        {"RecoveryXmlAdditional_TerrainVisualSettings_Combined","TerrainVisualSettings"},
        {"RecoveryXmlAdditional_CameraSettings_Combined","CameraSettings"},
        {"RecoveryXmlAdditional_AIData_Defaults","AIData"},
        {"RecoveryXmlAdditional_UnitAISettings_Defaults","UnitAISettings"},
        {"RecoveryXmlAdditional_PlayerTemplate_Defaults","PlayerTemplate"},
        {"RecoveryXmlAdditional_MultiplayerSettings_Defaults","MultiplayerSettings"},
        {"RecoveryXmlAdditional_AIBuildStateDefinition_Defaults","AIBuildStateDefinition"},
        {"RecoveryXmlAdditional_AIStrategicStateName_Defaults","AIStrategicStateName"},
        {"RecoveryXmlAdditional_MultiplayerColor_Defaults","MultiplayerColor"},
        {"RecoveryXmlAdditional_AIPersonalityToAchievementEventMapping_Defaults","AIPersonalityToAchievementEventMapping"},
        {"RecoveryXmlAdditional_FormationAssistantUnitDefinition_Defaults","FormationAssistantUnitDefinition"},
        {"RecoveryXmlAdditional_FormationAssistantTemplate_Defaults","FormationAssistantTemplate"},
        {"RecoveryXmlAdditional_FormationSelectionTemplate_Defaults","FormationSelectionTemplate"},
        {"RecoveryXmlAdditional_FormationAssistant_Defaults","FormationAssistant"},
        {"RecoveryXmlAdditional_SkirmishSettings_Defaults","SkirmishSettings"}
    };
    static uint TypeId(string name) {
        if(name=="AttributeModifier")return 3319822471u;
        if(name=="FXList")return 2254974584u;
        if(name=="SpecialPowerTemplate")return 2178440954u;
        if(name=="ObjectCreationList")return 3899542881u;
        if(name=="LocomotorTemplate")return 3972178387u;
        if(name=="LogicCommandSet")return 3959844197u;
        if(name=="FXParticleSystemTemplate")return 686292351u;
        if(name=="UpgradeTemplate")return 3786401627u;
        if(name=="AIPersonalityDefinition")return 3604279694u;
        if(name=="AIStrategicStateDefinition")return 607123156u;
        if(name=="AIMicroManagerData")return 2423780215u;
        if(name=="AIBudgetStateDefinition")return 2893598307u;
        if(name=="AIStateDefinition")return 3239935619u;
        if(name=="AITargetingHeuristic")return 926814458u;
        if(name=="LogicCommand")return 2101756272u;
        if(name=="PlayerTech")return 2131237081u;
        if(name=="PlayerTechRank")return 1802643394u;
        if(name=="PlayerTechUpgradeBinding")return 2140592493u;
        if(name=="ExperienceLevelTemplate")return 1874610847u;
        if(name=="PlayerTechStoreTemplate")return 1213037816u;
        if(name=="PurchasePlayerTechButtonTemplate")return 1566941920u;
        if(name=="ButtonSingleStateData")return 3048039928u;
        if(name=="UICommandButton")return 134745024u;
        if(name=="FontDefaultSetting")return 1371131236u;
        if(name=="FontTranslation")return 1318336226u;
        if(name=="FontSubstitution")return 91719417u;
        if(name=="Weather")return 2430161325u;
        if(name=="WaterTransparency")return 1479531562u;
        if(name=="WaterSetting")return 2728605516u;
        if(name=="Environment")return 680780553u;
        if(name=="LightingSettings")return 3169690211u;
        if(name=="WaterTextureList")return 2995571269u;
        if(name=="TerrainParticleMapping")return 1469498551u;
        if(name=="BounceKickTerrainMapping")return 2368878637u;
        if(name=="ShadowMap")return 4157475773u;
        if(name=="OrdersManagerSettings")return 2996766068u;
        if(name=="TerrainVisualSettings")return 1184276792u;
        if(name=="CameraSettings")return 3898802054u;
        if(name=="AIData")return 3827723072u;
        if(name=="UnitAISettings")return 2964891936u;
        if(name=="PlayerTemplate")return 4063352333u;
        if(name=="MultiplayerSettings")return 191082224u;
        if(name=="AIBuildStateDefinition")return 1644972043u;
        if(name=="AIStrategicStateName")return 2010562192u;
        if(name=="MultiplayerColor")return 2384988189u;
        if(name=="AIPersonalityToAchievementEventMapping")return 4086175864u;
        if(name=="FormationAssistantUnitDefinition")return 838130914u;
        if(name=="FormationAssistantTemplate")return 2912385462u;
        if(name=="FormationSelectionTemplate")return 3374519264u;
        if(name=="FormationAssistant")return 1963196508u;
        if(name=="GameStatsModeData")return 3249689110u;
        if(name=="SkirmishSettings")return 402082255u;
        throw new Exception("Unexpected SDK trace type: "+name);
    }
    static InstanceDeclaration Declaration(XmlDocument xml) {
        var namespaces=new XmlNamespaceManager(xml.NameTable);namespaces.AddNamespace("ea",NS);
        var document=new AssetDeclarationDocument();
        Type type=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(type);
        type.GetField("NamespaceManager").SetValue(state,namespaces);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        var instance=new InstanceDeclaration(document);instance.XmlNode=xml.DocumentElement;return instance;
    }
    static void Equal(byte[] actual,byte[] expected,string label) {
        if(actual==null || expected==null || actual.Length!=expected.Length)throw new Exception(label+": array length");
        for(int i=0;i<actual.Length;++i)if(actual[i]!=expected[i])throw new Exception(label+": byte "+i+" actual="+actual[i]+" expected="+expected[i]);
    }
    static bool Contains(byte[] haystack,byte[] value) {
        if(value.Length==0)throw new Exception("Cannot prove SDK output contains an empty asset");
        for(int i=0;i<=haystack.Length-value.Length;++i) {
            int j=0;while(j<value.Length && haystack[i+j]==value[j])++j;
            if(j==value.Length)return true;
        }
        return false;
    }
    static string Hash(string path) {
        using(var hash=SHA256.Create())using(var stream=File.OpenRead(path))
            return BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant();
    }
    static void NoOriginalFiles(string directory) {
        foreach(string name in new[]{"BinaryAssetBuilder.Tokenizer.dll","BinaryAssetBuilder.XmlCompiler.dll"})
            if(File.Exists(Path.Combine(directory,name)))throw new Exception("Original compiler physically present in isolated host: "+Path.Combine(directory,name));
    }
    static void NoOriginalAssemblies() {
        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            string name=assembly.GetName().Name;
            if(name=="BinaryAssetBuilder.Tokenizer" || name=="BinaryAssetBuilder.XmlCompiler")
                throw new Exception("Original compiler loaded in isolated host: "+assembly.FullName);
        }
    }
    static XmlNamespaceManager Namespaces(XmlDocument d) {
        var n=new XmlNamespaceManager(d.NameTable);n.AddNamespace("ea",NS);return n;
    }
    static void RequireTag(XmlElement element,string label,uint id) {
        uint actual;
        if(element==null || element.NamespaceURI!=NS || !UInt32.TryParse(element.GetAttribute("TypeId"),NumberStyles.None,CultureInfo.InvariantCulture,out actual) || actual!=id)
            throw new Exception("Missing or incorrect BAB numeric TypeId: "+label);
    }
    static void CheckPreprocessing(XmlDocument d,string id,string type) {
        // BAB adds root TypeId during ValidateInstances; the declaration handle must independently agree.
        RequireTag(d.DocumentElement,id,TypeId(type));
        if(Declaration(d).Handle.TypeId!=TypeId(type))throw new Exception("Incorrect BAB root handle TypeId: "+id);
        XmlNamespaceManager ns=Namespaces(d);
        foreach(XmlElement e in d.SelectNodes("/ea:FXList/ea:NuggetList/* | /ea:FXParticleSystemTemplate/ea:Physics/* | /ea:FXParticleSystemTemplate/ea:Draw/* | /ea:FXParticleSystemTemplate/ea:Volume/* | /ea:FXParticleSystemTemplate/ea:Velocity/* | /ea:FXParticleSystemTemplate/ea:Update/* | /ea:FXParticleSystemTemplate/ea:Event/*",ns)) {
            uint tag;
            if(e.NamespaceURI!=NS || !UInt32.TryParse(e.GetAttribute("TypeId"),NumberStyles.None,CultureInfo.InvariantCulture,out tag) || tag==0)
                throw new Exception("Missing BAB polymorphic TypeId: "+id+"/"+e.LocalName);
        }
        if(id=="RecoveryXmlPriority_FX_Filters") {
            if(d.SelectNodes("//ea:SourceObjectFilter",ns).Count==0 || d.SelectNodes("//ea:SecondaryObjectFilter",ns).Count==0)
                throw new Exception("SDK did not call both FXList filter paths");
        } else if(id=="RecoveryXmlPriority_Particle_Gpu") {
            RequireTag((XmlElement)d.SelectSingleNode("/ea:FXParticleSystemTemplate/ea:Draw/*",ns),id+"/DrawGpu",2248017887u);
            RequireTag((XmlElement)d.SelectSingleNode("/ea:FXParticleSystemTemplate/ea:Update/*",ns),id+"/UpdateDefault",2933826200u);
            bool life=false,collision=false;
            foreach(XmlElement e in d.SelectNodes("/ea:FXParticleSystemTemplate/ea:Event/*",ns)) {
                if(e.GetAttribute("TypeId")=="3465979181")life=true;
                if(e.GetAttribute("TypeId")=="1075723135")collision=true;
            }
            if(!life || !collision)throw new Exception("SDK did not call both concrete Event paths");
        } else if(id=="RecoveryXmlPriority_Particle_Lightning") {
            RequireTag((XmlElement)d.SelectSingleNode("/ea:FXParticleSystemTemplate/ea:Draw/*",ns),id+"/DrawLightning",3036920908u);
            if(d.SelectNodes("/ea:FXParticleSystemTemplate/ea:Draw/*/ea:RandomSet",ns).Count!=2)
                throw new Exception("SDK Lightning probe must contain two RandomSet entries");
        } else if(id=="RecoveryXmlPriority_Particle_RenderObject") {
            RequireTag((XmlElement)d.SelectSingleNode("/ea:FXParticleSystemTemplate/ea:Draw/*",ns),id+"/DrawRenderObject",3408220251u);
            RequireTag((XmlElement)d.SelectSingleNode("/ea:FXParticleSystemTemplate/ea:Update/*",ns),id+"/UpdateRenderObject",234581578u);
            if(d.SelectNodes("/ea:FXParticleSystemTemplate/ea:Draw/*/ea:ObjectSet",ns).Count!=3)
                throw new Exception("SDK RenderObject probe must contain three ObjectSet entries");
        }
    }
    static void ValidateProbe(string path) {
        var d=new XmlDocument();d.Load(path);
        if(d.DocumentElement.LocalName!="AssetDeclaration" || d.DocumentElement.NamespaceURI!=NS)throw new Exception("Expected source AssetDeclaration");
        var seen=new HashSet<string>(StringComparer.Ordinal);
        foreach(XmlNode node in d.DocumentElement.ChildNodes) {
            if(node.NodeType!=XmlNodeType.Element)continue;
            XmlElement e=(XmlElement)node;
            if(e.NamespaceURI!=NS)throw new Exception("Foreign source probe namespace");
            if(e.LocalName=="Includes")continue;
            string id=e.GetAttribute("id"),type;
            if(!Required.TryGetValue(id,out type) || e.LocalName!=type || !seen.Add(id))
                throw new Exception("Unexpected, duplicated, or wrong-type source probe: "+id+"/"+e.LocalName);
        }
        if(seen.Count!=Required.Count)throw new Exception("Source probe must contain exactly 94 expected SDK assets");
    }
    static void WriteResult(string path,object value) {
        File.WriteAllText(path,new JavaScriptSerializer().Serialize(value));
    }
    static void Schema(string sdk,string probe,string report) {
        ValidateProbe(probe);
        var schemas=new XmlSchemaSet();schemas.XmlResolver=new XmlUrlResolver();
        schemas.Add(NS,Path.Combine(sdk,"Schemas/xsd/CnC3Types.xsd"));schemas.Compile();
        var source=new XmlDocument();source.Load(probe);source.Schemas=schemas;
        var schemaErrors=new List<string>();
        source.Validate(delegate(object sender,ValidationEventArgs e){
            XmlSchemaValidationException invalid=e.Exception as XmlSchemaValidationException;
            XmlNode failed=invalid==null?null:invalid.SourceObject as XmlNode;
            XmlNode asset=failed;
            if(asset is XmlAttribute)asset=((XmlAttribute)asset).OwnerElement;
            while(asset!=null && asset.ParentNode!=source.DocumentElement)asset=asset.ParentNode;
            string identity=asset is XmlElement ? ((XmlElement)asset).GetAttribute("id") : "unknown-asset";
            schemaErrors.Add(identity+": "+e.Message);
        });
        if(schemaErrors.Count!=0)throw new Exception("Legal SDK probe schema errors ("+schemaErrors.Count+"): \n"+String.Join("\n",schemaErrors.ToArray()));
        int rejected=0;
        foreach(string kind in new[]{"missing-id","missing-physics","bad-fx-attribute"}) {
            var invalid=new XmlDocument();invalid.Load(probe);invalid.Schemas=schemas;var ns=Namespaces(invalid);
            if(kind=="missing-id") {
                ((XmlElement)invalid.SelectSingleNode("/ea:AssetDeclaration/ea:FXList",ns)).RemoveAttribute("id");
            } else if(kind=="missing-physics") {
                XmlNode physics=invalid.SelectSingleNode("/ea:AssetDeclaration/ea:FXParticleSystemTemplate/ea:Physics",ns);
                if(physics==null)throw new Exception("Probe has no mandatory Physics child to test");
                physics.ParentNode.RemoveChild(physics);
            } else ((XmlElement)invalid.SelectSingleNode("/ea:AssetDeclaration/ea:FXList",ns)).SetAttribute("Bad","1");
            bool failed=false;
            try { invalid.Validate(delegate(object sender,ValidationEventArgs e){throw new Exception(e.Message);}); }
            catch(XmlSchemaValidationException){failed=true;}
            catch(Exception){failed=true;}
            if(!failed)throw new Exception("Full SDK XSD accepted invalid "+kind);
            ++rejected;
        }
        WriteResult(report,new{succeeded=true,mode="schema",validatedDocuments=1,negativeFixtures=rejected,expectedAssets=Required.Count,probeSha256=Hash(probe),schemaEntry="Schemas/xsd/CnC3Types.xsd"});
        Console.WriteLine("PASS SDK full XSD: 94 assets; 3 negative fixtures rejected.");
    }
    static void CompareMetadata(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,int platform,ref int fields) {
        if(original.GetVersionNumber()!=recovered.GetVersionNumber())throw new Exception("SDK version differs on platform "+platform);
        if(original.GetAllTypesHash()!=recovered.GetAllTypesHash())throw new Exception("SDK all-types hash differs on platform "+platform);
        fields+=2;
        foreach(uint id in new[]{3319822471u,2254974584u,2178440954u,3899542881u,3972178387u,3959844197u,686292351u,3786401627u,3604279694u,607123156u,2423780215u,2893598307u,3239935619u,926814458u,2101756272u,2131237081u,1802643394u,2140592493u,1874610847u,1213037816u,1566941920u,3048039928u,134745024u,1371131236u,1318336226u,91719417u,2430161325u,1479531562u,2728605516u,680780553u,3169690211u,2995571269u,1469498551u,2368878637u,4157475773u,2996766068u,1184276792u,3898802054u,3827723072u,2964891936u,4063352333u,191082224u,1644972043u,2010562192u,2384988189u,4086175864u,838130914u,2912385462u,3374519264u,1963196508u,3249689110u,402082255u}) {
            object a=original.GetExtendedTypeInformation(id),b=recovered.GetExtendedTypeInformation(id);
            if(a==null || b==null)throw new Exception("Null ExtendedTypeInformation");
            foreach(FieldInfo field in typeof(ExtendedTypeInformation).GetFields(BindingFlags.Public|BindingFlags.Instance)) {
                if(!Object.Equals(field.GetValue(a),field.GetValue(b)))throw new Exception("Metadata field differs: "+id+"/"+field.Name+"/platform="+platform);
                ++fields;
            }
            foreach(PropertyInfo property in typeof(ExtendedTypeInformation).GetProperties(BindingFlags.Public|BindingFlags.Instance)) {
                if(!property.CanRead || property.GetIndexParameters().Length!=0)continue;
                if(!Object.Equals(property.GetValue(a,null),property.GetValue(b,null)))throw new Exception("Metadata property differs: "+id+"/"+property.Name+"/platform="+platform);
                ++fields;
            }
        }
    }
    static void Metadata(string originalPath,string recoveredPath,string report) {
        Environment.SetEnvironmentVariable("RA3_RECOVERED_XML_PRIORITY_TRACE",null);
        var original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(originalPath).GetType("BinaryAssetBuilder.XmlCompiler.Plugin",true));
        var recovered=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(recoveredPath).GetType("Recovered.XmlPriority.Plugin",true));
        int fields=0,initializations=0,reinitializations=0,rejections=0;
        foreach(int platform in new[]{0,1,2,99}) {
            original.Initialize(null,(TargetPlatform)platform);recovered.Initialize(null,(TargetPlatform)platform);
            CompareMetadata(original,recovered,platform,ref fields);++initializations;
            int next=platform==0?1:platform==1?2:platform==2?99:0;
            original.ReInitialize(null,(TargetPlatform)next);recovered.ReInitialize(null,(TargetPlatform)next);
            CompareMetadata(original,recovered,next,ref fields);++reinitializations;
            bool refused=false;
            try { recovered.GetExtendedTypeInformation(4294967294u); }catch(NotSupportedException){refused=true;}
            if(!refused)throw new Exception("Recovered SDK plugin accepts unknown type metadata");++rejections;
            var xml=new XmlDocument();xml.LoadXml("<Unsupported xmlns='"+NS+"' id='Unsupported' TypeId='4294967294'/>");
            refused=false;
            try { recovered.ProcessInstance(Declaration(xml)); }catch(NotSupportedException){refused=true;}
            if(!refused)throw new Exception("Recovered SDK plugin accepts unknown ProcessInstance type");++rejections;
        }
        if(fields<16)throw new Exception("No metadata fields were compared");
        WriteResult(report,new{succeeded=true,mode="metadata",platforms=new[]{0,1,2,99},supportedTypes=52,initializations=initializations,reinitializations=reinitializations,fieldComparisons=fields,unknownTypeRejections=rejections,originalSha256=Hash(originalPath),recoveredSha256=Hash(recoveredPath)});
        Console.WriteLine("PASS SDK metadata: four platforms, Initialize/ReInitialize, all public metadata fields, unknown type rejection.");
    }
    static int Main(string[] args) {
        string report=args.Length>5?args[5]:(args.Length==4?args[3]:null);
        try {
            if(args.Length==4 && args[0]=="schema") { Schema(args[1],args[2],args[3]);return 0; }
            if(args.Length==4 && args[0]=="metadata") { Metadata(args[1],args[2],args[3]);return 0; }
            if(args.Length!=6 && args.Length!=7 && args.Length!=8)throw new Exception("Usage: replay|isolated plugin.dll traceDir mod.bin probe.xml result.json [platform=0] [profile=Reduced|FullMymod]");
            bool isolated=args[0]=="isolated";
            if(!isolated && args[0]!="replay")throw new Exception("Unknown replay mode");
            int platform=args.Length>=7?Int32.Parse(args[6],CultureInfo.InvariantCulture):0;
            if(platform!=0 && platform!=1 && platform!=2 && platform!=99)throw new Exception("Unsupported platform selector");
            string profile=args.Length==8?args[7]:"Reduced";
            if(profile!="Reduced" && profile!="FullMymod")throw new Exception("Unknown SDK profile");
            string dll=Path.GetFullPath(args[1]),trace=Path.GetFullPath(args[2]);
            string manifest=Path.Combine(trace,"calls.tsv");
            ValidateProbe(args[4]);
            Environment.SetEnvironmentVariable("RA3_RECOVERED_XML_PRIORITY_TRACE",null);
            if(isolated) {
                NoOriginalFiles(Path.GetDirectoryName(dll));
                NoOriginalFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                NoOriginalAssemblies();
            }
            string pluginName=isolated?"Recovered.XmlPriority.Plugin":"BinaryAssetBuilder.XmlCompiler.Plugin";
            Assembly library=Assembly.LoadFrom(dll);
            Type pluginType=library.GetType(pluginName,true);
            var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(pluginType);
            plugin.Initialize(null,(TargetPlatform)platform);
            if(isolated)NoOriginalAssemblies();
            byte[] mod=File.ReadAllBytes(args[3]);
            if(mod.Length==0)throw new Exception("Final mod.bin is empty");
            var stems=new HashSet<string>(StringComparer.Ordinal);
            var seen=new HashSet<string>(StringComparer.Ordinal);
            var typeCalls=new Dictionary<string,int>(StringComparer.Ordinal);
            var idCalls=new Dictionary<string,int>(StringComparer.Ordinal);
            int calls=0,comparisons=0;
            foreach(string row in File.ReadAllLines(manifest)) {
                if(String.IsNullOrWhiteSpace(row))throw new Exception("Empty SDK trace manifest row");
                string[] fields=row.Split('\t');
                if(fields.Length!=4)throw new Exception("Malformed SDK trace manifest row");
                string type=fields[0],id=fields[1],stem=fields[2],expectedType;
                TypeId(type); // Reject every type outside the fourteen registered priority types.
                if(String.IsNullOrEmpty(id))throw new Exception("SDK trace asset id is empty");
                if(Required.TryGetValue(id,out expectedType)) {
                    if(type!=expectedType)throw new Exception("Wrong SDK probe type: "+type+"/"+id);
                } else if(profile!="FullMymod")throw new Exception("Unexpected reduced SDK call: "+type+"/"+id);
                int recordedPlatform;
                if(!Int32.TryParse(fields[3],NumberStyles.Integer,CultureInfo.InvariantCulture,out recordedPlatform) || recordedPlatform!=platform)throw new Exception("SDK trace platform mismatch");
                if(String.IsNullOrEmpty(stem) || stem!=Path.GetFileName(stem) || stem.IndexOfAny(new[]{'/','\\',':'})>=0 || !stems.Add(stem))
                    throw new Exception("Invalid or duplicated SDK trace stem");
                string xmlPath=Path.Combine(trace,stem+".xml");
                var xml=new XmlDocument();xml.Load(xmlPath);
                if(xml.DocumentElement.LocalName!=type || xml.DocumentElement.GetAttribute("id")!=id)throw new Exception("Trace XML identity differs from manifest");
                CheckPreprocessing(xml,id,type);
                var data=plugin.ProcessInstance(Declaration(xml));
                byte[] instance=File.ReadAllBytes(Path.Combine(trace,stem+".bin"));
                Equal(data.InstanceData,instance,id+"/instance");
                Equal(data.RelocationData,File.ReadAllBytes(Path.Combine(trace,stem+".relo")),id+"/relocations");
                Equal(data.ImportsData,File.ReadAllBytes(Path.Combine(trace,stem+".imp")),id+"/imports");
                comparisons+=3;
                if(!Contains(mod,instance))throw new Exception(id+" instance payload not found in final SDK mod.bin");
                if(Required.ContainsKey(id))seen.Add(id);++calls;
                if(!typeCalls.ContainsKey(type))typeCalls[type]=0;++typeCalls[type];
                if(!idCalls.ContainsKey(id))idCalls[id]=0;++idCalls[id];
                Console.WriteLine("PASS {0} SDK replay {1}/{2}, bytes={3}",isolated?"isolated recovered":"original",type,id,instance.Length);
                if(isolated)NoOriginalAssemblies();
            }
            if(calls<Required.Count || seen.Count!=Required.Count || typeCalls.Count!=51)throw new Exception("Incomplete SDK plugin call coverage");
            foreach(string id in Required.Keys)if(!seen.Contains(id))throw new Exception("No real SDK plugin call for "+id);
            foreach(string extension in new[]{"*.xml","*.bin","*.relo","*.imp"}) {
                string[] files=Directory.GetFiles(trace,extension,SearchOption.TopDirectoryOnly);
                if(files.Length!=calls)throw new Exception("Trace file count differs from manifest for "+extension);
                foreach(string file in files)if(!stems.Contains(Path.GetFileNameWithoutExtension(file)))throw new Exception("Orphan SDK trace file: "+file);
            }
            if(isolated) {
                NoOriginalFiles(Path.GetDirectoryName(dll));
                NoOriginalFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                NoOriginalAssemblies();
            }
            var result=new {
                succeeded=true,mode=args[0],profile=profile,platform=platform,sourceCommit=Environment.GetEnvironmentVariable("GITHUB_SHA"),
                runId=Environment.GetEnvironmentVariable("GITHUB_RUN_ID"),expectedAssets=Required.Count,requiredAssetsVerified=seen.Count,additionalAssets=idCalls.Count-Required.Count,verifiedCalls=calls,
                comparisons=comparisons,typeCalls=typeCalls,idCalls=idCalls,traceManifestSha256=Hash(manifest),
                probeSha256=Hash(args[4]),modSha256=Hash(args[3]),compilerSha256=Hash(dll),
                originalDllFilesAbsent=isolated,originalCompilerAssembliesAbsent=isolated,
                sdkScope=profile=="FullMymod"?"Full shipped mymod Includes plus 81 required probes across 45 recovered types":"Reduced disposable SDK MOD with 81 required probes across 45 recovered types"
            };
            File.WriteAllText(report,new JavaScriptSerializer().Serialize(result));
            Console.WriteLine("PASS {0}: {1} SDK calls, {2} exact buffer comparisons, 81 required assets across fourteen types, isolated={3}",args[0],calls,comparisons,isolated);
            return 0;
        } catch(Exception e) {
            Console.Error.WriteLine(e);
            if(report!=null)try { File.WriteAllText(report,new JavaScriptSerializer().Serialize(new{succeeded=false,error=e.ToString()})); }catch(Exception){}
            return 1;
        }
    }
}


