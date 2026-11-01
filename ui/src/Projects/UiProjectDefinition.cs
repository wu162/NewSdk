using System.Collections.Generic;

namespace BinaryAssetBuilder.UiCompiler.Projects
{
    /// <summary>A source workspace whose named library timelines publish as separate movies.</summary>
    public sealed class UiProjectDefinition
    {
        public string MasterFile { get; set; }
        public string SourceRoot { get; set; }
        public List<UiProjectMovie> Movies { get; set; } = new List<UiProjectMovie>();
    }

    public sealed class UiProjectMovie
    {
        public string Name { get; set; }
        public string SourceName { get; set; }
        public string EntrySymbol { get; set; }
        public int WidthTwips { get; set; }
        public int HeightTwips { get; set; }
        public double FrameRate { get; set; }
        /// <summary>Packed R | G&lt;&lt;8 | B&lt;&lt;16 | A&lt;&lt;24, matching FLA backgroundColorRgba.</summary>
        public uint BackgroundColor { get; set; } = 0xffffffff;
        /// <summary>Exact binary-FLA item names in original descriptor order; excludes EntrySymbol.</summary>
        public List<string> LibraryOrder { get; set; } = new List<string>();
        public List<string> MediaOrder { get; set; } = new List<string>();
        /// <summary>Only this movie's actual AS closure, relative to SourceRoot.</summary>
        public List<string> Sources { get; set; } = new List<string>();
        /// <summary>Original per-movie runtime export/class names. Unlisted items have no linkage.</summary>
        public List<UiProjectLinkage> Linkages { get; set; } = new List<UiProjectLinkage>();
    }

    public sealed class UiProjectLinkage
    {
        public string ItemName { get; set; }
        public string Identifier { get; set; }
        public string ClassName { get; set; }
        public int Flags { get; set; }
    }
}
