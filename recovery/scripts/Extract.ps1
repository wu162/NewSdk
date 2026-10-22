$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
New-Item -ItemType Directory -Force recovery/logs | Out-Null
dotnet build recovery/MetadataInventory -c Release -p:Platform=AnyCPU -o recovery/toolbin/inventory
if ($LASTEXITCODE -ne 0) { throw 'Metadata reader build failed' }
$ilspy = Join-Path $root 'recovery/toolbin/ilspycmd.exe'
$targets = @('BinaryAssetBuilder.Tokenizer', 'BinaryAssetBuilder.XmlCompiler', 'EALAModStudio', 'BinaryAssetBuilder.Core', 'BinaryAssetBuilder')
foreach ($name in $targets) {
    $inputFile = if ($name -eq 'EALAModStudio') { 'SDK/EALAModStudio.exe' } elseif ($name -eq 'BinaryAssetBuilder') { 'SDK/tools/BinaryAssetBuilder.exe' } else { "SDK/tools/$name.dll" }
    if (!(Test-Path $inputFile)) { throw "Missing input: $inputFile" }
    $out = "recovery/decompiled/$name"
    New-Item -ItemType Directory -Force $out | Out-Null
    & $ilspy -p -o $out -r SDK/tools $inputFile *> "recovery/logs/$name.decompile.txt"
    $decompileExit = $LASTEXITCODE
    # IL output preserves native method declarations that C# cannot represent.
    & $ilspy -il $inputFile > "recovery/logs/$name.il"
    $ilExit = $LASTEXITCODE
    dotnet recovery/toolbin/inventory/MetadataInventory.dll $inputFile "recovery/evidence/$name"
    if ($LASTEXITCODE -ne 0) { throw "Inventory failed: $name" }
    @{ decompilerExit = $decompileExit; ilExit = $ilExit } | ConvertTo-Json | Set-Content "recovery/evidence/$name/extraction.json"
    if ($name -in @('BinaryAssetBuilder.Tokenizer', 'BinaryAssetBuilder.XmlCompiler')) {
        dumpbin /headers /imports /exports $inputFile > "recovery/evidence/$name/pe.txt"
        dumpbin /disasm:nobytes $inputFile > "recovery/logs/$name.native.asm"
    }
}
# Raw native disassembly and complete IL are large but useful as reproducible evidence.
# Keep them in artifacts; repository sources retain metadata and decompiler logs.
Get-ChildItem recovery/logs -Filter *.il | ForEach-Object { Compress-Archive -Force $_.FullName ($_.FullName + '.zip'); Remove-Item $_.FullName }
Get-ChildItem recovery/logs -Filter *.asm | ForEach-Object { Compress-Archive -Force $_.FullName ($_.FullName + '.zip'); Remove-Item $_.FullName }
