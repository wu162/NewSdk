using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.Core;

// Cache metadata owns its mutable records, not the document graph those records
// refer to. No XML/SchemaInfo/Define evaluation is reconstructed here.
internal sealed class CompilerDocumentMetadataSnapshot
{
    internal List2<InclusionItem> Inclusions;
    internal List2<OutputAsset> Outputs;
    internal List2<Definition> Definitions;
    internal List2<DefinitionPair> Used;
    internal bool Legacy;

    private sealed class Identity<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T left, T right) { return Object.ReferenceEquals(left, right); }
        public int GetHashCode(T value) { return RuntimeHelpers.GetHashCode(value); }
    }

    private static bool Exact<T>(List2<T> values) where T : class
    {
        if (values == null) return true;
        if (values.GetType() != typeof(List2<T>)) return false;
        foreach (T value in values)
            if (value != null && value.GetType() != typeof(T)) return false;
        return true;
    }

    private static List2<T> Copy<T>(List2<T> values, bool legacy, Func<T, T> clone) where T : class
    {
        if (values == null) return null;
        if (legacy) return new List2<T>(values);
        var seen = new Dictionary<T, T>(new Identity<T>());
        var result = new List2<T>();
        foreach (T value in values)
        {
            if (value == null) { result.Add(null); continue; }
            T copy;
            if (!seen.TryGetValue(value, out copy)) { copy = clone(value); seen.Add(value, copy); }
            result.Add(copy);
        }
        return result;
    }

    internal static CompilerDocumentMetadataSnapshot Capture(List2<InclusionItem> inclusions,
        List2<OutputAsset> outputs, List2<Definition> definitions, List2<DefinitionPair> used, bool forceLegacy = false, bool forceValueCopy = false)
    {
        bool disabled;
        AppContext.TryGetSwitch("NewSdk.Compiler.DisableDocumentSnapshots", out disabled);
        bool legacy = forceLegacy || (disabled && !forceValueCopy) || !Exact(inclusions) || !Exact(outputs) || !Exact(definitions) || !Exact(used);
        if (!legacy && outputs != null)
            foreach (OutputAsset output in outputs)
                if (output != null && (object)output.Handle != null && output.Handle.GetType() != typeof(InstanceHandle))
                    { legacy = true; break; }
        var handles = legacy ? null : new Dictionary<InstanceHandle, InstanceHandle>(new Identity<InstanceHandle>());
        return new CompilerDocumentMetadataSnapshot {
            Legacy = legacy,
            Inclusions = Copy(inclusions, legacy, value => new InclusionItem(value.LogicalPath, value.PhysicalPath, value.Type) { Document = value.Document }),
            Definitions = Copy(definitions, legacy, value => new Definition { Name = value.Name, EvaluatedValue = value.EvaluatedValue,
                IsOverride = value.IsOverride, OriginalValue = value.OriginalValue, Document = value.Document }),
            Used = Copy(used, legacy, value => new DefinitionPair { Name = value.Name, EvaluatedValue = value.EvaluatedValue }),
            Outputs = Copy(outputs, legacy, value => {
                InstanceHandle handle = value.Handle, copy = null;
                if ((object)handle != null && !handles.TryGetValue(handle, out copy))
                    { copy = new InstanceHandle(handle); handles.Add(handle, copy); }
                return new OutputAsset { Handle = copy, InstanceFileSize = value.InstanceFileSize,
                    RelocationFileSize = value.RelocationFileSize, ImportsFileSize = value.ImportsFileSize };
            })
        };
    }
}
