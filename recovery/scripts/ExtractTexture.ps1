param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
$work = Join-Path $env:RUNNER_TEMP ('texture-inventory-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work | Out-Null
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/texture/runs/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite evidence: $OutputDirectory" }
New-Item -ItemType Directory $OutputDirectory | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$inputFile = Join-Path $root 'SDK/tools/BinaryAssetBuilder.TextureCompiler.dll'
$expected = '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'
$actual = (Get-FileHash $inputFile -Algorithm SHA256).Hash.ToLowerInvariant()
if ($actual -ne $expected) { throw 'Original Texture DLL changed; review provenance before extracting.' }
$source = (git rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0) { throw 'Cannot identify source commit' }
dotnet build recovery/MetadataInventory -c Release -p:Platform=AnyCPU -o "$work/inventory" *> "$out/inventory-build.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/inventory-build.log"; throw 'MetadataInventory build failed' }
dotnet "$work/inventory/MetadataInventory.dll" $inputFile $out *> "$out/inventory.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/inventory.log"; throw 'Texture metadata inventory failed' }
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
$summary = Get-Content "$out/summary.json" -Raw | ConvertFrom-Json
$methods = @(Get-Content "$out/methods.json" -Raw | ConvertFrom-Json)
$types = @(Get-Content "$out/types.json" -Raw | ConvertFrom-Json)
$plugin = @($types | Where-Object name -eq 'BinaryAssetBuilder.TextureCompiler.Plugin')
if ($plugin.Count -ne 1) { throw 'Texture Plugin type inventory mismatch' }
@{
    project = 'TextureCompiler'; scope = 'original evidence and contract inventory only'
    sourceCommit = $source; baseCommit = 'b122a54abe4405516b021e425b8fb9235d914a0e'
    runId = $env:GITHUB_RUN_ID; runAttempt = $env:GITHUB_RUN_ATTEMPT
    originalSha256 = $actual; ilspyVersion = '9.1.0.7988'
    decompilerExit = $decompileExit; ilExit = $ilExit
    summary = $summary; recoveredBehaviorValidated = $false; sdkValidated = $false
} | ConvertTo-Json -Depth 10 | Set-Content "$out/extraction.json"
@($methods | Where-Object type -eq 'BinaryAssetBuilder.TextureCompiler.Plugin') | ConvertTo-Json -Depth 10 | Set-Content "$out/plugin-methods.json"
Write-Output "PASS Texture evidence: $($summary.types) types; $($summary.methods) methods; C# rendering exit=$decompileExit"

