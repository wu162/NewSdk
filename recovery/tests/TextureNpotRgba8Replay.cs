using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

// Independent replay of immutable original LoadImageA captures and exact native
// coefficient tables. Runs without the original TextureCompiler binary.
internal static class TextureNpotRgba8Replay
{
    static int fixtures,checks,taps;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original assembly loaded");}
    static int PngValue(int x,int y,int channel,int pattern){return pattern==1?0:pattern==2?255:pattern==3?((x+y+channel)&1)*255:(x*1271+y*395+channel*739+17)&255;}
    static TexturePacker.Image Source(string path,string profile,int pattern,int width,int height)
    {
        if(profile.StartsWith("tga-")){
            byte[] tga=File.ReadAllBytes(path);int depth=tga[16],stride=depth/8;Check(tga[2]==2&&tga[17]==(depth==32?40:32),"Immutable TGA fixture profile");
            uint[] raw=new uint[width*height];for(int i=0;i<raw.Length;i++){int p=18+i*stride;raw[i]=(uint)tga[p+2]|(uint)tga[p+1]<<8|(uint)tga[p]<<16|(uint)(depth==32?tga[p+3]:255)<<24;}
            return new TexturePacker.Image(width,height,raw,depth==32);
        }
        uint[] pixels=new uint[width*height];bool alpha=profile.StartsWith("png-c6");
        if(profile.StartsWith("png-")){
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)pixels[y*width+x]=(uint)PngValue(x,y,0,pattern)|(uint)PngValue(x,y,1,pattern)<<8|(uint)PngValue(x,y,2,pattern)<<16|(uint)(alpha?PngValue(x,y,3,pattern):255)<<24;
        }else{
            // Preserve the already accepted legacy BMP contract: D3DX ignores
            // bfOffBits, consumes gap bytes as pixels and reports BMP24 alpha.
            byte[] file=File.ReadAllBytes(path);int start=54,signedHeight=BitConverter.ToInt32(file,22),depth=BitConverter.ToUInt16(file,28),stride=((width*depth+31)/32)*4;
            alpha=depth==24;
            if(depth==32)for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(file[start+y*stride+x*4+3]!=0)alpha=true;
            for(int y=0;y<height;y++)for(int x=0;x<width;x++){int p=start+(signedHeight<0?y:height-1-y)*stride+x*(depth/8);pixels[y*width+x]=(uint)file[p+2]|(uint)file[p+1]<<8|(uint)file[p]<<16|(uint)(depth==32&&alpha?file[p+3]:255)<<24;}
        }
        return new TexturePacker.Image(width,height,pixels,alpha);
    }
    static void Weights(string path)
    {
        var expected=new List<string>();foreach(string line in File.ReadAllLines(path)){if(line.StartsWith("source\t")||line.Length==0)continue;string[] f=line.Split('\t');expected.Add(String.Join("\t",f,0,5));}
        MethodInfo method=typeof(TextureNpotRgba8Resampler).GetMethod("Contributions",BindingFlags.Static|BindingFlags.NonPublic);Check(method!=null,"Weight contract");
        var actual=new List<string>();var visited=new HashSet<string>();
        foreach(string line in expected){string[] f=line.Split('\t');string key=f[0]+"/"+f[1];if(!visited.Add(key))continue;int source=int.Parse(f[0]),target=int.Parse(f[1]);Array table=(Array)method.Invoke(null,new object[]{source,target});Check(table.Length==source,"Coefficient source count");
            for(int i=0;i<source;i++)foreach(object tap in (Array)table.GetValue(i)){Type t=tap.GetType();int destination=(int)t.GetField("Target").GetValue(tap);float weight=(float)t.GetField("Weight").GetValue(tap);uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(weight),0);actual.Add(source+"\t"+target+"\t"+i+"\t"+destination+"\t"+bits.ToString("X8"));taps++;}
        }
        Check(expected.Count==actual.Count,"Coefficient tap count original="+expected.Count+" recovered="+actual.Count);
        for(int i=0;i<expected.Count;i++)Check(expected[i]==actual[i],"Coefficient bits/order "+i+" original="+expected[i]+" recovered="+actual[i]);
    }
    static int Main(string[] args)
    {
        try{
            Check(IntPtr.Size==4,"x86 required");Isolated();Weights(args[1]);
            string directory=Path.GetFullPath(args[0]);var rows=File.ReadAllLines(Path.Combine(directory,"transcript.tsv"));Check(rows.Length==1013,"Original capture corpus length");
            foreach(string line in rows){if(line.StartsWith("label\t")||line.Length==0)continue;string[] f=line.Split('\t');string label=f[0],profile=f[1],extension=profile.StartsWith("tga-")?"tga":profile.StartsWith("png-")?"png":"bmp";int pattern=int.Parse(f[2]),width=int.Parse(f[3]),height=int.Parse(f[4]),targetWidth=int.Parse(f[5]),targetHeight=int.Parse(f[6]);Check(f.Length>=10&&(f.Length<11||f[10]==""),"Original capture has an error "+label);
                TexturePacker.Image source=Source(Path.Combine(directory,label+"."+extension),profile,pattern,width,height);uint[] before=(uint[])source.Pixels.Clone();Isolated();TexturePacker.Image actual=TextureNpotRgba8Resampler.Resize(source);Isolated();
                Check(actual.Width==targetWidth&&actual.Height==targetHeight&&actual.HasAlpha==bool.Parse(f[7]),"Dimensions/alpha "+label);
                byte[] expected=File.ReadAllBytes(Path.Combine(directory,label+".loaded.rgba"));Check(expected.Length==actual.Pixels.Length*4,"Pixel length "+label);
                for(int i=0;i<actual.Pixels.Length;i++){uint value=BitConverter.ToUInt32(expected,i*4);Check(value==actual.Pixels[i],label+" pixel="+i+" x="+(i%targetWidth)+" y="+(i/targetWidth)+" original="+value.ToString("X8")+" recovered="+actual.Pixels[i].ToString("X8"));}
                Isolated();TexturePacker.Image decoded=TextureImageLoader.Load(Path.Combine(directory,label+"."+extension));Isolated();
                Check(decoded.Width==targetWidth&&decoded.Height==targetHeight&&decoded.HasAlpha==bool.Parse(f[7]),"Public loader dimensions/alpha "+label);
                Check(decoded.Pixels.Length==actual.Pixels.Length,"Public loader length "+label);
                for(int i=0;i<decoded.Pixels.Length;i++)Check(decoded.Pixels[i]==BitConverter.ToUInt32(expected,i*4),"Public loader pixel "+label+"/"+i);
                for(int i=0;i<before.Length;i++)Check(source.Pixels[i]==before[i],"Source modified "+label);fixtures++;
            }
            Check(fixtures==1012,"Full replay count");bool rejected=false;try{TextureNpotRgba8Resampler.Resize(null);}catch(ArgumentNullException){rejected=true;}Check(rejected,"Null input rejection");
            Console.WriteLine("PASS Texture NPOT RGBA8 isolated component fixtures="+fixtures+" coefficientTaps="+taps+" checks="+checks+"; complete original LoadImageA RGBA and exact native coefficients; original absent/unloaded per call; 8-bit public decoder dispatch also compared; plugin/SDK acceptance is separate");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
