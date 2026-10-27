using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

// Original-only numeric contracts; stdcall D3DX tables use the process heap,
// not the VC8 CRT allocator used by nvImage. Never mix the two allocators.
internal static class TextureNpotNumericProbe
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]private delegate IntPtr Weights(uint source,uint target,int wrap);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]private delegate void FreeTable(IntPtr table);
    static int checks,tables,taps;
    static void Check(bool condition,string label){checks++;if(!condition)throw new Exception(label);}
    static void Dump(IntPtr module,string output,int rva,int size)
    {
        byte[] bytes=new byte[size];Marshal.Copy(IntPtr.Add(module,rva),bytes,0,size);File.WriteAllBytes(Path.Combine(output,"rva-"+rva.ToString("x")+".bin"),bytes);
        using(var w=new StreamWriter(Path.Combine(output,"rva-"+rva.ToString("x")+".tsv"))){
            w.WriteLine("rva\tbits\tfloat32\tdouble64\tpointerRva");
            for(int i=0;i<size;i+=4){uint bits=BitConverter.ToUInt32(bytes,i);long relative=(long)bits-(long)(uint)module.ToInt32();
                w.WriteLine((rva+i).ToString("x")+"\t"+bits.ToString("X8")+"\t"+BitConverter.ToSingle(bytes,i).ToString("R",CultureInfo.InvariantCulture)+"\t"+(i%8==0&&i+8<=size?BitConverter.ToDouble(bytes,i).ToString("R",CultureInfo.InvariantCulture):"")+"\t"+(relative>=0&&relative<0x200000?relative.ToString("x"):""));
            }
        }
    }
    static int Main(string[] args)
    {
        IDisposable owner=null;
        try{
            Check(IntPtr.Size==4,"x86 required");string input=Path.GetFullPath(args[0]),directory=Path.GetDirectoryName(input),output=Path.GetFullPath(args[1]);Directory.CreateDirectory(output);
            using(var sha=SHA256.Create())Check(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(input))).Replace("-","").ToLowerInvariant()=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object sender,ResolveEventArgs e){string path=Path.Combine(directory,new AssemblyName(e.Name).Name+".dll");return File.Exists(path)?Assembly.LoadFrom(path):null;};
            Assembly original=Assembly.LoadFrom(input);Type type=original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin",true);object plugin=Activator.CreateInstance(type);owner=(IDisposable)plugin;
            MethodInfo initialize=type.GetMethod("Initialize");initialize.Invoke(plugin,new object[]{null,Enum.ToObject(initialize.GetParameters()[1].ParameterType,0)});
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Native module");
            Check(Marshal.ReadByte(IntPtr.Add(module,0x18ad8))==0x55&&Marshal.ReadByte(IntPtr.Add(module,0x18ad9))==0x8b,"D3DX contribution prologue");
            Check(Marshal.ReadByte(IntPtr.Add(module,0x1208a))==0xff&&Marshal.ReadByte(IntPtr.Add(module,0x12090))==0xff,"D3DX process-heap free stub");
            var weights=(Weights)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x18ad8),typeof(Weights));
            var free=(FreeTable)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x1208a),typeof(FreeTable));
            using(var transcript=new StreamWriter(Path.Combine(output,"weights.tsv"))){
                transcript.WriteLine("source\ttarget\tsourceIndex\ttargetIndex\tweightBits\tweight");
                foreach(uint source in new uint[]{1,2,3,4,5,7,8,9,15,17,31,33,63,127,255}){
                    uint target=1;while(target<source)target*=2;
                    IntPtr table=weights(source,target,1);Check(table!=IntPtr.Zero,"Weights allocation");
                    try{
                        int size=Marshal.ReadInt32(table);Check(size>=4&&size<1048576,"Weights size");byte[] bytes=new byte[size];Marshal.Copy(table,bytes,0,size);File.WriteAllBytes(Path.Combine(output,"weights-"+source+"-"+target+".bin"),bytes);
                        int offset=4;
                        for(uint i=0;i<source;i++){
                            int block=BitConverter.ToInt32(bytes,offset);Check(block>=4&&(block-4)%8==0&&block<=size-offset,"Contribution block");
                            for(int p=offset+4;p<offset+block;p+=8){uint destination=BitConverter.ToUInt32(bytes,p),bits=BitConverter.ToUInt32(bytes,p+4);float value=BitConverter.ToSingle(bytes,p+4);Check(destination<target&&!Single.IsNaN(value)&&value>=0,"Contribution domain");
                                transcript.WriteLine(source+"\t"+target+"\t"+i+"\t"+destination+"\t"+bits.ToString("X8")+"\t"+value.ToString("R",CultureInfo.InvariantCulture));taps++;
                            }offset+=block;
                        }
                        Check(offset==size,"Weights complete table");tables++;
                    }finally{free(table);}
                }
            }
            Dump(module,output,0x98080,0x40);
            Dump(module,output,0x99280,0xd80);
            Dump(module,output,0xb8fc8,0x100);
            Console.WriteLine("PROBE Texture D3DX numeric tables="+tables+" taps="+taps+" checks="+checks+"; original-only weights/constants/vtables/dither; process-heap ownership preserved; no recovered acceptance");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}finally{if(owner!=null)owner.Dispose();}
    }
}
