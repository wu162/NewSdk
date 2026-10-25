using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

// Original/recovered metadata differential only. Original Initialize and Dispose
// touch process-global native resources, so this probe deliberately uses only
// the original's state-only ReInitialize. No native lifecycle acceptance claim.
internal static class TextureMetadataOracle
{
    static int comparisons;
    static Type originalType, recoveredType, platformType;
    static readonly uint[] ids = { 568797146u, 2467477932u, 1601459053u, 0u, 1u, UInt32.MaxValue };
    static void Equal(object expected, object actual, string name)
    {
        if (!Object.Equals(expected, actual))
            throw new Exception(name + ": expected=" + expected + " actual=" + actual);
        comparisons++;
    }
    static object Call(Type type, object target, string name, params object[] args)
    {
        return type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance).Invoke(target, args);
    }
    static void Compare(object original, object recovered, string label)
    {
        Equal(Call(originalType, original, "GetVersionNumber"), Call(recoveredType, recovered, "GetVersionNumber"), label + "/version");
        Equal(Call(originalType, original, "GetAllTypesHash"), Call(recoveredType, recovered, "GetAllTypesHash"), label + "/allTypesHash");
        foreach (uint id in ids)
        {
            object expected = Call(originalType, original, "GetExtendedTypeInformation", id);
            object actual = Call(recoveredType, recovered, "GetExtendedTypeInformation", id);
            foreach (FieldInfo field in expected.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                FieldInfo recoveredField = actual.GetType().GetField(field.Name);
                if (recoveredField == null) throw new Exception("Missing metadata field " + field.Name);
                Equal(field.GetValue(expected), recoveredField.GetValue(actual), label + "/" + id + "/" + field.Name);
            }
            object another = Call(recoveredType, recovered, "GetExtendedTypeInformation", id);
            Equal(false, Object.ReferenceEquals(actual, another), label + "/freshMetadataObject/" + id);
        }
    }
    static void CheckManagedState(object recovered, object platform, string label)
    {
        Equal(platform, recoveredType.GetField("_Platform", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(recovered), label + "/platformState");
        Type hashProvider = platformType.Assembly.GetType("BinaryAssetBuilder.Core.HashProvider", true);
        object expected = hashProvider.GetMethod("GetTypeHash", new Type[] { typeof(Type) }).Invoke(null, new object[] { recoveredType });
        Equal(expected, recoveredType.GetField("_HashCode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(recovered), label + "/typeHashState");
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.Length != 2) throw new Exception("Usage: TextureMetadataOracle original.dll recovered.dll");
            if (IntPtr.Size != 4) throw new Exception("Original TextureCompiler requires x86 CLR");
            string originalPath = Path.GetFullPath(args[0]);
            string recoveredPath = Path.GetFullPath(args[1]);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
            {
                string filename = new AssemblyName(e.Name).Name + ".dll";
                foreach (string directory in new string[] { Path.GetDirectoryName(originalPath), Path.GetDirectoryName(recoveredPath) })
                {
                    string candidate = Path.Combine(directory, filename);
                    if (File.Exists(candidate)) return Assembly.LoadFrom(candidate);
                }
                return null;
            };
            originalType = Assembly.LoadFrom(originalPath).GetType("BinaryAssetBuilder.TextureCompiler.Plugin", true);
            recoveredType = Assembly.LoadFrom(recoveredPath).GetType("Recovered.Compiler.Texture.Plugin", true);
            platformType = originalType.GetMethod("ReInitialize").GetParameters()[1].ParameterType;
            object original = Activator.CreateInstance(originalType);
            object recovered = Activator.CreateInstance(recoveredType);
            Compare(original, recovered, "constructorDefaults");
            foreach(int target in new int[]{0})
            {
                object platform = Enum.ToObject(platformType, target);
                Call(originalType, original, "ReInitialize", null, platform);
                Call(recoveredType, recovered, "Initialize", null, platform);
                Compare(original, recovered, "initialize-managedState/" + target);
                CheckManagedState(recovered, platform, "initialize-managedState/" + target);
                object second = Enum.ToObject(platformType, 0);
                Call(originalType, original, "ReInitialize", new object(), second);
                Call(recoveredType, recovered, "ReInitialize", new object(), second);
                Compare(original, recovered, "reinitialize/" + target);
                CheckManagedState(recovered, second, "reinitialize/" + target);
            }
            // Non-PC requests must fail before changing the accepted PC state.
            foreach (int unsupported in new int[] { 1, 2, 99, -1 })
            foreach (string method in new string[] { "Initialize", "ReInitialize" })
            {
                bool rejected = false;
                try { Call(recoveredType, recovered, method, null, Enum.ToObject(platformType, unsupported)); }
                catch (TargetInvocationException e) { rejected = e.InnerException is NotSupportedException; }
                Equal(true, rejected, method + "/unsupported/" + unsupported);
                CheckManagedState(recovered, Enum.ToObject(platformType, 0), "unchangedAfterRejectedPlatform");
                Compare(original, recovered, "unchangedAfterRejectedPlatform");
            }
            ((IDisposable)recovered).Dispose();
            Compare(original, recovered, "afterRecoveredDispose");
            Console.WriteLine("PASS Texture metadata differential comparisons=" + comparisons + "; original state-only ReInitialize; native Initialize/Dispose and texture compilation not validated");
            // No original native resources were initialized, and no original
            // Dispose is invoked. Process exit owns the original-only object.
            GC.SuppressFinalize(original);
            return 0;
        }
        catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
    }
}

