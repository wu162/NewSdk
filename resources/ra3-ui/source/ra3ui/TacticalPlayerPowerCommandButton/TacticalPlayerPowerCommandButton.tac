movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA2ABD~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 10, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants '_up', 'bttn', 'gotoAndPlay', 'isMouseOver', '_hide', 'RollOver', 'RollOverTop', '_disabled', 'isEnabled', '_visible', 'timerVisible', 'timerOverlay0', 'timerOverlay1', 'Math', 'round', 'gotoAndStop', 'imageTint', 'image', 'colorizer', 'setRGB', 'quantityText', 'TacticalCommandButton.swf SetHighlighted( ', ' )', 'highlightClip', 'CreateHUDGadgetFlash', 'Close', 'SetEnabled', 'SetVisibility', 'SetTimerVisibility', 'SetTimerValue', 'SetImageTint', 'SetQuantityVisibility', 'onUnload', 'extern', 'InGame', '_root', 'GetCurrentFaction', 'Alien', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
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
      &pushsdb 0							//'_up'
      &pushone
      &pushsdbgv 1							//'bttn'
      &dcallmp 2							// gotoAndPlay()
      &jmp label3      
     label1:
      &pushsdbgv 3							//'isMouseOver'
      &not
      &jnz label2      
      &pushsdb 4							//'_hide'
      &pushone
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 5							//'RollOver'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 4							//'_hide'
      &pushone
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 6							//'RollOverTop'
      &dcallmp 2							// gotoAndPlay()
     label2:
      &pushsdb 7							//'_disabled'
      &pushone
      &pushsdbgv 1							//'bttn'
      &dcallmp 2							// gotoAndPlay()
     label3:
      &pushsdb 8							//'isEnabled'
      &pushregister 1
      &setVariable
    &end // of function SetEnabled

    &function2 SetVisibility (r:1='visArg') ()
      &pushsdbgv 1							//'bttn'
      &pushsdb 9							//'_visible'
      &pushregister 1
      &toNumber
      &pushzero
      &equals
      &not
      &setMember
    &end // of function SetVisibility

    &function2 SetTimerVisibility (r:1='vis') ()
      &pushsdb 10							//'timerVisible'
      &pushregister 1
      &toNumber
      &pushzero
      &equals
      &not
      &setVariable
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 11							//'timerOverlay0'
      &pushsdb 9							//'_visible'
      &pushsdbgv 10							//'timerVisible'
      &setMember
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 12							//'timerOverlay1'
      &pushsdb 9							//'_visible'
      &pushsdbgv 10							//'timerVisible'
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
      &pushsdbgv 13							//'Math'
      &pushsdb 14							//'round'
      &callMethod
      &pushone
      &add
      &setRegister r:1
      &pop
      &pushregister 1
      &pushone
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 11							//'timerOverlay0'
      &dcallmp 15							// gotoAndStop()
      &pushregister 1
      &pushone
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 12							//'timerOverlay1'
      &dcallmp 15							// gotoAndStop()
    &end // of function SetTimerValue

    &function2 SetImageTint (r:2='tintColorArg') ()
      &pushregister 2
      &toNumber
      &setRegister r:1
      &pop
      &pushsdb 16							//'imageTint'
      &pushregister 1
      &setVariable
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 17							//'image'
      &pushsdbgm 18							//'colorizer'
      &pushundef
      &equals
      &not
      &not
      &jnz label4      
      &pushsdbgv 16							//'imageTint'
      &pushone
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 17							//'image'
      &pushsdbgm 18							//'colorizer'
      &dcallmp 19							// setRGB()
     label4:
    &end // of function SetImageTint

    &function2 SetQuantityVisibility (r:2='visArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushsdbgv 1							//'bttn'
      &pushsdbgm 20							//'quantityText'
      &pushsdb 9							//'_visible'
      &pushregister 1
      &setMember
    &end // of function SetQuantityVisibility

    &function2 SetHighlighted (r:4='highlightArg') (r:1='this', r:2='_global')
      &pushsdb 21							//'TacticalCommandButton.swf SetHighlighted( '
      &pushregister 4
      &toString
      &add
      &pushsdb 22							//' )'
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
      &jnz label6      
      &pushsdbgv 23							//'highlightClip'
      &pushundef
      &equals
      &not
      &jnz label5      
      &pushsdb 23							//'highlightClip'
      &pushregister 1
      &pushone
      &pushregister 2
      &dcallmsv 24							// CreateHUDGadgetFlash()
     label5:
      &jmp label7      
     label6:
      &pushsdbgv 23							//'highlightClip'
      &pushundef
      &equals
      &not
      &not
      &jnz label7      
      &pushzero
      &pushsdbgv 23							//'highlightClip'
      &dcallmp 25							// Close()
      &pushsdb 23							//'highlightClip'
      &pushundef
      &setVariable
     label7:
    &end // of function SetHighlighted

    &function onUnload (    )    
      &pushzero
      &pushsdbgv 23							//'highlightClip'
      &dcallmp 25							// Close()
      &pushsdb 26							//'SetEnabled'
      &delete2
      &pop
      &pushsdb 27							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 28							//'SetTimerVisibility'
      &delete2
      &pop
      &pushsdb 29							//'SetTimerValue'
      &delete2
      &pop
      &pushsdb 30							//'SetImageTint'
      &delete2
      &pop
      &pushsdb 31							//'SetQuantityVisibility'
      &delete2
      &pop
      &pushsdb 32							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 33							//'extern'
    &pushsdbgm 34							//'InGame'
    &not
    &not
    &jnz label8    
    &pushsdbgv 35							//'_root'
    &pushsdb 36							//'GetCurrentFaction'
    &function  (    )    
      &pushsdb 37							//'Alien'
      &return
    &end // of function 

    &setMember
   label8:
    &pushsdbgv 38							//'initialized'
    &not
    &not
    &jnz label9    
    &pushthisgv
    &pushone
    &pushsdbgv 40							//'_parent'
    &dcallmp 41							// OnContentLoaded()
    &pushsdb 8							//'isEnabled'
    &pushfalse
    &setVariable
    &pushsdb 3							//'isMouseOver'
    &pushfalse
    &setVariable
    &pushsdb 10							//'timerVisible'
    &pushfalse
    &setVariable
    &pushsdb 16							//'imageTint'
    &pushlong 16777215
    &setVariable
    &pushsdb 38							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 42							//'FSCommand:'
    &pushsdb 43							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 44							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 45							//''
    &getURL2
    &pushsdbgv 33							//'extern'
    &pushsdbgm 34							//'InGame'
    &not
    &not
    &jnz label9    
    &pushsdb 8							//'isEnabled'
    &pushtrue
    &setVariable
   label9:
  &end // of frame 0

  &defineButton 2
  
    &on     &overUpToOverDown
      &pushsgv '_parent'
      &pushsgm 'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_down'
      &play
     label1:
    &end
  
    &on     &overDownToOverUp
      &constants '_parent', 'isEnabled', 'FSCommand:', 'OnClicked', 'qualifyName', ''  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 1							//'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_release'
      &play
     label1:
      &pushsdb 2							//'FSCommand:'
      &pushsdb 3							//'OnClicked'
      &pushsdbgv 0							//'_parent'
      &pushbyte 2
      &pushsdb 4							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 5							//''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &constants '_parent', 'isEnabled', '_show', 'RollOver', 'gotoAndPlay', 'RollOverTop', 'isMouseOver', 'FSCommand:', 'OnRollOver', 'qualifyName', ''  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 1							//'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_over'
      &play
      &pushsdb 2							//'_show'
      &pushone
      &pushsdbgv 3							//'RollOver'
      &dcallmp 4							// gotoAndPlay()
      &pushsdb 2							//'_show'
      &pushone
      &pushsdbgv 5							//'RollOverTop'
      &dcallmp 4							// gotoAndPlay()
     label1:
      &pushsdbgv 0							//'_parent'
      &pushsdb 6							//'isMouseOver'
      &pushtrue
      &setMember
      &pushsdb 7							//'FSCommand:'
      &pushsdb 8							//'OnRollOver'
      &pushsdbgv 0							//'_parent'
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 10							//''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &constants '_parent', 'isEnabled', '_hide', 'RollOver', 'gotoAndPlay', 'RollOverTop', 'isMouseOver', 'FSCommand:', 'OnRollOut', 'qualifyName', ''  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 1							//'isEnabled'
      &not
      &jnz label1      
      &gotoLabel '_up'
      &play
      &pushsdb 2							//'_hide'
      &pushone
      &pushsdbgv 3							//'RollOver'
      &dcallmp 4							// gotoAndPlay()
      &pushsdb 2							//'_hide'
      &pushone
      &pushsdbgv 5							//'RollOverTop'
      &dcallmp 4							// gotoAndPlay()
     label1:
      &pushsdbgv 0							//'_parent'
      &pushsdb 6							//'isMouseOver'
      &pushfalse
      &setMember
      &pushsdb 7							//'FSCommand:'
      &pushsdb 8							//'OnRollOut'
      &pushsdbgv 0							//'_parent'
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 10							//''
      &getURL2
    &end
  &end // of defineButton 2

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 93 // total frames: 101

    &frame 0
      &constants '', '_parent', 'timerVisible'  
      &pushsdb 0							//''
      &pushbyte 7
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'timerVisible'
      &setProperty
      &stop
    &end // of frame 0
  &end // of defineMovieClip 93
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImageDisabled' &as 94
  &end // of importAssets

  &defineMovieClip 97 // total frames: 1
  &end // of defineMovieClip 97

  &defineMovieClip 98 // total frames: 23

    &frame 4
      &stop
    &end // of frame 4

    &frame 12
      &stop
    &end // of frame 12

    &frame 22
      &stop
    &end // of frame 22
  &end // of defineMovieClip 98

  &defineMovieClip 101 // total frames: 1
  &end // of defineMovieClip 101

  &defineMovieClip 108 // total frames: 30

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
  &end // of defineMovieClip 108
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImage' &as 109
  &end // of importAssets

  &defineMovieClip 110 // total frames: 50

    &placeMovieClip 94 &as 'disabledImage'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImageDisabled'
        &pushs '_imageMap'
        &pushssv ''
      &end
    
      &onClipEvent &load
        &constants '_imageMap', 'Image', '_parent', 'qualifyName'  
        &pushsdb 0							//'_imageMap'
        &pushsdb 1							//'Image'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &dcallfsv 3							// qualifyName()
      &end
    &end // of placeMovieClip 94

    &placeMovieClip 101 &as 'quantityText'
    
      &onClipEvent &load
        &constants 'text', '$', 'QuantityText', '_parent', 'qualifyName', '&dropshadow'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'QuantityText'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &pushsdb 5							//'&dropshadow'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 101

    &frame 8
      &stop
    &end // of frame 8

    &placeMovieClip 109 &as 'image'
    
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
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &dcallfsv 3							// qualifyName()
        &pushsdb 4							//'colorizer'
        &pushthisgv
        &pushone
        &pushsdb 6							//'Color'
        &new
        &setVariable
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushsdbgm 7							//'imageTint'
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushsdbgm 7							//'imageTint'
        &pushone
        &pushsdbgv 4							//'colorizer'
        &dcallmp 8							// setRGB()
       label1:
      &end
    &end // of placeMovieClip 109

    &frame 18
      &stop
    &end // of frame 18

    &frame 28
      &stop
    &end // of frame 28

    &frame 38
      &stop
    &end // of frame 38

    &frame 49
      &gotoLabel '_over'
      &play
    &end // of frame 49
  &end // of defineMovieClip 110

  &placeMovieClip 110 &as 'bttn'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 110

  &frame 9
    &pushsgv 'bttn'
    &pushs '_visible'
    &pushtrue
    &setMember
  &end // of frame 9
&end
