$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-gameobject'
$report = 'recovery/evidence/gameobject-root'
New-Item -ItemType Directory -Force $out, $report | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; sdkStatus = 'not run; original route retained' } | ConvertTo-Json | Set-Content "$report/validation.json"
python recovery/scripts/GenerateGameObjectFields.py --check
if ($LASTEXITCODE -ne 0) { throw 'GameObject source metadata mismatch' }
Copy-Item SDK/tools/*.dll $out
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$summaries = @()
foreach ($mode in @('Od','O2')) {
    cl /nologo /clr /LD /MD /EHa /$mode /fp:precise /AI"$out" /Fo"$out/" /Fe"$out/Recovered.GameObject.dll" recovery/src/Native/Relocation.cpp recovery/src/Native/ScalarParsing.cpp recovery/src/Native/DataInitializers.cpp recovery/src/Native/ResourceMarshaling.cpp recovery/src/Native/GameObject.cpp recovery/src/Bridge/GameObjectCompiler.cpp *> "$report/build-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/build-$mode.txt"; throw "GameObject build failed: $mode" }
    & $csc /nologo /platform:x86 /r:"$out/Recovered.GameObject.dll" /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/GameObjectOracle.exe" (Resolve-Path 'recovery/tests/GameObjectOracle.cs') (Resolve-Path 'recovery/tests/GameObjectCases.cs') *> "$report/host-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/host-$mode.txt"; throw 'GameObject oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/GameObjectOracle.exe.config"
    & "$out/GameObjectOracle.exe" "$out/BinaryAssetBuilder.Tokenizer.dll" *> "$report/oracle-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/oracle-$mode.txt" -Tail 20; throw "GameObject differential failed: $mode" }
    $summary = (Get-Content "$report/oracle-$mode.txt" -Tail 1)
    $summaries += @{ mode = $mode; summary = $summary }
    Write-Host $summary
}
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; scope = '85 root attributes including Browser string list; all child blocks, masks and duration override rejected'; platforms = @(0,1,2,99); modes = $summaries; sdkStatus = 'not run; original route retained' } | ConvertTo-Json -Depth 5 | Set-Content "$report/validation.json"
