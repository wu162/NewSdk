movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TACC40~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 1
  &end // of importAssets

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineButton 6
  
    &on     &overUpToOverDown
      &constants 'client', 'this', 'OnTimerPressed'  
      &pushsdbgv 0							//'client'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushthisgv
      &pushone
      &pushsdbgv 0							//'client'
      &dcallmp 2							// OnTimerPressed()
     label1:
    &end
  
    &on     &overDownToOverUp
      &constants 'client', 'this', 'OnTimerClicked'  
      &pushsdbgv 0							//'client'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushthisgv
      &pushone
      &pushsdbgv 0							//'client'
      &dcallmp 2							// OnTimerClicked()
     label1:
    &end
  
    &on     &idleToOverUp
      &constants 'mouseOverObserver', 'this', 'OnTimerRollOver', 'client'  
      &pushsdbgv 0							//'mouseOverObserver'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushthisgv
      &pushone
      &pushsdbgv 0							//'mouseOverObserver'
      &dcallmp 2							// OnTimerRollOver()
     label1:
      &pushsdbgv 3							//'client'
      &pushundef
      &equals
      &not
      &not
      &jnz label2      
      &pushthisgv
      &pushone
      &pushsdbgv 3							//'client'
      &dcallmp 2							// OnTimerRollOver()
     label2:
    &end
  
    &on     &overUpToIdle
      &constants 'mouseOverObserver', 'this', 'OnTimerRollOut', 'client'  
      &pushsdbgv 0							//'mouseOverObserver'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushthisgv
      &pushone
      &pushsdbgv 0							//'mouseOverObserver'
      &dcallmp 2							// OnTimerRollOut()
     label1:
      &pushsdbgv 3							//'client'
      &pushundef
      &equals
      &not
      &not
      &jnz label2      
      &pushthisgv
      &pushone
      &pushsdbgv 3							//'client'
      &dcallmp 2							// OnTimerRollOut()
     label2:
    &end
  &end // of defineButton 6

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 11 // total frames: 2

    &frame 0
      &gotoFrame 1
      &play
    &end // of frame 0

    &frame 1
      &stop
    &end // of frame 1
  &end // of defineMovieClip 11

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineMovieClip 16 // total frames: 12

    &frame 0
      &constants 'childStage', 'child', 'overChild', 'timeText', '$', 'Time', 'qualifyName', '&outline', 'timerText', 'gotoAndPlay', 'image', '_imageMap', 'Image', 'OnTimerRollOver', 'OnTimerRollOut', 'SetID', 'onEnterFrame', 'onUnload', 'initialized', 'innerFrameColorizer', 'innerFrame', 'Color', 'client', 'this', 'OnTimerInitialized'  
      &function2 OnTimerRollOver (r:1='timer') ()
        &pushregister 1
        &pushsdbgv 0							//'childStage'
        &pushsdbgm 1							//'child'
        &equals
        &not
        &jnz label1        
        &pushsdb 2							//'overChild'
        &pushtrue
        &setVariable
       label1:
      &end // of function OnTimerRollOver

      &function2 OnTimerRollOut (r:1='timer') ()
        &pushregister 1
        &pushsdbgv 0							//'childStage'
        &pushsdbgm 1							//'child'
        &equals
        &not
        &jnz label2        
        &pushsdb 2							//'overChild'
        &pushfalse
        &setVariable
       label2:
      &end // of function OnTimerRollOut

      &function2 SetID (r:1='id') ()
        &pushsdb 3							//'timeText'
        &pushsdb 4							//'$'
        &pushsdb 5							//'Time'
        &pushregister 1
        &pushbyte 2
        &pushsdb 6							//'qualifyName'
        &callFunction
        &add
        &pushsdb 7							//'&outline'
        &add
        &setVariable
        &pushone
        &pushone
        &pushsdbgv 8							//'timerText'
        &dcallmp 9							// gotoAndPlay()
        &pushsdbgv 10							//'image'
        &pushsdb 11							//'_imageMap'
        &pushsdb 12							//'Image'
        &pushregister 1
        &pushbyte 2
        &pushsdb 6							//'qualifyName'
        &callFunction
        &setMember
      &end // of function SetID

      &function onUnload (      )      
        &pushsdb 13							//'OnTimerRollOver'
        &delete2
        &pop
        &pushsdb 14							//'OnTimerRollOut'
        &delete2
        &pop
        &pushsdb 15							//'SetID'
        &delete2
        &pop
        &pushsdb 16							//'onEnterFrame'
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
      &pushsdb 2							//'overChild'
      &pushfalse
      &setVariable
      &pushsdb 19							//'innerFrameColorizer'
      &pushsdbgv 20							//'innerFrame'
      &pushone
      &pushsdb 21							//'Color'
      &new
      &setVariable
      &pushsdb 18							//'initialized'
      &pushtrue
      &setVariable
      &pushsdbgv 22							//'client'
      &pushundef
      &equals
      &not
      &not
      &jnz label3      
      &pushthisgv
      &pushone
      &pushsdbgv 22							//'client'
      &dcallmp 24							// OnTimerInitialized()
     label3:
    &end // of frame 0

    &frame 0
      &constants 'onEnterFrame', 'overChild'  
      &stop
      &pushsdb 0							//'onEnterFrame'
      &function  (      )      
        &pushsdbgv 1							//'overChild'
        &not
        &jnz label1        
        &pushsdb 0							//'onEnterFrame'
        &delete2
        &pop
        &gotoLabel '_down'
        &play
       label1:
      &end // of function 

      &setVariable
    &end // of frame 0

    &placeMovieClip 1 &as 'helpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'bottom'
        &pushs 'horzAlignment'
        &pushssv 'left'
      &end
    &end // of placeMovieClip 1

    &placeMovieClip 8 &as 'image'
    
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
    &end // of placeMovieClip 8

    &frame 6
      &constants 'onEnterFrame', 'overChild'  
      &stop
      &pushsdb 0							//'onEnterFrame'
      &function  (      )      
        &pushsdbgv 1							//'overChild'
        &not
        &not
        &jnz label1        
        &pushsdb 0							//'onEnterFrame'
        &delete2
        &pop
        &gotoLabel '_up'
        &play
       label1:
      &end // of function 

      &setVariable
    &end // of frame 6

    &frame 11
      &constants 'onEnterFrame', 'overChild'  
      &stop
      &pushsdb 0							//'onEnterFrame'
      &function  (      )      
        &pushsdbgv 1							//'overChild'
        &not
        &jnz label1        
        &pushsdb 0							//'onEnterFrame'
        &delete2
        &pop
        &gotoLabel '_down'
        &play
       label1:
      &end // of function 

      &setVariable
    &end // of frame 11
  &end // of defineMovieClip 16
  
  &exportAssets
    16 &as 'Timer'
  &end // of exportAssets

  &frame 0
    &constants 'bottom', 'Stack', 'Timer', 'attachMovie', '_x', 'top', 'size', 'stage', 'id', 'color', 'Attach', 'timerStacks', 'length', 'clipStack', 'Push', 'Object', 'timerStackWidth', 'nextTimerStackId', 'TimerClipStack', 'timers', 'Array', 'push', 'pop', 'Clear', 'GetStackForNewTimer', 'stack', 'index', 'Pointer', 'encode', 'clip', 'KillTimerStack', 'Pop', 'AddTimer', 'EraseTimer', 'onUnload', 'prototype', 'child', 'childStage', 'mouseOverObserver', '_parent', 'overChild', 'OnTimerRollOut', 'removeMovieClip', 'initialized', '.', 'SetID', 'innerFrameColorizer', 'setRGB', 'OnTimerInitialized', 'client', 'GetHelpSite', 'helpSite', 'OnTimerClicked', 'FSCommand:', 'OnClicked', 'qualifyName', '', 'OnTimerPressed', 'OnPressed', 'OnTimerRollOver', 'OnRollOver', 'OnRollOut', 'OnInitialized', 'this'  
    &function2 TimerClipStack (r:3='stage', r:2='id', r:4='x') (r:1='this')
      &pushregister 1
      &pushsdb 0							//'bottom'
      &pushregister 2
      &pushsdb 1							//'Stack'
      &pushregister 2
      &toString
      &add
      &pushsdb 2							//'Timer'
      &pushbyte 3
      &pushregister 3
      &pushsdb 3							//'attachMovie'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 0							//'bottom'
      &pushsdb 4							//'_x'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 5							//'top'
      &pushregister 1
      &pushsdbgm 0							//'bottom'
      &setMember
      &pushregister 1
      &pushsdb 6							//'size'
      &pushone
      &setMember
    &end // of function TimerClipStack

    &function2 Timer (r:5='stage', r:3='id', r:4='color', r:2='clip') (r:1='this')
      &pushregister 1
      &pushsdb 7							//'stage'
      &pushregister 5
      &setMember
      &pushregister 1
      &pushsdb 8							//'id'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 9							//'color'
      &pushregister 4
      &setMember
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 10							// Attach()
    &end // of function Timer

    &function2 GetStackForNewTimer (r:5='timerColor') (r:1='this')
      &pushsdbgv 11							//'timerStacks'
      &pushsdbgm 12							//'length'
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:3
      &pop
     label1:
      &pushregister 3
      &pushregister 4
      &lessThan
      &not
      &jnz label3      
      &pushsdbgv 11							//'timerStacks'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgm 9							//'color'
      &pushregister 5
      &equals
      &not
      &jnz label2      
      &pushzero
      &pushregister 2
      &pushsdbgm 13							//'clipStack'
      &dcallmp 14							// Push()
      &pushregister 2
      &return
     label2:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label1      
     label3:
      &pushzero
      &pushsdb 15							//'Object'
      &new
      &setRegister r:6
      &pop
      &pushregister 6
      &pushsdb 9							//'color'
      &pushregister 5
      &setMember
      &pushregister 6
      &pushsdb 13							//'clipStack'
      &pushregister 4
      &pushsdbgv 16							//'timerStackWidth'
      &multiply
      &pushsdbgv 17							//'nextTimerStackId'
      &pushsdb 17							//'nextTimerStackId'
      &pushsdbgv 17							//'nextTimerStackId'
      &increment
      &setVariable
      &pushregister 1
      &pushbyte 3
      &pushsdb 18							//'TimerClipStack'
      &new
      &setMember
      &pushregister 6
      &pushsdb 19							//'timers'
      &pushzero
      &pushsdb 20							//'Array'
      &new
      &setMember
      &pushregister 6
      &pushone
      &pushsdbgv 11							//'timerStacks'
      &dcallmp 21							// push()
      &pushregister 6
      &return
    &end // of function GetStackForNewTimer

    &function2 KillTimerStack (r:4='timerStack') ()
      &pushsdbgv 11							//'timerStacks'
      &pushsdbgm 12							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:1
      &pop
     label4:
      &pushregister 1
      &pushregister 3
      &lessThan
      &not
      &jnz label6      
      &pushsdbgv 11							//'timerStacks'
      &pushregister 1
      &getMember
      &pushregister 4
      &equals
      &not
      &jnz label5      
      &jmp label6      
     label5:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label4      
     label6:
      &pushregister 1
      &pushone
      &add
      &pushregister 3
      &lessThan
      &not
      &jnz label7      
      &pushsdbgv 11							//'timerStacks'
      &pushregister 1
      &pushone
      &add
      &getMember
      &setRegister r:2
      &pop
      &pushsdbgv 11							//'timerStacks'
      &pushregister 1
      &pushregister 2
      &setMember
      &pushregister 2
      &pushsdbgm 13							//'clipStack'
      &pushsdbgm 0							//'bottom'
      &pushsdb 4							//'_x'
      &pushregister 1
      &pushsdbgv 16							//'timerStackWidth'
      &multiply
      &setMember
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label6      
     label7:
      &pushzero
      &pushsdbgv 11							//'timerStacks'
      &dcallmp 22							// pop()
      &pushsdbgv 11							//'timerStacks'
      &pushsdbgm 12							//'length'
      &pushzero
      &equals
      &not
      &jnz label8      
      &pushsdb 17							//'nextTimerStackId'
      &pushzerosv
     label8:
      &pushzero
      &pushregister 4
      &pushsdbgm 13							//'clipStack'
      &dcallmp 23							// Clear()
    &end // of function KillTimerStack

    &function2 AddTimer (r:6='timerId', r:5='timerColor') (r:1='this')
      &pushregister 5
      &pushone
      &pushsdb 24							//'GetStackForNewTimer'
      &callFunction
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdbgm 13							//'clipStack'
      &pushsdbgm 5							//'top'
      &pushregister 5
      &pushregister 6
      &toString
      &pushregister 1
      &pushbyte 4
      &pushsdb 2							//'Timer'
      &new
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdbgm 19							//'timers'
      &pushsdbgm 12							//'length'
      &setRegister r:4
      &pop
      &pushregister 2
      &pushone
      &pushregister 3
      &pushsdbgm 19							//'timers'
      &dcallmp 21							// push()
      &pushregister 2
      &pushsdb 25							//'stack'
      &pushregister 3
      &setMember
      &pushregister 2
      &pushsdb 26							//'index'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushregister 6
      &toString
      &pushregister 2
      &setMember
      &pushregister 2
      &pushone
      &pushsdbgv 27							//'Pointer'
      &pushsdb 28							//'encode'
      &callMethod
      &return
    &end // of function AddTimer

    &function2 EraseTimer (r:7='timerId') (r:1='this')
      &pushregister 1
      &pushregister 7
      &toString
      &getMember
      &setRegister r:5
      &pop
      &pushregister 5
      &pushsdbgm 25							//'stack'
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgm 26							//'index'
      &setRegister r:2
      &pop
      &pushregister 4
      &pushsdbgm 19							//'timers'
      &pushsdbgm 12							//'length'
      &setRegister r:6
      &pop
     label9:
      &pushregister 2
      &pushone
      &add
      &pushregister 6
      &lessThan
      &not
      &jnz label10      
      &pushregister 4
      &pushsdbgm 19							//'timers'
      &pushregister 2
      &pushone
      &add
      &getMember
      &setRegister r:3
      &pop
      &pushregister 5
      &pushsdbgm 29							//'clip'
      &pushone
      &pushregister 3
      &dcallmp 10							// Attach()
      &pushregister 4
      &pushsdbgm 19							//'timers'
      &pushregister 2
      &pushregister 3
      &setMember
      &pushregister 3
      &pushsdb 26							//'index'
      &pushregister 2
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label9      
     label10:
      &pushzero
      &pushregister 4
      &pushsdbgm 19							//'timers'
      &dcallmp 22							// pop()
      &pushregister 4
      &pushsdbgm 13							//'clipStack'
      &pushsdbgm 6							//'size'
      &pushone
      &equals
      &not
      &jnz label11      
      &pushregister 4
      &pushone
      &dcallfp 30							// KillTimerStack()
      &jmp label12      
     label11:
      &pushzero
      &pushregister 4
      &pushsdbgm 13							//'clipStack'
      &dcallmp 31							// Pop()
     label12:
      &pushregister 1
      &pushregister 7
      &toString
      &delete
      &pop
    &end // of function EraseTimer

    &function onUnload (    )    
      &pushsdb 18							//'TimerClipStack'
      &delete2
      &pop
      &pushsdb 2							//'Timer'
      &delete2
      &pop
      &pushsdb 24							//'GetStackForNewTimer'
      &delete2
      &pop
      &pushsdb 30							//'KillTimerStack'
      &delete2
      &pop
      &pushsdb 32							//'AddTimer'
      &delete2
      &pop
      &pushsdb 33							//'EraseTimer'
      &delete2
      &pop
      &pushsdb 34							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 18							//'TimerClipStack'
    &pushsdbgm 35							//'prototype'
    &pushsdb 14							//'Push'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 36							//'child'
      &pushsdb 2							//'Timer'
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 5							//'top'
      &pushsdbgm 37							//'childStage'
      &pushsdb 3							//'attachMovie'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 38							//'mouseOverObserver'
      &pushregister 1
      &pushsdbgm 5							//'top'
      &setMember
      &pushregister 1
      &pushsdb 5							//'top'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 6							//'size'
      &pushregister 1
      &pushsdbgm 6							//'size'
      &increment
      &setMember
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushsdbgv 18							//'TimerClipStack'
    &pushsdbgm 35							//'prototype'
    &pushsdb 31							//'Pop'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 5							//'top'
      &pushsdbgm 39							//'_parent'
      &pushsdbgm 39							//'_parent'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgm 40							//'overChild'
      &not
      &jnz label13      
      &pushregister 1
      &pushsdbgm 5							//'top'
      &pushone
      &pushregister 2
      &dcallmp 41							// OnTimerRollOut()
     label13:
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'top'
      &dcallmp 42							// removeMovieClip()
      &pushregister 1
      &pushsdb 5							//'top'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 6							//'size'
      &pushregister 1
      &pushsdbgm 6							//'size'
      &decrement
      &setMember
    &end // of function 

    &setMember
    &pushsdbgv 18							//'TimerClipStack'
    &pushsdbgm 35							//'prototype'
    &pushsdb 23							//'Clear'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &pushsdbgm 0							//'bottom'
      &dcallmp 42							// removeMovieClip()
      &pushregister 1
      &pushsdb 0							//'bottom'
      &delete
      &pop
      &pushregister 1
      &pushsdb 5							//'top'
      &delete
      &pop
      &pushregister 1
      &pushsdb 6							//'size'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushsdbgv 2							//'Timer'
    &pushsdbgm 35							//'prototype'
    &pushsdb 10							//'Attach'
    &function2  (r:2='clip') (r:1='this')
      &pushregister 1
      &pushsdb 29							//'clip'
      &pushregister 2
      &setMember
      &pushregister 2
      &pushsdbgm 43							//'initialized'
      &not
      &jnz label14      
      &pushregister 1
      &pushsdbgm 7							//'stage'
      &toString
      &pushsdb 44							//'.'
      &add
      &pushregister 1
      &pushsdbgm 8							//'id'
      &add
      &pushone
      &pushregister 2
      &dcallmp 45							// SetID()
      &pushregister 1
      &pushsdbgm 9							//'color'
      &pushone
      &pushregister 2
      &pushsdbgm 46							//'innerFrameColorizer'
      &dcallmp 47							// setRGB()
      &jmp label16      
     label14:
      &pushregister 1
      &pushsdb 48							//'OnTimerInitialized'
      &function2  (r:2='timerClip') (r:1='this')
        &pushregister 2
        &pushregister 1
        &pushsdbgm 29							//'clip'
        &equals
        &not
        &jnz label15        
        &pushregister 1
        &pushsdbgm 7							//'stage'
        &toString
        &pushsdb 44							//'.'
        &add
        &pushregister 1
        &pushsdbgm 8							//'id'
        &add
        &pushone
        &pushregister 2
        &dcallmp 45							// SetID()
        &pushregister 1
        &pushsdbgm 9							//'color'
        &pushone
        &pushregister 2
        &pushsdbgm 46							//'innerFrameColorizer'
        &dcallmp 47							// setRGB()
       label15:
      &end // of function 

      &setMember
     label16:
      &pushregister 2
      &pushsdb 49							//'client'
      &pushregister 1
      &setMember
    &end // of function 

    &setMember
    &pushsdbgv 2							//'Timer'
    &pushsdbgm 35							//'prototype'
    &pushsdb 50							//'GetHelpSite'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 29							//'clip'
      &pushsdbgm 51							//'helpSite'
      &toString
      &return
    &end // of function 

    &setMember
    &pushsdbgv 2							//'Timer'
    &pushsdbgm 35							//'prototype'
    &pushsdb 52							//'OnTimerClicked'
    &function2  (r:2='timerClip') (r:1='this')
      &pushregister 2
      &pushregister 1
      &pushsdbgm 29							//'clip'
      &equals
      &not
      &jnz label17      
      &pushsdb 53							//'FSCommand:'
      &pushsdb 54							//'OnClicked'
      &pushregister 1
      &pushsdbgm 7							//'stage'
      &toString
      &pushsdb 44							//'.'
      &add
      &pushregister 1
      &pushsdbgm 8							//'id'
      &add
      &pushbyte 2
      &pushsdb 55							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 56							//''
      &getURL2
     label17:
    &end // of function 

    &setMember
    &pushsdbgv 2							//'Timer'
    &pushsdbgm 35							//'prototype'
    &pushsdb 57							//'OnTimerPressed'
    &function2  (r:2='timerClip') (r:1='this')
      &pushregister 2
      &pushregister 1
      &pushsdbgm 29							//'clip'
      &equals
      &not
      &jnz label18      
      &pushsdb 53							//'FSCommand:'
      &pushsdb 58							//'OnPressed'
      &pushregister 1
      &pushsdbgm 7							//'stage'
      &toString
      &pushsdb 44							//'.'
      &add
      &pushregister 1
      &pushsdbgm 8							//'id'
      &add
      &pushbyte 2
      &pushsdb 55							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 56							//''
      &getURL2
     label18:
    &end // of function 

    &setMember
    &pushsdbgv 2							//'Timer'
    &pushsdbgm 35							//'prototype'
    &pushsdb 59							//'OnTimerRollOver'
    &function2  (r:2='timerClip') (r:1='this')
      &pushregister 2
      &pushregister 1
      &pushsdbgm 29							//'clip'
      &equals
      &not
      &jnz label19      
      &pushsdb 53							//'FSCommand:'
      &pushsdb 60							//'OnRollOver'
      &pushregister 1
      &pushsdbgm 7							//'stage'
      &toString
      &pushsdb 44							//'.'
      &add
      &pushregister 1
      &pushsdbgm 8							//'id'
      &add
      &pushbyte 2
      &pushsdb 55							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 56							//''
      &getURL2
     label19:
    &end // of function 

    &setMember
    &pushsdbgv 2							//'Timer'
    &pushsdbgm 35							//'prototype'
    &pushsdb 41							//'OnTimerRollOut'
    &function2  (r:2='timerClip') (r:1='this')
      &pushregister 2
      &pushregister 1
      &pushsdbgm 29							//'clip'
      &equals
      &not
      &jnz label20      
      &pushsdb 53							//'FSCommand:'
      &pushsdb 61							//'OnRollOut'
      &pushregister 1
      &pushsdbgm 7							//'stage'
      &toString
      &pushsdb 44							//'.'
      &add
      &pushregister 1
      &pushsdbgm 8							//'id'
      &add
      &pushbyte 2
      &pushsdb 55							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 56							//''
      &getURL2
     label20:
    &end // of function 

    &setMember
    &pushsdbgv 43							//'initialized'
    &not
    &not
    &jnz label21    
    &pushsdb 16							//'timerStackWidth'
    &pushbyte 42
    &setVariable
    &pushsdb 17							//'nextTimerStackId'
    &pushzerosv
    &pushsdb 11							//'timerStacks'
    &pushzero
    &pushsdb 20							//'Array'
    &new
    &setVariable
    &pushsdb 43							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 53							//'FSCommand:'
    &pushsdb 62							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 55							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 56							//''
    &getURL2
   label21:
  &end // of frame 0
  
  &exportAssets
    16 &as 'Timer'
  &end // of exportAssets

  &defineMovieClip 17 // total frames: 2

    &frame 0
      &stop
    &end // of frame 0
  &end // of defineMovieClip 17
&end
