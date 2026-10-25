using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;
internal static class TexturePngRangeOracle
{
    static void Big(BinaryWriter w,uint n){w.Write(new[]{(byte)(n>>24),(byte)(n>>16),(byte)(n>>8),(byte)n});}
    static void Chunk(BinaryWriter w,string tag,byte[] data)
    {
        byte[] kind=System.Text.Encoding.ASCII.GetBytes(tag);uint c=0xffffffff;Big(w,(uint)data.Length);w.Write(kind);w.Write(data);
        foreach(var bytes in new[]{kind,data})foreach(byte v in bytes){c^=v;for(int i=0;i<8;i++)c=(c>>1)^((c&1)==0?0u:0xedb88320u);}Big(w,~c);
    }
    static byte[] Fixture(int color,int depth,uint gamma)
    {
        int width=color==2&&depth==16?512:256;int channels=color==0?1:color==2?3:color==4?2:4,height=depth==16?256:1;byte[] raw;
        using(var s=new MemoryStream())using(var w=new BinaryWriter(s)){
            for(int y=0;y<height;y++){w.Write((byte)0);for(int x=0;x<width;x++)for(int c=0;c<channels;c++){
                int v=(y*256+x+c*13107)&((1<<depth)-1);if(depth==16)w.Write((byte)(v>>8));w.Write((byte)v);
            }}raw=s.ToArray();
        }
        byte[] deflated;using(var s=new MemoryStream()){using(var z=new DeflateStream(s,CompressionMode.Compress,true))z.Write(raw,0,raw.Length);deflated=s.ToArray();}
        using(var s=new MemoryStream())using(var w=new BinaryWriter(s)){
            w.Write(new byte[]{137,80,78,71,13,10,26,10});byte[] h={0,0,(byte)(width>>8),(byte)width,0,0,(byte)(height>>8),(byte)height,(byte)depth,(byte)color,0,0,0};Chunk(w,"IHDR",h);
            if(gamma!=0)Chunk(w,"gAMA",new[]{(byte)(gamma>>24),(byte)(gamma>>16),(byte)(gamma>>8),(byte)gamma});
            using(var z=new MemoryStream())using(var zw=new BinaryWriter(z)){
                zw.Write((byte)0x78);zw.Write((byte)0x01);zw.Write(deflated);uint a=1,b=0;foreach(byte v in raw){a=(a+v)%65521;b=(b+a)%65521;}Big(zw,b*65536+a);Chunk(w,"IDAT",z.ToArray());
            }
            Chunk(w,"IEND",new byte[0]);return s.ToArray();
        }
    }
    static string Hash(uint[] p){byte[] b=new byte[p.Length*4];Buffer.BlockCopy(p,0,b,0,b.Length);using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","");}
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            string input=Path.GetFullPath(args[0]),dir=Path.GetDirectoryName(input),work=Path.GetFullPath(args[1]);Directory.CreateDirectory(work);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            var original=Assembly.LoadFrom(input);typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{original,input});
            owner=(IDisposable)typeof(TexturePngOracle).GetField("owner",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
            var load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.Static|BindingFlags.NonPublic);
            int cases=0,differences=0;long pixels=0;
            foreach(int color in new[]{0,2,4,6})foreach(int depth in color==4?new[]{8}:new[]{8,16})
            foreach(uint gamma in new uint[]{0,1000,10000,20000,43181,43182,45455,47727,47728,50000,70000,100000,180000,220000}){
                string label="c"+color+"d"+depth+"gamma"+gamma,path=Path.Combine(work,"range.png");byte[] png=Fixture(color,depth,gamma);File.WriteAllBytes(path,png);
                var a=(TexturePacker.Image)load.Invoke(null,new object[]{path});var b=TexturePngDecoder.Decode(png);int bad=0,first=-1;
                if(a.Width!=b.Width||a.Height!=b.Height||a.HasAlpha!=b.HasAlpha)throw new Exception("Range metadata mismatch "+label);
                for(int i=0;i<a.Pixels.Length;i++)if(a.Pixels[i]!=b.Pixels[i]){bad++;if(first<0)first=i;}
                Console.WriteLine("RANGE "+label+" bad="+bad+" pixels="+a.Pixels.Length+" original="+Hash(a.Pixels)+" recovered="+Hash(b.Pixels)+(first<0?"":" first="+first+" expected="+a.Pixels[first].ToString("X8")+" actual="+b.Pixels[first].ToString("X8")));
                if(bad!=0)differences++;cases++;pixels+=a.Pixels.Length;
            }
            bool probe=args.Length>2&&args[2]=="--probe";
            Console.WriteLine((probe?"PROBE":"PASS")+" Texture PNG full-sample ranges cases="+cases+" comparedPixels="+pixels+" differentCases="+differences);return !probe&&differences!=0?1:0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
