param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/w3x/native-runs/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite evidence: $OutputDirectory" }
New-Item -ItemType Directory $OutputDirectory -Force | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$originalHash = (Get-FileHash SDK/tools/BinaryAssetBuilder.W3XCompiler.dll -Algorithm SHA256).Hash.ToLowerInvariant()
if ($originalHash -ne 'bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1') { throw 'Original W3X DLL changed' }
$sourceCommit = (git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify source commit' }
$tests = @('W3xTimecodedTests', 'W3xAdaptiveDeltaTests', 'W3xMeshConvertersTests', 'W3xMeshBuilderTests', 'W3xAnimationPipelineTests', 'W3xAnimationCompilerTests', 'W3xMeshSerializationTests')
foreach ($test in $tests) { if (!(Test-Path "recovery/tests/$test.cpp")) { throw "Missing required component test $test" } }
$sources = @(Get-ChildItem recovery/src/Native/W3x -Filter '*.cpp' | ForEach-Object FullName)
if ($sources.Count -lt 5) { throw 'Expected five W3X algorithm source files' }
$sources += (Resolve-Path 'recovery/src/Native/Relocation.cpp').Path
$sources += (Resolve-Path 'recovery/src/Native/ScalarParsing.cpp').Path
$config = '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'
$results = @()
foreach ($mode in @('Od','O2')) {
    $work = Join-Path $env:RUNNER_TEMP ('w3x-native-' + $mode + '-' + [Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory $work | Out-Null
    Copy-Item SDK/tools/*.dll $work
    $objects = @()
    foreach ($source in $sources) {
        $obj = Join-Path $work ([IO.Path]::GetFileNameWithoutExtension($source) + '.obj')
        & cl.exe /nologo /c /EHsc /std:c++14 /fp:precise "/$mode" /MD "/Fo$obj" $source *> "$out/$mode-$([IO.Path]::GetFileNameWithoutExtension($source))-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$([IO.Path]::GetFileNameWithoutExtension($source))-build.log"; throw "$mode native source build failed: $source" }
        $objects += $obj
    }
    foreach ($test in $tests) {
        $exe = Join-Path $work "$test.exe"
        & cl.exe /nologo /EHsc /std:c++14 /fp:precise "/$mode" /MD "/Fe$exe" "/Fo$work/$test.obj" (Resolve-Path "recovery/tests/$test.cpp").Path $objects *> "$out/$mode-$test-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$test-build.log"; throw "$mode $test build failed" }
        & $exe *> "$out/$mode-$test.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$test.log"; throw "$mode $test failed" }
        Get-Content "$out/$mode-$test.log"
    }
    $oracle = Join-Path $work 'W3xNativeOracle.exe'
    & cl.exe /nologo /clr /EHa /fp:precise "/$mode" /MD "/Fe$oracle" "/Fo$work/W3xNativeOracle.obj" (Resolve-Path 'recovery/tests/W3xNativeOracle.cpp').Path (Join-Path $work 'MeshConverters.obj') *> "$out/$mode-oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-oracle-build.log"; throw "$mode converter oracle build failed" }
    $config | Set-Content "$oracle.config"
    & $oracle (Join-Path $work 'BinaryAssetBuilder.W3XCompiler.dll') *> "$out/$mode-oracle.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-oracle.log" | Select-Object -Last 40; throw "$mode original converter differential failed" }
    Get-Content "$out/$mode-oracle.log" | Where-Object { $_ -match '^PASS ' }
    foreach ($nativeOracleName in @('W3xAdaptiveDeltaOracle', 'W3xMeshBuilderOracle', 'W3xTimecodedOracle')) {
        $nativeOracle = Join-Path $work "$nativeOracleName.exe"
        & cl.exe /nologo /clr /EHa /fp:precise "/$mode" /MD "/Fe$nativeOracle" "/Fo$work/$nativeOracleName.obj" (Resolve-Path "recovery/tests/$nativeOracleName.cpp").Path $objects *> "$out/$mode-$nativeOracleName-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$nativeOracleName-build.log"; throw "$mode $nativeOracleName build failed" }
        $config | Set-Content "$nativeOracle.config"
        & $nativeOracle (Join-Path $work 'BinaryAssetBuilder.W3XCompiler.dll') *> "$out/$mode-$nativeOracleName.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$nativeOracleName.log" | Select-Object -Last 50; throw "$mode $nativeOracleName original differential failed" }
        Get-Content "$out/$mode-$nativeOracleName.log" | Where-Object { $_ -match '^PASS ' }
    }
    $bridge = Join-Path $work 'Recovered.W3x.Native.dll'
    & cl.exe /nologo /clr /LD /EHa /fp:precise "/$mode" /MD "/AI$work" "/Fe$bridge" "/Fo$work/AnimationBridge.obj" (Resolve-Path 'recovery/src/Bridge/W3x/AnimationCompiler.cpp').Path $objects *> "$out/$mode-animation-bridge-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-animation-bridge-build.log"; throw "$mode animation bridge build failed" }
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $animationOracle = Join-Path $work 'W3xAnimationOracle.exe'
    $optimize = if ($mode -eq 'O2') { '/optimize+' } else { '/optimize-' }
    & $csc /nologo /platform:x86 $optimize /r:System.Xml.dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.W3XCompiler.dll" /r:"$bridge" /out:"$animationOracle" (Resolve-Path 'recovery/tests/W3xAnimationOracle.cs').Path *> "$out/$mode-animation-oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-animation-oracle-build.log"; throw "$mode animation oracle build failed" }
    $config | Set-Content "$animationOracle.config"
    & $animationOracle *> "$out/$mode-animation-oracle.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-animation-oracle.log" | Select-Object -Last 50; throw "$mode original animation three-buffer differential failed" }
    Get-Content "$out/$mode-animation-oracle.log" | Where-Object { $_ -match '^PASS ' }
    $results += @{ mode=$mode; independentComponentTests=$tests; converterOriginalDifferential=$true; adaptiveDeltaCompressFrameOriginalDifferential=$true; adaptiveDeltaCompressAndEstimateOriginalDifferential=$true; meshVertexDataThreeBufferOriginalDifferential=$true; animationThreeBufferOriginalDifferential=$true }
}
@{
    scope='independent native tests; original mesh converter and AdaptiveDelta CompressFrame/Compress/Estimate IL differential; mesh vertex-data and ProcessInstance animation three-buffer differential; full mesh ProcessInstance and SDK not accepted'
    sourceCommit=$sourceCommit; runId=$env:GITHUB_RUN_ID; runAttempt=$env:GITHUB_RUN_ATTEMPT
    originalSha256=$originalHash; modes=$results
    completeAnimationBuffersValidated=$true; completeMeshBuffersValidated=$false; sdkValidated=$false
} | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json"
