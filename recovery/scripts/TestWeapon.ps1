$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-weapon'
$evidence = 'recovery/evidence/weapon'
New-Item -ItemType Directory -Force $out, $evidence | Out-Null
@{ succeeded = $false; sourceCommit = $env:GITHUB_SHA; scope = 'WeaponTemplate scalar/duration/resource subset, not full type or DLL' } | ConvertTo-Json | Set-Content "$evidence/validation.json"
Copy-Item SDK/tools/*.dll $out
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
foreach ($mode in @('Od', 'O2')) {
    cl /nologo /clr /LD /MD /EHa /$mode /fp:precise /AI"$out" /Fo"$out/" /Fe"$out/Recovered.Weapon.dll" recovery/src/Native/Relocation.cpp recovery/src/Native/ScalarParsing.cpp recovery/src/Native/DataInitializers.cpp recovery/src/Native/ResourceMarshaling.cpp recovery/src/Native/WeaponTemplate.cpp recovery/src/Bridge/WeaponCompiler.cpp *> "$evidence/build-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/build-$mode.txt"; throw "Weapon C++/CLI build failed: $mode" }
    & $csc /nologo /platform:x86 /r:"$out/Recovered.Weapon.dll" /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/WeaponOracle.exe" (Resolve-Path 'recovery/tests/WeaponOracle.cs') *> "$evidence/host-build-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/host-build-$mode.txt"; throw 'Weapon oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/WeaponOracle.exe.config"
    & "$out/WeaponOracle.exe" (Resolve-Path 'SDK/tools/BinaryAssetBuilder.Tokenizer.dll') *> "$evidence/oracle-$mode.txt"
    if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/oracle-$mode.txt" -Tail 20; throw "Weapon differential failed: $mode" }
    Get-Content "$evidence/oracle-$mode.txt" -Tail 1
}
@{ succeeded = $true; sourceCommit = $env:GITHUB_SHA; scope = '89 root attributes and four RangeDuration blocks; remaining optional fields absent and lists empty'; modes = @('Od','O2') } | ConvertTo-Json | Set-Content "$evidence/validation.json"


