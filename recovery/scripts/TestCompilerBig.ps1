#requires -Version 7.0
[CmdletBinding()]
param(
    [string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180,
    [switch]$Preflight
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(!$Preflight){$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path}
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite BIG evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$short=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('b'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Within([string]$Directory,[string]$Path){
    $directory=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$path=[IO.Path]::GetFullPath($Path)
    $path.StartsWith($directory+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function EqualBytes([string]$Left,[string]$Right){
    $leftBytes=[IO.File]::ReadAllBytes($Left);$rightBytes=[IO.File]::ReadAllBytes($Right)
    Need ($leftBytes.Length -eq $rightBytes.Length) 'No-cache/cache artifact lengths differ'
    for($i=0;$i -lt $leftBytes.Length;$i++){Need ($leftBytes[$i] -eq $rightBytes[$i]) "No-cache/cache raw artifact byte differs at $i"}
}
function SameOutputs($Left,$Right){
    Need ($Left.files.Count -eq $Right.files.Count) 'No-cache/cache artifact counts differ'
    foreach($key in $Left.files.Keys){Need ($Right.files.ContainsKey($key)) "Cache output omitted $key";EqualBytes $Left.files[$key] $Right.files[$key]}
}
function DifferentOutputs($Left,$Right){
    Need ($Left.files.Count -eq $Right.files.Count) 'Changed fixture output layout differs'
    $changed=$false
    foreach($key in $Left.files.Keys){if(!$Right.files.ContainsKey($key)){$changed=$true}elseif((Hash $Left.files[$key]) -cne (Hash $Right.files[$key])){$changed=$true}}
    Need $changed 'Input mutation did not exercise any observable compiled output change'
}
function CacheFiles([string]$Destination){
    $files=@{};$cache=Join-Path $Destination '.cache'
    if(Test-Path -LiteralPath $cache){Get-ChildItem -LiteralPath $cache -File -Recurse|ForEach-Object{$files[$_.FullName.Substring($cache.Length)]=Hash $_.FullName}}
    return $files
}
function CacheUnchanged([string]$Destination,$Before){
    $after=CacheFiles $Destination;Need ($after.Count -eq $Before.Count) 'Failed/cancelled build persisted additional session-cache files'
    foreach($key in $Before.Keys){Need ($after.ContainsKey($key) -and $after[$key] -ceq $Before[$key]) 'Failed/cancelled build changed previously confirmed session-cache bytes'}
}
function RunCli([string]$Name,[string]$Mod,[string]$Destination,[string]$Selection,[string]$Format,[int]$Expected=0,[bool]$Cancel=$false,[string]$HoldReadFile=$null,[string[]]$Extra=@()){
    $caseOut=(New-Item -ItemType Directory -Path (Join-Path $script:modeOut $Name)).FullName
    New-Item -ItemType Directory -Force -Path $Destination|Out-Null
    $arguments=@('build','--mod',$Mod,'--sdk-root',$script:sdk,'--out',$Destination,'--package-content',$Selection,'--package-format',$Format)+$Extra
    $events=Join-Path $Destination 'build.events.jsonl';$report=Join-Path $Destination 'build-report.json'
    $request=@{exe=$script:exe;cwd=$script:runtime;arguments=$arguments;stdout=(Join-Path $caseOut 'stdout.log');stderr=(Join-Path $caseOut 'stderr.log');events=$events;cancel=$Cancel;timeoutSeconds=$TimeoutSeconds}
    if($HoldReadFile){$request.holdReadFile=$HoldReadFile}
    $request|ConvertTo-Json -Depth 6|Set-Content "$caseOut/request.json"
    & $script:control "$caseOut/request.json" "$caseOut/control.json" *> "$caseOut/controller.log"
    $exit=$LASTEXITCODE;$audit=Get-Content "$caseOut/control.json" -Raw|ConvertFrom-Json
    $entry=@{name=$Name;arguments=$arguments;expectedExit=$Expected;passed=$false;controller=$audit};$script:modeState.cases+=,$entry
    try{
        Need ($exit -eq 0 -and $audit.succeeded -and $audit.processExitCode -eq $Expected) "BIG CLI failed: $Name"
        Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) 'Asset cache introduced a product child process'
        if($Cancel){Need ($audit.signalSent -and $audit.freshEventsPathObserved -and $audit.streamStartedObserved -and $Expected -eq 6) 'Cache cancellation did not receive actual CtrlBreak'}
        if($Expected -eq 2){
            Need (!(Test-Path -LiteralPath $report) -and !(Test-Path -LiteralPath $events)) 'Unsupported BIG option started a build'
            Need ((Get-Content -LiteralPath (Join-Path $caseOut 'stderr.log') -Raw).Contains('PACKAGE_BASE_UNSUPPORTED')) 'Base BIG rejection did not explain missing LoD backend'
            $entry.passed=$true;return @{name=$Name;destination=$Destination;caseOut=$caseOut}
        }
        $body=Get-Content -LiteralPath $report -Raw|ConvertFrom-Json
        Need ($body.exitCode -eq $Expected) 'Cache process/report exit differs'
        $rows=@(Get-Content -LiteralPath $events|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})
        Need ($rows[-1].eventType -ceq 'BuildFinished' -and @($rows|Where-Object{$_.eventType -ceq 'BuildStarted'}).Count -eq 1 -and @($rows|Where-Object{$_.eventType -ceq 'BuildFinished'}).Count -eq 1) 'Cache CLI terminal protocol differs'
        $sequence=0;foreach($row in $rows){$sequence++;Need ($row.buildId -ceq $body.buildId -and $row.sequence -eq $sequence) 'Cache CLI build identity/sequence differs'}
        Need ($rows[0].payload.processId -eq $audit.processId -and @($body.externalTools).Count -eq 0) 'Cache CLI production identity/external tools differs'
        if($Cancel){Need ($audit.observedBuildStarted.buildId -ceq $body.buildId -and $audit.observedBuildStarted.payload.processId -eq $audit.processId -and $audit.observedStreamStarted.buildId -ceq $body.buildId) 'CtrlBreak observed a stale cached build generation'}
        $files=@{};$archived=Join-Path $caseOut 'artifacts';New-Item -ItemType Directory -Path $archived|Out-Null
        if($Expected -eq 0){
            Need ($body.publicationStatus -ceq 'PublishedDirectory' -and @($body.artifacts).Count -eq $(if($Format -ceq 'big'){2}else{$script:expectedArtifacts})) 'Session fixture did not publish exact stream/CDATAs count'
            $published=Join-Path $Destination 'published'
            foreach($artifact in $body.artifacts){
                Need ((Within $published $artifact.Path) -and (Hash $artifact.Path) -ceq $artifact.Sha256 -and (Get-Item -LiteralPath $artifact.Path).Length -eq $artifact.Size) 'Cached artifact identity/containment differs'
                $relative=[IO.Path]::GetRelativePath($published,$artifact.Path);$copy=Join-Path $archived $relative
                New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($copy))|Out-Null
                Copy-Item -LiteralPath $artifact.Path -Destination $copy;$files[$relative]=$copy
            }
        }else{Need (@($body.artifacts).Count -eq 0) 'Failed/cancelled cache build accepted artifacts'}
        Need (!(Test-Path -LiteralPath (Join-Path $Destination ('.build/'+$body.buildId)))) 'Cache build retained owned staging'
        Need (@(Get-ChildItem -LiteralPath $Destination -Directory|Where-Object{$_.Name -like '.cache-candidate-*'}).Count -eq 0) 'Session-cache transaction retained its candidate directory'
        $entry.passed=$true;$entry.buildId=$body.buildId
        return @{name=$Name;report=$body;files=$files;mod=$Mod;caseOut=$caseOut;destination=$Destination}
    }catch{
        foreach($name in @('stdout.log','stderr.log','controller.log')){Get-Content -LiteralPath (Join-Path $caseOut $name) -Tail 30 -ErrorAction SilentlyContinue|Out-Host}
        throw
    }finally{
        if(Test-Path -LiteralPath $report){Copy-Item -LiteralPath $report -Destination "$caseOut/build-report.json"}
        if(Test-Path -LiteralPath $events){Copy-Item -LiteralPath $events -Destination "$caseOut/build.events.jsonl"}
    }
}

function ExpectedInventory($Loose){
    $expected=@();$published=Join-Path $Loose.destination 'published'
    foreach($artifact in $Loose.report.artifacts){
        $relative=[IO.Path]::GetRelativePath($published,$artifact.Path).Replace('\','/')
        $task=@($Loose.report.plan.Tasks|Where-Object{$relative.StartsWith($_.Content+'/'+[IO.Path]::GetFileNameWithoutExtension($_.InputPath)+'/',[StringComparison]::Ordinal)})
        Need ($task.Count -eq 1) 'Loose BIG oracle artifact has ambiguous task owner'
        $stem=[IO.Path]::GetFileNameWithoutExtension($task[0].InputPath)
        $prefix=if($task[0].Content -ceq 'apt'){'Data/AptUI/'}elseif($task[0].Content -ceq 'maps'){'Data/AdditionalMaps/'}else{throw 'Base artifact reached BIG oracle'}
        $tail=[IO.Path]::GetFileName($artifact.Path)
        # Strip private Core streams/category directories. Game archive names
        # use four linked basenames and manifest-bound stem/cdata children.
        if($artifact.Kind -ceq 'custom-data'){$tail=$stem+'/cdata/'+$tail}
        $name=($prefix+$tail).Replace('/','\').ToLowerInvariant()
        $source=$Loose.files[[IO.Path]::GetRelativePath($published,$artifact.Path)]
        Need ($null -ne $source -and (Hash $source) -ceq $artifact.Sha256) 'Archived loose oracle source changed'
        $expected+=,@{name=$name;source=$source;bytes=$artifact.Size;sha256=$artifact.Sha256}
    }
    # The old SDK writes echo. after each successful APT stream, producing
    # exactly space + CRLF (Windows probe 200d0a) in <stream>.version. Derive only from this current plan.
    foreach($task in @($Loose.report.plan.Tasks|Where-Object{$_.Content -ceq 'apt'})){
        $stem=[IO.Path]::GetFileNameWithoutExtension($task.InputPath)
        $directory=Join-Path $Loose.caseOut 'versions';New-Item -ItemType Directory -Force -Path $directory|Out-Null
        $source=Join-Path $directory ($stem+'.version');[IO.File]::WriteAllBytes($source,[byte[]](32,13,10))
        $expected+=,@{name=('data\aptui\'+$stem.ToLowerInvariant()+'.version');source=$source;bytes=3;sha256=(Hash $source)}
    }
    return $expected
}
function CompareBig($Big,$Loose,[string]$Name){
    $caseOut=(New-Item -ItemType Directory -Path (Join-Path $script:modeOut $Name)).FullName
    $expected=@(ExpectedInventory $Loose)
    Need ($expected.Count -eq ($Loose.files.Count+@($Loose.report.plan.Tasks|Where-Object{$_.Content -ceq 'apt'}).Count) -and @($expected.name|Select-Object -Unique).Count -eq $expected.Count) 'BIG oracle inventory duplicates or omits current artifacts'
    $expected|ConvertTo-Json -Depth 6 -AsArray|Set-Content "$caseOut/expected.json"
    $oracleWork=(New-Item -ItemType Directory -Path (Join-Path $short ('o'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
    $tools=(New-Item -ItemType Directory -Path (Join-Path $oracleWork 't')).FullName
    $inputs=(New-Item -ItemType Directory -Path (Join-Path $oracleWork 'i')).FullName
    foreach($item in $expected){$path=Join-Path $inputs $item.name;New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($path))|Out-Null;Copy-Item -LiteralPath $item.source -Destination $path}
    [IO.File]::WriteAllText((Join-Path $inputs 'excluded.asset'),'Original -x:*.asset must exclude this stale fixture')
    $original=Join-Path $tools 'MakeBig.exe';Copy-Item -LiteralPath "$root/SDK/tools/MakeBig.exe" -Destination $original
    Need ((Hash $original) -ceq (Hash "$root/SDK/tools/MakeBig.exe")) 'MakeBig oracle EXE differs from original SDK bytes'
    [IO.File]::WriteAllText(($original+'.config'),'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0"/></startup></configuration>')
    $archive=Join-Path $oracleWork 'original.big'
    $start=[Diagnostics.ProcessStartInfo]::new();$start.FileName=$original;$start.WorkingDirectory=$tools;$start.UseShellExecute=$false;$start.CreateNoWindow=$true;$start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true
    foreach($argument in @('-f',$inputs,'-x:*.asset',('-o:'+$archive))){$start.ArgumentList.Add($argument)}
    $process=[Diagnostics.Process]::new();$process.StartInfo=$start
    try{
        Need ($process.Start()) 'Original MakeBig did not start'
        $stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
        if(!$process.WaitForExit($TimeoutSeconds*1000)){$process.Kill($true);throw 'Original MakeBig timed out'}
        [IO.File]::WriteAllText("$caseOut/original.stdout.log",$stdout.GetAwaiter().GetResult());[IO.File]::WriteAllText("$caseOut/original.stderr.log",$stderr.GetAwaiter().GetResult())
        Need ($process.ExitCode -eq 0 -and (Test-Path -LiteralPath $archive)) 'Original MakeBig rejected exact artifact inventory'
    }finally{$process.Dispose()}
    $bigFiles=@($Big.report.artifacts|Where-Object{$_.Kind -ceq 'package.big'});$skuFiles=@($Big.report.artifacts|Where-Object{$_.Kind -ceq 'package.skudef'})
    Need ($bigFiles.Count -eq 1 -and $skuFiles.Count -eq 1) 'BIG report omitted exact package identities'
    Need ([IO.Path]::GetFileName($bigFiles[0].Path) -ceq 'business_1.0.big' -and [IO.Path]::GetFileName($skuFiles[0].Path) -ceq 'business_1.0.skudef') 'BIG/skudef filenames differ from production contract'
    $skudef=[IO.File]::ReadAllBytes($skuFiles[0].Path)
    Need ([Convert]::ToBase64String($skudef) -ceq [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("mod-game 1.12`r`nadd-big business_1.0.big`r`n"))) 'Game skudef raw ASCII/CRLF commands/version differ from old SDK script'
    & python "$PSScriptRoot/ValidateCompilerBig.py" --archive $bigFiles[0].Path --oracle $archive --expected "$caseOut/expected.json" --output "$caseOut/comparison.json"
    Need ($LASTEXITCODE -eq 0) 'BIG/original MakeBig format, member inventory or raw contents differ'
    Copy-Item -LiteralPath $archive -Destination (Join-Path $caseOut 'original.big')
    $script:modeState.comparisons+=,@{name=$Name;originalExeSha256=(Hash $original);expected=$expected;comparison=(Get-Content "$caseOut/comparison.json" -Raw|ConvertFrom-Json)}
}

function PrepareFixture([string]$Path,[string]$Log){
    & python "$PSScriptRoot/PrepareCompilerCliBusiness.py" $Path *> $Log;Need ($LASTEXITCODE -eq 0) 'BIG business fixture generation failed'
    [IO.File]::WriteAllText("$Path/Data/AptUI/OnDemand.xml",'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><OnDemandTexture id="BigCdata" File="probe.tga" Type="StandardTexture" GenerateMipMaps="false" OutputFormat="A8R8G8B8" AllowAutomaticResize="false"/></AssetDeclaration>')
    & python "$root/recovery/probes/TextureRedirect/MakeFixture.py" "$Path/Data/AptUI/probe.tga";Need ($LASTEXITCODE -eq 0) 'BIG CDATA fixture generation failed'
    $check='import pathlib,runpy,sys;from lxml import etree as E;r=pathlib.Path(sys.argv[1]);p=pathlib.Path(sys.argv[2]);s=runpy.run_path(str(r/"recovery/scripts/GenerateXmlSdkModA.py"),run_name="big_fixture_schema")["aggregate_schema"]();s.assertValid(E.parse(str(p/"Data/AptUI/OnDemand.xml")));assert len((p/"Data/AptUI/probe.tga").read_bytes())==18+256*256*4;assert b"mod-game" in (r/"SDK/defaultscript.cs").read_bytes();assert (r/"SDK/tools/MakeBig.exe").read_bytes()[:2]==b"MZ"'
    & python -c $check $root $Path *>> $Log;Need ($LASTEXITCODE -eq 0) 'BIG OnDemand/CDATAs fixture source-XSD preflight failed'
    [IO.File]::WriteAllText("$Path/Data/AptUI/not-listed.asset",'Must never be included by directory globbing')
}
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=(& git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Actual production P7 BIG apt/maps selected and combined, exact current loose member bytes and independent original MakeBig format/content oracle, skudef, CDATA, cancellation/failure/publication rollback; base BIG explicitly unsupported; not a base LoD backend or final distribution acceptance'}
try{
    & python "$PSScriptRoot/ValidateCompilerBig.py" --self-test *> "$out/parser-preflight.json";Need ($LASTEXITCODE -eq 0) 'Independent BIG parser preflight failed'
    $fixture=Join-Path $short 'preflight-business'
    PrepareFixture $fixture "$out/fixture-preflight.log"
    if($Preflight){$state.succeeded=$true;$state.preflight=$true;$state.scope='Source-XSD business fixture and independent BIG parser preflight only';$global:LASTEXITCODE=0;return}
    Need (!(Test-Path -LiteralPath (Join-Path $CompilerDirectory 'reuse.json')) -and !(Test-Path -LiteralPath (Join-Path $CompilerDirectory 'session-replay.json'))) 'New BIG production inputs require a current production build'
    $script:control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'controller')
    $state.scriptSha256=Hash $PSCommandPath;$state.parserSha256=Hash "$PSScriptRoot/ValidateCompilerBig.py"
    foreach($mode in @('Debug','Release')){
        $script:modeOut=(New-Item -ItemType Directory -Path (Join-Path $out $mode)).FullName
        $modeWork=(New-Item -ItemType Directory -Path (Join-Path $short $mode.Substring(0,1))).FullName
        $script:runtime=(New-Item -ItemType Directory -Path (Join-Path $modeWork 'r')).FullName
        foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){Copy-Item -LiteralPath (Join-Path $CompilerDirectory "$mode/bin/$file") -Destination $runtime}
        $script:exe=Join-Path $runtime 'NewModCompiler.exe'
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $exe *> "$modeOut/prepare-texture.log";Need ($LASTEXITCODE -eq 0) 'BIG runtime Texture deployment failed'
        $script:modeState=@{configuration=$mode;passed=$false;exeSha256=(Hash $exe);cases=@();comparisons=@()};$state.modes+=,$modeState
        $script:sdk=(New-Item -ItemType Directory -Path (Join-Path $modeWork 's')).FullName
        New-Item -ItemType Directory -Path "$sdk/tools","$sdk/Schemas"|Out-Null
        Copy-Item -LiteralPath "$root/SDK/Schemas/xsd" -Destination "$sdk/Schemas" -Recurse;Copy-Item -LiteralPath "$root/SDK/tools/BinaryAssetBuilder.exe.config" -Destination "$sdk/tools"
        $mod=Join-Path $modeWork 'business'
        PrepareFixture $mod "$modeOut/fixture.log"
        $combined=$null;$plain=$null
        foreach($selection in @('apt','maps','apt,maps')){
            $name=$selection.Replace(',','-');$script:expectedArtifacts=if($selection -ceq 'apt'){9}elseif($selection -ceq 'maps'){4}else{13}
            $loose=RunCli ($name+'-loose') $mod (Join-Path $modeWork ($name+'-loose')) $selection 'loose'
            $big=RunCli ($name+'-big') $mod (Join-Path $modeWork ($name+'-big')) $selection 'big'
            CompareBig $big $loose ($name+'-original-makebig')
            if($selection -ceq 'apt,maps'){$combined=$big;$plain=$loose}
        }
        Need (@($plain.report.artifacts|Where-Object{$_.Kind -ceq 'custom-data'}).Count -eq 1) 'BIG fixture did not exercise exactly one actual complete CDATA'
        $destination=$combined.destination;$published=Join-Path $destination 'published'
        [IO.File]::WriteAllText((Join-Path $published 'stale.asset'),'Stale generation file must not survive')
        $rebuilt=RunCli 'replace-stale-generation' $mod $destination 'apt,maps' 'big'
        Need (@(Get-ChildItem -LiteralPath $published -File -Recurse).Count -eq 2) 'BIG publication retained stale generation files'
        CompareBig $rebuilt $plain 'replacement-original-makebig'
        $before=CacheFiles $destination;$publishedBefore=@{};Get-ChildItem -LiteralPath $published -File|ForEach-Object{$publishedBefore[$_.Name]=Hash $_.FullName}
        $held=@($rebuilt.report.artifacts|Where-Object{$_.Kind -ceq 'package.big'})[0].Path
        $failed=RunCli 'publication-fault' $mod $destination 'apt,maps' 'big' 7 $false $held
        foreach($name in $publishedBefore.Keys){Need ((Hash (Join-Path $published $name)) -ceq $publishedBefore[$name]) 'Failed BIG publication changed the previous generation'}
        $retry=RunCli 'retry-after-publication-fault' $mod $destination 'apt,maps' 'big';CompareBig $retry $plain 'publication-retry-original'
        $cancelled=RunCli 'actual-cancel' $mod $destination 'apt,maps' 'big' 6 $true
        foreach($name in $publishedBefore.Keys){Need ((Hash (Join-Path $published $name)) -ceq $publishedBefore[$name]) 'Cancelled BIG build changed the previous generation'}
        $retry=RunCli 'retry-after-cancel' $mod $destination 'apt,maps' 'big';CompareBig $retry $plain 'cancel-retry-original'
        $base=RunCli 'base-explicitly-unsupported' $mod (Join-Path $modeWork 'base') 'base' 'big' 2
        $modeState.passed=$true
    }
    $state.succeeded=$true;$global:LASTEXITCODE=0
}catch{$state.error=$_.Exception.ToString();throw}
finally{$state|ConvertTo-Json -Depth 22|Set-Content "$out/validation.json";try{Remove-Item -LiteralPath $short -Recurse -Force}catch{Write-Warning "BIG test work cleanup failed: $_"}}
