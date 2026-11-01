movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_MIS~2\\IG_MIS~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 3 // total frames: 1

    &frame 0
      &constants 'MoveIndicator', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnClipDestroyed', 'Move', 'OnBound', 'OnDestroying', 'onUnload', 'this'  
      &function2 Move (r:4='newX', r:3='newY') (r:1='this', r:2='_parent')
        &pushregister 3
        &pushregister 4
        &pushregister 1
        &pushbyte 3
        &pushregister 2
        &dcallmp 0							// MoveIndicator()
      &end // of function Move

      &function2 OnBound () (r:1='this')
        &pushsdbgv 1							//'initialized'
        &not
        &jnz label1        
        &pushsdb 2							//'FSCommand:'
        &pushsdb 3							//'OnInitialized'
        &pushregister 1
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 5							//''
        &getURL2
       label1:
      &end // of function OnBound

      &function2 OnDestroying () (r:1='this')
        &pushsdbgv 1							//'initialized'
        &not
        &jnz label2        
        &pushsdb 2							//'FSCommand:'
        &pushsdb 6							//'OnClipDestroyed'
        &pushregister 1
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 5							//''
        &getURL2
       label2:
      &end // of function OnDestroying

      &function onUnload (      )      
        &pushsdb 7							//'Move'
        &delete2
        &pop
        &pushsdb 8							//'OnBound'
        &delete2
        &pop
        &pushsdb 9							//'OnDestroying'
        &delete2
        &pop
        &pushsdb 10							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 1							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdb 1							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 2							//'FSCommand:'
      &pushsdb 3							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 4							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 5							//''
      &getURL2
     label3:
    &end // of frame 0
  &end // of defineMovieClip 3
  
  &exportAssets
    3 &as 'HotSpotIndicator'
  &end // of exportAssets

  &frame 0
    &constants 'nextIndicatorId', 'HotSpotIndicator', 'attachMovie', 'relX', 'relY', 'indicators', 'push', 'Pointer', 'encode', 'length', 'splice', 'removeMovieClip', '_x', 'sizer', '_width', '_y', '_height', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnDestroying', 'OnClipDestroyed', 'AddIndicator', 'RemoveIndicator', 'MoveIndicator', 'OnBound', 'onUnload', 'this', '_parent', 'OnContentLoaded', 'Array'  
    &function2 AddIndicator () (r:1='this')
      &pushsdbgv 0							//'nextIndicatorId'
      &setRegister r:3
      &pop
      &pushsdb 0							//'nextIndicatorId'
      &pushsdbgv 0							//'nextIndicatorId'
      &increment
      &setVariable
      &pushregister 3
      &pushregister 3
      &toString
      &pushsdb 1							//'HotSpotIndicator'
      &pushbyte 3
      &pushregister 1
      &pushsdb 2							//'attachMovie'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 3							//'relX'
      &pushzero
      &setMember
      &pushregister 2
      &pushsdb 4							//'relY'
      &pushzero
      &setMember
      &pushregister 2
      &pushone
      &pushsdbgv 5							//'indicators'
      &dcallmp 6							// push()
      &pushregister 2
      &pushone
      &pushsdbgv 7							//'Pointer'
      &pushsdb 8							//'encode'
      &callMethod
      &return
    &end // of function AddIndicator

    &function2 RemoveIndicator (r:3='indicator') ()
      &pushsdbgv 5							//'indicators'
      &pushsdbgm 9							//'length'
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:1
      &pop
     label1:
      &pushregister 1
      &pushregister 2
      &lessThan
      &not
      &jnz label4      
      &pushregister 3
      &pushsdbgv 5							//'indicators'
      &pushregister 1
      &getMember
      &equals
      &not
      &jnz label3      
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 5							//'indicators'
      &dcallmp 10							// splice()
      &pushsdbgv 5							//'indicators'
      &pushsdbgm 9							//'length'
      &pushzero
      &equals
      &not
      &jnz label2      
      &pushsdb 0							//'nextIndicatorId'
      &pushzerosv
     label2:
      &jmp label4      
     label3:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label1      
     label4:
      &pushzero
      &pushregister 3
      &dcallmp 11							// removeMovieClip()
    &end // of function RemoveIndicator

    &function2 MoveIndicator (r:1='indicator', r:3='newX', r:2='newY') ()
      &pushregister 1
      &pushsdb 3							//'relX'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 4							//'relY'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 12							//'_x'
      &pushsdbgv 13							//'sizer'
      &pushsdbgm 14							//'_width'
      &pushregister 3
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 15							//'_y'
      &pushsdbgv 13							//'sizer'
      &pushsdbgm 16							//'_height'
      &pushregister 2
      &multiply
      &setMember
    &end // of function MoveIndicator

    &function2 Resize (r:5='newWidth', r:4='newHeight') ()
      &pushsdbgv 13							//'sizer'
      &pushsdb 14							//'_width'
      &pushregister 5
      &setMember
      &pushsdbgv 13							//'sizer'
      &pushsdb 16							//'_height'
      &pushregister 4
      &setMember
      &pushsdbgv 5							//'indicators'
      &pushsdbgm 9							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label5:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label6      
      &pushsdbgv 5							//'indicators'
      &pushregister 2
      &getMember
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 12							//'_x'
      &pushregister 1
      &pushsdbgm 3							//'relX'
      &pushregister 5
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 15							//'_y'
      &pushregister 1
      &pushsdbgm 4							//'relY'
      &pushregister 4
      &multiply
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label5      
     label6:
    &end // of function Resize

    &function2 OnBound () (r:1='this')
      &pushsdbgv 17							//'initialized'
      &not
      &jnz label7      
      &pushsdb 18							//'FSCommand:'
      &pushsdb 19							//'OnInitialized'
      &pushregister 1
      &pushbyte 2
      &pushsdb 20							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 21							//''
      &getURL2
     label7:
    &end // of function OnBound

    &function2 OnDestroying () (r:1='this')
      &pushsdbgv 5							//'indicators'
      &pushsdbgm 9							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label8:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label9      
      &pushzero
      &pushsdbgv 5							//'indicators'
      &pushregister 2
      &getMember
      &dcallmp 22							// OnDestroying()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label8      
     label9:
      &pushsdbgv 17							//'initialized'
      &not
      &jnz label10      
      &pushsdb 18							//'FSCommand:'
      &pushsdb 23							//'OnClipDestroyed'
      &pushregister 1
      &pushbyte 2
      &pushsdb 20							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 21							//''
      &getURL2
     label10:
    &end // of function OnDestroying

    &function onUnload (    )    
      &pushsdb 24							//'AddIndicator'
      &delete2
      &pop
      &pushsdb 25							//'RemoveIndicator'
      &delete2
      &pop
      &pushsdb 26							//'MoveIndicator'
      &delete2
      &pop
      &pushsdb 27							//'OnBound'
      &delete2
      &pop
      &pushsdb 22							//'OnDestroying'
      &delete2
      &pop
      &pushsdb 28							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 17							//'initialized'
    &not
    &not
    &jnz label11    
    &pushthisgv
    &pushone
    &pushsdbgv 30							//'_parent'
    &dcallmp 31							// OnContentLoaded()
    &pushsdb 5							//'indicators'
    &pushzero
    &pushsdb 32							//'Array'
    &new
    &setVariable
    &pushsdb 0							//'nextIndicatorId'
    &pushzerosv
    &pushsdb 17							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 18							//'FSCommand:'
    &pushsdb 19							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 20							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 21							//''
    &getURL2
   label11:
  &end // of frame 0

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5
&end
