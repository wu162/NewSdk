# Localization, font descriptions and credits

Five complete original Plugin component roots; all remain SDK pending. No main SDK plugin routing or real SDK compilation is changed.

| Entry | TypeId | TypeHash | Root bytes | Complete data |
| --- | ---: | ---: | ---: | --- |
| Language | 2915726794 | 3221193174 | 44 | Separators24@4, UnicodeFontName String8@28, FontInfo list8@36 |
| HeaderTemplate | 1218703046 | 1075836011 | 20 | inline FontDesc16@4 |
| FontDirectory | 3569007303 | 3617141059 | 12 | Path String8@4 |
| TheVersion | 4236434438 | 2782889071 | 12 | inline Version8@4, Major/Minor signed32 |
| Credits | 3978455785 | 282482670 | 32 | Scroll12@4, Style list8@16, CreditItem list8@24 |

All five are global schema assets, with reserved zero first words. FontDesc16 is Name String8@0, unsigned Size@8, byte Bold@12 defaultfalse. FontInfo20 is FontUse enum@0 then FontDesc@4. CreditStyle24 is StyleName enum@0, packed ARGB@4, FontDesc@8. CreditItem12 is StyleName enum@0 then Text String8 list at+4. Scroll12 is two unsigned words plus byte ScrollDown.

## Original evidence

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Module part0070 lines35409/35430: complete FontDesc Marshal/EndianCorrect; only Name length and Size swap, byte Bold does not.
- Part0090 lines45120/45141: Version and TheVersion; absent Version leaves zero, signed CRT parsing of Major/Minor only when attrs exist.
- Part0092 line46337: HeaderTemplate inline FontDesc@4.
- Part0093 lines46625/46650: SeparatorList three String8, getter defaults comma/period/colon. Present-empty Separators allocates all three default strings; absent Separators remains zero. FontDefine at46681/46725; Language at46758, processing Separators, UnicodeFontName, then FontInfo list.
- Part0094 lines47236/47258/47299/47341: ScrollInfo, CreditStyle and Credits. Credits processes all Style records and their fonts before any CreditItem records regardless of XML element order. ColorTag body at18499 in parts0036/0037 packs ARGB and applies alpha255 only to present Color.
- Part0146 line73200: FontDefine20 list; StyleName getter73490 has presentunknown fallback3. Part0147 lines73501/73543/73616: Style24 list and CreditElement12 list with nested Text String8 list; full list array allocated before entry string suballocations.
- Part0173 line86972: FontUse getter presentunknown fallback2. Exact5/4 PE enum tables retained in `xml-source/enums/FontUse.json` and `StyleName.json`; lookup is exact case-sensitive C-string.
- Original Plugin FontDirectory HandleType around10870 directly marshals Path@4 and swaps only its length. Root tracker constructors and decompiled sequential structs confirm all listed root/helper sizes.
- Schema cross-checks: AssetTypeLanguage.xsd, AssetTypeHeaderTemplate.xsd, AssetTypeFontDirectory.xsd, AssetTypeTheVersion.xsd, AssetTypeCredits.xsd. Component behavior follows original getters and allocation, including optionality distinct from schema validation.

Strings and enums truncate at original IValue C-string boundaries. Unknown/qualified attrs, foreign namespaces, unknown children, structured text and duplicate singleton children explicitly reject. Repeated FontInfo/Style/CreditItem/Text lists preserve input order and allocate without imposing unrelated schema caps. No font/text field is an import reference.

## Verification

`TestXmllocalization.ps1`: x86 /Od and /O2, platforms0/1/2/99, original ProcessInstance and restored ProcessInstance/direct CompileXml, all three arrays. `XmllocalizationOracle.cs` covers every root and nested field, PE enum values/case/sign/NUL/fallback, unsigned/signed CRT/boolean boundaries, packed color truncation, Unicode/empty/NUL strings and lengths up to65535, independent fixed ABI (font16/FontInfo20/Style24/Item12 offsets, byte fields, defaults, allocation and ownerblock relocation order), strict refusal boundaries, and all lists including nested texts to1024 entries.

Native `Xmllocalization.cpp` syntax preflight passes g++ after only removing the x86 pointer-size assertion in a temporary copy. Production source retains the assertion. Windows original-DLL evidence is supplied by parent integration; this document claims no unrun differential or SDK success.
