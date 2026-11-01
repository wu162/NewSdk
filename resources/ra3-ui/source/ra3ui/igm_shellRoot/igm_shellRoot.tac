movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IGM_SH~1\\IGM_SH~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'igm_shellRoot'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 1 // total frames: 0
  &end // of defineMovieClip 1
  
  &exportAssets
    1 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 1
    &constants '_global', 'Cafe2_BaseUIScreen', 'prototype', 'outtroComplete', 'Cafe2_BaseUIScreen::outtroComplete() ', 'm_thisClass', 'm_uiComponentArray', 'length', 'splice', 'm_changeToScreen', 'gSM', 'changeMainScreen', 'introComplete', 'Cafe2_BaseUIScreen::introComplete()', 'restoreVisualState', 'm_introCount', 'onIntrosComplete', 'Cafe2_BaseUIScreen::onIntrosComplete()', 'localEventHandler', 'gEH', 'setLocalEventHandler', 'enableCurrentLocalEventHandler', 'registerIntroOuttroComponents', 'Cafe2_BaseUIScreen::registerIntroOuttroComponents()', 'setIntroCallback', 'executeBackFunction', 'Cafe2_BaseUIScreen::executeBackFunction', 'goBackScreen', 'startIntro', 'Cafe2_BaseUIScreen::startIntro()', 'intro', 'startOuttro', 'Cafe2_BaseUIScreen::startOuttro', 'outtro', 'backButtonClicked', 'Cafe2_BaseUIScreen::backButtonClicked()', 'Cafe2_BaseUIScreen::localEventHandler() keyCode == ', 'gFM', 'handleDefaultInputs', 'clearData', 'm_screen', 'bind0', 'obj', 'func', 'call', 'Array', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &not
    &not
    &jnz label20    
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
    &pushsdb 41							//'bind0'
    &function  ('obj', 'func'    )    
      &function  (      )      
        &pushsdbgv 42							//'obj'
        &pushone
        &pushsdbgv 43							//'func'
        &dcallmp 44							// call()
      &end // of function 

      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 9							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 1
    &pushsdb 6							//'m_uiComponentArray'
    &pushzero
    &pushsdb 45							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &pushsdbgm 2							//'prototype'
    &pushbyte 3
    &pushsdb 46							//'ASSetPropFlags'
    &callFunction
   label20:
    &pop
  &end // of initMovieClip 1

  &defineMovieClip 2 // total frames: 0
  &end // of defineMovieClip 2
  
  &exportAssets
    2 &as '__Packages.igm_shellRoot'
  &end // of exportAssets
  
  &initMovieClip 2
    &constants '_global', 'igm_shellRoot', 'igm_shellRoot::igm_shellRoot() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'Object', 'QueryGameEngine?igm_shellRoot_mode', 'igm_shellRoot_mode', 'PauseMenu', 'LoadMenu', 'SaveMenu', 'SCREEN', 'IGM_PAUSE_MENU', 'GAME', 'SAVELOADSCREEN_TYPE', 'ENUM', 'SAVELOADSCREEN_LOAD', 'FEG_SAVELOAD', 'SAVELOADSCREEN_SAVE', 'changeMainScreen', 'prototype', 'obj', 'fn', 'call', 'igm_shellRoot::onScreenExit()', 'clearData', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'igm_shellRoot'
    &not
    &not
    &jnz label6    
    &pushglobalgv
    &pushsdb 1							//'igm_shellRoot'
    &function2  (r:6='screen') (r:1='this', r:2='super', r:3='_global')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'igm_shellRoot::igm_shellRoot() screen = '
      &pushregister 6
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushregister 6
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushregister 1
      &pushsdbgm 6							//'onScreenExit'
      &pushbyte 2
      &pushsdbgv 1							//'igm_shellRoot'
      &pushsdb 7							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 8							//'gSM'
      &dcallmp 9							// setOnExitScreen()
      &pushzero
      &pushsdb 10							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 11							//'QueryGameEngine?igm_shellRoot_mode'
      &pushregister 5
      &loadVariables
      &pushundef
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgm 12							//'igm_shellRoot_mode'
      &setRegister r:0
      &pushsdb 13							//'PauseMenu'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushsdb 14							//'LoadMenu'
      &strictEquals
      &jnz label2      
      &pushregister 0
      &pushsdb 15							//'SaveMenu'
      &strictEquals
      &jnz label3      
      &jmp label4      
     label1:
      &pushregister 3
      &pushsdbgm 16							//'SCREEN'
      &pushsdbgm 17							//'IGM_PAUSE_MENU'
      &setRegister r:4
      &pop
      &jmp label5      
     label2:
      &pushregister 3
      &pushsdbgm 18							//'GAME'
      &pushsdb 19							//'SAVELOADSCREEN_TYPE'
      &pushregister 3
      &pushsdbgm 20							//'ENUM'
      &pushsdbgm 21							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 3
      &pushsdbgm 16							//'SCREEN'
      &pushsdbgm 22							//'FEG_SAVELOAD'
      &setRegister r:4
      &pop
      &jmp label5      
     label3:
      &pushregister 3
      &pushsdbgm 18							//'GAME'
      &pushsdb 19							//'SAVELOADSCREEN_TYPE'
      &pushregister 3
      &pushsdbgm 20							//'ENUM'
      &pushsdbgm 23							//'SAVELOADSCREEN_SAVE'
      &setMember
      &pushregister 3
      &pushsdbgm 16							//'SCREEN'
      &pushsdbgm 22							//'FEG_SAVELOAD'
      &setRegister r:4
      &pop
      &jmp label5      
     label4:
      &getURL 'FSCommand:CallGameFunction'       '%igm_shellRoot_onClosed'      
      &pushundef
      &return
     label5:
      &getURL 'FSCommand:CallGameFunction'       '%igm_shellRoot_onChildLoaded'      
      &pushregister 4
      &pushone
      &pushregister 3
      &pushsdbgm 8							//'gSM'
      &dcallmp 24							// changeMainScreen()
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'igm_shellRoot'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 25							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 0
    &pushsdb 7							//'bind0'
    &function  ('fn', 'obj'    )    
      &function2  (r:1='arg0') ()
        &pushregister 1
        &pushsdbgv 26							//'obj'
        &pushbyte 2
        &pushsdbgv 27							//'fn'
        &pushsdb 28							//'call'
        &callMethod
        &return
      &end // of function 

      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 6							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 29							//'igm_shellRoot::onScreenExit()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 30							// clearData()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'igm_shellRoot'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'igm_shellRoot'
    &pushsdbgm 25							//'prototype'
    &pushbyte 3
    &pushsdb 31							//'ASSetPropFlags'
    &callFunction
   label6:
    &pop
  &end // of initMovieClip 2
&end
