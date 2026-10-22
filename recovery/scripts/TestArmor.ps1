$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-armor'
$evidence = 'recovery/evidence/armor'
New-Item -ItemType Directory -Force $out, $evidence | Out-Null
Copy-Item SDK/tools/*.dll $out
cl /nologo /clr /LD /MD /EHa /Od /fp:precise /AI"$out" /Fo"$out/" /Fe"$out/Recovered.Armor.dll" recovery/src/Native/Relocation.cpp recovery/src/Native/ScalarParsing.cpp recovery/src/Native/ArmorTemplate.cpp recovery/src/Bridge/ArmorCompiler.cpp *> "$evidence/build.txt"
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
& $csc /nologo /unsafe /platform:x86 /r:System.Core.dll /r:"$out/Recovered.Armor.dll" /out:"$out/XmlNodeOracle.exe" (Resolve-Path 'recovery/tests/XmlNodeOracle.cs') *> "$evidence/node-host-build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/node-host-build.txt"; throw 'XML node oracle build failed' }
Copy-Item "$out/ArmorOracle.exe.config" "$out/XmlNodeOracle.exe.config"
foreach ($name in @('Tokenizer', 'XmlCompiler')) {
    & "$out/XmlNodeOracle.exe" (Resolve-Path "SDK/tools/BinaryAssetBuilder.$name.dll").Path *> "$evidence/$name-node-oracle.txt"
    $nodeCode = $LASTEXITCODE
    Get-Content "$evidence/$name-node-oracle.txt"
    @{ exitCode = $nodeCode; succeeded = ($nodeCode -eq 0); sourceCommit = $env:GITHUB_SHA } | ConvertTo-Json | Set-Content "$evidence/$name-node-result.json"
    if ($nodeCode -ne 0) { throw "$name XML node differential test failed" }
}
& $csc /nologo /unsafe /platform:x86 /r:System.Core.dll /r:"$out/Recovered.Armor.dll" /out:"$out/ScalarOracle.exe" (Resolve-Path 'recovery/tests/ScalarOracle.cs') *> "$evidence/scalar-host-build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/scalar-host-build.txt"; throw 'Scalar oracle build failed' }
Copy-Item "$out/ArmorOracle.exe.config" "$out/ScalarOracle.exe.config"
foreach ($name in @('Tokenizer', 'XmlCompiler')) {
    & "$out/ScalarOracle.exe" (Resolve-Path "SDK/tools/BinaryAssetBuilder.$name.dll").Path *> "$evidence/$name-scalar-oracle.txt"
    $scalarCode = $LASTEXITCODE
    Get-Content "$evidence/$name-scalar-oracle.txt"
    @{ exitCode = $scalarCode; succeeded = ($scalarCode -eq 0); sourceCommit = $env:GITHUB_SHA } | ConvertTo-Json | Set-Content "$evidence/$name-scalar-result.json"
    if ($scalarCode -ne 0) { throw "$name scalar differential test failed" }
}
