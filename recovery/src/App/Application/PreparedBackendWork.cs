#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.AudioCompiler;
using Recovered.W3x;

namespace NewModCompiler.Application
{
    // This is a backend input, not a serialized replacement for Core's semantic
    // front end. Prepare runs on the owner after schema/reference processing.
    // The certified bridges consume XML + namespace bindings + type identity;
    // none reads schema objects, another asset's payload or its owning processor.
    internal sealed class PreparedBackendWork : IDisposable
    {
        readonly IAssetBuilderPlugin plugin;
        readonly InstanceDeclaration declaration;
        readonly List<FileStream> inputLeases;
        bool disposed;

        public string Family { get; private set; }
        public bool CanRunParallel { get; private set; }
        public long PreparedBytes { get; private set; }
        public long ExternalReadBytes { get; private set; }
        public InstanceDeclaration Declaration { get { return declaration; } }

        PreparedBackendWork(string family, IAssetBuilderPlugin plugin, InstanceDeclaration declaration,
            List<FileStream> inputLeases, long preparedBytes)
        {
            Family = family; this.plugin = plugin; this.declaration = declaration;
            this.inputLeases = inputLeases;
            PreparedBytes = preparedBytes; CanRunParallel = true;
            foreach (FileStream input in inputLeases) ExternalReadBytes = checked(ExternalReadBytes + input.Length);
        }

        public static bool TryPrepare(InstanceDeclaration original, IAssetBuilderPlugin plugin,
            out PreparedBackendWork prepared)
        {
            return TryPrepareWithinBudget(original, plugin, 128L * 1024L * 1024L, out prepared);
        }

        public static bool TryPrepareWithinBudget(InstanceDeclaration original, IAssetBuilderPlugin plugin,
            long preparationBudget, out PreparedBackendWork prepared)
        {
            string reason;
            return TryPrepareWithReason(original, plugin, preparationBudget, out prepared, out reason);
        }

        // The reason records the first actual gate that refused preparation.
        // It describes neither a cache lookup nor a backend execution failure;
        // exceptions retain their original propagation and compatibility path.
        public static bool TryPrepareWithReason(InstanceDeclaration original, IAssetBuilderPlugin plugin,
            long preparationBudget, out PreparedBackendWork prepared, out string reason)
        {
            prepared = null;
            reason = null;
            string family;
            if (!Eligibility(original, plugin, out family, out reason)) return false;

            long preparationEstimate = EstimatePreparationBytes(original);
            // Avoid first allocating an enormous private representation and
            // only then discovering that the task exceeds the preparation pool.
            // The owner compatibility path retains the original input contract.
            if (preparationEstimate > preparationBudget) return Reject("preparation-budget-exceeded", out reason);

            var leases = new List<FileStream>();
            try
            {
                // Only the certified backend's actual reopened files require
                // leases. No action identity, serialization or file digest is built.
                if (family == "xml" && !PrepareXmlReads(original, leases, out reason)) return false;
                if (family == "audio" && !PrepareAudio(original, leases, out reason)) return false;
                long retained = preparationEstimate;
                if (family == "w3x") retained = W3xBudget(original.Node, retained);
                else if (family == "xml") retained = ExternalReadBudget(leases, retained, 4L);
                else retained = ExternalReadBudget(leases, retained,
                    original.Handle.TypeId == AudioCatalog.AudioFileTypeId ? 64L : 4L);
                if (retained > preparationBudget) return Reject("preparation-budget-exceeded", out reason);
                var document = new XmlDocument { XmlResolver = null, PreserveWhitespace = true };
                // Import into a private document and name table. All type IDs,
                // reference slots, defaults and evaluated values were installed
                // by the existing front end before this immutable copy is taken.
                XmlNode node = CopyBackendXml(document, original.Node);
                document.AppendChild(node);
                var namespaces = new XmlNamespaceManager(document.NameTable);
                var bindings = original.Document.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.All);
                var prefixes = new List<string>(bindings.Keys);
                foreach (string prefix in prefixes)
                    if (prefix != "xml" && prefix != "xmlns") namespaces.AddNamespace(prefix, bindings[prefix]);
                var frozen = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(namespaces, original.Document.HasValidatedSchema));
                frozen.XmlNode = node;
                frozen.Handle.TypeHash = original.Handle.TypeHash;
                frozen.Handle.InstanceHash = original.Handle.InstanceHash;
                frozen.ProcessingHash = original.ProcessingHash;
                frozen.HasCustomData = original.HasCustomData;
                frozen.IsInheritable = original.IsInheritable;
                frozen.CustomDataPath = original.CustomDataPath;
                CopyReferences(original.ReferencedInstances, frozen.ReferencedInstances);
                CopyReferences(original.WeakReferencedInstances, frozen.WeakReferencedInstances);
                foreach (string file in original.ReferencedFiles) frozen.ReferencedFiles.Add(file);

                prepared = new PreparedBackendWork(family, plugin, frozen, leases, retained);
                leases = null; // Ownership moves to the envelope through commit.
                return true;
            }
            finally
            {
                if (leases != null) foreach (FileStream lease in leases) lease.Dispose();
            }
        }

        // Cheap route gates: no XML traversal, clone, allocation or external I/O.
        public static bool CanPrepare(InstanceDeclaration original, IAssetBuilderPlugin plugin, out string reason)
        {
            string family;
            return Eligibility(original, plugin, out family, out reason);
        }

        static bool Eligibility(InstanceDeclaration original, IAssetBuilderPlugin plugin, out string family, out string reason)
        {
            family = null;
            reason = null;
            if (original == null || original.Node == null || original.Document == null || plugin == null)
                return Reject("invalid-input", out reason);
            // Exact built-in classes only: a subclass may introduce untracked
            // reads, metadata writes or registrations even for an identical type.
            Type implementation = plugin.GetType();
            if (implementation == typeof(Recovered.XmlPriority.Plugin))
            {
                family = "xml";
                if (!IsParallelXml(original.Handle.TypeId)) return Reject("uncertified-xml-type", out reason);
            }
            else if (implementation == typeof(Recovered.Compiler.W3x.NativePlugin))
            {
                family = "w3x";
                uint type = original.Handle.TypeId;
                if (type != W3xCatalog.AnimationTypeId && type != W3xCatalog.MeshTypeId)
                    return Reject("uncertified-w3x-type", out reason);
            }
            else if (implementation == typeof(BinaryAssetBuilder.AudioCompiler.Plugin))
            {
                family = "audio";
                uint type = original.Handle.TypeId;
                if (type == AudioCatalog.AudioFileTypeId)
                {
                    if (!((BinaryAssetBuilder.AudioCompiler.Plugin)plugin).UsesRecoveredAudioFileCompiler)
                        return Reject("uncertified-audio-compiler", out reason);
                    if (original.Node.Attributes["PCSampleRate"] != null)
                        return Reject("audio-explicit-sample-rate", out reason);
                    XmlAttribute compression = original.Node.Attributes["PCCompression"];
                    if (compression != null && compression.Value != "NONE" && compression.Value != "XAS")
                        return Reject("audio-compression-diagnostic", out reason);
                }
                else if (type != AudioCatalog.Mp3PassthroughTypeId)
                    return Reject("uncertified-audio-type", out reason);
            }
            else return Reject("uncertified-plugin", out reason);

            return true;
        }

        public static long EstimatePreparationBytes(InstanceDeclaration original)
        {
            if (original == null || original.Node == null) return 0;
            // Count the existing tree before allocating the private DOM. The
            // allowance covers private node/string storage and reference metadata.
            try { return checked(4096L + EstimateNode(original.Node)); }
            catch (OverflowException) { return long.MaxValue; }
        }

        static long EstimateNode(XmlNode node)
        {
            long characters = (long)node.Name.Length + node.NamespaceURI.Length + (node.Value == null ? 0L : node.Value.Length);
            long bytes = checked(512L + characters * 8L);
            if (node.Attributes != null)
                foreach (XmlAttribute attribute in node.Attributes) bytes = checked(bytes + EstimateNode(attribute));
            foreach (XmlNode child in node.ChildNodes) bytes = checked(bytes + EstimateNode(child));
            return bytes;
        }

        public AssetBuffer Execute()
        {
            if (disposed) throw new ObjectDisposedException("PreparedBackendWork");
            // W3X's initialized plugin only reads its immutable platform field;
            // its backend consumes this private node/namespace view. Native
            // scratch and outputs are per-call; the only lazy native table is
            // const and built with C++17's thread-safe static initialization.
            // Certified XML routes use a per-call bridge tree and Relo::Tracker;
            // XmlPriority's optional trace writes already hold TraceGate. Audio
            // uses a stateless exact encoder, readonly tables and private CDATA.
            // Other routes remain owner barriers, including Audio's Tracer paths.
            return plugin.ProcessInstance(declaration);
        }

        public void Dispose()
        {
            if (disposed) return;
            foreach (FileStream lease in inputLeases) lease.Dispose();
            disposed = true;
        }

        static XmlNode CopyBackendXml(XmlDocument document, XmlNode source)
        {
            if (source.NodeType != XmlNodeType.Element) return document.ImportNode(source, true);
            XmlElement copy = document.CreateElement(source.Prefix, source.LocalName, source.NamespaceURI);
            // XmlDocument.ImportNode deliberately drops unspecified default
            // attributes. Schema validation has already made their values part
            // of the real backend input, so copy every effective attribute.
            foreach (XmlAttribute attribute in source.Attributes)
            {
                XmlAttribute value = document.CreateAttribute(attribute.Prefix, attribute.LocalName, attribute.NamespaceURI);
                value.Value = attribute.Value; copy.Attributes.Append(value);
            }
            foreach (XmlNode child in source.ChildNodes) copy.AppendChild(CopyBackendXml(document, child));
            return copy;
        }

        static bool Reject(string value, out string reason)
        {
            reason = value;
            return false;
        }

        static bool PrepareXmlReads(InstanceDeclaration original, List<FileStream> leases, out string reason)
        {
            reason = null;
            uint type = original.Handle.TypeId;
            if (!IsApt(type)) return true;
            XmlAttribute file = original.Node.Attributes["File"];
            if (file == null) return true;
            // Native APT uses narrow fopen. ASCII preserves the exact pathname
            // shared by the managed lease; other paths use owner compatibility.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return Reject("unsupported-platform", out reason);
            foreach (char character in file.Value)
                if (character > 127) return Reject("native-path-non-ascii", out reason);
            return LeaseInputPath(file.Value, leases, out reason);
        }

        static bool IsApt(uint type)
        {
            return type == 1816258268u || type == 983359664u || type == 1110676961u || type == 1034587168u;
        }

        static bool IsParallelXml(uint type)
        {
            // APT retains its separately certified read-lease envelope. All
            // metadata routes use the closed module audit, never an open-ended
            // "all XML except" rule. Unknown/new routes remain owner-only.
            return IsApt(type) || XmlBackendCertification.IsCertified(type);
        }

        static long ExternalReadBudget(List<FileStream> leases, long minimum, long multiplier)
        {
            try
            {
                long bytes = minimum;
                foreach (FileStream lease in leases) bytes = checked(bytes + checked(lease.Length * multiplier));
                return bytes;
            }
            catch (OverflowException) { return long.MaxValue; }
        }

        static bool PrepareAudio(InstanceDeclaration original, List<FileStream> leases, out string reason)
        {
            // This exact Windows share mode excludes replacement, deletion and
            // writing while the backend reopens its original pathname.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return Reject("unsupported-platform", out reason);
            XmlAttribute file = original.Node.Attributes["File"];
            return LeaseInputPath(file == null ? null : file.Value, leases, out reason);
        }

        static bool LeaseInputPath(string value, List<FileStream> leases, out string reason)
        {
            reason = null;
            if (string.IsNullOrEmpty(value)) return Reject("external-input-path-missing", out reason);
            try
            {
                string path = Path.GetFullPath(value);
                for (var parent = new FileInfo(path).Directory; parent != null; parent = parent.Parent)
                    if (parent.Exists && (parent.Attributes & FileAttributes.ReparsePoint) != 0)
                        return Reject("external-input-reparse-point", out reason);
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                    return Reject("external-input-reparse-point", out reason);
                leases.Add(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
                return true;
            }
            catch (IOException) { return Reject("external-input-unavailable", out reason); }
            catch (UnauthorizedAccessException) { return Reject("external-input-unavailable", out reason); }
            catch (ArgumentException) { return Reject("external-input-path-invalid", out reason); }
            catch (NotSupportedException) { return Reject("external-input-path-invalid", out reason); }
        }

        static void CopyReferences(IEnumerable<InstanceHandle> source, List2<InstanceHandle> destination)
        {
            foreach (InstanceHandle handle in source)
            {
                var copy = new InstanceHandle(handle.TypeName, handle.InstanceName);
                copy.TypeHash = handle.TypeHash; copy.InstanceHash = handle.InstanceHash;
                destination.Add(copy);
            }
        }

        static long W3xBudget(XmlNode root, long minimum)
        {
            // Worst-case channel/frame expansion is much larger than its XML.
            // Count every element and apply a conservative per-node allowance;
            // declared-frame animation expansion is covered independently.
            long nodes = CountNodes(root);
            long frames = 0;
            XmlAttribute numFrames = root.Attributes["NumFrames"];
            uint value;
            if (numFrames != null && !uint.TryParse(numFrames.Value, out value)) return long.MaxValue;
            if (numFrames != null) frames = uint.Parse(numFrames.Value, System.Globalization.CultureInfo.InvariantCulture);
            long channels = root.SelectNodes("*[local-name()='Channels']/*").Count;
            try { return checked(minimum + nodes * 1024L + frames * Math.Max(1L, channels) * 256L); }
            catch (OverflowException) { return long.MaxValue; }
        }

        static long CountNodes(XmlNode root)
        {
            long result = 1;
            foreach (XmlNode child in root.ChildNodes) result += CountNodes(child);
            return result;
        }
    }
}
#endif
