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
PlayerTech and Binding root HandleType omit EndianCorrect: retain little-endian
scalar/list counts on big targets, while Tracker writes references/pointers big.
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
