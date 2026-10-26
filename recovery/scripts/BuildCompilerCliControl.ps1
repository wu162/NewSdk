#requires -Version 7.0
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
if(Test-Path -LiteralPath $OutputDirectory){throw "Refusing to overwrite CLI controller build: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Path $OutputDirectory).FullName
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Need([bool]$Condition,[string]$Message){if(!$Condition){throw $Message}}
$state=@{sourceCommit=(git -C $root rev-parse HEAD).Trim();sourceSha256=(Hash "$root/recovery/tests/CompilerCliControl.cs");succeeded=$false}
try {
    # Use the same modern NET48 toolchain as the application contracts and
    # canonical Windows source paths; Framework csc misparses mixed separators.
    $vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
    $vs=& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -property installationPath
    Need ($LASTEXITCODE -eq 0 -and !!$vs) 'VS Roslyn compiler not found'
    $csc=(Resolve-Path -LiteralPath (Join-Path $vs 'MSBuild/Current/Bin/Roslyn/csc.exe')).Path
    $refs=Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
    $controlSource=(Resolve-Path -LiteralPath (Join-Path $root 'recovery/tests/CompilerCliControl.cs')).Path
    $script:control=Join-Path $out 'ctrl.exe'
    $compilerArguments=@('/nologo','/noconfig','/nostdlib+','/langversion:7.3','/platform:x86','/codepage:65001',('/out:'+$script:control))
    $state.controllerReferences=@(foreach($name in @('mscorlib','System','System.Core','System.Web','System.Web.Extensions')){
        $reference=(Resolve-Path -LiteralPath (Join-Path $refs ($name+'.dll'))).Path
        $compilerArguments+=('/reference:'+$reference)
        @{name=$name;sha256=(Hash $reference)}
    })
    $compilerArguments+=$controlSource
    $state.controllerCompiler=@{path=$csc;sha256=(Hash $csc);version=([Diagnostics.FileVersionInfo]::GetVersionInfo($csc).FileVersion);languageVersion='7.3';target='net48/x86'}
    & $csc @compilerArguments *> "$out/controller-build.log"
    Need ($LASTEXITCODE -eq 0) 'External CLI control helper compilation failed'
    & $script:control --test-parent-identity *> "$out/parent-identity-tests.log"
    Need ($LASTEXITCODE -eq 0) 'Controller parent creation identity self-test failed'
    $state.parentIdentitySelfTest=@{succeeded=$true;logSha256=(Hash "$out/parent-identity-tests.log");assertions=6}
    $state.controllerSha256=Hash $script:control
    $state.executable=$script:control;$state.succeeded=$true
    Write-Output $script:control
} catch {if(Test-Path "$out/controller-build.log"){Get-Content "$out/controller-build.log" -Tail 60|Out-Host};throw}
finally {$state|ConvertTo-Json -Depth 6|Set-Content "$out/build.json"}
