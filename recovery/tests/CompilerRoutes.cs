using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

// External test host: never linked into the production executable.
class CompilerRoutes
{
    static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    static object Read(object instance, string name)
    {
        Type type = instance.GetType();
        PropertyInfo property = type.GetProperty(name);
        return property != null ? property.GetValue(instance, null) : type.GetField(name).GetValue(instance);
    }
    static List<object> Items(object value)
    {
        var result = new List<object>();
        foreach (object item in (IEnumerable)value) result.Add(item);
        return result;
    }
    static void Reject(MethodInfo method, object target, object[] args, string label)
    {
        try { method.Invoke(target, args); }
        catch (TargetInvocationException error)
        {
            Require(error.InnerException is ArgumentException || error.InnerException is InvalidOperationException || error.InnerException is NotSupportedException,
                label + " failed for an unrelated reason: " + error.InnerException);
            return;
        }
        throw new Exception(label + " was accepted");
    }
    static int Main(string[] args)
    {
        try
        {
            Require(args.Length == 2, "Usage: CompilerRoutes.exe product.exe XmlSdkTypes.json");
            string exe = Path.GetFullPath(args[0]), bin = Path.GetDirectoryName(exe);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string name = new AssemblyName(e.Name).Name;
                if (name == "antlr.runtime") return Assembly.LoadFrom(Path.Combine(bin, "antlr.runtime.dll"));
                throw new Exception("Unexpected runtime assembly resolution: " + e.Name);
            };
            Assembly own = Assembly.LoadFrom(exe);
            Type factory = own.GetType("NewModCompiler.Application.BuiltinCompilers", true);
            Type platformType = own.GetType("BinaryAssetBuilder.Core.TargetPlatform", true);
            foreach (string unsupported in new[] { "Xbox360", "PlayStation3" })
                Reject(factory.GetMethod("Create"), null, new[] { Enum.Parse(platformType, unsupported) }, "Unsupported product platform " + unsupported);
            Reject(factory.GetMethod("Create"), null, new[] { Enum.ToObject(platformType, 99) }, "Unknown product platform");
            object win32 = Enum.Parse(platformType, "Win32");
            object owner = factory.GetMethod("Create").Invoke(null, new[] { win32 });
            try
            {
                var routes = Items(Read(owner, "Routes"));
                var plugins = Items(Read(owner, "Plugins"));
                Require(routes.Count == 299, "Expected 299 declared built-in routes");
                Require(plugins.Count == 6, "Expected six explicitly constructed built-in plugins");
                foreach (object plugin in plugins) Require(plugin.GetType().Assembly == own, "Plugin implementation escaped production EXE");
                var ids = new HashSet<uint>(); var names = new HashSet<string>(StringComparer.Ordinal);
                var actual = new Dictionary<string, object>(StringComparer.Ordinal);
                object registry = Read(owner, "Registry");
                MethodInfo getPlugin = registry.GetType().GetMethod("GetPlugin");
                MethodInfo getInfo = registry.GetType().GetMethod("GetExtendedTypeInformation");
                foreach (object route in routes)
                {
                    uint id = Convert.ToUInt32(Read(route, "TypeId")); string name = (string)Read(route, "TypeName");
                    Require(ids.Add(id) && names.Add(name), "Duplicate built-in route: " + name);
                    actual.Add(name, route);
                    object plugin = getPlugin.Invoke(registry, new object[] { id });
                    Require(plugins.Contains(plugin) && plugin.GetType().Assembly == own, "Route uses undeclared/external factory: " + name);
                    object info = getInfo.Invoke(registry, new object[] { id });
                    Require(Convert.ToUInt32(Read(info, "TypeId")) == id && (string)Read(info, "TypeName") == name &&
                        Convert.ToUInt32(Read(info, "TypeHash")) == Convert.ToUInt32(Read(route, "TypeHash")), "Runtime route metadata differs: " + name);
                }
                foreach (string required in new[] { "GameObject", "WeaponTemplate", "ArmorTemplate", "W3DAnimation", "W3DMesh" })
                    Require(actual.ContainsKey(required), "Required native family route absent: " + required);
                var catalog = (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(File.ReadAllText(args[1]));
                int xmlCount = 0, globalCount = 0;
                foreach (Dictionary<string, object> expected in (object[])catalog["roots"])
                {
                    string name = (string)expected["name"];
                    Require(actual.ContainsKey(name), "Declared XML route missing: " + name);
                    object route = actual[name];
                    Require(Convert.ToUInt32(Read(route, "TypeId")) == Convert.ToUInt32(expected["typeId"]) &&
                        Convert.ToUInt32(Read(route, "TypeHash")) == Convert.ToUInt32(expected["typeHash"]) &&
                        (bool)Read(route, "SchemaGlobal") == (bool)expected["schemaGlobal"], "XML catalog metadata differs: " + name);
                    ++xmlCount; if ((bool)expected["schemaGlobal"]) ++globalCount;
                }
                Require(xmlCount == 289 && globalCount == 267, "XML schema-global/non-global boundary changed");
                foreach (Dictionary<string, object> excluded in (object[])catalog["excluded"])
                    Require(!ids.Contains(Convert.ToUInt32(excluded["typeId"])), "Excluded XML root became supported");
                Type routeType = routes[0].GetType();
                MethodInfo validate = factory.GetMethod("ValidateRoutes");
                Array valid = Array.CreateInstance(routeType, routes.Count);
                for (int i = 0; i < routes.Count; ++i) valid.SetValue(routes[i], i);
                validate.Invoke(null, new object[] { valid });
                Array missing = Array.CreateInstance(routeType, routes.Count - 1);
                Array.Copy(valid, missing, missing.Length);
                Reject(validate, null, new object[] { missing }, "Missing route");
                Array duplicate = Array.CreateInstance(routeType, routes.Count + 1);
                Array.Copy(valid, duplicate, valid.Length); duplicate.SetValue(routes[0], routes.Count);
                Reject(validate, null, new object[] { duplicate }, "Duplicate route");
                Require(!ids.Contains(0u), "Unknown-route fixture collided");
                object unknown = getPlugin.Invoke(registry, new object[] { 0u });
                Reject(unknown.GetType().GetMethod("ProcessInstance"), unknown, new object[] { null }, "Unknown route");
                Console.WriteLine("PASS production built-in registry: 299 routes, six EXE-owned factories, 289 XML declarations (267 global/22 non-global), missing/duplicate/unknown rejection; metadata only, no asset differential");
            }
            finally { ((IDisposable)owner).Dispose(); ((IDisposable)owner).Dispose(); }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = assembly.GetName().Name;
                Require(name == own.GetName().Name || name == Assembly.GetExecutingAssembly().GetName().Name || name == "antlr.runtime" ||
                    name == "mscorlib" || name == "System" || name.StartsWith("System.", StringComparison.Ordinal) || name == "Microsoft.CSharp",
                    "Unexpected loaded business assembly: " + name);
            }
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
