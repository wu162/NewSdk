using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Recovered.Compiler.Texture;

// Original-only descriptor/storage/RGBA evidence. All native method tokens and
// ABI layouts are the already-used TextureNpotStorageProbe contracts; no custom
// unmanaged function is invoked. Repeated equality is an observation, not gold.
internal static unsafe class TextureBmpBitfieldsProbe
{
    sealed class Snapshot
    {
        internal int Status, Width, Height, Pitch;
        internal uint Format;
        internal byte[] Descriptor, Storage;
        internal string Error = "";
    }
    sealed class Loaded
    {
        internal int Width, Height;
        internal bool HasAlpha;
        internal byte[] Rgba;
        internal string Error = "";
    }
    static Assembly original;
    static MethodInfo create, describe, lockRect, unlockRect, release, load;
    static object device;
    static string warmup;
    static int fixtures, controls, decoded, rejected, unstable, checks;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static object Arg(MethodInfo method,int index,void* pointer){return Pointer.Box(pointer,method.GetParameters()[index].ParameterType);}
    static string Hash(byte[] bytes){if(bytes==null)return "";using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception error)
    {
        if(error==null)return "";while(error is TargetInvocationException&&error.InnerException!=null)error=error.InnerException;
        var property=error.GetType().GetProperty("ErrorCode");
        return (error.GetType().FullName+"/"+(property==null?"":Convert.ToString(property.GetValue(error,null)))+"/"+error.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');
    }
    static bool Equal(byte[] first,byte[] second)
    {
        if(first==null||second==null)return first==second;
        if(first.Length!=second.Length)return false;
        for(int i=0;i<first.Length;i++)if(first[i]!=second[i])return false;return true;
    }
    static Snapshot Native(string path)
    {
        IntPtr texture=IntPtr.Zero;bool locked=false;Snapshot result=new Snapshot();
        byte[] name=Encoding.Default.GetBytes(path+"\0");
        try
        {
            fixed(byte* p=name)result.Status=Convert.ToInt32(create.Invoke(null,new[]{device,Arg(create,1,p),Arg(create,2,&texture)}));
            if(result.Status<0)return result;
            Check(texture!=IntPtr.Zero,"Successful native create requires texture");
            // Original _XGTEXTURE_DESC is exactly 60 bytes, _D3DLOCKED_RECT 8.
            byte* descriptor=stackalloc byte[60];for(int i=0;i<60;i++)descriptor[i]=0;
            describe.Invoke(null,new[]{Arg(describe,0,texture.ToPointer()),(object)0u,Arg(describe,2,descriptor)});
            result.Descriptor=new byte[60];Marshal.Copy((IntPtr)descriptor,result.Descriptor,0,60);
            result.Width=*(int*)(descriptor+4);result.Height=*(int*)(descriptor+8);result.Format=*(uint*)(descriptor+16);
            byte* rectangle=stackalloc byte[8];for(int i=0;i<8;i++)rectangle[i]=0;
            int status=Convert.ToInt32(lockRect.Invoke(null,new[]{Arg(lockRect,0,texture.ToPointer()),(object)0u,Arg(lockRect,2,rectangle),Arg(lockRect,3,null),(object)0u}));
            Check(status==0,"Native lock failed");locked=true;result.Pitch=*(int*)rectangle;byte* pixels=*(byte**)(rectangle+4);
            Check(result.Width>0&&result.Width<=64&&result.Height>0&&result.Height<=64&&result.Pitch>0&&result.Pitch<=4096&&pixels!=null,"Bounded native locked surface");
            result.Storage=new byte[checked(result.Pitch*result.Height)];Marshal.Copy((IntPtr)pixels,result.Storage,0,result.Storage.Length);
        }
        catch(Exception e){result.Error=Error(e);}
        finally
        {
            if(locked)unlockRect.Invoke(null,new[]{Arg(unlockRect,0,texture.ToPointer()),(object)0u});
            if(texture!=IntPtr.Zero)release.Invoke(null,new[]{Arg(release,0,texture.ToPointer())});
        }
        return result;
    }
    static Loaded Load(string path)
    {
        Loaded result=new Loaded();
        try
        {
            var image=(TexturePacker.Image)load.Invoke(null,new object[]{path});result.Width=image.Width;result.Height=image.Height;result.HasAlpha=image.HasAlpha;
            result.Rgba=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,result.Rgba,0,result.Rgba.Length);
        }
        catch(Exception e){result.Error=Error(e);}return result;
    }
    static void Seed(int context)
    {
        if(context==0)return;
        Loaded known=Load(warmup);Check(known.Error==""&&known.Width==16&&known.Height==8&&known.Rgba!=null,"Known BI_RGB context control");
    }
    static void Word(byte[] bytes,int position,uint value){for(int i=0;i<4;i++)bytes[position+i]=(byte)(value>>(8*i));}
    static byte[] Fixture(int width,int height,int depth,bool top,int pattern,int dib,uint[] masks,int compression)
    {
        byte[] basic=TextureBmpOracle.Fixture(width,height,depth,top,pattern,0,0);
        int external=dib==40&&compression==3?12:0,offset=14+dib+external;
        byte[] result=new byte[offset+basic.Length-54];Buffer.BlockCopy(basic,0,result,0,54);
        Word(result,2,(uint)result.Length);Word(result,10,(uint)offset);Word(result,14,(uint)dib);Word(result,30,(uint)compression);
        if(compression==3)
        {
            for(int channel=0;channel<3;channel++)Word(result,54+channel*4,masks[channel]);
            if(dib>=56)Word(result,66,masks[3]);
        }
        Buffer.BlockCopy(basic,54,result,offset,basic.Length-54);return result;
    }
    static uint[] Masks(int depth,int profile)
    {
        switch(profile)
        {
            case 0:return depth==16?new uint[]{0xf800,0x7e0,0x1f,0}:new uint[]{0xff0000,0xff00,0xff,0};
            case 1:return new uint[]{0x7c00,0x3e0,0x1f,0};
            case 2:return depth==16?new uint[]{0x7c00,0x3e0,0x1f,0x8000}:new uint[]{0xff0000,0xff00,0xff,depth==32?0xff000000u:0u};
            case 3:return new uint[]{0xf00,0xf0,0xf,0xf000};
            case 4:return depth==16?new uint[]{0x1f,0x7e0,0xf800,0}:new uint[]{0xff,0xff00,0xff0000,0};
            case 5:return new uint[]{0xe0,0x1c,0x3,0};
            case 6:return new uint[]{0x4,0x2,0x1,0x8};
            default:return new uint[]{0,0,0,0};
        }
    }
    static string SnapshotColumns(Snapshot sample)
    {
        return sample.Status+"\t"+sample.Width+"\t"+sample.Height+"\t"+sample.Format.ToString("X8")+"\t"+sample.Pitch+"\t"+(sample.Storage==null?0:sample.Storage.Length)+"\t"+Hash(sample.Descriptor)+"\t"+Hash(sample.Storage)+"\t"+sample.Error;
    }
    static string LoadedColumns(Loaded sample)
    {
        return sample.Width+"\t"+sample.Height+"\t"+sample.HasAlpha+"\t"+(sample.Rgba==null?0:sample.Rgba.Length)+"\t"+Hash(sample.Rgba)+"\t"+sample.Error;
    }
    static void Save(string directory,string label,int index,Snapshot native,Loaded loaded)
    {
        string name=Path.Combine(directory,label+".call"+index);
        if(native.Descriptor!=null)File.WriteAllBytes(name+".descriptor.bin",native.Descriptor);
        if(native.Storage!=null)File.WriteAllBytes(name+".locked.bin",native.Storage);
        if(loaded.Rgba!=null)File.WriteAllBytes(name+".rgba",loaded.Rgba);
    }
    static void Case(StreamWriter log,string output,int process,int context,int dib,int depth,int width,int height,bool top,int pattern,int profile,bool control)
    {
        int compression=control?0:3;uint[] masks=Masks(depth,profile);
        if(dib<56)masks[3]=0;
        string label=(control?"control":"bf")+"-h"+dib+"-d"+depth+"-"+width+"x"+height+"-top"+top+"-p"+pattern+"-m"+profile+"-ctx"+context,path=Path.Combine(output,label+".bmp");
        byte[] bytes=Fixture(width,height,depth,top,pattern,dib,masks,compression);File.WriteAllBytes(path,bytes);
        Seed(context);Snapshot first=Native(path);Loaded image=Load(path);
        Seed(context);Snapshot repeated=Native(path);Loaded again=Load(path);
        Save(output,label,1,first,image);Save(output,label,2,repeated,again);
        bool stable=image.Error==again.Error&&image.Width==again.Width&&image.Height==again.Height&&image.HasAlpha==again.HasAlpha&&Equal(image.Rgba,again.Rgba);
        bool descriptorStable=first.Status==repeated.Status&&first.Error==repeated.Error&&Equal(first.Descriptor,repeated.Descriptor);
        bool storageStable=first.Status==repeated.Status&&first.Error==repeated.Error&&first.Width==repeated.Width&&first.Height==repeated.Height&&first.Pitch==repeated.Pitch&&first.Format==repeated.Format&&Equal(first.Storage,repeated.Storage);
        Check(Hash(bytes)==Hash(File.ReadAllBytes(path)),"Source changed "+label);
        if(control){controls++;Check(image.Error==""&&again.Error==""&&stable&&image.Width==width&&image.Height==height,"Known BI_RGB positive control "+label);Check(first.Status>=0&&repeated.Status>=0&&first.Error==""&&repeated.Error==""&&first.Descriptor!=null&&repeated.Descriptor!=null&&first.Storage!=null&&repeated.Storage!=null&&first.Width==width&&first.Height==height&&repeated.Width==width&&repeated.Height==height,"Known native descriptor/storage control "+label);}
        else{fixtures++;if(image.Error=="")decoded++;else rejected++;if(!stable)unstable++;}
        log.WriteLine(process+"\t"+(control?"control":"bitfields")+"\t"+label+"\t"+context+"\t"+dib+"\t"+depth+"\t"+width+"\t"+height+"\t"+top+"\t"+pattern+"\t"+profile+"\t"+masks[0].ToString("X8")+"\t"+masks[1].ToString("X8")+"\t"+masks[2].ToString("X8")+"\t"+masks[3].ToString("X8")+"\t"+compression+"\t"+Hash(bytes)+"\t"+SnapshotColumns(first)+"\t"+LoadedColumns(image)+"\t"+SnapshotColumns(repeated)+"\t"+LoadedColumns(again)+"\t"+stable+"\t"+descriptorStable+"\t"+storageStable);
    }
    static void BindNative()
    {
        var module=original.ManifestModule;create=(MethodInfo)module.ResolveMethod(0x060000e8);describe=(MethodInfo)module.ResolveMethod(0x060000d5);lockRect=(MethodInfo)module.ResolveMethod(0x060000c5);unlockRect=(MethodInfo)module.ResolveMethod(0x060000c6);release=(MethodInfo)module.ResolveMethod(0x060000c4);
        Check(create.Name=="D3DXCreateTextureFromFileA"&&create.GetParameters().Length==3&&describe.Name=="XGGetTextureDesc"&&describe.GetParameters().Length==3,"Original create/descriptor contracts");
        Check(lockRect.Name=="D3DTexture.LockRect"&&lockRect.GetParameters().Length==5&&unlockRect.Name=="D3DTexture.UnlockRect"&&release.Name=="D3DResource.Release","Original lock/release contracts");
        device=module.ResolveField(0x04000039).GetValue(null);load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic);
    }
    static int Main(string[] args)
    {
        try
        {
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),output=Path.GetFullPath(args[1]),directory=Path.GetDirectoryName(input);int process=int.Parse(args[2]);Check(process==0||process==1,"Two explicit independent processes");Directory.CreateDirectory(output);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string path=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(path)?Assembly.LoadFrom(path):null;};
            original=Assembly.LoadFrom(input);typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{original,input});BindNative();
            warmup=Path.Combine(output,"context-known-birgb.bmp");File.WriteAllBytes(warmup,TextureBmpOracle.Fixture(16,8,32,false,process==0?0:2,0,0));
            string native="Status\tWidth\tHeight\tFormat\tPitch\tStorageBytes\tDescriptorSha256\tStorageSha256\tError",loaded="Width\tHeight\tHasAlpha\tBytes\tSha256\tError";
            string[] n=native.Split('\t'),l=loaded.Split('\t');for(int i=0;i<n.Length;i++)n[i]="native1"+n[i];for(int i=0;i<l.Length;i++)l[i]="loaded1"+l[i];
            string header="process\tkind\tlabel\tcontext\tdib\tdepth\tsourceWidth\tsourceHeight\ttopDown\tpattern\tmaskProfile\tmaskR\tmaskG\tmaskB\tmaskA\tcompression\tinputSha256\t"+string.Join("\t",n)+"\t"+string.Join("\t",l)+"\t";
            for(int i=0;i<n.Length;i++)n[i]=n[i].Replace("native1","native2");for(int i=0;i<l.Length;i++)l[i]=l[i].Replace("loaded1","loaded2");header+=string.Join("\t",n)+"\t"+string.Join("\t",l)+"\toriginalStable\tdescriptorStable\tstorageStable";
            using(var log=new StreamWriter(Path.Combine(output,"transcript.tsv"))){log.AutoFlush=true;log.WriteLine(header);
                foreach(int depth in new[]{16,24,32})foreach(bool top in new[]{false,true})Case(log,output,process,0,40,depth,16,8,top,0,0,true);
                int[] dibs=process==0?new[]{40,52,56,108,124}:new[]{124,108,56,52,40};
                int[] profiles=process==0?new[]{0,1,2,3,4,5,6,7}:new[]{7,6,5,4,3,2,1,0};
                foreach(int context in new[]{0,1})foreach(int dib in dibs)foreach(int depth in new[]{16,24,32})foreach(int[] size in new[]{new[]{1,1},new[]{16,8},new[]{7,5}})foreach(bool top in new[]{false,true})foreach(int pattern in new[]{0,1,2})foreach(int profile in profiles)Case(log,output,process,context,dib,depth,size[0],size[1],top,pattern,profile,false);
            }
            Check(fixtures==4320&&controls==6&&decoded+rejected==fixtures,"Complete original-only bitfields matrix");Console.WriteLine("PROBE Texture BMP bitfields process="+process+" fixtures="+fixtures+" controls="+controls+" decoded="+decoded+" rejected="+rejected+" unstableOriginals="+unstable+" fixtureLoadCalls="+((fixtures+controls)*2)+" contextLoadCalls=4320 initializationControlLoadCalls=1 nativeSnapshots="+((fixtures+controls)*2)+" checks="+checks+"; native descriptor/locked storage and full repeated original RGBA/errors; two contexts, reversed process order, not recovered/plugin/SDK acceptance");return 0;
        }
        catch(Exception e){Console.Error.WriteLine(e);return 1;}
        finally{var field=typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic);var owner=field.GetValue(null) as IDisposable;if(owner!=null)owner.Dispose();}
    }
}
