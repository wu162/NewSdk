using System;
using System.Collections.Generic;
using System.Globalization;

namespace BinaryAssetBuilder.Core;

// One synchronous output-binding traversal. Answers still flow through the
// original location precedence, derived-type selection and observation sites.
// No query/result survives the scope; public callbacks permanently deactivate it.
internal sealed class CompilerSymbolLookup
{
    internal const int MinimumEntries = 16, MaximumEntries = 65536;
    internal sealed class Work
    {
        internal long Builds, Entries, IndexQueries, LegacyQueries, CallbackFallbacks, ExternalFallbacks;
        internal bool Enabled, Disabled;
        internal string Reason;
        internal object Snapshot() => new { calls = 1, enabled = Enabled, disabled = Disabled, builds = Builds,
            entries = Entries, indexQueries = IndexQueries, legacyQueries = LegacyQueries,
            callbackFallbacks = CallbackFallbacks, externalFallbacks = ExternalFallbacks, scopeReason = Reason,
            minimumEntries = MinimumEntries, maximumEntries = MaximumEntries };
    }
    private readonly Work work;
    private readonly long callbackVersion = Tracer.CallbackVersion;
    private readonly Dictionary<InstanceSet, Dictionary<ulong, InstanceDeclaration>> indexes = new Dictionary<InstanceSet, Dictionary<ulong, InstanceDeclaration>>();
    private bool active;
    private CompilerSymbolLookup(Work work) { this.work = work; }
    private void Reject(string reason) { active = false; work.Enabled = false; work.Reason = reason; }
    internal static ulong Key(InstanceHandle handle) => ((ulong)handle.TypeId << 32) | handle.InstanceId;
    internal static CompilerSymbolLookup Create(bool safeOwner, InstanceSet[] sets, Work work)
    {
        var result = new CompilerSymbolLookup(work);
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableSymbolLookupIndex", out disabled); work.Disabled = disabled;
        if (disabled) { work.Reason = "disabled"; return result; }
        if (!safeOwner || sets == null) { result.Reject("custom-or-legacy-binding-owner"); return result; }
        CultureInfo culture = CultureInfo.CurrentCulture;
        if (culture.GetType() != typeof(CultureInfo) || culture.CompareInfo.GetType() != typeof(CompareInfo))
            { result.Reject("custom-culture"); return result; }
        int count = 0;
        foreach (InstanceSet set in sets)
        {
            if (set == null) continue;
            if (set.GetType() != typeof(InstanceSet)) { result.Reject("custom-symbol-table"); return result; }
            if (result.indexes.ContainsKey(set)) continue;
            SortedDictionary<InstanceHandle, InstanceDeclaration> table;
            if (!set.TryGetSymbolTable(out table)) { result.Reject("custom-symbol-table"); return result; }
            count += table == null ? 0 : table.Count;
            if (count > MaximumEntries) { result.Reject("entry-budget"); return result; }
            result.indexes.Add(set, null);
        }
        if (count < MinimumEntries) { result.Reject("small-scope"); return result; }
        // Admission invokes no custom enumerators, equality or item getters.
        // Validate every reachable declaration before recursive binding can call
        // a custom reference collection. Mutable/invalid key trees fall back.
        int referenceBudget = MaximumEntries * 4;
        foreach (InstanceSet set in result.indexes.Keys)
        {
            SortedDictionary<InstanceHandle, InstanceDeclaration> table; set.TryGetSymbolTable(out table);
            foreach (InstanceDeclaration declaration in set)
                if (declaration == null || !declaration.IsPlainSymbolCandidate(ref referenceBudget)) { result.Reject("custom-or-incomplete-declaration"); return result; }
            ulong previous = 0; bool first = true;
            if (table == null) continue;
            foreach (var pair in table)
            {
                if ((object)pair.Key == null || pair.Key.GetType() != typeof(InstanceHandle) || pair.Value == null || !pair.Value.IsPlainSymbolCandidate(ref referenceBudget))
                    { result.Reject("custom-key-or-value"); return result; }
                ulong key = Key(pair.Key);
                if (!first && key <= previous) { result.Reject("mutated-key-order"); return result; }
                first = false; previous = key;
            }
        }
        if (Tracer.CallbackVersion != result.callbackVersion) { result.Reject("callback-during-admission"); return result; }
        result.active = true; work.Enabled = true; work.Reason = "exact-synchronous-binding-scope";
        return result;
    }
    internal void ExternalBoundary()
    {
        if (!active) return;
        work.ExternalFallbacks++; Reject("external-boundary");
    }
    internal bool TryGetValue(InstanceSet set, InstanceHandle handle, out InstanceDeclaration value)
    {
        if (active && Tracer.CallbackVersion != callbackVersion)
            { work.CallbackFallbacks++; Reject("callback-version-changed"); }
        Dictionary<ulong, InstanceDeclaration> index;
        if (active && (set == null || set.GetType() != typeof(InstanceSet) || (object)handle == null || handle.GetType() != typeof(InstanceHandle) || !indexes.TryGetValue(set, out index)))
            Reject("changed-query-graph");
        if (!active) { work.LegacyQueries++; return set.TryGetValue(handle, out value); }
        index = indexes[set];
        if (index == null)
        {
            SortedDictionary<InstanceHandle, InstanceDeclaration> table;
            set.TryGetSymbolTable(out table);
            index = new Dictionary<ulong, InstanceDeclaration>();
            if (table != null) foreach (var pair in table) index.Add(Key(pair.Key), pair.Value);
            indexes[set] = index; work.Builds++; work.Entries += index.Count;
        }
        work.IndexQueries++;
        return index.TryGetValue(Key(handle), out value);
    }
}
