#requires -Version 7.0
[CmdletBinding()]
param(
    [string]$CompilerDirectory,
    [string]$ReplayReceipt,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(15,600)][int]$TimeoutSeconds=180,
    [switch]$Preflight
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(!$Preflight){$CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path}
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite build-cache evidence: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$short=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('c'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
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
    foreach($key in $Left.files.Keys){Need ($Right.files.ContainsKey($key)) "Changed fixture omitted $key";if((Hash $Left.files[$key]) -cne (Hash $Right.files[$key])){$changed=$true}}
    Need $changed 'Input mutation did not exercise any observable compiled output change'
}
function CacheFiles([string]$Destination){
    $files=@{};$cache=Join-Path $Destination '.cache'
    if(Test-Path -LiteralPath $cache){Get-ChildItem -LiteralPath $cache -File -Recurse|ForEach-Object{$files[$_.FullName.Substring($cache.Length)]=Hash $_.FullName}}
    return $files
}
function CacheUnchanged([string]$Destination,$Before){
    $after=CacheFiles $Destination;Need ($after.Count -eq $Before.Count) 'Failed/cancelled build persisted additional asset-cache files'
    foreach($key in $Before.Keys){Need ($after.ContainsKey($key) -and $after[$key] -ceq $Before[$key]) 'Failed/cancelled build changed previously confirmed asset-cache bytes'}
}
function ReasonCount($Reasons,[string]$Name){
    $property=$Reasons.PSObject.Properties[$Name]
    if($null -eq $property){return 0L}
    return [long]$property.Value
}
function PhaseTimings($Statistics,[bool]$Succeeded,[bool]$Enabled){
    $timings=$Statistics.phaseTimings
    Need ($null -ne $timings -and $timings.format -ceq 'NewModCompiler.CachePhaseTimings.v1' -and $timings.semantics -ceq 'inclusive-overlapping-ticks') 'Cache report omitted its versioned inclusive phase timing contract'
    Need (($timings.frequency -is [int] -or $timings.frequency -is [long]) -and $timings.frequency -gt 0 -and $timings.frequency -eq $Statistics.intervalFrequency) 'Phase timings and backend intervals must use the same monotonic clock frequency'
    foreach($name in @('coreCompileTicks','streamSetupTicks','streamCleanupTicks','inputInventoryTicks','inputEnumerationTicks','inputReadTicks','inputDigestTicks','processContractTicks','documentProcessTicks','artifactCollectionTicks','preparationTicks','actionKeyTicks','resultRestoreTicks','resultWriteTicks','resultCommitTicks','workerWaitTicks','backendWorkTicks','storeLookupTicks','storeValidationTicks','storeMaterializationTicks','storeStageTicks','cacheCommitTicks')){
        $property=$timings.PSObject.Properties[$name]
        Need ($null -ne $property -and ($property.Value -is [int] -or $property.Value -is [long]) -and $property.Value -ge 0) "Cache phase timing must be a nonnegative integer tick count: $name"
    }
    $intervals=@($Statistics.backendIntervals);$recordedTicks=0L
    foreach($interval in $intervals){
        Need ($interval.endTicks -ge $interval.startTicks) 'A real backend interval has a negative duration'
        $recordedTicks+=[long]$interval.endTicks-[long]$interval.startTicks
    }
    if($Statistics.backendCalls -le 512){
        Need ($intervals.Count -eq $Statistics.backendCalls -and $timings.backendWorkTicks -eq $recordedTicks) 'Complete backend intervals and aggregate real backend work ticks disagree'
    }else{
        Need ($timings.backendWorkTicks -ge $recordedTicks) 'Aggregate backend work omitted some retained interval duration'
    }
    # These scopes overlap: document work contains owner preparation/commit,
    # lookup contains validation/materialization, and backend work can overlap
    # across workers. Never add them together or impose wall-clock thresholds.
    if($Succeeded){
        foreach($name in @('documentProcessTicks','artifactCollectionTicks','preparationTicks','resultWriteTicks','resultCommitTicks','backendWorkTicks')){
            Need ($timings.$name -gt 0) "Successful real fixture omitted active phase work: $name"
        }
        if($Enabled){
            foreach($name in @('inputInventoryTicks','inputEnumerationTicks','inputReadTicks','inputDigestTicks','processContractTicks','actionKeyTicks','storeLookupTicks','storeValidationTicks')){
                Need ($timings.$name -gt 0) "Successful cached fixture omitted active phase work: $name"
            }
        }
    }
    if(!$Enabled){
        foreach($name in @('inputInventoryTicks','inputEnumerationTicks','inputReadTicks','inputDigestTicks','processContractTicks','storeLookupTicks','storeValidationTicks','storeMaterializationTicks','storeStageTicks')){
            Need ($timings.$name -eq 0) "Disabled cache invented inventory or store work: $name"
        }
    }
}
function ReasonAccounting($Statistics){
    Need ($null -ne $Statistics.actionMissReasons -and $null -ne $Statistics.cacheBypassReasons) 'Cache report omitted miss/bypass reason dictionaries'
    Need ($null -ne $Statistics.preparationRejectedReasons) 'Cache report omitted actual preparation rejection reasons'
    Need ((ReasonCount $Statistics.preparationRejectedReasons 'uncertified-plugin') -eq 1 -and @($Statistics.preparationRejectedReasons.PSObject.Properties).Count -eq 1) 'The sole Texture rejection must identify its actual unrecognized provider in cache and no-cache modes'
    $sum=0L
    foreach($property in $Statistics.actionMissReasons.PSObject.Properties){
        Need (@('entry-not-found','entry-invalidated','entry-pending','invalid-manifest','invalid-payload','entry-unavailable','invalid-result-contract') -ccontains $property.Name) 'Cache reported an unknown miss reason'
        Need (($property.Value -is [int] -or $property.Value -is [long]) -and $property.Value -gt 0) 'Miss reason counters must be positive actual lookup counts'
        $sum+=[long]$property.Value
    }
    Need ($sum -eq $Statistics.misses -and $sum -eq $Statistics.actionMisses) 'Reason counts changed or double-counted existing certified-action miss semantics'
    foreach($property in $Statistics.cacheBypassReasons.PSObject.Properties){
        Need (@('not-prepared','cache-disabled','base-patch-source') -ccontains $property.Name) 'Cache reported an unknown bypass reason'
        Need (($property.Value -is [int] -or $property.Value -is [long]) -and $property.Value -gt 0) 'Bypass counters must describe actual decisions independently of lookup misses'
    }
}
function Cache($Case,[string]$Expected){
    $statistics=$Case.report.cache
    Need ($null -ne $statistics) 'Durable report omitted actual asset-cache statistics'
    Need ($statistics.executionEngine -ceq 'AssetTasks.v1') 'Build did not use the shared asset executor'
    ReasonAccounting $statistics
    if($Expected -eq 'disabled'){
        Need ($statistics.cacheMode -ceq 'Disabled' -and $statistics.hits -eq 0 -and $statistics.misses -eq 0 -and $statistics.actionHits -eq 0) 'Disabled cache recorded reuse'
        Need ((ReasonCount $statistics.cacheBypassReasons 'cache-disabled') -gt 0) 'No-cache build omitted actual certified-action bypass decisions'
        return
    }
    Need ($statistics.status -ceq 'ActionCache' -and $statistics.cacheMode -ceq 'ActionCache') 'Build cache was accepted without the actual action-cache implementation'
    Need ((ReasonCount $statistics.cacheBypassReasons 'not-prepared') -eq 1) 'The single noncertified Texture must remain an unprepared bypass rather than a lookup miss'
    if($Expected -eq 'cold'){Need ($statistics.hits -eq 0 -and $statistics.misses -gt 0) 'Cold asset cache did not record real misses'}
    elseif($Expected -eq 'hot'){Need ($statistics.hits -gt 0 -and $statistics.misses -eq 0) 'Hot asset cache did not reuse all fixture assets'}
    elseif($Expected -eq 'partial'){Need ($statistics.misses -eq 1 -and $statistics.hits -gt 0) 'A single changed action did not preserve unrelated certified asset hits'}
    else{throw "Unknown cache expectation: $Expected"}
    Need (($Expected -eq 'cold' -and $statistics.restoredBytes -eq 0) -or ($Expected -ne 'cold' -and $statistics.restoredBytes -gt 0)) 'Restored-byte accounting must describe complete consumed hit payloads, including after private file moves'
    if($Expected -eq 'hot'){
        Need ($statistics.phaseTimings.storeStageTicks -eq 0) 'Fully hot actions unexpectedly staged new cache payloads'
        Need ($statistics.phaseTimings.resultRestoreTicks -gt 0 -and $statistics.phaseTimings.storeMaterializationTicks -gt 0) 'Hot cache omitted actual validated result materialization timing'
    }else{
        Need ($statistics.phaseTimings.storeStageTicks -gt 0) 'Real cache misses omitted candidate staging timing'
    }
}
function SameLengthMutation([string]$Path,[string]$From,[string]$To){
    $before=[IO.File]::ReadAllBytes($Path);$stamp=[IO.File]::GetLastWriteTimeUtc($Path)
    # Match raw UTF-8 bytes to retain BOM, line endings and all other bytes.
    $find=[Text.Encoding]::UTF8.GetBytes($From);$replace=[Text.Encoding]::UTF8.GetBytes($To)
    Need ($find.Length -eq $replace.Length -and $find.Length -gt 0) 'Mutation tokens must have identical UTF-8 lengths'
    $offset=-1
    for($i=0;$i -le $before.Length-$find.Length;$i++){
        $match=$true;for($j=0;$j -lt $find.Length;$j++){if($before[$i+$j] -ne $find[$j]){$match=$false;break}}
        if($match){$offset=$i;break}
    }
    Need ($offset -ge 0) "Mutation token absent: $Path"
    [Array]::Copy($replace,0,$before,$offset,$replace.Length)
    [IO.File]::WriteAllBytes($Path,$before)
    [IO.File]::SetLastWriteTimeUtc($Path,$stamp)
    Need (([IO.File]::ReadAllBytes($Path)).Length -eq $before.Length -and [IO.File]::GetLastWriteTimeUtc($Path) -eq $stamp) 'Mutation did not preserve exact input length and timestamp'
}
function MakeMod([string]$Path,[string]$Probe,[string]$TexturePath=$null){
    New-Item -ItemType Directory -Path "$Path/Data/AptUI" -Force|Out-Null
    [IO.File]::WriteAllText("$Path/Data/Armor.xml",$Probe,[Text.UTF8Encoding]::new($false))
    # XML/Armor, PathMusicEvent and file-backed APT exercise independent action
    # identities. Texture remains a real uncacheable compatibility call.
    [IO.File]::WriteAllText("$Path/Data/Mod.xml",'<AssetDeclaration xmlns="uri:ea.com:eala:asset" xmlns:xi="http://www.w3.org/2001/XInclude"><Includes><Include type="all" source="Armor.xml"/></Includes><xi:include href="XArmor.xml"/><WeaponTemplate id="BuildCacheWeapon" AttackRange="101"/><GameObject id="BuildCacheObject" ThreatLevel="1.0"><Body><InactiveBody id="BuildCacheBody"/></Body></GameObject><Texture id="BuildCacheTexture" File="cache.tga" Type="StandardTexture" GenerateMipMaps="false" OutputFormat="A8R8G8B8" AllowAutomaticResize="false"/><PathMusicEvent id="CacheEvent" PathfinderEventHeader="events.h" IsXferable="true"/></AssetDeclaration>')
    [IO.File]::WriteAllText("$Path/Data/XArmor.xml",'<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="XIncludeArmor" Default="22.5%"/>')
    [IO.File]::WriteAllText("$Path/Data/events.h","#define PATH_EVENT_CacheEvent 0x12345678`r`n")
    & python "$root/recovery/probes/TextureRedirect/MakeFixture.py" "$Path/Data/cache.tga"
    Need ($LASTEXITCODE -eq 0) 'Build-cache deterministic Texture fixture preparation failed'
    if($TexturePath){
        New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($TexturePath))|Out-Null
        # A second MOD shares the current SDK texture version. Do not undo the
        # earlier same-length/mtime pixel edit by copying the original again.
        if(!(Test-Path -LiteralPath $TexturePath)){Copy-Item -LiteralPath "$Path/Data/cache.tga" -Destination $TexturePath}
        $xml=[IO.File]::ReadAllText("$Path/Data/Mod.xml")
        Need ($xml.Contains('File="cache.tga"')) 'Texture SDK-path substitution token is missing'
        [IO.File]::WriteAllText("$Path/Data/Mod.xml",$xml.Replace('File="cache.tga"',('File="'+[Security.SecurityElement]::Escape($TexturePath)+'"')))
    }
    [IO.File]::WriteAllText("$Path/Data/AptUI/Payload.xml",'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AptAptData id="BuildCachePayload" File="payload.bin"/></AssetDeclaration>')
    [IO.File]::WriteAllBytes("$Path/Data/AptUI/payload.bin",[byte[]](1,2,3,4,5,6,7))
}
function RunCli([string]$Name,[string]$Mod,[string]$Destination,[bool]$Enabled,[int]$Expected=0,[bool]$Cancel=$false,[string]$HoldReadFile=$null,[string[]]$Extra=@()){
    $caseOut=(New-Item -ItemType Directory -Path (Join-Path $script:modeOut $Name)).FullName
    New-Item -ItemType Directory -Force -Path $Destination|Out-Null
    $arguments=@('build','--mod',$Mod,'--sdk-root',$script:sdk,'--out',$Destination,'--package-format','loose','--package-content','base,apt',('--bc='+$Enabled.ToString().ToLowerInvariant()))+$Extra
    $events=Join-Path $Destination 'build.events.jsonl';$report=Join-Path $Destination 'build-report.json'
    $request=@{exe=$script:exe;cwd=$script:runtime;arguments=$arguments;stdout=(Join-Path $caseOut 'stdout.log');stderr=(Join-Path $caseOut 'stderr.log');events=$events;cancel=$Cancel;timeoutSeconds=$TimeoutSeconds}
    if($HoldReadFile){$request.holdReadFile=$HoldReadFile}
    $request|ConvertTo-Json -Depth 6|Set-Content "$caseOut/request.json"
    & $script:control "$caseOut/request.json" "$caseOut/control.json" *> "$caseOut/controller.log"
    $exit=$LASTEXITCODE;$audit=Get-Content "$caseOut/control.json" -Raw|ConvertFrom-Json
    $entry=@{name=$Name;arguments=$arguments;expectedExit=$Expected;passed=$false;controller=$audit};$script:modeState.cases+=,$entry
    try{
        Need ($exit -eq 0 -and $audit.succeeded -and $audit.processExitCode -eq $Expected) "Build-cache CLI failed: $Name"
        Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) 'Asset cache introduced a product child process'
        if($Cancel){Need ($audit.signalSent -and $audit.freshEventsPathObserved -and $audit.streamStartedObserved -and $Expected -eq 6) 'Cache cancellation did not receive actual CtrlBreak'}
        $body=Get-Content -LiteralPath $report -Raw|ConvertFrom-Json
        Need ($body.exitCode -eq $Expected) 'Cache process/report exit differs'
        PhaseTimings $body.cache ($Expected -eq 0) $Enabled
        $rows=@(Get-Content -LiteralPath $events|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})
        Need ($rows[-1].eventType -ceq 'BuildFinished' -and @($rows|Where-Object{$_.eventType -ceq 'BuildStarted'}).Count -eq 1 -and @($rows|Where-Object{$_.eventType -ceq 'BuildFinished'}).Count -eq 1) 'Cache CLI terminal protocol differs'
        $sequence=0;foreach($row in $rows){$sequence++;Need ($row.buildId -ceq $body.buildId -and $row.sequence -eq $sequence) 'Cache CLI build identity/sequence differs'}
        Need ($rows[0].payload.processId -eq $audit.processId -and @($body.externalTools).Count -eq 0) 'Cache CLI production identity/external tools differs'
        if($Cancel){Need ($audit.observedBuildStarted.buildId -ceq $body.buildId -and $audit.observedBuildStarted.payload.processId -eq $audit.processId -and $audit.observedStreamStarted.buildId -ceq $body.buildId) 'CtrlBreak observed a stale cached build generation'}
        $files=@{};$archived=Join-Path $caseOut 'artifacts';New-Item -ItemType Directory -Path $archived|Out-Null
        if($Expected -eq 0){
            Need ($body.publicationStatus -ceq 'PublishedDirectory' -and @($body.artifacts).Count -eq 8) 'Cache fixture did not publish exactly two four-file streams'
            if($Enabled){
                Need ($body.cache.inputInventoryCaptures -eq 4) 'Two cached streams must preserve both initial and final whole-root inventories'
                Need ($body.cache.inputHashLeaseFiles -gt 0 -and $body.cache.inputHashLeaseFiles -le 512) 'Tool digest sharing omitted its bounded retained-file count'
                Need ($body.cache.inputHashReusedBytes -gt 0 -and $body.cache.hashReadBytes -gt 0) 'Shared tool digests must report avoided reads while ordinary input and payload reads remain counted'
            }else{
                Need ($body.cache.inputInventoryCaptures -eq 0 -and $body.cache.inputHashReusedBytes -eq 0 -and $body.cache.inputHashLeaseFiles -eq 0) 'Disabled cache invented input inventory or shared digest work'
            }
            $published=Join-Path $Destination 'published'
            foreach($artifact in $body.artifacts){
                Need ((Within $published $artifact.Path) -and (Hash $artifact.Path) -ceq $artifact.Sha256 -and (Get-Item -LiteralPath $artifact.Path).Length -eq $artifact.Size) 'Cached artifact identity/containment differs'
                $relative=[IO.Path]::GetRelativePath($published,$artifact.Path);$copy=Join-Path $archived $relative
                New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($copy))|Out-Null
                Copy-Item -LiteralPath $artifact.Path -Destination $copy;$files[$relative]=$copy
            }
        }else{Need (@($body.artifacts).Count -eq 0) 'Failed/cancelled cache build accepted artifacts'}
        Need (!(Test-Path -LiteralPath (Join-Path $Destination ('.build/'+$body.buildId)))) 'Cache build retained owned staging'
        Need (@(Get-ChildItem -LiteralPath $Destination -Directory|Where-Object{$_.Name -like '.cache-candidate-*' -or $_.Name -like '.action-cache-candidate-*'}).Count -eq 0) 'Asset-cache transaction retained its candidate directory'
        $entry.passed=$true;$entry.cache=$body.cache;$entry.buildId=$body.buildId
        return @{name=$Name;report=$body;files=$files;mod=$Mod;caseOut=$caseOut;destination=$Destination}
    }catch{
        foreach($name in @('stdout.log','stderr.log','controller.log')){Get-Content -LiteralPath (Join-Path $caseOut $name) -Tail 30 -ErrorAction SilentlyContinue|Out-Host}
        throw
    }finally{
        if(Test-Path -LiteralPath $report){Copy-Item -LiteralPath $report -Destination "$caseOut/build-report.json"}
        if(Test-Path -LiteralPath $events){Copy-Item -LiteralPath $events -Destination "$caseOut/build.events.jsonl"}
    }
}
function OriginalOracle($Case,[string]$Name){
    $baseline=@();$index=0
    foreach($task in $Case.report.plan.Tasks){
        $outputs=@{};$prefix=$task.Content+'/'+[IO.Path]::GetFileNameWithoutExtension($task.InputPath)
        foreach($key in $Case.files.Keys){if($key.Replace('\','/').StartsWith($prefix+'/',[StringComparison]::Ordinal)){$outputs[[IO.Path]::GetExtension($key).Substring(1)]=$Case.files[$key]}}
        Need ($outputs.Count -eq 4) 'Cache original oracle requires exact four files per task'
        $baseline+=,@{name=$(if($index++ -eq 0){'mod-a'}else{'mod-b'});modRoot=$Case.mod;inputPath=$task.InputPath;sdkRoot=$script:sdk;originalToolsRoot=(Join-Path $root 'SDK/tools');stagingDirectory=([IO.Path]::GetDirectoryName($outputs.manifest));outputs=$outputs;expectedTextures=$(if($task.Content -ceq 'base'){@('Texture:BuildCacheTexture')}else{@()});scope='Current build-cache fixture, strict original BAB raw four-output comparison; no normalization'}
    }
    $fixture=Join-Path $script:modeOut "$Name.json"
    @{succeeded=$true;sourceCommit=$script:state.sourceCommit;runId=$env:GITHUB_RUN_ID;baselineCases=$baseline;scope='One Armor/include stream and one file-backed APT stream, not full SDK'}|ConvertTo-Json -Depth 9|Set-Content $fixture
    & "$PSScriptRoot/TestCompilerBackendBaseline.ps1" -FixtureResult $fixture -OutputDirectory (Join-Path $script:modeOut $Name) -TimeoutSeconds $TimeoutSeconds
    Need ($LASTEXITCODE -eq 0) 'Build-cache independent original BAB oracle failed'
}
$state=@{schemaVersion=1;succeeded=$false;sourceCommit=(& git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;modes=@();scope='Actual production action-cache CLI acceptance: XML/Armor, Weapon, GameObject, PathMusicEvent and file-backed APT with noncached Texture; exact single-action invalidation, unread Additional, complete raw four outputs, original BAB, same-length/same-mtime changes, MOD reuse boundaries, bounded cross-stream stable SDK/runtime digest leases, real SDK/Art native Texture reads and unused SageXml payload reuse with all four root inventories preserved, actual lookup-miss reason accounting and independent disabled/unprepared bypasses, corrupt and pending entry repair, actual cancellation and failure/retry; /sc and full SDK remain separate'}
try{
    Need ($state.sourceCommit -cmatch '^[0-9a-f]{40}$') 'Cannot identify source commit'
    if($Preflight){
        $probe=[IO.File]::ReadAllText("$root/SDK/Mods/mymod/Data/RecoveryArmorProbe.xml")
        MakeMod (Join-Path $short 'm') $probe (Join-Path $short 's/Art/cache.tga')
        $check=@'
import pathlib, runpy, sys
from lxml import etree as E
scripts=pathlib.Path(sys.argv[1]); work=pathlib.Path(sys.argv[2])/'m'
module=runpy.run_path(str(scripts/'GenerateXmlSdkModA.py'), run_name='action_cache_schema')
schema=module['aggregate_schema'](); ns={'ea':'uri:ea.com:eala:asset'}
main=E.parse(str(work/'Data/Mod.xml')); main.xinclude(); schema.assertValid(main)
for name in ('Data/Armor.xml','Data/AptUI/Payload.xml'):
    schema.assertValid(E.parse(str(work/name)))
assert len(main.xpath('/ea:AssetDeclaration/ea:GameObject/ea:Body/ea:InactiveBody[@id="BuildCacheBody"]',namespaces=ns))==1
assert len(main.xpath('/ea:AssetDeclaration/ea:WeaponTemplate[@id="BuildCacheWeapon"]',namespaces=ns))==1
assert b'AttackRange="101"' in (work/'Data/Mod.xml').read_bytes()
assert b'ThreatLevel="1.0"' in (work/'Data/Mod.xml').read_bytes()
assert b'72.5%' in (work/'Data/Armor.xml').read_bytes()
assert b'22.5%' in (work/'Data/XArmor.xml').read_bytes()
assert b'0x12345678' in (work/'Data/events.h').read_bytes()
assert len((work/'Data/AptUI/payload.bin').read_bytes())==7
assert b'monitorPaths="..\\"' in (module['ROOT']/'SDK/tools/BinaryAssetBuilder.exe.config').read_bytes()
assert b'elementFormDefault="qualified"' in (module['S']/'AssetTypeTexture.xsd').read_bytes()
texture=(work.parent/'s/Art/cache.tga').read_bytes()
assert texture==(work/'Data/cache.tga').read_bytes()
assert texture[2]==2 and int.from_bytes(texture[12:14],'little')==256 and int.from_bytes(texture[14:16],'little')==256 and texture[16]==32
assert len(texture)==18+256*256*4
texture_paths=main.xpath('/ea:AssetDeclaration/ea:Texture/@File',namespaces=ns)
assert len(texture_paths)==1 and pathlib.Path(texture_paths[0]).resolve()==(work.parent/'s/Art/cache.tga').resolve()
# The real SDK texture is larger than every XSD. Allow five deployed runtime
# files and one config; it must survive the production 512-largest lease cap.
assert sum(path.stat().st_size>=len(texture) for path in module['S'].rglob('*') if path.is_file())+6 < 512

print('PASS action-cache fixture XSD, required GameObject Body, Weapon/Armor/XInclude, external reads and mutation tokens')
'@
        & python -c $check $PSScriptRoot $short *> "$out/fixture-preflight.log"
        if($LASTEXITCODE -ne 0){Get-Content "$out/fixture-preflight.log" -Tail 30|Out-Host}
        Need ($LASTEXITCODE -eq 0) 'Action-cache fixture preflight failed'
        $state.preflight=$true;$state.scope='Source-XSD and mutation-token fixture preflight only; no production execution';$state.succeeded=$true;$global:LASTEXITCODE=0
        return
    }
    Need (!(Test-Path -LiteralPath (Join-Path $CompilerDirectory 'reuse.json'))) 'Build-cache acceptance requires current production EXE bytes'

    $state.testSourceCommit=$state.sourceCommit;$state.binarySourceCommit=$state.sourceCommit
    $replayFiles=@{};$replayPath=Join-Path $CompilerDirectory 'candidate-replay.json'
    $successfulReplay=Join-Path $CompilerDirectory 'cache-payload-replay.json'
    Need ($ReplayReceipt -or !(Test-Path -LiteralPath $successfulReplay)) 'Successful archive reuse requires explicit ReplayReceipt'
    if($ReplayReceipt){
        Need (!(Test-Path -LiteralPath $replayPath)) 'Cannot combine fixed failed-test replay and successful archive replay'
        $ReplayReceipt=(Resolve-Path -LiteralPath $ReplayReceipt).Path
        Need ($ReplayReceipt -ceq $successfulReplay) 'ReplayReceipt must belong to this prepared runtime directory'
        & python "$PSScriptRoot/PrepareCompilerCacheReplay.py" --repo $root --candidate-commit $state.sourceCommit --verify-prepared $ReplayReceipt *> "$out/replay-verification.log"
        Need ($LASTEXITCODE -eq 0) 'Successful archive API, closure, receipts or runtime verification rejected'
        $replay=Get-Content -LiteralPath $ReplayReceipt -Raw|ConvertFrom-Json
        Need ($replay.prepared -eq $true -and $replay.candidateValidated -eq $false -and $replay.productionSourcesUnchanged -eq $true -and $replay.testSourceCommit -ceq $state.sourceCommit -and $replay.buildCacheTestScriptSha256 -ceq (Hash $PSCommandPath)) 'Successful replay source or current test script identity differs'
        $expectedFiles=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
        foreach($configuration in @('Debug','Release')){foreach($fileName in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){$null=$expectedFiles.Add($configuration+'/bin/'+$fileName)}}
        Need (@($replay.files).Count -eq 6) 'Successful replay requires exactly six runtime identities'
        foreach($identity in $replay.files){
            Need ($expectedFiles.Remove([string]$identity.path) -and $identity.sha256 -cmatch '^[0-9a-f]{64}$' -and (Hash (Join-Path $CompilerDirectory $identity.path)) -ceq $identity.sha256) 'Successful replay runtime identity or bytes differ'
            $replayFiles[[string]$identity.path]=[string]$identity.sha256
        }
        Need ($expectedFiles.Count -eq 0) 'Successful replay runtime set is incomplete'
        $state.binarySourceCommit=$replay.binarySourceCommit;$state.successfulArchiveReplay=$replay
        $state.scope+='; successful archived binarySourceCommit with current testSourceCommit; production inputs unchanged and no production rebuild'
    }
    if(Test-Path -LiteralPath $replayPath){
        $replay=Get-Content -LiteralPath $replayPath -Raw|ConvertFrom-Json
        $fixedSource='fb6f67a0aaf5889bcd950485c41a883bc5807523'
        Need ($replay.prepared -eq $true -and $replay.candidateValidated -eq $false -and $replay.productionSourcesUnchanged -eq $true -and $replay.sourceCommit -ceq $fixedSource -and $replay.binarySourceCommit -ceq $fixedSource -and $replay.candidateCommit -ceq $state.sourceCommit -and $replay.testSourceCommit -ceq $state.sourceCommit -and [string]$replay.runId -ceq '40360344360' -and [string]$replay.runAttempt -ceq '1' -and $replay.archiveSha256 -ceq 'd1308d63759466789eb657ac3389a78f2274c52d261b0e078144923af435d8de') 'Candidate replay identity or preparation contract differs'
        # The fixed archive checker verifies API status, successful scopes and
        # raw receipts. Independently re-evaluate unchanged production inputs.
        $closureCode='import sys;sys.path.insert(0,sys.argv[1]);from PrepareCompilerReplay import closure;closure(sys.argv[2],sys.argv[3],sys.argv[4])'
        & python -c $closureCode $PSScriptRoot $root $fixedSource $state.sourceCommit *> "$out/replay-closure.log"
        Need ($LASTEXITCODE -eq 0) 'Candidate replay production-source closure rejected'
        $expectedFiles=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
        foreach($configuration in @('Debug','Release')){foreach($fileName in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){$null=$expectedFiles.Add($configuration+'/bin/'+$fileName)}}
        Need (@($replay.files).Count -eq 6) 'Candidate replay requires exactly six runtime identities'
        foreach($identity in $replay.files){
            Need ($expectedFiles.Remove([string]$identity.path) -and $identity.sha256 -cmatch '^[0-9a-f]{64}$') 'Duplicate or unexpected candidate replay runtime identity'
            Need ((Hash (Join-Path $CompilerDirectory $identity.path)) -ceq $identity.sha256) 'Candidate replay runtime bytes differ from validated archive receipt'
            $replayFiles[[string]$identity.path]=[string]$identity.sha256
        }
        Need ($expectedFiles.Count -eq 0) 'Candidate replay runtime set is incomplete'
        $state.binarySourceCommit=$fixedSource;$state.candidateReplay=$replay
        $state.scope+='; fixed binarySourceCommit replay with separately recorded current testSourceCommit and unchanged production inputs; this is not a current production rebuild'
    }
    $control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'controller')
    $state.scriptSha256=Hash $PSCommandPath;$state.controllerSha256=Hash $control
    foreach($mode in @('Debug','Release')){
        $script:modeOut=(New-Item -ItemType Directory -Path (Join-Path $out $mode)).FullName
        $modeWork=(New-Item -ItemType Directory -Path (Join-Path $short $mode.Substring(0,1))).FullName
        $script:runtime=(New-Item -ItemType Directory -Path (Join-Path $modeWork 'r')).FullName
        foreach($file in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll')){
            Copy-Item -LiteralPath (Join-Path $CompilerDirectory "$mode/bin/$file") -Destination $runtime
            if($replayFiles.Count -gt 0){Need ((Hash (Join-Path $runtime $file)) -ceq $replayFiles[$mode+'/bin/'+$file]) 'Copied candidate replay runtime differs from validated identity'}
        }
        $script:exe=Join-Path $runtime 'NewModCompiler.exe'
        & "$PSScriptRoot/PrepareLegacyTexture.ps1" -TargetExe $exe *> "$modeOut/prepare-texture.log";Need ($LASTEXITCODE -eq 0) 'Build-cache Texture deployment failed'
        $script:modeState=@{configuration=$mode;passed=$false;exeSha256=(Hash $exe);cases=@()};$state.modes+=,$modeState
        $script:sdk=(New-Item -ItemType Directory -Path (Join-Path $modeWork 's')).FullName
        New-Item -ItemType Directory -Path "$sdk/tools","$sdk/Schemas"|Out-Null
        Copy-Item -LiteralPath "$root/SDK/Schemas/xsd" -Destination "$sdk/Schemas" -Recurse
        Copy-Item -LiteralPath "$root/SDK/tools/BinaryAssetBuilder.exe.config" -Destination "$sdk/tools"
        $probe=[IO.File]::ReadAllText("$root/SDK/Mods/mymod/Data/RecoveryArmorProbe.xml")
        $sdkTexture=Join-Path $sdk 'Art/cache.tga'
        $mod=Join-Path $modeWork 'm';MakeMod $mod $probe $sdkTexture
        $textureLength=(Get-Item -LiteralPath $sdkTexture).Length
        $largerLeaseInputs=@(Get-ChildItem -LiteralPath $sdk,$runtime -File -Recurse|Where-Object{$_.Length -ge $textureLength}).Count
        Need ($textureLength -eq 262162 -and $largerLeaseInputs -lt 512) 'Real SDK Texture must stay inside the bounded largest-input read leases'
        $modeState.sdkTextureInput=@{path=$sdkTexture;bytes=$textureLength;initialSha256=(Hash $sdkTexture);atLeastAsLargeLeaseInputs=$largerLeaseInputs;scope='Existing legal 256-square BGRA fixture consumed from SDK/Art while SDK input digest leases remain active; unchanged owner-only Texture backend and original byte oracle'}
        $cachedOut=Join-Path $modeWork 'cache';$plainOut=Join-Path $modeWork 'plain'
        $plain=RunCli 'disabled' $mod $plainOut $false;Cache $plain 'disabled'
        $cold=RunCli 'cold' $mod $cachedOut $true;Cache $cold 'cold';SameOutputs $plain $cold
        Need ((ReasonCount $cold.report.cache.actionMissReasons 'entry-not-found') -eq $cold.report.cache.misses) 'Cold cache misses must describe absent action entries'
        $hot=RunCli 'hot' $mod $cachedOut $true;Cache $hot 'hot';SameOutputs $plain $hot
        Need (@($hot.report.cache.actionMissReasons.PSObject.Properties).Count -eq 0) 'Hot reuse invented a lookup failure reason'
        $store=Join-Path $cachedOut '.cache/action-cache-v1'
        $entries=@(Get-ChildItem -LiteralPath $store -Directory|Where-Object{$_.Name -cmatch '^[0-9a-f]{64}$'}|Sort-Object Name)
        Need ($entries.Count -gt 1) 'Miss-reason repair fixture needs multiple independent persisted actions'
        $entry=$entries[0].FullName
        $members=@(Get-ChildItem -LiteralPath (Join-Path $entry 'files') -File|Sort-Object Name)
        Need ($members.Count -gt 0 -and (Test-Path -LiteralPath (Join-Path $entry 'index.bin'))) 'Miss-reason repair fixture lacks a complete persisted entry'
        $member=$members[0].FullName;$stamp=[IO.File]::GetLastWriteTimeUtc($member);$bytes=[IO.File]::ReadAllBytes($member)
        Need ($bytes.Length -gt 0) 'Miss-reason corruption fixture requires a nonempty real cache payload'
        $bytes[0]=$bytes[0] -bxor 0x7f;[IO.File]::WriteAllBytes($member,$bytes);[IO.File]::SetLastWriteTimeUtc($member,$stamp)
        Need ((Get-Item -LiteralPath $member).Length -eq $bytes.Length -and [IO.File]::GetLastWriteTimeUtc($member) -eq $stamp) 'Cache corruption must preserve exact payload size and mtime'
        $corrupt=RunCli 'corrupt-payload-reason-repaired' $mod $cachedOut $true;Cache $corrupt 'partial';SameOutputs $plain $corrupt
        Need ((ReasonCount $corrupt.report.cache.actionMissReasons 'invalid-payload') -eq 1) 'Corrupt bytes must produce exactly one real payload-validation miss'
        [IO.File]::WriteAllText((Join-Path $entry 'pending'),'abandoned-transaction')
        $pending=RunCli 'pending-entry-reason-repaired' $mod $cachedOut $true;Cache $pending 'partial';SameOutputs $plain $pending
        Need ((ReasonCount $pending.report.cache.actionMissReasons 'entry-pending') -eq 1) 'Abandoned pending entry must produce exactly one exclusion miss'
        $repairedHot=RunCli 'reason-repairs-hot' $mod $cachedOut $true;Cache $repairedHot 'hot';SameOutputs $plain $repairedHot
        OriginalOracle $hot 'original-initial'
        SameLengthMutation "$mod/Data/Armor.xml" '72.5%' '12.5%'
        $include=RunCli 'include-content-invalidated' $mod $cachedOut $true;Cache $include 'partial';DifferentOutputs $hot $include
        $includePlain=RunCli 'include-content-no-cache' $mod $plainOut $false;SameOutputs $includePlain $include
        $includeHot=RunCli 'include-content-hot' $mod $cachedOut $true;Cache $includeHot 'hot';SameOutputs $includePlain $includeHot
        SameLengthMutation "$mod/Data/XArmor.xml" '22.5%' '32.5%'
        $xinclude=RunCli 'xinclude-content-invalidated' $mod $cachedOut $true;Cache $xinclude 'partial';DifferentOutputs $includeHot $xinclude
        $includePlain=RunCli 'xinclude-content-no-cache' $mod $plainOut $false;SameOutputs $includePlain $xinclude
        $includeHot=RunCli 'xinclude-content-hot' $mod $cachedOut $true;Cache $includeHot 'hot';SameOutputs $includePlain $includeHot
        SameLengthMutation "$mod/Data/Mod.xml" 'AttackRange="101"' 'AttackRange="102"'
        $weapon=RunCli 'weapon-value-invalidated' $mod $cachedOut $true;Cache $weapon 'partial';DifferentOutputs $includeHot $weapon
        $weaponPlain=RunCli 'weapon-value-no-cache' $mod $plainOut $false;Cache $weaponPlain 'disabled';SameOutputs $weaponPlain $weapon
        SameLengthMutation "$mod/Data/Mod.xml" 'ThreatLevel="1.0"' 'ThreatLevel="2.0"'
        $gameObject=RunCli 'gameobject-value-invalidated' $mod $cachedOut $true;Cache $gameObject 'partial';DifferentOutputs $weapon $gameObject
        $includePlain=RunCli 'gameobject-value-no-cache' $mod $plainOut $false;SameOutputs $includePlain $gameObject
        $includeHot=RunCli 'gameobject-value-hot' $mod $cachedOut $true;Cache $includeHot 'hot';SameOutputs $includePlain $includeHot
        $payload="$mod/Data/AptUI/payload.bin";$stamp=[IO.File]::GetLastWriteTimeUtc($payload);$bytes=[IO.File]::ReadAllBytes($payload);$bytes[0]=9
        [IO.File]::WriteAllBytes($payload,$bytes);[IO.File]::SetLastWriteTimeUtc($payload,$stamp)
        Need ((Get-Item -LiteralPath $payload).Length -eq 7 -and [IO.File]::GetLastWriteTimeUtc($payload) -eq $stamp) 'Payload mutation failed to preserve length/mtime'
        $changed=RunCli 'payload-content-invalidated' $mod $cachedOut $true;Cache $changed 'partial';DifferentOutputs $includeHot $changed
        $changedPlain=RunCli 'payload-content-no-cache' $mod $plainOut $false;SameOutputs $changedPlain $changed
        $changedHot=RunCli 'payload-content-hot' $mod $cachedOut $true;Cache $changedHot 'hot';SameOutputs $changedPlain $changedHot
        $texture=$sdkTexture;$stamp=[IO.File]::GetLastWriteTimeUtc($texture);$bytes=[IO.File]::ReadAllBytes($texture);$originalLength=$bytes.Length;$bytes[18]=253
        [IO.File]::WriteAllBytes($texture,$bytes);[IO.File]::SetLastWriteTimeUtc($texture,$stamp)
        Need ((Get-Item -LiteralPath $texture).Length -eq $originalLength -and [IO.File]::GetLastWriteTimeUtc($texture) -eq $stamp) 'Texture mutation failed to preserve length/mtime'
        $textureChanged=RunCli 'texture-content-invalidated' $mod $cachedOut $true;Cache $textureChanged 'hot';DifferentOutputs $changedHot $textureChanged
        Need ($textureChanged.report.cache.backendCalls -eq 1) 'Texture edit recompiled unrelated certified assets or reused the uncertified Texture'
        $changedPlain=RunCli 'texture-content-no-cache' $mod $plainOut $false;SameOutputs $changedPlain $textureChanged
        $changedHot=RunCli 'texture-content-hot' $mod $cachedOut $true;Cache $changedHot 'hot';SameOutputs $changedPlain $changedHot
        SameLengthMutation "$mod/Data/events.h" '0x12345678' '0x23456789'
        $audioChanged=RunCli 'audio-header-content-invalidated' $mod $cachedOut $true;Cache $audioChanged 'partial';DifferentOutputs $changedHot $audioChanged
        $changedPlain=RunCli 'audio-header-content-no-cache' $mod $plainOut $false;SameOutputs $changedPlain $audioChanged
        $changedHot=RunCli 'audio-header-content-hot' $mod $cachedOut $true;Cache $changedHot 'hot';SameOutputs $changedPlain $changedHot
        New-Item -ItemType Directory -Force -Path "$mod/Additional"|Out-Null
        [IO.File]::WriteAllText("$mod/Additional/not-consumed.txt",'unread package member')
        $additional=RunCli 'unread-additional-hot' $mod $cachedOut $true;Cache $additional 'hot';SameOutputs $changedPlain $additional
        Need ($additional.report.cache.backendCalls -eq 1) 'Unread Additional member invalidated certified actions'
        # An unread SDK payload remains in all four whole-root inventories but
        # must not widen the compiler/tool identity or invalidate any action.
        $unused=Join-Path $sdk 'SageXml/Unconsumed.bin'
        New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($unused))|Out-Null
        $stream=[IO.File]::Create($unused)
        try{
            $block=[byte[]]::new(65536)
            for($i=0;$i -lt $block.Length;$i++){$block[$i]=[byte]($i%251)}
            foreach($unusedBlock in 1..16){$stream.Write($block,0,$block.Length)}
        }finally{$stream.Dispose()}
        $unusedLength=(Get-Item -LiteralPath $unused).Length
        Need ($unusedLength -eq 1024*1024) 'Unused SDK asset fixture has an unexpected byte count'
        $sdkAsset=RunCli 'unread-sdk-asset-hot' $mod $cachedOut $true;Cache $sdkAsset 'hot';SameOutputs $changedPlain $sdkAsset
        Need ($sdkAsset.report.cache.backendCalls -eq 1 -and $sdkAsset.report.cache.actionHits -eq $additional.report.cache.actionHits) 'Unread SDK payload widened tool identity or recompiled certified actions'
        $beforeLedger=[long]$additional.report.cache.hashReadBytes+[long]$additional.report.cache.inputHashReusedBytes
        $afterLedger=[long]$sdkAsset.report.cache.hashReadBytes+[long]$sdkAsset.report.cache.inputHashReusedBytes
        Need ($afterLedger-$beforeLedger -eq 4*$unusedLength) 'Unread SDK payload escaped one of four whole-root content inventories'
        Need ($sdkAsset.report.cache.inputHashReusedBytes-$additional.report.cache.inputHashReusedBytes -ge 2*$unusedLength) 'Unread SDK asset digest was not retained and reused across stream snapshots'
        $modeState.sdkAssetDigestReuse=@{path='SageXml/Unconsumed.bin';bytes=$unusedLength;sha256=(Hash $unused);inventoryCaptures=$sdkAsset.report.cache.inputInventoryCaptures;baselineHashReadBytes=$additional.report.cache.hashReadBytes;baselineReusedBytes=$additional.report.cache.inputHashReusedBytes;hashReadBytes=$sdkAsset.report.cache.hashReadBytes;reusedBytes=$sdkAsset.report.cache.inputHashReusedBytes;ledgerIncrease=$afterLedger-$beforeLedger;actionHits=$sdkAsset.report.cache.actionHits;backendCalls=$sdkAsset.report.cache.backendCalls;allRawOutputsEqual=$true}
        OriginalOracle $changedHot 'original-mutated'
        # Semantically neutral edits still require content invalidation. Keep
        # original BOM/length/mtime, and prove resulting bytes stay unchanged.
        SameLengthMutation "$sdk/Schemas/xsd/AssetTypeTexture.xsd" 'elementFormDefault="qualified"' "elementFormDefault='qualified'"
        $schema=RunCli 'schema-content-invalidated' $mod $cachedOut $true;Cache $schema 'cold';SameOutputs $changedPlain $schema
        SameLengthMutation "$sdk/tools/BinaryAssetBuilder.exe.config" 'monitorPaths="..\"' "monitorPaths='..\'"
        $config=RunCli 'configuration-content-invalidated' $mod $cachedOut $true;Cache $config 'cold';SameOutputs $changedPlain $config
        SameLengthMutation "$runtime/NewModCompiler.exe.config" 'version="v4.0"' "version='v4.0'"
        $runtimeChange=RunCli 'runtime-content-invalidated' $mod $cachedOut $true;Cache $runtimeChange 'cold';SameOutputs $changedPlain $runtimeChange
        $runtimeHot=RunCli 'runtime-content-hot' $mod $cachedOut $true;Cache $runtimeHot 'hot';SameOutputs $changedPlain $runtimeHot
        $other=Join-Path $modeWork 'b';MakeMod $other ($probe.Replace('72.5%','32.5%')) $sdkTexture
        $otherCold=RunCli 'second-mod' $other $cachedOut $true;Need ($otherCold.report.cache.misses -gt 0) 'Different MOD inputs silently reused every action'
        $otherPlain=RunCli 'second-mod-no-cache' $other $plainOut $false;SameOutputs $otherPlain $otherCold
        $returnA=RunCli 'first-mod-return' $mod $cachedOut $true;Cache $returnA 'hot';SameOutputs $changedPlain $returnA
        $armorPath="$mod/Data/Armor.xml";$valid=[IO.File]::ReadAllBytes($armorPath)
        $confirmed=CacheFiles $cachedOut
        [IO.File]::WriteAllText($armorPath,'<AssetDeclaration>broken')
        $failed=RunCli 'invalid-include' $mod $cachedOut $true 4
        CacheUnchanged $cachedOut $confirmed
        [IO.File]::WriteAllBytes($armorPath,$valid)
        $retry=RunCli 'retry-after-invalid-include' $mod $cachedOut $true;SameOutputs $changedPlain $retry
        $confirmed=CacheFiles $cachedOut
        $cancelled=RunCli 'actual-cancel' $mod $cachedOut $true 6 $true
        CacheUnchanged $cachedOut $confirmed
        $afterCancel=RunCli 'retry-after-actual-cancel' $mod $cachedOut $true;SameOutputs $changedPlain $afterCancel
        $held=@($afterCancel.report.artifacts)[0].Path
        $confirmed=CacheFiles $cachedOut
        # A genuine new action must be staged before publication fails. A hot
        # no-write build alone cannot prove candidate installation rollback.
        SameLengthMutation "$mod/Data/Mod.xml" 'AttackRange="102"' 'AttackRange="103"'
        $occupied=RunCli 'publication-fault' $mod $cachedOut $true 7 $false $held
        Cache $occupied 'partial'
        CacheUnchanged $cachedOut $confirmed
        $afterFault=RunCli 'retry-after-publication-fault' $mod $cachedOut $true;Cache $afterFault 'partial'
        $afterFaultPlain=RunCli 'publication-fault-retry-no-cache' $mod $plainOut $false;Cache $afterFaultPlain 'disabled';SameOutputs $afterFaultPlain $afterFault
        $afterFaultHot=RunCli 'publication-fault-retry-hot' $mod $cachedOut $true;Cache $afterFaultHot 'hot';SameOutputs $afterFaultPlain $afterFaultHot
        $modeState.passed=$true
    }
    $state.succeeded=$true;$global:LASTEXITCODE=0
}catch{$state.error=$_.Exception.ToString();throw}
finally{
    $state|ConvertTo-Json -Depth 18|Set-Content "$out/validation.json"
    try{Remove-Item -LiteralPath $short -Recurse -Force}catch{Write-Warning "Build-cache work cleanup failed: $_"}
}
