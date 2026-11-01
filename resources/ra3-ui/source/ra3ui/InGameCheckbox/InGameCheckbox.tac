movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\INGAME~2\\INGAME~1.eaf' &compressed // flash 7, total frames: 20, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'isEnabled', '_disabled', 'isOver', '_up', '_over', 'box', 'gotoAndPlay', '_unchecked', '_checked', '', 'FSCommand:', 'OnClicked', 'qualifyName', 'OnRollOver', 'OnRollOut', 'SetEnabled', 'SetChecked', 'SetVisibility', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'client', 'OnInitialized'  
    &function2 SetEnabled (r:2='enableArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 0							//'isEnabled'
      &equals
      &not
      &not
      &jnz label4      
      &pushsdb 0							//'isEnabled'
      &pushregister 1
      &setVariable
      &pushregister 1
      &jnz label1      
      &pushsdb 1							//'_disabled'
      &jmp label3      
     label1:
      &pushsdbgv 2							//'isOver'
      &jnz label2      
      &pushsdb 3							//'_up'
      &jmp label3      
     label2:
      &pushsdb 4							//'_over'
     label3:
      &pushone
      &pushsdbgv 5							//'box'
      &dcallmp 6							// gotoAndPlay()
     label4:
    &end // of function SetEnabled

    &function2 SetChecked (r:2='checkedArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushregister 1
      &jnz label5      
      &pushsdb 7							//'_unchecked'
      &jmp label6      
     label5:
      &pushsdb 8							//'_checked'
     label6:
      &gotoAndPlay    &end // of function SetChecked

    &function2 SetVisibility (r:2='visArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushsdb 9							//''
      &pushbyte 7
      &pushregister 1
      &setProperty
    &end // of function SetVisibility

    &function2 OnClicked ('clip') (r:1='this')
      &pushsdb 10							//'FSCommand:'
      &pushsdb 11							//'OnClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 9							//''
      &getURL2
    &end // of function OnClicked

    &function2 OnRollOver ('clip') (r:1='this')
      &pushsdb 10							//'FSCommand:'
      &pushsdb 13							//'OnRollOver'
      &pushregister 1
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 9							//''
      &getURL2
    &end // of function OnRollOver

    &function2 OnRollOut ('clip') (r:1='this')
      &pushsdb 10							//'FSCommand:'
      &pushsdb 14							//'OnRollOut'
      &pushregister 1
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 9							//''
      &getURL2
    &end // of function OnRollOut

    &function onUnload (    )    
      &pushsdb 15							//'SetEnabled'
      &delete2
      &pop
      &pushsdb 16							//'SetChecked'
      &delete2
      &pop
      &pushsdb 17							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 11							//'OnClicked'
      &delete2
      &pop
      &pushsdb 13							//'OnRollOver'
      &delete2
      &pop
      &pushsdb 14							//'OnRollOut'
      &delete2
      &pop
      &pushsdb 18							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 19							//'initialized'
    &not
    &not
    &jnz label8    
    &pushthisgv
    &pushone
    &pushsdbgv 21							//'_parent'
    &dcallmp 22							// OnContentLoaded()
    &pushsdb 0							//'isEnabled'
    &pushtrue
    &setVariable
    &pushsdb 2							//'isOver'
    &pushfalse
    &setVariable
    &pushsdbgv 23							//'client'
    &pushundef
    &equals
    &not
    &jnz label7    
    &pushsdb 23							//'client'
    &pushthisgv
    &setVariable
   label7:
    &pushsdb 19							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 10							//'FSCommand:'
    &pushsdb 24							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 12							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 9							//''
    &getURL2
   label8:
  &end // of frame 0

  &defineButton 2
  
    &on     &overDownToOverUp
      &pushthisgv
      &pushone
      &pushsgv 'client'
      &pushs 'OnClicked'
      &callmp
    &end
  
    &on     &idleToOverUp
      &constants 'isOver', 'isEnabled', '_over', 'box', 'gotoAndPlay', 'this', 'client', 'OnRollOver'  
      &pushsdbgv 0							//'isOver'
      &not
      &not
      &jnz label2      
      &pushsdb 0							//'isOver'
      &pushtrue
      &setVariable
      &pushsdbgv 1							//'isEnabled'
      &not
      &jnz label1      
      &pushsdb 2							//'_over'
      &pushone
      &pushsdbgv 3							//'box'
      &dcallmp 4							// gotoAndPlay()
     label1:
      &pushthisgv
      &pushone
      &pushsdbgv 6							//'client'
      &dcallmp 7							// OnRollOver()
     label2:
    &end
  
    &on     &overUpToIdle
      &constants 'isOver', 'isEnabled', '_up', 'box', 'gotoAndPlay', 'this', 'client', 'OnRollOut'  
      &pushsdbgv 0							//'isOver'
      &not
      &jnz label2      
      &pushsdb 0							//'isOver'
      &pushfalse
      &setVariable
      &pushsdbgv 1							//'isEnabled'
      &not
      &jnz label1      
      &pushsdb 2							//'_up'
      &pushone
      &pushsdbgv 3							//'box'
      &dcallmp 4							// gotoAndPlay()
     label1:
      &pushthisgv
      &pushone
      &pushsdbgv 6							//'client'
      &dcallmp 7							// OnRollOut()
     label2:
    &end
  &end // of defineButton 2

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 12 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 12

  &placeMovieClip 12 &as 'box'
  
    &onClipEvent &load
      &constants '_parent', 'isEnabled', '_disabled', 'isOver', '_up', '_over'  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 1							//'isEnabled'
      &jnz label1      
      &pushsdb 2							//'_disabled'
      &jmp label3      
     label1:
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 3							//'isOver'
      &jnz label2      
      &pushsdb 4							//'_up'
      &jmp label3      
     label2:
      &pushsdb 5							//'_over'
     label3:
      &gotoAndPlay    &end
  &end // of placeMovieClip 12

  &frame 9
    &stop
  &end // of frame 9

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &defineMovieClip 22 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 22

  &placeMovieClip 22 &as 'box'
  
    &onClipEvent &load
      &constants '_parent', 'isEnabled', '_disabled', 'isOver', '_up', '_over'  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 1							//'isEnabled'
      &jnz label1      
      &pushsdb 2							//'_disabled'
      &jmp label3      
     label1:
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 3							//'isOver'
      &jnz label2      
      &pushsdb 4							//'_up'
      &jmp label3      
     label2:
      &pushsdb 5							//'_over'
     label3:
      &gotoAndPlay    &end
  &end // of placeMovieClip 22

  &frame 19
    &stop
  &end // of frame 19
&end
