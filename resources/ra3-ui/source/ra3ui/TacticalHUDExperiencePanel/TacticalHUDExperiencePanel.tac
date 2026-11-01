movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA4C72~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'playerTechStoreButton', 'Pointer', 'encode', 'playerExperienceMeter', 'threatMeter', 'GetPlayerTechStoreButton', 'GetPlayerExperienceMeter', 'GetThreatMeter', 'onUnload', 'initialized', 'helpSite', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
    &function GetPlayerTechStoreButton (    )    
      &pushsdbgv 0							//'playerTechStoreButton'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerTechStoreButton

    &function GetPlayerExperienceMeter (    )    
      &pushsdbgv 3							//'playerExperienceMeter'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerExperienceMeter

    &function GetThreatMeter (    )    
      &pushsdbgv 4							//'threatMeter'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetThreatMeter

    &function onUnload (    )    
      &pushsdb 5							//'GetPlayerTechStoreButton'
      &delete2
      &pop
      &pushsdb 6							//'GetPlayerExperienceMeter'
      &delete2
      &pop
      &pushsdb 7							//'GetThreatMeter'
      &delete2
      &pop
      &pushsdb 8							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 9							//'initialized'
    &not
    &not
    &jnz label1    
    &pushsdbgv 0							//'playerTechStoreButton'
    &pushsdb 10							//'helpSite'
    &pushsdbgv 10							//'helpSite'
    &setMember
    &pushsdbgv 4							//'threatMeter'
    &pushsdb 10							//'helpSite'
    &pushsdbgv 10							//'helpSite'
    &setMember
    &pushsdb 9							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 11							//'FSCommand:'
    &pushsdb 12							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 14							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 15							//''
    &getURL2
   label1:
    &stop
  &end // of frame 0

  &defineMovieClip 13 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

      &function2 onUnload () (r:1='this', r:2='_root')
        &pushregister 1
        &pushone
        &pushregister 2
        &dcallmp 0							// DetachFactionObserver()
        &pushsdb 1							//'OnCurrentFactionChanged'
        &delete2
        &pop
        &pushsdb 2							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 3							//'factionSkinned'
      &not
      &not
      &jnz label1      
      &pushzero
      &pushsdbgv 4							//'_root'
      &pushsdb 5							//'GetCurrentFaction'
      &callMethod
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'factionSkinned'
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
  &end // of defineMovieClip 13

  &defineButton 15
  
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
  &end // of defineButton 15

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &defineMovieClip 24 // total frames: 1
  &end // of defineMovieClip 24

  &defineMovieClip 27 // total frames: 1
  &end // of defineMovieClip 27

  &defineMovieClip 30 // total frames: 1
  &end // of defineMovieClip 30

  &defineMovieClip 33 // total frames: 1
  &end // of defineMovieClip 33

  &defineMovieClip 36 // total frames: 1
  &end // of defineMovieClip 36

  &defineMovieClip 39 // total frames: 1
  &end // of defineMovieClip 39

  &defineMovieClip 42 // total frames: 1
  &end // of defineMovieClip 42

  &defineMovieClip 45 // total frames: 1
  &end // of defineMovieClip 45

  &defineMovieClip 48 // total frames: 1
  &end // of defineMovieClip 48

  &defineMovieClip 51 // total frames: 1
  &end // of defineMovieClip 51

  &defineMovieClip 54 // total frames: 1
  &end // of defineMovieClip 54

  &defineMovieClip 57 // total frames: 1
  &end // of defineMovieClip 57

  &defineMovieClip 58 // total frames: 1
  &end // of defineMovieClip 58

  &defineMovieClip 60 // total frames: 1
  &end // of defineMovieClip 60

  &defineMovieClip 61 // total frames: 1

    &frame 0
      &constants 'orangeThreshold', 'redThreshold', 'Math', 'floor', 'min', 'max', 'meterMask', '_visible', 'GetPipIndex', 'meter', '_xscale', '', 'helpSite', 'Pointer', 'encode', 'Calibrate', 'SetValue', 'SetValueUsingMask', 'SetVisibility', 'GetHelpSite', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', 'extern', 'InGame', '0.67', '0.33', '0.9'  
      &function2 Calibrate (r:1='orangeThresholdArg', r:2='redThresholdArg') ()
        &pushsdb 0							//'orangeThreshold'
        &pushregister 1
        &toNumber
        &setVariable
        &pushsdb 1							//'redThreshold'
        &pushregister 2
        &toNumber
        &setVariable
      &end // of function Calibrate

      &function2 GetPipIndex (r:2='intervals', r:4='meterValue', r:3='meterRange') ()
        &pushregister 4
        &pushregister 3
        &divide
        &pushregister 2
        &multiply
        &pushone
        &pushsdbgv 2							//'Math'
        &pushsdb 3							//'floor'
        &callMethod
        &setRegister r:1
        &pop
        &pushregister 2
        &pushregister 1
        &pushbyte 2
        &pushsdbgv 2							//'Math'
        &pushsdb 4							//'min'
        &callMethod
        &pushzero
        &pushbyte 2
        &pushsdbgv 2							//'Math'
        &pushsdb 5							//'max'
        &callMethod
        &return
      &end // of function GetPipIndex

      &function2 SetValue (r:5='valArg') ()
        &pushsdbgv 6							//'meterMask'
        &pushsdb 7							//'_visible'
        &pushfalse
        &setMember
        &pushbyte 14
        &setRegister r:3
        &pop
        &pushone
        &pushregister 5
        &toNumber
        &pushregister 3
        &pushbyte 3
        &pushsdb 8							//'GetPipIndex'
        &callFunction
        &setRegister r:4
        &pop
        &pushzero
        &setRegister r:1
        &pop
       label1:
        &pushregister 1
        &pushregister 3
        &lessThan
        &not
        &jnz label2        
        &pushsdbgv 9							//'meter'
        &pushregister 1
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushregister 2
        &pushsdb 7							//'_visible'
        &pushregister 1
        &pushregister 4
        &lessThan
        &setMember
        &pushregister 1
        &increment
        &setRegister r:1
        &pop
        &jmp label1        
       label2:
      &end // of function SetValue

      &function2 SetValueUsingMask (r:5='valArg') ()
        &pushregister 5
        &toNumber
        &setRegister r:1
        &pop
        &pushfloat 0.449999988079071
        &setRegister r:2
        &pop
        &pushfloat 0.889999985694885
        &setRegister r:4
        &pop
        &pushundef
        &setRegister r:3
        &pop
        &pushregister 1
        &pushsdbgv 0							//'orangeThreshold'
        &lessThan
        &not
        &jnz label3        
        &pushregister 1
        &pushsdbgv 0							//'orangeThreshold'
        &divide
        &pushregister 2
        &multiply
        &setRegister r:3
        &pop
        &jmp label5        
       label3:
        &pushregister 1
        &pushsdbgv 1							//'redThreshold'
        &lessThan
        &not
        &jnz label4        
        &pushregister 2
        &pushregister 1
        &pushsdbgv 0							//'orangeThreshold'
        &subtract
        &pushsdbgv 1							//'redThreshold'
        &pushsdbgv 0							//'orangeThreshold'
        &subtract
        &divide
        &pushregister 4
        &pushregister 2
        &subtract
        &multiply
        &add
        &setRegister r:3
        &pop
        &jmp label5        
       label4:
        &pushregister 4
        &pushregister 1
        &pushsdbgv 1							//'redThreshold'
        &subtract
        &pushone
        &pushsdbgv 1							//'redThreshold'
        &subtract
        &divide
        &pushone
        &pushregister 4
        &subtract
        &multiply
        &add
        &setRegister r:3
        &pop
       label5:
        &pushsdbgv 6							//'meterMask'
        &pushsdb 10							//'_xscale'
        &pushbyte 100
        &pushregister 3
        &multiply
        &setMember
      &end // of function SetValueUsingMask

      &function2 SetVisibility (r:1='visArg') ()
        &pushsdb 11							//''
        &pushbyte 7
        &pushregister 1
        &toNumber
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function GetHelpSite (      )      
        &pushsdbgv 12							//'helpSite'
        &pushone
        &pushsdbgv 13							//'Pointer'
        &pushsdb 14							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function onUnload (      )      
        &pushsdb 15							//'Calibrate'
        &delete2
        &pop
        &pushsdb 8							//'GetPipIndex'
        &delete2
        &pop
        &pushsdb 16							//'SetValue'
        &delete2
        &pop
        &pushsdb 17							//'SetValueUsingMask'
        &delete2
        &pop
        &pushsdb 18							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 19							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 20							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 21							//'initialized'
      &not
      &not
      &jnz label6      
      &pushsdb 21							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 22							//'FSCommand:'
      &pushsdb 23							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 25							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 11							//''
      &getURL2
      &pushsdbgv 26							//'extern'
      &pushsdbgm 27							//'InGame'
      &not
      &not
      &jnz label6      
      &pushsdb 28							//'0.67'
      &pushsdb 29							//'0.33'
      &pushbyte 2
      &dcallfp 15							// Calibrate()
      &pushsdb 30							//'0.9'
      &pushone
      &dcallfp 16							// SetValue()
     label6:
    &end // of frame 0
  &end // of defineMovieClip 61

  &defineMovieClip 110 // total frames: 26

    &frame 0
      &constants 'SetProgress( ', ' )', 'Math', 'floor', 'frameNumStr = ', 'SetProgress', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 SetProgress (r:2='val') ()
        &pushsdb 0							//'SetProgress( '
        &pushregister 2
        &toString
        &add
        &pushsdb 1							//' )'
        &add
        &trace
        &pushregister 2
        &pushbyte 24
        &multiply
        &pushone
        &pushsdbgv 2							//'Math'
        &pushsdb 3							//'floor'
        &callMethod
        &pushone
        &add
        &toString
        &setRegister r:1
        &pop
        &pushsdb 4							//'frameNumStr = '
        &pushregister 1
        &add
        &trace
        &pushregister 1
        &toNumber
        &gotoAndStop      &end // of function SetProgress

      &function onUnload (      )      
        &pushsdb 5							//'SetProgress'
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
      &pushsdb 8							//'FSCommand:'
      &pushsdb 9							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 11							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 12							//''
      &getURL2
     label1:
    &end // of frame 0

    &frame 1
      &pushsgv 'extern'
      &pushsgm 'InGame'
      &not
      &not
      &jnz label1      
      &pushzero
      &pushsgv 'Math'
      &pushs 'random'
      &callMethod
      &pushone
      &pushs 'SetProgress'
      &callfp
     label1:
    &end // of frame 1

    &frame 1
      &stop
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2

    &frame 3
      &stop
    &end // of frame 3

    &frame 4
      &stop
    &end // of frame 4

    &frame 5
      &stop
    &end // of frame 5

    &frame 6
      &stop
    &end // of frame 6

    &frame 7
      &stop
    &end // of frame 7

    &frame 8
      &stop
    &end // of frame 8

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &stop
    &end // of frame 10

    &frame 11
      &stop
    &end // of frame 11

    &frame 12
      &stop
    &end // of frame 12

    &frame 13
      &stop
    &end // of frame 13

    &frame 14
      &stop
    &end // of frame 14

    &frame 15
      &stop
    &end // of frame 15

    &frame 16
      &stop
    &end // of frame 16

    &frame 17
      &stop
    &end // of frame 17

    &frame 18
      &stop
    &end // of frame 18

    &frame 19
      &stop
    &end // of frame 19

    &frame 20
      &stop
    &end // of frame 20

    &frame 21
      &stop
    &end // of frame 21

    &frame 22
      &stop
    &end // of frame 22

    &frame 23
      &stop
    &end // of frame 23

    &frame 24
      &stop
    &end // of frame 24

    &frame 25
      &stop
    &end // of frame 25
  &end // of defineMovieClip 110

  &defineButton 112
  
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
  &end // of defineButton 112

  &defineMovieClip 115 // total frames: 1
  &end // of defineMovieClip 115

  &defineMovieClip 116 // total frames: 26

    &frame 4
      &stop
    &end // of frame 4

    &frame 25
      &gotoLabel '_on'
      &play
    &end // of frame 25
  &end // of defineMovieClip 116

  &defineMovieClip 139 // total frames: 55

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

    &frame 29
      &stop
    &end // of frame 29

    &frame 34
      &stop
    &end // of frame 34

    &frame 39
      &stop
    &end // of frame 39

    &frame 44
      &stop
    &end // of frame 44

    &frame 49
      &stop
    &end // of frame 49

    &frame 54
      &stop
    &end // of frame 54
  &end // of defineMovieClip 139

  &defineMovieClip 140 // total frames: 1

    &frame 0
      &constants 'isEnabled', 'Color', 'setRGB', 'isHighlighted', 'playerTechStoreButtonHighlight', 'done', 'on', 'gotoAndPlay', 'isFlashing', '_off', '_on', 'flashyThingy', 'ugly', 'Start', 'Stop', 'helpSite', 'Pointer', 'encode', '_', 'techPoints', 'SetEnabled', 'SetHighlighted', 'SetFlashing', 'GetHelpSite', 'SetTechPoints', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', '', 'extern', 'InGame'  
      &function2 SetEnabled (r:4='enabledArg') (r:1='this')
        &pushregister 4
        &pushzero
        &equals
        &not
        &setRegister r:2
        &pop
        &pushregister 2
        &pushsdbgv 0							//'isEnabled'
        &equals
        &not
        &not
        &jnz label3        
        &pushsdb 0							//'isEnabled'
        &pushregister 2
        &setVariable
        &pushregister 1
        &pushone
        &pushsdb 1							//'Color'
        &new
        &setRegister r:3
        &pop
        &pushsdbgv 0							//'isEnabled'
        &jnz label1        
        &pushlong 8355711
        &jmp label2        
       label1:
        &pushlong 16777215
       label2:
        &pushone
        &pushregister 3
        &dcallmp 2							// setRGB()
       label3:
      &end // of function SetEnabled

      &function2 SetHighlighted (r:4='highlightArg') (r:1='_parent')
        &pushregister 4
        &pushzero
        &equals
        &not
        &setRegister r:3
        &pop
        &pushregister 3
        &pushsdbgv 3							//'isHighlighted'
        &equals
        &not
        &not
        &jnz label5        
        &pushsdb 3							//'isHighlighted'
        &pushregister 3
        &setVariable
        &pushregister 1
        &pushsdbgm 4							//'playerTechStoreButtonHighlight'
        &setRegister r:2
        &pop
        &pushsdbgv 3							//'isHighlighted'
        &not
        &jnz label4        
        &pushregister 2
        &pushsdb 5							//'done'
        &pushfalse
        &setMember
        &pushsdb 6							//'on'
        &pushone
        &pushregister 2
        &dcallmp 7							// gotoAndPlay()
        &jmp label5        
       label4:
        &pushregister 2
        &pushsdb 5							//'done'
        &pushtrue
        &setMember
       label5:
      &end // of function SetHighlighted

      &function2 SetFlashing (r:4='flashingArg') (r:1='_parent')
        &pushregister 4
        &pushzero
        &equals
        &not
        &setRegister r:2
        &pop
        &pushregister 2
        &pushsdbgv 8							//'isFlashing'
        &equals
        &not
        &not
        &jnz label9        
        &pushsdb 8							//'isFlashing'
        &pushregister 2
        &setVariable
        &pushregister 2
        &jnz label6        
        &pushsdb 9							//'_off'
        &jmp label7        
       label6:
        &pushsdb 10							//'_on'
       label7:
        &setRegister r:3
        &pop
        &pushregister 3
        &pushone
        &pushsdbgv 11							//'flashyThingy'
        &dcallmp 7							// gotoAndPlay()
        &pushregister 2
        &not
        &jnz label8        
        &pushzero
        &pushregister 1
        &pushsdbgm 12							//'ugly'
        &dcallmp 13							// Start()
        &jmp label9        
       label8:
        &pushzero
        &pushregister 1
        &pushsdbgm 12							//'ugly'
        &dcallmp 14							// Stop()
       label9:
      &end // of function SetFlashing

      &function GetHelpSite (      )      
        &pushsdbgv 15							//'helpSite'
        &pushone
        &pushsdbgv 16							//'Pointer'
        &pushsdb 17							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function2 SetTechPoints (r:1='points') ()
        &pushsdb 18							//'_'
        &pushregister 1
        &toString
        &add
        &pushone
        &pushsdbgv 19							//'techPoints'
        &dcallmp 7							// gotoAndPlay()
      &end // of function SetTechPoints

      &function onUnload (      )      
        &pushsdb 20							//'SetEnabled'
        &delete2
        &pop
        &pushsdb 21							//'SetHighlighted'
        &delete2
        &pop
        &pushsdb 22							//'SetFlashing'
        &delete2
        &pop
        &pushsdb 23							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 24							//'SetTechPoints'
        &delete2
        &pop
        &pushsdb 25							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 26							//'initialized'
      &not
      &not
      &jnz label10      
      &pushsdb 0							//'isEnabled'
      &pushtrue
      &setVariable
      &pushsdb 3							//'isHighlighted'
      &pushfalse
      &setVariable
      &pushsdb 8							//'isFlashing'
      &pushfalse
      &setVariable
      &pushsdb 26							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 27							//'FSCommand:'
      &pushsdb 28							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 30							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 31							//''
      &getURL2
      &pushsdbgv 32							//'extern'
      &pushsdbgm 33							//'InGame'
      &not
      &not
      &jnz label10      
      &pushone
      &pushone
      &dcallfp 22							// SetFlashing()
     label10:
    &end // of frame 0
  &end // of defineMovieClip 140

  &defineMovieClip 143 // total frames: 1
  &end // of defineMovieClip 143

  &defineMovieClip 144 // total frames: 36

    &frame 4
      &stop
    &end // of frame 4

    &frame 35
      &pushsgv 'done'
      &jnz label1      
      &pushs 'on'
      &jmp label2      
     label1:
      &pushs 'off'
     label2:
      &gotoAndPlay    &end // of frame 35
  &end // of defineMovieClip 144

  &defineMovieClip 147 // total frames: 1
  &end // of defineMovieClip 147

  &defineMovieClip 148 // total frames: 10

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 148

  &defineMovieClip 151 // total frames: 1
  &end // of defineMovieClip 151

  &defineMovieClip 154 // total frames: 1
  &end // of defineMovieClip 154

  &defineMovieClip 157 // total frames: 1
  &end // of defineMovieClip 157

  &defineMovieClip 158 // total frames: 21

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &pushzero
      &pushsgv 'client'
      &pushs 'NextFlash'
      &callmp
    &end // of frame 19

    &frame 20
      &stop
    &end // of frame 20
  &end // of defineMovieClip 158

  &defineMovieClip 159 // total frames: 1

    &frame 0
      &constants 'cyclesLeft', 'cycleCount', 'currentArrowNum', '_flash', 'gotoAndPlay', '', '_off', 'arrowCount', 'sunburst', 'Start', 'Stop', 'NextFlash', 'onUnload', 'initialized', 'i', 'arrowClip', 'this', 'client', 'extern', 'InGame', 'onEnterFrame'  
      &function2 Start () (r:1='this')
        &stop
        &pushsdb 0							//'cyclesLeft'
        &pushsdbgv 1							//'cycleCount'
        &setVariable
        &pushsdb 2							//'currentArrowNum'
        &pushzerosv
        &pushregister 1
        &pushsdbgv 2							//'currentArrowNum'
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushsdb 3							//'_flash'
        &pushone
        &pushregister 2
        &dcallmp 4							// gotoAndPlay()
        &pushsdb 5							//''
        &pushbyte 7
        &pushtrue
        &setProperty
      &end // of function Start

      &function2 Stop () (r:1='this')
        &pushsdbgv 0							//'cyclesLeft'
        &pushzero
        &greaterThan
        &not
        &jnz label1        
        &pushregister 1
        &pushsdbgv 2							//'currentArrowNum'
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushsdb 6							//'_off'
        &pushone
        &pushregister 2
        &dcallmp 4							// gotoAndPlay()
        &pushsdb 0							//'cyclesLeft'
        &pushzerosv
        &pushsdb 5							//''
        &pushbyte 7
        &pushfalse
        &setProperty
       label1:
      &end // of function Stop

      &function2 NextFlash () (r:1='this')
        &pushsdbgv 0							//'cyclesLeft'
        &pushzero
        &greaterThan
        &not
        &not
        &jnz label2        
        &pushundef
        &return
       label2:
        &pushsdb 2							//'currentArrowNum'
        &pushsdbgv 2							//'currentArrowNum'
        &increment
        &setVariable
        &pushsdbgv 2							//'currentArrowNum'
        &pushsdbgv 7							//'arrowCount'
        &lessThan
        &not
        &not
        &jnz label4        
        &pushsdb 0							//'cyclesLeft'
        &pushsdbgv 0							//'cyclesLeft'
        &decrement
        &setVariable
        &pushsdb 3							//'_flash'
        &pushone
        &pushsdbgv 8							//'sunburst'
        &dcallmp 4							// gotoAndPlay()
        &pushsdbgv 0							//'cyclesLeft'
        &pushzero
        &greaterThan
        &not
        &not
        &jnz label3        
        &pushsdb 5							//''
        &pushbyte 7
        &pushfalse
        &setProperty
        &pushundef
        &return
       label3:
        &pushsdb 2							//'currentArrowNum'
        &pushzerosv
       label4:
        &pushregister 1
        &pushsdbgv 2							//'currentArrowNum'
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushsdb 3							//'_flash'
        &pushone
        &pushregister 2
        &dcallmp 4							// gotoAndPlay()
      &end // of function NextFlash

      &function onUnload (      )      
        &pushsdb 9							//'Start'
        &delete2
        &pop
        &pushsdb 10							//'Stop'
        &delete2
        &pop
        &pushsdb 11							//'NextFlash'
        &delete2
        &pop
        &pushsdb 12							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 13							//'initialized'
      &not
      &not
      &jnz label7      
      &pushsdb 5							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 7							//'arrowCount'
      &pushbyte 5
      &setVariable
      &pushsdb 1							//'cycleCount'
      &pushbyte 5
      &setVariable
      &pushsdb 14							//'i'
      &pushzero
      &varEquals
     label5:
      &pushsdbgv 14							//'i'
      &pushsdbgv 7							//'arrowCount'
      &lessThan
      &not
      &jnz label6      
      &pushsdb 15							//'arrowClip'
      &pushthisgv
      &pushsdbgv 14							//'i'
      &toString
      &getMember
      &varEquals
      &pushsdbgv 15							//'arrowClip'
      &pushsdb 17							//'client'
      &pushthisgv
      &setMember
      &pushsdb 14							//'i'
      &pushsdbgv 14							//'i'
      &increment
      &setVariable
      &jmp label5      
     label6:
      &pushsdb 13							//'initialized'
      &pushtrue
      &setVariable
      &pushsdbgv 18							//'extern'
      &pushsdbgm 19							//'InGame'
      &not
      &not
      &jnz label7      
      &pushsdb 20							//'onEnterFrame'
      &function  (      )      
        &pushzero
        &dcallfp 9							// Start()
        &pushsdb 20							//'onEnterFrame'
        &delete2
        &pop
      &end // of function 

      &setVariable
     label7:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 159
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 160
  &end // of importAssets

  &placeMovieClip 160 &as 'helpSite'
  
    &onClipEvent &construct
      &pushs 'vertAlignment'
      &pushssv 'bottom'
      &pushs 'horzAlignment'
      &pushssv 'left'
    &end
  &end // of placeMovieClip 160
&end
