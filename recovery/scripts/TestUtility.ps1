$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
$out = "recovery/evidence/utility/validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
$work = Join-Path $env:RUNNER_TEMP ('utility-validation-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work,$out | Out-Null
$out = (Resolve-Path $out).Path
$receipt = @{ sourceCommit=(git rev-parse HEAD).Trim(); runId=$env:GITHUB_RUN_ID; runAttempt=$env:GITHUB_RUN_ATTEMPT; status='started'; sdkValidated=$false; configurations=@(); originalSha256=(Get-FileHash SDK/tools/BinaryAssetBuilder.Utility.dll -Algorithm SHA256).Hash.ToLowerInvariant() }
$receipt.scope = 'Utility public API and behavior component differential; no full SDK compilation'
$receipt.inputs = @(Get-ChildItem SDK/builtmods -Recurse -Filter *.manifest | Sort-Object FullName | ForEach-Object { @{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/'); sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()} })
$receipt.sourceFiles = @(Get-ChildItem recovery/src/Managed/Utility,recovery/tools/UtilityIdentity -Recurse -File | Where-Object {$_.FullName -notmatch '[\\/](bin|obj)[\\/]'} | Sort-Object FullName | ForEach-Object { @{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/'); sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()} })
try {
    $original = "$work/original"
    New-Item -ItemType Directory -Force $original | Out-Null
    Copy-Item SDK/tools/*.dll $original
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    & $csc /nologo /unsafe /platform:x86 /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Utility.dll" /out:"$original/UtilityOracle.exe" (Resolve-Path recovery/tests/UtilityOracle.cs).Path *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'Oracle compilation failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/UtilityOracle.exe.config"
    $shared = "$work/fixtures"
    New-Item -ItemType Directory -Force $shared | Out-Null
    & "$original/UtilityOracle.exe" "$out/original.txt" "$root/SDK/builtmods" $shared *> "$out/original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log"; throw 'Original oracle failed' }
    Get-Content "$out/original.log"
    $baseline = (Get-FileHash "$out/original.txt" -Algorithm SHA256).Hash
    foreach ($mode in @('Debug','Release')) {
        $rebuilt = "$work/$mode"
        New-Item -ItemType Directory -Force $rebuilt | Out-Null
        # Target original DLL is never copied into these hosts.
        Get-ChildItem SDK/tools/*.dll | Where-Object Name -ne 'BinaryAssetBuilder.Utility.dll' | Copy-Item -Destination $rebuilt
        dotnet build recovery/src/Managed/Utility/BinaryAssetBuilder.Utility.csproj -c $mode -o "$work/build-$mode" --nologo *> "$out/$mode-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-build.log" -Tail 80; throw "$mode Utility build failed" }
        Copy-Item "$work/build-$mode/BinaryAssetBuilder.Utility.dll" $rebuilt
        Copy-Item "$original/UtilityOracle.exe*" $rebuilt
        & "$rebuilt/UtilityOracle.exe" "$out/$mode.txt" "$root/SDK/builtmods" $shared *> "$out/$mode.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode.log"; throw "$mode oracle failed" }
        $actual = (Get-FileHash "$out/$mode.txt" -Algorithm SHA256).Hash
        if ($actual -ne $baseline) {
            Compare-Object (Get-Content "$out/original.txt") (Get-Content "$out/$mode.txt") | Out-String -Width 500 | Set-Content "$out/$mode-difference.txt"
            Get-Content "$out/$mode-difference.txt" -First 80
            throw "$mode differs from original"
        }
        Get-Content "$out/$mode.log"
        $receipt.configurations += @{ configuration=$mode; passed=$true; records=@(Get-Content "$out/$mode.txt").Count; transcriptSha256=$actual.ToLowerInvariant(); rebuiltSha256=(Get-FileHash "$rebuilt/BinaryAssetBuilder.Utility.dll" -Algorithm SHA256).Hash.ToLowerInvariant(); withoutOriginalUtility=$true }
    }
    # Both matched transcripts are byte-identical; preserve one canonical copy.
    Compress-Archive -Path "$out/original.txt" -DestinationPath "$out/transcript.zip"
    Remove-Item "$out/original.txt", "$out/Debug.txt", "$out/Release.txt"
    $receipt.status='passed' 
} finally {
    $receipt | ConvertTo-Json -Depth 10 | Set-Content "$out/validation.json"
}
