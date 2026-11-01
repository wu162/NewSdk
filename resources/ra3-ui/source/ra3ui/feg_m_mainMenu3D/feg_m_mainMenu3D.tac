movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\feg_m_mainMenu3D\\feg_m_mainMenu3D.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_mainMenu3D'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'floorAnimationSymbol' &as 1
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'cityscapeMC Symbol' &as 2
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseAccordionButton.swf'
    'std_AccordionMenuSymbol' &as 3
  &end // of importAssets

  &placeMovieClip 3 &as 'newLoadContinueMenuUI'
  
    &onClipEvent &construct
      &constants 'm_refFM', '_global.gFM', 'm_bEnabled', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_menuButtonSymbol', 'menuButtonContentClipSymbol', 'm_menuItemSymbol', 'menuItemContentClipSymbol', 'm_menuDefinitionArray', '$CampaignMenu_NewButton', '$CampaignMenu_LoadButton', '$CampaignMenu_ContinueButton', '$CampaignMenu_ChaptersButton', '$CampaignMenu_BackButton'  
      &pushsdb 0							//'m_refFM'
      &pushsdb 1							//'_global.gFM'
      &setVariable
      &pushsdb 2							//'m_bEnabled'
      &pushfalse
      &setVariable
      &pushsdb 3							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 4							//'m_tabIndex'
      &pushzerosv
      &pushsdb 5							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 6							//'m_menuButtonSymbol'
      &pushsdb 7							//'menuButtonContentClipSymbol'
      &setVariable
      &pushsdb 8							//'m_menuItemSymbol'
      &pushsdb 9							//'menuItemContentClipSymbol'
      &setVariable
      &pushsdb 10							//'m_menuDefinitionArray'
      &pushzero
      &initArray
      &setVariable
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushzero
      &pushsdb 11							//'$CampaignMenu_NewButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushone
      &pushsdb 12							//'$CampaignMenu_LoadButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 2
      &pushsdb 13							//'$CampaignMenu_ContinueButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 3
      &pushsdb 14							//'$CampaignMenu_ChaptersButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 4
      &pushsdb 15							//'$CampaignMenu_BackButton'
      &setMember
    &end
  &end // of placeMovieClip 3

  &placeMovieClip 3 &as 'mainMenuUI'
  
    &onClipEvent &construct
      &constants 'm_refFM', '_global.gFM', 'm_bEnabled', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_menuButtonSymbol', 'menuButtonContentClipSymbol', 'm_menuItemSymbol', 'menuItemContentClipSymbol', 'm_menuDefinitionArray', '$Tutorial_MenuButton', '$Campaign_MenuButton,$CampaignMenu_NewButton,$CampaignMenu_LoadButton,$CampaignMenu_ContinueButton,$CampaignMenu_ChaptersButton', '$Versus_MenuButton,$Skirmish_MenuButton,$Online_MenuButton,$Network_MenuButton', '$Profiles_MenuButton,$Manage_MenuButton,$Stats_MenuButton,$TransmissionLog_MenuButton,$ReplayTheater_MenuButton', '$Options_MenuButton,$Setting_MenuButton,$Credits_MenuButton', '$Exit_MenuButton'  
      &pushsdb 0							//'m_refFM'
      &pushsdb 1							//'_global.gFM'
      &setVariable
      &pushsdb 2							//'m_bEnabled'
      &pushfalse
      &setVariable
      &pushsdb 3							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 4							//'m_tabIndex'
      &pushzerosv
      &pushsdb 5							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 6							//'m_menuButtonSymbol'
      &pushsdb 7							//'menuButtonContentClipSymbol'
      &setVariable
      &pushsdb 8							//'m_menuItemSymbol'
      &pushsdb 9							//'menuItemContentClipSymbol'
      &setVariable
      &pushsdb 10							//'m_menuDefinitionArray'
      &pushzero
      &initArray
      &setVariable
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushzero
      &pushsdb 11							//'$Tutorial_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushone
      &pushsdb 12							//'$Campaign_MenuButton,$CampaignMenu_NewButton,$CampaignMenu_LoadButton,$CampaignMenu_ContinueButton,$CampaignMenu_ChaptersButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 2
      &pushsdb 13							//'$Versus_MenuButton,$Skirmish_MenuButton,$Online_MenuButton,$Network_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 3
      &pushsdb 14							//'$Profiles_MenuButton,$Manage_MenuButton,$Stats_MenuButton,$TransmissionLog_MenuButton,$ReplayTheater_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 4
      &pushsdb 15							//'$Options_MenuButton,$Setting_MenuButton,$Credits_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 5
      &pushsdb 16							//'$Exit_MenuButton'
      &setMember
    &end
  &end // of placeMovieClip 3
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'libertySymbol' &as 4
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'hammer&SickleSymbol' &as 5
  &end // of importAssets

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 11
  &end // of importAssets

  &defineMovieClip 14 // total frames: 1

    &placeMovieClip 10 &as 'creditsMC'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'CreditsImage'
      &end
    &end // of placeMovieClip 10

    &placeMovieClip 11 &as 'backButton'
    
      &onClipEvent &construct
        &pushs 'm_width'
        &pushshort 166
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
        &pushzerosv
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 11
  &end // of defineMovieClip 14

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 27 // total frames: 120
  &end // of defineMovieClip 27
  
  &importAssets &from '../cafe/mouseComponents/std_MouseCheckBox.swf'
    'std_MouseCheckBoxSymbol' &as 32
  &end // of importAssets

  &defineMovieClip 35 // total frames: 1

    &placeMovieClip 11 &as 'buttonOK'
    
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
        &pushssv '$GUI:OK'
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
    &end // of placeMovieClip 11

    &placeMovieClip 32 &as 'hideForeverCheckBox'
    
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
        &pushssv '$GUI:HIDE_FOREVER'
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
        &pushssv 'checkBoxContentClipSymbol'
      &end
    &end // of placeMovieClip 32
  &end // of defineMovieClip 35
  
  &importAssets &from './screens/fe_m_assets.swf'
    'newProfileMC' &as 36
  &end // of importAssets

  &defineMovieClip 37 // total frames: 0
  &end // of defineMovieClip 37
  
  &exportAssets
    37 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 37
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
  &end // of initMovieClip 37

  &defineMovieClip 38 // total frames: 0
  &end // of defineMovieClip 38
  
  &exportAssets
    38 &as '__Packages.feg_m_mainMenu3D'
  &end // of exportAssets
  
  &initMovieClip 38
    &constants '_global', 'feg_m_mainMenu3D', 'Cafe2_BaseUIScreen', 'm_thisClass', 'GAME', 'SAVELOADSCREEN_TYPE', 'ENUM', 'SAVELOADSCREEN_LOAD', 'MODE', 'GAME_MODE_SHELL', 'COOPMODE', 'GAME_COOPMODE_AI', 'm_screen', 'newProfileMC', '_visible', 'comLinkPrompt', 'creditMC', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'SOUND', 'SHELL_MAIN', 'PlayShellMusic', 'prototype', 'frameDelayedInit', 'initGUI', 'startIntro', 'loadFloorMovie', 'm_floorMC', 'floorMC', '_xrotation', 'FLOOR_XROT', '_yrotation', 'FLOOR_YROT', '_x', 'FLOOR_X', '_y', 'FLOOR_Y', 'initFloorParameters', 'm_floorUI', 'floorUI', 'FLOOR_SCALE', 'UI_Y_ORIGIN', 'UI_POS_PADDING', 'UI_X_ORIGIN', 'FLOOR_UI_NAME', 'init', 'loadWallMovie', 'm_wallMC', 'wallMC', 'WALL_XROT', 'WALL_YROT', 'WALL_X', 'WALL_Y', '_z', 'WALL_Z', 'initWallParameters', 'm_wallUI', 'wallUI', 'WALL_SCALE', 'WALL_UI_NAME', 'loadMainMenu', 'm_mainMenuMC', 'mainMenuUI', 'processUserInputMainMenu', 'bind2DynamicParams', 'setOnClickCallBack', 'feg_m_mainMenu3d::loadMainMenu() RA3PublicBeta: ', 'extern', 'RA3PublicBeta', '1', 'Array', '$Versus_MenuButton,$Online_MenuButton', '$Options_MenuButton,$Setting_MenuButton', '$Exit_MenuButton', 'setMenuDefinitionParam', 'onBetaMenuClicked', 'show', 'MENU_XROT', 'MENU_YROT', 'MENU_X', 'MENU_Y', 'MENU_Z', 'loadNewLoadContinueMenu', 'm_NLCMenuMC', 'newLoadContinueMenuUI', 'processUserInputNLC', 'NLC_XROT', 'NLC_YROT', 'NLC_X', 'NLC_Y', 'NLC_Z', 'MENU_ITEM_1', 'MENU_ITEM_2', 'MENU_ITEM_3', 'MENU_ITEM_4', 'MENU_ITEM_5', 'MENU_ITEM_6', 'goToTutorialMenu', 'processCampaignMenuClick', 'processVersusMenuClick', 'processProfileMenuClick', 'processOptionsMenuClick', 'processQuitMenuClick', 'feg_m_mainMenu3D::processCampaignMenuClick() Going to skirmish screen.', 'SCREEN', 'FES_SKIRMISH_SETUP', 'startOuttro', 'FEM_MP_ONLINE_MULTIPLAYER', 'goToNextOnlineScreen', 'feg_m_mainMenu3D::processVersusMenuClick() -> Network (LAN) Selection', 'FEM_LAN_LOBBY', 'm_nextScreen', 'Object', 'QueryGameEngine?ONLINE_SIGNED_IN', 'ONLINE_SIGNED_IN', 'feg_m_mainMenu3D::goToNextOnlineScreen : nextScreen = ', 'GAME_MODE_MULTIPLAYER', 'm_loginPending', 'comLink', 'screen', 'open', 'processNewMenuClick', 'processLoadMenuClick', 'processContinueMenuClick', 'processChaptersMenuClick', 'feg_m_mainMenu3D::processCampaignMenuClick() Level List / Map List', 'FES_SINGLE_PLAYER_MENU', 'feg_m_mainMenu3D::processProfileMenuClick() :: Management', 'FEG_PROFILE_MANAGER', 'feg_m_mainMenu3D::processProfileMenuClick() :: Stats', 'FEG_PROFILE_STATS', 'feg_m_mainMenu3D::processProfileMenuClick() :: Film Archive', 'FEG_FILMARCHIVE', 'feg_m_mainMenu3D::processProfileMenuClick() :: Replay Theater', 'FEG_SAVELOAD', 'feg_m_mainMenu3D::processOptionsMenuClick::Options : Settings', 'FEG_OPTIONS', 'feg_m_mainMenu3D::processOptionsMenuClick::Options : Credits', 'goToCredits', 'processBackMenuClick', 'GAME_MODE_CAMPAIGN', 'FES_FACTION_SELECT_MENU', 'FES_CHAPTER_SELECT', 'transitionToMainMenu', 'showCredits', 'hideMainMenu', 'startCredits', 'goToMainMenu', 'hideCredits', 'showMainMenu', 'stopCredits', 'initMenuParameters', 'm_menuUI', 'menuUI', 'MENU_SCALE', 'MENU_UI_NAME', 'loadLibertyMovie', 'm_LibertyMC', 'libertyMC', 'LIBERTY_XROT', 'LIBERTY_YROT', 'LIBERTY_X', 'LIBERTY_Y', 'LIBERTY_Z', 'initLibertyParameters', 'm_libertyUI', 'libertyUI', 'LIBERTY_SCALE', 'LIBERTY_UI_NAME', 'loadCreditsMovie', 'm_creditMC', 'CREDIT_XROT', 'CREDIT_YROT', 'CREDIT_X', 'CREDIT_Y', 'backButton', 'enable', 'setOnMouseUpFunction', 'push', 'loadHammerNSickleMovie', 'm_HammerSickleMC', 'hammerMC', 'HAMMER_XROT', 'HAMMER_YROT', 'HAMMER_X', 'HAMMER_Y', 'HAMMER_Z', 'initHammerParameters', 'm_hammerSickleUI', 'hammerUI', 'HAMMER_SCALE', 'HAMMER_UI_NAME', 'loadTestControls', 'm_testUIMC', 'UI_ADJUSTMENT_LAYER', 'testControls', 'userInterfaceSymbol', 'attachMovie', 'loadGuide', 'm_guideMC', 'UI_IMAGEGUIDE_LAYER', 'imageGuide', 'image_guideSymbol', '_alpha', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'btnNewConfirm', 'buttonGuard', 'showHideTestUI', 'showHideGuideMC', 'disable', 'initNewProfilePrompt', 'QueryGameEngine?FE_PROFILE_COUNT', 'FE_PROFILE_COUNT', '0', 'close', 'lock', 'newProfileName', 'takeFocus', 'checkForComLinkPrompt', 'registerIntroOuttroComponents', 'FEG_TUTORIALMENU', 'QueryGameEngine?COMLINK_PROMPT_DISMISSED', 'COMLINK_PROMPT_DISMISSED', 'SHELL', 'COMLINK_PROMPT_SEEN', 'checkForNewPatch', 'initComLinkPrompt', 'buttonOK', 'noIntro', 'onComLinkPromptDismissed', 'bind1', 'hideForeverCheckBox', 'alwaysHideComLinkPrompt', 'setOnMouseDownFunction', 'm_frameCount', 'flashIcon', 'onEnterFrame', 'delayedComLinkPrompt', 'onCreateProfile', 'setOnEnterFunction', 'btnNewCancel', 'getContents', 'FSCommand:CallGameFunction', '%AddProfile?ProfileName=', '', 'setContents', 'setIconLit', 'isChecked', '%ToggleComLinkPrompt?DismissBoxChecked=', 'm_isCreditsListScrolling', 'CREDITS', 'removeMessageHandler', 'clearData', 'realizeIconVisualState', 'MSGCODE', 'FE_MP_ONLINE_SIGN_IN_SUCCESS', 'FE_MP_ONLINE_SIGN_IN_FAILURE', 'FE_PROFILE_CREATE_SUCCESS', 'unlock', 'toggleTestUI', 'm_testUIVisible', 'showTestUI', 'hideTestUI', 'm_speedUI', 'toggleGuide', 'initData', 'xPosSliderUI', 'setValue', 'yPosSliderUI', 'zPosSliderUI', 'rotXSliderUI', 'rotYSliderUI', 'update', 'transitionToNLC', 'updateNLCAnimation', 'gotoAndPlay', 'NLC_ANIM_LENGTH', 'lerp', 'SLIDE_DISTANCE_X', 'SLIDE_DISTANCE_Z', 'stop', 'updateMainMenuAnimation', 'FLOOR_Z', 'Wall Adjustment UI', 'Floor Adjustment UI', 'Hammer Adjustment UI', 'Menu Adjustment UI', 'Liberty Adjustment UI', 'MENU_ITEM_7', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu3D'
    &not
    &not
    &jnz label85    
    &pushglobalgv
    &pushsdb 1							//'feg_m_mainMenu3D'
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
      &pushsdb 5							//'SAVELOADSCREEN_TYPE'
      &pushregister 3
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 7							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 3
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 3
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 9							//'GAME_MODE_SHELL'
      &setMember
      &pushregister 3
      &pushsdbgm 4							//'GAME'
      &pushsdb 10							//'COOPMODE'
      &pushregister 3
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 11							//'GAME_COOPMODE_AI'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 15							//'comLinkPrompt'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 16							//'creditMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 17							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 18							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 19							//'gSM'
      &dcallmp 20							// setOnExitScreen()
      &pushregister 3
      &pushsdbgm 21							//'SOUND'
      &pushsdbgm 22							//'SHELL_MAIN'
      &pushone
      &pushregister 3
      &dcallmp 23							// PlayShellMusic()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu3D'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 24							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 25							//'frameDelayedInit'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 26							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 27							// startIntro()
      &getURL 'FSCommand:CallGameFunction'       '%StartRevertResolutionTimer'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'loadFloorMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 29							//'m_floorMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 30							//'floorMC'
      &setMember
      &pushregister 1
      &pushsdbgm 29							//'m_floorMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 29							//'m_floorMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 29							//'m_floorMC'
      &pushsdb 31							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 32							//'FLOOR_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 29							//'m_floorMC'
      &pushsdb 33							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 34							//'FLOOR_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 29							//'m_floorMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 36							//'FLOOR_X'
      &setMember
      &pushregister 1
      &pushsdbgm 29							//'m_floorMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 38							//'FLOOR_Y'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 39							//'initFloorParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 40							//'m_floorUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 41							//'floorUI'
      &setMember
      &pushsdbgv 42							//'FLOOR_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 43							//'UI_Y_ORIGIN'
      &pushshort 231
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 44							//'UI_POS_PADDING'
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_X_ORIGIN'
      &pushregister 1
      &pushsdbgm 29							//'m_floorMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 46							//'FLOOR_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 40							//'m_floorUI'
      &dcallmp 47							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 48							//'loadWallMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 49							//'m_wallMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 50							//'wallMC'
      &setMember
      &pushregister 1
      &pushsdbgm 49							//'m_wallMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 49							//'m_wallMC'
      &pushsdb 31							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 51							//'WALL_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 49							//'m_wallMC'
      &pushsdb 33							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 52							//'WALL_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 49							//'m_wallMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 53							//'WALL_X'
      &setMember
      &pushregister 1
      &pushsdbgm 49							//'m_wallMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 54							//'WALL_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 49							//'m_wallMC'
      &pushsdb 55							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 56							//'WALL_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 57							//'initWallParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 58							//'m_wallUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 59							//'wallUI'
      &setMember
      &pushsdbgv 60							//'WALL_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 43							//'UI_Y_ORIGIN'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_X_ORIGIN'
      &pushregister 1
      &pushsdbgm 49							//'m_wallMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 61							//'WALL_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 58							//'m_wallUI'
      &dcallmp 47							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 62							//'loadMainMenu'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 63							//'m_mainMenuMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 64							//'mainMenuUI'
      &setMember
      &pushregister 1
      &pushsdbgm 65							//'processUserInputMainMenu'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 66							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &dcallmp 67							// setOnClickCallBack()
      &pushsdb 68							//'feg_m_mainMenu3d::loadMainMenu() RA3PublicBeta: '
      &pushsdbgv 69							//'extern'
      &pushsdbgm 70							//'RA3PublicBeta'
      &add
      &trace
      &pushsdbgv 69							//'extern'
      &pushsdbgm 70							//'RA3PublicBeta'
      &pushsdb 71							//'1'
      &equals
      &not
      &jnz label1      
      &pushzero
      &pushsdb 72							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &pushsdb 73							//'$Versus_MenuButton,$Online_MenuButton'
      &setMember
      &pushregister 3
      &pushone
      &pushsdb 74							//'$Options_MenuButton,$Setting_MenuButton'
      &setMember
      &pushregister 3
      &pushbyte 2
      &pushsdb 75							//'$Exit_MenuButton'
      &setMember
      &pushregister 3
      &pushone
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &dcallmp 76							// setMenuDefinitionParam()
      &pushregister 1
      &pushsdbgm 77							//'onBetaMenuClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 66							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &dcallmp 67							// setOnClickCallBack()
     label1:
      &pushzero
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &dcallmp 47							// init()
      &pushzero
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &dcallmp 78							// show()
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 31							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 79							//'MENU_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 33							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 80							//'MENU_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_X'
      &setMember
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 82							//'MENU_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 55							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 84							//'loadNewLoadContinueMenu'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 85							//'m_NLCMenuMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 86							//'newLoadContinueMenuUI'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &dcallmp 47							// init()
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 87							//'processUserInputNLC'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 66							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &dcallmp 67							// setOnClickCallBack()
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdb 31							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 88							//'NLC_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdb 33							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 89							//'NLC_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 90							//'NLC_X'
      &setMember
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 91							//'NLC_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdb 55							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 92							//'NLC_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 65							//'processUserInputMainMenu'
    &function2  (r:3='menuID', r:2='itemID') (r:1='this')
      &pushregister 3
      &setRegister r:0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 93							//'MENU_ITEM_1'
      &strictEquals
      &jnz label2      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 94							//'MENU_ITEM_2'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 95							//'MENU_ITEM_3'
      &strictEquals
      &jnz label4      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 96							//'MENU_ITEM_4'
      &strictEquals
      &jnz label5      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 97							//'MENU_ITEM_5'
      &strictEquals
      &jnz label6      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 98							//'MENU_ITEM_6'
      &strictEquals
      &jnz label7      
      &jmp label8      
     label2:
      &pushzero
      &pushregister 1
      &dcallmp 99							// goToTutorialMenu()
      &jmp label8      
     label3:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 100							// processCampaignMenuClick()
      &jmp label8      
     label4:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 101							// processVersusMenuClick()
      &jmp label8      
     label5:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 102							// processProfileMenuClick()
      &jmp label8      
     label6:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 103							// processOptionsMenuClick()
      &jmp label8      
     label7:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 104							// processQuitMenuClick()
      &jmp label8      
     label8:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 101							//'processVersusMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &setRegister r:0
      &pushone
      &strictEquals
      &jnz label9      
      &pushregister 0
      &pushbyte 2
      &strictEquals
      &jnz label10      
      &pushregister 0
      &pushbyte 3
      &strictEquals
      &jnz label11      
      &jmp label12      
     label9:
      &pushsdb 105							//'feg_m_mainMenu3D::processCampaignMenuClick() Going to skirmish screen.'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 107							//'FES_SKIRMISH_SETUP'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label13      
     label10:
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 109							//'FEM_MP_ONLINE_MULTIPLAYER'
      &pushone
      &pushregister 1
      &dcallmp 110							// goToNextOnlineScreen()
      &jmp label13      
     label11:
      &pushsdb 111							//'feg_m_mainMenu3D::processVersusMenuClick() -> Network (LAN) Selection'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 112							//'FEM_LAN_LOBBY'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label13      
     label12:
      &jmp label13      
     label13:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 110							//'goToNextOnlineScreen'
    &function2  (r:4='nextScreen') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 113							//'m_nextScreen'
      &pushregister 4
      &setMember
      &pushzero
      &pushsdb 114							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 115							//'QueryGameEngine?ONLINE_SIGNED_IN'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 116							//'ONLINE_SIGNED_IN'
      &toNumber
      &pushone
      &equals
      &not
      &jnz label14      
      &pushsdb 117							//'feg_m_mainMenu3D::goToNextOnlineScreen : nextScreen = '
      &pushregister 4
      &add
      &trace
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 118							//'GAME_MODE_MULTIPLAYER'
      &setMember
      &pushregister 4
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label15      
     label14:
      &pushregister 1
      &pushsdb 119							//'m_loginPending'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &dcallmp 122							// open()
     label15:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 100							//'processCampaignMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &setRegister r:0
      &pushone
      &strictEquals
      &jnz label16      
      &pushregister 0
      &pushbyte 2
      &strictEquals
      &jnz label17      
      &pushregister 0
      &pushbyte 3
      &strictEquals
      &jnz label18      
      &pushregister 0
      &pushbyte 4
      &strictEquals
      &jnz label19      
      &pushregister 0
      &pushbyte 5
      &strictEquals
      &jnz label20      
      &jmp label21      
     label16:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 123							// processNewMenuClick()
      &jmp label22      
     label17:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 124							// processLoadMenuClick()
      &jmp label22      
     label18:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 125							// processContinueMenuClick()
      &jmp label22      
     label19:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 126							// processChaptersMenuClick()
      &jmp label22      
     label20:
      &pushsdb 127							//'feg_m_mainMenu3D::processCampaignMenuClick() Level List / Map List'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 128							//'FES_SINGLE_PLAYER_MENU'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label22      
     label21:
      &jmp label22      
     label22:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 102							//'processProfileMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &setRegister r:0
      &pushone
      &strictEquals
      &jnz label23      
      &pushregister 0
      &pushbyte 2
      &strictEquals
      &jnz label24      
      &pushregister 0
      &pushbyte 3
      &strictEquals
      &jnz label25      
      &pushregister 0
      &pushbyte 4
      &strictEquals
      &jnz label26      
      &jmp label27      
     label23:
      &pushsdb 129							//'feg_m_mainMenu3D::processProfileMenuClick() :: Management'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 130							//'FEG_PROFILE_MANAGER'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label28      
     label24:
      &pushsdb 131							//'feg_m_mainMenu3D::processProfileMenuClick() :: Stats'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 132							//'FEG_PROFILE_STATS'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label28      
     label25:
      &pushsdb 133							//'feg_m_mainMenu3D::processProfileMenuClick() :: Film Archive'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 134							//'FEG_FILMARCHIVE'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &dcallmp 108							// startOuttro()
      &jmp label28      
     label26:
      &pushsdb 135							//'feg_m_mainMenu3D::processProfileMenuClick() :: Replay Theater'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 136							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label28      
     label27:
      &jmp label28      
     label28:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 103							//'processOptionsMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &setRegister r:0
      &pushone
      &strictEquals
      &jnz label29      
      &pushregister 0
      &pushbyte 2
      &strictEquals
      &jnz label30      
      &jmp label31      
     label29:
      &pushsdb 137							//'feg_m_mainMenu3D::processOptionsMenuClick::Options : Settings'
      &trace
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 138							//'FEG_OPTIONS'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label32      
     label30:
      &pushsdb 139							//'feg_m_mainMenu3D::processOptionsMenuClick::Options : Credits'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 140							// goToCredits()
      &jmp label32      
     label31:
      &jmp label32      
     label32:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 104							//'processQuitMenuClick'
    &function2  (r:1='menuItem') ()
      &pushregister 1
      &pushbyte -1
      &strictEquals
      &jnz label33      
      &jmp label34      
     label33:
      &getURL 'FSCommand:CallGameFunction'       '%ExitApplication'      
      &jmp label35      
     label34:
      &jmp label35      
     label35:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 87							//'processUserInputNLC'
    &function2  (r:3='menuID', r:2='itemID') (r:1='this')
      &pushregister 3
      &setRegister r:0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 93							//'MENU_ITEM_1'
      &strictEquals
      &jnz label36      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 94							//'MENU_ITEM_2'
      &strictEquals
      &jnz label37      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 95							//'MENU_ITEM_3'
      &strictEquals
      &jnz label38      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 96							//'MENU_ITEM_4'
      &strictEquals
      &jnz label39      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 97							//'MENU_ITEM_5'
      &strictEquals
      &jnz label40      
      &jmp label41      
     label36:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 123							// processNewMenuClick()
      &jmp label41      
     label37:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 124							// processLoadMenuClick()
      &jmp label41      
     label38:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 125							// processContinueMenuClick()
      &jmp label41      
     label39:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 126							// processChaptersMenuClick()
      &jmp label41      
     label40:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 141							// processBackMenuClick()
      &jmp label41      
     label41:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 123							//'processNewMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &pushbyte -1
      &strictEquals
      &jnz label42      
      &jmp label43      
     label42:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 142							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 143							//'FES_FACTION_SELECT_MENU'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label44      
     label43:
      &jmp label44      
     label44:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 124							//'processLoadMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &pushbyte -1
      &strictEquals
      &jnz label45      
      &jmp label46      
     label45:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 142							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 5							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 7							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 136							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label47      
     label46:
      &jmp label47      
     label47:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 125							//'processContinueMenuClick'
    &function2  (r:2='menuItem') (r:1='_global')
      &pushregister 2
      &pushbyte -1
      &strictEquals
      &jnz label48      
      &jmp label49      
     label48:
      &pushregister 1
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 1
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 142							//'GAME_MODE_CAMPAIGN'
      &setMember
      &getURL 'FSCommand:CallGameFunction'       'LoadMostRecentCampaignSave'      
      &jmp label50      
     label49:
      &jmp label50      
     label50:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 126							//'processChaptersMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &pushbyte -1
      &strictEquals
      &jnz label51      
      &jmp label52      
     label51:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 142							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 144							//'FES_CHAPTER_SELECT'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &jmp label53      
     label52:
      &jmp label53      
     label53:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 141							//'processBackMenuClick'
    &function2  (r:2='menuItem') (r:1='this')
      &pushregister 2
      &pushbyte -1
      &strictEquals
      &jnz label54      
      &jmp label55      
     label54:
      &pushzero
      &pushregister 1
      &dcallmp 145							// transitionToMainMenu()
      &jmp label56      
     label55:
      &jmp label56      
     label56:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 77							//'onBetaMenuClicked'
    &function2  (r:3='menuID', r:2='itemID') (r:1='this')
      &pushregister 3
      &setRegister r:0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 93							//'MENU_ITEM_1'
      &strictEquals
      &jnz label57      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 94							//'MENU_ITEM_2'
      &strictEquals
      &jnz label58      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 95							//'MENU_ITEM_3'
      &strictEquals
      &jnz label59      
      &jmp label60      
     label57:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 101							// processVersusMenuClick()
      &jmp label60      
     label58:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 103							// processOptionsMenuClick()
      &jmp label60      
     label59:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 104							// processQuitMenuClick()
      &jmp label60      
     label60:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 140							//'goToCredits'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 146							// showCredits()
      &pushzero
      &pushregister 1
      &dcallmp 147							// hideMainMenu()
      &pushzero
      &pushregister 1
      &dcallmp 148							// startCredits()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 149							//'goToMainMenu'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 150							// hideCredits()
      &pushzero
      &pushregister 1
      &dcallmp 151							// showMainMenu()
      &pushzero
      &pushregister 1
      &dcallmp 152							// stopCredits()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 153							//'initMenuParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 154							//'m_menuUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 155							//'menuUI'
      &setMember
      &pushsdbgv 156							//'MENU_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 43							//'UI_Y_ORIGIN'
      &pushshort 231
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 44							//'UI_POS_PADDING'
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_X_ORIGIN'
      &pushshort 501
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 44							//'UI_POS_PADDING'
      &add
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 157							//'MENU_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 154							//'m_menuUI'
      &dcallmp 47							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 158							//'loadLibertyMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 159							//'m_LibertyMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 160							//'libertyMC'
      &setMember
      &pushregister 1
      &pushsdbgm 159							//'m_LibertyMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 159							//'m_LibertyMC'
      &pushsdb 31							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 161							//'LIBERTY_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 159							//'m_LibertyMC'
      &pushsdb 33							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 162							//'LIBERTY_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 159							//'m_LibertyMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 163							//'LIBERTY_X'
      &setMember
      &pushregister 1
      &pushsdbgm 159							//'m_LibertyMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 164							//'LIBERTY_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 159							//'m_LibertyMC'
      &pushsdb 55							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 165							//'LIBERTY_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 166							//'initLibertyParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 167							//'m_libertyUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 168							//'libertyUI'
      &setMember
      &pushsdbgv 169							//'LIBERTY_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 43							//'UI_Y_ORIGIN'
      &pushshort 462
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 44							//'UI_POS_PADDING'
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_X_ORIGIN'
      &pushshort 501
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 44							//'UI_POS_PADDING'
      &add
      &pushregister 1
      &pushsdbgm 159							//'m_LibertyMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 170							//'LIBERTY_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 167							//'m_libertyUI'
      &dcallmp 47							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 171							//'loadCreditsMovie'
    &function2  (r:4='buttons') (r:1='this', r:2='_global')
      &pushsdb 172							//'m_creditMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 16							//'creditMC'
      &setVariable
      &pushsdbgv 172							//'m_creditMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 172							//'m_creditMC'
      &pushsdb 31							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 173							//'CREDIT_XROT'
      &setMember
      &pushsdbgv 172							//'m_creditMC'
      &pushsdb 33							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 174							//'CREDIT_YROT'
      &setMember
      &pushsdbgv 172							//'m_creditMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 175							//'CREDIT_X'
      &setMember
      &pushsdbgv 172							//'m_creditMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 176							//'CREDIT_Y'
      &setMember
      &pushsdbgv 172							//'m_creditMC'
      &pushsdbgm 177							//'backButton'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 3
      &dcallmp 178							// enable()
      &pushzero
      &pushregister 3
      &dcallmp 78							// show()
      &pushregister 1
      &pushsdbgm 149							//'goToMainMenu'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 18							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 179							// setOnMouseUpFunction()
      &pushregister 3
      &pushone
      &pushregister 4
      &dcallmp 180							// push()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 181							//'loadHammerNSickleMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 182							//'m_HammerSickleMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 183							//'hammerMC'
      &setMember
      &pushregister 1
      &pushsdbgm 182							//'m_HammerSickleMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 182							//'m_HammerSickleMC'
      &pushsdb 31							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 184							//'HAMMER_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 182							//'m_HammerSickleMC'
      &pushsdb 33							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 185							//'HAMMER_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 182							//'m_HammerSickleMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 186							//'HAMMER_X'
      &setMember
      &pushregister 1
      &pushsdbgm 182							//'m_HammerSickleMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 187							//'HAMMER_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 182							//'m_HammerSickleMC'
      &pushsdb 55							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 188							//'HAMMER_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 189							//'initHammerParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 190							//'m_hammerSickleUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 191							//'hammerUI'
      &setMember
      &pushsdbgv 192							//'HAMMER_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 43							//'UI_Y_ORIGIN'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_X_ORIGIN'
      &pushshort 501
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 44							//'UI_POS_PADDING'
      &add
      &pushregister 1
      &pushsdbgm 182							//'m_HammerSickleMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 193							//'HAMMER_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 190							//'m_hammerSickleUI'
      &dcallmp 47							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 194							//'loadTestControls'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 195							//'m_testUIMC'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 196							//'UI_ADJUSTMENT_LAYER'
      &pushsdb 197							//'testControls'
      &pushsdb 198							//'userInterfaceSymbol'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdb 199							//'attachMovie'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 195							//'m_testUIMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 200							//'loadGuide'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 201							//'m_guideMC'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 202							//'UI_IMAGEGUIDE_LAYER'
      &pushsdb 203							//'imageGuide'
      &pushsdb 204							//'image_guideSymbol'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdb 199							//'attachMovie'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 201							//'m_guideMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 201							//'m_guideMC'
      &pushsdb 205							//'_alpha'
      &pushbyte 50
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 206							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 207							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &dcallmsv 208							// bind1DynamicParams()
      &pushsdbgv 206							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 209							//'gMH'
      &dcallmp 210							// addMessageHandler()
      &pushzero
      &pushsdb 72							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 211							//'btnNewConfirm'
      &pushone
      &pushregister 3
      &dcallmp 180							// push()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 212							//'buttonGuard'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 48							// loadWallMovie()
      &pushzero
      &pushregister 1
      &dcallmp 28							// loadFloorMovie()
      &pushzero
      &pushregister 1
      &dcallmp 158							// loadLibertyMovie()
      &pushzero
      &pushregister 1
      &dcallmp 181							// loadHammerNSickleMovie()
      &pushzero
      &pushregister 1
      &dcallmp 62							// loadMainMenu()
      &pushzero
      &pushregister 1
      &dcallmp 84							// loadNewLoadContinueMenu()
      &pushregister 3
      &pushone
      &pushregister 1
      &dcallmp 171							// loadCreditsMovie()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 213							//'showHideTestUI'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_X_ORIGIN'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 213							//'showHideTestUI'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 43							//'UI_Y_ORIGIN'
      &pushshort 580
      &add
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 214							//'showHideGuideMC'
      &pushsdb 35							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_X_ORIGIN'
      &pushsdbgv 213							//'showHideTestUI'
      &pushsdbgm 35							//'_x'
      &add
      &pushbyte 10
      &add
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 214							//'showHideGuideMC'
      &pushsdb 37							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 43							//'UI_Y_ORIGIN'
      &pushshort 580
      &add
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &dcallmp 215							// disable()
      &pushzero
      &pushregister 1
      &dcallmp 150							// hideCredits()
      &pushzero
      &pushregister 1
      &dcallmp 216							// initNewProfilePrompt()
      &pushzero
      &pushsdb 114							//'Object'
      &new
      &setRegister r:4
      &pop
      &pushsdb 217							//'QueryGameEngine?FE_PROFILE_COUNT'
      &pushregister 4
      &loadVariables
      &pushregister 4
      &pushsdbgm 218							//'FE_PROFILE_COUNT'
      &pushsdb 219							//'0'
      &equals
      &dup
      &not
      &jnz label61      
      &pop
      &pushsdbgv 69							//'extern'
      &pushsdbgm 70							//'RA3PublicBeta'
      &pushsdb 219							//'0'
      &equals
     label61:
      &not
      &jnz label62      
      &pushzero
      &pushregister 2
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &dcallmp 220							// close()
      &pushzero
      &pushregister 2
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &dcallmp 221							// lock()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 222							//'newProfileName'
      &dcallmp 223							// takeFocus()
      &jmp label63      
     label62:
      &pushregister 3
      &pushone
      &pushregister 1
      &dcallmp 224							// checkForComLinkPrompt()
     label63:
      &pushregister 3
      &pushone
      &pushregister 1
      &dcallmp 225							// registerIntroOuttroComponents()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 99							//'goToTutorialMenu'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 2
      &pushsdbgm 106							//'SCREEN'
      &pushsdbgm 226							//'FEG_TUTORIALMENU'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 150							//'hideCredits'
    &function  (    )    
      &pushsdbgv 172							//'m_creditMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 146							//'showCredits'
    &function  (    )    
      &pushsdbgv 172							//'m_creditMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 224							//'checkForComLinkPrompt'
    &function2  (r:7='buttons') (r:1='this', r:2='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 15							//'comLinkPrompt'
      &setRegister r:4
      &pop
      &pushregister 4
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushsdb 114							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 227							//'QueryGameEngine?COMLINK_PROMPT_DISMISSED'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 228							//'COMLINK_PROMPT_DISMISSED'
      &pushsdb 71							//'1'
      &equals
      &setRegister r:5
      &pop
      &pushsdbgv 69							//'extern'
      &pushsdbgm 70							//'RA3PublicBeta'
      &pushsdb 71							//'1'
      &equals
      &setRegister r:6
      &pop
      &pushregister 2
      &pushsdbgm 229							//'SHELL'
      &pushsdbgm 230							//'COMLINK_PROMPT_SEEN'
      &dup
      &jnz label64      
      &pop
      &pushregister 5
     label64:
      &dup
      &jnz label65      
      &pop
      &pushregister 6
     label65:
      &not
      &jnz label66      
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 15							//'comLinkPrompt'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 212							//'buttonGuard'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 231							// checkForNewPatch()
      &pushundef
      &return
      &jmp label67      
     label66:
      &pushregister 7
      &pushregister 4
      &pushbyte 2
      &pushregister 1
      &dcallmp 232							// initComLinkPrompt()
     label67:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 232							//'initComLinkPrompt'
    &function2  (r:5='comLinkPrompt', r:6='buttons') (r:1='this', r:2='_global')
      &pushregister 2
      &pushsdbgm 229							//'SHELL'
      &pushsdb 230							//'COMLINK_PROMPT_SEEN'
      &pushtrue
      &setMember
      &pushregister 5
      &pushsdbgm 233							//'buttonOK'
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &dcallmp 234							// noIntro()
      &pushzero
      &pushregister 3
      &dcallmp 178							// enable()
      &pushzero
      &pushregister 3
      &dcallmp 78							// show()
      &pushregister 1
      &pushsdbgm 235							//'onComLinkPromptDismissed'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 236							//'bind1'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 179							// setOnMouseUpFunction()
      &pushregister 3
      &pushone
      &pushregister 6
      &dcallmp 180							// push()
      &pushregister 5
      &pushsdbgm 237							//'hideForeverCheckBox'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 178							// enable()
      &pushzero
      &pushregister 4
      &dcallmp 78							// show()
      &pushregister 4
      &pushregister 1
      &pushsdbgm 238							//'alwaysHideComLinkPrompt'
      &pushregister 1
      &pushbyte 3
      &pushregister 2
      &pushsdb 236							//'bind1'
      &callMethod
      &pushone
      &pushregister 4
      &dcallmp 239							// setOnMouseDownFunction()
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 240							//'m_frameCount'
      &pushzero
      &pushregister 2
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &pushsdb 241							//'flashIcon'
      &callMethod
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdb 242							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 243							//'delayedComLinkPrompt'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 216							//'initNewProfilePrompt'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 244							//'onCreateProfile'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 18							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 211							//'btnNewConfirm'
      &dcallmp 179							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 244							//'onCreateProfile'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 18							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 222							//'newProfileName'
      &dcallmp 245							// setOnEnterFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 211							//'btnNewConfirm'
      &dcallmp 178							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 246							//'btnNewCancel'
      &dcallmp 215							// disable()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 244							//'onCreateProfile'
    &function2  () ()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 222							//'newProfileName'
      &pushsdb 247							//'getContents'
      &callMethod
      &setRegister r:1
      &pop
      &pushsdb 248							//'FSCommand:CallGameFunction'
      &pushsdb 249							//'%AddProfile?ProfileName='
      &pushregister 1
      &add
      &getURL2
      &pushsdb 250							//''
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 222							//'newProfileName'
      &dcallmp 251							// setContents()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 243							//'delayedComLinkPrompt'
    &function2  () (r:1='_global')
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &pushzero
      &greaterThan
      &not
      &jnz label68      
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 240							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &decrement
      &setMember
      &jmp label69      
     label68:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdb 242							//'onEnterFrame'
      &pushnull
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 15							//'comLinkPrompt'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &dcallmp 252							// setIconLit()
     label69:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 231							//'checkForNewPatch'
    &function  (    )    
      &pushsdbgv 69							//'extern'
      &pushsdbgm 70							//'RA3PublicBeta'
      &pushsdb 71							//'1'
      &equals
      &not
      &not
      &jnz label70      
      &getURL 'FSCommand:CallGameFunction'       '%OnlinePatchCheckFromMainMenu'      
     label70:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 238							//'alwaysHideComLinkPrompt'
    &function2  (r:2='hideCheckBox') ()
      &pushzero
      &pushregister 2
      &pushsdb 253							//'isChecked'
      &callMethod
      &jnz label71      
      &pushsdb 219							//'0'
      &jmp label72      
     label71:
      &pushsdb 71							//'1'
     label72:
      &setRegister r:1
      &pop
      &pushsdb 248							//'FSCommand:CallGameFunction'
      &pushsdb 254							//'%ToggleComLinkPrompt?DismissBoxChecked='
      &pushregister 1
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 147							//'hideMainMenu'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 151							//'showMainMenu'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 148							//'startCredits'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 255							//'m_isCreditsListScrolling'
      &not
      &not
      &jnz label73      
      &pushregister 1
      &pushsdb 255							//'m_isCreditsListScrolling'
      &pushtrue
      &setMember
      &pushregister 2
      &pushsdbgm 21							//'SOUND'
      &pushsdw 256							//'CREDITS'
      &getMember
      &pushone
      &pushregister 2
      &dcallmp 23							// PlayShellMusic()
      &getURL 'FSCommand:CallGameFunction'       'Options_StartCredits'      
     label73:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 152							//'stopCredits'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 255							//'m_isCreditsListScrolling'
      &not
      &jnz label74      
      &pushregister 1
      &pushsdb 255							//'m_isCreditsListScrolling'
      &pushfalse
      &setMember
      &pushregister 2
      &pushsdbgm 21							//'SOUND'
      &pushsdbgm 22							//'SHELL_MAIN'
      &pushone
      &pushregister 2
      &dcallmp 23							// PlayShellMusic()
      &getURL 'FSCommand:CallGameFunction'       'Options_StopCredits'      
     label74:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 17							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       'InterruptSilentPatchCheck'      
      &pushzero
      &pushregister 1
      &dcallmp 150							// hideCredits()
      &pushzero
      &pushregister 1
      &dcallmp 152							// stopCredits()
      &pushsdbgv 206							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 209							//'gMH'
      &pushsdw 257							//'removeMessageHandler'
      &callmp
      &pushsdb 206							//'m_localMessageHandlerFunc'
      &delete2
      &pop
      &pushzero
      &pushregister 1
      &pushsdw 258							//'clearData'
      &callmp
      &pushregister 2
      &pushsdb 1							//'feg_m_mainMenu3D'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 235							//'onComLinkPromptDismissed'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushregister 2
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &pushsdw 259							//'realizeIconVisualState'
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 15							//'comLinkPrompt'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 212							//'buttonGuard'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 231							// checkForNewPatch()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 207							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushfalse
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdw 260							//'MSGCODE'
      &getMember
      &pushsdw 261							//'FE_MP_ONLINE_SIGN_IN_SUCCESS'
      &getMember
      &strictEquals
      &jnz label75      
      &pushregister 0
      &pushregister 2
      &pushsdw 260							//'MSGCODE'
      &getMember
      &pushsdw 262							//'FE_MP_ONLINE_SIGN_IN_FAILURE'
      &getMember
      &strictEquals
      &jnz label77      
      &pushregister 0
      &pushregister 2
      &pushsdw 260							//'MSGCODE'
      &getMember
      &pushsdw 263							//'FE_PROFILE_CREATE_SUCCESS'
      &getMember
      &strictEquals
      &jnz label78      
      &jmp label79      
     label75:
      &pushregister 1
      &pushsdbgm 119							//'m_loginPending'
      &not
      &jnz label76      
      &pushregister 1
      &pushsdb 119							//'m_loginPending'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 113							//'m_nextScreen'
      &pushone
      &pushregister 1
      &dcallmp 108							// startOuttro()
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 118							//'GAME_MODE_MULTIPLAYER'
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &dcallmp 220							// close()
     label76:
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label80      
     label77:
      &pushregister 1
      &pushsdb 119							//'m_loginPending'
      &pushfalse
      &setMember
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label80      
     label78:
      &pushzero
      &pushregister 2
      &pushsdbgm 120							//'comLink'
      &pushsdbgm 121							//'screen'
      &pushsdw 264							//'unlock'
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 224							// checkForComLinkPrompt()
      &pushtrue
      &setRegister r:3
      &pop
      &jmp label80      
     label79:
      &jmp label80      
     label80:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 265							//'toggleTestUI'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdw 266							//'m_testUIVisible'
      &pushregister 1
      &pushsdw 266							//'m_testUIVisible'
      &getMember
      &not
      &setMember
      &pushregister 1
      &pushsdw 266							//'m_testUIVisible'
      &getMember
      &not
      &jnz label81      
      &pushzero
      &pushregister 1
      &pushsdw 267							//'showTestUI'
      &callmp
      &jmp label82      
     label81:
      &pushzero
      &pushregister 1
      &pushsdw 268							//'hideTestUI'
      &callmp
     label82:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 268							//'hideTestUI'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 58							//'m_wallUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 40							//'m_floorUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 190							//'m_hammerSickleUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 154							//'m_menuUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 167							//'m_libertyUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdw 269							//'m_speedUI'
      &getMember
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 267							//'showTestUI'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 58							//'m_wallUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 40							//'m_floorUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 190							//'m_hammerSickleUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 154							//'m_menuUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 167							//'m_libertyUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdw 269							//'m_speedUI'
      &getMember
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 270							//'toggleGuide'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 201							//'m_guideMC'
      &pushsdb 14							//'_visible'
      &pushregister 1
      &pushsdbgm 201							//'m_guideMC'
      &pushsdbgm 14							//'_visible'
      &not
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 271							//'initData'
    &function2  ('animationMC', r:1='uiMC', r:6='x', r:5='y', r:4='z', r:3='xrot', r:2='yrot') ()
      &pushregister 6
      &pushone
      &pushregister 1
      &pushsdw 272							//'xPosSliderUI'
      &getMember
      &pushsdw 273							//'setValue'
      &callmp
      &pushregister 5
      &pushone
      &pushregister 1
      &pushsdw 274							//'yPosSliderUI'
      &getMember
      &pushsdw 273							//'setValue'
      &callmp
      &pushregister 4
      &toNumber
      &pushone
      &pushregister 1
      &pushsdw 275							//'zPosSliderUI'
      &getMember
      &pushsdw 273							//'setValue'
      &callmp
      &pushregister 3
      &pushone
      &pushregister 1
      &pushsdw 276							//'rotXSliderUI'
      &getMember
      &pushsdw 273							//'setValue'
      &callmp
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdw 277							//'rotYSliderUI'
      &getMember
      &pushsdw 273							//'setValue'
      &callmp
      &pushzero
      &pushregister 1
      &pushsdw 278							//'update'
      &callmp
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 279							//'transitionToNLC'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 240							//'m_frameCount'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdb 242							//'onEnterFrame'
      &pushregister 1
      &pushsdw 280							//'updateNLCAnimation'
      &getMember
      &setMember
      &pushzero
      &pushone
      &pushregister 1
      &pushsdbgm 85							//'m_NLCMenuMC'
      &pushsdw 281							//'gotoAndPlay'
      &callmp
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 280							//'updateNLCAnimation'
    &function2  () ()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 85							//'m_NLCMenuMC'
      &setRegister r:2
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 63							//'m_mainMenuMC'
      &setRegister r:3
      &pop
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 282							//'NLC_ANIM_LENGTH'
      &getMember
      &divide
      &setRegister r:1
      &pop
      &pushregister 2
      &pushsdb 35							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 90							//'NLC_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_X'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdb 55							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 92							//'NLC_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_Z'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 35							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 284							//'SLIDE_DISTANCE_X'
      &getMember
      &subtract
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 55							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 285							//'SLIDE_DISTANCE_Z'
      &getMember
      &subtract
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 282							//'NLC_ANIM_LENGTH'
      &getMember
      &lessThan
      &not
      &not
      &jnz label83      
      &pushzero
      &pushregister 2
      &pushsdw 286							//'stop'
      &callmp
      &pushregister 2
      &pushsdb 242							//'onEnterFrame'
      &delete
      &pop
     label83:
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 240							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &increment
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 145							//'transitionToMainMenu'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 240							//'m_frameCount'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdb 242							//'onEnterFrame'
      &pushregister 1
      &pushsdw 287							//'updateMainMenuAnimation'
      &getMember
      &setMember
      &pushzero
      &pushone
      &pushregister 1
      &pushsdbgm 63							//'m_mainMenuMC'
      &pushsdw 281							//'gotoAndPlay'
      &callmp
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 287							//'updateMainMenuAnimation'
    &function2  () ()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 85							//'m_NLCMenuMC'
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 63							//'m_mainMenuMC'
      &setRegister r:2
      &pop
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 282							//'NLC_ANIM_LENGTH'
      &getMember
      &divide
      &setRegister r:1
      &pop
      &pushregister 2
      &pushsdb 35							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 284							//'SLIDE_DISTANCE_X'
      &getMember
      &subtract
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_X'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdb 55							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 285							//'SLIDE_DISTANCE_Z'
      &getMember
      &subtract
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_Z'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 35							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 90							//'NLC_X'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 55							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 92							//'NLC_Z'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdw 283							//'lerp'
      &callMethod
      &setMember
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdw 282							//'NLC_ANIM_LENGTH'
      &getMember
      &lessThan
      &not
      &not
      &jnz label84      
      &pushzero
      &pushregister 2
      &pushsdw 286							//'stop'
      &callmp
      &pushregister 2
      &pushsdb 242							//'onEnterFrame'
      &delete
      &pop
     label84:
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 240							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 240							//'m_frameCount'
      &increment
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdw 283							//'lerp'
    &function2  (r:3='num1', r:2='num2', r:1='fraction') ()
      &pushregister 3
      &pushregister 1
      &multiply
      &pushone
      &pushregister 1
      &subtract
      &pushregister 2
      &multiply
      &add
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 119							//'m_loginPending'
    &pushfalse
    &setMember
    &pushregister 1
    &pushsdb 45							//'UI_X_ORIGIN'
    &pushshort 300
    &setMember
    &pushregister 1
    &pushsdb 43							//'UI_Y_ORIGIN'
    &pushbyte 100
    &setMember
    &pushregister 1
    &pushsdb 44							//'UI_POS_PADDING'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 53							//'WALL_X'
    &pushshort 1450
    &setMember
    &pushregister 1
    &pushsdb 54							//'WALL_Y'
    &pushshort 1040
    &setMember
    &pushregister 1
    &pushsdb 56							//'WALL_Z'
    &pushbyte 60
    &setMember
    &pushregister 1
    &pushsdb 51							//'WALL_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 52							//'WALL_YROT'
    &pushshort 328
    &setMember
    &pushregister 1
    &pushsdb 81							//'MENU_X'
    &pushshort 280
    &setMember
    &pushregister 1
    &pushsdb 82							//'MENU_Y'
    &pushshort 160
    &setMember
    &pushregister 1
    &pushsdb 83							//'MENU_Z'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 79							//'MENU_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 80							//'MENU_YROT'
    &pushshort 328
    &setMember
    &pushregister 1
    &pushsdw 284							//'SLIDE_DISTANCE_X'
    &pushshort 1900
    &setMember
    &pushregister 1
    &pushsdw 285							//'SLIDE_DISTANCE_Z'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdw 284							//'SLIDE_DISTANCE_X'
    &getMember
    &pushfloat 0.661006033420563
    &multiply
    &setMember
    &pushregister 1
    &pushsdb 90							//'NLC_X'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 81							//'MENU_X'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdw 284							//'SLIDE_DISTANCE_X'
    &getMember
    &add
    &setMember
    &pushregister 1
    &pushsdb 91							//'NLC_Y'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 82							//'MENU_Y'
    &setMember
    &pushregister 1
    &pushsdb 92							//'NLC_Z'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 83							//'MENU_Z'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdw 285							//'SLIDE_DISTANCE_Z'
    &getMember
    &add
    &setMember
    &pushregister 1
    &pushsdb 88							//'NLC_XROT'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 79							//'MENU_XROT'
    &setMember
    &pushregister 1
    &pushsdb 89							//'NLC_YROT'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 80							//'MENU_YROT'
    &setMember
    &pushregister 1
    &pushsdw 282							//'NLC_ANIM_LENGTH'
    &pushbyte 20
    &setMember
    &pushregister 1
    &pushsdb 36							//'FLOOR_X'
    &pushshort 900
    &setMember
    &pushregister 1
    &pushsdb 38							//'FLOOR_Y'
    &pushshort 610
    &setMember
    &pushregister 1
    &pushsdw 288							//'FLOOR_Z'
    &pushbyte 30
    &setMember
    &pushregister 1
    &pushsdb 32							//'FLOOR_XROT'
    &pushbyte 90
    &setMember
    &pushregister 1
    &pushsdb 34							//'FLOOR_YROT'
    &pushshort 358
    &setMember
    &pushregister 1
    &pushsdb 186							//'HAMMER_X'
    &pushshort 1050
    &setMember
    &pushregister 1
    &pushsdb 187							//'HAMMER_Y'
    &pushshort 570
    &setMember
    &pushregister 1
    &pushsdb 188							//'HAMMER_Z'
    &pushshort 420
    &setMember
    &pushregister 1
    &pushsdb 184							//'HAMMER_XROT'
    &pushshort 352
    &setMember
    &pushregister 1
    &pushsdb 185							//'HAMMER_YROT'
    &pushbyte 37
    &setMember
    &pushregister 1
    &pushsdb 163							//'LIBERTY_X'
    &pushshort 738
    &setMember
    &pushregister 1
    &pushsdb 164							//'LIBERTY_Y'
    &pushbyte 90
    &setMember
    &pushregister 1
    &pushsdb 165							//'LIBERTY_Z'
    &pushshort -230
    &setMember
    &pushregister 1
    &pushsdb 161							//'LIBERTY_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 162							//'LIBERTY_YROT'
    &pushbyte 23
    &setMember
    &pushregister 1
    &pushsdb 175							//'CREDIT_X'
    &pushshort 350
    &setMember
    &pushregister 1
    &pushsdb 176							//'CREDIT_Y'
    &pushshort 200
    &setMember
    &pushregister 1
    &pushsdb 173							//'CREDIT_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 174							//'CREDIT_YROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 196							//'UI_ADJUSTMENT_LAYER'
    &pushshort 150
    &setMember
    &pushregister 1
    &pushsdb 202							//'UI_IMAGEGUIDE_LAYER'
    &pushshort 200
    &setMember
    &pushregister 1
    &pushsdb 61							//'WALL_UI_NAME'
    &pushsdw 289							//'Wall Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 46							//'FLOOR_UI_NAME'
    &pushsdw 290							//'Floor Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 193							//'HAMMER_UI_NAME'
    &pushsdw 291							//'Hammer Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 157							//'MENU_UI_NAME'
    &pushsdw 292							//'Menu Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 170							//'LIBERTY_UI_NAME'
    &pushsdw 293							//'Liberty Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 93							//'MENU_ITEM_1'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 94							//'MENU_ITEM_2'
    &pushone
    &setMember
    &pushregister 1
    &pushsdb 95							//'MENU_ITEM_3'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 96							//'MENU_ITEM_4'
    &pushbyte 3
    &setMember
    &pushregister 1
    &pushsdb 97							//'MENU_ITEM_5'
    &pushbyte 4
    &setMember
    &pushregister 1
    &pushsdb 98							//'MENU_ITEM_6'
    &pushbyte 5
    &setMember
    &pushregister 1
    &pushsdw 294							//'MENU_ITEM_7'
    &pushbyte 6
    &setMember
    &pushregister 2
    &pushsdw 266							//'m_testUIVisible'
    &pushfalse
    &setMember
    &pushregister 1
    &pushsdb 240							//'m_frameCount'
    &pushzero
    &setMember
    &pushregister 2
    &pushsdb 255							//'m_isCreditsListScrolling'
    &pushfalse
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu3D'
    &pushsdbgm 24							//'prototype'
    &pushbyte 3
    &pushsdw 295							//'ASSetPropFlags'
    &callFunction
   label85:
    &pop
  &end // of initMovieClip 38
&end
