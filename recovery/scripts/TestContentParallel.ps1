#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$ProductBuildDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(30,600)][int]$TimeoutSeconds=240
)
$ErrorActionPreference='Stop'
if(!$IsWindows){throw 'Content-parallel production acceptance requires Windows'}
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$product=(Resolve-Path -LiteralPath $ProductBuildDirectory).Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$work=Join-Path ([IO.Path]::GetPathRoot($env:RUNNER_TEMP)) ('cp-'+[Guid]::NewGuid().ToString('N').Substring(0,8))
$null=New-Item -ItemType Directory -Path $work
$oldProgress=$env:NEWSDK_PROGRESS
$state=@{schemaVersion=1;sourceCommit=(git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';samples=@();coreFixtures=@();scope='One Release product, private production-object Core/application contracts; all seven nonempty base/apt/maps selections with two entries in each optional group; jobs1/4 cold/hot loose bytes and BIG/SKUDEF bytes; actual process-job enforced cancellation/failure/retry; targeted real Flash8 FLA plus file-backed APT XML and existing MapMetaData fixtures with jobs1/4 cold comparisons; a single pinned complete Upheaval base jobs1/4 cold comparison. The seven-combination scheduling matrix uses Armor assets; targeted UI/Maps cases are not full UI/Maps, original SDK or game runtime acceptance.';timingBoundary='External controller process wall, per-stream owner Compile intervals from one build Stopwatch, Windows peak process working set. OS page cache uncontrolled; no speed threshold.'}
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function SaveState{$state|ConvertTo-Json -Depth 24|Set-Content -LiteralPath "$out/validation.json"}
function Snapshot([string]$Directory){
    $map=@{}
    if(Test-Path -LiteralPath $Directory){foreach($file in Get-ChildItem -LiteralPath $Directory -Recurse -File){$map[[IO.Path]::GetRelativePath($Directory,$file.FullName)]=@{hash=(Hash $file.FullName);lastWrite=$file.LastWriteTimeUtc.Ticks;length=$file.Length}}}
    return $map
}
function Unchanged([string]$Directory,$Before){
    $after=Snapshot $Directory;Need ($after.Count -eq $Before.Count) "Unselected cache/source inventory changed: $Directory"
    foreach($key in $Before.Keys){Need ($after.ContainsKey($key) -and $after[$key].hash -ceq $Before[$key].hash -and $after[$key].lastWrite -eq $Before[$key].lastWrite) "Unselected cache/source changed: $Directory/$key"}
}
function SameOutputs($First,$Second){
    Need ($First.files.Count -gt 0 -and $First.files.Count -eq $Second.files.Count) 'Published output inventory differs'
    foreach($key in $First.files.Keys){Need ($Second.files.ContainsKey($key)) "Published output missing: $key";Need ([ContentParallelBytes]::Equal($First.files[$key],$Second.files[$key])) "Raw published bytes differ: $key"}
}
function Overlap($Execution){
    $groups=@($Execution.groups);[double]$best=0
    for($first=0;$first -lt $groups.Count;$first++){
        for($second=$first+1;$second -lt $groups.Count;$second++){
            foreach($a in $groups[$first].streams){foreach($b in $groups[$second].streams){
                $length=[Math]::Min([double]$a.completedMs,[double]$b.completedMs)-[Math]::Max([double]$a.startedMs,[double]$b.startedMs)
                $best=[Math]::Max($best,$length)
            }}
        }
    }
    return $best
}
function RunBuild([string]$Label,[string]$Mod,[string]$Selection,[string]$Format,[int]$Jobs,[int]$Expected=0,[string[]]$Extra=@(),[bool]$Cancel=$false,[bool]$ProgressOff=$false){
    $case=Join-Path $out $Label;$null=New-Item -ItemType Directory -Path $case
    $destination=Join-Path $work ('o'+$script:caseIndex);$script:caseIndex++
    $null=New-Item -ItemType Directory -Path $destination
    $arguments=@('build','--sdk-root',$script:sdk,'--mod',$Mod,'--out',$destination,'--package-content',$Selection,'--package-format',$Format,'--jobs',[string]$Jobs,'--performance','summary','--mod-version','1.0','--skudef-name','parallel','--include-additional=false')+$Extra
    $env:NEWSDK_PROGRESS=if($ProgressOff){'off'}else{$null}
    $request=@{exe=$script:exe;cwd=(Split-Path $script:exe);arguments=$arguments;stdout="$case/stdout.log";stderr="$case/stderr.log";events="$destination/build.events.jsonl";cancel=$Cancel;timeoutSeconds=$TimeoutSeconds}
    $request|ConvertTo-Json -Depth 6|Set-Content "$case/request.json"
    & $script:control "$case/request.json" "$case/control.json" *> "$case/controller.log"
    $exit=$LASTEXITCODE;$audit=Get-Content "$case/control.json" -Raw|ConvertFrom-Json
    $sample=@{label=$Label;selection=$Selection;jobs=$Jobs;format=$Format;expectedExit=$Expected;passed=$false;wallMs=$audit.elapsedMs;peakWorkingSetBytes=$audit.peakWorkingSetBytes;cpuMs=$audit.cpuMs}
    $state.samples+=,$sample;SaveState
    try {
        Need ($exit -eq 0 -and $audit.succeeded -and $audit.processExitCode -eq $Expected) "${Label}: CLI/control exit differs; see retained controller/stderr logs"
        Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) "${Label}: production introduced child processes"
        Need ($audit.peakWorkingSetBytes -gt 0) "${Label}: peak process memory unavailable"
        if($Cancel){Need ($audit.signalSent -and $audit.streamStartedObserved -and $audit.processExitCode -eq 6) 'Actual CtrlBreak was not verified'}
        $body=Get-Content "$destination/build-report.json" -Raw|ConvertFrom-Json
        $events=@(Get-Content "$destination/build.events.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
        Need ($body.exitCode -eq $Expected -and $events[-1].eventType -ceq 'BuildFinished') "${Label}: terminal report/events differ"
        $sequence=0;foreach($event in $events){$sequence++;Need ($event.sequence -eq $sequence -and $event.buildId -ceq $body.buildId) 'Event sequence/build identity crossed owners'}
        $starts=@($events|Where-Object eventType -CEQ 'StreamStarted');$ends=@($events|Where-Object eventType -CEQ 'StreamCompleted')
        Need ($starts.Count -eq $ends.Count) "${Label}: started streams were not joined/completed"
        foreach($start in $starts){Need (@($ends|Where-Object streamId -CEQ $start.streamId).Count -eq 1) 'Stream completion ownership differs'}
        $files=@{}
        if($Expected -eq 0){
            $tasks=@($body.plan.Tasks);$groups=@($tasks.Content|Select-Object -Unique)
            Need ($starts.Count -eq $tasks.Count -and $body.contentExecution.groupCount -eq $groups.Count) 'Only nonempty planned content groups must create owners'
            Need ((@($body.contentExecution.groups|ForEach-Object content|Sort-Object) -join ',') -ceq (@($groups|Sort-Object) -join ',')) 'Active owner groups differ from actual planner inputs'
            $resources=$body.contentExecution.resources
            Need ($resources.peakCpuTasks -le $resources.cpuCapacity -and $resources.peakIoTasks -le $resources.ioCapacity -and $resources.peakRetainedResultBytes -le $resources.byteBudget) 'Shared CPU/IO/result budget was exceeded'
            $progress=@($events|Where-Object eventType -CEQ 'Progress')
            Need ($progress.Count -eq $tasks.Count) 'Persistent Progress must remain one event per successfully completed stream'
            for($index=0;$index -lt $progress.Count;$index++){Need ($progress[$index].payload.completed -eq $index+1 -and $progress[$index].payload.total -eq $tasks.Count -and $progress[$index].payload.unit -ceq 'streams') 'Persistent Progress is not completed-stream aggregation'}
            $published=Join-Path $destination 'published';$archived=Join-Path $case 'published'
            if(Test-Path -LiteralPath $published){Copy-Item -LiteralPath $published -Destination $archived -Recurse;foreach($file in Get-ChildItem -LiteralPath $archived -Recurse -File){$files[[IO.Path]::GetRelativePath($archived,$file.FullName)]=$file.FullName}}
            Need ($files.Count -eq $(if($tasks.Count -eq 0){0}elseif($Format -eq 'big'){2}else{4*$tasks.Count+@($body.artifacts|Where-Object Kind -CEQ 'custom-data').Count})) 'Unexpected linked stream/package artifact count'
            if($Format -eq 'big' -and $tasks.Count -gt 0){Need (@($files.Keys|Where-Object{$_ -like '*.big'}).Count -eq 1 -and @($files.Keys|Where-Object{$_ -like '*.skudef'}).Count -eq 1) 'BIG/SKUDEF pair missing'}
            $sample.execution=$body.contentExecution;$sample.cache=$body.cache;$sample.overlapMs=Overlap $body.contentExecution
        }else{Need (@($body.artifacts).Count -eq 0) 'Failed/cancelled build published partial artifacts'}
        $sample.passed=$true;SaveState
        return @{files=$files;report=$body;events=$events;sample=$sample;case=$case;destination=$destination}
    } finally {
        foreach($name in @('build-report.json','build.events.jsonl')){if(Test-Path "$destination/$name"){Copy-Item "$destination/$name" $case}}
        SaveState
    }
}
try {
    Add-Type -TypeDefinition @'
using System.IO;
public static class ContentParallelBytes {
    public static bool Equal(string first,string second) {
        using(var a=File.OpenRead(first))using(var b=File.OpenRead(second)) {
            if(a.Length!=b.Length)return false;var x=new byte[65536];var y=new byte[65536];int n;
            while((n=a.Read(x,0,x.Length))!=0){int done=0;while(done<n){int read=b.Read(y,done,n-done);if(read==0)return false;done+=read;}for(int i=0;i<n;i++)if(x[i]!=y[i])return false;}return true;
        }
    }
}
'@
    $receipt=Get-Content "$product/build.json" -Raw|ConvertFrom-Json
    $script:exe=(Resolve-Path "$product/bin/NewModCompiler.exe").Path
    Need ($receipt.sourceCommit -ceq $state.sourceCommit -and $receipt.configuration -ceq 'Release' -and (Hash $script:exe) -ceq $receipt.exeSha256) 'Current Release product identity differs'
    $state.binarySourceCommit=$receipt.sourceCommit;$state.binarySha256=$receipt.exeSha256
    $vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    $vs=& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
    Need ($LASTEXITCODE -eq 0 -and !!$vs) 'VS Roslyn compiler unavailable'
    $csc=Join-Path $vs 'MSBuild/Current/Bin/Roslyn/csc.exe'
    $refs=Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
    . "$PSScriptRoot/CompilerValidation.Common.ps1"
    . "$PSScriptRoot/CompilerCacheLayout.ps1"
    $runtime=Join-Path $work 'core-runtime'
    $validation=Initialize-CompilerValidationRun $product $runtime 'Release' $state.sourceCommit
    $manifest=Get-Content "$root/recovery/build/Managed.Foundation.json" -Raw|ConvertFrom-Json
    foreach($fixture in @('CompilerPreparedBackend','CompilerCoreLifecycle','CompilerResourceLifecycle','CompilerStateLifecycle','CompilerBackendLifecycle','CompilerApplicationBuild')){
        $case=Join-Path $out $fixture;$null=New-Item -ItemType Directory -Path $case
        $fixtureExe=Join-Path $runtime ($fixture+'.exe')
        $arguments=@('/nologo','/noconfig','/nostdlib+','/target:exe','/langversion:7.3','/codepage:65001','/platform:x86','/optimize+',('/out:'+$fixtureExe),('/r:'+$validation.exe),('/r:'+(Join-Path $runtime 'antlr.runtime.dll')))
        foreach($name in @(@($manifest.frameworkReferences)+@('System.Web.Extensions')|Select-Object -Unique)){$arguments+=('/r:'+(Join-Path $refs ($name+'.dll')))}
        $arguments+=(Join-Path $root ('recovery/tests/'+$fixture+'.cs'))
        & $csc @arguments *> "$case/fixture-build.log";Need ($LASTEXITCODE -eq 0) "$fixture fixture compilation failed"
        $info=[Diagnostics.ProcessStartInfo]::new($validation.exe);$info.WorkingDirectory=$runtime;$info.UseShellExecute=$false;$info.RedirectStandardOutput=$true;$info.RedirectStandardError=$true
        foreach($argument in @($fixtureExe,(Join-Path $work $fixture),"$case/result.json",$state.sourceCommit,[string]$state.runId,(Join-Path $root 'SDK'))){$info.ArgumentList.Add($argument)}
        $process=[Diagnostics.Process]::new();$process.StartInfo=$info
        try {
            Need ($process.Start()) 'Cannot start real Core fixture';$stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
            if(!$process.WaitForExit($TimeoutSeconds*1000)){$process.Kill($true);$process.WaitForExit();throw "$fixture timed out"}
            [IO.File]::WriteAllText("$case/runtime.log",$stdout.GetAwaiter().GetResult()+$stderr.GetAwaiter().GetResult())
            Need ($process.ExitCode -eq 0) "$fixture real Core contract failed"
        } finally {$process.Dispose()}
        $body=Get-Content "$case/result.json" -Raw|ConvertFrom-Json
        Need ($body.succeeded -and $body.sourceCommit -ceq $state.sourceCommit) "$fixture assertions/source identity differ"
        Complete-CompilerValidationRun $validation
        $state.coreFixtures+=@{name=$fixture;checks=$body.checks;sourceSha256=(Hash (Join-Path $root ('recovery/tests/'+$fixture+'.cs')));validationHostSha256=$validation.validationHostSha256};SaveState
    }
    $script:control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory "$out/controller"
    $script:sdk=Join-Path $work 's';$null=New-Item -ItemType Directory -Path $script:sdk
    Get-ChildItem (Join-Path $root 'SDK') -Force|Where-Object Name -notin @('Mods','builtmods','SuperCompilerCache')|Copy-Item -Destination $script:sdk -Recurse
    $script:caseIndex=0
    $mod=Join-Path $work 'fixture';$null=New-Item -ItemType Directory -Force -Path "$mod/Data/AptUI","$mod/Data/AdditionalMaps"
    $entries=@('Data/Mod.xml','Data/AptUI/One.xml','Data/AptUI/Two.xml','Data/AdditionalMaps/MapMetaData_One.xml','Data/AdditionalMaps/MapMetaData_Two.xml')
    foreach($entry in $entries){
        $content=[Text.StringBuilder]::new('<AssetDeclaration xmlns="uri:ea.com:eala:asset">')
        $stem=[IO.Path]::GetFileNameWithoutExtension($entry)
        for($index=0;$index -lt 256;$index++){[void]$content.Append('<ArmorTemplate id="').Append($stem).Append('_').Append($index).Append('" Default="72.5%"><Armor Damage="CANNON" Percent="35.5%"/></ArmorTemplate>')}
        [void]$content.Append('</AssetDeclaration>');[IO.File]::WriteAllText((Join-Path $mod $entry),$content.ToString())
    }
    $modBefore=Snapshot $mod;$cache=Get-CompilerTestCacheRoot $script:sdk $mod
    $selections=@('base','apt','maps','base,apt','base,maps','apt,maps','base,apt,maps')
    foreach($selection in $selections){
        $label=$selection.Replace(',','-');$reference=$null
        foreach($jobs in @(1,4)){
            if(Test-Path $cache){Remove-Item -LiteralPath $cache -Recurse -Force}
            foreach($temperature in @('cold','hot')){
                $sample=RunBuild "$label-j$jobs-$temperature" $mod $selection 'loose' $jobs
                if($null -eq $reference){$reference=$sample}else{SameOutputs $reference $sample}
                if($temperature -eq 'hot'){Need ($sample.report.cache.xmlParse -eq 0 -and $sample.report.cache.instanceCompile -eq 0 -and $sample.report.cache.documentReuse -gt 0) 'Hot group partitions were not reused'}
                if($jobs -eq 4 -and $temperature -eq 'cold' -and $selection.Contains(',')){Need ($sample.sample.overlapMs -gt 0) "No actual Compile overlap for $selection"}
                foreach($group in @('base','apt','maps')|Where-Object{$_ -notin $selection.Split(',')}){foreach($part in @('session','build-cache','intermediate','streams')){Need (!(Test-Path (Join-Path $cache "$part/$group"))) "Inactive $group created a $part partition"}}
            }
            $big=RunBuild "$label-j$jobs-big" $mod $selection 'big' $jobs
            if($jobs -eq 1){$bigReference=$big}else{SameOutputs $bigReference $big}
        }
    }
    # The last selection warmed all groups. An APT-only run may not touch any
    # existing Base/Maps cache or compiled output, even though those inputs exist.
    $otherPartitions=@('session/base','session/maps','build-cache/base','build-cache/maps','intermediate/base','intermediate/maps','streams/base','streams/maps')
    $before=@{};foreach($part in $otherPartitions){$before[$part]=Snapshot (Join-Path $cache $part)}
    $on=RunBuild 'selected-apt-on' $mod 'apt' 'loose' 4
    $off=RunBuild 'selected-apt-off' $mod 'apt' 'loose' 4 -ProgressOff $true
    SameOutputs $on $off
    foreach($part in $otherPartitions){Unchanged (Join-Path $cache $part) $before[$part]}
    Unchanged $mod $modBefore
    # Existing accepted direct-FLA and file-backed XML fixtures exercise two
    # different APT frontends in the same owner, without creating Base or Maps.
    $flaMod=Join-Path $work 'fla-xml';$null=New-Item -ItemType Directory -Force -Path "$flaMod/Data/AptUI"
    $flaSource=Join-Path $root 'ui/tests/fixtures/flash8-empty.fla'
    Copy-Item -LiteralPath $flaSource -Destination "$flaMod/Data/AptUI/flash8-empty.fla"
    $aptSource=Join-Path $root 'SDK/Mods/XmlCompilerRecoveryApt/Data'
    Copy-Item -LiteralPath "$aptSource/FixturesApt.xml" -Destination "$flaMod/Data/AptUI/FileBacked.xml"
    $aptDocument=[xml][IO.File]::ReadAllText("$aptSource/FixturesApt.xml")
    $payloadNames=@($aptDocument.DocumentElement.ChildNodes|Where-Object NodeType -EQ Element|ForEach-Object{ $_.GetAttribute('File') }|Select-Object -Unique)
    foreach($name in $payloadNames){Need ([IO.Path]::GetFileName($name) -ceq $name) 'Accepted APT payload must remain beside XML';Copy-Item -LiteralPath (Join-Path $aptSource $name) -Destination (Join-Path "$flaMod/Data/AptUI" $name)}
    $flaBefore=Snapshot $flaMod;$flaCache=Get-CompilerTestCacheRoot $script:sdk $flaMod
    foreach($jobs in @(1,4)){
        if(Test-Path $flaCache){Remove-Item -LiteralPath $flaCache -Recurse -Force}
        $flaBuild=RunBuild "fla-xml-j$jobs-cold" $flaMod 'apt' 'loose' $jobs
        Need ($flaBuild.report.contentExecution.groupCount -eq 1 -and @($flaBuild.report.plan.Tasks).Count -eq 2 -and @($flaBuild.report.plan.Tasks|Where-Object Content -CNE 'apt').Count -eq 0) 'FLA/XML inputs were not kept in one APT owner'
        $wrapper=Join-Path $flaCache 'generated/fla/flash8-empty.xml'
        Need ((Test-Path -LiteralPath $wrapper -PathType Leaf) -and @(Get-ChildItem "$flaCache/generated" -Recurse -File).Count -eq 1) 'Direct FLA generated declaration escaped the APT generated/fla directory'
        foreach($group in @('base','maps')){foreach($part in @('session','build-cache','intermediate','streams')){Need (!(Test-Path (Join-Path $flaCache "$part/$group"))) "FLA/XML created inactive $group $part"}}
        foreach($artifact in @($flaBuild.report.artifacts|Where-Object{[IO.Path]::GetExtension($_.Path) -ieq '.manifest'})){
            $manifestText=[IO.File]::ReadAllText($artifact.Path)
            Need ($manifestText.IndexOf('SuperCompilerCache',[StringComparison]::OrdinalIgnoreCase) -lt 0 -and $manifestText.IndexOf((Split-Path $flaCache -Leaf),[StringComparison]::OrdinalIgnoreCase) -lt 0) 'APT manifest leaked the private wrapper cache path'
        }
        if($jobs -eq 1){$flaSerial=$flaBuild}else{SameOutputs $flaSerial $flaBuild}
        Unchanged $flaMod $flaBefore
    }
    # Copy only the same three root MapMetaData nodes used by the accepted
    # PrepareCompilerCliBusiness fixture. Do not broaden the metadata workload.
    $mapsMod=Join-Path $work 'map-business';$null=New-Item -ItemType Directory -Force -Path "$mapsMod/Data/AdditionalMaps"
    $mapSource=Join-Path $root 'SDK/Mods/XmlCompilerRecoveryA/Data/FixturesA.xml'
    $mapDocument=[xml][IO.File]::ReadAllText($mapSource)
    $mapNodes=@($mapDocument.SelectNodes("/*[local-name()='AssetDeclaration']/*[local-name()='MapMetaData']"))
    Need ($mapNodes.Count -eq 3) 'Accepted MapMetaData fixture inventory changed'
    $mapDeclaration=[xml]'<AssetDeclaration xmlns="uri:ea.com:eala:asset"/>'
    foreach($node in $mapNodes){[void]$mapDeclaration.DocumentElement.AppendChild($mapDeclaration.ImportNode($node,$true))}
    foreach($name in @('One','Two')){$mapDeclaration.Save("$mapsMod/Data/AdditionalMaps/MapMetaData_Business_$name.xml")}
    $mapsBefore=Snapshot $mapsMod;$mapsCache=Get-CompilerTestCacheRoot $script:sdk $mapsMod
    foreach($jobs in @(1,4)){
        if(Test-Path $mapsCache){Remove-Item -LiteralPath $mapsCache -Recurse -Force}
        $mapsBuild=RunBuild "maps-business-j$jobs-cold" $mapsMod 'maps' 'loose' $jobs
        Need ($mapsBuild.report.contentExecution.groupCount -eq 1 -and @($mapsBuild.report.plan.Tasks).Count -eq 2 -and @($mapsBuild.report.plan.Tasks|Where-Object Content -CNE 'maps').Count -eq 0) 'MapMetaData inputs were not kept in one Maps owner'
        foreach($group in @('base','apt')){foreach($part in @('session','build-cache','intermediate','streams')){Need (!(Test-Path (Join-Path $mapsCache "$part/$group"))) "MapMetaData created inactive $group $part"}}
        Need (!(Test-Path (Join-Path $mapsCache 'generated'))) 'Maps generated an APT wrapper directory'
        if($jobs -eq 1){$mapsSerial=$mapsBuild}else{SameOutputs $mapsSerial $mapsBuild}
        Unchanged $mapsMod $mapsBefore
    }
    $state.targetedContent=@{apt=@{flaSource='ui/tests/fixtures/flash8-empty.fla';flaSha256=(Hash $flaSource);xmlSource='SDK/Mods/XmlCompilerRecoveryApt/Data/FixturesApt.xml';xmlSha256=(Hash "$aptSource/FixturesApt.xml");entryCount=2;scope='One accepted empty Flash8 project plus twelve accepted file-backed APT DataBlob assets; no full UI or game playback'};maps=@{source='SDK/Mods/XmlCompilerRecoveryA/Data/FixturesA.xml';sourceSha256=(Hash $mapSource);rootAssetsPerEntry=3;entryCount=2;scope='Existing XSD-valid MapMetaData fixture roots only; not full map business acceptance'}}
    SaveState
    $empty=Join-Path $work 'empty';$null=New-Item -ItemType Directory -Path "$empty/Data"
    $autoEmpty=RunBuild 'auto-empty' $empty 'auto' 'loose' 4 -Expected 2
    Need (@($autoEmpty.report.diagnostics|Where-Object Code -CEQ 'PLAN_NO_INPUTS').Count -eq 1) 'Automatic empty discovery changed'
    $missingBase=RunBuild 'missing-base' $empty 'base' 'loose' 4 -Expected 2
    Need (@($missingBase.report.diagnostics|Where-Object Code -CEQ 'PLAN_BASE_MISSING').Count -eq 1) 'Explicit missing Base changed'
    $emptyOptional=RunBuild 'empty-optional' $empty 'apt,maps' 'big' 4
    Need ($emptyOptional.files.Count -eq 0 -and $emptyOptional.report.contentExecution.groupCount -eq 0) 'Explicit empty optional groups created owners/BIG'
    $onlyApt=Join-Path $work 'only-apt';$null=New-Item -ItemType Directory -Force -Path "$onlyApt/Data/AptUI"
    Copy-Item "$mod/Data/AptUI/One.xml" "$onlyApt/Data/AptUI/One.xml"
    $automatic=RunBuild 'auto-only-apt' $onlyApt 'auto' 'loose' 4
    Need ($automatic.report.contentExecution.groupCount -eq 1 -and $automatic.report.plan.Tasks[0].Content -ceq 'apt') 'Auto optional selection introduced another owner'
    $neutral=RunBuild 'no-base-neutral' $onlyApt 'auto' 'big' 4 -Expected 2 -Extra @('--fix-neutral-assets=true')
    Need (@($neutral.report.diagnostics|Where-Object Code -CEQ 'PACKAGE_NEUTRAL_CONTENT').Count -eq 1) 'Neutral repair lost its actual Base requirement'
    $bad=Join-Path $work 'bad';$null=New-Item -ItemType Directory -Force -Path "$bad/Data/AptUI","$bad/Data/AdditionalMaps"
    [IO.File]::WriteAllText("$bad/Data/Mod.xml",'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><UnsupportedContent id="bad"/></AssetDeclaration>')
    Copy-Item "$mod/Data/AptUI/One.xml" "$bad/Data/AptUI/One.xml";Copy-Item "$mod/Data/AdditionalMaps/MapMetaData_One.xml" "$bad/Data/AdditionalMaps/MapMetaData_One.xml"
    $failed=RunBuild 'failed-group' $bad 'base,apt,maps' 'big' 4 -Expected 4
    $cancelled=RunBuild 'actual-ctrlbreak' $mod 'base,apt,maps' 'loose' 4 -Expected 6 -Extra @('--sc=false','--bc=false') -Cancel $true
    $retry=RunBuild 'retry-after-cancel' $mod 'base,apt,maps' 'loose' 4
    SameOutputs $reference $retry
    # Exactly one targeted complete historical Upheaval jobs1/4 pair. No legacy
    # benchmark launcher, default sample matrix, warm-ups or repeated pairs.
    $historical='bacfc69fc3e71c8116d1f22b933b8b6173170ecf';$tree='5551fc26a6f96b62613443e0d0fa2958c14cbce0'
    Need ((git -C $root rev-parse ($historical+':SDK/Mods/Upheaval')).Trim() -ceq $tree) 'Pinned Upheaval input identity differs'
    $archive=Join-Path $work 'upheaval.tar';git -C $root archive --output=$archive $historical 'SDK/Mods/Upheaval';Need ($LASTEXITCODE -eq 0) 'Upheaval archive failed'
    $external=Join-Path $work 'external';$null=New-Item -ItemType Directory -Path $external
    tar -xf $archive -C $external;Need ($LASTEXITCODE -eq 0) 'Upheaval extraction failed';Remove-Item $archive
    $upheaval=Join-Path $external 'SDK/Mods/Upheaval';$upheavalCache=Get-CompilerTestCacheRoot $script:sdk $upheaval
    $state.upheaval=@{sourceCommit=$historical;sourceTree=$tree;selection='base';files=@(Get-ChildItem $upheaval -Recurse -File).Count;pairCount=1}
    $serial=RunBuild 'upheaval-j1-cold' $upheaval 'base' 'loose' 1
    if(Test-Path $upheavalCache){Remove-Item -LiteralPath $upheavalCache -Recurse -Force}
    $parallel=RunBuild 'upheaval-j4-cold' $upheaval 'base' 'loose' 4
    SameOutputs $serial $parallel
    $state.status='passed';$state.succeeded=$true;SaveState
} catch {$state.status='failed';$state.error=$_.Exception.ToString();SaveState;throw}
finally {$env:NEWSDK_PROGRESS=$oldProgress;SaveState}
