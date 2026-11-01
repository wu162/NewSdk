movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA32BC~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'panel', '_visible', 'slider', 'playerSpinner', 'Pointer', 'encode', 'modeSpinner', 'SetVisibility', 'GetPlayerSpinner', 'GetModeSpinner', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
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

    &function GetPlayerSpinner (    )    
      &pushsdbgv 0							//'panel'
      &pushsdbgm 2							//'slider'
      &pushsdbgm 3							//'playerSpinner'
      &pushone
      &pushsdbgv 4							//'Pointer'
      &pushsdb 5							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerSpinner

    &function GetModeSpinner (    )    
      &pushsdbgv 0							//'panel'
      &pushsdbgm 2							//'slider'
      &pushsdbgm 6							//'modeSpinner'
      &pushone
      &pushsdbgv 4							//'Pointer'
      &pushsdb 5							//'encode'
      &callMethod
      &return
    &end // of function GetModeSpinner

    &function onUnload (    )    
      &pushsdb 7							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 8							//'GetPlayerSpinner'
      &delete2
      &pop
      &pushsdb 9							//'GetModeSpinner'
      &delete2
      &pop
      &pushsdb 10							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 11							//'initialized'
    &not
    &not
    &jnz label1    
    &pushthisgv
    &pushone
    &pushsdbgv 13							//'_parent'
    &dcallmp 14							// OnContentLoaded()
    &pushsdb 11							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 15							//'FSCommand:'
    &pushsdb 16							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 17							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 18							//''
    &getURL2
   label1:
    &stop
  &end // of frame 0

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineButton 20
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &gotoLabel '_over'
      &play
      &pushs 'FSCommand:'
      &pushs 'OnLeftButtonClicked'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 20

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 26 // total frames: 1
  &end // of defineMovieClip 26

  &defineMovieClip 27 // total frames: 40

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
  &end // of defineMovieClip 27

  &defineButton 28
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &gotoLabel '_over'
      &play
      &pushs 'FSCommand:'
      &pushs 'OnRightButtonClicked'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 28

  &defineMovieClip 29 // total frames: 40

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
  &end // of defineMovieClip 29

  &defineMovieClip 32 // total frames: 1
  &end // of defineMovieClip 32

  &defineMovieClip 33 // total frames: 1

    &frame 0
      &constants '_disabled', '_up', 'leftButton', 'gotoAndPlay', 'rightButton', 'SetLeftButtonEnabled', 'SetRightButtonEnabled', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 SetLeftButtonEnabled (r:2='enableArg') ()
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushregister 1
        &jnz label1        
        &pushsdb 0							//'_disabled'
        &jmp label2        
       label1:
        &pushsdb 1							//'_up'
       label2:
        &pushone
        &pushsdbgv 2							//'leftButton'
        &dcallmp 3							// gotoAndPlay()
      &end // of function SetLeftButtonEnabled

      &function2 SetRightButtonEnabled (r:2='enableArg') ()
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushregister 1
        &jnz label3        
        &pushsdb 0							//'_disabled'
        &jmp label4        
       label3:
        &pushsdb 1							//'_up'
       label4:
        &pushone
        &pushsdbgv 4							//'rightButton'
        &dcallmp 3							// gotoAndPlay()
      &end // of function SetRightButtonEnabled

      &function onUnload (      )      
        &pushsdb 5							//'SetLeftButtonEnabled'
        &delete2
        &pop
        &pushsdb 6							//'SetRightButtonEnabled'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label5      
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
     label5:
      &stop
    &end // of frame 0

    &placeMovieClip 32 
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'ItemText'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 32
  &end // of defineMovieClip 33

  &defineMovieClip 34 // total frames: 1
  &end // of defineMovieClip 34

  &defineMovieClip 37 // total frames: 1
  &end // of defineMovieClip 37

  &defineMovieClip 42 // total frames: 20

    &frame 0
      &stop
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 42

  &defineMovieClip 44 // total frames: 1
  &end // of defineMovieClip 44

  &defineButton 48
  
    &on     &overUpToOverDown
      &constants '_open', '_parent', 'gotoAndPlay', '_max', 'TitleFrame'  
      &gotoLabel '_maximize'
      &pushsdb 0							//'_open'
      &pushone
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 3							//'_max'
      &pushone
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 4							//'TitleFrame'
      &dcallmp 2							// gotoAndPlay()
    &end
  &end // of defineButton 48

  &defineButton 51
  
    &on     &overUpToOverDown
      &constants '_close', '_parent', 'gotoAndPlay', '_mini', 'TitleFrame'  
      &gotoLabel '_minimize'
      &pushsdb 0							//'_close'
      &pushone
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 3							//'_mini'
      &pushone
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 4							//'TitleFrame'
      &dcallmp 2							// gotoAndPlay()
    &end
  &end // of defineButton 51

  &defineMovieClip 52 // total frames: 20

    &frame 0
      &stop
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 52

  &defineMovieClip 53 // total frames: 1

    &placeMovieClip 44 
    
      &onClipEvent &load
        &constants 'text', '$', 'Title', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Title'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 44
  &end // of defineMovieClip 53

  &defineMovieClip 54 // total frames: 22

    &frame 0
      &stop
    &end // of frame 0

    &frame 11
      &stop
    &end // of frame 11

    &frame 21
      &stop
    &end // of frame 21
  &end // of defineMovieClip 54
&end
