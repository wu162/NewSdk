using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

internal static class AudioSdkSupport
{
    private const string Ns="uri:ea.com:eala:asset";
    private static void Wave(string file)
    {
        const int frames=4096,channels=2;
        using(var stream=File.Create(file))using(var b=new BinaryWriter(stream)){
            b.Write(Encoding.ASCII.GetBytes("RIFF"));b.Write(36+frames*channels*2);b.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
            b.Write(16);b.Write((short)1);b.Write((short)channels);b.Write(22050);b.Write(22050*channels*2);b.Write((short)(channels*2));b.Write((short)16);
            b.Write(Encoding.ASCII.GetBytes("data"));b.Write(frames*channels*2);short[] values={0,1,-1,32767,-32768,0x1234};
            for(int i=0;i<frames*channels;i++)b.Write(values[i%values.Length]);
        }
    }
    private static void Create(string sdk,string expected,string[] realFiles,bool aiffLoopsOnly,bool resamplingOnly,bool resamplingRegression)
    {
        string mod=Path.Combine(sdk,"Mods/AudioRecoveryProbe"),data=Path.Combine(mod,"Data"),audio=Path.Combine(mod,"Audio");
        Directory.CreateDirectory(data);Directory.CreateDirectory(audio);Directory.CreateDirectory(Path.Combine(audio,"sounds"));Directory.CreateDirectory(Path.Combine(audio,"music"));
        Wave(Path.Combine(audio,"probe.wav"));Wave(Path.Combine(audio,"sounds/default.wav"));Wave(Path.Combine(audio,"music/default.wav"));
        File.WriteAllBytes(Path.Combine(audio,"probe.mp3"),Convert.FromBase64String(AudioMp3Fixtures.Mono));
        File.WriteAllBytes(Path.Combine(audio,"map.mpf"),new byte[]{0,1,2,0xff,0xfe,0x55,0x80});
        File.WriteAllBytes(Path.Combine(audio,"track.mus"),new byte[]{0x50,0x41,0x54,0x48,0,0xff,0x34});
        File.WriteAllText(Path.Combine(audio,"headers.h"),"#define PATH_EVENT_AudioEvent 0x12345678\r\n#define PATH_TRACK_AudioTrack 0x0012D687\r\n");
        var xml=new StringBuilder("<AssetDeclaration xmlns='"+Ns+"'><Tags/><Includes><Include type='reference' source='DATA:static.xml'/><Include type='reference' source='DATA:global.xml'/><Include type='reference' source='DATA:audio.xml'/></Includes>");
        xml.Append("<PathMusicMap id='AudioMap' File='../Audio/map.mpf'/><PathMusicTrack id='AudioTrack' File='../Audio/track.mus' Type='MAIN' PathfinderTrackHeader='../Audio/headers.h'/><PathMusicEvent id='AudioEvent' PathfinderEventHeader='../Audio/headers.h' IsXferable='true'/>");
        xml.Append("<AudioFileMP3Passthrough id='AudioMp3' File='../Audio/probe.mp3' IsStreamed='true'/>");
        foreach(string codec in new string[]{"NONE","XAS"})foreach(bool streamed in new bool[]{false,true})
            xml.Append("<AudioFile id='Audio"+codec+streamed+"' File='../Audio/probe.wav' PCCompression='"+codec+"' IsStreamedOnPC='"+streamed.ToString().ToLowerInvariant()+"'/>");
        for(int index=0;index<realFiles.Length;index++) {
            string source=Path.GetFullPath(realFiles[index]);
            string extension=Path.GetExtension(source).ToLowerInvariant();
            if(extension!=".wav" && extension!=".mp3")throw new Exception("Unsupported real audio SDK fixture type: "+source);
            string target="real-"+index+extension;File.Copy(source,Path.Combine(audio,target),true);
            if(extension==".mp3")xml.Append("<AudioFileMP3Passthrough id='RealMp3"+index+"' File='../Audio/"+target+"' IsStreamed='true'/>");
            else foreach(string codec in new string[]{"NONE","XAS"})xml.Append("<AudioFile id='RealWave"+index+codec+"' File='../Audio/"+target+"' PCCompression='"+codec+"' IsStreamedOnPC='false'/>");
        }
        foreach(bool aiff in new bool[]{false,true}) {
            string name=aiff?"generated.aiff":"generated24.wav";
            AudioInputFormatCases.Write(Path.Combine(audio,name),aiff,aiff?16:24,2,2041);
            foreach(string codec in new string[]{"NONE","XAS"})foreach(bool streamed in new bool[]{false,true})
                xml.Append("<AudioFile id='Format"+(aiff?"Aiff":"Wave24")+codec+streamed+"' File='../Audio/"+name+"' PCCompression='"+codec+"' IsStreamedOnPC='"+streamed.ToString().ToLowerInvariant()+"'/>");
        }
        AudioLoopProbeCases.Wave(Path.Combine(audio,"loop.wav"),1,4096,129,4094);
        foreach(string codec in new string[]{"NONE","XAS"})foreach(bool streamed in new bool[]{false,true})
            xml.Append("<AudioFile id='Loop"+codec+streamed+"' File='../Audio/loop.wav' PCCompression='"+codec+"' IsStreamedOnPC='"+streamed.ToString().ToLowerInvariant()+"'/>");
        AudioInputFormatCases.WriteExtensible(Path.Combine(audio,"extensible.wav"),24,2,2041,true);
        foreach(string codec in new string[]{"NONE","XAS"})
            xml.Append("<AudioFile id='Extensible"+codec+"' File='../Audio/extensible.wav' PCCompression='"+codec+"' IsStreamedOnPC='false'/>");
        AudioAiffLoopCases.Write(Path.Combine(audio,"aiff-loop.aiff"),16,2,4096,129,4094,false);
        foreach(string codec in new string[]{"NONE","XAS"})foreach(bool streamed in new bool[]{false,true})
            xml.Append("<AudioFile id='AiffLoop"+codec+streamed+"' File='../Audio/aiff-loop.aiff' PCCompression='"+codec+"' IsStreamedOnPC='"+streamed.ToString().ToLowerInvariant()+"'/>");
        if(resamplingOnly || resamplingRegression) foreach(string element in AudioResampleCases.WriteSdkCases(audio)) xml.Append(element);
        xml.Append("<AudioFile id='AudioDefaultSounds' File='../Audio/sounds/default.wav'/><AudioFile id='AudioDefaultMusic' File='../Audio/music/default.wav'/></AssetDeclaration>");
        if(aiffLoopsOnly || resamplingOnly) {
            var selected=new XmlDocument(); selected.LoadXml(xml.ToString());
            var remove=new List<XmlNode>();
            foreach(XmlElement child in selected.DocumentElement.ChildNodes)
                if(child.HasAttribute("id") && !child.GetAttribute("id").StartsWith(resamplingOnly?"AudioResampleSdk":"AiffLoop",StringComparison.Ordinal))remove.Add(child);
            foreach(XmlNode child in remove)selected.DocumentElement.RemoveChild(child);
            xml.Length=0;xml.Append(selected.OuterXml);
        }
        string path=Path.Combine(data,"Mod.xml");File.WriteAllText(path,xml.ToString());
        var schemas=new XmlSchemaSet();schemas.XmlResolver=new XmlUrlResolver();schemas.Add(Ns,Path.Combine(sdk,"Schemas/xsd/CnC3Types.xsd"));schemas.Compile();
        var settings=new XmlReaderSettings();settings.Schemas=schemas;settings.ValidationType=ValidationType.Schema;
        settings.ValidationEventHandler+=delegate(object sender,ValidationEventArgs e){throw new Exception("Audio SDK schema: "+e.Message);};
        using(var reader=XmlReader.Create(path,settings))while(reader.Read()){}
        var document=new XmlDocument();document.Load(path);var ids=new List<string>();
        foreach(XmlElement child in document.DocumentElement.ChildNodes)if(child.HasAttribute("id"))ids.Add(child.LocalName+"\t"+child.GetAttribute("id"));
        ids.Sort(StringComparer.Ordinal);File.WriteAllLines(expected,ids.ToArray());
        if(ids.Count<(resamplingOnly?6:aiffLoopsOnly?4:10))throw new Exception("Missing expected audio SDK assets");
        Console.WriteLine("PASS Audio SDK input schema: "+ids.Count+" assets; "+realFiles.Length+" real input files");
    }
    private static void Compare(string first,string second,string expected)
    {
        string[] a=File.ReadAllLines(first),b=File.ReadAllLines(second);Array.Sort(a,StringComparer.Ordinal);Array.Sort(b,StringComparer.Ordinal);
        if(a.Length!=b.Length)throw new Exception("Actual SDK call count differs");
        for(int i=0;i<a.Length;i++)if(a[i]!=b[i])throw new Exception("Actual SDK three-buffer/custom-data call differs: "+a[i].Split('\t')[0]+"/"+a[i].Split('\t')[1]);
        var seen=new HashSet<string>();foreach(string line in a){string[] f=line.Split('\t');if(f.Length!=6)throw new Exception("Malformed actual SDK trace");seen.Add(f[0]+"\t"+f[1]);}
        foreach(string key in File.ReadAllLines(expected))if(!seen.Contains(key))throw new Exception("No actual SDK handler call for "+key);
        Console.WriteLine("PASS actual SDK plugin calls: "+a.Length+", three buffers and custom data identical, all expected assets covered");
    }
    private static int Main(string[] args)
    {
        try{if(args.Length>=3 && (args[0]=="create" || args[0]=="create-aiff-loops" || args[0]=="create-resample" || args[0]=="create-resample-regression")){string[] real=new string[args.Length-3];Array.Copy(args,3,real,0,real.Length);Create(args[1],args[2],real,args[0]=="create-aiff-loops",args[0]=="create-resample",args[0]=="create-resample-regression");return 0;}if(args.Length==4 && args[0]=="compare"){Compare(args[1],args[2],args[3]);return 0;}throw new Exception("Usage: AudioSdkSupport create sdk expected | compare originalTrace recoveredTrace expected");}
        catch(Exception ex){Console.Error.WriteLine(ex);return 1;}
    }
}
