$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-weapon-slot-sort-probe'
$report = 'recovery/evidence/gameobject-modules/weapon-slot-sort-probe'
New-Item -ItemType Directory -Force $out,$report | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; status = 'original-only investigation; recovered WeaponSetUpdate remains refused' } | ConvertTo-Json | Set-Content "$report/validation.json"
Copy-Item SDK/tools/*.dll $out
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/WeaponSlotSortProbe.exe" (Resolve-Path 'recovery/tests/WeaponSlotSortProbe.cs') *> "$report/host.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/host.txt"; throw 'Sort probe host build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/WeaponSlotSortProbe.exe.config"
& "$out/WeaponSlotSortProbe.exe" "$out/BinaryAssetBuilder.Tokenizer.dll" (Join-Path (Get-Location) "$report/permutations.tsv") *> "$report/oracle.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$report/oracle.txt"; throw 'Sort probe failed' }
$summary = (Get-Content "$report/oracle.txt" -Tail 1)
Write-Host $summary
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; originalBlob = '6e40a9ed6c9f961d294d49768d911e6a6f354619'; summary = $summary; status = 'original-only investigation; recovered WeaponSetUpdate remains refused'; sdkStatus = 'not run; original route retained' } | ConvertTo-Json | Set-Content "$report/validation.json"
