using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered.W3x;

// Full mesh component differential on identical BAB-preprocessed input. No SDK.
internal static class W3xMeshAssetOracle
{
    const string Ns="uri:ea.com:eala:asset";
    static string label;
    static int fixtures,rawFixtures,comparisons;
    static BinaryWriter capture;
    static void Capture(int platform,string xmlText,AssetBuffer expected)
    {
        if(capture==null) return;
        capture.Write(platform);capture.Write(xmlText);
        foreach(byte[] bytes in new[]{expected.InstanceData,expected.RelocationData,expected.ImportsData}) {
            capture.Write(bytes.Length);capture.Write(bytes);
        }
    }
    static InstanceDeclaration Declaration(XmlDocument xml,XmlNamespaceManager ns)
    {
        var document=new AssetDeclarationDocument();
        Type type=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(type);
        type.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        var declaration=new InstanceDeclaration(document);declaration.XmlNode=xml.DocumentElement;return declaration;
    }
    static string Number(float v) { return v.ToString("R",CultureInfo.InvariantCulture); }
    static string Vector(string tag,string x,string y,string z) { return "<"+tag+" X='"+x+"' Y='"+y+"' Z='"+z+"'/>"; }
    static string Shader(int variant,string special)
    {
        string open=variant==0 ? "<FXShader ShaderName='' TechniqueIndex='0'>" :
            "<FXShader ShaderName='shader_&amp;_name' TechniqueName='technique' TechniqueIndex='255'>";
        if(variant==0) return open+"</FXShader>";
        string constants=variant==1 ? "<Bool TypeId='2750958653' Name='flag'><Value>true</Value></Bool>" :
            variant==2 ? "<Int TypeId='2300057986' Name='signed'><Value>-2147483648</Value></Int>" :
            variant==3 ? "<Float TypeId='3741900581' Name='params'><Value>"+special+"</Value><Value>-0</Value><Value>0.5</Value><Value>1</Value></Float>" :
            variant==4 ? "<Texture TypeId='2777716390' Name='texture'><Value>Texture_Resolved\\37</Value></Texture>" :
            "<Bool TypeId='2750958653' Name=''><Value>false</Value></Bool>"+
            "<Float TypeId='3741900581' Name='mixed'><Value>"+special+"</Value><Value>-2.5</Value></Float>"+
            "<Texture TypeId='2777716390' Name='tex0'><Value>Resolved\\0</Value></Texture>"+
            "<Int TypeId='2300057986' Name='maximum'><Value>2147483647</Value></Int>"+
            "<Texture TypeId='2777716390' Name='tex1'><Value>Resolved\\4294967294</Value></Texture>";
        return open+"<Constants>"+constants+"</Constants></FXShader>";
    }
    static string Geometry(int style)
    {
        var s=new StringBuilder();
        for(int stream=0;stream<(style==2?2:1);++stream) {
            s.Append("<Vertices>");
            s.Append(Vector("V","0","0","0")).Append(Vector("V","1","0","0")).Append(Vector("V","0","1","0"));
            s.Append("</Vertices>");
        }
        for(int stream=0;stream<style;++stream) s.Append("<Normals>").Append(Vector("N","0","0","1")).Append(Vector("N","0","0","1")).Append(Vector("N","0","0","1")).Append("</Normals>");
        if(style==2) {
            s.Append("<Tangents>").Append(Vector("T","1","0","0")).Append(Vector("T","1","0","0")).Append(Vector("T","1","0","0")).Append("</Tangents>");
            s.Append("<Binormals>").Append(Vector("B","0","1","0")).Append(Vector("B","0","1","0")).Append(Vector("B","0","1","0")).Append("</Binormals>");
            s.Append("<VertexColors><C R='0.25' G='0.5' B='1' A='1'/><C R='1' G='0' B='0.5' A='0'/><C R='0' G='1' B='0' A='0.5'/></VertexColors>");
        }
        for(int stream=0;stream<(style==2?3:style);++stream) s.Append("<TexCoords><T X='0.125' Y='0.25'/><T X='0.5' Y='0.75'/><T X='1' Y='1'/></TexCoords>");
        for(int stream=0;stream<style;++stream) s.Append("<BoneInfluences><I Weight='0.25' Bone='65535'/><I Weight='0.5' Bone='9'/><I Weight='0.75' Bone='0'/></BoneInfluences>");
        s.Append("<ShadeIndices><I>2</I><I>0</I><I>1</I></ShadeIndices>");return s.ToString();
    }
    static string Tree(int variant)
    {
        if(variant==0) return "";
        string bounds=Vector("Min","-1","-2","-3")+Vector("Max","4","5","6");
        if(variant==1) return "<AABTree><PolyIndices><P>0</P><P>1</P></PolyIndices><Node>"+bounds+"<Polys Begin='0' Count='2'/></Node></AABTree>";
        return "<AABTree><PolyIndices><P>1</P><P>0</P></PolyIndices><Node>"+bounds+"<Children Front='1' Back='2'/></Node><Node>"+bounds+"<Polys Begin='0' Count='1'/></Node><Node>"+bounds+"<Polys Begin='1' Count='1'/></Node></AABTree>";
    }
    static string Mesh(int geometry,int style,int shader,int tree,string special)
    {
        string[] geometries={"Normal","Skin","CameraAligned","CameraOriented"};
        string triangles="<Triangles><T><V>0</V><V>1</V><V>2</V>"+Vector("Nrm",special,"0","1")+"<Dist>"+special+"</Dist></T><T><V>2</V><V>1</V><V>0</V>"+Vector("Nrm","0","0","-1")+"<Dist>-0.125</Dist></T></Triangles>";
        return "<W3DMesh xmlns='"+Ns+"' id='MeshOracle' GeometryType='"+geometries[geometry]+"' Hidden='"+(style==1?"true":"false")+"' CastShadow='"+(style==2?"true":"false")+"' SortLevel='"+(style==0?0:style==1?16:32)+"'>"+
            "<BoundingBox>"+Vector("Min","-1","-2","-3")+Vector("Max",special,"5","6")+"</BoundingBox>"+
            "<BoundingSphere Radius='"+special+"'>"+Vector("Center","1",special,"3")+"</BoundingSphere>"+
            Geometry(style)+triangles+Shader(shader,special)+Tree(tree)+"</W3DMesh>";
    }
    static void Equal(string part,byte[] expected,byte[] actual)
    {
        if(expected==null || actual==null || expected.Length!=actual.Length)
            throw new Exception(label+" "+part+" length original="+(expected==null?-1:expected.Length)+" recovered="+(actual==null?-1:actual.Length));
        for(int i=0;i<expected.Length;++i) if(expected[i]!=actual[i])
            throw new Exception(label+" "+part+" byte="+i+" original="+expected[i]+" recovered="+actual[i]);
        ++comparisons;
    }
    static void Test(int platform,string xmlText,string name,bool raw)
    {
        label=name+" platform="+platform;
        var xml=new XmlDocument();xml.LoadXml(xmlText);
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",Ns);
        var plugin=new BinaryAssetBuilder.W3XCompiler.Plugin();plugin.Initialize(null,(TargetPlatform)platform);
        AssetBuffer expected=plugin.ProcessInstance(Declaration(xml,ns));
        AssetBuffer actual=MeshCompiler.CompileXml(xml.DocumentElement,ns,(TargetPlatform)platform);
        Equal("instance",expected.InstanceData,actual.InstanceData);Equal("relocations",expected.RelocationData,actual.RelocationData);Equal("imports",expected.ImportsData,actual.ImportsData);
        if(raw) ++rawFixtures;else ++fixtures;
        Capture(platform,xmlText,expected);
    }
    static int Main(string[] args)
    {
        try {
            if(IntPtr.Size!=4) throw new Exception("x86 CLR host required");
            if(args.Length>1) throw new Exception("Usage: W3xMeshAssetOracle [capture-file]");
            if(args.Length==1) {
                capture=new BinaryWriter(File.Open(args[0],FileMode.CreateNew,FileAccess.Write,FileShare.None),Encoding.UTF8);
                capture.Write("W3X-FIXTURE-1");capture.Write("mesh");
            }
            foreach(int platform in new[]{0,1,2}) {
                for(int geometry=0;geometry<4;++geometry) for(int style=0;style<3;++style)
                for(int shader=0;shader<6;++shader) for(int tree=0;tree<3;++tree)
                    Test(platform,Mesh(geometry,style,shader,tree,"4.25"),"mesh-"+geometry+"-"+style+"-"+shader+"-"+tree,false);
                string high=Number(BitConverter.ToSingle(BitConverter.GetBytes(0x76d882ffu),0));
                Test(platform,Mesh(1,2,5,2,high),"finite-swap-signaling-NaN-bits",false);
                // Raw marshaler contracts outside XSD validation are separate
                // from the legal-shape/preprocessed component fixture count.
                string root=Mesh(0,0,0,0,"1");
                string[] variants={"<FXShader/>","<FXShader ShaderName=''/>",
                    "<FXShader ShaderName='S'><Constants><Float TypeId='3741900581'/></Constants></FXShader>",
                    "<FXShader ShaderName='S'><Constants><Bool TypeId='2750958653' Name='missingValue'/></Constants></FXShader>",
                    "<FXShader ShaderName='S'><Constants><Unknown TypeId='4294967295' Name='unknown'/></Constants></FXShader>"};
                foreach(string shader in variants) Test(platform,root.Replace("<FXShader ShaderName='' TechniqueIndex='0'></FXShader>",shader),"raw-shader-"+shader,true);
            }
            Console.WriteLine("PASS W3X full mesh original differential fixtures={0} rawMarshalerFixtures={1} threeBufferComparisons={2}; platforms=0,1,2; SDK not run",fixtures,rawFixtures,comparisons);
            return 0;
        }catch(Exception e){Console.Error.WriteLine(label);Console.Error.WriteLine(e);return 1;}
        finally { if(capture!=null) capture.Dispose(); }
    }
}
