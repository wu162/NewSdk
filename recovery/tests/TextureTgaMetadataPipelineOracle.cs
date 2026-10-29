using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

// Original ProcessInstance capture + complete recovered comparison, then isolated replay.
// Contract helpers follow TextureDxt1aPipelineOracle; fixture axis is TGA metadata.
internal static class TextureTgaMetadataPipelineOracle
{
    const int ExpectedFixtures=47248;
    static int fixtures, rejected, errors; static long compared;
    static void Isolated(){if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present during replay");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded during replay");}
    static InstanceDeclaration Declaration(string root,string file,string format,string cdata,bool mipmaps,string kind)
    {
        var xml=new XmlDocument();var node=xml.CreateElement(root,"uri:ea.com:eala:asset");xml.AppendChild(node);
        node.SetAttribute("id","TgaMetadataPipeline");
        if(root=="TerrainTextureAtlas"){
            node.SetAttribute("AtlasSize","2048");node.SetAttribute("AllowLossyCompression","true");var tile=xml.CreateElement("Tile",node.NamespaceURI);tile.SetAttribute("TextureID","42");tile.SetAttribute("BaseTexture",file);tile.SetAttribute("NormalTexture",format=="normal"?file:Path.Combine(Path.GetDirectoryName(file),"missing-normal.tga"));node.AppendChild(tile);
        }else{node.SetAttribute("File",file);node.SetAttribute("Type",kind);}
        node.SetAttribute("OutputFormat",format);node.SetAttribute("GenerateMipMaps",mipmaps?"true":"false");node.SetAttribute("AllowAutomaticResize","false");
        var manager=new XmlNamespaceManager(xml.NameTable);manager.AddNamespace("ea","uri:ea.com:eala:asset");
        var doc=new AssetDeclarationDocument();Type t=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(t);t.GetField("NamespaceManager").SetValue(state,manager);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);
        var d=new InstanceDeclaration(doc);d.XmlNode=node;d.CustomDataPath=cdata;
        d.Handle.TypeHash=0x12345678u;d.Handle.InstanceHash=0x9abcdef0u;return d;
    }
    static void Equal(byte[] expected,byte[] actual,string label)
    {
        if(expected==null||actual==null){if(expected!=actual)throw new Exception(label+" null presence");return;}
        if(expected.Length!=actual.Length)throw new Exception(label+" length original="+expected.Length+" recovered="+actual.Length);
        for(int i=0;i<expected.Length;i++)if(expected[i]!=actual[i])throw new Exception(label+" byte="+i+" original="+expected[i]+" recovered="+actual[i]);
        compared+=expected.Length;
    }
    static void Seed(string path,bool stale){if(File.Exists(path))File.Delete(path);if(stale){byte[] b=new byte[8192];for(int i=0;i<b.Length;i++)b[i]=(byte)(i*13);File.WriteAllBytes(path,b);}}
    static byte[] Cdata(string path){return File.Exists(path)?File.ReadAllBytes(path):null;}
    static void Bytes(BinaryWriter w,byte[] b){w.Write(b==null?-1:b.Length);if(b!=null)w.Write(b);}
    static byte[] Bytes(BinaryReader r){int n=r.ReadInt32();if(n==-1)return null;if(n<0||n>16777216)throw new Exception("Invalid replay length");byte[] b=r.ReadBytes(n);if(b.Length!=n)throw new Exception("Truncated replay");return b;}
    static string Error(Exception e){if(e==null)return "";var b=e as BinaryAssetBuilderException;return e.GetType().FullName+"/"+(b==null?"":b.ErrorCode.ToString())+"/"+e.Message;}
    static void Case(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,string work,string extension,byte[] image,string root,string format,bool stale,bool mipmaps,BinaryWriter replay,string kind="StandardTexture")
    {
        string file=Path.Combine(work,"generated."+extension),oa=Path.Combine(work,"original.cdata"),ra=Path.Combine(work,"recovered.cdata");
        File.WriteAllBytes(file,image);Seed(oa,stale);Seed(ra,stale);
        AssetBuffer expected=null,actual=null;Exception ea=null,eb=null;
        try{expected=original.ProcessInstance(Declaration(root,file,format,oa,mipmaps,kind));}catch(Exception e){ea=e;}
        try{actual=recovered.ProcessInstance(Declaration(root,file,format,ra,mipmaps,kind));}catch(Exception e){eb=e;}
        string label=extension+"/"+root+"/"+format+"/kind="+kind+"/stale="+stale+"/mips="+mipmaps+"/"+fixtures;
        if(Error(ea)!=Error(eb))throw new Exception(label+" errors original="+Error(ea)+" recovered="+Error(eb));
        if(ea!=null)errors++;
        if(ea==null&&(expected==null||actual==null))throw new Exception(label+" missing buffer");
        if(ea==null){Equal(expected.InstanceData,actual.InstanceData,label+" instance");Equal(expected.RelocationData,actual.RelocationData,label+" relo");Equal(expected.ImportsData,actual.ImportsData,label+" imports");}
        byte[] cdata=Cdata(oa);Equal(cdata,Cdata(ra),label+" cdata");Equal(image,File.ReadAllBytes(file),label+" source");
        if(ea==null&&root=="Texture"){
            if((cdata!=null)!=stale||(cdata!=null&&cdata.Length!=8192)||actual.InstanceData.Length<(format=="DXT1A"?148:format=="R5G6B5"?142:format=="A8R8G8B8"||format=="X8R8G8B8"?144:156)||BitConverter.ToUInt32(actual.InstanceData,4)!=12||BitConverter.ToUInt32(actual.InstanceData,12)!=0x20534444)throw new Exception(label+" independent Texture DDS layout");
        }else if(ea==null&&root=="OnDemandTexture"){
            if(cdata==null||actual.InstanceData.Length!=8||cdata.Length<(format=="DXT1A"?176:format=="R5G6B5"?170:format=="A8R8G8B8"||format=="X8R8G8B8"?172:184)||BitConverter.ToUInt32(cdata,32)!=8||BitConverter.ToUInt32(cdata,40)!=0x20534444)throw new Exception(label+" independent OnDemand DDS layout");
        }
        if(ea==null&&root=="TerrainTextureAtlas"){
            if(actual.InstanceData.Length<32||BitConverter.ToUInt32(actual.InstanceData,4)!=1||BitConverter.ToUInt32(actual.InstanceData,12)!=28||BitConverter.ToUInt32(actual.InstanceData,28)!=0x20534444||BitConverter.ToUInt32(actual.InstanceData,24)==0!=(format=="base-only"))throw new Exception(label+" independent atlas runtime layout");
        }
        replay.Write(extension);Bytes(replay,image);replay.Write(root);replay.Write(format);replay.Write(stale);replay.Write(mipmaps);replay.Write(kind);
        replay.Write(Error(ea));Bytes(replay,expected==null?null:expected.InstanceData);Bytes(replay,expected==null?null:expected.RelocationData);Bytes(replay,expected==null?null:expected.ImportsData);Bytes(replay,cdata);fixtures++;
    }
    static int Main(string[] args)
    {
        IAssetBuilderPlugin original=null,recovered=null;
        try{
            bool isolated=args.Length==3;if(!isolated&&args.Length!=5)throw new Exception("Usage: [original.dll] recovered.dll work replay [immutable-gold-directory]");
            if(IntPtr.Size!=4)throw new Exception("x86 required");int shift=isolated?0:1;
            string rd=Path.GetFullPath(args[shift]),work=Path.GetFullPath(args[shift+1]),replay=Path.GetFullPath(args[shift+2]);Directory.CreateDirectory(work);
            string dir=AppDomain.CurrentDomain.BaseDirectory;
            if(isolated)Isolated();
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            recovered=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(rd).GetType("Recovered.Compiler.Texture.Plugin",true));recovered.Initialize(null,TargetPlatform.Win32);
            if(isolated){
                using(var r=new BinaryReader(File.OpenRead(replay))){int count=r.ReadInt32();if(count!=ExpectedFixtures)throw new Exception("Incomplete corpus");
                    while(r.BaseStream.Position<r.BaseStream.Length){string ext=r.ReadString();byte[] image=Bytes(r);string root=r.ReadString(),format=r.ReadString();bool stale=r.ReadBoolean(),mipmaps=r.ReadBoolean();string kind=r.ReadString();
                        string expectedError=r.ReadString();byte[] instance=Bytes(r),relo=Bytes(r),imports=Bytes(r),cdata=Bytes(r);
                        string file=Path.Combine(work,"generated."+ext),path=Path.Combine(work,"recovered.cdata");File.WriteAllBytes(file,image);Seed(path,stale);
                        AssetBuffer actual=null;Exception error=null;Isolated();try{actual=recovered.ProcessInstance(Declaration(root,file,format,path,mipmaps,kind));}catch(Exception e){error=e;}Isolated();
                        if(expectedError!=Error(error))throw new Exception("Replay error "+fixtures+" expected="+expectedError+" actual="+Error(error));
                        if(error!=null)errors++;
                        if(expectedError==""){if(actual==null)throw new Exception("Missing replay buffer");Equal(instance,actual.InstanceData,"replay instance "+fixtures);Equal(relo,actual.RelocationData,"replay relo");Equal(imports,actual.ImportsData,"replay imports");}
                        Equal(cdata,Cdata(path),"replay full cdata");Equal(image,File.ReadAllBytes(file),"replay source");fixtures++;
                    }if(fixtures!=count)throw new Exception("Replay count mismatch");
                }
            }else{
                string od=Path.GetFullPath(args[0]);using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(od))).Replace("-","").ToLowerInvariant()!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw new Exception("Original hash");
                original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(od).GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));original.Initialize(null,TargetPlatform.Win32);
                using(var w=new BinaryWriter(File.Create(replay))){w.Write(0);
                    int primary=0,partial=0;
                    foreach(string line in File.ReadAllLines(Path.Combine(args[4],"transcript.tsv"))){
                        if(line.StartsWith("label\t"))continue;string[] fields=line.Split('\t');if(fields.Length!=15)throw new Exception("Incomplete immutable TGA row");
                        int f=Array.IndexOf(TextureTgaMetadataFixtures.Formats,fields[9]),p=Array.IndexOf(TextureTgaMetadataFixtures.Profiles,fields[1]),width=int.Parse(fields[10]),height=int.Parse(fields[11]);
                        if(f<0||p<0)throw new Exception("Unknown immutable TGA axis");
                        bool main=(width==16&&height==8)||(width==7&&height==5),small=width==1&&height==1&&(fields[1]=="all"||fields[1]=="tables-first");
                        if(!(main||small)||bool.Parse(fields[12])!=(((f+p)&1)==0)||int.Parse(fields[13])!=((f&1)==0?32:16)||int.Parse(fields[14])!=f%3)continue;
                        byte[] image=File.ReadAllBytes(Path.Combine(args[4],fields[0]+".tga"));
                        using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(image)).Replace("-","").ToLowerInvariant()!=fields[8])throw new Exception("Immutable TGA input hash");
                        foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5","DXT3","DXT5"})foreach(bool mip in new[]{false,true})foreach(bool stale in new[]{false,true})foreach(string kind in new[]{"StandardTexture","VolumeTexture"})
                            Case(original,recovered,work,"tga",image,root,format,stale,mip,w,kind);
                        if(main)primary++;else partial++;
                    }
                    if(primary!=520||partial!=20)throw new Exception("Incomplete TGA metadata consumer source families");
                    foreach(string format in new[]{"rgba32","rgba32-noattr","index32"})foreach(string profile in TextureTgaMetadataFixtures.Profiles)foreach(int[] size in new[]{new[]{8,4},new[]{16,8},new[]{7,5},new[]{1,1}}){
                        if(size[0]==1&&profile!="all"&&profile!="chained")continue;
                        // Alpha63 stays below 0.5 for every actual block/mip/slice;
                        // all-zero TGA alpha would instead become opaque.
                        byte[] image=TextureTgaMetadataFixtures.Create(size[0],size[1],format,true,32,3,profile);
                        foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(bool mip in new[]{false,true})foreach(bool stale in new[]{false,true})foreach(string kind in new[]{"StandardTexture","VolumeTexture"})
                            Case(original,recovered,work,"tga",image,root,"DXT1A",stale,mip,w,kind);
                    }
                    foreach(string format in new[]{"rgb24","index32"})for(int p=0;p<TextureTgaMetadataFixtures.Profiles.Length;p++){
                        byte[] image=TextureTgaMetadataFixtures.Create(256,256,format,(p&1)==0,32,0,TextureTgaMetadataFixtures.Profiles[p]);
                        foreach(string normal in new[]{"normal","base-only"})foreach(bool stale in new[]{false,true})
                            Case(original,recovered,work,"tga",image,"TerrainTextureAtlas",normal,stale,false,w);
                    }
                    if(fixtures!=ExpectedFixtures)throw new Exception("Incomplete generated corpus: "+fixtures);w.BaseStream.Position=0;w.Write(fixtures);
                }
            }
            Console.WriteLine("PASS Texture TGA metadata ProcessInstance "+(isolated?"NoOriginal":"differential")+" fixtures="+fixtures+" errors="+errors+" comparedBytes="+compared+"; 10 storage formats/26 metadata profiles POT/NPOT/partial raw3/DXT3/5 and transparent DXT1A standard/volume complete mip; 208 atlas inputs; three buffers/full cdata and old tails; SDK separate");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(original!=null)((IDisposable)original).Dispose();if(recovered!=null)((IDisposable)recovered).Dispose();}
    }
}
