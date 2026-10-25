using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

internal static class TextureRasterPipelineOracle
{
    static int fixtures, errors, unsupportedResampling; static long compared;
    static InstanceDeclaration Declaration(string root,string file,string kind,string format,string cdata,bool mips)
    {
        var xml=new XmlDocument(); var node=xml.CreateElement(root,"uri:ea.com:eala:asset");xml.AppendChild(node);
        node.SetAttribute("id","ImagePipeline");node.SetAttribute("File",file);node.SetAttribute("Type",kind);
        node.SetAttribute("OutputFormat",format);node.SetAttribute("GenerateMipMaps",mips?"true":"false");node.SetAttribute("AllowAutomaticResize","false");
        var manager=new XmlNamespaceManager(xml.NameTable);manager.AddNamespace("ea","uri:ea.com:eala:asset");
        var doc=new AssetDeclarationDocument();Type t=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(t);t.GetField("NamespaceManager").SetValue(state,manager);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);
        var d=new InstanceDeclaration(doc);d.XmlNode=node;d.CustomDataPath=cdata;
        d.Handle.TypeHash=0x12345678u;d.Handle.InstanceHash=0x9abcdef0u;return d;
    }
    static void Equal(byte[] a,byte[] b,string label)
    {
        if(a==null||b==null){if(a!=b)throw new Exception(label+" null differs");return;}
        if(a.Length!=b.Length)throw new Exception(label+" length original="+a.Length+" recovered="+b.Length);
        for(int i=0;i<a.Length;i++)if(a[i]!=b[i])throw new Exception(label+" byte="+i+" original="+a[i]+" recovered="+b[i]);
        compared+=a.Length;
    }
    static string Error(Exception e){if(e==null)return null;var b=e as BinaryAssetBuilderException;return e.GetType().FullName+"/"+(b==null?"":b.ErrorCode.ToString())+"/"+e.Message;}
    static void Case(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,string work,string label,string root,string file,string kind,string format,bool stale,bool mips)
    {
        string oa=Path.Combine(work,"original.cdata"),ra=Path.Combine(work,"recovered.cdata");
        foreach(string p in new string[]{oa,ra}){if(File.Exists(p))File.Delete(p);if(stale){byte[] old=new byte[4096];for(int i=0;i<old.Length;i++)old[i]=(byte)(i*13);File.WriteAllBytes(p,old);}}
        AssetBuffer a=null,b=null;Exception ea=null,eb=null;
        try{a=original.ProcessInstance(Declaration(root,file,kind,format,oa,mips));}catch(Exception e){ea=e;}
        try{b=recovered.ProcessInstance(Declaration(root,file,kind,format,ra,mips));}catch(Exception e){eb=e;}
        if(Error(ea)!=Error(eb))throw new Exception(label+" errors original="+Error(ea)+" recovered="+Error(eb));
        if(ea!=null)errors++;
        if(a!=null||b!=null){if(a==null||b==null)throw new Exception(label+" buffer presence");Equal(a.InstanceData,b.InstanceData,label+" instance");Equal(a.RelocationData,b.RelocationData,label+" relo");Equal(a.ImportsData,b.ImportsData,label+" imports");}
        if(File.Exists(oa)!=File.Exists(ra))throw new Exception(label+" cdata presence");
        if(File.Exists(oa))Equal(File.ReadAllBytes(oa),File.ReadAllBytes(ra),label+" cdata");fixtures++;
    }
    static int Main(string[] args)
    {
        try{
            if(args.Length!=4||IntPtr.Size!=4)throw new Exception("Usage (x86): TextureRasterPipelineOracle original.dll recovered.dll realFixtureDirectory workDirectory");
            string od=Path.GetFullPath(args[0]),rd=Path.GetFullPath(args[1]),work=Path.GetFullPath(args[3]);Directory.CreateDirectory(work);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string n=new AssemblyName(e.Name).Name+".dll";foreach(string dir in new[]{Path.GetDirectoryName(od),Path.GetDirectoryName(rd)}){string p=Path.Combine(dir,n);if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};
            var original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(od).GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));
            var recovered=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(rd).GetType("Recovered.Compiler.Texture.Plugin",true));
            original.Initialize(null,TargetPlatform.Win32);
            try{
                foreach(int platform in new int[]{0}){
                    original.ReInitialize(null,(TargetPlatform)platform);recovered.Initialize(null,(TargetPlatform)platform);
                    foreach(string raster in new[]{"png","bmp","bmp565","bmp555","bmp32fields"})
                    foreach(int color in raster=="png"?new[]{0,2,3,4,6}:new[]{0})
                    foreach(int depth in raster=="png"?new[]{1,2,4,8,16}:raster=="bmp"?new[]{1,4,8,16,24,32}:raster=="bmp32fields"?new[]{32}:new[]{16}) {
                        if(raster=="png"&&(color==3?depth==16:color!=0&&depth<8))continue;
                        foreach(int interlace in new[]{0,1}) foreach(int metadata in new[]{0,1}) {
                            string file=Path.Combine(work,"generated."+(raster=="png"?"png":"bmp"));
                            byte[] image=raster=="png"?TexturePngOracle.Fixture(8,4,depth,color,interlace,5,metadata,0):raster=="bmp"?TextureBmpOracle.Fixture(8,4,depth,interlace!=0,metadata==0?0:1,metadata==0?0:255,metadata==0?0:7):TextureBmpOracle.BitfieldsFixture(8,4,raster=="bmp565"?0:raster=="bmp555"?1:2,interlace!=0,metadata==0?0:1,metadata==0?0:7);
                            File.WriteAllBytes(file,image);
                            foreach(string root in new[]{"Texture","OnDemandTexture"})
                            foreach(string format in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5"})
                            foreach(bool mips in new[]{false,true}) foreach(string kind in new[]{"StandardTexture","VolumeTexture"})
                            foreach(bool stale in new[]{false,true})
                                Case(original,recovered,work,platform+"/"+raster+"/c"+color+"d"+depth+"i"+interlace+"t"+metadata+"/"+root+"/"+format+"/"+mips+"/"+kind+"/"+stale,root,file,kind,format,stale,mips);
                        }
                    }
                }
            }finally{((IDisposable)original).Dispose();((IDisposable)recovered).Dispose();}
            Console.WriteLine("PASS Texture image ProcessInstance differential fixtures="+fixtures+" errors="+errors+" unsupportedRgb16="+unsupportedResampling+" comparedBytes="+compared+"; PC; PNG/BMP format/depth/interlace/orientation group; standard/volume and no-mip/mips; three buffers and full cdata; no SDK acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}


