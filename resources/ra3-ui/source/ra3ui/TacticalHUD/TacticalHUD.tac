movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TacticalHUD\\TacticalHUD.eaf' &compressed // flash 7, total frames: 21, frame rate: 30 fps, 1365x768 px

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2
  
  &exportAssets
    2 &as 'SubtitleRenderingSurfaceContentPlaceholder'
  &end // of exportAssets

  &frame 0
    &constants 'letterBoxLayer', '_visible', 'hideMainCount', 'main', 'upperSidebarLayer', 'selectionFeedbackSurface', 'floatingTextSurface', 'letterBoxVisible', 'showRadarDuringLetterBox', '_fadeIn', 'gotoAndPlay', '_fadeOut', '_fade', 'shroud', 'inputShield', 'Pointer', 'encode', 'helpBox', 'upperSidebar', 'radar', 'mainCommandBar', 'lowerSidebar', 'selectionDetails', 'resourceDisplay', 'commandModeBar', 'energyMeter', 'playerPowersBar', 'specialPowerTimerDisplay', 'dialogStage', 'cutSceneStage', 'objectiveIndicatorStage', 'messageSurface', 'timerSurface', 'popUpMetaStage', 'observerControl', 'telestratorControl', 'replayControl', 'voiceChatControl', 'ticker', 'telestratorSurface', 'subtitlePanelSurface', 'coOpAIOrderPanel', 'beaconStage', 'fadeSurface', 'drawableOverlayStage', 'expPanel', 'displayLeft', 'displayTop', 'displayWidth', 'displayHeight', 'leftAnchoredClips', 'length', '_x', '_parent', 'GlobalToLocalX', 'leftMargins', 'rightAnchoredClips', 'rightMargins', 'topAnchoredClips', '_y', 'GlobalToLocalY', 'topMargins', 'bottomAnchoredClips', 'bottomMargins', 'centerAnchoredClips', 'centerOffsets', 'x', 'y', 'fullScreenClips', '_width', '_height', 'showRadarDuringLetterBox=', 'showRadar=', 'fading in radar', 'fading out radar', 'currentFaction', 'factionObserverMap', 'OnCurrentFactionChanged', '_root', 'ancestor', '_xscale', '_yscale', 'TacticalHUD.swf onUnloading()', 'gadgetFlashStage', 'onUnloading', 'TacticalHUD.swf onUnload()', 'Hide', 'Show', 'FadeIn', 'FadeOut', 'HideShroud', 'GetInputShield', 'GetHelpBox', 'GetSelectionFeedbackSurface', 'GetRadar', 'GetMainCommandBar', 'GetSelectionDetailsPanel', 'GetResourceDisplay', 'GetCommandModeBar', 'GetEnergyMeter', 'GetPlayerPowerBar', 'GetPlayerPowerTimerDisplay', 'GetDialogStage', 'GetCutSceneStage', 'GetObjectiveIndicatorStage', 'GetFloatingTextSurface', 'GetMessageSurface', 'GetTimerSurface', 'GetPopUpMetaStage', 'GetObserverControlPanel', 'GetTelestratorControlPanel', 'GetReplayControlPanel', 'GetVoiceChatControlPanel', 'GetTickerPanel', 'GetTelestratorSurface', 'GetSubtitlePanelSurface', 'GetCoOpAIOrderPanel', 'GetBeaconStage', 'GetDrawableOverlayStage', 'GetPlayerExperiencePanel', 'SetDisplayRegion', 'SetLetterBoxVisibility', 'SetCurrentFaction', 'onUnload', 'initialized', 'this', 'OnContentLoaded', 'TacticalHUDBeaconStage.swf', 'loadMovie', 'ig_DrawableOverlayStage.swf', 'igm_gadgetFlashStage.swf', 'watermark', 'Array', 'i', 'clip', 'Object', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'extern', 'InGame', 'Soviet'  
    &function Hide (    )    
      &pushsdbgv 0							//'letterBoxLayer'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'hideMainCount'
      &pushsdb 2							//'hideMainCount'
      &pushsdbgv 2							//'hideMainCount'
      &increment
      &setVariable
      &pushzero
      &equals
      &not
      &jnz label1      
      &pushsdbgv 3							//'main'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
     label1:
      &pushsdbgv 5							//'selectionFeedbackSurface'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 6							//'floatingTextSurface'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
    &end // of function Hide

    &function Show (    )    
      &pushsdbgv 0							//'letterBoxLayer'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 2							//'hideMainCount'
      &pushsdbgv 2							//'hideMainCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label2      
      &pushsdbgv 3							//'main'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
     label2:
      &pushsdbgv 5							//'selectionFeedbackSurface'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 6							//'floatingTextSurface'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
    &end // of function Show

    &function FadeIn (    )    
      &gotoLabel '_fadeIn'
      &play
      &pushsdbgv 7							//'letterBoxVisible'
      &pushfalse
      &equals
      &dup
      &jnz label3      
      &pop
      &pushsdbgv 8							//'showRadarDuringLetterBox'
      &pushfalse
      &equals
     label3:
      &not
      &jnz label4      
      &pushsdb 9							//'_fadeIn'
      &pushone
      &pushsdbgv 4							//'upperSidebarLayer'
      &dcallmp 10							// gotoAndPlay()
     label4:
      &pushsdb 2							//'hideMainCount'
      &pushsdbgv 2							//'hideMainCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label5      
      &pushsdbgv 3							//'main'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
     label5:
    &end // of function FadeIn

    &function FadeOut (    )    
      &gotoLabel '_fadeOut'
      &play
      &pushsdbgv 7							//'letterBoxVisible'
      &pushfalse
      &equals
      &dup
      &jnz label6      
      &pop
      &pushsdbgv 8							//'showRadarDuringLetterBox'
      &pushfalse
      &equals
     label6:
      &not
      &jnz label7      
      &pushsdb 11							//'_fadeOut'
      &pushone
      &pushsdbgv 4							//'upperSidebarLayer'
      &dcallmp 10							// gotoAndPlay()
     label7:
    &end // of function FadeOut

    &function HideShroud (    )    
      &pushsdb 12							//'_fade'
      &pushone
      &pushsdbgv 13							//'shroud'
      &dcallmp 10							// gotoAndPlay()
    &end // of function HideShroud

    &function GetInputShield (    )    
      &pushsdbgv 14							//'inputShield'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetInputShield

    &function GetHelpBox (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 17							//'helpBox'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetHelpBox

    &function GetSelectionFeedbackSurface (    )    
      &pushsdbgv 5							//'selectionFeedbackSurface'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetSelectionFeedbackSurface

    &function GetRadar (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 18							//'upperSidebar'
      &pushsdbgm 19							//'radar'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetRadar

    &function GetMainCommandBar (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 18							//'upperSidebar'
      &pushsdbgm 20							//'mainCommandBar'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetMainCommandBar

    &function GetSelectionDetailsPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 21							//'lowerSidebar'
      &pushsdbgm 22							//'selectionDetails'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetSelectionDetailsPanel

    &function GetResourceDisplay (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 18							//'upperSidebar'
      &pushsdbgm 23							//'resourceDisplay'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetResourceDisplay

    &function GetCommandModeBar (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 18							//'upperSidebar'
      &pushsdbgm 24							//'commandModeBar'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetCommandModeBar

    &function GetEnergyMeter (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 18							//'upperSidebar'
      &pushsdbgm 25							//'energyMeter'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetEnergyMeter

    &function GetPlayerPowersBar (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 26							//'playerPowersBar'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerPowersBar

    &function GetSpecialPowerTimerDisplay (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 27							//'specialPowerTimerDisplay'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetSpecialPowerTimerDisplay

    &function GetDialogStage (    )    
      &pushsdbgv 28							//'dialogStage'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetDialogStage

    &function GetCutSceneStage (    )    
      &pushsdbgv 29							//'cutSceneStage'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetCutSceneStage

    &function GetObjectiveIndicatorStage (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 30							//'objectiveIndicatorStage'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetObjectiveIndicatorStage

    &function GetFloatingTextSurface (    )    
      &pushsdbgv 6							//'floatingTextSurface'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetFloatingTextSurface

    &function GetMessageSurface (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 31							//'messageSurface'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetMessageSurface

    &function GetTimerSurface (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 32							//'timerSurface'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetTimerSurface

    &function GetPopUpMetaStage (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 33							//'popUpMetaStage'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetPopUpMetaStage

    &function GetObserverControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 34							//'observerControl'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetObserverControlPanel

    &function GetTelestratorControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 35							//'telestratorControl'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetTelestratorControlPanel

    &function GetReplayControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 36							//'replayControl'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetReplayControlPanel

    &function GetVoiceChatControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 37							//'voiceChatControl'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetVoiceChatControlPanel

    &function GetTickerPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 38							//'ticker'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetTickerPanel

    &function GetTelestratorSurface (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 39							//'telestratorSurface'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetTelestratorSurface

    &function GetSubtitlePanelSurface (    )    
      &pushsdbgv 40							//'subtitlePanelSurface'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetSubtitlePanelSurface

    &function GetCoOpAIOrderPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 41							//'coOpAIOrderPanel'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetCoOpAIOrderPanel

    &function GetBeaconStage (    )    
      &pushsdbgv 42							//'beaconStage'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetBeaconStage

    &function GetFadeSurface (    )    
      &pushsdbgv 43							//'fadeSurface'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetFadeSurface

    &function GetDrawableOverlayStage (    )    
      &pushsdbgv 44							//'drawableOverlayStage'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetDrawableOverlayStage

    &function GetPlayerExperiencePanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 45							//'expPanel'
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerExperiencePanel

    &function2 SetDisplayRegion (r:15='topArg', r:13='leftArg', r:16='bottomArg', r:14='rightArg') (r:1='_root')
      &pushregister 15
      &toNumber
      &setRegister r:4
      &pop
      &pushregister 13
      &toNumber
      &setRegister r:5
      &pop
      &pushregister 16
      &toNumber
      &setRegister r:6
      &pop
      &pushregister 14
      &toNumber
      &setRegister r:7
      &pop
      &pushregister 7
      &pushregister 5
      &subtract
      &setRegister r:11
      &pop
      &pushregister 6
      &pushregister 4
      &subtract
      &setRegister r:12
      &pop
      &pushregister 5
      &pushregister 7
      &add
      &pushfloat 0.500000000000000
      &multiply
      &setRegister r:9
      &pop
      &pushregister 4
      &pushregister 6
      &add
      &pushfloat 0.500000000000000
      &multiply
      &setRegister r:8
      &pop
      &pushregister 1
      &pushsdb 46							//'displayLeft'
      &pushregister 5
      &setMember
      &pushregister 1
      &pushsdb 47							//'displayTop'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 48							//'displayWidth'
      &pushregister 11
      &setMember
      &pushregister 1
      &pushsdb 49							//'displayHeight'
      &pushregister 12
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label8:
      &pushregister 3
      &pushsdbgv 50							//'leftAnchoredClips'
      &pushsdbgm 51							//'length'
      &lessThan
      &not
      &jnz label9      
      &pushsdbgv 50							//'leftAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 52							//'_x'
      &pushregister 5
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 54							//'GlobalToLocalX'
      &callFunction
      &pushsdbgv 55							//'leftMargins'
      &pushregister 3
      &getMember
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label8      
     label9:
      &pushzero
      &setRegister r:3
      &pop
     label10:
      &pushregister 3
      &pushsdbgv 56							//'rightAnchoredClips'
      &pushsdbgm 51							//'length'
      &lessThan
      &not
      &jnz label11      
      &pushsdbgv 56							//'rightAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 52							//'_x'
      &pushregister 7
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 54							//'GlobalToLocalX'
      &callFunction
      &pushsdbgv 57							//'rightMargins'
      &pushregister 3
      &getMember
      &subtract
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label10      
     label11:
      &pushzero
      &setRegister r:3
      &pop
     label12:
      &pushregister 3
      &pushsdbgv 58							//'topAnchoredClips'
      &pushsdbgm 51							//'length'
      &lessThan
      &not
      &jnz label13      
      &pushsdbgv 58							//'topAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 59							//'_y'
      &pushregister 4
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 60							//'GlobalToLocalY'
      &callFunction
      &pushsdbgv 61							//'topMargins'
      &pushregister 3
      &getMember
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label12      
     label13:
      &pushzero
      &setRegister r:3
      &pop
     label14:
      &pushregister 3
      &pushsdbgv 62							//'bottomAnchoredClips'
      &pushsdbgm 51							//'length'
      &lessThan
      &not
      &jnz label15      
      &pushsdbgv 62							//'bottomAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 59							//'_y'
      &pushregister 6
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 60							//'GlobalToLocalY'
      &callFunction
      &pushsdbgv 63							//'bottomMargins'
      &pushregister 3
      &getMember
      &subtract
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label14      
     label15:
      &pushzero
      &setRegister r:3
      &pop
     label16:
      &pushregister 3
      &pushsdbgv 64							//'centerAnchoredClips'
      &pushsdbgm 51							//'length'
      &lessThan
      &not
      &jnz label17      
      &pushsdbgv 64							//'centerAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 52							//'_x'
      &pushregister 9
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 54							//'GlobalToLocalX'
      &callFunction
      &pushsdbgv 65							//'centerOffsets'
      &pushregister 3
      &getMember
      &pushsdbgm 66							//'x'
      &add
      &setMember
      &pushregister 2
      &pushsdb 59							//'_y'
      &pushregister 8
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 60							//'GlobalToLocalY'
      &callFunction
      &pushsdbgv 65							//'centerOffsets'
      &pushregister 3
      &getMember
      &pushsdbgm 67							//'y'
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label16      
     label17:
      &pushzero
      &setRegister r:3
      &pop
     label18:
      &pushregister 3
      &pushsdbgv 68							//'fullScreenClips'
      &pushsdbgm 51							//'length'
      &lessThan
      &not
      &jnz label19      
      &pushsdbgv 68							//'fullScreenClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 52							//'_x'
      &pushregister 5
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 54							//'GlobalToLocalX'
      &callFunction
      &setMember
      &pushregister 2
      &pushsdb 69							//'_width'
      &pushregister 7
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 54							//'GlobalToLocalX'
      &callFunction
      &pushregister 2
      &pushsdbgm 52							//'_x'
      &subtract
      &setMember
      &pushregister 2
      &pushsdb 59							//'_y'
      &pushregister 4
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 60							//'GlobalToLocalY'
      &callFunction
      &setMember
      &pushregister 2
      &pushsdb 70							//'_height'
      &pushregister 6
      &pushregister 2
      &pushsdbgm 53							//'_parent'
      &pushbyte 2
      &pushsdb 60							//'GlobalToLocalY'
      &callFunction
      &pushregister 2
      &pushsdbgm 59							//'_y'
      &subtract
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label18      
     label19:
      &pushbyte 5
      &setRegister r:10
      &pop
      &pushsdbgv 14							//'inputShield'
      &pushsdb 52							//'_x'
      &pushregister 5
      &pushregister 10
      &subtract
      &setMember
      &pushsdbgv 14							//'inputShield'
      &pushsdb 69							//'_width'
      &pushregister 11
      &pushregister 10
      &pushbyte 2
      &multiply
      &add
      &setMember
      &pushsdbgv 14							//'inputShield'
      &pushsdb 59							//'_y'
      &pushregister 4
      &pushregister 10
      &subtract
      &setMember
      &pushsdbgv 14							//'inputShield'
      &pushsdb 70							//'_height'
      &pushregister 12
      &pushregister 10
      &pushbyte 2
      &multiply
      &add
      &setMember
    &end // of function SetDisplayRegion

    &function2 SetLetterBoxVisibility (r:3='visArg', r:4='showRadarArg') ()
      &pushregister 3
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:2
      &pop
      &pushregister 4
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushregister 2
      &not
      &jnz label25      
      &pushsdbgv 7							//'letterBoxVisible'
      &not
      &not
      &jnz label20      
      &pushsdb 7							//'letterBoxVisible'
      &pushtrue
      &setVariable
      &pushsdb 9							//'_fadeIn'
      &pushone
      &pushsdbgv 0							//'letterBoxLayer'
      &dcallmp 10							// gotoAndPlay()
     label20:
      &pushsdb 71							//'showRadarDuringLetterBox='
      &pushsdbgv 8							//'showRadarDuringLetterBox'
      &toString
      &add
      &trace
      &pushsdb 72							//'showRadar='
      &pushregister 1
      &toString
      &add
      &trace
      &pushsdbgv 8							//'showRadarDuringLetterBox'
      &not
      &dup
      &not
      &jnz label21      
      &pop
      &pushregister 1
     label21:
      &not
      &jnz label22      
      &pushsdb 73							//'fading in radar'
      &trace
      &pushsdb 9							//'_fadeIn'
      &pushone
      &pushsdbgv 4							//'upperSidebarLayer'
      &dcallmp 10							// gotoAndPlay()
      &jmp label24      
     label22:
      &pushsdbgv 8							//'showRadarDuringLetterBox'
      &dup
      &not
      &jnz label23      
      &pop
      &pushregister 1
      &not
     label23:
      &not
      &jnz label24      
      &pushsdb 74							//'fading out radar'
      &trace
      &pushsdb 11							//'_fadeOut'
      &pushone
      &pushsdbgv 4							//'upperSidebarLayer'
      &dcallmp 10							// gotoAndPlay()
     label24:
      &pushsdb 8							//'showRadarDuringLetterBox'
      &pushregister 1
      &setVariable
      &jmp label26      
     label25:
      &pushsdbgv 7							//'letterBoxVisible'
      &not
      &jnz label26      
      &pushsdb 7							//'letterBoxVisible'
      &pushfalse
      &setVariable
      &pushsdb 11							//'_fadeOut'
      &pushone
      &pushsdbgv 0							//'letterBoxLayer'
      &dcallmp 10							// gotoAndPlay()
     label26:
    &end // of function SetLetterBoxVisibility

    &function2 SetCurrentFaction (r:3='faction') (r:1='_root')
      &pushregister 3
      &pushregister 1
      &pushsdbgm 75							//'currentFaction'
      &equals
      &not
      &not
      &jnz label28      
      &pushregister 1
      &pushsdb 75							//'currentFaction'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdbgm 76							//'factionObserverMap'
      &enumerateValue
     label27:
      &setRegister r:0
      &pushnull
      &equals
      &jnz label28      
      &pushregister 0
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 76							//'factionObserverMap'
      &pushregister 4
      &getMember
      &setRegister r:2
      &pop
      &pushregister 3
      &pushone
      &pushregister 2
      &dcallmp 77							// OnCurrentFactionChanged()
      &jmp label27      
     label28:
    &end // of function SetCurrentFaction

    &function2 GlobalToLocalX (r:5='clip', r:6='x') ()
      &pushregister 5
      &pushsdbgm 78							//'_root'
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushone
      &setRegister r:3
      &pop
      &pushsdb 79							//'ancestor'
      &pushregister 5
      &setVariable
     label29:
      &pushsdbgv 79							//'ancestor'
      &pushregister 4
      &equals
      &not
      &not
      &jnz label30      
      &pushsdbgv 79							//'ancestor'
      &pushsdbgm 80							//'_xscale'
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:1
      &pop
      &pushregister 2
      &pushregister 1
      &multiply
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgv 79							//'ancestor'
      &pushsdbgm 52							//'_x'
      &subtract
      &setRegister r:2
      &pop
      &pushregister 3
      &pushregister 1
      &divide
      &setRegister r:3
      &pop
      &pushsdb 79							//'ancestor'
      &pushsdbgv 79							//'ancestor'
      &pushsdbgm 53							//'_parent'
      &setVariable
      &jmp label29      
     label30:
      &pushregister 6
      &pushregister 2
      &add
      &pushregister 3
      &multiply
      &return
    &end // of function GlobalToLocalX

    &function2 GlobalToLocalY (r:5='clip', r:6='y') ()
      &pushregister 5
      &pushsdbgm 78							//'_root'
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushone
      &setRegister r:3
      &pop
      &pushsdb 79							//'ancestor'
      &pushregister 5
      &setVariable
     label31:
      &pushsdbgv 79							//'ancestor'
      &pushregister 4
      &equals
      &not
      &not
      &jnz label32      
      &pushsdbgv 79							//'ancestor'
      &pushsdbgm 81							//'_yscale'
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:1
      &pop
      &pushregister 2
      &pushregister 1
      &multiply
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgv 79							//'ancestor'
      &pushsdbgm 59							//'_y'
      &subtract
      &setRegister r:2
      &pop
      &pushregister 3
      &pushregister 1
      &divide
      &setRegister r:3
      &pop
      &pushsdb 79							//'ancestor'
      &pushsdbgv 79							//'ancestor'
      &pushsdbgm 53							//'_parent'
      &setVariable
      &jmp label31      
     label32:
      &pushregister 6
      &pushregister 2
      &add
      &pushregister 3
      &multiply
      &return
    &end // of function GlobalToLocalY

    &function onUnloading (    )    
      &pushsdb 82							//'TacticalHUD.swf onUnloading()'
      &trace
      &pushzero
      &pushsdbgv 83							//'gadgetFlashStage'
      &dcallmp 84							// onUnloading()
    &end // of function onUnloading

    &function onUnload (    )    
      &pushsdb 85							//'TacticalHUD.swf onUnload()'
      &trace
      &pushsdb 86							//'Hide'
      &delete2
      &pop
      &pushsdb 87							//'Show'
      &delete2
      &pop
      &pushsdb 88							//'FadeIn'
      &delete2
      &pop
      &pushsdb 89							//'FadeOut'
      &delete2
      &pop
      &pushsdb 90							//'HideShroud'
      &delete2
      &pop
      &pushsdb 91							//'GetInputShield'
      &delete2
      &pop
      &pushsdb 92							//'GetHelpBox'
      &delete2
      &pop
      &pushsdb 93							//'GetSelectionFeedbackSurface'
      &delete2
      &pop
      &pushsdb 94							//'GetRadar'
      &delete2
      &pop
      &pushsdb 95							//'GetMainCommandBar'
      &delete2
      &pop
      &pushsdb 96							//'GetSelectionDetailsPanel'
      &delete2
      &pop
      &pushsdb 97							//'GetResourceDisplay'
      &delete2
      &pop
      &pushsdb 98							//'GetCommandModeBar'
      &delete2
      &pop
      &pushsdb 99							//'GetEnergyMeter'
      &delete2
      &pop
      &pushsdb 100							//'GetPlayerPowerBar'
      &delete2
      &pop
      &pushsdb 101							//'GetPlayerPowerTimerDisplay'
      &delete2
      &pop
      &pushsdb 102							//'GetDialogStage'
      &delete2
      &pop
      &pushsdb 103							//'GetCutSceneStage'
      &delete2
      &pop
      &pushsdb 104							//'GetObjectiveIndicatorStage'
      &delete2
      &pop
      &pushsdb 105							//'GetFloatingTextSurface'
      &delete2
      &pop
      &pushsdb 106							//'GetMessageSurface'
      &delete2
      &pop
      &pushsdb 107							//'GetTimerSurface'
      &delete2
      &pop
      &pushsdb 108							//'GetPopUpMetaStage'
      &delete2
      &pop
      &pushsdb 109							//'GetObserverControlPanel'
      &delete2
      &pop
      &pushsdb 110							//'GetTelestratorControlPanel'
      &delete2
      &pop
      &pushsdb 111							//'GetReplayControlPanel'
      &delete2
      &pop
      &pushsdb 112							//'GetVoiceChatControlPanel'
      &delete2
      &pop
      &pushsdb 113							//'GetTickerPanel'
      &delete2
      &pop
      &pushsdb 114							//'GetTelestratorSurface'
      &delete2
      &pop
      &pushsdb 115							//'GetSubtitlePanelSurface'
      &delete2
      &pop
      &pushsdb 116							//'GetCoOpAIOrderPanel'
      &delete2
      &pop
      &pushsdb 117							//'GetBeaconStage'
      &delete2
      &pop
      &pushsdb 118							//'GetDrawableOverlayStage'
      &delete2
      &pop
      &pushsdb 119							//'GetPlayerExperiencePanel'
      &delete2
      &pop
      &pushsdb 120							//'SetDisplayRegion'
      &delete2
      &pop
      &pushsdb 121							//'SetLetterBoxVisibility'
      &delete2
      &pop
      &pushsdb 122							//'SetCurrentFaction'
      &delete2
      &pop
      &pushsdb 54							//'GlobalToLocalX'
      &delete2
      &pop
      &pushsdb 60							//'GlobalToLocalY'
      &delete2
      &pop
      &pushsdb 84							//'onUnloading'
      &delete2
      &pop
      &pushsdb 123							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 124							//'initialized'
    &not
    &not
    &jnz label43    
    &pushthisgv
    &pushone
    &pushsdbgv 53							//'_parent'
    &dcallmp 126							// OnContentLoaded()
    &pushsdb 127							//'TacticalHUDBeaconStage.swf'
    &pushone
    &pushsdbgv 42							//'beaconStage'
    &dcallmp 128							// loadMovie()
    &pushsdb 129							//'ig_DrawableOverlayStage.swf'
    &pushone
    &pushsdbgv 44							//'drawableOverlayStage'
    &dcallmp 128							// loadMovie()
    &pushsdb 130							//'igm_gadgetFlashStage.swf'
    &pushone
    &pushsdbgv 83							//'gadgetFlashStage'
    &dcallmp 128							// loadMovie()
    &pushsdb 2							//'hideMainCount'
    &pushone
    &setVariable
    &pushsdbgv 3							//'main'
    &pushsdb 1							//'_visible'
    &pushfalse
    &setMember
    &pushsdb 7							//'letterBoxVisible'
    &pushfalse
    &setVariable
    &pushsdb 8							//'showRadarDuringLetterBox'
    &pushfalse
    &setVariable
    &pushsdb 50							//'leftAnchoredClips'
    &pushsdbgv 131							//'watermark'
    &pushsdbgv 3							//'main'
    &pushsdbgm 45							//'expPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 41							//'coOpAIOrderPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 38							//'ticker'
    &pushsdbgv 3							//'main'
    &pushsdbgm 27							//'specialPowerTimerDisplay'
    &pushsdbgv 3							//'main'
    &pushsdbgm 26							//'playerPowersBar'
    &pushsdbgv 3							//'main'
    &pushsdbgm 31							//'messageSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 32							//'timerSurface'
    &pushbyte 8
    &initArray
    &setVariable
    &pushsdb 56							//'rightAnchoredClips'
    &pushsdbgv 3							//'main'
    &pushsdbgm 21							//'lowerSidebar'
    &pushsdbgv 4							//'upperSidebarLayer'
    &pushsdbgv 3							//'main'
    &pushsdbgm 37							//'voiceChatControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 34							//'observerControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 35							//'telestratorControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 36							//'replayControl'
    &pushbyte 6
    &initArray
    &setVariable
    &pushsdb 58							//'topAnchoredClips'
    &pushsdbgv 4							//'upperSidebarLayer'
    &pushsdbgv 3							//'main'
    &pushsdbgm 41							//'coOpAIOrderPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 37							//'voiceChatControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 34							//'observerControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 35							//'telestratorControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 36							//'replayControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 26							//'playerPowersBar'
    &pushsdbgv 3							//'main'
    &pushsdbgm 31							//'messageSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 32							//'timerSurface'
    &pushbyte 9
    &initArray
    &setVariable
    &pushsdb 62							//'bottomAnchoredClips'
    &pushsdbgv 131							//'watermark'
    &pushsdbgv 40							//'subtitlePanelSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 27							//'specialPowerTimerDisplay'
    &pushsdbgv 3							//'main'
    &pushsdbgm 45							//'expPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 21							//'lowerSidebar'
    &pushsdbgv 3							//'main'
    &pushsdbgm 38							//'ticker'
    &pushbyte 6
    &initArray
    &setVariable
    &pushsdb 64							//'centerAnchoredClips'
    &pushsdbgv 3							//'main'
    &pushsdbgm 30							//'objectiveIndicatorStage'
    &pushsdbgv 3							//'main'
    &pushsdbgm 33							//'popUpMetaStage'
    &pushsdbgv 29							//'cutSceneStage'
    &pushsdbgv 28							//'dialogStage'
    &pushbyte 4
    &initArray
    &setVariable
    &pushsdb 68							//'fullScreenClips'
    &pushsdbgv 43							//'fadeSurface'
    &pushsdbgv 6							//'floatingTextSurface'
    &pushsdbgv 5							//'selectionFeedbackSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 39							//'telestratorSurface'
    &pushsdbgv 0							//'letterBoxLayer'
    &pushsdbgv 13							//'shroud'
    &pushbyte 6
    &initArray
    &setVariable
    &pushsdb 55							//'leftMargins'
    &pushsdbgv 50							//'leftAnchoredClips'
    &pushsdbgm 51							//'length'
    &pushone
    &pushsdb 132							//'Array'
    &new
    &setVariable
    &pushsdb 133							//'i'
    &pushzero
    &varEquals
   label33:
    &pushsdbgv 133							//'i'
    &pushsdbgv 50							//'leftAnchoredClips'
    &pushsdbgm 51							//'length'
    &lessThan
    &not
    &jnz label34    
    &pushsdb 134							//'clip'
    &pushsdbgv 50							//'leftAnchoredClips'
    &pushsdbgv 133							//'i'
    &getMember
    &varEquals
    &pushsdbgv 55							//'leftMargins'
    &pushsdbgv 133							//'i'
    &pushsdbgv 134							//'clip'
    &pushsdbgm 52							//'_x'
    &pushzero
    &pushsdbgv 134							//'clip'
    &pushsdbgm 53							//'_parent'
    &pushbyte 2
    &pushsdb 54							//'GlobalToLocalX'
    &callFunction
    &subtract
    &setMember
    &pushsdb 133							//'i'
    &pushsdbgv 133							//'i'
    &increment
    &setVariable
    &jmp label33    
   label34:
    &pushsdb 57							//'rightMargins'
    &pushsdbgv 56							//'rightAnchoredClips'
    &pushsdbgm 51							//'length'
    &pushone
    &pushsdb 132							//'Array'
    &new
    &setVariable
    &pushsdb 133							//'i'
    &pushzero
    &varEquals
   label35:
    &pushsdbgv 133							//'i'
    &pushsdbgv 56							//'rightAnchoredClips'
    &pushsdbgm 51							//'length'
    &lessThan
    &not
    &jnz label36    
    &pushsdb 134							//'clip'
    &pushsdbgv 56							//'rightAnchoredClips'
    &pushsdbgv 133							//'i'
    &getMember
    &varEquals
    &pushsdbgv 57							//'rightMargins'
    &pushsdbgv 133							//'i'
    &pushfloat 1365.300048828125000
    &pushsdbgv 134							//'clip'
    &pushsdbgm 53							//'_parent'
    &pushbyte 2
    &pushsdb 54							//'GlobalToLocalX'
    &callFunction
    &pushsdbgv 134							//'clip'
    &pushsdbgm 52							//'_x'
    &subtract
    &setMember
    &pushsdb 133							//'i'
    &pushsdbgv 133							//'i'
    &increment
    &setVariable
    &jmp label35    
   label36:
    &pushsdb 61							//'topMargins'
    &pushsdbgv 58							//'topAnchoredClips'
    &pushsdbgm 51							//'length'
    &pushone
    &pushsdb 132							//'Array'
    &new
    &setVariable
    &pushsdb 133							//'i'
    &pushzero
    &varEquals
   label37:
    &pushsdbgv 133							//'i'
    &pushsdbgv 58							//'topAnchoredClips'
    &pushsdbgm 51							//'length'
    &lessThan
    &not
    &jnz label38    
    &pushsdb 134							//'clip'
    &pushsdbgv 58							//'topAnchoredClips'
    &pushsdbgv 133							//'i'
    &getMember
    &varEquals
    &pushsdbgv 61							//'topMargins'
    &pushsdbgv 133							//'i'
    &pushsdbgv 134							//'clip'
    &pushsdbgm 59							//'_y'
    &pushzero
    &pushsdbgv 134							//'clip'
    &pushsdbgm 53							//'_parent'
    &pushbyte 2
    &pushsdb 60							//'GlobalToLocalY'
    &callFunction
    &subtract
    &setMember
    &pushsdb 133							//'i'
    &pushsdbgv 133							//'i'
    &increment
    &setVariable
    &jmp label37    
   label38:
    &pushsdb 63							//'bottomMargins'
    &pushsdbgv 62							//'bottomAnchoredClips'
    &pushsdbgm 51							//'length'
    &pushone
    &pushsdb 132							//'Array'
    &new
    &setVariable
    &pushsdb 133							//'i'
    &pushzero
    &varEquals
   label39:
    &pushsdbgv 133							//'i'
    &pushsdbgv 62							//'bottomAnchoredClips'
    &pushsdbgm 51							//'length'
    &lessThan
    &not
    &jnz label40    
    &pushsdb 134							//'clip'
    &pushsdbgv 62							//'bottomAnchoredClips'
    &pushsdbgv 133							//'i'
    &getMember
    &varEquals
    &pushsdbgv 63							//'bottomMargins'
    &pushsdbgv 133							//'i'
    &pushshort 768
    &pushsdbgv 134							//'clip'
    &pushsdbgm 53							//'_parent'
    &pushbyte 2
    &pushsdb 60							//'GlobalToLocalY'
    &callFunction
    &pushsdbgv 134							//'clip'
    &pushsdbgm 59							//'_y'
    &subtract
    &setMember
    &pushsdb 133							//'i'
    &pushsdbgv 133							//'i'
    &increment
    &setVariable
    &jmp label39    
   label40:
    &pushsdb 65							//'centerOffsets'
    &pushsdbgv 64							//'centerAnchoredClips'
    &pushsdbgm 51							//'length'
    &pushone
    &pushsdb 132							//'Array'
    &new
    &setVariable
    &pushsdb 133							//'i'
    &pushzero
    &varEquals
   label41:
    &pushsdbgv 133							//'i'
    &pushsdbgv 64							//'centerAnchoredClips'
    &pushsdbgm 51							//'length'
    &lessThan
    &not
    &jnz label42    
    &pushsdb 134							//'clip'
    &pushsdbgv 64							//'centerAnchoredClips'
    &pushsdbgv 133							//'i'
    &getMember
    &varEquals
    &pushsdbgv 65							//'centerOffsets'
    &pushsdbgv 133							//'i'
    &pushzero
    &pushsdb 135							//'Object'
    &new
    &setMember
    &pushsdbgv 65							//'centerOffsets'
    &pushsdbgv 133							//'i'
    &getMember
    &pushsdb 66							//'x'
    &pushsdbgv 134							//'clip'
    &pushsdbgm 52							//'_x'
    &pushfloat 682.650024414062500
    &pushsdbgv 134							//'clip'
    &pushsdbgm 53							//'_parent'
    &pushbyte 2
    &pushsdb 54							//'GlobalToLocalX'
    &callFunction
    &subtract
    &setMember
    &pushsdbgv 65							//'centerOffsets'
    &pushsdbgv 133							//'i'
    &getMember
    &pushsdb 67							//'y'
    &pushsdbgv 134							//'clip'
    &pushsdbgm 59							//'_y'
    &pushshort 384
    &pushsdbgv 134							//'clip'
    &pushsdbgm 53							//'_parent'
    &pushbyte 2
    &pushsdb 60							//'GlobalToLocalY'
    &callFunction
    &subtract
    &setMember
    &pushsdb 133							//'i'
    &pushsdbgv 133							//'i'
    &increment
    &setVariable
    &jmp label41    
   label42:
    &pushsdbgv 78							//'_root'
    &pushsdb 46							//'displayLeft'
    &pushzero
    &setMember
    &pushsdbgv 78							//'_root'
    &pushsdb 47							//'displayTop'
    &pushzero
    &setMember
    &pushsdbgv 78							//'_root'
    &pushsdb 48							//'displayWidth'
    &pushfloat 1365.300048828125000
    &setMember
    &pushsdbgv 78							//'_root'
    &pushsdb 49							//'displayHeight'
    &pushshort 768
    &setMember
    &pushsdb 124							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 136							//'FSCommand:'
    &pushsdb 137							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 138							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 139							//''
    &getURL2
    &pushsdbgv 140							//'extern'
    &pushsdbgm 141							//'InGame'
    &not
    &not
    &jnz label43    
    &pushsdbgv 13							//'shroud'
    &pushsdb 1							//'_visible'
    &pushfalse
    &setMember
    &pushshort 1024
    &pushshort 768
    &pushzero
    &pushzero
    &pushbyte 4
    &dcallfp 120							// SetDisplayRegion()
    &pushsdb 142							//'Soviet'
    &pushone
    &dcallfp 122							// SetCurrentFaction()
    &pushzero
    &dcallfp 88							// FadeIn()
   label43:
  &end // of frame 0

  &frame 0
    &stop
  &end // of frame 0
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'Dummy' &as 3
  &end // of importAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'SimpleRenderingSurface' &as 4
  &end // of importAssets

  &defineMovieClip 7 // total frames: 1

    &frame 0
      &constants '', 'extern', 'InGame', 'RA3PublicBeta', '1'  
      &pushsdb 0							//''
      &pushbyte 7
      &pushsdbgv 1							//'extern'
      &pushsdbgm 2							//'InGame'
      &not
      &dup
      &jnz label1      
      &pop
      &pushsdbgv 1							//'extern'
      &pushsdbgm 3							//'RA3PublicBeta'
      &pushsdb 4							//'1'
      &equals
     label1:
      &setProperty
      &stop
    &end // of frame 0
  &end // of defineMovieClip 7

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 10 // total frames: 1

    &placeMovieClip 9 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 9
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 1

    &frame 0
      &constants 'initialized', 'TacticalHUDSelectionDetails.swf', 'selectionDetails', 'loadMovie'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 1							//'TacticalHUDSelectionDetails.swf'
      &pushone
      &pushsdbgv 2							//'selectionDetails'
      &dcallmp 3							// loadMovie()
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 11

  &defineMovieClip 12 // total frames: 1

    &placeMovieClip 9 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 9
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 1

    &placeMovieClip 9 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 9
  &end // of defineMovieClip 13

  &defineMovieClip 14 // total frames: 1

    &placeMovieClip 9 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 9
  &end // of defineMovieClip 14

  &defineMovieClip 15 // total frames: 1

    &frame 0
      &constants 'removeMovieClip', 'LoadMovieStage', 'attachMovie', 'Pointer', 'encode', 'AddStage', 'RemoveStage', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 AddStage (r:4='idArg') (r:1='this')
        &pushregister 4
        &toNumber
        &setRegister r:3
        &pop
        &pushregister 1
        &pushregister 3
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushregister 2
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushzero
        &pushregister 2
        &dcallmp 0							// removeMovieClip()
       label1:
        &pushregister 3
        &pushregister 3
        &toString
        &pushsdb 1							//'LoadMovieStage'
        &pushbyte 3
        &pushregister 1
        &pushsdb 2							//'attachMovie'
        &callMethod
        &setRegister r:2
        &pop
        &pushregister 2
        &pushone
        &pushsdbgv 3							//'Pointer'
        &pushsdb 4							//'encode'
        &callMethod
        &return
      &end // of function AddStage

      &function2 RemoveStage (r:4='idArg') (r:1='this')
        &pushregister 4
        &toNumber
        &setRegister r:3
        &pop
        &pushregister 1
        &pushregister 3
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushzero
        &pushregister 2
        &dcallmp 0							// removeMovieClip()
      &end // of function RemoveStage

      &function onUnload (      )      
        &pushsdb 5							//'AddStage'
        &delete2
        &pop
        &pushsdb 6							//'RemoveStage'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 9							//'FSCommand:'
      &pushsdb 10							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 13							//''
      &getURL2
     label2:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 15

  &defineMovieClip 16 // total frames: 1

    &frame 0
      &constants 'initialized', 'HelpBox.swf', 'helpBox', 'loadMovie', 'TacticalHUDObjectiveIndicatorStage.swf', 'objectiveIndicatorStage', 'TacticalHUDPlayerPowersBar.swf', 'playerPowersBar', 'HUDTickerPanel.swf', 'ticker', 'TacticalHUDSpecialPowerTimers.swf', 'specialPowerTimerDisplay', 'igm_replayControlPanel.swf', 'replayControl', 'TacticalHUDTelestratorControlPanel.swf', 'telestratorControl', 'TacticalHUDObserverControlPanel.swf', 'observerControl', 'HUDVoiceChatControlPanel.swf', 'voiceChatControl', 'TacticalHUDCoOpAIOrderPanel.swf', 'coOpAIOrderPanel', 'TacticalHUDExperiencePanel.swf', 'expPanel'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 1							//'HelpBox.swf'
      &pushone
      &pushsdbgv 2							//'helpBox'
      &dcallmp 3							// loadMovie()
      &pushsdb 4							//'TacticalHUDObjectiveIndicatorStage.swf'
      &pushone
      &pushsdbgv 5							//'objectiveIndicatorStage'
      &dcallmp 3							// loadMovie()
      &pushsdb 6							//'TacticalHUDPlayerPowersBar.swf'
      &pushone
      &pushsdbgv 7							//'playerPowersBar'
      &dcallmp 3							// loadMovie()
      &pushsdb 8							//'HUDTickerPanel.swf'
      &pushone
      &pushsdbgv 9							//'ticker'
      &dcallmp 3							// loadMovie()
      &pushsdb 10							//'TacticalHUDSpecialPowerTimers.swf'
      &pushone
      &pushsdbgv 11							//'specialPowerTimerDisplay'
      &dcallmp 3							// loadMovie()
      &pushsdb 12							//'igm_replayControlPanel.swf'
      &pushone
      &pushsdbgv 13							//'replayControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 14							//'TacticalHUDTelestratorControlPanel.swf'
      &pushone
      &pushsdbgv 15							//'telestratorControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 16							//'TacticalHUDObserverControlPanel.swf'
      &pushone
      &pushsdbgv 17							//'observerControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 18							//'HUDVoiceChatControlPanel.swf'
      &pushone
      &pushsdbgv 19							//'voiceChatControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 20							//'TacticalHUDCoOpAIOrderPanel.swf'
      &pushone
      &pushsdbgv 21							//'coOpAIOrderPanel'
      &dcallmp 3							// loadMovie()
      &pushsdb 22							//'TacticalHUDExperiencePanel.swf'
      &pushone
      &pushsdbgv 23							//'expPanel'
      &dcallmp 3							// loadMovie()
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 16

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 18 // total frames: 33

    &frame 0
      &constants 'initialized', 'content', '_visible', '_parent', 'letterBoxVisible'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdbgv 1							//'content'
      &pushsdb 2							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 3							//'_parent'
      &pushsdbgm 4							//'letterBoxVisible'
      &not
      &jnz label1      
      &gotoLabel '_fadeIn'
      &play
     label1:
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label2:
    &end // of frame 0

    &frame 0
      &pushsgv 'content'
      &pushs '_visible'
      &pushfalse
      &setMember
    &end // of frame 0

    &frame 1
      &stop
    &end // of frame 1

    &frame 2
      &pushsgv 'content'
      &pushs '_visible'
      &pushtrue
      &setMember
    &end // of frame 2

    &frame 21
      &stop
    &end // of frame 21

    &frame 32
      &gotoLabel '_hide'
      &play
    &end // of frame 32
  &end // of defineMovieClip 18

  &defineMovieClip 31 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

      &function2 onUnload () (r:1='this', r:2='_root')
        &pushregister 1
        &pushone
        &pushregister 2
        &dcallmp 0							// DetachFactionObserver()
        &pushsdb 1							//'OnCurrentFactionChanged'
        &delete2
        &pop
        &pushsdb 2							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 3							//'factionSkinned'
      &not
      &not
      &jnz label1      
      &pushzero
      &pushsdbgv 4							//'_root'
      &pushsdb 5							//'GetCurrentFaction'
      &callMethod
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'factionSkinned'
      &pushtrue
      &setVariable
     label1:
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 31

  &defineMovieClip 32 // total frames: 1
  &end // of defineMovieClip 32
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'WhiteRectangle' &as 33
  &end // of importAssets

  &defineMovieClip 34 // total frames: 1
  &end // of defineMovieClip 34

  &defineMovieClip 37 // total frames: 1
  &end // of defineMovieClip 37

  &defineMovieClip 40 // total frames: 1

    &frame 0
      &constants '', 'highlightClip', 'highlightDummy', 'CreateHUDGadgetFlash', 'Close', 'SetVisibility', 'SetHighlighted', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 SetVisibility (r:1='visArg') ()
        &pushsdb 0							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 SetHighlighted (r:3='highlightArg') (r:1='_global')
        &pushregister 3
        &pushzero
        &equals
        &not
        &setRegister r:2
        &pop
        &pushregister 2
        &not
        &jnz label2        
        &pushsdbgv 1							//'highlightClip'
        &pushundef
        &equals
        &not
        &jnz label1        
        &pushsdb 1							//'highlightClip'
        &pushsdbgv 2							//'highlightDummy'
        &pushone
        &pushregister 1
        &dcallmsv 3							// CreateHUDGadgetFlash()
       label1:
        &jmp label3        
       label2:
        &pushzero
        &pushsdbgv 1							//'highlightClip'
        &dcallmp 4							// Close()
        &pushsdb 1							//'highlightClip'
        &pushundef
        &setVariable
       label3:
      &end // of function SetHighlighted

      &function onUnload (      )      
        &pushzero
        &pushsdbgv 1							//'highlightClip'
        &dcallmp 4							// Close()
        &pushsdb 5							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 6							//'SetHighlighted'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label4      
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 9							//'FSCommand:'
      &pushsdb 10							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label4:
      &stop
    &end // of frame 0

    &placeMovieClip 37 
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'Resource'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 37
  &end // of defineMovieClip 40

  &defineMovieClip 41 // total frames: 1

    &placeMovieClip 9 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 9
  &end // of defineMovieClip 41
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 42
  &end // of importAssets

  &defineMovieClip 43 // total frames: 1

    &placeMovieClip 9 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 9
  &end // of defineMovieClip 43

  &defineMovieClip 44 // total frames: 1

    &frame 0
      &constants 'energyMeter', 'helpSite', 'energyMeterHelpSite', 'OnContentLoaded', 'onUnload', 'initialized', 'TacticalHUDCommandModeBar.swf', 'commandModeBar', 'loadMovie', 'TacticalHUDEnergyMeter.swf', 'TacticalHUDRadar.swf', 'radar', 'TacticalHUDMainCommandBar.swf', 'mainCommandBar'  
      &function2 OnContentLoaded (r:1='clip') ()
        &pushregister 1
        &pushsdbgv 0							//'energyMeter'
        &equals
        &not
        &jnz label1        
        &pushsdbgv 0							//'energyMeter'
        &pushsdb 1							//'helpSite'
        &pushsdbgv 2							//'energyMeterHelpSite'
        &setMember
       label1:
      &end // of function OnContentLoaded

      &function onUnload (      )      
        &pushsdb 3							//'OnContentLoaded'
        &delete2
        &pop
        &pushsdb 4							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 6							//'TacticalHUDCommandModeBar.swf'
      &pushone
      &pushsdbgv 7							//'commandModeBar'
      &dcallmp 8							// loadMovie()
      &pushsdb 9							//'TacticalHUDEnergyMeter.swf'
      &pushone
      &pushsdbgv 0							//'energyMeter'
      &dcallmp 8							// loadMovie()
      &pushsdb 10							//'TacticalHUDRadar.swf'
      &pushone
      &pushsdbgv 11							//'radar'
      &dcallmp 8							// loadMovie()
      &pushsdb 12							//'TacticalHUDMainCommandBar.swf'
      &pushone
      &pushsdbgv 13							//'mainCommandBar'
      &dcallmp 8							// loadMovie()
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
     label2:
      &stop
    &end // of frame 0

    &placeMovieClip 42 &as 'energyMeterHelpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'right'
      &end
    &end // of placeMovieClip 42
  &end // of defineMovieClip 44

  &defineMovieClip 45 // total frames: 23

    &frame 2
      &stop
    &end // of frame 2

    &frame 3
      &pushsgv 'upperSidebar'
      &pushs '_visible'
      &pushtrue
      &setMember
      &play
    &end // of frame 3

    &frame 9
      &pushsgv 'upperSidebar'
      &pushs '_visible'
      &pushtrue
      &setMember
      &play
    &end // of frame 9

    &frame 15
      &stop
    &end // of frame 15

    &frame 16
      &play
    &end // of frame 16

    &frame 22
      &pushsgv 'upperSidebar'
      &pushs '_visible'
      &pushfalse
      &setMember
      &stop
    &end // of frame 22
  &end // of defineMovieClip 45
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'InputShield' &as 46
  &end // of importAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'LoadMovieStage' &as 47
  &end // of importAssets

  &defineMovieClip 48 // total frames: 1

    &frame 0
      &constants 'box', '_alpha', '', 'SetOpacity', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 SetOpacity (r:1='opacity') ()
        &pushsdbgv 0							//'box'
        &pushsdb 1							//'_alpha'
        &pushregister 1
        &pushbyte 100
        &multiply
        &setMember
        &pushsdb 2							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &greaterThan
        &setProperty
      &end // of function SetOpacity

      &function onUnload (      )      
        &pushsdb 3							//'SetOpacity'
        &delete2
        &pop
        &pushsdb 4							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 2							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 6							//'FSCommand:'
      &pushsdb 7							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 2							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 48

  &defineMovieClip 50 // total frames: 2
  &end // of defineMovieClip 50
  
  &exportAssets
    2 &as 'SubtitleRenderingSurfaceContentPlaceholder'
  &end // of exportAssets

  &defineMovieClip 51 // total frames: 2

    &frame 0
      &constants 'x', 'site', '_x', 'y', '_y', '_parent', 'localToGlobal', 'globalToLocal', '', 'onEnterFrame', 'TrackSite', 'baseLeft', 'baseBottom', 'SetSubtitlePanel', 'OnSubtitlePanelSiteSet', 'OnSubtitlePanelSiteCleared', 'onUnload', 'initialized', 'contentClip', '_type', 'Render', 'this', 'qualifyName', 'content', 'SubtitleRenderingSurfaceContentPlaceholder', 'attachMovie', '_height', '_root'  
      &function2 TrackSite () (r:1='this')
        &pushsdb 0							//'x'
        &pushsdbgv 1							//'site'
        &pushsdbgm 2							//'_x'
        &pushsdb 3							//'y'
        &pushsdbgv 1							//'site'
        &pushsdbgm 4							//'_y'
        &pushbyte 2
        &initObject
        &setRegister r:2
        &pop
        &pushregister 2
        &pushone
        &pushsdbgv 1							//'site'
        &pushsdbgm 5							//'_parent'
        &dcallmp 6							// localToGlobal()
        &pushregister 2
        &pushone
        &pushregister 1
        &dcallmp 7							// globalToLocal()
        &pushsdb 8							//''
        &pushzero
        &pushregister 2
        &pushsdbgm 0							//'x'
        &setProperty
        &pushsdb 8							//''
        &pushone
        &pushregister 2
        &pushsdbgm 3							//'y'
        &pushsdb 8							//''
        &pushbyte 9
        &getProperty
        &subtract
        &setProperty
      &end // of function TrackSite

      &function2 OnSubtitlePanelSiteSet (r:1='panelSite') ()
        &pushsdb 1							//'site'
        &pushregister 1
        &setVariable
        &pushsdb 9							//'onEnterFrame'
        &function  (        )        
          &pushzero
          &dcallfp 10							// TrackSite()
        &end // of function 

        &setVariable
        &pushzero
        &dcallfp 10							// TrackSite()
      &end // of function OnSubtitlePanelSiteSet

      &function OnSubtitlePanelSiteCleared (      )      
        &pushsdb 9							//'onEnterFrame'
        &delete2
        &pop
        &pushsdb 1							//'site'
        &pushnull
        &setVariable
        &pushsdb 8							//''
        &pushzero
        &pushsdbgv 11							//'baseLeft'
        &setProperty
        &pushsdb 8							//''
        &pushone
        &pushsdbgv 12							//'baseBottom'
        &pushsdb 8							//''
        &pushbyte 9
        &getProperty
        &subtract
        &setProperty
      &end // of function OnSubtitlePanelSiteCleared

      &function2 onUnload () (r:1='_root')
        &pushnull
        &pushone
        &pushregister 1
        &dcallmp 13							// SetSubtitlePanel()
        &pushsdb 10							//'TrackSite'
        &delete2
        &pop
        &pushsdb 14							//'OnSubtitlePanelSiteSet'
        &delete2
        &pop
        &pushsdb 15							//'OnSubtitlePanelSiteCleared'
        &delete2
        &pop
        &pushsdb 9							//'onEnterFrame'
        &delete2
        &pop
        &pushsdb 16							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 17							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 18							//'contentClip'
      &pushsdb 19							//'_type'
      &pushsdb 20							//'Render'
      &pushthisgv
      &pushbyte 2
      &pushsdb 22							//'qualifyName'
      &callFunction
      &pushone
      &initObject
      &pushzero
      &pushsdb 23							//'content'
      &pushsdb 24							//'SubtitleRenderingSurfaceContentPlaceholder'
      &pushbyte 4
      &pushthisgv
      &pushsdb 25							//'attachMovie'
      &callMethod
      &varEquals
      &pushsdbgv 18							//'contentClip'
      &pushsdb 4							//'_y'
      &pushzero
      &pushsdbgv 18							//'contentClip'
      &pushsdbgm 26							//'_height'
      &subtract
      &setMember
      &pushsdb 1							//'site'
      &pushnull
      &setVariable
      &pushsdb 11							//'baseLeft'
      &pushsdb 8							//''
      &pushzero
      &getProperty
      &setVariable
      &pushsdb 12							//'baseBottom'
      &pushsdb 8							//''
      &pushone
      &getProperty
      &pushsdb 8							//''
      &pushbyte 9
      &getProperty
      &add
      &setVariable
      &pushthisgv
      &pushone
      &pushsdbgv 27							//'_root'
      &dcallmp 13							// SetSubtitlePanel()
      &pushsdb 17							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0

    &placeMovieClip 50 &as 'sizer'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 50
  &end // of defineMovieClip 51

  &defineMovieClip 52 // total frames: 11

    &frame 1
      &stop
    &end // of frame 1

    &frame 10
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
      &stop
    &end // of frame 10
  &end // of defineMovieClip 52

  &frame 1
    &play
  &end // of frame 1

  &frame 7
    &pushs 'FSCommand:'
    &pushs 'OnFadeInComplete'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
  &end // of frame 7

  &frame 13
    &stop
  &end // of frame 13

  &frame 14
    &play
  &end // of frame 14

  &frame 20
    &constants 'hideMainCount', 'main', '_visible', 'FSCommand:', 'OnFadeOutComplete', 'this', 'qualifyName', ''  
    &pushsdbgv 0							//'hideMainCount'
    &pushundef
    &equals
    &not
    &dup
    &not
    &jnz label1    
    &pop
    &pushsdbgv 0							//'hideMainCount'
    &pushsdb 0							//'hideMainCount'
    &pushsdbgv 0							//'hideMainCount'
    &increment
    &setVariable
    &pushzero
    &equals
   label1:
    &not
    &jnz label2    
    &pushsdbgv 1							//'main'
    &pushsdb 2							//'_visible'
    &pushfalse
    &setMember
   label2:
    &stop
    &pushsdb 3							//'FSCommand:'
    &pushsdb 4							//'OnFadeOutComplete'
    &pushthisgv
    &pushbyte 2
    &pushsdb 6							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 7							//''
    &getURL2
  &end // of frame 20
&end
