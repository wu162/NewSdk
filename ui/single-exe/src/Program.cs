// SPDX-License-Identifier: GPL-3.0-only
using System.Text.Json;
using System.Text.Json.Serialization;
using NewSdk.Ui;

return Run(args);

static int Run(string[] args)
{
    try
    {
        NativeBridge.CheckAbi();
        if (args.Length == 0 || args is ["--help"] or ["-h"])
        {
            Console.WriteLine("SwfCompiler - single-EXE refactoring framework\n" +
                "  SwfCompiler inspect <input.swf>\n" +
                "  SwfCompiler compile <input.swf> [--ra3] [--optimize]\n" +
                "Compile is unavailable until the original conversion backend is migrated.");
            return 0;
        }
        if (args is ["--version"])
        {
            Console.WriteLine($"SwfCompiler framework 0.1; native ABI={NativeBridge.GetAbiVersion()}; native features={(uint)NativeBridge.GetFeatures()}");
            return 0;
        }
        if (args.Length < 2 || args[0] is not ("inspect" or "compile"))
            throw new ArgumentException("Expected inspect or compile followed by a SWF path. Use --help.");
        bool optimize = false;
        foreach (string option in args.Skip(2))
        {
            if (args[0] != "compile") throw new ArgumentException("inspect accepts only one SWF path.");
            switch (option)
            {
                case "--ra3": break;
                case "--optimize": optimize = true; break;
                default: throw new ArgumentException("Unknown option: " + option);
            }
        }
        string path = args[1];
        if (new FileInfo(path).Length > SwfInspector.MaxInputBytes)
            throw new InvalidDataException("SWF exceeds input size limit.");
        byte[] input = File.ReadAllBytes(path);
        if (args[0] == "inspect")
        {
            var result = SwfInspector.Inspect(input);
            Console.WriteLine(JsonSerializer.Serialize(result, InspectionJsonContext.Default.SwfInspection));
            return 0;
        }
        _ = new SwfCompiler().Compile(input, new CompilerOptions(Path.GetFileNameWithoutExtension(path), optimize));
        throw new InvalidOperationException("No final asset writer is configured.");
    }
    catch (BackendUnavailableException ex) { Console.Error.WriteLine(ex.Message); return 4; }
    catch (ArgumentException ex) { Console.Error.WriteLine(ex.Message); return 2; }
    catch (Exception ex) when (ex is IOException or InvalidDataException or NotSupportedException or UnauthorizedAccessException)
    { Console.Error.WriteLine(ex.Message); return 3; }
    catch (Exception ex) { Console.Error.WriteLine(ex.Message); return 1; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(SwfInspection))]
internal partial class InspectionJsonContext : JsonSerializerContext { }
