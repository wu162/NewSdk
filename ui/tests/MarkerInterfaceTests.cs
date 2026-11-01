using BinaryAssetBuilder.UiCompiler.Compilation;

internal static class MarkerInterfaceTests
{
    internal static void Run(Action<bool, string> check)
    {
        string root = Path.Combine(Path.GetTempPath(), "UiMarkerInterfaces-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "api"));
        try
        {
            File.WriteAllText(Path.Combine(root, "api", "IMarker.as"), "interface api.IMarker {}");
            File.WriteAllText(Path.Combine(root, "ISecond.as"), "interface ISecond {}");
            File.WriteAllText(Path.Combine(root, "IOther.as"), "interface IOther {}");
            File.WriteAllText(Path.Combine(root, "Direct.as"), "import api.IMarker; class Direct implements IMarker,ISecond { function value():Number{return 7;} }");
            File.WriteAllText(Path.Combine(root, "Child.as"), "class Child extends Direct {}");
            File.WriteAllText(Path.Combine(root, "Unrelated.as"), "class Unrelated {}");
            var project = new SourceProject(root);
            project.RequireClass("Child"); project.RequireClass("Unrelated"); project.RequireClass("IOther");
            check(project.ClassOrder.FindIndex(c => c.Name == "api.IMarker") < project.ClassOrder.FindIndex(c => c.Name == "Direct") &&
                project.ClassOrder.FindIndex(c => c.Name == "ISecond") < project.ClassOrder.FindIndex(c => c.Name == "Direct"), "marker interfaces initialize before implementing classes");
            check(project.ClassOrder.Single(c => c.Name == "api.IMarker").IsInterface, "empty interface is discovered as an AS2 type");
            check(project.ClassOrder.Single(c => c.Name == "Direct").Code.Instructions.Any(i => i.Operation == 0x2c), "class implements emits AVM1 ImplementsOp");
            // Avm1Machine serializes every program into final AssetBuffers, then
            // independently reads the game action stream and constant table.
            var machine = new Avm1Machine();
            foreach (var type in project.ClassOrder) machine.Run(type.Code);
            // Inlined shared libraries may initialize the same global marker
            // again; preserving its prototype keeps existing implements links.
            machine.Run(project.ClassOrder.Single(c => c.Name == "api.IMarker").Code);
            machine.Run(ActionScriptCompiler.Compile("var direct=new Direct(); var child=new Child(); trace(direct instanceof api.IMarker); trace(direct instanceof ISecond); trace(child instanceof api.IMarker); trace(child instanceof Direct); trace(new Unrelated() instanceof api.IMarker); trace(child instanceof IOther); trace(7 instanceof api.IMarker); trace(child.value());"));
            check(string.Join(",", machine.Traces) == "True,True,True,True,False,False,False,7", "final AssetBuffer execution preserves direct/inherited marker instanceof and unrelated/primitive negatives: " + string.Join(",", machine.Traces));

            void Reject(string name, string source, string message)
            {
                File.WriteAllText(Path.Combine(root, name + ".as"), source);
                bool rejected = false;
                try { new SourceProject(root).RequireClass(name); }
                catch (InvalidDataException) { rejected = true; }
                check(rejected, message);
                File.Delete(Path.Combine(root, name + ".as"));
            }
            Reject("Missing", "class Missing implements IMissing {}", "missing marker dependency is rejected");
            Reject("BadType", "class BadType implements Unrelated {}", "class cannot implement a non-interface");
            Reject("BadBase", "class BadBase extends api.IMarker {}", "class cannot extend a marker interface");
            Reject("Duplicate", "class Duplicate implements ISecond,ISecond {}", "duplicate marker dependency is rejected");
            Reject("NonEmpty", "interface NonEmpty { function run():Void; }", "nonempty interface remains explicitly unsupported");
            Reject("DerivedMarker", "interface DerivedMarker extends ISecond {}", "interface inheritance remains explicitly unsupported");
        }
        finally { Directory.Delete(root, true); }
    }
}
