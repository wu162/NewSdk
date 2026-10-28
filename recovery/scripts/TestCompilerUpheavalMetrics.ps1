#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$CompilerDirectory,
    [Parameter(Mandatory=$true)][string]$ReplayReceipt,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(60,2400)][int]$TimeoutSeconds=1200,
    [switch]$Preflight
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(!$env:GITHUB_RUN_ID -or !$env:RUNNER_TEMP){throw 'Upheaval measurement runs only in an identified GitHub Actions environment'}
if(Test-Path -LiteralPath $OutputDirectory){throw 'Refusing to overwrite Upheaval evidence'}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
$work=(New-Item -ItemType Directory -Path (Join-Path $env:RUNNER_TEMP ('u'+[Guid]::NewGuid().ToString('N').Substring(0,8)))).FullName
$state=@{schemaVersion=1;succeeded=$false;preflightOnly=[bool]$Preflight;preflightPassed=$false;runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;cases=@();scope='Complete original Upheaval Release-only controlled no-cache serial/parallel, empty action cache and three hot builds. OS file cache uncontrolled; sampled private memory is not continuous peak. No single-edit or packaging measurement. No speed threshold or general speedup claim.'}
$state.setupPhaseTimings=@();$state.verificationPhaseTimings=@()
$state.setupTimingScope='Controller preparation wall-clock only; excluded from every product report/controller CLI elapsed value. Inventory optimization has no measured speedup claim.'
$setupClock=[Diagnostics.Stopwatch]::StartNew();$phaseWatch=$null;$phaseName=$null
function EndSetupPhase([bool]$Succeeded=$true){
    if($script:phaseWatch){$script:phaseWatch.Stop();$state.setupPhaseTimings+=,@{name=$script:phaseName;elapsedMs=$script:phaseWatch.ElapsedMilliseconds;succeeded=$Succeeded};Write-Host ("END Upheaval setup {0}: elapsedMs={1} succeeded={2}" -f $script:phaseName,$script:phaseWatch.ElapsedMilliseconds,$Succeeded);$script:phaseWatch=$null}
}
function BeginSetupPhase([string]$Name){EndSetupPhase;$script:phaseName=$Name;$script:phaseWatch=[Diagnostics.Stopwatch]::StartNew();Write-Host ("START Upheaval setup {0}" -f $Name)}
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Within([string]$Directory,[string]$Path){
    $base=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$full=[IO.Path]::GetFullPath($Path)
    $full.StartsWith($base+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function Inventory([string]$Directory){
    # One managed traversal/hash loop avoids a PowerShell pipeline/Get-FileHash
    # invocation per SDK file. Before/after use the same explicit Ordinal order.
    @([UpheavalInputInventory]::Capture($Directory))
}
function Unchanged([string]$Directory,$Before){
    $actual=@(Inventory $Directory);Need ($actual.Count -eq $Before.Count) "Input inventory count changed: $Directory"
    for($i=0;$i -lt $actual.Count;$i++){
        Need ([string]::Equals($actual[$i].path,$Before[$i].path,[StringComparison]::Ordinal) -and $actual[$i].bytes -eq $Before[$i].bytes -and [string]::Equals($actual[$i].sha256,$Before[$i].sha256,[StringComparison]::Ordinal)) "Input bytes changed: $Directory"
    }
}
function InvokeProduct([string]$Name,[string]$Command,[string]$Destination,[bool]$Enabled,[int]$Jobs){
    $caseOut=(New-Item -ItemType Directory -Path (Join-Path $out $Name)).FullName
    New-Item -ItemType Directory -Force -Path $Destination|Out-Null
    $arguments=@($Command,'--mod',$script:mod,'--sdk-root',$script:sdk,'--out',$Destination,'--profile','development','--package-format','loose','--package-content','base,apt,maps','--include-additional=false','/sc:false',('--jobs='+$Jobs),('--bc='+$Enabled.ToString().ToLowerInvariant()),('/data:'+$script:data),('/art:'+$script:art),('/audio:'+$script:audio))
    $report=Join-Path $Destination 'build-report.json';$events=Join-Path $Destination 'build.events.jsonl'
    $request=@{exe=$script:exe;cwd=$script:runtime;arguments=$arguments;stdout=(Join-Path $caseOut 'stdout.log');stderr=(Join-Path $caseOut 'stderr.log');events=$events;cancel=$false;timeoutSeconds=$TimeoutSeconds}
    $request|ConvertTo-Json -Depth 8|Set-Content "$caseOut/request.json"
    $entry=@{name=$Name;command=$Command;arguments=$arguments;passed=$false};$state.cases+=,$entry
    try{
        & $script:control "$caseOut/request.json" "$caseOut/control.json" *> "$caseOut/controller.log"
        $controlExit=$LASTEXITCODE
        Need (Test-Path -LiteralPath "$caseOut/control.json") "No controlled process result: $Name"
        $audit=Get-Content "$caseOut/control.json" -Raw|ConvertFrom-Json;$entry.controller=$audit
        Need ($controlExit -eq 0 -and $audit.succeeded -and $audit.processExitCode -eq 0) "Upheaval process failed: $Name"
        Need ($audit.singleProcessJobEnforced -and $audit.singleProcessJobLimit -eq 1 -and @($audit.descendants).Count -eq 0) 'Upheaval process spawned a product child'
        $body=Get-Content -LiteralPath $report -Raw|ConvertFrom-Json
        Need ($body.exitCode -eq 0 -and $body.status -ceq 'succeeded' -and $body.executionMode -ceq $(if($Command -ceq 'plan'){'PlanOnly'}else{'InProcess'}) -and @($body.externalTools).Count -eq 0) 'Upheaval report is not a successful in-process invocation'
        $rows=@(Get-Content -LiteralPath $events|Where-Object{$_}|ForEach-Object{$_|ConvertFrom-Json})
        Need ($rows.Count -ge 2 -and $rows[0].eventType -ceq 'BuildStarted' -and $rows[-1].eventType -ceq 'BuildFinished') 'Upheaval protocol lacks terminal events'
        Need (@($rows|Where-Object{$_.eventType -ceq 'BuildStarted'}).Count -eq 1 -and @($rows|Where-Object{$_.eventType -ceq 'BuildFinished'}).Count -eq 1) 'Upheaval protocol terminal events are duplicated'
        $sequence=0;foreach($row in $rows){$sequence++;Need ($row.sequence -eq $sequence -and $row.buildId -ceq $body.buildId) 'Upheaval protocol identity/sequence differs'}
        Need ($rows[0].payload.processId -eq $audit.processId) 'Upheaval report is from another process'
        $tasks=@($body.plan.Tasks);Need ($tasks.Count -eq 5) 'Upheaval actual plan must have exactly five tasks'
        $actualInputs=@($tasks|ForEach-Object{[IO.Path]::GetRelativePath($script:mod,$_.InputPath).Replace('\','/')}|Sort-Object)
        Need (($actualInputs -join "`n") -ceq (($script:inputContract.entries|Sort-Object) -join "`n")) 'Actual CLI plan input set differs from pinned five entries'
        Need (@($tasks|Where-Object{$_.Content -ceq 'base'}).Count -eq 1 -and @($tasks|Where-Object{$_.Content -ceq 'apt'}).Count -eq 3 -and @($tasks|Where-Object{$_.Content -ceq 'maps'}).Count -eq 1) 'Actual CLI plan groups differ'
        $files=@{}
        if($Command -ceq 'build'){
            Need ($body.diagnosticCounts.error -eq 0) 'Upheaval reported Core errors despite a successful exit; stop before further large builds'
            Need ($body.publicationStatus -ceq 'PublishedDirectory' -and $body.cache.executionEngine -ceq 'AssetTasks.v1') 'Upheaval build did not publish through the shared asset executor'
            Need ($body.cache.requestedJobs -eq $Jobs) 'Reported user concurrency upper bound differs'
            if(!$Enabled){Need ($body.cache.cacheMode -ceq 'Disabled' -and $body.cache.hits -eq 0 -and $body.cache.misses -eq 0) 'No-cache build recorded action cache use'}
            else{Need ($body.cache.cacheMode -ceq 'ActionCache') 'Cache build did not use the actual action cache'}
            $published=Join-Path $Destination 'published';$artifactRoot=(New-Item -ItemType Directory -Path "$caseOut/artifacts").FullName
            foreach($artifact in $body.artifacts){
                Need ((Within $published $artifact.Path) -and (Test-Path -LiteralPath $artifact.Path -PathType Leaf)) 'Output artifact escaped its publication directory'
                Need ((Hash $artifact.Path) -ceq $artifact.Sha256 -and (Get-Item -LiteralPath $artifact.Path).Length -eq $artifact.Size) 'Output artifact bytes differ from durable report'
                $relative=[IO.Path]::GetRelativePath($published,$artifact.Path).Replace('\','/')
                Need (!$files.ContainsKey($relative)) 'Duplicate publication artifact'
                $copy=Join-Path $artifactRoot $relative;New-Item -ItemType Directory -Force -Path ([IO.Path]::GetDirectoryName($copy))|Out-Null
                Copy-Item -LiteralPath $artifact.Path -Destination $copy;$files[$relative]=$copy
            }
            foreach($extension in @('.manifest','.bin','.imp','.relo')){Need (@($files.Keys|Where-Object{[IO.Path]::GetExtension($_) -ceq $extension}).Count -eq 5) "Missing one of the five complete stream outputs: $extension"}
            $custom=@($files.Keys|Where-Object{[IO.Path]::GetExtension($_) -ceq '.cdata'})
            Need ($files.Count -eq 20+$custom.Count -and $files.Count -eq @(Get-ChildItem -LiteralPath $published -Recurse -File).Count) 'Publication includes missing or unlisted dynamic CDATA/output files'
            Need (@($body.artifacts|Where-Object{$_.Kind -ceq 'custom-data'}).Count -eq $custom.Count) 'CDATA kind/extension identities differ'
            Need (!(Test-Path -LiteralPath (Join-Path $Destination ('.build/'+$body.buildId)))) 'Successful Upheaval build retained owned staging'
            $entry.cache=$body.cache;$entry.elapsedMs=$body.elapsedMs;$entry.artifactCount=$files.Count;$entry.customDataCount=$custom.Count
            $entry.artifacts=@($files.Keys|Sort-Object|ForEach-Object{@{path=$_;bytes=(Get-Item -LiteralPath $files[$_]).Length;sha256=(Hash $files[$_])}})
        }else{Need (@($body.artifacts).Count -eq 0) 'Planning must not publish compilation artifacts'}
        $entry.passed=$true
        return @{report=$body;files=$files;entry=$entry}
    }catch{
        foreach($log in @('stdout.log','stderr.log','controller.log')){Get-Content -LiteralPath "$caseOut/$log" -Tail 30 -ErrorAction SilentlyContinue|Out-Host}
        throw
    }finally{
        foreach($pair in @(@($report,'build-report.json'),@($events,'build.events.jsonl'))){if(Test-Path -LiteralPath $pair[0]){Copy-Item -LiteralPath $pair[0] -Destination (Join-Path $caseOut $pair[1])}}
    }
}
function CompareOutputs($Baseline,$Current){
    Need ($Baseline.files.Count -eq $Current.files.Count) 'Upheaval output membership count changed'
    foreach($name in $Baseline.files.Keys){
        Need ($Current.files.ContainsKey($name)) "Upheaval output omitted: $name"
        Need ([UpheavalRawBytes]::Equal($Baseline.files[$name],$Current.files[$name])) "Upheaval raw output differs: $name"
    }
    $Current.entry.allRawBytesEqualToSerialNoCache=$true
}
try{
    BeginSetupPhase 'inventory-helper-compile'
    Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
public static class UpheavalInputInventory {
 public sealed class Entry {
  public string path { get; set; }
  public long bytes { get; set; }
  public string sha256 { get; set; }
 }
 static FileAttributes SafeAttributes(string path) {
  FileAttributes value=File.GetAttributes(path);
  if((value&FileAttributes.ReparsePoint)!=0) throw new IOException("Reparse input rejected: "+path);
  return value;
 }
 public static Entry[] Capture(string directory) {
  string root=Path.GetFullPath(directory);
  if((SafeAttributes(root)&FileAttributes.Directory)==0) throw new IOException("Inventory root is not a directory");
  var pending=new Stack<string>();pending.Push(root);
  var paths=new SortedDictionary<string,string>(StringComparer.Ordinal);
  while(pending.Count!=0) {
   string current=pending.Pop();SafeAttributes(current);
   foreach(string path in Directory.EnumerateFileSystemEntries(current)) {
    FileAttributes attributes=SafeAttributes(path);
    if((attributes&FileAttributes.Directory)!=0) pending.Push(path);
    else paths.Add(Path.GetRelativePath(root,path).Replace('\\','/'),path);
   }
  }
  var rows=new List<Entry>(paths.Count);
  using(SHA256 hash=SHA256.Create()) foreach(var pair in paths) {
   SafeAttributes(pair.Value);
   using(var input=new FileStream(pair.Value,FileMode.Open,FileAccess.Read,FileShare.Read,65536,FileOptions.SequentialScan)) {
    long size=input.Length;byte[] digest=hash.ComputeHash(input);
    if(input.Position!=size || input.Length!=size) throw new IOException("Inventory file length changed: "+pair.Value);
    rows.Add(new Entry{path=pair.Key,bytes=size,sha256=BitConverter.ToString(digest).Replace("-","").ToLowerInvariant()});
   }
  }
  return rows.ToArray();
 }
}
'@
    BeginSetupPhase 'strict-replay-verification'
    Push-Location $root
    try{$state.testSourceCommit=(& git rev-parse HEAD).Trim();Need ($LASTEXITCODE -eq 0) 'Cannot identify test source'}finally{Pop-Location}
    $CompilerDirectory=(Resolve-Path -LiteralPath $CompilerDirectory).Path;$ReplayReceipt=(Resolve-Path -LiteralPath $ReplayReceipt).Path
    Need ($ReplayReceipt -ceq (Join-Path $CompilerDirectory 'cache-payload-replay.json')) 'Replay receipt must belong to prepared compiler directory'
    & python "$PSScriptRoot/PrepareCompilerCacheReplay.py" --repo $root --candidate-commit $state.testSourceCommit --verify-prepared $ReplayReceipt --purpose upheaval-metrics *> "$out/replay-verification.log"
    Need ($LASTEXITCODE -eq 0) 'Strict successful archive/production closure/API/runtime replay verification failed'
    $replay=Get-Content -LiteralPath $ReplayReceipt -Raw|ConvertFrom-Json
    Need ($replay.purpose -ceq 'upheaval-metrics' -and $replay.prepared -eq $true -and $replay.candidateValidated -eq $false -and $replay.productionSourcesUnchanged -eq $true -and $replay.testSourceCommit -ceq $state.testSourceCommit) 'Reuse purpose/source contract differs'
    $measurementNames=@('recovery/scripts/TestCompilerUpheavalMetrics.ps1','recovery/scripts/PrepareUpheavalInputs.py')
    Need (@($replay.measurementScripts).Count -eq 2) 'Reuse receipt omitted measurement script identities'
    foreach($name in $measurementNames){$identity=@($replay.measurementScripts|Where-Object{$_.path -ceq $name});Need ($identity.Count -eq 1 -and $identity[0].sha256 -ceq (Hash (Join-Path $root $name))) 'Measurement script differs from strict replay receipt'}
    $state.binarySourceCommit=$replay.binarySourceCommit;$state.successfulArchiveReplay=$replay
    $sdk=(Resolve-Path -LiteralPath (Join-Path $root 'SDK')).Path;$mod=Join-Path $work 'Upheaval';$runtime=(New-Item -ItemType Directory -Path (Join-Path $work 'runtime')).FullName
    Need (!(Within $sdk $mod) -and !(Within $sdk $out) -and !(Within $mod $out)) 'Measurement must not add MOD or outputs to original SDK inputs'
    BeginSetupPhase 'input-extraction-schema-routes'
    & python "$PSScriptRoot/PrepareUpheavalInputs.py" --repo $root --sdk $sdk --mod $mod --output "$out/mod-inputs.json" *> "$out/input-preflight.log"
    Need ($LASTEXITCODE -eq 0) 'Complete pinned MOD blob/schema/route/input preflight failed'
    $inputContract=Get-Content -LiteralPath "$out/mod-inputs.json" -Raw|ConvertFrom-Json
    Need ($inputContract.sourceCommit -ceq 'bacfc69fc3e71c8116d1f22b933b8b6173170ecf' -and $inputContract.modTree -ceq '5551fc26a6f96b62613443e0d0fa2958c14cbce0' -and $inputContract.fileCount -eq 2723 -and $inputContract.bytes -eq 300900885) 'Complete original MOD contract differs'
    $state.modInputs=$inputContract
    BeginSetupPhase 'sdk-input-inventory'
    $state.sdkInputs=@(Inventory $sdk);Need ($state.sdkInputs.Count -gt 0) 'Real SDK input inventory is empty'
    $state.sdkInputBytes=[long](($state.sdkInputs|Measure-Object -Property bytes -Sum).Sum)
    BeginSetupPhase 'resource-root-reference-preflight'
    $data=$sdk+';'+(Join-Path $mod 'Data')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'SageXml')
    $art=$sdk+';'+(Join-Path $mod 'Art')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'Art')
    $audio=$sdk+';'+(Join-Path $mod 'Audio')+';'+(Join-Path $sdk 'Mods')+';'+(Join-Path $sdk 'Audio')
    foreach($directory in @(($data+';'+$art+';'+$audio).Split(';'))){Need (Test-Path -LiteralPath $directory -PathType Container) "Actual resolver root missing: $directory"}
    # static/audio references intentionally use the SDK's precompiled streams;
    # no matching source XML leaf is required by the original resolver contract.
    foreach($name in @('static','global','audio')){
        $source=Join-Path $sdk ('SageXml/'+$name+'.xml')
        $manifest=Join-Path $sdk ('builtmods/sagexml/'+$name+'.manifest')
        Need ((Test-Path -LiteralPath $source -PathType Leaf) -or (Test-Path -LiteralPath $manifest -PathType Leaf)) "Original MOD reference has neither source nor precompiled manifest: $name"
    }
    $state.referenceScope='Static source-or-manifest presence only; exact original resolver/reference compatibility is gated by the first no-cache build.'
    $state.resourceRoots=@{data=$data;art=$art;audio=$audio}
    BeginSetupPhase 'runtime-deployment-inventory'
    foreach($name in @('NewModCompiler.exe','NewModCompiler.exe.config','antlr.runtime.dll','BinaryAssetBuilder.TextureCompiler.dll','BinaryAssetBuilder.TextureCompiler.dll.patch.json')){
        $path='Release/bin/'+$name;$identity=@($replay.files|Where-Object{$_.path -ceq $path})
        Need ($identity.Count -eq 1 -and (Hash (Join-Path $CompilerDirectory $path)) -ceq $identity[0].sha256) 'Release runtime differs from verified archive'
        Copy-Item -LiteralPath (Join-Path $CompilerDirectory $path) -Destination $runtime
    }
    $exe=Join-Path $runtime 'NewModCompiler.exe'
    $patch=Get-Content -LiteralPath (Join-Path $runtime 'BinaryAssetBuilder.TextureCompiler.dll.patch.json') -Raw|ConvertFrom-Json
    Need ($patch.targetExeSha256 -ceq (Hash $exe) -and $patch.patchedSha256 -ceq (Hash (Join-Path $runtime 'BinaryAssetBuilder.TextureCompiler.dll')) -and $patch.originalSha256 -ceq '4f559428e11a5cf8b914792e0eccf3b94397b8851233ec658d4c02c4633e9d9e' -and $patch.protectedPeAudit.status -ceq 'passed' -and $patch.protectedPeAudit.mode -ceq 'redirect') 'Texture patch does not bind the verified EXE'
    $state.runtimeInputs=@(Inventory $runtime);Need ($state.runtimeInputs.Count -eq 5) 'Measurement requires exact five-file runtime'
    $state.textureDeployment=$patch
    BeginSetupPhase 'cli-controller-build'
    $control=& "$PSScriptRoot/BuildCompilerCliControl.ps1" -OutputDirectory (Join-Path $out 'controller')
    Need ($LASTEXITCODE -eq 0 -and (Test-Path -LiteralPath $control -PathType Leaf)) 'Actual CLI controller build failed'
    BeginSetupPhase 'five-entry-plan'
    $plan=InvokeProduct 'preflight-plan' 'plan' (Join-Path $work 'plan') $false 1
    $state.plan=$plan.report.plan;$state.preflightPassed=$true
    EndSetupPhase;$state.setupElapsedMs=$setupClock.ElapsedMilliseconds
    if(!$Preflight){
        BeginSetupPhase 'raw-output-comparison-helper-compile'
        Add-Type -TypeDefinition @'
using System;
using System.IO;
public static class UpheavalRawBytes {
 public static bool Equal(string a,string b) {
  using(var l=File.OpenRead(a)) using(var r=File.OpenRead(b)) {
   if(l.Length!=r.Length)return false;
   byte[] x=new byte[65536],y=new byte[65536];int n;
   while((n=l.Read(x,0,x.Length))!=0) {
    int done=0;while(done<n){int m=r.Read(y,done,n-done);if(m==0)return false;done+=m;}
    for(int i=0;i<n;i++)if(x[i]!=y[i])return false;
   }
   return r.ReadByte()==-1;
  }
 }
}
'@
        EndSetupPhase;$state.setupElapsedMs=$setupClock.ElapsedMilliseconds
        $baseline=InvokeProduct 'no-cache-jobs1' 'build' (Join-Path $work 'plain1') $false 1
        $state.baselineCompilationValidated=$true
        $parallel=InvokeProduct 'no-cache-jobs4' 'build' (Join-Path $work 'plain4') $false 4;CompareOutputs $baseline $parallel
        $cached=Join-Path $work 'cached';Need (!(Test-Path -LiteralPath $cached)) 'Empty action-cache destination already exists'
        $cold=InvokeProduct 'empty-action-cache-jobs4' 'build' $cached $true 4;CompareOutputs $baseline $cold
        Need ($cold.report.cache.misses -gt 0) 'Empty action cache did not record real misses'
        $cold.entry.cacheScope='Destination was absent before invocation; any hits are same-build reuse across its five streams.'
        foreach($repeat in 1..3){
            $hot=InvokeProduct ('hot-action-cache-'+$repeat) 'build' $cached $true 4;CompareOutputs $baseline $hot
            Need ($hot.report.cache.hits -gt 0) 'Unchanged hot build did not reuse any stored certified actions'
            $hot.entry.cacheScope='Actual hit/miss counts are preserved; the bounded 4096-entry/4GiB store may evict actions in a large MOD. No all-hit assumption.'
            # Uncertified owner backends, including Texture, remain real calls.
            # Neither a requested jobs value nor a hit count proves overlap.
        }
    }
    $state.succeeded=$true
}catch{
    EndSetupPhase $false
    $state.failure=$_.Exception.ToString();$state.succeeded=$false
    throw
}finally{
    Write-Host 'START Upheaval final input verification'
    $verificationClock=[Diagnostics.Stopwatch]::StartNew()
    $inputChecks=@{}
    if($sdk -and $state.sdkInputs){$watch=[Diagnostics.Stopwatch]::StartNew();try{Unchanged $sdk $state.sdkInputs;$inputChecks.sdk=$true}catch{$inputChecks.sdk=$false;$state.succeeded=$false;$state.inputVerificationFailure=$_.Exception.ToString()}finally{$state.verificationPhaseTimings+=,@{name='sdk-input-inventory';elapsedMs=$watch.ElapsedMilliseconds;succeeded=$inputChecks.sdk}}}
    if($runtime -and $state.runtimeInputs){$watch=[Diagnostics.Stopwatch]::StartNew();try{Unchanged $runtime $state.runtimeInputs;$inputChecks.runtime=$true}catch{$inputChecks.runtime=$false;$state.succeeded=$false;$state.inputVerificationFailure=$_.Exception.ToString()}finally{$state.verificationPhaseTimings+=,@{name='runtime-input-inventory';elapsedMs=$watch.ElapsedMilliseconds;succeeded=$inputChecks.runtime}}}
    if(Test-Path -LiteralPath "$out/mod-inputs.json"){
        $watch=[Diagnostics.Stopwatch]::StartNew()
        & python "$PSScriptRoot/PrepareUpheavalInputs.py" --mod $mod --verify "$out/mod-inputs.json" --output "$out/mod-after.json" *> "$out/input-after.log"
        $inputChecks.mod=($LASTEXITCODE -eq 0);if(!$inputChecks.mod){$state.succeeded=$false;$state.inputVerificationFailure='Complete MOD input changed'}
        $state.verificationPhaseTimings+=,@{name='complete-mod-input-inventory';elapsedMs=$watch.ElapsedMilliseconds;succeeded=$inputChecks.mod}
    }
    $state.finalVerificationElapsedMs=$verificationClock.ElapsedMilliseconds
    Write-Host ("END Upheaval final input verification: elapsedMs={0}" -f $state.finalVerificationElapsedMs)
    if(!$state.ContainsKey('setupElapsedMs')){$state.setupElapsedMs=[long](($state.setupPhaseTimings|Measure-Object -Property elapsedMs -Sum).Sum)}
    $state.inputChecks=$inputChecks
    $state|ConvertTo-Json -Depth 100|Set-Content "$out/validation.json"
    if(!$state.succeeded -and !$state.failure){throw 'Final input identity verification failed'}
}
