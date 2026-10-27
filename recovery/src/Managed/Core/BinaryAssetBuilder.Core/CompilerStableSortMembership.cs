using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BinaryAssetBuilder.Core;

// One synchronous StableSort call only. This is an index of already-constructed
// root-private dependency sets, never a replacement for closure construction.
internal sealed class CompilerStableSortMembership
{
    internal const int MinimumSetSize = 16, MaximumSets = 1024, MaximumEntries = 65536, MaximumDeclarations = 4096;
    internal sealed class Work
    {
        internal long Builds, Entries, IndexQueries, IndexMatches, LinearQueries, SmallSetQueries, BudgetBypasses, UnsafeScopeBypasses, CallbackFallbacks;
        internal bool Enabled, Disabled;
        internal string Reason;
        internal object Snapshot()
        {
            return new { calls = 1, enabled = Enabled, disabled = Disabled, builds = Builds, entries = Entries,
                indexQueries = IndexQueries, indexMatches = IndexMatches, linearQueries = LinearQueries, smallSetQueries = SmallSetQueries,
                budgetBypasses = BudgetBypasses, unsafeScopeBypasses = UnsafeScopeBypasses, callbackFallbacks = CallbackFallbacks,
                scopeReason = Reason, minimumSetSize = MinimumSetSize, maximumSets = MaximumSets, maximumEntries = MaximumEntries };
        }
    }
    private sealed class SetIdentity : IEqualityComparer<InstanceHandleSet>
    {
        public bool Equals(InstanceHandleSet left, InstanceHandleSet right) { return Object.ReferenceEquals(left, right); }
        public int GetHashCode(InstanceHandleSet value) { return RuntimeHelpers.GetHashCode(value); }
    }
    private readonly Work work;
    private readonly long callbackVersion;
    private readonly HashSet<InstanceHandleSet> allowed = new HashSet<InstanceHandleSet>(new SetIdentity());
    private readonly Dictionary<InstanceHandleSet, HashSet<ulong>> indexes = new Dictionary<InstanceHandleSet, HashSet<ulong>>(new SetIdentity());
    private bool active;

    private CompilerStableSortMembership(Work work) { this.work = work; callbackVersion = Tracer.CallbackVersion; }
    private void Reject(string reason, bool budget = false)
    {
        active = false; work.Enabled = false; work.Reason = reason;
        if (budget) work.BudgetBypasses++; else work.UnsafeScopeBypasses++;
    }
    private static bool ExactHandle(InstanceHandle handle) { return (object)handle != null && handle.GetType() == typeof(InstanceHandle); }
    private static bool ExactDeclarations(List2<InstanceDeclaration> declarations)
    {
        if (declarations == null || declarations.GetType() != typeof(List2<InstanceDeclaration>)) return false;
        foreach (InstanceDeclaration declaration in declarations)
            if (declaration == null || declaration.GetType() != typeof(InstanceDeclaration) || !ExactHandle(declaration.ObservationHandle)) return false;
        return true;
    }
    private static bool BuiltinDictionary(IDictionary<uint, int> dependencies)
    {
        if (dependencies == null) return false;
        if (dependencies.GetType() == typeof(SortedDictionary<uint, int>))
            return Object.ReferenceEquals(((SortedDictionary<uint, int>)dependencies).Comparer, Comparer<uint>.Default);
        if (dependencies.GetType() == typeof(Dictionary<uint, int>))
            return Object.ReferenceEquals(((Dictionary<uint, int>)dependencies).Comparer, EqualityComparer<uint>.Default);
        return false;
    }
    internal static CompilerStableSortMembership Create(bool safeOwner, Dictionary<InstanceDeclaration, StreamBindingState> bindings,
        List2<InstanceDeclaration> inputs, List2<InstanceDeclaration> final, IDictionary<uint, int> typeDependencies, Work work)
    {
        var result = new CompilerStableSortMembership(work);
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableStableSortMembershipIndex", out disabled);
        work.Disabled = disabled;
        if (disabled) { work.Reason = "disabled"; return result; }
        if (!safeOwner || bindings == null || bindings.GetType() != typeof(Dictionary<InstanceDeclaration, StreamBindingState>))
            { result.Reject("legacy-or-custom-binding-owner"); return result; }
        if (!BuiltinDictionary(typeDependencies)) { result.Reject("custom-schema-dependency-dictionary-or-comparer"); return result; }
        CultureInfo culture = CultureInfo.CurrentCulture;
        if (culture.GetType() != typeof(CultureInfo) || culture.CompareInfo.GetType() != typeof(CompareInfo))
            { result.Reject("custom-culture-or-compare-info"); return result; }
        if (bindings.Count > MaximumSets || (long)(inputs == null ? 0 : inputs.Count) + (final == null ? 0 : final.Count) > MaximumDeclarations)
            { result.Reject("scope-budget", true); return result; }
        if (!ExactDeclarations(inputs) || !ExactDeclarations(final)) { result.Reject("nonexact-declaration-or-handle"); return result; }
        long entries = 0;
        foreach (StreamBindingState binding in bindings.Values)
        {
            if (binding == null || binding.Dependencies == null || binding.Dependencies.GetType() != typeof(InstanceHandleSet))
                { result.Reject("nonexact-dependency-set"); return result; }
            entries += binding.Dependencies.Count;
            if (entries > MaximumEntries) { result.Reject("entry-budget", true); return result; }
            foreach (InstanceHandle handle in binding.Dependencies)
                if (!ExactHandle(handle)) { result.Reject("nonexact-dependency-handle"); return result; }
            result.allowed.Add(binding.Dependencies);
        }
        // Validation itself calls no plugin/trace/custom equality. If a callback
        // did occur, no partially prepared scope may authorize indexed answers.
        if (Tracer.CallbackVersion != result.callbackVersion) { work.CallbackFallbacks++; work.Reason = "callback-during-preparation"; return result; }
        result.active = true; work.Enabled = true; work.Reason = "root-private-exact-synchronous-scope";
        return result;
    }
    internal bool Contains(InstanceHandleSet dependencies, InstanceHandle handle)
    {
        if (active && Tracer.CallbackVersion != callbackVersion)
        {
            active = false; work.Enabled = false; work.CallbackFallbacks++; work.Reason = "callback-version-changed";
        }
        if (active && (!allowed.Contains(dependencies) || !ExactHandle(handle))) Reject("changed-or-unsafe-query-graph");
        if (!active) { work.LinearQueries++; return dependencies.Contains(handle); }
        if (dependencies.Count < MinimumSetSize)
        {
            work.SmallSetQueries++; work.LinearQueries++; return dependencies.Contains(handle);
        }
        HashSet<ulong> index;
        if (!indexes.TryGetValue(dependencies, out index))
        {
            index = new HashSet<ulong>();
            foreach (InstanceHandle dependency in dependencies) index.Add(Key(dependency));
            indexes.Add(dependencies, index); work.Builds++; work.Entries += index.Count;
        }
        work.IndexQueries++;
        bool found = index.Contains(Key(handle)); if (found) work.IndexMatches++;
        return found;
    }
    private static ulong Key(InstanceHandle handle) { return ((ulong)handle.TypeId << 32) | handle.InstanceId; }
}
