using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    internal static class LoDCanonicalTests
    {
        static void Word(byte[] bytes,int offset,uint value) { for(int i=0;i<4;i++)bytes[offset+i]=(byte)(value>>(i*8)); }
        public static int Run(string root)
        {
            int checks=0;
            byte[] names=Encoding.ASCII.GetBytes("Test:asset1\0");
            byte[] oldDependencies=Encoding.ASCII.GetBytes("\u0002Static.manifest\0\u0002Global.manifest\0\u0002Audio.manifest\0");
            byte[] dependency=Encoding.ASCII.GetBytes("\u0002mod.manifest\0");
            for(int padding=0;padding<5;padding++) {
                byte[] sources=Encoding.ASCII.GetBytes("Data/asset1"+new string('x',padding)+".xml\0");
                byte[] input=new byte[96+oldDependencies.Length+names.Length+sources.Length];
                input[1]=1;input[2]=6;Word(input,4,0xf1234567);Word(input,12,1);
                Word(input,36,(uint)oldDependencies.Length);Word(input,40,(uint)names.Length);Word(input,44,(uint)sources.Length);
                Word(input,80,8);Word(input,84,4);Word(input,88,2);
                Buffer.BlockCopy(oldDependencies,0,input,96,oldDependencies.Length);
                Buffer.BlockCopy(names.Concat(sources).ToArray(),0,input,96+oldDependencies.Length,names.Length+sources.Length);
                // Independent expected wire layout: header + asset record,
                // replacement dependency, then unchanged logical string tables.
                byte[] expected=input.Take(96).Concat(dependency).Concat(names).Concat(sources).ToArray();
                Word(expected,36,(uint)dependency.Length);Array.Clear(expected,80,12);
                for(int repeat=0;repeat<3;repeat++) {
                    string directory=Path.Combine(root,padding+"-"+repeat);Directory.CreateDirectory(directory);
                    string manifest=Path.Combine(directory,"mod.manifest");File.WriteAllBytes(manifest,input);
                    var paths=LoDStreamBuilder.Create(manifest,CancellationToken.None);
                    foreach(string path in paths) {
                        byte[] actual=File.ReadAllBytes(path);
                        byte[] target=path.EndsWith(".manifest",StringComparison.Ordinal)?expected:new byte[]{0x67,0x45,0x23,0xf1};
                        if(!actual.SequenceEqual(target))throw new Exception("Canonical LoD wire layout differs: "+path);
                        checks++;
                    }
                    if(!File.ReadAllBytes(manifest).SequenceEqual(input))throw new Exception("LoD modified its source");
                    checks++;
                }
            }
            return checks;
        }
    }
}
