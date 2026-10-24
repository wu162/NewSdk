# XmlCompiler progression group (implementation checkpoint)
Original sourceBlob: 2d2f471c8008b7c2e1f78e39752d0726da395cea.
Original Plugin: 00bb9e5df72d07a21204f85f9a9a2f1d9557f00b.
Five roots: LogicCommand28, PlayerTech16, PlayerTechRank20,
PlayerTechUpgradeBinding12, ExperienceLevelTemplate108.
Native/bridge implementation prepared; complete Windows oracle is pending.
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

