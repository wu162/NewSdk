using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

internal static class TextureDibOracle
{
    static int fixtures,decoded,rejected,refused,originalCalls,originalBmpCalls,checks,tables,boundaries;static long compared;
    static bool isolated;static MethodInfo originalLoad;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static string Hash(byte[] data){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(data)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception error){if(error==null)return "";while(error is TargetInvocationException&&error.InnerException!=null)error=error.InnerException;var p=error.GetType().GetProperty("ErrorCode");return error.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(error,null)))+"/"+error.Message;}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original DLL loaded");}
    static byte[] Bytes(BinaryReader reader){int n=reader.ReadInt32();if(n==-1)return null;Check(n>=0&&n<16777216,"Bounded OS2 gold");byte[] data=reader.ReadBytes(n);Check(data.Length==n,"Complete OS2 gold");return data;}
    static void Put(byte[] data,int at,uint value){for(int i=0;i<4;i++)data[at+i]=(byte)(value>>(i*8));}
    static byte[] Table(byte[] bmp,int entries)
    {
        int header=(int)BitConverter.ToUInt32(bmp,14),bytes=entries*4,start=14+header;
        Check(header!=12&&BitConverter.ToUInt32(bmp,46)==0,"Direct-colour table source");
        byte[] result=new byte[bmp.Length+bytes];Buffer.BlockCopy(bmp,0,result,0,start);Buffer.BlockCopy(bmp,start,result,start+bytes,bmp.Length-start);
        for(int i=0;i<bytes;i++)result[start+i]=(byte)(i*73+139);
        Put(result,2,(uint)result.Length);Put(result,10,BitConverter.ToUInt32(bmp,10)+(uint)bytes);Put(result,46,(uint)entries);
        if(header==124&&BitConverter.ToUInt32(bmp,126)!=0)Put(result,126,BitConverter.ToUInt32(bmp,126)+(uint)bytes);
        tables++;return result;
    }
    static void Image(TexturePacker.Image image,int width,int height,bool alpha,byte[] expected,string label)
    {
        Check(image!=null&&image.Width==width&&image.Height==height&&image.HasAlpha==alpha,"Dimensions/alpha "+label);
        Check(expected!=null&&image.Pixels.Length*4==expected.Length,"RGBA length "+label);
        for(int i=0;i<image.Pixels.Length;i++)Check(image.Pixels[i]==BitConverter.ToUInt32(expected,i*4),"Full RGBA "+label+"/"+i);
        compared+=expected.Length;
    }
    static void Case(byte[] bmp,string label,int width,int height,bool alpha,string expectedError,byte[] expected,bool refusal,string work,StreamWriter log)
    {
        Check(bmp!=null&&bmp.Length>=26&&bmp[0]==66&&bmp[1]==77,"Bounded original BMP source");
        byte[] dib=new byte[bmp.Length-14];Buffer.BlockCopy(bmp,14,dib,0,dib.Length);
        string bmpBefore=Hash(bmp);string before=Hash(dib),path=Path.Combine(work,"input.dib");File.WriteAllBytes(path,dib);
        Check(TextureDibDecoder.IsDib(dib)&&!TextureDibDecoder.IsDib(bmp),"Raw DIB recognition "+label);
        if(!isolated&&!refusal){
            TexturePacker.Image actual=null;Exception error=null;originalCalls++;
            try{actual=(TexturePacker.Image)originalLoad.Invoke(null,new object[]{path});}catch(Exception e){error=e;}
            Check(Error(error)==expectedError,"Original raw DIB/BMP error equality "+label+": "+Error(error));
            if(expectedError=="")Image(actual,width,height,alpha,expected,"original raw DIB "+label);else Check(actual==null,"Original rejection returned image");
        }
        if(!isolated&&!refusal&&label.Contains("-table")){
            string bmpPath=Path.Combine(work,"input.bmp");File.WriteAllBytes(bmpPath,bmp);originalBmpCalls++;
            TexturePacker.Image actual=null;Exception error=null;
            try{actual=(TexturePacker.Image)originalLoad.Invoke(null,new object[]{bmpPath});}catch(Exception e){error=e;}
            Check(Error(error)==expectedError,"Original BMP table error "+label+": "+Error(error));
            if(expectedError=="")Image(actual,width,height,alpha,expected,"original BMP table "+label);
            Check(Hash(File.ReadAllBytes(bmpPath))==Hash(bmp),"Original BMP unchanged");
        }
        for(int entry=0;entry<6;entry++){
            TexturePacker.Image actual=null;Exception error=null;if(isolated)Isolated();
            try{switch(entry){
                case 0:actual=TextureDibDecoder.Decode(dib);break;
                case 1:actual=TextureDibDecoder.Load(path);break;
                case 2:actual=TextureImageLoader.Decode(dib);break;
                case 3:actual=TextureImageLoader.Load(path);break;
                case 4:actual=TextureBmpDecoder.Decode(bmp);break;
                default:actual=TextureImageLoader.Decode(bmp);break;
            }}catch(Exception e){error=e;}
            if(isolated)Isolated();
            Check(Hash(bmp)==bmpBefore&&Hash(dib)==before&&Hash(File.ReadAllBytes(path))==before,"DIB source unchanged "+label);
            if(refusal){Check(error is NotSupportedException&&actual==null,"Uncertain bitfields remain refused "+label);refused++;continue;}
            Check(Error(error)==expectedError,"Recovered DIB error "+label+": "+Error(error));
            if(expectedError!=""){Check(actual==null,"Rejected DIB returned image");rejected++;continue;}
            Image(actual,width,height,alpha,expected,"recovered DIB "+label);decoded++;
        }
        log.WriteLine(label+"\t"+before+"\t"+(refusal?"explicit-refusal":expectedError==""?"decoded":"original-error")+"\t"+(expected==null?"":Hash(expected)));fixtures++;
    }
    static void Boundary(string header)
    {
        // Safety refusals, not claims about undefined original out-of-bounds reads.
        foreach(int dib in new[]{40,52,56,64,108,124})foreach(int depth in new[]{16,24,32})foreach(int mutation in new[]{0,1,2}){
            byte[] bmp=null;
            foreach(string file in Directory.GetFiles(header,"*.bmp")){
                byte[] candidate=File.ReadAllBytes(file);
                if(BitConverter.ToUInt32(candidate,14)==dib&&candidate[28]==depth&&BitConverter.ToUInt32(candidate,30)==0&&BitConverter.ToUInt32(candidate,46)==0){bmp=candidate;break;}
            }
            if(bmp==null&&dib==64){
                bmp=new byte[14+64+16*8*(depth/8)];bmp[0]=66;bmp[1]=77;Put(bmp,2,(uint)bmp.Length);Put(bmp,10,78);Put(bmp,14,64);Put(bmp,18,16);Put(bmp,22,8);bmp[26]=1;bmp[28]=(byte)depth;
            }
            Check(bmp!=null,"Boundary source exists");bmp=Table(bmp,17);tables--;
            if(mutation==0)Put(bmp,46,uint.MaxValue);
            if(mutation==1)Array.Resize(ref bmp,14+dib+67);
            if(mutation==2){bmp[26]=2;}
            byte[] raw=new byte[bmp.Length-14];Buffer.BlockCopy(bmp,14,raw,0,raw.Length);
            foreach(int entry in new[]{0,1,2,3}){
                string before=Hash(bmp),rawBefore=Hash(raw);Exception error=null;TexturePacker.Image image=null;
                if(isolated)Isolated();
                try{image=entry==0?TextureBmpDecoder.Decode(bmp):entry==1?TextureDibDecoder.Decode(raw):entry==2?TextureImageLoader.Decode(bmp):TextureImageLoader.Decode(raw);}catch(Exception e){error=e;}
                if(isolated)Isolated();
                Check(error is InvalidDataException&&image==null,"Bounded table refusal "+dib+"/"+depth+"/"+mutation+"/"+entry+": "+error);
                Check(Hash(bmp)==before&&Hash(raw)==rawBefore,"Malformed table source unchanged");boundaries++;
            }
        }
    }
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");isolated=args.Length==4;if(!isolated&&args.Length!=5)throw new Exception("Usage: [original.dll] header-gold os2-gold work transcript");int shift=isolated?0:1;
            string header=Path.GetFullPath(args[shift]),os2=Path.GetFullPath(args[shift+1]),work=Path.GetFullPath(args[shift+2]);Directory.CreateDirectory(work);
            if(isolated)Isolated();else{
                string input=Path.GetFullPath(args[0]);typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{Assembly.LoadFrom(input),input});
                owner=(IDisposable)typeof(TexturePngOracle).GetField("owner",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
                originalLoad=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.NonPublic|BindingFlags.Static);
            }
            using(var log=new StreamWriter(args[shift+3])){
                log.WriteLine("label\tdibInputSha256\toutcome\tgoldRgbaSha256");
                foreach(string line in File.ReadAllLines(Path.Combine(header,"transcript.tsv"))){
                    if(line.StartsWith("label\t"))continue;string[] f=line.Split('\t');Check(f.Length==24,"Complete BMP header gold");
                    byte[] bmp=File.ReadAllBytes(Path.Combine(header,f[0]+".bmp"));Check(Hash(bmp)==f[8],"Immutable BMP input");
                    bool refusal=f[9]!="40"&&f[11]=="3";Check(refusal||bool.Parse(f[17]),"Deterministic original gold required");
                    byte[] rgba=!refusal&&f[7]==""?File.ReadAllBytes(Path.Combine(header,f[0]+".rgba")):null;
                    if(rgba!=null)Check(Hash(rgba)==f[6]&&rgba.Length==int.Parse(f[5]),"Immutable BMP full RGBA");
                    Case(bmp,"header/"+f[0],int.Parse(f[2]),int.Parse(f[3]),bool.Parse(f[4]),f[7],rgba,refusal,work,log);
                    if(f[11]=="0"&&f[9]!="12"&&f[13]=="0"&&f[0].Contains("-16x8-")&&f[0].Contains("-p0-"))
                        foreach(int entries in new[]{1,3,17})Case(Table(bmp,entries),"header-table"+entries+"/"+f[0],int.Parse(f[2]),int.Parse(f[3]),bool.Parse(f[4]),f[7],rgba,false,work,log);
                }
                using(var reader=new BinaryReader(File.OpenRead(os2))){Check(reader.ReadInt32()==1944,"Complete OS2 source corpus");
                    int count=0;while(reader.BaseStream.Position<reader.BaseStream.Length){string label=reader.ReadString();byte[] bmp=Bytes(reader);int width=reader.ReadInt32(),height=reader.ReadInt32();bool alpha=reader.ReadBoolean();string error=reader.ReadString();byte[] rgba=Bytes(reader);Case(bmp,"os2/"+label,width,height,alpha,error,rgba,false,work,log);count++;
                        if(label.StartsWith("h64-")&&bmp[28]>=16&&label.Contains("-16x8-")&&label.Contains("-p0-")&&label.Contains("-g0-")&&(bmp[28]!=24||label.Contains("-topTrue-")))
                            foreach(int entries in new[]{1,3,17})Case(Table(bmp,entries),"os2-table"+entries+"/"+label,width,height,alpha,error,rgba,false,work,log);
                    }Check(count==1944,"Complete OS2 source accounting");
                }
            }
            Boundary(header);
            Check(boundaries==216,"Complete malformed table accounting");
            Check(fixtures==4866&&tables==306&&decoded==17100&&rejected==10368&&refused==1728&&originalCalls==(isolated?0:4578)&&originalBmpCalls==(isolated?0:306),"Complete DIB accounting fixtures="+fixtures+" tables="+tables+" decoded="+decoded+" rejected="+rejected+" refused="+refused);
            Console.WriteLine("PASS Texture raw DIB "+(isolated?"NoOriginal":"differential")+" fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" explicitRefusals="+refused+" rawDibOriginalCalls="+originalCalls+" bmpTableOriginalCalls="+originalBmpCalls+" boundaries="+boundaries+" checks="+checks+" comparedBytes="+compared+"; core/INFO/V2/V3/OS2/V4/V5, full RGBA/alpha/NPOT/gaps/metadata, direct/public array/file and BMP APIs, actual original raw-DIB/BMP table equality and per-call isolation; Plugin/SDK separate");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
