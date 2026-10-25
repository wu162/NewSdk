# Content licenses and asset/string indexes

Three complete original registered global XmlCompiler entries. Component-only, SDK pending; no main SDK route change or real SDK compile.

| Entry | TypeId | TypeHash | Root bytes | Complete layout |
| --- | ---: | ---: | ---: | --- |
| DLContent | 212695140 | 1654379548 | 24 | License list@4, Patch list@12, PackageNumber byte@20 |
| AssetReportTable | 255713734 | 1788767883 | 12 | AssetReport20 list@4 |
| StringHashTable | 300273508 | 3959342059 | 16 | StringHashBin enum@4, StringAndHash12 list@8 |

License12: unsigned LicenseMask@0, Asset typed weak-hash list count@4/pointer@8 (stride4). Patch12: Name String8@0, PatchNumber byte@8. AssetReport20: raw StringHash Id@0/Type@4, AssetSize unsigned@8, Reference8 list count@12/pointer@16; each reference has raw Id@0/Type@4. StringAndHash12: Hash unsigned@0, Text String8@4. Package/Patch bytes truncate unsigned CRT values to8 bits and never swap with endian.

AssetReport StringHash fields hash exact case-sensitive C-string bytes. DL Asset typed ids lowercase before hashing. StringAndHash.Hash is a stored unsigned numeric value, not a hash of its textual numeric input. StringHashBin absent stays0, presentunknown falls backINSTANCEID1. `Version` is recognized schema pipelineOnly metadata ignored by the original StringHashTable Handler; it creates no output data.

## Original evidence

Module blob `2d2f471c8008b7c2e1f78e39752d0726da395cea`; Plugin blob `00bb9e5df72d07a21204f85f9a9a2f1d9557f00b`; DLL SHA256 `d4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6`.

- Part0089 lines44646/44666/44681: complete DLPatch, DLLicenseGroup and DLContent Marshal. Part0144 lines72185/72235/72268/72310/72352/72386: complete weak DLAsset4 lists, License12/Patch12 lists and endian. Typed weak hash implementation at116672 in part0233 lowercases before FastHash.
- Part0101 lines50512/50555/50605: complete Reference8/Report20 raw FastHash and report endian. Part0152 lines76147/76187/76231/76273: complete nested Reference8 and Report20 lists and endian. Plugin AssetReportTable HandleType around25248 establishes root list@4.
- Part0119 line59868: complete StringAndHash12 Marshal. Part0162 lines81364/81406: complete list and endian. Plugin StringHashTable HandleType around31277 reads only StringHashBin and StringAndHash, maps unknown bin1 and does not read pipelineOnly Version. Exact four-word PE table `xml-source/enums/StringHashBin.json`.
- Root tracker constructors99891/109719/114906 and decompiled sequential structures prove all listed sizes. Schemas: AssetTypeDLContent.xsd, AssetTypeAssetReport.xsd, AssetTypeStringHashTable.xsd and AssetTypeStringHashEnum.xsd.

DLContent allocates all License records, each nested weak Asset array, then the entire Patch array and its Name strings, regardless of XML child order. ReportTable allocates all20byte records before nested8byte references. StringHashTable allocates all12byte records before Text strings. Presentempty strings allocate padded4byte blocks; absent strings do not. Strictly rejects unknown/qualified attrs, unknown/foreign children and structured text; all input C-string truncation is preserved. None of these hash/index fields emits asset imports.

## Verification

`Xmlassetindexes.{h,cpp}`, `XmlassetindexesCompiler.cpp`, `XmlassetindexesTypes.json`, `XmlassetindexesOracle.cs`, `TestXmlassetindexes.ps1`.

Full x86 /Od and /O2, platforms0/1/2/99, original ProcessInstance versus restored ProcessInstance/direct CompileXml, exact three-array comparison. Complete all-field tests include rawcase-sensitive versus weakcase-folded hash contracts, Unicode/NUL/empty strings, every bin PE value/case/sign/whitespace/fallback, CRT unsigned overflow and byte truncation, all nested/outer lists through1024, text/hash lengths to65535, pipelineOnly ignore behavior, strict rejection and fixed independent ABI checks for24/12/16 roots,12/20/8 helpers, byte fields, string values, source allocation and ownerblock relocation arrays.

Local g++ syntax preflight passes with only x86 pointer-size assertion removed from a temporary copy; production retains x86 enforcement. Parent Windows integration supplies final differential evidence; no unrun component or SDK success is claimed here.
