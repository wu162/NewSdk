param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
$work = Join-Path $env:RUNNER_TEMP ('w3x-inventory-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work | Out-Null
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/w3x/runs/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite evidence: $OutputDirectory" }
New-Item -ItemType Directory $OutputDirectory | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$inputFile = Join-Path $root 'SDK/tools/BinaryAssetBuilder.W3XCompiler.dll'
$expected = 'bff82309b8e5e9f689112202e3613ad335148934c92dc70fe143f6e60d9ad3f1'
$actual = (Get-FileHash $inputFile -Algorithm SHA256).Hash.ToLowerInvariant()
if ($actual -ne $expected) { throw 'Original W3X DLL changed; review provenance before extracting.' }
$source = (git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify source commit' }
dotnet build recovery/MetadataInventory -c Release -p:Platform=AnyCPU -o "$work/inventory" *> "$out/inventory-build.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/inventory-build.log"; throw 'MetadataInventory build failed' }
dotnet "$work/inventory/MetadataInventory.dll" $inputFile $out *> "$out/inventory.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/inventory.log"; throw 'W3X metadata inventory failed' }
$ilspy = Join-Path $work 'ilspycmd.exe'
dotnet tool install ilspycmd --tool-path $work --version 9.1.0.7988 *> "$out/ilspy-install.log"
if ($LASTEXITCODE -ne 0) { throw 'Pinned decompiler installation failed' }
& $ilspy -p -o "$out/decompiled" -r "$root/SDK/tools" $inputFile *> "$out/decompile.log"
$decompileExit = $LASTEXITCODE
# C# rendering can fail on mixed-mode bodies; retain exit status and raw IL/native evidence.
& $ilspy -il $inputFile > "$work/original.il" 2> "$out/il.log"
$ilExit = $LASTEXITCODE
if ($ilExit -ne 0) { throw 'Complete IL extraction failed' }
dumpbin /headers /imports /exports $inputFile > "$out/pe.txt"
if ($LASTEXITCODE -ne 0) { throw 'PE inspection failed' }
dumpbin /disasm:nobytes $inputFile > "$work/original.native.asm"
if ($LASTEXITCODE -ne 0) { throw 'Native disassembly failed' }
Compress-Archive -Path "$work/original.il", "$work/original.native.asm" -DestinationPath "$out/raw-listings.zip"
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /out:"$work/W3xPluginContractOracle.exe" (Resolve-Path 'recovery/tests/W3xPluginContractOracle.cs').Path *> "$out/oracle-build.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'W3X contract oracle build failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$work/W3xPluginContractOracle.exe.config"
& "$work/W3xPluginContractOracle.exe" $inputFile "$out/plugin-contract.json" *> "$out/oracle.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle.log"; throw 'Original plugin contract inventory failed' }
$summary = Get-Content "$out/summary.json" -Raw | ConvertFrom-Json
$methods = @(Get-Content "$out/methods.json" -Raw | ConvertFrom-Json)
$types = @(Get-Content "$out/types.json" -Raw | ConvertFrom-Json)
$plugin = @($types | Where-Object name -eq 'BinaryAssetBuilder.W3XCompiler.Plugin')
if ($plugin.Count -ne 1) { throw 'W3X Plugin type inventory mismatch' }
@{
    project = 'W3XCompiler'; scope = 'original evidence and contract inventory only'
    sourceCommit = $source; baseCommit = 'c66339e49e32cfe3f44aa2f9f1757f9993bbac7a'
    runId = $env:GITHUB_RUN_ID; runAttempt = $env:GITHUB_RUN_ATTEMPT
    originalSha256 = $actual; ilspyVersion = '9.1.0.7988'
    decompilerExit = $decompileExit; ilExit = $ilExit
    summary = $summary; recoveredBehaviorValidated = $false; sdkValidated = $false
} | ConvertTo-Json -Depth 10 | Set-Content "$out/extraction.json"
@($methods | Where-Object type -eq 'BinaryAssetBuilder.W3XCompiler.Plugin') | ConvertTo-Json -Depth 10 | Set-Content "$out/plugin-methods.json"
Write-Output "PASS W3X evidence: $($summary.types) types; $($summary.methods) methods; C# rendering exit=$decompileExit"
Get-Content "$out/oracle.log"
