using BinaryAssetBuilder.UiCompiler.Fla;
using BinaryAssetBuilder.UiCompiler;
using BinaryAssetBuilder.Core;

int passed = 0;
void Check(bool condition, string message) { if (!condition) throw new Exception(message); passed++; }
Check(SageHash.Symbol("AptAptData") == 1816258268u, "SAGE type hash");
Check(SageHash.Symbol("AptGeometryData") == 1034587168u, "SAGE geometry type hash");
Check(SageHash.Symbol("Texture") != 0, "Texture type ID");
if (args.Length == 0)
{
    string testRoot = Path.Combine(Path.GetTempPath(), "UiCompiler-" + Guid.NewGuid().ToString("N"));
    try
    {
        string source = Path.Combine(testRoot, "source"); Directory.CreateDirectory(Path.Combine(source, "logic"));
        string fixture = Path.Combine(AppContext.BaseDirectory, "fixtures", "flash8-empty.fla");
        File.Copy(fixture, Path.Combine(source, "Main.fla"));
        File.WriteAllText(Path.Combine(source, "Main.as"), "import logic.Controller;\n#include \"setup.as\"\nvar controller:Controller = new Controller(); controller.advance(2); stop();");
        File.WriteAllText(Path.Combine(source, "setup.as"), "var count:Number = 0; for(var i:Number=0;i<3;i++){count+=i;} trace(count);");
        File.WriteAllText(Path.Combine(source, "logic", "Controller.as"), "class logic.Controller extends MovieClip { private var count:Number=0; public static var STEP:Number=2; function Controller(){super();} public function advance(amount:Number):Number {count += amount; return count;} }");
        using var plugin = new Plugin(); plugin.Initialize(null, TargetPlatform.Win32);
        string output = Path.Combine(testRoot, "output", "Mod");
        var first = plugin.CompileFolder(source, output);
        Check(Directory.GetFiles(Path.GetDirectoryName(output)).Length == 4, "Only final four stream files");
        Check(first.InputFiles.Count == 4, "FLA and all AS dependencies");
        File.AppendAllText(Path.Combine(source, "setup.as"), "\ntrace(\"changed\");");
        var second = plugin.CompileFolder(source, output);
        Check(!first.Binary.SequenceEqual(second.Binary), "External AS change rebuilds the stream");
        Check(first.Assets.Count == 3, "Script-only movie still emits all three APT assets");
        Console.WriteLine("Folder/AS/include/class/four-stream checks passed.");
    }
    finally { if (Directory.Exists(testRoot)) Directory.Delete(testRoot, true); }
}
foreach (string file in args)
{
    FlaDocument document = FlaDocument.Read(File.ReadAllBytes(file));
    Check(document.Contents.Number("revision") == 0x3f, "Flash 8 revision");
    Check(document.Scenes.Any(), "Scene list");
    byte[] swf = new BinaryAssetBuilder.UiCompiler.Swf.FlaCompiler(document).Compile();
    Check(swf[3] == 8, "SWF 8 version");
    var apt = new eaf2apt.AptConverter().Convert(swf);
    Check(apt.Apt.Length > 16 && apt.Constants.Length > 16, "APT output");
    using var plugin = new Plugin(); plugin.Initialize(null, TargetPlatform.Win32);
    var streams = plugin.CompileMod(file, "UiTest");
    Check(streams.Assets.Count == 3 + apt.Geometry.Count + apt.Textures.Count, "Complete asset group");
    Check(streams.Manifest[1] == 1 && streams.Manifest[2] == 6, "Linked manifest v6");
    Check(BitConverter.ToUInt32(streams.Manifest, 4) == BitConverter.ToUInt32(streams.Binary), "Binary checksum");
    Console.WriteLine("Compiled " + Path.GetFileName(file) + ": " + document.Pages.Count + " pages, " + document.Payloads.Count + " media; SWF=" + swf.Length + ", APT=" + apt.Apt.Length + ", geometry=" + apt.Geometry.Count + ", textures=" + apt.Textures.Count);
}
Console.WriteLine(passed + " checks passed.");
