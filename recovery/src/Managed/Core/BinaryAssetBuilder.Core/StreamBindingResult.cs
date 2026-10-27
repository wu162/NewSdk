using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinaryAssetBuilder.Core;

// A successful root stream's binding identity snapshot. This is deliberately
// separate from reusable declarations and contains no mutable Core objects.
public sealed class StreamBindingResult
{
    public sealed class AssetIdentity
    {
        public string typeName { get; private set; }
        public string instanceName { get; private set; }
        public uint typeId { get; private set; }
        public uint instanceId { get; private set; }
        public uint typeHash { get; private set; }
        public uint instanceHash { get; private set; }
        internal AssetIdentity(InstanceHandle handle)
        {
            typeName = handle.TypeName; instanceName = handle.InstanceName;
            typeId = handle.TypeId; instanceId = handle.InstanceId;
            typeHash = handle.TypeHash; instanceHash = handle.InstanceHash;
        }
    }
    public sealed class AssetBinding
    {
        public AssetIdentity identity { get; private set; }
        public string sourceDocument { get; private set; }
        public int strongReferenceCount { get; private set; }
        public IList<AssetIdentity> references { get; private set; }
        public IList<AssetIdentity> dependencies { get; private set; }
        private static IList<AssetIdentity> Snapshot(IEnumerable<InstanceHandle> handles)
        {
            var values = new List<AssetIdentity>();
            if (handles != null) foreach (InstanceHandle handle in handles) values.Add(new AssetIdentity(handle));
            return new ReadOnlyCollection<AssetIdentity>(values);
        }
        internal AssetBinding(InstanceDeclaration declaration, IEnumerable<InstanceHandle> references, IEnumerable<InstanceHandle> dependencies)
        {
            identity = new AssetIdentity(declaration.Handle);
            sourceDocument = declaration.ObservationDocument == null ? null : declaration.ObservationDocument.ObservationSourcePath;
            strongReferenceCount = declaration.ReferencedInstances.Count;
            this.references = Snapshot(references); this.dependencies = Snapshot(dependencies);
        }
    }
    public bool enabled { get; private set; }
    public string sourceDocument { get; private set; }
    public IList<AssetBinding> assets { get; private set; }
    internal StreamBindingResult(bool enabled, string source, IList<AssetBinding> assets)
    {
        this.enabled = enabled; sourceDocument = source;
        this.assets = new ReadOnlyCollection<AssetBinding>(new List<AssetBinding>(assets));
    }
}

// Mutable only while the owning root is being bound; never stored on a reusable
// InstanceDeclaration. Handle identity remains the legacy live identity during
// execution; the public successful result snapshots its scalar values separately.
internal sealed class StreamBindingState
{
    internal readonly List2<InstanceHandle> References = new List2<InstanceHandle>();
    internal readonly InstanceHandleSet Dependencies = new InstanceHandleSet();
}
