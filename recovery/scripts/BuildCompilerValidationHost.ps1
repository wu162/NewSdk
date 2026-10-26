[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$ProductBuildDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$product=(Resolve-Path $ProductBuildDirectory).Path
if(Test-Path $OutputDirectory){throw "Refusing to reuse validation host output: $OutputDirectory"}
$out=(New-Item -ItemType Directory -Force $OutputDirectory).FullName
$obj=(New-Item -ItemType Directory "$out/obj").FullName
$bin=(New-Item -ItemType Directory "$out/bin").FullName
function Hash([string]$Path){(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash.ToLowerInvariant()}
function Invoke-Checked([string]$Command,[string[]]$Arguments,[string]$Log){
    & $Command @Arguments *> $Log
    if($LASTEXITCODE -ne 0){Get-Content $Log -Tail 100|Out-Host;throw "Validation host tool failed: $Command ($Log)"}
}
Push-Location $root
try {
    $receipt=Get-Content "$product/build.json" -Raw|ConvertFrom-Json
    $recipe=Get-Content "$product/link-inputs.json" -Raw|ConvertFrom-Json
    $sourceCommit=(git rev-parse HEAD).Trim()
    if($recipe.schemaVersion -ne 1 -or $receipt.sourceCommit -cne $sourceCommit -or $recipe.sourceCommit -cne $sourceCommit -or
        $recipe.configuration -cne $receipt.configuration){throw 'Production link recipe source/configuration mismatch'}
    $productExe=(Resolve-Path "$product/bin/NewModCompiler.exe").Path
    $productHash=Hash $productExe
    if($productHash -cne $receipt.exeSha256){throw 'Production EXE differs from build receipt'}
    $seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach($input in $recipe.inputs){
        if(!$seen.Add($input.path) -or (Hash $input.path) -cne $input.sha256){throw "Missing, duplicate or modified production link input: $($input.path)"}
    }
    foreach($kind in @('foundation','application','entry')){
        if(@($recipe.inputs|Where-Object{$_.kind -ceq $kind}).Count -ne 1){throw "Expected exactly one $kind input"}
    }
    $foundation=@($recipe.inputs|Where-Object{$_.kind -ceq 'foundation'})[0]
    $application=@($recipe.inputs|Where-Object{$_.kind -ceq 'application'})[0]
    $entry=@($recipe.inputs|Where-Object{$_.kind -ceq 'entry'})[0]
    if($foundation.sha256 -cne $receipt.foundationSha256 -or $application.sha256 -cne $receipt.applicationSha256){throw 'Module hashes differ between product receipts'}
    if((@($recipe.inputs|Where-Object{$_.kind -ceq 'nativeOrBridge'}|ForEach-Object{$_.path}) -join "`n") -cne (@($receipt.nativeBuild.objects) -join "`n")){throw 'Native/Bridge object set differs from production receipt'}
    $hostSource=(Resolve-Path 'recovery/tests/CompilerValidationEntry.cpp').Path
    $hostObject=Join-Path $obj 'CompilerValidationEntry.obj'
    $optimization=if($receipt.configuration -ceq 'Release'){'/O2'}else{'/Od'}
    Invoke-Checked 'cl.exe' @('/nologo','/c','/clr','/MD','/EHa','/fp:precise',$optimization,('/AI'+(Split-Path $application.path)),('/AI'+(Split-Path $foundation.path)),('/Fo'+$hostObject),$hostSource) "$out/entry-build.log"
    $exe=Join-Path $bin 'NewModCompiler.exe'
    $arguments=@();$replaced=@{output=0;symbol=0;object=0}
    foreach($argument in $recipe.arguments){
        if($argument -ceq ('/OUT:'+$productExe)){$arguments+=('/OUT:'+$exe);$replaced.output++}
        elseif($argument -ceq '/ENTRY:NewModCompiler.Hosting.CompilerEntry.Main'){$arguments+='/ENTRY:NewModCompiler.Validation.ValidationEntry.Main';$replaced.symbol++}
        elseif($argument -ceq $entry.path){$arguments+=$hostObject;$replaced.object++}
        else{$arguments+=$argument}
    }
    if($replaced.output -ne 1 -or $replaced.symbol -ne 1 -or $replaced.object -ne 1){throw 'Production link recipe does not have exactly the expected entry/output replacements'}
    $rsp=Join-Path $obj 'Link.rsp'
    [IO.File]::WriteAllLines($rsp,[string[]]@($arguments|ForEach-Object{'"'+$_+'"'}),[Text.UTF8Encoding]::new($false))
    Invoke-Checked 'link.exe' @('@'+$rsp) "$out/link.log"
    Copy-Item "$productExe.config" "$exe.config"
    Copy-Item "$product/bin/antlr.runtime.dll" $bin
    foreach($input in $recipe.inputs){if((Hash $input.path) -cne $input.sha256){throw 'Production link input changed while linking validation host'}}
    if((Hash $productExe) -cne $productHash){throw 'Validation linking changed the production executable'}
    @{
        schemaVersion=1;sourceCommit=$sourceCommit;configuration=$receipt.configuration
        productExeSha256=$productHash;exeSha256=(Hash $exe);sameProductionObjects=$true
        entryPoint='NewModCompiler.Validation.ValidationEntry.Main';productionEntryPoint=$receipt.entryPoint
        inputs=@($recipe.inputs|Where-Object{$_.kind -cne 'entry'})
        entrySource=@{path='recovery/tests/CompilerValidationEntry.cpp';sha256=(Hash $hostSource)}
        entryObjectSha256=(Hash $hostObject);linkRecipeSha256=(Hash "$product/link-inputs.json")
        status='linked test-only host; runtime validation required';productionExeUnchanged=$true
    }|ConvertTo-Json -Depth 8|Set-Content "$out/build.json"
    Write-Output $exe
} finally {Pop-Location}
