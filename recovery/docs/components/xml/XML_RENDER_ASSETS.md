# Texture-image and mesh-instance descriptions

Three complete original registered global XmlCompiler roots. Component-only, SDK pending; no image decoding, renderer replacement or SDK route changes are implied.

| Entry | TypeId | TypeHash | Root bytes | Complete fields |
| --- | ---: | ---: | ---: | --- |
| MeshInstanceTemplate | 815686049 | 1966615229 | 12 | Rule list count@4/pointer@8 of MatchRule12 |
| PackedTextureImage | 1449288224 | 4177308976 | 36 | Texture strongref@4; Dimensions ICoord2D8@8; Coords@16; TextureDimensions@24; Rotated byte@32 |
| OnDemandTextureImage | 4262364347 | 333131708 | 36 | same layout, original OnDemandTexture strongref helper |

All roots have reserved zero first words. MatchRule12 is MeshID String8@0 and Instances signed32@8. MeshID is actually a case-preserving string in the original implementation, despite schema AssetId annotation. Each ICoord2D8 contains signed32 x/y with getter defaults0. Root padding33..35 remainszero and Rotated byte32 is not endian-swapped. Texture strongrefs finalize as index+1 and imports; inline coordinates create no allocations.

Original evidence: Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`, Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`, DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Module part0081 lines40772/40829/40887/40944 contains complete PackedTextureImage/OnDemandTextureImage Marshal and EndianCorrect; all six coordinate words swap, bool and padding do not.
- Part0038 line19010 contains complete ICoord2D signed CRT Marshal, x/y defaults0.
- Part0093 line46605 contains complete MatchRule Marshal. Part0146 lines73114/73156 contains complete12byte list and endian. Original Plugin MeshInstanceTemplate HandleType around14942 places this list at+4.
- Root tracker constructors96342/96433/101165 and decompiled sequential structures prove36/36/12 roots and12byte MatchRule. Schema cross-checks: AssetTypeMeshInstanceTemplate.xsd, AssetTypePackedTexture.xsd, AssetTypeOnDemandTextureImage.xsd.

Mesh array allocates in full before entry MeshID strings. Missing names remain null; presentempty strings allocate padded4bytes. C-string truncation, CRT signed boundaries and boolean parsing reuse independent native helpers. Unknown/qualified attrs, foreign children, structured text and duplicate coordinate singletons explicitly reject.

Implementation `Xmlrenderassets.{h,cpp}`, public routes `XmlrenderassetsCompiler.cpp`, catalog `XmlrenderassetsTypes.json`, oracle `XmlrenderassetsOracle.cs`, runner `TestXmlrenderassets.ps1`. Full x86 /Od and /O2, platforms0/1/2/99, original ProcessInstance versus restored ProcessInstance/direct CompileXml, exact three arrays, every field, scalar/CRT partial/overflow/NUL/boolean/reference boundaries, Unicode/empty/NUL names to65535, complete Rule lists to1024, explicit refusals and independent fixed ABI for root36/12, Rule12, all ICoord words, unswapped Rotated byte/padding, strings and complete import/relocation order.

Local native g++ syntax preflight passes after removing only pointer-size assertion in a temporary copy; production retains x86 enforcement. Parent Windows integration supplies final differential evidence; no unrun component acceptance or SDK compilation is claimed here.
