#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$Compiler,
    [Parameter(Mandatory=$true)][string]$OutputDirectory,
    [ValidateRange(30,600)][int]$TimeoutSeconds=240
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$Compiler=(Resolve-Path -LiteralPath $Compiler).Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to replace evidence: $OutputDirectory"}
$evidence=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
# Short paths preserve the existing Win32/MAX_PATH contract, particularly the
# SDK cache prefix and legacy asset hash filenames.
$work=Join-Path ([IO.Path]::GetPathRoot($env:RUNNER_TEMP)) ('scc-'+$env:GITHUB_RUN_ID+'-'+$env:GITHUB_RUN_ATTEMPT)
if(Test-Path -LiteralPath $work){throw "Refusing to replace work: $work"}
$null=New-Item -ItemType Directory -Path $work
$sdk=Join-Path $work 's';$cacheBase=Join-Path $sdk 'SuperCompilerCache'
$state=@{schemaVersion=1;sourceCommit=(git -C $root rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;status='started';samples=@();inputs=@();scope='One Release executable; complete current mymod inside SDK/Mods and complete pinned historical Upheaval outside SDK; base content only, loose cold/hot with changed output and BIG from warm cache. No Debug build, original SDK byte differential, APT/FLA/maps or full cache flag matrix.'}
$watchers=@()
function Need([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Within([string]$Directory,[string]$Path){
    $base=[IO.Path]::GetFullPath($Directory).TrimEnd([char[]]'\/');$full=[IO.Path]::GetFullPath($Path)
    $full.Equals($base,[StringComparison]::OrdinalIgnoreCase) -or $full.StartsWith($base+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)
}
function Snapshot([string]$Directory){
    $result=@{};Get-ChildItem -LiteralPath $Directory -Recurse -Force|ForEach-Object{
        $key=[IO.Path]::GetRelativePath($Directory,$_.FullName).Replace('\','/')
        $result[$key]=if($_.PSIsContainer){'directory'}else{[string]$_.Length+':'+$_.LastWriteTimeUtc.Ticks+':'+(Hash $_.FullName)}
    };$result
}
function Unchanged([string]$Directory,$Before){
    $after=Snapshot $Directory;Need ($after.Count -eq $Before.Count) "Source entry count changed: $Directory"
    foreach($key in $Before.Keys){Need ($after.ContainsKey($key) -and $after[$key] -ceq $Before[$key]) "Source changed: $Directory/$key"}
}
function InvokeBuild([string]$Label,[string]$Command,[string]$Mod,[string]$Format,[bool]$Version=$true){
    $out=Join-Path $work ('out/'+$Label);$case=Join-Path $evidence $Label;$null=New-Item -ItemType Directory -Path $case
    $arguments=@($Command,'--sdk-root',$sdk,'--mod',$Mod,'--out',$out,'--package-content','base','--package-format',$Format,'--jobs','4','--performance','summary')
    if($Format -eq 'big' -and $Version){$arguments+=@('--mod-version','1.0')}
    $row=@{label=$Label;command=$arguments;status='started'};$state.samples+=,$row
    $start=[Diagnostics.ProcessStartInfo]::new();$start.FileName=$Compiler;$start.WorkingDirectory=Split-Path $Compiler;$start.UseShellExecute=$false;$start.CreateNoWindow=$true;$start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true
    foreach($argument in $arguments){$start.ArgumentList.Add($argument)}
    $process=[Diagnostics.Process]::new();$process.StartInfo=$start
    try {
        Need ($process.Start()) "Cannot launch $Label"
        $stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
        if(!$process.WaitForExit($TimeoutSeconds*1000)){$process.Kill($true);throw "$Label timed out"}
        $text=$stdout.GetAwaiter().GetResult();$errors=$stderr.GetAwaiter().GetResult()
        [IO.File]::WriteAllText((Join-Path $case 'stdout.log'),$text);[IO.File]::WriteAllText((Join-Path $case 'stderr.log'),$errors)
        $row.exitCode=$process.ExitCode
        if(!$Version){Need ($process.ExitCode -eq 2 -and $errors.Contains('CLI_MOD_VERSION')) 'BIG still requires an explicit CLI MOD version';$row.status='passed';return $null}
        if($process.ExitCode -ne 0){Write-Host $text;Write-Host $errors;throw "$Label failed with exit $($process.ExitCode)"}
        $reportPath=Join-Path $out 'build-report.json';Copy-Item -LiteralPath $reportPath -Destination $case
        Copy-Item -LiteralPath (Join-Path $out 'build.events.jsonl') -Destination $case
        $report=Get-Content -LiteralPath $reportPath -Raw|ConvertFrom-Json
        Need ($report.status -ceq 'succeeded' -and $report.diagnosticCounts.error -eq 0) "$Label reported failure/errors"
        foreach($entry in @(Get-ChildItem -LiteralPath $out -Force)){Need ($entry.Name -in @('published','build.log','build.events.jsonl','build-report.json')) "$Label left non-delivery files in output: $($entry.Name)"}
        $row.cache=$report.cache;$row.artifactCount=@($report.artifacts).Count;$row.status='passed'
        return @{report=$report;output=$out}
    } finally {$process.Dispose();$state|ConvertTo-Json -Depth 16|Set-Content (Join-Path $evidence 'validation.json')}
}

# Event callbacks run in CLR handlers, so even transient create/delete writes
# are recorded while PowerShell waits for the child. Buffer errors reject the
# audit instead of allowing a source snapshot to hide transient writes.
Add-Type -TypeDefinition @'
using System;
using System.Collections.Concurrent;
using System.IO;
public sealed class SuperCompilerCacheWriteAudit : IDisposable {
    readonly FileSystemWatcher watcher;
    public readonly ConcurrentQueue<string> Paths = new ConcurrentQueue<string>();
    public readonly ConcurrentQueue<string> Errors = new ConcurrentQueue<string>();
    public SuperCompilerCacheWriteAudit(string root) {
        watcher = new FileSystemWatcher(root) { IncludeSubdirectories = true, InternalBufferSize = 65536,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes | NotifyFilters.Security };
        watcher.Created += Changed; watcher.Changed += Changed; watcher.Deleted += Changed;
        watcher.Renamed += (s,e) => { Paths.Enqueue(e.OldFullPath); Paths.Enqueue(e.FullPath); };
        watcher.Error += (s,e) => Errors.Enqueue(e.GetException().ToString());
        watcher.EnableRaisingEvents = true;
    }
    void Changed(object sender, FileSystemEventArgs args) { Paths.Enqueue(args.FullPath); }
    public void Dispose() { watcher.EnableRaisingEvents = false; watcher.Dispose(); }
}
'@
try {
    $null=New-Item -ItemType Directory -Path $sdk
    Get-ChildItem (Join-Path $root 'SDK') -Force|Where-Object Name -notin @('Mods','builtmods','SuperCompilerCache')|Copy-Item -Destination $sdk -Recurse
    $modInside=Join-Path $sdk 'Mods/mymod';$null=New-Item -ItemType Directory -Path (Split-Path $modInside)
    Copy-Item (Join-Path $root 'SDK/Mods/mymod') -Destination $modInside -Recurse
    $historical='bacfc69fc3e71c8116d1f22b933b8b6173170ecf';$tree='5551fc26a6f96b62613443e0d0fa2958c14cbce0'
    Need ((git -C $root rev-parse ($historical+':SDK/Mods/Upheaval')).Trim() -ceq $tree) 'Pinned historical Upheaval tree differs'
    $archive=Join-Path $work 'upheaval.tar'
    & git -C $root archive --output=$archive $historical 'SDK/Mods/Upheaval';Need ($LASTEXITCODE -eq 0) 'Cannot archive pinned Upheaval'
    $external=Join-Path $work 'external';$null=New-Item -ItemType Directory -Path $external
    & tar -xf $archive -C $external;Need ($LASTEXITCODE -eq 0) 'Cannot extract pinned Upheaval';Remove-Item $archive
    $modOutside=Join-Path $external 'SDK/Mods/Upheaval'
    $mods=@(@{name='mymod';path=$modInside;source='current repository';snapshot=(Snapshot $modInside)},@{name='upheaval';path=$modOutside;source=$historical;sourceTree=$tree;snapshot=(Snapshot $modOutside)})
    foreach($mod in $mods){
        $state.inputs+=@{name=$mod.name;path=$mod.path;source=$mod.source;sourceTree=$mod.sourceTree;entries=$mod.snapshot.Count}
        $mod.snapshot|ConvertTo-Json -Depth 4|Set-Content (Join-Path $evidence ('inputs-'+$mod.name+'.json'))
        $audit=[SuperCompilerCacheWriteAudit]::new($mod.path);$watchers+=,$audit;$mod.audit=$audit
    }
    $sdkAudit=[SuperCompilerCacheWriteAudit]::new($sdk);$watchers+=,$sdkAudit
    foreach($mod in $mods){$null=InvokeBuild ($mod.name+'-plan') 'plan' $mod.path 'big';Need (!(Test-Path $cacheBase)) 'Plan created a cache directory'}
    $null=InvokeBuild 'big-version-required' 'plan' $modInside 'big' $false
    foreach($mod in $mods){
        $cold=InvokeBuild ($mod.name+'-loose-cold') 'build' $mod.path 'loose'
        $session=$cold.report.cache.sessionCacheDirectory;Need ([bool]$session -and (Within $cacheBase $session)) 'Session cache escaped the supplied SDK'
        $partition=Split-Path $session;Need ((Split-Path $partition) -ieq $cacheBase) 'Cache did not use a MOD partition'
        Need ((Split-Path $partition -Leaf) -match '^.{1,12}_[0-9a-f]{32}$') 'MOD partition identity format differs'
        $mod.partition=$partition
        Need (@($cold.report.artifacts).Count -gt 0) 'Cold loose build produced no delivery'
        foreach($artifact in $cold.report.artifacts){
            Need (Within (Join-Path $cold.output 'published') $artifact.Path) 'A private cache path entered final artifacts'
            $relative=[IO.Path]::GetRelativePath((Join-Path $cold.output 'published'),$artifact.Path)
            $cached=Join-Path (Join-Path $partition 'streams') $relative
            Need ((Test-Path -LiteralPath $cached -PathType Leaf) -and (Hash $cached) -ceq (Hash $artifact.Path)) 'Loose export differs from the current cache stream'
        }
        $hot=InvokeBuild ($mod.name+'-loose-hot-newout') 'build' $mod.path 'loose'
        Need ($hot.report.cache.sessionCacheDirectory -ieq $session) 'Changing --out changed the MOD partition'
        Need ($hot.report.cache.xmlParse -eq 0 -and $hot.report.cache.instanceCompile -eq 0) 'Unmodified hot build parsed or compiled assets'
        Need (@($cold.report.artifacts).Count -eq @($hot.report.artifacts).Count) 'Hot delivery count differs'
        foreach($artifact in $cold.report.artifacts){
            $relative=[IO.Path]::GetRelativePath((Join-Path $cold.output 'published'),$artifact.Path)
            Need ((Hash $artifact.Path) -ceq (Hash (Join-Path (Join-Path $hot.output 'published') $relative))) 'Cold/hot loose delivery bytes differ'
        }
        $big=InvokeBuild ($mod.name+'-big-hot-newout') 'build' $mod.path 'big'
        Need ($big.report.cache.sessionCacheDirectory -ieq $session -and $big.report.cache.xmlParse -eq 0 -and $big.report.cache.instanceCompile -eq 0) 'BIG did not reuse the same warm stream cache'
        $published=@(Get-ChildItem -LiteralPath (Join-Path $big.output 'published') -Recurse -File)
        Need ($published.Count -eq 2 -and @($published|Where-Object Extension -notin @('.big','.skudef')).Count -eq 0) 'BIG output contains private cache/intermediate streams'
        Need (@(Get-ChildItem -LiteralPath (Join-Path $partition 'temp') -Force).Count -eq 0) 'Package temporary inputs were retained'
        Need (@(Get-ChildItem -LiteralPath $partition -Filter '*.asset' -Recurse -File).Count -gt 0) 'No reusable/intermediate asset was written to the partition'
        $mod.cacheInventory=@(Get-ChildItem -LiteralPath $partition -Recurse -File|ForEach-Object{@{path=[IO.Path]::GetRelativePath($partition,$_.FullName).Replace('\','/');bytes=$_.Length}})
        $mod.cacheInventory|ConvertTo-Json -Depth 4|Set-Content (Join-Path $evidence ('cache-'+$mod.name+'.json'))
    }
    Need ($mods[0].partition -ine $mods[1].partition) 'Different MODs shared a partition'
    Start-Sleep -Milliseconds 200
    foreach($mod in $mods){Need ($mod.audit.Errors.IsEmpty) "Source watcher failed: $($mod.name)";Need ($mod.audit.Paths.IsEmpty) "Source write event observed: $($mod.name) $($mod.audit.Paths.ToArray() -join ', ')";Unchanged $mod.path $mod.snapshot}
    Need ($sdkAudit.Errors.IsEmpty) 'SDK watcher overflowed or failed'
    foreach($path in $sdkAudit.Paths.ToArray()){Need (Within $cacheBase $path) "SDK write escaped SuperCompilerCache: $path"}
    $state.sourceWriteEvents=0;$state.sdkWriteEvents=$sdkAudit.Paths.Count;$state.sourceSnapshotsUnchanged=$true;$state.status='passed'
} catch {$state.status='failed';$state.error=$_.ToString();throw}
finally {
    $state.sourceAudits=@(foreach($mod in $mods){@{name=$mod.name;writeEvents=$mod.audit.Paths.Count;paths=@($mod.audit.Paths.ToArray()|Select-Object -First 100);errors=@($mod.audit.Errors.ToArray())}})
    if($sdkAudit){$state.sdkAudit=@{writeEvents=$sdkAudit.Paths.Count;outsideCache=@($sdkAudit.Paths.ToArray()|Where-Object{!(Within $cacheBase $_)}|Select-Object -First 100);errors=@($sdkAudit.Errors.ToArray())}}
    foreach($watcher in $watchers){$watcher.Dispose()}
    $state|ConvertTo-Json -Depth 16|Set-Content (Join-Path $evidence 'validation.json')
}
