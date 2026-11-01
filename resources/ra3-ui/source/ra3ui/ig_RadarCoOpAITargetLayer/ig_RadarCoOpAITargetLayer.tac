movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_RAD~1\\IG_RAD~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 7 // total frames: 30

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

  &defineMovieClip 8 // total frames: 32

    &frame 0
      &constants 'relX', 'relY', '', 'sizer', '_width', '_height', 'icon', 'gotoAndPlay', 'Move', 'SetDisposition', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 Move (r:3='x', r:2='y') (r:1='_parent')
        &pushsdb 0							//'relX'
        &pushregister 3
        &setVariable
        &pushsdb 1							//'relY'
        &pushregister 2
        &setVariable
        &pushsdb 2							//''
        &pushzero
        &pushregister 1
        &pushsdbgm 3							//'sizer'
        &pushsdbgm 4							//'_width'
        &pushsdbgv 0							//'relX'
        &multiply
        &setProperty
        &pushsdb 2							//''
        &pushone
        &pushregister 1
        &pushsdbgm 3							//'sizer'
        &pushsdbgm 5							//'_height'
        &pushsdbgv 1							//'relY'
        &multiply
        &setProperty
      &end // of function Move

      &function2 SetDisposition (r:1='disposition') ()
        &pushregister 1
        &pushone
        &pushsdbgv 6							//'icon'
        &dcallmp 7							// gotoAndPlay()
      &end // of function SetDisposition

      &function onUnload (      )      
        &pushsdb 8							//'Move'
        &delete2
        &pop
        &pushsdb 9							//'SetDisposition'
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
      &pushsdb 11							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 12							//'FSCommand:'
      &pushsdb 13							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 15							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 2							//''
      &getURL2
     label1:
    &end // of frame 0
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as 'AITargetIndicator'
  &end // of exportAssets

  &frame 0
    &constants 'sizer', '_width', '_height', 'indicators', 'length', 'relX', 'relY', '_x', '_y', 'nextIndicatorId', 'AITargetIndicator', 'attachMovie', 'push', 'Pointer', 'encode', 'splice', 'removeMovieClip', 'FSCommand:', 'OnDestroyed', 'qualifyName', '', 'Resize', 'AddIndicator', 'RemoveIndicator', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'Array', 'OnInitialized'  
    &function2 Resize (r:4='newWidth', r:3='newHeight') ()
      &pushsdbgv 0							//'sizer'
      &pushsdb 1							//'_width'
      &pushregister 4
      &setMember
      &pushsdbgv 0							//'sizer'
      &pushsdb 2							//'_height'
      &pushregister 3
      &setMember
      &pushzero
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label5      
      &pushsdbgv 3							//'indicators'
      &pushregister 2
      &getMember
      &setRegister r:1
      &pop
      &pushregister 1
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label2      
      &pop
      &pushregister 1
      &pushsdbgm 5							//'relX'
      &pushundef
      &equals
      &not
     label2:
      &dup
      &not
      &jnz label3      
      &pop
      &pushregister 1
      &pushsdbgm 6							//'relY'
      &pushundef
      &equals
      &not
     label3:
      &not
      &jnz label4      
      &pushregister 1
      &pushsdb 7							//'_x'
      &pushregister 1
      &pushsdbgm 5							//'relX'
      &pushregister 4
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 8							//'_y'
      &pushregister 1
      &pushsdbgm 6							//'relY'
      &pushregister 3
      &multiply
      &setMember
     label4:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label1      
     label5:
    &end // of function Resize

    &function2 AddIndicator () (r:1='this')
      &pushsdbgv 9							//'nextIndicatorId'
      &pushsdb 9							//'nextIndicatorId'
      &pushsdbgv 9							//'nextIndicatorId'
      &increment
      &setVariable
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 3
      &toString
      &pushsdb 10							//'AITargetIndicator'
      &pushbyte 3
      &pushregister 1
      &pushsdb 11							//'attachMovie'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 5							//'relX'
      &pushzero
      &setMember
      &pushregister 2
      &pushsdb 6							//'relY'
      &pushzero
      &setMember
      &pushregister 2
      &pushone
      &pushsdbgv 3							//'indicators'
      &dcallmp 12							// push()
      &pushregister 2
      &pushone
      &pushsdbgv 13							//'Pointer'
      &pushsdb 14							//'encode'
      &callMethod
      &return
    &end // of function AddIndicator

    &function2 RemoveIndicator (r:2='indicator') ()
      &pushzero
      &setRegister r:1
      &pop
     label6:
      &pushregister 1
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label9      
      &pushsdbgv 3							//'indicators'
      &pushregister 1
      &getMember
      &pushregister 2
      &equals
      &not
      &jnz label8      
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 3							//'indicators'
      &dcallmp 15							// splice()
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &pushzero
      &equals
      &not
      &jnz label7      
      &pushsdb 9							//'nextIndicatorId'
      &pushzerosv
     label7:
      &jmp label9      
     label8:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label6      
     label9:
      &pushzero
      &pushregister 2
      &dcallmp 16							// removeMovieClip()
    &end // of function RemoveIndicator

    &function2 OnDestroyed () (r:1='this')
      &pushsdb 17							//'FSCommand:'
      &pushsdb 18							//'OnDestroyed'
      &pushregister 1
      &pushbyte 2
      &pushsdb 19							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 20							//''
      &getURL2
    &end // of function OnDestroyed

    &function onUnload (    )    
      &pushsdb 21							//'Resize'
      &delete2
      &pop
      &pushsdb 22							//'AddIndicator'
      &delete2
      &pop
      &pushsdb 23							//'RemoveIndicator'
      &delete2
      &pop
      &pushsdb 18							//'OnDestroyed'
      &delete2
      &pop
      &pushsdb 24							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 25							//'initialized'
    &not
    &not
    &jnz label10    
    &pushthisgv
    &pushone
    &pushsdbgv 27							//'_parent'
    &dcallmp 28							// OnContentLoaded()
    &pushsdb 9							//'nextIndicatorId'
    &pushzerosv
    &pushsdb 3							//'indicators'
    &pushzero
    &pushsdb 29							//'Array'
    &new
    &setVariable
    &pushsdb 25							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 17							//'FSCommand:'
    &pushsdb 30							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 19							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 20							//''
    &getURL2
   label10:
    &stop
  &end // of frame 0

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &placeMovieClip 10 &as 'sizer'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 10
&end
