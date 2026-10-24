param()
$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path "$PSScriptRoot/../..")
$repo = (Get-Location).Path
$evidence = Join-Path $repo 'recovery/evidence/xml-priority-link'
$tempRoot = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
$out = Join-Path $tempRoot 'ra3-recovered-xml-priority-link'
New-Item -ItemType Directory -Force $evidence, $out | Out-Null
# Combined-TU/CLI metadata/link smoke including the completion paths; group full matrices remain mandatory.
$expectedRootTypeIds = [ordered]@{
    AttributeModifier = [uint32]3319822471
    FXList = [uint32]2254974584
    SpecialPowerTemplate = [uint32]2178440954
    ObjectCreationList = [uint32]3899542881
    LocomotorTemplate = [uint32]3972178387
    LogicCommandSet = [uint32]3959844197
    FXParticleSystemTemplate = [uint32]686292351
    UpgradeTemplate = [uint32]3786401627
    AIPersonalityDefinition = [uint32]3604279694
    AIStrategicStateDefinition = [uint32]607123156
    AIMicroManagerData = [uint32]2423780215
    AIBudgetStateDefinition = [uint32]2893598307
    AIStateDefinition = [uint32]3239935619
    AITargetingHeuristic = [uint32]926814458
    LogicCommand = [uint32]2101756272
    PlayerTech = [uint32]2131237081
    PlayerTechRank = [uint32]1802643394
    PlayerTechUpgradeBinding = [uint32]2140592493
    ExperienceLevelTemplate = [uint32]1874610847
    PlayerTechStoreTemplate = [uint32]1213037816
    PurchasePlayerTechButtonTemplate = [uint32]1566941920
    ButtonSingleStateData = [uint32]3048039928
    UICommandButton = [uint32]134745024
    FontDefaultSetting = [uint32]1371131236
    FontTranslation = [uint32]1318336226
    FontSubstitution = [uint32]91719417
    Weather = [uint32]2430161325
    WaterTransparency = [uint32]1479531562
    WaterSetting = [uint32]2728605516
    Environment = [uint32]680780553
    LightingSettings = [uint32]3169690211
    WaterTextureList = [uint32]2995571269
    TerrainParticleMapping = [uint32]1469498551
    BounceKickTerrainMapping = [uint32]2368878637
    ShadowMap = [uint32]4157475773
    OrdersManagerSettings = [uint32]2996766068
    TerrainVisualSettings = [uint32]1184276792
    CameraSettings = [uint32]3898802054
    AIData = [uint32]3827723072
    UnitAISettings = [uint32]2964891936
    PlayerTemplate = [uint32]4063352333
    MultiplayerSettings = [uint32]191082224
    AIBuildStateDefinition = [uint32]1644972043
    AIStrategicStateName = [uint32]2010562192
    MultiplayerColor = [uint32]2384988189
    AIPersonalityToAchievementEventMapping = [uint32]4086175864
    FormationAssistantUnitDefinition = [uint32]838130914
    FormationAssistantTemplate = [uint32]2912385462
    FormationSelectionTemplate = [uint32]3374519264
    FormationAssistant = [uint32]1963196508
    GameStatsModeData = [uint32]3249689110
    SkirmishSettings = [uint32]402082255
    DamageFX = [uint32]2905958645
    DamageFXSettings = [uint32]75989905
    GeneralHotKeySlots = [uint32]417176643
    DefaultHotKeys = [uint32]3741098742
    DebugHotKeys = [uint32]3359870461
    MappableKey = [uint32]1350608344
    AudioEventLimitGroup = [uint32]3906227957
    AudioLOD = [uint32]299416263
    AudioSettings = [uint32]1443425905
    Video = [uint32]587347702
    VideoEventList = [uint32]1447728787
    MicrophoneSettings = [uint32]1826140078
    BootupDisplaySequence = [uint32]3370391592
    HotKeySlot = [uint32]2800139175
    MouseCursor = [uint32]3784442503
    BasicMouseCursors = [uint32]611271731
    UIMouseDragSelectionFeedback = [uint32]611126373
    UIFloatingTextDisplay = [uint32]1559937240
    UIMessageDisplay = [uint32]1697313181
    UIMilitaryCaptionDisplay = [uint32]1043989897
    UISubtitleDisplay = [uint32]165611890
    UITimerDisplay = [uint32]951036436
    DynamicGameLOD = [uint32]2901356964
    GameLODPreset = [uint32]354222972
    StaticGameLOD = [uint32]531798225
    UICoopTinting = [uint32]4243804314
    RadiusCursor = [uint32]544776545
    AnimationSoundSettings = [uint32]1302035577
    MiscAudio = [uint32]2070603733
    LargeGroupAudioMap = [uint32]534008255
    UIMouseTacticalSelectionDetailsButtonHotKeys = [uint32]1296329066
    UIMouseTacticalMainCommandBarHotKeys = [uint32]2591692061
    UIMouseTacticalPlayerPowerHotKeys = [uint32]2283998413
    UIMouseTacticalMiscHotKeys = [uint32]2014061824
    UIMouseHUDElementHotKeyGate = [uint32]2738273474
    UIQuickChatMessage = [uint32]4098431913
    UIMouseTacticalQuickChatDisplay = [uint32]2891878770
    UIMouseVoiceChat = [uint32]3746130610
    UIDecalGrid = [uint32]2599418799
    UIDecalCloud = [uint32]2524893660
    UIDecalDestinationMarker = [uint32]1532214660
    UIWorldBuilderDecalGrid = [uint32]1238702511
    BaseAudioEventInfo = [uint32]1079232276
    BaseSingleSound = [uint32]3228425025
    AudioEvent = [uint32]2219670431
    MusicTrack = [uint32]1883691512
    MP3MusicTrack = [uint32]1013434420
    DialogEvent = [uint32]3558134211
    MP3DialogEvent = [uint32]1479988269
    AmbientStream = [uint32]3928762264
    MP3AmbientStream = [uint32]1813598662
    Multisound = [uint32]2745675575
    PathMusicGameDynamicState = [uint32]97402485
    PathMusicGameDynamicStateSet = [uint32]343094706
    PathMusicGameDynamicTransition = [uint32]2343360052
    PathMusicEventRuntime = [uint32]1122327197
    PathMusicEventSet = [uint32]3339496789
    PathMusicMapRuntime = [uint32]2026048286
    MusicPalette = [uint32]2407383451
    MusicScriptConditionNuggetBase = [uint32]2292378780
    MusicScriptConditionExpensiveNuggetBase = [uint32]849624115
    MusicScriptConditionNugget_LocalPlayerIsObserver = [uint32]188286714
    MusicScriptConditionNugget_UnitsFarFromBase = [uint32]2620791304
    MusicScriptConditionNugget_TimeFromStartOfLevel = [uint32]1464425282
    MusicScriptConditionNugget_TrackPlayedCount = [uint32]57370491
    MusicScriptConditionNugget_SpecificTrackTypePlaying = [uint32]739280456
    MusicScriptConditionNugget_AnyTrackPlaying = [uint32]3402992833
    MusicScriptConditionNugget_ObjectsOfTypeExist = [uint32]4286308287
    MusicScriptConditionNugget_EvaEventPlayedRecently = [uint32]2743973749
    MusicScriptConditionNugget_ObjectsNearEvaEvent = [uint32]3386715757
    MusicScriptConditionNugget_ScoredKillCount = [uint32]3620764004
    MusicScriptConditionNugget_Not = [uint32]4007043648
    MusicScriptConditionNugget_Or = [uint32]2281773838
    MusicScriptConditionNugget_And = [uint32]1343682755
    MusicScriptConditionNugget_PathMusicStateLastActive = [uint32]2912435838
    MusicScriptConditionNugget_PathMusicStateCurrentlyActive = [uint32]3727134806
    MusicScriptConditionNugget_EngagementCount = [uint32]3476397162
    MusicScriptConditionNugget_LocalPlayerKillRatio = [uint32]2818967523
    MusicScriptConditionNugget_HitBySuperWeapon = [uint32]1634186921
    MusicScriptTrack = [uint32]1038067901
    UIMouseTacticalCoOpAIButtonsHelpButton = [uint32]104126924
    UIMouseTacticalCommandCommunicateWithAllies = [uint32]113029208
    UIMouseTacticalToggleRepairModeButton = [uint32]346077409
    UIMouseTacticalAttackMoveButton = [uint32]442957564
    UIMouseTacticalCommandPlaceWall = [uint32]485977567
    UIMouseTacticalForceAttackMoveButton = [uint32]694838069
    UIMouseTacticalCoOpAIButtonsHoldObjectButton = [uint32]772462768
    UIMouseTacticalExitContainerButton = [uint32]1014034471
    UIMouseTacticalPlayerTechStoreButton = [uint32]1138847078
    UIMouseTacticalCommandMove = [uint32]1234485619
    UIMouseTacticalCommandSell = [uint32]1249197161
    UIMouseTacticalUnitIntroButton = [uint32]1277109486
    UIMouseTacticalCommandSelect = [uint32]1334543376
    UIMouseTacticalCoOpAIButtonsNormalButton = [uint32]1371977384
    UIMouseTacticalCommandPlaceRallyPoint = [uint32]1474300054
    UIMouseTacticalCommandHarvest = [uint32]1499648070
    UIMouseTacticalCommandToggleSelfRepair = [uint32]1956336137
    UIMouseTacticalBuildQueueTab = [uint32]1974922393
    UIMouseTacticalCommandGetCrate = [uint32]2067763016
    UIMouseTacticalDeactivateMissionHotSpotsButton = [uint32]2047841880
    UIMouseTacticalCommandForceAttack = [uint32]2074164365
    UIMouseTacticalPlanningModeButton = [uint32]2278083954
    UIMouseTacticalCommandMisc = [uint32]2505151835
    UIMouseTacticalSelectionRefinementTab = [uint32]2635272840
    UIMouseTacticalCommandForceMove = [uint32]2697587777
    UIMouseTacticalOpenAdvancedCommandsButton = [uint32]2808773267
    UIMouseTacticalSetStanceButton = [uint32]2873949396
    UIMouseTacticalCoOpAIButtonsHoldPositionButton = [uint32]2914137727
    UIMouseTacticalCommandMineOre = [uint32]2957071942
    UIMouseTacticalWaypointModeButton = [uint32]2958905921
    UIMouseTacticalReverseMoveButton = [uint32]2987234945
    UIMouseTacticalCommandEnter = [uint32]3057018816
    UIMouseTacticalCoOpAIButtonsAssaultButton = [uint32]3094553884
    UIMouseTacticalPauseButton = [uint32]3420020972
    UIMouseTacticalCommandAttack = [uint32]3548787763
    UIMouseTacticalCommandAttackMove = [uint32]3575834349
    UIMouseTacticalForceMoveButton = [uint32]3716369729
    UIMouseTacticalForceAttackButton = [uint32]3756168709
    UIMouseTacticalCommandPlaceAIBeacon = [uint32]3809842710
    UIMouseTacticalCommandDock = [uint32]3976885676
    UIMouseTacticalCommandReverseMove = [uint32]4026878150
    UIMouseTacticalBuildQueueTypeTab = [uint32]4094578721
    UIMouseTacticalSellModeButton = [uint32]4102242385
    UIMouseTacticalStopButton = [uint32]4140422551
    UIMouseTacticalCommandSetRallyPoint = [uint32]4180354873
    UIMouseTacticalFastForwardButton = [uint32]4232364483
    UIMouseTacticalGroupSelection = [uint32]241329492
    UIMouseTacticalLookAt = [uint32]1795213437
    UIMouseTacticalRadarTelestrator = [uint32]3366599054
    UIMouseTacticalTelestrator = [uint32]1921726501
    UIMouseTacticalThreatMeter = [uint32]4143989686
    BaseConditionData = [uint32]3899256902
    ObjectConditionData = [uint32]519519951
    UpgradeConditionData = [uint32]76901015
    GameplayConditionData = [uint32]2309012900
    TargetingBaseCompare = [uint32]3600209407
    TargetingDistanceCompare = [uint32]2359666647
    TargetingCombatChainCompare = [uint32]89767986
    TargetingInTurretArcCompare = [uint32]2007776008
    TargetingCompareList = [uint32]3188107749
    SiegeEvaEvent = [uint32]1268003905
    EvaSettings = [uint32]2361666546
    EvaEvent = [uint32]357806494
    MiscEvaEvents = [uint32]1807540972
    AchievementEvent = [uint32]4230032093
    MapNameToAchievementEventMapping = [uint32]3594283852
    GameplayAchievementTriggerData = [uint32]348670903
    HealthBarTemplate = [uint32]1507490649
    HealthBarSettings = [uint32]3919360587
    UnitOverlayIconSettings = [uint32]2421413379
    ObjectFilterAsset = [uint32]1151702845
    RegisteredObjectSet = [uint32]814796096
    ObjectVisionRangeItem = [uint32]772930644
    InvisibilityTemplate = [uint32]1431290504
    InvisibilityManagerSettings = [uint32]254471188
    StanceTemplate = [uint32]2525284163
    EmotionTemplate = [uint32]2836737428
    ProjectilePath = [uint32]1710207315
    ForceFeedbackEvent = [uint32]474482543
    EngagingContent = [uint32]3058114834
    EngagingContentGroup = [uint32]11280974
    EngagingContentSequence = [uint32]3799634821
    IntelDB = [uint32]530081230
    UnitIntro = [uint32]1870180772
    BreezeInfo = [uint32]1626415288
    ModelSettings = [uint32]1968811301
    AutoParticleSystemSettings = [uint32]3278684478
    CrowdResponse = [uint32]400896388
    RadarTerrainImage = [uint32]1099769729
    UIRadarObjectsLayer = [uint32]4039592522
    W3DDeviceSettings = [uint32]1469055344
    ScorchMarkSettings = [uint32]2383722691
    Language = [uint32]2915726794
    HeaderTemplate = [uint32]1218703046
    FontDirectory = [uint32]3569007303
    TheVersion = [uint32]4236434438
    Credits = [uint32]3978455785
    CommandSet = [uint32]2584759047
    Road = [uint32]3516764358
    ShaderOverride = [uint32]3166846828
    ShaderOverrideRule = [uint32]3648907125
    ImageSequence = [uint32]565855655
    UnitTypeIcon = [uint32]3614134471
    DrawGroupInfo = [uint32]2913856066
    OnlineChatColors = [uint32]1345252658
    ArchivableMovie = [uint32]3741496377
    ArchiveMovieList = [uint32]2099472558
    NetworkSettings = [uint32]4272453293
    PathfindSettings = [uint32]970663416
    ResourceSettings = [uint32]3279338544
    GraphicsHardware = [uint32]2846460739
}
$sourceCommit = $env:GITHUB_SHA
if (-not $sourceCommit) { $sourceCommit = (& git rev-parse HEAD).Trim(); if ($LASTEXITCODE -ne 0) { throw 'Cannot resolve sourceCommit' } }
$runId = $env:GITHUB_RUN_ID
if (-not $runId) { $runId = 'local-' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssZ') }
$validation = [ordered]@{
    succeeded = $false; sourceCommit = $sourceCommit; runId = $runId
    scope = 'Combined XmlCompiler priority link/default-root and completion smoke only; group full matrices and SDK acceptance remain separate'
    expectedRootTypeIds = $expectedRootTypeIds; expectedTypes = 243
    expectedFixturesPerMode = 972; expectedComparisonsPerMode = 2916
    expectedCompletionFixturesPerMode = 128; expectedCompletionComparisonsPerMode = 384
    platforms = @(0,1,2,99); modes = @(); processExits = @(); status = 'started'
}
$validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
$originalPath = (Resolve-Path 'SDK/tools/BinaryAssetBuilder.XmlCompiler.dll').Path
$originalHash = (Get-FileHash $originalPath -Algorithm SHA256).Hash.ToLowerInvariant()
$validation['originalDllSha256'] = $originalHash
try {
    if ($env:VSCMD_ARG_TGT_ARCH -and $env:VSCMD_ARG_TGT_ARCH -ne 'x86') { throw 'Run in an x86 VS2022 devshell' }
    if ($expectedRootTypeIds.Count -ne 243) { throw 'Expected root map must contain all 243 recovered entries' }
    if ($originalHash -ne 'd4b78e2da951dd458357bee7b358e6bc9689d1987d55867254d68b3f87bbf0e6') { throw 'Unexpected original XmlCompiler DLL fingerprint' }
    $native = Join-Path $repo 'recovery/src/Native'
    $bridge = Join-Path $repo 'recovery/src/Bridge'
    $commonNames = @('Relocation','ScalarParsing','DataInitializers','ResourceMarshaling','EnumBitset','ObjectFilter')
    $common = @($commonNames | ForEach-Object { (Resolve-Path (Join-Path $native "$_.cpp")).Path })
    $xmlNative = @(Get-ChildItem -LiteralPath $native -Filter 'Xml*.cpp' -File)
    $xmlBridge = @(Get-ChildItem -LiteralPath $bridge -Filter 'Xml*Compiler.cpp' -File)
    $candidates = @($xmlNative) + @($xmlBridge)
    $excluded = @($candidates | Where-Object { $_.Name -match 'Draft' })
    $formal = @($candidates | Where-Object { $_.Name -notmatch 'Draft' } | Sort-Object FullName)
    if ($formal.Count -eq 0) { throw 'No formal Xml sources found' }
    # A #if 0 snapshot does not satisfy the 243-class reflection preflight below.
    $sources = @($common) + @($formal | ForEach-Object { $_.FullName })
    $sources = @($sources | Select-Object -Unique)
    $validation['sources'] = @($sources | ForEach-Object {
        [ordered]@{ path = $_.Substring($repo.Length + 1).Replace('\','/'); sha256 = (Get-FileHash $_ -Algorithm SHA256).Hash.ToLowerInvariant() }
    })
    $validation['excludedDraftSources'] = @($excluded | ForEach-Object { $_.FullName.Substring($repo.Length + 1).Replace('\','/') })
    $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
    $manifestPath = Join-Path $out 'expected-root-types.tsv'
    @($expectedRootTypeIds.GetEnumerator() | ForEach-Object { "$($_.Key)`t$($_.Value)" }) | Set-Content -Encoding ASCII $manifestPath
    Copy-Item $manifestPath "$evidence/expected-root-types.tsv" -Force
    $csc = "$env:WINDIR/Microsoft.NET/Framework/v4.0.30319/csc.exe"
    $oracleSource = (Resolve-Path 'recovery/tests/XmlPriorityLinkOracle.cs').Path
    foreach ($mode in @('Od','O2')) {
        $modeOut = Join-Path $out $mode
        if (Test-Path $modeOut) { Remove-Item $modeOut -Recurse -Force }
        New-Item -ItemType Directory -Force $modeOut | Out-Null
        Copy-Item SDK/tools/*.dll $modeOut -Force
        & cl /nologo /clr /LD /MD /EHa /std:c++17 "/$mode" /fp:precise "/AI$modeOut" "/Fo$modeOut/" "/Fe$modeOut/Recovered.XmlCompilerPriority.dll" @sources *> "$evidence/$mode-build.txt"
        if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/$mode-build.txt"; throw "$mode combined XmlCompiler C++/CLI link failed" }
        & $csc /nologo /platform:x86 /r:System.Core.dll "/r:$modeOut/BinaryAssetBuilder.Core.dll" "/out:$modeOut/XmlPriorityLinkOracle.exe" $oracleSource *> "$evidence/$mode-host-build.txt"
        if ($LASTEXITCODE -ne 0) { Get-Content "$evidence/$mode-host-build.txt"; throw "$mode priority link oracle build failed" }
        '<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" /></startup></configuration>' | Set-Content "$modeOut/XmlPriorityLinkOracle.exe.config"
        $dllPath = Join-Path $modeOut 'Recovered.XmlCompilerPriority.dll'
        # Preserve each linked DLL even if the later oracle rejects an absent class.
        $artifactDirectory = Join-Path $evidence $mode
        New-Item -ItemType Directory -Force $artifactDirectory | Out-Null
        Copy-Item $dllPath (Join-Path $artifactDirectory 'Recovered.XmlCompilerPriority.dll') -Force
        & "$modeOut/XmlPriorityLinkOracle.exe" $originalPath $dllPath "$evidence/$mode-result.json" $manifestPath $sourceCommit $runId *> "$evidence/$mode-oracle.txt"
        $code = $LASTEXITCODE
        # Preserve the real child-process status before any later command or parsing.
        $exitCodeHex = '0x' + [BitConverter]::ToUInt32([BitConverter]::GetBytes([int32]$code), 0).ToString('X8')
        $exitRecord = [ordered]@{
            mode = $mode; exitCode = [int32]$code; exitCodeHex = $exitCodeHex
            sourceCommit = $sourceCommit; runId = $runId
            processSucceeded = ($code -eq 0)
        }
        $validation.processExits += $exitRecord
        $exitRecord | ConvertTo-Json -Depth 4 | Set-Content "$evidence/$mode-exit.json"
        "PROCESS EXIT mode=$mode exitCode=$code exitCodeHex=$exitCodeHex" | Add-Content "$evidence/$mode-oracle.txt"
        $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
        Get-Content "$evidence/$mode-oracle.txt"
        if ($code -ne 0) { throw "$mode combined priority default-root smoke failed: exitCode=$code ($exitCodeHex); all 243 types and a clean process exit are required" }
        $result = Get-Content "$evidence/$mode-result.json" -Raw | ConvertFrom-Json
        if (-not $result.succeeded -or $result.sourceCommit -ne $sourceCommit -or $result.runId -ne $runId -or $result.types -ne 243 -or $result.fixtures -ne 972 -or $result.comparisons -ne 2916 -or $result.completionFixtures -ne 128 -or $result.completionComparisons -ne 384 -or ($result.platforms -join ',') -ne '0,1,2,99') { throw "$mode incomplete or mismatched priority smoke result" }
        if (@($result.typeCoverage.PSObject.Properties).Count -ne 243) { throw "$mode incomplete per-type coverage" }
        foreach ($entry in $expectedRootTypeIds.GetEnumerator()) {
            if ($result.typeCoverage.($entry.Key) -ne 4 -or $result.rootTypeIds.($entry.Key) -ne $entry.Value) { throw "$mode missing type coverage or incorrect TypeId: $($entry.Key)" }
        }
        $validation.modes += [ordered]@{
            mode = $mode; succeeded = $true; exitCode = [int32]$code; exitCodeHex = $exitCodeHex; types = $result.types
            fixtures = $result.fixtures; comparisons = $result.comparisons
            completionFixtures = $result.completionFixtures; completionComparisons = $result.completionComparisons
            platforms = $result.platforms; typeCoverage = $result.typeCoverage
            compilerClasses = $result.compilerClasses
            linkedAssembly = "$mode/Recovered.XmlCompilerPriority.dll"
            linkedAssemblySha256 = (Get-FileHash $dllPath -Algorithm SHA256).Hash.ToLowerInvariant()
            log = "$mode-oracle.txt"
        }
        $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
    }
    $validation.succeeded = $true
    $validation.status = 'All 243 exact compiler routes linked; default-root and 128 completion fixtures passed three-array smoke on Od/O2, platforms 0/1/2/99; not full group or SDK acceptance'
} catch {
    $validation.status = $_.Exception.Message
    throw
} finally {
    $afterHash = (Get-FileHash $originalPath -Algorithm SHA256).Hash.ToLowerInvariant()
    $validation['originalDllUnchanged'] = ($afterHash -eq $originalHash)
    if ($afterHash -ne $originalHash) { $validation.succeeded = $false; $validation.status = 'Original XmlCompiler DLL changed during smoke' }
    $validation | ConvertTo-Json -Depth 12 | Set-Content "$evidence/validation.json"
    if ($afterHash -ne $originalHash) { throw 'Original XmlCompiler DLL changed during smoke' }
}



