using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

// PNG/APNG chunk construction follows the PNG specification. ICC fixture bytes
// are unmodified Compact-ICC-Profiles at the commit recorded in PROVENANCE.json.
internal static class TexturePngMetadataFixtures
{
    internal static readonly string[] Profiles = {
        "none", "icc-rgb2", "icc-rgb4", "icc-adobe", "icc-gray2", "icc-gray4",
        "icc-gamma", "icc-srgb", "icc-srgb-gamma", "apng-first", "apng-fallback",
        "apng-subframes", "apng-three", "apng-icc", "apng-gamma", "apng-icc-gamma",
        "icc-empty", "icc-short", "icc-method", "icc-zlib", "icc-duplicate", "icc-after",
        "apng-sequence", "apng-empty-control", "apng-outside", "apng-empty-data"
    };
    internal static readonly int[][] Formats = {
        new[]{0,1},new[]{0,2},new[]{0,4},new[]{0,8},new[]{0,16},
        new[]{2,8},new[]{2,16},new[]{3,1},new[]{3,2},new[]{3,4},new[]{3,8},
        new[]{4,8},new[]{4,16},new[]{6,8},new[]{6,16}
    };
    internal sealed class Part { internal string Name; internal byte[] Data; internal Part(string n, byte[] d){Name=n;Data=d;} }
    internal static uint Big(byte[] b,int at){return (uint)b[at]<<24|(uint)b[at+1]<<16|(uint)b[at+2]<<8|b[at+3];}
    internal static void Big(BinaryWriter w,uint v){w.Write((byte)(v>>24));w.Write((byte)(v>>16));w.Write((byte)(v>>8));w.Write((byte)v);}
    internal static List<Part> Parts(byte[] png){
        var result=new List<Part>();for(int at=8;at<png.Length;){int n=checked((int)Big(png,at));byte[] b=new byte[n];Buffer.BlockCopy(png,at+8,b,0,n);result.Add(new Part(Encoding.ASCII.GetString(png,at+4,4),b));at+=n+12;}return result;
    }
    internal static byte[] Encode(List<Part> parts){
        using(var s=new MemoryStream())using(var w=new BinaryWriter(s)){
            w.Write(new byte[]{137,80,78,71,13,10,26,10});foreach(var p in parts){byte[] tag=Encoding.ASCII.GetBytes(p.Name);Big(w,(uint)p.Data.Length);w.Write(tag);w.Write(p.Data);uint crc=0xffffffff;
                foreach(byte v in Join(tag,p.Data)){crc^=v;for(int bit=0;bit<8;bit++)crc=(crc>>1)^((crc&1)!=0?0xedb88320u:0u);}Big(w,~crc);
            }return s.ToArray();
        }
    }
    internal static byte[] Join(params byte[][] rows){int n=0;foreach(byte[] r in rows)n+=r.Length;byte[] b=new byte[n];int at=0;foreach(byte[] r in rows){Buffer.BlockCopy(r,0,b,at,r.Length);at+=r.Length;}return b;}
    static byte[] Words(params uint[] values){using(var s=new MemoryStream())using(var w=new BinaryWriter(s)){foreach(uint v in values)Big(w,v);return s.ToArray();}}
    static byte[] Zlib(byte[] b){
        byte[] packed;using(var s=new MemoryStream()){using(var z=new DeflateStream(s,CompressionMode.Compress,true))z.Write(b,0,b.Length);packed=s.ToArray();}
        uint a=1,c=0;foreach(byte v in b){a=(a+v)%65521;c=(c+a)%65521;}return Join(new byte[]{0x78,0x9c},packed,Words(c<<16|a));
    }
    static Part Icc(string directory,string name){return new Part("iCCP",Join(Encoding.ASCII.GetBytes("Recovery profile\0\0"),Zlib(Convert.FromBase64String(File.ReadAllText(Path.Combine(directory,name+".icc.base64"))))));}
    static Part Frame(uint sequence,int width,int height,int x,int y,byte dispose,byte blend){return new Part("fcTL",Join(Words(sequence,(uint)width,(uint)height,(uint)x,(uint)y),new byte[]{0,1,0,10,dispose,blend}));}
    static byte[] Data(byte[] png){var rows=new List<byte[]>();foreach(var p in Parts(png))if(p.Name=="IDAT")rows.Add(p.Data);return Join(rows.ToArray());}
    internal static byte[] Create(string directory,int width,int height,int color,int depth,int interlace,int pattern,string profile){
        int meta=profile.IndexOf("gamma",StringComparison.Ordinal)>=0?5:0;
        var parts=Parts(TexturePngOracle.Fixture(width,height,depth,color,interlace,5,meta,pattern));
        int first=parts.FindIndex(p=>p.Name=="IDAT");
        var before=new List<Part>();var after=new List<Part>();
        bool icc=profile.StartsWith("icc-")||profile=="apng-icc"||profile=="apng-icc-gamma";
        Part cp=null;
        if(icc){string name=profile=="icc-rgb4"?"sRGB-v4":profile=="icc-gray2"?"sGrey-v2-micro":profile=="icc-gray4"?"sGrey-v4":profile=="icc-adobe"?"AdobeCompat-v2":"sRGB-v2-micro";cp=Icc(directory,name);
            if(profile=="icc-empty")cp.Data=new byte[0];
            if(profile=="icc-short")cp.Data=Join(Encoding.ASCII.GetBytes("short\0\0"),Zlib(new byte[]{1,2,3}));
            if(profile=="icc-method")cp.Data[17]=1;
            if(profile=="icc-zlib")cp.Data[18]=0;
            if(profile=="icc-after")after.Add(cp);else before.Add(cp);
            if(profile=="icc-duplicate")before.Add(Icc(directory,"sRGB-v4"));
            if(profile.IndexOf("srgb",StringComparison.Ordinal)>=0)before.Add(new Part("sRGB",new byte[]{0}));
        }
        if(profile.StartsWith("apng-")){
            bool fallback=profile=="apng-fallback",three=profile=="apng-three";
            before.Add(new Part("acTL",Words(three?3u:fallback?1u:2u,0)));
            if(!fallback)before.Add(profile=="apng-empty-control"?new Part("fcTL",new byte[0]):Frame(0,width,height,0,0,0,0));
            int fw=profile=="apng-subframes"?Math.Max(1,width/2):width,fh=profile=="apng-subframes"?Math.Max(1,height/2):height;
            int x=profile=="apng-subframes"?width-fw:profile=="apng-outside"?width:0,y=profile=="apng-subframes"?height-fh:0;
            uint sequence=fallback?0u:profile=="apng-sequence"?77u:1u;
            after.Add(Frame(sequence,fw,fh,x,y,1,1));
            byte[] frameData=Data(TexturePngOracle.Fixture(fw,fh,depth,color,interlace,5,0,(pattern+2)%4));
            after.Add(new Part("fdAT",profile=="apng-empty-data"?Words(sequence+1):Join(Words(sequence+1),frameData)));
            if(three){after.Add(Frame(3,width,height,0,0,2,0));after.Add(new Part("fdAT",Join(Words(4),Data(TexturePngOracle.Fixture(width,height,depth,color,interlace,5,0,(pattern+1)%4)))));}
        }
        parts.InsertRange(first,before);parts.InsertRange(parts.Count-1,after);return Encode(parts);
    }
}
