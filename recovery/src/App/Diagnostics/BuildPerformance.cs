using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NewModCompiler.Diagnostics
{
    // One synchronous build owns this collector. Names are fixed phase names
    // (or asset types), never file/instance names, so memory does not grow per call.
    internal sealed class BuildPerformance
    {
        readonly string level;
        readonly Dictionary<string, Entry> entries;
        Scope current;
        internal string StreamId { get; set; }
        internal bool Enabled { get { return level != "off"; } }
        internal bool Detailed { get { return level == "detailed"; } }

        internal BuildPerformance(string level)
        {
            this.level = level ?? "off";
            if (Enabled) entries = new Dictionary<string, Entry>(StringComparer.Ordinal);
        }

        internal IDisposable Measure(string name, bool detailed = false)
        {
            // Off: no timestamp, scope allocation, dictionary lookup or output.
            if (!Enabled || (detailed && !Detailed)) return null;
            string key = (StreamId ?? "") + "\n" + name;
            Entry entry;
            if (!entries.TryGetValue(key, out entry))
                entries.Add(key, entry = new Entry { Name = name, StreamId = StreamId });
            var scope = new Scope(this, current, entry);
            current = scope;
            return scope;
        }

        internal object Snapshot()
        {
            return new {
                level, clock = "Stopwatch", frequency = Stopwatch.Frequency,
                boundary = "before-report-write",
                entries = entries.Values.OrderBy(e => e.StreamId, StringComparer.Ordinal).ThenBy(e => e.Name, StringComparer.Ordinal).Select(e => new {
                    name = e.Name, streamId = e.StreamId, count = e.Count,
                    totalMs = Milliseconds(e.Total), selfMs = Milliseconds(e.Self), maxMs = Milliseconds(e.Max)
                }).ToArray()
            };
        }
        static double Milliseconds(long ticks) { return ticks * (1000.0 / Stopwatch.Frequency); }

        sealed class Entry
        {
            internal string Name, StreamId;
            internal long Count, Total, Self, Max;
        }
        sealed class Scope : IDisposable
        {
            readonly BuildPerformance owner;
            readonly Scope parent;
            readonly Entry entry;
            readonly long started;
            long children;
            bool disposed;
            internal Scope(BuildPerformance owner, Scope parent, Entry entry)
            { this.owner = owner; this.parent = parent; this.entry = entry; started = Stopwatch.GetTimestamp(); }
            public void Dispose()
            {
                if (disposed) return;
                long elapsed = Stopwatch.GetTimestamp() - started;
                disposed = true;
                entry.Count++; entry.Total += elapsed; entry.Self += Math.Max(0, elapsed - children);
                entry.Max = Math.Max(entry.Max, elapsed);
                if (parent != null) parent.children += elapsed;
                owner.current = parent;
            }
        }
    }
}
