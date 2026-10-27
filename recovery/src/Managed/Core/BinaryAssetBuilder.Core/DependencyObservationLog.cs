using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BinaryAssetBuilder.Core;

// Bounded shadow evidence only. These observations never authorize a cache hit,
// change a resolver answer, memoize a query or remove any legacy existence check.
public sealed class DependencyObservationLog
{
    [ThreadStatic] private static DependencyObservationLog _Current;
    private readonly bool _Enabled;
    private readonly int _Limit;
    private readonly long _TextBudget;
    private long _RecordedTextChars;
    private readonly List<Observation> _Observations = new List<Observation>();
    private int _Thread;
    private string _TaskId;
    private Resolution _Resolution;
    private long _Sequence, _QueryId, _Dropped, _ResolutionCalls, _CandidateChecks, _FoundChecks, _MissingChecks, _Succeeded, _Failed;
    private long _DefineReads, _SymbolLookups, _SymbolCandidateChecks, _InheritanceTemplates, _SymbolSelections;

    public DependencyObservationLog(int observationLimit = 8192, long textBudget = 1048576)
    {
        if (observationLimit < 0) throw new ArgumentOutOfRangeException("observationLimit");
        if (textBudget < 0) throw new ArgumentOutOfRangeException("textBudget");
        bool disabled; _Enabled = !(AppContext.TryGetSwitch("NewSdk.Compiler.DisableDependencyObservations", out disabled) && disabled);
        _Limit = observationLimit; _TextBudget = textBudget;
    }
    public sealed class Observation
    {
        public long sequence { get; private set; }
        public string kind { get; private set; }
        public long queryId { get; private set; }
        public long parentQueryId { get; private set; }
        public string taskId { get; private set; }
        public string baseDirectory { get; private set; }
        public string targetPath { get; private set; }
        public string candidatePath { get; private set; }
        public bool? found { get; private set; }
        public string selectedPath { get; private set; }
        public string errorCode { get; private set; }
        public string sourceDocument { get; private set; }
        public string stage { get; private set; }
        public string readSite { get; private set; }
        public string requestedName { get; private set; }
        public string requestedType { get; private set; }
        public string requestedInstance { get; private set; }
        public string providerDocument { get; private set; }
        public string providerType { get; private set; }
        public string providerInstance { get; private set; }
        public string providerLocation { get; private set; }
        public string value { get; private set; }
        public long scopeEpoch { get; private set; }
        public string templateStage { get; private set; }
        internal Observation(long sequence, string kind, long queryId, string taskId, string baseDirectory, string targetPath, string candidatePath, bool? found, string selectedPath, string errorCode,
            string source = null, string stage = null, string site = null, string name = null,
            string requestedType = null, string requestedInstance = null, string provider = null,
            string providerType = null, string providerInstance = null, string location = null,
            string value = null, long epoch = 0, string templateStage = null, long parentQueryId = 0)
        {
            this.sequence = sequence; this.kind = kind; this.queryId = queryId; this.taskId = taskId;
            this.baseDirectory = baseDirectory; this.targetPath = targetPath; this.candidatePath = candidatePath;
            this.found = found; this.selectedPath = selectedPath; this.errorCode = errorCode;
            sourceDocument = source; this.stage = stage; readSite = site; requestedName = name;
            this.requestedType = requestedType; this.requestedInstance = requestedInstance; providerDocument = provider;
            this.providerType = providerType; this.providerInstance = providerInstance; providerLocation = location;
            this.value = value; scopeEpoch = epoch; this.templateStage = templateStage; this.parentQueryId = parentQueryId;
        }
    }
    // Snapshot only immutable strings/scalars from actual legacy lookup results.
    // No Definition, InstanceHandle, DOM or collection is retained in the report.
    private void AddSemantic(string kind, long queryId, string source, string stage, string site, string name,
        string requestedType, string requestedInstance, string provider, string providerType, string providerInstance,
        string location, string value, bool? found, long epoch, string templateStage, string error, long parentQueryId = 0)
    {
        long sequence = ++_Sequence;
        long chars = TextLength(kind) + TextLength(_TaskId) + TextLength(source) + TextLength(stage) + TextLength(site) +
            TextLength(name) + TextLength(requestedType) + TextLength(requestedInstance) + TextLength(provider) +
            TextLength(providerType) + TextLength(providerInstance) + TextLength(location) + TextLength(value) + TextLength(templateStage) + TextLength(error);
        if (_Observations.Count >= _Limit || chars > _TextBudget - _RecordedTextChars) { _Dropped++; return; }
        _Observations.Add(new Observation(sequence, kind, queryId, _TaskId, null, null, null, found, null, error,
            source, stage, site, name, requestedType, requestedInstance, provider, providerType, providerInstance,
            location, value, epoch, templateStage, parentQueryId));
        _RecordedTextChars += chars;
    }
    internal static void RecordDefineRead(AssetDeclarationDocument document, string stage, string site, long epoch,
        string name, Definition definition, string value, bool succeeded)
    {
        DependencyObservationLog owner = _Current; if (owner == null) return;
        owner._DefineReads++;
        owner.AddSemantic("defineRead", ++owner._QueryId, document.ObservationSourcePath, stage, site, name, null, null,
            definition == null || definition.Document == null ? null : definition.Document.ObservationSourcePath, null, null,
            definition == null ? null : Object.ReferenceEquals(definition.Document, document) ? "Self" : "Imported",
            value, succeeded, epoch, null, succeeded ? null : ErrorCode.ExpressionEvaluationError.ToString());
    }
    internal static long BeginSymbolLookup()
    {
        DependencyObservationLog owner = _Current; if (owner == null) return 0;
        owner._SymbolLookups++; return ++owner._QueryId;
    }
    internal static long BeginSymbolSelection()
    {
        DependencyObservationLog owner = _Current; return owner == null ? 0 : ++owner._QueryId;
    }
    internal static void RecordSymbol(string kind, long queryId, AssetDeclarationDocument document, string stage, string site,
        InstanceHandle requested, FindLocation location, InstanceDeclaration provider, bool found, string error = null, long parentQueryId = 0)
    {
        DependencyObservationLog owner = _Current; if (owner == null) return;
        if (kind == "symbolCandidate") owner._SymbolCandidateChecks++;
        if (kind == "symbolSelection") owner._SymbolSelections++;
        owner.AddSemantic(kind, queryId, document.ObservationSourcePath, stage, site, null,
            requested == null ? null : requested.TypeName, requested == null ? null : requested.InstanceName,
            provider == null || provider.ObservationDocument == null ? null : provider.ObservationDocument.ObservationSourcePath,
            provider == null || provider.ObservationHandle == null ? null : provider.ObservationHandle.TypeName, provider == null || provider.ObservationHandle == null ? null : provider.ObservationHandle.InstanceName,
            location.ToString(), null, found, 0, null, error, parentQueryId);
    }
    internal static void RecordInheritance(AssetDeclarationDocument document, InstanceDeclaration consumer,
        InstanceDeclaration provider, FindLocation location, string stage)
    {
        DependencyObservationLog owner = _Current; if (owner == null) return;
        owner._InheritanceTemplates++;
        owner.AddSemantic("inheritanceTemplate", ++owner._QueryId, document.ObservationSourcePath, "inheritance", consumer.Handle.TypeName + ":" + consumer.Handle.InstanceName,
            null, consumer.InheritFromHandle.TypeName, consumer.InheritFromHandle.InstanceName,
            provider.ObservationDocument == null ? null : provider.ObservationDocument.ObservationSourcePath, provider.Handle.TypeName, provider.Handle.InstanceName,
            location.ToString(), provider.PrevalidationXmlHash.ToString(System.Globalization.CultureInfo.InvariantCulture), true, 0, stage, null);
    }
    public IDisposable BeginScope(string taskId)
    {
        if (taskId == null) throw new ArgumentNullException("taskId");
        if (_Current != null) throw new InvalidOperationException("A dependency observation scope is already active.");
        if (!_Enabled) return new DisabledScope();
        int thread = Thread.CurrentThread.ManagedThreadId;
        if (_Thread != 0 && _Thread != thread) throw new InvalidOperationException("Dependency observations belong to one build thread.");
        _Thread = thread; _TaskId = taskId; _Current = this; return new Scope(this);
    }
    private sealed class DisabledScope : IDisposable { public void Dispose() { } }
    private sealed class Scope : IDisposable
    {
        private DependencyObservationLog owner;
        internal Scope(DependencyObservationLog owner) { this.owner = owner; }
        public void Dispose()
        {
            if (owner == null) return;
            if (owner._Thread != Thread.CurrentThread.ManagedThreadId || !Object.ReferenceEquals(_Current, owner) || owner._Resolution != null)
                throw new InvalidOperationException("Dependency observation scopes must close after their queries on their owning thread.");
            _Current = null; owner._TaskId = null; owner = null;
        }
    }
    internal static bool IsActive { get { return _Current != null; } }
    internal static Resolution BeginResolution(string baseDirectory, string targetPath)
    {
        return _Current == null ? null : new Resolution(_Current, baseDirectory, targetPath);
    }
    internal sealed class Resolution : IDisposable
    {
        internal readonly DependencyObservationLog Owner;
        internal readonly Resolution Parent;
        internal readonly long Id;
        internal readonly string BaseDirectory, TargetPath;
        private bool completed, disposed;
        internal Resolution(DependencyObservationLog owner, string baseDirectory, string targetPath)
        {
            Owner = owner; Parent = owner._Resolution; BaseDirectory = baseDirectory; TargetPath = targetPath;
            Id = ++owner._QueryId; owner._ResolutionCalls++; owner._Resolution = this;
        }
        internal void Complete(string selectedPath)
        {
            if (completed) return; completed = true; Owner._Succeeded++;
            Owner.Add("resolve", this, null, null, selectedPath, null);
        }
        internal void Fail(Exception error)
        {
            if (completed) return; completed = true; Owner._Failed++;
            var buildError = error as BinaryAssetBuilderException;
            string code = buildError == null ? error.GetType().FullName : buildError.ErrorCode.ToString();
            Owner.Add("resolve", this, null, null, null, code);
        }
        public void Dispose()
        {
            if (disposed) return;
            if (!Object.ReferenceEquals(Owner._Resolution, this)) throw new InvalidOperationException("Resolver observations must close in stack order.");
            Owner._Resolution = Parent; disposed = true;
        }
    }
    // Keep the original call exactly once, even after the observer has filled.
    internal static bool RecordFileExists(string path)
    {
        bool found = File.Exists(path);
        DependencyObservationLog owner = _Current;
        if (owner != null)
        {
            owner._CandidateChecks++; if (found) owner._FoundChecks++; else owner._MissingChecks++;
            owner.Add("candidate", owner._Resolution, path, found, null, null);
        }
        return found;
    }
    private static long TextLength(string value) { return value == null ? 0 : value.Length; }
    private void Add(string kind, Resolution query, string candidate, bool? found, string selected, string error)
    {
        long sequence = ++_Sequence;
        string baseDirectory = query == null ? null : query.BaseDirectory, targetPath = query == null ? null : query.TargetPath;
        long textChars = TextLength(kind) + TextLength(_TaskId) + TextLength(baseDirectory) + TextLength(targetPath) +
            TextLength(candidate) + TextLength(selected) + TextLength(error);
        if (_Observations.Count >= _Limit || textChars > _TextBudget - _RecordedTextChars) { _Dropped++; return; }
        _Observations.Add(new Observation(sequence, kind, query == null ? 0 : query.Id, _TaskId,
            baseDirectory, targetPath, candidate, found, selected, error));
        _RecordedTextChars += textChars;
    }
    public object GetReport()
    {
        return new { enabled = _Enabled, scope = "ResolverCandidatesAndSemanticReads", canAuthorizeReuse = false, limit = _Limit, textBudget = _TextBudget, recordedTextChars = _RecordedTextChars,
            recorded = _Observations.Count, dropped = _Dropped, truncated = _Dropped != 0,
            resolutionCalls = _ResolutionCalls, candidateChecks = _CandidateChecks, foundChecks = _FoundChecks, missingChecks = _MissingChecks,
            resolutionsSucceeded = _Succeeded, resolutionsFailed = _Failed, defineReads = _DefineReads, symbolLookups = _SymbolLookups,
            symbolCandidateChecks = _SymbolCandidateChecks, symbolSelections = _SymbolSelections, inheritanceTemplates = _InheritanceTemplates, observations = _Observations.ToArray() };
    }
}
