movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\fem_m_load\\fem_m_load.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &defineMovieClip 11 // total frames: 101
  &end // of defineMovieClip 11

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as 'UnrankedMultiplayerLoadingBox'
  &end // of exportAssets

  &defineMovieClip 27 // total frames: 1
  &end // of defineMovieClip 27

  &defineMovieClip 33 // total frames: 1
  &end // of defineMovieClip 33

  &defineMovieClip 34 // total frames: 1
  &end // of defineMovieClip 34
  
  &exportAssets
    34 &as 'RankedMultiplayerLoadingBox'
  &end // of exportAssets

  &frame 0
    &stop
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fem_m_load'
    &new
    &varEquals
  &end // of frame 0

  &defineMovieClip 37 // total frames: 1
  &end // of defineMovieClip 37
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 38
  &end // of importAssets

  &defineMovieClip 41 // total frames: 1
  &end // of defineMovieClip 41

  &defineMovieClip 42 // total frames: 125
  &end // of defineMovieClip 42

  &defineMovieClip 45 // total frames: 1
  &end // of defineMovieClip 45

  &defineMovieClip 46 // total frames: 1
  &end // of defineMovieClip 46
  
  &exportAssets
    34 &as 'RankedMultiplayerLoadingBox'
  &end // of exportAssets
  
  &exportAssets
    19 &as 'UnrankedMultiplayerLoadingBox'
  &end // of exportAssets
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 47
  &end // of importAssets

  &placeMovieClip 47 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$MP_LOADING_SCREEN_TITLE'
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
  &end // of placeMovieClip 47

  &defineMovieClip 51 // total frames: 1
  &end // of defineMovieClip 51

  &defineMovieClip 53 // total frames: 1
  &end // of defineMovieClip 53

  &defineMovieClip 54 // total frames: 1

    &placeMovieClip 53 
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'MapPreview'
      &end
    &end // of placeMovieClip 53
  &end // of defineMovieClip 54

  &defineMovieClip 55 // total frames: 0
  &end // of defineMovieClip 55
  
  &exportAssets
    55 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 55
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
  &end // of initMovieClip 55

  &defineMovieClip 56 // total frames: 0
  &end // of defineMovieClip 56
  
  &exportAssets
    56 &as '__Packages.fem_m_load'
  &end // of exportAssets
  
  &initMovieClip 56
    &constants '_global', 'fem_m_load', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind1DynamicParams', 'Object', 'QueryGameEngine?IS_RANKED_GAME', 'm_isRankedGame', 'IS_RANKED_GAME', '1', 'unrankedMultiplayerLoadingBox', 'rankedMultiplayerLoadingBox', '_visible', 'm_playerClips', 'entry5', 'entry4', 'entry3', 'entry2', 'entry1', 'entry0', 'updateLoadProgress', 'prototype', 'frameDelayedInit', 'gMH', 'addMessageHandler', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'removeMessageHandler', 'clearData', 'MSGCODE', 'FE_UPDATE_LOADSCREEN', 'FE_LOADING_COMPLETE', '_outtro', 'm_screen', 'gotoAndPlay', 'QueryGameEngine?LOAD_INFO', ',', 'LOAD_INFO_PRESENT', 'split', 'LOAD_INFO_NAMES', 'LOAD_INFO_TEAMS', 'LOAD_INFO_FACTIONS', 'LOAD_INFO_PROGRESS', 'LOAD_INFO_PROGRESS_STR', 'LOAD_INFO_RANKS', 'length', 'playerTF', 'text', 'teamTF', 'factionTF', 'progressTF', 'rankTF', 'loadingbar', 'gotoAndStop', 'Array', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_load'
    &not
    &not
    &jnz label12    
    &pushglobalgv
    &pushsdb 1							//'fem_m_load'
    &function2  (r:5='screen') (r:1='this', r:2='super', r:3='_global')
      &pushregister 5
      &pushone
      &pushregister 2
      &pushundef
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushsdb 4							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 5							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushzero
      &pushsdb 7							//'Object'
      &new
      &setRegister r:6
      &pop
      &pushsdb 8							//'QueryGameEngine?IS_RANKED_GAME'
      &pushregister 6
      &loadVariables
      &pushregister 1
      &pushsdb 9							//'m_isRankedGame'
      &pushregister 6
      &pushsdbgm 10							//'IS_RANKED_GAME'
      &pushsdb 11							//'1'
      &equals
      &setMember
      &pushundef
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 9							//'m_isRankedGame'
      &jnz label1      
      &pushregister 5
      &pushsdbgm 12							//'unrankedMultiplayerLoadingBox'
      &jmp label2      
     label1:
      &pushregister 5
      &pushsdbgm 13							//'rankedMultiplayerLoadingBox'
     label2:
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgm 12							//'unrankedMultiplayerLoadingBox'
      &pushsdb 14							//'_visible'
      &pushregister 1
      &pushsdbgm 9							//'m_isRankedGame'
      &not
      &setMember
      &pushregister 5
      &pushsdbgm 13							//'rankedMultiplayerLoadingBox'
      &pushsdb 14							//'_visible'
      &pushregister 1
      &pushsdbgm 9							//'m_isRankedGame'
      &setMember
      &pushregister 1
      &pushsdb 15							//'m_playerClips'
      &pushregister 4
      &pushsdbgm 16							//'entry5'
      &pushregister 4
      &pushsdbgm 17							//'entry4'
      &pushregister 4
      &pushsdbgm 18							//'entry3'
      &pushregister 4
      &pushsdbgm 19							//'entry2'
      &pushregister 4
      &pushsdbgm 20							//'entry1'
      &pushregister 4
      &pushsdbgm 21							//'entry0'
      &pushbyte 6
      &initArray
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 22							// updateLoadProgress()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_load'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 23							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 24							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 4							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 25							//'gMH'
      &dcallmp 26							// addMessageHandler()
      &pushregister 1
      &pushsdbgm 27							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 28							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 29							//'gSM'
      &dcallmp 30							// setOnExitScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 27							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 4							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 25							//'gMH'
      &dcallmp 31							// removeMessageHandler()
      &pushregister 1
      &pushsdb 4							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 32							// clearData()
      &pushregister 2
      &pushsdb 1							//'fem_m_load'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 5							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushtrue
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 33							//'MSGCODE'
      &pushsdbgm 34							//'FE_UPDATE_LOADSCREEN'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 33							//'MSGCODE'
      &pushsdbgm 35							//'FE_LOADING_COMPLETE'
      &strictEquals
      &jnz label4      
      &jmp label5      
     label3:
      &pushzero
      &pushregister 1
      &dcallmp 22							// updateLoadProgress()
      &jmp label6      
     label4:
      &pushsdb 36							//'_outtro'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 37							//'m_screen'
      &dcallmp 38							// gotoAndPlay()
      &jmp label6      
     label5:
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label6      
     label6:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 22							//'updateLoadProgress'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 7							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 39							//'QueryGameEngine?LOAD_INFO'
      &pushregister 3
      &loadVariables
      &pushsdb 40							//','
      &pushone
      &pushregister 3
      &pushsdbgm 41							//'LOAD_INFO_PRESENT'
      &pushsdb 42							//'split'
      &callMethod
      &setRegister r:7
      &pop
      &pushsdb 40							//','
      &pushone
      &pushregister 3
      &pushsdbgm 43							//'LOAD_INFO_NAMES'
      &pushsdb 42							//'split'
      &callMethod
      &setRegister r:8
      &pop
      &pushsdb 40							//','
      &pushone
      &pushregister 3
      &pushsdbgm 44							//'LOAD_INFO_TEAMS'
      &pushsdb 42							//'split'
      &callMethod
      &setRegister r:6
      &pop
      &pushsdb 40							//','
      &pushone
      &pushregister 3
      &pushsdbgm 45							//'LOAD_INFO_FACTIONS'
      &pushsdb 42							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushsdb 40							//','
      &pushone
      &pushregister 3
      &pushsdbgm 46							//'LOAD_INFO_PROGRESS'
      &pushsdb 42							//'split'
      &callMethod
      &setRegister r:10
      &pop
      &pushsdb 40							//','
      &pushone
      &pushregister 3
      &pushsdbgm 47							//'LOAD_INFO_PROGRESS_STR'
      &pushsdb 42							//'split'
      &callMethod
      &setRegister r:5
      &pop
      &pushsdb 40							//','
      &pushone
      &pushregister 3
      &pushsdbgm 48							//'LOAD_INFO_RANKS'
      &pushsdb 42							//'split'
      &callMethod
      &setRegister r:9
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label7:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushsdbgm 49							//'length'
      &lessThan
      &not
      &jnz label11      
      &pushregister 7
      &pushregister 2
      &getMember
      &toNumber
      &pushzero
      &equals
      &not
      &not
      &jnz label9      
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 50							//'playerTF'
      &pushsdb 51							//'text'
      &pushregister 8
      &pushregister 2
      &getMember
      &setMember
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 52							//'teamTF'
      &pushsdb 51							//'text'
      &pushregister 6
      &pushregister 2
      &getMember
      &setMember
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 53							//'factionTF'
      &pushsdb 51							//'text'
      &pushregister 4
      &pushregister 2
      &getMember
      &setMember
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 54							//'progressTF'
      &pushsdb 51							//'text'
      &pushregister 5
      &pushregister 2
      &getMember
      &setMember
      &pushregister 1
      &pushsdbgm 9							//'m_isRankedGame'
      &not
      &jnz label8      
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 55							//'rankTF'
      &pushsdb 51							//'text'
      &pushregister 9
      &pushregister 2
      &getMember
      &setMember
     label8:
      &pushregister 10
      &pushregister 2
      &getMember
      &toNumber
      &pushone
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 56							//'loadingbar'
      &dcallmp 57							// gotoAndStop()
      &jmp label10      
     label9:
      &pushregister 1
      &pushsdbgm 15							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
     label10:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label7      
     label11:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 15							//'m_playerClips'
    &pushzero
    &pushsdb 58							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_load'
    &pushsdbgm 23							//'prototype'
    &pushbyte 3
    &pushsdb 59							//'ASSetPropFlags'
    &callFunction
   label12:
    &pop
  &end // of initMovieClip 56
&end
