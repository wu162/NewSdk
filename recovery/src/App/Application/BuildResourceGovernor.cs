using System;
using System.Threading;

namespace NewModCompiler.Application
{
    // One governor belongs to a build, including its XML owner threads. A slot
    // is released by the operation which uses it, not when its owner next polls.
    internal sealed class BuildResourceGovernor
    {
        readonly object gate = new object();
        readonly AsyncLocal<OwnerLease> currentOwner = new AsyncLocal<OwnerLease>();
        readonly int requestedJobs;
        int cpuCapacity, ioCapacity, cpu, io, waitingOwners;
        long byteBudget, retained, version;
        int peakCpu, peakIo;
        long peakRetained;
#if NEWSdk_MONOLITHIC
        BackendResourcePolicy policy;
        long nextSample;
#endif

        internal BuildResourceGovernor(int requestedJobs)
        {
            if (requestedJobs < 1) throw new ArgumentOutOfRangeException("requestedJobs");
            this.requestedJobs = requestedJobs;
#if NEWSdk_MONOLITHIC
            policy = BackendResourcePolicy.Capture(requestedJobs);
            cpuCapacity = policy.Capacity; ioCapacity = policy.IoCapacity; byteBudget = policy.ByteBudget;
#else
            cpuCapacity = Math.Max(1, Math.Min(32, Math.Min(requestedJobs, Environment.ProcessorCount)));
            ioCapacity = Math.Max(1, Math.Min(4, requestedJobs)); byteBudget = 16L * 1024 * 1024;
#endif
        }

        internal BuildResourceGovernor(int cpuCapacity, int ioCapacity, long byteBudget)
        {
            if (cpuCapacity < 1 || ioCapacity < 1 || byteBudget < 0) throw new ArgumentOutOfRangeException();
            requestedJobs = cpuCapacity;
            this.cpuCapacity = cpuCapacity; this.ioCapacity = ioCapacity; this.byteBudget = byteBudget;
        }

        internal int CpuCapacity { get { lock (gate) return cpuCapacity; } }
        internal int IoCapacity { get { lock (gate) return ioCapacity; } }
        internal long ByteBudget { get { lock (gate) return byteBudget; } }
        internal long Version { get { lock (gate) return version; } }
        internal bool OwnerActive
        {
            get { var owner = currentOwner.Value; return owner != null && owner.ThreadId == Thread.CurrentThread.ManagedThreadId && owner.Active; }
        }
#if NEWSdk_MONOLITHIC
        internal BackendResourcePolicy RefreshPolicy()
        {
            lock (gate)
            {
                long now = System.Diagnostics.Stopwatch.GetTimestamp();
                if (policy == null) return null; // Explicit capacities in deterministic contracts.
                if (now >= nextSample)
                {
                    nextSample = now + Math.Max(1, System.Diagnostics.Stopwatch.Frequency / 10);
                    policy = policy.RefreshMemoryBudget();
                    cpuCapacity = policy.Capacity; ioCapacity = policy.IoCapacity; byteBudget = policy.ByteBudget;
                    Changed();
                }
                return policy;
            }
        }
#endif
        internal IDisposable EnterOwner(CancellationToken cancellation)
        {
            if (currentOwner.Value != null) throw new InvalidOperationException("An owner resource scope is already active.");
            var owner = new OwnerLease(this);
            owner.Acquire(cancellation); currentOwner.Value = owner;
            return owner;
        }
        internal IDisposable SuspendOwner()
        {
            var owner = currentOwner.Value;
            if (owner == null || owner.ThreadId != Thread.CurrentThread.ManagedThreadId || !owner.Active) return null;
            owner.Release();
            return new Suspension(owner);
        }
        internal IDisposable TryAcquireCpu()
        {
            lock (gate)
            {
                // Waiting owners must be able to resume and harvest their
                // results even when another group keeps discovering work.
                if (cpu >= cpuCapacity || waitingOwners != 0) return null;
                cpu++; peakCpu = Math.Max(peakCpu, cpu);
                return new ResourceLease(this, true);
            }
        }
        internal IDisposable TryAcquireIo()
        {
            lock (gate)
            {
                if (io >= ioCapacity) return null;
                io++; peakIo = Math.Max(peakIo, io);
                return new ResourceLease(this, false);
            }
        }
        internal bool TryReserveMemory(long bytes)
        {
            if (bytes < 0) throw new ArgumentOutOfRangeException("bytes");
            lock (gate)
            {
                // A single owner encoder can exceed the available allowance;
                // it runs alone, as in the existing low-memory serial path.
                if (retained != 0 && (retained > byteBudget || bytes > byteBudget - retained)) return false;
                retained += bytes; peakRetained = Math.Max(peakRetained, retained); return true;
            }
        }
        internal void AdjustMemory(long bytes)
        {
            lock (gate)
            {
                retained += bytes; peakRetained = Math.Max(peakRetained, retained); Changed();
            }
        }
        internal void WaitForChange(long observedVersion, CancellationToken cancellation)
        {
            lock (gate)
            {
                cancellation.ThrowIfCancellationRequested();
                if (version == observedVersion) Monitor.Wait(gate, 50);
                cancellation.ThrowIfCancellationRequested();
            }
        }
        void Changed() { version++; Monitor.PulseAll(gate); }
        internal object Snapshot()
        {
            lock (gate) return new { requestedJobs, cpuCapacity, ioCapacity, byteBudget,
                peakCpuTasks = peakCpu, peakIoTasks = peakIo, peakRetainedResultBytes = peakRetained };
        }
        sealed class ResourceLease : IDisposable
        {
            BuildResourceGovernor governor;
            readonly bool cpu;
            internal ResourceLease(BuildResourceGovernor governor, bool cpu) { this.governor = governor; this.cpu = cpu; }
            public void Dispose()
            {
                var value = Interlocked.Exchange(ref governor, null); if (value == null) return;
                lock (value.gate) { if (cpu) value.cpu--; else value.io--; value.Changed(); }
            }
        }
        sealed class OwnerLease : IDisposable
        {
            readonly BuildResourceGovernor governor;
            internal readonly int ThreadId = Thread.CurrentThread.ManagedThreadId;
            internal bool Active;
            internal OwnerLease(BuildResourceGovernor governor) { this.governor = governor; }
            internal void Acquire(CancellationToken cancellation)
            {
                lock (governor.gate)
                {
                    governor.waitingOwners++;
                    try
                    {
                        while (governor.cpu >= governor.cpuCapacity)
                        { cancellation.ThrowIfCancellationRequested(); Monitor.Wait(governor.gate, 50); }
                        cancellation.ThrowIfCancellationRequested();
                        governor.cpu++; governor.peakCpu = Math.Max(governor.peakCpu, governor.cpu); Active = true;
                    }
                    finally { governor.waitingOwners--; }
                }
            }
            internal void Release()
            {
                lock (governor.gate) { if (!Active) return; Active = false; governor.cpu--; governor.Changed(); }
            }
            public void Dispose() { Release(); governor.currentOwner.Value = null; }
        }
        sealed class Suspension : IDisposable
        {
            OwnerLease owner;
            internal Suspension(OwnerLease owner) { this.owner = owner; }
            public void Dispose()
            {
                var value = Interlocked.Exchange(ref owner, null);
                // Re-enter for cleanup even after cancellation. Already started
                // native calls and IO must still be joined within the budget.
                if (value != null) value.Acquire(CancellationToken.None);
            }
        }
    }
}
