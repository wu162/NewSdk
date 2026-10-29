using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using Pipeline=TextureBmpHeaderPipelineOracle;

// Shares the existing full-buffer/cdata/layout assertions. All OS2 atlas inputs
// must actually succeed; matching errors cannot count as positive coverage.
internal static class TextureBmpOs2PipelineOracle
{
    const int ExpectedFixtures=28548;
    static void Replay(IAssetBuilderPlugin recovered,string work,string path)
    {
        using(var r=new BinaryReader(File.OpenRead(path))){
            if(r.ReadInt32()!=ExpectedFixtures)throw new Exception("Incomplete OS2 pipeline corpus");
            while(r.BaseStream.Position<r.BaseStream.Length){
                string ext=r.ReadString();byte[] image=Pipeline.Bytes(r);string root=r.ReadString(),format=r.ReadString();bool stale=r.ReadBoolean(),mip=r.ReadBoolean();string kind=r.ReadString();
                string expectedError=r.ReadString();byte[] instance=Pipeline.Bytes(r),relo=Pipeline.Bytes(r),imports=Pipeline.Bytes(r),cdata=Pipeline.Bytes(r);
                string file=Path.Combine(work,"generated."+ext),output=Path.Combine(work,"recovered.cdata");File.WriteAllBytes(file,image);Pipeline.Seed(output,stale);
                AssetBuffer actual=null;Exception error=null;Pipeline.Isolated();try{actual=recovered.ProcessInstance(Pipeline.Declaration(root,file,format,output,mip,kind));}catch(Exception e){error=e;}Pipeline.Isolated();
                if(Pipeline.Error(error)!=expectedError)throw new Exception("OS2 replay error "+Pipeline.fixtures+" expected="+expectedError+" actual="+Pipeline.Error(error));
                if(error!=null)Pipeline.errors++;
                if(expectedError==""){
                    if(actual==null)throw new Exception("Missing OS2 replay buffers");
                    Pipeline.Equal(instance,actual.InstanceData,"OS2 replay instance");Pipeline.Equal(relo,actual.RelocationData,"OS2 replay relo");Pipeline.Equal(imports,actual.ImportsData,"OS2 replay imports");
                }
                Pipeline.Equal(cdata,Pipeline.Cdata(output),"OS2 replay full cdata");Pipeline.Equal(image,File.ReadAllBytes(file),"OS2 replay source");Pipeline.fixtures++;
            }
        }
    }
    static void Capture(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,string work,string path)
    {
        using(var w=new BinaryWriter(File.Create(path))){w.Write(ExpectedFixtures);
            foreach(int dib in new[]{12,64})foreach(bool top in dib==12?new[]{false}:new[]{false,true}){
                foreach(int depth in new[]{16,24,32})foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1},new[]{8,16}})foreach(int pattern in new[]{0,1,2})foreach(int gap in new[]{0,3,17}){
                    byte[] image=TextureBmpOs2Fixtures.Create(size[0],size[1],depth,top,pattern,dib,gap,0);
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5","DXT3","DXT5"})foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(bool mip in new[]{false,true})foreach(bool stale in new[]{false,true})
                        Pipeline.Case(original,recovered,work,"bmp",image,root,format,stale,mip,w,kind);
                }
                foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1},new[]{8,16}})foreach(int gap in new[]{0,3,17}){
                    byte[] image=TextureBmpOs2Fixtures.Transparent(size[0],size[1],top,dib,gap);
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string kind in new[]{"StandardTexture","VolumeTexture"})foreach(bool mip in new[]{false,true})foreach(bool stale in new[]{false,true})
                        Pipeline.Case(original,recovered,work,"bmp",image,root,"DXT1A",stale,mip,w,kind);
                }
                foreach(int depth in new[]{16,24,32})foreach(int gap in new[]{0,3,17}){
                    byte[] image=TextureBmpOs2Fixtures.Create(256,256,depth,top,0,dib,gap,0);
                    foreach(string normal in new[]{"normal","base-only"})foreach(bool stale in new[]{false,true})
                        Pipeline.Case(original,recovered,work,"tga",image,"TerrainTextureAtlas",normal,stale,false,w);
                }
                foreach(int depth in new[]{1,4,8})foreach(int[] size in new[]{new[]{16,8},new[]{7,5},new[]{1,1}})foreach(int pattern in new[]{0,1,2})foreach(int gap in new[]{0,3,17})foreach(int reserved in new[]{0,255}){
                    byte[] image=TextureBmpOs2Fixtures.Create(size[0],size[1],depth,top,pattern,dib,gap,reserved);
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(bool stale in new[]{false,true})
                        Pipeline.Case(original,recovered,work,"bmp",image,root,"A8R8G8B8",stale,false,w);
                }
            }
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
            if(Pipeline.fixtures!=ExpectedFixtures||Pipeline.errors!=5256)throw new Exception("Complete OS2 pipeline accounting fixtures="+Pipeline.fixtures+" errors="+Pipeline.errors);
            Console.WriteLine("PASS Texture BMP OS2 ProcessInstance "+(isolated?"NoOriginal":"differential")+" fixtures="+Pipeline.fixtures+" errors="+Pipeline.errors+" comparedBytes="+Pipeline.compared+"; core12/OS2-64 raw3/DXT3/5/transparent DXT1A standard/volume/full mip; 108 successful atlas cases; indexed and invalid-volume original errors; three buffers/full cdata/source/old tails and per-call original isolation; SDK separate");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}finally{if(original!=null)((IDisposable)original).Dispose();if(recovered!=null)((IDisposable)recovered).Dispose();}
    }
}
