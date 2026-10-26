[CmdletBinding()]
param([ValidateSet('Debug','Release')][string]$Configuration = 'Release', [string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
if (-not $OutputDirectory) { $OutputDirectory = Join-Path $PSScriptRoot "..\artifacts\app\$Configuration" }
& (Join-Path $PSScriptRoot 'BuildApp.ps1') -Configuration $Configuration -OutputDirectory $OutputDirectory
& (Join-Path $PSScriptRoot 'BuildApp.ps1') -Configuration $Configuration -OutputDirectory $OutputDirectory -Tests
& (Join-Path $OutputDirectory 'NewModCompiler.AppTests.exe') (Join-Path $OutputDirectory 'test-work')
if ($LASTEXITCODE -ne 0) { throw "Application contract tests failed: $LASTEXITCODE" }
