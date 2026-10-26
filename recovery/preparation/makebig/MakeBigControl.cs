using System;
using NewModCompiler.Packaging.MakeBigRecovery;
class MakeBigControl {
    static int Main(string[] args) {try{return Program.Run(args,Console.In,Console.Out);}catch(Exception e){Console.Error.WriteLine(e);return 7;}}
}
