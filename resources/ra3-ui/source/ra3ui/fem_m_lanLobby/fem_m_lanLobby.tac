movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\fem_m_lanLobby\\fem_m_lanLobby.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1280x720 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fem_m_lanLobby'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 3
  &end // of importAssets
  
  &importAssets &from '../mouse/Components/chatComponent.swf'
    'chatComponentSymbol' &as 13
  &end // of importAssets

  &placeMovieClip 13 &as 'chatComponent'
  
    &onClipEvent &construct
      &pushs 'm_numLines'
      &pushzerosv
      &pushs 'm_width'
      &pushshort 465
      &setVariable
      &pushs 'm_chatMode'
      &pushbyte 2
      &setVariable
      &pushs 'm_preservePlacement'
      &pushfalse
      &setVariable
      &pushs 'm_height'
      &pushshort 213
      &setVariable
    &end
  &end // of placeMovieClip 13
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 14
  &end // of importAssets

  &placeMovieClip 14 &as 'playerList'
  
    &onClipEvent &construct
      &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
      &pushsdb 0							//'m_focusDirs'
      &pushsdb 1							//'Up/Down'
      &setVariable
      &pushsdb 2							//'m_numEntries'
      &pushbyte 20
      &setVariable
      &pushsdb 3							//'m_vPadding'
      &pushbyte 2
      &setVariable
      &pushsdb 4							//'m_bEnabled'
      &pushtrue
      &setVariable
      &pushsdb 5							//'m_refFM'
      &pushsdb 6							//'_root.gFM'
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
      &pushsdb 11							//'m_BgVisible'
      &pushfalse
      &setVariable
      &pushsdb 12							//'m_renderWidth'
      &pushshort 458
      &setVariable
      &pushsdb 13							//'m_renderHeight'
      &pushshort 499
      &setVariable
      &pushsdb 14							//'m_backgroundMargin'
      &pushbyte 2
      &setVariable
      &pushsdb 15							//'m_useHighlights'
      &pushtrue
      &setVariable
      &pushsdb 16							//'m_alternateBackgroundColors'
      &pushtrue
      &setVariable
      &pushsdb 17							//'m_backgroundColor1'
      &pushlong 2622726
      &setVariable
      &pushsdb 18							//'m_backgroundColor2'
      &pushzerosv
    &end
  &end // of placeMovieClip 14
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 15
  &end // of importAssets

  &placeMovieClip 15 &as 'playerListScrollBar'
  
    &onClipEvent &construct
      &pushs 'm_nLength'
      &pushbyte 100
      &setVariable
      &pushs 'm_nMax'
      &pushbyte 10
      &setVariable
      &pushs 'm_nMin'
      &pushzerosv
      &pushs 'm_nValue'
      &pushzerosv
    &end
  &end // of placeMovieClip 15

  &placeMovieClip 14 &as 'serverList'
  
    &onClipEvent &construct
      &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
      &pushsdb 0							//'m_focusDirs'
      &pushsdb 1							//'Up/Down'
      &setVariable
      &pushsdb 2							//'m_numEntries'
      &pushbyte 20
      &setVariable
      &pushsdb 3							//'m_vPadding'
      &pushbyte 2
      &setVariable
      &pushsdb 4							//'m_bEnabled'
      &pushtrue
      &setVariable
      &pushsdb 5							//'m_refFM'
      &pushsdb 6							//'_root.gFM'
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
      &pushsdb 11							//'m_BgVisible'
      &pushfalse
      &setVariable
      &pushsdb 12							//'m_renderWidth'
      &pushshort 458
      &setVariable
      &pushsdb 13							//'m_renderHeight'
      &pushshort 211
      &setVariable
      &pushsdb 14							//'m_backgroundMargin'
      &pushbyte 2
      &setVariable
      &pushsdb 15							//'m_useHighlights'
      &pushtrue
      &setVariable
      &pushsdb 16							//'m_alternateBackgroundColors'
      &pushtrue
      &setVariable
      &pushsdb 17							//'m_backgroundColor1'
      &pushlong 2622726
      &setVariable
      &pushsdb 18							//'m_backgroundColor2'
      &pushzerosv
    &end
  &end // of placeMovieClip 14

  &placeMovieClip 15 &as 'serverListScrollBar'
  
    &onClipEvent &construct
      &pushs 'm_nLength'
      &pushbyte 100
      &setVariable
      &pushs 'm_nMax'
      &pushbyte 10
      &setVariable
      &pushs 'm_nMin'
      &pushzerosv
      &pushs 'm_nValue'
      &pushzerosv
    &end
  &end // of placeMovieClip 15
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 16
  &end // of importAssets

  &placeMovieClip 16 &as 'joinButton'
  
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
      &pushssv '$JOINGAMEBUTTON'
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
  &end // of placeMovieClip 16

  &placeMovieClip 16 &as 'createButton'
  
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
      &pushssv '$AcceptCreate'
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
  &end // of placeMovieClip 16

  &placeMovieClip 16 &as 'backButton'
  
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
      &pushssv '$Back'
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
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 16
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 17
  &end // of importAssets

  &placeMovieClip 17 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$LANLOBBY'
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
  &end // of placeMovieClip 17

  &defineMovieClip 18 // total frames: 0
  &end // of defineMovieClip 18
  
  &exportAssets
    18 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 18
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
  &end // of initMovieClip 18

  &defineMovieClip 19 // total frames: 0
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as '__Packages.fem_m_lanLobby'
  &end // of exportAssets
  
  &initMovieClip 19
    &constants '_global', 'fem_m_lanLobby', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind1DynamicParams', 'm_serverListCallbackFunc', 'onServerListboxMouseClick', 'bind2DynamicParams', 'm_playerListCallbackFunc', 'onPlayerListboxMouseClick', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_LAN_MP', 'prototype', 'frameDelayedInit', 'gMH', 'addMessageHandler', 'initGUI', 'Array', 'joinButtonClicked', 'm_screen', 'joinButton', 'configureButton', 'createButtonClicked', 'createButton', 'backButtonClicked', 'backButton', 'initServerList', 'initPlayerList', 'updateButtonState', 'show', 'enable', 'setOnMouseUpFunction', 'noIntro', 'removeMessageHandler', 'serverList', 'removeListenerFunction', 'shutdown', 'playerList', 'clearData', 'MSGCODE', 'FE_LAN_REFRESH_GAMELIST', 'FE_LAN_REFRESH_PLAYERLIST', 'FE_LAN_HOST_GAME_CREATED', 'FE_LAN_CLIENT_GAME_JOINED', 'FE_MP_LAN_LOBBY_STATE_CHANGE', 'populateGameList', 'populatePlayerList', 'SCREEN', 'FEM_LAN_SETUP', 'startOuttro', 'Object', 'QueryGameEngine?LAN_GAMELIST', 'm_gameNames', ',', 'LAN_GAMELIST_NAMES', 'split', 'setColumnData', 'refreshDisplay', 'refreshScrollbar', 'QueryGameEngine?LAN_PLAYERLIST', 'm_playerIds', 'LAN_PLAYERLIST_NAMES', 'GAME_MODE_SHELL', 'disable', 'FSCommand:CallGameFunction', '%LanJoinGame?Game=', 'getSelectedEntryIndex', 'serverListScrollBar', 'setNumEntries', 'setVerticalPadding', 'addListenerFunction', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'push', 'init', 'attachScrollbarController', 'moveViewToFirst', 'playerListScrollBar', 'QueryGameEngine?LASTMOUSEPRESSTYPE&LASTMOUSEDOUBLECLICK', 'LASTMOUSEPRESSTYPE', 'MOUSEBUTTONPRESS_LEFT', 'LASTMOUSEDOUBLECLICK', 'QueryGameEngine?LAN_CAN_HOST_OR_JOIN', 'LAN_CAN_HOST_OR_JOIN', '1', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_lanLobby'
    &not
    &not
    &jnz label15    
    &pushglobalgv
    &pushsdb 1							//'fem_m_lanLobby'
    &function2  (r:4='screen') (r:1='this', r:2='super', r:3='_global')
      &pushregister 4
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
      &pushregister 1
      &pushsdb 7							//'m_serverListCallbackFunc'
      &pushregister 1
      &pushsdbgm 8							//'onServerListboxMouseClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 9							//'bind2DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 10							//'m_playerListCallbackFunc'
      &pushregister 1
      &pushsdbgm 11							//'onPlayerListboxMouseClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 9							//'bind2DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 12							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 13							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 14							//'gSM'
      &dcallmp 15							// setOnExitScreen()
      &getURL 'FSCommand:CallGameFunction'       '%LanInit'      
      &pushregister 3
      &pushsdbgm 16							//'GAME'
      &pushsdb 17							//'MODE'
      &pushregister 3
      &pushsdbgm 18							//'ENUM'
      &pushsdbgm 19							//'GAME_MODE_LAN_MP'
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_lanLobby'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 20							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 21							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 4							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 22							//'gMH'
      &dcallmp 23							// addMessageHandler()
      &pushzero
      &pushregister 1
      &dcallmp 24							// initGUI()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 25							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdbgm 26							//'joinButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 13							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 28							//'joinButton'
      &pushbyte 2
      &pushregister 1
      &dcallmp 29							// configureButton()
      &pushregister 1
      &pushsdbgm 30							//'createButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 13							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 31							//'createButton'
      &pushbyte 2
      &pushregister 1
      &dcallmp 29							// configureButton()
      &pushregister 1
      &pushsdbgm 32							//'backButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 13							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 33							//'backButton'
      &pushbyte 2
      &pushregister 1
      &dcallmp 29							// configureButton()
      &pushzero
      &pushregister 1
      &dcallmp 34							// initServerList()
      &pushzero
      &pushregister 1
      &dcallmp 35							// initPlayerList()
      &pushzero
      &pushregister 1
      &dcallmp 36							// updateButtonState()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 29							//'configureButton'
    &function2  (r:1='button', r:2='callback') ()
      &pushzero
      &pushregister 1
      &dcallmp 37							// show()
      &pushzero
      &pushregister 1
      &dcallmp 38							// enable()
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 39							// setOnMouseUpFunction()
      &pushzero
      &pushregister 1
      &dcallmp 40							// noIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 12							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 4							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 22							//'gMH'
      &dcallmp 41							// removeMessageHandler()
      &pushregister 1
      &pushsdb 4							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 7							//'m_serverListCallbackFunc'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &dcallmp 43							// removeListenerFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &dcallmp 44							// shutdown()
      &pushregister 1
      &pushsdb 7							//'m_serverListCallbackFunc'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 10							//'m_playerListCallbackFunc'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 45							//'playerList'
      &dcallmp 43							// removeListenerFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 45							//'playerList'
      &dcallmp 44							// shutdown()
      &pushregister 1
      &pushsdb 10							//'m_playerListCallbackFunc'
      &delete
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 46							// clearData()
      &pushregister 2
      &pushsdb 1							//'fem_m_lanLobby'
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
    &function2  (r:4='message') (r:1='this', r:2='_global')
      &pushtrue
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 47							//'MSGCODE'
      &pushsdbgm 48							//'FE_LAN_REFRESH_GAMELIST'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 47							//'MSGCODE'
      &pushsdbgm 49							//'FE_LAN_REFRESH_PLAYERLIST'
      &strictEquals
      &jnz label2      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 47							//'MSGCODE'
      &pushsdbgm 50							//'FE_LAN_HOST_GAME_CREATED'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 47							//'MSGCODE'
      &pushsdbgm 51							//'FE_LAN_CLIENT_GAME_JOINED'
      &strictEquals
      &jnz label4      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 47							//'MSGCODE'
      &pushsdbgm 52							//'FE_MP_LAN_LOBBY_STATE_CHANGE'
      &strictEquals
      &jnz label5      
      &jmp label6      
     label1:
      &pushzero
      &pushregister 1
      &dcallmp 53							// populateGameList()
      &jmp label7      
     label2:
      &pushzero
      &pushregister 1
      &dcallmp 54							// populatePlayerList()
      &jmp label7      
     label3:
      &pushregister 2
      &pushsdbgm 55							//'SCREEN'
      &pushsdbgm 56							//'FEM_LAN_SETUP'
      &pushone
      &pushregister 1
      &dcallmp 57							// startOuttro()
      &jmp label7      
     label4:
      &pushregister 2
      &pushsdbgm 55							//'SCREEN'
      &pushsdbgm 56							//'FEM_LAN_SETUP'
      &pushone
      &pushregister 1
      &dcallmp 57							// startOuttro()
      &jmp label7      
     label5:
      &pushzero
      &pushregister 1
      &dcallmp 36							// updateButtonState()
      &jmp label7      
     label6:
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label7      
     label7:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 53							//'populateGameList'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 25							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 58							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 59							//'QueryGameEngine?LAN_GAMELIST'
      &pushregister 2
      &loadVariables
      &pushregister 1
      &pushsdb 60							//'m_gameNames'
      &pushsdb 61							//','
      &pushone
      &pushregister 2
      &pushsdbgm 62							//'LAN_GAMELIST_NAMES'
      &pushsdb 63							//'split'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 60							//'m_gameNames'
      &pushzero
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &dcallmp 64							// setColumnData()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &dcallmp 65							// refreshDisplay()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &dcallmp 66							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 54							//'populatePlayerList'
    &function2  () ()
      &pushzero
      &pushsdb 25							//'Array'
      &new
      &setRegister r:2
      &pop
      &pushzero
      &pushsdb 58							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 67							//'QueryGameEngine?LAN_PLAYERLIST'
      &pushregister 1
      &loadVariables
      &pushsdb 68							//'m_playerIds'
      &pushsdb 61							//','
      &pushone
      &pushregister 1
      &pushsdbgm 69							//'LAN_PLAYERLIST_NAMES'
      &dcallmsv 63							// split()
      &pushsdbgv 68							//'m_playerIds'
      &pushzero
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 45							//'playerList'
      &dcallmp 64							// setColumnData()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 45							//'playerList'
      &dcallmp 65							// refreshDisplay()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 45							//'playerList'
      &dcallmp 66							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 32							//'backButtonClicked'
    &function2  () (r:1='super', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       '%LanShutdown'      
      &pushregister 2
      &pushsdbgm 16							//'GAME'
      &pushsdb 17							//'MODE'
      &pushregister 2
      &pushsdbgm 18							//'ENUM'
      &pushsdbgm 70							//'GAME_MODE_SHELL'
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 32							// backButtonClicked()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 30							//'createButtonClicked'
    &function  (    )    
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 28							//'joinButton'
      &dcallmp 71							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 31							//'createButton'
      &dcallmp 71							// disable()
      &getURL 'FSCommand:CallGameFunction'       '%LanCreateGame'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'joinButtonClicked'
    &function  (    )    
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 28							//'joinButton'
      &dcallmp 71							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 31							//'createButton'
      &dcallmp 71							// disable()
      &pushsdb 72							//'FSCommand:CallGameFunction'
      &pushsdb 73							//'%LanJoinGame?Game='
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &pushsdb 74							//'getSelectedEntryIndex'
      &callMethod
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'initServerList'
    &function2  () (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 75							//'serverListScrollBar'
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &dcallmp 37							// show()
      &pushzero
      &pushregister 3
      &dcallmp 38							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &setRegister r:2
      &pop
      &pushbyte 15
      &pushone
      &pushregister 2
      &dcallmp 76							// setNumEntries()
      &pushone
      &pushone
      &pushregister 2
      &dcallmp 77							// setVerticalPadding()
      &pushregister 1
      &pushsdbgm 7							//'m_serverListCallbackFunc'
      &pushone
      &pushregister 2
      &dcallmp 78							// addListenerFunction()
      &pushzero
      &pushsdb 25							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 79							//'symbol'
      &pushsdb 80							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 81							//'width'
      &pushshort 458
      &pushsdb 82							//'leftMargin'
      &pushzero
      &pushsdb 83							//'rightMargin'
      &pushzero
      &pushbyte 4
      &initObject
      &pushone
      &pushregister 4
      &dcallmp 84							// push()
      &pushbyte 25
      &pushregister 4
      &pushbyte 2
      &pushregister 2
      &dcallmp 85							// init()
      &pushregister 3
      &pushone
      &pushregister 2
      &dcallmp 86							// attachScrollbarController()
      &pushzero
      &pushregister 2
      &dcallmp 87							// moveViewToFirst()
      &pushzero
      &pushregister 2
      &dcallmp 65							// refreshDisplay()
      &pushzero
      &pushregister 2
      &dcallmp 66							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 35							//'initPlayerList'
    &function2  () (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 88							//'playerListScrollBar'
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &dcallmp 37							// show()
      &pushzero
      &pushregister 3
      &dcallmp 38							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 45							//'playerList'
      &setRegister r:2
      &pop
      &pushbyte 20
      &pushone
      &pushregister 2
      &dcallmp 76							// setNumEntries()
      &pushone
      &pushone
      &pushregister 2
      &dcallmp 77							// setVerticalPadding()
      &pushregister 1
      &pushsdbgm 10							//'m_playerListCallbackFunc'
      &pushone
      &pushregister 2
      &dcallmp 78							// addListenerFunction()
      &pushzero
      &pushsdb 25							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 79							//'symbol'
      &pushsdb 80							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 81							//'width'
      &pushshort 458
      &pushsdb 82							//'leftMargin'
      &pushzero
      &pushsdb 83							//'rightMargin'
      &pushzero
      &pushbyte 4
      &initObject
      &pushone
      &pushregister 4
      &dcallmp 84							// push()
      &pushbyte 25
      &pushregister 4
      &pushbyte 2
      &pushregister 2
      &dcallmp 85							// init()
      &pushregister 3
      &pushone
      &pushregister 2
      &dcallmp 86							// attachScrollbarController()
      &pushzero
      &pushregister 2
      &dcallmp 87							// moveViewToFirst()
      &pushzero
      &pushregister 2
      &dcallmp 65							// refreshDisplay()
      &pushzero
      &pushregister 2
      &dcallmp 66							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 8							//'onServerListboxMouseClick'
    &function2  ('index', 'prevIndex') (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 58							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 89							//'QueryGameEngine?LASTMOUSEPRESSTYPE&LASTMOUSEDOUBLECLICK'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 90							//'LASTMOUSEPRESSTYPE'
      &toNumber
      &pushregister 2
      &pushsdbgm 18							//'ENUM'
      &pushsdbgm 91							//'MOUSEBUTTONPRESS_LEFT'
      &equals
      &dup
      &not
      &jnz label8      
      &pop
      &pushregister 3
      &pushsdbgm 92							//'LASTMOUSEDOUBLECLICK'
      &toNumber
      &pushzero
      &equals
      &not
     label8:
      &not
      &jnz label9      
      &pushzero
      &pushregister 1
      &dcallmp 26							// joinButtonClicked()
      &jmp label10      
     label9:
      &pushzero
      &pushregister 1
      &dcallmp 36							// updateButtonState()
     label10:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 11							//'onPlayerListboxMouseClick'
    &function  ('index', 'prevIndex'    )    
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 36							//'updateButtonState'
    &function2  () ()
      &pushzero
      &pushsdb 58							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 93							//'QueryGameEngine?LAN_CAN_HOST_OR_JOIN'
      &pushregister 1
      &loadVariables
      &pushregister 1
      &pushsdbgm 94							//'LAN_CAN_HOST_OR_JOIN'
      &pushsdb 95							//'1'
      &equals
      &not
      &jnz label13      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 42							//'serverList'
      &pushsdb 74							//'getSelectedEntryIndex'
      &callMethod
      &pushzero
      &lessThan
      &not
      &not
      &jnz label11      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 28							//'joinButton'
      &dcallmp 38							// enable()
      &jmp label12      
     label11:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 28							//'joinButton'
      &dcallmp 71							// disable()
     label12:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 31							//'createButton'
      &dcallmp 38							// enable()
      &jmp label14      
     label13:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 28							//'joinButton'
      &dcallmp 71							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 27							//'m_screen'
      &pushsdbgm 31							//'createButton'
      &dcallmp 71							// disable()
     label14:
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_lanLobby'
    &pushsdbgm 20							//'prototype'
    &pushbyte 3
    &pushsdb 96							//'ASSetPropFlags'
    &callFunction
   label15:
    &pop
  &end // of initMovieClip 19
&end
