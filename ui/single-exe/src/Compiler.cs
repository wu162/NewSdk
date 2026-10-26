// SPDX-License-Identifier: GPL-3.0-only
namespace NewSdk.Ui;

public sealed record CompilerOptions(string LogicalName, bool Optimize = false);
public enum ResourceKind { Apt, Constants, TextureMap, Geometry, Texture }
public sealed record CompiledResource(ResourceKind Kind, string Name,
    ReadOnlyMemory<byte> Data, int? AptId = null);
public sealed record CompilationResult(IReadOnlyList<CompiledResource> Resources);

// The backend owns its returned arrays. Their lifetime does not depend on the
// input buffer or a native call's temporary allocations. No paths in this API.
public interface ISwfBackend
{
    CompilationResult Compile(ReadOnlyMemory<byte> input, CompilerOptions options);
}

public sealed class BackendUnavailableException(string message) : Exception(message) { }

public sealed class SwfCompiler(ISwfBackend? backend = null)
{
    public CompilationResult Compile(ReadOnlyMemory<byte> input, CompilerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.LogicalName);
        _ = SwfInspector.Inspect(input.Span);
        if (backend is null)
            throw new BackendUnavailableException(
                "SWF conversion backend has not been migrated. The single-EXE framework cannot compile SWF yet.");
        return backend.Compile(input, options);
    }
}
