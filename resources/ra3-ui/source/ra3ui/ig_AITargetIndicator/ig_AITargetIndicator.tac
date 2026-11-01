movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_AIT~1\\IG_AIT~1.eaf' &compressed // flash 7, total frames: 30, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants '_x', '_y', '_parent', '', 'Move', 'SetDisposition', 'onUnload', 'initialized', 'this', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName'  
    &function2 Move (r:6='x', r:5='y') (r:1='_root', r:2='_parent')
      &pushregister 1
      &setRegister r:4
      &pop
      &pushregister 2
      &setRegister r:3
      &pop
     label1:
      &pushregister 3
      &pushregister 4
      &equals
      &not
      &not
      &jnz label2      
      &pushregister 6
      &pushregister 3
      &pushsdbgm 0							//'_x'
      &subtract
      &setRegister r:6
      &pop
      &pushregister 5
      &pushregister 3
      &pushsdbgm 1							//'_y'
      &subtract
      &setRegister r:5
      &pop
      &pushregister 3
      &pushsdbgm 2							//'_parent'
      &setRegister r:3
      &pop
      &jmp label1      
     label2:
      &pushsdb 3							//''
      &pushzero
      &pushregister 6
      &setProperty
      &pushsdb 3							//''
      &pushone
      &pushregister 5
      &setProperty
    &end // of function Move

    &function2 SetDisposition (r:1='disposition') ()
      &pushregister 1
      &gotoAndPlay    &end // of function SetDisposition

    &function onUnload (    )    
      &pushsdb 4							//'Move'
      &delete2
      &pop
      &pushsdb 5							//'SetDisposition'
      &delete2
      &pop
      &pushsdb 6							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 7							//'initialized'
    &not
    &not
    &jnz label3    
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
   label3:
    &stop
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
&end
