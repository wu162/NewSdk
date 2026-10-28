#if NEWSdk_MONOLITHIC
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.AudioCompiler;

namespace NewModCompiler.Application
{
    // This is a backend input, not a serialized replacement for Core's semantic
    // front end. Prepare runs on the owner after schema/reference processing.
    // The certified bridges consume XML + namespace bindings + type identity;
    // none reads schema objects, another asset's payload or its owning processor.
    internal sealed class PreparedBackend : IDisposable
    {
        readonly IAssetBuilderPlugin plugin;
        readonly InstanceDeclaration declaration;
        readonly byte[] identity;
        readonly List<FileStream> inputLeases;
        bool disposed;

        public string Family { get; private set; }
        public bool CanCache { get { return true; } }
        public bool CanRunParallel { get; private set; }
        public long PreparedBytes { get; private set; }
        public long ExternalReadBytes { get; private set; }
        public InstanceDeclaration Declaration { get { return declaration; } }

        PreparedBackend(string family, IAssetBuilderPlugin plugin, InstanceDeclaration declaration,
            byte[] identity, List<FileStream> inputLeases, long preparedBytes, bool parallel)
        {
            Family = family; this.plugin = plugin; this.declaration = declaration;
            this.identity = identity; this.inputLeases = inputLeases;
            PreparedBytes = preparedBytes; CanRunParallel = parallel;
            foreach (FileStream input in inputLeases) ExternalReadBytes = checked(ExternalReadBytes + input.Length);
        }

        public static bool TryPrepare(InstanceDeclaration original, IAssetBuilderPlugin plugin,
            out PreparedBackend prepared)
        {
            prepared = null;
            if (original == null || original.Node == null || original.Document == null || plugin == null)
                return false;
            string family;
            // Exact built-in classes only: a subclass may introduce untracked
            // reads, metadata writes or registrations even for an identical type.
            Type implementation = plugin.GetType();
            if (implementation == typeof(Recovered.GameObject.Plugin)) family = "gameobject";
            else if (implementation == typeof(Recovered.Weapon.Plugin)) family = "weapon";
            else if (implementation == typeof(Recovered.Compiler.Plugin)) family = "armor";
            else if (implementation == typeof(Recovered.XmlPriority.Plugin)) family = "xml";
            else if (implementation == typeof(Recovered.Compiler.W3x.NativePlugin)) family = "w3x";
            else if (implementation == typeof(BinaryAssetBuilder.AudioCompiler.Plugin)) family = "audio";
            else return false;

            long preparationEstimate = EstimatePreparationBytes(original);
            // Avoid first allocating an enormous private representation and
            // only then discovering that the task exceeds the preparation pool.
            // The owner compatibility path retains the original input contract.
            if (preparationEstimate > 128L * 1024L * 1024L) return false;

            var leases = new List<FileStream>();
            try
            {
                var document = new XmlDocument { XmlResolver = null, PreserveWhitespace = true };
                // Import into a private document and name table. All type IDs,
                // reference slots, defaults and evaluated values were installed
                // by the existing front end before this immutable copy is taken.
                XmlNode node = CopyBackendXml(document, original.Node);
                document.AppendChild(node);
                var namespaces = new XmlNamespaceManager(document.NameTable);
                var bindings = original.Document.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.All);
                var prefixes = new List<string>(bindings.Keys); prefixes.Sort(StringComparer.Ordinal);
                foreach (string prefix in prefixes)
                    if (prefix != "xml" && prefix != "xmlns") namespaces.AddNamespace(prefix, bindings[prefix]);
                var frozen = new InstanceDeclaration(AssetDeclarationDocument.CreateBackendView(namespaces));
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

                byte[] inputIdentity;
                long retained = preparationEstimate;
                using (var bytes = new MemoryStream())
                using (var writer = new BinaryWriter(bytes, new UTF8Encoding(false, true)))
                {
                    writer.Write("NewModCompiler.PreparedBackend.v1");
                    writer.Write(family); writer.Write(implementation.FullName);
                    writer.Write(node.OuterXml);
                    writer.Write(prefixes.Count);
                    foreach (string prefix in prefixes) { writer.Write(prefix); writer.Write(bindings[prefix]); }
                    WriteHandle(writer, original.Handle);
                    writer.Write(original.ProcessingHash); writer.Write(original.HasCustomData);
                    writer.Write(original.IsInheritable);
                    WriteReferences(writer, original.ReferencedInstances);
                    WriteReferences(writer, original.WeakReferencedInstances);
                    WriteSchema(writer, original.Node);
                    // Most XML bridges serialize File fields as runtime paths;
                    // APT blobs and TestTexture actually open them. Their exact
                    // reads are covered separately from the Audio closure.
                    if (family == "xml" && !PrepareXmlReads(original, writer, leases)) return false;
                    // Hold actual input handles throughout hashing AND execution,
                    // never just hash then reopen freely.
                    if (family == "audio" && !PrepareAudio(original, (BinaryAssetBuilder.AudioCompiler.Plugin)plugin, writer, leases)) return false;
                    writer.Flush(); inputIdentity = bytes.ToArray();
                }
                bool parallel = family == "w3x";
                if (parallel)
                {
                    retained = W3xBudget(node, retained);
                    // Animation expansion depends on declared frames, not just
                    // the small XML source. Oversized work remains owner serial.
                    parallel = retained <= 64L * 1024L * 1024L;
                }
                else if (family == "xml" && IsParallelXml(original.Handle.TypeId))
                {
                    // APT's tracker payload, relocatable copy and managed output
                    // coexist. The seven Xmlaudio roots have no external reads.
                    retained = ExternalReadBudget(leases, retained, 4L);
                    parallel = retained <= 64L * 1024L * 1024L;
                }
                else if (family == "audio")
                {
                    uint type = original.Handle.TypeId;
                    // Without rate conversion, integral WAVE/AIFF decoding,
                    // optional loop slices and PCM/XAS output are bounded by
                    // input bytes. A deliberately conservative allowance covers
                    // float samples, temporary lists, copies and CDATA. Explicit
                    // rates stay serial: expansion depends on source metadata.
                    bool boundedAudioFile = type == AudioCatalog.AudioFileTypeId && node.Attributes["PCSampleRate"] == null;
                    if (boundedAudioFile || type == AudioCatalog.Mp3PassthroughTypeId)
                    {
                        retained = ExternalReadBudget(leases, retained, boundedAudioFile ? 64L : 4L);
                        parallel = retained <= 64L * 1024L * 1024L;
                    }
                    // PathMusic handlers can invoke Core's scoped Tracer even
                    // on an I/O fallback. Their cache/read contracts do not yet
                    // certify worker diagnostic delivery, so retain the owner.
                }
                prepared = new PreparedBackend(family, plugin, frozen, inputIdentity, leases, retained, parallel);
                leases = null; // Ownership moves to the envelope through commit.
                return true;
            }
            finally
            {
                if (leases != null) foreach (FileStream lease in leases) lease.Dispose();
            }
        }

        public void WriteIdentity(BinaryWriter writer)
        {
            if (disposed) throw new ObjectDisposedException("PreparedBackend");
            writer.Write(identity.Length); writer.Write(identity);
        }

        public static long EstimatePreparationBytes(InstanceDeclaration original)
        {
            if (original == null || original.Node == null) return 0;
            // Count the existing tree without OuterXml, a cloned DOM or an
            // encoded identity. The allowance covers original/private strings,
            // serialization scratch, retained identity and per-node metadata.
            try { return checked(4096L + EstimateNode(original.Node)); }
            catch (OverflowException) { return long.MaxValue; }
        }

        static long EstimateNode(XmlNode node)
        {
            long characters = (long)node.Name.Length + node.NamespaceURI.Length + (node.Value == null ? 0L : node.Value.Length);
            long bytes = checked(2048L + characters * 64L);
            if (node.Attributes != null)
                foreach (XmlAttribute attribute in node.Attributes) bytes = checked(bytes + EstimateNode(attribute));
            foreach (XmlNode child in node.ChildNodes) bytes = checked(bytes + EstimateNode(child));
            return bytes;
        }

        public AssetBuffer Execute()
        {
            if (disposed) throw new ObjectDisposedException("PreparedBackend");
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

        static bool PrepareXmlReads(InstanceDeclaration original, BinaryWriter writer, List<FileStream> leases)
        {
            uint type = original.Handle.TypeId;
            if (type == 1816258268u || type == 983359664u || type == 1110676961u || type == 1034587168u)
            {
                XmlAttribute file = original.Node.Attributes["File"];
                writer.Write(file == null ? 0 : 1);
                return file == null || LeaseNativeInput(file.Value, writer, leases);
            }
            if (type == 2826773070u)
            {
                XmlNodeList files = original.Node.SelectNodes("*[local-name()='File']");
                writer.Write(files.Count);
                foreach (XmlNode file in files)
                {
                    // Match XmltesttexturesCompiler.Read's text assembly, which
                    // excludes regular whitespace nodes but includes CDATA.
                    var text = new StringBuilder();
                    foreach (XmlNode child in file.ChildNodes)
                        if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA || child.NodeType == XmlNodeType.SignificantWhitespace)
                            text.Append(child.Value);
                    if (!LeaseNativeInput(text.ToString(), writer, leases)) return false;
                }
                return true;
            }
            writer.Write(0);
            return true;
        }

        static bool LeaseNativeInput(string path, BinaryWriter writer, List<FileStream> leases)
        {
            // The native bridges use narrow fopen. Until codepage conversion is
            // separately proven, only ASCII paths can share the managed lease's
            // exact pathname guarantee. Missing reads retain their real fallback.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return false;
            foreach (char character in path) if (character > 127) return false;
            return LeaseInputPath(path, writer, leases);
        }

        static bool IsParallelXml(uint type)
        {
            switch (type)
            {
                // XmlaptdataCompiler.cpp / Xmlaptdata.cpp: leased File payload.
                case 1816258268u: case 983359664u: case 1110676961u: case 1034587168u:
                // XmlaudioCompiler.cpp / Xmlaudio.cpp: one private native Builder,
                // const field/enum tables and no process-wide hash registration.
                case 3906227957u: case 299416263u: case 1443425905u:
                case 587347702u: case 1447728787u: case 1826140078u: case 3370391592u:
                    return true;
                default: return false;
            }
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

        static bool PrepareAudio(InstanceDeclaration original, BinaryAssetBuilder.AudioCompiler.Plugin plugin,
            BinaryWriter writer, List<FileStream> leases)
        {
            // FileShare.Read excludes both writes and delete/rename while the
            // same pathname is reopened by the current managed Audio backend.
            // Certification is Windows-only and never assumes Unix semantics.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return false;
            uint type = original.Handle.TypeId;
            XmlAttributeCollection attrs = original.Node.Attributes;
            if (type == AudioCatalog.AudioFileTypeId)
            {
                if (!plugin.UsesRecoveredAudioFileCompiler) return false;
                XmlAttribute compression = attrs["PCCompression"];
                // EALAYER3 emits a process-once diagnostic. Do not cache until
                // the diagnostic result can be replayed with the same contract.
                if (compression != null && compression.Value != "NONE" && compression.Value != "XAS") return false;
                return LeaseInput(attrs["File"], writer, leases);
            }
            if (type == AudioCatalog.Mp3PassthroughTypeId || type == AudioCatalog.PathMusicMapTypeId)
                return LeaseInput(attrs["File"], writer, leases);
            if (type == AudioCatalog.PathMusicTrackTypeId || type == AudioCatalog.PathMusicEventTypeId)
            {
                string field = type == AudioCatalog.PathMusicTrackTypeId ? "PathfinderTrackHeader" : "PathfinderEventHeader";
                if (type == AudioCatalog.PathMusicTrackTypeId && !LeaseInput(attrs["File"], writer, leases)) return false;
                if (!LeaseInput(attrs[field], writer, leases)) return false;
                // Missing handles emit warnings. Keep those calls on their
                // compatibility path rather than silently eliding diagnostics.
                bool opened;
                int handle = PathMusicHeader.Find(attrs[field].Value,
                    type == AudioCatalog.PathMusicTrackTypeId ? "PATH_TRACK_" : "PATH_EVENT_",
                    attrs["id"].Value, out opened);
                return opened && handle != 0;
            }
            return false;
        }

        static bool LeaseInput(XmlAttribute attribute, BinaryWriter writer, List<FileStream> leases)
        {
            if (attribute == null || string.IsNullOrEmpty(attribute.Value)) return false;
            return LeaseInputPath(attribute.Value, writer, leases);
        }

        static bool LeaseInputPath(string value, BinaryWriter writer, List<FileStream> leases)
        {
            if (string.IsNullOrEmpty(value)) return false;
            string path = Path.GetFullPath(value);
            for (var parent = new FileInfo(path).Directory; parent != null; parent = parent.Parent)
                if (parent.Exists && (parent.Attributes & FileAttributes.ReparsePoint) != 0) return false;
            if (!File.Exists(path) || (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) return false;
            FileStream input;
            try { input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
            leases.Add(input);
            writer.Write(path); writer.Write(input.Length);
            using (var hash = SHA256.Create()) writer.Write(hash.ComputeHash(input));
            input.Position = 0;
            return true;
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

        static void WriteReferences(BinaryWriter writer, ICollection<InstanceHandle> handles)
        {
            writer.Write(handles.Count);
            foreach (InstanceHandle handle in handles) WriteHandle(writer, handle);
        }

        static void WriteHandle(BinaryWriter writer, InstanceHandle handle)
        {
            writer.Write(handle.TypeId); writer.Write(handle.TypeHash);
            writer.Write(handle.InstanceId); writer.Write(handle.InstanceHash);
            writer.Write(handle.TypeName); writer.Write(handle.InstanceName);
        }

        static void WriteSchema(BinaryWriter writer, XmlNode node)
        {
            // Schema objects are immutable front-end metadata; the backend
            // never sees them, but changes to their effective identities are
            // conservatively represented in the compatibility identity.
            IXmlSchemaInfo info = node.SchemaInfo;
            writer.Write(info != null);
            if (info != null)
            {
                writer.Write((int)info.Validity); writer.Write(info.IsDefault); writer.Write(info.IsNil);
                WriteName(writer, info.SchemaType == null ? null : info.SchemaType.QualifiedName);
                WriteName(writer, info.MemberType == null ? null : info.MemberType.QualifiedName);
                WriteName(writer, info.SchemaElement == null ? null : info.SchemaElement.QualifiedName);
                WriteName(writer, info.SchemaAttribute == null ? null : info.SchemaAttribute.QualifiedName);
            }
            writer.Write(node.Attributes == null ? 0 : node.Attributes.Count);
            if (node.Attributes != null) foreach (XmlAttribute attribute in node.Attributes) WriteSchema(writer, attribute);
            writer.Write(node.ChildNodes.Count);
            foreach (XmlNode child in node.ChildNodes) WriteSchema(writer, child);
        }

        static void WriteName(BinaryWriter writer, XmlQualifiedName name)
        {
            writer.Write(name != null);
            if (name != null) { writer.Write(name.Namespace); writer.Write(name.Name); }
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
