$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-relocation'
$evidence = 'recovery/evidence/relocation'
New-Item -ItemType Directory -Force $out, $evidence | Out-Null
cl /nologo /clr /LD /MD /EHa /Od /Fo"$out/" /Fe"$out/Recovered.Relocation.dll" recovery/src/Native/Relocation.cpp recovery/src/Bridge/RelocationBridge.cpp *> "$evidence/build.txt"
if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/build.txt"; throw 'Recovered C++/CLI relocation build failed' }
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
Copy-Item SDK/tools/*.dll $out
& $csc /nologo /unsafe /platform:x86 /r:"$out/Recovered.Relocation.dll" /r:System.Core.dll /out:"$out/RelocationOracle.exe" (Resolve-Path 'recovery/tests/RelocationOracle.cs')
if ($LASTEXITCODE -ne 0) { throw 'Relocation oracle host build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/RelocationOracle.exe.config"
$results = @()
foreach ($name in @('Tokenizer', 'XmlCompiler')) {
    $original = (Resolve-Path "SDK/tools/BinaryAssetBuilder.$name.dll").Path
    & "$out/RelocationOracle.exe" $original *> "$evidence/$name-oracle.txt"
    $code = $LASTEXITCODE
    $results += @{ assembly = $name; exitCode = $code; succeeded = ($code -eq 0) }
    $results | ConvertTo-Json -AsArray | Set-Content "$evidence/result.json"
    if ($code -ne 0) { Get-Content "$evidence/$name-oracle.txt"; throw "$name relocation differential test failed" }
}
