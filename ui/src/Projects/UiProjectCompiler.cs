using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.UiCompiler.Compilation;
using BinaryAssetBuilder.UiCompiler.Fla;
using BinaryAssetBuilder.UiCompiler.Runtime;

namespace BinaryAssetBuilder.UiCompiler.Projects
{
    /// <summary>Compiles selected master-FLA library timelines as independent final APT movies.</summary>
    public sealed class UiProjectCompiler
    {
        public UiAssetGroup Compile(UiProjectDefinition project, string movieName)
        {
            var prepared = Prepare(project);
            var movies = project.Movies.Where(m => m.Name == movieName).ToArray();
            if (movies.Length != 1) throw new InvalidDataException("Unknown project movie: " + movieName);
            return Compile(prepared.Document, prepared.MasterFile, prepared.SourceRoot, movies[0]);
        }

        public ModStreams CompileStreams(UiProjectDefinition project, string movieName) => new ModStreams(Compile(project, movieName));

        /// <summary>Reads the master document once, then compiles every root with an isolated AS scope.</summary>
        public IReadOnlyDictionary<string, UiAssetGroup> CompileAll(UiProjectDefinition project)
        {
            var prepared = Prepare(project);
            var result = new Dictionary<string, UiAssetGroup>(StringComparer.Ordinal);
            foreach (var movie in project.Movies)
                result.Add(movie.Name, Compile(prepared.Document, prepared.MasterFile, prepared.SourceRoot, movie));
            return result;
        }

        public IReadOnlyDictionary<string, ModStreams> CompileAllStreams(UiProjectDefinition project)
        {
            var result = new Dictionary<string, ModStreams>(StringComparer.Ordinal);
            foreach (var movie in CompileAll(project)) result.Add(movie.Key, new ModStreams(movie.Value));
            return result;
        }

        private static (FlaDocument Document, string MasterFile, string SourceRoot) Prepare(UiProjectDefinition project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            if (string.IsNullOrWhiteSpace(project.MasterFile) || string.IsNullOrWhiteSpace(project.SourceRoot)) throw new InvalidDataException("Project needs MasterFile and SourceRoot.");
            string master = Path.GetFullPath(project.MasterFile), root = Path.GetFullPath(project.SourceRoot);
            string prefix = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!master.StartsWith(prefix, comparison)) throw new InvalidDataException("Project master FLA must be inside SourceRoot.");
            if (!Path.GetExtension(master).Equals(".fla", StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Project master must be a binary FLA.");
            if (project.Movies == null || project.Movies.Count == 0) throw new InvalidDataException("Project has no publish roots.");
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var movie in project.Movies)
            {
                if (movie == null || string.IsNullOrWhiteSpace(movie.Name) || !names.Add(movie.Name)) throw new InvalidDataException("Project movie names must be unique and nonempty.");
                SageHash.Symbol(movie.Name);
                if (string.IsNullOrWhiteSpace(movie.SourceName) || movie.SourceName.IndexOfAny(new[] { '/', '\\', '\0' }) >= 0) throw new InvalidDataException("Project SourceName must preserve a movie source basename.");
                if (string.IsNullOrWhiteSpace(movie.EntrySymbol)) throw new InvalidDataException("Project movie needs an EntrySymbol.");
                if (movie.WidthTwips <= 0 || movie.HeightTwips <= 0 || movie.FrameRate <= 0 || movie.FrameRate >= 256 || double.IsNaN(movie.FrameRate)) throw new InvalidDataException("Invalid project stage dimensions/frame rate.");
                if (movie.LibraryOrder == null || movie.MediaOrder == null || movie.Sources == null || movie.Linkages == null) throw new InvalidDataException("Project movie dependency lists must be explicit.");
            }
            return (FlaDocument.Read(File.ReadAllBytes(master)), master, root);
        }

        private static UiAssetGroup Compile(FlaDocument document, string master, string root, UiProjectMovie movie)
        {
            var previousCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var source = new SourceProject(root, movie.Sources);
                var scene = new FlaCompiler(document, source, master, movie).Compile();
                var assets = new AssetEncoder(scene).Build(movie.Name, movie.SourceName);
                return new UiAssetGroup(assets, new[] { master }.Concat(source.Inputs).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p, StringComparer.Ordinal).ToArray());
            }
            catch (Exception ex) when (ex is InvalidDataException || ex is NotSupportedException || ex is EndOfStreamException)
            { throw new InvalidDataException("Could not compile UI project movie '" + movie.Name + "': " + ex.Message, ex); }
            finally { System.Threading.Thread.CurrentThread.CurrentCulture = previousCulture; }
        }
    }
}
