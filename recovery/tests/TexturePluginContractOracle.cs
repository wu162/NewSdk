using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

// Inventory and original-only metadata probes. Does not compile textures or
// establish differential acceptance. Use x86 CLR4 with legacy CLR2 activation.
internal static class TexturePluginContractOracle
{
    private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
    private static string TypeName(Type type)
    {
        return type.FullName ?? type.ToString();
    }
    private static object Parameter(ParameterInfo parameter)
    {
        return new {
            name = parameter.Name, type = TypeName(parameter.ParameterType),
            attributes = parameter.Attributes.ToString(),
            requiredModifiers = parameter.GetRequiredCustomModifiers().Select(TypeName).ToArray(),
            optionalModifiers = parameter.GetOptionalCustomModifiers().Select(TypeName).ToArray()
        };
    }
    private static object Method(MethodBase method)
    {
        MethodInfo info = method as MethodInfo;
        return new {
            token = "0x" + method.MetadataToken.ToString("X8"), name = method.Name,
            signature = method.ToString(), attributes = method.Attributes.ToString(),
            implementation = method.GetMethodImplementationFlags().ToString(),
            callingConvention = method.CallingConvention.ToString(),
            returns = info == null ? null : Parameter(info.ReturnParameter),
            parameters = method.GetParameters().Select(Parameter).ToArray()
        };
    }
    private static object Field(FieldInfo field)
    {
        return new {
            token = "0x" + field.MetadataToken.ToString("X8"), name = field.Name,
            type = TypeName(field.FieldType), attributes = field.Attributes.ToString(),
            requiredModifiers = field.GetRequiredCustomModifiers().Select(TypeName).ToArray(),
            optionalModifiers = field.GetOptionalCustomModifiers().Select(TypeName).ToArray(),
            constant = field.IsLiteral ? field.GetRawConstantValue() : null
        };
    }
    private static object Error(Exception exception)
    {
        while (exception is TargetInvocationException && exception.InnerException != null)
            exception = exception.InnerException;
        return new { type = exception.GetType().FullName, message = exception.Message };
    }
    private static object Invoke(MethodInfo method, object target, object[] arguments)
    {
        return method.Invoke(target, arguments);
    }
    private static object Snapshot(object target, Type plugin, MethodInfo getTypeId)
    {
        string[] names = { "TerrainTextureAtlas", "Texture", "OnDemandTexture", "UnknownTextureContractType" };
        List<object> types = new List<object>();
        MethodInfo getInfo = plugin.GetMethod("GetExtendedTypeInformation", new Type[] { typeof(uint) });
        foreach (string name in names)
        {
            uint id = Convert.ToUInt32(getTypeId.Invoke(null, new object[] { name }));
            try
            {
                object value = Invoke(getInfo, target, new object[] { id });
                Dictionary<string, object> fields = null;
                if (value != null)
                {
                    fields = new Dictionary<string, object>();
                    foreach (FieldInfo field in value.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name))
                        fields.Add(field.Name, field.GetValue(value));
                }
                types.Add(new { name = name, id = id, value = fields });
            }
            catch (Exception exception)
            {
                types.Add(new { name = name, id = id, error = Error(exception) });
            }
        }
        return new {
            allTypesHash = Invoke(plugin.GetMethod("GetAllTypesHash", Type.EmptyTypes), target, new object[0]),
            version = Invoke(plugin.GetMethod("GetVersionNumber", Type.EmptyTypes), target, new object[0]),
            types = types.ToArray()
        };
    }
    private static object Probe(Type plugin, Type platformType, MethodInfo getTypeId, int platform)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result.Add("platform", platform);
        object instance = null;
        bool disposed = false;
        string stage = "constructor";
        try
        {
            instance = Activator.CreateInstance(plugin);
            stage = "beforeInitialize";
            result.Add(stage, Snapshot(instance, plugin, getTypeId));
            stage = "initialize";
            plugin.GetMethod("Initialize", new Type[] { typeof(object), platformType }).Invoke(instance,
                new object[] { null, Enum.ToObject(platformType, platform) });
            stage = "afterInitialize";
            result.Add(stage, Snapshot(instance, plugin, getTypeId));
            stage = "reinitializeWin32";
            plugin.GetMethod("ReInitialize", new Type[] { typeof(object), platformType }).Invoke(instance,
                new object[] { null, Enum.ToObject(platformType, 0) });
            stage = "afterReinitializeWin32";
            result.Add(stage, Snapshot(instance, plugin, getTypeId));
            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                stage = "dispose";
                disposable.Dispose();
                disposed = true;
                stage = "afterDispose";
                result.Add(stage, Snapshot(instance, plugin, getTypeId));
            }
            result.Add("completed", true);
        }
        catch (Exception exception)
        {
            result.Add("completed", false);
            result.Add("failedStage", stage);
            result.Add("error", Error(exception));
        }
        finally
        {
            // Dispose failure is captured separately rather than replacing the
            // original failure or silently reporting successful lifecycle probes.
            IDisposable disposable = instance as IDisposable;
            if (disposable != null && !disposed)
            {
                try { disposable.Dispose(); }
                catch (Exception exception) { result.Add("cleanupError", Error(exception)); }
            }
        }
        return result;
    }
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new ArgumentException("Usage: TexturePluginContractOracle original.dll output.json");
            if (IntPtr.Size != 4) throw new InvalidOperationException("Original TextureCompiler requires an x86 host.");
            string input = Path.GetFullPath(args[0]);
            string directory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string candidate = Path.Combine(directory, new AssemblyName(e.Name).Name + ".dll");
                return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
            };
            Assembly original = Assembly.LoadFrom(input);
            Type plugin = original.GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
            string[] interfaces = plugin.GetInterfaces().Select(TypeName).OrderBy(n => n).ToArray();
            if (!interfaces.Contains("BinaryAssetBuilder.Core.IAssetBuilderPlugin"))
                throw new InvalidOperationException("Expected BAB plugin interface was not found.");
            Type core = plugin.GetInterfaces().First(t => t.FullName == "BinaryAssetBuilder.Core.IAssetBuilderPlugin");
            Type platformType = core.Assembly.GetType("BinaryAssetBuilder.Core.TargetPlatform", true);
            Type handle = core.Assembly.GetType("BinaryAssetBuilder.Core.InstanceHandle", true);
            MethodInfo getTypeId = handle.GetMethod("GetTypeId", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            if (getTypeId == null) throw new MissingMethodException(handle.FullName, "GetTypeId(string)");
            string hash;
            using (SHA256 sha = SHA256.Create())
                hash = BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(input))).Replace("-", "").ToLowerInvariant();
            object[] probes = new int[] { 0, 1, 2, 99 }.Select(p => Probe(plugin, platformType, getTypeId, p)).ToArray();
            var result = new {
                scope = "original-only contract and metadata/lifecycle inventory; no texture compilation, recovered differential acceptance, or SDK validation",
                sha256 = hash, assembly = original.FullName, runtime = original.ImageRuntimeVersion,
                pointerSize = IntPtr.Size, plugin = plugin.FullName, interfaces = interfaces,
                baseType = TypeName(plugin.BaseType),
                constructors = plugin.GetConstructors(Declared).OrderBy(c => c.MetadataToken).Select(Method).ToArray(),
                methods = plugin.GetMethods(Declared).OrderBy(m => m.MetadataToken).Select(Method).ToArray(),
                fields = plugin.GetFields(Declared).OrderBy(f => f.MetadataToken).Select(Field).ToArray(),
                interfaceContracts = plugin.GetInterfaces().OrderBy(TypeName).Select(t => new {
                    name = TypeName(t), assembly = t.Assembly.FullName,
                    methods = t.GetMethods().OrderBy(m => m.MetadataToken).Select(Method).ToArray()
                }).ToArray(),
                probes = probes
            };
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            File.WriteAllText(args[1], serializer.Serialize(result));
            bool complete = probes.Cast<Dictionary<string, object>>().All(p => (bool)p["completed"] && !p.ContainsKey("cleanupError"));
            Console.WriteLine("TextureCompiler original contract saved; metadata/lifecycle probes complete=" + complete + "; no texture or SDK acceptance");
            return complete ? 0 : 2;
        }
        catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
    }
}
