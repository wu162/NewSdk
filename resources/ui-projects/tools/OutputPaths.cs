namespace UiProjectTools;

/// <summary>Keep all previously recovered resources read-only, including tool output.</summary>
public static class OutputPaths
{
    public static void EnsureWritableOutput(string output, string? projectDirectory = null)
    {
        string destination = Path.GetFullPath(output).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var seen = new HashSet<string>(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
        foreach (string start in new[] { destination, projectDirectory ?? Directory.GetCurrentDirectory(), Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (var folder = new DirectoryInfo(Path.GetFullPath(start)); folder != null && seen.Add(folder.FullName); folder = folder.Parent)
            {
                string marker = Path.Combine(folder.FullName, ".git");
                if (!Directory.Exists(marker) && !File.Exists(marker)) continue;
                string preserved = Path.Combine(folder.FullName, "resources", "ra3-ui");
                if (destination.Equals(preserved, comparison) || destination.StartsWith(preserved + Path.DirectorySeparatorChar, comparison))
                    throw new InvalidOperationException("Output cannot be inside the preserved recovered resource tree: " + preserved);
            }
        }
    }
}
