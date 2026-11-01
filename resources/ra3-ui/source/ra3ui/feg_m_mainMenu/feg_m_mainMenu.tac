movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\feg_m_mainMenu\\feg_m_mainMenu.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1280x720 px
  
  &exportAssets
    2 &as 'screenAreaButtonSymbol'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_mainMenu'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 3
  &end // of importAssets

  &placeMovieClip 3 &as 'singlePlayerButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushtrue
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
      &pushssv 'Single Player Maps'
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
  &end // of placeMovieClip 3

  &placeMovieClip 3 &as 'multiPlayerButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushtrue
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
      &pushssv 'Multiplayer'
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
  &end // of placeMovieClip 3

  &placeMovieClip 3 &as 'optionsButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushtrue
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
      &pushssv 'Options'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_global.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 2
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 3

  &placeMovieClip 3 &as 'quitButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushtrue
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
      &pushssv 'Quit'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_global.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 3
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 3

  &defineMovieClip 6 // total frames: 0
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 6
    &constants '_global', 'Cafe2_BaseUIScreen', 'prototype', 'outtroComplete', 'Cafe2_BaseUIScreen::outtroComplete() ', 'm_thisClass', 'm_uiComponentArray', 'length', 'splice', 'm_changeToScreen', 'gSM', 'changeMainScreen', 'introComplete', 'Cafe2_BaseUIScreen::introComplete()', 'restoreVisualState', 'm_introCount', 'onIntrosComplete', 'Cafe2_BaseUIScreen::onIntrosComplete()', 'localEventHandler', 'gEH', 'setLocalEventHandler', 'enableCurrentLocalEventHandler', 'registerIntroOuttroComponents', 'Cafe2_BaseUIScreen::registerIntroOuttroComponents()', 'setIntroCallback', 'executeBackFunction', 'Cafe2_BaseUIScreen::executeBackFunction', 'goBackScreen', 'startIntro', 'Cafe2_BaseUIScreen::startIntro()', 'intro', 'startOuttro', 'Cafe2_BaseUIScreen::startOuttro', 'outtro', 'backButtonClicked', 'Cafe2_BaseUIScreen::backButtonClicked()', 'Cafe2_BaseUIScreen::localEventHandler() keyCode == ', 'gFM', 'handleDefaultInputs', 'clearData', 'm_screen', 'Array', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &not
    &not
    &jnz label20    
    &pushglobalgv
    &pushsdb 1							//'Cafe2_BaseUIScreen'
    &function  (    )    
    &end // of function 

    &setRegister r:1
    &setMember
    &pushregister 1
    &pushsdbgm 2							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 3							//'outtroComplete'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 4							//'Cafe2_BaseUIScreen::outtroComplete() '
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
    &pushregister 2
    &pushsdb 12							//'introComplete'
    &function2  (r:1='theComponent') ()
      &pushsdb 13							//'Cafe2_BaseUIScreen::introComplete()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 14							// restoreVisualState()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdb 15							//'m_introCount'
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 15							//'m_introCount'
      &decrement
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 15							//'m_introCount'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label5      
      &pushzero
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 16							// onIntrosComplete()
     label5:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 16							//'onIntrosComplete'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 17							//'Cafe2_BaseUIScreen::onIntrosComplete()'
      &trace
      &pushregister 1
      &pushsdbgm 18							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 19							//'gEH'
      &dcallmp 20							// setLocalEventHandler()
      &pushzero
      &pushregister 2
      &pushsdbgm 19							//'gEH'
      &dcallmp 21							// enableCurrentLocalEventHandler()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 22							//'registerIntroOuttroComponents'
    &function2  (r:3='components') (r:1='this')
      &pushsdb 23							//'Cafe2_BaseUIScreen::registerIntroOuttroComponents()'
      &trace
      &pushregister 1
      &pushsdb 6							//'m_uiComponentArray'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 15							//'m_introCount'
      &pushregister 1
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushsdbgm 7							//'length'
      &setMember
      &pushzero
      &setRegister r:2
      &pop
     label6:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 15							//'m_introCount'
      &lessThan
      &not
      &jnz label7      
      &pushregister 1
      &pushsdbgm 12							//'introComplete'
      &pushone
      &pushregister 1
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 24							// setIntroCallback()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label6      
     label7:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 25							//'executeBackFunction'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 26							//'Cafe2_BaseUIScreen::executeBackFunction'
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
      &dcallmp 27							// goBackScreen()
     label11:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'startIntro'
    &function2  () (r:1='this')
      &pushsdb 29							//'Cafe2_BaseUIScreen::startIntro()'
      &trace
      &pushregister 1
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
      &pushzero
      &pushregister 1
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 30							// intro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label12      
     label13:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 31							//'startOuttro'
    &function2  (r:4='newScreen') (r:1='this')
      &pushsdb 32							//'Cafe2_BaseUIScreen::startOuttro'
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
      &pushsdbgm 3							//'outtroComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 33							// outtro()
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
      &dcallmp 3							// outtroComplete()
     label17:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'backButtonClicked'
    &function2  () ()
      &pushsdb 35							//'Cafe2_BaseUIScreen::backButtonClicked()'
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
     label18:
      &pushregister 1
      &pushregister 2
      &lessThan
      &not
      &jnz label19      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 25							//'executeBackFunction'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 6							//'m_uiComponentArray'
      &pushregister 1
      &getMember
      &dcallmp 33							// outtro()
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label18      
     label19:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 18							//'localEventHandler'
    &function2  (r:2='keyCode', 'controller') (r:1='_global')
      &pushsdb 36							//'Cafe2_BaseUIScreen::localEventHandler() keyCode == '
      &pushregister 2
      &add
      &trace
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 37							//'gFM'
      &dcallmp 38							// handleDefaultInputs()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 39							//'clearData'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 40							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 9							//'m_changeToScreen'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 6							//'m_uiComponentArray'
      &pushnull
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 9							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 6							//'m_uiComponentArray'
    &pushzero
    &pushsdb 41							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &pushsdbgm 2							//'prototype'
    &pushbyte 3
    &pushsdb 42							//'ASSetPropFlags'
    &callFunction
   label20:
    &pop
  &end // of initMovieClip 6

  &defineMovieClip 7 // total frames: 0
  &end // of defineMovieClip 7
  
  &exportAssets
    7 &as '__Packages.feg_m_mainMenu'
  &end // of exportAssets
  
  &initMovieClip 7
    &constants '_global', 'feg_m_mainMenu', 'feg_m_mainMenu::feg_m_mainMenu() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'initGUI', 'prototype', 'feg_m_mainMenu::initGUI() ', 'gEH', 'singlePlayerButton', '_visible', 'enable', 'singlePlayerGotFocus', 'setOnFocus', 'singlePlayerLostFocus', 'setOnBlur', 'singlePlayerSelected', 'setOnMouseUpFunction', 'multiPlayerButton', 'multiPlayerGotFocus', 'multiPlayerLostFocus', 'multiPlayerSelected', 'optionsButton', 'optionsGotFocus', 'optionsLostFocus', 'optionsSelected', 'quitButton', 'quitGotFocus', 'quitLostFocus', 'quitSelected', 'uiComponentsTestButton', 'testComponentsGotFocus', 'testComponentsLostFocus', 'testComponentsSelected', 'mmPrototype', 'mmPrototypeGotFocus', 'mmPrototypeLostFocus', 'mmPrototypeSelected', 'registerIntroOuttroComponents', 'startIntro', 'onScreenExit', 'gSM', 'setOnExitScreen', 'feg_m_mainMenu::onScreenExit()', 'localMessageHandler', 'gMH', 'removeMessageHandler', 'clearData', 'feg_m_mainMenu::localMessageHandler() messageCode = \'', '\'', 'feg_m_mainMenu::button 1 got focus', 'feg_m_mainMenu::button lost focus', 'feg_m_mainMenu::GO TO SINGLE PLAYER MENU', 'SCREEN', 'FES_SINGLE_PLAYER_MENU', 'startOuttro', 'feg_m_mainMenu::button 2 got focus', 'feg_m_mainMenu::button 2 lost focus', 'feg_m_mainMenu::GO TO MULTI PLAYER MENU', 'FEM_MULTI_PLAYER_MENU', 'feg_m_mainMenu::button 3 got focus', 'feg_m_mainMenu::button 3 lost focus', 'feg_m_mainMenu::GO TO OPTIONS MENU', 'FEG_BB_OPTIONS', 'feg_m_mainMenu::quitGotFocus()', 'feg_m_mainMenu::quitLostFocus()', 'feg_m_mainMenu::quitSelected() ', 'feg_m_mainMenu::testComponentsButtonGotFocus', 'feg_m_mainMenu::testComponentsButtonLostFocus', 'feg_m_mainMenu::testComponentsSelected', 'FEG_CAFE_UI_WIDGET_TEST_SCREEN', 'feg_m_mainMenu::mmPrototypeGotFocus', 'feg_m_mainMenu::mmPrototypeLostFocus', 'feg_m_mainMenu::mmPrototypeSelected', 'FEG_MAINMENU_TEST_SCREEN', 'm_names', 'Array', 'm_values', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu'
    &not
    &not
    &jnz label3    
    &pushglobalgv
    &pushsdb 1							//'feg_m_mainMenu'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'feg_m_mainMenu::feg_m_mainMenu() screen = '
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

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 7							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 6							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 8							//'feg_m_mainMenu::initGUI() '
      &pushregister 2
      &pushsdbgm 9							//'gEH'
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 10							//'singlePlayerButton'
      &pushsdb 11							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 10							//'singlePlayerButton'
      &dcallmp 12							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 13							//'singlePlayerGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 10							//'singlePlayerButton'
      &dcallmp 14							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 15							//'singlePlayerLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 10							//'singlePlayerButton'
      &dcallmp 16							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 17							//'singlePlayerSelected'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 10							//'singlePlayerButton'
      &dcallmp 18							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 19							//'multiPlayerButton'
      &pushsdb 11							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 19							//'multiPlayerButton'
      &dcallmp 12							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 20							//'multiPlayerGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 19							//'multiPlayerButton'
      &dcallmp 14							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 21							//'multiPlayerLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 19							//'multiPlayerButton'
      &dcallmp 16							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 22							//'multiPlayerSelected'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 19							//'multiPlayerButton'
      &dcallmp 18							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 23							//'optionsButton'
      &pushsdb 11							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 23							//'optionsButton'
      &dcallmp 12							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 24							//'optionsGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 23							//'optionsButton'
      &dcallmp 14							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 25							//'optionsLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 23							//'optionsButton'
      &dcallmp 16							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 26							//'optionsSelected'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 23							//'optionsButton'
      &dcallmp 18							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'quitButton'
      &pushsdb 11							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'quitButton'
      &dcallmp 12							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 28							//'quitGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'quitButton'
      &dcallmp 14							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 29							//'quitLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'quitButton'
      &dcallmp 16							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 30							//'quitSelected'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 27							//'quitButton'
      &dcallmp 18							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 31							//'uiComponentsTestButton'
      &pushsdb 11							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 31							//'uiComponentsTestButton'
      &dcallmp 12							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 32							//'testComponentsGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 31							//'uiComponentsTestButton'
      &dcallmp 14							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 33							//'testComponentsLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 31							//'uiComponentsTestButton'
      &dcallmp 16							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 34							//'testComponentsSelected'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 31							//'uiComponentsTestButton'
      &dcallmp 18							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 35							//'mmPrototype'
      &pushsdb 11							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 35							//'mmPrototype'
      &dcallmp 12							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 36							//'mmPrototypeGotFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 35							//'mmPrototype'
      &dcallmp 14							// setOnFocus()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 37							//'mmPrototypeLostFocus'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 35							//'mmPrototype'
      &dcallmp 16							// setOnBlur()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 38							//'mmPrototypeSelected'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 35							//'mmPrototype'
      &dcallmp 18							// setOnMouseUpFunction()
      &pushsdbgv 27							//'quitButton'
      &pushsdbgv 23							//'optionsButton'
      &pushsdbgv 19							//'multiPlayerButton'
      &pushsdbgv 10							//'singlePlayerButton'
      &pushbyte 4
      &initArray
      &pushone
      &pushregister 1
      &dcallmp 39							// registerIntroOuttroComponents()
      &pushzero
      &pushregister 1
      &dcallmp 40							// startIntro()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 41							//'onScreenExit'
      &pushone
      &pushregister 2
      &pushsdbgm 42							//'gSM'
      &dcallmp 43							// setOnExitScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 41							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 44							//'feg_m_mainMenu::onScreenExit()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 45							//'localMessageHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 46							//'gMH'
      &dcallmp 47							// removeMessageHandler()
      &pushzero
      &pushregister 1
      &dcallmp 48							// clearData()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'feg_m_mainMenu'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 45							//'localMessageHandler'
    &function2  (r:2='messageCode') ()
      &pushsdb 49							//'feg_m_mainMenu::localMessageHandler() messageCode = \''
      &pushregister 2
      &add
      &pushsdb 50							//'\''
      &add
      &trace
      &pushfalse
      &setRegister r:1
      &pop
      &jmp label1      
     label1:
      &jmp label2      
     label2:
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 13							//'singlePlayerGotFocus'
    &function  (    )    
      &pushsdb 51							//'feg_m_mainMenu::button 1 got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 15							//'singlePlayerLostFocus'
    &function  (    )    
      &pushsdb 52							//'feg_m_mainMenu::button lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 17							//'singlePlayerSelected'
    &function2  () (r:1='_global')
      &pushsdb 53							//'feg_m_mainMenu::GO TO SINGLE PLAYER MENU'
      &trace
      &pushregister 1
      &pushsdbgm 54							//'SCREEN'
      &pushsdbgm 55							//'FES_SINGLE_PLAYER_MENU'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 56							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 20							//'multiPlayerGotFocus'
    &function  (    )    
      &pushsdb 57							//'feg_m_mainMenu::button 2 got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 21							//'multiPlayerLostFocus'
    &function  (    )    
      &pushsdb 58							//'feg_m_mainMenu::button 2 lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 22							//'multiPlayerSelected'
    &function2  () (r:1='_global')
      &pushsdb 59							//'feg_m_mainMenu::GO TO MULTI PLAYER MENU'
      &trace
      &pushregister 1
      &pushsdbgm 54							//'SCREEN'
      &pushsdbgm 60							//'FEM_MULTI_PLAYER_MENU'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 56							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'optionsGotFocus'
    &function  (    )    
      &pushsdb 61							//'feg_m_mainMenu::button 3 got focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 25							//'optionsLostFocus'
    &function  (    )    
      &pushsdb 62							//'feg_m_mainMenu::button 3 lost focus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'optionsSelected'
    &function2  () (r:1='_global')
      &pushsdb 63							//'feg_m_mainMenu::GO TO OPTIONS MENU'
      &trace
      &pushregister 1
      &pushsdbgm 54							//'SCREEN'
      &pushsdbgm 64							//'FEG_BB_OPTIONS'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 56							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'quitGotFocus'
    &function  (    )    
      &pushsdb 65							//'feg_m_mainMenu::quitGotFocus()'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 29							//'quitLostFocus'
    &function  (    )    
      &pushsdb 66							//'feg_m_mainMenu::quitLostFocus()'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 30							//'quitSelected'
    &function  (    )    
      &pushsdb 67							//'feg_m_mainMenu::quitSelected() '
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%ExitApplication'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 32							//'testComponentsGotFocus'
    &function  (    )    
      &pushsdb 68							//'feg_m_mainMenu::testComponentsButtonGotFocus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 33							//'testComponentsLostFocus'
    &function  (    )    
      &pushsdb 69							//'feg_m_mainMenu::testComponentsButtonLostFocus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'testComponentsSelected'
    &function2  () (r:1='_global')
      &pushsdb 70							//'feg_m_mainMenu::testComponentsSelected'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &add
      &trace
      &pushregister 1
      &pushsdbgm 54							//'SCREEN'
      &pushsdbgm 71							//'FEG_CAFE_UI_WIDGET_TEST_SCREEN'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 56							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 36							//'mmPrototypeGotFocus'
    &function  (    )    
      &pushsdb 72							//'feg_m_mainMenu::mmPrototypeGotFocus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 37							//'mmPrototypeLostFocus'
    &function  (    )    
      &pushsdb 73							//'feg_m_mainMenu::mmPrototypeLostFocus'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 38							//'mmPrototypeSelected'
    &function2  () (r:1='_global')
      &pushsdb 74							//'feg_m_mainMenu::mmPrototypeSelected'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &add
      &trace
      &pushregister 1
      &pushsdbgm 54							//'SCREEN'
      &pushsdbgm 75							//'FEG_MAINMENU_TEST_SCREEN'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 56							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 76							//'m_names'
    &pushzero
    &pushsdb 77							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 78							//'m_values'
    &pushzero
    &pushsdb 77							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 79							//'ASSetPropFlags'
    &callFunction
   label3:
    &pop
  &end // of initMovieClip 7
&end
