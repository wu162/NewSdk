param([string]$OutputDirectory, [string]$TargetExe, [switch]$RequireComplete)
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
if (!$OutputDirectory) { $OutputDirectory = "recovery/evidence/refactor/g2-$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" }
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite evidence: $OutputDirectory" }
$out = (New-Item -ItemType Directory $OutputDirectory).FullName
$work = (New-Item -ItemType Directory (Join-Path ([IO.Path]::GetTempPath()) ('texture-g2-' + [Guid]::NewGuid().ToString('N')))).FullName
$receipt = [ordered]@{schemaVersion=1; sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';A='not-run';B='not-run';C='not-run';scope='Texture/OnDemandTexture/TerrainTextureAtlas component probe; not full SDK preprocessor validation'}
try {
    $original = (New-Item -ItemType Directory "$work/original").FullName
    # Oracle environment only, never copied to a product directory.
    Copy-Item SDK/tools/*.dll $original
    $hostExe = "$original/TextureProbe.exe"
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    & $csc /nologo /codepage:65001 /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$hostExe" (Resolve-Path recovery/probes/TextureRedirect/TextureProbe.cs).Path *> "$out/host-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/host-build.log"; throw 'Texture component host build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0"/></startup></configuration>' | Set-Content "$hostExe.config"
    & python recovery/probes/TextureRedirect/MakeFixture.py "$work/fixture.tga"
    if ($LASTEXITCODE -ne 0) { throw 'Texture fixture generation failed' }
    & python recovery/probes/TextureRedirect/MakeFixture.py "$work/fixture.tga.wrong-size.tga" 512
    if ($LASTEXITCODE -ne 0) { throw 'Wrong-size terrain fixture generation failed' }
    & $hostExe "$original/BinaryAssetBuilder.TextureCompiler.dll" "$work/fixture.tga" "$out/A.txt" original *> "$out/A.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/A.log"; throw 'G2 A original Texture baseline failed' }
    $receipt.A='passed'
    & dotnet build recovery/probes/TextureRedirect/Writer/TextureReferencePatch.csproj -c Release -o "$work/writer" *> "$out/writer-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/writer-build.log"; throw 'Native Texture writer build failed' }
    $rewritten = (New-Item -ItemType Directory "$work/roundtrip").FullName
    Copy-Item "$original/*" $rewritten
    Remove-Item "$rewritten/BinaryAssetBuilder.TextureCompiler.dll"
    & dotnet "$work/writer/TextureReferencePatch.dll" roundtrip "$original/BinaryAssetBuilder.TextureCompiler.dll" "$rewritten/BinaryAssetBuilder.TextureCompiler.dll" - *> "$out/B-write.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/B-write.log"; throw 'Native Texture roundtrip failed' }
    Copy-Item "$rewritten/BinaryAssetBuilder.TextureCompiler.dll.patch.json" "$out/B.patch.json"
    & dotnet "$work/writer/TextureReferencePatch.dll" roundtrip "$original/BinaryAssetBuilder.TextureCompiler.dll" "$work/B-repeat.dll" - *> "$out/B-repeat-write.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/B-repeat-write.log"; throw 'Second native Texture roundtrip failed' }
    $receipt.writerDeterministic = (Get-FileHash "$work/B-repeat.dll").Hash -eq (Get-FileHash "$rewritten/BinaryAssetBuilder.TextureCompiler.dll").Hash
    if (!$receipt.writerDeterministic) { throw 'Native Texture writer output is not deterministic for identical inputs' }
    & python recovery/probes/TextureRedirect/AuditPe.py "$original/BinaryAssetBuilder.TextureCompiler.dll" "$rewritten/BinaryAssetBuilder.TextureCompiler.dll" "$out/B.pe.json"
    if ($LASTEXITCODE -ne 0) { throw 'G2 B protected PE/native range audit failed' }
    & "$rewritten/TextureProbe.exe" "$rewritten/BinaryAssetBuilder.TextureCompiler.dll" "$work/fixture.tga" "$out/B.txt" original *> "$out/B.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/B.log"; throw 'G2 B rewritten Texture failed' }
    if ((Get-FileHash "$out/A.txt").Hash -ne (Get-FileHash "$out/B.txt").Hash) { throw 'G2 B three buffers/CustomData/errors differ from A' }
    $receipt.B='passed'
    if (!$TargetExe) {
        $receipt.status='partial-A-B-only'
        $receipt.C='blocked-final-EXE-with-complete-Core-required'
        if ($RequireComplete) { throw 'G2 C requires a final EXE exposing --texture-probe and complete compatible Core types' }
        return
    }
    $target = (Resolve-Path $TargetExe).Path
    $candidate = Split-Path $target
    # C must be a prebuilt isolated final deployment, not the oracle directory.
    foreach ($name in @('BinaryAssetBuilder.Core','BinaryAssetBuilder.Utility','EALA.Hash','EALA.Metrics')) {
        if (Test-Path "$candidate/$name.dll") { throw "G2 C contains forbidden old/forwarding business DLL: $name" }
    }
    $texture = "$candidate/BinaryAssetBuilder.TextureCompiler.dll"
    if (Test-Path $texture) { throw 'C output must start without a Texture DLL; patch from the pinned oracle' }
    & dotnet "$work/writer/TextureReferencePatch.dll" redirect "$original/BinaryAssetBuilder.TextureCompiler.dll" $texture $target *> "$out/C-write.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/C-write.log"; throw 'Texture EXE reference redirect failed' }
    Copy-Item "$texture.patch.json" "$out/C.patch.json"
    & python recovery/probes/TextureRedirect/AuditPe.py "$original/BinaryAssetBuilder.TextureCompiler.dll" $texture "$out/C.pe.json" --mode redirect
    if ($LASTEXITCODE -ne 0) { throw 'G2 C protected PE/native range audit failed' }
    & $target --texture-probe $texture "$work/fixture.tga" "$out/C.txt" exe *> "$out/C.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/C.log"; throw 'G2 C EXE-bound Texture probe failed' }
    if ((Get-FileHash "$out/A.txt").Hash -ne (Get-FileHash "$out/C.txt").Hash) { throw 'G2 C three buffers/CustomData/errors differ from A' }
    $receipt.C='passed'
    $receipt.status='component-A-B-C-and-protected-PE-audit-passed-real-SDK-pending'
} catch {
    $receipt.status='failed';$receipt.error=$_.ToString();throw
} finally {
    $receipt | ConvertTo-Json -Depth 6 | Set-Content -Encoding utf8 "$out/validation.json"
}
