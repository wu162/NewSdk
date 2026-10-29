using System;
using System.IO;

internal static class TextureBmpHeaderFixtures
{
    internal static readonly int[] WindowsDibs={40,52,56,108,124};
    internal const int ProbeCount=2616;
    static void Word(byte[] b,int at,int value){b[at]=(byte)value;b[at+1]=(byte)(value>>8);}
    static void Dword(byte[] b,int at,uint value){for(int i=0;i<4;i++)b[at+i]=(byte)(value>>(i*8));}
    internal static byte[] Create(string icc,int width,int height,int depth,bool top,int pattern,int dib,int compression,string metadata,int gap){
        byte[] basic=TextureBmpOracle.Fixture(width,height,depth,top,pattern,0,0);int pixels=basic.Length-54;
        int external=dib==40&&compression!=0?(compression==6?16:12):dib==52&&compression==6?4:0;
        int start=14+dib+external+gap;byte[] profile=null;
        if(metadata.StartsWith("embedded-")){string name=metadata=="embedded-srgb2"?"sRGB-v2-micro":metadata=="embedded-srgb4"?"sRGB-v4":"sGrey-v4";profile=Convert.FromBase64String(File.ReadAllText(Path.Combine(icc,name+".icc.base64")));}
        byte[] b=new byte[start+pixels+(profile==null?0:profile.Length)];Buffer.BlockCopy(basic,0,b,0,14);
        Dword(b,2,(uint)b.Length);Dword(b,10,(uint)start);
        if(dib==12){Dword(b,14,12);Word(b,18,width);Word(b,20,height);Word(b,22,1);Word(b,24,depth);}
        else{
            Buffer.BlockCopy(basic,14,b,14,40);Dword(b,14,(uint)dib);Dword(b,30,(uint)compression);
            if(compression!=0){uint[] masks=depth==16?(compression==6?new uint[]{0x7c00,0x3e0,0x1f,0x8000}:new uint[]{0xf800,0x7e0,0x1f,0}):new uint[]{0xff0000,0xff00,0xff,depth==32?0xff000000u:0u};
                for(int channel=0;channel<(compression==6?4:3);channel++){int at=dib==40?54+channel*4:channel==3&&dib==52?66:54+channel*4;Dword(b,at,masks[channel]);}
            }
            if(dib>=108&&metadata!="plain"){
                uint space=metadata=="srgb"?0x73524742u:profile!=null?0x4d424544u:0u;Dword(b,70,space);
                if(space==0){double[] xyz={0.4124564,0.2126729,0.0193339,0.3575761,0.7151522,0.1191920,0.1804375,0.0721750,0.9503041};for(int i=0;i<xyz.Length;i++)Dword(b,74+i*4,(uint)(xyz[i]*1073741824.0+0.5));uint gamma=(uint)((metadata=="gamma1"?1.0:metadata=="calibrated"?1.8:2.2)*65536.0+0.5);for(int i=0;i<3;i++)Dword(b,110+i*4,gamma);}
            }
            if(dib==124){Dword(b,122,4);if(profile!=null){Dword(b,126,(uint)(start+pixels-14));Dword(b,130,(uint)profile.Length);Buffer.BlockCopy(profile,0,b,start+pixels,profile.Length);}}
        }
        for(int i=0;i<gap;i++)b[14+dib+external+i]=(byte)(i*17+111);
        Buffer.BlockCopy(basic,54,b,start,pixels);return b;
    }
}
