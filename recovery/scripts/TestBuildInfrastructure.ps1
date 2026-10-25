$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$work = Join-Path $env:RUNNER_TEMP ('infrastructure-validation-' + [Guid]::NewGuid().ToString('N'))
$out = "recovery/evidence/build-infrastructure/validation/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
New-Item -ItemType Directory -Force $work,$out | Out-Null
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$receipt = @{ sourceCommit=(git rev-parse HEAD).Trim(); runId=$env:GITHUB_RUN_ID; runAttempt=$env:GITHUB_RUN_ATTEMPT; status='started'; sdkValidated=$false; modes=@() }
try {
    $original = "$work/original"
    New-Item -ItemType Directory -Force $original | Out-Null
    Copy-Item SDK/tools/*.dll $original
    Copy-Item SDK/EALABuild.dll $original
    & $csc /nologo /platform:x86 /r:System.Xml.dll /r:"$original/BinaryAssetBuilder.Core.dll" /out:"$original/BuildInfrastructureContract.exe" recovery/tests/BuildInfrastructureContract.cs *> "$out/oracle-build.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'Oracle build failed' }
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$original/BuildInfrastructureContract.exe.config"
    & "$original/BuildInfrastructureContract.exe" "$out/original.txt" *> "$out/original.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/original.log"; throw 'Original oracle failed' }
    foreach ($mode in @('Debug','Release')) {
        $rebuilt="$work/$mode"
        New-Item -ItemType Directory -Force $rebuilt | Out-Null
        # Exclude target originals: this directory contains only source-built targets.
        Get-ChildItem SDK/tools/*.dll | Where-Object { $_.Name -notin @('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Remote.dll') } | Copy-Item -Destination $rebuilt
        Copy-Item "$original/BuildInfrastructureContract.exe*" $rebuilt
        foreach ($name in @('Core','Remote','EALABuild')) {
            $assembly=if($name -eq 'EALABuild'){'EALABuild'}else{"BinaryAssetBuilder.$name"}
            dotnet build "recovery/src/Managed/$name/$assembly.csproj" -c $mode -p:PlatformTarget=x86 -o "$work/build-$mode-$name" --nologo *> "$out/$mode-$name-build.log"
            if($LASTEXITCODE -ne 0){Get-Content "$out/$mode-$name-build.log" -Tail 50; throw "$mode $name build failed"}
            Copy-Item "$work/build-$mode-$name/$assembly.dll" $rebuilt
        }
        & "$rebuilt/BuildInfrastructureContract.exe" "$out/$mode.txt" *> "$out/$mode.log"
        if($LASTEXITCODE -ne 0){Get-Content "$out/$mode.log"; throw "$mode oracle failed"}
        $expected=(Get-FileHash "$out/original.txt").Hash
        $actual=(Get-FileHash "$out/$mode.txt").Hash
        if($expected -ne $actual){
            Compare-Object (Get-Content "$out/original.txt") (Get-Content "$out/$mode.txt") | Out-String -Width 400 | Set-Content "$out/$mode-difference.txt"
            Get-Content "$out/$mode-difference.txt" -First 70
            throw "$mode contract/behavior mismatch"
        }
        $receipt.modes+=@{ mode=$mode; records=@(Get-Content "$out/$mode.txt").Count; sha256=$actual.ToLowerInvariant(); passed=$true }
    }
    $receipt.status='passed'
} finally {
    $receipt | ConvertTo-Json -Depth 10 | Set-Content "$out/validation.json"
    Get-ChildItem recovery/src/Managed/Core,recovery/src/Managed/Remote,recovery/src/Managed/EALABuild -Recurse -Directory -ErrorAction SilentlyContinue | Where-Object { $_.Name -in @('bin','obj') } | Remove-Item -Recurse -Force
}
