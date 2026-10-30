[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [string]$HostDirectory
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=(New-Item -ItemType Directory $OutputDirectory).FullName
$work=(New-Item -ItemType Directory (Join-Path $env:RUNNER_TEMP ('texture-parallel-'+[Guid]::NewGuid().ToString('N')))).FullName
$oracle=(New-Item -ItemType Directory "$work/oracle").FullName
if($HostDirectory) {
    Copy-Item "$HostDirectory/*" $oracle
    $exe=Join-Path $oracle 'NewModCompiler.exe'
    $modes=@('locked','isolated')
} else {
    Copy-Item "$root/SDK/tools/*.dll" $oracle
    $dll=Join-Path $oracle 'BinaryAssetBuilder.TextureCompiler.dll'
    if((Get-FileHash $dll).Hash.ToLowerInvariant() -cne '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e'){throw 'Unexpected original Texture DLL'}
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $exe=Join-Path $oracle 'Probe.exe'
    $source=(Resolve-Path (Join-Path $root 'recovery/tests/TextureParallelProbe.cs')).Path
    $core=(Resolve-Path (Join-Path $oracle 'BinaryAssetBuilder.Core.dll')).Path
    & $csc /nologo /platform:x86 /r:System.Core.dll /r:System.Xml.dll /r:System.Web.Extensions.dll "/r:$core" "/out:$exe" $source *> "$out/build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$out/build.log";throw 'Probe build failed'}
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/></startup></configuration>'|Set-Content "$exe.config"
    $modes=@('unsafe','locked','isolated')
}
$dll=Join-Path $oracle 'BinaryAssetBuilder.TextureCompiler.dll'
$summary=@{sourceCommit=$env:GITHUB_SHA;runId=$env:GITHUB_RUN_ID;status='started';hostBound=[bool]$HostDirectory;exeSha256=(Get-FileHash $exe).Hash.ToLowerInvariant();textureSha256=(Get-FileHash $dll).Hash.ToLowerInvariant();scope='Win32 Texture DLL thread reentry experiment, not production certification';cases=@()}
$failures=@()
foreach($route in @('Texture','OnDemandTexture','TerrainTextureAtlas','mixed')) {
    foreach($mode in $modes) {
        $case=(New-Item -ItemType Directory "$out/$route-$mode").FullName
        $info=[Diagnostics.ProcessStartInfo]::new($exe)
        $info.UseShellExecute=$false;$info.WorkingDirectory=$oracle
        $info.RedirectStandardOutput=$true;$info.RedirectStandardError=$true
        foreach($arg in @($dll,$case,$route,$mode,'3')){$info.ArgumentList.Add($arg)}
        $process=[Diagnostics.Process]::new();$process.StartInfo=$info
        try {
            if(!$process.Start()){throw 'Probe process did not start'}
            $stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
            $timeout=!$process.WaitForExit(120000)
            if($timeout){$process.Kill($true);$process.WaitForExit()}
            [IO.File]::WriteAllText("$case/stdout.log",$stdout.GetAwaiter().GetResult())
            [IO.File]::WriteAllText("$case/stderr.log",$stderr.GetAwaiter().GetResult())
            $row=@{route=$route;mode=$mode;exitCode=$process.ExitCode;timeout=$timeout;passed=(!$timeout -and $process.ExitCode -eq 0)}
            if(Test-Path "$case/result.json"){$row.result=Get-Content "$case/result.json" -Raw|ConvertFrom-Json}
            $summary.cases+=,$row
            if(!$row.passed -and $mode -ne 'unsafe'){$failures+="$route/$mode"}
        } finally {$process.Dispose()}
        $summary|ConvertTo-Json -Depth 20|Set-Content "$out/validation.json" -Encoding utf8
    }
}
$unsafe=@($summary.cases|Where-Object{$_.mode -eq 'unsafe'})
$summary.sharedModuleReentry=if($unsafe.Count -eq 0){'not-retested'}elseif(@($unsafe|Where-Object{$_.passed}).Count -eq 0){'rejected'}else{'inconclusive'}
$summary.status=if($failures.Count){'failed'}else{'isolation-passed'}
$summary|ConvertTo-Json -Depth 20|Set-Content "$out/validation.json" -Encoding utf8
if($failures.Count){throw "Texture reentry experiment rejected: $($failures -join ', ')"}
Write-Host "Texture isolation passed; shared module reentry: $($summary.sharedModuleReentry). Not production worker certification."
