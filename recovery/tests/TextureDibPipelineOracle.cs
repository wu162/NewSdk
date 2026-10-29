using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using Pipeline=TextureBmpHeaderPipelineOracle;

// Shares the existing full-buffer/cdata/layout assertions. All DIB/table atlas inputs
// must actually succeed; matching errors cannot count as positive coverage.
internal static class TextureDibPipelineOracle
{
    const int ExpectedFixtures=50304;
    static void Replay(IAssetBuilderPlugin recovered,string work,string path)
    {
        using(var r=new BinaryReader(File.OpenRead(path))){
            if(r.ReadInt32()!=ExpectedFixtures)throw new Exception("Incomplete DIB/table pipeline corpus");
            while(r.BaseStream.Position<r.BaseStream.Length){
                string ext=r.ReadString();byte[] image=Pipeline.Bytes(r);string root=r.ReadString(),format=r.ReadString();bool stale=r.ReadBoolean(),mip=r.ReadBoolean();string kind=r.ReadString();
                string expectedError=r.ReadString();byte[] instance=Pipeline.Bytes(r),relo=Pipeline.Bytes(r),imports=Pipeline.Bytes(r),cdata=Pipeline.Bytes(r);
                string file=Path.Combine(work,"generated."+ext),output=Path.Combine(work,"recovered.cdata");File.WriteAllBytes(file,image);Pipeline.Seed(output,stale);
                AssetBuffer actual=null;Exception error=null;Pipeline.Isolated();try{actual=recovered.ProcessInstance(Pipeline.Declaration(root,file,format,output,mip,kind));}catch(Exception e){error=e;}Pipeline.Isolated();
                if(Pipeline.Error(error)!=expectedError)throw new Exception("DIB/table replay error "+Pipeline.fixtures+" expected="+expectedError+" actual="+Pipeline.Error(error));
                if(error is NotSupportedException)Pipeline.refusals++;else if(error!=null)Pipeline.errors++;
                if(expectedError==""){
                    if(actual==null)throw new Exception("Missing DIB/table replay buffers");
                    Pipeline.Equal(instance,actual.InstanceData,"DIB/table replay instance");Pipeline.Equal(relo,actual.RelocationData,"DIB/table replay relo");Pipeline.Equal(imports,actual.ImportsData,"DIB/table replay imports");
                }
                Pipeline.Equal(cdata,Pipeline.Cdata(output),"DIB/table replay full cdata");Pipeline.Equal(image,File.ReadAllBytes(file),"DIB/table replay source");Pipeline.fixtures++;
            }
        }
    }
    static void Refusal(IAssetBuilderPlugin recovered,string work,byte[] image,string ext,string root,bool stale,BinaryWriter w)
    {
        string file=Path.Combine(work,"generated."+ext),output=Path.Combine(work,"recovered.cdata");File.WriteAllBytes(file,image);Pipeline.Seed(output,stale);byte[] before=Pipeline.Cdata(output);
        AssetBuffer actual=null;Exception error=null;
        try{actual=recovered.ProcessInstance(Pipeline.Declaration(root,file,"A8R8G8B8",output,false,"StandardTexture"));}catch(Exception e){error=e;}
        if(!(error is NotSupportedException)||actual!=null)throw new Exception("Uncertain DIB/table bitfields must remain refused: "+error);
        Pipeline.Equal(before,Pipeline.Cdata(output),"Refusal cdata ownership");Pipeline.Equal(image,File.ReadAllBytes(file),"Refusal source unchanged");
        w.Write(ext);Pipeline.Bytes(w,image);w.Write(root);w.Write("A8R8G8B8");w.Write(stale);w.Write(false);w.Write("StandardTexture");
        w.Write(Pipeline.Error(error));Pipeline.Bytes(w,null);Pipeline.Bytes(w,null);Pipeline.Bytes(w,null);Pipeline.Bytes(w,before);Pipeline.fixtures++;Pipeline.refusals++;
    }
    static void Capture(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,string work,string path,string icc)
    {
        using(var w=new BinaryWriter(File.Create(path))){w.Write(ExpectedFixtures);
            int raw=0,transparent=0,atlas=0,indexed=0;
            foreach(var source in TextureDibConsumerFixtures.All(icc)){
                if(source.Atlas)atlas++;else if(source.Transparent)transparent++;else if(source.Indexed)indexed++;else raw++;
                foreach(string ext in new[]{"bmp","dib"}){
                    byte[] image=ext=="bmp"?source.Bmp:TextureDibConsumerFixtures.Bare(source.Bmp);
                    if(source.Atlas){
                        foreach(string normal in new[]{"normal","base-only"})foreach(bool stale in new[]{false,true})
                            Pipeline.Case(original,recovered,work,ext,image,"TerrainTextureAtlas",normal,stale,false,w);
                    }else if(source.Indexed){
                        foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(bool stale in new[]{false,true})
                            Pipeline.Case(original,recovered,work,ext,image,root,"A8R8G8B8",stale,false,w);
                    }else{
                        foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in source.Transparent?new[]{"DXT1A"}:new[]{"A8R8G8B8","X8R8G8B8","R5G6B5","DXT3","DXT5"})foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(bool mip in new[]{false,true})foreach(bool stale in new[]{false,true})
                            Pipeline.Case(original,recovered,work,ext,image,root,format,stale,mip,w,kind);
                    }
                }
            }
            foreach(int dib in new[]{52,56,108,124})foreach(int depth in new[]{16,24,32}){
                byte[] bmp=TextureBmpHeaderFixtures.Create(icc,16,8,depth,false,0,dib,3,"plain",0);
                foreach(string ext in new[]{"bmp","dib"})foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(bool stale in new[]{false,true})
                    Refusal(recovered,work,ext=="bmp"?bmp:TextureDibConsumerFixtures.Bare(bmp),ext,root,stale,w);
            }
            if(raw!=288||transparent!=96||atlas!=54||indexed!=78)throw new Exception("Complete DIB/table source families "+raw+"/"+transparent+"/"+atlas+"/"+indexed);
        }
    }
    static int Main(string[] args)
    {
        IAssetBuilderPlugin original=null,recovered=null;
        try{
            bool isolated=args.Length==3;if(!isolated&&args.Length!=5)throw new Exception("Usage: [original.dll] recovered.dll work replay [icc]");
            if(IntPtr.Size!=4)throw new Exception("x86 required");int shift=isolated?0:1;
            string work=Path.GetFullPath(args[shift+1]),replay=Path.GetFullPath(args[shift+2]);Directory.CreateDirectory(work);
            string directory=AppDomain.CurrentDomain.BaseDirectory;
            if(isolated)Pipeline.Isolated();
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            recovered=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(Path.GetFullPath(args[shift])).GetType("Recovered.Compiler.Texture.Plugin",true));recovered.Initialize(null,TargetPlatform.Win32);
            if(isolated)Replay(recovered,work,replay);
            else{
                string od=Path.GetFullPath(args[0]);using(var sha=SHA256.Create())if(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(od))).Replace("-","").ToLowerInvariant()!="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e")throw new Exception("Original hash");
                original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(od).GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));original.Initialize(null,TargetPlatform.Win32);
                Capture(original,recovered,work,replay,args[4]);
            }
            if(Pipeline.fixtures!=ExpectedFixtures||Pipeline.errors!=3952||Pipeline.refusals!=96)throw new Exception("Complete DIB/table pipeline accounting fixtures="+Pipeline.fixtures+" errors="+Pipeline.errors);
            Console.WriteLine("PASS Texture BMP DIB/table ProcessInstance "+(isolated?"NoOriginal":"differential")+" fixtures="+Pipeline.fixtures+" errors="+Pipeline.errors+" explicitRefusals="+Pipeline.refusals+" comparedBytes="+Pipeline.compared+"; all declared BMP/bare-DIB headers and truecolour tables/ICC; raw3/DXT3/5/transparent DXT1A standard/volume/full mip; 432 successful atlas cases; indexed and invalid-volume original errors; three buffers/full cdata/source/old tails and per-call original isolation; SDK separate");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}finally{if(original!=null)((IDisposable)original).Dispose();if(recovered!=null)((IDisposable)recovered).Dispose();}
    }
}
