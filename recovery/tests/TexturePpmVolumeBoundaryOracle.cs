using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using Pipeline=TextureBmpHeaderPipelineOracle;

// Shares the existing full-buffer/cdata/layout assertions. All PPM atlas inputs
// must actually succeed; matching errors cannot count as positive coverage.
internal static class TexturePpmVolumeBoundaryOracle
{
    const int ExpectedFixtures=11520;
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
    static void Capture(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,string work,string path)
    {
        using(var w=new BinaryWriter(File.Create(path))){w.Write(ExpectedFixtures);
            int sources=0;
            // Positive standard textures and actual invalid volume strips use
            // exactly the same P3/P6 bytes, including NPOT tall rectangles.
            foreach(int[] size in new[]{new[]{8,16},new[]{3,9},new[]{1,2}})
            foreach(int max in new[]{7,255})foreach(int format in new[]{3,6})
            foreach(int profile in new[]{0,1,3,4,8,9})foreach(int pattern in new[]{0,4}){
                byte[] image=TexturePpmFixtures.Create(size[0],size[1],max,format,pattern,profile);
                foreach(string root in new[]{"Texture","OnDemandTexture"})
                foreach(string output in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5","DXT3","DXT5"})
                foreach(string kind in new[]{"StandardTexture","VolumeTexture"})
                foreach(bool mip in new[]{false,true})foreach(bool stale in new[]{false,true})
                    Pipeline.Case(original,recovered,work,"ppm",image,root,output,stale,mip,w,kind);
                sources++;
            }
            if(sources!=144)throw new Exception("Complete PPM volume boundary source families");
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
            if(Pipeline.fixtures!=ExpectedFixtures||Pipeline.refusals!=0||Pipeline.errors!=5760)throw new Exception("Complete PPM pipeline accounting fixtures="+Pipeline.fixtures+" errors="+Pipeline.errors);
            Console.WriteLine("PASS Texture PPM tall-volume ProcessInstance "+(isolated?"NoOriginal":"differential")+" fixtures="+Pipeline.fixtures+" originalVolumeErrors="+Pipeline.errors+" positiveStandard=5760 comparedBytes="+Pipeline.compared+"; same P3/P6 tall POT/NPOT sources; raw3/DXT3/5 full mip, two roots, three buffers/full cdata/old tails, exact original BadTextureFile and per-call NoOriginal; SDK separate");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}finally{if(original!=null)((IDisposable)original).Dispose();if(recovered!=null)((IDisposable)recovered).Dispose();}
    }
}
