using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using Recovered;

unsafe class ScalarOracle
{
    static MethodInfo[] methods;
    static object Call(MethodInfo method, params object[] args)
    {
        var parameters = method.GetParameters();
        for (int i = 0; i < args.Length; ++i)
            if (parameters[i].ParameterType.IsPointer)
                args[i] = Pointer.Box(((IntPtr)args[i]).ToPointer(), parameters[i].ParameterType);
        return method.Invoke(null, args);
    }
    static object Call(string name, params object[] args)
    {
        return Call(methods.Single(m => m.Name == name && m.GetParameters().Length == args.Length), args);
    }
    static IntPtr Ptr(object result) { return new IntPtr(Pointer.Unbox(result)); }
    static uint Original(IntPtr node, string type, string text)
    {
        MethodInfo method = methods.Single(m => m.Name == type && m.GetParameters()[0].ParameterType.Name == "IValue*");
        IntPtr name = Marshal.StringToHGlobalAnsi("A"), fallback = Marshal.StringToHGlobalAnsi(text);
        IntPtr value;
        try { value = Ptr(Call("Node.GetAttributeValue", node, name, fallback)); }
        finally { Marshal.FreeHGlobal(name); Marshal.FreeHGlobal(fallback); }
        uint output = 0;
        if (method.GetParameters().Length == 3) Call(method, value, new IntPtr(&output), IntPtr.Zero);
        else Call(method, value, new IntPtr(&output), IntPtr.Zero, IntPtr.Zero);
        return output;
    }
    static int Main(string[] args)
    {
        try {
            methods = Assembly.LoadFrom(args[0]).GetModules()[0].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var xml = new XmlDocument(); xml.LoadXml("<Root/>");
            var ns = new XmlNamespaceManager(xml.NameTable);
            IntPtr node = Marshal.AllocHGlobal(16);
            IntPtr navHandle = GCHandle.ToIntPtr(GCHandle.Alloc(xml.DocumentElement.CreateNavigator()));
            IntPtr nsHandle = GCHandle.ToIntPtr(GCHandle.Alloc(ns));
            Call("Node.{ctor}", node, new IntPtr(&navHandle), new IntPtr(&nsHandle));
            int count = 0, failures = 0;
            try {
                string[] integers = { "0", "1", "-1", "+42", " 17tail", "010", "08", "0x1f", "-0x20", "0b11", "2147483647", "2147483648", "-2147483648", "-2147483649", "4294967295", "4294967296", "-4294967295", "999999999999999999999999", "-999999999999999999999999", "", "garbage", "12.5", "65536", "257", "--1", "+-1", "++1", "-+1", "- 1", "-4294967296" };
                string[] reals = { "0", "-0", "1", "+1", "12.5", " 12.5", "12,5", "garbage", "2d1", "2D-1", "1e-40", "1e38", "-1e38", "16777217", "0x10", "0x1p2", "INF", "NaN", "1.#INF", "1e39", "1e-50", "-INF", "-NaN", "-garbage", "-0x10", ".25", "-.25", "1e", "1d+", "1d-2tail", "- 1", "++1" };
                foreach (string kind in new[] { "unsigned int", "int", "unsigned char", "unsigned short", "bool", "float", "percent" }) {
                    string methodName = kind == "percent" ? "Marshaler.Marshal<float,class Relo::Tracker,struct PercentageTag>" : "Marshaler.Marshal<" + kind + ",class Relo::Tracker>";
                    string[] values = kind == "float" || kind == "percent" ? reals : kind == "bool" ? new[] { "true", "True", "TRUE", "turtle", "false", "False", "1", "10", "2", "-1", " 1", " true", "", "yes", "T" } : integers;
                    foreach (string text in values) {
                        uint expected = Original(node, methodName, text), actual;
                        switch (kind) {
                            case "unsigned int": actual = ScalarParser.Unsigned(text); break;
                            case "int": actual = unchecked((uint)ScalarParser.Signed(text)); break;
                            case "unsigned char": actual = (byte)ScalarParser.Unsigned(text); break;
                            case "unsigned short": actual = (ushort)ScalarParser.Unsigned(text); break;
                            case "bool": actual = ScalarParser.Boolean(text) ? 1u : 0u; break;
                            default: float number = kind == "percent" ? ScalarParser.Percentage(text) : ScalarParser.Float(text); actual = *(uint*)&number; break;
                        }
                        Console.WriteLine("{0}\t{1}\t{2:X8}\t{3:X8}", kind, text, expected, actual);
                        if (expected != actual) { Console.WriteLine("MISMATCH " + kind + " for " + text); failures++; }
                        count++;
                    }
                }
            } finally { Call("Node.{dtor}", node); Marshal.FreeHGlobal(node); }
            if (failures != 0) throw new Exception(failures + " scalar comparisons failed out of " + count);
            Console.WriteLine("PASS scalar parsing: " + count + " original/recovered bit comparisons.");
            return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
