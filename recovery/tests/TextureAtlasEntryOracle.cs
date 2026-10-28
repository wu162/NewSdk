using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
// Immutable original ProcessInstance evidence only; all source images have
// bounded dimensions and normal copies fit the original packer's destination.
internal static class TextureAtlasEntryOracle
{
    sealed class Tile { internal string Id;internal string Base,Normal;internal Tile(uint id,string b,string n){Id=id.ToString(System.Globalization.CultureInfo.InvariantCulture);Base=b;Normal=n;} }
    static int fixtures,decoded,rejected,checks;
    static void Check(bool value,string label){checks++;if(!value)throw new Exception(label);}
    static string Hash(byte[] b){if(b==null)return "null";using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";var code=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(code==null?"":Convert.ToString(code.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static byte[] Tga(int width,int height,int seed,int depth,bool gray)
    {
        int stride=depth/8;byte[] b=new byte[18+width*height*stride];b[2]=(byte)(gray?3:2);b[12]=(byte)width;b[13]=(byte)(width>>8);b[14]=(byte)height;b[15]=(byte)(height>>8);b[16]=(byte)depth;b[17]=(byte)(32+(depth==32?8:0));
        for(int i=0;i<width*height;i++){int at=18+i*stride;b[at]=(byte)(seed*17+79+(i%width)*7+(i/width)*3);if(stride>1)b[at+1]=(byte)(seed*29+5+(i%width)*5-(i/width)*11);if(stride>2)b[at+2]=(byte)(seed*47+13+(i%width)*13+(i/width)*17);if(stride>3)b[at+3]=(byte)(seed%4==0?0:seed%4==1?255:seed%4==2?i*19:i*31);}return b;
    }
    static InstanceDeclaration Declaration(string label,Tile[] tiles,int atlasSize,bool lossy,string cdata,out XmlDocument xml)
    {
        xml=new XmlDocument();var node=xml.CreateElement("TerrainTextureAtlas","uri:ea.com:eala:asset");xml.AppendChild(node);node.SetAttribute("id","AtlasProbe");node.SetAttribute("AtlasSize",atlasSize.ToString(System.Globalization.CultureInfo.InvariantCulture));node.SetAttribute("AllowLossyCompression",lossy?"true":"false");
        foreach(Tile tile in tiles){var n=xml.CreateElement("Tile",node.NamespaceURI);n.SetAttribute("TextureID",tile.Id);n.SetAttribute("BaseTexture",tile.Base);n.SetAttribute("NormalTexture",tile.Normal);var cell=xml.CreateElement("Cell",node.NamespaceURI);cell.SetAttribute("IndexX","0");cell.SetAttribute("IndexY","255");n.AppendChild(cell);node.AppendChild(n);}
        var manager=new XmlNamespaceManager(xml.NameTable);manager.AddNamespace("ea",node.NamespaceURI);var doc=new AssetDeclarationDocument();Type type=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);object state=FormatterServices.GetUninitializedObject(type);type.GetField("NamespaceManager").SetValue(state,manager);typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);
        var declaration=new InstanceDeclaration(doc);declaration.XmlNode=node;declaration.CustomDataPath=cdata;declaration.Handle.TypeHash=0x12345678u;declaration.Handle.InstanceHash=0x9abcdef0u;return declaration;
    }
    static Tile[] Tiles(int count){Tile[] result=new Tile[count];for(int i=0;i<count;i++)result[i]=new Tile((uint)((i*37)%11),"base-"+i.ToString("D2")+".tga",count%3==0||i%2!=0?"missing-normal.tga":count%3==1?"normal-256.tga":"normal-128.tga");return result;}
    static void Same(byte[] a,byte[] b,string label){Check((a==null)==(b==null),label+" presence");if(a!=null)Check(a.Length==b.Length&&Hash(a)==Hash(b),label+" complete bytes");}
    static void Save(string path,byte[] b){if(b!=null)File.WriteAllBytes(path,b);}
    static void Case(IAssetBuilderPlugin plugin,StreamWriter transcript,string output,string label,Tile[] tiles,int size,bool lossy,bool success,int count)
    {
        string cdata=Path.Combine(output,label+".cdata");byte[] stale=new byte[8192];for(int i=0;i<stale.Length;i++)stale[i]=(byte)(i*13+7);File.WriteAllBytes(cdata,stale);
        AssetBuffer first=null;string firstError=null;string xmlText=null;
        for(int repeat=0;repeat<2;repeat++){
            XmlDocument xml;var declaration=Declaration(label,tiles,size,lossy,cdata,out xml);string inputXml=xml.OuterXml;File.WriteAllText(Path.Combine(output,"current.txt"),label+" repeat="+repeat);AssetBuffer actual=null;Exception error=null;
            try{actual=plugin.ProcessInstance(declaration);}catch(Exception e){error=e;}
            Check(xml.OuterXml==inputXml,"Original mutated XML "+label);Same(stale,File.ReadAllBytes(cdata),"Atlas cdata unchanged "+label);Check(success?error==null&&actual!=null:error!=null&&actual==null,"Original expected outcome "+label+": "+Error(error));
            if(repeat==0){first=actual;firstError=Error(error);xmlText=inputXml;}
            else{Check(Error(error)==firstError,"Repeated exception "+label);if(actual!=null){Same(first.InstanceData,actual.InstanceData,"Repeated instance "+label);Same(first.RelocationData,actual.RelocationData,"Repeated relo "+label);Same(first.ImportsData,actual.ImportsData,"Repeated imports "+label);}}
        }
        int bytes=0;string instanceHash="",reloHash="",importsHash="";
        File.WriteAllText(Path.Combine(output,label+".input.xml"),xmlText);
        if(success&&tiles.Length==0){Check(first.InstanceData==null&&first.RelocationData==null&&first.ImportsData==null,"Empty declared atlas returns null fields");bytes=-1;instanceHash=Hash(null);reloHash=Hash(null);importsHash=Hash(null);decoded++;}
        else if(success){Save(Path.Combine(output,label+".instance.bin"),first.InstanceData);Save(Path.Combine(output,label+".relo.bin"),first.RelocationData);Save(Path.Combine(output,label+".imports.bin"),first.ImportsData);Check(first.InstanceData!=null,"Nonempty declared atlas requires runtime data "+label);Check(first.InstanceData.Length>=28&&BitConverter.ToUInt32(first.InstanceData,4)==(uint)count,"Runtime root/count "+label);bytes=first.InstanceData.Length;instanceHash=Hash(first.InstanceData);reloHash=Hash(first.RelocationData);importsHash=Hash(first.ImportsData);
            int baseAt=checked((int)BitConverter.ToUInt32(first.InstanceData,12)),baseBytes=checked((int)BitConverter.ToUInt32(first.InstanceData,16));Check(baseAt>=28&&baseBytes>=128&&baseAt<=bytes-baseBytes&&BitConverter.ToUInt32(first.InstanceData,baseAt)==0x20534444u,"Embedded base DDS "+label);
            int normalAt=checked((int)BitConverter.ToUInt32(first.InstanceData,20)),normalBytes=checked((int)BitConverter.ToUInt32(first.InstanceData,24));Check(normalAt==0?normalBytes==0:normalAt>=28&&normalBytes>=128&&normalAt<=bytes-normalBytes&&BitConverter.ToUInt32(first.InstanceData,normalAt)==0x20534444u,"Embedded normal DDS "+label);
            Save(Path.Combine(output,label+".instance.bin"),first.InstanceData);Save(Path.Combine(output,label+".relo.bin"),first.RelocationData);Save(Path.Combine(output,label+".imports.bin"),first.ImportsData);decoded++;
        }else rejected++;
        File.WriteAllText(Path.Combine(output,label+".input.xml"),xmlText);transcript.WriteLine(label+"\t"+tiles.Length+"\t"+size+"\t"+lossy+"\t"+count+"\t"+bytes+"\t"+instanceHash+"\t"+reloHash+"\t"+importsHash+"\t"+firstError);fixtures++;
    }
    static int Main(string[] args)
    {
        IDisposable owner=null;string previous=Directory.GetCurrentDirectory();
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(input).GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));owner=(IDisposable)plugin;plugin.Initialize(null,TargetPlatform.Win32);Directory.SetCurrentDirectory(output);
            for(int i=0;i<50;i++)File.WriteAllBytes("base-"+i.ToString("D2")+".tga",Tga(256,256,i,32,false));File.WriteAllBytes("normal-256.tga",Tga(256,256,1,32,false));File.WriteAllBytes("normal-128.tga",Tga(128,128,2,32,false));File.WriteAllBytes("base-other.png",Tga(256,256,2,32,false));File.WriteAllBytes("base-small.tga",Tga(128,256,2,32,false));File.WriteAllBytes("reject.tga",Tga(256,256,2,16,true));File.WriteAllBytes("base-npot.tga",Tga(255,255,2,32,false));File.WriteAllBytes("base-24.tga",Tga(256,256,2,24,false));
            var sourceHashes=new Dictionary<string,string>();using(var inputs=new StreamWriter(Path.Combine(output,"input-files.tsv"))){inputs.WriteLine("file\tbytes\tsha256");foreach(string file in Directory.GetFiles(output)){if(!file.EndsWith(".tga")&&!file.EndsWith(".png"))continue;byte[] bytes=File.ReadAllBytes(file);sourceHashes[file]=Hash(bytes);inputs.WriteLine(Path.GetFileName(file)+"\t"+bytes.Length+"\t"+sourceHashes[file]);}}
            using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"))){transcript.AutoFlush=true;transcript.WriteLine("label\tdeclaredTiles\tatlasSize\tlossy\teffectiveTiles\tbytes\tinstanceSha256\treloSha256\timportsSha256\terror");
                foreach(int n in new[]{1,2,3,4})Case(plugin,transcript,output,"pattern-count-"+n,Tiles(n),2048,true,true,n);
                foreach(int depth in new[]{16,24,32}){
                    string file="pattern-depth-"+depth+".tga";File.WriteAllBytes(file,Tga(256,256,2,depth,false));sourceHashes[Path.GetFullPath(file)]=Hash(File.ReadAllBytes(file));
                    Case(plugin,transcript,output,"depth-"+depth,new[]{new Tile(0xffffffffu,file,"normal-128.tga")},2048,true,true,1);
                }
                File.Copy("base-00.tga","image.tga.backup");sourceHashes[Path.GetFullPath("image.tga.backup")]=Hash(File.ReadAllBytes("image.tga.backup"));
                Case(plugin,transcript,output,"tga-substring",new[]{new Tile(0,"image.tga.backup","absent.tga.backup")},2048,true,true,1);
                Case(plugin,transcript,output,"uppercase-path",new[]{new Tile(1,"BASE-01.TGA","NORMAL-256.TGA")},2048,true,true,1);
                foreach(string id in new[]{"0x80000001","0377","4294967295","4294967296","-1","-4294967296","+42tail","09","0x",""}){
                    var tile=new Tile(0,"base-02.tga","missing-normal.tga");tile.Id=id;
                    Case(plugin,transcript,output,"id-"+fixtures,new[]{tile},2048,true,true,1);
                }
                Case(plugin,transcript,output,"missing-and-valid",new[]{new Tile(9,"absent.png","absent.png"),new Tile(7,"base-00.tga","missing-normal.tga")},2048,true,true,1);
                Case(plugin,transcript,output,"pattern-base-npot",new[]{new Tile(7,"base-npot.tga","normal-128.tga")},2048,true,true,1);
                Case(plugin,transcript,output,"pattern-base-size",new[]{new Tile(1,"base-small.tga","normal-128.tga")},2048,true,false,0);
                Case(plugin,transcript,output,"pattern-normal-reject",new[]{new Tile(1,"base-00.tga","reject.tga")},2048,true,false,0);
            }
            using(var inputs=new StreamWriter(Path.Combine(output,"input-files.tsv"))){inputs.WriteLine("file\tbytes\tsha256");foreach(var source in sourceHashes)inputs.WriteLine(Path.GetFileName(source.Key)+"\t"+new FileInfo(source.Key).Length+"\t"+source.Value);}
            foreach(var source in sourceHashes)Check(Hash(File.ReadAllBytes(source.Key))==source.Value,"Original changed source image "+Path.GetFileName(source.Key));
            Check(fixtures==23&&decoded==21&&rejected==2,"Full additional original atlas entry corpus");Console.WriteLine("PROBE Texture PC atlas additional entry fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" calls="+(fixtures*2)+" checks="+checks+"; complete runtime/embedded DDS/relo/imports, XML and input files, repeated determinism and unchanged cdata; original-only, recovered/SDK not accepted");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{Directory.SetCurrentDirectory(previous);if(owner!=null)owner.Dispose();}
    }
}
