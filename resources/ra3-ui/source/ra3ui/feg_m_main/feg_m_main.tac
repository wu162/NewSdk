movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\feg_m_main\\feg_m_main.eaf' // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px
  
  &exportAssets
    2 &as 'FullScreenButtonGuard'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_main'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 3 // total frames: 0
  &end // of defineMovieClip 3
  
  &exportAssets
    3 &as '__Packages.feg_m_main'
  &end // of exportAssets
  
  &initMovieClip 3
    &constants '_global', 'feg_m_main', 'm_screen', 'm_messageHandler', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'prototype', 'shutdown', 'removeMessageHandler', 'onChildLoad', 'MSGCODE', 'FE_LOAD_SHELL', 'FE_UNLOAD', 'loadContent', 'unloadContent', 'LAYER_BACKGROUNDSCREEN', 'backgroundMC', 'createEmptyMovieClip', 'LAYER_SHELLSCREEN', 'shellScreenMC', 'm_buttonGuard', 'LAYER_BUTTONGUARD', 'transitionButtonGuard', 'FullScreenButtonGuard', 'attachMovie', '_alpha', '_visible', 'contextMenu', 'LAYER_CONTEXTMENU', 'contextMenuMC', 'comLink', 'LAYER_COMLINK', 'comLinkMC', 'OnChildLoaded', 'bind0', 'registerOnLoadCallback', 'SCREEN', 'FEG_BACKGROUND', '.swf', 'loadMovie', 'FEG_CONTEXTMENU', 'hideBackgroundMovie', 'extern', 'InGame', 'FEG_MAIN_MENU', 'NONE', 'initComLink', 'unloadMovie', 'screen', 'changeScreen', 'm_transitionScreenState', 'Start', 'screenTransition', 'doScreenTransition', 'm_transitionScreenIntervalId', 'clearInterval', 'm_transitionFunc', 'setInterval', 'doScreenTransition() - ', 'FadeOut', 'ChangeScreen', 'FadeIn', 'Done', 'FADE_STEP', 'gCT', 'unregisterFromRoot', 'gSM', 'm_file', 'hideForeground', 'showForeground', 'showBackgroundMovie', 'backgroundUnload', 'onUnload', 'm_callbackCount', '_x', '_y', 'FEG_COMLINK', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_main'
    &not
    &not
    &jnz label22    
    &pushglobalgv
    &pushsdb 1							//'feg_m_main'
    &function2  (r:3='screen') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 2							//'m_screen'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 3							//'m_messageHandler'
      &pushregister 1
      &pushsdbgm 4							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 3							//'m_messageHandler'
      &pushone
      &pushsdbgv 6							//'gMH'
      &dcallmp 7							// addMessageHandler()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushregister 1
    &pushsdbgm 8							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 9							//'shutdown'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 3							//'m_messageHandler'
      &pushone
      &pushsdbgv 6							//'gMH'
      &dcallmp 10							// removeMessageHandler()
      &pushregister 1
      &pushsdb 3							//'m_messageHandler'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdb 11							//'onChildLoad'
      &delete
      &pop
      &pushregister 2
      &pushsdb 1							//'feg_m_main'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 4							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushtrue
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 12							//'MSGCODE'
      &pushsdbgm 13							//'FE_LOAD_SHELL'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 12							//'MSGCODE'
      &pushsdbgm 14							//'FE_UNLOAD'
      &strictEquals
      &jnz label2      
      &jmp label3      
     label1:
      &pushzero
      &pushregister 1
      &dcallmp 15							// loadContent()
      &jmp label4      
     label2:
      &pushzero
      &pushregister 1
      &dcallmp 16							// unloadContent()
      &jmp label4      
     label3:
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label4      
     label4:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 15							//'loadContent'
    &function2  () (r:1='this', r:2='_global')
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 17							//'LAYER_BACKGROUNDSCREEN'
      &pushsdb 18							//'backgroundMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &dcallmp 19							// createEmptyMovieClip()
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 20							//'LAYER_SHELLSCREEN'
      &pushsdb 21							//'shellScreenMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &dcallmp 19							// createEmptyMovieClip()
      &pushregister 1
      &pushsdb 22							//'m_buttonGuard'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 23							//'LAYER_BUTTONGUARD'
      &pushsdb 24							//'transitionButtonGuard'
      &pushsdb 25							//'FullScreenButtonGuard'
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdb 26							//'attachMovie'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 27							//'_alpha'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
      &pushregister 2
      &pushsdb 29							//'contextMenu'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 30							//'LAYER_CONTEXTMENU'
      &pushsdb 31							//'contextMenuMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdb 19							//'createEmptyMovieClip'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdb 32							//'comLink'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 33							//'LAYER_COMLINK'
      &pushsdb 34							//'comLinkMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdb 19							//'createEmptyMovieClip'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdb 11							//'onChildLoad'
      &pushregister 1
      &pushsdbgm 35							//'OnChildLoaded'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 36							//'bind0'
      &callMethod
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 37							// registerOnLoadCallback()
      &pushregister 2
      &pushsdbgm 38							//'SCREEN'
      &pushsdbgm 39							//'FEG_BACKGROUND'
      &pushsdb 40							//'.swf'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &dcallmp 41							// loadMovie()
      &pushregister 2
      &pushsdbgm 38							//'SCREEN'
      &pushsdbgm 42							//'FEG_CONTEXTMENU'
      &pushsdb 40							//'.swf'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 31							//'contextMenuMC'
      &dcallmp 41							// loadMovie()
      &pushzero
      &pushregister 1
      &dcallmp 43							// hideBackgroundMovie()
      &pushsdbgv 44							//'extern'
      &pushsdbgm 45							//'InGame'
      &not
      &not
      &jnz label5      
      &pushregister 2
      &pushsdbgm 38							//'SCREEN'
      &pushsdbgm 46							//'FEG_MAIN_MENU'
      &pushsdb 40							//'.swf'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 41							// loadMovie()
      &jmp label6      
     label5:
      &pushregister 2
      &pushsdbgm 38							//'SCREEN'
      &pushsdbgm 47							//'NONE'
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 41							// loadMovie()
     label6:
      &pushzero
      &pushregister 1
      &dcallmp 48							// initComLink()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 16							//'unloadContent'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &dcallmp 49							// unloadMovie()
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 49							// unloadMovie()
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 34							//'comLinkMC'
      &pushsdbgm 50							//'screen'
      &dcallmp 9							// shutdown()
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 34							//'comLinkMC'
      &dcallmp 49							// unloadMovie()
      &pushzero
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &dcallmp 49							// unloadMovie()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 51							//'changeScreen'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 52							//'m_transitionScreenState'
      &pushsdb 53							//'Start'
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 54							// screenTransition()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 54							//'screenTransition'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushregister 1
      &pushsdb 55							//'doScreenTransition'
      &callMethod
      &not
      &jnz label8      
      &pushregister 1
      &pushsdbgm 56							//'m_transitionScreenIntervalId'
      &pushbyte -1
      &equals
      &not
      &not
      &jnz label7      
      &pushregister 1
      &pushsdbgm 56							//'m_transitionScreenIntervalId'
      &pushone
      &dcallfp 57							// clearInterval()
      &pushregister 1
      &pushsdb 56							//'m_transitionScreenIntervalId'
      &pushbyte -1
      &setMember
      &pushregister 1
      &pushsdb 58							//'m_transitionFunc'
      &delete
      &pop
     label7:
      &jmp label9      
     label8:
      &pushregister 1
      &pushsdbgm 56							//'m_transitionScreenIntervalId'
      &pushbyte -1
      &equals
      &not
      &jnz label9      
      &pushregister 1
      &pushsdb 58							//'m_transitionFunc'
      &pushregister 1
      &pushsdbgm 54							//'screenTransition'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 36							//'bind0'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 56							//'m_transitionScreenIntervalId'
      &pushbyte 30
      &pushregister 1
      &pushsdbgm 58							//'m_transitionFunc'
      &pushbyte 2
      &pushsdb 59							//'setInterval'
      &callFunction
      &setMember
     label9:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 55							//'doScreenTransition'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 60							//'doScreenTransition() - '
      &pushregister 1
      &pushsdbgm 52							//'m_transitionScreenState'
      &add
      &trace
      &pushregister 1
      &pushsdbgm 52							//'m_transitionScreenState'
      &setRegister r:0
      &pushsdb 53							//'Start'
      &strictEquals
      &jnz label10      
      &pushregister 0
      &pushsdb 61							//'FadeOut'
      &strictEquals
      &jnz label12      
      &pushregister 0
      &pushsdb 62							//'ChangeScreen'
      &strictEquals
      &jnz label15      
      &pushregister 0
      &pushsdb 63							//'FadeIn'
      &strictEquals
      &jnz label17      
      &pushregister 0
      &pushsdb 64							//'Done'
      &strictEquals
      &jnz label19      
      &jmp label20      
     label10:
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushundef
      &equals
      &not
      &jnz label11      
      &pushregister 1
      &pushsdb 52							//'m_transitionScreenState'
      &pushsdb 62							//'ChangeScreen'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdb 55							//'doScreenTransition'
      &callMethod
      &return
     label11:
      &pushregister 1
      &pushsdb 52							//'m_transitionScreenState'
      &pushsdb 61							//'FadeOut'
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 65							//'FADE_STEP'
      &subtract
      &setMember
      &jmp label20      
     label12:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 65							//'FADE_STEP'
      &greaterThan
      &not
      &not
      &jnz label13      
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 52							//'m_transitionScreenState'
      &pushsdb 62							//'ChangeScreen'
      &setMember
      &jmp label14      
     label13:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 65							//'FADE_STEP'
      &subtract
      &setMember
     label14:
      &jmp label20      
     label15:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushundef
      &equals
      &not
      &not
      &jnz label16      
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushone
      &pushregister 2
      &pushsdbgm 66							//'gCT'
      &dcallmp 67							// unregisterFromRoot()
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 49							// unloadMovie()
     label16:
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 20							//'LAYER_SHELLSCREEN'
      &pushsdb 21							//'shellScreenMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &dcallmp 19							// createEmptyMovieClip()
      &pushregister 2
      &pushsdbgm 68							//'gSM'
      &pushsdbgm 69							//'m_file'
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 41							// loadMovie()
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 52							//'m_transitionScreenState'
      &pushsdb 63							//'FadeIn'
      &setMember
      &jmp label20      
     label17:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 65							//'FADE_STEP'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushbyte 100
      &lessThan
      &not
      &not
      &jnz label18      
      &pushregister 1
      &pushsdb 52							//'m_transitionScreenState'
      &pushsdb 64							//'Done'
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushbyte 100
      &setMember
     label18:
      &jmp label20      
     label19:
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
      &pushtrue
      &return
     label20:
      &pushfalse
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 70							//'hideForeground'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 71							//'showForeground'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 43							//'hideBackgroundMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 72							//'showBackgroundMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 73							//'backgroundUnload'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &pushsdbgm 50							//'screen'
      &dcallmp 74							// onUnload()
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &dcallmp 49							// unloadMovie()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 37							//'registerOnLoadCallback'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 75							//'m_callbackCount'
      &pushregister 1
      &pushsdbgm 75							//'m_callbackCount'
      &increment
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 35							//'OnChildLoaded'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 75							//'m_callbackCount'
      &pushregister 1
      &pushsdbgm 75							//'m_callbackCount'
      &decrement
      &setMember
      &pushregister 1
      &pushsdbgm 75							//'m_callbackCount'
      &pushzero
      &equals
      &not
      &jnz label21      
      &getURL 'FSCommand:CallGameFunction'       'OnFrameworkInitialized'      
     label21:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 48							//'initComLink'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 34							//'comLinkMC'
      &pushsdb 76							//'_x'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 34							//'comLinkMC'
      &pushsdb 77							//'_y'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 34							//'comLinkMC'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
      &pushregister 2
      &pushsdbgm 38							//'SCREEN'
      &pushsdbgm 78							//'FEG_COMLINK'
      &pushsdb 40							//'.swf'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 34							//'comLinkMC'
      &dcallmp 41							// loadMovie()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 75							//'m_callbackCount'
    &pushzero
    &setMember
    &pushregister 2
    &pushsdb 56							//'m_transitionScreenIntervalId'
    &pushbyte -1
    &setMember
    &pushregister 1
    &pushsdb 23							//'LAYER_BUTTONGUARD'
    &pushbyte 5
    &setMember
    &pushregister 1
    &pushsdb 30							//'LAYER_CONTEXTMENU'
    &pushbyte 4
    &setMember
    &pushregister 1
    &pushsdb 33							//'LAYER_COMLINK'
    &pushbyte 3
    &setMember
    &pushregister 1
    &pushsdb 20							//'LAYER_SHELLSCREEN'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 17							//'LAYER_BACKGROUNDSCREEN'
    &pushone
    &setMember
    &pushregister 1
    &pushsdb 65							//'FADE_STEP'
    &pushbyte 25
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_main'
    &pushsdbgm 8							//'prototype'
    &pushbyte 3
    &pushsdb 79							//'ASSetPropFlags'
    &callFunction
   label22:
    &pop
  &end // of initMovieClip 3
&end
