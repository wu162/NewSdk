movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEC8F2~1\\FEG_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &constants 'feg_m_messageBox::onLoad()', 'onEnterFrame', 'frameDelayedInit', 'bind0', 'm_refGlobalEventHandler', 'gEH', 'button1Clicked', 'messageBoxMC', 'button1', 'setOnMouseUpFunction', 'button2Clicked', 'button2', 'button3Clicked', 'button3', 'button4Clicked', 'button4', 'localMessageHandler', 'gMH', 'addMessageHandler', 'enable', 'noIntro', 'disable', 'onHide', 'sendResponseToEngine', 'feg_m_messageBox::onUnload()', 'removeMessageHandler', 'feg_m_messageBox', 'feg_m_messageBox::localMessageHandler() messageCode = \'', '\'', 'MSGCODE', 'FE_MESSAGE_SHOW', 'FE_MESSAGE_HIDE', 'loadData', 'feg_m_messageBox::loadData()', 'Object', 'QueryGameEngine?MESSAGE_INFO', 'Array', 'MESSAGE_INFO_OPTION1', 'length', '$', 'push', 'MESSAGE_INFO_OPTION2', 'MESSAGE_INFO_OPTION3', 'MESSAGE_INFO_OPTION4', 'MESSAGE_INFO_TEXT', 'MESSAGE_INFO_TITLE', 'MESSAGE_INFO_ID', 'showMessageBox', 'feg_m_messageBox::showMessageBox()', 'm_messageID', 'messageID', 'm_messageCallback', 'messageCallback', 'getCurrentLocalEventHandler', 'setLocalEventHandler', 'm_eventHandlerIndex', 'm_eventHandlerPriority', 'localEventHandler', 'addPriorityLocalEventHandler', 'this', 'titleTF', 'text', 'messageTitle', '&Outline', 'messageTF', 'messageText', 'messageBottom', '_y', 'textHeight', 'bodyHeight', 'PADDING', 'm_handleEvents', 'messageOptions', 'show', 'buttonWidth', 'getWidth', 'buttonHeight', '_height', 'buttonBarWidth', 'buttonBarX', 'panelMiddleMC', '_width', '_x', 'btnMC', 'index', 'this.messageBoxMC.button', 'setText', 'feg_m_messageBox::showMessageBox() bodyHeight == ', 'panelBottomMC', 'OVERLAP_OFFSET', 'onShow', 'm_FM', 'handleDefaultInputs', '%Message_Response?MessageID=', '|ResponseID=', 'feg_m_messageBox::sendResponseToEngine() commandStr == ', 'FSCommand:CallGameFunction', 'feg_m_messageBox::onShow()', 'shadeMC', '_visible', 'm_playAudio', 'feg_m_messageBox::onHide()', '', 'hide', 'removeLocalEventHandler', 'Gui_GlobalShell_LoadingLights', 'playSound', 'feg_m_messageBox loading now', 'onLoad', 'feg_m_messageBox loaded'  
    &function2 onLoad () (r:1='this', r:2='_global')
      &pushsdb 0							//'feg_m_messageBox::onLoad()'
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
      &pushsdbgm 6							//'button1Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 10							//'button2Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 12							//'button3Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 14							//'button4Clicked'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 9							// setOnMouseUpFunction()
      &pushsdbgv 16							//'localMessageHandler'
      &pushone
      &pushsdbgv 17							//'gMH'
      &dcallmp 18							// addMessageHandler()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 19							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 20							// noIntro()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 21							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 21							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 21							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 21							// disable()
      &pushzero
      &dcallfp 22							// onHide()
    &end // of function frameDelayedInit

    &function button1Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushzero
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button1Clicked

    &function button2Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushone
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button2Clicked

    &function button3Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushbyte 2
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button3Clicked

    &function button4Clicked (    )    
      &pushzero
      &dcallfp 22							// onHide()
      &pushbyte 3
      &pushone
      &dcallfp 23							// sendResponseToEngine()
    &end // of function button4Clicked

    &function2 onUnload () (r:1='_global')
      &pushsdb 24							//'feg_m_messageBox::onUnload()'
      &trace
      &pushsdbgv 16							//'localMessageHandler'
      &pushone
      &pushsdbgv 17							//'gMH'
      &dcallmp 25							// removeMessageHandler()
      &pushregister 1
      &pushsdb 26							//'feg_m_messageBox'
      &delete
      &pop
    &end // of function onUnload

    &function2 localMessageHandler (r:3='messageCode') (r:1='_global')
      &pushsdb 27							//'feg_m_messageBox::localMessageHandler() messageCode = \''
      &pushregister 3
      &add
      &pushsdb 28							//'\''
      &add
      &trace
      &pushfalse
      &setRegister r:2
      &pop
      &pushregister 3
      &setRegister r:0
      &pushregister 1
      &pushsdbgm 29							//'MSGCODE'
      &pushsdbgm 30							//'FE_MESSAGE_SHOW'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 1
      &pushsdbgm 29							//'MSGCODE'
      &pushsdbgm 31							//'FE_MESSAGE_HIDE'
      &strictEquals
      &jnz label2      
      &jmp label3      
     label1:
      &pushzero
      &dcallfp 32							// loadData()
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label4      
     label2:
      &pushzero
      &dcallfp 22							// onHide()
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

    &function2 loadData () ()
      &pushsdb 33							//'feg_m_messageBox::loadData()'
      &trace
      &pushzero
      &pushsdb 34							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 35							//'QueryGameEngine?MESSAGE_INFO'
      &pushregister 1
      &loadVariables
      &pushzero
      &pushsdb 36							//'Array'
      &new
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdbgm 37							//'MESSAGE_INFO_OPTION1'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label5      
      &pop
      &pushregister 1
      &pushsdbgm 37							//'MESSAGE_INFO_OPTION1'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label5:
      &not
      &jnz label6      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 37							//'MESSAGE_INFO_OPTION1'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label6:
      &pushregister 1
      &pushsdbgm 41							//'MESSAGE_INFO_OPTION2'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label7      
      &pop
      &pushregister 1
      &pushsdbgm 41							//'MESSAGE_INFO_OPTION2'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label7:
      &not
      &jnz label8      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 41							//'MESSAGE_INFO_OPTION2'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label8:
      &pushregister 1
      &pushsdbgm 42							//'MESSAGE_INFO_OPTION3'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label9      
      &pop
      &pushregister 1
      &pushsdbgm 42							//'MESSAGE_INFO_OPTION3'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label9:
      &not
      &jnz label10      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 42							//'MESSAGE_INFO_OPTION3'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label10:
      &pushregister 1
      &pushsdbgm 43							//'MESSAGE_INFO_OPTION4'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label11      
      &pop
      &pushregister 1
      &pushsdbgm 43							//'MESSAGE_INFO_OPTION4'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
     label11:
      &not
      &jnz label12      
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 43							//'MESSAGE_INFO_OPTION4'
      &add
      &pushone
      &pushregister 2
      &dcallmp 40							// push()
     label12:
      &pushsdbgv 23							//'sendResponseToEngine'
      &pushregister 2
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 44							//'MESSAGE_INFO_TEXT'
      &add
      &pushsdb 39							//'$'
      &pushregister 1
      &pushsdbgm 45							//'MESSAGE_INFO_TITLE'
      &add
      &pushregister 1
      &pushsdbgm 46							//'MESSAGE_INFO_ID'
      &pushbyte 5
      &dcallfp 47							// showMessageBox()
    &end // of function loadData

    &function showMessageBox ('messageID', 'messageTitle', 'messageText', 'messageOptions', 'messageCallback'    )    
      &pushsdb 48							//'feg_m_messageBox::showMessageBox()'
      &trace
      &pushsdb 49							//'m_messageID'
      &pushsdbgv 50							//'messageID'
      &setVariable
      &pushsdb 51							//'m_messageCallback'
      &pushsdbgv 52							//'messageCallback'
      &setVariable
      &pushzero
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &pushsdb 53							//'getCurrentLocalEventHandler'
      &callMethod
      &pushundef
      &equals
      &not
      &jnz label13      
      &pushnull
      &pushone
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmp 54							// setLocalEventHandler()
     label13:
      &pushsdb 55							//'m_eventHandlerIndex'
      &pushsdbgv 56							//'m_eventHandlerPriority'
      &pushsdbgv 57							//'localEventHandler'
      &pushbyte 2
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmsv 58							// addPriorityLocalEventHandler()
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'titleTF'
      &pushsdb 61							//'text'
      &pushsdbgv 62							//'messageTitle'
      &pushsdb 63							//'&Outline'
      &add
      &setMember
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 64							//'messageTF'
      &pushsdb 61							//'text'
      &pushsdbgv 65							//'messageText'
      &pushsdb 63							//'&Outline'
      &add
      &setMember
      &pushsdb 66							//'messageBottom'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 64							//'messageTF'
      &pushsdbgm 67							//'_y'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 64							//'messageTF'
      &pushsdbgm 68							//'textHeight'
      &add
      &varEquals
      &pushsdb 69							//'bodyHeight'
      &pushsdbgv 70							//'PADDING'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 64							//'messageTF'
      &pushsdbgm 68							//'textHeight'
      &add
      &varEquals
      &pushsdb 71							//'m_handleEvents'
      &pushfalse
      &setVariable
      &pushsdbgv 72							//'messageOptions'
      &pushsdbgm 38							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label16      
      &pushsdb 71							//'m_handleEvents'
      &pushtrue
      &setVariable
      &pushzero
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 73							// show()
      &pushsdb 74							//'buttonWidth'
      &pushbyte 2
      &pushsdbgv 70							//'PADDING'
      &multiply
      &pushzero
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &pushsdb 75							//'getWidth'
      &callMethod
      &add
      &varEquals
      &pushsdb 76							//'buttonHeight'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &pushsdbgm 77							//'_height'
      &varEquals
      &pushsdb 78							//'buttonBarWidth'
      &pushsdbgv 72							//'messageOptions'
      &pushsdbgm 38							//'length'
      &pushsdbgv 74							//'buttonWidth'
      &multiply
      &varEquals
      &pushsdb 79							//'buttonBarX'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 80							//'panelMiddleMC'
      &pushsdbgm 81							//'_width'
      &pushsdbgv 78							//'buttonBarWidth'
      &subtract
      &pushbyte 2
      &divide
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 80							//'panelMiddleMC'
      &pushsdbgm 82							//'_x'
      &add
      &varEquals
      &pushsdb 83							//'btnMC'
      &var
      &pushsdb 84							//'index'
      &pushzero
      &varEquals
     label14:
      &pushsdbgv 84							//'index'
      &pushsdbgv 72							//'messageOptions'
      &pushsdbgm 38							//'length'
      &lessThan
      &not
      &jnz label15      
      &pushsdb 83							//'btnMC'
      &pushsdb 85							//'this.messageBoxMC.button'
      &pushsdbgv 84							//'index'
      &pushone
      &add
      &add
      &getVariable
      &setVariable
      &pushsdbgv 83							//'btnMC'
      &pushsdb 67							//'_y'
      &pushsdbgv 66							//'messageBottom'
      &pushsdbgv 70							//'PADDING'
      &add
      &setMember
      &pushsdbgv 83							//'btnMC'
      &pushsdb 82							//'_x'
      &pushsdbgv 79							//'buttonBarX'
      &pushsdbgv 84							//'index'
      &pushsdbgv 74							//'buttonWidth'
      &multiply
      &add
      &setMember
      &pushsdbgv 72							//'messageOptions'
      &pushsdbgv 84							//'index'
      &getMember
      &pushone
      &pushsdbgv 83							//'btnMC'
      &dcallmp 86							// setText()
      &pushzero
      &pushsdbgv 83							//'btnMC'
      &dcallmp 73							// show()
      &pushzero
      &pushsdbgv 83							//'btnMC'
      &dcallmp 19							// enable()
      &pushsdb 84							//'index'
      &pushsdbgv 84							//'index'
      &increment
      &setVariable
      &jmp label14      
     label15:
      &pushsdb 69							//'bodyHeight'
      &pushsdbgv 69							//'bodyHeight'
      &pushsdbgv 76							//'buttonHeight'
      &add
      &setVariable
     label16:
      &pushsdb 87							//'feg_m_messageBox::showMessageBox() bodyHeight == '
      &pushsdbgv 69							//'bodyHeight'
      &add
      &trace
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 80							//'panelMiddleMC'
      &pushsdb 77							//'_height'
      &pushsdbgv 69							//'bodyHeight'
      &setMember
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 88							//'panelBottomMC'
      &pushsdb 67							//'_y'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 80							//'panelMiddleMC'
      &pushsdbgm 67							//'_y'
      &pushthisgv
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 80							//'panelMiddleMC'
      &pushsdbgm 77							//'_height'
      &add
      &pushsdbgv 89							//'OVERLAP_OFFSET'
      &subtract
      &setMember
      &pushzero
      &dcallfp 90							// onShow()
    &end // of function showMessageBox

    &function2 localEventHandler (r:1='keyCode', 'controller') ()
      &pushregister 1
      &pushone
      &pushsdbgv 91							//'m_FM'
      &dcallmp 92							// handleDefaultInputs()
    &end // of function localEventHandler

    &function2 sendResponseToEngine (r:2='index') ()
      &pushsdb 93							//'%Message_Response?MessageID='
      &pushsdbgv 49							//'m_messageID'
      &add
      &pushsdb 94							//'|ResponseID='
      &add
      &pushregister 2
      &add
      &setRegister r:1
      &pop
      &pushsdb 95							//'feg_m_messageBox::sendResponseToEngine() commandStr == '
      &pushregister 1
      &add
      &trace
      &pushsdb 96							//'FSCommand:CallGameFunction'
      &pushregister 1
      &getURL2
    &end // of function sendResponseToEngine

    &function2 onShow () (r:1='this')
      &pushsdb 97							//'feg_m_messageBox::onShow()'
      &trace
      &pushregister 1
      &pushsdbgm 98							//'shadeMC'
      &pushsdb 99							//'_visible'
      &pushtrue
      &setMember
      &pushshort 360
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdb 67							//'_y'
      &pushregister 2
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 77							//'_height'
      &pushbyte 2
      &divide
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdb 99							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 100							//'m_playAudio'
      &pushtrue
      &setVariable
    &end // of function onShow

    &function2 onHide () (r:1='this')
      &pushsdb 101							//'feg_m_messageBox::onHide()'
      &trace
      &pushsdb 100							//'m_playAudio'
      &pushfalse
      &setVariable
      &pushregister 1
      &pushsdbgm 98							//'shadeMC'
      &pushsdb 99							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdb 99							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'titleTF'
      &pushsdb 61							//'text'
      &pushsdb 102							//''
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 64							//'messageTF'
      &pushsdb 61							//'text'
      &pushsdb 102							//''
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &pushsdb 67							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'titleTF'
      &pushsdbgm 67							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 8							//'button1'
      &dcallmp 103							// hide()
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &pushsdb 67							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'titleTF'
      &pushsdbgm 67							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 11							//'button2'
      &dcallmp 103							// hide()
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &pushsdb 67							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'titleTF'
      &pushsdbgm 67							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 13							//'button3'
      &dcallmp 103							// hide()
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &pushsdb 67							//'_y'
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 60							//'titleTF'
      &pushsdbgm 67							//'_y'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'messageBoxMC'
      &pushsdbgm 15							//'button4'
      &dcallmp 103							// hide()
      &pushsdbgv 55							//'m_eventHandlerIndex'
      &pushbyte -1
      &equals
      &not
      &not
      &jnz label17      
      &pushsdbgv 55							//'m_eventHandlerIndex'
      &pushone
      &pushsdbgv 4							//'m_refGlobalEventHandler'
      &dcallmp 104							// removeLocalEventHandler()
     label17:
      &pushsdb 55							//'m_eventHandlerIndex'
      &pushbyte -1
      &setVariable
      &pushsdb 71							//'m_handleEvents'
      &pushfalse
      &setVariable
    &end // of function onHide

    &function2 playSoundFX () (r:1='_global')
      &pushsdbgv 100							//'m_playAudio'
      &not
      &jnz label18      
      &pushsdb 105							//'Gui_GlobalShell_LoadingLights'
      &pushone
      &pushregister 1
      &dcallmp 106							// playSound()
     label18:
    &end // of function playSoundFX

    &pushsdb 107							//'feg_m_messageBox loading now'
    &trace
    &pushsdb 70							//'PADDING'
    &pushbyte 5
    &varEquals
    &pushsdb 49							//'m_messageID'
    &var
    &pushsdb 51							//'m_messageCallback'
    &var
    &pushsdb 100							//'m_playAudio'
    &pushfalse
    &varEquals
    &pushsdb 71							//'m_handleEvents'
    &pushfalse
    &varEquals
    &pushsdb 55							//'m_eventHandlerIndex'
    &pushbyte -1
    &varEquals
    &pushsdb 56							//'m_eventHandlerPriority'
    &pushone
    &varEquals
    &pushsdb 4							//'m_refGlobalEventHandler'
    &var
    &pushsdb 89							//'OVERLAP_OFFSET'
    &pushbyte 4
    &varEquals
    &pushzero
    &dcallfp 108							// onLoad()
    &pushsdb 109							//'feg_m_messageBox loaded'
    &trace
    &stop
  &end // of frame 0

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 15 // total frames: 120
  &end // of defineMovieClip 15
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 20
  &end // of importAssets

  &defineMovieClip 21 // total frames: 1

    &frame 0
      &pushs 'feg_m_MesaageBox start up'
      &trace
      &stop
    &end // of frame 0

    &placeMovieClip 20 &as 'button1'
    
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
    &end // of placeMovieClip 20

    &placeMovieClip 20 &as 'button2'
    
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
        &pushone
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 20

    &placeMovieClip 20 &as 'button3'
    
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
        &pushbyte 2
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 20

    &placeMovieClip 20 &as 'button4'
    
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
        &pushbyte 3
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'buttonContentSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 20
  &end // of defineMovieClip 21
&end
