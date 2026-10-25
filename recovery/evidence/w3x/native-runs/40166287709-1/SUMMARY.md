# Complete component and original-absent replay success

Exact original artifact, raw logs and four binary fixture captures are preserved in artifact.zip. Source/run/archive/capture hashes are in execution.json.

```text
O2-animation-oracle.log: PASS W3X animation original differential fixtures=675 threeBufferComparisons=2025; platforms=0,1,2,99; SDK not run
O2-mesh-oracle.log: PASS W3X full mesh original differential fixtures=651 rawMarshalerFixtures=15 threeBufferComparisons=1998; platforms=0,1,2; SDK not run
O2-standalone.log: PASS W3X standalone route=animation fixtures=675
O2-standalone.log: PASS W3X standalone route=mesh fixtures=666
O2-standalone.log: PASS W3X standalone replay fixtures=1341 threeBufferComparisons=4023 comparedBytes=540216; routes=animation,mesh; originalW3XLoaded=false; SDK not run
O2-W3xAdaptiveDeltaOracle.log: PASS W3X adaptive delta original differential frameFixtures=73728 channelFixtures=324 comparedBytes=13987299 nullChannels=138 serializedChunks=884; SDK not run
O2-W3xAdaptiveDeltaTests.log: PASS AdaptiveDelta 14292 checks, 4bit=27 8bit=37 null=56
O2-W3xAnimationCompilerTests.log: PASS W3X animation compiler independent three-buffer layout
O2-W3xAnimationPipelineTests.log: PASS W3X animation preparation and selection
O2-W3xMeshAssetTests.log: PASS W3x mesh asset 171 assertions; three target platforms
O2-W3xMeshBuilderOracle.log: PASS W3X mesh builder original differential fixtures=1296 comparedBytes=550208; original 0xCE fields and isolated 28-byte vertex component three buffers; full HandleMesh and SDK not accepted
O2-W3xMeshBuilderTests.log: PASS W3x mesh builder 37 assertions
O2-W3xMeshConvertersTests.log: W3X mesh converters PASS 461 assertions (local golden/stride checks; not original DLL differential)
O2-W3xMeshSerializationTests.log: PASS W3X mesh vertex serialization platforms=0,1,2
O2-W3xNativeOracle.log: PASS W3X converter original differential fixtures=9947 comparedBytes=1591520 boundaryProbes=728; SDK not run
O2-W3xShaderTests.log: PASS W3X Shader independent layout checks
O2-W3xTimecodedOracle.log: PASS W3X Timecoded original differential channels=3847 serializedChunks=6414 comparedBytes=1646145; Compress/Estimate/WriteOut; SDK not run
O2-W3xTimecodedTests.log: PASS W3X Timecoded local source-contract checks
Od-animation-oracle.log: PASS W3X animation original differential fixtures=675 threeBufferComparisons=2025; platforms=0,1,2,99; SDK not run
Od-mesh-oracle.log: PASS W3X full mesh original differential fixtures=651 rawMarshalerFixtures=15 threeBufferComparisons=1998; platforms=0,1,2; SDK not run
Od-standalone.log: PASS W3X standalone route=animation fixtures=675
Od-standalone.log: PASS W3X standalone route=mesh fixtures=666
Od-standalone.log: PASS W3X standalone replay fixtures=1341 threeBufferComparisons=4023 comparedBytes=540216; routes=animation,mesh; originalW3XLoaded=false; SDK not run
Od-W3xAdaptiveDeltaOracle.log: PASS W3X adaptive delta original differential frameFixtures=73728 channelFixtures=324 comparedBytes=13987299 nullChannels=138 serializedChunks=884; SDK not run
Od-W3xAdaptiveDeltaTests.log: PASS AdaptiveDelta 14292 checks, 4bit=27 8bit=37 null=56
Od-W3xAnimationCompilerTests.log: PASS W3X animation compiler independent three-buffer layout
Od-W3xAnimationPipelineTests.log: PASS W3X animation preparation and selection
Od-W3xMeshAssetTests.log: PASS W3x mesh asset 171 assertions; three target platforms
Od-W3xMeshBuilderOracle.log: PASS W3X mesh builder original differential fixtures=1296 comparedBytes=550208; original 0xCE fields and isolated 28-byte vertex component three buffers; full HandleMesh and SDK not accepted
Od-W3xMeshBuilderTests.log: PASS W3x mesh builder 37 assertions
Od-W3xMeshConvertersTests.log: W3X mesh converters PASS 461 assertions (local golden/stride checks; not original DLL differential)
Od-W3xNativeOracle.log: PASS W3X converter original differential fixtures=9947 comparedBytes=1591520 boundaryProbes=728; SDK not run
Od-W3xMeshSerializationTests.log: PASS W3X mesh vertex serialization platforms=0,1,2
Od-W3xShaderTests.log: PASS W3X Shader independent layout checks
Od-W3xTimecodedOracle.log: PASS W3X Timecoded original differential channels=3847 serializedChunks=6414 comparedBytes=1646145; Compress/Estimate/WriteOut; SDK not run
Od-W3xTimecodedTests.log: PASS W3X Timecoded local source-contract checks
```
