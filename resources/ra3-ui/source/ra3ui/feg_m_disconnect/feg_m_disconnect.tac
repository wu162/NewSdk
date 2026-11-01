movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEA0BA~1\\FEG_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &constants 'feg_m_disconnect::onLoad()', 'onEnterFrame', 'frameDelayedInit', 'bind0', 'm_refGlobalEventHandler', 'gEH', 'quitButtonClicked', 'disconnectMC', 'quitButton', 'setOnMouseUpFunction', 'localMessageHandler', 'gMH', 'addMessageHandler', 'enable', 'm_playerClips', 'entry5', 'entry4', 'entry3', 'entry2', 'entry1', 'entry0', 'hideDisconnect', 'feg_m_disconnect::onUnload()', 'removeMessageHandler', 'feg_m_disconnect', 'feg_m_disconnect::localMessageHandler() messageCode = \'', '\'', 'MSGCODE', 'FE_SHOW_MP_DISCONNECT', 'FE_HIDE_MP_DISCONNECT', 'showDisconnect', 'feg_m_disconnect::showDisconnect()', 'getCurrentLocalEventHandler', 'setLocalEventHandler', 'm_eventHandlerIndex', 'm_eventHandlerPriority', 'localEventHandler', 'addPriorityLocalEventHandler', '$APT:DISCONNECTQUITGAME', 'setText', 'show', 'shadeMC', '_visible', 'm_playAudio', 'updateScreen', 'feg_m_disconnect::hideDisconnect()', 'removeLocalEventHandler', 'm_FM', 'handleDefaultInputs', 'Object', 'QueryGameEngine?DISCONNECT_INFO', ',', 'DISCONNECT_INFO_NAMES', 'split', 'DISCONNECT_INFO_TIMES', 'length', 'playerTF', 'text', 'timeTF', 'feg_m_disconnect loading now', 'PADDING', 'm_messageCallback', 'OVERLAP_OFFSET', 'onLoad', 'feg_m_disconnect loaded'  
    &function2 onLoad () (r:1='this', r:2='_global')
      &pushsdb 0							//'feg_m_disconnect::onLoad()'
      &trace
      &pushregister 1
      &pushsdb 1							//'onEnterFrame'
      &pushsdbgv 2							//'frameDelayedInit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 3							//'bind0'
      &callMethod
      &setMember
    &end // of function onLoad

    &function2 frameDelayedInit () (r:1='this', r:2='_root')
      &pushregister 1
      &pushsdb 1							//'onEnterFrame'
      &pushnull
      &setMember
      &pushsdb 4							//'m_refGlobalEventHandler'
      &pushregister 2
      &pushsdbgm 5							//'gEH'
      &setVariable
      &pushregister 1
      &pushsdbgm 6							//'quitButtonClicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'disconnectMC'
      &pushsdbgm 8							//'quitButton'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushsdbgv 10							//'localMessageHandler'
      &pushone
      &pushsdbgv 11							//'gMH'
      &dcallmp 12							// addMessageHandler()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'disconnectMC'
      &pushsdbgm 8							//'quitButton'
      &dcallmp 13							// enable()
      &pushregister 1
      &pushsdbgm 7							//'disconnectMC'
      &setRegister r:3
      &pop
      &pushsdb 14							//'m_playerClips'
      &pushregister 3
      &pushsdbgm 15							//'entry5'
      &pushregister 3
      &pushsdbgm 16							//'entry4'
      &pushregister 3
      &pushsdbgm 17							//'entry3'
      &pushregister 3
      &pushsdbgm 18							//'entry2'
      &pushregister 3
      &pushsdbgm 19							//'entry1'
      &pushregister 3
      &pushsdbgm 20							//'entry0'
      &pushbyte 6
      &initArray
      &setVariable
      &pushzero
      &dcallfp 21							// hideDisconnect()
    &end // of function frameDelayedInit

    &function quitButtonClicked (    )    
      &pushzero
      &dcallfp 21							// hideDisconnect()
      &getURL 'FSCommand:CallGameFunction'       '%DisconnectQuitGame'      
    &end // of function quitButtonClicked

    &function2 onUnload () (r:1='_global')
      &pushsdb 22							//'feg_m_disconnect::onUnload()'
      &trace
      &pushsdbgv 10							//'localMessageHandler'
      &pushone
      &pushsdbgv 11							//'gMH'
      &dcallmp 23							// removeMessageHandler()
      &pushregister 1
      &pushsdb 24							//'feg_m_disconnect'
      &delete
      &pop
    &end // of function onUnload

    &function2 localMessageHandler (r:3='messageCode') (r:1='_global')
      &pushsdb 25							//'feg_m_disconnect::localMessageHandler() messageCode = \''
      &pushregister 3
      &add
      &pushsdb 26							//'\''
      &add
      &trace
      &pushfalse
      &setRegister r:2
      &pop
      &pushregister 3
      &setRegister r:0
      &pushregister 1
      &pushsdbgm 27							//'MSGCODE'
      &pushsdbgm 28							//'FE_SHOW_MP_DISCONNECT'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 1
      &pushsdbgm 27							//'MSGCODE'
      &pushsdbgm 29							//'FE_HIDE_MP_DISCONNECT'
      &strictEquals
      &jnz label2      
      &jmp label3      
     label1:
      &pushzero
      &dcallfp 30							// showDisconnect()
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label4      
     label2:
      &pushzero
      &dcallfp 21							// hideDisconnect()
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label4      
     label3:
      &jmp label4      
     label4:
      &pushregister 2
      &return
    &end // of function localMessageHandler

    &function2 showDisconnect () (r:1='this', r:2='_global')
      &pushsdb 31							//'feg_m_disconnect::showDisconnect()'
      &trace
      &pushzero
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &pushsdb 32							//'getCurrentLocalEventHandler'
      &callMethod
      &pushundef
      &equals
      &not
      &jnz label5      
      &pushnull
      &pushone
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmp 33							// setLocalEventHandler()
     label5:
      &pushsdb 34							//'m_eventHandlerIndex'
      &pushsdbgv 35							//'m_eventHandlerPriority'
      &pushsdbgv 36							//'localEventHandler'
      &pushbyte 2
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmsv 37							// addPriorityLocalEventHandler()
      &pushregister 1
      &pushsdbgm 7							//'disconnectMC'
      &pushsdbgm 8							//'quitButton'
      &setRegister r:3
      &pop
      &pushsdb 38							//'$APT:DISCONNECTQUITGAME'
      &pushone
      &pushregister 3
      &dcallmp 39							// setText()
      &pushzero
      &pushregister 3
      &dcallmp 40							// show()
      &pushzero
      &pushregister 3
      &dcallmp 13							// enable()
      &pushregister 1
      &pushsdbgm 41							//'shadeMC'
      &pushsdb 42							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'disconnectMC'
      &pushsdb 42							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 43							//'m_playAudio'
      &pushtrue
      &setVariable
      &pushzero
      &dcallfp 44							// updateScreen()
      &pushregister 1
      &pushsdb 1							//'onEnterFrame'
      &pushsdbgv 44							//'updateScreen'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 3							//'bind0'
      &callMethod
      &setMember
    &end // of function showDisconnect

    &function2 hideDisconnect () (r:1='this')
      &pushsdb 45							//'feg_m_disconnect::hideDisconnect()'
      &trace
      &pushregister 1
      &pushsdb 1							//'onEnterFrame'
      &pushnull
      &setMember
      &pushsdb 43							//'m_playAudio'
      &pushfalse
      &setVariable
      &pushregister 1
      &pushsdbgm 41							//'shadeMC'
      &pushsdb 42							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'disconnectMC'
      &pushsdb 42							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 34							//'m_eventHandlerIndex'
      &pushbyte -1
      &equals
      &not
      &not
      &jnz label6      
      &pushsdbgv 34							//'m_eventHandlerIndex'
      &pushone
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmp 46							// removeLocalEventHandler()
     label6:
      &pushsdb 34							//'m_eventHandlerIndex'
      &pushbyte -1
      &setVariable
    &end // of function hideDisconnect

    &function2 localEventHandler (r:1='keyCode', 'controller') ()
      &pushregister 1
      &pushone
      &pushsdbgv 47							//'m_FM'
      &dcallmp 48							// handleDefaultInputs()
    &end // of function localEventHandler

    &function2 updateScreen () ()
      &pushzero
      &pushsdb 49							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 50							//'QueryGameEngine?DISCONNECT_INFO'
      &pushregister 5
      &loadVariables
      &pushsdb 51							//','
      &pushone
      &pushregister 5
      &pushsdbgm 52							//'DISCONNECT_INFO_NAMES'
      &pushsdb 53							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdb 51							//','
      &pushone
      &pushregister 5
      &pushsdbgm 54							//'DISCONNECT_INFO_TIMES'
      &pushsdb 53							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:1
      &pop
     label7:
      &pushregister 1
      &pushsdbgv 14							//'m_playerClips'
      &pushsdbgm 55							//'length'
      &lessThan
      &not
      &jnz label9      
      &pushsdbgv 14							//'m_playerClips'
      &pushregister 1
      &getMember
      &pushsdb 42							//'_visible'
      &pushfalse
      &setMember
      &pushregister 3
      &pushregister 1
      &getMember
      &pushsdbgm 55							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label8      
      &pushsdbgv 14							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdb 42							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 14							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 56							//'playerTF'
      &pushsdb 57							//'text'
      &pushregister 3
      &pushregister 1
      &getMember
      &setMember
      &pushsdbgv 14							//'m_playerClips'
      &pushregister 2
      &getMember
      &pushsdbgm 58							//'timeTF'
      &pushsdb 57							//'text'
      &pushregister 4
      &pushregister 1
      &getMember
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
     label8:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label7      
     label9:
    &end // of function updateScreen

    &pushsdb 59							//'feg_m_disconnect loading now'
    &trace
    &pushsdb 60							//'PADDING'
    &pushbyte 5
    &varEquals
    &pushsdb 61							//'m_messageCallback'
    &var
    &pushsdb 43							//'m_playAudio'
    &pushfalse
    &varEquals
    &pushsdb 34							//'m_eventHandlerIndex'
    &pushbyte -1
    &varEquals
    &pushsdb 35							//'m_eventHandlerPriority'
    &pushone
    &varEquals
    &pushsdb 4							//'m_refGlobalEventHandler'
    &var
    &pushsdb 14							//'m_playerClips'
    &var
    &pushsdb 62							//'OVERLAP_OFFSET'
    &pushbyte 4
    &varEquals
    &pushzero
    &dcallfp 63							// onLoad()
    &pushsdb 64							//'feg_m_disconnect loaded'
    &trace
    &stop
  &end // of frame 0

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 15 // total frames: 120
  &end // of defineMovieClip 15

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 28
  &end // of importAssets

  &defineMovieClip 29 // total frames: 1

    &frame 0
      &pushs 'feg_m_MesaageBox start up'
      &trace
      &stop
    &end // of frame 0

    &placeMovieClip 28 &as 'quitButton'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
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
        &pushsdb 11							//''
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//''
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
        &pushzerosv
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 28
  &end // of defineMovieClip 29
&end
