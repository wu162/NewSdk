movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEM_M_~2\\FEM_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fem_m_autoMatchSetup'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '../cafe/mouseComponents/std_MouseCheckBox.swf'
    'std_MouseCheckBoxSymbol' &as 1
  &end // of importAssets

  &placeMovieClip 1 &as 'broadcastCheckBox'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$BroadcastGame', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'checkBoxContentClipSymbol'  
      &pushsdb 0							//'m_width'
      &pushshort 200
      &setVariable
      &pushsdb 1							//'m_textAlign'
      &pushfalse
      &setVariable
      &pushsdb 2							//'m_labelPosition'
      &pushsdb 3							//'right align'
      &setVariable
      &pushsdb 4							//'m_label_x'
      &pushzerosv
      &pushsdb 5							//'m_label_y'
      &pushzerosv
      &pushsdb 6							//'m_focusDirs'
      &pushsdb 7							//'Up/Down'
      &setVariable
      &pushsdb 8							//'m_iconType'
      &pushsdb 9							//'default'
      &setVariable
      &pushsdb 10							//'m_extLabel'
      &pushsdb 11							//'$BroadcastGame'
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//'$BroadcastGame'
      &setVariable
      &pushsdb 14							//'m_bEnabled'
      &pushtrue
      &setVariable
      &pushsdb 15							//'m_refFM'
      &pushsdb 16							//'_root.gFM'
      &setVariable
      &pushsdb 17							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 18							//'m_tabIndex'
      &pushbyte 2
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'checkBoxContentClipSymbol'
      &setVariable
    &end
  &end // of placeMovieClip 1
  
  &importAssets &from '../cafe/mouseComponents/std_mouseDropDownMenu.swf'
    'std_mouseDropDownMenuSymbol' &as 2
  &end // of importAssets

  &placeMovieClip 2 &as 'factionsMenu'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 3
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_visibleEntryCount'
      &pushbyte 4
      &setVariable
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'TextDropdownMenuContentSymbol'
      &pushs 'm_entrySymbol'
      &pushssv 'std_mouseListboxEntryTextElementSymbol'
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'colorsMenu'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
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
      &pushs 'm_visibleEntryCount'
      &pushbyte 9
      &setVariable
      &pushs 'm_width'
      &pushbyte 50
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'ColorSwatchDropdownContentSymbol'
      &pushs 'm_entrySymbol'
      &pushssv 'std_mouseListboxEntryColorSwatchElementSymbol'
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'gameTypesMenu'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
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
      &pushs 'm_visibleEntryCount'
      &pushbyte 2
      &setVariable
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'TextDropdownMenuContentSymbol'
      &pushs 'm_entrySymbol'
      &pushssv 'std_mouseListboxEntryTextElementSymbol'
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
      &pushzerosv
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$Back'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 3

  &placeMovieClip 3 &as 'searchButton'
  
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
      &pushssv '$StartSearch'
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
    7 &as '__Packages.autoMatchSetup'
  &end // of exportAssets
  
  &initMovieClip 7
    &constants '_global', 'autoMatchSetup', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'prototype', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'autoMatchSetup'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'autoMatchSetup'
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
    &pushsdbgm 1							//'autoMatchSetup'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 5							//'prototype'
    &setRegister r:2
    &pop
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'autoMatchSetup'
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
    8 &as '__Packages.fem_m_autoMatchSetup'
  &end // of exportAssets
  
  &initMovieClip 8
    &constants '_global', 'fem_m_autoMatchSetup', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'initGUI', 'autoMatchSetup', 'prototype', 'initButtons', 'initCheckBox', 'initMenus', 'searchButton', 'backButton', 'registerIntroOuttroComponents', 'onScreenExit', 'gSM', 'setOnExitScreen', 'startIntro', 'clearData', 'show', 'enable', 'backButtonClicked', 'bind0', 'setOnMouseUpFunction', 'searchButtonClicked', 'populateAMTypesMenu', 'populateAMColorsMenu', 'populateAMFactionsMenu', 'setupSelectionsChanged', 'gameTypesMenu', 'setOnChange', 'colorsMenu', 'factionsMenu', 'Object', 'QueryGameEngine?AUTOMATCHTYPES', 'Array', ',', 'AUTOMATCHTYPES_VALUES', 'split', 'AUTOMATCHTYPES_STRINGS', 'shift', 'setData', 'setSelectedIndex', 'QueryGameEngine?AUTOMATCHCOLORS', 'AUTOMATCHCOLORS', 'length', 'i', 'push', 'QueryGameEngine?AUTOMATCHFACTIONS', 'AUTOMATCHFACTIONS_VALUES', 'AUTOMATCHFACTIONS_STRINGS', 'broadcastCheckBox', 'QueryGameEngine?AUTOMATCHBROADCAST', 'AutoMatchBroadcast', 'check', 'setOnMouseDownFunction', 'SCREEN', 'FEM_MP_AUTOMATCH_SEARCH', 'startOuttro', 'sendParamsToBackEnd', 'fem_m_autoMatchSetup::sendParamsToBackEnd() : Cast=', ', Type=', ', Color=', ', Faction=', 'FSCommand:CallGameFunction', '%PROCESSAUTOMATCHSETUPCHOICES?CAST=', '|TYPE=', '|COLOR=', '|FACTION=', 'isChecked', '0', '1', 'getCurrentIndex', 'getValueAtIndex', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_autoMatchSetup'
    &not
    &not
    &jnz label6    
    &pushglobalgv
    &pushsdb 1							//'fem_m_autoMatchSetup'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushregister 3
      &pushone
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
      &pushzero
      &pushregister 1
      &dcallmp 5							// initGUI()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_autoMatchSetup'
    &pushsdbgv 6							//'autoMatchSetup'
    &extends
    &pushregister 1
    &pushsdbgm 7							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 5							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       '%PREPAREAUTOMATCHSETUPDEFAULTS'      
      &pushzero
      &pushregister 1
      &dcallmp 8							// initButtons()
      &pushzero
      &pushregister 1
      &dcallmp 9							// initCheckBox()
      &pushzero
      &pushregister 1
      &dcallmp 10							// initMenus()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 11							//'searchButton'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 12							//'backButton'
      &pushbyte 2
      &initArray
      &pushone
      &pushregister 1
      &dcallmp 13							// registerIntroOuttroComponents()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 14							//'onScreenExit'
      &pushone
      &pushregister 2
      &pushsdbgm 15							//'gSM'
      &dcallmp 16							// setOnExitScreen()
      &pushzero
      &pushregister 1
      &dcallmp 17							// startIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 14							//'onScreenExit'
    &function2  () (r:1='_global')
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 18							// clearData()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 1							//'fem_m_autoMatchSetup'
      &delete
      &pop
      &pushregister 1
      &pushsdb 6							//'autoMatchSetup'
      &delete
      &pop
      &pushregister 1
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 8							//'initButtons'
    &function2  () (r:1='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 12							//'backButton'
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &dcallmp 19							// show()
      &pushzero
      &pushregister 3
      &dcallmp 20							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 21							//'backButtonClicked'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushbyte 2
      &pushregister 1
      &pushsdb 22							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 23							// setOnMouseUpFunction()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 11							//'searchButton'
      &setRegister r:2
      &pop
      &pushzero
      &pushregister 2
      &dcallmp 19							// show()
      &pushzero
      &pushregister 2
      &dcallmp 20							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 24							//'searchButtonClicked'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushbyte 2
      &pushregister 1
      &pushsdb 22							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &dcallmp 23							// setOnMouseUpFunction()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 10							//'initMenus'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushregister 1
      &dcallmp 25							// populateAMTypesMenu()
      &pushzero
      &pushregister 1
      &dcallmp 26							// populateAMColorsMenu()
      &pushzero
      &pushregister 1
      &dcallmp 27							// populateAMFactionsMenu()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 28							//'setupSelectionsChanged'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushbyte 2
      &pushregister 2
      &pushsdb 22							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 29							//'gameTypesMenu'
      &dcallmp 30							// setOnChange()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 28							//'setupSelectionsChanged'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushbyte 2
      &pushregister 2
      &pushsdb 22							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 31							//'colorsMenu'
      &dcallmp 30							// setOnChange()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 28							//'setupSelectionsChanged'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushbyte 2
      &pushregister 2
      &pushsdb 22							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 32							//'factionsMenu'
      &dcallmp 30							// setOnChange()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 25							//'populateAMTypesMenu'
    &function2  () ()
      &pushzero
      &pushsdb 33							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 34							//'QueryGameEngine?AUTOMATCHTYPES'
      &pushregister 2
      &loadVariables
      &pushzero
      &pushsdb 35							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 35							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 36							//','
      &pushone
      &pushregister 2
      &pushsdbgm 37							//'AUTOMATCHTYPES_VALUES'
      &pushsdb 38							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdb 36							//','
      &pushone
      &pushregister 2
      &pushsdbgm 39							//'AUTOMATCHTYPES_STRINGS'
      &pushsdb 38							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 3
      &pushsdb 40							//'shift'
      &callMethod
      &toNumber
      &setRegister r:5
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 29							//'gameTypesMenu'
      &setRegister r:1
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 20							// enable()
      &pushregister 3
      &pushregister 4
      &pushbyte 2
      &pushregister 1
      &dcallmp 41							// setData()
      &pushregister 5
      &pushone
      &pushregister 1
      &dcallmp 42							// setSelectedIndex()
      &pushzero
      &pushregister 1
      &dcallmp 19							// show()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'populateAMColorsMenu'
    &function2  () ()
      &pushzero
      &pushsdb 33							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 43							//'QueryGameEngine?AUTOMATCHCOLORS'
      &pushregister 5
      &loadVariables
      &pushsdb 36							//','
      &pushone
      &pushregister 5
      &pushsdbgm 44							//'AUTOMATCHCOLORS'
      &pushsdb 38							//'split'
      &callMethod
      &setRegister r:1
      &pop
      &pushzero
      &pushregister 1
      &pushsdb 40							//'shift'
      &callMethod
      &toNumber
      &setRegister r:6
      &pop
      &pushregister 1
      &pushsdbgm 45							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 35							//'Array'
      &new
      &setRegister r:2
      &pop
      &pushsdb 46							//'i'
      &pushzerosv
     label1:
      &pushsdbgv 46							//'i'
      &pushregister 3
      &lessThan
      &not
      &jnz label2      
      &pushregister 1
      &pushsdbgv 46							//'i'
      &getMember
      &toNumber
      &pushone
      &pushregister 2
      &dcallmp 47							// push()
      &pushsdb 46							//'i'
      &pushsdbgv 46							//'i'
      &increment
      &setVariable
      &jmp label1      
     label2:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 31							//'colorsMenu'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 20							// enable()
      &pushregister 2
      &pushregister 1
      &pushbyte 2
      &pushregister 4
      &dcallmp 41							// setData()
      &pushregister 6
      &pushone
      &pushregister 4
      &dcallmp 42							// setSelectedIndex()
      &pushzero
      &pushregister 4
      &dcallmp 19							// show()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 27							//'populateAMFactionsMenu'
    &function2  () ()
      &pushzero
      &pushsdb 33							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 48							//'QueryGameEngine?AUTOMATCHFACTIONS'
      &pushregister 2
      &loadVariables
      &pushzero
      &pushsdb 35							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 35							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 36							//','
      &pushone
      &pushregister 2
      &pushsdbgm 49							//'AUTOMATCHFACTIONS_VALUES'
      &pushsdb 38							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdb 36							//','
      &pushone
      &pushregister 2
      &pushsdbgm 50							//'AUTOMATCHFACTIONS_STRINGS'
      &pushsdb 38							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 3
      &pushsdb 40							//'shift'
      &callMethod
      &toNumber
      &setRegister r:5
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 32							//'factionsMenu'
      &setRegister r:1
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 20							// enable()
      &pushregister 3
      &pushregister 4
      &pushbyte 2
      &pushregister 1
      &dcallmp 41							// setData()
      &pushregister 5
      &pushone
      &pushregister 1
      &dcallmp 42							// setSelectedIndex()
      &pushzero
      &pushregister 1
      &dcallmp 19							// show()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 9							//'initCheckBox'
    &function2  () (r:1='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 51							//'broadcastCheckBox'
      &setRegister r:2
      &pop
      &pushzero
      &pushregister 2
      &dcallmp 19							// show()
      &pushzero
      &pushregister 2
      &dcallmp 20							// enable()
      &pushzero
      &pushsdb 33							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 52							//'QueryGameEngine?AUTOMATCHBROADCAST'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 53							//'AutoMatchBroadcast'
      &setRegister r:4
      &pop
      &pushregister 4
      &pushzero
      &greaterThan
      &not
      &jnz label3      
      &pushzero
      &pushregister 2
      &dcallmp 54							// check()
     label3:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 28							//'setupSelectionsChanged'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushbyte 2
      &pushregister 1
      &pushsdb 22							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &dcallmp 55							// setOnMouseDownFunction()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'searchButtonClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushregister 1
      &dcallmp 28							// setupSelectionsChanged()
      &pushregister 2
      &pushsdbgm 56							//'SCREEN'
      &pushsdbgm 57							//'FEM_MP_AUTOMATCH_SEARCH'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 58							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 59							//'sendParamsToBackEnd'
    &function2  (r:1='bcast', r:2='gtype', r:4='pcolor', r:3='pfaction') ()
      &pushsdb 60							//'fem_m_autoMatchSetup::sendParamsToBackEnd() : Cast='
      &pushregister 1
      &add
      &pushsdb 61							//', Type='
      &add
      &pushregister 2
      &add
      &pushsdb 62							//', Color='
      &add
      &pushregister 4
      &add
      &pushsdb 63							//', Faction='
      &add
      &pushregister 3
      &add
      &trace
      &pushsdb 64							//'FSCommand:CallGameFunction'
      &pushsdb 65							//'%PROCESSAUTOMATCHSETUPCHOICES?CAST='
      &pushregister 1
      &add
      &pushsdb 66							//'|TYPE='
      &add
      &pushregister 2
      &add
      &pushsdb 67							//'|COLOR='
      &add
      &pushregister 4
      &add
      &pushsdb 68							//'|FACTION='
      &add
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'setupSelectionsChanged'
    &function2  () (r:1='this')
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 51							//'broadcastCheckBox'
      &pushsdb 69							//'isChecked'
      &callMethod
      &jnz label4      
      &pushsdb 70							//'0'
      &jmp label5      
     label4:
      &pushsdb 71							//'1'
     label5:
      &setRegister r:8
      &pop
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 29							//'gameTypesMenu'
      &pushsdb 72							//'getCurrentIndex'
      &callMethod
      &setRegister r:6
      &pop
      &pushregister 6
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 29							//'gameTypesMenu'
      &pushsdb 73							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:7
      &pop
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 31							//'colorsMenu'
      &pushsdb 72							//'getCurrentIndex'
      &callMethod
      &setRegister r:3
      &pop
      &pushregister 3
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 31							//'colorsMenu'
      &pushsdb 73							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:2
      &pop
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 32							//'factionsMenu'
      &pushsdb 72							//'getCurrentIndex'
      &callMethod
      &setRegister r:4
      &pop
      &pushregister 4
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 32							//'factionsMenu'
      &pushsdb 73							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:5
      &pop
      &pushregister 5
      &pushregister 2
      &pushregister 7
      &pushregister 8
      &pushbyte 4
      &pushregister 1
      &dcallmp 59							// sendParamsToBackEnd()
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_autoMatchSetup'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 74							//'ASSetPropFlags'
    &callFunction
   label6:
    &pop
  &end // of initMovieClip 8
&end
