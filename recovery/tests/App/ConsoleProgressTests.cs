using System;
using System.IO;
using System.Linq;
using System.Threading;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Tests
{
    internal static class ConsoleProgressTests
    {
        static int checks;
        static void Check(bool value,string message) { checks++;if(!value)throw new Exception("Console progress: "+message); }
        sealed class BrokenWriter : StringWriter
        {
            public override void Write(char value) { throw new IOException("closed terminal"); }
            public override void WriteLine(string value) { throw new IOException("closed terminal"); }
        }
        internal static int Run()
        {
            checks=0;
            long elapsed=0;
            var state=new BuildProgressState(()=>elapsed);
            var redirected=new StringWriter();
            using(var renderer=new ConsoleProgressRenderer(state,redirected,false,()=>240,false)) {
                state.SetPlan(2);state.StartStage("compile");state.StartStream("base/Mod");
                string before=redirected.ToString();
                var workers=Enumerable.Range(0,8).Select(_=>new Thread(()=> {
                    for(int index=0;index<1000;index++)state.RecordWork(1,2,3);
                })).ToArray();
                foreach(var worker in workers)worker.Start();
                foreach(var worker in workers)worker.Join();
                var snapshot=state.Snapshot();
                Check(snapshot.AssetsCompleted==8000 && snapshot.XmlParsed==16000 && snapshot.DocumentsReused==24000,"Concurrent work increments were lost");
                Check(redirected.ToString()==before,"Work callbacks performed console output");
                state.SetPhase("cache-save");renderer.Refresh();
                Check(redirected.ToString()==before,"Redirected output included periodic/subphase refreshes");
                elapsed=1250;state.CompleteStream("succeeded");
                Check(redirected.ToString().Contains("完成 8,000") && redirected.ToString().Contains("1/2"),"Completed stream omitted true counters or stream denominator");
                state.StartStream("apt/UI");
                Check(state.Snapshot().AssetsCompleted==0 && state.Snapshot().StreamsCompleted==1,"Stream counters did not reset independently of stream completion");
                state.CompleteStream("failed");state.CompleteStage("failed");state.Finish("failed");
                Check(state.Snapshot().StreamsCompleted==1,"Failed stream counted as completed");
            }
            string text=redirected.ToString();
            Check(!text.Contains("\r") && !text.Contains("%") && text.Contains("总耗时 1.3s"),"Redirected output contains CR/percentage or lacks final elapsed time");

            elapsed=0;state=new BuildProgressState(()=>elapsed);
            var interactive=new StringWriter();
            using(var renderer=new ConsoleProgressRenderer(state,interactive,true,()=>400,false)) {
                state.SetPlan(1);state.StartStage("compile");state.StartStream("base/LongStream");
                text=interactive.ToString();state.RecordWork(7,5,3);
                Check(interactive.ToString()==text,"Interactive work callback wrote synchronously");
                elapsed=1000;renderer.Refresh();
                Check(interactive.ToString().Contains("完成 7") && interactive.ToString().Contains("XML 5"),"Periodic refresh did not show growing work");
                renderer.WriteLine("[warning] visible warning");
                text=interactive.ToString();int warning=text.IndexOf("[warning] visible warning",StringComparison.Ordinal);
                Check(warning>0 && text.IndexOf("[进行]",warning,StringComparison.Ordinal)>warning,"Warning was not followed by restored progress");
                state.CompleteStream("cancelled");state.CompleteStage("cancelled");
                text=interactive.ToString();renderer.Refresh();state.SetPhase("cache-save");
                Check(interactive.ToString()==text,"Completed stage reappeared as an active progress row");
                renderer.WriteLine("late teardown diagnostic");
                Check(!interactive.ToString().Substring(text.Length).Contains("[进行]"),"Teardown diagnostic resurrected a completed stage");
                state.Finish("cancelled");
                text=interactive.ToString();renderer.Refresh();
                Check(interactive.ToString()==text && text.EndsWith(Environment.NewLine,StringComparison.Ordinal),"Cancellation left an active progress row");
            }
            state=new BuildProgressState(()=>0);
            var narrow=new StringWriter();
            using(var renderer=new ConsoleProgressRenderer(state,narrow,true,()=>100,false)) {
                state.SetPlan(1);state.StartStage("compile");state.StartStream(new string('长',100));
                state.RecordWork(1140,186,720);state.SetPhase("process");
                string row=narrow.ToString().Split('\r').Last();
                Check(row.Contains("完成 1,140") && row.Contains("XML 186") && row.Contains("复用文档 720") && row.Contains("0.0s"),"Long stream name hid work counters or elapsed time");
                state.CompleteStream("succeeded");state.CompleteStage("succeeded");state.Finish("succeeded");
            }
            state=new BuildProgressState(()=>0);
            using(var renderer=new ConsoleProgressRenderer(state,new BrokenWriter(),true,()=>80,false)) {
                state.StartStage("plan");state.Finish("failed");
            }
            Check(true,"Disconnected console changed build outcome");
            return checks;
        }
    }
}
