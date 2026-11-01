movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEM_M_~4\\FEM_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5
  
  &exportAssets
    5 &as 'Chapter Chooser Frame'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as 'Chat Frame MC'
  &end // of exportAssets

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11
  
  &exportAssets
    11 &as 'mainCommanderLabelMC'
  &end // of exportAssets
  
  &exportAssets
    11 &as 'mainCommanderLabelMC'
  &end // of exportAssets

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17
  
  &exportAssets
    11 &as 'mainCommanderLabelMC'
  &end // of exportAssets

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as 'Commander Frame'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fem_m_coopStaging'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 22
  &end // of importAssets
  
  &exportAssets
    19 &as 'Commander Frame'
  &end // of exportAssets
  
  &exportAssets
    9 &as 'Chat Frame MC'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'Chapter Chooser Frame'
  &end // of exportAssets
  
  &importAssets &from 'screens/fe_m_assetsGameSetup.swf'
    'MapPreviewComponent' &as 25
  &end // of importAssets
  
  &importAssets &from '../mouse/Components/mouse_chapterSelectComponent.swf'
    'chapterSelectComponentSymbol' &as 26
  &end // of importAssets

  &defineMovieClip 27 // total frames: 1

    &placeMovieClip 25 &as 'mapPreview'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'MapPreview'
        &pushs '_indexMap'
        &pushbyte -1
        &setVariable
      &end
    &end // of placeMovieClip 25
  &end // of defineMovieClip 27

  &defineMovieClip 30 // total frames: 1

    &placeMovieClip 25 &as 'mapPreview'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'MapPreview'
        &pushs '_indexMap'
        &pushbyte -1
        &setVariable
      &end
    &end // of placeMovieClip 25
  &end // of defineMovieClip 30

  &defineMovieClip 31 // total frames: 1
  &end // of defineMovieClip 31
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 32
  &end // of importAssets

  &placeMovieClip 32 &as 'button_Play'
  
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
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$CONTINUE'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 32

  &placeMovieClip 32 &as 'button_Back'
  
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
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$ABORT_BUTTON'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 32

  &placeMovieClip 32 &as 'button_Load'
  
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
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$LOAD'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 32
  
  &importAssets &from '../mouse/Components/chatComponent.swf'
    'chatComponentSymbol' &as 33
  &end // of importAssets

  &placeMovieClip 33 &as 'chatComponent'
  
    &onClipEvent &construct
      &pushs 'm_numLines'
      &pushzerosv
      &pushs 'm_width'
      &pushshort 476
      &setVariable
      &pushs 'm_chatMode'
      &pushone
      &setVariable
      &pushs 'm_preservePlacement'
      &pushfalse
      &setVariable
      &pushs 'm_height'
      &pushshort 400
      &setVariable
    &end
  &end // of placeMovieClip 33
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 34
  &end // of importAssets

  &placeMovieClip 34 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$HEADERCOOPSTAGING'
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
  &end // of placeMovieClip 34

  &defineMovieClip 35 // total frames: 0
  &end // of defineMovieClip 35
  
  &exportAssets
    35 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 35
    &constants '_global', 'Cafe2_BaseUIScreen', 'm_screen', 'onEnterFrame', 'frameDelayedInitCallback', 'bind0', '_alpha', 'prototype', 'frameDelayedInit', '\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :', '\nscreens and move the call to initGUI() to frameDelayedInit().\n', 'N.B. You *must* override frameDelayedInit() in your', 'Array', 'm_initCalled', 'Fading in screen:', 'outtroComplete', 'Cafe2_BaseUIScreen::outtroComplete() ', 'm_thisClass', 'm_uiComponentArray', 'length', 'splice', 'm_changeToScreen', 'gSM', 'changeMainScreen', 'introComplete', 'Cafe2_BaseUIScreen::introComplete()', 'restoreVisualState', 'm_introCount', 'onIntrosComplete', 'Cafe2_BaseUIScreen::onIntrosComplete()', 'localEventHandler', 'gEH', 'setLocalEventHandler', 'enableCurrentLocalEventHandler', 'registerIntroOuttroComponents', 'Cafe2_BaseUIScreen::registerIntroOuttroComponents()', 'setIntroCallback', 'executeBackFunction', 'Cafe2_BaseUIScreen::executeBackFunction', 'goBackScreen', 'startIntro', 'Cafe2_BaseUIScreen::startIntro()', 'intro', 'startOuttro', 'Cafe2_BaseUIScreen::startOuttro', 'outtro', 'startOuttroAndCallBack', 'm_executeOuttroCallBack', 'outtroAndCallBackComplete', 'backButtonClicked', 'Cafe2_BaseUIScreen::backButtonClicked()', 'Cafe2_BaseUIScreen::localEventHandler() keyCode == ', 'gFM', 'handleDefaultInputs', 'clearData', 'obj', 'func', 'call', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &not
    &not
    &jnz label32    
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
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 6							//'_alpha'
      &pushzero
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushregister 1
    &pushsdbgm 7							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 8							//'frameDelayedInit'
    &function2  () ()
      &pushsdb 9							//'\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :'
      &trace
      &pushsdb 10							//'\nscreens and move the call to initGUI() to frameDelayedInit().\n'
      &pushsdb 11							//'N.B. You *must* override frameDelayedInit() in your'
      &pushbyte 2
      &pushsdb 12							//'Array'
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
      &pushregister 1
      &pushsdbgm 13							//'m_initCalled'
      &not
      &not
      &jnz label1      
      &pushregister 1
      &pushsdb 13							//'m_initCalled'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 8							// frameDelayedInit()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 6							//'_alpha'
      &pushbyte 20
      &setMember
      &jmp label2      
     label1:
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 6							//'_alpha'
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 6							//'_alpha'
      &pushbyte 20
      &add
      &setMember
      &pushsdb 14							//'Fading in screen:'
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 6							//'_alpha'
      &add
      &trace
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 6							//'_alpha'
      &pushbyte 100
      &lessThan
      &not
      &not
      &jnz label2      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 3							//'onEnterFrame'
      &pushnull
      &setMember
     label2:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 15							//'outtroComplete'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 16							//'Cafe2_BaseUIScreen::outtroComplete() '
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label3:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &lessThan
      &not
      &jnz label5      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &equals
      &not
      &jnz label4      
      &jmp label5      
     label4:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label3      
     label5:
      &pushone
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &dcallmp 20							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label6      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 21							//'m_changeToScreen'
      &pushnull
      &equals
      &not
      &not
      &jnz label6      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 21							//'m_changeToScreen'
      &pushone
      &pushregister 1
      &pushsdbgm 22							//'gSM'
      &dcallmp 23							// changeMainScreen()
     label6:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'introComplete'
    &function2  (r:1='theComponent') ()
      &pushsdb 25							//'Cafe2_BaseUIScreen::introComplete()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 26							// restoreVisualState()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdb 27							//'m_introCount'
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 27							//'m_introCount'
      &decrement
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 27							//'m_introCount'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label7      
      &pushzero
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &dcallmp 28							// onIntrosComplete()
     label7:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'onIntrosComplete'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 29							//'Cafe2_BaseUIScreen::onIntrosComplete()'
      &trace
      &pushregister 1
      &pushsdbgm 30							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 31							//'gEH'
      &dcallmp 32							// setLocalEventHandler()
      &pushzero
      &pushregister 2
      &pushsdbgm 31							//'gEH'
      &dcallmp 33							// enableCurrentLocalEventHandler()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'registerIntroOuttroComponents'
    &function2  (r:3='components') (r:1='this')
      &pushsdb 35							//'Cafe2_BaseUIScreen::registerIntroOuttroComponents()'
      &trace
      &pushregister 1
      &pushsdb 18							//'m_uiComponentArray'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 27							//'m_introCount'
      &pushregister 1
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &setMember
      &pushzero
      &setRegister r:2
      &pop
     label8:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 27							//'m_introCount'
      &lessThan
      &not
      &jnz label9      
      &pushregister 1
      &pushsdbgm 24							//'introComplete'
      &pushone
      &pushregister 1
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 36							// setIntroCallback()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label8      
     label9:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 37							//'executeBackFunction'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 38							//'Cafe2_BaseUIScreen::executeBackFunction'
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label10:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &lessThan
      &not
      &jnz label12      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &equals
      &not
      &jnz label11      
      &jmp label12      
     label11:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label10      
     label12:
      &pushone
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &dcallmp 20							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label13      
      &pushzero
      &pushregister 1
      &pushsdbgm 22							//'gSM'
      &dcallmp 39							// goBackScreen()
     label13:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 40							//'startIntro'
    &function2  () (r:1='this')
      &pushsdb 41							//'Cafe2_BaseUIScreen::startIntro()'
      &trace
      &pushregister 1
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label14:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label15      
      &pushzero
      &pushregister 1
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 42							// intro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label14      
     label15:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 43							//'startOuttro'
    &function2  (r:4='newScreen') (r:1='this')
      &pushsdb 44							//'Cafe2_BaseUIScreen::startOuttro'
      &trace
      &pushregister 1
      &pushsdb 21							//'m_changeToScreen'
      &pushregister 4
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label18      
      &pushzero
      &setRegister r:2
      &pop
     label16:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label17      
      &pushregister 1
      &pushsdbgm 15							//'outtroComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 45							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label16      
     label17:
      &jmp label19      
     label18:
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 15							// outtroComplete()
     label19:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 46							//'startOuttroAndCallBack'
    &function2  (r:4='doAfterOuttroComplete') (r:1='this')
      &pushsdb 44							//'Cafe2_BaseUIScreen::startOuttro'
      &trace
      &pushregister 1
      &pushsdb 47							//'m_executeOuttroCallBack'
      &pushregister 4
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label22      
      &pushzero
      &setRegister r:2
      &pop
     label20:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label21      
      &pushregister 1
      &pushsdbgm 48							//'outtroAndCallBackComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 45							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label20      
     label21:
      &jmp label23      
     label22:
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 48							// outtroAndCallBackComplete()
     label23:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 48							//'outtroAndCallBackComplete'
    &function2  (r:2='theComponent') ()
      &pushsdb 16							//'Cafe2_BaseUIScreen::outtroComplete() '
      &pushregister 2
      &add
      &trace
      &pushzero
      &setRegister r:1
      &pop
     label24:
      &pushregister 1
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &lessThan
      &not
      &jnz label26      
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 1
      &getMember
      &equals
      &not
      &jnz label25      
      &jmp label26      
     label25:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label24      
     label26:
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &dcallmp 20							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label27      
      &pushzero
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &dcallmp 47							// m_executeOuttroCallBack()
     label27:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 49							//'backButtonClicked'
    &function2  () (r:1='_global')
      &pushsdb 50							//'Cafe2_BaseUIScreen::backButtonClicked()'
      &trace
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushsdbgm 19							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label30      
      &pushzero
      &setRegister r:2
      &pop
     label28:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label29      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 37							//'executeBackFunction'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 17							//'m_thisClass'
      &pushsdbgm 18							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 45							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label28      
     label29:
      &jmp label31      
     label30:
      &pushzero
      &pushregister 1
      &pushsdbgm 22							//'gSM'
      &dcallmp 39							// goBackScreen()
     label31:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 30							//'localEventHandler'
    &function2  (r:2='keyCode', 'controller') (r:1='_global')
      &pushsdb 51							//'Cafe2_BaseUIScreen::localEventHandler() keyCode == '
      &pushregister 2
      &add
      &trace
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 52							//'gFM'
      &dcallmp 53							// handleDefaultInputs()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 54							//'clearData'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 2							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 17							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 21							//'m_changeToScreen'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 18							//'m_uiComponentArray'
      &pushnull
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 5							//'bind0'
    &function  ('obj', 'func'    )    
      &function  (      )      
        &pushsdbgv 55							//'obj'
        &pushone
        &pushsdbgv 56							//'func'
        &dcallmp 57							// call()
      &end // of function 

      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 21							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 47							//'m_executeOuttroCallBack'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 18							//'m_uiComponentArray'
    &pushzero
    &pushsdb 12							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 13							//'m_initCalled'
    &pushfalse
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 58							//'ASSetPropFlags'
    &callFunction
   label32:
    &pop
  &end // of initMovieClip 35

  &defineMovieClip 36 // total frames: 0
  &end // of defineMovieClip 36
  
  &exportAssets
    36 &as '__Packages.fem_m_coopStaging'
  &end // of exportAssets
  
  &initMovieClip 36
    &constants '_global', 'fem_m_coopStaging', 'fem_m_coopStaging::fem_m_coopStaging()', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_chapterSelectPanel', 'missionChooserPanelMC', 'hostPanel', 'chapterSelectPanel', 'prototype', 'frameDelayedInit', 'm_chapterSelectCallback', 'onChapterSelectChanged', 'bind1DynamicParams', 'addObserver', 'm_localMessageHandlerFunc', 'localMessageHandler', 'gMH', 'addMessageHandler', 'm_eventHandlerIndex', 'localEventHandler', 'gEH', 'addLocalEventHandler', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_CAMPAIGN', 'initGUI', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'OnAbortButtonClick', 'm_screen', 'button_Back', 'setOnMouseUpFunction', 'OnPlayButtonClick', 'button_Play', 'OnLoadButtonClick', 'button_Load', 'enable', 'COOPMODE', 'GAME_COOPMODE_HOST', 'show', '_visible', 'clientPanel', 'hide', 'initCoopCampaignPlayerSlots', 'registerIntroOuttroComponents', 'startIntro', 'MSGCODE', 'FE_COOP_CAMPAIGN_STAGING_STEP_MISSION_BRIEFING', 'FE_MP_UPDATE_GAME_SETTINGS', 'SCREEN', 'FES_COOP_MISSION_BRIEFING', 'changeMainScreen', 'm_initSlot', 'INPUTCODE', 'ESCAPE', 'gFM', 'handleDefaultInputs', 'removeMessageHandler', 'removeLocalEventHandler', 'removeObserver', 'clearData', 'GAME_MODE_NONE', 'FEG_MAIN_MENU', 'gotoShellScreen', 'Object', 'QueryGameEngine?IS_NAT_DONE', 'IS_NAT_DONE', '1', 'SAVELOADSCREEN_TYPE', 'SAVELOADSCREEN_LOAD', 'FEG_SAVELOAD', 'FSCommand:CallGameFunction', '%SetMap?Map=', 'getSelectedMapId', '%Campaign_SetFaction?FactionIndex=', 'getSelectedCampaign', '%Campaign_SetMission?MissionIndex=', 'getSelectedMission', 'QueryGameEngine?PLAYER_STATUS?Slot=0&PLAYER_STATUS?Slot=1', 'commanderPanelMC', 'commanderNameTF', 'text', '$PLAYER_STATUS_0_0', 'co_commanderNameTF', '$PLAYER_STATUS_1_0', 'MAX_MISSIONS', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_coopStaging'
    &not
    &not
    &jnz label13    
    &pushglobalgv
    &pushsdb 1							//'fem_m_coopStaging'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushregister 3
      &pushone
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'fem_m_coopStaging::fem_m_coopStaging()'
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushsdb 5							//'m_chapterSelectPanel'
      &pushregister 3
      &pushsdbgm 6							//'missionChooserPanelMC'
      &pushsdbgm 7							//'hostPanel'
      &pushsdbgm 8							//'chapterSelectPanel'
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_coopStaging'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 9							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 10							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 11							//'m_chapterSelectCallback'
      &pushregister 1
      &pushsdbgm 12							//'onChapterSelectChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 13							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 11							//'m_chapterSelectCallback'
      &pushone
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &dcallmp 14							// addObserver()
      &pushregister 1
      &pushsdb 15							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 16							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 13							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 15							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 17							//'gMH'
      &dcallmp 18							// addMessageHandler()
      &pushregister 1
      &pushsdb 19							//'m_eventHandlerIndex'
      &pushregister 1
      &pushsdbgm 20							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 21							//'gEH'
      &pushsdb 22							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdbgm 23							//'GAME'
      &pushsdb 24							//'MODE'
      &pushregister 2
      &pushsdbgm 25							//'ENUM'
      &pushsdbgm 26							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 27							// initGUI()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 27							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 28							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 29							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 30							//'gSM'
      &dcallmp 31							// setOnExitScreen()
      &pushregister 1
      &pushsdbgm 32							//'OnAbortButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 29							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 34							//'button_Back'
      &dcallmp 35							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 36							//'OnPlayButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 29							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 37							//'button_Play'
      &dcallmp 35							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 38							//'OnLoadButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 29							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 39							//'button_Load'
      &dcallmp 35							// setOnMouseUpFunction()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 34							//'button_Back'
      &dcallmp 40							// enable()
      &pushregister 2
      &pushsdbgm 23							//'GAME'
      &pushsdbgm 41							//'COOPMODE'
      &pushregister 2
      &pushsdbgm 25							//'ENUM'
      &pushsdbgm 42							//'GAME_COOPMODE_HOST'
      &equals
      &not
      &jnz label1      
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 37							//'button_Play'
      &dcallmp 43							// show()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 37							//'button_Play'
      &dcallmp 40							// enable()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 39							//'button_Load'
      &dcallmp 43							// show()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 39							//'button_Load'
      &dcallmp 40							// enable()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 6							//'missionChooserPanelMC'
      &pushsdbgm 7							//'hostPanel'
      &pushsdb 44							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 6							//'missionChooserPanelMC'
      &pushsdbgm 45							//'clientPanel'
      &pushsdb 44							//'_visible'
      &pushfalse
      &setMember
      &jmp label2      
     label1:
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 37							//'button_Play'
      &dcallmp 46							// hide()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 39							//'button_Load'
      &dcallmp 46							// hide()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 6							//'missionChooserPanelMC'
      &pushsdbgm 7							//'hostPanel'
      &pushsdb 44							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 6							//'missionChooserPanelMC'
      &pushsdbgm 45							//'clientPanel'
      &pushsdb 44							//'_visible'
      &pushtrue
      &setMember
     label2:
      &pushzero
      &pushregister 1
      &dcallmp 12							// onChapterSelectChanged()
      &pushzero
      &pushregister 1
      &dcallmp 47							// initCoopCampaignPlayerSlots()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 39							//'button_Load'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 37							//'button_Play'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 34							//'button_Back'
      &pushbyte 3
      &initArray
      &pushone
      &pushregister 1
      &dcallmp 48							// registerIntroOuttroComponents()
      &pushzero
      &pushregister 1
      &dcallmp 49							// startIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 16							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushtrue
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 50							//'MSGCODE'
      &pushsdbgm 51							//'FE_COOP_CAMPAIGN_STAGING_STEP_MISSION_BRIEFING'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 50							//'MSGCODE'
      &pushsdbgm 52							//'FE_MP_UPDATE_GAME_SETTINGS'
      &strictEquals
      &jnz label4      
      &jmp label6      
     label3:
      &pushregister 2
      &pushsdbgm 53							//'SCREEN'
      &pushsdbgm 54							//'FES_COOP_MISSION_BRIEFING'
      &pushone
      &pushregister 2
      &pushsdbgm 30							//'gSM'
      &dcallmp 55							// changeMainScreen()
      &jmp label7      
     label4:
      &pushregister 1
      &pushsdbgm 56							//'m_initSlot'
      &not
      &jnz label5      
      &pushzero
      &pushregister 1
      &dcallmp 47							// initCoopCampaignPlayerSlots()
      &pushregister 1
      &pushsdb 56							//'m_initSlot'
      &pushfalse
      &setMember
     label5:
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
    &pushsdb 20							//'localEventHandler'
    &function2  (r:3='keyCode', 'controller') (r:1='this', r:2='_global')
      &pushregister 3
      &pushsdbgv 57							//'INPUTCODE'
      &pushsdbgm 58							//'ESCAPE'
      &strictEquals
      &jnz label8      
      &jmp label9      
     label8:
      &pushzero
      &pushregister 1
      &dcallmp 32							// OnAbortButtonClick()
      &jmp label10      
     label9:
      &pushregister 3
      &pushone
      &pushregister 2
      &pushsdbgm 59							//'gFM'
      &dcallmp 60							// handleDefaultInputs()
      &jmp label10      
     label10:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 15							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 17							//'gMH'
      &dcallmp 61							// removeMessageHandler()
      &pushregister 1
      &pushsdb 15							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 19							//'m_eventHandlerIndex'
      &pushone
      &pushregister 2
      &pushsdbgm 21							//'gEH'
      &dcallmp 62							// removeLocalEventHandler()
      &pushregister 1
      &pushsdbgm 11							//'m_chapterSelectCallback'
      &pushone
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &dcallmp 63							// removeObserver()
      &pushregister 1
      &pushsdb 11							//'m_chapterSelectCallback'
      &delete
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 64							// clearData()
      &pushregister 2
      &pushsdb 1							//'fem_m_coopStaging'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 32							//'OnAbortButtonClick'
    &function2  () (r:1='_global')
      &getURL 'FSCommand:CallGameFunction'       'Campaign_ExitGame'      
      &pushregister 1
      &pushsdbgm 23							//'GAME'
      &pushsdb 24							//'MODE'
      &pushregister 1
      &pushsdbgm 25							//'ENUM'
      &pushsdbgm 65							//'GAME_MODE_NONE'
      &setMember
      &getURL 'FSCommand:CallGameFunction'       '%SetGameSetupMode?Mode=None'      
      &pushzero
      &initArray
      &pushregister 1
      &pushsdbgm 53							//'SCREEN'
      &pushsdbgm 66							//'FEG_MAIN_MENU'
      &pushbyte 2
      &pushregister 1
      &dcallmp 67							// gotoShellScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 36							//'OnPlayButtonClick'
    &function2  () (r:1='_global')
      &getURL 'FSCommand:CallGameFunction'       '%CoopCampaignFlow_MissionBriefing'      
      &pushzero
      &initArray
      &pushregister 1
      &pushsdbgm 53							//'SCREEN'
      &pushsdbgm 54							//'FES_COOP_MISSION_BRIEFING'
      &pushbyte 2
      &pushregister 1
      &dcallmp 67							// gotoShellScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 38							//'OnLoadButtonClick'
    &function2  () (r:1='_global')
      &pushzero
      &pushsdb 68							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 69							//'QueryGameEngine?IS_NAT_DONE'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 70							//'IS_NAT_DONE'
      &pushsdb 71							//'1'
      &equals
      &not
      &jnz label11      
      &pushregister 1
      &pushsdbgm 23							//'GAME'
      &pushsdb 72							//'SAVELOADSCREEN_TYPE'
      &pushregister 1
      &pushsdbgm 25							//'ENUM'
      &pushsdbgm 73							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 1
      &pushsdbgm 53							//'SCREEN'
      &pushsdbgm 74							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &pushsdbgm 30							//'gSM'
      &dcallmp 55							// changeMainScreen()
     label11:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 12							//'onChapterSelectChanged'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 2
      &pushsdbgm 23							//'GAME'
      &pushsdbgm 41							//'COOPMODE'
      &pushregister 2
      &pushsdbgm 25							//'ENUM'
      &pushsdbgm 42							//'GAME_COOPMODE_HOST'
      &equals
      &not
      &jnz label12      
      &getURL 'FSCommand:CallGameFunction'       '%SetMapType?MapType=1'      
      &pushsdb 75							//'FSCommand:CallGameFunction'
      &pushsdb 76							//'%SetMap?Map='
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 77							//'getSelectedMapId'
      &callMethod
      &toString
      &add
      &getURL2
      &pushsdb 75							//'FSCommand:CallGameFunction'
      &pushsdb 78							//'%Campaign_SetFaction?FactionIndex='
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 79							//'getSelectedCampaign'
      &callMethod
      &toString
      &add
      &getURL2
      &pushsdb 75							//'FSCommand:CallGameFunction'
      &pushsdb 80							//'%Campaign_SetMission?MissionIndex='
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'m_chapterSelectPanel'
      &pushsdb 81							//'getSelectedMission'
      &callMethod
      &toString
      &add
      &getURL2
     label12:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 47							//'initCoopCampaignPlayerSlots'
    &function2  () ()
      &pushzero
      &pushsdb 68							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 82							//'QueryGameEngine?PLAYER_STATUS?Slot=0&PLAYER_STATUS?Slot=1'
      &pushregister 1
      &loadVariables
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 83							//'commanderPanelMC'
      &pushsdbgm 84							//'commanderNameTF'
      &pushsdb 85							//'text'
      &pushsdb 86							//'$PLAYER_STATUS_0_0'
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 33							//'m_screen'
      &pushsdbgm 83							//'commanderPanelMC'
      &pushsdbgm 87							//'co_commanderNameTF'
      &pushsdb 85							//'text'
      &pushsdb 88							//'$PLAYER_STATUS_1_0'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 56							//'m_initSlot'
    &pushtrue
    &setMember
    &pushregister 1
    &pushsdb 89							//'MAX_MISSIONS'
    &pushbyte 9
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fem_m_coopStaging'
    &pushsdbgm 9							//'prototype'
    &pushbyte 3
    &pushsdb 90							//'ASSetPropFlags'
    &callFunction
   label13:
    &pop
  &end // of initMovieClip 36
&end
