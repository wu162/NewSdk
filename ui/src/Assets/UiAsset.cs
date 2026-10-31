using System;
using System.Collections.Generic;
using System.IO;
using BinaryAssetBuilder.Core;
namespace BinaryAssetBuilder.UiCompiler
{
    public sealed class CompiledUiAsset
    {
        public string TypeName { get; }
        public string Name { get; }
        public string Source { get; }
        public uint TypeId { get; }
        public uint TypeHash { get; }
        public uint InstanceId { get; }
        public uint InstanceHash { get; }
        public AssetBuffer Buffer { get; }
        internal CompiledUiAsset(string type, string name, string source, byte[] payload, int aptId = 0)
        {
            if (string.IsNullOrWhiteSpace(name) || name.IndexOf('\0') >= 0) throw new ArgumentException("An asset name is required.", nameof(name));
            TypeName = type; Name = name; Source = source; var metadata = UiCatalog.Find(type); TypeId = metadata.TypeId; TypeHash = metadata.TypeHash; InstanceId = SageHash.Symbol(name.ToLowerInvariant());
            int rootSize = type == "AptGeometryData" ? 16 : 12; var instance = new byte[checked(rootSize + ((payload.Length + 3) & ~3))];
            Word(instance, 4, (uint)rootSize); Word(instance, 8, (uint)payload.Length); if (rootSize == 16) Word(instance, 12, unchecked((uint)aptId)); System.Buffer.BlockCopy(payload, 0, instance, rootSize, payload.Length);
            Buffer = new AssetBuffer { InstanceData = instance, RelocationData = new byte[] { 4, 0, 0, 0, 255, 255, 255, 255 }, ImportsData = Array.Empty<byte>() };
            InstanceHash = SageHash.Data(instance);
        }
        internal static void Word(byte[] bytes, int offset, uint value) { for (int i = 0; i < 4; i++) bytes[offset + i] = (byte)(value >> (8 * i)); }
    }
    internal static class UiCatalog
    {
        internal const uint ProcessingHash = 0x55493802;
        internal static readonly Tuple<string, uint, uint>[] Types = {
            Tuple.Create("AptAptData", 1816258268u, 3795385027u), Tuple.Create("AptConstData", 983359664u, 3876251416u),
            Tuple.Create("AptDatData", 1110676961u, 230038576u), Tuple.Create("AptGeometryData", 1034587168u, 1390234602u),
            Tuple.Create("Texture", SageHash.Symbol("Texture"), 2673866970u)
        };
        internal static ExtendedTypeInformation Find(string name)
        { foreach (var t in Types) if (t.Item1 == name) return new ExtendedTypeInformation { TypeName = name, TypeId = t.Item2, TypeHash = t.Item3, ProcessingHash = ProcessingHash, UseBuildCache = false }; throw new NotSupportedException(name); }
        internal static ExtendedTypeInformation Find(uint id) { foreach (var t in Types) if (t.Item2 == id) return Find(t.Item1); return null; }
        internal static uint AllTypesHash { get { uint hash = ProcessingHash; foreach (var t in Types) hash = SageHash.Combine(hash, BitConverter.GetBytes(t.Item3)); return hash; } }
    }
}
