movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FE3F22~1\\FEG_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_profileManager'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 3
  &end // of importAssets

  &placeMovieClip 3 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$PROFILE_MANAGER_SCREEN_TITLE'
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
  &end // of placeMovieClip 3
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 4
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 9
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 10
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 16
  &end // of importAssets

  &defineMovieClip 17 // total frames: 1

    &placeMovieClip 9 &as 'txtProfileList'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 10
        &setVariable
        &pushsdb 3							//'m_vPadding'
        &pushbyte 5
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
        &pushbyte -1
        &setVariable
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
        &pushtrue
        &setVariable
        &pushsdb 12							//'m_renderWidth'
        &pushshort 525
        &setVariable
        &pushsdb 13							//'m_renderHeight'
        &pushshort 162
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
        &pushzerosv
        &pushsdb 18							//'m_backgroundColor2'
        &pushlong 3737609
        &setVariable
      &end
    &end // of placeMovieClip 9

    &placeMovieClip 10 &as 'sldrProfiles'
    
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
    &end // of placeMovieClip 10

    &placeMovieClip 16 &as 'btnNewProfile'
    
      &onClipEvent &construct
        &pushs 'm_width'
        &pushshort 150
        &setVariable
        &pushs 'm_textAlign'
        &pushtrue
        &setVariable
        &pushs 'm_labelPosition'
        &pushssv 'top center'
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
        &pushssv '$new'
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_global.gFM'
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
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 16

    &placeMovieClip 16 &as 'btnDeleteProfile'
    
      &onClipEvent &construct
        &pushs 'm_width'
        &pushshort 150
        &setVariable
        &pushs 'm_textAlign'
        &pushtrue
        &setVariable
        &pushs 'm_labelPosition'
        &pushssv 'top center'
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
        &pushssv '$delete'
        &pushs 'm_bEnabled'
        &pushfalse
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_global.gFM'
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
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 16

    &placeMovieClip 16 &as 'btnAcceptProfile'
    
      &onClipEvent &construct
        &pushs 'm_width'
        &pushshort 150
        &setVariable
        &pushs 'm_textAlign'
        &pushtrue
        &setVariable
        &pushs 'm_labelPosition'
        &pushssv 'top center'
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
        &pushssv '$accept'
        &pushs 'm_bEnabled'
        &pushfalse
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_global.gFM'
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
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 16
  &end // of defineMovieClip 17
  
  &importAssets &from './screens/fe_m_assets.swf'
    'newProfileMC' &as 18
  &end // of importAssets

  &placeMovieClip 16 &as 'btnMainMenu'
  
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
      &pushssv '$BACK'
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
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 16

  &defineMovieClip 19 // total frames: 0
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 19
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
  &end // of initMovieClip 19

  &defineMovieClip 20 // total frames: 0
  &end // of defineMovieClip 20
  
  &exportAssets
    20 &as '__Packages.IListboxEventListener'
  &end // of exportAssets
  
  &initMovieClip 20
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
  &end // of initMovieClip 20

  &defineMovieClip 21 // total frames: 0
  &end // of defineMovieClip 21
  
  &exportAssets
    21 &as '__Packages.feg_m_profileManager'
  &end // of exportAssets
  
  &initMovieClip 21
    &constants '_global', 'feg_m_profileManager', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'newProfileMC', '_visible', 'prototype', 'IListboxEventListener', 'frameDelayedInit', 'feg_m_profileManager::frameDelayedInit()', 'm_localMessageHandler', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'initGUI', 'startIntro', 'OnListboxIndexMouseClick', 'OnListboxIndexElementMouseClick', 'OnProfileClicked', 'Array', 'onNewProfile', 'profileMC', 'btnNewProfile', 'configureButton', 'push', 'onDeleteProfile', 'btnDeleteProfile', 'onSetProfile', 'btnAcceptProfile', 'onNewProfileCancel', 'btnNewCancel', 'onNewProfileConfirm', 'btnNewConfirm', 'onMainMenuClicked', 'btnMainMenu', 'newProfileName', 'setOnEnterFunction', 'txtProfileList', 'setNumEntries', 'setVerticalPadding', 'addListener', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'useHitRegion', 'init', 'sldrProfiles', 'attachScrollbarController', 'disable', 'updateProfileList', 'updateCurrentProfile', 'highlightCurrentProfile', 'moveViewToFirst', 'refreshDisplay', 'refreshScrollbar', 'registerIntroOuttroComponents', 'show', 'enable', 'setOnMouseUpFunction', 'MSGCODE', 'FE_PROFILE_CREATE_SUCCESS', 'FE_PROFILE_CREATE_FAIL', 'FE_PROFILE_REFRESH_LIST', 'Object', 'QueryGameEngine?FE_PROFILE_LIST', 'm_profiles', ',', 'FE_PROFILE_LIST', 'split', 'm_profileTokens', 'length', 'splice', '&Outline', 'setColumnData', 'QueryGameEngine?FE_CURRENT_PROFILE', 'txtProfileName', 'text', 'FE_CURRENT_PROFILE', 'QueryGameEngine?FE_CURRENT_PROFILE_INDEX', 'FE_CURRENT_PROFILE_INDEX', 'setSelectedIndex', 'feg_m_profileManager::onMainMenuClicked()', 'backButtonClicked', 'removeMessageHandler', 'removeListener', 'shutdown', 'clearData', 'takeFocus', 'getSelectedEntryIndex', 'slice', 'FSCommand:CallGameFunction', '%DeleteProfile?ProfileName=', '%SetProfileByToken?ProfileToken=', 'goBackScreen', 'm_newProfileName', '', 'setContents', 'getContents', '%AddProfile?ProfileName=', 'QueryGameEngine?FE_CURRENT_PROFILE_INDEX&FE_PROFILE_COUNT', 'FE_PROFILE_COUNT', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_profileManager'
    &not
    &not
    &jnz label11    
    &pushglobalgv
    &pushsdb 1							//'feg_m_profileManager'
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
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_profileManager'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 7							//'prototype'
    &setRegister r:2
    &pop
    &pushglobalgv
    &pushsdbgm 8							//'IListboxEventListener'
    &pushone
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_profileManager'
    &implements
    &pushregister 2
    &pushsdb 9							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 10							//'feg_m_profileManager::frameDelayedInit()'
      &trace
      &pushregister 1
      &pushsdb 11							//'m_localMessageHandler'
      &pushregister 1
      &pushsdbgm 12							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 13							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 11							//'m_localMessageHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 14							//'gMH'
      &dcallmp 15							// addMessageHandler()
      &pushregister 1
      &pushsdbgm 16							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 18							//'gSM'
      &dcallmp 19							// setOnExitScreen()
      &pushzero
      &pushregister 1
      &dcallmp 20							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 21							// startIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 22							//'OnListboxIndexMouseClick'
    &function  ('index', 'previouslySelectedIndex'    )    
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 23							//'OnListboxIndexElementMouseClick'
    &function2  (r:2='index', 'element', 'previouslySelectedIndex') (r:1='this')
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 24							// OnProfileClicked()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 20							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 25							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdbgm 26							//'onNewProfile'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 28							//'btnNewProfile'
      &pushbyte 2
      &pushregister 1
      &pushsdb 29							//'configureButton'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 30							// push()
      &pushregister 1
      &pushsdbgm 31							//'onDeleteProfile'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 32							//'btnDeleteProfile'
      &pushbyte 2
      &pushregister 1
      &pushsdb 29							//'configureButton'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 30							// push()
      &pushregister 1
      &pushsdbgm 33							//'onSetProfile'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 34							//'btnAcceptProfile'
      &pushbyte 2
      &pushregister 1
      &pushsdb 29							//'configureButton'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 30							// push()
      &pushregister 1
      &pushsdbgm 35							//'onNewProfileCancel'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdbgm 36							//'btnNewCancel'
      &pushbyte 2
      &pushregister 1
      &pushsdb 29							//'configureButton'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 30							// push()
      &pushregister 1
      &pushsdbgm 37							//'onNewProfileConfirm'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdbgm 38							//'btnNewConfirm'
      &pushbyte 2
      &pushregister 1
      &pushsdb 29							//'configureButton'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 30							// push()
      &pushregister 1
      &pushsdbgm 39							//'onMainMenuClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 40							//'btnMainMenu'
      &pushbyte 2
      &pushregister 1
      &pushsdb 29							//'configureButton'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 30							// push()
      &pushregister 1
      &pushsdbgm 37							//'onNewProfileConfirm'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdbgm 41							//'newProfileName'
      &dcallmp 42							// setOnEnterFunction()
      &pushbyte 6
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 44							// setNumEntries()
      &pushzero
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 45							// setVerticalPadding()
      &pushregister 1
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 46							// addListener()
      &pushzero
      &pushsdb 25							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 47							//'symbol'
      &pushsdb 48							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 49							//'width'
      &pushshort 523
      &pushsdb 50							//'leftMargin'
      &pushbyte 2
      &pushsdb 51							//'rightMargin'
      &pushzero
      &pushsdb 52							//'useHitRegion'
      &pushfalse
      &pushbyte 5
      &initObject
      &pushone
      &pushregister 4
      &dcallmp 30							// push()
      &pushbyte 27
      &pushregister 4
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 53							// init()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 54							//'sldrProfiles'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 55							// attachScrollbarController()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 34							//'btnAcceptProfile'
      &dcallmp 56							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 32							//'btnDeleteProfile'
      &dcallmp 56							// disable()
      &pushzero
      &pushregister 1
      &dcallmp 57							// updateProfileList()
      &pushzero
      &pushregister 1
      &dcallmp 58							// updateCurrentProfile()
      &pushzero
      &pushregister 1
      &dcallmp 59							// highlightCurrentProfile()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 60							// moveViewToFirst()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 61							// refreshDisplay()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 62							// refreshScrollbar()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdb 6							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushregister 3
      &pushone
      &pushregister 1
      &dcallmp 63							// registerIntroOuttroComponents()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 29							//'configureButton'
    &function2  (r:1='button', r:2='callback') ()
      &pushzero
      &pushregister 1
      &dcallmp 64							// show()
      &pushzero
      &pushregister 1
      &dcallmp 65							// enable()
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 66							// setOnMouseUpFunction()
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 12							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushfalse
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 67							//'MSGCODE'
      &pushsdbgm 68							//'FE_PROFILE_CREATE_SUCCESS'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 67							//'MSGCODE'
      &pushsdbgm 69							//'FE_PROFILE_CREATE_FAIL'
      &strictEquals
      &jnz label2      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 67							//'MSGCODE'
      &pushsdbgm 70							//'FE_PROFILE_REFRESH_LIST'
      &strictEquals
      &jnz label3      
      &jmp label4      
     label1:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdb 6							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 57							// updateProfileList()
      &pushzero
      &pushregister 1
      &dcallmp 58							// updateCurrentProfile()
      &pushzero
      &pushregister 1
      &dcallmp 59							// highlightCurrentProfile()
      &pushtrue
      &setRegister r:3
      &pop
      &jmp label5      
     label2:
      &pushtrue
      &setRegister r:3
      &pop
      &jmp label5      
     label3:
      &pushzero
      &pushregister 1
      &dcallmp 57							// updateProfileList()
      &pushzero
      &pushregister 1
      &dcallmp 59							// highlightCurrentProfile()
      &pushtrue
      &setRegister r:3
      &pop
      &jmp label5      
     label4:
      &jmp label5      
     label5:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 57							//'updateProfileList'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 71							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 72							//'QueryGameEngine?FE_PROFILE_LIST'
      &pushregister 3
      &loadVariables
      &pushregister 1
      &pushsdb 73							//'m_profiles'
      &pushsdb 74							//','
      &pushone
      &pushregister 3
      &pushsdbgm 75							//'FE_PROFILE_LIST'
      &pushsdb 76							//'split'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 77							//'m_profileTokens'
      &pushsdbgm 78							//'length'
      &pushzero
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 77							//'m_profileTokens'
      &dcallmp 79							// splice()
      &pushzero
      &setRegister r:2
      &pop
     label6:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 73							//'m_profiles'
      &pushsdbgm 78							//'length'
      &lessThan
      &not
      &jnz label7      
      &pushregister 1
      &pushsdbgm 73							//'m_profiles'
      &pushregister 2
      &getMember
      &pushsdb 80							//'&Outline'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 77							//'m_profileTokens'
      &dcallmp 30							// push()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label6      
     label7:
      &pushregister 1
      &pushsdbgm 77							//'m_profileTokens'
      &pushzero
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 81							// setColumnData()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 61							// refreshDisplay()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 62							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 58							//'updateCurrentProfile'
    &function2  () ()
      &pushzero
      &pushsdb 71							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 82							//'QueryGameEngine?FE_CURRENT_PROFILE'
      &pushregister 1
      &loadVariables
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 83							//'txtProfileName'
      &pushsdb 84							//'text'
      &pushregister 1
      &pushsdbgm 85							//'FE_CURRENT_PROFILE'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 59							//'highlightCurrentProfile'
    &function2  () ()
      &pushzero
      &pushsdb 71							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 86							//'QueryGameEngine?FE_CURRENT_PROFILE_INDEX'
      &pushregister 1
      &loadVariables
      &pushregister 1
      &pushsdbgm 87							//'FE_CURRENT_PROFILE_INDEX'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 88							// setSelectedIndex()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 34							//'btnAcceptProfile'
      &dcallmp 56							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 32							//'btnDeleteProfile'
      &dcallmp 56							// disable()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 39							//'onMainMenuClicked'
    &function2  () (r:1='this')
      &pushsdb 89							//'feg_m_profileManager::onMainMenuClicked()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 90							// backButtonClicked()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 16							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 11							//'m_localMessageHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 14							//'gMH'
      &dcallmp 91							// removeMessageHandler()
      &pushregister 1
      &pushsdb 11							//'m_localMessageHandler'
      &delete
      &pop
      &pushregister 1
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 92							// removeListener()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 43							//'txtProfileList'
      &dcallmp 93							// shutdown()
      &pushzero
      &pushregister 1
      &dcallmp 94							// clearData()
      &pushregister 2
      &pushsdb 1							//'feg_m_profileManager'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'onNewProfile'
    &function  (    )    
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdb 6							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdbgm 41							//'newProfileName'
      &dcallmp 95							// takeFocus()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 31							//'onDeleteProfile'
    &function2  () (r:1='this')
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &pushsdb 96							//'getSelectedEntryIndex'
      &callMethod
      &setRegister r:2
      &pop
      &pushone
      &pushone
      &pushregister 1
      &pushsdbgm 73							//'m_profiles'
      &pushregister 2
      &getMember
      &pushsdb 97							//'slice'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdb 98							//'FSCommand:CallGameFunction'
      &pushsdb 99							//'%DeleteProfile?ProfileName='
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 33							//'onSetProfile'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 43							//'txtProfileList'
      &pushsdb 96							//'getSelectedEntryIndex'
      &callMethod
      &setRegister r:3
      &pop
      &pushone
      &pushone
      &pushregister 1
      &pushsdbgm 73							//'m_profiles'
      &pushregister 3
      &getMember
      &pushsdb 97							//'slice'
      &callMethod
      &setRegister r:4
      &pop
      &pushsdb 98							//'FSCommand:CallGameFunction'
      &pushsdb 100							//'%SetProfileByToken?ProfileToken='
      &pushregister 4
      &add
      &getURL2
      &pushzero
      &pushregister 1
      &dcallmp 57							// updateProfileList()
      &pushzero
      &pushregister 1
      &dcallmp 58							// updateCurrentProfile()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 18							//'gSM'
      &dcallmp 101							// goBackScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 35							//'onNewProfileCancel'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 102							//'m_newProfileName'
      &pushsdb 103							//''
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 103							//''
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdbgm 41							//'newProfileName'
      &dcallmp 104							// setContents()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdb 6							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 37							//'onNewProfileConfirm'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 102							//'m_newProfileName'
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdbgm 41							//'newProfileName'
      &pushsdb 105							//'getContents'
      &callMethod
      &setMember
      &pushsdb 98							//'FSCommand:CallGameFunction'
      &pushsdb 106							//'%AddProfile?ProfileName='
      &pushregister 1
      &pushsdbgm 102							//'m_newProfileName'
      &add
      &getURL2
      &pushsdb 103							//''
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 5							//'newProfileMC'
      &pushsdbgm 41							//'newProfileName'
      &dcallmp 104							// setContents()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'OnProfileClicked'
    &function2  (r:3='index') ()
      &pushzero
      &pushsdb 71							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 107							//'QueryGameEngine?FE_CURRENT_PROFILE_INDEX&FE_PROFILE_COUNT'
      &pushregister 1
      &loadVariables
      &pushregister 3
      &pushregister 1
      &pushsdbgm 108							//'FE_PROFILE_COUNT'
      &lessThan
      &not
      &not
      &jnz label8      
      &pushundef
      &return
     label8:
      &pushregister 1
      &pushsdbgm 87							//'FE_CURRENT_PROFILE_INDEX'
      &setRegister r:2
      &pop
      &pushregister 3
      &pushregister 2
      &equals
      &not
      &jnz label9      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 34							//'btnAcceptProfile'
      &dcallmp 56							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 32							//'btnDeleteProfile'
      &dcallmp 56							// disable()
      &jmp label10      
     label9:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 34							//'btnAcceptProfile'
      &dcallmp 65							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 27							//'profileMC'
      &pushsdbgm 32							//'btnDeleteProfile'
      &dcallmp 65							// enable()
     label10:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 73							//'m_profiles'
    &pushzero
    &pushsdb 25							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 77							//'m_profileTokens'
    &pushzero
    &pushsdb 25							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_profileManager'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 109							//'ASSetPropFlags'
    &callFunction
   label11:
    &pop
  &end // of initMovieClip 21
&end
