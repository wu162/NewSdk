$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
$work = Join-Path $env:RUNNER_TEMP ('build-infrastructure-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $work | Out-Null
$out = "recovery/evidence/build-infrastructure/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT"
New-Item -ItemType Directory -Force $out | Out-Null
$receipt = @{ sourceCommit = (git rev-parse HEAD).Trim(); runId = $env:GITHUB_RUN_ID; runAttempt = $env:GITHUB_RUN_ATTEMPT; ilspyVersion = '9.1.0.7988'; sdkValidated = $false; status = 'started'; assemblies = @(); builds = @() }
try {
    dotnet tool install ilspycmd --tool-path $work --version 9.1.0.7988 *> "$out/ilspy-install.log"
    if ($LASTEXITCODE -ne 0) { throw 'ILSpy installation failed' }
    foreach ($name in @('Core', 'Remote', 'EALABuild')) {
        $assembly = if ($name -eq 'EALABuild') { 'EALABuild' } else { "BinaryAssetBuilder.$name" }
        $inputFile = if ($name -eq 'EALABuild') { "$root/SDK/EALABuild.dll" } else { "$root/SDK/tools/$assembly.dll" }
        $destination = "$root/recovery/src/Managed/$name"
        if (!(Test-Path $destination)) {
            & "$work/ilspycmd.exe" -p -o $destination -r "$root/SDK/tools" -r "$root/SDK" $inputFile *> "$out/$name-decompile.log"
            if ($LASTEXITCODE -ne 0) { throw "$name decompilation failed" }
            $projectPath = "$destination/$assembly.csproj"
            [xml]$project = Get-Content $projectPath -Raw
            foreach ($hint in @($project.SelectNodes('//HintPath'))) {
                $file = [IO.Path]::GetFileName($hint.InnerText.Replace('\', '/'))
                $hint.InnerText = if (Test-Path "$root/SDK/tools/$file") { "../../../../SDK/tools/$file" } elseif (Test-Path "$root/SDK/$file") { "../../../../SDK/$file" } else { throw "Unknown dependency: $file" }
            }
            foreach ($lang in @($project.SelectNodes('//LangVersion'))) { $lang.InnerText = '10.0' }
            $project.Save($projectPath)
        }
        & "$work/ilspycmd.exe" -il $inputFile > "$work/$name-original.il"
        if ($LASTEXITCODE -ne 0) { throw "$name IL extraction failed" }
        Compress-Archive "$work/$name-original.il" "$out/$name-original.il.zip"
        $receipt.assemblies += @{ name = $assembly; originalSha256 = (Get-FileHash $inputFile).Hash.ToLowerInvariant(); sources = @(Get-ChildItem $destination -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }).Count }
    }
    foreach ($mode in @('Debug', 'Release')) {
        foreach ($name in @('Core', 'Remote', 'EALABuild')) {
            $assembly = if ($name -eq 'EALABuild') { 'EALABuild' } else { "BinaryAssetBuilder.$name" }
            dotnet build "recovery/src/Managed/$name/$assembly.csproj" -c $mode -p:PlatformTarget=x86 -o "$work/$mode/$name" --nologo *> "$out/$mode-$name-build.log"
            $code = $LASTEXITCODE
            $receipt.builds += @{ mode = $mode; assembly = $assembly; exitCode = $code }
            if ($code -ne 0) { Get-Content "$out/$mode-$name-build.log" -Tail 55; throw "$mode $name build failed" }
        }
    }
    $receipt.status = 'builds-passed'
} finally {
    $receipt | ConvertTo-Json -Depth 10 | Set-Content "$out/receipt.json"
    Get-ChildItem recovery/src/Managed/Core,recovery/src/Managed/Remote,recovery/src/Managed/EALABuild -Recurse -Directory -ErrorAction SilentlyContinue | Where-Object { $_.Name -in @('bin', 'obj') } | Remove-Item -Recurse -Force
}
