using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using BinaryAssetBuilder.Core;

[Serializable]
public sealed class BaseManifestCase
{
    public string name;
    public int[] candidates, bases, patched = new int[0];
    public uint[] candidateTypes, baseTypes;
    public int equalityMutation;
    public int callback, nullCandidate = -1, uninitializedCandidate = -1, zeroTypeHash = -1, nullBase = -1;
    public bool sameLists, nullOutput, nullFinal;
}
[Serializable]
public sealed class BaseManifestResult
{
    public string outcome, metrics;
    public int callbacks;
}

// Reflection-only adapter also runs in the original SDK's disposable AppDomain.
// Every asset entry owns real native header bytes; no original runtime object or
// assembly is returned to the production validation host.
public sealed class BaseManifestReflectionRunner
{
    const BindingFlags Fields = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    readonly Assembly core, utility;
    readonly Type documentType, managerType, instanceType, handleType, binaryType, manifestType, assetType, tracerType;
    readonly Func<IntPtr, Type, object> pointer;
    readonly List<IntPtr> allocations = new List<IntPtr>();
    object manager, document; IList inputs, final; object[] instances; BaseManifestCase spec;
    int callbacks; bool acted; Type derivedInstanceType;
    public BaseManifestReflectionRunner(Assembly core, Assembly utility)
    {
        this.core = core; this.utility = utility;
        documentType = core.GetType("BinaryAssetBuilder.Core.AssetDeclarationDocument", true);
        managerType = core.GetType("BinaryAssetBuilder.Core.OutputManager", true);
        instanceType = core.GetType("BinaryAssetBuilder.Core.InstanceDeclaration", true);
        handleType = core.GetType("BinaryAssetBuilder.Core.InstanceHandle", true);
        binaryType = core.GetType("BinaryAssetBuilder.Core.BinaryAsset", true);
        tracerType = core.GetType("BinaryAssetBuilder.Core.Tracer", true);
        manifestType = utility.GetType("BinaryAssetBuilder.Utility.Manifest", true);
        assetType = utility.GetType("BinaryAssetBuilder.Utility.Asset", true);
        var method = new DynamicMethod("BoxNativeAssetEntry", typeof(object), new[] { typeof(IntPtr), typeof(Type) }, typeof(BaseManifestReflectionRunner), true);
        var il = method.GetILGenerator(); il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Conv_U); il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, typeof(Pointer).GetMethod("Box", BindingFlags.Public | BindingFlags.Static)); il.Emit(OpCodes.Ret);
        pointer = (Func<IntPtr, Type, object>)method.CreateDelegate(typeof(Func<IntPtr, Type, object>));
    }
    static FieldInfo Field(Type type, string name)
    {
        for (Type current = type; current != null; current = current.BaseType)
        { var field = current.GetField(name, Fields | BindingFlags.DeclaredOnly); if (field != null) return field; }
        throw new Exception("Missing fixture field: " + type.FullName + "." + name);
    }
    static void Set(object owner, string name, object value) { Field(owner.GetType(), name).SetValue(owner, value); }
    static object Get(object owner, string name) { return Field(owner.GetType(), name).GetValue(owner); }
    object Handle(int id, uint type)
    {
        object handle = Activator.CreateInstance(handleType, new object[] { type, unchecked((uint)id) });
        Set(handle, "_TypeName", "FixtureType" + type); Set(handle, "_InstanceName", "Fixture" + id);
        handleType.GetProperty("TypeHash").SetValue(handle, 1u, null); handleType.GetProperty("InstanceHash").SetValue(handle, 1u, null); return handle;
    }
    Type DerivedInstanceType()
    {
        if (derivedInstanceType != null) return derivedInstanceType;
        var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("BaseManifestEqualsProbe" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run);
        var builder = assembly.DefineDynamicModule("Probe").DefineType("MutatingInstanceDeclaration", TypeAttributes.Public | TypeAttributes.Class, instanceType);
        builder.DefineDefaultConstructor(MethodAttributes.Public);
        var hook = builder.DefineField("OnEquals", typeof(Action), FieldAttributes.Public);
        var method = builder.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(bool), new[] { typeof(object) });
        var il = method.GetILGenerator(); var action = il.DeclareLocal(typeof(Action)); var noAction = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldfld, hook); il.Emit(OpCodes.Stloc, action);
        il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldnull); il.Emit(OpCodes.Stfld, hook);
        il.Emit(OpCodes.Ldloc, action); il.Emit(OpCodes.Brfalse_S, noAction); il.Emit(OpCodes.Ldloc, action); il.Emit(OpCodes.Callvirt, typeof(Action).GetMethod("Invoke"));
        il.MarkLabel(noAction); il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Ldarg_1);
        MethodInfo baseEquals = instanceType.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(object) }, null);
        il.Emit(OpCodes.Call, baseEquals); il.Emit(OpCodes.Ret); builder.DefineMethodOverride(method, baseEquals);
        derivedInstanceType = builder.CreateType(); return derivedInstanceType;
    }
    object Instance(int id, uint type, bool derived = false)
    {
        object instance = Activator.CreateInstance(derived ? DerivedInstanceType() : instanceType);
        FieldInfo current = Field(instanceType, "_Current"); object state = FormatterServices.GetUninitializedObject(current.FieldType);
        Set(state, "Handle", Handle(id, type)); current.SetValue(instance, state); return instance;
    }
    object Asset(int id, uint type)
    {
        object asset = FormatterServices.GetUninitializedObject(assetType); IntPtr memory = Marshal.AllocHGlobal(48); allocations.Add(memory);
        for (int offset = 0; offset < 48; offset += 4) Marshal.WriteInt32(memory, offset, 0);
        Marshal.WriteInt32(memory, 0, unchecked((int)type)); Marshal.WriteInt32(memory, 4, id);
        FieldInfo entry = Field(assetType, "m_AssetEntry"); entry.SetValue(asset, pointer(memory, entry.FieldType)); return asset;
    }
    string FileBase(object instance) { return (string)handleType.GetProperty("FileBase").GetValue(Get(Get(instance, "_Current"), "Handle"), null); }
    void Binary(IDictionary assets, object instance, bool reusable)
    {
        if (instance == null || Get(instance, "_Current") == null) return;
        object binary = FormatterServices.GetUninitializedObject(binaryType); Set(binary, "_Instance", instance); Set(binary, "_Parent", manager);
        FieldInfo availability = Field(binaryType, "_BasePatchStreamAvailability"); availability.SetValue(binary, Enum.ToObject(availability.FieldType, reusable ? 2 : 1));
        assets[FileBase(instance)] = binary;
    }
    int Label(object instance)
    {
        if (instance == null) return -1;
        for (int index = 0; index < instances.Length; index++) if (Object.ReferenceEquals(instance, instances[index])) return index;
        return -2;
    }
    string[] Shape(IList list)
    {
        if (list == null) return null;
        var result = new string[list.Count];
        for (int index = 0; index < list.Count; index++)
        {
            object instance = list[index]; int label = Label(instance);
            if (instance == null) { result[index] = "null"; continue; }
            try
            {
                object state = Get(instance, "_Current"), handle = state == null ? null : Get(state, "Handle");
                result[index] = label + ":" + (handle == null ? "uninitialized" : handleType.GetProperty("TypeId").GetValue(handle, null) + ":" + handleType.GetProperty("InstanceId").GetValue(handle, null));
            }
            catch (Exception error) { result[index] = label + ":state-error:" + error.GetType().Name; }
        }
        return result;
    }
    public void Trace(string source, TraceEventType kind, string message)
    {
        callbacks++;
        if (acted) return; acted = true;
        if (spec.callback == 2) inputs.RemoveAt(0);
        else if (spec.callback == 3) inputs.Insert(0, instances[instances.Length - 1]);
        else if (spec.callback == 4) Set(Get(instances[0], "_Current"), "Handle", Handle(spec.bases[0], spec.baseTypes == null ? 7u : spec.baseTypes[0]));
        else if (spec.callback == 5) Set(Get(instances[spec.bases[0]], "_Current"), "Handle", Handle(spec.candidates[0], spec.candidateTypes == null ? 7u : spec.candidateTypes[0]));
        else if (spec.callback == 6) throw new InvalidOperationException("injected base-manifest trace failure");
        else if (spec.callback == 7) inputs.Clear();
        else if (spec.callback == 8) { object first = inputs[0]; inputs[0] = inputs[inputs.Count - 1]; inputs[inputs.Count - 1] = first; }
    }
    public BaseManifestResult Run(BaseManifestCase value, bool disableIndex)
    {
        spec = value; callbacks = 0; acted = false; allocations.Clear();
        string oldEnvironment = Environment.GetEnvironmentVariable("BabEnableTrace"); Environment.SetEnvironmentVariable("BabEnableTrace", "False");
        bool oldSwitch; AppContext.TryGetSwitch("NewSdk.Compiler.DisableBaseManifestIndex", out oldSwitch);
        AppContext.SetSwitch("NewSdk.Compiler.DisableBaseManifestIndex", disableIndex);
        object oldWriter = null, oldMask = null; FieldInfo writerField = null, maskField = null;
        try
        {
            document = Activator.CreateInstance(documentType); manager = FormatterServices.GetUninitializedObject(managerType);
            object manifest = FormatterServices.GetUninitializedObject(manifestType);
            var assetsArray = System.Array.CreateInstance(assetType, value.bases.Length);
            for (int index = 0; index < value.bases.Length; index++)
                if (index != value.nullBase) assetsArray.SetValue(Asset(value.bases[index], value.baseTypes == null ? 7u : value.baseTypes[index]), index);
            Set(manifest, "m_Assets", assetsArray); Set(manager, "_BasePatchStreamManifest", manifest);
            Type listType = core.GetType("BinaryAssetBuilder.Core.List2`1", true).MakeGenericType(instanceType);
            inputs = value.nullOutput ? null : (IList)Activator.CreateInstance(listType);
            final = value.sameLists ? inputs : value.nullFinal ? null : (IList)Activator.CreateInstance(listType);
            instances = new object[value.candidates.Length + 1];
            for (int index = 0; index < value.candidates.Length; index++)
            {
                object instance = index == value.nullCandidate ? null : index == value.uninitializedCandidate ? Activator.CreateInstance(instanceType) : Instance(value.candidates[index], value.candidateTypes == null ? 7u : value.candidateTypes[index], value.equalityMutation != 0 && index == 0);
                if (instance != null && index == value.zeroTypeHash) handleType.GetProperty("TypeHash").SetValue(Get(Get(instance, "_Current"), "Handle"), 0u, null);
                instances[index] = instance; if (inputs != null) inputs.Add(instance);
            }
            instances[instances.Length - 1] = Instance(value.bases.Length == 0 ? 0 : value.bases[0], value.baseTypes == null || value.baseTypes.Length == 0 ? 7u : value.baseTypes[0]);
            var binaryAssets = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(string), binaryType));
            Set(manager, "_Assets", binaryAssets);
            Type oldOutputValue = Field(managerType, "_OldOutputInstances").FieldType.GetGenericArguments()[1];
            Set(manager, "_OldOutputInstances", Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(string), oldOutputValue)));
            for (int index = 0; index < value.candidates.Length; index++) Binary(binaryAssets, instances[index], !value.patched.Contains(value.candidates[index]));
            Binary(binaryAssets, instances[instances.Length - 1], value.bases.Length == 0 || !value.patched.Contains(value.bases[0]));
            if (value.equalityMutation != 0)
            {
                Action mutation = () => {
                    if (value.equalityMutation == 2) throw new InvalidOperationException("injected derived Equals failure");
                    object selected = instances[value.bases[0]];
                    for (int index = 0; index < inputs.Count; index++)
                        if (Object.ReferenceEquals(inputs[index], selected)) { inputs.RemoveAt(index); break; }
                    // The new equal entry is deliberately beyond the count at
                    // entry to legacy Remove/IndexOf; it must not be searched.
                    inputs.Add(instances[instances.Length - 2]); inputs.Add(instances[instances.Length - 1]);
                };
                instances[0].GetType().GetField("OnEquals").SetValue(instances[0], mutation);
            }
            writerField = tracerType.GetField("TraceWrite", Fields); maskField = tracerType.GetField("_TraceMask", Fields);
            oldWriter = writerField.GetValue(null); oldMask = maskField.GetValue(null);
            tracerType.GetMethod("SetTraceLevel").Invoke(null, new object[] { 9 });
            object tracer = Field(documentType, "_Tracer").GetValue(null); Set(tracer, "_TraceEnabled", false);
            writerField.SetValue(null, value.callback == 0 ? null : Delegate.CreateDelegate(writerField.FieldType, this, GetType().GetMethod("Trace")));
            MethodInfo helper = documentType.GetMethod("StableSort_InitializeFromBaseManifest", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            object[] arguments = { manager, inputs, final }; string error = null;
            try { helper.Invoke(helper.IsStatic ? null : document, arguments); }
            catch (Exception failure)
            {
                while (failure is TargetInvocationException && failure.InnerException != null) failure = failure.InnerException;
                error = failure.GetType().FullName + ":" + failure.Message;
            }
            string metrics = "{}"; MethodInfo getMetrics = documentType.GetMethod("GetBaseManifestWorkMetrics");
            if (getMetrics != null) metrics = new JavaScriptSerializer().Serialize(getMetrics.Invoke(document, null));
            return new BaseManifestResult { callbacks = callbacks, metrics = metrics,
                outcome = new JavaScriptSerializer().Serialize(new { error, remaining = Shape((IList)arguments[1]), final = Shape((IList)arguments[2]), sameLists = Object.ReferenceEquals(arguments[1], arguments[2]) }) };
        }
        finally
        {
            if (writerField != null) writerField.SetValue(null, oldWriter); if (maskField != null) maskField.SetValue(null, oldMask);
            foreach (IntPtr memory in allocations) Marshal.FreeHGlobal(memory); allocations.Clear();
            AppContext.SetSwitch("NewSdk.Compiler.DisableBaseManifestIndex", oldSwitch); Environment.SetEnvironmentVariable("BabEnableTrace", oldEnvironment);
        }
    }
}

public sealed class LegacyBaseManifestDomain : MarshalByRefObject
{
    BaseManifestReflectionRunner runner;
    public override object InitializeLifetimeService() { return null; }
    public string Initialize(string corePath, string utilityPath)
    {
        Assembly utility = Assembly.LoadFrom(utilityPath), core = Assembly.LoadFrom(corePath);
        if (Path.GetFullPath(core.Location) != Path.GetFullPath(corePath) || Path.GetFullPath(utility.Location) != Path.GetFullPath(utilityPath)) throw new Exception("Original baseline resolved outside pinned SDK paths");
        runner = new BaseManifestReflectionRunner(core, utility); return core.FullName;
    }
    public BaseManifestResult Run(BaseManifestCase spec) { return runner.Run(spec, true); }
}

public static class CompilerBaseManifestIndex
{
    static int checks, cases;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> measurements = new List<object>();
    static LegacyBaseManifestDomain original;
    static BaseManifestReflectionRunner recovered;
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static void Require(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static IDictionary<string, object> Metrics(BaseManifestResult result) { return Json.Deserialize<Dictionary<string, object>>(result.metrics); }
    static long Number(IDictionary<string, object> value, string key) { return Convert.ToInt64(value[key]); }
    static object[] Items(object value) { return ((IEnumerable)value).Cast<object>().ToArray(); }
    static BaseManifestResult Compare(BaseManifestCase spec)
    {
        BaseManifestResult baseline = original.Run(spec), indexed = recovered.Run(spec, false), fallback = recovered.Run(spec, true);
        Require(baseline.outcome == indexed.outcome && baseline.outcome == fallback.outcome,
            spec.name + " exact original/index/fallback order/error/partial-state differential\noriginal=" + baseline.outcome + "\nindexed=" + indexed.outcome + "\nfallback=" + fallback.outcome);
        Require(baseline.callbacks == indexed.callbacks && baseline.callbacks == fallback.callbacks, spec.name + " changed actual external callback execution");
        var metrics = Metrics(indexed); Require(Number(metrics, "calls") == 1, spec.name + " work snapshot omitted the actual helper call");
        if (spec.callback != 0 && spec.callback != 6 && spec.candidates.Length >= 16 && spec.bases.Length >= 8 && !spec.sameLists)
            Require(Number(metrics, "callbackFallbacks") > 0, spec.name + " did not fall back after a real callback");
        cases++; measurements.Add(new { name = spec.name, indexed = metrics, fallback = Metrics(fallback), callbacks = indexed.callbacks }); return indexed;
    }
    static BaseManifestCase Case(string name, int count, int bases)
    {
        return new BaseManifestCase { name = name, candidates = Enumerable.Range(0, count).ToArray(), bases = Enumerable.Range(0, bases).Select(index => count == 0 ? 99 : index % count).ToArray() };
    }
    static void Cases()
    {
        foreach (int count in new[] { 0, 1, 15, 16, 17, 64 })
        {
            Compare(Case("ordered-" + count, count, count == 0 ? 0 : 8));
            var reverse = Case("reverse-" + count, count, count == 0 ? 8 : count); System.Array.Reverse(reverse.bases); Compare(reverse);
            var missing = Case("missing-" + count, count, 8); missing.bases = Enumerable.Repeat(9999, 8).ToArray(); Compare(missing);
        }
        var duplicates = Case("duplicate-handles-and-base-entries", 32, 16);
        duplicates.candidates = Enumerable.Range(0, 32).Select(index => index / 2).ToArray(); duplicates.bases = new[] { 3, 3, 3, 0, 0, 7, 7, 15, 15, 15, 99, 1, 1, 4, 4, 4 }; Compare(duplicates);
        var patched = Case("patched-first-candidate-retained", 32, 24); patched.patched = new[] { 0, 2, 4, 6, 8 }; patched.bases = Enumerable.Range(0, 24).Select(index => index % 10).ToArray(); Compare(patched);
        var types = Case("same-id-distinct-type-pairs", 32, 16); types.candidates = Enumerable.Range(0, 32).Select(index => index / 2).ToArray(); types.candidateTypes = Enumerable.Range(0, 32).Select(index => (uint)(7 + index % 2)).ToArray(); types.bases = Enumerable.Range(0, 16).Select(index => index / 2).ToArray(); types.baseTypes = Enumerable.Range(0, 16).Select(index => (uint)(7 + index % 2)).ToArray(); Compare(types);
        var nullCandidate = Case("null-candidate-partial-state", 32, 8); nullCandidate.nullCandidate = 31; nullCandidate.bases = new[] { 0, 999, 1, 2, 3, 4, 5, 6 }; Compare(nullCandidate);
        var uninitialized = Case("uninitialized-candidate-partial-state", 32, 8); uninitialized.uninitializedCandidate = 31; uninitialized.bases = nullCandidate.bases; Compare(uninitialized);
        var nullBase = Case("null-base-partial-state", 32, 8); nullBase.nullBase = 2; Compare(nullBase);
        var zeroHash = Case("zero-typehash-null-binary-partial-state", 32, 8); zeroHash.zeroTypeHash = 2; Compare(zeroHash);
        var same = Case("aliased-output-and-final-lists", 32, 8); same.sameLists = true; Compare(same);
        var empty = Case("empty-manifest-does-not-touch-null-lists", 0, 0); empty.nullOutput = empty.nullFinal = true; Compare(empty);
        var finalNullPatched = Case("null-final-all-patched-no-add", 32, 8); finalNullPatched.nullFinal = true; finalNullPatched.patched = Enumerable.Range(0, 32).ToArray();
        var patchedOutcome = Json.Deserialize<Dictionary<string, object>>(Compare(finalNullPatched).outcome);
        Require(patchedOutcome["error"] == null && patchedOutcome["final"] == null && Items(patchedOutcome["remaining"]).Length == 32,
            "All patched entries with a null final list must succeed without adding or removing any candidate");
        var finalNull = Case("null-final-first-reuse-error", 32, 8); finalNull.nullFinal = true; Compare(finalNull);
        for (int action = 1; action <= 8; action++)
        {
            var callback = Case("callback-" + action, 32, 16); callback.callback = action;
            if (action == 4 || action == 5) callback.bases[0] = 8;
            if (action == 2 || action == 3) { callback.candidates[1] = 0; callback.bases[1] = 0; }
            Compare(callback);
        }
        foreach (int mutation in new[] { 1, 2 })
        {
            var derived = Case("derived-Equals-" + mutation, 32, 8); derived.equalityMutation = mutation;
            derived.bases = new[] { 8, 9999, 9999, 9999, 9999, 9999, 9999, 9999 };
            var result = Compare(derived); var derivedMetrics = Metrics(result);
            var state = Json.Deserialize<Dictionary<string, object>>(result.outcome);
            Require(Number(derivedMetrics, "indexedCalls") == 0 && Number(derivedMetrics, "unsafeFallbacks") == 1 && Number(derivedMetrics, "linearCalls") == 1,
                "A derived Equals implementation was incorrectly authorized for indexed removal");
            if (mutation == 1)
                Require(state["error"] == null && Items(state["remaining"]).Length == 33 && Items(state["final"]).Length == 1 &&
                    Convert.ToString(Items(state["remaining"])[32]).StartsWith("32:", StringComparison.Ordinal) && Number(derivedMetrics, "linearRemovalChecks") == 32,
                    "Legacy Remove did not preserve its entry count after Equals changed the list and appended a new matching candidate");
            else
                Require(Convert.ToString(state["error"]).Contains("injected derived Equals failure") && Items(state["remaining"]).Length == 32 && Items(state["final"]).Length == 1,
                    "Derived Equals failure lost original exception or final-add-before-remove partial state");
        }
        tests.Add("derived-Equals-mutation-and-exception-force-unsafe-fallback-preserve-entry-count-and-original-partial-state");
        tests.Add("original-SDK-three-way-exact-candidate-final-order-duplicates-patched-missing-partial-failure-null-and-list-alias-differential");
        tests.Add("real-trace-callback-list-remove-insert-clear-swap-handle-replacement-and-thrown-error-force-compatible-legacy-removal");
        var work = Case("work-reduction-reverse-512", 512, 512); System.Array.Reverse(work.bases);
        var indexed = Compare(work); var fallbackWork = recovered.Run(work, true); var metrics = Metrics(indexed); var linear = Metrics(fallbackWork);
        Require(Number(metrics, "indexedCalls") == 1 && Number(metrics, "indexEntries") == 512 && Number(metrics, "indexLookups") == 512 &&
            Number(metrics, "linearCandidateChecks") == 0 && Number(metrics, "linearRemovalChecks") == 0 && Number(metrics, "rankSteps") > 0,
            "Wide callback-free group did not use exact-key queues and immediate rank removal");
        Require(Number(linear, "linearCandidateChecks") > 100000 && Number(linear, "linearRemovalChecks") > 100000,
            "Fallback did not retain the historical duplicate scans");
        tests.Add("wide-callback-free-index-eliminates-legacy-candidate-and-removal-scans-without-claiming-List2-storage-or-full-build-speedup");
    }
    public static int Main(string[] args)
    {
        int code = 0; object result; AppDomain domain = null;
        try
        {
            Require(args.Length == 5 && IntPtr.Size == 4, "Expected x86 work result source run SDK"); Directory.CreateDirectory(args[0]);
            string corePath = Path.Combine(args[4], "tools/BinaryAssetBuilder.Core.dll"), utilityPath = Path.Combine(args[4], "tools/BinaryAssetBuilder.Utility.dll");
            domain = AppDomain.CreateDomain("BaseManifest-original-" + Guid.NewGuid().ToString("N"), null, new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(typeof(CompilerBaseManifestIndex).Assembly.Location), ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile });
            original = (LegacyBaseManifestDomain)domain.CreateInstanceFromAndUnwrap(typeof(LegacyBaseManifestDomain).Assembly.Location, typeof(LegacyBaseManifestDomain).FullName);
            string identity = original.Initialize(corePath, utilityPath); Require(identity.StartsWith("BinaryAssetBuilder.Core,", StringComparison.Ordinal), "Original Core identity mismatch");
            recovered = new BaseManifestReflectionRunner(typeof(AssetDeclarationDocument).Assembly, typeof(AssetDeclarationDocument).Assembly); Cases();
            AppDomain.Unload(domain); domain = null; original = null;
            foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
                Require(loaded.GetName().Name != "BinaryAssetBuilder.Core" && loaded.GetName().Name != "BinaryAssetBuilder.Utility", "Original business assembly escaped its isolated baseline domain");
            tests.Add("original-Core-and-Utility-confined-to-unloaded-AppDomain-host-ownership-retained");
            string sha; using (var hash = SHA256.Create()) sha = BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(corePath))).Replace("-", "").ToLowerInvariant();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, cases, tests = tests.ToArray(), measurements = measurements.ToArray(), originalDllSha256 = sha, originalDllAssembly = identity,
                scope = "Base-manifest initialization private-helper exact original/index/fallback state and work differential; full stable sorting, patch layouts and end-to-end speedup remain separate" };
            Console.WriteLine("PASS BaseManifestIndex: " + cases + " cases; " + checks + " checks");
        }
        catch (Exception error)
        {
            code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, cases, tests = tests.ToArray(), measurements = measurements.ToArray(), error = error.ToString() };
        }
        finally { if (domain != null) AppDomain.Unload(domain); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
}
