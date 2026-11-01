movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\ig_RadarObjectivesLayer\\ig_RadarObjectivesLayer.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 6 // total frames: 75

    &frame 74
      &stop
    &end // of frame 74
  &end // of defineMovieClip 6

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 12 // total frames: 75

    &frame 74
      &stop
    &end // of frame 74
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 50

    &frame 0
      &constants 'initialized', 'category', 'bonus', '_visible', 'isBonus', 'primary'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 1							//'category'
      &gotoAndPlay      &pushsdbgv 2							//'bonus'
      &pushsdb 3							//'_visible'
      &pushsdbgv 4							//'isBonus'
      &setMember
      &pushsdbgv 5							//'primary'
      &pushsdb 3							//'_visible'
      &pushsdbgv 4							//'isBonus'
      &not
      &setMember
      &pushsdb 0							//'initialized'
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

    &frame 39
      &stop
    &end // of frame 39

    &frame 49
      &stop
    &end // of frame 49
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'ObjectiveIndicator'
  &end // of exportAssets

  &frame 0
    &constants 'sizer', '_width', '_height', 'indicators', 'length', 'relX', 'relY', '_x', '_y', 'nextIndicatorId', 'ObjectiveIndicator', 'attachMovie', 'category', 'isBonus', 'push', 'inGame', 'Pointer', 'encode', 'splice', 'removeMovieClip', 'FSCommand:', 'OnDestroyed', 'qualifyName', '', 'Resize', 'AddIndicator', 'RemoveIndicator', 'MoveIndicator', 'onUnload', 'extern', 'InGame', 'initialized', 'this', '_parent', 'OnContentLoaded', 'Array', 'OnInitialized', 'Move', 'Attack'  
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

    &function2 AddIndicator (r:4='category', r:3='bonusArg') ()
      &pushsdbgv 9							//'nextIndicatorId'
      &pushsdb 9							//'nextIndicatorId'
      &pushsdbgv 9							//'nextIndicatorId'
      &increment
      &setVariable
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 2
      &toString
      &pushsdb 10							//'ObjectiveIndicator'
      &pushbyte 3
      &pushsdb 11							//'attachMovie'
      &callFunction
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 12							//'category'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 13							//'isBonus'
      &pushregister 3
      &pushzero
      &equals
      &not
      &setMember
      &pushregister 1
      &pushsdb 5							//'relX'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 6							//'relY'
      &pushzero
      &setMember
      &pushregister 1
      &pushone
      &pushsdbgv 3							//'indicators'
      &dcallmp 14							// push()
      &pushsdbgv 15							//'inGame'
      &jnz label6      
      &pushregister 1
      &jmp label7      
     label6:
      &pushregister 1
      &pushone
      &pushsdbgv 16							//'Pointer'
      &pushsdb 17							//'encode'
      &callMethod
     label7:
      &return
    &end // of function AddIndicator

    &function2 RemoveIndicator (r:2='indicator') ()
      &pushzero
      &setRegister r:1
      &pop
     label8:
      &pushregister 1
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label11      
      &pushsdbgv 3							//'indicators'
      &pushregister 1
      &getMember
      &pushregister 2
      &equals
      &not
      &jnz label10      
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 3							//'indicators'
      &dcallmp 18							// splice()
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &pushzero
      &equals
      &not
      &jnz label9      
      &pushsdb 9							//'nextIndicatorId'
      &pushzerosv
     label9:
      &jmp label11      
     label10:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label8      
     label11:
      &pushzero
      &pushregister 2
      &dcallmp 19							// removeMovieClip()
    &end // of function RemoveIndicator

    &function2 MoveIndicator (r:1='indicator', r:3='relX', r:2='relY') ()
      &pushregister 1
      &pushsdb 5							//'relX'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 6							//'relY'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 7							//'_x'
      &pushregister 3
      &pushsdbgv 0							//'sizer'
      &pushsdbgm 1							//'_width'
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 8							//'_y'
      &pushregister 2
      &pushsdbgv 0							//'sizer'
      &pushsdbgm 2							//'_height'
      &multiply
      &setMember
    &end // of function MoveIndicator

    &function2 OnDestroyed () (r:1='this')
      &pushsdb 20							//'FSCommand:'
      &pushsdb 21							//'OnDestroyed'
      &pushregister 1
      &pushbyte 2
      &pushsdb 22							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 23							//''
      &getURL2
    &end // of function OnDestroyed

    &function onUnload (    )    
      &pushsdb 24							//'Resize'
      &delete2
      &pop
      &pushsdb 25							//'AddIndicator'
      &delete2
      &pop
      &pushsdb 26							//'RemoveIndicator'
      &delete2
      &pop
      &pushsdb 27							//'MoveIndicator'
      &delete2
      &pop
      &pushsdb 21							//'OnDestroyed'
      &delete2
      &pop
      &pushsdb 28							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 15							//'inGame'
    &pushundef
    &equals
    &not
    &jnz label12    
    &pushsdb 15							//'inGame'
    &pushsdbgv 29							//'extern'
    &pushsdbgm 30							//'InGame'
    &setVariable
   label12:
    &pushsdbgv 31							//'initialized'
    &not
    &not
    &jnz label13    
    &pushthisgv
    &pushone
    &pushsdbgv 33							//'_parent'
    &dcallmp 34							// OnContentLoaded()
    &pushsdb 9							//'nextIndicatorId'
    &pushzerosv
    &pushsdb 3							//'indicators'
    &pushzero
    &pushsdb 35							//'Array'
    &new
    &setVariable
    &pushsdb 31							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 20							//'FSCommand:'
    &pushsdb 36							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 22							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 23							//''
    &getURL2
    &pushsdbgv 15							//'inGame'
    &not
    &not
    &jnz label13    
    &pushfloat 0.750000000000000
    &pushfloat 0.250000000000000
    &pushzero
    &pushsdb 37							//'Move'
    &pushbyte 2
    &pushsdb 25							//'AddIndicator'
    &callFunction
    &pushbyte 3
    &dcallfp 27							// MoveIndicator()
    &pushfloat 0.750000000000000
    &pushfloat 0.750000000000000
    &pushone
    &pushsdb 38							//'Attack'
    &pushbyte 2
    &pushsdb 25							//'AddIndicator'
    &callFunction
    &pushbyte 3
    &dcallfp 27							// MoveIndicator()
   label13:
    &stop
  &end // of frame 0

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &placeMovieClip 15 &as 'sizer'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 15
&end
