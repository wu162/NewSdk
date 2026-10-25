# Movie archive components

Two complete original registered XmlCompiler entries, both nested schema types rather than global CnC3Types.xsd assets. Both remain component-only/SDK pending.

| Entry | TypeId | TypeHash | Bytes | Complete layout |
| --- | ---: | ---: | ---: | --- |
| ArchivableMovie | 3741496377 | 291064791 | 32 | ProgressLock unsigned@0 default0, MovieImage strongref@4, FileName String8@8, OnScreenName String8@16, Description String8@24 |
| ArchiveMovieList | 2099472558 | 131545626 | 12 | Faction enum@0 defaultSoviet1, count@4/pointer@8 of32byte Movie records |

Neither has a reserved zero first word. Presentunknown Faction mapsAllies0; missing Faction suppliesSoviet1. The RANDOM enum is signed-1 retained as32bits. All other enum words are exact case-sensitive C-string lookups.

Original evidence: Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`. Part0066 lines33348/33381 contains complete ArchivableMovie Marshal/EndianCorrect. Part0133 line66703 contains complete32byte list Marshal/EndianCorrect. Original Plugin ArchiveMovieList HandleType around9045 directly applies Faction Soviet default, maps unknown0, marshals Movie list@4 and endians enum/count and all movie contents. Exact PE enum is `xml-source/enums/FactionType.json`; decompiled sequential layouts/root tracker constructors96069/96160 prove32/12. Schema cross-check: UI/Components/UIComponentMovieArchive.xsd.

Source order allocates the whole Movie array first, then each item's FileName, OnScreenName and Description strings; strong MovieImage imports remain ownerblock/reference-index ordered. Presentempty strings allocate4byte blocks; absent strings do not. C-string NUL truncation follows original helpers. Unknown/qualified attrs, foreign children, structured text and nested movie children explicitly reject.

Implementation `Xmlmoviearchive.{h,cpp}`, public routes `XmlmoviearchiveCompiler.cpp`, catalog `XmlmoviearchiveTypes.json`, oracle `XmlmoviearchiveOracle.cs`, runner `TestXmlmoviearchive.ps1`. Full x86 /Od and /O2, four platforms0/1/2/99, original ProcessInstance versus restored ProcessInstance/direct CompileXml, three-array exact comparison, all fields, enum PE values/case/sign/NUL/fallbacks, CRT unsigned boundaries, Unicode/empty/NUL strings up to65535, list growth1024, explicit refusals and fixed independent ABI assertions for all32/12 layouts, byte strings, defaults and source allocation/import/relocation order.

Local native g++ syntax preflight passes in a temporary copy with only the pointer-size assertion removed; production keeps x86 enforcement. Parent integration supplies Windows differential evidence. No real SDK compile or main SDK route change is included.
