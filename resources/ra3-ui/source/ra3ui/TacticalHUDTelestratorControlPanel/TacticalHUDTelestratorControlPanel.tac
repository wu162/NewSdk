movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA6512~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'panel', '_visible', 'isOpen', '_maximize', 'titleBox', 'openToggle', 'gotoAndStop', '_open', 'gotoAndPlay', '_max', 'TitleFrame', '_minimize', '_close', '_mini', 'extern', 'InGame', 'FSCommand:', 'OnOpenToggleClicked', 'qualifyName', '', 'Close', 'Open', 'selectedColorIndex', 'box', 'SelectColorButton', 'OnColorClicked', 'SelectColor', 'selectedLineWidthIndex', 'SelectLineWidthButton', 'OnLineWidthClicked', 'SelectLineWidth', 'OnEraseAllClicked', 'SetVisibility', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'OnInitialized'  
    &function2 SetVisibility (r:2='visArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushsdbgv 0							//'panel'
      &pushsdb 1							//'_visible'
      &pushregister 1
      &setMember
    &end // of function SetVisibility

    &function Open (    )    
      &pushsdbgv 2							//'isOpen'
      &not
      &not
      &jnz label1      
      &pushsdb 3							//'_maximize'
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 4							//'titleBox'
      &pushsdbgm 5							//'openToggle'
      &dcallmp 6							// gotoAndStop()
      &pushsdb 7							//'_open'
      &pushone
      &pushsdbgv 0							//'panel'
      &dcallmp 8							// gotoAndPlay()
      &pushsdb 9							//'_max'
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 4							//'titleBox'
      &pushsdbgm 10							//'TitleFrame'
      &dcallmp 8							// gotoAndPlay()
      &pushsdb 2							//'isOpen'
      &pushtrue
      &setVariable
     label1:
    &end // of function Open

    &function Close (    )    
      &pushsdbgv 2							//'isOpen'
      &not
      &jnz label2      
      &pushsdb 11							//'_minimize'
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 4							//'titleBox'
      &pushsdbgm 5							//'openToggle'
      &dcallmp 6							// gotoAndStop()
      &pushsdb 12							//'_close'
      &pushone
      &pushsdbgv 0							//'panel'
      &dcallmp 8							// gotoAndPlay()
      &pushsdb 13							//'_mini'
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 4							//'titleBox'
      &pushsdbgm 10							//'TitleFrame'
      &dcallmp 8							// gotoAndPlay()
      &pushsdb 2							//'isOpen'
      &pushfalse
      &setVariable
     label2:
    &end // of function Close

    &function2 OnOpenToggleClicked () (r:1='this')
      &pushsdbgv 14							//'extern'
      &pushsdbgm 15							//'InGame'
      &not
      &jnz label3      
      &pushsdb 16							//'FSCommand:'
      &pushsdb 17							//'OnOpenToggleClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 19							//''
      &getURL2
      &jmp label5      
     label3:
      &pushsdbgv 2							//'isOpen'
      &not
      &jnz label4      
      &pushzero
      &dcallfp 20							// Close()
      &jmp label5      
     label4:
      &pushzero
      &dcallfp 21							// Open()
     label5:
    &end // of function OnOpenToggleClicked

    &function2 SelectColor (r:2='colorIndexArg') ()
      &pushregister 2
      &toNumber
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 22							//'selectedColorIndex'
      &equals
      &not
      &not
      &jnz label6      
      &pushsdb 22							//'selectedColorIndex'
      &pushregister 1
      &setVariable
      &pushregister 1
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 23							//'box'
      &dcallmp 24							// SelectColorButton()
     label6:
    &end // of function SelectColor

    &function2 OnColorClicked (r:2='colorIndex') (r:1='this')
      &pushsdbgv 14							//'extern'
      &pushsdbgm 15							//'InGame'
      &not
      &jnz label7      
      &pushsdb 16							//'FSCommand:'
      &pushsdb 25							//'OnColorClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &concat
      &pushregister 2
      &toString
      &getURL2
      &jmp label8      
     label7:
      &pushregister 2
      &pushone
      &dcallfp 26							// SelectColor()
     label8:
    &end // of function OnColorClicked

    &function2 SelectLineWidth (r:2='lineWidthIndexArg') ()
      &pushregister 2
      &toNumber
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 27							//'selectedLineWidthIndex'
      &equals
      &not
      &not
      &jnz label9      
      &pushsdb 27							//'selectedLineWidthIndex'
      &pushregister 1
      &setVariable
      &pushregister 1
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 23							//'box'
      &dcallmp 28							// SelectLineWidthButton()
     label9:
    &end // of function SelectLineWidth

    &function2 OnLineWidthClicked (r:2='lineWidthIndex') (r:1='this')
      &pushsdbgv 14							//'extern'
      &pushsdbgm 15							//'InGame'
      &not
      &jnz label10      
      &pushsdb 16							//'FSCommand:'
      &pushsdb 29							//'OnLineWidthClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &concat
      &pushregister 2
      &toString
      &getURL2
      &jmp label11      
     label10:
      &pushregister 2
      &pushone
      &dcallfp 30							// SelectLineWidth()
     label11:
    &end // of function OnLineWidthClicked

    &function2 OnEraseAllClicked () (r:1='this')
      &pushsdb 16							//'FSCommand:'
      &pushsdb 31							//'OnEraseAllClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 19							//''
      &getURL2
    &end // of function OnEraseAllClicked

    &function onUnload (    )    
      &pushsdb 21							//'Open'
      &delete2
      &pop
      &pushsdb 20							//'Close'
      &delete2
      &pop
      &pushsdb 17							//'OnOpenToggleClicked'
      &delete2
      &pop
      &pushsdb 26							//'SelectColor'
      &delete2
      &pop
      &pushsdb 25							//'OnColorClicked'
      &delete2
      &pop
      &pushsdb 30							//'SelectLineWidth'
      &delete2
      &pop
      &pushsdb 29							//'OnLineWidthClicked'
      &delete2
      &pop
      &pushsdb 31							//'OnEraseAllClicked'
      &delete2
      &pop
      &pushsdb 32							//'SetVisibility'
      &delete2
      &pop
      &pushsdb 33							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 34							//'initialized'
    &not
    &not
    &jnz label12    
    &pushthisgv
    &pushone
    &pushsdbgv 36							//'_parent'
    &dcallmp 37							// OnContentLoaded()
    &pushsdb 2							//'isOpen'
    &pushfalse
    &setVariable
    &pushsdb 22							//'selectedColorIndex'
    &pushzerosv
    &pushsdb 27							//'selectedLineWidthIndex'
    &pushzerosv
    &pushsdb 34							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 16							//'FSCommand:'
    &pushsdb 38							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 18							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 19							//''
    &getURL2
   label12:
    &stop
  &end // of frame 0

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineButton 6
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &constants 'this', '_parent', 'OnColorButtonClicked'  
      &gotoLabel '_over'
      &play
      &pushthisgv
      &pushone
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &dcallmp 2							// OnColorButtonClicked()
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 6
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'WhiteRectangle' &as 7
  &end // of importAssets

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 12 // total frames: 40

    &placeMovieClip 7 
    
      &onClipEvent &load
        &constants 'boxColor', '_parent', 'red', 'green', 'blue', 'colorizer', 'this', 'Color', 'setRGB'  
        &pushsdb 0							//'boxColor'
        &pushsdbgv 1							//'_parent'
        &pushsdbgm 2							//'red'
        &pushbyte 16
        &shiftLeft
        &pushsdbgv 1							//'_parent'
        &pushsdbgm 3							//'green'
        &pushbyte 8
        &shiftLeft
        &bitwiseOr
        &pushsdbgv 1							//'_parent'
        &pushsdbgm 4							//'blue'
        &bitwiseOr
        &varEquals
        &pushsdb 5							//'colorizer'
        &pushthisgv
        &pushone
        &pushsdb 7							//'Color'
        &new
        &setVariable
        &pushsdbgv 0							//'boxColor'
        &pushone
        &pushsdbgv 5							//'colorizer'
        &dcallmp 8							// setRGB()
      &end
    &end // of placeMovieClip 7

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
  &end // of defineMovieClip 12

  &defineMovieClip 14 // total frames: 1

    &placeMovieClip 12 &as '0'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushshort 255
        &setVariable
        &pushs 'blue'
        &pushshort 255
        &setVariable
        &pushs 'green'
        &pushshort 255
        &setVariable
      &end
    &end // of placeMovieClip 12

    &placeMovieClip 12 &as '1'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushshort 255
        &setVariable
        &pushs 'blue'
        &pushzerosv
        &pushs 'green'
        &pushzerosv
      &end
    &end // of placeMovieClip 12

    &placeMovieClip 12 &as '2'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushzerosv
        &pushs 'blue'
        &pushzerosv
        &pushs 'green'
        &pushshort 255
        &setVariable
      &end
    &end // of placeMovieClip 12

    &placeMovieClip 12 &as '3'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushzerosv
        &pushs 'blue'
        &pushshort 255
        &setVariable
        &pushs 'green'
        &pushzerosv
      &end
    &end // of placeMovieClip 12

    &placeMovieClip 12 &as '4'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushshort 255
        &setVariable
        &pushs 'blue'
        &pushzerosv
        &pushs 'green'
        &pushshort 255
        &setVariable
      &end
    &end // of placeMovieClip 12

    &placeMovieClip 12 &as '5'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushzerosv
        &pushs 'blue'
        &pushshort 255
        &setVariable
        &pushs 'green'
        &pushshort 255
        &setVariable
      &end
    &end // of placeMovieClip 12

    &placeMovieClip 12 &as '6'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushshort 255
        &setVariable
        &pushs 'blue'
        &pushshort 255
        &setVariable
        &pushs 'green'
        &pushzerosv
      &end
    &end // of placeMovieClip 12

    &placeMovieClip 12 &as '7'
    
      &onClipEvent &construct
        &pushs 'red'
        &pushzerosv
        &pushs 'blue'
        &pushzerosv
        &pushs 'green'
        &pushzerosv
      &end
    &end // of placeMovieClip 12
  &end // of defineMovieClip 14

  &defineButton 15
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &constants '_parent', 'OnEraseAllClicked'  
      &gotoLabel '_over'
      &play
      &pushzero
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &dcallmp 1							// OnEraseAllClicked()
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 15

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 20

  &defineButton 22
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &constants 'this', '_parent', 'OnLineWidthButtonClicked'  
      &gotoLabel '_over'
      &play
      &pushthisgv
      &pushone
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &dcallmp 2							// OnLineWidthButtonClicked()
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 22

  &defineMovieClip 27 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 27

  &defineMovieClip 28 // total frames: 40

    &placeMovieClip 27 &as 'image'
    
      &onClipEvent &load
        &pushsgv '_parent'
        &pushsgm 'image'
        &gotoAndPlay      &end
    &end // of placeMovieClip 27

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
  &end // of defineMovieClip 28

  &defineMovieClip 29 // total frames: 1

    &placeMovieClip 28 &as '0'
    
      &onClipEvent &load
        &pushs 'image'
        &pushssv '_thin'
      &end
    &end // of placeMovieClip 28

    &placeMovieClip 28 &as '1'
    
      &onClipEvent &load
        &pushs 'image'
        &pushssv '_thick'
      &end
    &end // of placeMovieClip 28
  &end // of defineMovieClip 29

  &defineMovieClip 30 // total frames: 1

    &frame 0
      &constants 'colors', '_up', 'selectedColorButton', 'gotoAndPlay', '_selected', '_name', '_parent', 'OnColorClicked', 'lineWidths', 'selectedLineWidthButton', 'OnLineWidthClicked', 'SelectColorButton', 'OnColorButtonClicked', 'SelectLineWidthButton', 'OnLineWidthButtonClicked', 'onUnload', 'initialized', 'selectedColorIndex', 'selectedLineWidthIndex'  
      &function2 SelectColorButton (r:2='colorIndex') ()
        &pushsdbgv 0							//'colors'
        &pushregister 2
        &toString
        &getMember
        &setRegister r:1
        &pop
        &pushsdb 1							//'_up'
        &pushone
        &pushsdbgv 2							//'selectedColorButton'
        &dcallmp 3							// gotoAndPlay()
        &pushsdb 2							//'selectedColorButton'
        &pushregister 1
        &setVariable
        &pushsdb 4							//'_selected'
        &pushone
        &pushregister 1
        &dcallmp 3							// gotoAndPlay()
      &end // of function SelectColorButton

      &function2 OnColorButtonClicked (r:2='colorButton') (r:1='_parent')
        &pushregister 2
        &pushsdbgm 5							//'_name'
        &toNumber
        &pushone
        &pushregister 1
        &pushsdbgm 6							//'_parent'
        &dcallmp 7							// OnColorClicked()
      &end // of function OnColorButtonClicked

      &function2 SelectLineWidthButton (r:2='lineWidthIndex') ()
        &pushsdbgv 8							//'lineWidths'
        &pushregister 2
        &toString
        &getMember
        &setRegister r:1
        &pop
        &pushsdb 1							//'_up'
        &pushone
        &pushsdbgv 9							//'selectedLineWidthButton'
        &dcallmp 3							// gotoAndPlay()
        &pushsdb 9							//'selectedLineWidthButton'
        &pushregister 1
        &setVariable
        &pushsdb 4							//'_selected'
        &pushone
        &pushregister 1
        &dcallmp 3							// gotoAndPlay()
      &end // of function SelectLineWidthButton

      &function2 OnLineWidthButtonClicked (r:2='lineWidthButton') (r:1='_parent')
        &pushregister 2
        &pushsdbgm 5							//'_name'
        &toNumber
        &pushone
        &pushregister 1
        &pushsdbgm 6							//'_parent'
        &dcallmp 10							// OnLineWidthClicked()
      &end // of function OnLineWidthButtonClicked

      &function onUnload (      )      
        &pushsdb 11							//'SelectColorButton'
        &delete2
        &pop
        &pushsdb 12							//'OnColorButtonClicked'
        &delete2
        &pop
        &pushsdb 13							//'SelectLineWidthButton'
        &delete2
        &pop
        &pushsdb 14							//'OnLineWidthButtonClicked'
        &delete2
        &pop
        &pushsdb 15							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 16							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdb 17							//'selectedColorIndex'
      &pushsdbgv 6							//'_parent'
      &pushsdbgm 6							//'_parent'
      &pushsdbgm 17							//'selectedColorIndex'
      &varEquals
      &pushsdbgv 17							//'selectedColorIndex'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushsdb 2							//'selectedColorButton'
      &pushsdbgv 0							//'colors'
      &pushsdbgv 17							//'selectedColorIndex'
      &toString
      &getMember
      &setVariable
      &pushsdb 4							//'_selected'
      &pushone
      &pushsdbgv 2							//'selectedColorButton'
      &dcallmp 3							// gotoAndPlay()
     label1:
      &pushsdb 18							//'selectedLineWidthIndex'
      &pushsdbgv 6							//'_parent'
      &pushsdbgm 6							//'_parent'
      &pushsdbgm 18							//'selectedLineWidthIndex'
      &varEquals
      &pushsdbgv 18							//'selectedLineWidthIndex'
      &pushundef
      &equals
      &not
      &not
      &jnz label2      
      &pushsdb 9							//'selectedLineWidthButton'
      &pushsdbgv 8							//'lineWidths'
      &pushsdbgv 18							//'selectedLineWidthIndex'
      &toString
      &getMember
      &setVariable
      &pushsdb 4							//'_selected'
      &pushone
      &pushsdbgv 9							//'selectedLineWidthButton'
      &dcallmp 3							// gotoAndPlay()
     label2:
      &pushsdb 16							//'initialized'
      &pushtrue
      &setVariable
     label3:
    &end // of frame 0
  &end // of defineMovieClip 30

  &defineMovieClip 33 // total frames: 1
  &end // of defineMovieClip 33

  &defineMovieClip 38 // total frames: 20

    &frame 0
      &stop
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 38

  &defineButton 42
  
    &on     &overUpToOverDown
      &constants '_parent', 'OnOpenToggleClicked'  
      &pushzero
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &dcallmp 1							// OnOpenToggleClicked()
    &end
  &end // of defineButton 42

  &defineMovieClip 45 // total frames: 20

    &frame 0
      &stop
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 45

  &defineMovieClip 48 // total frames: 1
  &end // of defineMovieClip 48

  &defineMovieClip 49 // total frames: 1

    &placeMovieClip 48 
    
      &onClipEvent &load
        &constants 'text', '$', 'Title', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Title'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 48
  &end // of defineMovieClip 49

  &defineMovieClip 50 // total frames: 30

    &frame 0
      &pushsgv 'box'
      &pushs '_visible'
      &pushfalse
      &setMember
      &stop
    &end // of frame 0

    &frame 9
      &pushsgv 'box'
      &pushs '_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnOpening'
      &callmp
    &end // of frame 9

    &frame 19
      &stop
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnOpen'
      &callmp
    &end // of frame 19

    &frame 20
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnClosing'
      &callmp
    &end // of frame 20

    &frame 29
      &pushsgv 'box'
      &pushs '_visible'
      &pushfalse
      &setMember
      &stop
      &pushzero
      &pushsgv '_parent'
      &pushs 'OnClosed'
      &callmp
    &end // of frame 29
  &end // of defineMovieClip 50
&end
