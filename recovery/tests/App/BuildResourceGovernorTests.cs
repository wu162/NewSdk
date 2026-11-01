using System;
using System.Collections.Generic;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    internal static class BuildResourceGovernorTests
    {
        static int checks;
        static void Check(bool condition, string message)
        { checks++; if (!condition) throw new Exception("Build resource governor: " + message); }

        internal static int Run()
        {
            checks = 0;
            var governor = new BuildResourceGovernor(2, 1, 100);
            using (governor.EnterOwner(CancellationToken.None))
            {
                Check(governor.OwnerActive, "XML owner consumes one CPU slot");
                using (var worker = governor.TryAcquireCpu())
                {
                    Check(worker != null, "one worker fits beside the owner");
                    Check(governor.TryAcquireCpu() == null, "all groups share the CPU limit");
                    using (governor.SuspendOwner())
                    {
                        Check(!governor.OwnerActive, "waiting owner releases its slot");
                        using (var otherOwnerWork = governor.TryAcquireCpu())
                            Check(otherOwnerWork != null, "another group uses the released owner slot");
                    }
                    Check(governor.OwnerActive, "owner reacquires its slot before Core resumes");
                }
            }
            Check(!governor.OwnerActive, "owner scope returns its CPU resource");
            using (var first = governor.TryAcquireIo())
            {
                Check(first != null && governor.TryAcquireIo() == null, "IO is a shared independent limit");
                first.Dispose();
                using (var second = governor.TryAcquireIo()) Check(second != null, "IO completion returns its resource immediately");
            }
            Check(governor.TryReserveMemory(60), "first group's prepared result fits");
            Check(!governor.TryReserveMemory(50), "another group cannot oversubscribe retained bytes");
            Check(governor.TryReserveMemory(40), "exact remaining bytes fit");
            governor.AdjustMemory(-60);
            Check(governor.TryReserveMemory(60), "completed group returns its retained-byte allocation");
            governor.AdjustMemory(-100);

            // Waiting admission must notice cancellation even if no worker is
            // available to pulse the governor. No background thread may remain.
            var serial = new BuildResourceGovernor(1, 1, 100);
            Exception rejected = null;
            using (var occupied = serial.TryAcquireCpu())
            using (var cancel = new CancellationTokenSource())
            using (var entered = new ManualResetEventSlim())
            {
                var blocked = new Thread(() => {
                    entered.Set();
                    try { using (serial.EnterOwner(cancel.Token)) { } }
                    catch (Exception error) { rejected = error; }
                });
                blocked.Start(); Check(entered.Wait(2000), "waiting owner started");
                cancel.Cancel(); Check(blocked.Join(2000), "cancelled owner admission leaves no thread");
                Check(rejected is OperationCanceledException, "waiting owner reports cancellation");
            }
            using (serial.EnterOwner(CancellationToken.None))
            {
                Check(serial.TryAcquireCpu() == null, "jobs 1 reserves its sole CPU for the owner");
                using (serial.SuspendOwner())
                using (var nativeWork = serial.TryAcquireCpu()) Check(nativeWork != null, "jobs 1 can yield during a join");
            }

            var totals = governor.Snapshot();
            var snapshot = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string,object>>(
                new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(totals));
            Check(Convert.ToInt32(snapshot["peakCpuTasks"]) == 2, "reported peak includes XML owners and workers");
            Check(Convert.ToInt32(snapshot["peakIoTasks"]) == 1, "reported IO peak is global");
            Check(Convert.ToInt64(snapshot["peakRetainedResultBytes"]) == 100, "reported retained bytes combine all groups");
            return checks;
        }
    }
}
