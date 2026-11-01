movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA4E8A~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineButton 2
  
    &on     &overUpToOverDown
      &pushzero
      &pushsgv 'client'
      &pushs 'OnPress'
      &callmp
    &end
  
    &on     &overDownToOverUp
      &pushzero
      &pushsgv 'client'
      &pushs 'OnRelease'
      &callmp
    &end
  
    &on     &idleToOverUp    ,&outDownToOverDown    ,&idleToOverDown
      &pushzero
      &pushsgv 'client'
      &pushs 'OnRollOver'
      &callmp
    &end
  
    &on     &overUpToIdle    ,&overDownToOutDown    ,&overDownToIdle
      &pushzero
      &pushsgv 'client'
      &pushs 'OnRollOut'
      &callmp
    &end
  &end // of defineButton 2

  &defineMovieClip 3 // total frames: 40

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29

    &frame 39
      &stop
    &end // of frame 39
  &end // of defineMovieClip 3
  
  &exportAssets
    3 &as 'RA3CommandModeButtonClip'
  &end // of exportAssets

  &defineMovieClip 10 // total frames: 30

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
  &end // of defineMovieClip 10

  &defineMovieClip 13 // total frames: 20

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'SellButtonNormal'
  &end // of exportAssets

  &defineMovieClip 20 // total frames: 30

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
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 20

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 21
  
  &exportAssets
    21 &as 'RepairButtonNormal'
  &end // of exportAssets

  &defineMovieClip 28 // total frames: 30

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
  &end // of defineMovieClip 28

  &defineMovieClip 29 // total frames: 20

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 29
  
  &exportAssets
    29 &as 'SellButtonHighlighted'
  &end // of exportAssets

  &defineMovieClip 36 // total frames: 30

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
  &end // of defineMovieClip 36

  &defineMovieClip 37 // total frames: 20

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 37
  
  &exportAssets
    37 &as 'RepairButtonHighlighted'
  &end // of exportAssets

  &defineMovieClip 44 // total frames: 30

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
  &end // of defineMovieClip 44

  &defineMovieClip 45 // total frames: 20

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 45
  
  &exportAssets
    45 &as 'ObjectivesButtonHighlighted'
  &end // of exportAssets

  &defineMovieClip 52 // total frames: 30

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
  &end // of defineMovieClip 52

  &defineMovieClip 53 // total frames: 20

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 53
  
  &exportAssets
    53 &as 'ObjectivesButtonNormal'
  &end // of exportAssets

  &frame 0
    &constants 'sellButton', 'Pointer', 'encode', 'repairButton', 'pauseButton', 'GetSellButton', 'GetRepairButton', 'GetPauseButton', 'onUnload', 'initialized', 'helpSite', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
    &function GetSellButton (    )    
      &pushsdbgv 0							//'sellButton'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetSellButton

    &function GetRepairButton (    )    
      &pushsdbgv 3							//'repairButton'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetRepairButton

    &function GetPauseButton (    )    
      &pushsdbgv 4							//'pauseButton'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetPauseButton

    &function onUnload (    )    
      &pushsdb 5							//'GetSellButton'
      &delete2
      &pop
      &pushsdb 6							//'GetRepairButton'
      &delete2
      &pop
      &pushsdb 7							//'GetPauseButton'
      &delete2
      &pop
      &pushsdb 8							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 9							//'initialized'
    &not
    &not
    &jnz label1    
    &pushsdbgv 0							//'sellButton'
    &pushsdb 10							//'helpSite'
    &pushsdbgv 10							//'helpSite'
    &setMember
    &pushsdbgv 3							//'repairButton'
    &pushsdb 10							//'helpSite'
    &pushsdbgv 10							//'helpSite'
    &setMember
    &pushsdbgv 4							//'pauseButton'
    &pushsdb 10							//'helpSite'
    &pushsdbgv 10							//'helpSite'
    &setMember
    &pushsdb 9							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 11							//'FSCommand:'
    &pushsdb 12							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 14							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 15							//''
    &getURL2
   label1:
    &stop
  &end // of frame 0
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 54
  &end // of importAssets

  &placeMovieClip 54 &as 'helpSite'
  
    &onClipEvent &construct
      &pushs 'vertAlignment'
      &pushssv 'top'
      &pushs 'horzAlignment'
      &pushssv 'right'
    &end
  &end // of placeMovieClip 54

  &defineMovieClip 55 // total frames: 1
  &end // of defineMovieClip 55

  &defineMovieClip 56 // total frames: 1

    &frame 0
      &constants 'stateLabel', 'imageStage', 'clip', 'gotoAndPlay', 'backgroundStage', 'buttonStage', 'isEnabled', '_down', 'SetState', '_over', 'FSCommand:', 'OnClicked', 'qualifyName', '', 'isOver', 'OnRollOver', '_up', 'OnRollOut', 'removeMovieClip', 'imageClipName', 'length', 'isSelected', 'Normal', 'Highlighted', 'attachMovie', 'backgroundClipName', 'flashyThingy', '_visible', '_disabled', 'AttachContent', 'initialized', 'Start', 'Stop', 'shouldPlay', 'highlightClip', 'CreateHUDGadgetFlash', 'Close', 'helpSite', 'Pointer', 'encode', 'OnPress', 'OnRelease', 'SetVisibility', 'SetEnabled', 'SetSelected', 'SetFlashing', 'SetHighlighted', 'GetHelpSite', 'onUnload', 'buttonClip', 'buttonClipName', 'client', 'this', '1', 'OnInitialized', 'extern', 'InGame'  
      &function2 SetState (r:1='newStateLabel') ()
        &pushregister 1
        &pushsdbgv 0							//'stateLabel'
        &equals
        &not
        &not
        &jnz label1        
        &pushregister 1
        &pushone
        &pushsdbgv 1							//'imageStage'
        &pushsdbgm 2							//'clip'
        &dcallmp 3							// gotoAndPlay()
        &pushregister 1
        &pushone
        &pushsdbgv 4							//'backgroundStage'
        &pushsdbgm 2							//'clip'
        &dcallmp 3							// gotoAndPlay()
        &pushregister 1
        &pushone
        &pushsdbgv 5							//'buttonStage'
        &pushsdbgm 2							//'clip'
        &dcallmp 3							// gotoAndPlay()
        &pushsdb 0							//'stateLabel'
        &pushregister 1
        &setVariable
       label1:
      &end // of function SetState

      &function OnPress (      )      
        &pushsdbgv 6							//'isEnabled'
        &not
        &jnz label2        
        &pushsdb 7							//'_down'
        &pushone
        &dcallfp 8							// SetState()
       label2:
      &end // of function OnPress

      &function2 OnRelease () (r:1='this')
        &pushsdbgv 6							//'isEnabled'
        &not
        &jnz label3        
        &pushsdb 9							//'_over'
        &pushone
        &dcallfp 8							// SetState()
       label3:
        &pushsdb 10							//'FSCommand:'
        &pushsdb 11							//'OnClicked'
        &pushregister 1
        &pushbyte 2
        &pushsdb 12							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 13							//''
        &getURL2
      &end // of function OnRelease

      &function2 OnRollOver () (r:1='this')
        &pushsdbgv 14							//'isOver'
        &not
        &not
        &jnz label5        
        &pushsdbgv 6							//'isEnabled'
        &not
        &jnz label4        
        &pushsdb 9							//'_over'
        &pushone
        &dcallfp 8							// SetState()
       label4:
        &pushsdb 14							//'isOver'
        &pushtrue
        &setVariable
        &pushsdb 10							//'FSCommand:'
        &pushsdb 15							//'OnRollOver'
        &pushregister 1
        &pushbyte 2
        &pushsdb 12							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 13							//''
        &getURL2
       label5:
      &end // of function OnRollOver

      &function2 OnRollOut () (r:1='this')
        &pushsdbgv 14							//'isOver'
        &not
        &jnz label7        
        &pushsdbgv 6							//'isEnabled'
        &not
        &jnz label6        
        &pushsdb 16							//'_up'
        &pushone
        &dcallfp 8							// SetState()
       label6:
        &pushsdb 14							//'isOver'
        &pushfalse
        &setVariable
        &pushsdb 10							//'FSCommand:'
        &pushsdb 17							//'OnRollOut'
        &pushregister 1
        &pushbyte 2
        &pushsdb 12							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 13							//''
        &getURL2
       label7:
      &end // of function OnRollOut

      &function2 AttachContent () ()
        &pushsdbgv 1							//'imageStage'
        &pushsdbgm 2							//'clip'
        &setRegister r:1
        &pop
        &pushregister 1
        &pushundef
        &equals
        &not
        &not
        &jnz label8        
        &pushzero
        &pushregister 1
        &dcallmp 18							// removeMovieClip()
       label8:
        &pushsdbgv 19							//'imageClipName'
        &pushundef
        &equals
        &not
        &dup
        &not
        &jnz label9        
        &pop
        &pushsdbgv 19							//'imageClipName'
        &pushsdbgm 20							//'length'
        &pushzero
        &greaterThan
       label9:
        &not
        &jnz label12        
        &pushsdbgv 19							//'imageClipName'
        &pushsdbgv 21							//'isSelected'
        &jnz label10        
        &pushsdb 22							//'Normal'
        &jmp label11        
       label10:
        &pushsdb 23							//'Highlighted'
       label11:
        &add
        &setRegister r:2
        &pop
        &pushzero
        &pushsdb 2							//'clip'
        &pushregister 2
        &pushbyte 3
        &pushsdbgv 1							//'imageStage'
        &pushsdb 24							//'attachMovie'
        &callMethod
        &setRegister r:1
        &pop
        &pushsdbgv 0							//'stateLabel'
        &pushone
        &pushregister 1
        &dcallmp 3							// gotoAndPlay()
       label12:
        &pushsdbgv 4							//'backgroundStage'
        &pushsdbgm 2							//'clip'
        &setRegister r:1
        &pop
        &pushregister 1
        &pushundef
        &equals
        &not
        &not
        &jnz label13        
        &pushzero
        &pushregister 1
        &dcallmp 18							// removeMovieClip()
       label13:
        &pushsdbgv 25							//'backgroundClipName'
        &pushundef
        &equals
        &not
        &dup
        &not
        &jnz label14        
        &pop
        &pushsdbgv 25							//'backgroundClipName'
        &pushsdbgm 20							//'length'
        &pushzero
        &greaterThan
       label14:
        &not
        &jnz label17        
        &pushzero
        &pushsdb 2							//'clip'
        &pushsdbgv 25							//'backgroundClipName'
        &pushsdbgv 21							//'isSelected'
        &jnz label15        
        &pushsdb 22							//'Normal'
        &jmp label16        
       label15:
        &pushsdb 23							//'Highlighted'
       label16:
        &add
        &pushbyte 3
        &pushsdbgv 4							//'backgroundStage'
        &pushsdb 24							//'attachMovie'
        &callMethod
        &setRegister r:1
        &pop
        &pushsdbgv 0							//'stateLabel'
        &pushone
        &pushregister 1
        &dcallmp 3							// gotoAndPlay()
       label17:
      &end // of function AttachContent

      &function2 SetVisibility (r:2='visArg') ()
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushsdb 13							//''
        &pushbyte 7
        &pushregister 1
        &setProperty
        &pushsdbgv 26							//'flashyThingy'
        &pushsdb 27							//'_visible'
        &pushregister 1
        &setMember
      &end // of function SetVisibility

      &function2 SetEnabled (r:2='enableArg') ()
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushregister 1
        &pushsdbgv 6							//'isEnabled'
        &equals
        &not
        &not
        &jnz label20        
        &pushsdb 6							//'isEnabled'
        &pushregister 1
        &setVariable
        &pushsdbgv 6							//'isEnabled'
        &jnz label18        
        &pushsdb 28							//'_disabled'
        &jmp label19        
       label18:
        &pushsdb 16							//'_up'
       label19:
        &pushone
        &dcallfp 8							// SetState()
       label20:
      &end // of function SetEnabled

      &function2 SetSelected (r:2='selectArg') ()
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushregister 1
        &pushsdbgv 21							//'isSelected'
        &equals
        &not
        &not
        &jnz label21        
        &pushsdb 21							//'isSelected'
        &pushregister 1
        &setVariable
        &pushzero
        &dcallfp 29							// AttachContent()
       label21:
      &end // of function SetSelected

      &function2 SetFlashing (r:2='flashArg') ()
        &pushsdbgv 26							//'flashyThingy'
        &pushundef
        &equals
        &not
        &jnz label22        
        &pushundef
        &return
       label22:
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushsdbgv 26							//'flashyThingy'
        &pushsdbgm 30							//'initialized'
        &not
        &jnz label25        
        &pushregister 1
        &not
        &jnz label23        
        &pushzero
        &pushsdbgv 26							//'flashyThingy'
        &dcallmp 31							// Start()
        &jmp label24        
       label23:
        &pushzero
        &pushsdbgv 26							//'flashyThingy'
        &dcallmp 32							// Stop()
       label24:
        &jmp label26        
       label25:
        &pushsdbgv 26							//'flashyThingy'
        &pushsdb 33							//'shouldPlay'
        &pushregister 1
        &setMember
       label26:
      &end // of function SetFlashing

      &function2 SetHighlighted (r:4='highlightArg') (r:1='this', r:2='_global')
        &pushregister 4
        &pushzero
        &equals
        &not
        &setRegister r:3
        &pop
        &pushregister 3
        &not
        &jnz label28        
        &pushsdbgv 34							//'highlightClip'
        &pushundef
        &equals
        &not
        &jnz label27        
        &pushsdb 34							//'highlightClip'
        &pushregister 1
        &pushone
        &pushregister 2
        &dcallmsv 35							// CreateHUDGadgetFlash()
       label27:
        &jmp label29        
       label28:
        &pushsdbgv 34							//'highlightClip'
        &pushundef
        &equals
        &not
        &not
        &jnz label29        
        &pushzero
        &pushsdbgv 34							//'highlightClip'
        &dcallmp 36							// Close()
        &pushsdb 34							//'highlightClip'
        &pushundef
        &setVariable
       label29:
      &end // of function SetHighlighted

      &function GetHelpSite (      )      
        &pushsdbgv 37							//'helpSite'
        &pushone
        &pushsdbgv 38							//'Pointer'
        &pushsdb 39							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function onUnload (      )      
        &pushzero
        &pushsdbgv 34							//'highlightClip'
        &dcallmp 36							// Close()
        &pushsdb 8							//'SetState'
        &delete2
        &pop
        &pushsdb 40							//'OnPress'
        &delete2
        &pop
        &pushsdb 41							//'OnRelease'
        &delete2
        &pop
        &pushsdb 15							//'OnRollOver'
        &delete2
        &pop
        &pushsdb 17							//'OnRollOut'
        &delete2
        &pop
        &pushsdb 29							//'AttachContent'
        &delete2
        &pop
        &pushsdb 42							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 43							//'SetEnabled'
        &delete2
        &pop
        &pushsdb 44							//'SetSelected'
        &delete2
        &pop
        &pushsdb 45							//'SetFlashing'
        &delete2
        &pop
        &pushsdb 46							//'SetHighlighted'
        &delete2
        &pop
        &pushsdb 47							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 48							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 30							//'initialized'
      &not
      &not
      &jnz label31      
      &pushsdb 0							//'stateLabel'
      &pushsdb 28							//'_disabled'
      &setVariable
      &pushsdb 6							//'isEnabled'
      &pushfalse
      &setVariable
      &pushsdb 21							//'isSelected'
      &pushfalse
      &setVariable
      &pushsdb 14							//'isOver'
      &pushfalse
      &setVariable
      &pushsdb 49							//'buttonClip'
      &pushzero
      &pushsdb 2							//'clip'
      &pushsdbgv 50							//'buttonClipName'
      &pushbyte 3
      &pushsdbgv 5							//'buttonStage'
      &pushsdb 24							//'attachMovie'
      &callMethod
      &varEquals
      &pushsdbgv 49							//'buttonClip'
      &pushsdb 51							//'client'
      &pushthisgv
      &setMember
      &pushsdb 28							//'_disabled'
      &pushone
      &pushsdbgv 49							//'buttonClip'
      &dcallmp 3							// gotoAndPlay()
      &pushzero
      &dcallfp 29							// AttachContent()
      &pushsdb 53							//'1'
      &pushone
      &dcallfp 43							// SetEnabled()
      &pushsdb 30							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 10							//'FSCommand:'
      &pushsdb 54							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 13							//''
      &getURL2
      &pushsdbgv 55							//'extern'
      &pushsdbgm 56							//'InGame'
      &not
      &dup
      &not
      &jnz label30      
      &pop
      &pushsdbgv 26							//'flashyThingy'
      &pushundef
      &equals
      &not
     label30:
      &not
      &jnz label31      
     label31:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 56

  &placeMovieClip 56 &as 'pauseButton'
  
    &onClipEvent &construct
      &pushs 'buttonClipName'
      &pushssv 'RA3CommandModeButtonClip'
      &pushs 'imageClipName'
      &pushssv 'ObjectivesButton'
      &pushs 'backgroundClipName'
      &pushssv ''
    &end
  &end // of placeMovieClip 56

  &placeMovieClip 56 &as 'sellButton'
  
    &onClipEvent &construct
      &pushs 'buttonClipName'
      &pushssv 'RA3CommandModeButtonClip'
      &pushs 'imageClipName'
      &pushssv 'SellButton'
      &pushs 'backgroundClipName'
      &pushssv ''
    &end
  &end // of placeMovieClip 56

  &placeMovieClip 56 &as 'repairButton'
  
    &onClipEvent &construct
      &pushs 'buttonClipName'
      &pushssv 'RA3CommandModeButtonClip'
      &pushs 'imageClipName'
      &pushssv 'RepairButton'
      &pushs 'backgroundClipName'
      &pushssv ''
    &end
  &end // of placeMovieClip 56
&end
