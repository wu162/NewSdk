$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-gameobject'
$report = 'recovery/evidence/gameobject-root'
New-Item -ItemType Directory -Force $out, $report | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; sdkStatus = 'not run; original route retained' } | ConvertTo-Json | Set-Content "$report/validation.json"
python recovery/scripts/GenerateGameObjectFields.py --check
if ($LASTEXITCODE -ne 0) { throw 'GameObject source metadata mismatch' }
python recovery/scripts/GenerateSoundSelectorFields.py --check
if ($LASTEXITCODE -ne 0) { throw 'Sound selector source metadata mismatch' }
python recovery/scripts/GenerateLifecycleFields.py --check
if ($LASTEXITCODE -ne 0) { throw 'Lifecycle source metadata mismatch' }
python recovery/scripts/GenerateDieFields.py --check
if ($LASTEXITCODE -ne 0) { throw 'Die source metadata mismatch' }
python recovery/scripts/GenerateUpgradeFields.py --check
if ($LASTEXITCODE -ne 0) { throw 'Upgrade source metadata mismatch' }
python recovery/scripts/GenerateUpgradeRemaining.py --check
if ($LASTEXITCODE -ne 0) { throw 'Remaining Upgrade source metadata mismatch' }
python recovery/scripts/GenerateProductionFields.py --check
if ($LASTEXITCODE -ne 0) { throw 'Production source metadata mismatch' }
Copy-Item SDK/tools/*.dll $out
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$summaries = @()
foreach ($mode in @('Od','O2')) {
    cl /nologo /clr /LD /MD /EHa /$mode /fp:precise /AI"$out" /Fo"$out/" /Fe"$out/Recovered.GameObject.dll" recovery/src/Native/Relocation.cpp recovery/src/Native/ScalarParsing.cpp recovery/src/Native/DataInitializers.cpp recovery/src/Native/ResourceMarshaling.cpp recovery/src/Native/EnumBitset.cpp recovery/src/Native/ObjectFilter.cpp recovery/src/Native/GameObject.cpp recovery/src/Bridge/GameObjectCompiler.cpp *> "$report/build-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/build-$mode.txt"; throw "GameObject build failed: $mode" }
    & $csc /nologo /platform:x86 /r:"$out/Recovered.GameObject.dll" /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/GameObjectOracle.exe" (Resolve-Path 'recovery/tests/GameObjectOracle.cs') (Resolve-Path 'recovery/tests/GameObjectCases.cs') (Resolve-Path 'recovery/tests/GameObjectKindOf.cs') *> "$report/host-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/host-$mode.txt"; throw 'GameObject oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/GameObjectOracle.exe.config"
    & "$out/GameObjectOracle.exe" "$out/BinaryAssetBuilder.Tokenizer.dll" *> "$report/oracle-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$report/oracle-$mode.txt" -Tail 20; throw "GameObject differential failed: $mode" }
    $summary = (Get-Content "$report/oracle-$mode.txt" -Tail 1)
    $summaries += @{ mode = $mode; summary = $summary }
    Write-Host $summary
}
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; runId = $env:GITHUB_RUN_ID; scope = '88 root attributes; thirty-eight child nodes including module containers with 31 complete base-only types plus all seven remaining Body types (all nine XSD Body choices), full DamageRangeModifier and nullable ShieldArmor; all six ClientUpdate types including exact-case Key hash lists and ushort UnitWeight; complete three-type Production Update/exit family and twenty-two-type named Upgrade group and seven-type shared Die family and six lifecycle Updates and four sound/EVA selectors and AnimationSound/ModelConditionAudioLoop playback groups (nullable masks/range, signed Frame list and inline masks); unknown fallback (234 other known module types rejected); complete empty ReplaceModule/InheritableModule and complete SkirmishAIInformation dependency group (64 fields, contextual power lists, base criteria, nullable references and filters); AudioArrayVoice (all three lists), AudioArraySound and EvaEventArray; complete raw Geometry with PlacementShape/Shape/ContactPoint lists and Vector2/Vector3; CrusherInfo/ExtraCrushLevels and repeated ProjectedBuildabilityInfo with nullable RadiusY, all masks and AllowedObjectFilter; GameDependency masks/typed lists/ObjectFilter; ObjectResourceInfo/BuildCost (pointer StringLists on XSD-valid text surface); AutoResolve family with nested upgrade lists including visual metadata, leaf structures, repeated lists, ArmorSet and LocomotorSet; other children rejected'; platforms = @(0,1,2,99); modes = $summaries; sdkStatus = 'not run; original route retained' } | ConvertTo-Json -Depth 5 | Set-Content "$report/validation.json"



