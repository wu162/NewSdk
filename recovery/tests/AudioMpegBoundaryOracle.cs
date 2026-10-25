using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

// Process-isolated Win32 MP3 acceptance probe. Full output buffers and unchanged
// input bytes are recorded; a decoder error is evidence, never a successful asset.
internal static class AudioMpegBoundaryOracle
{
    private static string Data(byte[] b) { return b == null ? "<null>" : Convert.ToBase64String(b); }
    private static readonly Dictionary<string, byte[]> cases = new Dictionary<string, byte[]>();
    private static byte[] Join(params byte[][] blocks) { return blocks.SelectMany(b => b).ToArray(); }
    private static void Add(string name, byte[] data) { cases.Add(name, data); }
    private static int FrameSize(byte[] b, int at)
    {
        int version=(b[at+1] >> 3) & 3;
        int[] bitrates = version==3 ? new int[] { 0,32,40,48,56,64,80,96,112,128,160,192,224,256,320 } : new int[] { 0,8,16,24,32,40,48,56,64,80,96,112,128,144,160 };
        if (at > b.Length - 4 || b[at] != 255 || (b[at+1] & 0xe6) != 0xe2) throw new Exception("Fixture is not MPEG Layer3");
        int rate=new int[] { 44100,48000,32000 }[(b[at+2] >> 2) & 3] / (version==3 ? 1 : version==2 ? 2 : 4);
        return (version==3 ? 144000 : 72000) * bitrates[b[at+2] >> 4] / rate + ((b[at+2] >> 1) & 1);
    }
    private static void CreateCases()
    {
        byte[] mono = Convert.FromBase64String(AudioMp3Fixtures.Mono), stereo = Convert.FromBase64String(AudioMp3Fixtures.Stereo);
        foreach (var pair in new[] { new KeyValuePair<string,byte[]>("mono",mono),new KeyValuePair<string,byte[]>("stereo",stereo) })
        {
            string name = pair.Key; byte[] data = pair.Value;
            var starts = new List<int>(); int pos = 0;
            while (pos <= data.Length - 4) { starts.Add(pos); pos += FrameSize(data,pos); }
            if (starts.Count < 7) throw new Exception("Need at least seven fixture frames");
            Add(name+"-valid",data);
            foreach (int prefix in new[] { 1,3,10,127,1024 }) Add(name+"-prefix-"+prefix,Join(Enumerable.Repeat((byte)0x55,prefix).ToArray(),data));
            foreach (int suffix in new[] { 1,3,4,127,128 }) Add(name+"-suffix-"+suffix,Join(data,Enumerable.Repeat((byte)0x55,suffix).ToArray()));
            Add(name+"-id3v2",Join(new byte[] { 73,68,51,4,0,0,0,0,0,0 },data));
            Add(name+"-id3v1",Join(data,Encoding.ASCII.GetBytes("TAG"),new byte[125]));
            // Invalid final headers can mutate channel metadata before rejecting
            // free-format bitrate0; verify that behavior through SIMEX_info.
            foreach (int mode in new[] {0,3}) foreach (string defect in new[] {"free","reserved-bitrate","reserved-rate","reserved-version"})
            {
                byte[] tail=data.Take(4).ToArray(); tail[3]=(byte)((tail[3]&0x3f)|(mode<<6));
                if (defect=="free") tail[2]=(byte)(tail[2]&15);
                else if (defect=="reserved-bitrate") tail[2]=(byte)(tail[2]|0xf0);
                else if (defect=="reserved-rate") tail[2]=(byte)(tail[2]|12);
                else tail[1]=(byte)((tail[1]&0xe7)|8);
                Add(name+"-tail-"+defect+"-mode"+mode,Join(data,tail));
            }

            foreach (int frames in new[] { 1,6,7,8 }) if (frames <= starts.Count)
            {
                int last = starts[frames-1], size = FrameSize(data,last);
                foreach (int payload in new[] { 0,1,size/2,size-4 })
                    Add(name+"-frames"+frames+"-lastpayload"+payload,data.Take(Math.Min(data.Length,last+4+payload)).ToArray());
            }
            foreach (int frame in new[] { 0,3,6,starts.Count-1 }.Distinct())
            {
                byte[] broken = (byte[])data.Clone(); broken[starts[frame]]=0;
                Add(name+"-broken-sync-"+frame,broken);
            }
            foreach (byte fill in new byte[] { 0,255 })
            {
                byte[] broken = (byte[])data.Clone();
                foreach (int start in starts) for (int i=start+4;i<Math.Min(data.Length,start+FrameSize(data,start));i++) broken[i]=fill;
                Add(name+"-payload-fill-"+fill,broken);
            }
            // SIMEX identification considers enclosing-container signatures first.
            foreach (string magic in new[] { "RIFF","FORM","OggS","fLaC","SCHl" }) Add(name+"-container-"+magic,Join(Encoding.ASCII.GetBytes(magic),data));
        }
        Add("mono-stereo",Join(mono,stereo)); Add("stereo-mono",Join(stereo,mono));
        Add("empty",new byte[0]); Add("junk",new byte[4096]);
    }
    private static InstanceDeclaration Declaration(string path, string custom, bool streamed)
    {
        var xml = new XmlDocument(); xml.LoadXml("<AudioFileMP3Passthrough xmlns='uri:ea.com:eala:asset' id='MpegBoundary' File='"+System.Security.SecurityElement.Escape(path)+"' SubtitleStringName='MpegBoundary' IsStreamed='"+streamed.ToString().ToLowerInvariant()+"'/>");
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea","uri:ea.com:eala:asset");
        var document = new AssetDeclarationDocument();
        Type current = typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(current); current.GetField("NamespaceManager").SetValue(state,ns);
        typeof(AssetDeclarationDocument).GetField("_Current",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(document,state);
        return new InstanceDeclaration(document) { XmlNode=xml.DocumentElement,CustomDataPath=custom };
    }
    private static int Main(string[] args)
    {
        try {
            if ((args.Length != 4 && args.Length != 5) || IntPtr.Size != 4) throw new ArgumentException("Usage (x86): AudioMpegBoundaryOracle assembly fixtures output original|recovered");
            string root=Path.GetFullPath(args[1]); Directory.CreateDirectory(root); CreateCases();
            if (args[3]=="list") {
                foreach (var pair in cases) File.WriteAllBytes(Path.Combine(root,pair.Key+".mp3"),pair.Value);
                File.WriteAllLines(args[2],cases.Keys); return 0;
            }
            const string originalName="BinaryAssetBuilder.AudioCompiler";
            bool recovered=args[3]=="recovered";
            if (recovered && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,originalName+".dll"))) throw new Exception("Original DLL present in recovered host");
            Assembly library=Assembly.LoadFrom(Path.GetFullPath(args[0]));
            var plugin=(IAssetBuilderPlugin)Activator.CreateInstance(library.GetType(originalName+".Plugin",true));
            int records=0, accepted=0;
            var selected=cases.Where(p=>args.Length==4 || p.Key==args[4]).ToArray();
            if (selected.Length==0) throw new Exception("Unknown fixture");
            using (var output=new StreamWriter(args[2],false,new UTF8Encoding(false))) foreach (var pair in selected)
            {
                string path=Path.Combine(root,pair.Key+".mp3"); File.WriteAllBytes(path,pair.Value);
                foreach (bool direct in new[] { false,true }) foreach (bool streamed in new[] { false,true })
                {
                    plugin.Initialize(null,TargetPlatform.Win32);
                    string custom=Path.Combine(root,"mpeg-custom.bin"); if (File.Exists(custom)) File.Delete(custom);
                    string result;
                    try {
                        var declaration=Declaration(path,custom,streamed);
                        AssetBuffer buffer=direct ? (AssetBuffer)plugin.GetType().GetMethod("ProcessAudioFileMP3PassthroughInstance").Invoke(plugin,new object[] { declaration }) : plugin.ProcessInstance(declaration);
                        result=buffer==null ? "NULL" : "BUFFER:"+Data(buffer.InstanceData)+":"+Data(buffer.RelocationData)+":"+Data(buffer.ImportsData);
                        if (buffer!=null) {
                            if (!File.Exists(custom) || !pair.Value.SequenceEqual(File.ReadAllBytes(custom))) throw new Exception("Passthrough did not copy input exactly");
                            accepted++;
                        }
                        if (pair.Key.EndsWith("-valid") && buffer==null) throw new Exception("Valid fixture rejected");
                    } catch (Exception error) {
                        while (error is TargetInvocationException && error.InnerException!=null) error=error.InnerException;
                        if (error.Message=="Passthrough did not copy input exactly" || error.Message=="Valid fixture rejected") throw;
                        result="ERROR:"+error.GetType().FullName+":"+Convert.ToBase64String(Encoding.UTF8.GetBytes(error.Message));
                    }
                    output.WriteLine(pair.Key+":"+direct+":"+streamed+":"+result+":CUSTOM:"+(File.Exists(custom)?Data(File.ReadAllBytes(custom)):"<absent>"));
                    output.Flush(); records++;
                }
            }
            ((IDisposable)plugin).Dispose();
            if (recovered && AppDomain.CurrentDomain.GetAssemblies().Any(a=>a.GetName().Name==originalName)) throw new Exception("Original DLL loaded in recovered host");
            if (records!=selected.Length*4) throw new Exception("Missing MPEG corpus records");
            Console.WriteLine("PASS MPEG corpus: "+selected.Length+" fixtures; "+records+" full records; "+accepted+" accepted"); return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
