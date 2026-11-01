movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEEBCC~1\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_singleplayerMenu'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 1
  &end // of importAssets

  &placeMovieClip 1 &as 'uiScrollingTextBox'
  
    &onClipEvent &construct
      &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_global.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset'  
      &pushsdb 0							//'m_focusDirs'
      &pushsdb 1							//'Up/Down'
      &setVariable
      &pushsdb 2							//'m_numEntries'
      &pushbyte 15
      &setVariable
      &pushsdb 3							//'m_vPadding'
      &pushone
      &setVariable
      &pushsdb 4							//'m_bEnabled'
      &pushtrue
      &setVariable
      &pushsdb 5							//'m_refFM'
      &pushsdb 6							//'_global.gFM'
      &setVariable
      &pushsdb 7							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 8							//'m_tabIndex'
      &pushzerosv
      &pushsdb 9							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 10							//'m_scrollbarOffset'
      &pushzero
      &initArray
      &setVariable
      &pushsdbgv 10							//'m_scrollbarOffset'
      &pushzero
      &pushbyte 10
      &setMember
      &pushsdbgv 10							//'m_scrollbarOffset'
      &pushone
      &pushzero
      &setMember
    &end
  &end // of placeMovieClip 1
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 2
  &end // of importAssets

  &placeMovieClip 2 &as 'uiScrollBar'
  
    &onClipEvent &construct
      &pushs 'm_nLength'
      &pushbyte 100
      &setVariable
      &pushs 'm_nMax'
      &pushbyte 19
      &setVariable
      &pushs 'm_nMin'
      &pushzerosv
      &pushs 'm_nValue'
      &pushzerosv
    &end
  &end // of placeMovieClip 2
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 3
  &end // of importAssets

  &placeMovieClip 3 &as 'backButton'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 5
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$Back'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 3

  &placeMovieClip 3 &as 'playButton'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 4
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$StartGame'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 3
  
  &importAssets &from '../cafe/mouseComponents/std_mouseRadioButton.swf'
    'std_mouseRadioButtonSymbol' &as 4
  &end // of importAssets

  &placeMovieClip 4 &as 'easyRadioButton'
  
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
      &pushssv 'Easy'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'radioButtonContentClip'
      &pushs 'm_controller'
      &pushssv 'difficultyRadioButtonController'
    &end
  &end // of placeMovieClip 4

  &placeMovieClip 4 &as 'mediumRadioButton'
  
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
      &pushssv 'Medium'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'radioButtonContentClip'
      &pushs 'm_controller'
      &pushssv 'difficultyRadioButtonController'
    &end
  &end // of placeMovieClip 4

  &placeMovieClip 4 &as 'hardRadioButton'
  
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
      &pushssv 'Hard'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'radioButtonContentClip'
      &pushs 'm_controller'
      &pushssv 'difficultyRadioButtonController'
    &end
  &end // of placeMovieClip 4
  
  &importAssets &from '../cafe/mouseComponents/std_mouseRadioButton.swf'
    'std_mouseRadioButtonControllerSymbol' &as 5
  &end // of importAssets

  &placeMovieClip 5 &as 'difficultyRadioButtonController'
  
    &onClipEvent &construct
      &pushs 'm_name'
      &pushssv 'RadioButtonController'
    &end
  &end // of placeMovieClip 5

  &defineMovieClip 8 // total frames: 0
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 8
    &constants '_global', 'Cafe2_BaseUIScreen', 'm_screen', 'onEnterFrame', 'frameDelayedInitCallback', 'bind0', 'prototype', 'frameDelayedInit', '\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :', '\nscreens and move the call to initGUI() to frameDelayedInit().\n', 'N.B. You *must* override frameDelayedInit() in your', 'Array', 'outtroComplete', 'Cafe2_BaseUIScreen::outtroComplete() ', 'm_thisClass', 'm_uiComponentArray', 'length', 'splice', 'm_changeToScreen', 'gSM', 'changeMainScreen', 'introComplete', 'Cafe2_BaseUIScreen::introComplete()', 'restoreVisualState', 'm_introCount', 'onIntrosComplete', 'Cafe2_BaseUIScreen::onIntrosComplete()', 'localEventHandler', 'gEH', 'setLocalEventHandler', 'enableCurrentLocalEventHandler', 'registerIntroOuttroComponents', 'Cafe2_BaseUIScreen::registerIntroOuttroComponents()', 'setIntroCallback', 'executeBackFunction', 'Cafe2_BaseUIScreen::executeBackFunction', 'goBackScreen', 'startIntro', 'Cafe2_BaseUIScreen::startIntro()', 'intro', 'startOuttro', 'Cafe2_BaseUIScreen::startOuttro', 'outtro', 'startOuttroAndCallBack', 'm_executeOuttroCallBack', 'outtroAndCallBackComplete', 'backButtonClicked', 'Cafe2_BaseUIScreen::backButtonClicked()', 'Cafe2_BaseUIScreen::localEventHandler() keyCode == ', 'gFM', 'handleDefaultInputs', 'clearData', 'obj', 'func', 'call', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &not
    &not
    &jnz label30    
    &pushglobalgv
    &pushsdb 1							//'Cafe2_BaseUIScreen'
    &function2  (r:3='screen') (r:1='this', r:2='_global')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 2							//'m_screen'
      &pushregister 3
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 3							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 4							//'frameDelayedInitCallback'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'bind0'
      &callMethod
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 6							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 7							//'frameDelayedInit'
    &function2  () ()
      &pushsdb 8							//'\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :'
      &trace
      &pushsdb 9							//'\nscreens and move the call to initGUI() to frameDelayedInit().\n'
      &pushsdb 10							//'N.B. You *must* override frameDelayedInit() in your'
      &pushbyte 2
      &pushsdb 11							//'Array'
      &new
      &setRegister r:1
      &pop
      &pushregister 1
      &pushzero
      &getMember
      &pushregister 1
      &pushone
      &getMember
      &add
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 4							//'frameDelayedInitCallback'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 3							//'onEnterFrame'
      &pushnull
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 7							// frameDelayedInit()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 12							//'outtroComplete'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 13							//'Cafe2_BaseUIScreen::outtroComplete() '
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
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
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &dcallmp 17							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label4      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 18							//'m_changeToScreen'
      &pushnull
      &equals
      &not
      &not
      &jnz label4      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 18							//'m_changeToScreen'
      &pushone
      &pushregister 1
      &pushsdbgm 19							//'gSM'
      &dcallmp 20							// changeMainScreen()
     label4:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 21							//'introComplete'
    &function2  (r:1='theComponent') ()
      &pushsdb 22							//'Cafe2_BaseUIScreen::introComplete()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 23							// restoreVisualState()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdb 24							//'m_introCount'
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 24							//'m_introCount'
      &decrement
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 24							//'m_introCount'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label5      
      &pushzero
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &dcallmp 25							// onIntrosComplete()
     label5:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 25							//'onIntrosComplete'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 26							//'Cafe2_BaseUIScreen::onIntrosComplete()'
      &trace
      &pushregister 1
      &pushsdbgm 27							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 28							//'gEH'
      &dcallmp 29							// setLocalEventHandler()
      &pushzero
      &pushregister 2
      &pushsdbgm 28							//'gEH'
      &dcallmp 30							// enableCurrentLocalEventHandler()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 31							//'registerIntroOuttroComponents'
    &function2  (r:3='components') (r:1='this')
      &pushsdb 32							//'Cafe2_BaseUIScreen::registerIntroOuttroComponents()'
      &trace
      &pushregister 1
      &pushsdb 15							//'m_uiComponentArray'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 24							//'m_introCount'
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setMember
      &pushzero
      &setRegister r:2
      &pop
     label6:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 24							//'m_introCount'
      &lessThan
      &not
      &jnz label7      
      &pushregister 1
      &pushsdbgm 21							//'introComplete'
      &pushone
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 33							// setIntroCallback()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label6      
     label7:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 34							//'executeBackFunction'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 35							//'Cafe2_BaseUIScreen::executeBackFunction'
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label8:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &lessThan
      &not
      &jnz label10      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
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
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &dcallmp 17							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label11      
      &pushzero
      &pushregister 1
      &pushsdbgm 19							//'gSM'
      &dcallmp 36							// goBackScreen()
     label11:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 37							//'startIntro'
    &function2  () (r:1='this')
      &pushsdb 38							//'Cafe2_BaseUIScreen::startIntro()'
      &trace
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
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
      &pushzero
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 39							// intro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label12      
     label13:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 40							//'startOuttro'
    &function2  (r:4='newScreen') (r:1='this')
      &pushsdb 41							//'Cafe2_BaseUIScreen::startOuttro'
      &trace
      &pushregister 1
      &pushsdb 18							//'m_changeToScreen'
      &pushregister 4
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label16      
      &pushzero
      &setRegister r:2
      &pop
     label14:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label15      
      &pushregister 1
      &pushsdbgm 12							//'outtroComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 42							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label14      
     label15:
      &jmp label17      
     label16:
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 12							// outtroComplete()
     label17:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 43							//'startOuttroAndCallBack'
    &function2  (r:4='doAfterOuttroComplete') (r:1='this')
      &pushsdb 41							//'Cafe2_BaseUIScreen::startOuttro'
      &trace
      &pushregister 1
      &pushsdb 44							//'m_executeOuttroCallBack'
      &pushregister 4
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label20      
      &pushzero
      &setRegister r:2
      &pop
     label18:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label19      
      &pushregister 1
      &pushsdbgm 45							//'outtroAndCallBackComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 42							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label18      
     label19:
      &jmp label21      
     label20:
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 45							// outtroAndCallBackComplete()
     label21:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 45							//'outtroAndCallBackComplete'
    &function2  (r:2='theComponent') ()
      &pushsdb 13							//'Cafe2_BaseUIScreen::outtroComplete() '
      &pushregister 2
      &add
      &trace
      &pushzero
      &setRegister r:1
      &pop
     label22:
      &pushregister 1
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &lessThan
      &not
      &jnz label24      
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 1
      &getMember
      &equals
      &not
      &jnz label23      
      &jmp label24      
     label23:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label22      
     label24:
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &dcallmp 17							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label25      
      &pushzero
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &dcallmp 44							// m_executeOuttroCallBack()
     label25:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 46							//'backButtonClicked'
    &function2  () (r:1='_global')
      &pushsdb 47							//'Cafe2_BaseUIScreen::backButtonClicked()'
      &trace
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label28      
      &pushzero
      &setRegister r:2
      &pop
     label26:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label27      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 34							//'executeBackFunction'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 42							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label26      
     label27:
      &jmp label29      
     label28:
      &pushzero
      &pushregister 1
      &pushsdbgm 19							//'gSM'
      &dcallmp 36							// goBackScreen()
     label29:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 27							//'localEventHandler'
    &function2  (r:2='keyCode', 'controller') (r:1='_global')
      &pushsdb 48							//'Cafe2_BaseUIScreen::localEventHandler() keyCode == '
      &pushregister 2
      &add
      &trace
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 49							//'gFM'
      &dcallmp 50							// handleDefaultInputs()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 51							//'clearData'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 2							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 14							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 18							//'m_changeToScreen'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 15							//'m_uiComponentArray'
      &pushnull
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 5							//'bind0'
    &function  ('obj', 'func'    )    
      &function  (      )      
        &pushsdbgv 52							//'obj'
        &pushone
        &pushsdbgv 53							//'func'
        &dcallmp 54							// call()
      &end // of function 

      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 18							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 1
    &pushsdb 44							//'m_executeOuttroCallBack'
    &pushnull
    &setMember
    &pushregister 1
    &pushsdb 15							//'m_uiComponentArray'
    &pushzero
    &pushsdb 11							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &pushsdbgm 6							//'prototype'
    &pushbyte 3
    &pushsdb 55							//'ASSetPropFlags'
    &callFunction
   label30:
    &pop
  &end // of initMovieClip 8

  &defineMovieClip 9 // total frames: 0
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as '__Packages.IListboxEventListener'
  &end // of exportAssets
  
  &initMovieClip 9
    &constants '_global', 'IListboxEventListener'  
    &pushglobalgv
    &pushsdbgm 1							//'IListboxEventListener'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'IListboxEventListener'
    &function  (    )    
    &end // of function 

    &setMember
   label1:
    &pop
  &end // of initMovieClip 9

  &defineMovieClip 10 // total frames: 0
  &end // of defineMovieClip 10
  
  &exportAssets
    10 &as '__Packages.fes_m_singleplayerMenu'
  &end // of exportAssets
  
  &initMovieClip 10
    &constants '_global', 'fes_m_singleplayerMenu', 'fes_m_singleplayerMenu::fes_m_singleplayerMenu() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'initGUI', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_CAMPAIGN', 'prototype', 'IListboxEventListener', 'OnListboxIndexMouseClick', 'OnMapNameClicked', 'OnListboxIndexElementMouseClick', 'fes_m_singleplayerMenu::initGUI()', 'easyRadioButton', 'difficultyRadioButtonController', 'SetSelectedRadioButton', 'difficulty', 'DIFFICULTY_EASY', 'SetUserObject', 'DIFFICULTY_MEDIUM', 'mediumRadioButton', 'DIFFICULTY_HARD', 'hardRadioButton', 'uiScrollBar', '_visible', 'enable', 'Object', 'QueryGameEngine?MAP_LIST?Official=1|User=1|Multiplayer=1|SinglePlayer=1', 'MAP_LIST', '0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15', ',16,17,18,19,20,21,22,23,24,25,26,27,28,29', 'm_values', ',', 'split', 'length', '$MapList_', 'm_names', 'push', 'uiScrollingTextBox', 'setNumEntries', 'setVerticalPadding', 'addListener', 'Array', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'init', 'attachScrollbarController', 'setColumnData', 'moveViewToFirst', 'refreshDisplay', 'refreshScrollbar', 'playButton', 'playButtonGotFocus', 'setOnFocus', 'playButtonLostFocus', 'setOnBlur', 'playExecute', 'setOnMouseUpFunction', 'backButton', 'backButtonGotFocus', 'backButtonLostFocus', 'backButtonClicked', 'onScreenExit', 'gSM', 'setOnExitScreen', 'fes_m_singleplayerMenu::onScreenExit()', 'removeListener', 'shutdown', 'clearData', 'scrollBarGotFocus', 'fes_m_singleplayerMenu::scrollBar got focus', 'scrollBarLostFocus', 'fes_m_singleplayerMenu::scrollBar lost focus', 'fes_m_singleplayerMenu::Back button got focus', 'fes_m_singleplayerMenu::Back button lost focus', 'GetSelectedRadioButton', 'm_difficulty', 'GetUserObject', 'FSCommand:CallGameFunction', '%StartMap?MapId=', 'm_mapIndex', '|Difficulty=', 'fes_m_singleplayerMenu::OnMapNameClicked(), index is ', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_singleplayerMenu'
    &not
    &not
    &jnz label4    
    &pushglobalgv
    &pushsdb 1							//'fes_m_singleplayerMenu'
    &function2  (r:4='screen') (r:1='this', r:2='super', r:3='_global')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'fes_m_singleplayerMenu::fes_m_singleplayerMenu() screen = '
      &pushregister 4
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushregister 4
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 6							// initGUI()
      &pushregister 3
      &pushsdbgm 7							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 3
      &pushsdbgm 9							//'ENUM'
      &pushsdbgm 10							//'GAME_MODE_CAMPAIGN'
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_singleplayerMenu'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 11							//'prototype'
    &setRegister r:1
    &pop
    &pushglobalgv
    &pushsdbgm 12							//'IListboxEventListener'
    &pushone
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_singleplayerMenu'
    &implements
    &pushregister 1
    &pushsdb 13							//'OnListboxIndexMouseClick'
    &function2  (r:2='index', 'previouslySelectedIndex') (r:1='this')
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 14							// OnMapNameClicked()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 15							//'OnListboxIndexElementMouseClick'
    &function  ('index', 'element', 'previouslySelectedIndex'    )    
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 6							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 16							//'fes_m_singleplayerMenu::initGUI()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 17							//'easyRadioButton'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 18							//'difficultyRadioButtonController'
      &dcallmp 19							// SetSelectedRadioButton()
      &pushsdb 20							//'difficulty'
      &pushsdbgv 1							//'fes_m_singleplayerMenu'
      &pushsdbgm 21							//'DIFFICULTY_EASY'
      &pushone
      &initObject
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 17							//'easyRadioButton'
      &dcallmp 22							// SetUserObject()
      &pushsdb 20							//'difficulty'
      &pushsdbgv 1							//'fes_m_singleplayerMenu'
      &pushsdbgm 23							//'DIFFICULTY_MEDIUM'
      &pushone
      &initObject
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 24							//'mediumRadioButton'
      &dcallmp 22							// SetUserObject()
      &pushsdb 20							//'difficulty'
      &pushsdbgv 1							//'fes_m_singleplayerMenu'
      &pushsdbgm 25							//'DIFFICULTY_HARD'
      &pushone
      &initObject
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 26							//'hardRadioButton'
      &dcallmp 22							// SetUserObject()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'uiScrollBar'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'uiScrollBar'
      &dcallmp 29							// enable()
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:4
      &pop
      &pushsdb 31							//'QueryGameEngine?MAP_LIST?Official=1|User=1|Multiplayer=1|SinglePlayer=1'
      &pushregister 4
      &loadVariables
      &pushregister 4
      &pushsdbgm 32							//'MAP_LIST'
      &pushundef
      &equals
      &not
      &jnz label1      
      &pushregister 4
      &pushsdb 32							//'MAP_LIST'
      &pushsdb 33							//'0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15'
      &setMember
      &pushregister 4
      &pushsdb 32							//'MAP_LIST'
      &pushregister 4
      &pushsdbgm 32							//'MAP_LIST'
      &pushsdb 34							//',16,17,18,19,20,21,22,23,24,25,26,27,28,29'
      &add
      &setMember
     label1:
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdb 35							//'m_values'
      &pushsdb 36							//','
      &pushone
      &pushregister 4
      &pushsdbgm 32							//'MAP_LIST'
      &pushsdb 37							//'split'
      &callMethod
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label2:
      &pushregister 3
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 35							//'m_values'
      &pushsdbgm 38							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushsdb 39							//'$MapList_'
      &pushregister 3
      &add
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 40							//'m_names'
      &dcallmp 41							// push()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label2      
     label3:
      &pushbyte 15
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 43							// setNumEntries()
      &pushone
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 44							// setVerticalPadding()
      &pushregister 1
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 45							// addListener()
      &pushzero
      &pushsdb 46							//'Array'
      &new
      &setRegister r:5
      &pop
      &pushsdb 47							//'symbol'
      &pushsdb 48							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 49							//'width'
      &pushshort 300
      &pushsdb 50							//'leftMargin'
      &pushzero
      &pushsdb 51							//'rightMargin'
      &pushzero
      &pushbyte 4
      &initObject
      &pushone
      &pushregister 5
      &dcallmp 41							// push()
      &pushbyte 25
      &pushregister 5
      &pushbyte 2
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 52							// init()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'uiScrollBar'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 53							// attachScrollbarController()
      &pushregister 1
      &pushsdbgm 40							//'m_names'
      &pushzero
      &pushbyte 2
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 54							// setColumnData()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 55							// moveViewToFirst()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 56							// refreshDisplay()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 57							// refreshScrollbar()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 58							//'playButton'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 58							//'playButton'
      &dcallmp 29							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 59							//'playButtonGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 58							//'playButton'
      &dcallmp 60							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 61							//'playButtonLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 58							//'playButton'
      &dcallmp 62							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 63							//'playExecute'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 58							//'playButton'
      &dcallmp 64							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 65							//'backButton'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 65							//'backButton'
      &dcallmp 29							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 66							//'backButtonGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 65							//'backButton'
      &dcallmp 60							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 67							//'backButtonLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 65							//'backButton'
      &dcallmp 62							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 68							//'backButtonClicked'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 65							//'backButton'
      &dcallmp 64							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 69							//'onScreenExit'
      &pushone
      &pushregister 2
      &pushsdbgm 70							//'gSM'
      &dcallmp 71							// setOnExitScreen()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 69							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 72							//'fes_m_singleplayerMenu::onScreenExit()'
      &trace
      &pushregister 1
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 73							// removeListener()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 42							//'uiScrollingTextBox'
      &dcallmp 74							// shutdown()
      &pushzero
      &pushregister 1
      &dcallmp 75							// clearData()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'fes_m_singleplayerMenu'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 76							//'scrollBarGotFocus'
    &function  (    )    
      &pushsdb 77							//'fes_m_singleplayerMenu::scrollBar got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 78							//'scrollBarLostFocus'
    &function  (    )    
      &pushsdb 79							//'fes_m_singleplayerMenu::scrollBar lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 66							//'backButtonGotFocus'
    &function  (    )    
      &pushsdb 80							//'fes_m_singleplayerMenu::Back button got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 67							//'backButtonLostFocus'
    &function  (    )    
      &pushsdb 81							//'fes_m_singleplayerMenu::Back button lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 59							//'playButtonGotFocus'
    &function  (    )    
      &pushsdb 80							//'fes_m_singleplayerMenu::Back button got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 61							//'playButtonLostFocus'
    &function  (    )    
      &pushsdb 81							//'fes_m_singleplayerMenu::Back button lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 63							//'playExecute'
    &function2  () ()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 18							//'difficultyRadioButtonController'
      &pushsdb 82							//'GetSelectedRadioButton'
      &callMethod
      &setRegister r:1
      &pop
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdb 83							//'m_difficulty'
      &pushzero
      &pushregister 1
      &pushsdb 84							//'GetUserObject'
      &callMethod
      &pushsdbgm 20							//'difficulty'
      &setMember
      &pushsdb 85							//'FSCommand:CallGameFunction'
      &pushsdb 86							//'%StartMap?MapId='
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 35							//'m_values'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 87							//'m_mapIndex'
      &getMember
      &add
      &pushsdb 88							//'|Difficulty='
      &add
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 83							//'m_difficulty'
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 14							//'OnMapNameClicked'
    &function2  (r:1='index') ()
      &pushsdb 89							//'fes_m_singleplayerMenu::OnMapNameClicked(), index is '
      &pushregister 1
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdb 87							//'m_mapIndex'
      &pushregister 1
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 58							//'playButton'
      &dcallmp 29							// enable()
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 21							//'DIFFICULTY_EASY'
    &pushzero
    &setMember
    &pushregister 0
    &pushsdb 23							//'DIFFICULTY_MEDIUM'
    &pushone
    &setMember
    &pushregister 0
    &pushsdb 25							//'DIFFICULTY_HARD'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 40							//'m_names'
    &pushzero
    &pushsdb 46							//'Array'
    &new
    &setMember
    &pushregister 1
    &pushsdb 35							//'m_values'
    &pushzero
    &pushsdb 46							//'Array'
    &new
    &setMember
    &pushregister 1
    &pushsdb 83							//'m_difficulty'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 87							//'m_mapIndex'
    &pushbyte -1
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_singleplayerMenu'
    &pushsdbgm 11							//'prototype'
    &pushbyte 3
    &pushsdb 90							//'ASSetPropFlags'
    &callFunction
   label4:
    &pop
  &end // of initMovieClip 10
&end
