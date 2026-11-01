movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA317F~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 10, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants '_x', '_y', '_parent', '', 'Move', 'Close', 'onUnload', 'initialized', 'this', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName'  
    &function2 Move (r:5='x', r:4='y') (r:1='_root', r:2='_parent')
      &pushregister 5
      &pushshort -5000
      &lessThan
      &not
      &jnz label1      
      &pushshort -5000
      &setRegister r:5
      &pop
      &jmp label2      
     label1:
      &pushregister 5
      &pushshort 5000
      &greaterThan
      &not
      &jnz label2      
      &pushshort 5000
      &setRegister r:5
      &pop
     label2:
      &pushregister 4
      &pushshort -5000
      &lessThan
      &not
      &jnz label3      
      &pushshort -5000
      &setRegister r:4
      &pop
      &jmp label4      
     label3:
      &pushregister 4
      &pushshort 5000
      &greaterThan
      &not
      &jnz label4      
      &pushshort 5000
      &setRegister r:4
      &pop
     label4:
      &pushregister 1
      &setRegister r:6
      &pop
      &pushregister 2
      &setRegister r:3
      &pop
     label5:
      &pushregister 3
      &pushregister 6
      &equals
      &not
      &not
      &jnz label6      
      &pushregister 5
      &pushregister 3
      &pushsdbgm 0							//'_x'
      &subtract
      &setRegister r:5
      &pop
      &pushregister 4
      &pushregister 3
      &pushsdbgm 1							//'_y'
      &subtract
      &setRegister r:4
      &pop
      &pushregister 3
      &pushsdbgm 2							//'_parent'
      &setRegister r:3
      &pop
      &jmp label5      
     label6:
      &pushsdb 3							//''
      &pushzero
      &pushregister 5
      &setProperty
      &pushsdb 3							//''
      &pushone
      &pushregister 4
      &setProperty
    &end // of function Move

    &function Close (    )    
      &gotoLabel '_close'
      &play
    &end // of function Close

    &function onUnload (    )    
      &pushsdb 4							//'Move'
      &delete2
      &pop
      &pushsdb 5							//'Close'
      &delete2
      &pop
      &pushsdb 6							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 7							//'initialized'
    &not
    &not
    &jnz label7    
    &pushthisgv
    &pushone
    &pushsdbgv 2							//'_parent'
    &dcallmp 9							// OnContentLoaded()
    &pushsdb 7							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 10							//'FSCommand:'
    &pushsdb 11							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 12							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 3							//''
    &getURL2
   label7:
  &end // of frame 0

  &defineButton 4
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &constants 'FSCommand:', 'OnExecuteClicked', '_parent', 'qualifyName', ''  
      &gotoLabel '_over'
      &play
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnExecuteClicked'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 2							//'_parent'
      &pushbyte 2
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
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
  &end // of defineButton 4

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 13 // total frames: 15

    &placeMovieClip 9 
    
      &onClipEvent &load
        &constants 'myText', '$', 'ExecuteButton', '_parent', 'qualifyName'  
        &pushsdb 0							//'myText'
        &pushsdb 1							//'$'
        &pushsdb 2							//'ExecuteButton'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 9

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14
  &end // of defineMovieClip 13

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 19 // total frames: 1

    &placeMovieClip 16 
    
      &onClipEvent &load
        &constants 'myText', '$', 'Description', '_parent', 'qualifyName', '&dropshadow'  
        &pushsdb 0							//'myText'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Description'
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
    &end // of placeMovieClip 16

    &placeMovieClip 18 
    
      &onClipEvent &load
        &constants 'myText', '$', 'Title', '_parent', 'qualifyName', '&dropshadow'  
        &pushsdb 0							//'myText'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Title'
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
    &end // of placeMovieClip 18
  &end // of defineMovieClip 19

  &frame 4
    &stop
  &end // of frame 4

  &frame 9
    &stop
    &pushs 'FSCommand:'
    &pushs 'OnClosed'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
  &end // of frame 9
&end
