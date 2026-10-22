using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.XPath;
using Recovered;

// Original Node/List/Value objects are confined to this oracle process.
// No original constructors or functions are called by the recovered adapter.
unsafe class XmlNodeOracle
{
    static MethodInfo[] methods;
    static int comparisons;
    static object Call(string name, params object[] args)
    {
        MethodInfo method = methods.Single(m => m.Name == name && m.GetParameters().Length == args.Length);
        var parameters = method.GetParameters();
        for (int i = 0; i < args.Length; ++i)
            if (parameters[i].ParameterType.IsPointer)
                args[i] = Pointer.Box(((IntPtr)args[i]).ToPointer(), parameters[i].ParameterType);
        return method.Invoke(null, args);
    }
    static IntPtr Ptr(object value) { return new IntPtr(Pointer.Unbox(value)); }
    static string ReadValue(IntPtr value)
    {
        if (value == IntPtr.Zero) return null;
        try { return Marshal.PtrToStringAnsi(Ptr(Call("Value.GetText", value))); }
        finally { Call("Value.Release", value); }
    }
    static string Attribute(IntPtr node, string name, string fallback)
    {
        IntPtr n = Marshal.StringToHGlobalAnsi(name), f = fallback == null ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(fallback);
        try { return ReadValue(Ptr(Call("Node.GetAttributeValue", node, n, f))); }
        finally { Marshal.FreeHGlobal(n); Marshal.FreeHGlobal(f); }
    }
    static string Value(IntPtr node) { return ReadValue(Ptr(Call("Node.GetValue", node))); }
    static string Child(IntPtr node, string name, string fallback)
    {
        IntPtr n = Marshal.StringToHGlobalAnsi(name), f = fallback == null ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(fallback);
        IntPtr child = IntPtr.Zero;
        try {
            Call("Node.GetChildNode", node, new IntPtr(&child), n, f);
            return child == IntPtr.Zero ? null : Value(child);
        } finally {
            if (child != IntPtr.Zero) Call("Node.Release", child);
            Marshal.FreeHGlobal(n); Marshal.FreeHGlobal(f);
        }
    }
    static List<string> ReadList(IntPtr list)
    {
        var values = new List<string>();
        while (true) {
            IntPtr child = IntPtr.Zero;
            Call("List.GetNextNode", list, new IntPtr(&child));
            if (child == IntPtr.Zero) break;
            try { values.Add(Value(child)); }
            finally { Call("Node.Release", child); }
        }
        return values;
    }
    static List<string> ReadList(XPathNodeIterator iterator)
    {
        var values = new List<string>();
        while (iterator.MoveNext()) values.Add(iterator.Current.Value);
        return values;
    }
    static void Equal(string label, string expected, string actual)
    {
        if (expected != actual) throw new Exception(label + " expected=" + (expected ?? "<null>") + " actual=" + (actual ?? "<null>"));
        comparisons++;
    }
    static void Test(string label, string xmlText)
    {
        var xml = new XmlDocument(); xml.PreserveWhitespace = true; xml.LoadXml(xmlText);
        var namespaces = new XmlNamespaceManager(xml.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        var adapter = new XmlNodeAdapter(xml.DocumentElement.CreateNavigator(), namespaces);
        IntPtr node = Marshal.AllocHGlobal(16);
        IntPtr navigatorHandle = GCHandle.ToIntPtr(GCHandle.Alloc(xml.DocumentElement.CreateNavigator()));
        IntPtr namespaceHandle = GCHandle.ToIntPtr(GCHandle.Alloc(namespaces));
        bool constructed = false;
        try {
            // Original ctor consumes the two temporary gcroot handles.
            Call("Node.{ctor}", node, new IntPtr(&navigatorHandle), new IntPtr(&namespaceHandle));
            constructed = true;
            foreach (string name in new[] { "A", "a", "Missing", "Empty", "Scoped" })
                foreach (string fallback in new string[] { null, "", "fallback" }) {
                    Equal(label + "/attribute/" + name, Attribute(node, name, fallback), adapter.Attribute(name, fallback));
                    Equal(label + "/value-after-attribute", Value(node), adapter.Value());
                }
            foreach (string name in new[] { "Child", "Missing", "child" })
                foreach (string fallback in new string[] { null, "", "default child" }) {
                    XmlNodeAdapter child = adapter.Child(name, fallback);
                    Equal(label + "/child/" + name, Child(node, name, fallback), child == null ? null : child.Value());
                    Equal(label + "/value-after-child", Value(node), adapter.Value());
                }
            foreach (string name in new string[] { null, "Child", "Missing", "child" }) {
                IntPtr n = name == null ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(name);
                IntPtr list = Ptr(Call("Node.GetChildNodes", node, n));
                try {
                    var iterator = adapter.Children(name);
                    Equal(label + "/list-count", ((int)Call("List.GetCount", list)).ToString(), iterator.Count.ToString());
                    var original = ReadList(list); var recovered = ReadList(iterator);
                    Equal(label + "/list-values", string.Join("|", original), string.Join("|", recovered));
                    Call("List.Reset", list);
                    Equal(label + "/list-reset", string.Join("|", original), string.Join("|", ReadList(list)));
                } finally { Call("List.Release", list); Marshal.FreeHGlobal(n); }
            }
            Console.WriteLine("PASS " + label);
        } finally {
            if (constructed) Call("Node.{dtor}", node);
            if (navigatorHandle != IntPtr.Zero) GCHandle.FromIntPtr(navigatorHandle).Free();
            if (namespaceHandle != IntPtr.Zero) GCHandle.FromIntPtr(namespaceHandle).Free();
            Marshal.FreeHGlobal(node);
        }
    }
    static int Main(string[] args)
    {
        try {
            methods = Assembly.LoadFrom(args[0]).GetModules()[0].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (string ns in new[] { "", "uri:ea.com:eala:asset", "other" }) {
                string root = "<Root xmlns='" + ns + "' xmlns:e='uri:ea.com:eala:asset' xmlns:o='other' A='first' a='second' Empty='' o:Scoped='hidden'>";
                Test("namespace=" + ns, root + "start<Child A='nested'>one<Inner>two</Inner></Child><Child>three</Child><child>lower</child><e:Child>asset</e:Child><o:Child>other</o:Child><!-- comment -->end</Root>");
                Test("empty namespace=" + ns, root + "</Root>");
            }
            Console.WriteLine("PASS Node adapter: " + comparisons + " original/recovered comparisons.");
            return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
