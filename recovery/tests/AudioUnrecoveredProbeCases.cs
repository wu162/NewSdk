using System;
using System.IO;
using System.Text;

// Original-only evidence for the next complete codec/filter recovery batch.
// These records must never be counted as candidate support or acceptance.
internal static class AudioUnrecoveredProbeCases
{
    internal static void Run(string root, Action<string,int,string,bool> run, bool xma)
    {
        if(xma) {
            foreach(int channels in new int[]{1,2})foreach(int frames in new int[]{257,4096})foreach(bool streamed in new bool[]{false,true}) {
                string file=Path.Combine(root,"xma-original-"+channels+"-"+frames+".wav");
                Wave(file,channels,frames,22050,0);
                run("xma-evidence-"+channels+"-"+frames+"-"+streamed,1,
                    Element(file,"XenonCompression='XMA' XenonQuality='75' IsStreamedOnXenon='"+streamed.ToString().ToLowerInvariant()+"'"),false);
            }
            return;
        }
        int[,] cases={
            {22050,44100,129},{22050,11025,129},{22050,32000,129},{8000,22050,129},
            {22050,400,2},{22050,400,8193},{400,96000,1},{400,96000,2},
            {22050,44100,8191},{22050,44100,8192},{22050,44100,8193},{96000,400,129}
        };
        for(int c=0;c<cases.GetLength(0);c++)foreach(int channels in new int[]{1,2})
        foreach(int signal in (c==0?new int[]{0,1,2}:new int[]{c%3})) {
            int source=cases[c,0],target=cases[c,1],frames=cases[c,2];
            string name="resample-"+source+"-"+target+"-"+frames+"-"+channels+"-signal"+signal;
            string file=Path.Combine(root,name+".wav");Wave(file,channels,frames,source,signal);
            foreach(string codec in new string[]{"NONE","XAS"})foreach(bool streamed in new bool[]{false,true})
                run("pcm-"+name+"-"+codec+"-"+streamed,0,
                    Element(file,"PCCompression='"+codec+"' PCQuality='75' PCSampleRate='"+target+"' IsStreamedOnPC='"+streamed.ToString().ToLowerInvariant()+"'"),false);
        }
    }
    private static string Element(string file,string attrs)
    {return "<AudioFile xmlns='uri:ea.com:eala:asset' id='Fixture' File='"+System.Security.SecurityElement.Escape(file)+"' "+attrs+"/>";}
    private static void Wave(string path,int channels,int frames,int rate,int signal)
    {
        using(var file=File.Create(path))using(var b=new BinaryWriter(file)) {
            b.Write(Encoding.ASCII.GetBytes("RIFF"));b.Write(36+channels*frames*2);b.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
            b.Write(16);b.Write((short)1);b.Write((short)channels);b.Write(rate);b.Write(rate*channels*2);
            b.Write((short)(channels*2));b.Write((short)16);b.Write(Encoding.ASCII.GetBytes("data"));b.Write(channels*frames*2);
            for(int frame=0;frame<frames;frame++)for(int channel=0;channel<channels;channel++) {
                short value=signal==0?(short)(frame==0?24000:0):signal==1?(short)(channel==0?8192:-4096):
                    (short)(Math.Sin((frame+channel)*0.137)*24000);
                b.Write(value);
            }
        }
    }
}
