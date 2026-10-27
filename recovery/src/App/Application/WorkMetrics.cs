using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NewModCompiler.Application
{
    // Operation wall intervals, independent of legacy metrics. Nested intervals
    // overlap; callers must not add them to infer total build wall time.
    internal sealed class WorkMetrics
    {
        internal sealed class Measurement
        {
            public string stage { get; internal set; }
            public long calls { get; internal set; }
            public long failedCalls { get; internal set; }
            public double elapsedMs { get; internal set; }
            internal long ticks;
        }
        readonly SortedDictionary<string, Measurement> values = new SortedDictionary<string, Measurement>(StringComparer.Ordinal);
        public T Measure<T>(string stage, Func<T> operation)
        {
            Measurement value;
            if (!values.TryGetValue(stage, out value)) values.Add(stage, value = new Measurement { stage = stage });
            value.calls++;
            long started = Stopwatch.GetTimestamp(); bool succeeded = false;
            try { T result = operation(); succeeded = true; return result; }
            finally { value.ticks += Stopwatch.GetTimestamp() - started; if (!succeeded) value.failedCalls++; }
        }
        public void Measure(string stage, Action operation) { Measure(stage, () => { operation(); return true; }); }
        public Measurement[] Snapshot()
        {
            var result = new Measurement[values.Count]; int index = 0;
            foreach (Measurement value in values.Values)
                result[index++] = new Measurement { stage = value.stage, calls = value.calls, failedCalls = value.failedCalls,
                    elapsedMs = value.ticks * 1000.0 / Stopwatch.Frequency };
            return result;
        }
    }
}
