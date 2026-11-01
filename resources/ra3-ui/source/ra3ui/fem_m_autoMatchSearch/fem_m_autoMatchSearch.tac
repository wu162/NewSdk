movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEM_M_~1\\FEM_M_~1.eaf' &compressed // flash 7, total frames: 60, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fem_m_autoMatchSearch'
    &new
    &varEquals
  &end // of frame 0
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 1
  &end // of importAssets

  &placeMovieClip 1 &as 'backButton'
  
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
      &pushssv '$Back'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 1

  &defineMovieClip 6 // total frames: 0
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 6
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
    &pushregister 2
    &pushsdb 9							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 2
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
  &end // of initMovieClip 6

  &defineMovieClip 7 // total frames: 0
  &end // of defineMovieClip 7
  
  &exportAssets
    7 &as '__Packages.autoMatchSearch'
  &end // of exportAssets
  
  &initMovieClip 7
    &constants '_global', 'autoMatchSearch', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'prototype', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'autoMatchSearch'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'autoMatchSearch'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_screen'
      &pushregister 3
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushregister 1
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'autoMatchSearch'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 5							//'prototype'
    &setRegister r:2
    &pop
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'autoMatchSearch'
    &pushsdbgm 5							//'prototype'
    &pushbyte 3
    &pushsdb 6							//'ASSetPropFlags'
    &callFunction
   label1:
    &pop
  &end // of initMovieClip 7

  &defineMovieClip 8 // total frames: 0
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as '__Packages.fem_m_autoMatchSearch'
  &end // of exportAssets
  
  &initMovieClip 8
    &constants '_global', 'fem_m_autoMatchSearch', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'localMessageHandler', 'gMH', 'addMessageHandler', 'initGUI', 'startAutoMatchSearch', 'autoMatchSearch', 'prototype', 'initButtons', 'backButton', 'registerIntroOuttroComponents', 'onScreenExit', 'gSM', 'setOnExitScreen', 'startIntro', 'removeMessageHandler', 'clearData', 'show', 'enable', 'cancelAutoMatchSearch', 'setOnMouseUpFunction', 'backButtonClicked', 'MSGCODE', 'FE_MP_AUTOMATCH_SEARCH_FAILED', 'failedAutoMatchSearch', 'fem_m_autoMatchSearch::failedAutoMatchSearch()', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_autoMatchSearch'
    &not
    &not
    &jnz label4    
    &pushglobalgv
    &pushsdb 1							//'fem_m_autoMatchSearch'
    &function2  (r:4='screen') (r:1='this', r:2='super', r:3='_global')
      &pushregister 4
      &pushone
      &pushregister 2
      &pushundef
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_screen'
      &pushregister 4
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 5							//'localMessageHandler'
      &pushone
      &pushregister 3
      &pushsdbgm 6							//'gMH'
      &dcallmp 7							// addMessageHandler()
      &pushzero
      &pushregister 1
      &dcallmp 8							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 9							// startAutoMatchSearch()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_autoMatchSearch'
    &pushsdbgv 10							//'autoMatchSearch'
    &extends
    &pushregister 1
    &pushsdbgm 11							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 8							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushregister 1
      &dcallmp 12							// initButtons()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 13							//'backButton'
      &pushone
      &initArray
      &pushone
      &pushregister 1
      &dcallmp 14							// registerIntroOuttroComponents()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 15							//'onScreenExit'
      &pushone
      &pushregister 2
      &pushsdbgm 16							//'gSM'
      &dcallmp 17							// setOnExitScreen()
      &pushzero
      &pushregister 1
      &dcallmp 18							// startIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 15							//'onScreenExit'
    &function2  () (r:1='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 5							//'localMessageHandler'
      &pushone
      &pushregister 1
      &pushsdbgm 6							//'gMH'
      &dcallmp 19							// removeMessageHandler()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 20							// clearData()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 1							//'fem_m_autoMatchSearch'
      &delete
      &pop
      &pushregister 1
      &pushsdb 10							//'autoMatchSearch'
      &delete
      &pop
      &pushregister 1
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 12							//'initButtons'
    &function2  () ()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 13							//'backButton'
      &setRegister r:1
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 21							// show()
      &pushzero
      &pushregister 1
      &dcallmp 22							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 23							//'cancelAutoMatchSearch'
      &pushone
      &pushregister 1
      &dcallmp 24							// setOnMouseUpFunction()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 9							//'startAutoMatchSearch'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       'STARTAUTOMATCHSEARCH'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 23							//'cancelAutoMatchSearch'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       'CANCELAUTOMATCHSEARCH'      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 25							// backButtonClicked()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 5							//'localMessageHandler'
    &function2  (r:3='messageCode') (r:1='_global')
      &pushtrue
      &setRegister r:2
      &pop
      &pushregister 3
      &pushregister 1
      &pushsdbgm 26							//'MSGCODE'
      &pushsdbgm 27							//'FE_MP_AUTOMATCH_SEARCH_FAILED'
      &strictEquals
      &jnz label1      
      &jmp label2      
     label1:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 28							// failedAutoMatchSearch()
      &jmp label3      
     label2:
      &pushfalse
      &setRegister r:2
      &pop
      &jmp label3      
     label3:
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'failedAutoMatchSearch'
    &function  (    )    
      &pushsdb 29							//'fem_m_autoMatchSearch::failedAutoMatchSearch()'
      &trace
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 25							// backButtonClicked()
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_autoMatchSearch'
    &pushsdbgm 11							//'prototype'
    &pushbyte 3
    &pushsdb 30							//'ASSetPropFlags'
    &callFunction
   label4:
    &pop
  &end // of initMovieClip 8

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &frame 59
    &gotoLabel '_start'
    &play
  &end // of frame 59
&end
