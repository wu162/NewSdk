using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

internal static class TextureTgaMetadataReplay
{
    static int checks,fixtures,decoded,rejected,boundaryRejections;static long compared;
    static void Check(bool v,string s){checks++;if(!v)throw new Exception(s);}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original loaded");}
    static string Hash(byte[] b){using(var s=SHA256.Create())return BitConverter.ToString(s.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";var p=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static uint Dword(byte[] b,int at){return (uint)b[at]|(uint)b[at+1]<<8|(uint)b[at+2]<<16|(uint)b[at+3]<<24;}
    static void Set(byte[] b,int at,uint value){for(int i=0;i<4;i++)b[at+i]=(byte)(value>>(i*8));}
    static void Boundaries(string directory){
        foreach(string format in new[]{"rgba32","index32"}){
            byte[] valid=File.ReadAllBytes(Path.Combine(directory,format+"-8x4-rFalse-d32-p0-all.tga"));int footer=valid.Length-26,ext=(int)Dword(valid,footer),dev=(int)Dword(valid,footer+4),scan=(int)Dword(valid,ext+490),stamp=(int)Dword(valid,ext+486);
            for(int mutation=0;mutation<24;mutation++){
                byte[] b=(byte[])valid.Clone();
                switch(mutation){
                    case 0:Set(b,footer,uint.MaxValue);break;case 1:Set(b,footer+4,uint.MaxValue);break;
                    case 2:b[ext]=238;break;case 3:b[ext+494]=5;break;
                    case 4:Set(b,ext+482,uint.MaxValue);break;case 5:Set(b,ext+486,uint.MaxValue);break;case 6:Set(b,ext+490,uint.MaxValue);break;
                    case 7:b[dev]=255;b[dev+1]=255;break;case 8:Set(b,dev+4,uint.MaxValue);break;case 9:Set(b,dev+8,uint.MaxValue);break;
                    case 10:Set(b,scan,0);break;case 11:Set(b,scan,(uint)(ext+1));break;
                    case 12:b[stamp]=65;break;case 13:b[stamp+1]=65;break;case 14:b[footer+8]=0;break;case 15:b[b.Length-1]=1;break;
                    case 16:Array.Resize(ref b,b.Length-1);break;
                    case 17:Array.Resize(ref b,b.Length+1);b[b.Length-1]=77;break;
                    case 18:b=new byte[valid.Length+1];Buffer.BlockCopy(valid,0,b,0,footer);b[footer]=77;Buffer.BlockCopy(valid,footer,b,footer+1,26);break;
                    case 19:Set(b,dev+4,2);break;case 20:Set(b,ext+482,0);break;case 21:Set(b,footer,0);break;case 22:Set(b,footer+4,0);break;case 23:b[stamp+1]=0;break;
                }
                string before=Hash(b);
                foreach(bool dispatch in new[]{false,true}){
                    Exception error=null;Isolated();try{if(dispatch)TextureImageLoader.Decode(b);else if(TextureTgaAdditionalFormats.CanDecode(b))TextureTgaAdditionalFormats.Decode(b);else TextureTgaDecoder.Decode(b);}catch(Exception e){error=e;}Isolated();
                    Check(error is NotSupportedException,"Unsupported corrupt metadata boundary "+format+"/"+mutation+"/"+dispatch+": "+error);Check(Hash(b)==before,"Rejected source modified");boundaryRejections++;
                }
            }
        }
        Check(boundaryRejections==96,"Complete corrupt-tail refusal matrix");
    }
    static int Main(string[] args){try{
        Check(IntPtr.Size==4,"x86 required");int count=int.Parse(args[1]);Check(count==12480,"Known immutable corpus");string directory=Path.GetFullPath(args[0]);string[] lines=File.ReadAllLines(Path.Combine(directory,"transcript.tsv"));Check(lines.Length==count+1,"Complete immutable corpus");var labels=new HashSet<string>();
        foreach(string line in lines){if(line.StartsWith("label\t"))continue;string[] f=line.Split('\t');Check(f.Length==15&&labels.Add(f[0]),"Unique complete row");string label=f[0],path=Path.Combine(directory,label+".tga");byte[] source=File.ReadAllBytes(path);Check(Hash(source)==f[8],"Immutable source "+label);
            foreach(bool dispatch in new[]{false,true}){
                TexturePacker.Image actual=null;Exception error=null;Isolated();try{actual=dispatch?TextureImageLoader.Decode(source):(TextureTgaAdditionalFormats.CanDecode(source)?TextureTgaAdditionalFormats.Decode(source):TextureTgaDecoder.Decode(source));}catch(Exception e){error=e;}Isolated();
                Check(Hash(source)==f[8]&&Hash(File.ReadAllBytes(path))==f[8],"Source changed "+label);
                Check(Error(error)==f[7],"Original error "+label+" public="+dispatch+" expected="+f[7]+" actual="+Error(error));
                if(f[7].Length!=0){Check(actual==null,"Error returned image");rejected++;continue;}
                Check(actual!=null&&actual.Width==int.Parse(f[2])&&actual.Height==int.Parse(f[3])&&actual.HasAlpha==bool.Parse(f[4]),"Dimensions/alpha "+label);
                byte[] expected=File.ReadAllBytes(Path.Combine(directory,label+".rgba"));Check(expected.Length==int.Parse(f[5])&&Hash(expected)==f[6],"Immutable RGBA "+label);Check(actual.Pixels.Length*4==expected.Length,"Full pixel length "+label);
                for(int i=0;i<actual.Pixels.Length;i++)Check(actual.Pixels[i]==BitConverter.ToUInt32(expected,i*4),"Full RGBA "+label+"/"+i+" expected="+BitConverter.ToUInt32(expected,i*4).ToString("X8")+" actual="+actual.Pixels[i].ToString("X8"));compared+=expected.Length;decoded++;
            }fixtures++;
        }
        Boundaries(directory);
        Check(fixtures==count&&decoded+rejected==count*2,"Complete dual-entry accounting");Console.WriteLine("PASS Texture TGA metadata NoOriginal fixtures="+fixtures+" publicCalls="+(fixtures*2)+" decoded="+decoded+" rejected="+rejected+" boundaryRejections="+boundaryRejections+" checks="+checks+" comparedBytes="+compared+"; full original RGBA/error domains, unchanged inputs, absent/unloaded original per call; plugin/SDK separate");return 0;
    }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
