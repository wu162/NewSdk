# Content and descriptive media components

Five complete original registered XmlCompiler roots, grouped for shared string/strong-reference/weak-hash serialization validation. No main Plugin/SDK routing changes; every new entry remains SDK pending.

| Entry | TypeId | TypeHash | Root bytes | Fields |
| --- | ---: | ---: | ---: | --- |
| EngagingContent | 3058114834 | 725292512 | 28 | TitleId String8@4, DescId@12, ImageId@20 |
| EngagingContentGroup | 11280974 | 1507896455 | 12 | EngagingContent strong-reference list count@4/pointer@8, stride4 |
| EngagingContentSequence | 3799634821 | 226620221 | 20 | MapName String8@4, RequiredContent strongref@12, RandomContent strongref@16 |
| IntelDB | 530081230 | 751047049 | 12 | IntelDBEntry list count@4/pointer@8, stride24; CategoryId String8@0, EntryId@8, TextureId@16 |
| UnitIntro | 1870180772 | 1460143826 | 12 | Video typed weak hash@4, Audio strongref@8 |

All five are global schema assets and each has a reserved zero root word. IntelDBEntry is a complete nested helper, not an extra Plugin entry. EngagingContent/Group/Sequence form the first dependency family; IntelDB and UnitIntro are independent descriptive media roots, sharing exactly the same proven serialization helpers and test matrix.

## Evidence

Fixed Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; original DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Module parts0084 lines42060/42085: complete EngagingContent Marshal/EndianCorrect; Strings are processed TitleId then DescId then ImageId, regardless of XML attribute order. Sequence Marshal at42116 processes MapName then two strong references. Original Plugin sequence HandleType swaps only MapName.length; strong refs finalize through tracker.
- Part0140 line70260: complete4-byte strong reference list for EngagingContentGroup. Original Plugin HandleType around10650 proves root list at+4 and swaps only its count before finalizing.
- Part0088 lines44388/44413: complete IntelDBEntry Marshal/EndianCorrect of three String8. Part0143 lines71902/71944: list24stride, allocate entire list first, then each entry's string blocks in field order. Original Plugin HandleType around13145 proves IntelDB root list@4.
- Part0101 line50807: complete UnitIntro Marshal. Part0174 line87180 proves VideoEventList typed id is a lowercase C-string FastHash, not an import. Original Plugin HandleType around25768 swaps Video word and finalizes Audio through tracker.
- Root tracker constructors97434/97525/97616/99618/110174 and decompiled sequential layouts independently establish28/12/20/12/12 bytes.
- Schema cross-checks: AssetTypeEngagingContent.xsd, AssetTypeIntelDatabase.xsd and AssetTypeUnitIntro.xsd. Required schema attrs are not artificially imposed on the component Marshal API: absent fields stayzero, matching the original DLL.

Absent strings leave zero length and null pointer; present-empty strings allocate a NUL byte padded to4. Strings preserve C-string truncation at embedded NUL. Strong references use BAB-preprocessed numeric suffixes, produce index+1 and sorted tracker import bookmarks; unbound names yield no reference. Typed weak Video hashes lowercase text and never create imports. Native string, reference and hash work uses independently restored ResourceMarshaling.cpp.

## Tests

Sources `Xmlcontent.{h,cpp}`, public routes `XmlcontentCompiler.cpp`, catalog `XmlcontentTypes.json`, oracle `XmlcontentOracle.cs`, runner `TestXmlcontent.ps1`.

Runner retains x86 /Od and /O2, platforms0/1/2/99, original ProcessInstance versus both restored ProcessInstance and direct CompileXml, exact comparison of all three arrays. Oracle covers every field and optional presence, Unicode and embedded NUL, string lengths crossing4/8/16/28/32/256/1024/4096/65535 allocation boundaries, strong-reference signed/overflow/unbound inputs, weak Video hash boundaries, group and IntelDB list growth to1024, ownerblock/reference-value ordering, strict namespace/unknown/qualified/text rejection, and independent fixed ABI assertions for all roots, pointers, strides, lengths, bytes, booleans of absence, index+1 values and relocation/import arrays.

Local syntax-only g++ preflight passed after removing the pointer-size assertion in a temporary source copy; production source retains x86 assertion. Parent Windows integration supplies full original-DLL differential evidence; no unrun matrix or SDK success is implied by this document.
