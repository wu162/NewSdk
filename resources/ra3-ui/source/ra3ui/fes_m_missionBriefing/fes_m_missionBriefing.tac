movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FES_M_~3\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4
  
  &exportAssets
    4 &as 'WaitingMessageMC'
  &end // of exportAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 5
  &end // of importAssets
  
  &exportAssets
    4 &as 'WaitingMessageMC'
  &end // of exportAssets

  &defineMovieClip 6 // total frames: 1

    &placeMovieClip 5 &as 'btnReplay'
    
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
        &pushssv '$REPLAY_BUTTON'
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
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5

    &placeMovieClip 5 &as 'btnSkip'
    
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
        &pushssv '$CONTINUE'
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
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5

    &placeMovieClip 5 &as 'btnMainMenu'
    
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
        &pushssv '$MAIN_MENU_BUTTON'
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
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as 'mcButtonBar2'
  &end // of exportAssets

  &defineMovieClip 10 // total frames: 1

    &placeMovieClip 5 &as 'btnMainMenu'
    
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
        &pushssv '$MAIN_MENU_BUTTON'
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
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5

    &placeMovieClip 5 &as 'btnRecruit'
    
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
        &pushssv '$RECRUIT_BUTTON'
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
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5

    &placeMovieClip 5 &as 'btnAI'
    
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
        &pushssv '$AI_BUTTON'
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
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
    &end // of placeMovieClip 5

    &placeMovieClip 5 &as 'btnReplayVideo'
    
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
        &pushssv '$REPLAY_VIDEO_BUTTON'
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
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
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5
  &end // of defineMovieClip 10
  
  &exportAssets
    10 &as 'mcButtonBar1'
  &end // of exportAssets

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 22 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 22
  
  &exportAssets
    22 &as 'mcDashboard Symbol'
  &end // of exportAssets

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23
  
  &exportAssets
    23 &as 'mcBGMovie'
  &end // of exportAssets

  &defineMovieClip 28 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 28
  
  &exportAssets
    28 &as 'mcBackgoundElements'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_missionBriefing'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &exportAssets
    28 &as 'mcBackgoundElements'
  &end // of exportAssets
  
  &exportAssets
    23 &as 'mcBGMovie'
  &end // of exportAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 31
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 32
  &end // of importAssets

  &defineMovieClip 33 // total frames: 1

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 31 &as 'helpTextScrollList'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset'  
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
      &end
    &end // of placeMovieClip 31

    &placeMovieClip 32 &as 'helpTextScrollBar'
    
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
    &end // of placeMovieClip 32
  &end // of defineMovieClip 33
  
  &exportAssets
    22 &as 'mcDashboard Symbol'
  &end // of exportAssets
  
  &exportAssets
    10 &as 'mcButtonBar1'
  &end // of exportAssets
  
  &exportAssets
    6 &as 'mcButtonBar2'
  &end // of exportAssets

  &defineMovieClip 42 // total frames: 10

    &frame 9
      &gotoFrame 0
      &play
    &end // of frame 9
  &end // of defineMovieClip 42
  
  &importAssets &from 'fe_components.swf'
    'MovieComponentSymbol' &as 43
  &end // of importAssets

  &defineMovieClip 46 // total frames: 1
  &end // of defineMovieClip 46

  &defineMovieClip 47 // total frames: 8

    &placeMovieClip 43 &as 'movTalkingHead'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'MovieComponent'
        &pushs '_handle'
        &pushssv 'video1'
      &end
    &end // of placeMovieClip 43

    &frame 3
      &stop
    &end // of frame 3

    &frame 7
      &stop
    &end // of frame 7
  &end // of defineMovieClip 47

  &defineMovieClip 48 // total frames: 0
  &end // of defineMovieClip 48
  
  &exportAssets
    48 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 48
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
  &end // of initMovieClip 48

  &defineMovieClip 49 // total frames: 0
  &end // of defineMovieClip 49
  
  &exportAssets
    49 &as '__Packages.fes_m_missionBriefing'
  &end // of exportAssets
  
  &initMovieClip 49
    &constants '_global', 'fes_m_missionBriefing', 'Cafe2_BaseUIScreen', 'm_thisClass', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_CAMPAIGN', 'm_screen', 'mcBtnBar1', '_visible', 'mcBtnBar2', 'prototype', 'frameDelayedInit', 'initGUI', 'fes_m_missionBriefing::initGUI()', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'Object', 'Array', 'QueryGameEngine?CAMPAIGN_MISSION&CAMPAIGN_MISSION_NAME', 'm_missionIdx', 'CAMPAIGN_MISSION', 'm_missionName', 'CAMPAIGN_MISSION_NAME', '&Outline', 'QueryGameEngine?MISSION_OVERVIEW_BACKGROUND?MissionIndex=', '&MISSION_OVERVIEW_TALKING_HEAD?MissionIndex=', '&MISSION_INTRO_MOVIE?MissionIndex=', 'm_swfBackground', 'MISSION_OVERVIEW_BACKGROUND', 'm_fmvTalkingHead', 'MISSION_OVERVIEW_TALKING_HEAD', 'm_introMovie', 'MISSION_INTRO_MOVIE', 'mcTransmission', 'loadBriefingAnimation', 'onMainMenuClicked', 'btnMainMenu', 'configureButton', 'onReplayVideoClicked', 'btnReplayVideo', 'COOPMODE', 'GAME_COOPMODE_CHOOSER', 'GAME_COOPMODE_HOST', 'GAME_COOPMODE_CLIENT', 'GAME_COOPMODE_AI', 'PLAY_MISSION_INTRO_MOVIE', 'playMissionIntroMovie', 'onRecruitClicked', 'btnRecruit', 'onAIClicked', 'btnAI', 'onSkipClicked', 'btnSkip', 'waitingMessageMC', 'helpTextScrollBar', 'helpTextScrollList', '$CAMPAIGNMODESELECTIONHELPTEXT', 'initHelpPanel', 'initCampaignModeSelectionUI', '$ABORT_BUTTON', 'setLabel', 'disable', 'initMissionBriefingUI', 'm_playBriefing', 'PlayBriefing', 'hide', 'onReplayClicked', 'btnReplay', 'show', 'enable', 'setOnMouseUpFunction', 'noIntro', 'fes_m_missionBriefing::loadBriefingAnimation() : m_swfBackground = ', 'm_mcBGMovie', 'mcBGMovie', '.swf', 'loadMovie', 'WARNING: m_swfBackground is undefined in fes_m_missionBriefing::loadBriefingAnimation()', 'onBriefingLoaded', 'fes_m_missionBriefing::onBriefingLoaded()', '_wait', 'gotoAndStop', 'fes_m_missionBriefing::onScreenExit()', 'removeMessageHandler', 'StopBriefing', 'unloadMovie', 'clearData', 'm_isContinuingToObjectivesBriefing', 'SOUND', 'SHELL_MAIN', 'PlayShellMusic', 'fes_m_missionBriefing::localMessageHandler() Received message \'', '\'', 'MSGCODE', 'FE_COOP_ONLINE_GAME_CONNECT_SUCCESS', 'FE_MP_ONLINE_GAMECONNECT_FAILURE', 'FE_MP_ONLINE_LOBBY_JOINED', 'FE_COOP_CAMPAIGN_STAGING_STEP_OBJECTIVE_BRIEFING', 'FE_COOP_CAMPAIGN_COCOMMANDER_READY', 'FE_COOP_CAMPAIGN_COMMANDER_READY', 'fes_m_missionBriefing::localMessageHandler() Handling message \'', 'm_waitingForSignIn', 'comLink', 'screen', 'close', 'SCREEN', 'FEG_RECRUITMENT_UI', 'startOuttro', 'goToObjectivesBriefing', 'fes_m_missionBriefing::onMainMenuClicked', 'GAME_MODE_NONE', 'FEG_MAIN_MENU', 'changeMainScreen', 'fes_m_missionBriefing::onRecruitClicked', 'QueryGameEngine?ONLINE_SIGNED_IN', 'ONLINE_SIGNED_IN', 'open', 'fes_m_missionBriefing::onAIClicked', 'fes_m_missionBriefing::PlayBriefing', '_run', 'gotoAndPlay', 'm_playingVideo', 'mcTalkingHead', 'movTalkingHead', 'StopMovie', 'PlayMovie', 'fes_m_missionBriefing::StopBriefing', 'stop', 'fes_m_missionBriefing::initCampaignModeSelectionUI', '_alpha', 'disableTalkingHeadVideo', 'disableGadget', '_disable', 'mcBackground', 'disableDashboard', 'fes_m_missionBriefing::initMissionBriefingUI', 'enableTalkingHeadVideo', 'enableGadget', '_enable', 'enableDashboard', 'BRIEFING', 'FSCommand:CallGameFunction', '%PlayMovieAtomic?movie=', '|allowCancel=1', 'fes_m_missionBriefing::onReplayClicked', 'fes_m_missionBriefing::onSkipClicked', 'FES_COOP_OBJECTIVES_BRIEFING', 'onBackgroundMovieFinished', 'fes_m_missionBriefing::onBackgroundMovieFinished', '$CONTINUE', 'setText', 'dashBoardMC', 'titleMC', 'titleTF', 'text', 'shadowTF', 'mcGadget', 'setNumEntries', 'setVerticalPadding', 'addListener', 'disableEntryHitRegion', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'useHighlights', 'isSelectable', 'useHitRegion', 'autoSize', 'push', 'init', 'attachScrollbarController', 'setColumnData', 'setSelectedIndex', 'refreshDisplay', 'refreshScrollbar', 'moveViewToFirst', '', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_missionBriefing'
    &not
    &not
    &jnz label29    
    &pushglobalgv
    &pushsdb 1							//'fes_m_missionBriefing'
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
      &pushregister 3
      &pushsdbgm 4							//'GAME'
      &pushsdb 5							//'MODE'
      &pushregister 3
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 7							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_missionBriefing'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 12							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 13							//'frameDelayedInit'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 14							// initGUI()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 14							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 15							//'fes_m_missionBriefing::initGUI()'
      &trace
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
      &pushregister 1
      &pushsdb 20							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 21							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 22							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 20							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 23							//'gMH'
      &dcallmp 24							// addMessageHandler()
      &pushzero
      &pushsdb 25							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 26							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 27							//'QueryGameEngine?CAMPAIGN_MISSION&CAMPAIGN_MISSION_NAME'
      &pushregister 3
      &loadVariables
      &pushregister 1
      &pushsdb 28							//'m_missionIdx'
      &pushregister 3
      &pushsdbgm 29							//'CAMPAIGN_MISSION'
      &setMember
      &pushregister 1
      &pushsdb 30							//'m_missionName'
      &pushregister 3
      &pushsdbgm 31							//'CAMPAIGN_MISSION_NAME'
      &pushsdb 32							//'&Outline'
      &add
      &setMember
      &pushsdb 33							//'QueryGameEngine?MISSION_OVERVIEW_BACKGROUND?MissionIndex='
      &pushregister 1
      &pushsdbgm 28							//'m_missionIdx'
      &add
      &pushsdb 34							//'&MISSION_OVERVIEW_TALKING_HEAD?MissionIndex='
      &add
      &pushregister 1
      &pushsdbgm 28							//'m_missionIdx'
      &add
      &pushsdb 35							//'&MISSION_INTRO_MOVIE?MissionIndex='
      &add
      &pushregister 1
      &pushsdbgm 28							//'m_missionIdx'
      &add
      &pushregister 3
      &loadVariables
      &pushregister 1
      &pushsdb 36							//'m_swfBackground'
      &pushregister 3
      &pushsdbgm 37							//'MISSION_OVERVIEW_BACKGROUND'
      &setMember
      &pushregister 1
      &pushsdb 38							//'m_fmvTalkingHead'
      &pushregister 3
      &pushsdbgm 39							//'MISSION_OVERVIEW_TALKING_HEAD'
      &setMember
      &pushregister 1
      &pushsdb 40							//'m_introMovie'
      &pushregister 3
      &pushsdbgm 41							//'MISSION_INTRO_MOVIE'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 42							//'mcTransmission'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 43							// loadBriefingAnimation()
      &pushregister 1
      &pushsdbgm 44							//'onMainMenuClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdbgm 45							//'btnMainMenu'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushregister 1
      &pushsdbgm 47							//'onReplayVideoClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdbgm 48							//'btnReplayVideo'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushregister 1
      &pushsdbgm 44							//'onMainMenuClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 45							//'btnMainMenu'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdbgm 49							//'COOPMODE'
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 50							//'GAME_COOPMODE_CHOOSER'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 51							//'GAME_COOPMODE_HOST'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 52							//'GAME_COOPMODE_CLIENT'
      &strictEquals
      &jnz label4      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 53							//'GAME_COOPMODE_AI'
      &strictEquals
      &jnz label5      
      &jmp label5      
     label1:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 42							//'mcTransmission'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdbgm 54							//'PLAY_MISSION_INTRO_MOVIE'
      &not
      &jnz label2      
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 54							//'PLAY_MISSION_INTRO_MOVIE'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 55							// playMissionIntroMovie()
     label2:
      &pushregister 1
      &pushsdbgm 56							//'onRecruitClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdbgm 57							//'btnRecruit'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushregister 1
      &pushsdbgm 58							//'onAIClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdbgm 59							//'btnAI'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushregister 1
      &pushsdbgm 60							//'onSkipClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 61							//'btnSkip'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 62							//'waitingMessageMC'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 42							//'mcTransmission'
      &pushsdbgm 63							//'helpTextScrollBar'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 42							//'mcTransmission'
      &pushsdbgm 64							//'helpTextScrollList'
      &pushsdb 65							//'$CAMPAIGNMODESELECTIONHELPTEXT'
      &pushone
      &initArray
      &pushbyte 3
      &pushregister 1
      &dcallmp 66							// initHelpPanel()
      &pushzero
      &pushregister 1
      &dcallmp 67							// initCampaignModeSelectionUI()
      &jmp label6      
     label3:
      &pushsdb 68							//'$ABORT_BUTTON'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 45							//'btnMainMenu'
      &dcallmp 69							// setLabel()
      &pushregister 1
      &pushsdbgm 60							//'onSkipClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 61							//'btnSkip'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 61							//'btnSkip'
      &dcallmp 70							// disable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 62							//'waitingMessageMC'
      &pushsdb 10							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 55							// playMissionIntroMovie()
      &getURL 'FSCommand:CallGameFunction'       'CoopCampaignFlow_HostReady'      
      &pushzero
      &pushregister 1
      &dcallmp 71							// initMissionBriefingUI()
      &pushregister 1
      &pushsdb 72							//'m_playBriefing'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 73							// PlayBriefing()
      &jmp label6      
     label4:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 61							//'btnSkip'
      &dcallmp 74							// hide()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 62							//'waitingMessageMC'
      &pushsdb 10							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 68							//'$ABORT_BUTTON'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 45							//'btnMainMenu'
      &dcallmp 69							// setLabel()
      &pushzero
      &pushregister 1
      &dcallmp 55							// playMissionIntroMovie()
      &getURL 'FSCommand:CallGameFunction'       'CoopCampaignFlow_ClientReady'      
      &pushzero
      &pushregister 1
      &dcallmp 71							// initMissionBriefingUI()
      &pushregister 1
      &pushsdb 72							//'m_playBriefing'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 73							// PlayBriefing()
      &jmp label6      
     label5:
      &pushregister 1
      &pushsdbgm 60							//'onSkipClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 61							//'btnSkip'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushregister 1
      &pushsdbgm 44							//'onMainMenuClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdbgm 45							//'btnMainMenu'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 62							//'waitingMessageMC'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 71							// initMissionBriefingUI()
      &pushregister 1
      &pushsdb 72							//'m_playBriefing'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 73							// PlayBriefing()
      &jmp label6      
     label6:
      &pushregister 1
      &pushsdbgm 75							//'onReplayClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 76							//'btnReplay'
      &pushbyte 2
      &pushregister 1
      &dcallmp 46							// configureButton()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 46							//'configureButton'
    &function2  (r:1='button', r:2='callback') ()
      &pushzero
      &pushregister 1
      &dcallmp 77							// show()
      &pushzero
      &pushregister 1
      &dcallmp 78							// enable()
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 79							// setOnMouseUpFunction()
      &pushzero
      &pushregister 1
      &dcallmp 80							// noIntro()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 43							//'loadBriefingAnimation'
    &function2  () (r:1='this')
      &pushsdb 81							//'fes_m_missionBriefing::loadBriefingAnimation() : m_swfBackground = '
      &pushregister 1
      &pushsdbgm 36							//'m_swfBackground'
      &add
      &trace
      &pushregister 1
      &pushsdbgm 36							//'m_swfBackground'
      &pushundef
      &equals
      &not
      &not
      &jnz label7      
      &pushregister 1
      &pushsdb 82							//'m_mcBGMovie'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 83							//'mcBGMovie'
      &setMember
      &pushregister 1
      &pushsdbgm 36							//'m_swfBackground'
      &pushsdb 84							//'.swf'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &dcallmp 85							// loadMovie()
      &jmp label8      
     label7:
      &pushsdb 86							//'WARNING: m_swfBackground is undefined in fes_m_missionBriefing::loadBriefingAnimation()'
      &trace
     label8:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 87							//'onBriefingLoaded'
    &function2  () (r:1='this')
      &pushsdb 88							//'fes_m_missionBriefing::onBriefingLoaded()'
      &trace
      &pushregister 1
      &pushsdbgm 72							//'m_playBriefing'
      &not
      &jnz label9      
      &pushzero
      &pushregister 1
      &dcallmp 73							// PlayBriefing()
      &jmp label10      
     label9:
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 89							//'_wait'
      &pushone
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &dcallmp 90							// gotoAndStop()
     label10:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 16							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 91							//'fes_m_missionBriefing::onScreenExit()'
      &trace
      &pushregister 1
      &pushsdbgm 20							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 23							//'gMH'
      &dcallmp 92							// removeMessageHandler()
      &pushregister 1
      &pushsdb 20							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 93							// StopBriefing()
      &pushzero
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &dcallmp 94							// unloadMovie()
      &pushregister 1
      &pushsdb 82							//'m_mcBGMovie'
      &pushnull
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 95							// clearData()
      &pushregister 2
      &pushsdb 1							//'fes_m_missionBriefing'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 96							//'m_isContinuingToObjectivesBriefing'
      &not
      &not
      &jnz label11      
      &pushregister 2
      &pushsdbgm 97							//'SOUND'
      &pushsdbgm 98							//'SHELL_MAIN'
      &pushone
      &pushregister 2
      &dcallmp 99							// PlayShellMusic()
     label11:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 21							//'localMessageHandler'
    &function2  (r:4='message') (r:1='this', r:2='_global')
      &pushsdb 100							//'fes_m_missionBriefing::localMessageHandler() Received message \''
      &pushregister 4
      &add
      &pushsdb 101							//'\''
      &add
      &trace
      &pushtrue
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 102							//'MSGCODE'
      &pushsdbgm 103							//'FE_COOP_ONLINE_GAME_CONNECT_SUCCESS'
      &strictEquals
      &jnz label12      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 102							//'MSGCODE'
      &pushsdbgm 104							//'FE_MP_ONLINE_GAMECONNECT_FAILURE'
      &strictEquals
      &jnz label14      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 102							//'MSGCODE'
      &pushsdbgm 105							//'FE_MP_ONLINE_LOBBY_JOINED'
      &strictEquals
      &jnz label16      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 102							//'MSGCODE'
      &pushsdbgm 106							//'FE_COOP_CAMPAIGN_STAGING_STEP_OBJECTIVE_BRIEFING'
      &strictEquals
      &jnz label18      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 102							//'MSGCODE'
      &pushsdbgm 107							//'FE_COOP_CAMPAIGN_COCOMMANDER_READY'
      &strictEquals
      &jnz label19      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 102							//'MSGCODE'
      &pushsdbgm 108							//'FE_COOP_CAMPAIGN_COMMANDER_READY'
      &strictEquals
      &jnz label21      
      &jmp label23      
     label12:
      &pushsdb 109							//'fes_m_missionBriefing::localMessageHandler() Handling message \''
      &pushregister 4
      &add
      &pushsdb 101							//'\''
      &add
      &trace
      &pushregister 1
      &pushsdbgm 110							//'m_waitingForSignIn'
      &not
      &jnz label13      
      &pushregister 1
      &pushsdb 110							//'m_waitingForSignIn'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 111							//'comLink'
      &pushsdbgm 112							//'screen'
      &dcallmp 113							// close()
     label13:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 49							//'COOPMODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 51							//'GAME_COOPMODE_HOST'
      &setMember
      &getURL 'FSCommand:CallGameFunction'       '%SetGameSetupMode?Mode=Online'      
      &pushregister 2
      &pushsdbgm 114							//'SCREEN'
      &pushsdbgm 115							//'FEG_RECRUITMENT_UI'
      &pushone
      &pushregister 1
      &dcallmp 116							// startOuttro()
      &jmp label24      
     label14:
      &pushsdb 109							//'fes_m_missionBriefing::localMessageHandler() Handling message \''
      &pushregister 4
      &add
      &pushsdb 101							//'\''
      &add
      &trace
      &pushregister 1
      &pushsdbgm 110							//'m_waitingForSignIn'
      &not
      &jnz label15      
      &pushregister 1
      &pushsdb 110							//'m_waitingForSignIn'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 111							//'comLink'
      &pushsdbgm 112							//'screen'
      &dcallmp 113							// close()
     label15:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdbgm 57							//'btnRecruit'
      &dcallmp 78							// enable()
      &jmp label24      
     label16:
      &pushregister 1
      &pushsdbgm 110							//'m_waitingForSignIn'
      &not
      &jnz label17      
      &pushsdb 109							//'fes_m_missionBriefing::localMessageHandler() Handling message \''
      &pushregister 4
      &add
      &pushsdb 101							//'\''
      &add
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%CreateOnlineCoop'      
     label17:
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label24      
     label18:
      &pushzero
      &pushregister 1
      &dcallmp 117							// goToObjectivesBriefing()
      &jmp label24      
     label19:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdbgm 49							//'COOPMODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 51							//'GAME_COOPMODE_HOST'
      &equals
      &not
      &jnz label20      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 61							//'btnSkip'
      &dcallmp 78							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 62							//'waitingMessageMC'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
     label20:
      &jmp label24      
     label21:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdbgm 49							//'COOPMODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 52							//'GAME_COOPMODE_CLIENT'
      &equals
      &not
      &jnz label22      
      &getURL 'FSCommand:CallGameFunction'       'CoopCampaignFlow_ClientReady'      
     label22:
      &jmp label24      
     label23:
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label24      
     label24:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 44							//'onMainMenuClicked'
    &function2  () (r:1='_global')
      &pushsdb 118							//'fes_m_missionBriefing::onMainMenuClicked'
      &trace
      &getURL 'FSCommand:CallGameFunction'       'Campaign_ExitGame'      
      &pushregister 1
      &pushsdbgm 4							//'GAME'
      &pushsdb 5							//'MODE'
      &pushregister 1
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 119							//'GAME_MODE_NONE'
      &setMember
      &getURL 'FSCommand:CallGameFunction'       '%SetGameSetupMode?Mode=None'      
      &pushregister 1
      &pushsdbgm 114							//'SCREEN'
      &pushsdbgm 120							//'FEG_MAIN_MENU'
      &pushone
      &pushregister 1
      &pushsdbgm 18							//'gSM'
      &dcallmp 121							// changeMainScreen()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 56							//'onRecruitClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 122							//'fes_m_missionBriefing::onRecruitClicked'
      &trace
      &pushzero
      &pushsdb 25							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 123							//'QueryGameEngine?ONLINE_SIGNED_IN'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 124							//'ONLINE_SIGNED_IN'
      &toNumber
      &pushone
      &equals
      &not
      &jnz label25      
      &getURL 'FSCommand:CallGameFunction'       '%CreateOnlineCoop'      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdbgm 57							//'btnRecruit'
      &dcallmp 70							// disable()
      &jmp label26      
     label25:
      &pushregister 1
      &pushsdb 110							//'m_waitingForSignIn'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 111							//'comLink'
      &pushsdbgm 112							//'screen'
      &dcallmp 125							// open()
     label26:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 58							//'onAIClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 126							//'fes_m_missionBriefing::onAIClicked'
      &trace
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 49							//'COOPMODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 53							//'GAME_COOPMODE_AI'
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 71							// initMissionBriefingUI()
      &pushzero
      &pushregister 1
      &dcallmp 73							// PlayBriefing()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 73							//'PlayBriefing'
    &function2  () (r:1='this')
      &pushsdb 127							//'fes_m_missionBriefing::PlayBriefing'
      &trace
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &pushsdb 10							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 128							//'_run'
      &pushone
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &dcallmp 129							// gotoAndPlay()
      &pushregister 1
      &pushsdbgm 38							//'m_fmvTalkingHead'
      &pushundef
      &equals
      &not
      &not
      &jnz label28      
      &pushregister 1
      &pushsdbgm 130							//'m_playingVideo'
      &not
      &jnz label27      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 131							//'mcTalkingHead'
      &pushsdbgm 132							//'movTalkingHead'
      &dcallmp 133							// StopMovie()
     label27:
      &pushregister 1
      &pushsdb 130							//'m_playingVideo'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 38							//'m_fmvTalkingHead'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 131							//'mcTalkingHead'
      &pushsdbgm 132							//'movTalkingHead'
      &dcallmp 134							// PlayMovie()
     label28:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 93							//'StopBriefing'
    &function2  () (r:1='this')
      &pushsdb 135							//'fes_m_missionBriefing::StopBriefing'
      &trace
      &pushzero
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &dcallmp 136							// stop()
      &pushregister 1
      &pushsdbgm 82							//'m_mcBGMovie'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 131							//'mcTalkingHead'
      &pushsdbgm 132							//'movTalkingHead'
      &dcallmp 133							// StopMovie()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 67							//'initCampaignModeSelectionUI'
    &function2  () (r:1='this')
      &pushsdb 137							//'fes_m_missionBriefing::initCampaignModeSelectionUI'
      &trace
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 42							//'mcTransmission'
      &pushsdb 10							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 42							//'mcTransmission'
      &pushsdb 138							//'_alpha'
      &pushbyte 100
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdb 10							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 139							// disableTalkingHeadVideo()
      &pushzero
      &pushregister 1
      &dcallmp 140							// disableGadget()
      &pushsdb 141							//'_disable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 142							//'mcBackground'
      &dcallmp 129							// gotoAndPlay()
      &pushzero
      &pushregister 1
      &dcallmp 143							// disableDashboard()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 71							//'initMissionBriefingUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 144							//'fes_m_missionBriefing::initMissionBriefingUI'
      &trace
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 42							//'mcTransmission'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 9							//'mcBtnBar1'
      &pushsdb 10							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 145							// enableTalkingHeadVideo()
      &pushzero
      &pushregister 1
      &dcallmp 146							// enableGadget()
      &pushsdb 147							//'_enable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 142							//'mcBackground'
      &dcallmp 129							// gotoAndPlay()
      &pushzero
      &pushregister 1
      &dcallmp 148							// enableDashboard()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdb 10							//'_visible'
      &pushtrue
      &setMember
      &pushregister 2
      &pushsdbgm 97							//'SOUND'
      &pushsdbgm 149							//'BRIEFING'
      &pushone
      &pushregister 2
      &dcallmp 99							// PlayShellMusic()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 55							//'playMissionIntroMovie'
    &function2  () (r:1='this')
      &pushsdb 150							//'FSCommand:CallGameFunction'
      &pushsdb 151							//'%PlayMovieAtomic?movie='
      &pushregister 1
      &pushsdbgm 40							//'m_introMovie'
      &add
      &pushsdb 152							//'|allowCancel=1'
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 47							//'onReplayVideoClicked'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 55							// playMissionIntroMovie()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 75							//'onReplayClicked'
    &function2  () (r:1='this')
      &pushsdb 153							//'fes_m_missionBriefing::onReplayClicked'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 73							// PlayBriefing()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 60							//'onSkipClicked'
    &function2  () (r:1='this')
      &pushsdb 154							//'fes_m_missionBriefing::onSkipClicked'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 93							// StopBriefing()
      &getURL 'FSCommand:CallGameFunction'       '%CoopCampaignFlow_ObjectiveBriefing'      
      &pushzero
      &pushregister 1
      &dcallmp 117							// goToObjectivesBriefing()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 117							//'goToObjectivesBriefing'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 96							//'m_isContinuingToObjectivesBriefing'
      &pushtrue
      &setMember
      &pushregister 2
      &pushsdbgm 114							//'SCREEN'
      &pushsdbgm 155							//'FES_COOP_OBJECTIVES_BRIEFING'
      &pushone
      &pushregister 1
      &dcallmp 116							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 156							//'onBackgroundMovieFinished'
    &function  (    )    
      &pushsdb 157							//'fes_m_missionBriefing::onBackgroundMovieFinished'
      &trace
      &pushsdb 158							//'$CONTINUE'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 11							//'mcBtnBar2'
      &pushsdbgm 61							//'btnSkip'
      &dcallmp 159							// setText()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 148							//'enableDashboard'
    &function2  () (r:1='this')
      &pushsdb 147							//'_enable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 160							//'dashBoardMC'
      &dcallmp 129							// gotoAndPlay()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 160							//'dashBoardMC'
      &pushsdbgm 161							//'titleMC'
      &pushsdbgm 162							//'titleTF'
      &pushsdb 163							//'text'
      &pushregister 1
      &pushsdbgm 30							//'m_missionName'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 160							//'dashBoardMC'
      &pushsdbgm 161							//'titleMC'
      &pushsdbgm 164							//'shadowTF'
      &pushsdb 163							//'text'
      &pushregister 1
      &pushsdbgm 30							//'m_missionName'
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 143							//'disableDashboard'
    &function2  () (r:1='this')
      &pushsdb 141							//'_disable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 160							//'dashBoardMC'
      &dcallmp 129							// gotoAndPlay()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 160							//'dashBoardMC'
      &pushsdbgm 161							//'titleMC'
      &pushsdbgm 162							//'titleTF'
      &pushsdb 163							//'text'
      &pushregister 1
      &pushsdbgm 30							//'m_missionName'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 160							//'dashBoardMC'
      &pushsdbgm 161							//'titleMC'
      &pushsdbgm 164							//'shadowTF'
      &pushsdb 163							//'text'
      &pushregister 1
      &pushsdbgm 30							//'m_missionName'
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 145							//'enableTalkingHeadVideo'
    &function  (    )    
      &pushsdb 147							//'_enable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 131							//'mcTalkingHead'
      &dcallmp 129							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 139							//'disableTalkingHeadVideo'
    &function  (    )    
      &pushsdb 141							//'_disable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 131							//'mcTalkingHead'
      &dcallmp 129							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 146							//'enableGadget'
    &function  (    )    
      &pushsdb 147							//'_enable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 165							//'mcGadget'
      &dcallmp 129							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 140							//'disableGadget'
    &function  (    )    
      &pushsdb 141							//'_disable'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 8							//'m_screen'
      &pushsdbgm 165							//'mcGadget'
      &dcallmp 129							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 66							//'initHelpPanel'
    &function2  (r:13='listData', r:2='listMC', r:4='listScrollBar') (r:1='this')
      &pushzero
      &pushregister 4
      &dcallmp 77							// show()
      &pushzero
      &pushregister 4
      &dcallmp 78							// enable()
      &pushbyte 10
      &pushone
      &pushregister 2
      &dcallmp 166							// setNumEntries()
      &pushzero
      &pushone
      &pushregister 2
      &dcallmp 167							// setVerticalPadding()
      &pushregister 1
      &pushone
      &pushregister 2
      &dcallmp 168							// addListener()
      &pushzero
      &pushregister 2
      &dcallmp 169							// disableEntryHitRegion()
      &pushzero
      &pushsdb 26							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 170							//'symbol'
      &pushsdb 171							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 172							//'width'
      &pushshort 520
      &pushsdb 173							//'leftMargin'
      &pushbyte 20
      &pushsdb 174							//'rightMargin'
      &pushbyte 40
      &pushsdb 175							//'useHighlights'
      &pushfalse
      &pushsdb 176							//'isSelectable'
      &pushfalse
      &pushsdb 177							//'useHitRegion'
      &pushfalse
      &pushsdb 178							//'autoSize'
      &pushtrue
      &pushbyte 8
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 179							// push()
      &pushbyte 27
      &pushregister 3
      &pushbyte 2
      &pushregister 2
      &dcallmp 180							// init()
      &pushregister 4
      &pushone
      &pushregister 2
      &dcallmp 181							// attachScrollbarController()
      &pushregister 13
      &pushzero
      &pushbyte 2
      &pushregister 2
      &dcallmp 182							// setColumnData()
      &pushzero
      &pushone
      &pushregister 2
      &dcallmp 183							// setSelectedIndex()
      &pushzero
      &pushregister 2
      &dcallmp 184							// refreshDisplay()
      &pushzero
      &pushregister 2
      &dcallmp 185							// refreshScrollbar()
      &pushzero
      &pushregister 2
      &dcallmp 186							// moveViewToFirst()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 38							//'m_fmvTalkingHead'
    &pushsdb 187							//''
    &setMember
    &pushregister 1
    &pushsdb 28							//'m_missionIdx'
    &pushbyte -1
    &setMember
    &pushregister 1
    &pushsdb 110							//'m_waitingForSignIn'
    &pushfalse
    &setMember
    &pushregister 1
    &pushsdb 96							//'m_isContinuingToObjectivesBriefing'
    &pushfalse
    &setMember
    &pushregister 1
    &pushsdb 130							//'m_playingVideo'
    &pushfalse
    &setMember
    &pushregister 1
    &pushsdb 72							//'m_playBriefing'
    &pushfalse
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_missionBriefing'
    &pushsdbgm 12							//'prototype'
    &pushbyte 3
    &pushsdb 188							//'ASSetPropFlags'
    &callFunction
   label29:
    &pop
  &end // of initMovieClip 49
&end
