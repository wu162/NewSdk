using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace NewModCompiler.Application
{
    public sealed class BigArchiveEntry
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
    }

    // Uncompressed SAGE BIG archive. Callers supply the exact current build
    // inventory; this writer never discovers files or invokes MakeBig.
    public static class BigArchiveWriter
    {
        sealed class Input { public string Name, Path; public long Length; public byte[] Bytes; }

        public static void Write(string path, IEnumerable<BigArchiveEntry> entries, CancellationToken cancellation = default(CancellationToken))
        {
            if (entries == null) throw new ArgumentNullException("entries");
            string output=Output(path); cancellation.ThrowIfCancellationRequested();
            var inputs=new List<Input>();var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            long indexLength=24, payloadLength=0;
            foreach(var entry in entries)
            {
                cancellation.ThrowIfCancellationRequested();
                if(entry==null) throw new ArgumentException("A BIG entry is null.");
                string name=NormalizeName(entry.Name);
                if(!names.Add(name)) throw new InvalidDataException("Duplicate BIG entry (case insensitive): "+name);
                if(string.IsNullOrEmpty(entry.SourcePath) || !Path.IsPathRooted(entry.SourcePath)) throw new ArgumentException("A BIG source must be an absolute file path.");
                string source=Path.GetFullPath(entry.SourcePath);RejectLinks(source);
                if(string.Equals(output,source,StringComparison.OrdinalIgnoreCase)) throw new IOException("A BIG archive cannot overwrite its source.");
                var file=new FileInfo(source);
                if(!file.Exists) throw new FileNotFoundException("A listed BIG input is missing.",source);
                if(file.Length>uint.MaxValue) throw new IOException("A BIG entry exceeds its 32-bit size field.");
                byte[] bytes=Encoding.ASCII.GetBytes(name);
                indexLength=checked(indexLength+8L+bytes.Length+1L);payloadLength=checked(payloadLength+file.Length);
                CheckedArchiveSize(indexLength,payloadLength);
                inputs.Add(new Input {Name=name,Path=source,Length=file.Length,Bytes=bytes});
            }
            if(inputs.Count==0) throw new InvalidDataException("An empty BIG archive is not a published package.");
            inputs.Sort((a,b)=>StringComparer.Ordinal.Compare(a.Name,b.Name));
            uint total=CheckedArchiveSize(indexLength,payloadLength);
            Atomic(output,cancellation,stream=>{
                using(var writer=new BinaryWriter(stream,Encoding.ASCII,true))
                {
                    writer.Write(Encoding.ASCII.GetBytes("BIG4"));
                    writer.Write(total); // BIG archive total size is little endian.
                    BigEndian(writer,(uint)inputs.Count);BigEndian(writer,(uint)indexLength);
                    long offset=indexLength;
                    foreach(var input in inputs)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        BigEndian(writer,(uint)offset);BigEndian(writer,(uint)input.Length);writer.Write(input.Bytes);writer.Write((byte)0);
                        offset+=input.Length;
                    }
                    writer.Write(Encoding.ASCII.GetBytes("L219"));writer.Write(0u);
                    var buffer=new byte[64*1024];
                    foreach(var input in inputs)
                    {
                        cancellation.ThrowIfCancellationRequested();RejectLinks(input.Path);
                        using(var source=new FileStream(input.Path,FileMode.Open,FileAccess.Read,FileShare.Read))
                        {
                            if(source.Length!=input.Length) throw new IOException("A BIG input changed length before it was copied: "+input.Name);
                            long remaining=input.Length;
                            while(remaining!=0)
                            {
                                cancellation.ThrowIfCancellationRequested();int read=source.Read(buffer,0,(int)Math.Min(buffer.Length,remaining));
                                if(read==0) throw new EndOfStreamException("A BIG input ended before its listed size.");
                                writer.Write(buffer,0,read);remaining-=read;
                            }
                            if(source.ReadByte()!=-1) throw new IOException("A BIG input grew while it was copied.");
                        }
                    }
                    writer.Flush();if(stream.Position!=total) throw new IOException("BIG archive length differs from its header.");
                }
            });
        }

        public static void WriteSkudef(string path, string bigFileName, CancellationToken cancellation = default(CancellationToken))
        {
            string name=NormalizeName(bigFileName,false);
            if(name.IndexOf('\\')>=0 || !name.EndsWith(".big",StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("SKUDEF requires an ASCII BIG file name, without directories.");
            byte[] text=Encoding.ASCII.GetBytes("mod-game 1.12\r\nadd-big "+name+"\r\n");
            Atomic(Output(path),cancellation,stream=>stream.Write(text,0,text.Length));
        }

        internal static uint CheckedArchiveSize(long indexLength,long payloadLength)
        {
            if(indexLength<16 || payloadLength<0 || indexLength>uint.MaxValue || payloadLength>uint.MaxValue || (ulong)indexLength+(ulong)payloadLength>uint.MaxValue)
                throw new IOException("A BIG archive exceeds its 32-bit index, offset or total-size fields.");
            return (uint)(indexLength+payloadLength);
        }
        static string NormalizeName(string name,bool lowercase=true)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentException("A BIG entry requires a relative ASCII name.");
            string result=name.Replace('/','\\');
            foreach(char c in result)
                if(c<32 || c>126 || ":*?\"<>|".IndexOf(c)>=0) throw new ArgumentException("BIG entry names must be ASCII without control characters or wildcard/device syntax.");
            foreach(string part in result.Split('\\'))
                if(part.Length==0 || part=="." || part==".." || part.Trim()!=part) throw new ArgumentException("BIG entry names cannot be rooted, empty, traversal paths or ambiguous whitespace.");
            return lowercase ? result.ToLowerInvariant() : result;
        }
        static string Output(string path)
        {
            if(string.IsNullOrEmpty(path) || !Path.IsPathRooted(path)) throw new ArgumentException("A BIG/SKUDEF output path must be absolute.");
            string result=Path.GetFullPath(path);RejectLinks(result);
            if(File.Exists(result) || Directory.Exists(result)) throw new IOException("BIG/SKUDEF output already exists.");
            return result;
        }
        static void Atomic(string output,CancellationToken cancellation,Action<FileStream> write)
        {
            cancellation.ThrowIfCancellationRequested();string temporary=output+".tmp-"+Guid.NewGuid().ToString("N");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            try
            {
                using(var stream=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)) { write(stream);stream.Flush(true); }
                cancellation.ThrowIfCancellationRequested();RejectLinks(output);File.Move(temporary,output);
            }
            finally { if(File.Exists(temporary)) File.Delete(temporary); }
        }
        static void BigEndian(BinaryWriter writer,uint value) { writer.Write((byte)(value>>24));writer.Write((byte)(value>>16));writer.Write((byte)(value>>8));writer.Write((byte)value); }
        static void RejectLinks(string path)
        {
            for(var item=new DirectoryInfo(path);item!=null;item=item.Parent)
                if(item.Exists && (item.Attributes&FileAttributes.ReparsePoint)!=0) throw new IOException("BIG input/output paths cannot use reparse points.");
            if(File.Exists(path) && (File.GetAttributes(path)&FileAttributes.ReparsePoint)!=0) throw new IOException("BIG input/output files cannot use reparse points.");
        }
    }
}
