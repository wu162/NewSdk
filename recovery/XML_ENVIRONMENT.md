# XmlCompiler environment group

Implementation checkpoint, Windows/SDK verification pending. Branch recovery/xmlcompiler-environment starts at 7610d1cfa3e1992ad95ea3331e054bddacf78a46. No original compiler is called by recovered code; SDK originals remain unchanged.

## Complete root contracts

| Root | TypeId | TypeHash | Root bytes |
| --- | ---: | ---: | ---: |
| Weather | 2430161325 | 3795761625 | 24 |
| WaterTransparency | 1479531562 | 525928939 | 16 |
| WaterSetting | 2728605516 | 469423908 | 12 |
| Environment | 680780553 | 1276225790 | 156 |
| LightingSettings | 3169690211 | 616355229 | 48 |
| WaterTextureList | 2995571269 | 1905565848 | 16 |
| TerrainParticleMapping | 1469498551 | 2736882942 | 16 |
| BounceKickTerrainMapping | 2368878637 | 2109862985 | 12 |

Each exposes Recovered::<Root>Compiler::CompileXml and ProcessInstance; shared XmlenvironmentCompiler routes only these eight canonical roots. Metadata follows original XmlCompiler: AllTypesHash1424942948; version Win32/unknown4, PS3=7, Xbox360=6; ProcessingHash=TypeHash XOR version; Tokenized/HasCustomData/UseBuildCache=false.

All roots reserve word@0 and use a single Tracker with absolute base0, no Tokenizer header. Platform0 little endian;1/2/99 big endian. Packed bools remain bytes, numeric fields/counts swap, and Tracker handles pointer/import conversion. Namespace-qualified attributes, foreign elements, unknown attributes/children/text and duplicate singleton nodes are refused. BAB's unqualified structural TypeId is accepted and ignored; heuristic-style polymorphism is not used by these roots. Canonical root name/Handle determines ProcessInstance dispatch. id/inheritFrom are metadata on roots only: WeatherData.id remains the real WeatherType field.

## Layouts and original evidence

Original Module blob2d2f471c8008b7c2e1f78e39752d0726da395cea, SHA256be379d88db4c9f6de9cb4844bfc86a58d880d7a88a6b28cc76ba1c1bfc0fc6c1. Original Plugin blob00bb9e5df72d07a21204f85f9a9a2f1d9557f00b supplies exact root metadata and inline WaterTextureList/BounceKickTerrainMapping contracts. Fixed original DLL SHA256d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6 is checked by the oracle.

- Weather Marshal25822/endian25900, parts0050/0051; root ctor91246 part0182. Root has nullable Rain@4(size60), Lightning@8(size12), Cloud@12(size8), and WeatherData count@16/items@20, in that allocation order. Rain has texture ref@0, signed count@4(default16000), twelve floats@8..52 with original defaults, bool@56(defaultfalse), padding57..59. Lightning has signed duration@0(default30), chance@4(default0.01), optional random pointer@8(size12). Cloud contains two optional Vector2 pointers@0/4. WeatherData items are12 bytes: WeatherType id@0, Sound ref@4, HasLightning byte@8, padding9..11.
- WaterTransparency Marshal25989 part0051/0052, ctor91428 part0182. Depth@4(default3), minOpacity@8(default1), StandingWaterTexture typed hash@12.
- WaterSetting Marshal26047 part0052, ctor91519 part0183. TimeOfDay@4, optional inline packed Color4@8. Present color defaults RGB0/A255, truncates unsigned channels to byte and packs ARGB; absent color stays0. Original Plugin explicitly swaps both words for big endian.
- Environment Marshal43452 part0086, ctor98253 part0196. Inline CloudEffect@4(size152) includes refs@0/4/8/32, PropagateSpeed@12, signed Angle/DarkeningRate/LighteningRate@16/20/24, CloudScrollSpeed@28, dissipate floats@36/40/44, lightning chance/frequency/shadowIntensity@48/52/56, LightningFX typed hash@60, three RGBColor@64/76/88, three Vector2@100/108/116, two inline random variables@124/136 and three packed bools@148..150. All fields/dependencies are implemented.
- LightingSettings Marshal52614 part0105, ctor111539 part0223. LoadAfter ref@4, TimeOfDay@8, OccludedLuminanceScale@12(default0.5), RGB multiplier@16, inline CurrentLightingSettings@28(size12), inline TerrainLighting list header@40. LightingTime is12 bytes: enum@0/list count@4/items@8; LightingInfo is36 bytes with Ambient RGB@0, Diffuse RGB@12, LightPos Vector3@24. TerrainLighting holds LightingTimeGroup entries, allocated contiguously before each nested Lighting array. Source list methods87349/87418 part0174 verify sizes36/12.
- WaterTextureList original Plugin HandleType around3646; ctor91610 part0183. Type enum@4, string-list count@8/items@12; each8-byte String contains native count and pointer. Entire item array precedes per-string blocks. Missing/empty lists remain null; empty strings allocate NUL blocks.
- TerrainParticleMapping Marshal53710 and TerrainParticleMap53689 part0107, ctor112449 part0224. AccelerationMultiplier@4(default100%, serialized1), Mapping count@8/items@12. Each128-byte item has TerrainClass@0, ParticleSystem ref@4, RequiredModelConditions456bit@8 and ForbiddenModelConditions456bit@68. List allocation/endian methods76911/76953 part0153 verify size128 and15 words per bitset.
- BounceKickTerrainMapping original Plugin HandleType around3022, ctor91064 part0182. Mapping count@4/items@8, each8-byte BounceKickTerrainData is TerrainClass@0 plus BounceMultiplier percentage@4; Marshal24859 part0049, list62833/endian62873 part0125.

Enum name/value tables are original PE exports at evidence/xml-source/enums. Unknown WeatherType falls back1, TimeOfDay2, WaterTextureType1, Distribution0, TerrainClass3. Enum names are case-sensitive; absence is not the same as a present unknown word. Vector2/Vector3 present nodes default axes0; RGBColor has no channel defaults; present Random defaults TypeUNIFORM with absent low/high retaining0.

Model conditions reuse the already recovered MarshalModelCondition and EnumBitset.cpp, with456 bits, original unknown index233, original ALL sign/tail behavior and padding-bit preservation; shared code is not modified. Strings follow original Value constructor118053 part0236 (StringToHGlobalAnsi) and String Marshal60055 part0120 (strlen), not UTF-8. Embedded NUL truncation, empty/present allocation and four-byte alignment are preserved.

## Files and verification

Native Xmlenvironment.cpp/h, XmlenvironmentLayouts.inc and XmlenvironmentEnums.inc implement only this group. The immutable layout registry follows native InitOnce with process-lifetime heap ownership, so no std::map/vector destructor is registered at CLR exit. Bridge XmlenvironmentCompiler.cpp validates namespaces and provides the eight conventional public compiler classes.

TestXmlenvironment.ps1 builds /Od and /O2 with the existing scalar/resource/relocation/bitset sources. XmlenvironmentOracle.cs runs platforms0/1/2/99 and compares all three arrays against original ProcessInstance plus conventional CompileXml; each fixture produces six array comparisons. Every field/enum has boundary cases; all457 model-condition names and sign/ALL/unknown behavior are tested in both bitsets. List cardinalities0/1/2/3/27/28/29/64/257, pointers, imports, strings including Unicode/NUL, metadata, malformed input and independent full Weather/Lighting/String/Color/Mapping layouts are included. Script preserves real process exit codes and writes validation sourceCommit/runId and per-mode logs/results. Local GNU syntax checking uses a scratch Windows API stub and disabled x86 size assertion solely for syntax; it does not replace Windows validation.

XmlenvironmentProbe.xml contains sixteen schema-shaped SDK assets (Defaults/Combined for each root) and no new strong references. Environment requires CloudEffect; WaterSetting requires TimeOfDay+DiffuseColor; WaterTextureList requires Type+Texture; LightingSettings includes multiplier, exactly3 current lights and exactly6 terrain time groups of3 lights. The combined mapping probes use valid enum/bitset/percentage forms. SDK XSD validation, original/recovered fresh builds, four-artifact comparison, trace replay and no-original-DLL replay still need their own successful final-source evidence. Component malformed-boundary samples are not represented as legal SDK samples.

