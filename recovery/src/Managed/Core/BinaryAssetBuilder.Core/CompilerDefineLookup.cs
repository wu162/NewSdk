using System;
using System.Collections.Generic;
using System.Globalization;

namespace BinaryAssetBuilder.Core;

// A final-expression lookup index, not a frozen Define environment or cached
// value. Every result retains the actual live Definition selected by its key.
internal sealed class CompilerDefineLookup : IDisposable
{
    private const int MinimumEntries = 16, MinimumQueries = 32, MaximumEntries = 65536, TextBudget = 1048576;
    internal sealed class Work
    {
        internal long Queries, Builds, Entries, IndexQueries, LegacyQueries, VersionChecks;
        internal bool Disabled, Disposed;
        internal int RetainedEntries, RequiredQueries;
        internal string Reason;
        internal object Snapshot() => new { queries = Queries, builds = Builds, entries = Entries,
            indexQueries = IndexQueries, legacyQueries = LegacyQueries, versionChecks = VersionChecks,
            disabled = Disabled, scopeReason = Reason, scopeDisposed = Disposed, retainedIndexEntries = RetainedEntries,
            minimumEntries = MinimumEntries, minimumQueries = MinimumQueries, requiredQueries = RequiredQueries, maximumEntries = MaximumEntries,
            textBudget = TextBudget, canAuthorizeReuse = false };
    }
    private readonly AssetDeclarationDocument owner;
    private readonly DefinitionSet definitions;
    private readonly object current;
    private readonly CultureInfo culture;
    private readonly Work work;
    private SortedDictionary<string, Definition>.Enumerator guard;
    private Dictionary<string, Definition> index;
    private bool active, hasGuard;

    internal CompilerDefineLookup(AssetDeclarationDocument owner, DefinitionSet definitions, Work work, bool reentrant)
    {
        this.owner = owner; this.definitions = definitions; this.work = work; current = owner.DefineLookupOwner;
        culture = CultureInfo.CurrentCulture;
        bool disabled; AppContext.TryGetSwitch("NewSdk.Compiler.DisableDefineLookupIndex", out disabled); work.Disabled = disabled;
        if (disabled) { work.Reason = "disabled"; return; }
        if (reentrant) { work.Reason = "reentrant-scope"; return; }
        if (definitions == null || definitions.GetType() != typeof(DefinitionSet) ||
            !Object.ReferenceEquals(definitions.Comparer, Comparer<string>.Default)) { work.Reason = "custom-definition-table-or-comparer"; return; }
        if (culture.GetType() != typeof(CultureInfo) || culture.CompareInfo.GetType() != typeof(CompareInfo))
            { work.Reason = "custom-culture"; return; }
        if (definitions.Count < MinimumEntries || definitions.Count > MaximumEntries)
            { work.Reason = "entry-budget-or-small-scope"; return; }
        // The standard enumerator guards the actual SortedDictionary version,
        // including mutations through a base-class/interface cast. MoveNext
        // still checks its version after reaching EOF; do not replace it with
        // Count or an end-of-enumeration shortcut.
        work.RequiredQueries = Math.Max(MinimumQueries, definitions.Count / 4);
        guard = definitions.GetEnumerator(); hasGuard = true; active = true; work.Reason = "query-warmup";
    }
    private void Reject(string reason) { active = false; index = null; work.RetainedEntries = 0; work.Reason = reason; }
    internal void Reentered() { if (active) Reject("reentrant-scope"); }
    private bool CheckVersion()
    {
        if (!Object.ReferenceEquals(owner.DefineLookupOwner, current) || !Object.ReferenceEquals(owner.AllDefines, definitions) || !Object.ReferenceEquals(CultureInfo.CurrentCulture, culture))
            { Reject("environment-or-culture-changed"); return false; }
        try { guard.MoveNext(); work.VersionChecks++; return true; }
        catch (InvalidOperationException) { Reject("definition-table-version-changed"); return false; }
    }
    internal bool TryGetValue(string name, out Definition value)
    {
        work.Queries++;
        if (active && name != null && CheckVersion() && work.Queries >= work.RequiredQueries)
        {
            if (index == null)
            {
                int chars = 0; string previous = null; bool first = true;
                var candidate = new Dictionary<string, Definition>(StringComparer.Create(culture, false));
                foreach (var pair in definitions)
                {
                    if (pair.Key.Length > TextBudget - chars) { Reject("text-budget"); break; }
                    // A default string comparer observes CurrentCulture. A tree
                    // built under a former culture may no longer be ordered;
                    // do not repair its legacy lookup answers with a new index.
                    if (!first && definitions.Comparer.Compare(previous, pair.Key) >= 0)
                        { Reject("definition-key-order-changed"); break; }
                    first = false; previous = pair.Key;
                    chars += pair.Key.Length; candidate.Add(pair.Key, pair.Value);
                }
                if (active && CheckVersion())
                {
                    index = candidate; work.Builds++; work.Entries += candidate.Count; work.RetainedEntries = candidate.Count; work.Reason = "bounded-final-expression-scope";
                }
            }
            if (active) { work.IndexQueries++; return index.TryGetValue(name, out value); }
        }
        work.LegacyQueries++; return owner.AllDefines.TryGetValue(name, out value);
    }
    public void Dispose()
    {
        active = false; index = null; work.RetainedEntries = 0; if (hasGuard) guard.Dispose(); work.Disposed = true;
    }
}
