using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;

internal static class TextureImagePipelineOracle
{
    static int fixtures, errors, unsupportedResampling; static long compared;
    static InstanceDeclaration Declaration(string root,string file,string kind,string format,string cdata)
    {
        var xml=new XmlDocument(); var node=xml.CreateElement(root,"uri:ea.com:eala:asset");xml.AppendChild(node);
        node.SetAttribute("id","ImagePipeline");node.SetAttribute("File",file);node.SetAttribute("Type",kind);
        node.SetAttribute("OutputFormat",format);node.SetAttribute("GenerateMipMaps","false");node.SetAttribute("AllowAutomaticResize","false");
        var manager=new XmlNamespaceManager(xml.NameTable);manager.AddNamespace("ea","uri:ea.com:eala:asset");
        var doc=new AssetDeclarationDocument();Type t=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(t);t.GetField("NamespaceManager").SetValue(state,manager);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);
        var d=new InstanceDeclaration(doc);d.XmlNode=node;d.CustomDataPath=cdata;
        d.Handle.TypeHash=0x12345678u;d.Handle.InstanceHash=0x9abcdef0u;return d;
    }
    static byte[] Tga(int width,int height,int alpha)
    {
        byte[] b=new byte[18+width*height*4];b[2]=2;b[12]=(byte)width;b[13]=(byte)(width>>8);b[14]=(byte)height;b[15]=(byte)(height>>8);b[16]=32;b[17]=40;
        for(int i=0;i<width*height;i++){int at=18+i*4;b[at]=(byte)(i*31);b[at+1]=(byte)(i*43+11);b[at+2]=(byte)(i*7+171);b[at+3]=alpha==0?(byte)0:alpha==1?(byte)255:(byte)(i*17);}
        return b;
    }
    static void Equal(byte[] a,byte[] b,string label)
    {
        if(a==null||b==null){if(a!=b)throw new Exception(label+" null differs");return;}
        if(a.Length!=b.Length)throw new Exception(label+" length original="+a.Length+" recovered="+b.Length);
        for(int i=0;i<a.Length;i++)if(a[i]!=b[i])throw new Exception(label+" byte="+i+" original="+a[i]+" recovered="+b[i]);
        compared+=a.Length;
    }
    static string Error(Exception e){if(e==null)return null;var b=e as BinaryAssetBuilderException;return e.GetType().FullName+"/"+(b==null?"":b.ErrorCode.ToString())+"/"+e.Message;}
    static void Case(IAssetBuilderPlugin original,IAssetBuilderPlugin recovered,string work,string label,string root,string file,string kind,string format,bool stale)
    {
        string oa=Path.Combine(work,"original.cdata"),ra=Path.Combine(work,"recovered.cdata");
        foreach(string p in new string[]{oa,ra}){if(File.Exists(p))File.Delete(p);if(stale){byte[] old=new byte[4096];for(int i=0;i<old.Length;i++)old[i]=(byte)(i*13);File.WriteAllBytes(p,old);}}
        AssetBuffer a=null,b=null;Exception ea=null,eb=null;
        try{a=original.ProcessInstance(Declaration(root,file,kind,format,oa));}catch(Exception e){ea=e;}
        try{b=recovered.ProcessInstance(Declaration(root,file,kind,format,ra));}catch(Exception e){eb=e;}
        if(label.EndsWith("/3"))
        {
            if(!(eb is NotSupportedException) || eb.Message!="Original D3DX power-of-two image resampling has not been recovered.")
                throw new Exception(label+" NPOT resampling must be explicitly rejected rather than emitting different pixels");
            unsupportedResampling++; fixtures++; return;
        }
        if(Error(ea)!=Error(eb))throw new Exception(label+" errors original="+Error(ea)+" recovered="+Error(eb));
        if(ea!=null)errors++;
        if(a!=null||b!=null){if(a==null||b==null)throw new Exception(label+" buffer presence");Equal(a.InstanceData,b.InstanceData,label+" instance");Equal(a.RelocationData,b.RelocationData,label+" relo");Equal(a.ImportsData,b.ImportsData,label+" imports");}
        if(File.Exists(oa)!=File.Exists(ra))throw new Exception(label+" cdata presence");
        if(File.Exists(oa))Equal(File.ReadAllBytes(oa),File.ReadAllBytes(ra),label+" cdata");fixtures++;
    }
    static int Main(string[] args)
    {
        try{
            if(args.Length!=4||IntPtr.Size!=4)throw new Exception("Usage (x86): TextureImagePipelineOracle original.dll recovered.dll realFixtureDirectory workDirectory");
            string od=Path.GetFullPath(args[0]),rd=Path.GetFullPath(args[1]),work=Path.GetFullPath(args[3]);Directory.CreateDirectory(work);
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string n=new AssemblyName(e.Name).Name+".dll";foreach(string dir in new[]{Path.GetDirectoryName(od),Path.GetDirectoryName(rd)}){string p=Path.Combine(dir,n);if(File.Exists(p))return Assembly.LoadFrom(p);}return null;};
            var original=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(od).GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true));
            var recovered=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(rd).GetType("Recovered.Compiler.Texture.Plugin",true));
            original.Initialize(null,TargetPlatform.Win32);
            try{
                foreach(int platform in new[]{0,2,99}){
                    original.ReInitialize(null,(TargetPlatform)platform);recovered.Initialize(null,(TargetPlatform)platform);
                    foreach(string root in new[]{"Texture","OnDemandTexture"})
                    foreach(string format in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5"})
                    foreach(int alpha in new[]{0,1,2})
                    foreach(int size in new[]{1,2,4,8}){
                        string file=Path.Combine(work,"generated.tga");File.WriteAllBytes(file,Tga(size,size==1?1:size/2,alpha));
                        foreach(bool stale in new[]{false,true})Case(original,recovered,work,platform+"/"+root+"/"+format+"/"+alpha+"/"+size+"/"+stale,root,file,"StandardTexture",format,stale);
                    }
                    foreach(string file in Directory.GetFiles(args[2],"*.tga"))foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string format in new[]{"A8R8G8B8","X8R8G8B8","R5G6B5"})
                        Case(original,recovered,work,platform+"/real/"+Path.GetFileName(file)+"/"+root+"/"+format,root,file,"StandardTexture",format,false);
                    foreach(string root in new[]{"Texture","OnDemandTexture"})foreach(string kind in new[]{"StandardTexture","CubeTexture","Unknown"})foreach(int width in new[]{3,4}){
                        string file=Path.Combine(work,"edge.tga");File.WriteAllBytes(file,Tga(width,2,2));Case(original,recovered,work,platform+"/edge/"+root+"/"+kind+"/"+width,root,file,kind,"A8R8G8B8",false);
                    }
                }
            }finally{((IDisposable)original).Dispose();((IDisposable)recovered).Dispose();}
            Console.WriteLine("PASS Texture image ProcessInstance differential fixtures="+fixtures+" errors="+errors+" unsupportedResampling="+unsupportedResampling+" comparedBytes="+compared+"; PC/PS3/unknown; real main TGA; three buffers and full cdata; no SDK acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
