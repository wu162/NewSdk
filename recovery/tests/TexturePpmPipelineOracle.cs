using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using Pipeline=TextureBmpHeaderPipelineOracle;

// Shares the existing full-buffer/cdata/layout assertions. All PPM atlas inputs
// must actually succeed; matching errors cannot count as positive coverage.
internal static class TexturePpmPipelineOracle
{
    const int ExpectedFixtures=41120;
    static void Replay(IAssetBuilderPlugin recovered,string work,string path)
    {
        using(var r=new BinaryReader(File.OpenRead(path))){
            if(r.ReadInt32()!=ExpectedFixtures)throw new Exception("Incomplete PPM pipeline corpus");
            while(r.BaseStream.Position<r.BaseStream.Length){
                string ext=r.ReadString();byte[] image=Pipeline.Bytes(r);string root=r.ReadString(),format=r.ReadString();bool stale=r.ReadBoolean(),mip=r.ReadBoolean();string kind=r.ReadString();
                string expectedError=r.ReadString();byte[] instance=Pipeline.Bytes(r),relo=Pipeline.Bytes(r),imports=Pipeline.Bytes(r),cdata=Pipeline.Bytes(r);
                string file=Path.Combine(work,"generated."+ext),output=Path.Combine(work,"recovered.cdata");File.WriteAllBytes(file,image);Pipeline.Seed(output,stale);
                AssetBuffer actual=null;Exception error=null;Pipeline.Isolated();try{actual=recovered.ProcessInstance(Pipeline.Declaration(root,file,format,output,mip,kind));}catch(Exception e){error=e;}Pipeline.Isolated();
                if(Pipeline.Error(error)!=expectedError)throw new Exception("PPM replay error "+Pipeline.fixtures+" expected="+expectedError+" actual="+Pipeline.Error(error));
                if(error is NotSupportedException)Pipeline.refusals++;else if(error!=null)Pipeline.errors++;
                if(expectedError==""){
                    if(actual==null)throw new Exception("Missing PPM replay buffers");
                    Pipeline.Equal(instance,actual.InstanceData,"PPM replay instance");Pipeline.Equal(relo,actual.RelocationData,"PPM replay relo");Pipeline.Equal(imports,actual.ImportsData,"PPM replay imports");
                }
                Pipeline.Equal(cdata,Pipeline.Cdata(output),"PPM replay full cdata");Pipeline.Equal(image,File.ReadAllBytes(file),"PPM replay source");Pipeline.fixtures++;
            }
        }
    }
    static void Refusal(IAssetBuilderPlugin recovered,string work,byte[] image,string ext,string root,string format,bool stale,BinaryWriter w)
    {
        string file=Path.Combine(work,"generated."+ext),output=Path.Combine(work,"recovered.cdata");File.WriteAllBytes(file,image);Pipeline.Seed(output,stale);byte[] before=Pipeline.Cdata(output);
        AssetBuffer actual=null;Exception error=null;
        try{actual=recovered.ProcessInstance(Pipeline.Declaration(root,file,format,output,false,"StandardTexture"));}catch(Exception e){error=e;}
        if(!(error is NotSupportedException)||actual!=null)throw new Exception("Opaque PPM DXT1/1A must remain refused: "+error);
        Pipeline.Equal(before,Pipeline.Cdata(output),"Refusal cdata ownership");Pipeline.Equal(image,File.ReadAllBytes(file),"Refusal source unchanged");
        w.Write(ext);Pipeline.Bytes(w,image);w.Write(root);w.Write(format);w.Write(stale);w.Write(false);w.Write("StandardTexture");
        w.Write(Pipeline.Error(error));Pipeline.Bytes(w,null);Pipeline.Bytes(w,null);Pipeline.Bytes(w,null);Pipeline.Bytes(w,before);Pipeline.fixtures++;Pipeline.refusals++;
    }
    static void Capture(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,string work,string path)
    {
        using(var w=new BinaryWriter(File.Create(path))){w.Write(ExpectedFixtures);
            int raw=0,atlas=0,pgm=0;
            foreach(int[] size in new[]{new[]{8,4},new[]{7,5},new[]{1,1},new[]{16,16}})
            foreach(int max in new[]{1,7,255,65535})foreach(int format in new[]{3,6})
            foreach(int profile in new[]{0,1,2,3,4,5,8,9})foreach(int pattern in new[]{0,4}){
                byte[] image=TexturePpmFixtures.Create(size[0],size[1],max,format,pattern,profile);
                foreach(string root in new[]{"Texture","OnDemandTexture"})
                foreach(string output in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5","DXT3","DXT5"})
                foreach(string kind in new[]{"StandardTexture","VolumeTexture"})
                foreach(bool mip in new[]{false,true})foreach(bool stale in new[]{false,true})
                    Pipeline.Case(original,recovered,work,"ppm",image,root,output,stale,mip,w,kind);
                raw++;
            }
            foreach(int max in new[]{7,255})foreach(int format in new[]{3,6})
            foreach(int profile in new[]{0,4})foreach(int pattern in new[]{0,4}){
                byte[] image=TexturePpmFixtures.Create(256,256,max,format,pattern,profile);
                foreach(string normal in new[]{"normal","base-only"})foreach(bool stale in new[]{false,true})
                    Pipeline.Case(original,recovered,work,"ppm",image,"TerrainTextureAtlas",normal,stale,false,w);
                atlas++;
            }
            foreach(int[] size in new[]{new[]{8,4},new[]{7,5},new[]{1,1},new[]{16,16}})
            foreach(int format in new[]{2,5})foreach(int pattern in new[]{0,4}){
                byte[] image=TexturePpmFixtures.Create(size[0],size[1],255,format,pattern,0);
                foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(bool stale in new[]{false,true})
                    Pipeline.Case(original,recovered,work,"ppm",image,root,"A8R8G8B8",stale,false,w);
                pgm++;
            }
            foreach(int[] size in new[]{new[]{8,4},new[]{7,5},new[]{1,1},new[]{16,16}}){
                byte[] image=TexturePpmFixtures.Create(size[0],size[1],255,6,0,0);
                foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(bool stale in new[]{false,true})
                foreach(string output in new[]{"DXT1","DXT1A"})
                    Refusal(recovered,work,image,"ppm",root,output,stale,w);
            }
            if(raw!=512||atlas!=16||pgm!=16)throw new Exception("Complete PPM source families");
        }
    }
    static int Main(string[] args)
    {
        IAssetBuilderPlugin original=null,recovered=null;
        try{
            bool isolated=args.Length==3;if(!isolated&&args.Length!=4)throw new Exception("Usage: [original.dll] recovered.dll work replay");
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
                Capture(original,recovered,work,replay);
            }
            if(Pipeline.fixtures!=ExpectedFixtures||Pipeline.refusals!=32)throw new Exception("Complete PPM pipeline accounting fixtures="+Pipeline.fixtures+" errors="+Pipeline.errors);
            Console.WriteLine("PASS Texture BMP PPM ProcessInstance "+(isolated?"NoOriginal":"differential")+" fixtures="+Pipeline.fixtures+" errors="+Pipeline.errors+" explicitRefusals="+Pipeline.refusals+" comparedBytes="+Pipeline.compared+"; P3/P6 token/maxval/separator/NPOT; raw3/DXT3/5 standard/volume/full mip; 64 successful atlas cases; PGM/invalid header/invalid-volume original errors; opaque DXT1/1A refusals; three buffers/full cdata/source/old tails and per-call original isolation; SDK separate");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}finally{if(original!=null)((IDisposable)original).Dispose();if(recovered!=null)((IDisposable)recovered).Dispose();}
    }
}
