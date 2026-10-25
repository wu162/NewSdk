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
$tests = @('W3xTimecodedTests', 'W3xAdaptiveDeltaTests', 'W3xMeshConvertersTests', 'W3xMeshBuilderTests', 'W3xAnimationPipelineTests', 'W3xAnimationCompilerTests', 'W3xMeshSerializationTests', 'W3xShaderTests', 'W3xMeshAssetTests')
foreach ($test in $tests) { if (!(Test-Path "recovery/tests/$test.cpp")) { throw "Missing required component test $test" } }
$sources = @(Get-ChildItem recovery/src/Native/W3x -Filter '*.cpp' | ForEach-Object FullName)
if ($sources.Count -lt 5) { throw 'Expected five W3X algorithm source files' }
$sources += (Resolve-Path 'recovery/src/Native/Relocation.cpp').Path
$sources += (Resolve-Path 'recovery/src/Native/ScalarParsing.cpp').Path
$sources += (Resolve-Path 'recovery/src/Native/ResourceMarshaling.cpp').Path
$config = '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'
$results = @()
$script:failures = [Collections.Generic.List[string]]::new()
$script:groupResults = @{}
function Invoke-ComponentGroup([string]$Label, [scriptblock]$Body) {
    try {
        & $Body | ForEach-Object { Write-Host $_ }
        $script:groupResults[$Label] = $true
    } catch {
        $message = "$Label failed: $($_.Exception.Message)"
        $script:failures.Add($message)
        $script:groupResults[$Label] = $false
        Write-Host $message
    }
}
foreach ($mode in @('Od','O2')) {
    $work = Join-Path $env:RUNNER_TEMP ('w3x-native-' + $mode + '-' + [Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory $work | Out-Null
    Copy-Item SDK/tools/*.dll $work
    $objects = [Collections.Generic.List[string]]::new()
    Invoke-ComponentGroup "$mode/native-sources" {
        foreach ($source in $sources) {
            $obj = Join-Path $work ([IO.Path]::GetFileNameWithoutExtension($source) + '.obj')
            & cl.exe /nologo /c /EHsc /std:c++14 /fp:precise "/$mode" /MD "/Fo$obj" $source *> "$out/$mode-$([IO.Path]::GetFileNameWithoutExtension($source))-build.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$([IO.Path]::GetFileNameWithoutExtension($source))-build.log"; throw "native source build failed: $source" }
            $objects.Add($obj)
        }
    }
    if (!$script:groupResults["$mode/native-sources"]) {
        $results += @{ mode=$mode; sourceBuildSucceeded=$false; dependentGroupsSkipped='native source build failed' }
        continue
    }
    foreach ($test in $tests) {
        Invoke-ComponentGroup "$mode/$test" {
            $exe = Join-Path $work "$test.exe"
            & cl.exe /nologo /EHsc /std:c++14 /fp:precise "/$mode" /MD "/Fe$exe" "/Fo$work/$test.obj" (Resolve-Path "recovery/tests/$test.cpp").Path $objects.ToArray() *> "$out/$mode-$test-build.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$test-build.log"; throw "$test build failed" }
            & $exe *> "$out/$mode-$test.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$test.log"; throw "$test failed" }
            Get-Content "$out/$mode-$test.log"
        }
    }
    foreach ($nativeOracleName in @('W3xNativeOracle', 'W3xAdaptiveDeltaOracle', 'W3xMeshBuilderOracle', 'W3xTimecodedOracle')) {
        Invoke-ComponentGroup "$mode/$nativeOracleName" {
            $nativeOracle = Join-Path $work "$nativeOracleName.exe"
            & cl.exe /nologo /clr /EHa /fp:precise "/$mode" /MD "/Fe$nativeOracle" "/Fo$work/$nativeOracleName.obj" (Resolve-Path "recovery/tests/$nativeOracleName.cpp").Path $objects.ToArray() *> "$out/$mode-$nativeOracleName-build.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$nativeOracleName-build.log"; throw "$nativeOracleName build failed" }
            $config | Set-Content "$nativeOracle.config"
            & $nativeOracle (Join-Path $work 'BinaryAssetBuilder.W3XCompiler.dll') *> "$out/$mode-$nativeOracleName.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$nativeOracleName.log" | Select-Object -Last 50; throw "$nativeOracleName original differential failed" }
            Get-Content "$out/$mode-$nativeOracleName.log" | Where-Object { $_ -match '^PASS ' }
        }
    }
    $bridge = Join-Path $work 'Recovered.W3x.Native.dll'
    Invoke-ComponentGroup "$mode/animation-bridge" {
        & cl.exe /nologo /clr /LD /EHa /fp:precise "/$mode" /MD "/AI$work" "/Fe$bridge" "/Fo$work/AnimationBridge.obj" (Resolve-Path 'recovery/src/Bridge/W3x/AnimationCompiler.cpp').Path $objects.ToArray() *> "$out/$mode-animation-bridge-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-animation-bridge-build.log"; throw 'animation bridge build failed' }
    }
    if ($script:groupResults["$mode/animation-bridge"]) {
        Invoke-ComponentGroup "$mode/W3xAnimationOracle" {
            $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
            $animationOracle = Join-Path $work 'W3xAnimationOracle.exe'
            $optimize = if ($mode -eq 'O2') { '/optimize+' } else { '/optimize-' }
            & $csc /nologo /platform:x86 $optimize /r:System.Xml.dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.W3XCompiler.dll" /r:"$bridge" /out:"$animationOracle" (Resolve-Path 'recovery/tests/W3xAnimationOracle.cs').Path *> "$out/$mode-animation-oracle-build.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-animation-oracle-build.log"; throw 'animation oracle build failed' }
            $config | Set-Content "$animationOracle.config"
            & $animationOracle "$out/$mode-animation.fixtures" *> "$out/$mode-animation-oracle.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-animation-oracle.log" | Select-Object -Last 50; throw 'original animation three-buffer differential failed' }
            Get-Content "$out/$mode-animation-oracle.log" | Where-Object { $_ -match '^PASS ' }
        }
    } else { $script:groupResults["$mode/W3xAnimationOracle"] = $false }
    $meshBridge = Join-Path $work 'Recovered.W3x.Mesh.dll'
    Invoke-ComponentGroup "$mode/mesh-bridge" {
        & cl.exe /nologo /clr /LD /EHa /fp:precise "/$mode" /MD "/AI$work" "/Fe$meshBridge" "/Fo$work/MeshBridge.obj" (Resolve-Path 'recovery/src/Bridge/W3x/MeshCompiler.cpp').Path $objects.ToArray() *> "$out/$mode-mesh-bridge-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-mesh-bridge-build.log"; throw 'mesh bridge build failed' }
    }
    if ($script:groupResults["$mode/mesh-bridge"]) {
        Invoke-ComponentGroup "$mode/W3xMeshAssetOracle" {
            $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
            $meshOracle = Join-Path $work 'W3xMeshAssetOracle.exe'
            $optimize = if ($mode -eq 'O2') { '/optimize+' } else { '/optimize-' }
            & $csc /nologo /platform:x86 $optimize /r:System.Xml.dll /r:"$work/BinaryAssetBuilder.Core.dll" /r:"$work/BinaryAssetBuilder.W3XCompiler.dll" /r:"$meshBridge" /out:"$meshOracle" (Resolve-Path 'recovery/tests/W3xMeshAssetOracle.cs').Path *> "$out/$mode-mesh-oracle-build.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-mesh-oracle-build.log"; throw 'mesh oracle build failed' }
            $config | Set-Content "$meshOracle.config"
            & $meshOracle "$out/$mode-mesh.fixtures" *> "$out/$mode-mesh-oracle.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-mesh-oracle.log" | Select-Object -Last 50; throw 'original full mesh three-buffer differential failed' }
            Get-Content "$out/$mode-mesh-oracle.log" | Where-Object { $_ -match '^PASS ' }
        }
    } else { $script:groupResults["$mode/W3xMeshAssetOracle"] = $false }
    if ($script:groupResults["$mode/W3xAnimationOracle"] -and $script:groupResults["$mode/W3xMeshAssetOracle"]) {
        Invoke-ComponentGroup "$mode/W3xStandaloneReplay" {
            $standalone = Join-Path $work 'without-original'
            New-Item -ItemType Directory $standalone | Out-Null
            Get-ChildItem $work -Filter '*.dll' | Where-Object Name -ne 'BinaryAssetBuilder.W3XCompiler.dll' | Copy-Item -Destination $standalone
            if (Test-Path (Join-Path $standalone 'BinaryAssetBuilder.W3XCompiler.dll')) { throw 'Original W3X DLL unexpectedly present in replay directory' }
            $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
            $replay = Join-Path $standalone 'W3xStandaloneReplay.exe'
            $optimize = if ($mode -eq 'O2') { '/optimize+' } else { '/optimize-' }
            & $csc /nologo /platform:x86 $optimize /r:System.Xml.dll /r:"$standalone/BinaryAssetBuilder.Core.dll" /r:"$standalone/Recovered.W3x.Native.dll" /r:"$standalone/Recovered.W3x.Mesh.dll" /out:"$replay" (Resolve-Path 'recovery/tests/W3xStandaloneReplay.cs').Path *> "$out/$mode-standalone-build.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-standalone-build.log"; throw 'standalone replay build failed' }
            $config | Set-Content "$replay.config"
            & $replay "$out/$mode-animation.fixtures" "$out/$mode-mesh.fixtures" *> "$out/$mode-standalone.log"
            if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-standalone.log" | Select-Object -Last 50; throw 'component replay without original W3X DLL failed' }
            Get-Content "$out/$mode-standalone.log"
        }
    } else { $script:groupResults["$mode/W3xStandaloneReplay"] = $false }
    $independent = @{}
    foreach ($test in $tests) { $independent[$test] = $script:groupResults["$mode/$test"] }
    $results += @{
        mode=$mode; sourceBuildSucceeded=$true; independentComponentTests=$independent
        converterOriginalDifferential=$script:groupResults["$mode/W3xNativeOracle"]
        adaptiveDeltaCompressFrameOriginalDifferential=$script:groupResults["$mode/W3xAdaptiveDeltaOracle"]
        adaptiveDeltaCompressAndEstimateOriginalDifferential=$script:groupResults["$mode/W3xAdaptiveDeltaOracle"]
        timecodedCompressEstimateWriteOutOriginalDifferential=$script:groupResults["$mode/W3xTimecodedOracle"]
        meshVertexDataThreeBufferOriginalDifferential=$script:groupResults["$mode/W3xMeshBuilderOracle"]
        animationThreeBufferOriginalDifferential=$script:groupResults["$mode/W3xAnimationOracle"]
        completeMeshThreeBufferOriginalDifferential=$script:groupResults["$mode/W3xMeshAssetOracle"]
        componentWithoutOriginalReplay=$script:groupResults["$mode/W3xStandaloneReplay"]
    }
}
$allSucceeded = $script:failures.Count -eq 0
@{
    succeeded=$allSucceeded
    scope='independent native tests; original mesh converter, AdaptiveDelta and Timecoded IL differential; mesh vertex-data and complete mesh/animation ProcessInstance three-buffer differential; SDK not accepted'
    sourceCommit=$sourceCommit; runId=$env:GITHUB_RUN_ID; runAttempt=$env:GITHUB_RUN_ATTEMPT
    originalSha256=$originalHash; modes=$results; failures=$script:failures.ToArray()
    completeAnimationBuffersValidated=($script:groupResults['Od/W3xAnimationOracle'] -eq $true -and $script:groupResults['O2/W3xAnimationOracle'] -eq $true)
    completeMeshBuffersValidated=($script:groupResults['Od/W3xMeshAssetOracle'] -eq $true -and $script:groupResults['O2/W3xMeshAssetOracle'] -eq $true)
    componentWithoutOriginalReplay=($script:groupResults['Od/W3xStandaloneReplay'] -eq $true -and $script:groupResults['O2/W3xStandaloneReplay'] -eq $true)
    sdkValidated=$false
} | ConvertTo-Json -Depth 8 | Set-Content "$out/validation.json"
if (!$allSucceeded) { throw "W3X component matrix failed in $($script:failures.Count) groups; all independently runnable groups were attempted, see validation.json and logs" }
