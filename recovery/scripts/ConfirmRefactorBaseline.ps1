param([Parameter(Mandatory=$true)][string]$EvidenceDirectory)
$ErrorActionPreference='Stop'
$directory=(Resolve-Path $EvidenceDirectory).Path
$receipt=Get-Content "$directory/validation.json" -Raw|ConvertFrom-Json
if(!$receipt.succeeded -or !$receipt.originalBaselinePassed -or
   $receipt.sourceCommit -ne '87cf54e254dfb44a129da087316fd04806ed2b10' -or
   $receipt.harnessCommit -ne '1d260dfc1806b772aebfef568bec23031adee238' -or
   $receipt.modSourceCommit -ne '068643219fded09a05fb7b75c6507b769c430316' -or
   $receipt.runId -ne '40269297741' -or $receipt.runAttempt -ne '1') {
    throw 'Unsuccessful or mismatched fixed G0 receipt'
}
if(@($receipt.invocations).Count -ne 10 -or @($receipt.comparisons).Count -ne 20 -or
   @($receipt.invocations|Where-Object{!$_.succeeded -or $_.exitCode -ne 0 -or $_.timedOut}).Count -or
   @($receipt.comparisons|Where-Object{!$_.identical}).Count -or
   !$receipt.customData.identical -or $receipt.customData.files -ne 1293 -or
   !$receipt.audioCalls.identical -or $receipt.audioCalls.original -ne 1289 -or $receipt.audioCalls.recovered -ne 1289) {
    throw 'Missing SDK invocation/output/CustomData/callback evidence'
}
foreach($mode in @('Original','Recovered')) {
    if(@($receipt.invocations|Where-Object mode -eq $mode).Count -ne 5){throw 'Wrong per-mode invocation count'}
}
foreach($original in @($receipt.invocations|Where-Object mode -eq Original)) {
    $recovered=@($receipt.invocations|Where-Object{$_.mode -eq 'Recovered' -and $_.step -eq $original.step -and $_.input -ceq $original.input})
    if($recovered.Count -ne 1){throw 'Missing paired invocation'}
    foreach($field in @('executable','workingDirectory','inputAbsolute','arguments')) {
        if($original.$field -cne $recovered[0].$field){throw "Original/recovered invocation path or parameter mismatch: $field"}
    }
}
$a=@(Get-Content "$directory/Original-custom.json" -Raw|ConvertFrom-Json)
$b=@(Get-Content "$directory/Recovered-custom.json" -Raw|ConvertFrom-Json)
if($a.Count -ne 1293 -or (ConvertTo-Json -InputObject $a -Compress) -cne (ConvertTo-Json -InputObject $b -Compress)){throw 'CustomData receipt mismatch'}
$a=@(Get-Content "$directory/Original-audio-calls.tsv"|Sort-Object)
$b=@(Get-Content "$directory/Recovered-audio-calls.tsv"|Sort-Object)
if($a.Count -ne 1289 -or (ConvertTo-Json -InputObject $a -Compress) -cne (ConvertTo-Json -InputObject $b -Compress)){throw 'Audio callback receipt mismatch'}
foreach($file in @('inputs.json','sdk-tools.json','schemas.json','Original.config','Recovered.config')) {
    if(!(Test-Path "$directory/$file") -or (Get-Item "$directory/$file").Length -eq 0){throw "Missing baseline input/deployment evidence: $file"}
}
Write-Host 'PASS reused fixed G0: 10 real SDK calls, 20 identical four-stream comparisons, 1293 CustomData files, 1289 audio callbacks per mode'
