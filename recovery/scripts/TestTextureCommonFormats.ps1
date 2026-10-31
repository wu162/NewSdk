[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,180)][int]$TimeoutSeconds=90
)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerValidation.Common.ps1"
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$CompilerDirectory=(Resolve-Path $CompilerDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to overwrite common texture evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory $OutputDirectory).FullName
$work=Join-Path ([IO.Path]::GetTempPath()) ('texture-common-'+[Guid]::NewGuid().ToString('N'))
$state=@{schemaVersion=1;sourceCommit=$null;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';configurations=@();productionCli=@();scope='Common formats through production plugin objects and real MOD CLI; independent DDS/BC decode, Core schema/file preprocessing, cache invalidation, missing-input retry; no game runtime claim'}
function Invoke-CommonTool([string]$Command,[string[]]$Arguments,[string]$Directory,[string]$Log){
    Push-Location $Directory
    try{& $Command @Arguments *> $Log;$code=$LASTEXITCODE}finally{Pop-Location}
    if($code -ne 0){Get-Content $Log -Tail 80|Out-Host;throw "Texture common-format tool failed: $Command (exit $code)"}
}
function Need([bool]$Condition,[string]$Message){if(!$Condition){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Equal-TextureOutputs($Left,$Right){
    Need ($Left.Count -eq $Right.Count) 'Common-format CLI output file counts differ'
    foreach($name in $Left.Keys){Need ($Right.ContainsKey($name) -and $Left[$name] -ceq $Right[$name]) "Common-format CLI raw output bytes differ: $name"}
}
function Run-TextureCli([string]$Name,[string]$Destination,[bool]$Cache=$true,[int]$Expected=0){
    $case=(New-Item -ItemType Directory -Path (Join-Path $script:cliEvidence $Name)).FullName
    New-Item -ItemType Directory -Path $Destination -Force|Out-Null
    $arguments=@('build','--mod',$script:cliMod,'--sdk-root',$script:cliSdk,'--out',$Destination,'--package-format','loose','--package-content','base','--performance','detailed',('--sc='+$Cache.ToString().ToLowerInvariant()),('--bc='+$Cache.ToString().ToLowerInvariant()),'--iod',($Destination+'-intermediate'))
    $events=Join-Path $Destination 'build.events.jsonl';$report=Join-Path $Destination 'build-report.json'
    @{exe=$script:cliExe;cwd=$script:cliRuntime;arguments=$arguments;stdout=(Join-Path $case 'stdout.log');stderr=(Join-Path $case 'stderr.log');events=$events;cancel=$false;timeoutSeconds=$TimeoutSeconds}|ConvertTo-Json -Depth 6|Set-Content "$case/request.json"
    & $script:cliControl "$case/request.json" "$case/control.json" *> "$case/controller.log"
    $exit=$LASTEXITCODE;$audit=Get-Content "$case/control.json" -Raw|ConvertFrom-Json
    $entry=@{name=$Name;arguments=$arguments;expectedExit=$Expected;passed=$false;controller=$audit};$script:cliMode.cases+=,$entry
    try{
        Need ($exit -eq 0 -and $audit.succeeded -and $audit.processExitCode -eq $Expected) "Common-format production CLI failed: $Name"
        Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) 'Common-format CLI created a child process'
        $body=Get-Content -LiteralPath $report -Raw|ConvertFrom-Json
        Need ($body.exitCode -eq $Expected) 'Common-format CLI report/process exit differs'
        $rows=@(Get-Content -LiteralPath $events|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})
        Need (@($rows|Where-Object{$_.eventType -ceq 'BuildFinished'}).Count -eq 1 -and $rows[-1].eventType -ceq 'BuildFinished') 'Common-format CLI terminal event differs'
        $files=@{};$published=Join-Path $Destination 'published'
        if($Expected -eq 0){
            Need ($body.publicationStatus -ceq 'PublishedDirectory' -and @($body.artifacts).Count -eq 5 -and @($body.artifacts|Where-Object{$_.Kind -ceq 'custom-data'}).Count -eq 1) 'Common-format CLI must publish one four-file stream and one OnDemand CDATA'
            foreach($file in Get-ChildItem -LiteralPath $published -File -Recurse){$files[[IO.Path]::GetRelativePath($published,$file.FullName)]=Hash $file.FullName}
            Need ($files.Count -eq 5) 'Unexpected common-format CLI published file'
            $manifest=@(Get-ChildItem -LiteralPath $published -Filter *.manifest -File -Recurse)
            Need ($manifest.Count -eq 1 -and [BitConverter]::ToUInt32([IO.File]::ReadAllBytes($manifest[0].FullName),12) -eq 7) 'Common-format CLI omitted declared texture assets'
        }else{Need (@($body.artifacts).Count -eq 0) 'Missing common-format source accepted artifacts'}
        $entry.passed=$true;$entry.cache=$body.cache;$entry.outputSha256=$files;$entry.buildId=$body.buildId
        return @{files=$files;report=$body;published=$published}
    }catch{
        foreach($log in @('stdout.log','stderr.log','controller.log')){Get-Content -LiteralPath (Join-Path $case $log) -Tail 35 -ErrorAction SilentlyContinue|Out-Host};throw
    }finally{
        if(Test-Path -LiteralPath $report){Copy-Item -LiteralPath $report -Destination "$case/build-report.json"}
        if(Test-Path -LiteralPath $events){Copy-Item -LiteralPath $events -Destination "$case/build.events.jsonl"}
    }
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
    $script:cliControl=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'controller')
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
        $script:cliEvidence=(New-Item -ItemType Directory "$evidence/cli").FullName
        $modeWork=(New-Item -ItemType Directory "$work/$mode/production").FullName
        $script:cliRuntime=(New-Item -ItemType Directory "$modeWork/runtime").FullName
        foreach($name in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){Copy-Item -LiteralPath (Join-Path $CompilerDirectory "$mode/bin/$name") -Destination $cliRuntime}
        $script:cliExe=Join-Path $cliRuntime 'NewModCompiler.exe'
        Need ((Hash $cliExe) -ceq $run.productExeSha256 -and @(Get-ChildItem -LiteralPath $cliRuntime -Force).Count -eq 3) 'Common-format production runtime differs from built product'
        $script:cliMode=@{configuration=$mode;passed=$false;exeSha256=(Hash $cliExe);cases=@()};$state.productionCli+=,$cliMode
        $script:cliSdk=(New-Item -ItemType Directory "$modeWork/sdk").FullName
        New-Item -ItemType Directory "$cliSdk/tools","$cliSdk/Schemas"|Out-Null
        Copy-Item -LiteralPath "$root/SDK/Schemas/xsd" -Destination "$cliSdk/Schemas" -Recurse
        Copy-Item -LiteralPath "$root/SDK/tools/BinaryAssetBuilder.exe.config" -Destination "$cliSdk/tools"
        $script:cliMod=(New-Item -ItemType Directory "$modeWork/MOD 中文纹理").FullName
        New-Item -ItemType Directory "$cliMod/Data"|Out-Null
        foreach($name in @('opaque.png','opaque.tga','opaque.bmp','alpha.png','solid.jpg','preserve.dds')){Copy-Item -LiteralPath (Join-Path $fixtures $name) -Destination "$cliMod/Data/$name"}
        Copy-Item -LiteralPath "$fixtures/opaque.png" -Destination "$cliMod/Data/on-demand.png"
        $xml=@'
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <Texture id="PngDefault" File="opaque.png"/>
  <Texture id="TgaRaw" File="opaque.tga" OutputFormat="A8R8G8B8" GenerateMipMaps="false"/>
  <Texture id="BmpDxt3" File="opaque.bmp" OutputFormat="DXT3"/>
  <Texture id="JpegDxt5" File="solid.jpg" OutputFormat="DXT5"/>
  <Texture id="AlphaDxt5" File="alpha.png" OutputFormat="DXT5"/>
  <Texture id="DdsPreserve" File="preserve.dds"/>
  <OnDemandTexture id="OnDemandPng" File="on-demand.png" OutputFormat="A8R8G8B8" GenerateMipMaps="false"/>
</AssetDeclaration>
'@
        [IO.File]::WriteAllText("$cliMod/Data/Mod.xml",$xml,[Text.UTF8Encoding]::new($false))
        $cached=Join-Path $modeWork 'cached';$fresh=Join-Path $modeWork 'fresh'
        $cold=Run-TextureCli 'cold' $cached
        Need ($cold.report.cache.instanceCompile -eq 7) 'Cold common-format build did not compile all seven declared assets'
        $warm=Run-TextureCli 'warm' $cached;Equal-TextureOutputs $cold.files $warm.files
        Need ($warm.report.cache.instanceCompile -eq 0 -and $warm.report.cache.documentReuse -gt 0) 'Warm common-format build did not reuse compiled assets'
        $image="$cliMod/Data/opaque.tga";$stamp=[IO.File]::GetLastWriteTimeUtc($image);$before=[IO.File]::ReadAllBytes($image);$before[18]=$before[18] -bxor 127
        [IO.File]::WriteAllBytes($image,$before);[IO.File]::SetLastWriteTimeUtc($image,$stamp)
        $changed=Run-TextureCli 'one-image-mutated' $cached
        Need ($changed.report.cache.instanceCompile -eq 1) 'Single image mutation must compile only its dependent texture'
        Need (@($warm.files.Keys|Where-Object{$warm.files[$_] -cne $changed.files[$_]}).Count -gt 0) 'Image mutation did not change any final output'
        $plain=Run-TextureCli 'fresh-no-cache' $fresh $false;Equal-TextureOutputs $changed.files $plain.files
        Need ($plain.report.cache.instanceCompile -eq 7) 'Independent no-cache build did not compile every texture'
        Remove-Item -LiteralPath $image
        $null=Run-TextureCli 'missing-image' $cached $true 4
        [IO.File]::WriteAllBytes($image,$before)
        $retry=Run-TextureCli 'restored-retry' $cached;Equal-TextureOutputs $changed.files $retry.files
        $cliMode.passed=$true;$cliMode.schemaDefaultsExercised=$true;$cliMode.nonAsciiModPath=$true;$cliMode.onlyDependentTextureRecompiled=$true;$cliMode.failureRetryPassed=$true
    }
    if((Get-FileHash "$out/Debug/results.json" -Algorithm SHA256).Hash -cne (Get-FileHash "$out/Release/results.json" -Algorithm SHA256).Hash){throw 'Debug/Release common-format outputs differ'}
    $state.status='passed';$global:LASTEXITCODE=0
    Write-Host 'PASS common texture formats Debug/Release: independent DDS decode plus real seven-asset CLI defaults/cold/warm/single-image invalidation/no-cache equality/missing-image retry; no original texture DLL.'
}catch{$state.status='failed';$state.error=$_.Exception.ToString();throw}
finally{$state|ConvertTo-Json -Depth 12|Set-Content "$out/validation.json" -Encoding utf8;Pop-Location}
