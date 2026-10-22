$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-armor'
$evidence = 'recovery/evidence/armor'
New-Item -ItemType Directory -Force $out, $evidence | Out-Null
Copy-Item SDK/tools/*.dll $out
cl /nologo /clr /LD /MD /EHa /Od /fp:strict /AI"$out" /Fo"$out/" /Fe"$out/Recovered.Armor.dll" recovery/src/Native/Relocation.cpp recovery/src/Native/ArmorTemplate.cpp recovery/src/Bridge/ArmorCompiler.cpp *> "$evidence/build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/build.txt"; throw 'Armor C++/CLI build failed' }
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:"$out/Recovered.Armor.dll" /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/ArmorOracle.exe" (Resolve-Path 'recovery/tests/ArmorOracle.cs') *> "$evidence/host-build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/host-build.txt"; throw 'Armor oracle build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/ArmorOracle.exe.config"
& "$out/ArmorOracle.exe" (Resolve-Path 'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').Path *> "$evidence/oracle.txt"
$code = $LASTEXITCODE
@{ exitCode = $code; succeeded = ($code -eq 0); sourceCommit = $env:GITHUB_SHA; scope = 'ArmorTemplate full ProcessInstance buffers; not replacement DLLs' } | ConvertTo-Json | Set-Content "$evidence/result.json"
Get-Content "$evidence/oracle.txt"
if ($code -ne 0) { throw 'Armor ProcessInstance differential test failed' }
