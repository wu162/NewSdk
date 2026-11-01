using System;
using System.Threading;
using BinaryAssetBuilder;

namespace BinaryAssetBuilder.Core;

// One logical Core owner. ExecutionContext carries this reference into managed
// Tasks, while each owner retains its own mutable legacy state and providers.
public sealed class CompilationContext
{
    private static readonly AsyncLocal<CompilationContext> Ambient = new AsyncLocal<CompilationContext>();
    private static readonly AsyncLocal<ContextScope> AmbientScope = new AsyncLocal<ContextScope>();
    private static readonly CompilationContext Legacy = new CompilationContext();

    internal Settings Settings;
    internal readonly DocumentProcessor.State Documents = new DocumentProcessor.State();
    internal readonly HashProvider.State Hashes = new HashProvider.State();
    internal readonly Tracer.State Tracing = new Tracer.State();
    internal readonly AssetReport.State AssetReport = new AssetReport.State();
    internal FileHashItem.KeepAliveDelegate KeepAlive;

    public static CompilationContext Current => Ambient.Value ?? Legacy;

    public IDisposable Enter()
    {
        var scope = new ContextScope(this);
        Ambient.Value = this;
        AmbientScope.Value = scope;
        return scope;
    }

    private sealed class ContextScope : IDisposable
    {
        private readonly CompilationContext context, parent;
        private readonly ContextScope parentScope;
        private readonly int threadId = Thread.CurrentThread.ManagedThreadId;
        private bool disposed;
        internal ContextScope(CompilationContext context)
        {
            this.context = context;
            parent = Ambient.Value;
            parentScope = AmbientScope.Value;
        }
        public void Dispose()
        {
            if (disposed) return;
            if (threadId != Thread.CurrentThread.ManagedThreadId ||
                !ReferenceEquals(Ambient.Value, context) || !ReferenceEquals(AmbientScope.Value, this))
                throw new InvalidOperationException("Compilation contexts must be disposed by their owner in reverse creation order.");
            Ambient.Value = parent;
            AmbientScope.Value = parentScope;
            disposed = true;
        }
    }
}
