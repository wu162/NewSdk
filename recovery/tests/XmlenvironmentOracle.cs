using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;
class XmlenvironmentOracle {
    const string NS="uri:ea.com:eala:asset";
    static readonly string[] Types={"Weather","WaterTransparency","WaterSetting","Environment","LightingSettings","WaterTextureList"};
    static readonly int[] Sizes={24,16,12,156,48,16};
    static readonly uint[] TypeIds={2430161325u,1479531562u,2728605516u,680780553u,3169690211u,2995571269u};
    static readonly string[] Numbers={"0","-0","1","-1","12.25","1e-7","1e30","16777217","2d1","2D-1","0x10","garbage",""," 25.75junk","25,5","100%","-12.5%","2147483648","-2147483649","4294967295","4294967296","077","0xff","1e400","-1e400"};
    static readonly string[] Refs={"","Name","Name\\0","Name\\27","Name\\-1","Name\\2147483648","Name\\ +17junk","资源\\7","MIXEDCase"};
    static readonly string[] Bools={"true","false","TRUE","1","0","t","T"," true","yes","","10"};
    static IAssetBuilderPlugin original;
    static int fixtures,comparisons,rejections,layouts;
    static XmlDocument Parse(string text){XmlDocument d=new XmlDocument();d.LoadXml(text);return d;}
    static string Escape(string text){return text.Replace("&","&amp;").Replace("'","&apos;").Replace("<","&lt;").Replace("\"","&quot;");}
    static string Root(string type,string attrs,string children){return "<"+type+" xmlns='"+NS+"' id='Oracle' "+attrs+">"+children+"</"+type+">";}
    static XmlNamespaceManager Namespaces(XmlDocument d){XmlNamespaceManager n=new XmlNamespaceManager(d.NameTable);n.AddNamespace("ea",NS);return n;}
    static InstanceDeclaration Declaration(XmlDocument d) {
        AssetDeclarationDocument document=new AssetDeclarationDocument();
        Type stateType=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state,Namespaces(d));
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        InstanceDeclaration declaration=new InstanceDeclaration(document);declaration.XmlNode=d.DocumentElement;return declaration;
    }
    static void Equal(string label,byte[] a,byte[] b){
        if(a==null||b==null||a.Length!=b.Length)throw new Exception(label+" length original="+(a==null?-1:a.Length)+" recovered="+(b==null?-1:b.Length));
        for(int i=0;i<a.Length;++i)if(a[i]!=b[i])throw new Exception(label+" byte="+i+" original="+a[i]+" recovered="+b[i]);++comparisons;
    }
    static void Check(bool test,string label){if(!test)throw new Exception(label);++layouts;}
    static uint Word(byte[] bytes,int offset,bool big){return big?((uint)bytes[offset]<<24)|((uint)bytes[offset+1]<<16)|((uint)bytes[offset+2]<<8)|bytes[offset+3]:BitConverter.ToUInt32(bytes,offset);}
    static AssetBuffer Call(Type type,string method,object[] args){try{return (AssetBuffer)type.GetMethod(method).Invoke(null,args);}catch(TargetInvocationException e){throw e.InnerException;}}
    static void SideArray(byte[] bytes,byte[] instance,bool big,bool relocation,string label){
        if(bytes.Length==0)return;
        Check(bytes.Length%4==0,label+" alignment");Check(Word(bytes,bytes.Length-4,big)==UInt32.MaxValue,label+" sentinel");
        uint previous=0;for(int i=0;i<bytes.Length-4;i+=4){uint at=Word(bytes,i,big);Check(at+4<=instance.Length,label+" field bounds");Check(i==0||at>previous,label+" sorted unique offsets");previous=at;if(relocation){uint target=Word(instance,(int)at,big);Check(target%4==0&&target<instance.Length,label+" target bounds/alignment");}}
    }
    static AssetBuffer TestDoc(string label,XmlDocument d,TargetPlatform p){
        try {
            InstanceDeclaration declaration=Declaration(d);AssetBuffer a=original.ProcessInstance(declaration);
            Type type=typeof(XmlenvironmentCompiler).Assembly.GetType("Recovered."+d.DocumentElement.LocalName+"Compiler");
            AssetBuffer b=Call(type,"ProcessInstance",new object[]{declaration,p});
            AssetBuffer direct=Call(type,"CompileXml",new object[]{d.DocumentElement,Namespaces(d),p!=TargetPlatform.Win32});
            Equal(label+"/instance",a.InstanceData,b.InstanceData);Equal(label+"/relocation",a.RelocationData,b.RelocationData);Equal(label+"/imports",a.ImportsData,b.ImportsData);
            Equal(label+"/direct-instance",a.InstanceData,direct.InstanceData);Equal(label+"/direct-relocation",a.RelocationData,direct.RelocationData);Equal(label+"/direct-imports",a.ImportsData,direct.ImportsData);
            bool big=p!=TargetPlatform.Win32;Check(Word(b.InstanceData,0,big)==0,label+" reserved root word");
            SideArray(b.RelocationData,b.InstanceData,big,true,label+" relocation");SideArray(b.ImportsData,b.InstanceData,big,false,label+" import");++fixtures;return b;
        }catch(Exception e){throw new Exception(label+" platform="+(int)p+" xml="+d.OuterXml,e);}
    }
    static AssetBuffer Test(string label,string xml,TargetPlatform p){return TestDoc(label,Parse(xml),p);}
    static void Reject(string label,string xml){XmlDocument d=Parse(xml);try{XmlenvironmentCompiler.CompileXml(d.DocumentElement,Namespaces(d),false);}catch(NotSupportedException){++rejections;return;}throw new Exception("Expected rejection: "+label);}
    static XmlDocument FieldDocument(string root,string path,string name,string value){XmlDocument d=Parse(Root(root,"",""));XmlElement node=d.DocumentElement;if(path.Length!=0)foreach(string part in path.Split('/')){XmlElement child=d.CreateElement(part,NS);node.AppendChild(child);node=child;}node.SetAttribute(name,value);return d;}
    static string[] EnumValues(string name){switch(name){case "WeatherType":return new string[]{"CLOUDY","CLOUDYRAINY","INVALID","NORMAL","RAINY","SNOWY","SUNNY","unknown","normal","","0"};case "TimeOfDayType":return new string[]{"AFTERNOON","EVENING","INTERPOLATE","INVALID","MORNING","NIGHT","unknown","morning","","0"};case "WaterTextureType":return new string[]{"BUMP","FIRST","NOISE","NORMAL","RIVER","SKY","SPARKLE","unknown","river","","0"};case "DistributionType":return new string[]{"CONSTANT","GAUSSIAN","HIGH_BIAS","LOW_BIAS","TRIANGULAR","UNIFORM","unknown","uniform","","0"};default:throw new Exception("Unknown test enum");}}
    static string[][] FieldCases=new string[][] {
        new string[]{"Weather","Rain","RainTexture","Reference",""},
        new string[]{"Weather","Rain","IsRaining","Bool",""},
        new string[]{"Weather","Rain","NumRaindropsPerBox","Signed",""},
        new string[]{"Weather","Rain","RainBoxWidth","Float",""},
        new string[]{"Weather","Rain","RainBoxLength","Float",""},
        new string[]{"Weather","Rain","RainBoxHeight","Float",""},
        new string[]{"Weather","Rain","MinWidth","Float",""},
        new string[]{"Weather","Rain","MaxWidth","Float",""},
        new string[]{"Weather","Rain","MinHeight","Float",""},
        new string[]{"Weather","Rain","MaxHeight","Float",""},
        new string[]{"Weather","Rain","MinSpeed","Float",""},
        new string[]{"Weather","Rain","MaxSpeed","Float",""},
        new string[]{"Weather","Rain","MinAlpha","Float",""},
        new string[]{"Weather","Rain","MaxAlpha","Float",""},
        new string[]{"Weather","Rain","WindStrength","Float",""},
        new string[]{"Weather","Lightning","LightningDuration","Signed",""},
        new string[]{"Weather","Lightning","LightningChance","Float",""},
        new string[]{"Weather","Lightning/LightningFactor","Type","Enum","DistributionType"},
        new string[]{"Weather","Lightning/LightningFactor","Low","Float",""},
        new string[]{"Weather","Lightning/LightningFactor","High","Float",""},
        new string[]{"Weather","Cloud/CloudTextureSize","x","Float",""},
        new string[]{"Weather","Cloud/CloudTextureSize","y","Float",""},
        new string[]{"Weather","Cloud/CloudOffsetPerSecond","x","Float",""},
        new string[]{"Weather","Cloud/CloudOffsetPerSecond","y","Float",""},
        new string[]{"Weather","WeatherData","id","Enum","WeatherType"},
        new string[]{"Weather","WeatherData","HasLightning","Bool",""},
        new string[]{"Weather","WeatherData","Sound","Reference",""},
        new string[]{"WaterTransparency","","TransparentWaterDepth","Float",""},
        new string[]{"WaterTransparency","","TransparentWaterMinOpacity","Float",""},
        new string[]{"WaterTransparency","","StandingWaterTexture","Hash",""},
        new string[]{"WaterSetting","","TimeOfDay","Enum","TimeOfDayType"},
        new string[]{"WaterSetting","DiffuseColor","R","Unsigned",""},
        new string[]{"WaterSetting","DiffuseColor","G","Unsigned",""},
        new string[]{"WaterSetting","DiffuseColor","B","Unsigned",""},
        new string[]{"WaterSetting","DiffuseColor","A","Unsigned",""},
        new string[]{"Environment","CloudEffect","CloudTexture","Reference",""},
        new string[]{"Environment","CloudEffect","DarkCloudTexture","Reference",""},
        new string[]{"Environment","CloudEffect","AlphaTexture","Reference",""},
        new string[]{"Environment","CloudEffect","PropagateSpeed","Float",""},
        new string[]{"Environment","CloudEffect","Angle","Signed",""},
        new string[]{"Environment","CloudEffect","DarkeningRate","Signed",""},
        new string[]{"Environment","CloudEffect","LighteningRate","Signed",""},
        new string[]{"Environment","CloudEffect","CloudScrollSpeed","Float",""},
        new string[]{"Environment","CloudEffect","DissipateTexture","Reference",""},
        new string[]{"Environment","CloudEffect","DissipateStartLevel","Float",""},
        new string[]{"Environment","CloudEffect","DissipateSpeed","Float",""},
        new string[]{"Environment","CloudEffect","DissipateRateScale","Float",""},
        new string[]{"Environment","CloudEffect","LightningShadows","Bool",""},
        new string[]{"Environment","CloudEffect","JitterLightningLightIntensity","Bool",""},
        new string[]{"Environment","CloudEffect","JitterLightningLightPosition","Bool",""},
        new string[]{"Environment","CloudEffect","LightningChance","Float",""},
        new string[]{"Environment","CloudEffect","LightningFrequency","Float",""},
        new string[]{"Environment","CloudEffect","LightningShadowIntensity","Float",""},
        new string[]{"Environment","CloudEffect","LightningFX","Hash",""},
        new string[]{"Environment","CloudEffect/DarkeningFactor","R","Float",""},
        new string[]{"Environment","CloudEffect/DarkeningFactor","G","Float",""},
        new string[]{"Environment","CloudEffect/DarkeningFactor","B","Float",""},
        new string[]{"Environment","CloudEffect/DarkeningFactorRain","R","Float",""},
        new string[]{"Environment","CloudEffect/DarkeningFactorRain","G","Float",""},
        new string[]{"Environment","CloudEffect/DarkeningFactorRain","B","Float",""},
        new string[]{"Environment","CloudEffect/LightningShadowColor","R","Float",""},
        new string[]{"Environment","CloudEffect/LightningShadowColor","G","Float",""},
        new string[]{"Environment","CloudEffect/LightningShadowColor","B","Float",""},
        new string[]{"Environment","CloudEffect/LightningLightPosition1","x","Float",""},
        new string[]{"Environment","CloudEffect/LightningLightPosition1","y","Float",""},
        new string[]{"Environment","CloudEffect/LightningLightPosition2","x","Float",""},
        new string[]{"Environment","CloudEffect/LightningLightPosition2","y","Float",""},
        new string[]{"Environment","CloudEffect/LightningLightPosition3","x","Float",""},
        new string[]{"Environment","CloudEffect/LightningLightPosition3","y","Float",""},
        new string[]{"Environment","CloudEffect/LightningIntensity","Type","Enum","DistributionType"},
        new string[]{"Environment","CloudEffect/LightningIntensity","Low","Float",""},
        new string[]{"Environment","CloudEffect/LightningIntensity","High","Float",""},
        new string[]{"Environment","CloudEffect/LightningDuration","Type","Enum","DistributionType"},
        new string[]{"Environment","CloudEffect/LightningDuration","Low","Float",""},
        new string[]{"Environment","CloudEffect/LightningDuration","High","Float",""},
        new string[]{"LightingSettings","","TimeOfDay","Enum","TimeOfDayType"},
        new string[]{"LightingSettings","","OccludedLuminanceScale","Float",""},
        new string[]{"LightingSettings","","LoadAfter","Reference",""},
        new string[]{"LightingSettings","LightingMultiplierIfNoClouds","R","Float",""},
        new string[]{"LightingSettings","LightingMultiplierIfNoClouds","G","Float",""},
        new string[]{"LightingSettings","LightingMultiplierIfNoClouds","B","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings","TimeOfDay","Enum","TimeOfDayType"},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/Ambient","R","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/Ambient","G","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/Ambient","B","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/Diffuse","R","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/Diffuse","G","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/Diffuse","B","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/LightPos","x","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/LightPos","y","Float",""},
        new string[]{"LightingSettings","CurrentLightingSettings/Lighting/LightPos","z","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup","TimeOfDay","Enum","TimeOfDayType"},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/Ambient","R","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/Ambient","G","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/Ambient","B","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/Diffuse","R","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/Diffuse","G","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/Diffuse","B","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/LightPos","x","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/LightPos","y","Float",""},
        new string[]{"LightingSettings","TerrainLighting/LightingTimeGroup/Lighting/LightPos","z","Float",""},
        new string[]{"WaterTextureList","","Type","Enum","WaterTextureType"},
    };
    static void AbiCases(TargetPlatform p) {
        bool big=p!=TargetPlatform.Win32;
        AssetBuffer weather=Test("ABI/Weather",Root("Weather","","<Rain RainTexture='Name\\7' IsRaining='true'/><Lightning><LightningFactor Type='CONSTANT' Low='2' High='3'/></Lightning><Cloud><CloudTextureSize x='10' y='20'/><CloudOffsetPerSecond x='1' y='2'/></Cloud><WeatherData id='RAINY' Sound='Name\\11' HasLightning='true'/><WeatherData id='SUNNY' Sound='Name\\13'/ >".Replace("/ >","/>")),p);
        Check(weather.InstanceData.Length==156,"Weather independent full size");
        int[] at={4,8,12,20,92,108,112};uint[] target={24u,84u,108u,132u,96u,116u,124u};
        for(int i=0;i<at.Length;++i)Check(Word(weather.InstanceData,at[i],big)==target[i],"Weather independent pointer "+at[i]);
        Check(Word(weather.InstanceData,16,big)==2&&Word(weather.InstanceData,132,big)==2&&Word(weather.InstanceData,144,big)==4,"Weather independent list/enum");
        Check(weather.InstanceData[80]==1&&weather.InstanceData[81]==0&&weather.InstanceData[82]==0&&weather.InstanceData[83]==0,"Rain packed bool/padding");
        Check(Word(weather.ImportsData,0,big)==24&&Word(weather.ImportsData,4,big)==136&&Word(weather.ImportsData,8,big)==148,"Weather reference allocated field offsets");
        AssetBuffer color=Test("ABI/WaterSetting",Root("WaterSetting","TimeOfDay='NIGHT'","<DiffuseColor R='17' G='34' B='51' A='68'/>"),p);
        Check(Word(color.InstanceData,4,big)==4&&Word(color.InstanceData,8,big)==0x44112233u,"packed ARGB and enum independent");
        AssetBuffer emptyColor=Test("ABI/emptyColor",Root("WaterSetting","","<DiffuseColor/>"),p);Check(Word(emptyColor.InstanceData,8,big)==0xff000000u,"packed color default alpha");
        string light="<Lighting><Ambient R='0.1' G='0.2' B='0.3'/><Diffuse R='1' G='0.5' B='0.25'/><LightPos x='1' y='2' z='3'/></Lighting>";
        AssetBuffer lighting=Test("ABI/Lighting",Root("LightingSettings","LoadAfter='Name\\7' TimeOfDay='AFTERNOON'","<CurrentLightingSettings TimeOfDay='MORNING'>"+light+light+"</CurrentLightingSettings><TerrainLighting><LightingTimeGroup TimeOfDay='NIGHT'>"+light+"</LightingTimeGroup><LightingTimeGroup TimeOfDay='EVENING'>"+light+light+"</LightingTimeGroup></TerrainLighting>"),p);
        Check(lighting.InstanceData.Length==252,"Lighting nested array size");
        Check(Word(lighting.InstanceData,32,big)==2&&Word(lighting.InstanceData,36,big)==48&&Word(lighting.InstanceData,40,big)==2&&Word(lighting.InstanceData,44,big)==120,"Lighting inline headers");
        Check(Word(lighting.InstanceData,128,big)==144&&Word(lighting.InstanceData,140,big)==180,"Lighting nested pointer targets");
        Check(Word(lighting.ImportsData,0,big)==4,"Lighting LoadAfter import");
        AssetBuffer strings=Test("ABI/WaterTextureList",Root("WaterTextureList","Type='SKY'","<Texture>abc</Texture><Texture/><Texture>abcde</Texture>"),p);
        Check(strings.InstanceData.Length==56&&Word(strings.InstanceData,8,big)==3&&Word(strings.InstanceData,12,big)==16,"String list root size/count/target");
        Check(Word(strings.InstanceData,16,big)==3&&Word(strings.InstanceData,20,big)==40&&Word(strings.InstanceData,24,big)==0&&Word(strings.InstanceData,28,big)==44&&Word(strings.InstanceData,32,big)==5&&Word(strings.InstanceData,36,big)==48,"String item lengths/allocation order");
    }
    static void Matrix(TargetPlatform p) {
        original.Initialize(null,p);
        for(int i=0;i<Types.Length;++i) {
            AssetBuffer b=Test("defaults/"+Types[i],Root(Types[i],"",""),p);Check(b.InstanceData.Length==Sizes[i],"root independent size "+Types[i]);
            Test("metadata/"+Types[i],Root(Types[i],"TypeId='"+TypeIds[i]+"'",""),p);
            Reject("unknown attribute/"+Types[i],Root(Types[i],"Unknown='1'",""));Reject("qualified attribute/"+Types[i],Root(Types[i],"xmlns:q='urn:other' q:TypeId='1'",""));
            Reject("unknown child/"+Types[i],Root(Types[i],"","<Unknown/>"));Reject("foreign child/"+Types[i],Root(Types[i],"","<Unknown xmlns='urn:other'/>"));
            Reject("text/"+Types[i],Root(Types[i],"","payload"));
        }
        foreach(string[] f in FieldCases) {
            string[] values=f[3]=="Enum"?EnumValues(f[4]):f[3]=="Bool"?Bools:(f[3]=="Reference"||f[3]=="Hash")?Refs:Numbers;
            foreach(string value in values)TestDoc("field/"+f[0]+"/"+f[1]+"/"+f[2]+"/"+value,FieldDocument(f[0],f[1],f[2],value),p);
            XmlDocument metadata=FieldDocument(f[0],f[1],f[2],values[0]);foreach(XmlNode node in metadata.SelectNodes("//*"))((XmlElement)node).SetAttribute("TypeId","1");TestDoc("field-metadata/"+f[0]+"/"+f[1]+"/"+f[2],metadata,p);
            XmlDocument invalid=FieldDocument(f[0],f[1],f[2],values[0]);XmlNodeList nodes=invalid.SelectNodes("//*");((XmlElement)nodes[nodes.Count-1]).SetAttribute("Unknown","1");Reject("nested unknown",invalid.OuterXml);
        }
        foreach(int count in new int[]{0,1,2,3,27,28,29,64,257}) {
            StringBuilder data=new StringBuilder(),textures=new StringBuilder(),lights=new StringBuilder(),times=new StringBuilder();
            for(int i=0;i<count;++i) {
                data.Append("<WeatherData id='RAINY' Sound='Name\\"+i+"' HasLightning='true'/>");textures.Append("<Texture>Texture"+i+"</Texture>");
                lights.Append("<Lighting><Ambient R='0.1'/><Diffuse G='0.5'/><LightPos x='1'/></Lighting>");
                times.Append("<LightingTimeGroup TimeOfDay='NIGHT'><Lighting><Ambient/><Diffuse/><LightPos/></Lighting></LightingTimeGroup>");
            }
            Test("weather-list/"+count,Root("Weather","",data.ToString()),p);Test("string-list/"+count,Root("WaterTextureList","Type='BUMP'",textures.ToString()),p);
            Test("lighting-list/"+count,Root("LightingSettings","","<CurrentLightingSettings TimeOfDay='MORNING'>"+lights+"</CurrentLightingSettings>"),p);
            Test("lighting-time-list/"+count,Root("LightingSettings","","<TerrainLighting>"+times+"</TerrainLighting>"),p);
        }
        foreach(string value in new string[]{"","a","abc","abcd","abcde","A&B<texture>","é","资源","😀","e\u0301","a\0tail","\0leading","多字节\0尾部"}) {
            XmlDocument d=Parse(Root("WaterTextureList","Type='RIVER'","<Texture/>"));d.DocumentElement.FirstChild.InnerText=value;TestDoc("ANSI-string/"+value.Length,d,p);
        }
        foreach(string type in new string[]{"Weather","Environment","LightingSettings"})Test("null-empty/"+type,Root(type,"",type=="Weather"?"<Rain/><Lightning><LightningFactor/></Lightning><Cloud><CloudTextureSize/><CloudOffsetPerSecond/></Cloud>":type=="Environment"?"<CloudEffect><DarkeningFactor/><LightningIntensity/><LightningDuration/></CloudEffect>":"<LightingMultiplierIfNoClouds/><CurrentLightingSettings/><TerrainLighting/>"),p);
        Reject("duplicate Rain",Root("Weather","","<Rain/><Rain/>"));Reject("duplicate color",Root("WaterSetting","","<DiffuseColor/><DiffuseColor/>"));Reject("string child",Root("WaterTextureList","","<Texture><Unknown/></Texture>"));Reject("unknown root",Root("Unknown","",""));
        AbiCases(p);
        bool rejected=false;try{XmlenvironmentCompiler.ProcessInstance(null,p);}catch(ArgumentNullException){rejected=true;}Check(rejected,"null declaration rejected");++rejections;
    }
    static int Main(string[] args) {
        Dictionary<string,object> result=new Dictionary<string,object>();result["succeeded"]=false;
        try {
            CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("de-DE");Assembly assembly=Assembly.LoadFrom(args[0]);original=(IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.XmlCompiler.Plugin"));
            using(SHA256 sha=SHA256.Create()){string hash=BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(args[0]))).Replace("-","");Check(hash=="D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6","fixed original DLL");result["originalDllSha256"]=hash;}
            List<object> rows=new List<object>();
            foreach(TargetPlatform p in new TargetPlatform[]{(TargetPlatform)0,(TargetPlatform)1,(TargetPlatform)2,(TargetPlatform)99}) {
                int f=fixtures,c=comparisons,r=rejections,l=layouts;Matrix(p);Dictionary<string,object> row=new Dictionary<string,object>();row["platform"]=(int)p;row["fixtures"]=fixtures-f;row["arrayComparisons"]=comparisons-c;row["rejections"]=rejections-r;row["layoutAssertions"]=layouts-l;rows.Add(row);
                Console.WriteLine("PASS platform={0} fixtures={1} arrayComparisons={2} rejections={3} layoutAssertions={4}",(int)p,fixtures-f,comparisons-c,rejections-r,layouts-l);
            }
            result["platforms"]=rows;result["fixtures"]=fixtures;result["arrayComparisons"]=comparisons;result["rejections"]=rejections;result["layoutAssertions"]=layouts;result["succeeded"]=true;
            Console.WriteLine("PASS Environment fixtures={0} arrayComparisons={1} rejections={2} layoutAssertions={3}",fixtures,comparisons,rejections,layouts);return 0;
        }catch(Exception e){result["error"]=e.ToString();Console.Error.WriteLine(e);return 1;}finally{if(args.Length>1)File.WriteAllText(args[1],new JavaScriptSerializer().Serialize(result));}
    }
}
