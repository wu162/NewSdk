[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path $CompilerDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite common texture evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory $OutputDirectory).FullName
$work=Join-Path ([IO.Path]::GetTempPath()) ('texture-common-'+[Guid]::NewGuid().ToString('N'))
$state=@{schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';configurations=@();scope='Common formats through production plugin objects; independent DDS/BC decode and storage contracts; no game runtime claim'}
function Invoke-CommonTool([string]$Command,[string[]]$Arguments,[string]$Directory,[string]$Log){
    Push-Location $Directory
    try{& $Command @Arguments *> $Log;$code=$LASTEXITCODE}finally{Pop-Location}
    if($code -ne 0){Get-Content $Log -Tail 80|Out-Host;throw "Texture common-format tool failed: $Command (exit $code)"}
}
Push-Location $root
try{
    $state.sourceCommit=(git rev-parse HEAD).Trim()
    $source=(Resolve-Path 'recovery/tests/TextureCommonFormats.cs').Path
    $generator=(Resolve-Path 'recovery/scripts/GenerateTextureFixtures.py').Path
    $state.testSources=@(@($source,$generator)|ForEach-Object{@{path=[IO.Path]::GetRelativePath($root,$_).Replace('\','/');sha256=(Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant()}})
    New-Item -ItemType Directory -Force $work|Out-Null
    $fixtures=Join-Path $work '中文纹理 fixtures'
    Invoke-CommonTool 'python' @($generator,$fixtures) $root "$out/generate.log"
    $state.fixtureInputs=@(Get-ChildItem $fixtures -File|Sort-Object Name|ForEach-Object{@{name=$_.Name;bytes=$_.Length;sha256=(Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()}})
    $csc="$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    foreach($mode in @('Debug','Release')){
        $evidence=(New-Item -ItemType Directory "$out/$mode").FullName
        $clean=Join-Path $work "$mode/clean"
        $run=Initialize-CompilerValidationRun (Join-Path $CompilerDirectory $mode) $clean $mode $state.sourceCommit
        $fixture=Join-Path $clean 'TextureCommonFormats.exe'
        Invoke-CommonTool $csc @('/nologo','/codepage:65001','/platform:x86','/r:System.Core.dll','/r:System.Xml.dll','/r:System.Drawing.dll','/r:System.Web.Extensions.dll',"/r:$($run.exe)","/out:$fixture",$source) $root "$evidence/fixture-build.log"
        $allowed=@('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll','TextureCommonFormats.exe')
        if(@(Get-ChildItem -LiteralPath $clean -Force|Where-Object{$_.PSIsContainer -or $_.Name -notin $allowed}).Count -ne 0){throw 'Unexpected common-format validation deployment entry'}
        Invoke-CommonTool $run.exe @($fixture,$fixtures,"$evidence/results.json") $clean "$evidence/runtime.log"
        $results=Get-Content "$evidence/results.json" -Raw|ConvertFrom-Json
        if($results.status -cne 'passed' -or @($results.cases).Count -lt 20){throw 'Common-format matrix incomplete'}
        Complete-CompilerValidationRun $run
        $state.configurations+=@{configuration=$mode;passed=$true;caseCount=@($results.cases).Count;assertions=$results.assertions;resultSha256=(Get-FileHash "$evidence/results.json" -Algorithm SHA256).Hash.ToLowerInvariant();exeSha256=$run.productExeSha256;validationHostSha256=$run.validationHostSha256;sameProductionObjects=$true;legacyTextureDllPresent=$false}
    }
    if((Get-FileHash "$out/Debug/results.json" -Algorithm SHA256).Hash -cne (Get-FileHash "$out/Release/results.json" -Algorithm SHA256).Hash){throw 'Debug/Release common-format outputs differ'}
    $state.status='passed';$global:LASTEXITCODE=0
    Write-Host 'PASS common texture formats Debug/Release: PNG/TGA/BMP/DIB/JPEG/DDS, raw/BC outputs, mips, Chinese paths, OnDemand and failures; no original texture DLL.'
}catch{$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally{$state|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json" -Encoding utf8;Pop-Location}
