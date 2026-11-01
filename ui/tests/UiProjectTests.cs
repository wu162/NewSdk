using BinaryAssetBuilder.UiCompiler;
using BinaryAssetBuilder.UiCompiler.Compilation;
using BinaryAssetBuilder.UiCompiler.Fla;
using BinaryAssetBuilder.UiCompiler.Projects;
using BinaryAssetBuilder.UiCompiler.Runtime;

internal static class UiProjectTests
{
    internal static void Run(Action<bool, string> check)
    {
        string root = Path.Combine(Path.GetTempPath(), "UiProject-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "ui"));
        Directory.CreateDirectory(Path.Combine(root, "unrelated"));
        try
        {
            File.WriteAllText(Path.Combine(root, "ui", "Panel.as"), LayoutDocument.ClassSource);
            File.WriteAllText(Path.Combine(root, "unrelated", "Panel.as"), "class ui.Panel { function conflicting(){} }");
            string master = Path.Combine(root, "Master.fla");
            File.Copy(Path.Combine(AppContext.BaseDirectory, "fixtures", "flash8-empty.fla"), master);
            var movie = Movie();
            var expectedDocument = Document(false);
            var expectedModel = new FlaCompiler(expectedDocument, new SourceProject(root, movie.Sources), master).Compile();
            var expected = new ModStreams(new AssetEncoder(expectedModel).Build(movie.Name, movie.SourceName));
            var document = Document(true);
            var originalNames = document.Library.Select(d => d.Text("name")).ToArray();
            var source = new SourceProject(root, movie.Sources);
            var selected = new FlaCompiler(document, source, master, movie).Compile();
            var actual = new ModStreams(new AssetEncoder(selected).Build(movie.Name, movie.SourceName));
            check(selected.Frames.Count == 2 && selected.Symbols.Count == expectedModel.Symbols.Count,
                "project library root is published directly, without an extra wrapper symbol/frame");
            check(selected.WidthTwips == 7400 && selected.HeightTwips == 4200 && selected.FrameRate == 29.5,
                "project root restores its own stage dimensions and frame rate");
            check(selected.Exports.Count == 1 && selected.Exports[0].Key == "ProjectPanel",
                "project root restores its own runtime linkage alias");
            check(expected.Manifest.SequenceEqual(actual.Manifest) && expected.Binary.SequenceEqual(actual.Binary) &&
                  expected.Relocations.SequenceEqual(actual.Relocations) && expected.Imports.SequenceEqual(actual.Imports),
                "project root, media, linked class and events produce identical final four streams");
            check(source.Inputs.Count == 1 && source.Inputs[0] == Path.Combine(root, "ui", "Panel.as"),
                "project AS scope excludes an unrelated conflicting class");
            check(document.Library.Select(d => d.Text("name")).SequenceEqual(originalNames) &&
                  document.Library.Single(d => d.Text("name") == "Panel").Record("linkage").Text("identifier") == "OriginalPanel",
                "project alias overlay does not mutate the shared master document");

            void Reject(Action action, string message)
            {
                bool rejected = false;
                try { action(); }
                catch (Exception ex) when (ex is InvalidDataException || ex is FileNotFoundException || ex is NotSupportedException)
                { rejected = true; }
                check(rejected, message);
            }
            var duplicate = Movie(); duplicate.LibraryOrder.Add("Panel");
            Reject(() => new FlaCompiler(Document(true), new SourceProject(root, duplicate.Sources), master, duplicate),
                "project rejects duplicate library dependency entries");
            var missing = Movie(); missing.EntrySymbol = "Roots/Missing";
            Reject(() => new FlaCompiler(Document(true), new SourceProject(root, missing.Sources), master, missing).Compile(),
                "project rejects a missing entry symbol");
            var wrapped = Movie(); wrapped.LibraryOrder.Add(wrapped.EntrySymbol);
            Reject(() => new FlaCompiler(Document(true), new SourceProject(root, wrapped.Sources), master, wrapped).Compile(),
                "project rejects publishing its own root as a dependency wrapper");
            var badLinkage = Movie(); badLinkage.Linkages[0].ItemName = "unselected";
            Reject(() => new FlaCompiler(Document(true), new SourceProject(root, badLinkage.Sources), master, badLinkage),
                "project rejects a linkage outside the selected movie dependency scope");
            bool outsideRejected = false;
            try { new SourceProject(root, new[] { "../outside.as" }); }
            catch (InvalidDataException error) { outsideRejected = error.Message.Contains("outside the source root"); }
            check(outsideRejected, "project rejects AS paths outside its source root before checking file existence");
            Reject(() => new SourceProject(root, new[] { "missing.as" }), "project rejects missing declared AS dependencies");
            foreach (string badName in new[] { ".", "..", "../outside", "folder\\outside", "C:outside", "bad\0name" })
            {
                var unsafeMovie = Movie(); unsafeMovie.Name = badName;
                var unsafeProject = new UiProjectDefinition { MasterFile = master, SourceRoot = root, Movies = new() { unsafeMovie } };
                bool rejectedAsName = false;
                try { new UiProjectCompiler().Compile(unsafeProject, badName); }
                catch (InvalidDataException error)
                { rejectedAsName = error.Message.Contains("portable filename basename"); }
                check(rejectedAsName, "project rejects output-path movie names before reading publish dependencies: " + badName.Replace("\0", "NUL"));
            }
            File.WriteAllText(Path.Combine(root, "hidden.as"), "trace(1);");
            var hidden = Document(true);
            hidden.Pages[hidden.Scenes.First().Text("streamName")].getChildren()[0].getChildren()[0].Record("actions")
                .set("source", "#include \"hidden.as\"");
            Reject(() => new FlaCompiler(hidden, new SourceProject(root, movie.Sources), master, movie).Compile(),
                "project rejects an include omitted from the declared source closure");
        }
        finally { Directory.Delete(root, true); }
    }

    private static UiProjectMovie Movie() => new()
    {
        Name = "ProjectMovie", SourceName = "Original.fla", EntrySymbol = "Roots/Layout",
        WidthTwips = 7400, HeightTwips = 4200, FrameRate = 29.5, BackgroundColor = 0xff332211,
        LibraryOrder = new() { "Button", "Panel" }, MediaOrder = new() { "Pixels" },
        Sources = new() { "ui/Panel.as" },
        Linkages = new() { new UiProjectLinkage { ItemName = "Panel", Identifier = "ProjectPanel", ClassName = "ui.Panel", Flags = 1 } }
    };

    private static FlaDocument Document(bool master)
    {
        var document = LayoutDocument.Create();
        foreach (var item in document.Library) item.set("name", item.Text("streamName"));
        foreach (var item in document.Media) item.set("name", item.Text("streamName"));
        document.Contents.set("boundsWords", new List<int> { 0, 7400, 0, 4200 });
        document.Contents.set("frameRateWord", (long)(29.5 * 65536));
        document.Contents.set("backgroundColorRgba", 0xff332211L);
        var linkage = document.Library.Single(d => d.Text("name") == "Panel").Record("linkage");
        linkage.set("identifier", master ? "OriginalPanel" : "ProjectPanel");
        if (master)
        {
            // A named library entry points at the original root's own timeline.
            // A distinct master stage proves that the publish profile is used.
            var entry = new FlaRecord("CLibSymbol"); entry.set("libraryId", 100); entry.set("name", "Roots/Layout");
            entry.set("streamName", document.Scenes.First().Text("streamName"));
            document.Contents.Records("library").Add(entry);
            document.Contents.set("boundsWords", new List<int> { 0, 10000, 0, 9000 });
            document.Contents.set("frameRateWord", 12 * 65536L);
            document.Contents.set("backgroundColorRgba", 0xffeeeeeeL);
        }
        return document;
    }
}
