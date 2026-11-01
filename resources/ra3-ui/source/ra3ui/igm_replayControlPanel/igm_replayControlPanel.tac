movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\igm_replayControlPanel\\igm_replayControlPanel.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'ffButtonMC', 'Pointer', 'encode', '', 'GetFastForwardButton', 'SetVisibility', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', '_global', 'qualifyName'  
    &function GetFastForwardButton (    )    
      &pushsdbgv 0							//'ffButtonMC'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetFastForwardButton

    &function2 SetVisibility (r:1='visArg') ()
      &pushsdb 3							//''
      &pushbyte 7
      &pushregister 1
      &pushzero
      &equals
      &not
      &setProperty
    &end // of function SetVisibility

    &function onUnload (    )    
      &pushsdb 4							//'GetFastForwardButton'
      &delete2
      &pop
      &pushsdb 5							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 6							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 7							//'initialized'
    &not
    &not
    &jnz label1    
    &pushthisgv
    &pushone
    &pushsdbgv 9							//'_parent'
    &dcallmp 10							// OnContentLoaded()
    &pushsdb 7							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 11							//'FSCommand:'
    &pushsdb 12							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 14							//'qualifyName'
    &callMethod
    &concat
    &pushsdb 3							//''
    &getURL2
   label1:
  &end // of frame 0

  &defineMovieClip 7 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 7
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 8
  &end // of importAssets

  &defineMovieClip 11 // total frames: 30

    &frame 0
      &constants 'FSCommand:', 'OnClicked', 'qualifyName', '', 'OnRollOver', 'OnRollOut', 'normal', 'selected', 'imageMC', 'gotoAndPlay', 'helpSiteMC', 'Pointer', 'encode', 'OnPress', 'OnRelease', 'SetSelected', 'GetHelpSite', 'onUnload', 'initialized', 'hitAreaButton', 'onPress', 'this', '_global', 'bind0', 'onRelease', 'onRollOver', 'onRollOut', 'OnInitialized'  
      &function OnPress (      )      
        &gotoLabel 'down'
        &play
      &end // of function OnPress

      &function2 OnRelease () (r:1='this', r:2='_global')
        &gotoLabel 'over'
        &play
        &pushsdb 0							//'FSCommand:'
        &pushsdb 1							//'OnClicked'
        &pushregister 1
        &pushbyte 2
        &pushregister 2
        &pushsdb 2							//'qualifyName'
        &callMethod
        &concat
        &pushsdb 3							//''
        &getURL2
      &end // of function OnRelease

      &function2 OnRollOver () (r:1='this', r:2='_global')
        &gotoLabel 'over'
        &play
        &pushsdb 0							//'FSCommand:'
        &pushsdb 4							//'OnRollOver'
        &pushregister 1
        &pushbyte 2
        &pushregister 2
        &pushsdb 2							//'qualifyName'
        &callMethod
        &concat
        &pushsdb 3							//''
        &getURL2
      &end // of function OnRollOver

      &function2 OnRollOut () (r:1='this', r:2='_global')
        &gotoLabel 'up'
        &play
        &pushsdb 0							//'FSCommand:'
        &pushsdb 5							//'OnRollOut'
        &pushregister 1
        &pushbyte 2
        &pushregister 2
        &pushsdb 2							//'qualifyName'
        &callMethod
        &concat
        &pushsdb 3							//''
        &getURL2
      &end // of function OnRollOut

      &function2 SetSelected (r:1='selectedArg') ()
        &pushregister 1
        &pushzero
        &equals
        &not
        &jnz label1        
        &pushsdb 6							//'normal'
        &jmp label2        
       label1:
        &pushsdb 7							//'selected'
       label2:
        &pushone
        &pushsdbgv 8							//'imageMC'
        &dcallmp 9							// gotoAndPlay()
      &end // of function SetSelected

      &function GetHelpSite (      )      
        &pushsdbgv 10							//'helpSiteMC'
        &pushone
        &pushsdbgv 11							//'Pointer'
        &pushsdb 12							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function onUnload (      )      
        &pushsdb 13							//'OnPress'
        &delete2
        &pop
        &pushsdb 14							//'OnRelease'
        &delete2
        &pop
        &pushsdb 4							//'OnRollOver'
        &delete2
        &pop
        &pushsdb 5							//'OnRollOut'
        &delete2
        &pop
        &pushsdb 15							//'SetSelected'
        &delete2
        &pop
        &pushsdb 16							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 17							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 18							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdbgv 19							//'hitAreaButton'
      &pushsdb 20							//'onPress'
      &pushsdbgv 13							//'OnPress'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 23							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 19							//'hitAreaButton'
      &pushsdb 24							//'onRelease'
      &pushsdbgv 14							//'OnRelease'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 23							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 19							//'hitAreaButton'
      &pushsdb 25							//'onRollOver'
      &pushsdbgv 4							//'OnRollOver'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 23							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 19							//'hitAreaButton'
      &pushsdb 26							//'onRollOut'
      &pushsdbgv 5							//'OnRollOut'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 23							//'bind0'
      &callMethod
      &setMember
      &pushsdb 18							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 0							//'FSCommand:'
      &pushsdb 27							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 2							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 3							//''
      &getURL2
     label3:
    &end // of frame 0

    &placeMovieClip 8 &as 'helpSiteMC'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'right'
      &end
    &end // of placeMovieClip 8

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 11
&end
