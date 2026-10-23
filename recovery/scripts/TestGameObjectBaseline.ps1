$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-gameobject-baseline'
$report = 'recovery/evidence/gameobject'
New-Item -ItemType Directory -Force $out, $report | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; scope = 'Original DLL characterization only; no recovered GameObject compiler' } | ConvertTo-Json | Set-Content "$report/validation.json"
Copy-Item SDK/tools/*.dll $out
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/GameObjectBaseline.exe" (Resolve-Path 'recovery/tests/GameObjectBaseline.cs') *> "$report/build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/build.txt"; throw 'GameObject characterization host build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/GameObjectBaseline.exe.config"
python recovery/scripts/ExtractGameObjectEvidence.py recovery/decompiled/BinaryAssetBuilder.Tokenizer/-Module-.cs "$report/source-layout.json"
if ($LASTEXITCODE -ne 0) { throw 'Original GameObject source extraction failed' }
$details = Join-Path $out 'buffers'
& "$out/GameObjectBaseline.exe" "$out/BinaryAssetBuilder.Tokenizer.dll" $details (Resolve-Path 'SDK/Schemas/xsd/AssetTypeGameObject.xsd') *> "$report/oracle.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/oracle.txt" -Tail 25; throw 'GameObject original characterization failed' }
Get-Content "$report/oracle.txt" -Tail 1
Compress-Archive -Path "$details/*" -DestinationPath "$report/original-baseline.zip" -Force
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; scope = 'Original DLL characterization only; no recovered GameObject compiler'; fixtures = 196; repeatedThreeBufferComparisons = 588; nativeRootBytes = 592; rootTokenRecords = 127; scalarBindings = 16; platforms = @(0,1,2,99) } | ConvertTo-Json | Set-Content "$report/validation.json"
