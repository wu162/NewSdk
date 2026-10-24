$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-weapon-slot-order'
$report = 'recovery/evidence/gameobject-modules/weapon-slot-order'
New-Item -ItemType Directory -Force $out,$report | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; scope = 'ordering helper only; recovered WeaponSetUpdate remains refused' } | ConvertTo-Json | Set-Content "$report/validation.json"
$summaries = @()
foreach ($mode in @('Od','O2')) {
    cl /nologo /EHsc /MD /std:c++17 /$mode /Fo"$out/" /Fe"$out/WeaponSlotOrderTest-$mode.exe" recovery/src/Native/WeaponSlotOrder.cpp recovery/tests/WeaponSlotOrderTest.cpp *> "$report/build-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/build-$mode.txt"; throw 'WeaponSlot ordering helper build failed' }
    & "$out/WeaponSlotOrderTest-$mode.exe" recovery/evidence/gameobject-modules/weapon-slot-sort-probe/permutations.tsv *> "$report/oracle-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/oracle-$mode.txt"; throw 'WeaponSlot ordering helper differential failed' }
    $summary = (Get-Content "$report/oracle-$mode.txt" -Tail 1)
    Write-Host $summary
    $summaries += @{ mode = $mode; summary = $summary }
}
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; modes = $summaries; originalProbeSource = '72b573545f0d8b88f6057d0e245bdb6a5d419f72'; originalProbeRun = '40069703938'; originalPermutations = 720; signedKeyCases = 1024; scope = 'ordering helper only; Tracker exchanges and full WeaponSetUpdate not implemented; module remains refused'; sdkStatus = 'not run; original route retained' } | ConvertTo-Json -Depth 5 | Set-Content "$report/validation.json"
