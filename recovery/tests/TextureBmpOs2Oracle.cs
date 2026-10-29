using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Recovered.Compiler.Texture;

internal static class TextureBmpOs2Oracle
{
    static int fixtures,decoded,rejected,checks,boundaries;static long compared;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static string Hash(byte[] b){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","");}
    static string Error(Exception error){if(error==null)return "";while(error is TargetInvocationException&&error.InnerException!=null)error=error.InnerException;var p=error.GetType().GetProperty("ErrorCode");return error.GetType().FullName+"/"+(p==null?"":Convert.ToString(p.GetValue(error,null)))+"/"+error.Message;}
    static void Isolated(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original file present");foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original loaded");}
    static byte[] Rgba(TexturePacker.Image image){if(image==null)return null;byte[] bytes=new byte[image.Pixels.Length*4];Buffer.BlockCopy(image.Pixels,0,bytes,0,bytes.Length);return bytes;}
    static void Bytes(BinaryWriter w,byte[] b){w.Write(b==null?-1:b.Length);if(b!=null)w.Write(b);}
    static byte[] Bytes(BinaryReader r){int n=r.ReadInt32();if(n==-1)return null;Check(n>=0&&n<16777216,"Bounded replay");byte[] b=r.ReadBytes(n);Check(b.Length==n,"Complete replay");return b;}
    static void Compare(byte[] source,string label,int width,int height,bool alpha,string error,byte[] expected,bool isolated)
    {
        string before=Hash(source);
        for(int dispatch=0;dispatch<2;dispatch++){
            TexturePacker.Image image=null;Exception failure=null;if(isolated)Isolated();
            try{image=dispatch==0?TextureBmpDecoder.Decode(source):TextureImageLoader.Decode(source);}catch(Exception e){failure=e;}
            if(isolated)Isolated();Check(Hash(source)==before,"Source unchanged "+label);
            Check(Error(failure)==error,"Error "+label+" expected="+error+" actual="+Error(failure));
            if(error!=""){Check(image==null,"Rejected image returned");rejected++;continue;}
            Check(image!=null&&image.Width==width&&image.Height==height&&image.HasAlpha==alpha,"Dimensions/alpha "+label);
            byte[] actual=Rgba(image);Check(expected!=null&&actual.Length==expected.Length,"RGBA length "+label);
            for(int i=0;i<expected.Length;i++)Check(expected[i]==actual[i],"RGBA "+label+"/"+i);compared+=expected.Length;decoded++;
        }
        fixtures++;
    }
    static void Boundary()
    {
        // New core16/32/OS2-64 paths retain bounded source validation. These
        // malformed inputs are recovery safety checks, not original gold claims.
        foreach(int dib in new[]{12,64})foreach(int depth in new[]{16,24,32})foreach(int mutation in new[]{0,1,2,3,4,5}){
            byte[] b=TextureBmpOs2Fixtures.Create(4,2,depth,false,0,dib,0,0);
            if(mutation==0)Array.Resize(ref b,17);
            if(mutation==1)Array.Resize(ref b,14+dib-1);
            if(mutation==2)Array.Resize(ref b,b.Length-1);
            if(mutation==3){b[18]=0;b[19]=0;if(dib!=12){b[20]=0;b[21]=0;}}
            if(mutation==4){b[dib==12?22:26]=2;}
            if(mutation==5){for(int i=0;i<4;i++)b[10+i]=255;}
            string before=Hash(b);Exception error=null;try{TextureBmpDecoder.Decode(b);}catch(Exception e){error=e;}
            Check(error is InvalidDataException,"Bounded OS2 malformed source "+dib+"/"+depth+"/"+mutation+": "+error);Check(Hash(b)==before,"Malformed source unchanged");boundaries++;
        }
    }
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");bool isolated=args.Length==1;
            if(isolated){
                Isolated();using(var r=new BinaryReader(File.OpenRead(args[0]))){Check(r.ReadInt32()==TextureBmpOs2Fixtures.DirectCount,"Full direct corpus");
                    while(r.BaseStream.Position<r.BaseStream.Length){string label=r.ReadString();byte[] source=Bytes(r);int width=r.ReadInt32(),height=r.ReadInt32();bool alpha=r.ReadBoolean();string error=r.ReadString();byte[] rgba=Bytes(r);Compare(source,label,width,height,alpha,error,rgba,true);}
                }
            }else{
                string original=Path.GetFullPath(args[0]),work=Path.GetFullPath(args[1]);Directory.CreateDirectory(work);
                typeof(TexturePngOracle).GetMethod("Bind",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{Assembly.LoadFrom(original),original});
                owner=(IDisposable)typeof(TexturePngOracle).GetField("owner",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
                MethodInfo load=typeof(TexturePngOracle).GetMethod("Original",BindingFlags.NonPublic|BindingFlags.Static);
                using(var w=new BinaryWriter(File.Create(args[2]))){w.Write(TextureBmpOs2Fixtures.DirectCount);
                    foreach(int dib in new[]{12,64})foreach(int depth in new[]{1,4,8,16,24,32})foreach(int[] size in TextureBmpOs2Fixtures.Sizes)foreach(bool top in dib==12?new[]{false}:new[]{false,true})foreach(int pattern in new[]{0,1,2})foreach(int gap in new[]{0,3,17})foreach(int reserved in depth<=8?new[]{0,255}:new[]{0}){
                        string label="h"+dib+"-d"+depth+"-"+size[0]+"x"+size[1]+"-top"+top+"-p"+pattern+"-g"+gap+"-a"+reserved;
                        byte[] source=TextureBmpOs2Fixtures.Create(size[0],size[1],depth,top,pattern,dib,gap,reserved);string path=Path.Combine(work,"input.bmp");File.WriteAllBytes(path,source);
                        TexturePacker.Image image=null;Exception failure=null;try{image=(TexturePacker.Image)load.Invoke(null,new object[]{path});}catch(Exception e){failure=e;}
                        string error=Error(failure);Check(Hash(source)==Hash(File.ReadAllBytes(path)),"Original source unchanged");
                        Check(depth<=8?error=="BinaryAssetBuilder.BinaryAssetBuilderException/BadTextureFile/D3DX cannot load the texture from disk.":error=="","Original full OS2 BI_RGB contract "+label+": "+error);
                        int width=image==null?0:image.Width,height=image==null?0:image.Height;bool alpha=image!=null&&image.HasAlpha;byte[] rgba=Rgba(image);
                        w.Write(label);Bytes(w,source);w.Write(width);w.Write(height);w.Write(alpha);w.Write(error);Bytes(w,rgba);
                        Compare(source,label,width,height,alpha,error,rgba,false);
                    }
                }
            }
            Boundary();Check(fixtures==1944&&decoded==1296&&rejected==2592&&boundaries==36,"Complete full OS2 dual-entry accounting");
            Console.WriteLine("PASS Texture BMP OS2 "+(isolated?"NoOriginal":"differential")+" fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" boundaries="+boundaries+" checks="+checks+" comparedBytes="+compared+"; core12/OS2-64 BI_RGB16/24/32, signed64 height, eight POT/NPOT/partial shapes, gaps/alpha/orientation; original indexed errors; source unchanged; SDK separate");return 0;
        }catch(Exception error){Console.Error.WriteLine(error);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
