using System;
using System.Globalization;
using System.IO;
using BinaryAssetBuilder.Core;

namespace Recovered.XmlPriority
{
    // Register only FXList and FXParticleSystemTemplate. Every payload is generated
    // by the recovered bridge; this assembly does not load the original compiler.
    public sealed class Plugin : IAssetBuilderPlugin
    {
        private TargetPlatform platform;
        private static readonly object TraceGate = new object();

        public void Initialize(object configObject, TargetPlatform target) { platform = target; }
        public void ReInitialize(object configObject, TargetPlatform target) { Initialize(configObject, target); }
        public uint GetAllTypesHash() { return 1424942948u; }
        public uint GetVersionNumber()
        {
            return platform == TargetPlatform.PlayStation3 ? 7u :
                platform == TargetPlatform.Xbox360 ? 6u : 4u;
        }
        public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            ExtendedTypeInformation info = new ExtendedTypeInformation();
            info.TypeId = typeId;
            // Original XmlCompiler leaves Tokenized and UseBuildCache false.
            info.HasCustomData = false;
            info.Tokenized = false;
            info.UseBuildCache = false;
            switch (typeId)
            {
                case 2254974584u:
                    info.TypeName = "FXList";
                    info.TypeHash = 2601721382u;
                    break;
                case 686292351u:
                    info.TypeName = "FXParticleSystemTemplate";
                    info.TypeHash = 1465656250u;
                    break;
                default:
                    throw new NotSupportedException("XmlPriority supports only FXList and FXParticleSystemTemplate.");
            }
            info.ProcessingHash = info.TypeHash ^ GetVersionNumber();
            return info;
        }
        private void WriteFailedInput(InstanceDeclaration instance, uint typeId, Exception error)
        {
            string trace = Environment.GetEnvironmentVariable("RA3_RECOVERED_XML_PRIORITY_TRACE");
            if (String.IsNullOrEmpty(trace)) return;
            // Diagnostics must preserve the original compiler exception. Failed inputs
            // are separate from successful calls.tsv and never enter replay counts.
            try
            {
                lock (TraceGate)
                {
                    Directory.CreateDirectory(trace);
                    string stem = "failed-" + Guid.NewGuid().ToString("N");
                    string path = Path.Combine(trace, stem);
                    File.WriteAllText(path + ".xml", instance.Node.OuterXml);
                    File.WriteAllText(path + ".error.txt", "TypeId=" +
                        typeId.ToString(CultureInfo.InvariantCulture) + " platform=" +
                        ((int)platform).ToString(CultureInfo.InvariantCulture) + Environment.NewLine + error);
                }
            }
            catch { /* Preserve the original failure if diagnostics cannot be written. */ }
        }
        public AssetBuffer ProcessInstance(InstanceDeclaration instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            uint typeId = instance.Handle.TypeId;
            string typeName;
            AssetBuffer result;
            try
            {
                switch (typeId)
                {
                    case 2254974584u:
                        typeName = "FXList";
                        result = global::Recovered.FXListCompiler.ProcessInstance(instance, platform);
                        break;
                    case 686292351u:
                        typeName = "FXParticleSystemTemplate";
                        result = global::Recovered.FXParticleSystemTemplateCompiler.ProcessInstance(instance, platform);
                        break;
                    default:
                        throw new NotSupportedException("XmlPriority supports only FXList and FXParticleSystemTemplate.");
                }
            }
            catch (Exception error)
            {
                WriteFailedInput(instance, typeId, error);
                throw;
            }
            string trace = Environment.GetEnvironmentVariable("RA3_RECOVERED_XML_PRIORITY_TRACE");
            if (!String.IsNullOrEmpty(trace))
            {
                lock (TraceGate)
                {
                    Directory.CreateDirectory(trace);
                    string stem = Guid.NewGuid().ToString("N");
                    string path = Path.Combine(trace, stem);
                    File.WriteAllText(path + ".xml", instance.Node.OuterXml);
                    File.WriteAllBytes(path + ".bin", result.InstanceData);
                    File.WriteAllBytes(path + ".relo", result.RelocationData);
                    File.WriteAllBytes(path + ".imp", result.ImportsData);
                    System.Xml.XmlAttribute id = instance.Node.Attributes["id"];
                    File.AppendAllText(Path.Combine(trace, "calls.tsv"), typeName + "\t" +
                        (id == null ? "" : id.Value) + "\t" + stem + "\t" +
                        ((int)platform).ToString(CultureInfo.InvariantCulture) + Environment.NewLine);
                }
            }
            return result;
        }
    }
}

