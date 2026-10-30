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
    // Certified backends only read the finalized XML, namespace bindings and
    // type identity. Core keeps these alive through ordered commit/join; only
    // CDATA writers need a separate declaration for their output pathname.
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
            if (inputLeases != null)
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
            // Bound the bridge's native input, scratch and output allocation.
            // Sharing the DOM does not eliminate that per-call working set.
            if (preparationEstimate > preparationBudget) return Reject("preparation-budget-exceeded", out reason);

            var leases = family == "audio" || (family == "xml" && IsApt(original.Handle.TypeId)) ? new List<FileStream>() : null;
            try
            {
                // Only the certified backend's actual reopened files require
                // leases. No action identity, serialization or file digest is built.
                if (family == "xml" && !PrepareXmlReads(original, leases, out reason)) return false;
                if (family == "audio" && !PrepareAudio(original, leases, out reason)) return false;
                long retained = preparationEstimate;
                if (family == "w3x") retained = W3xBudget(original.Node, retained);
                else if (family == "xml") retained = ExternalReadBudget(leases, retained, 4L);
                else if (family == "audio") retained = AudioBudget(original, leases, retained);
                if (retained > preparationBudget) return Reject("preparation-budget-exceeded", out reason);
                // ProcessExpressions/ProcessOverrides/Validate finish before
                // ProcessInstances. Pending documents cannot be reopened or made
                // cacheable until their workers join; repeated assets flush before
                // reassignment. No certified backend mutates XML, namespace bindings,
                // handles or reference metadata. Keep that original input directly,
                // including unspecified schema defaults and inherited XML context.
                var input = original.HasCustomData ? original.CreateBackendView() : original;
                prepared = new PreparedBackendWork(family, plugin, input, leases, retained);
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
                    XmlAttribute compression = original.Node.Attributes["PCCompression"];
                    if (compression != null && compression.Value != "NONE" && compression.Value != "XAS")
                        return Reject("audio-compression-diagnostic", out reason);
                }
                else if (type != AudioCatalog.Mp3PassthroughTypeId)
                    return Reject("uncertified-audio-type", out reason);
            }
            else if (implementation == typeof(Recovered.GameObject.Plugin))
            {
                family = "gameobject";
                if (original.Handle.TypeId != 2486173485u) return Reject("uncertified-tokenizer-type", out reason);
            }
            else if (implementation == typeof(Recovered.Weapon.Plugin))
            {
                family = "weapon";
                if (original.Handle.TypeId != 2496977262u) return Reject("uncertified-tokenizer-type", out reason);
            }
            else if (implementation == typeof(Recovered.Compiler.Plugin))
            {
                family = "armor";
                if (original.Handle.TypeId != 980180622u) return Reject("uncertified-tokenizer-type", out reason);
            }
            else return Reject("uncertified-plugin", out reason);

            return true;
        }

        public static long EstimatePreparationBytes(InstanceDeclaration original)
        {
            if (original == null || original.Node == null) return 0;
            // The bridges still marshal strings, maps, vectors and output buffers.
            // Tokenized modules additionally emit data and procedure trees.
            uint type = original.Handle.TypeId;
            long multiplier = type == 2486173485u || type == 2496977262u || type == 980180622u ? 4L : 1L;
            try { return checked(4096L + EstimateNode(original.Node) * multiplier); }
            catch (OverflowException) { return long.MaxValue; }
        }

        static long EstimateNode(XmlNode node)
        {
            string value = node.Value;
            long characters = (long)node.Name.Length + node.NamespaceURI.Length + (value == null ? 0L : value.Length);
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
            // its backend reads the finalized node/namespace bindings. Native
            // scratch and outputs are per-call; the only lazy native table is
            // const and built with C++17's thread-safe static initialization.
            // Certified XML routes use a per-call bridge tree and Relo::Tracker;
            // XmlPriority's optional trace writes already hold TraceGate. Audio
            // uses a stateless exact encoder, readonly tables and private CDATA.
            // Tokenizers also use per-call native inputs/trackers/buffers and
            // serialize only their optional shared trace index. Other routes
            // remain owner barriers, including Audio's Tracer paths.
            return plugin.ProcessInstance(declaration);
        }

        public void Dispose()
        {
            if (disposed) return;
            if (inputLeases != null) foreach (FileStream lease in inputLeases) lease.Dispose();
            disposed = true;
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
            if (file == null) return Reject("external-input-path-missing", out reason);
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
            if (leases == null) return minimum;
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

        static long AudioBudget(InstanceDeclaration original, List<FileStream> leases, long minimum)
        {
            if (original.Handle.TypeId != AudioCatalog.AudioFileTypeId)
                return ExternalReadBudget(leases, minimum, 4L);
            long multiplier = 64L;
            XmlAttribute requested = original.Node.Attributes["PCSampleRate"];
            if (requested != null)
            {
                // The existing source-byte allowance covers importer/encoder
                // storage. Rate conversion also retains expanded PCM and FFT
                // scratch. Read only chunk metadata through the held lease;
                // never decode or copy the sample payload during preparation.
                int target = PcmWaveAudioCompiler.ParseCInt(requested.Value);
                try
                {
                    int source = AudioSourceRate(leases[0]);
                    if (source <= 0) return long.MaxValue;
                    if (target != source && target >= 400 && target <= 96000)
                    {
                        multiplier = checked(multiplier * Math.Max(1L, ((long)target + source - 1L) / source));
                        minimum = checked(minimum + 1024L * 1024L);
                    }
                }
                catch (IOException) { return long.MaxValue; }
                catch (OverflowException) { return long.MaxValue; }
            }
            return ExternalReadBudget(leases, minimum, multiplier);
        }

        static uint AudioWord(BinaryReader reader, bool bigEndian)
        {
            uint value = reader.ReadUInt32();
            return bigEndian ? (value >> 24) | ((value >> 8) & 0xff00u) |
                ((value << 8) & 0xff0000u) | (value << 24) : value;
        }

        static int AudioSourceRate(FileStream input)
        {
            input.Position = 0;
            using (var reader = new BinaryReader(input, System.Text.Encoding.ASCII, true))
            {
                uint magic = reader.ReadUInt32();
                bool aiff = magic == 0x4d524f46u; // FORM
                long end = 8L + AudioWord(reader, aiff);
                uint kind = reader.ReadUInt32();
                bool wave = magic == 0x46464952u && kind == 0x45564157u; // RIFF/WAVE
                if ((!wave && !(aiff && (kind == 0x46464941u || kind == 0x43464941u))) ||
                    end < 12 || end > input.Length) return 0;
                int rate = 0;
                // Match the importer's last fmt/COMM selection and padded
                // chunk traversal, including non-canonical chunk order.
                for (long at = 12; at + 8 <= end; )
                {
                    input.Position = at;
                    uint tag = reader.ReadUInt32(), size = AudioWord(reader, aiff);
                    long next = at + 8 + size;
                    if (next > end) return 0;
                    if (wave && tag == 0x20746d66u && size >= 16) // fmt
                    {
                        input.Position = at + 12;
                        uint value = AudioWord(reader, false);
                        rate = value <= 0x3ffffu ? (int)value : 0;
                    }
                    else if (aiff && tag == 0x4d4d4f43u && size >= 18) // COMM
                    {
                        input.Position = at + 16;
                        int exponent = (reader.ReadByte() << 8) | reader.ReadByte();
                        int shift = 0x401e - exponent;
                        uint high = AudioWord(reader, true);
                        uint value = shift >= 0 && shift <= 31 ? high >> shift : 0;
                        rate = value <= 0x3ffffu ? (int)value : 0;
                    }
                    at = next + (size & 1u);
                }
                return rate;
            }
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
