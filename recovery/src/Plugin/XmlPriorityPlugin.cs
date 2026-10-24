using System;
using System.Globalization;
using System.IO;
using BinaryAssetBuilder.Core;

namespace Recovered.XmlPriority
{
    // Register the thirty-one recovered types. Every payload is generated
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
                case 3319822471u: info.TypeName = "AttributeModifier"; info.TypeHash = 4177657499u; break;
                case 2254974584u: info.TypeName = "FXList"; info.TypeHash = 2601721382u; break;
                case 2178440954u: info.TypeName = "SpecialPowerTemplate"; info.TypeHash = 984303644u; break;
                case 3899542881u: info.TypeName = "ObjectCreationList"; info.TypeHash = 578005457u; break;
                case 3972178387u: info.TypeName = "LocomotorTemplate"; info.TypeHash = 3392338269u; break;
                case 3959844197u: info.TypeName = "LogicCommandSet"; info.TypeHash = 2330506892u; break;
                case 686292351u: info.TypeName = "FXParticleSystemTemplate"; info.TypeHash = 1465656250u; break;
                case 3786401627u: info.TypeName = "UpgradeTemplate"; info.TypeHash = 1334651241u; break;
                case 3604279694u: info.TypeName = "AIPersonalityDefinition"; info.TypeHash = 3409747243u; break;
                case 607123156u: info.TypeName = "AIStrategicStateDefinition"; info.TypeHash = 3594381428u; break;
                case 2423780215u: info.TypeName = "AIMicroManagerData"; info.TypeHash = 1392447443u; break;
                case 2893598307u: info.TypeName = "AIBudgetStateDefinition"; info.TypeHash = 2583631670u; break;
                case 3239935619u: info.TypeName = "AIStateDefinition"; info.TypeHash = 50334453u; break;
                case 926814458u: info.TypeName = "AITargetingHeuristic"; info.TypeHash = 2460454737u; break;
                case 2101756272u: info.TypeName = "LogicCommand"; info.TypeHash = 1451688104u; break;
                case 2131237081u: info.TypeName = "PlayerTech"; info.TypeHash = 903715207u; break;
                case 1802643394u: info.TypeName = "PlayerTechRank"; info.TypeHash = 304552910u; break;
                case 2140592493u: info.TypeName = "PlayerTechUpgradeBinding"; info.TypeHash = 3288487200u; break;
                case 1874610847u: info.TypeName = "ExperienceLevelTemplate"; info.TypeHash = 550595366u; break;
                case 2430161325u: info.TypeName = "Weather"; info.TypeHash = 3795761625u; break;
                case 1479531562u: info.TypeName = "WaterTransparency"; info.TypeHash = 525928939u; break;
                case 2728605516u: info.TypeName = "WaterSetting"; info.TypeHash = 469423908u; break;
                case 680780553u: info.TypeName = "Environment"; info.TypeHash = 1276225790u; break;
                case 3169690211u: info.TypeName = "LightingSettings"; info.TypeHash = 616355229u; break;
                case 2995571269u: info.TypeName = "WaterTextureList"; info.TypeHash = 1905565848u; break;
                case 3827723072u: info.TypeName = "AIData"; info.TypeHash = 1092854186u; break;
                case 2964891936u: info.TypeName = "UnitAISettings"; info.TypeHash = 4050675268u; break;
                case 4063352333u: info.TypeName = "PlayerTemplate"; info.TypeHash = 891373896u; break;
                case 191082224u: info.TypeName = "MultiplayerSettings"; info.TypeHash = 2620092030u; break;
                case 1644972043u: info.TypeName = "AIBuildStateDefinition"; info.TypeHash = 3400281710u; break;
                case 2010562192u: info.TypeName = "AIStrategicStateName"; info.TypeHash = 76343204u; break;
                default: throw new NotSupportedException("XmlPriority supports only the thirty-one recovered types.");
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
                case 3319822471u:
                    typeName = "AttributeModifier";
                    result = global::Recovered.AttributeModifierCompiler.ProcessInstance(instance, platform);
                    break;
                case 2254974584u:
                    typeName = "FXList";
                    result = global::Recovered.FXListCompiler.ProcessInstance(instance, platform);
                    break;
                case 2178440954u:
                    typeName = "SpecialPowerTemplate";
                    result = global::Recovered.SpecialPowerTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 3899542881u:
                    typeName = "ObjectCreationList";
                    result = global::Recovered.ObjectCreationListCompiler.ProcessInstance(instance, platform);
                    break;
                case 3972178387u:
                    typeName = "LocomotorTemplate";
                    result = global::Recovered.LocomotorTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 3959844197u:
                    typeName = "LogicCommandSet";
                    result = global::Recovered.LogicCommandSetCompiler.ProcessInstance(instance, platform);
                    break;
                case 686292351u:
                    typeName = "FXParticleSystemTemplate";
                    result = global::Recovered.FXParticleSystemTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 3786401627u:
                    typeName = "UpgradeTemplate";
                    result = global::Recovered.UpgradeTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 3604279694u:
                    typeName = "AIPersonalityDefinition";
                    result = global::Recovered.XmlaiCompiler.ProcessInstance(instance, platform);
                    break;
                case 607123156u:
                    typeName = "AIStrategicStateDefinition";
                    result = global::Recovered.XmlaiCompiler.ProcessInstance(instance, platform);
                    break;
                case 2423780215u:
                    typeName = "AIMicroManagerData";
                    result = global::Recovered.XmlaiCompiler.ProcessInstance(instance, platform);
                    break;
                case 2893598307u:
                    typeName = "AIBudgetStateDefinition";
                    result = global::Recovered.XmlaiCompiler.ProcessInstance(instance, platform);
                    break;
                case 3239935619u:
                    typeName = "AIStateDefinition";
                    result = global::Recovered.XmlaiCompiler.ProcessInstance(instance, platform);
                    break;
                case 926814458u:
                    typeName = "AITargetingHeuristic";
                    result = global::Recovered.XmlaiCompiler.ProcessInstance(instance, platform);
                    break;
                case 2101756272u:
                    typeName = "LogicCommand";
                    result = global::Recovered.LogicCommandCompiler.ProcessInstance(instance, platform);
                    break;
                case 2131237081u:
                    typeName = "PlayerTech";
                    result = global::Recovered.PlayerTechCompiler.ProcessInstance(instance, platform);
                    break;
                case 1802643394u:
                    typeName = "PlayerTechRank";
                    result = global::Recovered.PlayerTechRankCompiler.ProcessInstance(instance, platform);
                    break;
                case 2140592493u:
                    typeName = "PlayerTechUpgradeBinding";
                    result = global::Recovered.PlayerTechUpgradeBindingCompiler.ProcessInstance(instance, platform);
                    break;
                case 1874610847u:
                    typeName = "ExperienceLevelTemplate";
                    result = global::Recovered.ExperienceLevelTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 2430161325u:
                    typeName = "Weather";
                    result = global::Recovered.WeatherCompiler.ProcessInstance(instance, platform);
                    break;
                case 1479531562u:
                    typeName = "WaterTransparency";
                    result = global::Recovered.WaterTransparencyCompiler.ProcessInstance(instance, platform);
                    break;
                case 2728605516u:
                    typeName = "WaterSetting";
                    result = global::Recovered.WaterSettingCompiler.ProcessInstance(instance, platform);
                    break;
                case 680780553u:
                    typeName = "Environment";
                    result = global::Recovered.EnvironmentCompiler.ProcessInstance(instance, platform);
                    break;
                case 3169690211u:
                    typeName = "LightingSettings";
                    result = global::Recovered.LightingSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 2995571269u:
                    typeName = "WaterTextureList";
                    result = global::Recovered.WaterTextureListCompiler.ProcessInstance(instance, platform);
                    break;
                case 3827723072u:
                    typeName = "AIData";
                    result = global::Recovered.AIDataCompiler.ProcessInstance(instance, platform);
                    break;
                case 2964891936u:
                    typeName = "UnitAISettings";
                    result = global::Recovered.UnitAISettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 4063352333u:
                    typeName = "PlayerTemplate";
                    result = global::Recovered.PlayerTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 191082224u:
                    typeName = "MultiplayerSettings";
                    result = global::Recovered.MultiplayerSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 1644972043u:
                    typeName = "AIBuildStateDefinition";
                    result = global::Recovered.AIBuildStateDefinitionCompiler.ProcessInstance(instance, platform);
                    break;
                case 2010562192u:
                    typeName = "AIStrategicStateName";
                    result = global::Recovered.AIStrategicStateNameCompiler.ProcessInstance(instance, platform);
                    break;
                default: throw new NotSupportedException("XmlPriority supports only the thirty-one recovered types.");
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



