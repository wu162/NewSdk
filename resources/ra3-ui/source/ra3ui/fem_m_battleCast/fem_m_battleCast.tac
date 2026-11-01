movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEM_M_~3\\FEM_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1280x720 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fem_m_battleCast'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 1
  &end // of importAssets

  &placeMovieClip 1 &as 'backButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushfalse
      &setVariable
      &pushs 'm_labelPosition'
      &pushssv 'right align'
      &pushs 'm_label_x'
      &pushzerosv
      &pushs 'm_label_y'
      &pushzerosv
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
      &pushs 'm_iconType'
      &pushssv 'default'
      &pushs 'm_extLabel'
      &pushssv ''
      &pushs 'm_nTruncateType'
      &pushzerosv
      &pushs 'm_label'
      &pushssv 'Back'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_global.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushzerosv
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 1

  &placeMovieClip 1 &as 'acceptButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushfalse
      &setVariable
      &pushs 'm_labelPosition'
      &pushssv 'right align'
      &pushs 'm_label_x'
      &pushzerosv
      &pushs 'm_label_y'
      &pushzerosv
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
      &pushs 'm_iconType'
      &pushssv 'default'
      &pushs 'm_extLabel'
      &pushssv ''
      &pushs 'm_nTruncateType'
      &pushzerosv
      &pushs 'm_label'
      &pushssv 'Accept'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_global.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushone
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 1

  &defineMovieClip 4 // total frames: 0
  &end // of defineMovieClip 4
  
  &exportAssets
    4 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 4
    &constants '_global', 'Cafe2_BaseUIScreen', 'prototype', 'outtroComplete', 'Cafe2_BaseUIScreen::OUTTRO COMPLETE ', 'm_thisClass', 'm_uiComponentArray', 'length', 'splice', 'm_changeToScreen', 'gSM', 'changeMainScreen', 'isIntroComplete', 'executeBackFunction', 'Cafe2_BaseUIScreen::executeBackFunction', 'goBackScreen', 'menuButtonClicked', 'Cafe2_BaseUIScreen::menuButtonClicked', 'outtro', 'backButtonClicked', 'Cafe2_BaseUIScreen::Back button selected (clicked)', 'Array', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &not
    &not
    &jnz label16    
    &pushglobalgv
    &pushsdb 1							//'Cafe2_BaseUIScreen'
    &function  (    )    
    &end // of function 

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 2							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 3							//'outtroComplete'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 4							//'Cafe2_BaseUIScreen::OUTTRO COMPLETE '
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &equals
      &not
      &jnz label2      
      &jmp label3      
     label2:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label1      
     label3:
      &pushone
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &dcallmp 8							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label4      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 9							//'m_changeToScreen'
      &pushnull
      &equals
      &not
      &not
      &jnz label4      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 9							//'m_changeToScreen'
      &pushone
      &pushregister 1
      &pushsdbgm 10							//'gSM'
      &dcallmp 11							// changeMainScreen()
     label4:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 12							//'isIntroComplete'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label5:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &lessThan
      &not
      &jnz label7      
      &pushzero
      &pushregister 1
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &pushsdb 12							//'isIntroComplete'
      &callMethod
      &pushfalse
      &equals
      &not
      &jnz label6      
      &pushfalse
      &return
     label6:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label5      
     label7:
      &pushtrue
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 13							//'executeBackFunction'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 14							//'Cafe2_BaseUIScreen::executeBackFunction'
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label8:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &lessThan
      &not
      &jnz label10      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &equals
      &not
      &jnz label9      
      &jmp label10      
     label9:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label8      
     label10:
      &pushone
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &dcallmp 8							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label11      
      &pushzero
      &pushregister 1
      &pushsdbgm 10							//'gSM'
      &dcallmp 15							// goBackScreen()
     label11:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 16							//'menuButtonClicked'
    &function2  (r:4='newScreen') (r:1='this')
      &pushsdb 17							//'Cafe2_BaseUIScreen::menuButtonClicked'
      &trace
      &pushregister 1
      &pushsdb 9							//'m_changeToScreen'
      &pushregister 4
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label12:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label13      
      &pushregister 1
      &pushsdbgm 3							//'outtroComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 18							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label12      
     label13:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 19							//'backButtonClicked'
    &function2  () ()
      &pushsdb 20							//'Cafe2_BaseUIScreen::Back button selected (clicked)'
      &trace
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:1
      &pop
     label14:
      &pushregister 1
      &pushregister 2
      &lessThan
      &not
      &jnz label15      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 13							//'executeBackFunction'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 1
      &getMember
      &dcallmp 18							// outtro()
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label14      
     label15:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 9							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 1
    &pushsdb 6							//'m_uiComponentArray'
    &pushzero
    &pushsdb 21							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &pushsdbgm 2							//'prototype'
    &pushbyte 3
    &pushsdb 22							//'ASSetPropFlags'
    &callFunction
   label16:
    &pop
  &end // of initMovieClip 4

  &defineMovieClip 5 // total frames: 0
  &end // of defineMovieClip 5
  
  &exportAssets
    5 &as '__Packages.fem_m_battleCast'
  &end // of exportAssets
  
  &initMovieClip 5
    &constants '_global', 'fem_m_battleCast', 'fem_m_battleCast::fem_m_battleCast() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'initGUI', 'prototype', 'fem_m_battleCast::initGUI() ', 'gEH', 'localEventHandler', 'acceptButton', '_visible', 'enableGUI', 'acceptButtonGotFocus', 'setOnFocus', 'acceptButtonLostFocus', 'setOnBlur', 'acceptButtonClicked', 'setOnMouseUpFunction', 'introComplete', 'setIntroCallback', 'backButton', 'backButtonGotFocus', 'backButtonLostFocus', 'backButtonClicked', 'm_uiComponentArray', 'onScreenExit', 'gSM', 'setOnExitScreen', 'setLocalEventHandler', 'enableLocalEventHandler', 'unhighlight', 'fem_m_battleCast::onScreenExit()', 'fem_m_battleCast::localEventHandler() keyCode == ', 'debug', 'text', 'kc = ', 'gFM', 'handleDefaultInputs', 'fem_m_battleCast::Back button got focus', 'fem_m_battleCast::Back button lost focus', 'fem_m_battleCast::accept button got focus', 'fem_m_battleCast::accept button lost focus', 'fem_m_battleCast::accept button selected (clicked)', 'SCREEN', 'FEM_MP_LOBBY', 'menuButtonClicked', 'm_names', 'Array', 'm_values', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_battleCast'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'fem_m_battleCast'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'fem_m_battleCast::fem_m_battleCast() screen = '
      &pushregister 3
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushregister 3
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 6							// initGUI()
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_battleCast'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 7							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 6							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 8							//'fem_m_battleCast::initGUI() '
      &pushregister 2
      &pushsdbgm 9							//'gEH'
      &add
      &trace
      &pushsdb 8							//'fem_m_battleCast::initGUI() '
      &pushregister 1
      &pushsdbgm 10							//'localEventHandler'
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 11							//'acceptButton'
      &pushsdb 12							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 11							//'acceptButton'
      &dcallmp 13							// enableGUI()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 14							//'acceptButtonGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 11							//'acceptButton'
      &dcallmp 15							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 16							//'acceptButtonLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 11							//'acceptButton'
      &dcallmp 17							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 18							//'acceptButtonClicked'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 11							//'acceptButton'
      &dcallmp 19							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 20							//'introComplete'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 11							//'acceptButton'
      &dcallmp 21							// setIntroCallback()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 22							//'backButton'
      &pushsdb 12							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 22							//'backButton'
      &dcallmp 13							// enableGUI()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 23							//'backButtonGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 22							//'backButton'
      &dcallmp 15							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 24							//'backButtonLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 22							//'backButton'
      &dcallmp 17							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 25							//'backButtonClicked'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 22							//'backButton'
      &dcallmp 19							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 20							//'introComplete'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 22							//'backButton'
      &dcallmp 21							// setIntroCallback()
      &pushregister 1
      &pushsdb 26							//'m_uiComponentArray'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 22							//'backButton'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 11							//'acceptButton'
      &pushbyte 2
      &initArray
      &setMember
      &pushregister 1
      &pushsdbgm 27							//'onScreenExit'
      &pushone
      &pushregister 2
      &pushsdbgm 28							//'gSM'
      &dcallmp 29							// setOnExitScreen()
      &pushregister 1
      &pushsdbgm 10							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 9							//'gEH'
      &dcallmp 30							// setLocalEventHandler()
      &pushzero
      &pushregister 2
      &pushsdbgm 9							//'gEH'
      &dcallmp 31							// enableLocalEventHandler()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 20							//'introComplete'
    &function2  (r:1='theComponent') ()
      &pushzero
      &pushregister 1
      &dcallmp 32							// unhighlight()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 27							//'onScreenExit'
    &function2  () (r:1='_global')
      &pushsdb 33							//'fem_m_battleCast::onScreenExit()'
      &trace
      &pushregister 1
      &pushsdb 1							//'fem_m_battleCast'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 10							//'localEventHandler'
    &function2  (r:1='keyCode', 'controller') ()
      &pushsdb 34							//'fem_m_battleCast::localEventHandler() keyCode == '
      &pushregister 1
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 35							//'debug'
      &pushsdb 36							//'text'
      &pushsdb 37							//'kc = '
      &pushregister 1
      &toString
      &add
      &setMember
      &pushregister 1
      &pushone
      &pushsdbgv 38							//'gFM'
      &dcallmp 39							// handleDefaultInputs()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 23							//'backButtonGotFocus'
    &function  (    )    
      &pushsdb 40							//'fem_m_battleCast::Back button got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 24							//'backButtonLostFocus'
    &function  (    )    
      &pushsdb 41							//'fem_m_battleCast::Back button lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 14							//'acceptButtonGotFocus'
    &function  (    )    
      &pushsdb 42							//'fem_m_battleCast::accept button got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 16							//'acceptButtonLostFocus'
    &function  (    )    
      &pushsdb 43							//'fem_m_battleCast::accept button lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 18							//'acceptButtonClicked'
    &function2  () (r:1='_global')
      &pushsdb 44							//'fem_m_battleCast::accept button selected (clicked)'
      &trace
      &pushregister 1
      &pushsdbgm 45							//'SCREEN'
      &pushsdbgm 46							//'FEM_MP_LOBBY'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 47							// menuButtonClicked()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 48							//'m_names'
    &pushzero
    &pushsdb 49							//'Array'
    &new
    &setMember
    &pushregister 1
    &pushsdb 50							//'m_values'
    &pushzero
    &pushsdb 49							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_battleCast'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 51							//'ASSetPropFlags'
    &callFunction
   label1:
    &pop
  &end // of initMovieClip 5
&end
