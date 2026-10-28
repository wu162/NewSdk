using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered.Compiler.Texture;

internal static class TextureAtlasEntryReplay
{
    static int fixtures, decoded, rejected, checks; static long compared;
    static void Check(bool ok,string label){checks++;if(!ok)throw new Exception(label);}
    static string Hash(byte[] b){if(b==null)return "null";using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(b)).Replace("-","").ToLowerInvariant();}
    static string Error(Exception e){if(e==null)return "";var code=e.GetType().GetProperty("ErrorCode");return (e.GetType().FullName+"/"+(code==null?"":Convert.ToString(code.GetValue(e,null)))+"/"+e.Message).Replace('\t',' ').Replace('\r',' ').Replace('\n',' ');}
    static void Absent(){Check(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"BinaryAssetBuilder.TextureCompiler.dll")),"Original DLL present");foreach(var a in AppDomain.CurrentDomain.GetAssemblies())Check(a.GetName().Name!="BinaryAssetBuilder.TextureCompiler","Original assembly loaded");}
    static InstanceDeclaration Declaration(XmlDocument xml,string cdata){var manager=new XmlNamespaceManager(xml.NameTable);manager.AddNamespace("ea","uri:ea.com:eala:asset");var doc=new AssetDeclarationDocument();Type type=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);object state=FormatterServices.GetUninitializedObject(type);type.GetField("NamespaceManager").SetValue(state,manager);typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(doc,state);var d=new InstanceDeclaration(doc);d.XmlNode=xml.DocumentElement;d.CustomDataPath=cdata;d.Handle.TypeHash=0x12345678u;d.Handle.InstanceHash=0x9abcdef0u;return d;}
    static void Same(string directory,string label,string name,byte[] actual,bool empty){if(empty){Check(actual==null,label+" null "+name);return;}byte[] expected=File.ReadAllBytes(Path.Combine(directory,label+"."+name+".bin"));Check(actual!=null&&actual.Length==expected.Length,label+" "+name+" length");for(int i=0;i<expected.Length;i++)if(expected[i]!=actual[i])throw new Exception(label+" "+name+" mismatch at "+i+" expected="+expected[i]+" actual="+actual[i]);checks++;compared+=expected.Length;}
    static int Main(string[] args){string previous=Directory.GetCurrentDirectory();try{Check(IntPtr.Size==4,"x86");using(var plugin=new Plugin()){plugin.Initialize(null,TargetPlatform.Win32);foreach(string argument in args){string dir=Path.GetFullPath(argument);Directory.SetCurrentDirectory(dir);string[] sourceRows=File.ReadAllLines("input-files.tsv");foreach(string line in sourceRows){if(line.StartsWith("file\t"))continue;var row=line.Split('\t');Check(Hash(File.ReadAllBytes(row[0]))==row[2],"Source hash before "+row[0]);}
      foreach(string line in File.ReadAllLines("transcript.tsv")){if(line.StartsWith("label\t"))continue;var row=line.Split('\t');string label=row[0];var xml=new XmlDocument();xml.Load(label+".input.xml");string originalXml=xml.OuterXml;string cdata=Path.Combine(dir,"recovered.cdata");byte[] stale=new byte[8192];for(int i=0;i<stale.Length;i++)stale[i]=(byte)(i*13+7);File.WriteAllBytes(cdata,stale);AssetBuffer actual=null;Exception error=null;Absent();try{actual=plugin.ProcessInstance(Declaration(xml,cdata));}catch(Exception e){error=e;}Absent();Check(Error(error)==row[9],label+" error expected="+row[9]+" actual="+Error(error));Check(xml.OuterXml==originalXml,label+" XML mutation");Check(Hash(File.ReadAllBytes(cdata))==Hash(stale),label+" cdata mutation");
        if(row[9]==""){Check(actual!=null,label+" result");bool empty=row[5]=="-1";Same(dir,label,"instance",actual.InstanceData,empty);Same(dir,label,"relo",actual.RelocationData,empty);Same(dir,label,"imports",actual.ImportsData,empty);decoded++;}else{Check(actual==null,label+" failure result");rejected++;}fixtures++;Console.WriteLine("CHECK "+label);}
      foreach(string line in sourceRows){if(line.StartsWith("file\t"))continue;var row=line.Split('\t');Check(Hash(File.ReadAllBytes(row[0]))==row[2],"Source hash after "+row[0]);}Directory.SetCurrentDirectory(previous);
    }}Check(fixtures==48&&decoded==40&&rejected==8,"Full immutable and additional atlas corpora");Console.WriteLine("PASS Texture PC atlas entry fixtures="+fixtures+" decoded="+decoded+" rejected="+rejected+" checks="+checks+" comparedBytes="+compared+"; full instance/relo/imports, exact exceptions, XML/cdata/source invariance; per-call original absent and not loaded");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{Directory.SetCurrentDirectory(previous);}}
}
