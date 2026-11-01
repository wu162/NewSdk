movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA8271~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 7 // total frames: 20

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
  &end // of defineMovieClip 7
  
  &exportAssets
    7 &as 'RA3PowerBarPip'
  &end // of exportAssets

  &frame 0
    &constants 'powerBar', 'SetValues', 'highlightClip', 'highlightDummy', '_visible', 'CreateHUDGadgetFlash', 'Close', 'SetHighlighted', 'OnPipsLoaded', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function2 SetValues (r:2='productionArg', r:1='consumptionArg') ()
      &pushregister 1
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 0							//'powerBar'
      &dcallmp 1							// SetValues()
    &end // of function SetValues

    &function2 SetHighlighted (r:3='highlightArg') (r:1='_global')
      &pushregister 3
      &pushzero
      &equals
      &not
      &setRegister r:2
      &pop
      &pushregister 2
      &not
      &jnz label2      
      &pushsdbgv 2							//'highlightClip'
      &pushundef
      &equals
      &not
      &jnz label1      
      &pushsdbgv 3							//'highlightDummy'
      &pushsdb 4							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 2							//'highlightClip'
      &pushsdbgv 3							//'highlightDummy'
      &pushone
      &pushregister 1
      &dcallmsv 5							// CreateHUDGadgetFlash()
     label1:
      &jmp label3      
     label2:
      &pushzero
      &pushsdbgv 2							//'highlightClip'
      &dcallmp 6							// Close()
      &pushsdb 2							//'highlightClip'
      &pushundef
      &setVariable
     label3:
    &end // of function SetHighlighted

    &function onUnload (    )    
      &pushzero
      &pushsdbgv 2							//'highlightClip'
      &dcallmp 6							// Close()
      &pushsdb 1							//'SetValues'
      &delete2
      &pop
      &pushsdb 7							//'SetHighlighted'
      &delete2
      &pop
      &pushsdb 8							//'OnPipsLoaded'
      &delete2
      &pop
      &pushsdb 9							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 10							//'initialized'
    &not
    &not
    &jnz label4    
    &pushsdbgv 3							//'highlightDummy'
    &pushsdb 4							//'_visible'
    &pushfalse
    &setMember
    &pushsdb 10							//'initialized'
    &pushtrue
    &setVariable
    &pushthisgv
    &pushone
    &pushsdbgv 12							//'_parent'
    &dcallmp 13							// OnContentLoaded()
    &pushsdb 14							//'FSCommand:'
    &pushsdb 15							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 16							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 17							//''
    &getURL2
   label4:
    &stop
  &end // of frame 0

  &defineButton 9
  
    &on     &idleToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnRollOver'
      &pushsgv '_parent'
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
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 9

  &defineMovieClip 10 // total frames: 1

    &frame 0
      &constants 'pipCount', 'RA3PowerBarPip', 'attachMovie', '_x', '_y', '_width', '_green', 'gotoAndPlay', 'Math', 'max', 'floor', 'min', 'CalculateMeterRange', 'GetPipIndex', '_visible', '_red', 'Initialize', 'ConfigurePips', 'onUnload', 'initialized', 'this', '_parent', 'OnPipsLoaded'  
      &function2 Initialize (r:4='count') ()
        &pushsdb 0							//'pipCount'
        &pushregister 4
        &setVariable
        &pushzero
        &setRegister r:3
        &pop
        &pushzero
        &setRegister r:2
        &pop
       label1:
        &pushregister 2
        &pushsdbgv 0							//'pipCount'
        &lessThan
        &not
        &jnz label2        
        &pushregister 2
        &pushregister 2
        &toString
        &pushsdb 1							//'RA3PowerBarPip'
        &pushbyte 3
        &pushsdb 2							//'attachMovie'
        &callFunction
        &setRegister r:1
        &pop
        &pushregister 1
        &pushsdb 3							//'_x'
        &pushregister 3
        &setMember
        &pushregister 1
        &pushsdb 4							//'_y'
        &pushzero
        &setMember
        &pushregister 3
        &pushregister 1
        &pushsdbgm 5							//'_width'
        &add
        &setRegister r:3
        &pop
        &pushsdb 6							//'_green'
        &pushone
        &pushregister 1
        &dcallmp 7							// gotoAndPlay()
        &pushregister 2
        &increment
        &setRegister r:2
        &pop
        &jmp label1        
       label2:
      &end // of function Initialize

      &function2 CalculateMeterRange (r:2='production', r:3='consumption') ()
        &pushshort 1000
        &setRegister r:1
        &pop
       label3:
        &pushregister 1
        &pushregister 3
        &pushregister 2
        &pushbyte 2
        &pushsdbgv 8							//'Math'
        &pushsdb 9							//'max'
        &callMethod
        &lessThan
        &not
        &jnz label4        
        &pushregister 1
        &pushbyte 2
        &multiply
        &setRegister r:1
        &pop
        &jmp label3        
       label4:
        &pushregister 1
        &return
      &end // of function CalculateMeterRange

      &function2 GetPipIndex (r:2='intervals', r:4='meterValue', r:3='meterRange') ()
        &pushregister 4
        &pushregister 3
        &divide
        &pushregister 2
        &multiply
        &pushone
        &pushsdbgv 8							//'Math'
        &pushsdb 10							//'floor'
        &callMethod
        &setRegister r:1
        &pop
        &pushregister 2
        &pushone
        &subtract
        &pushregister 1
        &pushbyte 2
        &pushsdbgv 8							//'Math'
        &pushsdb 11							//'min'
        &callMethod
        &pushzero
        &pushbyte 2
        &pushsdbgv 8							//'Math'
        &pushsdb 9							//'max'
        &callMethod
        &return
      &end // of function GetPipIndex

      &function2 ConfigurePips (r:8='production', r:9='consumption') (r:1='this')
        &pushregister 9
        &pushregister 8
        &pushbyte 2
        &pushsdb 12							//'CalculateMeterRange'
        &callFunction
        &setRegister r:7
        &pop
        &pushregister 7
        &pushregister 8
        &pushsdbgv 0							//'pipCount'
        &pushbyte 3
        &pushsdb 13							//'GetPipIndex'
        &callFunction
        &setRegister r:4
        &pop
        &pushregister 7
        &pushregister 9
        &pushsdbgv 0							//'pipCount'
        &pushbyte 3
        &pushsdb 13							//'GetPipIndex'
        &callFunction
        &setRegister r:6
        &pop
        &pushregister 6
        &pushregister 4
        &pushbyte 2
        &pushsdbgv 8							//'Math'
        &pushsdb 9							//'max'
        &callMethod
        &setRegister r:5
        &pop
        &pushregister 4
        &pushregister 6
        &equals
        &dup
        &not
        &jnz label5        
        &pop
        &pushregister 8
        &pushregister 9
        &lessThan
       label5:
        &not
        &jnz label6        
        &pushregister 4
        &pushone
        &subtract
        &pushzero
        &pushbyte 2
        &pushsdbgv 8							//'Math'
        &pushsdb 9							//'max'
        &callMethod
        &setRegister r:4
        &pop
       label6:
        &pushzero
        &setRegister r:2
        &pop
       label7:
        &pushregister 2
        &pushsdbgv 0							//'pipCount'
        &lessThan
        &not
        &jnz label12        
        &pushregister 1
        &pushregister 2
        &toString
        &getMember
        &setRegister r:3
        &pop
        &pushregister 3
        &pushsdb 14							//'_visible'
        &pushregister 2
        &pushregister 5
        &greaterThan
        &not
        &jnz label8        
        &pushfalse
        &jmp label9        
       label8:
        &pushtrue
       label9:
        &setMember
        &pushregister 2
        &pushregister 4
        &greaterThan
        &not
        &jnz label10        
        &pushsdb 15							//'_red'
        &jmp label11        
       label10:
        &pushsdb 6							//'_green'
       label11:
        &pushone
        &pushregister 3
        &dcallmp 7							// gotoAndPlay()
        &pushregister 2
        &increment
        &setRegister r:2
        &pop
        &jmp label7        
       label12:
        &pushregister 1
        &pushregister 6
        &toString
        &getMember
        &pushsdbgm 3							//'_x'
        &return
      &end // of function ConfigurePips

      &function onUnload (      )      
        &pushsdb 16							//'Initialize'
        &delete2
        &pop
        &pushsdb 12							//'CalculateMeterRange'
        &delete2
        &pop
        &pushsdb 13							//'GetPipIndex'
        &delete2
        &pop
        &pushsdb 17							//'ConfigurePips'
        &delete2
        &pop
        &pushsdb 18							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 19							//'initialized'
      &not
      &not
      &jnz label13      
      &pushsdb 19							//'initialized'
      &pushtrue
      &setVariable
      &pushthisgv
      &pushone
      &pushsdbgv 21							//'_parent'
      &dcallmp 22							// OnPipsLoaded()
     label13:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 10

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 26

    &frame 0
      &constants 'pipsClip', '_pipCount', 'Initialize', 'button', '_x', '_y', '_xscale', '_width', '_yscale', '_height', 'extern', 'InGame', 'SetValues', 'marker', 'ConfigurePips', 'SetAlertMode', 'alertMode', 'OnPipsLoaded', 'onUnload', 'initialized'  
      &function2 OnPipsLoaded (r:4='mc') (r:1='_parent')
        &pushsdb 0							//'pipsClip'
        &pushregister 4
        &setVariable
        &pushsdbgv 1							//'_pipCount'
        &pushone
        &pushsdbgv 0							//'pipsClip'
        &dcallmp 2							// Initialize()
        &pushbyte 3
        &setRegister r:3
        &pop
        &pushbyte 3
        &setRegister r:2
        &pop
        &pushsdbgv 3							//'button'
        &pushsdb 4							//'_x'
        &pushzero
        &pushregister 3
        &subtract
        &setMember
        &pushsdbgv 3							//'button'
        &pushsdb 5							//'_y'
        &pushzero
        &pushregister 2
        &subtract
        &setMember
        &pushsdbgv 3							//'button'
        &pushsdb 6							//'_xscale'
        &pushregister 3
        &pushbyte 2
        &multiply
        &pushsdbgv 0							//'pipsClip'
        &pushsdbgm 7							//'_width'
        &add
        &setMember
        &pushsdbgv 3							//'button'
        &pushsdb 8							//'_yscale'
        &pushregister 2
        &pushbyte 2
        &multiply
        &pushsdbgv 0							//'pipsClip'
        &pushsdbgm 9							//'_height'
        &add
        &setMember
        &pushsdbgv 10							//'extern'
        &pushsdbgm 11							//'InGame'
        &not
        &not
        &jnz label1        
        &pushshort 1132
        &pushshort 1130
        &pushbyte 2
        &pushregister 1
        &dcallmp 12							// SetValues()
       label1:
      &end // of function OnPipsLoaded

      &function2 SetValues (r:1='production', r:2='consumption') ()
        &pushsdbgv 13							//'marker'
        &pushsdb 4							//'_x'
        &pushregister 2
        &pushregister 1
        &pushbyte 2
        &pushsdbgv 0							//'pipsClip'
        &pushsdb 14							//'ConfigurePips'
        &callMethod
        &setMember
        &pushregister 2
        &pushregister 1
        &greaterThan
        &not
        &not
        &jnz label2        
        &pushfalse
        &pushone
        &dcallfp 15							// SetAlertMode()
        &jmp label3        
       label2:
        &pushtrue
        &pushone
        &dcallfp 15							// SetAlertMode()
       label3:
      &end // of function SetValues

      &function2 SetAlertMode (r:1='newMode') ()
        &pushsdbgv 16							//'alertMode'
        &pushregister 1
        &equals
        &not
        &not
        &jnz label5        
        &pushsdb 16							//'alertMode'
        &pushregister 1
        &setVariable
        &pushsdbgv 16							//'alertMode'
        &pushtrue
        &equals
        &not
        &jnz label4        
        &gotoLabel '_pulse'
        &play
        &jmp label5        
       label4:
        &gotoLabel '_steady'
        &play
       label5:
      &end // of function SetAlertMode

      &function onUnload (      )      
        &pushsdb 17							//'OnPipsLoaded'
        &delete2
        &pop
        &pushsdb 12							//'SetValues'
        &delete2
        &pop
        &pushsdb 15							//'SetAlertMode'
        &delete2
        &pop
        &pushsdb 18							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 19							//'initialized'
      &not
      &not
      &jnz label6      
      &pushsdb 19							//'initialized'
      &pushtrue
      &setVariable
      &pushfalse
      &pushone
      &dcallfp 15							// SetAlertMode()
     label6:
    &end // of frame 0

    &frame 5
      &stop
    &end // of frame 5

    &frame 25
      &gotoLabel '_pulse'
      &play
    &end // of frame 25
  &end // of defineMovieClip 20

  &placeMovieClip 20 &as 'powerBar'
  
    &onClipEvent &construct
      &pushs '_pipCount'
      &pushbyte 38
      &setVariable
    &end
  &end // of placeMovieClip 20
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'WhiteRectangle' &as 21
  &end // of importAssets

  &defineMovieClip 22 // total frames: 1
  &end // of defineMovieClip 22
&end
