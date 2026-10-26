param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path $CompilerDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite differential evidence: $OutputDirectory"}
New-Item -ItemType Directory -Force $OutputDirectory|Out-Null
$out=(Resolve-Path $OutputDirectory).Path
$tempRoot=if($env:RUNNER_TEMP){$env:RUNNER_TEMP}else{[IO.Path]::GetTempPath()}
$work=Join-Path $tempRoot ('compiler-differential-'+[Guid]::NewGuid().ToString('N'))
$oracle=Join-Path $work 'original'
New-Item -ItemType Directory -Force $oracle|Out-Null
$state=@{schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';configurations=@();scope='Production EXE Armor/Hash combination differential only; not all P4 routes or real MOD acceptance'}
$oldPath=$env:PATH
function Invoke-Fixture([string]$Executable,[string[]]$Arguments,[string]$Directory,[string]$Log){
    Push-Location $Directory
    try{ & $Executable @Arguments *> $Log; $exitCode=$LASTEXITCODE }
    finally{Pop-Location}
    if($exitCode -ne 0){Get-Content $Log -Tail 80|Out-Host;throw "Fixture failed: $Executable (exit $exitCode)"}
}
Push-Location $root
try{
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    $source=(Resolve-Path 'recovery/probes/SingleExe/Probe.cs').Path
    $state.fixtureSha256=(Get-FileHash $source -Algorithm SHA256).Hash.ToLowerInvariant()
    $state.originalInputs=@('BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Tokenizer.dll','EALA.Hash.dll'|ForEach-Object{@{path="SDK/tools/$_";sha256=(Get-FileHash "SDK/tools/$_" -Algorithm SHA256).Hash.ToLowerInvariant()}})
    Copy-Item SDK/tools/*.dll $oracle
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    & $csc /nologo /codepage:65001 /platform:x86 /define:ORACLE /r:System.Xml.dll "/r:$oracle/BinaryAssetBuilder.Core.dll" "/r:$oracle/EALA.Hash.dll" "/out:$oracle/Original.exe" $source *> "$out/oracle-build.log"
    if($LASTEXITCODE -ne 0){Get-Content "$out/oracle-build.log" -Tail 80|Out-Host;throw 'Oracle build failed'}
    '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>'|Set-Content "$oracle/Original.exe.config"
    Invoke-Fixture "$oracle/Original.exe" @("$out/original.tsv","$oracle/BinaryAssetBuilder.Tokenizer.dll") $oracle "$out/original.log"
    if(@(Get-Content "$out/original.tsv").Count -ne 610){throw 'Baseline fixture row count changed'}
    $baseline=(Get-FileHash "$out/original.tsv" -Algorithm SHA256).Hash
    foreach($mode in @('Debug','Release')){
        $build=Join-Path $CompilerDirectory $mode
        $receipt=Get-Content "$build/build.json" -Raw|ConvertFrom-Json
        if($receipt.configuration -cne $mode -or $receipt.sourceCommit -cne $state.sourceCommit){throw 'Product receipt configuration/source does not match current validation'}
        $product=Join-Path $build 'bin/NewModCompiler.exe'
        if((Get-FileHash $product -Algorithm SHA256).Hash.ToLowerInvariant() -cne $receipt.exeSha256){throw 'Product executable differs from build receipt'}
        $clean=Join-Path $work $mode
        $evidence=Join-Path $out $mode
        New-Item -ItemType Directory -Force $clean,$evidence|Out-Null
        $validationRun=Initialize-CompilerValidationRun $build $clean $mode $state.sourceCommit
        & $csc /nologo /codepage:65001 /platform:x86 /define:EXTERNAL_PRODUCT /r:System.Xml.dll "/r:$clean/NewModCompiler.exe" "/out:$clean/ProductFixture.exe" $source *> "$evidence/fixture-build.log"
        if($LASTEXITCODE -ne 0){Get-Content "$evidence/fixture-build.log" -Tail 80|Out-Host;throw "$mode product fixture build failed"}
        Copy-Item "$clean/NewModCompiler.exe.config" "$clean/ProductFixture.exe.config"
        # Only system runtime locations remain; no SDK/tools or build search path.
        $env:PATH="$env:WINDIR/System32;$env:WINDIR;$env:WINDIR/Microsoft.NET/Framework/v4.0.30319"
        try{Invoke-Fixture $validationRun.exe @("$clean/ProductFixture.exe","$evidence/recovered.tsv") $clean "$evidence/runtime.log"}
        finally{$env:PATH=$oldPath}
        if(@(Get-Content "$evidence/recovered.tsv").Count -ne 610 -or (Get-FileHash "$evidence/recovered.tsv" -Algorithm SHA256).Hash -cne $baseline){throw "$mode original/product transcript mismatch"}
        Complete-CompilerValidationRun $validationRun
        $state.configurations+=@{validationHostSha256=$validationRun.validationHostSha256;configuration=$mode;passed=$true;rows=610;exeSha256=$receipt.exeSha256;transcriptSha256=$baseline.ToLowerInvariant();businessTypesOwnedByProduct=$true;separateOracleProcess=$true}
    }
    $state.status='passed'
    Write-Output 'PASS production EXE Armor/Hash Debug/Release: 610 rows each, four platforms, three buffers, independent Armor layout and rejection checks'
}catch{$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally{$env:PATH=$oldPath;$state|ConvertTo-Json -Depth 8|Set-Content "$out/validation.json";Pop-Location}

