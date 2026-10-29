using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Test-only TGA 2.0 construction: 18-byte image header, 495-byte extension,
// 10-byte developer entries, absolute auxiliary-table offsets, 26-byte footer.
// Fields follow the TRUEVISION TGA 2.0 specification; no encoder backend change.
internal static class TextureTgaMetadataFixtures
{
    internal static readonly string[] Formats={"rgb24","rgba32","rgba32-noattr","rgb16","rgb16-noattr","gray8","index15","index16","index24","index32"};
    internal static readonly string[] Profiles={"none","footer","text","attr0","attr1","attr2","attr3","attr4","gamma1","gammahalf","gamma22","gamma-zero","gamma-undefined","aspect","key","developer","scanline","correction-identity","correction-invert","stamp","all","developer-first","tables-first","chained","origin","origin-all"};
    internal const int ProbeCount=12480;
    static void Word(byte[] b,int at,int value){b[at]=(byte)value;b[at+1]=(byte)(value>>8);}
    static void Dword(byte[] b,int at,uint value){for(int i=0;i<4;i++)b[at+i]=(byte)(value>>(i*8));}
    static void Text(byte[] b,int at,string value){byte[] text=Encoding.ASCII.GetBytes(value);Buffer.BlockCopy(text,0,b,at,text.Length);}
    static uint Pixel(int x,int y,int pattern){uint a=pattern==1?0u:pattern==2?127u:(uint)((x*61+y*37+17)&255);return pattern==2?0x7f2b7913u:(uint)((x*31+y*19+13)&255)|(uint)((x*17+y*67+79)&255)<<8|(uint)((x*47+y*23+181)&255)<<16|a<<24;}
    static byte[] Color(uint rgba,int depth){if(depth==15||depth==16){ushort v=(ushort)(((rgba&255)>>3)<<10|(((rgba>>8)&255)>>3)<<5|(((rgba>>16)&255)>>3)|(depth==16&&(rgba>>24)>=128?0x8000u:0));return new[]{(byte)v,(byte)(v>>8)};}return depth==32?new[]{(byte)(rgba>>16),(byte)(rgba>>8),(byte)rgba,(byte)(rgba>>24)}:new[]{(byte)(rgba>>16),(byte)(rgba>>8),(byte)rgba};}
    internal static byte[] Create(int width,int height,string format,bool rle,int direction,int pattern,string profile){
        if(Array.IndexOf(Formats,format)<0||Array.IndexOf(Profiles,profile)<0)throw new Exception("Unknown TGA fixture axis");
        bool indexed=format.StartsWith("index"),gray=format=="gray8";int paletteDepth=indexed?int.Parse(format.Substring(5)):0;
        int depth=indexed||gray?8:format.StartsWith("rgba32")?32:format.StartsWith("rgb16")?16:24;
        byte[] header=new byte[18];header[0]=7;header[1]=(byte)(indexed?1:0);header[2]=(byte)((indexed?1:gray?3:2)+(rle?8:0));
        if(indexed){Word(header,5,4);header[7]=(byte)paletteDepth;}Word(header,12,width);Word(header,14,height);header[16]=(byte)depth;header[17]=(byte)(direction|(depth==32&&!format.EndsWith("noattr")?8:depth==16&&!format.EndsWith("noattr")?1:0));
        if(profile=="origin"||profile=="origin-all"){Word(header,8,17);Word(header,10,29);}
        using(var stream=new MemoryStream())using(var writer=new BinaryWriter(stream)){
            writer.Write(header);writer.Write(new byte[]{82,101,99,111,118,101,114});
            if(indexed)for(int e=0;e<4;e++)writer.Write(Color(Pixel(e,0,pattern),paletteDepth));
            uint[] scanlines=new uint[height];
            for(int fy=0;fy<height;fy++){
                scanlines[fy]=(uint)stream.Position;
                for(int fx=0;fx<width;){int n=Math.Min(128,width-fx);bool repeat=rle&&pattern==2;if(rle)writer.Write((byte)((repeat?128:0)|(n-1)));
                    for(int i=0;i<(repeat?1:n);i++){int fileX=fx+i,x=(direction&16)!=0?width-1-fileX:fileX,y=(direction&32)!=0?fy:height-1-fy;uint pixel=Pixel(x,y,pattern);
                        if(indexed)writer.Write((byte)(pattern==2?0:(x+y)&3));else if(gray)writer.Write((byte)pixel);else writer.Write(Color(pixel,depth));
                    }fx+=n;
                }
            }
            if(profile=="none"||profile=="origin")return stream.ToArray();
            if(profile=="footer"){Footer(writer,0,0);return stream.ToArray();}
            bool all=profile=="all"||profile=="origin-all",dev=all||profile=="developer"||profile=="developer-first",tablesFirst=profile=="tables-first";
            uint developer=0,correction=0,scanline=0,stamp=0;
            if(profile=="developer-first")developer=Developer(writer);
            if(tablesFirst){correction=Correction(writer,false);scanline=Scanlines(writer,scanlines);stamp=Stamp(writer,depth,indexed,gray);}
            uint extension=(uint)stream.Position;byte[] ext=new byte[495];Word(ext,0,495);
            if(profile=="text"||all){Text(ext,2,"TextureCompiler recovery");Text(ext,43,"Valid nonzero metadata fixture");Word(ext,367,10);Word(ext,369,29);Word(ext,371,2026);Word(ext,373,8);Word(ext,375,15);Word(ext,377,30);Text(ext,379,"Metadata job");Word(ext,420,1);Word(ext,422,2);Word(ext,424,3);Text(ext,426,"Recovery fixture writer");Word(ext,467,100);ext[469]=(byte)'a';}
            if(profile.StartsWith("attr"))ext[494]=(byte)int.Parse(profile.Substring(4));else if(all)ext[494]=4;
            if(profile=="gamma1"){Word(ext,478,1);Word(ext,480,1);}else if(profile=="gammahalf"){Word(ext,478,1);Word(ext,480,2);}else if(profile=="gamma22"||all){Word(ext,478,22);Word(ext,480,10);}else if(profile=="gamma-zero")Word(ext,480,1);else if(profile=="gamma-undefined")Word(ext,478,1);
            if(profile=="aspect"||all){Word(ext,474,4);Word(ext,476,3);}if(profile=="key"||all){ext[470]=13;ext[471]=79;ext[472]=181;ext[473]=127;}
            writer.Write(ext);
            if(dev&&developer==0)developer=Developer(writer);
            if(profile=="correction-identity"||profile=="correction-invert"||all)correction=Correction(writer,profile=="correction-invert"||all);
            if(profile=="scanline"||all)scanline=Scanlines(writer,scanlines);
            if(profile=="stamp"||all)stamp=Stamp(writer,depth,indexed,gray);
            long end=stream.Position;Dword(ext,482,correction);Dword(ext,486,stamp);Dword(ext,490,scanline);stream.Position=extension;writer.Write(ext);stream.Position=end;Footer(writer,extension,developer);
            if(profile=="chained"){uint next=(uint)stream.Position;byte[] second=new byte[495];Word(second,0,495);Text(second,2,"Second valid extension");Word(second,478,1);Word(second,480,2);second[494]=3;writer.Write(second);Footer(writer,next,0);}
            return stream.ToArray();
        }
    }
    static void Footer(BinaryWriter w,uint extension,uint developer){w.Write(extension);w.Write(developer);w.Write(Encoding.ASCII.GetBytes("TRUEVISION-XFILE.\0"));}
    static uint Developer(BinaryWriter w){uint start=(uint)w.BaseStream.Position;w.Write((ushort)2);w.Write((ushort)65000);w.Write(start+22);w.Write(13u);w.Write((ushort)65001);w.Write(start+35);w.Write(9u);w.Write(Encoding.ASCII.GetBytes("metadata-one!"));w.Write(new byte[]{0,1,2,3,5,8,13,21,34});return start;}
    static uint Correction(BinaryWriter w,bool invert){uint at=(uint)w.BaseStream.Position;for(int i=0;i<256;i++){ushort v=(ushort)((invert?255-i:i)*257);w.Write((ushort)(i*257));w.Write(v);w.Write(v);w.Write(v);}return at;}
    static uint Scanlines(BinaryWriter w,uint[] rows){uint at=(uint)w.BaseStream.Position;foreach(uint row in rows)w.Write(row);return at;}
    static uint Stamp(BinaryWriter w,int depth,bool indexed,bool gray){uint at=(uint)w.BaseStream.Position;w.Write((byte)2);w.Write((byte)2);for(int i=0;i<4;i++){if(indexed||gray)w.Write((byte)i);else w.Write(Color(Pixel(i,0,0),depth));}return at;}
}
