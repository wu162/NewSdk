# Export complete managed assemblies, retaining original assembly/type/member identities.
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$work = Join-Path $env:RUNNER_TEMP ('utility-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work | Out-Null
$out = "recovery/evidence/utility/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
New-Item -ItemType Directory -Force $out | Out-Null
dotnet tool install ilspycmd --tool-path $work --version 9.1.0.7988 *> "$out/ilspy-install.log"
if ($LASTEXITCODE -ne 0) { throw 'Pinned ILSpy install failed' }
$receipts = @()
foreach ($name in @('Utility')) {
    $inputFile = "$root/SDK/tools/BinaryAssetBuilder.$name.dll"
    $destination = "$root/recovery/src/Managed/$name"
    if (Test-Path $destination) { throw "Refusing to overwrite recovered source: $destination" }
    & "$work/ilspycmd.exe" -p -o $destination -r "$root/SDK/tools" $inputFile *> "$out/$name-decompile.log"
    if ($LASTEXITCODE -ne 0) { Get-Content "$out/$name-decompile.log"; throw "$name decompilation failed" }
    & "$work/ilspycmd.exe" -il $inputFile > "$out/$name-original.il"
    if ($LASTEXITCODE -ne 0) { throw "$name IL extraction failed" }
    # ILSpy emits HintPaths relative to its working directory. Make projects portable in this repository.
    $projectPath = "$destination/BinaryAssetBuilder.$name.csproj"
    [xml]$project = Get-Content $projectPath -Raw
    foreach ($hint in @($project.SelectNodes('//HintPath'))) {
        $hint.InnerText = '../../../../SDK/tools/' + [IO.Path]::GetFileName($hint.InnerText.Replace('\', '/'))
    }
    $group = $project.SelectSingleNode('/Project/PropertyGroup')
    $lang = $project.CreateElement('LangVersion')
    $lang.InnerText = '10.0'
    [void]$group.AppendChild($lang)
    $project.Save($projectPath)
    $receipts += @{
        assembly = "BinaryAssetBuilder.$name"
        originalSha256 = (Get-FileHash $inputFile -Algorithm SHA256).Hash.ToLowerInvariant()
        sources = @(Get-ChildItem $destination -Recurse -Filter *.cs).Count
        project = "recovery/src/Managed/$name/BinaryAssetBuilder.$name.csproj"
    }
}
@{
    sourceCommit = (git rev-parse HEAD).Trim()
    runId = $env:GITHUB_RUN_ID
    runAttempt = $env:GITHUB_RUN_ATTEMPT
    ilspyVersion = '9.1.0.7988'
    scope = 'Complete ILSpy project export; validation follows separately'
    assemblies = $receipts
    sdkValidated = $false
} | ConvertTo-Json -Depth 8 | Set-Content "$out/extraction.json"
