# Run 40165527804 attempt 1 (overall failure)

Source `4085f0a0073a1be1f8e2ac622e6ffed5489980d9`. Raw logs are preserved in artifact.zip; validation.json reports all group results. Native groups passed both modes; XML animation bridge did not compile (C2664), so animation buffers were not accepted. SDK not run.

```text
O2-W3xNativeOracle.log: PASS W3X converter original differential fixtures=9937 comparedBytes=1589920 boundaryProbes=728; SDK not run
O2-W3xAnimationCompilerTests.log: PASS W3X animation compiler independent three-buffer layout
O2-W3xAnimationPipelineTests.log: PASS W3X animation preparation and selection
Od-W3xMeshBuilderOracle.log: PASS W3X mesh builder original differential fixtures=1296 comparedBytes=550208; original 0xCE fields and isolated 28-byte vertex component three buffers; full HandleMesh and SDK not accepted
O2-W3xTimecodedOracle.log: PASS W3X Timecoded original differential channels=3847 serializedChunks=6414 comparedBytes=1646145; Compress/Estimate/WriteOut; SDK not run
Od-W3xMeshConvertersTests.log: W3X mesh converters PASS 461 assertions (local golden/stride checks; not original DLL differential)
O2-W3xAdaptiveDeltaTests.log: PASS AdaptiveDelta 14292 checks, 4bit=27 8bit=37 null=56
O2-W3xMeshBuilderTests.log: PASS W3x mesh builder 37 assertions
Od-W3xAdaptiveDeltaTests.log: PASS AdaptiveDelta 14292 checks, 4bit=27 8bit=37 null=56
O2-animation-bridge-build.log: D:\a\NewSdk\NewSdk\recovery\src\Bridge\W3x\AnimationCompiler.cpp(17): error C2664: 'std::string msclr::interop::marshal_as<std::string,System::String^>(System::String ^const &)': cannot convert argument 1 from 'System::String ^' to 'System::String ^const &'
Od-W3xMeshSerializationTests.log: PASS W3X mesh vertex serialization platforms=0,1,2
Od-W3xNativeOracle.log: PASS W3X converter original differential fixtures=9937 comparedBytes=1589920 boundaryProbes=728; SDK not run
Od-W3xAnimationPipelineTests.log: PASS W3X animation preparation and selection
Od-W3xAdaptiveDeltaOracle.log: PASS W3X adaptive delta original differential frameFixtures=73728 channelFixtures=324 comparedBytes=13987299 nullChannels=138 serializedChunks=884; SDK not run
Od-W3xAnimationCompilerTests.log: PASS W3X animation compiler independent three-buffer layout
Od-W3xMeshBuilderTests.log: PASS W3x mesh builder 37 assertions
O2-W3xMeshBuilderOracle.log: PASS W3X mesh builder original differential fixtures=1296 comparedBytes=550208; original 0xCE fields and isolated 28-byte vertex component three buffers; full HandleMesh and SDK not accepted
Od-animation-bridge-build.log: D:\a\NewSdk\NewSdk\recovery\src\Bridge\W3x\AnimationCompiler.cpp(17): error C2664: 'std::string msclr::interop::marshal_as<std::string,System::String^>(System::String ^const &)': cannot convert argument 1 from 'System::String ^' to 'System::String ^const &'
O2-W3xMeshConvertersTests.log: W3X mesh converters PASS 461 assertions (local golden/stride checks; not original DLL differential)
Od-W3xTimecodedTests.log: PASS W3X Timecoded local source-contract checks
O2-W3xMeshSerializationTests.log: PASS W3X mesh vertex serialization platforms=0,1,2
O2-W3xTimecodedTests.log: PASS W3X Timecoded local source-contract checks
Od-W3xTimecodedOracle.log: PASS W3X Timecoded original differential channels=3847 serializedChunks=6414 comparedBytes=1646145; Compress/Estimate/WriteOut; SDK not run
O2-W3xAdaptiveDeltaOracle.log: PASS W3X adaptive delta original differential frameFixtures=73728 channelFixtures=324 comparedBytes=13987299 nullChannels=138 serializedChunks=884; SDK not run
```
