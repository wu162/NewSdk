param(
    [Parameter(Mandatory)][string]$SdkRoot,
    [Parameter(Mandatory)][string]$ModName,
    [Parameter(Mandatory)][string]$EvidenceDirectory,
    [int]$TimeoutSeconds = 300
)
$ErrorActionPreference = 'Stop'
$sdk = (Resolve-Path $SdkRoot).Path
$source = Join-Path $sdk 'defaultscript.cs'
$code = [IO.File]::ReadAllText($source)
# ModStudioUI.loadBuildScript compiles this shipped file; its BuildScript drives the GUI.
$flags = '/csc:false /ls:true /osh:false /pc:true /res:true /slowclean:true /ss:true'
if (!$code.Contains($flags) -or !$code.Contains('private void executeModBuildStep(int stepId)') -or !$code.Contains('public void buildMod(string modName')) { throw 'Shipped EALAModStudio build contract changed; re-extract it before compiling' }
$modPath = Join-Path $sdk "Mods/$ModName"
$modData = Join-Path $modPath 'Data'
$modXml = Join-Path $modData 'Mod.xml'
if (!(Test-Path $modXml)) { throw "Missing actual SDK MOD: $modXml" }
$built = Join-Path $sdk 'builtmods'
$builtMod = Join-Path $built "mods/$ModName"
$builtData = Join-Path $builtMod 'data'
$report = [IO.Path]::GetFullPath($EvidenceDirectory)
New-Item -ItemType Directory -Force $report | Out-Null
$result = [ordered]@{
    succeeded = $false; entry = 'EALAModStudio.ModStudioUI.loadBuildScript -> SDK/defaultscript.cs BuildScript.buildMod -> executeModBuildStep'
    defaultscriptSha256 = (Get-FileHash $source -Algorithm SHA256).Hash
    modName = $ModName; workingDirectory = $sdk; sourceModXml = $modXml
    selectedSteps = @(1,2,5); disabledSteps = @(3,4,6,7,8,9,10,11,12)
    disabledReason = 'User requested only establishing base data (Studio case5); no AptUI/additionalmaps, LoD, asset merge/fix, copy, packaging or installation steps'
    steps = @(); outputs = @(); status = 'started'
}
function Invoke-EalaTool([int]$Step, [string]$Label, [string]$Tool, [string[]]$ToolArguments) {
    $exe = Join-Path $sdk "tools/$Tool"
    if (!(Test-Path $exe)) { throw "Missing EALAModStudio tool $exe" }
    $index = $result.steps.Count
    $stdout = Join-Path $report ("{0:D2}-{1}-stdout.txt" -f $index,$Label)
    $stderr = Join-Path $report ("{0:D2}-{1}-stderr.txt" -f $index,$Label)
    $arguments = $ToolArguments -join ' '
    $start = [DateTime]::UtcNow
    $p = Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $sdk -NoNewWindow -PassThru -RedirectStandardOutput $stdout -RedirectStandardError $stderr
    if (!$p.WaitForExit($TimeoutSeconds * 1000)) { Stop-Process -Id $p.Id -Force; throw "Studio step $Step/$Label timed out" }
    $p.Refresh()
    $row = [ordered]@{ step = $Step; label = $Label; executable = $Tool; arguments = $arguments; workingDirectory = $sdk; exitCode = $p.ExitCode; startedUtc = $start.ToString('o'); endedUtc = [DateTime]::UtcNow.ToString('o'); stdout = [IO.Path]::GetFileName($stdout); stderr = [IO.Path]::GetFileName($stderr) }
    $result.steps += $row
    if ($p.ExitCode -ne 0) { Get-Content $stdout; Get-Content $stderr; throw "Studio step $Step/$Label failed: $($p.ExitCode)" }
}
function Invoke-StudioBab([int]$Step, [string]$Label, [string]$Xml) {
    # Exact base-data BAB options from defaultscript.cs executeModBuildStep case5; user excludes subsequent LoD and packaging.
    $babArguments = @("`"$Xml`"", "/od:`"$built`"", "/iod:`"$built`"", '/csc:false','/ls:true','/osh:false','/pc:true','/res:true','/slowclean:true','/ss:true',
        "/art:`".;.\Mods\$ModName\Art;.\Mods;.\Art`"", "/audio:`".;.\Mods\$ModName\Audio;.\Mods;.\Audio`"", "/data:`".;.\Mods\$ModName\Data;.\Mods;.\SageXml`"")
    Invoke-EalaTool $Step $Label 'BinaryAssetBuilder.exe' $babArguments
}
try {
    # Fresh SDK copies have no valuable build state. This is the clean result of GUI steps1+2.
    if (Test-Path $built) { Remove-Item $built -Recurse -Force }
    New-Item -ItemType Directory -Force $builtData | Out-Null
    $result.steps += [ordered]@{ step=1; label='clear-built-mod'; succeeded=$true; freshCopy=$true }
    $result.steps += [ordered]@{ step=2; label='clear-cache'; succeeded=$true; freshCopy=$true }
    Invoke-StudioBab 5 'mod' $modXml
    foreach ($ext in @('bin','manifest','imp','relo')) {
        $path = Join-Path $builtData "mod.$ext"
        if (!(Test-Path $path)) { throw "Studio BAB did not create $path" }
        $result.outputs += [ordered]@{name="mod.$ext";stage='BAB';bytes=(Get-Item $path).Length;sha256=(Get-FileHash $path -Algorithm SHA256).Hash}
    }
    $result.succeeded=$true; $result.status='EALAModStudio base-data BAB invocation completed; later GUI stages intentionally excluded'
} catch { $result.status='failed';$result['error']=$_.ToString();throw }
finally { $result | ConvertTo-Json -Depth 15 | Set-Content -Encoding UTF8 (Join-Path $report 'studio-flow.json') }
