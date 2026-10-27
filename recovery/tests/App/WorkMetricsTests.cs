using System;
using System.IO;
using System.Linq;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    internal static class WorkMetricsTests
    {
        public static int Run()
        {
            var metrics = new WorkMetrics();
            object expected = new object();
            if (!Object.ReferenceEquals(metrics.Measure("operation", () => expected), expected)) throw new Exception("Work measurement changed a result");
            var snapshot = metrics.Snapshot();
            var error = new IOException("original operation failure");
            try { metrics.Measure("operation", (Action)(() => { throw error; })); throw new Exception("Work measurement swallowed failure"); }
            catch (IOException observed) { if (!Object.ReferenceEquals(observed, error)) throw new Exception("Work measurement replaced failure"); }
            var current = metrics.Snapshot().Single();
            if (current.calls != 2 || current.failedCalls != 1 || current.elapsedMs < snapshot[0].elapsedMs || Double.IsInfinity(current.elapsedMs) || Double.IsNaN(current.elapsedMs))
                throw new Exception("Work measurement lost failed operation work");
            if (snapshot.Single().calls != 1 || snapshot.Single().failedCalls != 0) throw new Exception("Work measurement mutated a previous snapshot");
            return 4;
        }
    }
}
