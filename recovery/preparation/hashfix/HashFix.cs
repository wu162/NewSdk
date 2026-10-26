using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NewModCompiler.Application
{
    // RA3 PC HashFix: preserve the original nested-loop's first DIFFERENT
    // matching instance hash, including its behavior for duplicate identities.
    public static class HashFix
    {
        public static int Apply(string manifest,string worldbuilder,CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            byte[] data=ReadManifest(manifest),reference=ReadManifest(worldbuilder);
            var hashes=new Dictionary<ulong,List<uint>>();
            for(int offset=48;offset<48L+48L*Read(reference,12);offset+=48) {
                cancellation.ThrowIfCancellationRequested();
                ulong id=Identity(reference,offset);List<uint> list;
                if(!hashes.TryGetValue(id,out list)) {list=new List<uint>();hashes.Add(id,list);}
                list.Add(Read(reference,offset+12));
            }
            int changed=0;
            for(int offset=48;offset<48L+48L*Read(data,12);offset+=48) {
                cancellation.ThrowIfCancellationRequested();List<uint> list;
                if(!hashes.TryGetValue(Identity(data,offset),out list)) continue;
                foreach(uint hash in list) if(hash!=Read(data,offset+12)) {
                    for(int i=0;i<4;i++) data[offset+12+i]=(byte)(hash>>(8*i));
                    changed++;break;
                }
            }
            cancellation.ThrowIfCancellationRequested();
            // Input must be this build's isolated staging manifest. No arbitrary
            // cache/source file is ever passed by the eventual package stage.
            using(var stream=new FileStream(manifest,FileMode.Open,FileAccess.Write,FileShare.None)) stream.Write(data,0,data.Length);
            return changed;
        }
        static byte[] ReadManifest(string path) {
            byte[] data=File.ReadAllBytes(path);
            if(data.Length<48 || data[0]!=0 || data[1]!=1 || data[2]!=6 || data[3]!=0)
                throw new InvalidDataException("HashFix requires a linked little-endian RA3 v6 manifest.");
            long end=48L+48L*Read(data,12)+Read(data,32)+Read(data,36)+Read(data,40)+Read(data,44);
            if(end!=data.LongLength) throw new InvalidDataException("HashFix manifest table length differs.");
            return data;
        }
        static ulong Identity(byte[] data,int offset) {return (ulong)Read(data,offset)|((ulong)Read(data,offset+4)<<32);}
        static uint Read(byte[] data,int offset) {return (uint)data[offset]|((uint)data[offset+1]<<8)|((uint)data[offset+2]<<16)|((uint)data[offset+3]<<24);}
    }
}
