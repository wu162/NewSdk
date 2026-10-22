using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder.Core;
using Recovered;

class ArmorOracle
{
    const string AssetNamespace = "uri:ea.com:eala:asset";
    static int comparisons;
    static InstanceDeclaration Declaration(XmlDocument xml, XmlNamespaceManager namespaces)
    {
        // Supply the same Node and namespace manager that BAB supplies after
        // schema processing, without initializing unrelated build/cache services.
        var document = new AssetDeclarationDocument();
        Type stateType = typeof(AssetDeclarationDocument).GetNestedType("CurrentState", BindingFlags.NonPublic);
        object state = FormatterServices.GetUninitializedObject(stateType);
        stateType.GetField("NamespaceManager").SetValue(state, namespaces);
        typeof(AssetDeclarationDocument).GetField("_Current", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(document, state);
        var declaration = new InstanceDeclaration(document);
        declaration.XmlNode = xml.DocumentElement;
        return declaration;
    }
    static void Equal(string name, byte[] original, byte[] recovered)
    {
        if (original == null || recovered == null || original.Length != recovered.Length)
            throw new Exception(name + " lengths: original=" + (original == null ? -1 : original.Length) + " recovered=" + (recovered == null ? -1 : recovered.Length));
        for (int i = 0; i < original.Length; ++i)
            if (original[i] != recovered[i]) throw new Exception(name + " byte " + i + ": original=" + original[i] + " recovered=" + recovered[i]);
        ++comparisons;
    }
    static uint Word(byte[] bytes, int offset, bool big)
    {
        if (!big) return BitConverter.ToUInt32(bytes, offset);
        return ((uint)bytes[offset] << 24) | ((uint)bytes[offset+1] << 16) | ((uint)bytes[offset+2] << 8) | bytes[offset+3];
    }
    static void Test(IAssetBuilderPlugin plugin, string label, string xmlText, TargetPlatform platform, int count)
    {
        plugin.Initialize(null, platform);
        var xml = new XmlDocument(); xml.LoadXml(xmlText);
        var namespaces = new XmlNamespaceManager(xml.NameTable); namespaces.AddNamespace("ea", AssetNamespace);
        var declaration = Declaration(xml, namespaces);
        AssetBuffer original = plugin.ProcessInstance(declaration);
        AssetBuffer recovered = ArmorCompiler.ProcessInstance(declaration, platform);
        Equal(label + "/instance", original.InstanceData, recovered.InstanceData);
        Equal(label + "/relocations", original.RelocationData, recovered.RelocationData);
        Equal(label + "/imports", original.ImportsData, recovered.ImportsData);
        // Independently assert the layout, rather than only comparing two hosts.
        int tokenSize = 96 + count * 48;
        bool big = platform != TargetPlatform.Win32;
        if (recovered.InstanceData.Length != 128 + 56 * count ||
            Word(recovered.InstanceData, tokenSize + 24, big) != count ||
            Word(recovered.InstanceData, 8, big) != tokenSize + 4 ||
            Word(recovered.InstanceData, 68, big) != 84 ||
            Word(recovered.InstanceData, 92, big) != tokenSize + 24 ||
            recovered.RelocationData.Length != (count == 0 ? 0 : 8) || recovered.ImportsData.Length != 0)
            throw new Exception(label + " independent layout assertion");
        if (count != 0 && (Word(recovered.InstanceData, tokenSize + 28, big) != tokenSize + 32 ||
            Word(recovered.RelocationData, 0, big) != tokenSize + 28 || Word(recovered.RelocationData, 4, big) != uint.MaxValue))
            throw new Exception(label + " independent relocation assertion");
        using (var sha = SHA256.Create())
            Console.WriteLine("PASS {0} platform={1} entries={2} bytes={3} sha256={4}", label, (int)platform, count, recovered.InstanceData.Length, BitConverter.ToString(sha.ComputeHash(recovered.InstanceData)).Replace("-", ""));
    }
    static string Armor(string attrs, string children)
    {
        return "<ArmorTemplate xmlns='" + AssetNamespace + "' id='Oracle' " + attrs + ">" + children + "</ArmorTemplate>";
    }
    static int Main(string[] args)
    {
        try {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            var assembly = Assembly.LoadFrom(args[0]);
            var plugin = (IAssetBuilderPlugin)Activator.CreateInstance(assembly.GetType("BinaryAssetBuilder.Tokenizer.Plugin"));
            string[] damages = { "FORCE", "CRUSH", "SLASH", "PIERCE", "FLAME", "HEALING", "UNRESISTABLE", "WATER", "PENALTY", "FALLING", "BLAST", "REFLECTED", "PASSENGER", "MAGIC", "UNDEFINED", "LOGICAL_FIRE", "POISON", "LASER", "PLASMA", "FIRE", "ROCKET", "GUN", "CANNON", "GRENADE", "SNIPER", "MINE", "TIBERIUM", "STORM_SHIELD", "MELEE", "AUTO_CANNON", "IMPACT", "EXPLOSIVE", "CONCUSSIVE", "FLAK", "PRISM", "TESLA", "KI", "RADIATION", "ALL" };
            string[] numbers = { "0", "-0", "-25.5%", "33.3333333333", "16777217", "2d1", "2D-1", "1e-7", "", "garbage", " 25.75junk", "25,5", "100%", "1e30", "1e-30" };
            foreach (TargetPlatform platform in new[] { TargetPlatform.Win32, TargetPlatform.Xbox360, TargetPlatform.PlayStation3, (TargetPlatform)99 }) {
                Test(plugin, "defaults", Armor("", ""), platform, 0);
                for (int i = 0; i < numbers.Length; ++i)
                    Test(plugin, "percent-" + i, Armor("Default='" + numbers[i] + "' DamageScalar='" + numbers[i] + "' SideDamageScalar='" + numbers[i] + "' RearDamageScalar='" + numbers[i] + "' FlankedPenalty='" + numbers[i] + "'", "<Armor Damage='CANNON' Percent='" + numbers[i] + "'/>"), platform, 1);
                for (int i = 0; i < damages.Length; ++i) {
                    string child = "<Armor Damage='" + damages[i] + "' Percent='67.125%'/>";
                    Test(plugin, "enum-" + damages[i], Armor("", child), platform, 1);
                }
                foreach (string damage in new[] { "cannon", " CANNON", "CANNON ", "", "UNKNOWN", "炮弹" })
                    Test(plugin, "unknown-" + damage, Armor("", "<Armor Damage='" + damage + "' Percent='15'/ >".Replace("/ >", "/>")), platform, 1);
                Test(plugin, "missing-and-duplicate", Armor("", "<Armor/><Armor Damage='FORCE'/><Armor Percent='10'/><Armor Damage='FORCE' Percent='200'/>"), platform, 4);
                Test(plugin, "namespace-filter", Armor("default='1'", "<Armor xmlns='' Percent='1'/><Armor xmlns='other' Percent='2'/><armor Percent='3'/><Other/><e:Armor xmlns:e='" + AssetNamespace + "' Damage='GUN' Percent='42'/><Armor><Percent>5</Percent></Armor>"), platform, 2);
                foreach (int count in new[] { 2, 27, 28, 29, 64, 257 }) {
                    string children = "";
                    for (int i = 0; i < count; ++i) children += "<Armor Damage='" + damages[i % damages.Length] + "' Percent='" + (i * 7 - 40).ToString(CultureInfo.InvariantCulture) + "'/>";
                    Test(plugin, "list-" + count, Armor("", children), platform, count);
                }
            }
            Console.WriteLine("PASS ArmorTemplate: " + comparisons + " byte-array comparisons (3 arrays per fixture); original ProcessInstance invoked for every fixture.");
            return 0;
        } catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
}
