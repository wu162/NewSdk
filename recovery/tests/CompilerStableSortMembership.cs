using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using BinaryAssetBuilder.Core;

[Serializable]
public sealed class StableMembershipCase
{
    public string name, culture = "en-US";
    public int count = 40, extraBindings;
    public bool legacyBindings, customComparer, customDictionary, customCulture, derivedHandle, derivedDeclaration, derivedSet, derivedList, duplicateOutput, traceMutation, swapSchemaAfterTypeSort, nullSchemaAfterTypeSort, collision, xorQuery, entryBudget;
}
[Serializable]
public sealed class StableMembershipResult
{
    public string outcome, metrics;
    public long elapsedTicks;
}
// Shared reflection adapter: synthetic graphs feed real StableSort and the
// original private insertion helper. No original Core object crosses AppDomains.
public sealed class StableMembershipRunner
{
    const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    readonly Assembly core;
    readonly Type docType, instanceType, handleType, setType, processorType, settingsType, schemaType, tracerType;
    readonly JavaScriptSerializer json = new JavaScriptSerializer();
    object document, current, manager, processor, settings, oldSettings, tracer;
    object[] instances;
    IList outputs;
    IDictionary<uint, int> counts;
    StableMembershipCase spec;
    object oldWriter, oldMask, oldTraceEnabled;
    FieldInfo writerField, maskField;
    int callbacks;
    bool schemaMapReplaced, schemaSetNull;
    Type derivedHandleType;
    readonly Dictionary<Type, Type> plainDerived = new Dictionary<Type, Type>();
    public StableMembershipRunner(Assembly assembly)
    {
        core = assembly; docType = core.GetType("BinaryAssetBuilder.Core.AssetDeclarationDocument", true);
        instanceType = core.GetType("BinaryAssetBuilder.Core.InstanceDeclaration", true); handleType = core.GetType("BinaryAssetBuilder.Core.InstanceHandle", true);
        setType = core.GetType("BinaryAssetBuilder.Core.InstanceHandleSet", true); processorType = core.GetType("BinaryAssetBuilder.Core.DocumentProcessor", true);
        settingsType = core.GetType("BinaryAssetBuilder.Settings", true); schemaType = core.GetType("BinaryAssetBuilder.Core.SchemaSet", true);
        tracerType = core.GetType("BinaryAssetBuilder.Core.Tracer", true);
    }
    static FieldInfo Field(Type type, string name)
    {
        for (Type at = type; at != null; at = at.BaseType)
        { var field = at.GetField(name, All | BindingFlags.DeclaredOnly); if (field != null) return field; }
        throw new Exception("Missing graph-adapter field " + type.FullName + "." + name);
    }
    static object Get(object owner, string name) { return Field(owner.GetType(), name).GetValue(owner); }
    static void Set(object owner, string name, object value) { Field(owner.GetType(), name).SetValue(owner, value); }
    object Handle(object instance) { return Get(Get(instance, "_Current"), "Handle"); }
    public void EqualityCallback() { callbacks++; Set(Handle(instances[0]), "_InstanceId", 0x50000001u); }
    Type DerivedHandle()
    {
        if (derivedHandleType != null) return derivedHandleType;
        var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("SortEquals" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run);
        var builder = assembly.DefineDynamicModule("Probe").DefineType("DerivedSortHandle", TypeAttributes.Public, handleType);
        builder.DefineDefaultConstructor(MethodAttributes.Public);
        var hook = builder.DefineField("Once", typeof(Action), FieldAttributes.Public);
        var method = builder.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.Virtual, typeof(bool), new[] { typeof(object) });
        var il = method.GetILGenerator(); var action = il.DeclareLocal(typeof(Action)); var skip = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldfld, hook); il.Emit(OpCodes.Stloc, action);
        il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldnull); il.Emit(OpCodes.Stfld, hook);
        il.Emit(OpCodes.Ldloc, action); il.Emit(OpCodes.Brfalse_S, skip); il.Emit(OpCodes.Ldloc, action); il.Emit(OpCodes.Callvirt, typeof(Action).GetMethod("Invoke"));
        il.MarkLabel(skip); il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldarg_1);
        var original = handleType.GetMethod("Equals", new[] { typeof(object) }); il.Emit(OpCodes.Call, original); il.Emit(OpCodes.Ret);
        builder.DefineMethodOverride(method, original); derivedHandleType = builder.CreateType(); return derivedHandleType;
    }
    Type PlainDerived(Type parent)
    {
        Type type;
        if (plainDerived.TryGetValue(parent, out type)) return type;
        var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("SortDerived" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run);
        var builder = assembly.DefineDynamicModule("Probe").DefineType("Derived" + plainDerived.Count, TypeAttributes.Public, parent);
        builder.DefineDefaultConstructor(MethodAttributes.Public); type = builder.CreateType(); plainDerived.Add(parent, type); return type;
    }
    sealed class WrappedDictionary : Dictionary<uint, int> { }
    sealed class ReverseUInt : IComparer<uint> { public int Compare(uint left, uint right) { return right.CompareTo(left); } }
    sealed class WrappedCulture : CultureInfo
    {
        internal WrappedCulture(string name) : base(name) { }
        public override CompareInfo CompareInfo { get { return base.CompareInfo; } }
    }
    object NewInstance(int index)
    {
        object value = Activator.CreateInstance(spec.derivedDeclaration && index == 1 ? PlainDerived(instanceType) : instanceType);
        object state = FormatterServices.GetUninitializedObject(Field(instanceType, "_Current").FieldType);
        object handle = Activator.CreateInstance(spec.derivedHandle && index == 1 ? DerivedHandle() : handleType);
        uint type = spec.collision ? (uint)(index + 1) : (index % 2 == 0 ? 101u : 202u);
        uint id = spec.collision ? (index == 0 ? 10u : 9u) : unchecked((uint)(index + 1));
        if (spec.xorQuery && index == spec.count - 1) { type = 202u; id = 174u; } // same XOR as (101,1), different full identity
        Set(handle, "_TypeId", type); Set(handle, "_InstanceId", id); Set(handle, "_TypeHash", 1u);
        Set(handle, "_TypeName", "Type" + type); Set(handle, "_InstanceName", new[] { "I", "İ", "ı", "i", "ä", "A" }[index % 6] + index.ToString("D3"));
        Set(state, "Handle", handle); Set(state, "AllDependentInstances", Activator.CreateInstance(spec.derivedSet && index == spec.count - 1 ? PlainDerived(setType) : setType));
        Field(instanceType, "_Current").SetValue(value, state);
        if (!counts.ContainsKey(type)) counts.Add(type, (int)(type % 3));
        return value;
    }
    void Prepare(StableMembershipCase value)
    {
        spec = value; callbacks = 0; schemaMapReplaced = false; schemaSetNull = false;
        oldSettings = settingsType.GetField("Current", All).GetValue(null);
        settings = Activator.CreateInstance(settingsType); settingsType.GetProperty("BuildCache").SetValue(settings, false, null);
        settingsType.GetField("Current", All).SetValue(null, settings);
        counts = spec.customDictionary ? (IDictionary<uint, int>)new WrappedDictionary() :
            spec.customComparer ? new SortedDictionary<uint, int>(new ReverseUInt()) : new SortedDictionary<uint, int>();
        document = Activator.CreateInstance(docType); current = Activator.CreateInstance(docType.GetNestedType("CurrentState", BindingFlags.NonPublic), true);
        Field(docType, "_Current").SetValue(document, current); Set(current, "SourcePath", "reflection-graph://" + spec.name);
        processor = processorType.GetMethod("RecordStableSortMembershipWork", All) == null ? FormatterServices.GetUninitializedObject(processorType)
            : Activator.CreateInstance(processorType, new[] { settings, null, null });
        object schemas = FormatterServices.GetUninitializedObject(schemaType); Set(schemas, "_AssetDependencies", counts);
        processorType.GetProperty("SchemaSet").SetValue(processor, schemas, null); Set(current, "DocumentProcessor", processor);
        manager = FormatterServices.GetUninitializedObject(core.GetType("BinaryAssetBuilder.Core.OutputManager", true));
        instances = new object[spec.count + spec.extraBindings];
        for (int index = 0; index < instances.Length; index++) instances[index] = NewInstance(index);
        if (spec.xorQuery) { counts[101u] = 0; counts[202u] = 1; }
        MethodInfo begin = docType.GetMethod("BeginStreamBindings", All);
        if (begin != null && !spec.legacyBindings) begin.Invoke(document, null);
        IDictionary bindings = begin != null && !spec.legacyBindings ? (IDictionary)Get(current, "StreamBindings") : null;
        for (int index = 0; index < instances.Length; index++)
        {
            object dependencies = Get(Get(instances[index], "_Current"), "AllDependentInstances");
            bool aliasBudgetSet = spec.entryBudget && index >= spec.count;
            if (aliasBudgetSet) dependencies = Get(Get(instances[spec.count - 1], "_Current"), "AllDependentInstances");
            if (bindings != null)
            {
                object binding = Activator.CreateInstance(core.GetType("BinaryAssetBuilder.Core.StreamBindingState", true), true);
                if (aliasBudgetSet) Set(binding, "Dependencies", dependencies);
                else if (spec.derivedSet && index == spec.count - 1) Set(binding, "Dependencies", Activator.CreateInstance(PlainDerived(setType)));
                dependencies = Get(binding, "Dependencies"); bindings.Add(instances[index], binding);
            }
            Set(Get(instances[index], "_Current"), "AllDependentInstances", dependencies);
            int dependencyLimit = aliasBudgetSet || (spec.xorQuery && index == spec.count - 1) ? 0 : Math.Min(index, spec.count);
            for (int prior = 0; prior < dependencyLimit; prior++)
                setType.GetMethod("TryAdd").Invoke(dependencies, new[] { Handle(instances[prior]) });
        }
        outputs = (IList)Get(current, "OutputInstances");
        if (spec.derivedList) { outputs = (IList)Activator.CreateInstance(PlainDerived(outputs.GetType())); Set(current, "OutputInstances", outputs); }
        for (int index = spec.count - 1; index >= 0; index--) outputs.Add(instances[index]);
        if (spec.duplicateOutput) outputs.Add(instances[spec.count / 2]);
        if (spec.derivedHandle) Handle(instances[1]).GetType().GetField("Once").SetValue(Handle(instances[1]), (Action)EqualityCallback);
        writerField = tracerType.GetField("TraceWrite", All); maskField = tracerType.GetField("_TraceMask", All);
        oldWriter = writerField.GetValue(null); oldMask = maskField.GetValue(null);
        tracer = Field(docType, "_Tracer").GetValue(null); oldTraceEnabled = Get(tracer, "_TraceEnabled");
        Set(tracer, "_TraceEnabled", false); tracerType.GetMethod("SetTraceLevel").Invoke(null, new object[] { 9 });
        writerField.SetValue(null, spec.traceMutation || spec.swapSchemaAfterTypeSort || spec.nullSchemaAfterTypeSort ? Delegate.CreateDelegate(writerField.FieldType, this, GetType().GetMethod("Trace")) : null);
    }
    public void Trace(string source, TraceEventType kind, string message)
    {
        if (message == "Sorting by asset dependencies" && callbacks == 0)
        {
            callbacks++;
            if (spec.traceMutation) Set(Handle(instances[0]), "_InstanceId", 0x60000001u);
            if (spec.swapSchemaAfterTypeSort)
            {
                object replacement = FormatterServices.GetUninitializedObject(schemaType);
                Set(replacement, "_AssetDependencies", new SortedDictionary<uint, int>(counts));
                processorType.GetProperty("SchemaSet").SetValue(processor, replacement, null);
                schemaMapReplaced = true;
            }
            if (spec.nullSchemaAfterTypeSort)
            {
                processorType.GetProperty("SchemaSet").SetValue(processor, null, null);
                schemaMapReplaced = true;
                schemaSetNull = processorType.GetProperty("SchemaSet").GetValue(processor, null) == null;
            }
        }
    }
    object Order(IList list)
    {
        return list.Cast<object>().Select(value => new { label = Array.FindIndex(instances, item => Object.ReferenceEquals(item, value)),
            type = Get(Handle(value), "_TypeId"), id = Get(Handle(value), "_InstanceId") }).ToArray();
    }
    void Restore()
    {
        if (writerField != null) writerField.SetValue(null, oldWriter);
        if (maskField != null) maskField.SetValue(null, oldMask);
        if (tracer != null) Set(tracer, "_TraceEnabled", oldTraceEnabled);
        settingsType.GetField("Current", All).SetValue(null, oldSettings);
        writerField = null; maskField = null; tracer = null;
    }
    public string CallbackBoundary()
    {
        CultureInfo previous = CultureInfo.CurrentCulture; bool previousSwitch;
        AppContext.TryGetSwitch("NewSdk.Compiler.DisableStableSortMembershipIndex", out previousSwitch);
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            AppContext.SetSwitch("NewSdk.Compiler.DisableStableSortMembershipIndex", false);
            Prepare(new StableMembershipCase { name = "callback-boundary", traceMutation = true });
            Type helper = core.GetType("BinaryAssetBuilder.Core.CompilerStableSortMembership", true);
            object work = Activator.CreateInstance(helper.GetNestedType("Work", BindingFlags.NonPublic), true);
            object scope = helper.GetMethod("Create", All).Invoke(null, new object[] { true, Get(current, "StreamBindings"), outputs,
                Activator.CreateInstance(outputs.GetType()), counts, work });
            object dependencies = Get(Get(instances[instances.Length - 1], "_Current"), "AllDependentInstances");
            object target = Handle(instances[0]); MethodInfo contains = helper.GetMethod("Contains", All);
            bool before = (bool)contains.Invoke(scope, new[] { dependencies, target });
            tracerType.GetMethod("TraceInfo", new[] { typeof(string), typeof(object[]) }).Invoke(tracer, new object[] { "Sorting by asset dependencies", new object[0] });
            bool after = (bool)contains.Invoke(scope, new[] { dependencies, target });
            bool legacy = (bool)setType.GetMethod("Contains", new[] { handleType }).Invoke(dependencies, new[] { target });
            bool again = (bool)contains.Invoke(scope, new[] { dependencies, target });
            object snapshot = work.GetType().GetMethod("Snapshot", All).Invoke(work, null);
            return json.Serialize(new { before, after, legacy, again, callbacks, work = snapshot });
        }
        finally { Restore(); CultureInfo.CurrentCulture = previous; AppContext.SetSwitch("NewSdk.Compiler.DisableStableSortMembershipIndex", previousSwitch); }
    }
    public StableMembershipResult Run(StableMembershipCase value, bool disabled, bool privatePositions)
    {
        CultureInfo oldCulture = CultureInfo.CurrentCulture; bool oldSwitch; AppContext.TryGetSwitch("NewSdk.Compiler.DisableStableSortMembershipIndex", out oldSwitch);
        string error = null; long ticks = 0; object order = null; var positions = new List<int>(); string metrics = "null";
        try
        {
            AppContext.SetSwitch("NewSdk.Compiler.DisableStableSortMembershipIndex", disabled);
            CultureInfo.CurrentCulture = value.customCulture ? new WrappedCulture(value.culture) : new CultureInfo(value.culture);
            Prepare(value);
            if (privatePositions)
            {
                object compare = Activator.CreateInstance(docType.GetNestedType("TypeDepCompare", BindingFlags.NonPublic), All, null, new object[] { counts }, null);
                outputs.GetType().GetMethods().Single(method => method.Name == "Sort" && method.GetParameters().Length == 1 &&
                    method.GetParameters()[0].ParameterType.IsGenericType && method.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IComparer<>)).Invoke(outputs, new[] { compare });
                IList final = (IList)Activator.CreateInstance(outputs.GetType());
                foreach (object instance in outputs)
                {
                    int position = (int)docType.GetMethod("StableSort_CalcInsertPosition", All).Invoke(null, new[] { final, instance, compare });
                    positions.Add(position); final.Insert(position, instance);
                }
                order = Order(final);
            }
            else
            {
                var watch = Stopwatch.StartNew(); docType.GetMethod("StableSort").Invoke(document, new[] { manager }); watch.Stop(); ticks = watch.ElapsedTicks;
                order = Order((IList)Get(current, "OutputInstances"));
                MethodInfo getMetrics = docType.GetMethod("GetStableSortMembershipMetrics"); if (getMetrics != null) metrics = json.Serialize(getMetrics.Invoke(document, null));
            }
        }
        catch (Exception failure)
        {
            while (failure is TargetInvocationException && failure.InnerException != null) failure = failure.InnerException;
            error = failure.GetType().Name + ":" + failure.Message;
        }
        finally { Restore(); CultureInfo.CurrentCulture = oldCulture; AppContext.SetSwitch("NewSdk.Compiler.DisableStableSortMembershipIndex", oldSwitch); }
        return new StableMembershipResult { outcome = json.Serialize(new { error, positions = positions.ToArray(), order, callbacks, schemaMapReplaced, schemaSetNull }), metrics = metrics, elapsedTicks = ticks };
    }
}
public sealed class LegacyStableMembershipDomain : MarshalByRefObject
{
    StableMembershipRunner runner;
    public override object InitializeLifetimeService() { return null; }
    public string Initialize(string path)
    {
        Assembly core = Assembly.LoadFrom(path);
        if (!Path.GetFullPath(core.Location).Equals(Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase)) throw new Exception("Original Core resolved outside fixed SDK path");
        runner = new StableMembershipRunner(core); return core.FullName;
    }
    public StableMembershipResult Run(StableMembershipCase value, bool positions) { return runner.Run(value, true, positions); }
}
public static class CompilerStableSortMembership
{
    static int checks;
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static readonly List<object> evidence = new List<object>();
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static Dictionary<string, object> Work(StableMembershipResult result) { return Json.DeserializeObject(result.metrics) as Dictionary<string, object>; }
    public static int Main(string[] args)
    {
        int code = 0; object result; AppDomain domain = null; bool priorBindings;
        AppContext.TryGetSwitch("NewSdk.Compiler.DisableStreamBindingResults", out priorBindings);
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4, "Expected x86 work result source run SDK"); Directory.CreateDirectory(args[0]);
            domain = AppDomain.CreateDomain("stable-membership-original-" + Guid.NewGuid().ToString("N"), null,
                new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(typeof(CompilerStableSortMembership).Assembly.Location), ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile });
            var original = (LegacyStableMembershipDomain)domain.CreateInstanceFromAndUnwrap(typeof(LegacyStableMembershipDomain).Assembly.Location, typeof(LegacyStableMembershipDomain).FullName);
            string originalLibrary = Path.Combine(args[4], "tools/BinaryAssetBuilder.Core.dll");
            string originalIdentity = original.Initialize(originalLibrary);
            Require(originalIdentity.StartsWith("BinaryAssetBuilder.Core,", StringComparison.Ordinal), "Original SDK Core identity mismatch");
            string originalSha256;
            using (var sha = System.Security.Cryptography.SHA256.Create())
                originalSha256 = String.Concat(sha.ComputeHash(File.ReadAllBytes(originalLibrary)).Select(value => value.ToString("x2")));
            AppContext.SetSwitch("NewSdk.Compiler.DisableStreamBindingResults", false);
            var current = new StableMembershipRunner(typeof(AssetDeclarationDocument).Assembly);
            var cases = new List<StableMembershipCase>();
            foreach (string culture in new[] { "en-US", "tr-TR", "de-DE", "zh-CN" })
                foreach (int count in new[] { 1, 15, 16, 17, 40 }) cases.Add(new StableMembershipCase { name = culture + "-" + count, culture = culture, count = count });
            cases.Add(new StableMembershipCase { name = "legacy-bindings", legacyBindings = true });
            cases.Add(new StableMembershipCase { name = "custom-schema-comparer", customComparer = true });
            cases.Add(new StableMembershipCase { name = "custom-schema-dictionary", customDictionary = true });
            cases.Add(new StableMembershipCase { name = "captured-custom-map-followed-by-trace-schema-swap", customDictionary = true, swapSchemaAfterTypeSort = true });
            cases.Add(new StableMembershipCase { name = "last-trace-schema-null", nullSchemaAfterTypeSort = true });
            cases.Add(new StableMembershipCase { name = "custom-culture", customCulture = true });
            cases.Add(new StableMembershipCase { name = "derived-Equals-untraced-mutation", derivedHandle = true });
            cases.Add(new StableMembershipCase { name = "derived-declaration", derivedDeclaration = true });
            cases.Add(new StableMembershipCase { name = "derived-dependency-set", derivedSet = true });
            cases.Add(new StableMembershipCase { name = "derived-output-list", derivedList = true });
            cases.Add(new StableMembershipCase { name = "duplicate-output-reference", duplicateOutput = true });
            cases.Add(new StableMembershipCase { name = "last-trace-mutates-live-identity", traceMutation = true });
            cases.Add(new StableMembershipCase { name = "scope-budget", count = 32, extraBindings = 993 });
            // Alias an already constructed exact 79-element set into extra
            // non-output bindings. Validate cumulative-entry refusal without
            // building a huge closure or sorting more than 80 declarations.
            cases.Add(new StableMembershipCase { name = "entry-budget-shared-exact-set", count = 80, extraBindings = 800, entryBudget = true });
            cases.Add(new StableMembershipCase { name = "same-XOR-distinct-query-full-identity", count = 18, xorQuery = true });
            cases.Add(new StableMembershipCase { name = "original-XOR-key-collision-error", count = 3, collision = true });
            foreach (var item in cases)
            {
                var expected = original.Run(item, false); var actual = current.Run(item, false, false); var fallback = current.Run(item, true, false);
                Require(expected.outcome == actual.outcome && expected.outcome == fallback.outcome, "Actual StableSort original/fallback differential: " + item.name + "\n" + expected.outcome + "\n" + actual.outcome + "\n" + fallback.outcome);
                string privateInsertionPositions = null;
                if (!item.traceMutation)
                {
                    var expectedPositions = original.Run(item, true); var positions = current.Run(item, false, true);
                    Require(expectedPositions.outcome == positions.outcome, "Original private insertion-position differential: " + item.name + "\n" + expectedPositions.outcome + "\n" + positions.outcome);
                    privateInsertionPositions = positions.outcome;
                }
                var metrics = Work(actual);
                if (item.name == "en-US-40") Require(metrics != null && Convert.ToInt64(metrics["builds"]) > 0 && Convert.ToInt64(metrics["indexQueries"]) > 0, "Actual StableSort did not use admitted membership indexes");
                if (item.legacyBindings || item.customComparer || item.customDictionary || item.customCulture || item.derivedHandle || item.derivedDeclaration || item.derivedSet || item.derivedList || item.extraBindings > 0)
                    Require(metrics != null && Convert.ToInt64(metrics["indexQueries"]) == 0 && Convert.ToInt64(metrics["builds"]) == 0, "Unsafe scope was indexed: " + item.name);
                if (item.swapSchemaAfterTypeSort)
                {
                    var actualRow = Json.Deserialize<Dictionary<string, object>>(actual.outcome);
                    Require((bool)actualRow["schemaMapReplaced"] && Convert.ToInt64(actualRow["callbacks"]) == 1 && metrics != null &&
                        (string)metrics["scopeReason"] == "custom-schema-dependency-dictionary-or-comparer",
                        "Eligibility guard did not follow the custom map captured by the actual TypeDepCompare across final-trace schema replacement");
                }
                if (item.nullSchemaAfterTypeSort)
                {
                    var actualRow = Json.Deserialize<Dictionary<string, object>>(actual.outcome);
                    Require((bool)actualRow["schemaMapReplaced"] && (bool)actualRow["schemaSetNull"] && Convert.ToInt64(actualRow["callbacks"]) == 1,
                        "Last-trace callback did not actually clear the processor SchemaSet");
                    // Limit this assertion to the original public StableSort's
                    // local successful contract. Do not infer that later Core
                    // processing supports an absent SchemaSet.
                    if (actualRow["error"] == null)
                        Require(metrics != null && (bool)metrics["enabled"] && Convert.ToInt64(metrics["builds"]) > 0 && Convert.ToInt64(metrics["indexQueries"]) > 0,
                            "StableSort eligibility reread a replaced/null SchemaSet instead of the standard map captured by its comparer");
                }
                if (item.entryBudget) Require(metrics != null && (string)metrics["scopeReason"] == "entry-budget" && Convert.ToInt64(metrics["budgetBypasses"]) > 0,
                    "Cumulative entry budget did not reject the actual aliased-binding scope");
                if (item.xorQuery) Require(metrics != null && Convert.ToInt64(metrics["indexQueries"]) > 0, "Full-identity differential never used indexed membership");
                if (item.collision) Require(expected.outcome.Contains("ArgumentException"), "Original XOR-key collision construction rejection was not retained");
                evidence.Add(new { id = item.name, outcome = actual.outcome, privateInsertionPositions, work = metrics });
            }
            string callbackEvidence = current.CallbackBoundary();
            var boundary = Json.Deserialize<Dictionary<string, object>>(callbackEvidence);
            var boundaryWork = (Dictionary<string, object>)boundary["work"];
            Require((bool)boundary["before"] && (bool)boundary["after"] && (bool)boundary["legacy"] && (bool)boundary["again"] &&
                Convert.ToInt64(boundary["callbacks"]) == 1 && Convert.ToInt64(boundaryWork["indexQueries"]) == 1 &&
                Convert.ToInt64(boundaryWork["callbackFallbacks"]) == 1 && Convert.ToInt64(boundaryWork["linearQueries"]) == 2,
                "Callback-version change failed to permanently restore actual legacy membership: " + callbackEvidence);
            evidence.Add(new { id = "direct-local-scope-callback-boundary-after-actual-sort-differentials", outcome = callbackEvidence });
            // Bounded actual StableSort timings, including scope admission/builds;
            // graph construction and original comparisons remain outside timers.
            var samples = new List<object>(); var enabledTicks = new List<long>(); var disabledTicks = new List<long>();
            var benchmark = new StableMembershipCase { name = "component-actual-sort", count = 64 };
            string unchanged = original.Run(benchmark, false).outcome;
            for (int round = 0; round < 7; round++) foreach (bool disabled in round % 2 == 0 ? new[] { false, true } : new[] { true, false })
            {
                var sample = current.Run(benchmark, disabled, false); Require(sample.outcome == unchanged, "Measured actual StableSort changed original ordering");
                (disabled ? disabledTicks : enabledTicks).Add(sample.elapsedTicks);
                samples.Add(new { round, disabled, elapsedTicks = sample.elapsedTicks, elapsedMs = sample.elapsedTicks * 1000.0 / Stopwatch.Frequency, work = Work(sample) });
            }
            enabledTicks.Sort(); disabledTicks.Sort();
            evidence.Add(new { id = "bounded-actual-StableSort-component-measurement", sampleCountPerMode = 7, declarations = 64, frequency = Stopwatch.Frequency,
                medianEnabledTicks = enabledTicks[3], medianDisabledTicks = disabledTicks[3], samples = samples.ToArray(),
                scope = "Actual public StableSort on reflection-supplied fixed dependency graphs; includes scope preparation/index builds and reflection invoke; no speed assertion, CLI speedup or memory-peak claim" });
            AppDomain.Unload(domain); domain = null;
            Require(!AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name == "BinaryAssetBuilder.Core"), "Original Core leaked into production host AppDomain");
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = new[] { "isolated-original-private-insert-position-and-actual-StableSort-differential", "exact-culture-legacy-derived-budget-collision-contracts", "bounded-interleaved-actual-StableSort-measurement" }, originalIdentity, originalSha256, evidence = evidence.ToArray(),
                scope = "Reflection-supplied graph component differential against original Core plus real public StableSort; independent real streams are a separate fixture" };
            Console.WriteLine("PASS StableSortMembership: " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, evidence = evidence.ToArray(), error = error.ToString() };
        }
        finally { if (domain != null) AppDomain.Unload(domain); AppContext.SetSwitch("NewSdk.Compiler.DisableStreamBindingResults", priorBindings); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
