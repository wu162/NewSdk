using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;
// Compares copied public implementations against immutable original output, no original DLL loaded.
internal static class TexturePublicCodecsReplay
{
    [DllImport("TexturePublicCodecs.dll",CallingConvention=CallingConvention.Cdecl)]
    static extern int PublicDxtEncode(int backend,int variant,int format,[In] byte[] rgba,[Out] byte[] output);
    [DllImport("TexturePublicCodecs.dll",CallingConvention=CallingConvention.Cdecl)]
    static extern int PublicImageDecode([In] byte[] input,int bytes,[Out] byte[] output,int capacity,out int width,out int height,out int channels);
    static string Hex(byte[] b){return BitConverter.ToString(b).Replace("-","").ToLowerInvariant();}
    static byte[] Pixels(int fixture){byte[] p=new byte[64];for(int i=0;i<16;i++){p[i*4]=(byte)(i*37+17);p[i*4+1]=(byte)(i*71+33);p[i*4+2]=(byte)(i*113+129);p[i*4+3]=(byte)(i*19+7);}
        if(fixture<256){for(int i=0;i<16;i++){p[i*4]=p[i*4+1]=p[i*4+2]=(byte)fixture;p[i*4+3]=(byte)(i*17);}}
        else if(fixture<512){for(int i=0;i<16;i++){p[i*4]=(byte)fixture;p[i*4+1]=(byte)(fixture*37);p[i*4+2]=(byte)(fixture*73);p[i*4+3]=(byte)(fixture+i*17);}}
        else if(fixture<768){for(int i=0;i<16;i++){p[i*4]=(byte)((i&1)==0?fixture:255-fixture);p[i*4+1]=(byte)((i&1)==0?17:229);p[i*4+2]=(byte)((i&1)==0?71:191);p[i*4+3]=(byte)((i&1)==0?127:128);}}
        else if(fixture<1024){for(int i=0;i<16;i++)p[i*4+3]=(byte)fixture;}
        else new Random(fixture*31+17).NextBytes(p);return p;
    }
    static bool Same(byte[] a,int offset,byte[] b,int length){if(a.Length<offset+length||b.Length<offset+length)return false;for(int i=offset;i<offset+length;i++)if(a[i]!=b[i])return false;return true;}
    static byte[] Unhex(string text){string s=text.Replace("-","");byte[] b=new byte[s.Length/2];for(int i=0;i<b.Length;i++)b[i]=Convert.ToByte(s.Substring(i*2,2),16);return b;}
    static void Isolated(){if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")))throw new Exception("Original present");foreach(var a in AppDomain.CurrentDomain.GetAssemblies())if(a.GetName().Name=="BinaryAssetBuilder.TextureCompiler")throw new Exception("Original loaded");}
    static void Dxt(string baseline,string legacy,string output,bool onlyAmd,bool patched){
        var rows=new List<string[]>();foreach(string line in File.ReadAllLines(baseline)){if(line.StartsWith("format\t"))continue;rows.Add(line.Split('\t'));}if(rows.Count!=10240)throw new Exception("Incomplete original block corpus");
        using(var result=new StreamWriter(Path.Combine(output,"dxt-summary.tsv")))using(var differences=new StreamWriter(Path.Combine(output,"dxt-first-differences.tsv")))using(var artifacts=new BinaryWriter(File.Create(Path.Combine(output,"dxt-blocks.bin")))){
            artifacts.Write(patched?430080:onlyAmd?122880:327680);
            result.WriteLine("backend\tvariant\tformat\tfixtures\tfullEqual\tcolourEqual\talphaEqual\teligibleForIntegration");differences.WriteLine("backend\tvariant\tformat\tfixture\toriginal\tcandidate");
            foreach(int backend in patched?new[]{0,2,4}:onlyAmd?new[]{3}:new[]{0,1,2})for(int variant=0;variant<(backend==0||backend>=3?12:backend==1?2:18);variant++){
                int[] equal=new int[2],colour=new int[2],alpha=new int[2],first=new int[2];string label=backend==0?"libsquish":backend==1?"stb_dxt+squish-dxt3-alpha":backend==2?"nvtt-icbc+squish-alpha":backend==3?"amd-ati-legacy":"amd-ati-patched";
                foreach(string[] row in rows){int f=row[0]=="D3DFMT_DXT3"?2:3,i=f-2;byte[] pixels=Pixels(int.Parse(row[1])),before=(byte[])pixels.Clone(),expected=Unhex(row[2]),actual=new byte[16];if(PublicDxtEncode(backend,variant,f,pixels,actual)!=16)throw new Exception("Missing public API");if(!Same(pixels,0,before,64))throw new Exception("Source mutation");artifacts.Write(backend);artifacts.Write(variant);artifacts.Write(f);artifacts.Write(int.Parse(row[1]));artifacts.Write(pixels);artifacts.Write(expected);artifacts.Write(actual);bool c=Same(expected,8,actual,8),a=Same(expected,0,actual,8);if(c)colour[i]++;if(a)alpha[i]++;if(c&&a)equal[i]++;else if(first[i]++<3)differences.WriteLine(label+"\t"+variant+"\t"+row[0]+"\t"+row[1]+"\t"+Hex(expected)+"\t"+Hex(actual));}
                for(int i=0;i<2;i++){result.WriteLine(label+"\t"+variant+"\tDXT"+(i==0?3:5)+"\t5120\t"+equal[i]+"\t"+colour[i]+"\t"+alpha[i]+"\t"+(equal[i]==5120));Console.WriteLine("COMPARE "+label+" variant="+variant+" DXT"+(i==0?3:5)+" full="+equal[i]+"/5120 color="+colour[i]+" alpha="+alpha[i]);}
            }
        }
        if(!onlyAmd&&!patched)using(var result=new StreamWriter(Path.Combine(output,"dxt1-snapshots.tsv"))){result.WriteLine("backend\tvariant\tformat\tfixture\texactSnapshot\tacceptance");foreach(string format in new[]{"D3DFMT_DXT1","D3DFMT_DXT1A"})for(int fixture=0;fixture<8;fixture++){
            string name="0-"+format+"-"+fixture;byte[] pixels=File.ReadAllBytes(Path.Combine(legacy,"rgba",name+".rgba")),dds=File.ReadAllBytes(Path.Combine(legacy,"dds",name+".dds"));byte[] expected=new byte[8];Buffer.BlockCopy(dds,128,expected,0,8);
            for(int backend=0;backend<3;backend++){byte[] actual=new byte[16];int size=PublicDxtEncode(backend,0,format.EndsWith("1A")?1:0,pixels,actual);result.WriteLine(backend+"\t0\t"+format+"\t"+fixture+"\t"+(size==8&&Same(expected,0,actual,8))+"\tprobe-only-default-state-not-defined");}
        }}
    }
    static void Jpeg(string directory,string output){int fixtures=0,equal=0,errors=0;using(var result=new StreamWriter(Path.Combine(output,"stb-jpeg.tsv"))){result.WriteLine("label\toriginalSuccess\tpublicSuccess\tfullRgbaEqual");foreach(string line in File.ReadAllLines(Path.Combine(directory,"transcript.tsv"))){if(line.StartsWith("label\t"))continue;var row=line.Split('\t');byte[] source=File.ReadAllBytes(Path.Combine(directory,row[0]+".jpg"));int w,h,c;int ok=PublicImageDecode(source,source.Length,null,0,out w,out h,out c);byte[] rgba=ok==0?null:new byte[checked(w*h*4)];if(ok!=0)ok=PublicImageDecode(source,source.Length,rgba,rgba.Length,out w,out h,out c);bool same=false;
        if(ok!=0&&row[9]==""){uint[] pixels=new uint[w*h];Buffer.BlockCopy(rgba,0,pixels,0,rgba.Length);var image=new TexturePacker.Image(w,h,pixels,false);if((w&(w-1))!=0||(h&(h-1))!=0)image=c==1?TextureNpotStorageResampler.Resize(image,TextureNpotStorageResampler.Format.L8):TextureNpotRgba8Resampler.Resize(image);rgba=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,rgba,0,rgba.Length);byte[] expected=File.ReadAllBytes(Path.Combine(directory,row[0]+".loaded.rgba"));same=image.Width==int.Parse(row[4])&&image.Height==int.Parse(row[5])&&rgba.Length==expected.Length&&Same(expected,0,rgba,rgba.Length);}
        if(same)equal++;if(ok==0&&row[9]!="")errors++;result.WriteLine(row[0]+"\t"+(row[9]=="")+"\t"+(ok!=0)+"\t"+same);fixtures++;
    }}if(fixtures!=1284)throw new Exception("Incomplete JPEG corpus");Console.WriteLine("COMPARE stb_image JPEG complete RGBA="+equal+"/1280 original errors matched="+errors+"/4");}
    static int Main(string[] args){try{Isolated();Directory.CreateDirectory(args[3]);bool amd=args.Length>4&&args[4]=="amd",patched=args.Length>4&&args[4]=="patched";Dxt(args[0],args[1],args[3],amd,patched);if(!amd&&!patched)Jpeg(args[2],args[3]);Isolated();Console.WriteLine("PASS Public codec comparison completed; mismatches retained, no production acceptance implied");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
