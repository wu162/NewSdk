// Test-only transport. The original CLR2 plugin lives in a separate process;
// no original business assembly is loaded by the product validation process.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

static class XmlOracleWire {
    internal static string Hash(byte[] data) { using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(data)).Replace("-","").ToLowerInvariant(); }
    // Preserve UTF-16 code units, including embedded NULs and surrogate pairs.
    internal static void Text(BinaryWriter w,string s) { w.Write(s.Length);foreach(char c in s)w.Write((ushort)c); }
    internal static string Text(BinaryReader r) { int n=r.ReadInt32();if(n<0||n>16000000)throw new InvalidDataException("String length");char[] s=new char[n];for(int i=0;i<n;i++)s[i]=(char)r.ReadUInt16();return new string(s); }
    internal static void Node(BinaryWriter w,XmlNode n) {
        w.Write((int)n.NodeType);Text(w,n.Prefix);Text(w,n.LocalName);Text(w,n.NamespaceURI);Text(w,n.Value??"");
        w.Write(n.Attributes==null?0:n.Attributes.Count);
        if(n.Attributes!=null)foreach(XmlAttribute a in n.Attributes)Node(w,a);
        w.Write(n.NodeType==XmlNodeType.Attribute?0:n.ChildNodes.Count);
        if(n.NodeType!=XmlNodeType.Attribute)foreach(XmlNode c in n.ChildNodes)Node(w,c);
    }
    internal static XmlNode Node(BinaryReader r,XmlDocument d) {
        XmlNodeType kind=(XmlNodeType)r.ReadInt32();string prefix=Text(r),name=Text(r),ns=Text(r),value=Text(r);
        XmlNode n=d.CreateNode(kind,prefix,name,ns);if(kind!=XmlNodeType.Element)n.Value=value;
        int attrs=r.ReadInt32();if(attrs<0||attrs>100000)throw new InvalidDataException("Attributes");
        for(int i=0;i<attrs;i++)n.Attributes.Append((XmlAttribute)Node(r,d));
        int children=r.ReadInt32();if(children<0||children>1000000)throw new InvalidDataException("Children");
        for(int i=0;i<children;i++)n.AppendChild(Node(r,d));return n;
    }
    internal static byte[] Bytes(BinaryReader r) { int n=r.ReadInt32();if(n<0||n>256000000)throw new InvalidDataException("Buffer length");byte[] b=r.ReadBytes(n);if(b.Length!=n)throw new EndOfStreamException();return b; }
    internal static void Bytes(BinaryWriter w,byte[] b) { if(b==null)throw new InvalidDataException("Null buffer");w.Write(b.Length);w.Write(b); }
    internal static void Buffer(BinaryWriter w,AssetBuffer b) {
        w.Write((byte)(b==null?0:1));if(b==null)return;
        Bytes(w,b.InstanceData);Bytes(w,b.RelocationData);Bytes(w,b.ImportsData);
    }
    internal static AssetBuffer Buffer(BinaryReader r) {
        byte tag=r.ReadByte();if(tag>1)throw new InvalidDataException("AssetBuffer response tag");
        return tag==0?null:new AssetBuffer{InstanceData=Bytes(r),RelocationData=Bytes(r),ImportsData=Bytes(r)};
    }
    internal static void SelfTest() {
        var d=new XmlDocument();d.LoadXml("<ea:Root xmlns:ea='uri:ea.com:eala:asset' xmlns:q='urn:q'><q:Child/><!--comment--><![CDATA[data]]> text </ea:Root>");
        d.DocumentElement.SetAttribute("Value","prefix\0资源\ud83d\ude00suffix");
        byte[] first;using(var m=new MemoryStream()){var w=new BinaryWriter(m);Node(w,d.DocumentElement);w.Flush();first=m.ToArray();}
        var clone=new XmlDocument();using(var r=new BinaryReader(new MemoryStream(first)))clone.AppendChild(Node(r,clone));
        using(var m=new MemoryStream()){var w=new BinaryWriter(m);Node(w,clone.DocumentElement);w.Flush();if(Hash(m.ToArray())!=Hash(first))throw new Exception("XML oracle transport self-test round trip");}
        if(clone.DocumentElement.GetAttribute("Value")!=d.DocumentElement.GetAttribute("Value"))throw new Exception("XML oracle transport UTF-16 self-test");
        // Unknown original TypeIds intentionally return null. Preserve that
        // outcome so the existing prefix-dispatch rejection assertions run.
        using(var m=new MemoryStream()) {
            var w=new BinaryWriter(m);Buffer(w,null);Buffer(w,new AssetBuffer{InstanceData=new byte[]{1,2},RelocationData=new byte[0],ImportsData=new byte[]{3}});w.Flush();m.Position=0;
            var r=new BinaryReader(m);if(Buffer(r)!=null)throw new Exception("Null original result changed");
            AssetBuffer b=Buffer(r);if(b==null||b.InstanceData.Length!=2||b.InstanceData[1]!=2||b.RelocationData.Length!=0||b.ImportsData[0]!=3||m.Position!=m.Length)throw new Exception("AssetBuffer framing changed");
        }
        try { Buffer(new BinaryReader(new MemoryStream(new byte[]{2})));throw new Exception("Invalid result tag accepted"); }
        catch(InvalidDataException) { }
    }
    internal static InstanceDeclaration Declaration(XmlNode node) {
        XmlDocument d=node.OwnerDocument;var ns=new XmlNamespaceManager(d.NameTable);ns.AddNamespace("ea","uri:ea.com:eala:asset");
        var document=new AssetDeclarationDocument();Type t=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state=FormatterServices.GetUninitializedObject(t);t.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        var declaration=new InstanceDeclaration(document);declaration.XmlNode=node;return declaration;
    }
}

#if XML_ORIGINAL_SERVER
static class XmlProductOriginalServer {
    static int Main(string[] args) {
        try {
            XmlOracleWire.SelfTest();
            CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("de-DE");
            if(XmlOracleWire.Hash(File.ReadAllBytes(args[1]))!="d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6")throw new Exception("Original XML fingerprint");
            var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(Assembly.LoadFrom(args[1]).GetType("BinaryAssetBuilder.XmlCompiler.Plugin",true));
            using(var pipe=new NamedPipeServerStream(args[0],PipeDirection.InOut,1,PipeTransmissionMode.Byte,PipeOptions.None)) {
                pipe.WaitForConnection();var r=new BinaryReader(pipe);var w=new BinaryWriter(pipe);int sequence=0,initializedPlatform=-1;
                using(var log=new StreamWriter(args[2],false,new UTF8Encoding(false))) {
                    while(true) {
                        int op=r.ReadInt32();if(op==0){w.Write(sequence);w.Flush();log.WriteLine("END\t"+sequence);return 0;}
                        if(op==3){string type=XmlOracleWire.Text(r);Type t=plugin.GetType().Assembly.GetType(type);w.Write(t==null?-1:Marshal.SizeOf(t));w.Flush();continue;}
                        if(op==2){initializedPlatform=r.ReadInt32();plugin.Initialize(null,(TargetPlatform)initializedPlatform);continue;}
                        if(op!=1||r.ReadInt32()!=++sequence)throw new InvalidDataException("Request sequence");
                        byte[] payload=XmlOracleWire.Bytes(r);string hash=XmlOracleWire.Hash(payload);
                        using(var data=new BinaryReader(new MemoryStream(payload))) {
                            int platform=data.ReadInt32();if(platform!=initializedPlatform||(platform!=0&&platform!=1&&platform!=2&&platform!=99))throw new InvalidDataException("Platform");
                            string typeName=XmlOracleWire.Text(data);uint typeId=data.ReadUInt32(),instanceId=data.ReadUInt32();
                            var d=new XmlDocument();XmlNode node=XmlOracleWire.Node(data,d);d.AppendChild(node);
                            if(data.BaseStream.Position!=data.BaseStream.Length)throw new InvalidDataException("Trailing DOM payload");
                            var declaration=XmlOracleWire.Declaration(node);declaration.Handle.TypeName=typeName;
                            if(declaration.Handle.TypeId!=typeId||declaration.Handle.InstanceId!=instanceId)throw new InvalidDataException("Declaration identity changed");
                            // Require a lossless DOM round trip before invoking the oracle.
                            using(var check=new MemoryStream()) {var cw=new BinaryWriter(check);cw.Write(platform);XmlOracleWire.Text(cw,typeName);cw.Write(typeId);cw.Write(instanceId);XmlOracleWire.Node(cw,node);cw.Flush();if(XmlOracleWire.Hash(check.ToArray())!=hash)throw new InvalidDataException("DOM round trip changed");}
                            AssetBuffer b=plugin.ProcessInstance(declaration);
                            w.Write(sequence);XmlOracleWire.Text(w,hash);XmlOracleWire.Buffer(w,b);w.Flush();
                            string nil=XmlOracleWire.Hash(Encoding.UTF8.GetBytes("<null AssetBuffer>"));
                            log.WriteLine(sequence+"\t"+platform+"\t"+hash+"\t"+(b==null?nil:XmlOracleWire.Hash(b.InstanceData))+"\t"+(b==null?nil:XmlOracleWire.Hash(b.RelocationData))+"\t"+(b==null?nil:XmlOracleWire.Hash(b.ImportsData)));
                        }
                    }
                }
            }
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
#else
sealed class XmlOriginalClient {
    static XmlOriginalClient current;
    NamedPipeClientStream pipe;BinaryReader reader;BinaryWriter writer;int platform,sequence;
    public XmlOriginalClient() {
        XmlOracleWire.SelfTest();
        if(current!=null)throw new Exception("Duplicate XML oracle client");
        Assembly own=Assembly.GetEntryAssembly();if(own.GetName().Name!="NewModCompiler"||typeof(AssetBuffer).Assembly!=own)throw new Exception("XML matrix must run in EXE-owned Core context");
        pipe=new NamedPipeClientStream(".",Environment.GetEnvironmentVariable("XML_ORIGINAL_PIPE"),PipeDirection.InOut);pipe.Connect(60000);reader=new BinaryReader(pipe);writer=new BinaryWriter(pipe);current=this;
    }
    public void Initialize(object ignored,TargetPlatform p) { platform=(int)p;writer.Write(2);writer.Write(platform);writer.Flush(); }
    public AssetBuffer ProcessInstance(InstanceDeclaration declaration) {
        byte[] payload;using(var data=new MemoryStream()) {var w=new BinaryWriter(data);w.Write(platform);XmlOracleWire.Text(w,declaration.Handle.TypeName);w.Write(declaration.Handle.TypeId);w.Write(declaration.Handle.InstanceId);XmlOracleWire.Node(w,declaration.XmlNode);w.Flush();payload=data.ToArray();}
        writer.Write(1);writer.Write(++sequence);XmlOracleWire.Bytes(writer,payload);writer.Flush();
        if(reader.ReadInt32()!=sequence||XmlOracleWire.Text(reader)!=XmlOracleWire.Hash(payload))throw new InvalidDataException("Oracle response request identity");
        return XmlOracleWire.Buffer(reader);
    }
    public static int OriginalSize(string type) { current.writer.Write(3);XmlOracleWire.Text(current.writer,type);current.writer.Flush();return current.reader.ReadInt32(); }
    public static void Finish() {
        if(current==null||current.sequence==0)throw new Exception("No original XML calls");
        current.writer.Write(0);current.writer.Flush();if(current.reader.ReadInt32()!=current.sequence)throw new Exception("Oracle terminal sequence");current.pipe.Dispose();
        Console.WriteLine("ORACLE-END calls="+current.sequence);current=null;
    }
}
#endif
