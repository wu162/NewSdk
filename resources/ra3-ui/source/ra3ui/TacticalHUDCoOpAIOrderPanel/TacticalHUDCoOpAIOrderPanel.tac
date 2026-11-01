movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA877A~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 25, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 7 // total frames: 30

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
  &end // of defineMovieClip 7

  &defineMovieClip 14 // total frames: 30

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
  &end // of defineMovieClip 14

  &defineMovieClip 19 // total frames: 25

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

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as 'NormalButton'
  &end // of exportAssets

  &defineMovieClip 26 // total frames: 30

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
  &end // of defineMovieClip 26

  &defineMovieClip 33 // total frames: 30

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
  &end // of defineMovieClip 33

  &defineMovieClip 36 // total frames: 25

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

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 36
  
  &exportAssets
    36 &as 'AssaultButton'
  &end // of exportAssets

  &defineMovieClip 43 // total frames: 30

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
  &end // of defineMovieClip 43

  &defineMovieClip 50 // total frames: 30

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
  &end // of defineMovieClip 50

  &defineMovieClip 53 // total frames: 25

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

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 53
  
  &exportAssets
    53 &as 'HoldObjectButton'
  &end // of exportAssets

  &defineMovieClip 60 // total frames: 30

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
  &end // of defineMovieClip 60

  &defineMovieClip 67 // total frames: 30

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
  &end // of defineMovieClip 67

  &defineMovieClip 70 // total frames: 25

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

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 70
  
  &exportAssets
    70 &as 'HoldPositionButton'
  &end // of exportAssets

  &defineMovieClip 74 // total frames: 1
  &end // of defineMovieClip 74

  &defineMovieClip 75 // total frames: 19

    &frame 3
      &stop
    &end // of frame 3

    &frame 4
      &pushs 'isOn'
      &pushtrue
      &setVariable
    &end // of frame 4

    &frame 18
      &pushsgv 'done'
      &not
      &not
      &jnz label1      
      &gotoLabel '_on'
      &play
      &jmp label2      
     label1:
      &pushs 'isOn'
      &pushfalse
      &setVariable
      &gotoLabel '_off'
      &play
     label2:
    &end // of frame 18
  &end // of defineMovieClip 75
  
  &exportAssets
    75 &as 'GadgetFlash'
  &end // of exportAssets

  &frame 0
    &constants '_opening', 'coopControl', 'gotoAndPlay', '_closing', '', 'monitor', 'Pointer', 'encode', 'normalButton', 'assaultButton', 'holdObjectButton', 'holdPositionButton', 'portrait', 'houseColorMC', 'Color', 'setRGB', 'highlightClip', 'highlightDummy', 'CreateHUDGadgetFlash', 'Close', 'portraitFlashMC', 'isOn', '_on', 'done', 'Open', 'SetVisibility', 'GetMonitor', 'GetDefaultButton', 'GetAssaultButton', 'GetHoldObjectButton', 'GetHoldPositionButton', 'InitializeButtons', 'SetHighlighted', 'SetPortraitHighlighted', 'onUnload', 'initialized', 'Co Op AI Order Panenl Initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', 'extern', 'InGame'  
    &function2 Open () (r:1='this')
      &pushsdb 0							//'_opening'
      &pushone
      &pushregister 1
      &pushsdbgm 1							//'coopControl'
      &dcallmp 2							// gotoAndPlay()
    &end // of function Open

    &function2 Close () (r:1='this')
      &pushsdb 3							//'_closing'
      &pushone
      &pushregister 1
      &pushsdbgm 1							//'coopControl'
      &dcallmp 2							// gotoAndPlay()
    &end // of function Close

    &function2 SetVisibility (r:1='visArg') ()
      &pushsdb 4							//''
      &pushbyte 7
      &pushregister 1
      &toNumber
      &pushzero
      &equals
      &not
      &setProperty
    &end // of function SetVisibility

    &function2 GetMonitor () (r:1='this')
      &pushregister 1
      &pushsdbgm 1							//'coopControl'
      &pushsdbgm 5							//'monitor'
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetMonitor

    &function2 GetNormalButton () (r:1='this')
      &pushregister 1
      &pushsdbgm 1							//'coopControl'
      &pushsdbgm 8							//'normalButton'
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetNormalButton

    &function2 GetAssaultButton () (r:1='this')
      &pushregister 1
      &pushsdbgm 1							//'coopControl'
      &pushsdbgm 9							//'assaultButton'
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetAssaultButton

    &function2 GetHoldObjectButton () (r:1='this')
      &pushregister 1
      &pushsdbgm 1							//'coopControl'
      &pushsdbgm 10							//'holdObjectButton'
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetHoldObjectButton

    &function2 GetHoldPositionButton () (r:1='this')
      &pushregister 1
      &pushsdbgm 1							//'coopControl'
      &pushsdbgm 11							//'holdPositionButton'
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetHoldPositionButton

    &function InitializeButtons (    )    
    &end // of function InitializeButtons

    &function2 setHouseColor (r:2='houseColor') ()
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 12							//'portrait'
      &pushsdbgm 13							//'houseColorMC'
      &pushone
      &pushsdb 14							//'Color'
      &new
      &setRegister r:1
      &pop
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 15							// setRGB()
    &end // of function setHouseColor

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
      &pushsdbgv 16							//'highlightClip'
      &pushundef
      &equals
      &not
      &jnz label1      
      &pushsdb 16							//'highlightClip'
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 17							//'highlightDummy'
      &pushone
      &pushregister 1
      &dcallmsv 18							// CreateHUDGadgetFlash()
     label1:
      &jmp label3      
     label2:
      &pushsdbgv 16							//'highlightClip'
      &pushundef
      &equals
      &not
      &not
      &jnz label3      
      &pushzero
      &pushsdbgv 16							//'highlightClip'
      &dcallmp 19							// Close()
      &pushsdb 16							//'highlightClip'
      &pushundef
      &setVariable
     label3:
    &end // of function SetHighlighted

    &function2 SetPortraitHighlighted (r:2='highlightArg') ()
      &pushregister 2
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushregister 1
      &not
      &jnz label5      
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 20							//'portraitFlashMC'
      &pushsdbgm 21							//'isOn'
      &not
      &not
      &jnz label4      
      &pushsdb 22							//'_on'
      &pushone
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 20							//'portraitFlashMC'
      &dcallmp 2							// gotoAndPlay()
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 20							//'portraitFlashMC'
      &pushsdb 21							//'isOn'
      &pushtrue
      &setMember
     label4:
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 20							//'portraitFlashMC'
      &pushsdb 23							//'done'
      &pushfalse
      &setMember
      &jmp label6      
     label5:
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 20							//'portraitFlashMC'
      &pushsdbgm 21							//'isOn'
      &not
      &jnz label6      
      &pushsdbgv 1							//'coopControl'
      &pushsdbgm 20							//'portraitFlashMC'
      &pushsdb 23							//'done'
      &pushtrue
      &setMember
     label6:
    &end // of function SetPortraitHighlighted

    &function onUnload (    )    
      &pushzero
      &pushsdbgv 16							//'highlightClip'
      &dcallmp 19							// Close()
      &pushsdb 24							//'Open'
      &delete2
      &pop
      &pushsdb 19							//'Close'
      &delete2
      &pop
      &pushsdb 25							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 26							//'GetMonitor'
      &delete2
      &pop
      &pushsdb 27							//'GetDefaultButton'
      &delete2
      &pop
      &pushsdb 28							//'GetAssaultButton'
      &delete2
      &pop
      &pushsdb 29							//'GetHoldObjectButton'
      &delete2
      &pop
      &pushsdb 30							//'GetHoldPositionButton'
      &delete2
      &pop
      &pushsdb 31							//'InitializeButtons'
      &delete2
      &pop
      &pushsdb 32							//'SetHighlighted'
      &delete2
      &pop
      &pushsdb 33							//'SetPortraitHighlighted'
      &delete2
      &pop
      &pushsdb 34							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 35							//'initialized'
    &not
    &not
    &jnz label7    
    &pushsdb 36							//'Co Op AI Order Panenl Initialized'
    &trace
    &pushtrue
    &pushone
    &dcallfp 25							// SetVisibility()
    &pushzero
    &dcallfp 31							// InitializeButtons()
    &pushsdb 35							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 37							//'FSCommand:'
    &pushsdb 38							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 40							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 4							//''
    &getURL2
    &pushsdbgv 41							//'extern'
    &pushsdbgm 42							//'InGame'
    &not
    &not
    &jnz label7    
    &pushzero
    &dcallfp 24							// Open()
    &pushone
    &pushone
    &dcallfp 33							// SetPortraitHighlighted()
   label7:
  &end // of frame 0

  &frame 0
    &pushsgv 'itemStage'
    &pushs '_visible'
    &pushfalse
    &setMember
  &end // of frame 0

  &defineMovieClip 88 // total frames: 30

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
  &end // of defineMovieClip 88

  &defineButton 90
  
    &on     &overUpToOverDown
      &pushs 'FSCommand:'
      &pushs 'OnPress'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 90

  &defineMovieClip 93 // total frames: 1
  &end // of defineMovieClip 93

  &defineMovieClip 94 // total frames: 1

    &frame 0
      &constants 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 1							//'FSCommand:'
      &pushsdb 2							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 4							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 5							//''
      &getURL2
     label1:
    &end // of frame 0

    &placeMovieClip 93 
    
      &onClipEvent &load
        &pushs 'myText'
        &pushs '$'
        &pushs 'Text'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&outline'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 93
  &end // of defineMovieClip 94

  &defineButton 95
  
    &on     &overUpToOverDown
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnPress'
      &callmp
    &end
  
    &on     &overDownToOverUp
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnRelease'
      &callmp
    &end
  
    &on     &idleToOverUp
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnRollOver'
      &callmp
    &end
  
    &on     &overUpToIdle
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnRollOut'
      &callmp
    &end
  &end // of defineButton 95

  &defineMovieClip 97 // total frames: 1
  &end // of defineMovieClip 97

  &defineMovieClip 98 // total frames: 1

    &placeMovieClip 97 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 97
  &end // of defineMovieClip 98

  &defineMovieClip 99 // total frames: 1

    &frame 0
      &constants 'enabled', 'UpdateButtonGraphic', 'selected', 'secondState', '', '_disabled', 'buttonContainerClip', 'gotoAndPlay', 'mouseOver', '_over', '_on', '_pressed', '_off', 'down', 'FSCommand:', 'OnClicked', 'qualifyName', 'OnRollOver', 'OnRollOut', 'helpSite', 'Pointer', 'encode', 'highlightClip', 'CreateHUDGadgetFlash', 'Close', 'SetEnabled', 'SetSelected', 'SetSecondState', 'SetVisibility', 'OnPress', 'OnRelease', 'GetHelpSite', 'SetHighlighted', 'onUnload', 'initialized', 'helpSiteRef', 'this', 'getNextHighestDepth', 'buttonContainer', 'attachMovie', 'hitTestButton', '_width', 'hitTestWidth', '_height', 'hitTestHeight', 'OnInitialized'  
      &function2 SetEnabled (r:1='val') ()
        &pushsdb 0							//'enabled'
        &pushregister 1
        &pushzero
        &equals
        &not
        &setVariable
        &pushzero
        &dcallfp 1							// UpdateButtonGraphic()
      &end // of function SetEnabled

      &function2 SetSelected (r:1='val') ()
        &pushsdb 2							//'selected'
        &pushregister 1
        &pushzero
        &equals
        &not
        &setVariable
        &pushzero
        &dcallfp 1							// UpdateButtonGraphic()
      &end // of function SetSelected

      &function2 SetSecondState (r:1='val') ()
        &pushsdb 3							//'secondState'
        &pushregister 1
        &pushzero
        &equals
        &not
        &setVariable
        &pushzero
        &dcallfp 1							// UpdateButtonGraphic()
      &end // of function SetSecondState

      &function2 SetVisibility (r:1='vis') ()
        &pushsdb 4							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function UpdateButtonGraphic (      )      
        &pushsdbgv 0							//'enabled'
        &not
        &not
        &jnz label1        
        &pushsdb 5							//'_disabled'
        &pushone
        &pushsdbgv 6							//'buttonContainerClip'
        &dcallmp 7							// gotoAndPlay()
        &pushundef
        &return
       label1:
        &pushsdbgv 8							//'mouseOver'
        &not
        &jnz label2        
        &pushsdb 9							//'_over'
        &pushone
        &pushsdbgv 6							//'buttonContainerClip'
        &dcallmp 7							// gotoAndPlay()
        &pushundef
        &return
       label2:
        &pushsdbgv 2							//'selected'
        &not
        &jnz label3        
        &pushsdb 10							//'_on'
        &pushone
        &pushsdbgv 6							//'buttonContainerClip'
        &dcallmp 7							// gotoAndPlay()
        &pushundef
        &return
       label3:
        &pushsdbgv 3							//'secondState'
        &not
        &jnz label4        
        &pushsdb 11							//'_pressed'
        &pushone
        &pushsdbgv 6							//'buttonContainerClip'
        &dcallmp 7							// gotoAndPlay()
        &pushundef
        &return
       label4:
        &pushsdb 12							//'_off'
        &pushone
        &pushsdbgv 6							//'buttonContainerClip'
        &dcallmp 7							// gotoAndPlay()
      &end // of function UpdateButtonGraphic

      &function OnPress (      )      
        &pushsdb 13							//'down'
        &pushtrue
        &setVariable
        &pushzero
        &dcallfp 1							// UpdateButtonGraphic()
      &end // of function OnPress

      &function2 OnRelease () (r:1='this')
        &pushsdb 13							//'down'
        &pushfalse
        &setVariable
        &pushzero
        &dcallfp 1							// UpdateButtonGraphic()
        &pushsdb 14							//'FSCommand:'
        &pushsdb 15							//'OnClicked'
        &pushregister 1
        &pushbyte 2
        &pushsdb 16							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 4							//''
        &getURL2
      &end // of function OnRelease

      &function2 OnRollOver () (r:1='this')
        &pushsdbgv 8							//'mouseOver'
        &not
        &not
        &jnz label5        
        &pushsdb 8							//'mouseOver'
        &pushtrue
        &setVariable
        &pushzero
        &dcallfp 1							// UpdateButtonGraphic()
        &pushsdb 14							//'FSCommand:'
        &pushsdb 17							//'OnRollOver'
        &pushregister 1
        &pushbyte 2
        &pushsdb 16							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 4							//''
        &getURL2
       label5:
      &end // of function OnRollOver

      &function2 OnRollOut () (r:1='this')
        &pushsdbgv 8							//'mouseOver'
        &not
        &jnz label6        
        &pushsdb 8							//'mouseOver'
        &pushfalse
        &setVariable
        &pushzero
        &dcallfp 1							// UpdateButtonGraphic()
        &pushsdb 14							//'FSCommand:'
        &pushsdb 18							//'OnRollOut'
        &pushregister 1
        &pushbyte 2
        &pushsdb 16							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 4							//''
        &getURL2
       label6:
      &end // of function OnRollOut

      &function GetHelpSite (      )      
        &pushsdbgv 19							//'helpSite'
        &pushone
        &pushsdbgv 20							//'Pointer'
        &pushsdb 21							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function2 SetHighlighted (r:3='highlightArg') (r:1='_global')
        &pushregister 3
        &pushzero
        &equals
        &not
        &setRegister r:2
        &pop
        &pushregister 2
        &not
        &jnz label8        
        &pushsdbgv 22							//'highlightClip'
        &pushundef
        &equals
        &not
        &jnz label7        
        &pushsdb 22							//'highlightClip'
        &pushsdbgv 6							//'buttonContainerClip'
        &pushone
        &pushregister 1
        &dcallmsv 23							// CreateHUDGadgetFlash()
       label7:
        &jmp label9        
       label8:
        &pushsdbgv 22							//'highlightClip'
        &pushundef
        &equals
        &not
        &not
        &jnz label9        
        &pushzero
        &pushsdbgv 22							//'highlightClip'
        &dcallmp 24							// Close()
        &pushsdb 22							//'highlightClip'
        &pushundef
        &setVariable
       label9:
      &end // of function SetHighlighted

      &function onUnload (      )      
        &pushzero
        &pushsdbgv 22							//'highlightClip'
        &dcallmp 24							// Close()
        &pushsdb 25							//'SetEnabled'
        &delete2
        &pop
        &pushsdb 26							//'SetSelected'
        &delete2
        &pop
        &pushsdb 27							//'SetSecondState'
        &delete2
        &pop
        &pushsdb 28							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 1							//'UpdateButtonGraphic'
        &delete2
        &pop
        &pushsdb 29							//'OnPress'
        &delete2
        &pop
        &pushsdb 30							//'OnRelease'
        &delete2
        &pop
        &pushsdb 17							//'OnRollOver'
        &delete2
        &pop
        &pushsdb 18							//'OnRollOut'
        &delete2
        &pop
        &pushsdb 31							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 32							//'SetHighlighted'
        &delete2
        &pop
        &pushsdb 33							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 34							//'initialized'
      &not
      &not
      &jnz label10      
      &pushsdb 0							//'enabled'
      &pushtrue
      &setVariable
      &pushsdb 13							//'down'
      &pushfalse
      &setVariable
      &pushsdb 8							//'mouseOver'
      &pushfalse
      &setVariable
      &pushsdb 2							//'selected'
      &pushfalse
      &setVariable
      &pushsdb 3							//'secondState'
      &pushfalse
      &setVariable
      &pushsdb 19							//'helpSite'
      &pushsdbgv 35							//'helpSiteRef'
      &getVariable
      &setVariable
      &pushzero
      &pushthisgv
      &pushsdb 37							//'getNextHighestDepth'
      &callMethod
      &pushsdb 6							//'buttonContainerClip'
      &pushthisgv
      &pushsdbgm 38							//'buttonContainer'
      &pushbyte 3
      &pushthisgv
      &dcallmp 39							// attachMovie()
      &pushzero
      &dcallfp 1							// UpdateButtonGraphic()
      &pushsdbgv 40							//'hitTestButton'
      &pushsdb 41							//'_width'
      &pushsdbgv 42							//'hitTestWidth'
      &setMember
      &pushsdbgv 40							//'hitTestButton'
      &pushsdb 43							//'_height'
      &pushsdbgv 44							//'hitTestHeight'
      &setMember
      &pushsdb 34							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 14							//'FSCommand:'
      &pushsdb 45							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 16							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
     label10:
    &end // of frame 0
  &end // of defineMovieClip 99
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 100
  &end // of importAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImage' &as 101
  &end // of importAssets

  &defineMovieClip 104 // total frames: 1
  &end // of defineMovieClip 104

  &defineMovieClip 105 // total frames: 2

    &placeMovieClip 101 
    
      &onClipEvent &construct
        &constants '_type', 'RenderImage', '_imageMap', '', '_mode'  
        &pushsdb 0							//'_type'
        &pushsdb 1							//'RenderImage'
        &setVariable
        &pushsdb 2							//'_imageMap'
        &pushsdb 3							//''
        &setVariable
        &pushsdb 4							//'_mode'
        &pushsdb 3							//''
        &setVariable
      &end
    
      &onClipEvent &load
        &constants '_imageMap', 'Portrait', '_parent', 'qualifyName'  
        &pushsdb 0							//'_imageMap'
        &pushsdb 1							//'Portrait'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &dcallfsv 3							// qualifyName()
      &end
    &end // of placeMovieClip 101
  &end // of defineMovieClip 105

  &defineMovieClip 107 // total frames: 1
  &end // of defineMovieClip 107
  
  &exportAssets
    75 &as 'GadgetFlash'
  &end // of exportAssets

  &defineMovieClip 108 // total frames: 1

    &placeMovieClip 99 &as 'holdPositionButton'
    
      &onClipEvent &construct
        &pushs 'hitTestWidth'
        &pushbyte 33
        &setVariable
        &pushs 'hitTestHeight'
        &pushbyte 33
        &setVariable
        &pushs 'buttonContainer'
        &pushssv 'HoldPositionButton'
        &pushs 'helpSiteRef'
        &pushssv '_parent.buttonHelpSite'
      &end
    &end // of placeMovieClip 99

    &placeMovieClip 99 &as 'holdObjectButton'
    
      &onClipEvent &construct
        &pushs 'hitTestWidth'
        &pushbyte 33
        &setVariable
        &pushs 'hitTestHeight'
        &pushbyte 33
        &setVariable
        &pushs 'buttonContainer'
        &pushssv 'HoldObjectButton'
        &pushs 'helpSiteRef'
        &pushssv '_parent.buttonHelpSite'
      &end
    &end // of placeMovieClip 99

    &placeMovieClip 99 &as 'assaultButton'
    
      &onClipEvent &construct
        &pushs 'hitTestWidth'
        &pushbyte 33
        &setVariable
        &pushs 'hitTestHeight'
        &pushbyte 33
        &setVariable
        &pushs 'buttonContainer'
        &pushssv 'AssaultButton'
        &pushs 'helpSiteRef'
        &pushssv '_parent.buttonHelpSite'
      &end
    &end // of placeMovieClip 99

    &placeMovieClip 99 &as 'normalButton'
    
      &onClipEvent &construct
        &pushs 'hitTestWidth'
        &pushbyte 33
        &setVariable
        &pushs 'hitTestHeight'
        &pushbyte 33
        &setVariable
        &pushs 'buttonContainer'
        &pushssv 'NormalButton'
        &pushs 'helpSiteRef'
        &pushssv '_parent.buttonHelpSite'
      &end
    &end // of placeMovieClip 99

    &placeMovieClip 100 &as 'buttonHelpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'left'
      &end
    &end // of placeMovieClip 100
  &end // of defineMovieClip 108

  &frame 9
    &stop
  &end // of frame 9

  &frame 10
    &pushsgv 'itemStage'
    &pushs '_visible'
    &pushtrue
    &setMember
  &end // of frame 10

  &frame 12
    &pushs 'FSCommand:'
    &pushs 'OnOpened'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushs 'qualifyName'
    &callMethod
    &concat
    &pushs ''
    &getURL2
  &end // of frame 12

  &frame 21
    &stop
  &end // of frame 21

  &frame 24
    &pushs 'FSCommand:'
    &pushs 'OnClosed'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushs 'qualifyName'
    &callMethod
    &concat
    &pushs ''
    &getURL2
    &pushsgv 'itemStage'
    &pushs '_visible'
    &pushfalse
    &setMember
    &gotoLabel '_closed'
    &play
  &end // of frame 24
&end
