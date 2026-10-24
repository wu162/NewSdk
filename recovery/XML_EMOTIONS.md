# Stance and Emotion components

Complete StanceTemplate and EmotionTemplate original registered roots. Both are global schema assets. Component-only; SDK integration pending. No SDK route or main Plugin modification.

## Fixed evidence

Original Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, original DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

Plugin HandleType methods at lines 10940 and 14480, exact TypeId/TypeHash in GetExtendedTypeInformation cases 2525284163/1210708834 and 2836737428/1088041681. Root tracker constructors at Module 97798/100801 allocate 12/324 bytes (`evidence/xml-source/parts/part-0195.cs.txt` and `part-0201.cs.txt`). StanceDefinition Marshal at 42131, nullable modifier helper 85788, list/endian at 85801/85841. Emotion Marshal/Endian at 46075/46186 (`part-0092.cs.txt`). Enum maps retained in `evidence/xml-source/enums/{Stance,EmotionType,EmotionAIType,EmotionTemplateThreatComparisonType,EmotionTemplateOptions}.json`; immutable native tables copy those exact values.

Schemas: `SDK/Schemas/xsd/AssetTypeStanceTemplate.xsd`, `AssetTypeEmotionTemplate.xsd`, `Includes/Stance.xsd`, `Includes/Emotion.xsd`.

## Exact complete layouts

StanceTemplate is 12 bytes: reserved word, count +4, list pointer +8. Each StanceDefinition is 8 bytes: enum Type +0 and nullable AttributeModifier reference pointer +4. A present modifier allocates a 4-byte child even for empty text or text without a backslash; absence allocates nothing. This is distinct from Emotion's inline modifier reference. Type is GUARD0/AGGRESSIVE1/HOLD_POSITION2/HOLD_FIRE3; unknown present text falls back to AGGRESSIVE1, absent stays zero. The original marshaler does not enforce the XSD max32 list bound.

EmotionTemplate is exactly 324 bytes; all absent fields remain zero.

| Offset | Full field |
| ---: | --- |
| 4 | Type enum, present unknown ALERT8 |
| 8 | Options bitset6, unknown token bit4; ALL sets all32 including padding |
| 12/16/20/24 | DurationActive/Inactive/InactiveObject/InactiveType, TimeTag seconds |
| 28/32 | EnemyThreatComparison enum / signed EnemyThreat |
| 36/40 | FriendlyThreatComparison enum / signed FriendlyThreat |
| 44/48/52/56 | StartFX/UpdateFX/EndFX/AttributeModifier inline strong references |
| 60/64/68 | AttributeModifierStartDelay/Duration, AIStateLockDuration seconds |
| 72 | AIState enum, present unknown AVOID_SCARER2 |
| 76/136/196/256 | Four inline60-byte ModelCondition sets: SetOnStart/SetOnEnd/ClearOnStart/ClearOnEnd |
| 316/320 | AILuaEventsList String length / data pointer |

Threat enum is NONE0/VALID_IF_BELOW1/VALID_IF_ABOVE2, unknown0. `Time` reuses original TimeTag initialization (case-sensitive `ms` before `s`, legacy float behavior). Strings use first-NUL length and allocate a terminator even when present-empty. Scalar enum comparisons follow original C-string truncation. All four model sets preserve full456-bit vocabulary, signs/XOR removal, ALL padding behavior and legacy unknown fallback. Reference pointers/imports use existing tracker ordering (owner block then target/index), not global output offsets. Every non-Win32 platform uses big endian.

## Implementation and matrix

`src/Native/Xmlemotions.{h,cpp}`, `XmlemotionsEnums.inc`, `src/Bridge/XmlemotionsCompiler.cpp`, `catalog/XmlemotionsTypes.json`, `tests/XmlemotionsOracle.cs`, `scripts/TestXmlemotions.ps1`.

Matrix: original ProcessInstance plus direct CompileXml, all three arrays, /Od and /O2, platforms0/1/2/99, fixed DLL fingerprint. Tests cover complete enum maps and independent numeric values, all457 ModelCondition spellings on all four fields (including ALL), signed repeated tokens, NUL truncation, model and option unknown fallbacks, all seven Time fields with numeric/unit/overflow boundaries, both signed threats, all inline/nullable reference forms, string present/absent/NUL/long/multilingual boundaries and Stance counts0–1024 including33. Independent assertions pin defaults, every scalar field offset, 12/324 root sizes, list8 and nullable4 allocations, 56-byte mixed stance ABI, inline model60 offsets/bits, 328-byte full Emotion ABI, Lua string bytes/pointer, owner-based imports and relocations. Unsupported roots, metadata namespace/qualified attributes, unknown children/attributes, nested text, and handle mismatch are explicit rejections.

Status: implementation/matrix prepared, actual Windows results to be recorded by integration. No SDK real compilation performed.
