$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$out = "recovery/evidence/texture/validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work = Join-Path $env:RUNNER_TEMP ('texture-validation-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work,$out | Out-Null
$out = (Resolve-Path $out).Path
$receipt = @{ sourceCommit=(git rev-parse HEAD).Trim(); runId=$env:GITHUB_RUN_ID; runAttempt=$env:GITHUB_RUN_ATTEMPT; status='started'; sdkValidated=$false; configurations=@(); originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.TextureCompiler.dll -Algorithm SHA256).Hash.ToLowerInvariant(); scope='partial recovery: metadata, image rules, native buffer, PC/PS3/unknown DDS paths only' }
if ($receipt.originalSha256 -ne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e') { throw 'Original TextureCompiler changed' }
$receipt.sourceFiles = @(Get-ChildItem src/TextureCompiler -Recurse -File | Where-Object {$_.FullName -notmatch '[\\/](bin|obj)[\\/]'} | Sort-Object FullName | ForEach-Object { @{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/'); sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()} })
function Run-Checked([string]$Label, [scriptblock]$Action) {
    & $Action *> "$out/$Label.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$Label.log" -Tail 100; throw "$Label failed ($LASTEXITCODE)" }
    Get-Content "$out/$Label.log" | Where-Object { $_ -match '^PASS' }
}
$script:failureList = @()
$receipt.groups = @()
function Invoke-Group([string]$Name, [scriptblock]$Action) {
    try {
        & $Action
        $receipt.groups += @{group=$Name; passed=$true}
    } catch {
        $script:failureList += $Name
        $receipt.groups += @{group=$Name; passed=$false; error=$_.ToString()}
        Write-Warning "$Name failed; collecting other independent component results: $_"
    }
}
function Host-Config([string]$Executable) {
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$Executable.config"
}
try {
    $original = "$work/original"
    New-Item -ItemType Directory -Force $original | Out-Null
    Copy-Item SDK/tools/*.dll $original
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    Run-Checked 'dds-oracle-build' { & $csc /nologo /platform:x86 /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /r:"$original/BinaryAssetBuilder.Utility.dll" /out:"$original/TextureDdsOracle.exe" (Resolve-Path "recovery/tests/TextureDdsOracle.cs").Path }
    Host-Config "$original/TextureDdsOracle.exe"
    Run-Checked 'original-dds' { & "$original/TextureDdsOracle.exe" "$original/BinaryAssetBuilder.TextureCompiler.dll" 'BinaryAssetBuilder.TextureCompiler.Plugin' "$out/original-dds.txt" "$work/fixtures" }
    Run-Checked 'contract-build' { & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Web.Extensions.dll /out:"$original/TexturePluginContractOracle.exe" (Resolve-Path "recovery/tests/TexturePluginContractOracle.cs").Path }
    Host-Config "$original/TexturePluginContractOracle.exe"
    Run-Checked 'contract' { & "$original/TexturePluginContractOracle.exe" "$original/BinaryAssetBuilder.TextureCompiler.dll" "$out/plugin-contract.json" }
    Run-Checked 'metadata-build' { & $csc /nologo /platform:x86 /r:System.Core.dll /out:"$original/TextureMetadataOracle.exe" (Resolve-Path "recovery/tests/TextureMetadataOracle.cs").Path }
    Host-Config "$original/TextureMetadataOracle.exe"
    $nativeOracle = (Resolve-Path recovery/tests/TextureSupportOracle.cpp).Path
    $nativeSupport = (Resolve-Path src/TextureCompiler/Native/TextureSupport.cpp).Path
    foreach ($mode in @('Debug','Release')) {
        $rebuilt = "$work/$mode"
        New-Item -ItemType Directory -Force $rebuilt | Out-Null
        Get-ChildItem SDK/tools/*.dll | Where-Object Name -ne 'BinaryAssetBuilder.TextureCompiler.dll' | Copy-Item -Destination $rebuilt
        Run-Checked "$mode-build" { dotnet build src/TextureCompiler/Managed/Recovered.TextureCompiler.csproj -c $mode -o "$work/build-$mode" --nologo }
        $dll = "$rebuilt/Recovered.BinaryAssetBuilder.TextureCompiler.dll"
        Copy-Item "$work/build-$mode/Recovered.BinaryAssetBuilder.TextureCompiler.dll" $dll
        Copy-Item "$original/TextureDdsOracle.exe*" $rebuilt
        Invoke-Group "$mode-metadata" { Run-Checked "$mode-metadata" { & "$original/TextureMetadataOracle.exe" "$original/BinaryAssetBuilder.TextureCompiler.dll" $dll } }
        $optimize = if ($mode -eq 'Release') { '/optimize+' } else { '/optimize-' }
        Invoke-Group "$mode-image" {
        Run-Checked "$mode-image-build" { & $csc /nologo /unsafe /platform:x86 $optimize /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/TextureImageRulesOracle-$mode.exe" (Resolve-Path "recovery/tests/TextureImageRulesOracle.cs").Path (Resolve-Path "src/TextureCompiler/Managed/TextureImageRules.cs").Path }
        Host-Config "$original/TextureImageRulesOracle-$mode.exe"
        Run-Checked "$mode-image" { & "$original/TextureImageRulesOracle-$mode.exe" "$original/BinaryAssetBuilder.TextureCompiler.dll" "$out/$mode-image.txt" }
        }
        if (Test-Path "$rebuilt/BinaryAssetBuilder.TextureCompiler.dll") { throw 'Original target DLL present in recovered host' }
        Invoke-Group "$mode-packer" {
        Run-Checked "$mode-packer-build" { & $csc /nologo /unsafe /platform:x86 $optimize /r:$dll /out:"$original/TexturePackerOracle-$mode.exe" (Resolve-Path recovery/tests/TexturePackerOracle.cs).Path }
        Copy-Item $dll $original -Force
        Host-Config "$original/TexturePackerOracle-$mode.exe"
        Run-Checked "$mode-packer" { & "$original/TexturePackerOracle-$mode.exe" "$original/BinaryAssetBuilder.TextureCompiler.dll" }
        }
        Invoke-Group "$mode-dds" {
        Run-Checked "$mode-dds" { & "$rebuilt/TextureDdsOracle.exe" $dll 'Recovered.Compiler.Texture.Plugin' "$out/$mode-dds.txt" "$work/fixtures" }
        if ((Get-FileHash "$out/original-dds.txt").Hash -ne (Get-FileHash "$out/$mode-dds.txt").Hash) {
            Compare-Object (Get-Content "$out/original-dds.txt") (Get-Content "$out/$mode-dds.txt") | Select-Object -First 12 | Out-String -Width 500 | Set-Content "$out/$mode-dds-difference.txt"
            Get-Content "$out/$mode-dds-difference.txt"; throw "$mode DDS differential mismatch"
        }
        }
        Push-Location $rebuilt
        try {
            $nativeOpt = if ($mode -eq 'Release') { '/O2' } else { '/Od' }
            Invoke-Group "$mode-native" {
            Run-Checked "$mode-native-build" { cl /nologo /clr /EHsc /std:c++17 $nativeOpt $nativeOracle $nativeSupport /Fe:TextureSupportOracle.exe }
            Host-Config "$rebuilt/TextureSupportOracle.exe"
            Run-Checked "$mode-native" { & "$rebuilt/TextureSupportOracle.exe" "$original/BinaryAssetBuilder.TextureCompiler.dll" }
            }
        } finally { Pop-Location }
        $receipt.configurations += @{configuration=$mode; passed=(@($script:failureList | Where-Object {$_ -like "$mode-*"}).Count -eq 0); withoutOriginalTextureCompiler=$true}
    }
    if ($script:failureList.Count -ne 0) { $receipt.status='failed'; throw ('Failed component groups: ' + ($script:failureList -join ', ')) }
    Compress-Archive -Path "$out/original-dds.txt","$out/Debug-image.txt","$out/Release-image.txt" -DestinationPath "$out/transcripts.zip"
    Remove-Item "$out/original-dds.txt","$out/Debug-dds.txt","$out/Release-dds.txt","$out/Debug-image.txt","$out/Release-image.txt"
    $receipt.status='passed'
} finally { $receipt | ConvertTo-Json -Depth 12 | Set-Content "$out/validation.json" }
