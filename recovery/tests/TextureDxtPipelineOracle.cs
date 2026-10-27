using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

// Original ProcessInstance capture + complete recovered comparison, then isolated replay.
internal static class TextureDxtPipelineOracle
{
    const int ExpectedFixtures=19404;
    static int fixtures, rejected, errors; static long compared;
    static void Isolated(){if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present during replay");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded during replay");}
    static InstanceDeclaration Declaration(string root,string file,string format,string cdata,bool mipmaps,string kind)
    {
        var xml=new XmlDocument();var node=xml.CreateElement(root,"uri:ea.com:eala:asset");xml.AppendChild(node);
        node.SetAttribute("id","DxtPipeline");node.SetAttribute("File",file);node.SetAttribute("Type",kind);
        node.SetAttribute("OutputFormat",format);node.SetAttribute("GenerateMipMaps",mipmaps?"true":"false");node.SetAttribute("AllowAutomaticResize","false");
        var manager=new XmlNamespaceManager(xml.NameTable);manager.AddNamespace("ea","uri:ea.com:eala:asset");
        var doc=new AssetDeclarationDocument();Type t=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(t);t.GetField("NamespaceManager").SetValue(state,manager);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);
        var d=new InstanceDeclaration(doc);d.XmlNode=node;d.CustomDataPath=cdata;
        d.Handle.TypeHash=0x12345678u;d.Handle.InstanceHash=0x9abcdef0u;return d;
    }
    static byte[] Tga(int width,int height,int pattern)
    {
        byte[] b=new byte[18+width*height*4];b[2]=2;b[12]=(byte)width;b[13]=(byte)(width>>8);b[14]=(byte)height;b[15]=(byte)(height>>8);b[16]=32;b[17]=40;
        for(int i=0;i<width*height;i++){
            int at=18+i*4;
            b[at]=(byte)(pattern==0?0:pattern==1?255:i*31);
            b[at+1]=(byte)(pattern==0?0:pattern==1?255:i*43+11);
            b[at+2]=(byte)(pattern==0?0:pattern==1?255:i*7+171);
            b[at+3]=(byte)(pattern==0?0:pattern==1?255:pattern==2?63:pattern==3?(i%2)*255:i*17);
        }
        if(pattern>=5){byte[] pixels=new byte[width*height*4];new Random(7919*pattern+width*31+height).NextBytes(pixels);Buffer.BlockCopy(pixels,0,b,18,pixels.Length);}
        return b;
    }
    static byte[] Tga16(int width,int height)
    {
        byte[] b=new byte[18+width*height*2];b[2]=2;b[12]=(byte)width;b[13]=(byte)(width>>8);b[14]=(byte)height;b[15]=(byte)(height>>8);b[16]=16;b[17]=33;
        for(int i=0;i<width*height;i++){ushort v=(ushort)(0x8000|((i*713)&0x7fff));b[18+i*2]=(byte)v;b[19+i*2]=(byte)(v>>8);}return b;
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
    static byte[] Bytes(BinaryReader r){int n=r.ReadInt32();if(n==-1)return null;if(n<0||n>1048576)throw new Exception("Invalid replay length");byte[] b=r.ReadBytes(n);if(b.Length!=n)throw new Exception("Truncated replay");return b;}
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
            if((cdata!=null)!=stale||(cdata!=null&&cdata.Length!=8192)||actual.InstanceData.Length<(format=="R5G6B5"?142:format=="A8R8G8B8"||format=="X8R8G8B8"?144:156)||BitConverter.ToUInt32(actual.InstanceData,4)!=12||BitConverter.ToUInt32(actual.InstanceData,12)!=0x20534444)throw new Exception(label+" independent Texture DDS layout");
        }else if(ea==null){
            if(cdata==null||actual.InstanceData.Length!=8||cdata.Length<(format=="R5G6B5"?170:format=="A8R8G8B8"||format=="X8R8G8B8"?172:184)||BitConverter.ToUInt32(cdata,32)!=8||BitConverter.ToUInt32(cdata,40)!=0x20534444)throw new Exception(label+" independent OnDemand DDS layout");
        }
        replay.Write(extension);Bytes(replay,image);replay.Write(root);replay.Write(format);replay.Write(stale);replay.Write(mipmaps);replay.Write(kind);
        replay.Write(Error(ea));Bytes(replay,expected==null?null:expected.InstanceData);Bytes(replay,expected==null?null:expected.RelocationData);Bytes(replay,expected==null?null:expected.ImportsData);Bytes(replay,cdata);fixtures++;
    }
    static void RejectBoundaries(IAssetBuilderPlugin recovered,string work)
    {
        // D3DX canonicalizes all-zero TGA alpha to opaque, so DXT1 remains DXT1.
        foreach(string root in new[]{"Texture","OnDemandTexture"}){
            string file=Path.Combine(work,"opaque-zero-alpha.tga"),cdata=Path.Combine(work,"reject.cdata");File.WriteAllBytes(file,Tga(8,4,0));Seed(cdata,false);
            bool failed=false;try{recovered.ProcessInstance(Declaration(root,file,"DXT1",cdata,false,"StandardTexture"));}catch(NotSupportedException){failed=true;}
            if(!failed||File.Exists(cdata))throw new Exception("All-zero TGA alpha becomes opaque and must retain DXT1 rejection");rejected++;
        }
        foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT1","DXT1A"}){
            string file=Path.Combine(work,"unsupported.tga"),cdata=Path.Combine(work,"reject.cdata");File.WriteAllBytes(file,Tga(4,4,1));Seed(cdata,false);
            bool failed=false;try{recovered.ProcessInstance(Declaration(root,file,format,cdata,false,"StandardTexture"));}catch(NotSupportedException){failed=true;}
            if(!failed||File.Exists(cdata))throw new Exception("Unsupported DXT1/1A must reject");rejected++;
        }
    }
    static int Main(string[] args)
    {
        IAssetBuilderPlugin original=null,recovered=null;
        try{
            bool isolated=args.Length==3;if(!isolated&&args.Length!=4)throw new Exception("Usage: [original.dll] recovered.dll work replay");
            if(IntPtr.Size!=4)throw new Exception("x86 required");
            int shift=isolated?0:1;string rd=Path.GetFullPath(args[shift]),work=Path.GetFullPath(args[shift+1]),replay=Path.GetFullPath(args[shift+2]);Directory.CreateDirectory(work);
            string dir=AppDomain.CurrentDomain.BaseDirectory;
            if(isolated&&File.Exists(Path.Combine(dir,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            recovered=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(rd).GetType("Recovered.Compiler.Texture.Plugin",true));recovered.Initialize(null,TargetPlatform.Win32);
            RejectBoundaries(recovered,work);
            if(isolated){
                using(var r=new BinaryReader(File.OpenRead(replay))){int count=r.ReadInt32();if(count!=ExpectedFixtures)throw new Exception("Incomplete corpus");
                    while(r.BaseStream.Position<r.BaseStream.Length){string ext=r.ReadString();byte[] image=Bytes(r);string root=r.ReadString(),format=r.ReadString();bool stale=r.ReadBoolean(),mipmaps=r.ReadBoolean();string kind=r.ReadString();
                        string expectedError=r.ReadString();byte[] instance=Bytes(r),relo=Bytes(r),imports=Bytes(r),cdata=Bytes(r);
                        string file=Path.Combine(work,"generated."+ext),path=Path.Combine(work,"recovered.cdata");File.WriteAllBytes(file,image);Seed(path,stale);
                        AssetBuffer actual=null;Exception error=null;
                        Isolated();try{actual=recovered.ProcessInstance(Declaration(root,file,format,path,mipmaps,kind));}catch(Exception e){error=e;}Isolated();
                        if(expectedError!=Error(error))throw new Exception("Replay error "+fixtures+" expected="+expectedError+" actual="+Error(error));
                        if(error!=null)errors++;
                        if(expectedError==""){if(actual==null)throw new Exception("Missing replay buffer");Equal(instance,actual.InstanceData,"replay instance "+fixtures);Equal(relo,actual.RelocationData,"replay relo");Equal(imports,actual.ImportsData,"replay imports");}
                        Equal(cdata,Cdata(path),"replay cdata");Equal(image,File.ReadAllBytes(file),"replay source");fixtures++;
                    }if(fixtures!=count)throw new Exception("Replay count mismatch");
                }
                foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded");
            }else{
                string od=Path.GetFullPath(args[0]);using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(od))).Replace("-","").ToLowerInvariant()!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw new Exception("Original hash");
                original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(od).GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));original.Initialize(null,TargetPlatform.Win32);
                using(var w=new BinaryWriter(File.Create(replay))){w.Write(0);
                    foreach(bool mipmaps in new[]{false,true}) {
                    foreach(int[] size in new[]{new[]{1,1},new[]{1,2},new[]{2,1},new[]{2,2},new[]{1,8},new[]{8,1},new[]{2,16},new[]{16,2},new[]{4,4},new[]{4,8},new[]{8,4},new[]{8,8},new[]{8,16},new[]{16,8},new[]{32,32}})foreach(int pattern in new[]{0,1,2,3,4,5,6,7})
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool stale in new[]{false,true})
                        Case(original,recovered,work,"tga",Tga(size[0],size[1],pattern),root,format,stale,mipmaps,w);
                    foreach(int side in new[]{1,2,4,8,16})foreach(int depth in new[]{1,2,4,8})foreach(int pattern in new[]{0,1,2,3,4,5,6,7})
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool stale in new[]{false,true})
                        Case(original,recovered,work,"tga",Tga(side*depth,side,pattern),root,format,stale,mipmaps,w,"VolumeTexture");
                    // POT images whose width is not a multiple of height must
                    // retain the original BadTextureFile volume-strip error.
                    foreach(int[] edge in new[]{new[]{1,2},new[]{2,4},new[]{4,8}})
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool stale in new[]{false,true})
                        Case(original,recovered,work,"tga",Tga(edge[0],edge[1],2),root,format,stale,mipmaps,w,"VolumeTexture");
                    // Inferred DXT1 alpha formats take the same accepted DXT3/5 route.
                    foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(int pattern in new[]{2,3,4,5})foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(bool stale in new[]{false,true})
                        Case(original,recovered,work,"tga",Tga(8,4,pattern),root,"DXT1",stale,mipmaps,w,kind);
                    foreach(int color in new[]{0,2,3,4,6})foreach(int depth in new[]{1,2,4,8,16}){
                        if(color==3?depth==16:color!=0&&depth<8)continue;
                        foreach(int interlace in new[]{0,1})foreach(int metadata in new[]{0,1}){
                            byte[] png=TexturePngOracle.Fixture(8,4,depth,color,interlace,5,metadata,0);
                            foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool stale in new[]{false,true})Case(original,recovered,work,"png",png,root,format,stale,mipmaps,w,kind);
                        }
                    }
                    foreach(int depth in new[]{1,4,8,16,24,32})foreach(bool top in new[]{false,true})foreach(int pattern in new[]{0,1,2}){
                        byte[] bmp=TextureBmpOracle.Fixture(8,4,depth,top,pattern,0,7);
                        foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool stale in new[]{false,true})Case(original,recovered,work,"bmp",bmp,root,format,stale,mipmaps,w,kind);
                    }
                    foreach(int profile in new[]{0,1,2})foreach(bool top in new[]{false,true}){
                        byte[] bmp=TextureBmpOracle.BitfieldsFixture(8,4,profile,top,0,7);
                        foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool stale in new[]{false,true})Case(original,recovered,work,"bmp",bmp,root,format,stale,mipmaps,w,kind);
                    }
                    }
                    string npotDirectory=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"npot-fixtures");string[] npotFiles=Directory.GetFiles(npotDirectory);Array.Sort(npotFiles,StringComparer.Ordinal);if(npotFiles.Length!=80)throw new Exception("Incomplete NPOT integration corpus");
                    foreach(string path in npotFiles)foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5","DXT3","DXT5"})foreach(bool mips in new[]{false,true})foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(bool stale in new[]{false,true})
                        Case(original,recovered,work,Path.GetExtension(path).Substring(1),File.ReadAllBytes(path),root,format,stale,mips,w,kind);
                    string storageDirectory=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"npot-storage-fixtures");string[] storageFiles=Directory.GetFiles(storageDirectory);Array.Sort(storageFiles,StringComparer.Ordinal);if(storageFiles.Length!=64)throw new Exception("Incomplete NPOT storage integration corpus");
                    foreach(string path in storageFiles)foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5","DXT3","DXT5"})foreach(bool mips in new[]{false,true})foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(bool stale in new[]{false,true})
                        Case(original,recovered,work,Path.GetExtension(path).Substring(1),File.ReadAllBytes(path),root,format,stale,mips,w,kind);
                    // Former NPOT rejection boundaries now receive full original
                    // buffer/cdata differential and the same isolated replay.
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool mips in new[]{false,true})
                        Case(original,recovered,work,"tga",Tga16(6,2),root,format,false,mips,w,"VolumeTexture");
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})
                        Case(original,recovered,work,"tga",Tga16(3,4),root,format,false,false,w);
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"DXT3","DXT5"})foreach(bool mips in new[]{false,true})foreach(string family in new[]{"png16","bmp16"})
                        Case(original,recovered,work,family=="png16"?"png":"bmp",family=="png16"?TexturePngOracle.Fixture(3,2,16,2,0,5,0,0):TextureBmpOracle.Fixture(3,2,16,false,0,0,7),root,format,false,mips,w);
                    if(fixtures!=ExpectedFixtures)throw new Exception("Incomplete generated corpus: "+fixtures);
                    w.BaseStream.Position=0;w.Write(fixtures);
                }
            }
            Console.WriteLine("PASS Texture DXT ProcessInstance "+(isolated?"NoOriginal":"differential")+" fixtures="+fixtures+" rejected="+rejected+" errors="+errors+" comparedBytes="+compared+"; PC DXT3/5 POT including partial-block and evidenced byte/5-bit/16-bit storage NPOT standard/volume no-mip/complete mip TGA/PNG/BMP; three buffers/full cdata; SDK pending");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(original!=null)((IDisposable)original).Dispose();if(recovered!=null)((IDisposable)recovered).Dispose();}
    }
}

