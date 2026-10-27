using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static unsafe class TextureDxt5QuantizerDifferential
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate double Quantizer(int levels,int count,IntPtr values,IntPtr endpoints,IntPtr indices);
    private static int fixtures, checks;
    private static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    private static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
    private static int Main(string[] args)
    {
        try {
            Check(IntPtr.Size==4,"x86 required");
            string input=Path.GetFullPath(args[0]), dir=Path.GetDirectoryName(input);
            Check(Hash(File.ReadAllBytes(input))=="4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e","Original hash");
            AppDomain.CurrentDomain.AssemblyResolve+=delegate(object s,ResolveEventArgs e){string p=Path.Combine(dir,new AssemblyName(e.Name).Name+".dll");return File.Exists(p)?Assembly.LoadFrom(p):null;};
            Assembly original=Assembly.LoadFrom(input), recovered=Assembly.LoadFrom(Path.GetFullPath(args[2]));
            IntPtr module=Marshal.GetHINSTANCE(original.ManifestModule);
            Check(module!=IntPtr.Zero&&module!=new IntPtr(-1),"Native module");
            Quantizer native=(Quantizer)Marshal.GetDelegateForFunctionPointer(IntPtr.Add(module,0x7d500),typeof(Quantizer));
            MethodInfo quantize=recovered.GetType("BinaryAssetBuilder.TextureCompiler.TextureDxt5AlphaEncoder",true).GetMethod("Quantize");
            int[] sizes={0,1,2,3,4,5,7,8,15,16,17,32,64,127,256,511,512};
            using(var output=new StreamWriter(args[1])) {
                output.WriteLine("levels\tcount\tpattern\tendpoints\terrorBits");
                foreach(int levels in new int[]{6,8}) foreach(int count in sizes) for(int pattern=0;pattern<64;pattern++) {
                    double[] values=new double[Math.Max(1,count)];
                    Random random=new Random(count*7919+pattern*31+17);
                    for(int i=0;i<count;i++) {
                        double value;
                        switch(pattern%8) {
                            case 0:value=(pattern/8)/7.0;break;
                            case 1:value=(i&1)==0?0:1;break;
                            case 2:value=(i%3)/2.0;break;
                            case 3:value=(double)random.Next(256)/255;break;
                            case 4:value=random.NextDouble();break;
                            case 5:value=(i&1)==0?127.0/255:128.0/255;break;
                            case 6:value=(double)i/Math.Max(1,count-1);break;
                            default:value=(i%3)==0?0:(i%3)==1?1:random.NextDouble();break;
                        }
                        values[i]=(double)(float)value;
                    }
                    double[] actualInput=new double[count];Array.Copy(values,actualInput,count);
                    double[] before=(double[])actualInput.Clone();
                    int[] expectedEndpoints=new int[2], expectedIndices=new int[Math.Max(1,count)];
                    double expectedError;
                    fixed(double* v=values) fixed(int* e=expectedEndpoints) fixed(int* ind=expectedIndices)
                        expectedError=native(levels,count,new IntPtr(v),new IntPtr(e),new IntPtr(ind));
                    int[] endpoints=new int[2], indices=new int[count];
                    double error=(double)quantize.Invoke(null,new object[]{levels,actualInput,endpoints,indices});
                    string label="levels="+levels+" count="+count+" pattern="+pattern;
                    if(endpoints[0]!=expectedEndpoints[0]||endpoints[1]!=expectedEndpoints[1]||BitConverter.DoubleToInt64Bits(error)!=BitConverter.DoubleToInt64Bits(expectedError)) {
                        Console.Error.WriteLine(label+" input="+String.Join(",",Array.ConvertAll(actualInput,delegate(double x){return x.ToString("R",System.Globalization.CultureInfo.InvariantCulture);}))); 
                    }
                    Check(endpoints[0]==expectedEndpoints[0]&&endpoints[1]==expectedEndpoints[1],label+" endpoints expected="+String.Join(",",expectedEndpoints)+" actual="+String.Join(",",endpoints));
                    Check(BitConverter.DoubleToInt64Bits(error)==BitConverter.DoubleToInt64Bits(expectedError),label+" error expected="+expectedError.ToString("R")+" actual="+error.ToString("R"));
                    for(int i=0;i<count;i++) {
                        Check(indices[i]==expectedIndices[i],label+" index="+i+" expected="+expectedIndices[i]+" actual="+indices[i]);
                        Check(actualInput[i]==before[i],"Input modified");
                    }
                    output.WriteLine(levels+"\t"+count+"\t"+pattern+"\t"+String.Join(",",endpoints)+"\t"+BitConverter.DoubleToInt64Bits(error));fixtures++;
                }
            }
            Console.WriteLine("PASS Texture DXT5 production quantizer fixtures="+fixtures+" checks="+checks+"; exact endpoints, indices and native error bits");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
