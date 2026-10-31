using BinaryAssetBuilder.UiCompiler.Fla;
using BinaryAssetBuilder.UiCompiler;
using BinaryAssetBuilder.Core;

int passed = 0;
void Check(bool condition, string message) { if (!condition) throw new Exception(message); passed++; }
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
