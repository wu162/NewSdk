movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FE5469~1\\FEG_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_load'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &placeMovieClip 9 
  
    &onClipEvent &construct
      &pushs '_type'
      &pushssv 'RenderImage'
      &pushs '_imageMap'
      &pushssv 'EngagingContentImage'
      &pushs '_mode'
      &pushssv ''
    &end
  &end // of placeMovieClip 9

  &defineMovieClip 13 // total frames: 125
  &end // of defineMovieClip 13

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 29 // total frames: 1
  &end // of defineMovieClip 29

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
    31 &as '__Packages.feg_m_load'
  &end // of exportAssets
  
  &initMovieClip 31
    &constants '_global', 'feg_m_load', 'feg_m_load::feg_m_load() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'initGUI', 'prototype', 'feg_m_load::initGUI()', 'onScreenExit', 'gSM', 'setOnExitScreen', 'm_pips', 'Array', 'pip0', '_visible', 'm_pipCount', 'm_levelOffset', 'pip', 'duplicateMovieClip', '_x', 'm_pipSpacing', 'localMessageHandler', 'gMH', 'addMessageHandler', 'feg_m_load::onScreenExit()', 'removeMessageHandler', 'clearData', 'feg_m_load::localMessageHandler() messageCode = \'', '\'', 'MSGCODE', 'FE_UPDATE_LOADSCREEN', 'FE_LOADING_COMPLETE', 'feg_m_load::localMessageHandler() Updating load progress', 'updateLoadProgress', 'feg_m_load::localMessageHandler() Playing load screen outtro', '_outtro', 'gotoAndPlay', 'fem_m_load::updateLoadProgress()', 'Object', 'QueryGameEngine?LOAD_PROGRESS', 'LOAD_PROGRESS', 'updateProgressDisplay', 'fem_m_load::updateProgressDisplay() Progress: ', 'fem_m_load::updateProgressDisplay() Pips to turn on: ', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_load'
    &not
    &not
    &jnz label8    
    &pushglobalgv
    &pushsdb 1							//'feg_m_load'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'feg_m_load::feg_m_load() screen = '
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
      &pushregister 1
      &dcallmp 6							// initGUI()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_load'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 7							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 6							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 8							//'feg_m_load::initGUI()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 9							//'onScreenExit'
      &pushone
      &pushregister 2
      &pushsdbgm 10							//'gSM'
      &dcallmp 11							// setOnExitScreen()
      &pushregister 1
      &pushsdb 12							//'m_pips'
      &pushzero
      &pushsdb 13							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdbgm 12							//'m_pips'
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 14							//'pip0'
      &setMember
      &pushregister 1
      &pushsdbgm 12							//'m_pips'
      &pushzero
      &getMember
      &pushsdb 15							//'_visible'
      &pushfalse
      &setMember
      &pushone
      &setRegister r:3
      &pop
     label1:
      &pushregister 3
      &pushsdbgv 1							//'feg_m_load'
      &pushsdbgm 16							//'m_pipCount'
      &lessThan
      &not
      &jnz label2      
      &pushregister 1
      &pushsdbgm 12							//'m_pips'
      &pushregister 3
      &pushsdbgv 1							//'feg_m_load'
      &pushsdbgm 17							//'m_levelOffset'
      &pushsdbgv 1							//'feg_m_load'
      &pushsdbgm 16							//'m_pipCount'
      &add
      &pushregister 3
      &subtract
      &pushsdb 18							//'pip'
      &pushregister 3
      &toString
      &add
      &pushbyte 2
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 14							//'pip0'
      &pushsdb 19							//'duplicateMovieClip'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 12							//'m_pips'
      &pushregister 3
      &getMember
      &pushsdb 20							//'_x'
      &pushregister 1
      &pushsdbgm 12							//'m_pips'
      &pushregister 3
      &pushone
      &subtract
      &getMember
      &pushsdbgm 20							//'_x'
      &pushsdbgv 1							//'feg_m_load'
      &pushsdbgm 21							//'m_pipSpacing'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 12							//'m_pips'
      &pushregister 3
      &getMember
      &pushsdb 15							//'_visible'
      &pushfalse
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label1      
     label2:
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 22							//'localMessageHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 23							//'gMH'
      &dcallmp 24							// addMessageHandler()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 9							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 25							//'feg_m_load::onScreenExit()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &pushsdbgm 22							//'localMessageHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 23							//'gMH'
      &dcallmp 26							// removeMessageHandler()
      &pushzero
      &pushregister 1
      &dcallmp 27							// clearData()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'feg_m_load'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 22							//'localMessageHandler'
    &function2  (r:3='messageCode') (r:1='_global')
      &pushsdb 28							//'feg_m_load::localMessageHandler() messageCode = \''
      &pushregister 3
      &add
      &pushsdb 29							//'\''
      &add
      &trace
      &pushfalse
      &setRegister r:2
      &pop
      &pushregister 3
      &setRegister r:0
      &pushregister 1
      &pushsdbgm 30							//'MSGCODE'
      &pushsdbgm 31							//'FE_UPDATE_LOADSCREEN'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 1
      &pushsdbgm 30							//'MSGCODE'
      &pushsdbgm 32							//'FE_LOADING_COMPLETE'
      &strictEquals
      &jnz label4      
      &jmp label5      
     label3:
      &pushsdb 33							//'feg_m_load::localMessageHandler() Updating load progress'
      &trace
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 5							//'m_thisClass'
      &dcallmp 34							// updateLoadProgress()
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label5      
     label4:
      &pushsdb 35							//'feg_m_load::localMessageHandler() Playing load screen outtro'
      &trace
      &pushsdb 36							//'_outtro'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &dcallmp 37							// gotoAndPlay()
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label5      
     label5:
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'updateLoadProgress'
    &function2  () (r:1='this')
      &pushsdb 38							//'fem_m_load::updateLoadProgress()'
      &trace
      &pushzero
      &pushsdb 39							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 40							//'QueryGameEngine?LOAD_PROGRESS'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 41							//'LOAD_PROGRESS'
      &pushone
      &pushregister 1
      &dcallmp 42							// updateProgressDisplay()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 42							//'updateProgressDisplay'
    &function2  (r:4='percentComplete') (r:1='this')
      &pushsdb 43							//'fem_m_load::updateProgressDisplay() Progress: '
      &pushregister 4
      &add
      &trace
      &pushregister 4
      &pushbyte 100
      &divide
      &pushsdbgv 1							//'feg_m_load'
      &pushsdbgm 16							//'m_pipCount'
      &multiply
      &setRegister r:3
      &pop
      &pushsdb 44							//'fem_m_load::updateProgressDisplay() Pips to turn on: '
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label6:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label7      
      &pushregister 1
      &pushsdbgm 12							//'m_pips'
      &pushregister 2
      &getMember
      &pushsdb 15							//'_visible'
      &pushtrue
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label6      
     label7:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 17							//'m_levelOffset'
    &pushbyte 5
    &setMember
    &pushregister 1
    &pushsdb 21							//'m_pipSpacing'
    &pushbyte 10
    &setMember
    &pushregister 1
    &pushsdb 16							//'m_pipCount'
    &pushbyte 64
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_load'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 45							//'ASSetPropFlags'
    &callFunction
   label8:
    &pop
  &end // of initMovieClip 31
&end
