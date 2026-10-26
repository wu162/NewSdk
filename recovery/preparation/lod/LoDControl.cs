using System;
using System.Threading;
using NewModCompiler.Application;
class LoDControl {
    static int Main(string[] args) {
        try {var token=new CancellationToken(args.Length>1 && args[1]=="cancel");LoDStreamBuilder.Create(args[0],token);return 0;}
        catch(OperationCanceledException) {return 6;}
        catch(Exception e) {Console.Error.WriteLine(e);return 7;}
    }
}
