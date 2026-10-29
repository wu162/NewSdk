using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

internal static class TextureBmpHeaderReplay
{
    static int checks,fixtures,decoded,rejected,explicitRefusals,boundaryRejections;static long compared;
    static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original loaded");}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";var p=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static void Set(byte[] b,int at,uint value){for(int i=0;i<4;i++)b[at+i]=(byte)(value>>(i*8));}
    static void RejectBoundary(byte[] image,string label)
    {
        string before=Hash(image);
        foreach(bool dispatch in new[]{false,true}){
            TexturePacker.Image actual=null;Exception error=null;Isolated();try{actual=dispatch?TextureImageLoader.Decode(image):TextureBmpDecoder.Decode(image);}catch(Exception e){error=e;}Isolated();
            Check(error is InvalidDataException&&actual==null,"Bounded BMP structure refusal "+label+" public="+dispatch+": "+error);Check(Hash(image)==before,"Rejected input changed "+label);boundaryRejections++;
        }
    }
    static void Boundaries(string directory)
    {
        foreach(int dib in new[]{12,40,52,56,64,108,124}){
            int depth=dib==12||dib==64?24:32;
            byte[] valid=File.ReadAllBytes(Path.Combine(directory,"h"+dib+"-d"+depth+"-16x8-topFalse-p0-c0-g0-plain.bmp"));
            for(int mutation=0;mutation<7;mutation++){
                byte[] image=(byte[])valid.Clone();
                switch(mutation){
                    case 0:Array.Resize(ref image,14+dib-1);break;
                    case 1:Set(image,10,(uint)(14+dib-1));break;
                    case 2:Set(image,10,uint.MaxValue);break;
                    case 3:if(dib==12){image[18]=image[19]=0;}else Set(image,18,0);break;
                    case 4:image[dib==12?22:26]=image[dib==12?23:27]=0;break;
                    case 5:if(dib==12){image[20]=image[21]=0;}else Set(image,22,0);break;
                    case 6:Array.Resize(ref image,image.Length-1);break;
                }
                RejectBoundary(image,"dib="+dib+" mutation="+mutation);
            }
            if(dib==124)for(int mutation=0;mutation<4;mutation++){
                byte[] image=(byte[])valid.Clone();
                if(mutation==0)Set(image,126,1);
                if(mutation==1)Set(image,130,1);
                if(mutation==2){Set(image,126,uint.MaxValue);Set(image,130,16);}
                if(mutation==3){Set(image,126,124);Set(image,130,16);}
                RejectBoundary(image,"V5 profile mutation="+mutation);
            }
        }
        Check(boundaryRejections==106,"Complete malformed-header/profile boundary matrix");
    }
    static int Main(string[] args){try{
        Check(IntPtr.Size==4,"x86 required");int count=int.Parse(args[1]);Check(count==2616,"Known immutable corpus");string directory=Path.GetFullPath(args[0]);string[] lines=File.ReadAllLines(Path.Combine(directory,"transcript.tsv"));Check(lines.Length==count+1,"Complete immutable corpus");var labels=new HashSet<string>();
        foreach(string line in lines){if(line.StartsWith("label\t"))continue;string[] f=line.Split('\t');Check(f.Length==24&&labels.Add(f[0]),"Unique complete row");string label=f[0],path=Path.Combine(directory,label+".bmp");byte[] source=File.ReadAllBytes(path);Check(Hash(source)==f[8],"Immutable source "+label);
            foreach(bool dispatch in new[]{false,true}){
                TexturePacker.Image actual=null;Exception error=null;Isolated();try{actual=dispatch?TextureImageLoader.Decode(source):TextureBmpDecoder.Decode(source);}catch(Exception e){error=e;}Isolated();
                Check(Hash(source)==f[8]&&Hash(File.ReadAllBytes(path))==f[8],"Source changed "+label);
                if((f[9]!="40"&&f[11]=="3")){Check(error is NotSupportedException&&actual==null,"Explicit unrecovered bitfields refusal "+label+" public="+dispatch+": "+error);explicitRefusals++;continue;}
                Check(bool.Parse(f[17]),"Deterministic original pair required "+label);
                Check(Error(error)==f[7],"Original error "+label+" public="+dispatch+" expected="+f[7]+" actual="+Error(error));
                if(f[7].Length!=0){Check(actual==null,"Error returned image");rejected++;continue;}
                Check(actual!=null&&actual.Width==int.Parse(f[2])&&actual.Height==int.Parse(f[3])&&actual.HasAlpha==bool.Parse(f[4]),"Dimensions/alpha "+label);
                byte[] expected=File.ReadAllBytes(Path.Combine(directory,label+".rgba"));Check(expected.Length==int.Parse(f[5])&&Hash(expected)==f[6],"Immutable RGBA "+label);Check(actual.Pixels.Length*4==expected.Length,"Full pixel length "+label);
                for(int i=0;i<actual.Pixels.Length;i++)Check(actual.Pixels[i]==BitConverter.ToUInt32(expected,i*4),"Full RGBA "+label+"/"+i+" expected="+BitConverter.ToUInt32(expected,i*4).ToString("X8")+" actual="+actual.Pixels[i].ToString("X8"));compared+=expected.Length;decoded++;
            }fixtures++;
        }
        Boundaries(directory);
        Check(fixtures==count&&decoded==1896*2&&rejected==432*2&&explicitRefusals==288*2&&decoded+rejected+explicitRefusals==count*2,"Complete dual-entry accounting");Console.WriteLine("PASS Texture BMP DIB/metadata NoOriginal fixtures="+fixtures+" publicCalls="+(fixtures*2)+" decoded="+decoded+" rejected="+rejected+" explicitRefusals="+explicitRefusals+" boundaryRejections="+boundaryRejections+" checks="+checks+" comparedBytes="+compared+"; full supported original RGBA/error domains; expanded-DIB bitfields refusals are not original error matches, unchanged inputs, absent/unloaded original per call; plugin/SDK separate");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
