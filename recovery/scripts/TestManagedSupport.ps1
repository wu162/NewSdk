param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/managed-support/validation/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite validation: $OutputDirectory" }
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$work = Join-Path $env:RUNNER_TEMP ('managed-support-tests-' + [Guid]::NewGuid().ToString('N'))
$original = Join-Path $work 'original'
New-Item -ItemType Directory -Force $original | Out-Null
Copy-Item SDK/tools/*.dll $original
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:System.Web.Extensions.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/ManagedSupportOracle.exe" (Resolve-Path 'recovery/tests/ManagedSupportOracle.cs').Path *> "$out/oracle-build.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'Oracle compilation failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/ManagedSupportOracle.exe.config"
& "$original/ManagedSupportOracle.exe" "$out/original.json" *> "$out/original.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log"; throw 'Original oracle failed' }
Get-Content "$out/original.log"
$baseline = (Get-FileHash "$out/original.json" -Algorithm SHA256).Hash
$results = @()
foreach ($mode in @('Debug', 'Release')) {
    $rebuilt = Join-Path $work $mode
    New-Item -ItemType Directory -Force $rebuilt | Out-Null
    # Original versions of the two target assemblies never enter this directory.
    Get-ChildItem SDK/tools -Filter *.dll | Where-Object Name -notin @('BinaryAssetBuilder.ExpressionEval.dll', 'BinaryAssetBuilder.GameDataVerifier.dll') | Copy-Item -Destination $rebuilt
    $libraries = @()
    foreach ($name in @('ExpressionEval', 'GameDataVerifier')) {
        $project = "recovery/src/Managed/$name/BinaryAssetBuilder.$name.csproj"
        $build = Join-Path $work "build-$mode-$name"
        dotnet build $project -c $mode -p:PlatformTarget=x86 -o $build --nologo *> "$out/$mode-$name-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$name-build.log"; throw "$mode $name build failed" }
        $dll = Join-Path $build "BinaryAssetBuilder.$name.dll"
        Copy-Item $dll $rebuilt
        $libraries += @{ assembly = $name; sha256 = (Get-FileHash $dll -Algorithm SHA256).Hash.ToLowerInvariant() }
    }
    Copy-Item "$original/ManagedSupportOracle.exe*" $rebuilt
    & "$rebuilt/ManagedSupportOracle.exe" "$out/$mode.json" *> "$out/$mode.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode.log"; throw "$mode oracle failed" }
    if ((Get-FileHash "$out/$mode.json" -Algorithm SHA256).Hash -ne $baseline) {
        $a = Get-Content "$out/original.json" -Raw | ConvertFrom-Json
        $b = Get-Content "$out/$mode.json" -Raw | ConvertFrom-Json
        for ($i = 0; $i -lt [Math]::Max($a.Count, $b.Count); $i++) {
            if (($a[$i] | ConvertTo-Json -Compress) -cne ($b[$i] | ConvertTo-Json -Compress)) {
                Write-Output "First difference at row $i"
                $a[$i] | ConvertTo-Json
                $b[$i] | ConvertTo-Json
                break
            }
        }
        throw "$mode differs from original"
    }
    Get-Content "$out/$mode.log"
    $results += @{ configuration = $mode; succeeded = $true; withoutOriginalTargetDlls = $true; transcriptSha256 = $baseline.ToLowerInvariant(); libraries = $libraries }
}
$inputs = @('SDK/tools/BinaryAssetBuilder.ExpressionEval.dll', 'SDK/tools/BinaryAssetBuilder.GameDataVerifier.dll', 'SDK/tools/BinaryAssetBuilder.Core.dll', 'SDK/tools/antlr.runtime.dll')
$fingerprints = @($inputs | ForEach-Object { @{ path = $_; sha256 = (Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant() } })
$sourceFiles = @(Get-ChildItem recovery/src/Managed/ExpressionEval,recovery/src/Managed/GameDataVerifier -Recurse -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
$sourceFingerprints = @($sourceFiles | Sort-Object FullName | ForEach-Object { @{ path = [IO.Path]::GetRelativePath($root, $_.FullName).Replace('\', '/'); sha256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() } })
@{
    sourceCommit = (git rev-parse HEAD).Trim(); runId = $env:GITHUB_RUN_ID; runAttempt = $env:GITHUB_RUN_ATTEMPT
    scope = 'Complete managed support source; API/member inventory and behavior differential in isolated processes'
    casesPerConfiguration = @(Get-Content "$out/original.json" -Raw | ConvertFrom-Json).Count
    inputs = $fingerprints; sourceFiles = $sourceFingerprints; configurations = $results
    expressionCultures = @('en-US', 'de-DE', 'tr-TR'); verifierPlatforms = @(0, 1, 2, 99)
    sdkValidated = $false
} | ConvertTo-Json -Depth 10 | Set-Content "$out/validation.json"
Compress-Archive -Path "$out/original.json", "$out/Debug.json", "$out/Release.json" -DestinationPath "$out/transcripts.zip"
Remove-Item "$out/original.json", "$out/Debug.json", "$out/Release.json"
Write-Output 'PASS Debug/Release full managed support differential; both target originals absent from rebuilt hosts'
