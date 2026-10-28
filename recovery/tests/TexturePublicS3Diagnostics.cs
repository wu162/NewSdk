using System;
using System.IO;
using System.Runtime.InteropServices;
internal static class TexturePublicS3Diagnostics {
 [DllImport("TexturePublicCodecs.dll",CallingConvention=CallingConvention.Cdecl,EntryPoint="S3TraceFile")]static extern void SourceTrace(string path);
 [DllImport("TextureS3LegacyDiagnostic.dll",CallingConvention=CallingConvention.Cdecl,EntryPoint="S3TraceFile")]static extern void LegacyTrace(string path);
 [DllImport("TexturePublicCodecs.dll",CallingConvention=CallingConvention.Cdecl)]static extern int PublicDxtEncode(int backend,int variant,int format,byte[] rgba,byte[] output);
 [DllImport("TextureS3LegacyDiagnostic.dll",CallingConvention=CallingConvention.StdCall,EntryPoint="TextureColorState")]static extern int Legacy(byte[] state,int bytes,int mode);
 static void Put(byte[] b,int offset,double value){Buffer.BlockCopy(BitConverter.GetBytes(value),0,b,offset,8);}
 static int Main(string[] args){try{
  Directory.CreateDirectory(args[1]);int count=0;
  foreach(string line in File.ReadAllLines(args[0])){
   if(!line.StartsWith("D3DFMT_DXT3\t"))continue;string[] row=line.Split('\t');int fixture=int.Parse(row[1]);
   // Diagnostic corpus identities only; no runtime encoder receives a fixture ID.
   if(Array.IndexOf(new[]{263,281,337,356,462,1024,1361,1445,1690,1937,2132,2144,2405,2487,2527,2758,2878,2944,3010,3618,3651,3787,3910,4001,4268,4299,4838,5115},fixture)<0)continue;
   byte[] pixels=TexturePublicCodecsReplay.Pixels(fixture),actual=new byte[16],state=new byte[0x3850];
   SourceTrace(Path.Combine(args[1],fixture+"-source.tsv"));if(PublicDxtEncode(5,0,2,pixels,actual)!=16)throw new Exception("Source encode");SourceTrace(null);
   Buffer.BlockCopy(BitConverter.GetBytes(2),0,state,0,4);Buffer.BlockCopy(BitConverter.GetBytes(1),0,state,4,4);Buffer.BlockCopy(BitConverter.GetBytes(16),0,state,8,4);Buffer.BlockCopy(BitConverter.GetBytes(71),0,state,0x3030,4);
   for(int i=0;i<16;i++)for(int c=0;c<3;c++)Put(state,0x10+(i*3+c)*8,(double)(float)(pixels[i*4+c]/255.0));Put(state,0x3010,16);Put(state,0x3018,(double)0.3086f);Put(state,0x3020,(double)0.6094f);Put(state,0x3028,(double)0.082f);
   LegacyTrace(Path.Combine(args[1],fixture+"-legacy.tsv"));if(Legacy(state,state.Length,0)!=0)throw new Exception("Legacy diagnostic encode");LegacyTrace(null);
   int a=(BitConverter.ToInt32(state,0x3038)<<11)|(BitConverter.ToInt32(state,0x303c)<<5)|BitConverter.ToInt32(state,0x3040),b=(BitConverter.ToInt32(state,0x3044)<<11)|(BitConverter.ToInt32(state,0x3048)<<5)|BitConverter.ToInt32(state,0x304c);uint bits=0;
   if(a==b){a=(a+1)&65535;for(int i=0;i<16;i++)Buffer.BlockCopy(BitConverter.GetBytes(1),0,state,0x3050+4*i,4);}bool swap=a<b;if(swap){int t=a;a=b;b=t;}for(int i=0;i<16;i++)bits|=(uint)(BitConverter.ToInt32(state,0x3050+4*i)^(swap?1:0))<<(2*i);
   string legacy=BitConverter.ToString(new[]{(byte)a,(byte)(a>>8),(byte)b,(byte)(b>>8),(byte)bits,(byte)(bits>>8),(byte)(bits>>16),(byte)(bits>>24)}).Replace("-","").ToLowerInvariant();
   if(legacy!=row[2].Replace("-","").Substring(16).ToLowerInvariant())throw new Exception("Instrumented reference differs from immutable original gold fixture="+fixture);
   File.WriteAllText(Path.Combine(args[1],fixture+"-result.txt"),"original="+row[2]+"\nsource="+BitConverter.ToString(actual)+"\n");count++;
  }
  if(count!=28)throw new Exception("Incomplete diagnostic corpus");Console.WriteLine("PASS S3 stage diagnostic inputs=28; instrumented reference matches immutable original gold; diagnostic-only, no acceptance");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
