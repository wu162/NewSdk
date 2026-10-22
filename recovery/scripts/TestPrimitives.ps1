$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-primitives'
New-Item -ItemType Directory -Force $out, recovery/evidence/primitives | Out-Null
cl /nologo /clr /LD /MD /EHa /Od /Fo"$out/" /Fe"$out/Recovered.Primitives.dll" recovery/src/Native/DataInitializers.cpp recovery/src/Bridge/PrimitiveInitializers.cpp
if ($LASTEXITCODE -ne 0) { throw 'Recovered C++/CLI component build failed' }
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /unsafe /platform:x86 /r:"$out/Recovered.Primitives.dll" /r:System.Core.dll /out:"$out/PrimitiveOracle.exe" recovery/tests/PrimitiveOracle.cs
if ($LASTEXITCODE -ne 0) { throw 'Oracle host build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/PrimitiveOracle.exe.config"
$original = (Resolve-Path 'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').Path
& "$out/PrimitiveOracle.exe" $original *> recovery/evidence/primitives/oracle.txt
$code = $LASTEXITCODE
@{ exitCode = $code; succeeded = ($code -eq 0); kind = 'Original DLL vs rebuilt native+C++/CLI, float bits' } | ConvertTo-Json | Set-Content recovery/evidence/primitives/result.json
if ($code -ne 0) { Get-Content recovery/evidence/primitives/oracle.txt; throw 'Differential primitive test failed' }
