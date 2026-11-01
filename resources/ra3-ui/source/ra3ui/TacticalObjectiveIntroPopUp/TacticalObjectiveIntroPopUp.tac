movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TAA2A0~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 136, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'popUpMC', 'titleTF', 'textColor', 'dividerMC', 'Color', 'setRGB', '_primary', '_bonus', 'outerFrameMC', 'SetOuterFrameState', 'innerFrameMC', 'SetInnerFrameState', 'symbolMC', 'gotoAndPlay', 'closing', 'bl', '_y', 'bottom', 'br', 'left', '_height', 'right', 'bg', 'tl', 'top', 'tr', 'SetObjectiveType', 'Close', 'AdjustOuterFrameHeight', 'AdjustInnerFrameHeight', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'origTextHeight', 'textTF', 'text', '$', 'Title', '_global', 'qualifyName', '&outline', 'autoSize', 'Text', 'newTextHeight', 'heightIncrease', 'FSCommand:', 'OnInitialized', ''  
    &function2 SetObjectiveType (r:7='cat', r:8='bonusArg') ()
      &pushregister 8
      &pushzero
      &equals
      &not
      &setRegister r:3
      &pop
      &pushlong 11993018
      &setRegister r:4
      &pop
      &pushlong 16772674
      &setRegister r:6
      &pop
      &pushregister 3
      &jnz label1      
      &pushregister 4
      &jmp label2      
     label1:
      &pushregister 6
     label2:
      &setRegister r:2
      &pop
      &pushsdbgv 0							//'popUpMC'
      &pushsdbgm 1							//'titleTF'
      &pushsdb 2							//'textColor'
      &pushregister 2
      &setMember
      &pushsdbgv 0							//'popUpMC'
      &pushsdbgm 3							//'dividerMC'
      &pushone
      &pushsdb 4							//'Color'
      &new
      &setRegister r:5
      &pop
      &pushregister 2
      &pushone
      &pushregister 5
      &dcallmp 5							// setRGB()
      &pushregister 3
      &jnz label3      
      &pushsdb 6							//'_primary'
      &jmp label4      
     label3:
      &pushsdb 7							//'_bonus'
     label4:
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 0							//'popUpMC'
      &pushsdbgm 8							//'outerFrameMC'
      &pushbyte 2
      &dcallfp 9							// SetOuterFrameState()
      &pushregister 1
      &pushsdbgv 0							//'popUpMC'
      &pushsdbgm 10							//'innerFrameMC'
      &pushbyte 2
      &dcallfp 11							// SetInnerFrameState()
      &pushregister 7
      &pushregister 1
      &add
      &pushone
      &pushsdbgv 0							//'popUpMC'
      &pushsdbgm 12							//'symbolMC'
      &dcallmp 13							// gotoAndPlay()
    &end // of function SetObjectiveType

    &function Close (    )    
      &pushsdbgv 14							//'closing'
      &not
      &not
      &jnz label5      
      &gotoLabel '_close'
      &play
      &pushsdb 14							//'closing'
      &pushtrue
      &setVariable
     label5:
    &end // of function Close

    &function2 AdjustOuterFrameHeight (r:1='frameMC', r:2='heightChange') ()
      &pushregister 1
      &pushsdbgm 15							//'bl'
      &pushsdb 16							//'_y'
      &pushregister 1
      &pushsdbgm 15							//'bl'
      &pushsdbgm 16							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 17							//'bottom'
      &pushsdb 16							//'_y'
      &pushregister 1
      &pushsdbgm 17							//'bottom'
      &pushsdbgm 16							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 18							//'br'
      &pushsdb 16							//'_y'
      &pushregister 1
      &pushsdbgm 18							//'br'
      &pushsdbgm 16							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 19							//'left'
      &pushsdb 20							//'_height'
      &pushregister 1
      &pushsdbgm 19							//'left'
      &pushsdbgm 20							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 21							//'right'
      &pushsdb 20							//'_height'
      &pushregister 1
      &pushsdbgm 21							//'right'
      &pushsdbgm 20							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 22							//'bg'
      &pushsdb 20							//'_height'
      &pushregister 1
      &pushsdbgm 22							//'bg'
      &pushsdbgm 20							//'_height'
      &pushregister 2
      &add
      &setMember
    &end // of function AdjustOuterFrameHeight

    &function2 SetOuterFrameState (r:1='frameMC', r:2='stateLabel') ()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 23							//'tl'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 24							//'top'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 25							//'tr'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 21							//'right'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 18							//'br'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 17							//'bottom'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 15							//'bl'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 19							//'left'
      &dcallmp 13							// gotoAndPlay()
    &end // of function SetOuterFrameState

    &function2 AdjustInnerFrameHeight (r:1='frameMC', r:2='heightChange') ()
      &pushregister 1
      &pushsdbgm 17							//'bottom'
      &pushsdb 16							//'_y'
      &pushregister 1
      &pushsdbgm 17							//'bottom'
      &pushsdbgm 16							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 19							//'left'
      &pushsdb 20							//'_height'
      &pushregister 1
      &pushsdbgm 19							//'left'
      &pushsdbgm 20							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 21							//'right'
      &pushsdb 20							//'_height'
      &pushregister 1
      &pushsdbgm 21							//'right'
      &pushsdbgm 20							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 22							//'bg'
      &pushsdb 20							//'_height'
      &pushregister 1
      &pushsdbgm 22							//'bg'
      &pushsdbgm 20							//'_height'
      &pushregister 2
      &add
      &setMember
    &end // of function AdjustInnerFrameHeight

    &function2 SetInnerFrameState (r:1='frameMC', r:2='stateLabel') ()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 24							//'top'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 21							//'right'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 17							//'bottom'
      &dcallmp 13							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 19							//'left'
      &dcallmp 13							// gotoAndPlay()
    &end // of function SetInnerFrameState

    &function onUnload (    )    
      &pushsdb 26							//'SetObjectiveType'
      &delete2
      &pop
      &pushsdb 27							//'Close'
      &delete2
      &pop
      &pushsdb 28							//'AdjustOuterFrameHeight'
      &delete2
      &pop
      &pushsdb 9							//'SetOuterFrameState'
      &delete2
      &pop
      &pushsdb 29							//'AdjustInnerFrameHeight'
      &delete2
      &pop
      &pushsdb 11							//'SetInnerFrameState'
      &delete2
      &pop
      &pushsdb 30							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 31							//'initialized'
    &not
    &not
    &jnz label7    
    &pushthisgv
    &pushone
    &pushsdbgv 33							//'_parent'
    &dcallmp 34							// OnContentLoaded()
    &pushsdb 35							//'origTextHeight'
    &pushsdbgv 0							//'popUpMC'
    &pushsdbgm 36							//'textTF'
    &pushsdbgm 20							//'_height'
    &varEquals
    &pushsdbgv 0							//'popUpMC'
    &pushsdbgm 1							//'titleTF'
    &pushsdb 37							//'text'
    &pushsdb 38							//'$'
    &pushsdb 39							//'Title'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 41							//'qualifyName'
    &callMethod
    &add
    &pushsdb 42							//'&outline'
    &add
    &setMember
    &pushsdbgv 0							//'popUpMC'
    &pushsdbgm 36							//'textTF'
    &pushsdb 43							//'autoSize'
    &pushsdb 19							//'left'
    &setMember
    &pushsdbgv 0							//'popUpMC'
    &pushsdbgm 36							//'textTF'
    &pushsdb 37							//'text'
    &pushsdb 38							//'$'
    &pushsdb 44							//'Text'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 41							//'qualifyName'
    &callMethod
    &add
    &pushsdb 42							//'&outline'
    &add
    &setMember
    &pushsdb 45							//'newTextHeight'
    &pushsdbgv 0							//'popUpMC'
    &pushsdbgm 36							//'textTF'
    &pushsdbgm 20							//'_height'
    &varEquals
    &pushsdbgv 45							//'newTextHeight'
    &pushsdbgv 35							//'origTextHeight'
    &greaterThan
    &not
    &jnz label6    
    &pushsdb 46							//'heightIncrease'
    &pushsdbgv 45							//'newTextHeight'
    &pushsdbgv 35							//'origTextHeight'
    &subtract
    &varEquals
    &pushsdbgv 46							//'heightIncrease'
    &pushsdbgv 0							//'popUpMC'
    &pushsdbgm 8							//'outerFrameMC'
    &pushbyte 2
    &dcallfp 28							// AdjustOuterFrameHeight()
    &pushsdbgv 46							//'heightIncrease'
    &pushsdbgv 0							//'popUpMC'
    &pushsdbgm 10							//'innerFrameMC'
    &pushbyte 2
    &dcallfp 29							// AdjustInnerFrameHeight()
   label6:
    &pushsdb 14							//'closing'
    &pushfalse
    &setVariable
    &pushsdb 31							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 47							//'FSCommand:'
    &pushsdb 48							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 41							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 49							//''
    &getURL2
   label7:
  &end // of frame 0

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 7 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 7

  &defineMovieClip 12 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 19 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 2
  &end // of defineMovieClip 20

  &defineMovieClip 41 // total frames: 50

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
  &end // of defineMovieClip 41

  &defineMovieClip 46 // total frames: 1
  &end // of defineMovieClip 46

  &frame 126
    &pushs 'closing'
    &pushtrue
    &setVariable
  &end // of frame 126

  &frame 135
    &pushs 'FSCommand:'
    &pushs 'OnClosed'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
    &stop
  &end // of frame 135
&end
