using System;
using System.Threading;
using NewModCompiler.Application;
class AssetMergeControl {
    static int Main(string[] args) {
        try {Console.WriteLine(AssetMerge.Apply(args[0],args[1],new CancellationToken(args.Length>2 && args[2]=="cancel")));return 0;}
        catch(OperationCanceledException) {return 6;}
        catch(Exception e) {Console.Error.WriteLine(e);return 7;}
    }
}
