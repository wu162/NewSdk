# XmlCompiler progression group (implementation checkpoint)
Original sourceBlob: 2d2f471c8008b7c2e1f78e39752d0726da395cea.
Original Plugin: 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b.
Initial five roots: LogicCommand28, PlayerTech16, PlayerTechRank20,
PlayerTechUpgradeBinding12, ExperienceLevelTemplate108.
Eight-root native/bridge implementation and complete Windows oracle are prepared; Windows acceptance is pending.
This checkpoint is not validation or SDK acceptance.

Evidence: Marshal/Endian LogicCommand42918/43028 (parts0085/0086);
AISpecialPowerInfo42587/42726, weapon42498/42534 (0084/0085).
PlayerTech45449, PlayerTechRank45464/45486 (0090), BindingInfo45517 (0091);
binding root HandleType directly allocates list at +4 with 8-byte elements.
Experience31677/31798 and LevelUpFxList31663 (0063);
RadiusDecal component26539/26650/template26989 (0053), list64165 (0128).
Tracker constructors97980,100437,100528,100619,95159 pin root allocation sizes.
PlayerTech and Binding root HandleType inline their endian corrections:
TechPointsRequired@4/TechDependency count@8 and Binding count@4 are corrected
on big targets; Tracker writes references/pointers in the target endian.
Other roots traverse their complete fields and nested values before FinalizeTracker.
Single Tracker, absolute offset zero, no Tokenizer procedure table; preserve imports.

Long XML names and initializer constants were read from all 7498 original PE
static fields rather than truncated MSVC symbols. Original PE maps pin enum values.
Existing shared resource/string/hash/EnumBitset/ObjectFilter helpers are reused,
without changing shared implementation. Bare BAB TypeId is skipped at bridge read;
foreign attributes and unsupported XML remain explicit refusals.


Added same-dependency roots: PurchasePlayerTechButtonTemplate12
(TypeId1566941920/TypeHash2999033148), PlayerTechStoreTemplate16
(TypeId1213037816/TypeHash981786901), ButtonSingleStateData32
(TypeId3048039928/TypeHash3780991768). Metadata from original Plugin.
ButtonState Marshal/Endian48331/48357 and root48388 (part0096), constructor107353
(part0214) prove inline State@4: Image ref@4, three length/pointer strings at
8/12,16/20,24/28. Empty/absent string and ANSI/NUL behavior preserved.
Store and Purchase inline HandleType contracts come from original Plugin;
constructors110265/110356 (part0220) prove12/16 sizes. Store Faction@4,
Row count/pointer@8/12 and eight-byte nested Button reference lists. Original
row/list Marshal87271 and Endian87316 (part0174) prove both count corrections.
Purchase PlayerTech@4 and StateData@8 are references finalized by Tracker;
there are no separately corrected scalar fields. Faction original PE enum
has five entries and unknown fallback zero, with RANDOM=-1.

Oracle covers all eight roots /Od /O2 and platforms0/1/2/99, all scalar fields,
original enum entries, model/KindOf bitsets with independent bits, pointer and
reference boundaries, three original and three direct arrays, empty/present
strings and Unicode/NUL, list sizes0/1/2/27/28/29/64/257, nested store row/button
lists, independent endian/default/string layouts, prefixes, mismatched handles,
unknown XML/namespaces/duplicate singleton rejections and real process exits.

SDK source probe has eight target types plus one existing UpgradeTemplate
support asset needed for binding. Binding requires min1 with two required refs;
Experience requires LevelUpTintColor; ButtonSingleStateData requires State;
Purchase requires PlayerTech and StateData; Store requires Faction and exactly
five Rows of exactly three Buttons. All refs resolve locally. Eight-root SDK
probe remains pending full CnC3Types validation and real BAB integration.

Additional SDK source cases are in sdk/XmlprogressionCombinedProbe.xml, included
alongside the eight defaults probe. This adds eight _Combined declarations and
five _Inherited declarations (all five initial roots are BaseInheritableAsset),
plus three already recovered support assets: FXList/ObjectCreationList/
AttributeModifier. Required Binding and LevelUpTintColor source children remain
present even in inherited declarations so raw XSD validation remains meaningful.
The original BAB must expand inheritance; no numeric TypeId or resolved resource
suffix is fabricated. Logic combined exercises enum flags, nullable refs,
AISpecialPowerInfo scalar/bitset/nullable/ObjectFilter fields; Experience combined
exercises nullable prerequisite, OCL/FX/modifier refs, typed upgrade, strings,
color, model bits and a two-component Decal. Rank/Tech/Binding/Store exercise
nonempty lists; ButtonSingleStateData exercises three nonempty strings, referenced
by Purchase and five-by-three Store. All support references resolve to probe
assets. Raw XML parsing passed locally; full XSD, original BAB, recovered BAB,
four outputs and isolated replay remain required before accepting these cases.

The initial matrix at run40093373602/source3733fbc passed Win32 but failed the
first nonzero PlayerTech scalar on platform1. Investigation found inline endian
operations in Plugin.HandleType which had been missed by filtering for named
EndianCorrect calls. The recovery and independent scalar/count expectations
were corrected; full final four-platform validation is required. The failed
run is preserved and is not an acceptance anchor.

Ninth root: UICommandButton12, TypeId134745024/TypeHash2853539982,
ProcessingHash=version XOR hash, Tokenized=false. Complete original Marshal for
FactionSettings is49069 (part0098), list Marshal75836/Endian75878 (part0151),
Tracker constructor108445 (part0216), inline root HandleType in fixed Plugin.
Root list count/pointer@4/8; each12-byte record has Faction@0, LockedImage ref@4,
packed ImageTintColor@8. Faction five-entry enum unknown fallback0; target endian
corrects root count, faction and packed color; Tracker finalizes image imports.
Missing ImageTintColor remains zero, present empty defaults alpha255; byte channels
wrap. Duplicate color, bad attributes/namespaces/text/nested data refuse explicitly.
Native implementation is isolated in XmlprogressionUi.cpp; the partial oracle in
XmlprogressionUiCases.cs covers all enums, malformed/numeric/reference/color
boundaries, independent stride/ARGB/import offsets and list counts up to257.
SDK default adds no required child; combined has Allies/Soviet/Japan entries,
each with schema-required ImageTintColor RGB and no image resource dependency.
UICommandButton directly extends BaseAssetType, so no inheritance fixture exists.
Both probes now contain nine target root types, with unchanged prior IDs.

Font capability group adds three complete20-byte roots in the separate
XmlprogressionFont.cpp and XmlprogressionFontCases.cs partial oracle:
FontSubstitution91719417/hash2789100025, FontTranslation1318336226/hash1815171461,
FontDefaultSetting1371131236/hash197748432. Original Marshal/Endian in part0094:
47019/47076 (DefaultSetting),47103/47130 (Translation),47161/47191 (Subtitution),
47222 (Substitution). FontDesc Marshal35409/Endian35430 (part0070) is complete:
Name String<char>@0/4, unsigned Size@8, Bold@12. Tracker constructors101711/
101802/115361 (parts0203/0230) pin root20. FontSubstitution's root endian is
inline in Plugin; its root Name@4/8 precedes Substitute count/pointer@12/16.
Each20-byte item has Size@0 and inline FontDesc@4. List Marshal86983 spans
parts0173/0174 and marshals all input elements without a runtime cap; the
SizedList10 restriction belongs to XSD (2..10), and the component matrix tests
11/27/28/29/64/257 against the original rather than imposing an invented cap.
DefaultSetting Name@4/8 is followed by nullable unsigned Size@12 (child text),
then inline Settings.AntiAliased@16. Translation has Name@4/8 then two floats
TranslateX/Y@12/16. Original ANSI string allocation order and absent/empty/NUL
contracts are preserved; no font file is opened and no original DLL is called.

Font matrix adds all fields/numeric/string/boolean boundaries, nullable Size,
partial and absent FontDesc, lists and independent full layouts, Unicode/NUL,
structural rejection and BAB TypeId metadata. SDK font source is in
sdk/XmlprogressionFontProbe.xml: three defaults, three combined and two inherited
(DefaultSetting and Substitution extend BaseInheritableAsset; Translation does
not). Raw-XSD required Name/Settings.AntiAliased and Substitute Size/Font are
explicit, as are FontDesc Name/Size and Translation X/Y. Inherited substitution
provides two raw entries and remains within max10 after inheriting two base
entries. Actual BAB preprocessing, XSD, four artifacts and isolated replay remain
pending final Windows acceptance; no font outcome is claimed from local syntax.
