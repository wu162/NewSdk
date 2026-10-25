param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$OutputDirectory) {
    $suffix = if ($env:GITHUB_RUN_ID) { "$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" } else { 'local-' + [Guid]::NewGuid().ToString('N') }
    $OutputDirectory = "recovery/evidence/mvp-xml/validation/$suffix"
}
if (Test-Path $OutputDirectory) { throw "Refusing to overwrite validation: $OutputDirectory" }
New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
$work = Join-Path $env:RUNNER_TEMP ('mvp-xml-tests-' + [Guid]::NewGuid().ToString('N'))
$env:MVP_XML_FIXTURE = Join-Path $work "fixtures"
$original = Join-Path $work 'original'
New-Item -ItemType Directory -Force $original | Out-Null
Copy-Item SDK/tools/*.dll $original
$msbuild = & "${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild/Current/Bin/MSBuild.exe | Select-Object -First 1
if (!$msbuild) { throw "Full .NET Framework MSBuild is required for net20 ResGen" }
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
& $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:System.Web.Extensions.dll /r:"$original/Mvp.Xml.dll" /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/MvpXmlOracle.exe" (Resolve-Path 'recovery/tests/MvpXmlOracle.cs').Path *> "$out/oracle-build.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'Oracle compilation failed' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/MvpXmlOracle.exe.config"
& "$original/MvpXmlOracle.exe" "$out/original.json" *> "$out/original.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log"; throw 'Original oracle failed' }
Get-Content "$out/original.log"
$baseline = (Get-FileHash "$out/original.json" -Algorithm SHA256).Hash
$results = @()
foreach ($mode in @('Debug', 'Release')) {
    $rebuilt = Join-Path $work $mode
    New-Item -ItemType Directory -Force $rebuilt | Out-Null
    # The original target assembly never enters this directory.
    Get-ChildItem SDK/tools -Filter *.dll | Where-Object Name -notin @('Mvp.Xml.dll') | Copy-Item -Destination $rebuilt
    $libraries = @()
    foreach ($name in @('MvpXml')) {
        $project = "recovery/src/Managed/$name/Mvp.Xml.csproj"
        $build = Join-Path $work "build-$mode-$name"
        & $msbuild $project /restore /nologo /p:Configuration=$mode /p:PlatformTarget=x86 /p:OutputPath="$build/" *> "$out/$mode-$name-build.log"
        if ($LASTEXITCODE -ne 0) { Get-Content "$out/$mode-$name-build.log"; throw "$mode $name build failed" }
        $dll = Join-Path $build "Mvp.Xml.dll"
        Copy-Item $dll $rebuilt
        $libraries += @{ assembly = $name; sha256 = (Get-FileHash $dll -Algorithm SHA256).Hash.ToLowerInvariant() }
    }
    Copy-Item "$original/MvpXmlOracle.exe*" $rebuilt
    & "$rebuilt/MvpXmlOracle.exe" "$out/$mode.json" *> "$out/$mode.log"
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
$inputs = @('SDK/tools/Mvp.Xml.dll','SDK/tools/BinaryAssetBuilder.Core.dll')
$fingerprints = @($inputs | ForEach-Object { @{ path = $_; sha256 = (Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant() } })
$sourceFiles = @(Get-ChildItem recovery/src/Managed/MvpXml -Recurse -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
$sourceFiles += @(Get-Item recovery/tests/MvpXmlOracle.cs,recovery/scripts/TestMvpXml.ps1,.github/workflows/recover-mvp-xml.yml)
$sourceFiles += @(Get-ChildItem recovery/tools/MvpXmlMetadata -Recurse -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
$sourceFingerprints = @($sourceFiles | Sort-Object FullName | ForEach-Object { @{ path = [IO.Path]::GetRelativePath($root, $_.FullName).Replace('\', '/'); sha256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() } })
@{
    sourceCommit = (git rev-parse HEAD).Trim(); runId = $env:GITHUB_RUN_ID; runAttempt = $env:GITHUB_RUN_ATTEMPT
    scope = 'Mvp.Xml source; complete type/member inventory and offline component behavior differential in isolated processes'
    casesPerConfiguration = @(Get-Content "$out/original.json" -Raw | ConvertFrom-Json).Count
    inputs = $fingerprints; sourceFiles = $sourceFingerprints; configurations = $results
    cultures = @('en-US', 'de-DE', 'tr-TR'); networkValidated = $false
    sdkValidated = $false
} | ConvertTo-Json -Depth 10 | Set-Content "$out/validation.json"
Compress-Archive -Path "$out/original.json", "$out/Debug.json", "$out/Release.json" -DestinationPath "$out/transcripts.zip"
Remove-Item "$out/original.json", "$out/Debug.json", "$out/Release.json"
Write-Output 'PASS Debug/Release Mvp.Xml offline differential; original target absent from rebuilt hosts'
