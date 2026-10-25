using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Recovered.Compiler.Texture;
internal static unsafe class TexturePngRgb16Probe
{
    static object Arg(MethodInfo m,int i,void* p){return Pointer.Box(p,m.GetParameters()[i].ParameterType);}
    static void Snapshot(Assembly a,string path)
    {
        var module=a.ManifestModule;
        var create=(MethodInfo)module.ResolveMethod(0x060000e8);var desc=(MethodInfo)module.ResolveMethod(0x060000d5);
        var locked=(MethodInfo)module.ResolveMethod(0x060000c5);var unlock=(MethodInfo)module.ResolveMethod(0x060000c6);var release=(MethodInfo)module.ResolveMethod(0x060000c4);
        object device=module.ResolveField(0x04000039).GetValue(null);IntPtr texture=IntPtr.Zero;bool isLocked=false;
        byte[] name=Encoding.Default.GetBytes(path+"\0");
        try{
            fixed(byte* p=name){object hr=create.Invoke(null,new[]{device,Arg(create,1,p),Arg(create,2,&texture)});Console.WriteLine("CREATE hr="+hr);}
            if(texture==IntPtr.Zero)throw new Exception("Missing original native texture");
            byte* d=stackalloc byte[128];for(int i=0;i<128;i++)d[i]=0;
            desc.Invoke(null,new[]{Arg(desc,0,texture.ToPointer()),(object)0u,Arg(desc,2,d)});
            Console.WriteLine("DESC "+BitConverter.ToString(new ReadOnlySpanCompat(d,64).Bytes));
            byte* r=stackalloc byte[16];for(int i=0;i<16;i++)r[i]=0;
            object status=locked.Invoke(null,new[]{Arg(locked,0,texture.ToPointer()),(object)0u,Arg(locked,2,r),Arg(locked,3,null),(object)0u});
            Console.WriteLine("LOCK hr="+status);if(Convert.ToInt32(status)!=0)throw new Exception("Native lock failed");isLocked=true;
            int pitch=*(int*)r;byte* pixels=*(byte**)(r+4);Console.WriteLine("PITCH="+pitch);
            if(pitch<=0||pitch>65536||pixels==null)throw new Exception("Unexpected native row layout");
            for(int y=0;y<Math.Min(2,*(int*)(d+8));y++)Console.WriteLine("ROW "+y+" "+BitConverter.ToString(new ReadOnlySpanCompat(pixels+y*pitch,Math.Min(pitch,96)).Bytes));
        }finally{
            if(isLocked)unlock.Invoke(null,new[]{Arg(unlock,0,texture.ToPointer()),(object)0u});
            if(texture!=IntPtr.Zero)release.Invoke(null,new[]{Arg(release,0,texture.ToPointer())});
        }
    }
    struct ReadOnlySpanCompat
    {
        public byte[] Bytes;
        public ReadOnlySpanCompat(byte* p,int n){Bytes=new byte[n];Marshal.Copy((IntPtr)p,Bytes,0,n);}
    }
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input),work=Path.GetFullPath(args[1]);Directory.CreateDirectory(work);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            var original=Assembly.LoadFrom(input);typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{original,input});
            owner=(IDisposable)typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
            foreach(int color in new[]{2,6})foreach(int depth in new[]{8,16})foreach(int width in new[]{2,4,8}){
                string path=Path.Combine(work,"c"+color+"d"+depth+"w"+width+".png");File.WriteAllBytes(path,TexturePngOracle.Fixture(width,2,depth,color,0,0,0,0));
                Console.WriteLine("IMAGE "+Path.GetFileName(path));Snapshot(original,path);
                var image=(TexturePacker.Image)typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{path});
                Console.WriteLine("RGBA "+String.Join(",",Array.ConvertAll(image.Pixels,p=>p.ToString("X8"))));
            }
            Console.WriteLine("PROBE Texture PNG native RGB16 layout captured");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
