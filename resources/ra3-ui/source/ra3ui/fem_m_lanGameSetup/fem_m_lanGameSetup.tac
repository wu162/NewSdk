movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\fem_m_lanGameSetup\\fem_m_lanGameSetup.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fem_m_lanGameSetup'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 3
  &end // of importAssets
  
  &importAssets &from 'screens/fe_m_assetsGameSetup.swf'
    'startPositionButtonContent' &as 4
  &end // of importAssets
  
  &importAssets &from 'screens/fe_m_assetsGameSetup.swf'
    'gameSettings' &as 5
  &end // of importAssets
  
  &importAssets &from '../mouse/Components/chatComponent.swf'
    'chatComponentSymbol' &as 13
  &end // of importAssets

  &placeMovieClip 13 &as 'chatComponent'
  
    &onClipEvent &construct
      &pushs 'm_numLines'
      &pushzerosv
      &pushs 'm_width'
      &pushshort 399
      &setVariable
      &pushs 'm_chatMode'
      &pushbyte 2
      &setVariable
      &pushs 'm_preservePlacement'
      &pushfalse
      &setVariable
      &pushs 'm_height'
      &pushshort 259
      &setVariable
    &end
  &end // of placeMovieClip 13
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 14
  &end // of importAssets

  &placeMovieClip 14 &as 'backButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushbyte 75
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
      &pushssv '$Back'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_global.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 10
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 14

  &placeMovieClip 14 &as 'playButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushbyte 75
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
      &pushssv '$StartGame'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_global.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 9
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 14
  
  &importAssets &from 'screens/fe_m_assetsGameSetup.swf'
    'playerSlotMC' &as 28
  &end // of importAssets
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 29
  &end // of importAssets

  &placeMovieClip 29 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$LAN_LOBBY_SCREEN_TITLE'
      &pushs 'm_DropShadow'
      &pushtrue
      &setVariable
      &pushs 'm_Outline'
      &pushtrue
      &setVariable
      &pushs 'm_visible'
      &pushtrue
      &setVariable
    &end
  &end // of placeMovieClip 29

  &defineMovieClip 30 // total frames: 0
  &end // of defineMovieClip 30
  
  &exportAssets
    30 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 30
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

    &setRegister r:1
    &setMember
    &pushregister 1
    &pushsdbgm 6							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
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
    &pushregister 2
    &pushsdb 18							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 44							//'m_executeOuttroCallBack'
    &pushnull
    &setMember
    &pushregister 2
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
  &end // of initMovieClip 30

  &defineMovieClip 31 // total frames: 0
  &end // of defineMovieClip 31
  
  &exportAssets
    31 &as '__Packages.IListboxEventListener'
  &end // of exportAssets
  
  &initMovieClip 31
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
  &end // of initMovieClip 31

  &defineMovieClip 32 // total frames: 0
  &end // of defineMovieClip 32
  
  &exportAssets
    32 &as '__Packages.GameSetupBase'
  &end // of exportAssets
  
  &initMovieClip 32
    &constants '_global', 'GameSetupBase', 'GameSetupBase::GameSetupBase()', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_localMessageDispatcher', 'localMessageHandler', 'bind1DynamicParams', 'm_mapIds', 'Array', 'm_screenButtons', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'prototype', 'IListboxEventListener', 'frameDelayedInit', 'Object', 'QueryGameEngine?IsPcGameHost', 'm_isHost', 'IsPcGameHost', '1', 'initGUI', 'gMH', 'addMessageHandler', 'OnListboxIndexMouseClick', 'OnMapListSelectionChanged', 'OnListboxIndexElementMouseClick', 'GameSetupBase::initGUI() ', 'initNavigationButtons', 'initMapPanel', 'initRulesPanel', 'initPlayerSlots', 'GameSetupBase::initNavigationButtons() ', 'm_screen', 'playButton', 'show', 'enable', 'playGame', 'setOnMouseUpFunction', 'push', 'backButton', 'backButtonClicked', 'GameSetupBase::initMapPanel() Map panel movieclip: ', 'gameSettings', 'mapPanel', 'm_playerStartPosMCArray', 'startPosition5', 'startPosition4', 'startPosition3', 'startPosition2', 'startPosition1', 'startPosition0', 'length', 'hide', 'disable', 'onStartPositionClicked', 'bind1', 'campaignCheckbox', 'onMapTypeChanged', 'setOnMouseDownFunction', 'createMapSelectionScrollList', 'rulesPanel', 'GameSetupBase::initRulesPanel() Rules panel movieclip: ', 'extern', 'RA3PublicBeta', '_visible', 'gameSpeedSlider', 'resourcesDropdown', 'cratesCheckbox', 'commentaryCheckbox', 'enableVoipCheckbox', 'setBroadcastCheckboxVisibility', 'onGameSpeedChanged', 'bind2DynamicParams', 'setOnChange', 'onResourcesChanged', 'onCratesChanged', 'onCommentaryChanged', 'onVoipChanged', 'defaultRules', 'onResetRules', 'GameSetupBase::initPlayerSlots() ', 'm_playerSlots', 'slotMC_6', 'slotMC_5', 'slotMC_4', 'slotMC_3', 'slotMC_2', 'slotMC_1', 'slotId', 'playerSlotOptions', 'onPlayerStatusChanged', 'factionOptions', 'onFactionChanged', 'personalityOptions', 'onAIPersonalityChanged', 'teamOptions', 'onTeamChanged', 'colorOptions', 'onColorChanged', 'initReadyControls', 'readyCheckBox', 'onReadyStatusChanged', 'initPlayButton', 'broadcastCheckbox', 'broadcastCheckboxLabel', 'broadcastCheckboxShadowLabel', 'mapListScrollbar', 'mapList', 'addListener', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'init', 'attachScrollbarController', 'moveViewToFirst', 'refreshDisplay', 'refreshScrollbar', 'MSGCODE', 'FE_MP_UPDATE_GAME_SETTINGS', 'refreshComponents', 'GameSetupBase::onScreenExit()', 'removeMessageHandler', 'removeListener', 'shutdown', 'clearData', 'GameSetupBase::playGame()', 'GameSetupBase::onMapTypeChanged() ', 'isChecked', '0', 'FSCommand:CallGameFunction', '%SetMapType?MapType=', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_CAMPAIGN', 'GameSetupBase::onMapTypeChanged()- Game mode set to GAME_MODE_CAMPAIGN.', 'GAME_MODE_SKIRMISH', 'GameSetupBase::onMapTypeChanged()- Game mode set to GAME_MODE_SKIRMISH.', 'GameSetupBase::OnMapListSelectionChanged(), index is ', '%SetMap?Map=', 'onShowMapPanel', 'GameSetupBase::onShowMapPanel()', 'onShowRulesPanel', 'GameSetupBase::onShowRulesPanel()', 'getCurrentIndex', 'getValueAtIndex', '_parent', '%SetPlayerStatus?Slot=', '|Status=', '%SetPlayerTemplate?Slot=', '|Template=', '%SetTeam?Slot=', '|Team=', '%SetColor?Slot=', '|Color=', '%SetAIPersonality?Slot=', '|Personality=', 'GameSetupBase::onReadyStatusChanged()', 'GameSetupBase::onStartPositionClicked() Position: ', '%CycleStartPositionOwner?Position=', 'GameSetupBase::onGameSpeedChanged()', '%SetGameSpeed?GameSpeed=', 'GameSetupBase::onResourcesChanged()', '%SetInitialResources?Resources=', 'GameSetupBase::onVoipChanged()', 'GameSetupBase::onCratesChanged()', 'GameSetupBase::onCommentaryChanged()', 'GameSetupBase::onResetRules()', 'GameSetupBase::refreshComponents()', 'refreshMapList', 'refreshSlotStatusControls', 'refreshCampaignMapControls', 'refreshResourcesControl', 'RANDOM_CRATES', 'refreshRulesCheckbox', 'ENABLE_VOIP', 'ALLOW_COMMENTARY', 'refreshGameSpeedControls', 'refreshTeamControls', 'refreshFactionControls', 'refreshAIPersonalityControls', 'refreshStartPositionControls', 'refreshColorControls', 'GameSetupBase::refreshMapList()', 'QueryGameEngine?MP_MAP_LIST', ',', 'MP_MAP_LIST', 'split', 'MP_MAP_LIST_SELECTED_INDEX', '$MP_MAP_LIST_', '$MP_MAP_SIZE_', '$MP_MAP_LIST_0', '$MP_MAP_SIZE_0', 'setColumnData', 'setSelectedIndex', 'GameSetupBase::refreshSlotStatusControls()', 'QueryGameEngine?PLAYER_STATUS?Slot=', 'PLAYER_STATUS', 'shift', '$PLAYER_STATUS_', '_', 'enableControl', 'setData', 'GameSetupBase::refreshTeamControls()', 'QueryGameEngine?TEAM_OPTIONS?Slot=', 'TEAM_OPTIONS_VALUES', '$TEAM_', 'TEAM_OPTIONS_ENABLE', 'GameSetupBase::refreshFactionControls()', 'QueryGameEngine?PLAYER_TEMPLATE_OPTIONS?Slot=', 'PLAYER_TEMPLATE_OPTIONS_VALUES', 'PLAYER_TEMPLATE_OPTIONS_IMAGES', 'GameSetupBase::refreshAIPersonalityControls() : m_playerSlots.length = ', 'QueryGameEngine?AI_PERSONALITY_OPTIONS?Slot=', 'AI_PERSONALITY_OPTIONS', 'clear', '$APT:AI_PERSONALITY_', 'GameSetupBase::refreshColorControls()', 'QueryGameEngine?PLAYER_COLOR?Slot=', 'PLAYER_COLOR_VALUES', 'PLAYER_COLOR_ENABLE', 'GameSetupBase::refreshStartPositionControls()', 'mapPreview', 'QueryGameEngine?START_POSITION?Position=', 'START_POSITION_VALID', 'Boolean', '$START_POSITION_', 'setText', 'START_POSITION_X', 'START_POSITION_Y', '_x', '_width', '_y', '_height', 'GameSetupBase::refreshCampaignMapControls()', 'QueryGameEngine?PLAY_AS_CAMPAIGN', 'PLAY_AS_CAMPAIGN_VALUE', 'unCheck', 'check', 'PLAY_AS_CAMPAIGN_ENABLE', 'GameSetupBase::refreshResourcesControl()', 'QueryGameEngine?RESOURCES_OPTIONS', 'RESOURCES_OPTIONS_VALUES', 'RESOURCES_OPTIONS_STRINGS', 'GameSetupBase::refreshRulesCheckbox() ', ':', 'QueryGameEngine?', '  ', '_VALUE', '_ENABLE', 'GameSetupBase::refreshGameSpeedControls()', 'QueryGameEngine?GAMESPEED_OPTIONS', 'GAMESPEED_OPTIONS_CURRENT', 'GAMESPEED_OPTIONS_MAX', 'GAMESPEED_OPTIONS_MIN', 'setIndexRange', 'GAMESPEED_OPTIONS_ENABLE', 'refreshReadyControls', 'refreshReadyCheckbox', 'QueryGameEngine?READY_STATUS?Slot=', 'READY_STATUS_ENABLED', 'READY_STATUS_VALUE', 'SetUnCheck', 'SetCheck', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &not
    &not
    &jnz label71    
    &pushglobalgv
    &pushsdb 1							//'GameSetupBase'
    &function2  (r:4='screen') (r:1='this', r:2='super', r:3='_global')
      &pushregister 4
      &pushone
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'GameSetupBase::GameSetupBase()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushsdb 5							//'m_localMessageDispatcher'
      &pushregister 1
      &pushsdbgm 6							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 7							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 8							//'m_mapIds'
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 10							//'m_screenButtons'
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdbgm 11							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 13							//'gSM'
      &dcallmp 14							// setOnExitScreen()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 15							//'prototype'
    &setRegister r:2
    &pop
    &pushglobalgv
    &pushsdbgm 16							//'IListboxEventListener'
    &pushone
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &implements
    &pushregister 2
    &pushsdb 17							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 19							//'QueryGameEngine?IsPcGameHost'
      &pushregister 3
      &loadVariables
      &pushregister 1
      &pushsdb 20							//'m_isHost'
      &pushregister 3
      &pushsdbgm 21							//'IsPcGameHost'
      &pushsdb 22							//'1'
      &equals
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 23							// initGUI()
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageDispatcher'
      &pushone
      &pushregister 2
      &pushsdbgm 24							//'gMH'
      &dcallmp 25							// addMessageHandler()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'OnListboxIndexMouseClick'
    &function2  (r:2='index', 'previouslySelectedIndex') (r:1='this')
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 27							// OnMapListSelectionChanged()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'OnListboxIndexElementMouseClick'
    &function2  (r:2='index', 'element', 'previouslySelectedIndex') (r:1='this')
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 27							// OnMapListSelectionChanged()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 23							//'initGUI'
    &function2  () (r:1='this')
      &pushsdb 29							//'GameSetupBase::initGUI() '
      &trace
      &pushregister 1
      &pushsdbgm 10							//'m_screenButtons'
      &pushone
      &pushregister 1
      &dcallmp 30							// initNavigationButtons()
      &pushzero
      &pushregister 1
      &dcallmp 31							// initMapPanel()
      &pushregister 1
      &pushsdbgm 10							//'m_screenButtons'
      &pushone
      &pushregister 1
      &dcallmp 32							// initRulesPanel()
      &pushzero
      &pushregister 1
      &dcallmp 33							// initPlayerSlots()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 30							//'initNavigationButtons'
    &function2  (r:3='navButtons') (r:1='this', r:2='_global')
      &pushsdb 34							//'GameSetupBase::initNavigationButtons() '
      &trace
      &pushregister 1
      &pushsdbgm 20							//'m_isHost'
      &not
      &jnz label1      
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &dcallmp 37							// show()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &dcallmp 38							// enable()
      &pushregister 1
      &pushsdbgm 39							//'playGame'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &dcallmp 40							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &pushone
      &pushregister 3
      &dcallmp 41							// push()
     label1:
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 42							//'backButton'
      &dcallmp 37							// show()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 42							//'backButton'
      &dcallmp 38							// enable()
      &pushregister 1
      &pushsdbgm 43							//'backButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 42							//'backButton'
      &dcallmp 40							// setOnMouseUpFunction()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 42							//'backButton'
      &pushone
      &pushregister 3
      &dcallmp 41							// push()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 31							//'initMapPanel'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 44							//'GameSetupBase::initMapPanel() Map panel movieclip: '
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdb 47							//'m_playerStartPosMCArray'
      &pushregister 4
      &pushsdbgm 48							//'startPosition5'
      &pushregister 4
      &pushsdbgm 49							//'startPosition4'
      &pushregister 4
      &pushsdbgm 50							//'startPosition3'
      &pushregister 4
      &pushsdbgm 51							//'startPosition2'
      &pushregister 4
      &pushsdbgm 52							//'startPosition1'
      &pushregister 4
      &pushsdbgm 53							//'startPosition0'
      &pushbyte 6
      &initArray
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label2:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushzero
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 3
      &getMember
      &dcallmp 55							// hide()
      &pushzero
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 3
      &getMember
      &dcallmp 56							// disable()
      &pushregister 3
      &pushregister 1
      &pushsdbgm 57							//'onStartPositionClicked'
      &pushregister 1
      &pushbyte 3
      &pushregister 2
      &pushsdb 58							//'bind1'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 3
      &getMember
      &dcallmp 40							// setOnMouseUpFunction()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label2      
     label3:
      &pushzero
      &pushregister 4
      &pushsdbgm 59							//'campaignCheckbox'
      &dcallmp 37							// show()
      &pushzero
      &pushregister 4
      &pushsdbgm 59							//'campaignCheckbox'
      &dcallmp 38							// enable()
      &pushregister 1
      &pushsdbgm 60							//'onMapTypeChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 4
      &pushsdbgm 59							//'campaignCheckbox'
      &dcallmp 61							// setOnMouseDownFunction()
      &pushzero
      &pushregister 1
      &dcallmp 62							// createMapSelectionScrollList()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 32							//'initRulesPanel'
    &function2  (r:5='buttons') (r:1='this', r:2='_global')
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &setRegister r:3
      &pop
      &pushsdb 64							//'GameSetupBase::initRulesPanel() Rules panel movieclip: '
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &add
      &trace
      &pushsdbgv 65							//'extern'
      &pushsdbgm 66							//'RA3PublicBeta'
      &pushsdb 22							//'1'
      &equals
      &not
      &jnz label4      
      &pushregister 3
      &pushsdb 67							//'_visible'
      &pushfalse
      &setMember
      &jmp label6      
     label4:
      &pushzero
      &pushregister 3
      &pushsdbgm 68							//'gameSpeedSlider'
      &dcallmp 37							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 69							//'resourcesDropdown'
      &dcallmp 37							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 70							//'cratesCheckbox'
      &dcallmp 37							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 71							//'commentaryCheckbox'
      &dcallmp 37							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 72							//'enableVoipCheckbox'
      &dcallmp 37							// show()
      &pushfalse
      &pushone
      &pushregister 1
      &dcallmp 73							// setBroadcastCheckboxVisibility()
      &pushregister 1
      &pushsdbgm 20							//'m_isHost'
      &not
      &jnz label5      
      &pushzero
      &pushregister 3
      &pushsdbgm 68							//'gameSpeedSlider'
      &dcallmp 38							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 69							//'resourcesDropdown'
      &dcallmp 38							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 70							//'cratesCheckbox'
      &dcallmp 38							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 71							//'commentaryCheckbox'
      &dcallmp 38							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 72							//'enableVoipCheckbox'
      &dcallmp 38							// enable()
      &pushregister 1
      &pushsdbgm 74							//'onGameSpeedChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 75							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 68							//'gameSpeedSlider'
      &dcallmp 76							// setOnChange()
      &pushregister 1
      &pushsdbgm 77							//'onResourcesChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 7							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 69							//'resourcesDropdown'
      &dcallmp 76							// setOnChange()
      &pushregister 1
      &pushsdbgm 78							//'onCratesChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 70							//'cratesCheckbox'
      &dcallmp 61							// setOnMouseDownFunction()
      &pushregister 1
      &pushsdbgm 79							//'onCommentaryChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 71							//'commentaryCheckbox'
      &dcallmp 61							// setOnMouseDownFunction()
      &pushregister 1
      &pushsdbgm 80							//'onVoipChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 72							//'enableVoipCheckbox'
      &dcallmp 61							// setOnMouseDownFunction()
      &pushregister 3
      &pushsdbgm 81							//'defaultRules'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 37							// show()
      &pushzero
      &pushregister 4
      &dcallmp 38							// enable()
      &pushregister 1
      &pushsdbgm 82							//'onResetRules'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 4
      &dcallmp 40							// setOnMouseUpFunction()
      &pushregister 4
      &pushone
      &pushregister 5
      &dcallmp 41							// push()
      &jmp label6      
     label5:
      &pushzero
      &pushregister 3
      &pushsdbgm 68							//'gameSpeedSlider'
      &dcallmp 56							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 69							//'resourcesDropdown'
      &dcallmp 56							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 70							//'cratesCheckbox'
      &dcallmp 56							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 71							//'commentaryCheckbox'
      &dcallmp 56							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 72							//'enableVoipCheckbox'
      &dcallmp 56							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 81							//'defaultRules'
      &dcallmp 55							// hide()
      &pushzero
      &pushregister 3
      &pushsdbgm 81							//'defaultRules'
      &dcallmp 56							// disable()
     label6:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 33							//'initPlayerSlots'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 83							//'GameSetupBase::initPlayerSlots() '
      &trace
      &pushregister 1
      &pushsdb 84							//'m_playerSlots'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 85							//'slotMC_6'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 86							//'slotMC_5'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 87							//'slotMC_4'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 88							//'slotMC_3'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 89							//'slotMC_2'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 90							//'slotMC_1'
      &pushbyte 6
      &initArray
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label7:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label8      
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdb 91							//'slotId'
      &pushregister 3
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 92							//'playerSlotOptions'
      &dcallmp 37							// show()
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 92							//'playerSlotOptions'
      &dcallmp 56							// disable()
      &pushregister 1
      &pushsdbgm 93							//'onPlayerStatusChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 7							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 92							//'playerSlotOptions'
      &dcallmp 76							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 94							//'factionOptions'
      &dcallmp 37							// show()
      &pushregister 1
      &pushsdbgm 95							//'onFactionChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 7							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 94							//'factionOptions'
      &dcallmp 76							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'personalityOptions'
      &dcallmp 37							// show()
      &pushregister 1
      &pushsdbgm 97							//'onAIPersonalityChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 7							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'personalityOptions'
      &dcallmp 76							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 98							//'teamOptions'
      &dcallmp 37							// show()
      &pushregister 1
      &pushsdbgm 99							//'onTeamChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 7							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 98							//'teamOptions'
      &dcallmp 76							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 100							//'colorOptions'
      &dcallmp 37							// show()
      &pushregister 1
      &pushsdbgm 101							//'onColorChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 7							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 100							//'colorOptions'
      &dcallmp 76							// setOnChange()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label7      
     label8:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 102							//'initReadyControls'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &setRegister r:3
      &pop
     label9:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label10      
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 103							//'readyCheckBox'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 37							// show()
      &pushzero
      &pushregister 4
      &dcallmp 56							// disable()
      &pushregister 1
      &pushsdbgm 104							//'onReadyStatusChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 4
      &dcallmp 61							// setOnMouseDownFunction()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label9      
     label10:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 105							//'initPlayButton'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 20							//'m_isHost'
      &not
      &jnz label11      
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &dcallmp 37							// show()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &dcallmp 38							// enable()
      &jmp label12      
     label11:
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &dcallmp 55							// hide()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 36							//'playButton'
      &dcallmp 56							// disable()
     label12:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 73							//'setBroadcastCheckboxVisibility'
    &function2  (r:2='isVisible') ()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &setRegister r:1
      &pop
      &pushregister 2
      &not
      &jnz label13      
      &pushzero
      &pushregister 1
      &pushsdbgm 106							//'broadcastCheckbox'
      &dcallmp 37							// show()
      &pushregister 1
      &pushsdbgm 107							//'broadcastCheckboxLabel'
      &pushsdb 67							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 108							//'broadcastCheckboxShadowLabel'
      &pushsdb 67							//'_visible'
      &pushtrue
      &setMember
      &jmp label14      
     label13:
      &pushzero
      &pushregister 1
      &pushsdbgm 106							//'broadcastCheckbox'
      &dcallmp 55							// hide()
      &pushregister 1
      &pushsdbgm 107							//'broadcastCheckboxLabel'
      &pushsdb 67							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 108							//'broadcastCheckboxShadowLabel'
      &pushsdb 67							//'_visible'
      &pushfalse
      &setMember
     label14:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 62							//'createMapSelectionScrollList'
    &function2  () (r:1='this')
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 109							//'mapListScrollbar'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 37							// show()
      &pushzero
      &pushregister 4
      &dcallmp 38							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 110							//'mapList'
      &setRegister r:2
      &pop
      &pushregister 1
      &pushone
      &pushregister 2
      &dcallmp 111							// addListener()
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 112							//'symbol'
      &pushsdb 113							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 114							//'width'
      &pushshort 270
      &pushsdb 115							//'leftMargin'
      &pushzero
      &pushsdb 116							//'rightMargin'
      &pushzero
      &pushbyte 4
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 41							// push()
      &pushsdb 112							//'symbol'
      &pushsdb 113							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 114							//'width'
      &pushbyte 75
      &pushsdb 115							//'leftMargin'
      &pushbyte 26
      &pushsdb 116							//'rightMargin'
      &pushzero
      &pushbyte 4
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 41							// push()
      &pushbyte 26
      &pushregister 3
      &pushbyte 2
      &pushregister 2
      &dcallmp 117							// init()
      &pushregister 4
      &pushone
      &pushregister 2
      &dcallmp 118							// attachScrollbarController()
      &pushzero
      &pushregister 2
      &dcallmp 37							// show()
      &pushzero
      &pushregister 2
      &dcallmp 38							// enable()
      &pushzero
      &pushregister 2
      &dcallmp 119							// moveViewToFirst()
      &pushzero
      &pushregister 2
      &dcallmp 120							// refreshDisplay()
      &pushzero
      &pushregister 2
      &dcallmp 121							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 6							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushfalse
      &setRegister r:3
      &pop
      &pushregister 4
      &pushregister 2
      &pushsdbgm 122							//'MSGCODE'
      &pushsdbgm 123							//'FE_MP_UPDATE_GAME_SETTINGS'
      &strictEquals
      &jnz label15      
      &jmp label16      
     label15:
      &pushzero
      &pushregister 1
      &dcallmp 124							// refreshComponents()
      &pushtrue
      &setRegister r:3
      &pop
      &jmp label17      
     label16:
      &jmp label17      
     label17:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 11							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 125							//'GameSetupBase::onScreenExit()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%SetGameSetupMode?Mode=None'      
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageDispatcher'
      &pushone
      &pushregister 2
      &pushsdbgm 24							//'gMH'
      &dcallmp 126							// removeMessageHandler()
      &pushregister 1
      &pushsdb 5							//'m_localMessageDispatcher'
      &delete
      &pop
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 110							//'mapList'
      &setRegister r:3
      &pop
      &pushregister 1
      &pushone
      &pushregister 3
      &dcallmp 127							// removeListener()
      &pushzero
      &pushregister 3
      &dcallmp 128							// shutdown()
      &pushzero
      &pushregister 1
      &dcallmp 129							// clearData()
      &pushregister 2
      &pushsdb 1							//'GameSetupBase'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 39							//'playGame'
    &function  (    )    
      &pushsdb 130							//'GameSetupBase::playGame()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%StartGame'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 60							//'onMapTypeChanged'
    &function2  () (r:1='_global')
      &pushsdb 131							//'GameSetupBase::onMapTypeChanged() '
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &add
      &trace
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 59							//'campaignCheckbox'
      &pushsdb 132							//'isChecked'
      &callMethod
      &jnz label18      
      &pushsdb 133							//'0'
      &jmp label19      
     label18:
      &pushsdb 22							//'1'
     label19:
      &setRegister r:2
      &pop
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 135							//'%SetMapType?MapType='
      &pushregister 2
      &add
      &getURL2
      &pushregister 2
      &not
      &jnz label20      
      &pushregister 1
      &pushsdbgm 136							//'GAME'
      &pushsdb 137							//'MODE'
      &pushregister 1
      &pushsdbgm 138							//'ENUM'
      &pushsdbgm 139							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushsdb 140							//'GameSetupBase::onMapTypeChanged()- Game mode set to GAME_MODE_CAMPAIGN.'
      &trace
      &jmp label21      
     label20:
      &pushregister 1
      &pushsdbgm 136							//'GAME'
      &pushsdb 137							//'MODE'
      &pushregister 1
      &pushsdbgm 138							//'ENUM'
      &pushsdbgm 141							//'GAME_MODE_SKIRMISH'
      &setMember
      &pushsdb 142							//'GameSetupBase::onMapTypeChanged()- Game mode set to GAME_MODE_SKIRMISH.'
      &trace
     label21:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 27							//'OnMapListSelectionChanged'
    &function2  (r:2='index') (r:1='this')
      &pushsdb 143							//'GameSetupBase::OnMapListSelectionChanged(), index is '
      &pushregister 2
      &add
      &trace
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 144							//'%SetMap?Map='
      &pushregister 1
      &pushsdbgm 8							//'m_mapIds'
      &pushregister 2
      &getMember
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 145							//'onShowMapPanel'
    &function  (    )    
      &pushsdb 146							//'GameSetupBase::onShowMapPanel()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdb 67							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &pushsdb 67							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 147							//'onShowRulesPanel'
    &function  (    )    
      &pushsdb 148							//'GameSetupBase::onShowRulesPanel()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdb 67							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &pushsdb 67							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 93							//'onPlayerStatusChanged'
    &function2  (r:3='playerStatusMC') ()
      &pushzero
      &pushregister 3
      &pushsdb 149							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdb 150							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdbgm 151							//'_parent'
      &pushsdbgm 91							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 152							//'%SetPlayerStatus?Slot='
      &pushregister 1
      &add
      &pushsdb 153							//'|Status='
      &add
      &pushregister 2
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 95							//'onFactionChanged'
    &function2  (r:1='factionsMC') ()
      &pushzero
      &pushregister 1
      &pushsdb 149							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdb 150							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdbgm 151							//'_parent'
      &pushsdbgm 91							//'slotId'
      &toString
      &setRegister r:2
      &pop
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 154							//'%SetPlayerTemplate?Slot='
      &pushregister 2
      &add
      &pushsdb 155							//'|Template='
      &add
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 99							//'onTeamChanged'
    &function2  (r:2='teamsMC') ()
      &pushzero
      &pushregister 2
      &pushsdb 149							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdb 150							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdbgm 151							//'_parent'
      &pushsdbgm 91							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 156							//'%SetTeam?Slot='
      &pushregister 1
      &add
      &pushsdb 157							//'|Team='
      &add
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 101							//'onColorChanged'
    &function2  (r:3='colorsMC') ()
      &pushzero
      &pushregister 3
      &pushsdb 149							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdb 150							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdbgm 151							//'_parent'
      &pushsdbgm 91							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 158							//'%SetColor?Slot='
      &pushregister 1
      &add
      &pushsdb 159							//'|Color='
      &add
      &pushregister 2
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 97							//'onAIPersonalityChanged'
    &function2  (r:2='personalitiesMC') ()
      &pushzero
      &pushregister 2
      &pushsdb 149							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdb 150							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdbgm 151							//'_parent'
      &pushsdbgm 91							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 160							//'%SetAIPersonality?Slot='
      &pushregister 1
      &add
      &pushsdb 161							//'|Personality='
      &add
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 104							//'onReadyStatusChanged'
    &function  (    )    
      &pushsdb 162							//'GameSetupBase::onReadyStatusChanged()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%ToggleReadyStatus'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 57							//'onStartPositionClicked'
    &function2  (r:1='startPosition') ()
      &pushsdb 163							//'GameSetupBase::onStartPositionClicked() Position: '
      &pushregister 1
      &add
      &trace
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 164							//'%CycleStartPositionOwner?Position='
      &pushregister 1
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 74							//'onGameSpeedChanged'
    &function2  ('component', r:1='newSpeed') ()
      &pushsdb 165							//'GameSetupBase::onGameSpeedChanged()'
      &trace
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 166							//'%SetGameSpeed?GameSpeed='
      &pushregister 1
      &toString
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 77							//'onResourcesChanged'
    &function2  (r:2='resourcesMC') ()
      &pushsdb 167							//'GameSetupBase::onResourcesChanged()'
      &trace
      &pushzero
      &pushregister 2
      &pushsdb 149							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdb 150							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:1
      &pop
      &pushsdb 134							//'FSCommand:CallGameFunction'
      &pushsdb 168							//'%SetInitialResources?Resources='
      &pushregister 1
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 80							//'onVoipChanged'
    &function  (    )    
      &pushsdb 169							//'GameSetupBase::onVoipChanged()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%ToggleVoipRule'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 78							//'onCratesChanged'
    &function  (    )    
      &pushsdb 170							//'GameSetupBase::onCratesChanged()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%ToggleRandomCrates'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 79							//'onCommentaryChanged'
    &function  (    )    
      &pushsdb 171							//'GameSetupBase::onCommentaryChanged()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%ToggleCommentary'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 82							//'onResetRules'
    &function  (    )    
      &pushsdb 172							//'GameSetupBase::onResetRules()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%ResetRules'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 124							//'refreshComponents'
    &function2  () (r:1='this')
      &pushsdb 173							//'GameSetupBase::refreshComponents()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 174							// refreshMapList()
      &pushzero
      &pushregister 1
      &dcallmp 175							// refreshSlotStatusControls()
      &pushzero
      &pushregister 1
      &dcallmp 176							// refreshCampaignMapControls()
      &pushzero
      &pushregister 1
      &dcallmp 177							// refreshResourcesControl()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &pushsdbgm 70							//'cratesCheckbox'
      &pushsdb 178							//'RANDOM_CRATES'
      &pushbyte 2
      &pushregister 1
      &dcallmp 179							// refreshRulesCheckbox()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &pushsdbgm 72							//'enableVoipCheckbox'
      &pushsdb 180							//'ENABLE_VOIP'
      &pushbyte 2
      &pushregister 1
      &dcallmp 179							// refreshRulesCheckbox()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &pushsdbgm 71							//'commentaryCheckbox'
      &pushsdb 181							//'ALLOW_COMMENTARY'
      &pushbyte 2
      &pushregister 1
      &dcallmp 179							// refreshRulesCheckbox()
      &pushzero
      &pushregister 1
      &dcallmp 182							// refreshGameSpeedControls()
      &pushzero
      &pushregister 1
      &dcallmp 183							// refreshTeamControls()
      &pushzero
      &pushregister 1
      &dcallmp 184							// refreshFactionControls()
      &pushzero
      &pushregister 1
      &dcallmp 185							// refreshAIPersonalityControls()
      &pushzero
      &pushregister 1
      &dcallmp 186							// refreshStartPositionControls()
      &pushzero
      &pushregister 1
      &dcallmp 187							// refreshColorControls()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 174							//'refreshMapList'
    &function2  () (r:1='this')
      &pushsdb 188							//'GameSetupBase::refreshMapList()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 110							//'mapList'
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 189							//'QueryGameEngine?MP_MAP_LIST'
      &pushregister 6
      &loadVariables
      &pushregister 1
      &pushsdb 8							//'m_mapIds'
      &pushsdb 190							//','
      &pushone
      &pushregister 6
      &pushsdbgm 191							//'MP_MAP_LIST'
      &pushsdb 192							//'split'
      &callMethod
      &setMember
      &pushregister 6
      &pushsdbgm 193							//'MP_MAP_LIST_SELECTED_INDEX'
      &toNumber
      &setRegister r:7
      &pop
      &pushregister 1
      &pushsdbgm 20							//'m_isHost'
      &not
      &jnz label24      
      &pushzero
      &setRegister r:2
      &pop
     label22:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 8							//'m_mapIds'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label23      
      &pushregister 4
      &pushregister 2
      &pushsdb 194							//'$MP_MAP_LIST_'
      &pushregister 2
      &toString
      &add
      &setMember
      &pushregister 3
      &pushregister 2
      &pushsdb 195							//'$MP_MAP_SIZE_'
      &pushregister 2
      &toString
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label22      
     label23:
      &jmp label25      
     label24:
      &pushregister 4
      &pushzero
      &pushsdb 196							//'$MP_MAP_LIST_0'
      &setMember
      &pushregister 3
      &pushzero
      &pushsdb 197							//'$MP_MAP_SIZE_0'
      &setMember
     label25:
      &pushregister 4
      &pushzero
      &pushbyte 2
      &pushregister 5
      &dcallmp 198							// setColumnData()
      &pushregister 3
      &pushone
      &pushbyte 2
      &pushregister 5
      &dcallmp 198							// setColumnData()
      &pushregister 7
      &pushone
      &pushregister 5
      &dcallmp 199							// setSelectedIndex()
      &pushzero
      &pushregister 5
      &dcallmp 120							// refreshDisplay()
      &pushzero
      &pushregister 5
      &dcallmp 121							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 175							//'refreshSlotStatusControls'
    &function2  () (r:1='this')
      &pushsdb 200							//'GameSetupBase::refreshSlotStatusControls()'
      &trace
      &pushzero
      &setRegister r:3
      &pop
     label26:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label29      
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:7
      &pop
      &pushsdb 201							//'QueryGameEngine?PLAYER_STATUS?Slot='
      &pushregister 3
      &toString
      &add
      &pushregister 5
      &loadVariables
      &pushsdb 190							//','
      &pushone
      &pushregister 5
      &pushsdbgm 202							//'PLAYER_STATUS'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &pushsdb 203							//'shift'
      &callMethod
      &toNumber
      &setRegister r:7
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label27:
      &pushregister 2
      &pushregister 4
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label28      
      &pushregister 6
      &pushregister 2
      &pushsdb 204							//'$PLAYER_STATUS_'
      &pushregister 3
      &toString
      &add
      &pushsdb 205							//'_'
      &add
      &pushregister 2
      &toString
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label27      
     label28:
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 92							//'playerSlotOptions'
      &pushregister 4
      &pushsdbgm 54							//'length'
      &pushone
      &greaterThan
      &pushbyte 2
      &pushregister 1
      &dcallmp 206							// enableControl()
      &pushregister 4
      &pushregister 6
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 92							//'playerSlotOptions'
      &dcallmp 207							// setData()
      &pushregister 7
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 92							//'playerSlotOptions'
      &dcallmp 199							// setSelectedIndex()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label26      
     label29:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 183							//'refreshTeamControls'
    &function2  () (r:1='this')
      &pushsdb 208							//'GameSetupBase::refreshTeamControls()'
      &trace
      &pushzero
      &setRegister r:4
      &pop
     label30:
      &pushregister 4
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label34      
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:7
      &pop
      &pushsdb 209							//'QueryGameEngine?TEAM_OPTIONS?Slot='
      &pushregister 4
      &toString
      &add
      &pushregister 5
      &loadVariables
      &pushsdb 190							//','
      &pushone
      &pushregister 5
      &pushsdbgm 210							//'TEAM_OPTIONS_VALUES'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &pushsdb 203							//'shift'
      &callMethod
      &toNumber
      &setRegister r:7
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label31:
      &pushregister 2
      &pushregister 3
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label32      
      &pushregister 6
      &pushregister 2
      &pushsdb 211							//'$TEAM_'
      &pushregister 3
      &pushregister 2
      &getMember
      &toString
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label31      
     label32:
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 4
      &getMember
      &pushsdbgm 98							//'teamOptions'
      &pushregister 5
      &pushsdbgm 212							//'TEAM_OPTIONS_ENABLE'
      &pushsdb 22							//'1'
      &equals
      &dup
      &not
      &jnz label33      
      &pop
      &pushregister 3
      &pushsdbgm 54							//'length'
      &pushone
      &greaterThan
     label33:
      &pushbyte 2
      &pushregister 1
      &dcallmp 206							// enableControl()
      &pushregister 3
      &pushregister 6
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 4
      &getMember
      &pushsdbgm 98							//'teamOptions'
      &dcallmp 207							// setData()
      &pushregister 7
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 4
      &getMember
      &pushsdbgm 98							//'teamOptions'
      &dcallmp 199							// setSelectedIndex()
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &jmp label30      
     label34:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 184							//'refreshFactionControls'
    &function2  () (r:1='this')
      &pushsdb 213							//'GameSetupBase::refreshFactionControls()'
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label35:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label36      
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:4
      &pop
      &pushsdb 214							//'QueryGameEngine?PLAYER_TEMPLATE_OPTIONS?Slot='
      &pushregister 2
      &toString
      &add
      &pushregister 4
      &loadVariables
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 190							//','
      &pushone
      &pushregister 4
      &pushsdbgm 215							//'PLAYER_TEMPLATE_OPTIONS_VALUES'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &pushsdb 203							//'shift'
      &callMethod
      &toNumber
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:5
      &pop
      &pushsdb 190							//','
      &pushone
      &pushregister 4
      &pushsdbgm 216							//'PLAYER_TEMPLATE_OPTIONS_IMAGES'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:5
      &pop
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 94							//'factionOptions'
      &pushregister 3
      &pushsdbgm 54							//'length'
      &pushone
      &greaterThan
      &pushbyte 2
      &pushregister 1
      &dcallmp 206							// enableControl()
      &pushregister 3
      &pushregister 5
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 94							//'factionOptions'
      &dcallmp 207							// setData()
      &pushregister 6
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 94							//'factionOptions'
      &dcallmp 199							// setSelectedIndex()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label35      
     label36:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 185							//'refreshAIPersonalityControls'
    &function2  () (r:1='this')
      &pushsdb 217							//'GameSetupBase::refreshAIPersonalityControls() : m_playerSlots.length = '
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label37:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label42      
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 218							//'QueryGameEngine?AI_PERSONALITY_OPTIONS?Slot='
      &pushregister 2
      &toString
      &add
      &pushregister 5
      &loadVariables
      &pushregister 5
      &pushsdbgm 219							//'AI_PERSONALITY_OPTIONS'
      &pushundef
      &equals
      &not
      &jnz label38      
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 96							//'personalityOptions'
      &dcallmp 220							// clear()
      &pushzero
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 96							//'personalityOptions'
      &dcallmp 56							// disable()
      &jmp label41      
     label38:
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 190							//','
      &pushone
      &pushregister 5
      &pushsdbgm 219							//'AI_PERSONALITY_OPTIONS'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &pushsdb 203							//'shift'
      &callMethod
      &toNumber
      &setRegister r:7
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &setRegister r:3
      &pop
     label39:
      &pushregister 3
      &pushregister 4
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label40      
      &pushregister 6
      &pushregister 3
      &pushsdb 221							//'$APT:AI_PERSONALITY_'
      &pushregister 2
      &toString
      &add
      &pushsdb 205							//'_'
      &add
      &pushregister 4
      &pushregister 3
      &getMember
      &toString
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label39      
     label40:
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 96							//'personalityOptions'
      &pushregister 4
      &pushsdbgm 54							//'length'
      &pushone
      &greaterThan
      &pushbyte 2
      &pushregister 1
      &dcallmp 206							// enableControl()
      &pushregister 4
      &pushregister 6
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 96							//'personalityOptions'
      &dcallmp 207							// setData()
      &pushregister 7
      &pushone
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 96							//'personalityOptions'
      &dcallmp 199							// setSelectedIndex()
     label41:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label37      
     label42:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 187							//'refreshColorControls'
    &function2  () (r:1='this')
      &pushsdb 222							//'GameSetupBase::refreshColorControls()'
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label43:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label46      
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 223							//'QueryGameEngine?PLAYER_COLOR?Slot='
      &pushregister 2
      &add
      &pushregister 3
      &loadVariables
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:5
      &pop
      &pushsdb 190							//','
      &pushone
      &pushregister 3
      &pushsdbgm 224							//'PLAYER_COLOR_VALUES'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:5
      &pop
      &pushzero
      &pushregister 5
      &pushsdb 203							//'shift'
      &callMethod
      &toNumber
      &setRegister r:6
      &pop
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 100							//'colorOptions'
      &setRegister r:4
      &pop
      &pushregister 5
      &pushregister 5
      &pushbyte 2
      &pushregister 4
      &dcallmp 207							// setData()
      &pushregister 6
      &pushone
      &pushregister 4
      &dcallmp 199							// setSelectedIndex()
      &pushregister 3
      &pushsdbgm 225							//'PLAYER_COLOR_ENABLE'
      &pushsdb 22							//'1'
      &equals
      &not
      &jnz label44      
      &pushzero
      &pushregister 4
      &dcallmp 38							// enable()
      &jmp label45      
     label44:
      &pushzero
      &pushregister 4
      &dcallmp 56							// disable()
     label45:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label43      
     label46:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 186							//'refreshStartPositionControls'
    &function2  () (r:1='this')
      &pushsdb 226							//'GameSetupBase::refreshStartPositionControls()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 227							//'mapPreview'
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label47:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label50      
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 228							//'QueryGameEngine?START_POSITION?Position='
      &pushregister 2
      &toString
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 229							//'START_POSITION_VALID'
      &pushone
      &pushsdb 230							//'Boolean'
      &callFunction
      &setRegister r:7
      &pop
      &pushsdb 231							//'$START_POSITION_'
      &pushregister 2
      &toString
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 232							// setText()
      &pushregister 7
      &not
      &jnz label48      
      &pushregister 3
      &pushsdbgm 233							//'START_POSITION_X'
      &toNumber
      &setRegister r:6
      &pop
      &pushregister 3
      &pushsdbgm 234							//'START_POSITION_Y'
      &toNumber
      &setRegister r:5
      &pop
      &pushzero
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 37							// show()
      &pushzero
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 38							// enable()
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &pushsdb 235							//'_x'
      &pushregister 6
      &pushregister 4
      &pushsdbgm 236							//'_width'
      &multiply
      &pushregister 4
      &pushsdbgm 235							//'_x'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &pushsdb 237							//'_y'
      &pushregister 5
      &pushregister 4
      &pushsdbgm 238							//'_height'
      &multiply
      &pushregister 4
      &pushsdbgm 237							//'_y'
      &add
      &setMember
      &jmp label49      
     label48:
      &pushzero
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 56							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 47							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 55							// hide()
     label49:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label47      
     label50:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 176							//'refreshCampaignMapControls'
    &function2  () (r:1='this')
      &pushsdb 239							//'GameSetupBase::refreshCampaignMapControls()'
      &trace
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 240							//'QueryGameEngine?PLAY_AS_CAMPAIGN'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 241							//'PLAY_AS_CAMPAIGN_VALUE'
      &pushsdb 133							//'0'
      &equals
      &not
      &jnz label51      
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 59							//'campaignCheckbox'
      &dcallmp 242							// unCheck()
      &jmp label52      
     label51:
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 59							//'campaignCheckbox'
      &dcallmp 243							// check()
     label52:
      &pushregister 2
      &pushsdbgm 244							//'PLAY_AS_CAMPAIGN_ENABLE'
      &pushsdb 133							//'0'
      &equals
      &dup
      &jnz label53      
      &pop
      &pushregister 1
      &pushsdbgm 20							//'m_isHost'
      &not
     label53:
      &not
      &jnz label54      
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 59							//'campaignCheckbox'
      &dcallmp 56							// disable()
      &jmp label55      
     label54:
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 46							//'mapPanel'
      &pushsdbgm 59							//'campaignCheckbox'
      &dcallmp 38							// enable()
     label55:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 177							//'refreshResourcesControl'
    &function2  () ()
      &pushsdb 245							//'GameSetupBase::refreshResourcesControl()'
      &trace
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 246							//'QueryGameEngine?RESOURCES_OPTIONS'
      &pushregister 1
      &loadVariables
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:2
      &pop
      &pushsdb 190							//','
      &pushone
      &pushregister 1
      &pushsdbgm 247							//'RESOURCES_OPTIONS_VALUES'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:2
      &pop
      &pushzero
      &pushregister 2
      &pushsdb 203							//'shift'
      &callMethod
      &toNumber
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 190							//','
      &pushone
      &pushregister 1
      &pushsdbgm 248							//'RESOURCES_OPTIONS_STRINGS'
      &pushsdb 192							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &pushsdbgm 69							//'resourcesDropdown'
      &setRegister r:4
      &pop
      &pushregister 2
      &pushregister 3
      &pushbyte 2
      &pushregister 4
      &dcallmp 207							// setData()
      &pushregister 5
      &pushone
      &pushregister 4
      &dcallmp 199							// setSelectedIndex()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 179							//'refreshRulesCheckbox'
    &function2  (r:2='ruleStr', r:3='checkbox') ()
      &pushsdb 249							//'GameSetupBase::refreshRulesCheckbox() '
      &pushregister 2
      &add
      &pushsdb 250							//':'
      &add
      &pushregister 3
      &add
      &trace
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 251							//'QueryGameEngine?'
      &pushregister 2
      &add
      &pushregister 1
      &loadVariables
      &pushsdb 252							//'  '
      &pushregister 1
      &pushregister 2
      &pushsdb 253							//'_VALUE'
      &add
      &getMember
      &add
      &trace
      &pushregister 1
      &pushregister 2
      &pushsdb 253							//'_VALUE'
      &add
      &getMember
      &pushsdb 133							//'0'
      &equals
      &not
      &jnz label56      
      &pushzero
      &pushregister 3
      &dcallmp 242							// unCheck()
      &jmp label57      
     label56:
      &pushzero
      &pushregister 3
      &dcallmp 243							// check()
     label57:
      &pushsdb 252							//'  '
      &pushregister 1
      &pushregister 2
      &pushsdb 254							//'_ENABLE'
      &add
      &getMember
      &add
      &trace
      &pushregister 1
      &pushregister 2
      &pushsdb 254							//'_ENABLE'
      &add
      &getMember
      &pushsdb 133							//'0'
      &equals
      &not
      &jnz label58      
      &pushzero
      &pushregister 3
      &dcallmp 56							// disable()
      &jmp label59      
     label58:
      &pushzero
      &pushregister 3
      &dcallmp 38							// enable()
     label59:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 182							//'refreshGameSpeedControls'
    &function2  () (r:1='this')
      &pushsdb 255							//'GameSetupBase::refreshGameSpeedControls()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 45							//'gameSettings'
      &pushsdbgm 63							//'rulesPanel'
      &pushsdbgm 68							//'gameSpeedSlider'
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdw 256							//'QueryGameEngine?GAMESPEED_OPTIONS'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdw 257							//'GAMESPEED_OPTIONS_CURRENT'
      &getMember
      &toNumber
      &pushregister 2
      &pushsdw 258							//'GAMESPEED_OPTIONS_MAX'
      &getMember
      &toNumber
      &pushregister 2
      &pushsdw 259							//'GAMESPEED_OPTIONS_MIN'
      &getMember
      &toNumber
      &pushbyte 3
      &pushregister 3
      &pushsdw 260							//'setIndexRange'
      &callmp
      &pushregister 2
      &pushsdw 261							//'GAMESPEED_OPTIONS_ENABLE'
      &getMember
      &pushsdb 133							//'0'
      &equals
      &dup
      &jnz label60      
      &pop
      &pushregister 1
      &pushsdbgm 20							//'m_isHost'
      &not
     label60:
      &not
      &jnz label61      
      &pushzero
      &pushregister 3
      &dcallmp 56							// disable()
      &jmp label62      
     label61:
      &pushzero
      &pushregister 3
      &dcallmp 38							// enable()
     label62:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 262							//'refreshReadyControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label63:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushsdbgm 54							//'length'
      &lessThan
      &not
      &jnz label64      
      &pushregister 1
      &pushsdbgm 84							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 103							//'readyCheckBox'
      &setRegister r:3
      &pop
      &pushregister 2
      &pushregister 3
      &pushbyte 2
      &pushregister 1
      &pushsdw 263							//'refreshReadyCheckbox'
      &callmp
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label63      
     label64:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 263							//'refreshReadyCheckbox'
    &function2  (r:2='checkbox', r:3='slotId') ()
      &pushzero
      &pushsdb 18							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdw 264							//'QueryGameEngine?READY_STATUS?Slot='
      &pushregister 3
      &toString
      &add
      &pushregister 1
      &loadVariables
      &pushregister 1
      &pushsdw 265							//'READY_STATUS_ENABLED'
      &getMember
      &pushsdb 22							//'1'
      &equals
      &jnz label65      
      &pushzero
      &pushregister 2
      &pushsdb 56							//'disable'
      &callMethod
      &jmp label66      
     label65:
      &pushzero
      &pushregister 2
      &pushsdb 38							//'enable'
      &callMethod
     label66:
      &pop
      &pushregister 1
      &pushsdw 266							//'READY_STATUS_VALUE'
      &getMember
      &pushsdb 22							//'1'
      &equals
      &jnz label67      
      &pushzero
      &pushregister 2
      &pushsdw 267							//'SetUnCheck'
      &callMethod
      &jmp label68      
     label67:
      &pushzero
      &pushregister 2
      &pushsdw 268							//'SetCheck'
      &callMethod
     label68:
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 206							//'enableControl'
    &function2  (r:2='enable', r:1='control') ()
      &pushregister 2
      &not
      &jnz label69      
      &pushzero
      &pushregister 1
      &dcallmp 38							// enable()
      &jmp label70      
     label69:
      &pushzero
      &pushregister 1
      &dcallmp 56							// disable()
     label70:
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &pushsdbgm 15							//'prototype'
    &pushbyte 3
    &pushsdw 269							//'ASSetPropFlags'
    &callFunction
   label71:
    &pop
  &end // of initMovieClip 32

  &defineMovieClip 33 // total frames: 0
  &end // of defineMovieClip 33
  
  &exportAssets
    33 &as '__Packages.fem_m_lanGameSetup'
  &end // of exportAssets
  
  &initMovieClip 33
    &constants '_global', 'fem_m_lanGameSetup', 'fem_m_lanGameSetup::fem_m_lanGameSetup() screen = ', 'GameSetupBase', 'prototype', 'frameDelayedInit', 'initReadyControls', 'initPlayButton', 'refreshComponents', 'm_screenButtons', 'registerIntroOuttroComponents', 'startIntro', 'initNavigationButtons', 'fem_m_lanGameSetup::initNavigationButtons() ', 'Cafe2_BaseUIScreen', 'm_screen', 'gameSettings', 'mapTabButton', 'enable', 'show', 'onShowMapPanel', 'bind0', 'setOnMouseUpFunction', 'push', 'rulesTabButton', 'onShowRulesPanel', 'setSelected', 'onScreenExit', 'fem_m_lanGameSetup::onScreenExit()', 'refreshReadyControls', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_lanGameSetup'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'fem_m_lanGameSetup'
    &function2  (r:2='screen') (r:1='super')
      &pushregister 2
      &pushone
      &pushregister 1
      &pushundef
      &callmp
      &pushsdb 2							//'fem_m_lanGameSetup::fem_m_lanGameSetup() screen = '
      &pushregister 2
      &add
      &trace
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_lanGameSetup'
    &pushsdbgv 3							//'GameSetupBase'
    &extends
    &pushregister 1
    &pushsdbgm 4							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 5							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='super')
      &getURL 'FSCommand:CallGameFunction'       '%SetGameSetupMode?Mode=Network'      
      &pushzero
      &pushregister 2
      &dcallmp 5							// frameDelayedInit()
      &pushzero
      &pushregister 1
      &dcallmp 6							// initReadyControls()
      &pushzero
      &pushregister 1
      &dcallmp 7							// initPlayButton()
      &pushzero
      &pushregister 1
      &dcallmp 8							// refreshComponents()
      &pushregister 1
      &pushsdbgm 9							//'m_screenButtons'
      &pushone
      &pushregister 1
      &dcallmp 10							// registerIntroOuttroComponents()
      &pushzero
      &pushregister 1
      &dcallmp 11							// startIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 12							//'initNavigationButtons'
    &function2  (r:4='navButtons') (r:1='this', r:2='super', r:3='_global')
      &pushsdb 13							//'fem_m_lanGameSetup::initNavigationButtons() '
      &trace
      &pushregister 4
      &pushone
      &pushregister 2
      &dcallmp 12							// initNavigationButtons()
      &pushzero
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 17							//'mapTabButton'
      &dcallmp 18							// enable()
      &pushzero
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 17							//'mapTabButton'
      &dcallmp 19							// show()
      &pushregister 1
      &pushsdbgm 20							//'onShowMapPanel'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 21							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 17							//'mapTabButton'
      &dcallmp 22							// setOnMouseUpFunction()
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 17							//'mapTabButton'
      &pushone
      &pushregister 4
      &dcallmp 23							// push()
      &pushzero
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 24							//'rulesTabButton'
      &dcallmp 18							// enable()
      &pushzero
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 24							//'rulesTabButton'
      &dcallmp 19							// show()
      &pushregister 1
      &pushsdbgm 25							//'onShowRulesPanel'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 21							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 24							//'rulesTabButton'
      &dcallmp 22							// setOnMouseUpFunction()
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 24							//'rulesTabButton'
      &pushone
      &pushregister 4
      &dcallmp 23							// push()
      &pushtrue
      &pushone
      &pushsdbgv 14							//'Cafe2_BaseUIScreen'
      &pushsdbgm 15							//'m_screen'
      &pushsdbgm 16							//'gameSettings'
      &pushsdbgm 17							//'mapTabButton'
      &dcallmp 26							// setSelected()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 27							//'onScreenExit'
    &function2  () (r:1='super', r:2='_global')
      &pushsdb 28							//'fem_m_lanGameSetup::onScreenExit()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 27							// onScreenExit()
      &pushregister 2
      &pushsdb 1							//'fem_m_lanGameSetup'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 8							//'refreshComponents'
    &function2  () (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &dcallmp 8							// refreshComponents()
      &pushzero
      &pushregister 1
      &dcallmp 29							// refreshReadyControls()
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_lanGameSetup'
    &pushsdbgm 4							//'prototype'
    &pushbyte 3
    &pushsdb 30							//'ASSetPropFlags'
    &callFunction
   label1:
    &pop
  &end // of initMovieClip 33
&end
