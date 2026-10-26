// AssetMerge compatibility implementation. Derived from Jonathan Wilson's
// cc3tools assetmerge/assetmerge.cpp, copyright 2007 Jonathan Wilson;
// portions copyright 2007 booto. GPL-3.0-or-later.
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace NewModCompiler.Application
{
    // Component preparation only: not enabled by the production CLI.
    public static class AssetMerge
    {
        sealed class Asset
        {
            public byte[] Header,References,Name,Source,Bin,Imp,Relo;
            public long SourceIdentity;
        }
        public static int Apply(string stem,string directory,CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            string[] files=Directory.GetFiles(directory,"*.asset",SearchOption.TopDirectoryOnly);
            if(files.Length==0)return 0; // Original leaves all four files untouched.
            byte[] original=File.ReadAllBytes(stem+".manifest");
            if(original.Length<48 || original[0]!=0 || original[1]!=1 || original[2]!=6 || original[3]!=0)
                throw new InvalidDataException("AssetMerge requires a linked little-endian RA3 v6 manifest.");
            byte[] header=Slice(original,0,48);int count=checked((int)Read(header,12));
            long table=48L+48L*count,refsEnd=table+Read(header,32),streamsEnd=refsEnd+Read(header,36),namesEnd=streamsEnd+Read(header,40);
            if(namesEnd+Read(header,44)!=original.LongLength || Read(header,32)%8!=0)throw new InvalidDataException("Invalid AssetMerge manifest tables.");
            byte[] streamNames=Slice(original,checked((int)refsEnd),checked((int)Read(header,36)));
            for(int offset=0;offset<streamNames.Length;) {
                if(streamNames[offset]!=1 && streamNames[offset]!=2)throw new InvalidDataException("Unsupported referenced-manifest marker.");
                offset++;while(offset<streamNames.Length && streamNames[offset]!=0)offset++;
                if(offset++>=streamNames.Length)throw new InvalidDataException("Unterminated referenced-manifest name.");
            }
            byte[] binary=File.ReadAllBytes(stem+".bin"),imports=File.ReadAllBytes(stem+".imp"),relocations=File.ReadAllBytes(stem+".relo");
            int bo=4,io=4,ro=4;var current=new List<Asset>();var incoming=new List<Asset>();
            for(int i=0;i<count;i++) {
                cancellation.ThrowIfCancellationRequested();byte[] row=Slice(original,48+i*48,48);
                if((ulong)Read(row,16)+8UL*Read(row,20)>Read(header,32))throw new InvalidDataException("AssetMerge reference range exceeds buffer.");
                var asset=new Asset {Header=row,References=Slice(original,checked((int)table+(int)Read(row,16)),checked((int)Read(row,20)*8)),
                    Name=Terminated(original,checked((int)streamsEnd+(int)Read(row,24)),checked((int)namesEnd)),
                    Source=Terminated(original,checked((int)namesEnd+(int)Read(row,28)),original.Length),SourceIdentity=Read(row,28),
                    Bin=Take(binary,ref bo,Read(row,32)),Imp=Take(imports,ref io,Read(row,40)),Relo=Take(relocations,ref ro,Read(row,36))};
                current.Add(asset);
            }
            if(bo!=binary.Length || io!=imports.Length || ro!=relocations.Length)throw new InvalidDataException("AssetMerge linked payload lengths differ.");
            for(int i=0;i<files.Length;i++) {
                cancellation.ThrowIfCancellationRequested();RejectLink(files[i]);byte[] bytes=File.ReadAllBytes(files[i]);
                if(bytes.Length<40)throw new InvalidDataException("Truncated precompiled asset.");
                byte[] row=new byte[48];Buffer.BlockCopy(bytes,0,row,0,16);Write(row,20,Read(bytes,16));Write(row,32,Read(bytes,20));Write(row,36,Read(bytes,24));Write(row,40,Read(bytes,28));
                int offset=32;byte[] references=Take(bytes,ref offset,checked(Read(row,20)*8));
                byte[] name=LengthString(bytes,ref offset),source=LengthString(bytes,ref offset);
                var asset=new Asset {Header=row,References=references,Name=name,Source=source,SourceIdentity=-1L-i,
                    Bin=Take(bytes,ref offset,Read(row,32)),Relo=Take(bytes,ref offset,Read(row,36)),Imp=Take(bytes,ref offset,Read(row,40))};
                if(offset!=bytes.Length)throw new InvalidDataException("Trailing precompiled asset data is unsupported.");incoming.Add(asset);
            }
            // Original mutates existing abstract texture references only. Last
            // matching incoming texture wins; incoming references are unchanged.
            foreach(Asset asset in current)for(int offset=0;offset<asset.References.Length;offset+=8)if(Read(asset.References,offset)==0xb22f87c9U)
                foreach(Asset replacement in incoming)if(Read(asset.References,offset+4)==Read(replacement.Header,4) && Texture(Read(replacement.Header,0)))Write(asset.References,offset,Read(replacement.Header,0));
            // The original std::find compares pointers, not asset identities.
            // Every separately loaded .asset is prepended, even with equal IDs.
            var assets=new List<Asset>(incoming);assets.AddRange(current);
            Write(header,8,0x0a979693U);Write(header,12,checked((uint)assets.Count));Write(header,16,0);Write(header,32,0);Write(header,40,0);Write(header,44,0);
            var sourceIdentities=new HashSet<long>();
            foreach(Asset asset in assets) {
                Write(header,16,checked(Read(header,16)+(uint)asset.Bin.Length));Write(header,32,checked(Read(header,32)+(uint)asset.References.Length));Write(header,40,checked(Read(header,40)+(uint)asset.Name.Length+1));
                if(sourceIdentities.Add(asset.SourceIdentity))Write(header,44,checked(Read(header,44)+(uint)asset.Source.Length+1));
            }
            foreach(Asset asset in current) {
                Write(header,20,Math.Max(Read(header,20),Read(asset.Header,32)));
                Write(header,24,Math.Max(Read(header,24),Read(asset.Header,36)));
                Write(header,28,Math.Max(Read(header,28),Read(asset.Header,40)));
            }
            foreach(Asset asset in incoming) {
                uint size=Read(asset.Header,32);Write(header,20,Math.Max(Read(header,20),size));
                // Preserve original bug: incoming instance size drives all maxima.
                Write(header,24,Math.Max(Read(header,24),size));Write(header,28,Math.Max(Read(header,28),size));
            }
            byte[] manifestResult,binResult,impResult,reloResult;
            using(var tableOut=new MemoryStream())using(var references=new MemoryStream())using(var names=new MemoryStream())using(var sources=new MemoryStream())
            using(var bins=new MemoryStream())using(var imps=new MemoryStream())using(var relos=new MemoryStream()) {
                Append(bins,Slice(header,4,4));Append(imps,Slice(header,4,4));Append(relos,Slice(header,4,4));
                foreach(Asset asset in assets) {
                    cancellation.ThrowIfCancellationRequested();Write(asset.Header,16,checked((uint)references.Length));Append(references,asset.References);
                    Write(asset.Header,24,checked((uint)names.Length));Append(names,asset.Name);names.WriteByte(0);
                    int position=Find(sources.ToArray(),asset.Source);
                    if(position<0) {position=checked((int)sources.Length);Append(sources,asset.Source);sources.WriteByte(0);}
                    Write(asset.Header,28,checked((uint)position));Append(tableOut,asset.Header);
                    Append(bins,asset.Bin);Append(imps,asset.Imp);Append(relos,asset.Relo);
                }
                if(sources.Length!=Read(header,44))throw new InvalidDataException("Original AssetMerge leaves undefined source-name bytes for this duplicate/substring layout; merge rejected before modifying staging files.");
                using(var result=new MemoryStream()) {Append(result,header);Append(result,tableOut.ToArray());Append(result,references.ToArray());Append(result,streamNames);Append(result,names.ToArray());Append(result,sources.ToArray());manifestResult=result.ToArray();}
                binResult=bins.ToArray();impResult=imps.ToArray();reloResult=relos.ToArray();
            }
            // All parsing and refusal checks precede changes. Caller must supply
            // an isolated staging stream and discard it on any write failure.
            cancellation.ThrowIfCancellationRequested();
            string cdata=Path.Combine(stem,"cdata");Directory.CreateDirectory(cdata);
            foreach(string file in Directory.GetFiles(directory,"*.cdata",SearchOption.TopDirectoryOnly)) {cancellation.ThrowIfCancellationRequested();RejectLink(file);File.Copy(file,Path.Combine(cdata,Path.GetFileName(file)),true);}
            File.WriteAllBytes(stem+".manifest",manifestResult);File.WriteAllBytes(stem+".bin",binResult);File.WriteAllBytes(stem+".imp",impResult);File.WriteAllBytes(stem+".relo",reloResult);
            return incoming.Count;
        }
        static bool Texture(uint type) {return type==0xe3181c04U || type==0xf0f08712U || type==0xc2b1a262U;}
        static int Find(byte[] haystack,byte[] needle) {if(needle.Length==0)return -1;for(int i=0;i<=haystack.Length-needle.Length;i++){int j=0;while(j<needle.Length && haystack[i+j]==needle[j])j++;if(j==needle.Length)return i;}return -1;}
        static void RejectLink(string path) {if((File.GetAttributes(path)&FileAttributes.ReparsePoint)!=0)throw new IOException("Precompiled asset links are unsupported.");}
        static byte[] LengthString(byte[] bytes,ref int offset) {uint size=Read(bytes,offset);offset=checked(offset+4);byte[] data=Take(bytes,ref offset,size);int end=Array.IndexOf(data,(byte)0);return end<0?data:Slice(data,0,end);}
        static byte[] Terminated(byte[] bytes,int offset,int end) {if(offset<0 || offset>=end)throw new InvalidDataException("Invalid asset string offset.");int cursor=offset;while(cursor<end && bytes[cursor]!=0)cursor++;if(cursor>=end)throw new InvalidDataException("Unterminated asset string.");return Slice(bytes,offset,cursor-offset);}
        static byte[] Take(byte[] data,ref int offset,uint size) {byte[] result=Slice(data,offset,checked((int)size));offset=checked(offset+result.Length);return result;}
        static byte[] Slice(byte[] bytes,int offset,int size) {if(offset<0 || size<0 || (long)offset+size>bytes.LongLength)throw new InvalidDataException("Truncated AssetMerge input.");byte[] result=new byte[size];Buffer.BlockCopy(bytes,offset,result,0,size);return result;}
        static void Append(Stream target,byte[] bytes) {target.Write(bytes,0,bytes.Length);}
        static uint Read(byte[] bytes,int offset) {if(offset<0 || (long)offset+4>bytes.Length)throw new InvalidDataException("Truncated AssetMerge word.");return (uint)bytes[offset]|((uint)bytes[offset+1]<<8)|((uint)bytes[offset+2]<<16)|((uint)bytes[offset+3]<<24);}
        static void Write(byte[] bytes,int offset,uint value) {for(int i=0;i<4;i++)bytes[offset+i]=(byte)(value>>(8*i));}
    }
}
