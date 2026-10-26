$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out = "recovery/evidence/audio"
$work = Join-Path $env:RUNNER_TEMP 'audio-ilspy'
New-Item -ItemType Directory -Force $out | Out-Null
dotnet tool install ilspycmd --tool-path $work --version 9.1.0.7988
if ($LASTEXITCODE -ne 0) { throw 'ILSpy install failed' }
& "$work/ilspycmd.exe" -p -o "$out/decompiled" -r "$root/SDK/tools" "$root/SDK/tools/BinaryAssetBuilder.AudioCompiler.dll"
if ($LASTEXITCODE -ne 0) { throw 'Decompilation failed' }
& "$work/ilspycmd.exe" -il "$root/SDK/tools/BinaryAssetBuilder.AudioCompiler.dll" > "$out/AudioCompiler-original.il"
if ($LASTEXITCODE -ne 0) { throw 'IL extraction failed' }
dotnet run --project recovery/MetadataInventory -- "$root/SDK/tools/BinaryAssetBuilder.AudioCompiler.dll" "$out/metadata"
if ($LASTEXITCODE -ne 0) { throw 'Inventory failed' }
@{sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.AudioCompiler.dll).Hash;status='Evidence only; not a recovered implementation'} | ConvertTo-Json | Set-Content "$out/extraction.json"
