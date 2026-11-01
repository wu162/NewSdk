movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA11F4~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineButton 2
  
    &on     &overUpToOverDown
      &pushs 'FSCommand:'
      &pushs 'OnPressed'
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
      &constants '', 'FSCommand:', 'OnRollOver', 'this', 'qualifyName'  
      &pushsdb 0							//''
      &pushbyte 6
      &pushbyte 100
      &setProperty
      &pushsdb 1							//'FSCommand:'
      &pushsdb 2							//'OnRollOver'
      &pushthisgv
      &pushbyte 2
      &pushsdb 4							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &constants '', 'FSCommand:', 'OnRollOut', 'this', 'qualifyName'  
      &pushsdb 0							//''
      &pushbyte 6
      &pushbyte 50
      &setProperty
      &pushsdb 1							//'FSCommand:'
      &pushsdb 2							//'OnRollOut'
      &pushthisgv
      &pushbyte 2
      &pushsdb 4							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
    &end
  &end // of defineButton 2

  &defineMovieClip 7 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 7

  &defineMovieClip 8 // total frames: 1

    &frame 0
      &constants '_primary', 'arrow', 'gotoAndPlay', '_bonus', 'SetPrimary', 'SetBonus', 'onUnload', 'initialized'  
      &function SetPrimary (      )      
        &pushsdb 0							//'_primary'
        &pushone
        &pushsdbgv 1							//'arrow'
        &dcallmp 2							// gotoAndPlay()
      &end // of function SetPrimary

      &function SetBonus (      )      
        &pushsdb 3							//'_bonus'
        &pushone
        &pushsdbgv 1							//'arrow'
        &dcallmp 2							// gotoAndPlay()
      &end // of function SetBonus

      &function onUnload (      )      
        &pushsdb 4							//'SetPrimary'
        &delete2
        &pop
        &pushsdb 5							//'SetBonus'
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
      &pushsdb 7							//'initialized'
      &pushtrue
      &setVariable
      &pushzero
      &dcallfp 4							// SetPrimary()
     label1:
    &end // of frame 0
  &end // of defineMovieClip 8

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 23

    &frame 0
      &stop
    &end // of frame 0

    &frame 1
      &pushs 'pulseCount'
      &pushbyte 5
      &setVariable
      &play
    &end // of frame 1

    &frame 21
      &constants 'pulseCount'  
      &pushsdb 0							//'pulseCount'
      &pushsdbgv 0							//'pulseCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &greaterThan
      &not
      &jnz label1      
      &gotoLabel '_loop'
      &play
     label1:
    &end // of frame 21

    &frame 22
      &stop
    &end // of frame 22
  &end // of defineMovieClip 13
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 14
  &end // of importAssets

  &defineMovieClip 15 // total frames: 50

    &frame 0
      &constants '_x', '_y', '_parent', 'Math', 'atan2', '', 'ordinal', '_rotation', 'helpSite', 'vertAlignment', 'top', 'bottom', 'horzAlignment', 'left', 'right', '_play', 'newness', 'gotoAndPlay', 'Pointer', 'encode', 'marker', 'SetBonus', 'SetObjectivePos', 'SetVisibility', 'IndicateNewness', 'GetHelpSite', 'onUnload', 'inGame', 'extern', 'InGame', 'initialized', 'category', 'isBonus', 'gotoAndStop', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', 'random'  
      &function2 SetObjectivePos (r:4='objectiveX', r:5='objectiveY') (r:1='this', r:2='_root')
        &pushregister 1
        &setRegister r:3
        &pop
       label1:
        &pushregister 3
        &pushregister 2
        &equals
        &not
        &not
        &jnz label2        
        &pushregister 4
        &pushregister 3
        &pushsdbgm 0							//'_x'
        &subtract
        &setRegister r:4
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
        &pushregister 4
        &pushregister 5
        &pushbyte 2
        &pushsdbgv 3							//'Math'
        &pushsdb 4							//'atan2'
        &callMethod
        &setRegister r:6
        &pop
        &pushregister 6
        &pushfloat 57.295780181884766
        &multiply
        &setRegister r:6
        &pop
        &pushsdb 5							//''
        &pushbyte 10
        &pushregister 6
        &setProperty
        &pushsdbgv 6							//'ordinal'
        &pushsdb 7							//'_rotation'
        &pushzero
        &pushregister 6
        &subtract
        &setMember
        &pushsdbgv 8							//'helpSite'
        &pushsdb 9							//'vertAlignment'
        &pushregister 5
        &pushzero
        &lessThan
        &not
        &jnz label3        
        &pushsdb 10							//'top'
        &jmp label4        
       label3:
        &pushsdb 11							//'bottom'
       label4:
        &setMember
        &pushsdbgv 8							//'helpSite'
        &pushsdb 12							//'horzAlignment'
        &pushregister 4
        &pushzero
        &lessThan
        &not
        &jnz label5        
        &pushsdb 13							//'left'
        &jmp label6        
       label5:
        &pushsdb 14							//'right'
       label6:
        &setMember
      &end // of function SetObjectivePos

      &function2 SetVisibility (r:1='visibleArg') ()
        &pushsdb 5							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function IndicateNewness (      )      
        &pushsdb 15							//'_play'
        &pushone
        &pushsdbgv 16							//'newness'
        &dcallmp 17							// gotoAndPlay()
      &end // of function IndicateNewness

      &function GetHelpSite (      )      
        &pushsdbgv 8							//'helpSite'
        &pushone
        &pushsdbgv 18							//'Pointer'
        &pushsdb 19							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function2 SetBonus (r:1='bonus') ()
        &pushregister 1
        &not
        &jnz label7        
        &pushzero
        &pushsdbgv 20							//'marker'
        &dcallmp 21							// SetBonus()
       label7:
      &end // of function SetBonus

      &function onUnload (      )      
        &pushsdb 22							//'SetObjectivePos'
        &delete2
        &pop
        &pushsdb 23							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 24							//'IndicateNewness'
        &delete2
        &pop
        &pushsdb 25							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 21							//'SetBonus'
        &delete2
        &pop
        &pushsdb 26							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 27							//'inGame'
      &pushundef
      &equals
      &not
      &jnz label8      
      &pushsdb 27							//'inGame'
      &pushsdbgv 28							//'extern'
      &pushsdbgm 29							//'InGame'
      &setVariable
     label8:
      &pushsdbgv 30							//'initialized'
      &not
      &not
      &jnz label10      
      &pushsdb 5							//''
      &pushbyte 6
      &pushbyte 50
      &setProperty
      &pushsdbgv 31							//'category'
      &gotoAndPlay      &pushsdbgv 32							//'isBonus'
      &not
      &jnz label9      
      &pushzero
      &pushsdbgv 20							//'marker'
      &dcallmp 21							// SetBonus()
     label9:
      &pushone
      &pushone
      &pushsdbgv 16							//'newness'
      &dcallmp 33							// gotoAndStop()
      &pushsdb 30							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 34							//'FSCommand:'
      &pushsdb 35							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 37							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 5							//''
      &getURL2
      &pushsdbgv 27							//'inGame'
      &not
      &not
      &jnz label10      
      &pushzero
      &pushsdbgv 3							//'Math'
      &pushsdb 38							//'random'
      &callMethod
      &pushshort 768
      &multiply
      &pushzero
      &pushsdbgv 3							//'Math'
      &pushsdb 38							//'random'
      &callMethod
      &pushshort 1024
      &multiply
      &pushbyte 2
      &dcallfp 22							// SetObjectivePos()
      &pushzero
      &dcallfp 24							// IndicateNewness()
     label10:
    &end // of frame 0

    &placeMovieClip 11 &as 'ordinal'
    
      &onClipEvent &load
        &pushsgv 'tf'
        &pushs 'text'
        &pushs '$'
        &pushs 'Ordinal'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&outline'
        &add
        &setMember
      &end
    &end // of placeMovieClip 11

    &placeMovieClip 14 &as 'helpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'right'
      &end
    &end // of placeMovieClip 14

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
  &end // of defineMovieClip 15
  
  &exportAssets
    15 &as 'OffScreenIndicator'
  &end // of exportAssets

  &defineButton 17
  
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
  &end // of defineButton 17

  &defineMovieClip 28 // total frames: 25

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 28

  &defineMovieClip 39 // total frames: 25

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 39

  &defineMovieClip 42 // total frames: 1
  &end // of defineMovieClip 42
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'LoadMovieStage' &as 43
  &end // of importAssets

  &defineMovieClip 44 // total frames: 1
  &end // of defineMovieClip 44

  &defineMovieClip 45 // total frames: 23

    &frame 0
      &stop
    &end // of frame 0

    &frame 1
      &pushs 'pulseCount'
      &pushbyte 5
      &setVariable
      &play
    &end // of frame 1

    &frame 21
      &constants 'pulseCount'  
      &pushsdb 0							//'pulseCount'
      &pushsdbgv 0							//'pulseCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &greaterThan
      &not
      &jnz label1      
      &gotoLabel '_loop'
      &play
     label1:
    &end // of frame 21

    &frame 22
      &stop
    &end // of frame 22
  &end // of defineMovieClip 45

  &defineMovieClip 46 // total frames: 1

    &frame 0
      &constants '_x', '_y', '_parent', '', '_play', 'newness', 'gotoAndPlay', 'popUpStage', 'Pointer', 'encode', 'Move', 'SetVisibility', 'IndicateNewness', 'GetPopUpStage', 'onUnload', 'initialized', 'category', 'primary', 'bonus', 'isBonus', '_visible', 'gotoAndStop', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', 'extern', 'InGame'  
      &function2 Move (r:5='x', r:4='y') (r:1='_root', r:2='_parent')
        &pushregister 2
        &setRegister r:3
        &pop
       label1:
        &pushregister 3
        &pushregister 1
        &equals
        &not
        &not
        &jnz label2        
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
        &jmp label1        
       label2:
        &pushsdb 3							//''
        &pushzero
        &pushregister 5
        &setProperty
        &pushsdb 3							//''
        &pushone
        &pushregister 4
        &pushbyte 20
        &subtract
        &setProperty
      &end // of function Move

      &function2 SetVisibility (r:1='visibleArg') ()
        &pushsdb 3							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function IndicateNewness (      )      
        &pushsdb 4							//'_play'
        &pushone
        &pushsdbgv 5							//'newness'
        &dcallmp 6							// gotoAndPlay()
      &end // of function IndicateNewness

      &function GetPopUpStage (      )      
        &pushsdbgv 7							//'popUpStage'
        &pushone
        &pushsdbgv 8							//'Pointer'
        &pushsdb 9							//'encode'
        &callMethod
        &return
      &end // of function GetPopUpStage

      &function onUnload (      )      
        &pushsdb 10							//'Move'
        &delete2
        &pop
        &pushsdb 11							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 12							//'IndicateNewness'
        &delete2
        &pop
        &pushsdb 13							//'GetPopUpStage'
        &delete2
        &pop
        &pushsdb 14							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 15							//'initialized'
      &not
      &not
      &jnz label5      
      &pushsdbgv 16							//'category'
      &pushone
      &pushsdbgv 17							//'primary'
      &dcallmp 6							// gotoAndPlay()
      &pushsdbgv 16							//'category'
      &pushone
      &pushsdbgv 18							//'bonus'
      &dcallmp 6							// gotoAndPlay()
      &pushsdbgv 19							//'isBonus'
      &not
      &jnz label3      
      &pushsdbgv 17							//'primary'
      &pushsdb 20							//'_visible'
      &pushfalse
      &setMember
      &jmp label4      
     label3:
      &pushsdbgv 18							//'bonus'
      &pushsdb 20							//'_visible'
      &pushfalse
      &setMember
     label4:
      &pushone
      &pushone
      &pushsdbgv 5							//'newness'
      &dcallmp 21							// gotoAndStop()
      &pushsdb 15							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 22							//'FSCommand:'
      &pushsdb 23							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 25							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 3							//''
      &getURL2
      &pushsdbgv 26							//'extern'
      &pushsdbgm 27							//'InGame'
      &not
      &not
      &jnz label5      
      &pushzero
      &dcallfp 12							// IndicateNewness()
     label5:
      &stop
    &end // of frame 0

    &placeMovieClip 42 
    
      &onClipEvent &load
        &pushsgv 'tf'
        &pushs 'text'
        &pushs '$'
        &pushs 'Ordinal'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&outline'
        &add
        &setMember
      &end
    &end // of placeMovieClip 42
  &end // of defineMovieClip 46
  
  &exportAssets
    46 &as 'OnScreenIndicator'
  &end // of exportAssets

  &frame 0
    &constants 'nextOnScreenIndicatorId', 'OnScreenIndicator', 'onScreen', 'attachMovie', 'category', 'isBonus', 'onScreenIndicatorCount', 'inGame', 'Pointer', 'encode', 'removeMovieClip', 'nextOffScreenIndicatorId', 'OffScreenIndicator', 'offScreen', 'SetBonus', 'offScreenIndicatorCount', 'AddOnScreenIndicator', 'RemoveOnScreenIndicator', 'AddOffScreenIndicator', 'RemoveOffScreenIndicator', 'onUnload', 'extern', 'InGame', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', '', 'Capture', 'Protect'  
    &function2 AddOnScreenIndicator (r:4='category', r:3='bonusArg') ()
      &pushsdbgv 0							//'nextOnScreenIndicatorId'
      &pushsdb 0							//'nextOnScreenIndicatorId'
      &pushsdbgv 0							//'nextOnScreenIndicatorId'
      &increment
      &setVariable
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 2
      &toString
      &pushsdb 1							//'OnScreenIndicator'
      &pushbyte 3
      &pushsdbgv 2							//'onScreen'
      &pushsdb 3							//'attachMovie'
      &callMethod
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 4							//'category'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 5							//'isBonus'
      &pushregister 3
      &pushzero
      &equals
      &not
      &setMember
      &pushsdb 6							//'onScreenIndicatorCount'
      &pushsdbgv 6							//'onScreenIndicatorCount'
      &increment
      &setVariable
      &pushsdbgv 7							//'inGame'
      &jnz label1      
      &pushregister 1
      &jmp label2      
     label1:
      &pushregister 1
      &pushone
      &pushsdbgv 8							//'Pointer'
      &pushsdb 9							//'encode'
      &callMethod
     label2:
      &return
    &end // of function AddOnScreenIndicator

    &function2 RemoveOnScreenIndicator (r:1='indicator') ()
      &pushzero
      &pushregister 1
      &dcallmp 10							// removeMovieClip()
      &pushsdb 6							//'onScreenIndicatorCount'
      &pushsdbgv 6							//'onScreenIndicatorCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label3      
      &pushsdb 0							//'nextOnScreenIndicatorId'
      &pushzerosv
     label3:
    &end // of function RemoveOnScreenIndicator

    &function2 AddOffScreenIndicator (r:4='category', r:3='bonusArg') ()
      &pushsdbgv 11							//'nextOffScreenIndicatorId'
      &pushsdb 11							//'nextOffScreenIndicatorId'
      &pushsdbgv 11							//'nextOffScreenIndicatorId'
      &increment
      &setVariable
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 2
      &toString
      &pushsdb 12							//'OffScreenIndicator'
      &pushbyte 3
      &pushsdbgv 13							//'offScreen'
      &pushsdb 3							//'attachMovie'
      &callMethod
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 4							//'category'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 5							//'isBonus'
      &pushregister 3
      &pushzero
      &equals
      &not
      &setMember
      &pushregister 3
      &pushzero
      &equals
      &not
      &pushone
      &pushregister 1
      &dcallmp 14							// SetBonus()
      &pushsdb 15							//'offScreenIndicatorCount'
      &pushsdbgv 15							//'offScreenIndicatorCount'
      &increment
      &setVariable
      &pushsdbgv 7							//'inGame'
      &jnz label4      
      &pushregister 1
      &jmp label5      
     label4:
      &pushregister 1
      &pushone
      &pushsdbgv 8							//'Pointer'
      &pushsdb 9							//'encode'
      &callMethod
     label5:
      &return
    &end // of function AddOffScreenIndicator

    &function2 RemoveOffScreenIndicator (r:1='indicator') ()
      &pushzero
      &pushregister 1
      &dcallmp 10							// removeMovieClip()
      &pushsdb 15							//'offScreenIndicatorCount'
      &pushsdbgv 15							//'offScreenIndicatorCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label6      
      &pushsdb 11							//'nextOffScreenIndicatorId'
      &pushzerosv
     label6:
    &end // of function RemoveOffScreenIndicator

    &function onUnload (    )    
      &pushsdb 16							//'AddOnScreenIndicator'
      &delete2
      &pop
      &pushsdb 17							//'RemoveOnScreenIndicator'
      &delete2
      &pop
      &pushsdb 18							//'AddOffScreenIndicator'
      &delete2
      &pop
      &pushsdb 19							//'RemoveOffScreenIndicator'
      &delete2
      &pop
      &pushsdb 20							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 7							//'inGame'
    &pushundef
    &equals
    &not
    &jnz label7    
    &pushsdb 7							//'inGame'
    &pushsdbgv 21							//'extern'
    &pushsdbgm 22							//'InGame'
    &setVariable
   label7:
    &pushsdbgv 23							//'initialized'
    &not
    &not
    &jnz label8    
    &pushsdb 0							//'nextOnScreenIndicatorId'
    &pushzerosv
    &pushsdb 6							//'onScreenIndicatorCount'
    &pushzerosv
    &pushsdb 11							//'nextOffScreenIndicatorId'
    &pushzerosv
    &pushsdb 15							//'offScreenIndicatorCount'
    &pushzerosv
    &pushsdb 23							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 24							//'FSCommand:'
    &pushsdb 25							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 27							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 28							//''
    &getURL2
    &pushsdbgv 7							//'inGame'
    &not
    &not
    &jnz label8    
    &pushone
    &pushsdb 29							//'Capture'
    &pushbyte 2
    &dcallfp 16							// AddOnScreenIndicator()
    &pushone
    &pushsdb 30							//'Protect'
    &pushbyte 2
    &dcallfp 18							// AddOffScreenIndicator()
    &pushzero
    &pushsdb 29							//'Capture'
    &pushbyte 2
    &dcallfp 18							// AddOffScreenIndicator()
   label8:
    &stop
  &end // of frame 0

  &defineMovieClip 47 // total frames: 1
  &end // of defineMovieClip 47
&end
