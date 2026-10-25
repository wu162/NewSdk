# W3X recovery SDK fixture MOD

Entry point: `Data/Mod.xml`. This MOD contains 8 W3DAnimation assets, 4
W3DMesh assets, and two local dependencies: a two-pivot W3DHierarchy and a
Texture backed by `Art/W3xRecoveryTexture.tga` (an unchanged copy of the
shipped `SDK/Art/W3/W3XView.tga`). No GameObject, weapons, audio source assets,
or unrelated existing mymod files are included.

The root retains the usual reference-only `DATA:static.xml`, `DATA:global.xml`
and `DATA:audio.xml` includes. Keep the shipped `SDK/builtmods/Static.manifest`,
`Global.manifest` and `Audio.manifest` in disposable SDK copies; remove only
derived caches and outputs for this MOD. These references must not become
fresh full static/global/audio builds.

`coverage.json` records exact compile-input file hashes, dependencies, asset
IDs and intended coverage. `static-xsd-validation.json` records a focused local
schema check. Full Windows `XmlSchemaSet` validation against unmodified
`SDK/Schemas/xsd/CnC3Types.xsd` remains required before BAB is launched.
`recovery/scripts/ValidateW3xSdkInputs.py` validates the four XML documents using
unchanged original SDK type definitions reachable from these assets, and
retains negative controls for missing vertex X and quaternion W attributes.
The focused check avoids unrelated SDK schemas that libxml cannot load in the
Linux environment; it does not replace the complete Windows schema gate.

## Coverage

- Animations include X/Y/Z translations, visibility, quaternion orientation,
  real Hierarchy references, omitted and explicit-default CompressionSettings,
  forced Timecoded, forced AdaptiveDelta, both-disabled Timecoded fallback,
  reduction/error settings, one-frame channels, nonzero FirstFrame, quaternion
  sign changes, and BinaryMove flags.
- `DeltaOnly` uses 65 constant frames, for which the original adaptive
  compressor succeeds with scale 0 and zero deltas. `DeltaFallbackError` uses
  schema-legal MaxAdaptiveDeltaError=-1 to force adaptive compression failure
  and Timecoded fallback even though AllowTimeCoded=false. BinaryMove alone
  does not disable AdaptiveDelta. Actual runtime codec selection should be
  checked from captured output, rather than inferred from the flags.
- Meshes cover Normal, Skin, CameraAligned and CameraOriented geometry; the
  Skin fixture has two position/normal/UV/bone-influence streams, complementary
  two-bone weights, tangents, binormals, RGBA vertex colors, ShadeIndices, four
  shader constant kinds including a real Texture reference, bounds, two
  triangles, and a three-node AABTree with Children and Polys alternatives.
  Other meshes cover default colors/no normals, no AABTree, no shader
  constants, and valid unpaired tangent/binormal streams ignored by the
  original vertex builder.

## SDK launch contract

The no-GUI basic-data build comes from `SDK/defaultscript.cs`, case 4
(`executeModBuildStep`), which EALAModStudio uses to launch BAB. From SDK as
working directory, use `tools/BinaryAssetBuilder.exe` with the absolute path
to `Mods/W3xRecovery/Data/Mod.xml`, output/input-output directories, and the
original switches `/csc:false /ls:true /osh:false /pc:true /res:true
/slowclean:true /ss:true`. Resolver paths are:

- `/art:".;.\Mods\W3xRecovery\Art;.\Mods;.\Art"`
- `/audio:".;.\Mods\W3xRecovery\Audio;.\Mods;.\Audio"`
- `/data:".;.\Mods\W3xRecovery\Data;.\Mods;.\SageXml"`

Expected four result files are under
`builtmods/mods/w3xrecovery/data/mod.{bin,manifest,imp,relo}` (Windows paths
are case-insensitive). Original and recovered runs must use identical input
and fresh derived caches. This fixture preparation did not execute the SDK;
SDK acceptance comes only from the corresponding immutable run evidence.
