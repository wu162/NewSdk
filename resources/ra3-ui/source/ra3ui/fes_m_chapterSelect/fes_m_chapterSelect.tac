movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FES_M_~1\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_chapterSelect'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 3
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 6
  &end // of importAssets

  &placeMovieClip 6 &as 'button_Back'
  
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
      &pushone
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$BACK'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 6

  &placeMovieClip 6 &as 'button_Play'
  
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
      &pushzerosv
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$PLAY'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 6
  
  &importAssets &from '../mouse/Components/mouse_chapterSelectComponent.swf'
    'chapterSelectComponentSymbol' &as 7
  &end // of importAssets
  
  &importAssets &from 'screens/fe_m_assetsGameSetup.swf'
    'MapPreviewComponent' &as 10
  &end // of importAssets

  &placeMovieClip 10 &as 'mapPreview'
  
    &onClipEvent &construct
      &pushs '_type'
      &pushssv 'ChapterMapPreview'
      &pushs '_indexMap'
      &pushbyte -1
      &setVariable
    &end
  &end // of placeMovieClip 10
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 13
  &end // of importAssets

  &placeMovieClip 13 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$HEADERCHAPTERSELECT'
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
  &end // of placeMovieClip 13

  &defineMovieClip 14 // total frames: 0
  &end // of defineMovieClip 14
  
  &exportAssets
    14 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 14
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
  &end // of initMovieClip 14

  &defineMovieClip 15 // total frames: 0
  &end // of defineMovieClip 15
  
  &exportAssets
    15 &as '__Packages.fes_m_chapterSelect'
  &end // of exportAssets
  
  &initMovieClip 15
    &constants '_global', 'fes_m_chapterSelect', 'fes_m_chapterSelect::fes_m_chapterSelect()', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_chapterSelectPanel', 'chapterSelectPanel', 'prototype', 'frameDelayedInit', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind0', 'gMH', 'addMessageHandler', 'm_eventHandlerIndex', 'localEventHandler', 'gEH', 'addLocalEventHandler', 'enableLocalEventHandler', 'initGUI', 'startIntro', 'onScreenExit', 'gSM', 'setOnExitScreen', 'Array', 'm_screen', 'button_Back', 'enable', 'OnBackButtonClick', 'setOnMouseUpFunction', 'push', 'button_Play', 'OnPlayButtonClick', 'm_chapterSelectCallback', 'onChapterSelectChanged', 'bind1DynamicParams', 'addObserver', 'registerIntroOuttroComponents', 'FSCommand:CallGameFunction', '%SetMap?Map=', 'getSelectedMapId', '%Campaign_SetMission?MissionIndex=', 'getSelectedMission', 'INPUTCODE', 'ESCAPE', 'gFM', 'handleDefaultInputs', 'removeObserver', 'removeMessageHandler', 'removeLocalEventHandler', 'goBackScreen', '%LoadAutoSaveSilent?FactionIndex=', 'getSelectedCampaign', 'GAME', 'COOPMODE', 'ENUM', 'GAME_COOPMODE_CHOOSER', 'PLAY_MISSION_INTRO_MOVIE', 'SCREEN', 'FES_COOP_MISSION_BRIEFING', 'changeMainScreen', 'OnQuickLaunchButtonClick', '%Campaign_SetDifficulty?Difficulty=', 'getSelectedDifficulty', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_chapterSelect'
    &not
    &not
    &jnz label6    
    &pushglobalgv
    &pushsdb 1							//'fes_m_chapterSelect'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushregister 3
      &pushone
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'fes_m_chapterSelect::fes_m_chapterSelect()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushsdb 5							//'m_chapterSelectPanel'
      &pushregister 3
      &pushsdbgm 6							//'chapterSelectPanel'
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_chapterSelect'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 7							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 8							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 9							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 10							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 11							//'bind0'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 9							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 12							//'gMH'
      &dcallmp 13							// addMessageHandler()
      &pushregister 1
      &pushsdb 14							//'m_eventHandlerIndex'
      &pushregister 1
      &pushsdbgm 15							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 16							//'gEH'
      &pushsdb 17							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 16							//'gEH'
      &dcallmp 18							// enableLocalEventHandler()
      &pushzero
      &pushregister 1
      &dcallmp 19							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 20							// startIntro()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 19							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 21							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 11							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 22							//'gSM'
      &dcallmp 23							// setOnExitScreen()
      &pushzero
      &pushsdb 24							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 25							//'m_screen'
      &pushsdbgm 26							//'button_Back'
      &setRegister r:5
      &pop
      &pushzero
      &pushregister 5
      &dcallmp 27							// enable()
      &pushregister 1
      &pushsdbgm 28							//'OnBackButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 11							//'bind0'
      &callMethod
      &pushone
      &pushregister 5
      &dcallmp 29							// setOnMouseUpFunction()
      &pushregister 5
      &pushone
      &pushregister 4
      &dcallmp 30							// push()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 25							//'m_screen'
      &pushsdbgm 31							//'button_Play'
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &dcallmp 27							// enable()
      &pushregister 1
      &pushsdbgm 32							//'OnPlayButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 11							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 29							// setOnMouseUpFunction()
      &pushregister 3
      &pushone
      &pushregister 4
      &dcallmp 30							// push()
      &pushregister 1
      &pushsdb 33							//'m_chapterSelectCallback'
      &pushregister 1
      &pushsdbgm 34							//'onChapterSelectChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 33							//'m_chapterSelectCallback'
      &pushone
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &dcallmp 36							// addObserver()
      &pushregister 4
      &pushone
      &pushregister 1
      &dcallmp 37							// registerIntroOuttroComponents()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 34							//'onChapterSelectChanged'
    &function2  () (r:1='this')
      &pushsdb 38							//'FSCommand:CallGameFunction'
      &pushsdb 39							//'%SetMap?Map='
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 25							//'m_screen'
      &pushsdbgm 6							//'chapterSelectPanel'
      &pushsdb 40							//'getSelectedMapId'
      &callMethod
      &toString
      &add
      &getURL2
      &pushsdb 38							//'FSCommand:CallGameFunction'
      &pushsdb 41							//'%Campaign_SetMission?MissionIndex='
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 42							//'getSelectedMission'
      &callMethod
      &toString
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 10							//'localMessageHandler'
    &function2  (r:2='messageCode') ()
      &pushtrue
      &setRegister r:1
      &pop
      &jmp label1      
     label1:
      &pushfalse
      &setRegister r:1
      &pop
      &jmp label2      
     label2:
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 15							//'localEventHandler'
    &function2  (r:3='keyCode', 'controller') (r:1='this', r:2='_global')
      &pushregister 3
      &pushsdbgv 43							//'INPUTCODE'
      &pushsdbgm 44							//'ESCAPE'
      &strictEquals
      &jnz label3      
      &jmp label4      
     label3:
      &pushzero
      &pushregister 1
      &dcallmp 28							// OnBackButtonClick()
      &jmp label5      
     label4:
      &pushregister 3
      &pushone
      &pushregister 2
      &pushsdbgm 45							//'gFM'
      &dcallmp 46							// handleDefaultInputs()
      &jmp label5      
     label5:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 21							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 33							//'m_chapterSelectCallback'
      &pushone
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &dcallmp 47							// removeObserver()
      &pushregister 1
      &pushsdb 33							//'m_chapterSelectCallback'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 9							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 12							//'gMH'
      &dcallmp 48							// removeMessageHandler()
      &pushregister 1
      &pushsdb 9							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 14							//'m_eventHandlerIndex'
      &pushone
      &pushregister 2
      &pushsdbgm 16							//'gEH'
      &dcallmp 49							// removeLocalEventHandler()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 25							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'fes_m_chapterSelect'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 28							//'OnBackButtonClick'
    &function2  () (r:1='_global')
      &pushzero
      &pushregister 1
      &pushsdbgm 22							//'gSM'
      &dcallmp 50							// goBackScreen()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 32							//'OnPlayButtonClick'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 42							//'getSelectedMission'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdb 38							//'FSCommand:CallGameFunction'
      &pushsdb 51							//'%LoadAutoSaveSilent?FactionIndex='
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 52							//'getSelectedCampaign'
      &callMethod
      &add
      &getURL2
      &pushsdb 38							//'FSCommand:CallGameFunction'
      &pushsdb 41							//'%Campaign_SetMission?MissionIndex='
      &pushregister 3
      &add
      &getURL2
      &pushregister 2
      &pushsdbgm 53							//'GAME'
      &pushsdb 54							//'COOPMODE'
      &pushregister 2
      &pushsdbgm 55							//'ENUM'
      &pushsdbgm 56							//'GAME_COOPMODE_CHOOSER'
      &setMember
      &pushregister 2
      &pushsdbgm 53							//'GAME'
      &pushsdb 57							//'PLAY_MISSION_INTRO_MOVIE'
      &pushtrue
      &setMember
      &pushregister 2
      &pushsdbgm 58							//'SCREEN'
      &pushsdbgm 59							//'FES_COOP_MISSION_BRIEFING'
      &pushone
      &pushregister 2
      &pushsdbgm 22							//'gSM'
      &dcallmp 60							// changeMainScreen()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 61							//'OnQuickLaunchButtonClick'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 42							//'getSelectedMission'
      &callMethod
      &setRegister r:2
      &pop
      &pushsdb 38							//'FSCommand:CallGameFunction'
      &pushsdb 41							//'%Campaign_SetMission?MissionIndex='
      &pushregister 2
      &add
      &getURL2
      &getURL 'FSCommand:CallGameFunction'       '%Mission_SetAI?AIIndex=0'      
      &pushsdb 38							//'FSCommand:CallGameFunction'
      &pushsdb 62							//'%Campaign_SetDifficulty?Difficulty='
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 63							//'getSelectedDifficulty'
      &callMethod
      &add
      &getURL2
      &getURL 'FSCommand:CallGameFunction'       '%Campaign_StartMission'      
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_chapterSelect'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 64							//'ASSetPropFlags'
    &callFunction
   label6:
    &pop
  &end // of initMovieClip 15
&end
