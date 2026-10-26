[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$FoundationDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path $CompilerDirectory).Path
$FoundationDirectory=(Resolve-Path $FoundationDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite Foundation product evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Force $OutputDirectory).FullName
$work=Join-Path ([IO.Path]::GetTempPath()) ('compiler-foundation-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work|Out-Null
$state=@{schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';modes=@();scope='Existing complete managed Foundation assertions inside the production-object validation host; not SDK or MOD compilation'}
Push-Location $root
try {
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    $source=(Resolve-Path 'recovery/build/ManagedValidation/ManagedFoundationValidation.cs').Path
    $state.fixtureSha256=(Get-FileHash $source -Algorithm SHA256).Hash.ToLowerInvariant()
    $main=Join-Path $work 'Main.cs'
    'class Program { static int Main(string[] args) { NewSdk.BuildValidation.ManagedFoundationValidation.Run(true,args[0]); return 0; } }'|Set-Content $main
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $manifest=Get-Content recovery/build/Managed.Foundation.json -Raw|ConvertFrom-Json
    foreach($mode in @('Debug','Release')) {
        $modeOut=(New-Item -ItemType Directory -Force (Join-Path $out $mode)).FullName
        $clean=Join-Path $work $mode
        $run=Initialize-CompilerValidationRun (Join-Path $CompilerDirectory $mode) $clean $mode $state.sourceCommit
        $inventory=(Resolve-Path (Join-Path $FoundationDirectory "$mode/Foundation.types.txt")).Path
        $fixture=Join-Path $clean 'FoundationFixture.exe'
        $arguments=@('/nologo','/codepage:65001','/platform:x86',('/out:'+$fixture),('/r:'+$run.exe),('/r:'+(Join-Path $clean 'antlr.runtime.dll')))
        foreach($name in $manifest.frameworkReferences){$arguments+=('/r:'+$name+'.dll')}
        $arguments+=@($source,$main)
        & $csc @arguments *> "$modeOut/fixture-build.log"
        if($LASTEXITCODE -ne 0){Get-Content "$modeOut/fixture-build.log" -Tail 80|Out-Host;throw "$mode Foundation product fixture compilation failed"}
        Push-Location $clean
        try {& $run.exe $fixture $inventory *> "$modeOut/runtime.log";$code=$LASTEXITCODE}
        finally {Pop-Location}
        if($code -ne 0 -or !(Select-String -LiteralPath "$modeOut/runtime.log" -SimpleMatch 'PASS Foundation:' -Quiet)){
            Get-Content "$modeOut/runtime.log" -Tail 80|Out-Host;throw "$mode Foundation product behavior failed"
        }
        Complete-CompilerValidationRun $run
        $state.modes+=@{configuration=$mode;passed=$true;exeSha256=$run.productExeSha256;validationHostSha256=$run.validationHostSha256;typeInventorySha256=(Get-FileHash $inventory -Algorithm SHA256).Hash.ToLowerInvariant();singlePhysicalModule=$true}
    }
    $state.status='passed';$global:LASTEXITCODE=0
} catch {$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally {$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json";Pop-Location}
