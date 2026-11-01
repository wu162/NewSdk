movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\HUDVoiceChatControlPanel\\HUDVoiceChatControlPanel.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'panel', '_visible', 'modeIndicator', 'Pointer', 'encode', 'talkIndicator', 'SetVisibility', 'GetModeIndicator', 'GetTalkIndicator', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function2 SetVisibility (r:2='visArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushsdbgv 0							//'panel'
      &pushsdb 1							//'_visible'
      &pushregister 1
      &setMember
    &end // of function SetVisibility

    &function GetModeIndicator (    )    
      &pushsdbgv 0							//'panel'
      &pushsdbgm 2							//'modeIndicator'
      &pushone
      &pushsdbgv 3							//'Pointer'
      &pushsdb 4							//'encode'
      &callMethod
      &return
    &end // of function GetModeIndicator

    &function GetTalkIndicator (    )    
      &pushsdbgv 0							//'panel'
      &pushsdbgm 5							//'talkIndicator'
      &pushone
      &pushsdbgv 3							//'Pointer'
      &pushsdb 4							//'encode'
      &callMethod
      &return
    &end // of function GetTalkIndicator

    &function onUnload (    )    
      &pushsdb 6							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 7							//'GetModeIndicator'
      &delete2
      &pop
      &pushsdb 8							//'GetTalkIndicator'
      &delete2
      &pop
      &pushsdb 9							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 10							//'initialized'
    &not
    &not
    &jnz label1    
    &pushthisgv
    &pushone
    &pushsdbgv 12							//'_parent'
    &dcallmp 13							// OnContentLoaded()
    &pushsdb 10							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 14							//'FSCommand:'
    &pushsdb 15							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 16							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 17							//''
    &getURL2
   label1:
    &stop
  &end // of frame 0

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &defineButton 8
  
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
  
    &on     &overDownToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnRelease'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnRollOver'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &pushs 'FSCommand:'
      &pushs 'OnRollOut'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 8
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 9
  &end // of importAssets

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 20

    &frame 0
      &constants '_off', '_on', 'helpSite', 'Pointer', 'encode', 'SetTalking', 'GetHelpSite', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 SetTalking (r:2='talkingArg') ()
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushregister 1
        &jnz label1        
        &pushsdb 0							//'_off'
        &jmp label2        
       label1:
        &pushsdb 1							//'_on'
       label2:
        &gotoAndPlay      &end // of function SetTalking

      &function GetHelpSite (      )      
        &pushsdbgv 2							//'helpSite'
        &pushone
        &pushsdbgv 3							//'Pointer'
        &pushsdb 4							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function onUnload (      )      
        &pushsdb 5							//'SetTalking'
        &delete2
        &pop
        &pushsdb 6							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label3      
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
     label3:
    &end // of frame 0

    &placeMovieClip 6 
    &end // of placeMovieClip 6

    &placeMovieClip 9 &as 'helpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'right'
      &end
    &end // of placeMovieClip 9

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 11

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineButton 16
  
    &on     &overDownToOverUp
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
      &pushs 'FSCommand:'
      &pushs 'OnRollOver'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &pushs 'FSCommand:'
      &pushs 'OnRollOut'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 16

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 20

    &frame 0
      &constants 'helpSite', 'Pointer', 'encode', '', 'SetMode', 'GetHelpSite', 'SetVisibility', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 SetMode (r:2='modeLabelArg') ()
        &pushregister 2
        &toString
        &setRegister r:1
        &pop
        &pushregister 1
        &gotoAndPlay      &end // of function SetMode

      &function GetHelpSite (      )      
        &pushsdbgv 0							//'helpSite'
        &pushone
        &pushsdbgv 1							//'Pointer'
        &pushsdb 2							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function2 SetVisibility (r:1='visArg') ()
        &pushsdb 3							//''
        &pushbyte 7
        &pushregister 1
        &toNumber
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function onUnload (      )      
        &pushsdb 4							//'SetMode'
        &delete2
        &pop
        &pushsdb 5							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 6							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label1      
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
      &pushsdb 3							//''
      &getURL2
     label1:
    &end // of frame 0

    &placeMovieClip 9 &as 'helpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'right'
      &end
    &end // of placeMovieClip 9

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21
&end
