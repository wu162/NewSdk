#requires -Version 7.0
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
. "$PSScriptRoot/CompilerCliOutputs.ps1"
if(Test-Path -LiteralPath $OutputDirectory){throw 'Refusing to overwrite output-oracle tests'}
New-Item -ItemType Directory -Path $OutputDirectory|Out-Null
$path=Join-Path $OutputDirectory 'fixture.manifest'
function Fixture([string]$Name='Texture:Expected',[uint32]$Type=2777716390u){
    $names=[Text.Encoding]::ASCII.GetBytes($Name+[char]0)
    $data=[byte[]]::new(96+$names.Length)
    $data[1]=1;$data[2]=6
    [BitConverter]::GetBytes([uint32]1).CopyTo($data,12)
    [BitConverter]::GetBytes([uint32]$names.Length).CopyTo($data,40)
    [BitConverter]::GetBytes($Type).CopyTo($data,48)
    [BitConverter]::GetBytes([uint32]0x01234567).CopyTo($data,52)
    [BitConverter]::GetBytes([uint32]2309737967).CopyTo($data,56)
    [BitConverter]::GetBytes([uint32]0x13579bdf).CopyTo($data,60)
    $names.CopyTo($data,96)
    return ,$data
}
$passed=@()
try {
    [IO.File]::WriteAllBytes($path,(Fixture))
    $result=@(Get-CompilerCustomDataOutputs $path @('Texture:Expected'))
    if($result.Count -ne 1 -or $result[0].qualifiedName -cne 'Texture:Expected' -or $result[0].relativePath -cne 'cdata/a59096a6.89abcdef.01234567.13579bdf.cdata' -or $result[0].path -cne (Join-Path $OutputDirectory 'fixture/cdata/a59096a6.89abcdef.01234567.13579bdf.cdata')){throw 'Independent manifest handle/field-order allowlist assertion failed'}
    $passed+='exact-handle-path'
    $cases=@(
        @{name='unexpected-texture';data=(Fixture 'Texture:Other');expected=@('Texture:Expected')},
        @{name='missing-texture';data=(Fixture 'ArmorTemplate:Expected' 1);expected=@('Texture:Expected')},
        @{name='wrong-route-id';data=(Fixture 'Texture:Expected' 1);expected=@('Texture:Expected')},
        @{name='unexpected-custom-route';data=(Fixture 'OnDemandTexture:Expected');expected=@()},
        @{name='duplicate-expectation';data=(Fixture);expected=@('Texture:Expected','Texture:Expected')},
        @{name='invalid-qualified-expectation';data=(Fixture);expected=@('Expected')}
    )
    $badVersion=Fixture;$badVersion[0]=1;$cases+=@{name='big-endian-refused';data=$badVersion;expected=@('Texture:Expected')}
    $badBounds=Fixture;$badBounds[40]=255;$cases+=@{name='table-bounds';data=$badBounds;expected=@('Texture:Expected')}
    $badOffset=Fixture;$badOffset[72]=255;$cases+=@{name='name-offset';data=$badOffset;expected=@('Texture:Expected')}
    $badTerminator=Fixture;$badTerminator[-1]=65;$cases+=@{name='unterminated-name';data=$badTerminator;expected=@('Texture:Expected')}
    foreach($case in $cases){
        [IO.File]::WriteAllBytes($path,$case.data)
        $rejected=$false
        try {$null=Get-CompilerCustomDataOutputs $path $case.expected}catch{$rejected=$true}
        if(!$rejected){throw "Output oracle accepted invalid contract: $($case.name)"}
        $passed+=$case.name
    }
    [IO.File]::WriteAllBytes($path,(Fixture 'ArmorTemplate:Expected' 1))
    if(@(Get-CompilerCustomDataOutputs $path @()).Count -ne 0){throw 'Nontexture manifest gained CustomData'}
    $passed+='no-texture-zero-custom'
    @{succeeded=$true;cases=$passed;count=$passed.Count}|ConvertTo-Json -Depth 4|Set-Content (Join-Path $OutputDirectory 'validation.json')
    Write-Host "PASS CompilerCliOutputs: $($passed.Count) manifest-bound CustomData contracts"
    $global:LASTEXITCODE=0
} finally {Remove-Item -LiteralPath $path -Force -ErrorAction SilentlyContinue}
