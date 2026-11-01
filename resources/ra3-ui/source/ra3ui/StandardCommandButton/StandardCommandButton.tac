movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\STANDA~1\\STANDA~1.eaf' &compressed // flash 7, total frames: 50, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'isMouseOver', '_hide', 'RollOver', 'gotoAndPlay', 'RollOverTop', 'isEnabled', '', 'numberText', '_visible', 'isSelected', 'IS_SELECTED:  ', 'selectedOverlay', 'Show', 'Hide', 'StandardCommandButton.swf SetHighlighted( ', ' )', 'highlightClip', 'CreateHUDGadgetFlash', 'Close', 'timerVisible', 'timerOverlay0', 'timerOverlay1', 'Math', 'round', 'gotoAndStop', 'imageTint', 'image', 'colorizer', 'setRGB', 'healthBar', 'useAlternateColor', '_normal', '_alternate', 'bg', 'frame', 'amphibiousOverlay', 'infiltratedOverlay', 'SetEnabled', 'SetVisibility', 'SetNumberVisibility', 'SetSelected', 'SetTimerVisibility', 'SetTimerValue', 'SetImageTint', 'SetHealthBarVisibility', 'SetHealthBarValue', 'UseAlternateColors', 'SetAmphibious', 'SetInfiltrated', 'onUnload', 'extern', 'InGame', '_root', 'GetCurrentFaction', 'Gdi', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName'  
    &function2 SetEnabled (r:2='enableArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushregister 1
      &not
      &jnz label1      
      &gotoLabel '_up'
      &play
      &jmp label3      
     label1:
      &pushsdbgv 0							//'isMouseOver'
      &not
      &jnz label2      
      &pushsdb 1							//'_hide'
      &pushone
      &pushsdbgv 2							//'RollOver'
      &dcallmp 3							// gotoAndPlay()
      &pushsdb 1							//'_hide'
      &pushone
      &pushsdbgv 4							//'RollOverTop'
      &dcallmp 3							// gotoAndPlay()
     label2:
      &gotoLabel '_disabled'
      &play
     label3:
      &pushsdb 5							//'isEnabled'
      &pushregister 1
      &setVariable
    &end // of function SetEnabled

    &function2 SetVisibility (r:1='visArg') ()
      &pushsdb 6							//''
      &pushbyte 7
      &pushregister 1
      &toNumber
      &pushzero
      &equals
      &not
      &setProperty
    &end // of function SetVisibility

    &function2 SetNumberVisibility (r:1='visArg') ()
      &pushsdbgv 7							//'numberText'
      &pushsdb 8							//'_visible'
      &pushregister 1
      &toNumber
      &pushzero
      &equals
      &not
      &setMember
    &end // of function SetNumberVisibility

    &function2 SetSelected (r:2='selectArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushsdb 9							//'isSelected'
      &pushregister 1
      &setVariable
      &pushsdb 10							//'IS_SELECTED:  '
      &pushsdbgv 9							//'isSelected'
      &add
      &trace
      &pushsdbgv 9							//'isSelected'
      &not
      &jnz label4      
      &pushzero
      &pushsdbgv 11							//'selectedOverlay'
      &dcallmp 12							// Show()
      &jmp label5      
     label4:
      &pushzero
      &pushsdbgv 11							//'selectedOverlay'
      &dcallmp 13							// Hide()
     label5:
    &end // of function SetSelected

    &function2 SetHighlighted (r:4='highlightArg') (r:1='this', r:2='_global')
      &pushsdb 14							//'StandardCommandButton.swf SetHighlighted( '
      &pushregister 4
      &toString
      &add
      &pushsdb 15							//' )'
      &add
      &trace
      &pushregister 4
      &pushzero
      &equals
      &not
      &setRegister r:3
      &pop
      &pushregister 3
      &not
      &jnz label7      
      &pushsdbgv 16							//'highlightClip'
      &pushundef
      &equals
      &not
      &jnz label6      
      &pushsdb 16							//'highlightClip'
      &pushregister 1
      &pushone
      &pushregister 2
      &dcallmsv 17							// CreateHUDGadgetFlash()
     label6:
      &jmp label8      
     label7:
      &pushsdbgv 16							//'highlightClip'
      &pushundef
      &equals
      &not
      &not
      &jnz label8      
      &pushzero
      &pushsdbgv 16							//'highlightClip'
      &dcallmp 18							// Close()
      &pushsdb 16							//'highlightClip'
      &pushundef
      &setVariable
     label8:
    &end // of function SetHighlighted

    &function2 SetTimerVisibility (r:1='visArg') ()
      &pushsdb 19							//'timerVisible'
      &pushregister 1
      &toNumber
      &pushzero
      &equals
      &not
      &setVariable
      &pushsdbgv 20							//'timerOverlay0'
      &pushsdb 8							//'_visible'
      &pushsdbgv 19							//'timerVisible'
      &setMember
      &pushsdbgv 21							//'timerOverlay1'
      &pushsdb 8							//'_visible'
      &pushsdbgv 19							//'timerVisible'
      &setMember
    &end // of function SetTimerVisibility

    &function2 SetTimerValue (r:3='valArg') ()
      &pushregister 3
      &toNumber
      &setRegister r:2
      &pop
      &pushregister 2
      &pushbyte 100
      &multiply
      &pushone
      &pushsdbgv 22							//'Math'
      &pushsdb 23							//'round'
      &callMethod
      &pushone
      &add
      &setRegister r:1
      &pop
      &pushregister 1
      &pushone
      &pushsdbgv 20							//'timerOverlay0'
      &dcallmp 24							// gotoAndStop()
      &pushregister 1
      &pushone
      &pushsdbgv 21							//'timerOverlay1'
      &dcallmp 24							// gotoAndStop()
    &end // of function SetTimerValue

    &function2 SetImageTint (r:2='tintColorArg') ()
      &pushregister 2
      &toNumber
      &setRegister r:1
      &pop
      &pushsdb 25							//'imageTint'
      &pushregister 1
      &setVariable
      &pushsdbgv 26							//'image'
      &pushsdbgm 27							//'colorizer'
      &pushundef
      &equals
      &not
      &not
      &jnz label9      
      &pushsdbgv 25							//'imageTint'
      &pushone
      &pushsdbgv 26							//'image'
      &pushsdbgm 27							//'colorizer'
      &dcallmp 28							// setRGB()
     label9:
    &end // of function SetImageTint

    &function2 SetHealthBarVisibility (r:2='visArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushsdbgv 29							//'healthBar'
      &pushsdb 8							//'_visible'
      &pushregister 1
      &setMember
    &end // of function SetHealthBarVisibility

    &function2 SetHealthBarValue (r:2='valueArg') ()
      &pushregister 2
      &toNumber
      &setRegister r:1
      &pop
      &pushregister 1
      &pushbyte 100
      &multiply
      &pushone
      &add
      &pushone
      &pushsdbgv 29							//'healthBar'
      &dcallmp 24							// gotoAndStop()
    &end // of function SetHealthBarValue

    &function2 UseAlternateColors (r:2='arg') ()
      &pushsdb 30							//'useAlternateColor'
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setVariable
      &pushsdbgv 30							//'useAlternateColor'
      &jnz label10      
      &pushsdb 31							//'_normal'
      &jmp label11      
     label10:
      &pushsdb 32							//'_alternate'
     label11:
      &setRegister r:1
      &pop
      &pushregister 1
      &pushone
      &pushsdbgv 33							//'bg'
      &pushsdbgm 26							//'image'
      &dcallmp 3							// gotoAndPlay()
      &pushregister 1
      &pushone
      &pushsdbgv 34							//'frame'
      &pushsdbgm 26							//'image'
      &dcallmp 3							// gotoAndPlay()
    &end // of function UseAlternateColors

    &function2 SetAmphibious (r:1='arg') ()
      &pushsdbgv 35							//'amphibiousOverlay'
      &pushsdb 8							//'_visible'
      &pushregister 1
      &pushzero
      &equals
      &not
      &setMember
    &end // of function SetAmphibious

    &function2 SetInfiltrated (r:1='arg') ()
      &pushsdbgv 36							//'infiltratedOverlay'
      &pushsdb 8							//'_visible'
      &pushregister 1
      &pushzero
      &equals
      &not
      &setMember
    &end // of function SetInfiltrated

    &function onUnload (    )    
      &pushzero
      &pushsdbgv 16							//'highlightClip'
      &dcallmp 18							// Close()
      &pushsdb 37							//'SetEnabled'
      &delete2
      &pop
      &pushsdb 38							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 39							//'SetNumberVisibility'
      &delete2
      &pop
      &pushsdb 40							//'SetSelected'
      &delete2
      &pop
      &pushsdb 41							//'SetTimerVisibility'
      &delete2
      &pop
      &pushsdb 42							//'SetTimerValue'
      &delete2
      &pop
      &pushsdb 43							//'SetImageTint'
      &delete2
      &pop
      &pushsdb 44							//'SetHealthBarVisibility'
      &delete2
      &pop
      &pushsdb 45							//'SetHealthBarValue'
      &delete2
      &pop
      &pushsdb 46							//'UseAlternateColors'
      &delete2
      &pop
      &pushsdb 47							//'SetAmphibious'
      &delete2
      &pop
      &pushsdb 48							//'SetInfiltrated'
      &delete2
      &pop
      &pushsdb 49							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 50							//'extern'
    &pushsdbgm 51							//'InGame'
    &not
    &not
    &jnz label12    
    &pushsdbgv 52							//'_root'
    &pushsdb 53							//'GetCurrentFaction'
    &function  (    )    
      &pushsdb 54							//'Gdi'
      &return
    &end // of function 

    &setMember
    &gotoLabel '_up'
    &play
   label12:
    &pushsdbgv 55							//'initialized'
    &not
    &not
    &jnz label13    
    &pushthisgv
    &pushone
    &pushsdbgv 57							//'_parent'
    &dcallmp 58							// OnContentLoaded()
    &pushsdbgv 29							//'healthBar'
    &pushsdb 8							//'_visible'
    &pushfalse
    &setMember
    &pushsdb 30							//'useAlternateColor'
    &pushfalse
    &setVariable
    &pushsdb 5							//'isEnabled'
    &pushfalse
    &setVariable
    &pushsdb 9							//'isSelected'
    &pushfalse
    &setVariable
    &pushsdb 0							//'isMouseOver'
    &pushfalse
    &setVariable
    &pushsdb 19							//'timerVisible'
    &pushfalse
    &setVariable
    &pushsdbgv 7							//'numberText'
    &pushsdb 8							//'_visible'
    &pushfalse
    &setMember
    &pushsdb 25							//'imageTint'
    &pushlong 16777215
    &setVariable
    &pushbyte 101
    &pushone
    &pushsdbgv 29							//'healthBar'
    &dcallmp 24							// gotoAndStop()
    &pushsdbgv 35							//'amphibiousOverlay'
    &pushsdb 8							//'_visible'
    &pushfalse
    &setMember
    &pushsdbgv 36							//'infiltratedOverlay'
    &pushsdb 8							//'_visible'
    &pushfalse
    &setMember
    &pushsdb 55							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 59							//'FSCommand:'
    &pushsdb 60							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 61							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 6							//''
    &getURL2
    &pushsdbgv 50							//'extern'
    &pushsdbgm 51							//'InGame'
    &not
    &not
    &jnz label13    
    &pushsdb 5							//'isEnabled'
    &pushtrue
    &setVariable
    &pushone
    &pushone
    &dcallfp 44							// SetHealthBarVisibility()
    &pushbyte 100
    &random
    &pushbyte 100
    &divide
    &pushone
    &dcallfp 45							// SetHealthBarValue()
   label13:
  &end // of frame 0

  &defineButton 2
  
    &on     &overUpToOverDown
      &pushsgv 'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_down'
      &play
     label1:
    &end
  
    &on     &overDownToOverUp
      &pushsgv 'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_release'
      &play
     label1:
      &pushs 'FSCommand:'
      &pushs 'OnClicked'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &constants 'isEnabled', '_show', 'RollOver', 'gotoAndPlay', 'RollOverTop', 'isMouseOver', 'FSCommand:', 'OnRollOver', 'this', 'qualifyName', ''  
      &pushsdbgv 0							//'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_over'
      &play
      &pushsdb 1							//'_show'
      &pushone
      &pushsdbgv 2							//'RollOver'
      &dcallmp 3							// gotoAndPlay()
      &pushsdb 1							//'_show'
      &pushone
      &pushsdbgv 4							//'RollOverTop'
      &dcallmp 3							// gotoAndPlay()
     label1:
      &pushsdb 5							//'isMouseOver'
      &pushtrue
      &setVariable
      &pushsdb 6							//'FSCommand:'
      &pushsdb 7							//'OnRollOver'
      &pushthisgv
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 10							//''
      &getURL2
    &end
  
    &on     &overUpToIdle    ,&overDownToOutDown    ,&overDownToIdle
      &constants 'isEnabled', '_hide', 'RollOver', 'gotoAndPlay', 'RollOverTop', 'isMouseOver', 'FSCommand:', 'OnRollOut', 'this', 'qualifyName', ''  
      &pushsdbgv 0							//'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_up'
      &play
      &pushsdb 1							//'_hide'
      &pushone
      &pushsdbgv 2							//'RollOver'
      &dcallmp 3							// gotoAndPlay()
      &pushsdb 1							//'_hide'
      &pushone
      &pushsdbgv 4							//'RollOverTop'
      &dcallmp 3							// gotoAndPlay()
     label1:
      &pushsdb 5							//'isMouseOver'
      &pushfalse
      &setVariable
      &pushsdb 6							//'FSCommand:'
      &pushsdb 7							//'OnRollOut'
      &pushthisgv
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 10							//''
      &getURL2
    &end
  &end // of defineButton 2

  &defineMovieClip 96 // total frames: 101

    &frame 0
      &pushs ''
      &pushbyte 7
      &pushsgv '_parent'
      &pushsgm 'timerVisible'
      &setProperty
      &stop
    &end // of frame 0
  &end // of defineMovieClip 96
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImageDisabled' &as 97
  &end // of importAssets

  &placeMovieClip 97 &as 'disabledImage'
  
    &onClipEvent &construct
      &pushs '_type'
      &pushssv 'RenderImageDisabled'
      &pushs '_imageMap'
      &pushssv ''
    &end
  
    &onClipEvent &load
      &pushs '_imageMap'
      &pushs 'Image'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callfsv
    &end
  &end // of placeMovieClip 97

  &defineMovieClip 100 // total frames: 1
  &end // of defineMovieClip 100

  &defineMovieClip 103 // total frames: 1
  &end // of defineMovieClip 103

  &defineMovieClip 110 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
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

      &pushsdbgv 3							//'initialized'
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
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 110

  &defineMovieClip 111 // total frames: 32

    &frame 0
      &constants 'onUnload', 'Show', 'Hide', '_parent', 'isSelected'  
      &function Show (      )      
        &gotoLabel '_show'
        &play
      &end // of function Show

      &function Hide (      )      
        &gotoLabel '_hide'
        &play
      &end // of function Hide

      &pushsdb 0							//'onUnload'
      &function  (      )      
        &pushsdb 1							//'Show'
        &delete2
        &pop
        &pushsdb 2							//'Hide'
        &delete2
        &pop
        &pushsdb 0							//'onUnload'
        &delete2
        &pop
      &end // of function 

      &setVariable
      &pushsdbgv 3							//'_parent'
      &pushsdbgm 4							//'isSelected'
      &not
      &jnz label1      
      &gotoLabel '_show'
      &play
      &jmp label2      
     label1:
      &stop
     label2:
    &end // of frame 0

    &placeMovieClip 110 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 110

    &frame 8
      &stop
    &end // of frame 8

    &frame 9
      &pushs '_snow'
      &pushbyte 5
      &random
      &add
      &pushone
      &pushsgv 'Snow'
      &pushs 'gotoAndPlay'
      &callmp
    &end // of frame 9

    &frame 20
      &stop
    &end // of frame 20

    &frame 31
      &gotoLabel '_hidden'
      &play
    &end // of frame 31
  &end // of defineMovieClip 111

  &defineMovieClip 115 // total frames: 1
  &end // of defineMovieClip 115

  &defineMovieClip 118 // total frames: 101

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 115 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 115
  &end // of defineMovieClip 118

  &defineMovieClip 121 // total frames: 1
  &end // of defineMovieClip 121

  &defineMovieClip 122 // total frames: 23

    &frame 4
      &stop
    &end // of frame 4

    &frame 12
      &stop
    &end // of frame 12

    &frame 22
      &stop
    &end // of frame 22
  &end // of defineMovieClip 122

  &defineMovieClip 124 // total frames: 1
  &end // of defineMovieClip 124

  &placeMovieClip 124 &as 'numberText'
  
    &onClipEvent &load
      &pushs 'text'
      &pushs '$'
      &pushs 'Number'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &add
      &pushs '&outline'
      &add
      &setVariable
    &end
  &end // of placeMovieClip 124

  &frame 9
    &stop
  &end // of frame 9
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImage' &as 125
  &end // of importAssets

  &placeMovieClip 125 &as 'image'
  
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
      &constants '_imageMap', 'Image', '_parent', 'qualifyName', 'colorizer', 'this', 'Color', 'imageTint', 'setRGB'  
      &pushsdb 0							//'_imageMap'
      &pushsdb 1							//'Image'
      &pushsdbgv 2							//'_parent'
      &pushbyte 2
      &dcallfsv 3							// qualifyName()
      &pushsdb 4							//'colorizer'
      &pushthisgv
      &pushone
      &pushsdb 6							//'Color'
      &new
      &setVariable
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 7							//'imageTint'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 7							//'imageTint'
      &pushone
      &pushsdbgv 4							//'colorizer'
      &dcallmp 8							// setRGB()
     label1:
    &end
  &end // of placeMovieClip 125

  &frame 19
    &stop
  &end // of frame 19

  &frame 28
    &stop
  &end // of frame 28

  &frame 29
    &pushs 'inDownState'
    &pushtrue
    &setVariable
  &end // of frame 29

  &frame 38
    &stop
  &end // of frame 38

  &frame 39
    &pushsgv 'extern'
    &pushsgm 'InGame'
    &not
    &not
    &jnz label1    
    &pushzero
    &pushsgv 'selectedOverlay'
    &pushs 'Show'
    &callmp
   label1:
  &end // of frame 39

  &frame 49
    &gotoLabel '_over'
    &play
  &end // of frame 49
&end
