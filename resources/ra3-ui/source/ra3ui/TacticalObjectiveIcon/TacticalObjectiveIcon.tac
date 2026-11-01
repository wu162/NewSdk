movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA335E~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'isBonus', '_primary', '_bonus', 'icon', 'gotoAndPlay', 'marker', 'textClip', 'textVar', 'SetIsBonus', 'SetOrdinalText', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded'  
    &function2 SetIsBonus (r:1='val') ()
      &pushsdb 0							//'isBonus'
      &pushregister 1
      &setVariable
      &pushsdbgv 0							//'isBonus'
      &jnz label1      
      &pushsdb 1							//'_primary'
      &jmp label2      
     label1:
      &pushsdb 2							//'_bonus'
     label2:
      &pushone
      &pushsdbgv 3							//'icon'
      &dcallmp 4							// gotoAndPlay()
    &end // of function SetIsBonus

    &function2 SetOrdinalText (r:1='val') ()
      &pushsdbgv 5							//'marker'
      &pushsdbgm 6							//'textClip'
      &pushsdb 7							//'textVar'
      &pushregister 1
      &setMember
    &end // of function SetOrdinalText

    &function onUnload (    )    
      &pushsdb 8							//'SetIsBonus'
      &delete2
      &pop
      &pushsdb 9							//'SetOrdinalText'
      &delete2
      &pop
      &pushsdb 10							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 11							//'initialized'
    &not
    &not
    &jnz label3    
    &pushsdb 0							//'isBonus'
    &pushfalse
    &setVariable
    &pushthisgv
    &pushone
    &pushsdbgv 13							//'_parent'
    &dcallmp 14							// OnContentLoaded()
    &pushsdb 11							//'initialized'
    &pushtrue
    &setVariable
   label3:
  &end // of frame 0

  &defineButton 2
  
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
  &end // of defineButton 2

  &defineMovieClip 13 // total frames: 25

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
  &end // of defineMovieClip 13

  &defineMovieClip 24 // total frames: 25

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
  &end // of defineMovieClip 24

  &defineMovieClip 27 // total frames: 1
  &end // of defineMovieClip 27
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'LoadMovieStage' &as 28
  &end // of importAssets

  &defineMovieClip 29 // total frames: 1
  &end // of defineMovieClip 29

  &defineMovieClip 30 // total frames: 23

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
  &end // of defineMovieClip 30

  &defineMovieClip 31 // total frames: 1

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

    &placeMovieClip 27 &as 'textClip'
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'Ordinal'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 27
  &end // of defineMovieClip 31
&end
