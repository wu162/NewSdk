using System;
using System.IO;
using System.Threading;
using BinaryAssetBuilder.Core;
using Recovered.W3x;

namespace Recovered.Compiler.W3x
{
    // Loadable recovered implementation with its own assembly/type identity.
    // The original managed metadata and dispatch shell remains unchanged.
    public sealed class NativePlugin : Plugin
    {
        public NativePlugin() : base(new Backend()) { }

        private sealed class Backend : IW3xAssetCompiler
        {
            private static long traceSequence;

            private static AssetBuffer Trace(string route, InstanceDeclaration declaration,
                TargetPlatform platform, AssetBuffer buffer)
            {
                string directory = Environment.GetEnvironmentVariable("RECOVERED_W3X_TRACE_DIRECTORY");
                if (String.IsNullOrEmpty(directory)) return buffer;
                if (!Path.IsPathRooted(directory))
                    throw new InvalidOperationException("RECOVERED_W3X_TRACE_DIRECTORY must be an absolute path.");
                Directory.CreateDirectory(directory);
                string name = Interlocked.Increment(ref traceSequence).ToString("D10") + "-" +
                    route + "-" + Guid.NewGuid().ToString("N") + ".fixtures";
                using (BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(directory, name),
                    FileMode.CreateNew, FileAccess.Write, FileShare.None)))
                {
                    writer.Write("W3X-FIXTURE-1");
                    writer.Write(route);
                    writer.Write((int)platform);
                    writer.Write(declaration.Node.OuterXml);
                    foreach (byte[] bytes in new byte[][] { buffer.InstanceData, buffer.RelocationData, buffer.ImportsData })
                    {
                        writer.Write(bytes.Length);
                        writer.Write(bytes);
                    }
                }
                return buffer;
            }

            public AssetBuffer CompileAnimation(InstanceDeclaration declaration, TargetPlatform platform)
            {
                AssetBuffer buffer = AnimationCompiler.CompileXml(declaration.Node,
                    declaration.Document.NamespaceManager, platform);
                return Trace("animation", declaration, platform, buffer);
            }

            public AssetBuffer CompileMesh(InstanceDeclaration declaration, TargetPlatform platform)
            {
                AssetBuffer buffer = MeshCompiler.CompileXml(declaration.Node,
                    declaration.Document.NamespaceManager, platform);
                return Trace("mesh", declaration, platform, buffer);
            }
        }
    }
}
