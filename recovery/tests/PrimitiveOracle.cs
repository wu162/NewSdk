using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

// A differential test calls the *original* global IL functions via reflection,
// then compares float bits with the rebuilt native+C++/CLI implementation.
class PrimitiveOracle
{
    static unsafe int Main(string[] args)
    {
        var original = Assembly.LoadFrom(args[0]);
        var methods = original.GetModules()[0].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        int count = 0;
        foreach (string tag in new[] { "TimeTag", "AngleTag" })
        {
            var candidates = methods.Where(m => m.Name == "DataInitializers.Initialize" &&
                m.GetParameters().Length == 3 && m.GetParameters()[2].ParameterType.Name == tag + "*").ToArray();
            if (candidates.Length != 1) throw new Exception("Expected exactly one original initializer for " + tag + ", got " + candidates.Length);
            var method = candidates[0];
            string[] values = tag == "TimeTag"
                ? new[] { "0", "1", "250ms", "1.25s", "-4ms", "123.456789ms", " 2s", "3S", "junk", "2s1ms", "1e-6s", "2d1s", "2D1", "2d-1s", "0x10s", "0x1p2s", "INF", "NaN", "-INF", "1.#INF", "-.25s" }
                : new[] { "0", "1", "90d", "180d", "3.1415927r", "-45d", "0.123456789d", " 2r", "3D", "junk", "2d1r", "2d-1r", "2d+1r", "0x10r", "0x1p2r", "INF", "NaN", "-INF", "1.#INF", "-.25r" };
            foreach (var text in values)
            {
                float expected = 0;
                IntPtr input = Marshal.StringToHGlobalAnsi(text);
                try
                {
                    var p = method.GetParameters();
                    method.Invoke(null, new object[] { Pointer.Box(&expected, p[0].ParameterType), Pointer.Box(input.ToPointer(), p[1].ParameterType), Pointer.Box(null, p[2].ParameterType) });
                }
                finally { Marshal.FreeHGlobal(input); }
                float actual = tag == "TimeTag" ? Recovered.PrimitiveInitializers.Time(text) : Recovered.PrimitiveInitializers.Angle(text);
                uint expectedBits = *(uint*)&expected, actualBits = *(uint*)&actual;
                Console.WriteLine("{0}\t{1}\t{2:X8}\t{3:X8}", tag, text, expectedBits, actualBits);
                if (expectedBits != actualBits) throw new Exception("Float-bit mismatch for " + tag + " " + text);
                count++;
            }
        }
        Console.WriteLine("PASS: " + count + " original/recovered bitwise comparisons");
        return 0;
    }
}

