param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path "$PSScriptRoot/../..").Path
Set-Location $root
if (!$env:RUNNER_TEMP) { $env:RUNNER_TEMP = [IO.Path]::GetTempPath() }
if (!$OutputDirectory) { $OutputDirectory = "recovery/evidence/w3x/preparation-runs/$env:GITHUB_RUN_ID-$env:GITHUB_RUN_ATTEMPT" }
if (Test-Path $OutputDirectory) { throw 'Refusing to overwrite preparation evidence' }
New-Item -ItemType Directory $OutputDirectory | Out-Null
$out = (Resolve-Path $OutputDirectory).Path
foreach ($name in @('VerifyW3xSdkGate.ps1','TestW3xSdk.ps1','BuildW3xPlugin.ps1','TestW3xPlugin.ps1')) {
    $tokens = $null; $parseErrors = $null
    [System.Management.Automation.Language.Parser]::ParseFile((Join-Path $PSScriptRoot $name),[ref]$tokens,[ref]$parseErrors) | Out-Null
    if ($parseErrors.Count) { $parseErrors | Out-String | Write-Host; throw "Invalid PowerShell source: $name" }
}
$hostDirectory = Join-Path $env:RUNNER_TEMP ('w3x-preparation-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $hostDirectory | Out-Null
Copy-Item SDK/tools/BinaryAssetBuilder.Core.dll $hostDirectory
$csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
$exe = Join-Path $hostDirectory 'W3xSdkOracle.exe'
& $csc /nologo /platform:x86 /optimize+ /r:System.Core.dll /r:System.Xml.dll /r:"$hostDirectory/BinaryAssetBuilder.Core.dll" /out:"$exe" (Resolve-Path 'recovery/tests/W3xSdkOracle.cs').Path *> "$out/oracle-build.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/oracle-build.log"; throw 'SDK test host build failed (SDK not started)' }
'<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$exe.config"
& $exe schema (Join-Path $root 'SDK') W3xRecovery (Join-Path $out 'expected-assets.txt') *> "$out/schema.log"
if ($LASTEXITCODE -ne 0) { Get-Content "$out/schema.log"; throw 'Full original SDK schema validation failed (SDK not started)' }
Get-Content "$out/schema.log"
@{ succeeded=$true;sourceCommit=(git rev-parse HEAD).Trim();runId=$env:GITHUB_RUN_ID;runAttempt=$env:GITHUB_RUN_ATTEMPT;sdkStarted=$false;scope='PowerShell parsing, SDK oracle compilation and complete original CnC3Types XSD validation only; no BAB execution' } | ConvertTo-Json | Set-Content "$out/validation.json"
