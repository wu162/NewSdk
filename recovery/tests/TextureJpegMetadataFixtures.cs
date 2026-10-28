using System;
using System.IO;
using System.Text;

internal static class TextureJpegMetadataFixtures
{
    internal static readonly string[] Profiles={"exif1","exif3","exif6","exif8","icc-sRGB-v2-micro","icc-sRGB-v4","icc-AdobeCompat-v2","icc-sGrey-v4","xmp","comment"};
    static byte[] Join(params byte[][] rows){int length=0;foreach(byte[] r in rows)length+=r.Length;byte[] b=new byte[length];int at=0;foreach(byte[] r in rows){Buffer.BlockCopy(r,0,b,at,r.Length);at+=r.Length;}return b;}
    static byte[] Segment(int marker,byte[] payload){if(payload.Length>=65534)throw new Exception("JPEG segment capacity");byte[] b=new byte[payload.Length+4];b[0]=255;b[1]=(byte)marker;int n=payload.Length+2;b[2]=(byte)(n>>8);b[3]=(byte)n;Buffer.BlockCopy(payload,0,b,4,payload.Length);return b;}
    static byte[] Insert(byte[] jpg,byte[] segment){if(jpg.Length<2||jpg[0]!=255||jpg[1]!=216)throw new Exception("JPEG fixture SOI");byte[] b=new byte[jpg.Length+segment.Length];Buffer.BlockCopy(jpg,0,b,0,2);Buffer.BlockCopy(segment,0,b,2,segment.Length);Buffer.BlockCopy(jpg,2,b,2+segment.Length,jpg.Length-2);return b;}
    internal static byte[] Create(string directory,byte[] source,string profile){
        if(Array.IndexOf(Profiles,profile)<0)throw new Exception("Unknown JPEG metadata fixture");
        if(profile.StartsWith("exif")){int orientation=int.Parse(profile.Substring(4));return Insert(source,Segment(0xe1,new byte[]{69,120,105,102,0,0,73,73,42,0,8,0,0,0,1,0,18,1,3,0,1,0,0,0,(byte)orientation,0,0,0,0,0,0,0}));}
        if(profile.StartsWith("icc-")){byte[] icc=Convert.FromBase64String(File.ReadAllText(Path.Combine(directory,profile.Substring(4)+".icc.base64")));return Insert(source,Segment(0xe2,Join(Encoding.ASCII.GetBytes("ICC_PROFILE\0"),new byte[]{1,1},icc)));}
        if(profile=="xmp")return Insert(source,Segment(0xe1,Encoding.UTF8.GetBytes("http://ns.adobe.com/xap/1.0/\0<x:xmpmeta xmlns:x='adobe:ns:meta/'/>")));
        return Insert(source,Segment(0xfe,Encoding.UTF8.GetBytes("TextureCompiler metadata evidence")));
    }
}
