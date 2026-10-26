using System;
using System.Threading;
using NewModCompiler.Application;

// Independent validation host, never compiled into the production EXE.
class BasePostprocessControl
{
    static int Main(string[] args)
    {
        try {
            var token = CancellationToken.None;
            if(args[0]=="hashfix") Console.WriteLine(HashFix.Apply(args[1],args[2],token));
            else if(args[0]=="resolver") Console.WriteLine(ModAssetResolver.Apply(args[1],args[2],token));
            else if(args[0]=="lod") Console.WriteLine(LoDStreamBuilder.Create(args[1],token).Length);
            else throw new ArgumentException("Unknown validation stage.");
            return 0;
        } catch(Exception error) {Console.Error.WriteLine(error);return 7;}
    }
}
