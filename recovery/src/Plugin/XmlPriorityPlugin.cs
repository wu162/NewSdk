using System;
using System.Globalization;
using System.IO;
using BinaryAssetBuilder.Core;

namespace Recovered.XmlPriority
{
    // Register the 82 recovered entries. Every payload is generated
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
                case 1213037816u: info.TypeName = "PlayerTechStoreTemplate"; info.TypeHash = 981786901u; break;
                case 1566941920u: info.TypeName = "PurchasePlayerTechButtonTemplate"; info.TypeHash = 2999033148u; break;
                case 3048039928u: info.TypeName = "ButtonSingleStateData"; info.TypeHash = 3780991768u; break;
                case 134745024u: info.TypeName = "UICommandButton"; info.TypeHash = 2853539982u; break;
                case 1371131236u: info.TypeName = "FontDefaultSetting"; info.TypeHash = 197748432u; break;
                case 1318336226u: info.TypeName = "FontTranslation"; info.TypeHash = 1815171461u; break;
                case 91719417u: info.TypeName = "FontSubstitution"; info.TypeHash = 2789100025u; break;
                case 2430161325u: info.TypeName = "Weather"; info.TypeHash = 3795761625u; break;
                case 1479531562u: info.TypeName = "WaterTransparency"; info.TypeHash = 525928939u; break;
                case 2728605516u: info.TypeName = "WaterSetting"; info.TypeHash = 469423908u; break;
                case 680780553u: info.TypeName = "Environment"; info.TypeHash = 1276225790u; break;
                case 3169690211u: info.TypeName = "LightingSettings"; info.TypeHash = 616355229u; break;
                case 2995571269u: info.TypeName = "WaterTextureList"; info.TypeHash = 1905565848u; break;
                case 1469498551u: info.TypeName = "TerrainParticleMapping"; info.TypeHash = 2736882942u; break;
                case 2368878637u: info.TypeName = "BounceKickTerrainMapping"; info.TypeHash = 2109862985u; break;
                case 4157475773u: info.TypeName = "ShadowMap"; info.TypeHash = 903854585u; break;
                case 2996766068u: info.TypeName = "OrdersManagerSettings"; info.TypeHash = 2164060776u; break;
                case 1184276792u: info.TypeName = "TerrainVisualSettings"; info.TypeHash = 1336753171u; break;
                case 3898802054u: info.TypeName = "CameraSettings"; info.TypeHash = 4258032094u; break;
                case 3827723072u: info.TypeName = "AIData"; info.TypeHash = 1092854186u; break;
                case 2964891936u: info.TypeName = "UnitAISettings"; info.TypeHash = 4050675268u; break;
                case 4063352333u: info.TypeName = "PlayerTemplate"; info.TypeHash = 891373896u; break;
                case 191082224u: info.TypeName = "MultiplayerSettings"; info.TypeHash = 2620092030u; break;
                case 1644972043u: info.TypeName = "AIBuildStateDefinition"; info.TypeHash = 3400281710u; break;
                case 2010562192u: info.TypeName = "AIStrategicStateName"; info.TypeHash = 76343204u; break;
                case 2384988189u: info.TypeName = "MultiplayerColor"; info.TypeHash = 320610464u; break;
                case 4086175864u: info.TypeName = "AIPersonalityToAchievementEventMapping"; info.TypeHash = 2395097450u; break;
                case 838130914u: info.TypeName = "FormationAssistantUnitDefinition"; info.TypeHash = 1672393125u; break;
                case 2912385462u: info.TypeName = "FormationAssistantTemplate"; info.TypeHash = 156475083u; break;
                case 3374519264u: info.TypeName = "FormationSelectionTemplate"; info.TypeHash = 87133761u; break;
                case 1963196508u: info.TypeName = "FormationAssistant"; info.TypeHash = 1061339997u; break;
                case 3249689110u: info.TypeName = "GameStatsModeData"; info.TypeHash = 4003247819u; break;
                case 402082255u: info.TypeName = "SkirmishSettings"; info.TypeHash = 3814569307u; break;
                case 2905958645u: info.TypeName = "DamageFX"; info.TypeHash = 187609259u; break;
                case 75989905u: info.TypeName = "DamageFXSettings"; info.TypeHash = 2052546860u; break;
                case 417176643u: info.TypeName = "GeneralHotKeySlots"; info.TypeHash = 655411104u; break;
                case 3741098742u: info.TypeName = "DefaultHotKeys"; info.TypeHash = 1229544125u; break;
                case 3359870461u: info.TypeName = "DebugHotKeys"; info.TypeHash = 73026796u; break;
                case 1350608344u: info.TypeName = "MappableKey"; info.TypeHash = 2226564801u; break;
                case 3906227957u: info.TypeName = "AudioEventLimitGroup"; info.TypeHash = 2503036394u; break;
                case 299416263u: info.TypeName = "AudioLOD"; info.TypeHash = 21805907u; break;
                case 1443425905u: info.TypeName = "AudioSettings"; info.TypeHash = 3631141684u; break;
                case 587347702u: info.TypeName = "Video"; info.TypeHash = 2217996198u; break;
                case 1447728787u: info.TypeName = "VideoEventList"; info.TypeHash = 2530876419u; break;
                case 1826140078u: info.TypeName = "MicrophoneSettings"; info.TypeHash = 1624658388u; break;
                case 3370391592u: info.TypeName = "BootupDisplaySequence"; info.TypeHash = 3238903012u; break;
                case 2800139175u: info.TypeName = "HotKeySlot"; info.TypeHash = 2481075826u; break;
                case 3784442503u: info.TypeName = "MouseCursor"; info.TypeHash = 334318473u; break;
                case 611271731u: info.TypeName = "BasicMouseCursors"; info.TypeHash = 1517904520u; break;
                case 611126373u: info.TypeName = "UIMouseDragSelectionFeedback"; info.TypeHash = 549522065u; break;
                case 1559937240u: info.TypeName = "UIFloatingTextDisplay"; info.TypeHash = 2264980354u; break;
                case 1697313181u: info.TypeName = "UIMessageDisplay"; info.TypeHash = 3604410282u; break;
                case 1043989897u: info.TypeName = "UIMilitaryCaptionDisplay"; info.TypeHash = 779899080u; break;
                case 165611890u: info.TypeName = "UISubtitleDisplay"; info.TypeHash = 1129081393u; break;
                case 951036436u: info.TypeName = "UITimerDisplay"; info.TypeHash = 2082140658u; break;
                case 2901356964u: info.TypeName = "DynamicGameLOD"; info.TypeHash = 1764459959u; break;
                case 354222972u: info.TypeName = "GameLODPreset"; info.TypeHash = 3645429389u; break;
                case 531798225u: info.TypeName = "StaticGameLOD"; info.TypeHash = 1251212385u; break;
                case 4243804314u: info.TypeName = "UICoopTinting"; info.TypeHash = 89086670u; break;
                case 544776545u: info.TypeName = "RadiusCursor"; info.TypeHash = 2695845168u; break;
                case 1302035577u: info.TypeName = "AnimationSoundSettings"; info.TypeHash = 3170370516u; break;
                case 2070603733u: info.TypeName = "MiscAudio"; info.TypeHash = 1484343325u; break;
                case 534008255u: info.TypeName = "LargeGroupAudioMap"; info.TypeHash = 1045439046u; break;
                default: throw new NotSupportedException("XmlPriority supports only the 82 recovered entries.");
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
                case 1213037816u:
                    typeName = "PlayerTechStoreTemplate";
                    result = global::Recovered.PlayerTechStoreTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 1566941920u:
                    typeName = "PurchasePlayerTechButtonTemplate";
                    result = global::Recovered.PurchasePlayerTechButtonTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 3048039928u:
                    typeName = "ButtonSingleStateData";
                    result = global::Recovered.ButtonSingleStateDataCompiler.ProcessInstance(instance, platform);
                    break;
                case 134745024u:
                    typeName = "UICommandButton";
                    result = global::Recovered.UICommandButtonCompiler.ProcessInstance(instance, platform);
                    break;
                case 1371131236u:
                    typeName = "FontDefaultSetting";
                    result = global::Recovered.FontDefaultSettingCompiler.ProcessInstance(instance, platform);
                    break;
                case 1318336226u:
                    typeName = "FontTranslation";
                    result = global::Recovered.FontTranslationCompiler.ProcessInstance(instance, platform);
                    break;
                case 91719417u:
                    typeName = "FontSubstitution";
                    result = global::Recovered.FontSubstitutionCompiler.ProcessInstance(instance, platform);
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
                case 1469498551u:
                    typeName = "TerrainParticleMapping";
                    result = global::Recovered.TerrainParticleMappingCompiler.ProcessInstance(instance, platform);
                    break;
                case 2368878637u:
                    typeName = "BounceKickTerrainMapping";
                    result = global::Recovered.BounceKickTerrainMappingCompiler.ProcessInstance(instance, platform);
                    break;
                case 4157475773u:
                    typeName = "ShadowMap";
                    result = global::Recovered.ShadowMapCompiler.ProcessInstance(instance, platform);
                    break;
                case 2996766068u:
                    typeName = "OrdersManagerSettings";
                    result = global::Recovered.OrdersManagerSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 1184276792u:
                    typeName = "TerrainVisualSettings";
                    result = global::Recovered.TerrainVisualSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 3898802054u:
                    typeName = "CameraSettings";
                    result = global::Recovered.CameraSettingsCompiler.ProcessInstance(instance, platform);
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
                case 2384988189u:
                    typeName = "MultiplayerColor";
                    result = global::Recovered.MultiplayerColorCompiler.ProcessInstance(instance, platform);
                    break;
                case 4086175864u:
                    typeName = "AIPersonalityToAchievementEventMapping";
                    result = global::Recovered.AIPersonalityToAchievementEventMappingCompiler.ProcessInstance(instance, platform);
                    break;
                case 838130914u:
                    typeName = "FormationAssistantUnitDefinition";
                    result = global::Recovered.FormationAssistantUnitDefinitionCompiler.ProcessInstance(instance, platform);
                    break;
                case 2912385462u:
                    typeName = "FormationAssistantTemplate";
                    result = global::Recovered.FormationAssistantTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 3374519264u:
                    typeName = "FormationSelectionTemplate";
                    result = global::Recovered.FormationSelectionTemplateCompiler.ProcessInstance(instance, platform);
                    break;
                case 1963196508u:
                    typeName = "FormationAssistant";
                    result = global::Recovered.FormationAssistantCompiler.ProcessInstance(instance, platform);
                    break;
                case 3249689110u:
                    typeName = "GameStatsModeData";
                    result = global::Recovered.GameStatsModeDataCompiler.ProcessInstance(instance, platform);
                    break;
                case 402082255u:
                    typeName = "SkirmishSettings";
                    result = global::Recovered.SkirmishSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 2905958645u:
                    typeName = "DamageFX";
                    result = global::Recovered.DamageFXCompiler.ProcessInstance(instance, platform);
                    break;
                case 75989905u:
                    typeName = "DamageFXSettings";
                    result = global::Recovered.DamageFXSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 417176643u:
                    typeName = "GeneralHotKeySlots";
                    result = global::Recovered.GeneralHotKeySlotsCompiler.ProcessInstance(instance, platform);
                    break;
                case 3741098742u:
                    typeName = "DefaultHotKeys";
                    result = global::Recovered.DefaultHotKeysCompiler.ProcessInstance(instance, platform);
                    break;
                case 3359870461u:
                    typeName = "DebugHotKeys";
                    result = global::Recovered.DebugHotKeysCompiler.ProcessInstance(instance, platform);
                    break;
                case 1350608344u:
                    typeName = "MappableKey";
                    result = global::Recovered.MappableKeyCompiler.ProcessInstance(instance, platform);
                    break;
                case 3906227957u:
                    typeName = "AudioEventLimitGroup";
                    result = global::Recovered.AudioEventLimitGroupCompiler.ProcessInstance(instance, platform);
                    break;
                case 299416263u:
                    typeName = "AudioLOD";
                    result = global::Recovered.AudioLODCompiler.ProcessInstance(instance, platform);
                    break;
                case 1443425905u:
                    typeName = "AudioSettings";
                    result = global::Recovered.AudioSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 587347702u:
                    typeName = "Video";
                    result = global::Recovered.VideoCompiler.ProcessInstance(instance, platform);
                    break;
                case 1447728787u:
                    typeName = "VideoEventList";
                    result = global::Recovered.VideoEventListCompiler.ProcessInstance(instance, platform);
                    break;
                case 1826140078u:
                    typeName = "MicrophoneSettings";
                    result = global::Recovered.MicrophoneSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 3370391592u:
                    typeName = "BootupDisplaySequence";
                    result = global::Recovered.BootupDisplaySequenceCompiler.ProcessInstance(instance, platform);
                    break;
                case 2800139175u:
                    typeName = "HotKeySlot";
                    result = global::Recovered.HotKeySlotCompiler.ProcessInstance(instance, platform);
                    break;
                case 3784442503u:
                    typeName = "MouseCursor";
                    result = global::Recovered.MouseCursorCompiler.ProcessInstance(instance, platform);
                    break;
                case 611271731u:
                    typeName = "BasicMouseCursors";
                    result = global::Recovered.BasicMouseCursorsCompiler.ProcessInstance(instance, platform);
                    break;
                case 611126373u:
                    typeName = "UIMouseDragSelectionFeedback";
                    result = global::Recovered.UIMouseDragSelectionFeedbackCompiler.ProcessInstance(instance, platform);
                    break;
                case 1559937240u:
                    typeName = "UIFloatingTextDisplay";
                    result = global::Recovered.UIFloatingTextDisplayCompiler.ProcessInstance(instance, platform);
                    break;
                case 1697313181u:
                    typeName = "UIMessageDisplay";
                    result = global::Recovered.UIMessageDisplayCompiler.ProcessInstance(instance, platform);
                    break;
                case 1043989897u:
                    typeName = "UIMilitaryCaptionDisplay";
                    result = global::Recovered.UIMilitaryCaptionDisplayCompiler.ProcessInstance(instance, platform);
                    break;
                case 165611890u:
                    typeName = "UISubtitleDisplay";
                    result = global::Recovered.UISubtitleDisplayCompiler.ProcessInstance(instance, platform);
                    break;
                case 951036436u:
                    typeName = "UITimerDisplay";
                    result = global::Recovered.UITimerDisplayCompiler.ProcessInstance(instance, platform);
                    break;
                case 2901356964u:
                    typeName = "DynamicGameLOD";
                    result = global::Recovered.DynamicGameLODCompiler.ProcessInstance(instance, platform);
                    break;
                case 354222972u:
                    typeName = "GameLODPreset";
                    result = global::Recovered.GameLODPresetCompiler.ProcessInstance(instance, platform);
                    break;
                case 531798225u:
                    typeName = "StaticGameLOD";
                    result = global::Recovered.StaticGameLODCompiler.ProcessInstance(instance, platform);
                    break;
                case 4243804314u:
                    typeName = "UICoopTinting";
                    result = global::Recovered.UICoopTintingCompiler.ProcessInstance(instance, platform);
                    break;
                case 544776545u:
                    typeName = "RadiusCursor";
                    result = global::Recovered.RadiusCursorCompiler.ProcessInstance(instance, platform);
                    break;
                case 1302035577u:
                    typeName = "AnimationSoundSettings";
                    result = global::Recovered.AnimationSoundSettingsCompiler.ProcessInstance(instance, platform);
                    break;
                case 2070603733u:
                    typeName = "MiscAudio";
                    result = global::Recovered.MiscAudioCompiler.ProcessInstance(instance, platform);
                    break;
                case 534008255u:
                    typeName = "LargeGroupAudioMap";
                    result = global::Recovered.LargeGroupAudioMapCompiler.ProcessInstance(instance, platform);
                    break;
                default: throw new NotSupportedException("XmlPriority supports only the 82 recovered entries.");
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




