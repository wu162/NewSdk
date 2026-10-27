using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

// Original-only storage/quantization evidence, including negative decoder domains.
internal static unsafe class TextureNpotStorageStressProbe
{
    sealed class Snapshot { public int Status,Width,Height,Pitch;public uint Format;public byte[] Descriptor,Storage; }
    static int fixtures,decoded,rejected;
    static object Arg(MethodInfo m,int i,void* p){return Pointer.Box(p,m.GetParameters()[i].ParameterType);}
    static void Check(bool v,string label){if(!v)throw new Exception(label);}
    static string Hash(byte[] bytes){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static void Progress(string path,string phase){File.WriteAllText(Path.Combine(Path.GetDirectoryName(path),"current.txt"),Path.GetFileName(path)+"\t"+phase);}
    static Snapshot Native(Assembly assembly,string path)
    {
        var module=assembly.ManifestModule;var create=(MethodInfo)module.ResolveMethod(0x060000e8);var desc=(MethodInfo)module.ResolveMethod(0x060000d5);var locked=(MethodInfo)module.ResolveMethod(0x060000c5);var unlock=(MethodInfo)module.ResolveMethod(0x060000c6);var release=(MethodInfo)module.ResolveMethod(0x060000c4);
        Check(create.Name=="D3DXCreateTextureFromFileA"&&desc.Name=="XGGetTextureDesc","Native metadata");
        object device=module.ResolveField(0x04000039).GetValue(null);IntPtr texture=IntPtr.Zero;bool isLocked=false;var result=new Snapshot();byte[] name=Encoding.Default.GetBytes(path+"\0");
        try{
            Progress(path,"create");
            fixed(byte* p=name)result.Status=Convert.ToInt32(create.Invoke(null,new[]{device,Arg(create,1,p),Arg(create,2,&texture)}));
            if(result.Status<0)return result;Check(texture!=IntPtr.Zero,"Successful create requires texture");
            Progress(path,"descriptor status="+result.Status);
            byte* d=stackalloc byte[128];for(int i=0;i<128;i++)d[i]=0;
            desc.Invoke(null,new[]{Arg(desc,0,texture.ToPointer()),(object)0u,Arg(desc,2,d)});
            result.Descriptor=new byte[64];Marshal.Copy((IntPtr)d,result.Descriptor,0,64);result.Width=*(int*)(d+4);result.Height=*(int*)(d+8);result.Format=*(uint*)(d+16);
            Progress(path,"lock format="+result.Format.ToString("X8")+" size="+result.Width+"x"+result.Height);
            byte* r=stackalloc byte[16];for(int i=0;i<16;i++)r[i]=0;
            int status=Convert.ToInt32(locked.Invoke(null,new[]{Arg(locked,0,texture.ToPointer()),(object)0u,Arg(locked,2,r),Arg(locked,3,null),(object)0u}));
            Check(status==0,"Native lock");isLocked=true;result.Pitch=*(int*)r;byte* pixels=*(byte**)(r+4);
            Check(result.Width>0&&result.Height>0&&result.Height<=1024&&result.Pitch>0&&result.Pitch<=65536&&pixels!=null,"Bounded native snapshot");
            uint masked=result.Format&0xfffffe3fu;int stride=masked==0x28000002u?1:masked==0x0800000au||masked==0x28000018u||masked==0x28280003u||masked==0x18280003u?2:masked==0x18280006u||masked==0x28280006u?4:masked==0x1a20001au?8:0;
            Check(stride!=0,"Known original storage format");int rowBytes=checked(result.Width*stride);
            string description="format="+result.Format.ToString("X8")+" size="+result.Width+"x"+result.Height+" pitch="+result.Pitch+" logicalRow="+rowBytes;
            File.WriteAllText(path+".native.txt",description);Progress(path,"native prefix copy "+description);
            // The raw layout/pitch is not an allocation-size contract. Copy
            // only a first-row-sized prefix; full logical pixels are captured
            // independently through original LoadImageA below. Never read
            // trailing padding or infer rows from an unverified native pitch.
            result.Storage=new byte[rowBytes];Marshal.Copy((IntPtr)pixels,result.Storage,0,rowBytes);
            Progress(path,"copied "+description);return result;
        }finally{if(isLocked)unlock.Invoke(null,new[]{Arg(unlock,0,texture.ToPointer()),(object)0u});if(texture!=IntPtr.Zero)release.Invoke(null,new[]{Arg(release,0,texture.ToPointer())});}
    }
    static void Case(Assembly original,MethodInfo load,StreamWriter transcript,string directory,string label,string profile,int width,int height,int pattern,int metadata,string extension,byte[] file)
    {
        string path=Path.Combine(directory,label+"."+extension);File.WriteAllBytes(path,file);Progress(path,"native begin");Snapshot native=Native(original,path);TexturePacker.Image image=null;Exception error=null;
        Progress(path,"LoadImageA");
        try{image=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(TargetInvocationException e){error=e;while(error is TargetInvocationException&&error.InnerException!=null)error=error.InnerException;}
        Check(Hash(File.ReadAllBytes(path))==Hash(file),"Original modified source "+label);
        if(native.Descriptor!=null)File.WriteAllBytes(Path.Combine(directory,label+".descriptor.bin"),native.Descriptor);
        if(native.Storage!=null)File.WriteAllBytes(Path.Combine(directory,label+".locked.bin"),native.Storage);
        string digest="",failure="",errorCode="";int lw=0,lh=0;bool alpha=false;
        if(error==null){Check(image!=null&&native.Status>=0,"Successful load/create agreement "+label);lw=image.Width;lh=image.Height;alpha=image.HasAlpha;Check(lw==native.Width&&lh==native.Height,"Descriptor/load dimensions "+label);byte[] rgba=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,rgba,0,rgba.Length);File.WriteAllBytes(Path.Combine(directory,label+".loaded.rgba"),rgba);digest=Hash(rgba);decoded++;}
        else{Check(native.Status<0,"Failed load should match native create failure "+label);failure=(error.GetType().FullName+":"+error.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');var code=error.GetType().GetProperty("ErrorCode");errorCode=code==null?"":Convert.ToString(code.GetValue(error,null));rejected++;}
        transcript.WriteLine(label+"\t"+profile+"\t"+width+"\t"+height+"\t"+pattern+"\t"+metadata+"\t"+native.Status+"\t"+native.Format.ToString("X8")+"\t"+native.Width+"\t"+native.Height+"\t"+native.Pitch+"\t"+(native.Storage==null?0:native.Storage.Length)+"\t"+lw+"\t"+lh+"\t"+alpha+"\t"+digest+"\t"+failure+"\t"+errorCode);fixtures++;
    }
    static byte[] Tga(MethodInfo fixture,int width,int height,int family,int depth,int paletteDepth,bool rle,int direction,int attributes,int alpha)
    {return(byte[])fixture.Invoke(null,new object[]{width,height,family,depth,paletteDepth,family==1?5:0,family==1?4:0,rle,direction,attributes,7,alpha});}
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            var original=Assembly.LoadFrom(input);typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{original,input});owner=(IDisposable)typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
            MethodInfo load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic),tga=typeof(TextureTgaFormatsOracle).GetMethod("Fixture",BindingFlags.Static|BindingFlags.NonPublic);
            int[][] sizes={new[]{31,17},new[]{17,31},new[]{65,3},new[]{3,65},new[]{129,1},new[]{1,129},new[]{63,33},new[]{64,32}};
            int[][] png={new[]{0,1},new[]{0,2},new[]{0,4},new[]{0,8},new[]{0,16},new[]{4,8},new[]{2,16},new[]{6,16}};
            int[][] positiveTga={new[]{2,16,0,0},new[]{2,16,0,1},new[]{3,8,0,0},new[]{1,8,15,0},new[]{1,8,16,0},new[]{1,8,24,0},new[]{1,8,32,0}};
            int[][] negativeTga={new[]{3,16,0,0},new[]{3,16,0,8},new[]{1,16,15,0},new[]{1,16,16,0},new[]{1,16,24,0},new[]{1,16,32,0}};
            using(var transcript=new StreamWriter(Path.Combine(output,"transcript.tsv"),false,new UTF8Encoding(false))){
                transcript.AutoFlush=true;transcript.WriteLine("label\tprofile\tsourceWidth\tsourceHeight\tpattern\tmetadata\tcreateStatus\tnativeFormat\tnativeWidth\tnativeHeight\tpitch\tnativeBytes\tloadedWidth\tloadedHeight\thasAlpha\tsha256\terror\terrorCode");
                foreach(int[] size in sizes){int w=size[0],h=size[1];
                    foreach(int[] format in png)foreach(int interlace in new[]{0,1})foreach(int metadata in new[]{0,1,4,5,7,9})for(int pattern=0;pattern<4;pattern++){
                        string profile="png-c"+format[0]+"-d"+format[1]+"-i"+interlace;string label=profile+"-m"+metadata+"-"+w+"x"+h+"-p"+pattern;
                        Case(original,load,transcript,output,label,profile,w,h,pattern,metadata,"png",TexturePngOracle.Fixture(w,h,format[1],format[0],interlace,5,metadata,pattern));
                    }
                    foreach(int depth in new[]{1,2,4,8})foreach(int interlace in new[]{0,1})foreach(int metadata in new[]{0,1})foreach(int pattern in new[]{0,3}){
                        string profile="png-c3-d"+depth+"-i"+interlace;string label=profile+"-m"+metadata+"-"+w+"x"+h+"-p"+pattern;Case(original,load,transcript,output,label,profile,w,h,pattern,metadata,"png",TexturePngOracle.Fixture(w,h,depth,3,interlace,5,metadata,pattern));
                    }
                    foreach(int interlace in new[]{0,1}){string profile="png-c4-d16-i"+interlace;Case(original,load,transcript,output,profile+"-"+w+"x"+h,profile,w,h,0,0,"png",TexturePngOracle.Fixture(w,h,16,4,interlace,5,0,0));}
                    foreach(int[] format in positiveTga)foreach(bool rle in new[]{false,true})foreach(int direction in new[]{0,48})for(int alpha=0;alpha<4;alpha++){
                        string profile="tga-f"+format[0]+"-d"+format[1]+"-pal"+format[2]+"-a"+format[3]+"-rle"+rle+"-dir"+direction;Case(original,load,transcript,output,profile+"-"+w+"x"+h+"-p"+alpha,profile,w,h,alpha,0,"tga",Tga(tga,w,h,format[0],format[1],format[2],rle,direction,format[3],alpha));
                    }
                    foreach(int[] format in negativeTga){string profile="tga-f"+format[0]+"-d"+format[1]+"-pal"+format[2]+"-a"+format[3];Case(original,load,transcript,output,profile+"-"+w+"x"+h,profile,w,h,3,0,"tga",Tga(tga,w,h,format[0],format[1],format[2],false,32,format[3],3));}
                    foreach(int depth in new[]{1,4,8,16})foreach(bool top in new[]{false,true})foreach(int pattern in depth==16?new[]{0,1,2}:new[]{0,2}){
                        string profile="bmp-d"+depth+"-top"+top;Case(original,load,transcript,output,profile+"-"+w+"x"+h+"-p"+pattern,profile,w,h,pattern,0,"bmp",TextureBmpOracle.Fixture(w,h,depth,top,pattern,0,7));
                    }
                }
            }
            Check(fixtures==4432,"Full storage probe corpus count");Console.WriteLine("PROBE Texture NPOT storage larger-ratio fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+"; source/texture descriptor/full locked data and final RGBA; original-only, not recovered acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
