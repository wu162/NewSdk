$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$out = Join-Path $env:RUNNER_TEMP 'ra3-recovered-primitives'
New-Item -ItemType Directory -Force $out, recovery/evidence/primitives | Out-Null
cl /nologo /clr /LD /MD /EHa /Od /Fo"$out/" /Fe"$out/Recovered.Primitives.dll" recovery/src/Native/DataInitializers.cpp recovery/src/Bridge/PrimitiveInitializers.cpp
if ($LASTEXITCODE -ne 0) { throw 'Recovered C++/CLI component build failed' }
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
Copy-Item SDK/tools/*.dll $out
& $csc /nologo /target:library /r:"$out/BinaryAssetBuilder.Core.dll" /out:"$out/Recovered.Catalog.dll" (Resolve-Path "recovery/src/Managed/CompilerCatalog.cs")
if ($LASTEXITCODE -ne 0) { throw 'Recovered C# catalog build failed' }
& $csc /nologo /r:"$out/Recovered.Catalog.dll" /r:"$out/BinaryAssetBuilder.Core.dll" /r:System.Core.dll /platform:x86 /out:"$out/CatalogOracle.exe" (Resolve-Path "recovery/tests/CatalogOracle.cs")
if ($LASTEXITCODE -ne 0) { throw 'Catalog oracle host build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/CatalogOracle.exe.config"
& "$out/CatalogOracle.exe" $out *> recovery/evidence/primitives/catalog-oracle.txt
if ($LASTEXITCODE -ne 0) { Get-Content recovery/evidence/primitives/catalog-oracle.txt; throw 'Catalog differential test failed' }
& $csc /nologo /unsafe /platform:x86 /r:"$out/Recovered.Primitives.dll" /r:System.Core.dll /out:"$out/PrimitiveOracle.exe" (Resolve-Path "recovery/tests/PrimitiveOracle.cs")
if ($LASTEXITCODE -ne 0) { throw 'Oracle host build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$out/PrimitiveOracle.exe.config"
$original = (Resolve-Path 'SDK/tools/BinaryAssetBuilder.Tokenizer.dll').Path
& "$out/PrimitiveOracle.exe" $original *> recovery/evidence/primitives/oracle.txt
$code = $LASTEXITCODE
@{ exitCode = $code; succeeded = ($code -eq 0); kind = 'Original DLL vs rebuilt native+C++/CLI, float bits' } | ConvertTo-Json | Set-Content recovery/evidence/primitives/result.json
if ($code -ne 0) { Get-Content recovery/evidence/primitives/oracle.txt; throw 'Differential primitive test failed' }
