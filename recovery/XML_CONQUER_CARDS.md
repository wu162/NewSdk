# Conquer card definitions and decks

Five complete original registered XmlCompiler entry points. Four derived/deck roots are global assets; ConquerCardDefinitionBase is a real Plugin entry with a nonglobal schema base. All remain component-only and SDK pending. GameObject appears only as an asset-reference field; no GameObject source is changed.

| Entry | TypeId | TypeHash | Bytes | Complete fields |
| --- | ---: | ---: | ---: | --- |
| ConquerCardDefinitionBase | 3258480561 | 343077480 | 12 | reserved word@0, Cost signed@4, Maximum signed@8 default0 |
| ConquerCardDefinitionUnit | 3562795973 | 3660672133 | 32 | base12, Unit strongref@12, Upgrade list8@16, Veterancy list8@24 |
| ConquerCardDefinitionStructure | 4289687760 | 3391723256 | 16 | base12, Structure strongref@12 |
| ConquerCardDefinitionPower | 4164089898 | 587170934 | 24 | base12, Power strongref@12, Side String8@16 |
| ConquerCardPrebuiltDeck | 2806522526 | 808620711 | 36 | reservedword, DisplayName String8@4, Description@12, Side@20, Cards pointer list@28 |

Definition Upgrade8 is Upgrade strongref@0 plus Cost signed@4. Definition Veterancy8 is VeterancyLevel enum@0 plus Cost signed@4. DefinitionUnit processes upgrades before veterancy regardless of XML order, then applies inherited Cost/Maximum. Power.Side and Deck.Side are actual String8 preserving text/case, despite the schema AssetId annotation; they are not hashes.

## Polymorphic cards: exact original dispatch

Deck.Cards is a list of4byte pointers. Original Marshal dispatches on each child's numeric TypeId, independently of element local name:

- TypeId849812912 allocates ConquerCardUnit24: type tag@0, Card strongref@4, Count signed@8 default1, Veterancy enum@12 defaultREGULAR0, Upgrade list count@16/pointer@20 of4byte strongrefs.
- Every other or absent numeric TypeId invokes original `MarshalUnknownPolymorphicType<ConquerCard>`: allocates the actual base ConquerCard8, marshals Card strongref@4, and writes canonical tag0@0. This preserves Structure/Power and arbitrary-tag base records exactly as the original. It is a specifically recovered original polymorphic-base method, not an unimplemented root fallback; unknown root entries still reject. Extra unsupported fields inside base records explicitly reject.

The pointer array allocates before any cards. Each concrete card allocates its Upgrade array before the next card; ownerblock ordering puts all pointer-array relocation bookmarks ahead of nested card bookmarks, while imports group by card/upgrade owner before sorting numeric reference indices. Original endian dispatch distinguishes tag849812912 versus base tag0; all fields and list counts retain the exact platform behavior.

## Evidence

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; fixed DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Part0117 lines58617/58640/58661/58678/58720/58766/58798/58813/58828/58843 contains the complete Card dispatch, Base, CardUnit Marshal/Endian, Deck Marshal/Endian, Power, definitionUpgrade, definitionVeterancy and definitionUnit.
- Original Plugin Structure HandleType around29984 directly marshals Structure strongref@12 then Base and corrects Cost/Maximum; no omitted specialized Marshal is guessed.
- Part0160 line80198 is the complete original unknown-polymorphic base8 method;80220 is CardUnit.Upgrade4 list;80265 is polymorphic list endian dispatch;80310/80350/80390/80425 are definition Upgrade8/Veterancy8 complete marshal/endian. Veterancy getter at80209 maps presentunknown toELITE2; exact PE five-word table `xml-source/enums/VeterancyLevel.json` includes INVALID-1.
- Part0176 line88251 is the complete Cards pointer-list Marshal using all child elements. Declarations and root tracker constructors113632..113996 prove root sizes and helpers8/24. Schema cross-check: Multiplayer/ConquerMode.xsd; CnC3Types globally registers only derivedUnit/Structure/Power/Deck.

## Verification

`Xmlconquercards.{h,cpp}`, public routes `XmlconquercardsCompiler.cpp`, catalog `XmlconquercardsTypes.json`, oracle `XmlconquercardsOracle.cs`, runner `TestXmlconquercards.ps1`.

Full x86 /Od and /O2; platforms0/1/2/99; original ProcessInstance versus recovered ProcessInstance/direct CompileXml; all three exact arrays. Oracle covers every root/nested field, complete Veterancy PE words and signedINVALID, CRT signed fields/unsigned dispatch parsing, case/sign/NUL/unknown enum boundaries, all asset-ref suffix boundaries, source-allocation ordering, mixed concrete/base card dispatch including missing/unknown/numeric/hex TypeIds and arbitrary tag names, strings to65535, nested definition/polymorphic/upgrade growth1024, metadata and explicit refusals, plus independent fixed ABI assertions for all12/16/24/32/36 roots and4/8/24 helpers, card type tags, values and exact relocation/import owner order.

Local g++ syntax preflight passes with only the pointer-size assertion removed from a temporary copy; production retains x86 enforcement. Parent Windows integration provides final differential evidence. No unrun component acceptance or real SDK success is claimed here.

## Host compile correction

The first run40141089972 failed before executing the Conquer oracle because C#4 rejects the later deck local `d` when a defaults-loop child scope already declared `d`. The defaults fixture local is now named `rootDefault`. No native code, fixture, byte comparison or ABI assertion changed; the final integration run must execute the complete matrix after this host-only correction.
