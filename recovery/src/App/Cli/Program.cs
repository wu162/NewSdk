using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NewModCompiler.Application;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 1 && (args[0] == "--help" || args[0] == "-h")) { Console.WriteLine(Options.Help); return 0; }
            var sinks = new List<IBuildEventSink>();
            try {
                BuildRequest request = Options.Parse(args);
                if (request.Command == "doctor") {
                    Console.Error.WriteLine("SDK directory exists: " + request.SdkRoot);
                    Console.Error.WriteLine("BACKEND_NOT_INTEGRATED: P2 application skeleton cannot validate runtime providers or compile assets.");
                    return (int)ExitCode.Environment;
                }
                // Invalidate a previous report before opening any required output channels.
                Reports.RemoveStale(request.ReportPath);
                sinks.Add(new JsonLineSink(request.EventsPath)); sinks.Add(new TextLogSink(request.LogPath));
                // Transfer ownership so partial-startup cleanup cannot close successful
                // session sinks a second time after the exit code has been finalized.
                var ownedSinks=sinks.ToArray(); sinks.Clear();
                return RunSession(request,ownedSinks);
            } catch (BuildFailure e) { Console.Error.WriteLine(e.Code + ": " + e.Message); return (int)e.ExitCode; }
            catch (IOException e) { Console.Error.WriteLine("OUTPUT_FAILED: " + e.Message); return (int)ExitCode.Output; }
            catch (UnauthorizedAccessException e) { Console.Error.WriteLine("OUTPUT_FAILED: " + e.Message); return (int)ExitCode.Output; }
            catch (Exception e) { Console.Error.WriteLine("INTERNAL_ERROR: " + e); return (int)ExitCode.Internal; }
            finally { foreach (IBuildEventSink sink in sinks) { try { sink.Dispose(); } catch { } } }
        }
        internal static int RunSession(BuildRequest request,IEnumerable<IBuildEventSink> sinks)
        {
            using (var events = new EventDispatcher(Guid.NewGuid().ToString("N"),sinks))
            using (var cancellation = new CancellationTokenSource()) {
                ConsoleCancelEventHandler handler = (sender,e) => { e.Cancel=true; cancellation.Cancel(); };
                Console.CancelKeyPress += handler;
                try { return (int)new BuildSession(request,null,events).Run(cancellation.Token); }
                finally { Console.CancelKeyPress -= handler; }
            }
        }
    }
}
