using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered.W3x;

// Component test: BAB's already-preprocessed XML directly enters both handlers.
// This does not launch the SDK or claim SDK preprocessing/integration coverage.
internal static class W3xAnimationOracle
{
    const string Ns="uri:ea.com:eala:asset";
    static int fixtures, comparisons;
    static string label;
    static InstanceDeclaration Declaration(XmlDocument xml, XmlNamespaceManager ns)
    {
        var document=new AssetDeclarationDocument();
        Type type=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(type);
        type.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        var declaration=new InstanceDeclaration(document); declaration.XmlNode=xml.DocumentElement;
        return declaration;
    }
    static string Number(double v) { return ((float)v).ToString("R",CultureInfo.InvariantCulture); }
    static string Scalar(string type,int count,int first,int pattern,bool binary)
    {
        var text=new StringBuilder("<ChannelScalar TypeId='1355976437' Type='"+type+"' Pivot='13' FirstFrame='"+first+"'>");
        for(int i=0;i<count;++i) {
            double value=pattern==0 ? 0.25 : pattern==1 ? (i*0.125-1.0) : Math.Sin(i*0.43);
            text.Append("<Frame BinaryMove='").Append(binary && i%3==1 ? "true" : "false").Append("'>").Append(Number(value)).Append("</Frame>");
        }
        return text.Append("</ChannelScalar>").ToString();
    }
    static string Quaternion(int count,int first,int pattern,bool binary)
    {
        var text=new StringBuilder("<ChannelQuaternion TypeId='4131577120' Type='Orientation' Pivot='65535' FirstFrame='"+first+"'>");
        for(int i=0;i<count;++i) {
            double angle=pattern==0 ? 0 : i*0.125;
            double sign=pattern==2 && i%2==1 ? -1 : 1;
            text.Append("<Frame X='0' Y='").Append(Number(Math.Sin(angle)*sign)).Append("' Z='0' W='").Append(Number(Math.Cos(angle)*sign));
            text.Append("' BinaryMove='").Append(binary && i%3==1 ? "true" : "false").Append("'/>");
        }
        return text.Append("</ChannelQuaternion>").ToString();
    }
    static void Equal(string part,byte[] expected,byte[] actual)
    {
        if(expected==null || actual==null || expected.Length!=actual.Length)
            throw new Exception(label+" "+part+" lengths original="+(expected==null ? -1 : expected.Length)+" recovered="+(actual==null ? -1 : actual.Length));
        for(int i=0;i<expected.Length;++i) if(expected[i]!=actual[i])
            throw new Exception(label+" "+part+" byte="+i+" original="+expected[i]+" recovered="+actual[i]);
        ++comparisons;
    }
    static void Test(int platform,string settings,string channels,int frames,string name)
    {
        label=name+" platform="+platform+" settings="+settings;
        var xml=new XmlDocument();
        xml.LoadXml("<W3DAnimation xmlns='"+Ns+"' id='ComponentOracle' Hierarchy='H\\37' NumFrames='"+frames+"' FrameRate='30'>"+settings+"<Channels>"+channels+"</Channels></W3DAnimation>");
        var ns=new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea",Ns);
        var plugin=new BinaryAssetBuilder.W3XCompiler.Plugin(); plugin.Initialize(null,(TargetPlatform)platform);
        AssetBuffer expected=plugin.ProcessInstance(Declaration(xml,ns));
        AssetBuffer actual=AnimationCompiler.CompileXml(xml.DocumentElement,ns,(TargetPlatform)platform);
        Equal("instance",expected.InstanceData,actual.InstanceData);
        Equal("relocations",expected.RelocationData,actual.RelocationData);
        Equal("imports",expected.ImportsData,actual.ImportsData); ++fixtures;
    }
    static int Main()
    {
        try {
            if(IntPtr.Size!=4) throw new Exception("x86 CLR host required");
            string[] settings={"", "<CompressionSettings/>",
                "<CompressionSettings AllowTimeCoded='true' AllowAdaptiveDelta='false'/>",
                "<CompressionSettings AllowTimeCoded='false' AllowAdaptiveDelta='true'/>",
                "<CompressionSettings AllowTimeCoded='false' AllowAdaptiveDelta='false'/>",
                "<CompressionSettings MaxTranslationError='0.003' MaxRotationError='0.005' MaxVisibilityError='0.025' MaxAdaptiveDeltaError='0.002' ForceReductionRate='2'/>",
                "<CompressionSettings AllowTimeCoded='true' AllowAdaptiveDelta='false' ForceReductionRate='0.5'/>"};
            foreach(int platform in new[]{0,1,2,99}) foreach(string compression in settings) {
                if(platform==99 && compression.Length==0) continue;
                Test(platform,compression,"",0,"empty");
                foreach(int count in new[]{1,2,9,33}) foreach(int first in new[]{0,3}) {
                    string scalar=Scalar("XTranslation",count,first,1,false);
                    Test(platform,compression,scalar,count+first,"scalar-linear-"+count+"-"+first);
                    string channels=Scalar("Visibility",count,first,0,true)+Scalar("YTranslation",count,first,2,true)+Quaternion(count,first,1,false);
                    Test(platform,compression,channels,count+first,"mixed-"+count+"-"+first);
                    Test(platform,compression,Quaternion(count,first,2,true),count+first,"quaternion-signs-"+count+"-"+first);
                }
            }
            Console.WriteLine("PASS W3X animation original differential fixtures={0} threeBufferComparisons={1}; platforms=0,1,2,99; SDK not run",fixtures,comparisons);
            return 0;
        } catch(Exception e) { Console.Error.WriteLine(label); Console.Error.WriteLine(e); return 1; }
    }
}
