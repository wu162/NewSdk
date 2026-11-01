movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\GLOBAL~1\\GLOBAL~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 12 fps, 550x400 px

  &frame 0
    &constants 'ButtonClass', 'ShellButtons Small', 'Object', 'registerClass', 'ShellButtons Kick', 'apiClass', 'AptApiClass', 'zombie', 'AptZombieClass', 'debug', 'DebugClass'  
    &pushsdbgv 0							//'ButtonClass'
    &pushsdb 1							//'ShellButtons Small'
    &pushbyte 2
    &pushsdbgv 2							//'Object'
    &dcallmp 3							// registerClass()
    &pushsdbgv 0							//'ButtonClass'
    &pushsdb 4							//'ShellButtons Kick'
    &pushbyte 2
    &pushsdbgv 2							//'Object'
    &dcallmp 3							// registerClass()
    &pushsdb 5							//'apiClass'
    &pushzero
    &pushsdb 6							//'AptApiClass'
    &new
    &varEquals
    &pushsdb 7							//'zombie'
    &pushzero
    &pushsdb 8							//'AptZombieClass'
    &new
    &varEquals
    &pushsdb 9							//'debug'
    &pushzero
    &pushsdb 10							//'DebugClass'
    &new
    &varEquals
  &end // of frame 0

  &defineMovieClip 1 // total frames: 0
  &end // of defineMovieClip 1
  
  &exportAssets
    1 &as '__Packages.DebugClass'
  &end // of exportAssets
  
  &initMovieClip 1
    &constants '_global', 'DebugClass', 'prototype', 'InitDebugClass', 'm_codePrefix', ':', 'Enable', 'extern', 'InGame', 'Boolean', 'DoTrace', 'm_enabled', 'Trace', 'Dump', 'Dump:', '\t', ': ', ' : ', '', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'DebugClass'
    &not
    &not
    &jnz label5    
    &pushglobalgv
    &pushsdb 1							//'DebugClass'
    &function  (    )    
    &end // of function 

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 2							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 3							//'InitDebugClass'
    &function2  (r:2='codePrefix') (r:1='this')
      &pushregister 1
      &pushsdb 4							//'m_codePrefix'
      &pushregister 2
      &pushsdb 5							//':'
      &add
      &setMember
      &pushfalse
      &pushone
      &pushregister 1
      &dcallmp 6							// Enable()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 6							//'Enable'
    &function2  (r:2='enable') (r:1='this')
      &pushregister 1
      &pushsdbgm 7							//'extern'
      &pushsdbgm 8							//'InGame'
      &toNumber
      &pushone
      &pushsdb 9							//'Boolean'
      &callFunction
      &not
      &not
      &jnz label1      
      &pushregister 1
      &pushsdbgm 7							//'extern'
      &pushsdb 10							//'DoTrace'
      &pushregister 2
      &setMember
     label1:
      &pushregister 1
      &pushsdb 11							//'m_enabled'
      &pushregister 1
      &pushsdbgm 7							//'extern'
      &pushsdbgm 10							//'DoTrace'
      &toNumber
      &pushone
      &pushsdb 9							//'Boolean'
      &callFunction
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 12							//'Trace'
    &function2  (r:2='show') (r:1='this')
      &pushregister 1
      &pushsdbgm 11							//'m_enabled'
      &not
      &jnz label2      
      &pushregister 1
      &pushsdbgm 4							//'m_codePrefix'
      &pushregister 2
      &add
      &trace
     label2:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 13							//'Dump'
    &function2  (r:2='obj', r:4='msg') (r:1='this')
      &pushregister 1
      &pushsdbgm 11							//'m_enabled'
      &not
      &jnz label4      
      &pushsdb 14							//'Dump:'
      &pushregister 4
      &add
      &pushsdb 5							//':'
      &add
      &pushregister 2
      &typeof
      &add
      &trace
      &pushregister 2
      &enumerateValue
     label3:
      &setRegister r:0
      &pushnull
      &equals
      &jnz label4      
      &pushregister 0
      &setRegister r:3
      &pop
      &pushsdb 15							//'\t'
      &pushregister 3
      &add
      &pushsdb 16							//': '
      &add
      &pushregister 2
      &pushregister 3
      &getMember
      &add
      &pushsdb 17							//' : '
      &add
      &pushregister 2
      &pushregister 3
      &getMember
      &typeof
      &add
      &trace
      &jmp label3      
     label4:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 7							//'extern'
    &pushsdb 10							//'DoTrace'
    &pushone
    &pushsdb 8							//'InGame'
    &pushzero
    &pushbyte 2
    &initObject
    &setMember
    &pushregister 1
    &pushsdb 4							//'m_codePrefix'
    &pushsdb 18							//''
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'DebugClass'
    &pushsdbgm 2							//'prototype'
    &pushbyte 3
    &pushsdb 19							//'ASSetPropFlags'
    &callFunction
   label5:
    &pop
  &end // of initMovieClip 1

  &defineMovieClip 2 // total frames: 0
  &end // of defineMovieClip 2
  
  &exportAssets
    2 &as '__Packages.ButtonClass'
  &end // of exportAssets
  
  &initMovieClip 2
    &constants '_global', 'ButtonClass', 'm_text', '$', '_name', 'movieclip', '_visible', 'Show', 'gotoAndPlay', 'Text', 'indexOf', 'State', 'Trans', 'Bttn', 'onPress', '_parent', 'Press', 'onRelease', 'Release', 'onRollOver', 'RollOver', 'onRollOut', 'RollOut', 'onDragOut', 'DragOut', 'onDragOver', 'DragOver', 'WARNING:ButtonClass:', '_target', '/', ' is an unknown movie clip!', 'MovieClip', 'prototype', 'SetText', 'SetSfxClick', 'm_sfxClick', 'SetSfxRollOver', 'm_sfxRollOver', 'SetSfxRollOut', 'm_sfxRollOut', 'SetMode', 'm_mode', 'm_state', 'UpState', 'ChangeState', 'ClearMode', '', 'GetMode', 'onUnload', 'AptZombieClass', 'DeleteFunctions', 'Button::ChangeState(', '):(', ')', '->', ' (', 'm_curStateMc', ':', 'm_curBttnMc', 'm_curTextMc', ')->(', 'm_nextState', 'GetText', 'SwitchObject', 'GetBttn', 'GetStateClip', 'StartTransition', 'EndTransition', 'stop', 'GetObject', 'PressCallback', 'DownState', 'FSCommand:PlaySound', 'ReleaseCallback', 'ReleaseTrans', 'OverState', 'RollOverCallback', 'RollOutCallback', 'RollOutTrans', 'DragOutCallback', 'OutState', 'DragOverCallback', 'debug', 'ButtonClass::', 'DebugClass', '$ClickButton', '$OverButton', '$OutButton', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'ButtonClass'
    &not
    &not
    &jnz label24    
    &pushglobalgv
    &pushsdb 1							//'ButtonClass'
    &function2  () (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushregister 1
      &pushsdb 2							//'m_text'
      &pushsdb 3							//'$'
      &pushregister 1
      &pushsdbgm 4							//'_name'
      &add
      &setMember
      &pushregister 1
      &enumerateValue
     label1:
      &setRegister r:0
      &pushnull
      &equals
      &jnz label8      
      &pushregister 0
      &setRegister r:5
      &pop
      &pushregister 1
      &pushregister 5
      &getMember
      &pushundef
      &equals
      &not
      &not
      &jnz label7      
      &pushregister 1
      &pushregister 5
      &getMember
      &setRegister r:3
      &pop
      &pushregister 3
      &typeof
      &pushsdb 5							//'movieclip'
      &equals
      &setRegister r:4
      &pop
      &pushregister 3
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushregister 4
      &not
      &jnz label2      
      &pushregister 3
      &pushsdbgm 7							//'Show'
      &pushundef
      &equals
      &not
      &jnz label2      
      &pushregister 3
      &pushsdb 7							//'Show'
      &function2  ('previous') (r:1='this')
        &pushone
        &pushone
        &pushregister 1
        &dcallmp 8							// gotoAndPlay()
      &end // of function 

      &setMember
     label2:
      &pushsdb 9							//'Text'
      &pushone
      &pushregister 5
      &pushsdb 10							//'indexOf'
      &callMethod
      &pushzero
      &lessThan
      &not
      &not
      &jnz label3      
      &jmp label7      
     label3:
      &pushsdb 11							//'State'
      &pushone
      &pushregister 5
      &pushsdb 10							//'indexOf'
      &callMethod
      &pushzero
      &lessThan
      &not
      &dup
      &jnz label4      
      &pop
      &pushsdb 12							//'Trans'
      &pushone
      &pushregister 5
      &pushsdb 10							//'indexOf'
      &callMethod
      &pushzero
      &lessThan
      &not
     label4:
      &not
      &jnz label5      
      &jmp label7      
     label5:
      &pushsdb 13							//'Bttn'
      &pushone
      &pushregister 5
      &pushsdb 10							//'indexOf'
      &callMethod
      &pushzero
      &lessThan
      &not
      &not
      &jnz label6      
      &pushregister 3
      &pushsdb 14							//'onPress'
      &function2  () (r:1='this')
        &pushzero
        &pushregister 1
        &pushsdbgm 15							//'_parent'
        &dcallmp 16							// Press()
      &end // of function 

      &setMember
      &pushregister 3
      &pushsdb 17							//'onRelease'
      &function2  () (r:1='this')
        &pushzero
        &pushregister 1
        &pushsdbgm 15							//'_parent'
        &dcallmp 18							// Release()
      &end // of function 

      &setMember
      &pushregister 3
      &pushsdb 19							//'onRollOver'
      &function2  () (r:1='this')
        &pushzero
        &pushregister 1
        &pushsdbgm 15							//'_parent'
        &dcallmp 20							// RollOver()
      &end // of function 

      &setMember
      &pushregister 3
      &pushsdb 21							//'onRollOut'
      &function2  () (r:1='this')
        &pushzero
        &pushregister 1
        &pushsdbgm 15							//'_parent'
        &dcallmp 22							// RollOut()
      &end // of function 

      &setMember
      &pushregister 3
      &pushsdb 23							//'onDragOut'
      &function2  () (r:1='this')
        &pushzero
        &pushregister 1
        &pushsdbgm 15							//'_parent'
        &dcallmp 24							// DragOut()
      &end // of function 

      &setMember
      &pushregister 3
      &pushsdb 25							//'onDragOver'
      &function2  () (r:1='this')
        &pushzero
        &pushregister 1
        &pushsdbgm 15							//'_parent'
        &dcallmp 26							// DragOver()
      &end // of function 

      &setMember
      &jmp label7      
     label6:
      &pushregister 4
      &not
      &jnz label7      
      &pushsdb 27							//'WARNING:ButtonClass:'
      &pushregister 1
      &pushsdbgm 28							//'_target'
      &add
      &pushsdb 29							//'/'
      &add
      &pushregister 5
      &add
      &pushsdb 30							//' is an unknown movie clip!'
      &add
      &trace
      &pushregister 3
      &pushsdb 6							//'_visible'
      &pushtrue
      &setMember
     label7:
      &jmp label1      
     label8:
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'ButtonClass'
    &pushsdbgv 31							//'MovieClip'
    &extends
    &pushregister 0
    &pushsdbgm 32							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 33							//'SetText'
    &function2  (r:2='textString') (r:1='this')
      &pushregister 1
      &pushsdb 2							//'m_text'
      &pushregister 2
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 34							//'SetSfxClick'
    &function2  (r:2='sfx') (r:1='this')
      &pushregister 1
      &pushsdb 35							//'m_sfxClick'
      &pushregister 2
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 36							//'SetSfxRollOver'
    &function2  (r:2='sfx') (r:1='this')
      &pushregister 1
      &pushsdb 37							//'m_sfxRollOver'
      &pushregister 2
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 38							//'SetSfxRollOut'
    &function2  (r:2='sfx') (r:1='this')
      &pushregister 1
      &pushsdb 39							//'m_sfxRollOut'
      &pushregister 2
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 40							//'SetMode'
    &function2  (r:2='mode') (r:1='this')
      &pushregister 1
      &pushsdb 41							//'m_mode'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdbgm 42							//'m_state'
      &pushundef
      &equals
      &not
      &jnz label9      
      &pushregister 1
      &pushsdb 42							//'m_state'
      &pushsdb 43							//'UpState'
      &setMember
     label9:
      &pushregister 1
      &pushsdbgm 42							//'m_state'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 45							//'ClearMode'
    &function2  ('Void') (r:1='this')
      &pushsdb 46							//''
      &pushone
      &pushregister 1
      &dcallmp 40							// SetMode()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 47							//'GetMode'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 41							//'m_mode'
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 48							//'onUnload'
    &function2  () (r:1='this')
      &pushregister 1
      &pushone
      &pushsdbgv 49							//'AptZombieClass'
      &dcallmp 50							// DeleteFunctions()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 44							//'ChangeState'
    &function2  (r:2='stateStr') (r:1='this')
      &pushsdb 51							//'Button::ChangeState('
      &pushregister 1
      &pushsdbgm 4							//'_name'
      &add
      &pushsdb 52							//'):('
      &add
      &pushregister 1
      &pushsdbgm 41							//'m_mode'
      &add
      &pushsdb 53							//')'
      &add
      &pushregister 1
      &pushsdbgm 42							//'m_state'
      &add
      &pushsdb 54							//'->'
      &add
      &pushregister 2
      &add
      &pushsdb 55							//' ('
      &add
      &pushregister 1
      &pushsdbgm 56							//'m_curStateMc'
      &pushsdbgm 4							//'_name'
      &add
      &pushsdb 57							//':'
      &add
      &pushregister 1
      &pushsdbgm 58							//'m_curBttnMc'
      &pushsdbgm 4							//'_name'
      &add
      &pushsdb 57							//':'
      &add
      &pushregister 1
      &pushsdbgm 59							//'m_curTextMc'
      &pushsdbgm 4							//'_name'
      &add
      &pushsdb 60							//')->('
      &add
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdb 42							//'m_state'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 61							//'m_nextState'
      &pushundef
      &setMember
      &pushregister 1
      &pushsdb 59							//'m_curTextMc'
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdb 62							//'GetText'
      &callMethod
      &pushregister 1
      &pushsdbgm 59							//'m_curTextMc'
      &pushbyte 2
      &pushregister 1
      &pushsdb 63							//'SwitchObject'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 58							//'m_curBttnMc'
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdb 64							//'GetBttn'
      &callMethod
      &pushregister 1
      &pushsdbgm 58							//'m_curBttnMc'
      &pushbyte 2
      &pushregister 1
      &pushsdb 63							//'SwitchObject'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 56							//'m_curStateMc'
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdb 65							//'GetStateClip'
      &callMethod
      &pushregister 1
      &pushsdbgm 56							//'m_curStateMc'
      &pushbyte 2
      &pushregister 1
      &pushsdb 63							//'SwitchObject'
      &callMethod
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 66							//'StartTransition'
    &function2  (r:3='transition', r:4='nextState') (r:1='this')
      &pushregister 3
      &pushone
      &pushregister 1
      &pushsdb 65							//'GetStateClip'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushundef
      &equals
      &not
      &not
      &jnz label10      
      &pushregister 1
      &pushsdb 42							//'m_state'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 61							//'m_nextState'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdbgm 58							//'m_curBttnMc'
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdb 58							//'m_curBttnMc'
      &pushundef
      &setMember
      &pushregister 1
      &pushsdb 59							//'m_curTextMc'
      &pushregister 3
      &pushone
      &pushregister 1
      &pushsdb 62							//'GetText'
      &callMethod
      &pushregister 1
      &pushsdbgm 59							//'m_curTextMc'
      &pushbyte 2
      &pushregister 1
      &pushsdb 63							//'SwitchObject'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 56							//'m_curStateMc'
      &pushregister 2
      &pushregister 1
      &pushsdbgm 56							//'m_curStateMc'
      &pushbyte 2
      &pushregister 1
      &pushsdb 63							//'SwitchObject'
      &callMethod
      &setMember
      &pushtrue
      &return
     label10:
      &pushfalse
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 67							//'EndTransition'
    &function2  (r:2='transition') (r:1='this')
      &pushregister 2
      &pushregister 1
      &pushsdbgm 56							//'m_curStateMc'
      &equals
      &dup
      &not
      &jnz label11      
      &pop
      &pushregister 1
      &pushsdbgm 61							//'m_nextState'
      &pushundef
      &equals
      &not
     label11:
      &not
      &jnz label12      
      &pushregister 1
      &pushsdbgm 61							//'m_nextState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
     label12:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 63							//'SwitchObject'
    &function2  (r:1='current', r:3='next') ()
      &pushregister 3
      &pushundef
      &equals
      &not
      &not
      &jnz label14      
      &pushregister 1
      &setRegister r:2
      &pop
      &pushregister 3
      &typeof
      &pushsdb 5							//'movieclip'
      &equals
      &setRegister r:4
      &pop
      &pushregister 1
      &not
      &jnz label13      
      &pushregister 1
      &pushsdb 6							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 68							// stop()
     label13:
      &pushregister 3
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 6							//'_visible'
      &pushtrue
      &setMember
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 7							// Show()
     label14:
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 65							//'GetStateClip'
    &function2  (r:3='stateStr') (r:1='this')
      &pushregister 1
      &pushregister 1
      &pushsdbgm 41							//'m_mode'
      &pushregister 3
      &add
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushundef
      &equals
      &not
      &jnz label15      
      &pushregister 1
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
     label15:
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 62							//'GetText'
    &function2  (r:3='stateStr') (r:1='this')
      &pushsdb 9							//'Text'
      &pushregister 3
      &pushbyte 2
      &pushregister 1
      &pushsdb 69							//'GetObject'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 64							//'GetBttn'
    &function2  (r:3='stateStr') (r:1='this')
      &pushsdb 13							//'Bttn'
      &pushregister 3
      &pushbyte 2
      &pushregister 1
      &pushsdb 69							//'GetObject'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 69							//'GetObject'
    &function2  (r:4='stateStr', r:3='postfix') (r:1='this')
      &pushregister 1
      &pushregister 1
      &pushsdbgm 41							//'m_mode'
      &pushregister 4
      &add
      &pushregister 3
      &add
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushundef
      &equals
      &not
      &jnz label16      
      &pushregister 1
      &pushregister 1
      &pushsdbgm 41							//'m_mode'
      &pushregister 3
      &add
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushundef
      &equals
      &not
      &jnz label16      
      &pushregister 1
      &pushregister 4
      &pushregister 3
      &add
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushundef
      &equals
      &not
      &jnz label16      
      &pushregister 1
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
     label16:
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 16							//'Press'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 13							//'Bttn'
      &pushsdbgm 6							//'_visible'
      &not
      &jnz label17      
      &pushzero
      &pushregister 1
      &dcallmp 70							// PressCallback()
      &pushsdb 71							//'DownState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
      &pushsdb 72							//'FSCommand:PlaySound'
      &pushregister 1
      &pushsdbgm 35							//'m_sfxClick'
      &getURL2
     label17:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 18							//'Release'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 13							//'Bttn'
      &pushsdbgm 6							//'_visible'
      &not
      &jnz label18      
      &pushzero
      &pushregister 1
      &dcallmp 73							// ReleaseCallback()
      &pushsdb 43							//'UpState'
      &pushsdb 74							//'ReleaseTrans'
      &pushbyte 2
      &pushregister 1
      &pushsdb 66							//'StartTransition'
      &callMethod
      &not
      &not
      &jnz label18      
      &pushsdb 75							//'OverState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
     label18:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 20							//'RollOver'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 13							//'Bttn'
      &pushsdbgm 6							//'_visible'
      &not
      &jnz label19      
      &pushzero
      &pushregister 1
      &dcallmp 76							// RollOverCallback()
      &pushsdb 75							//'OverState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
      &pushsdb 72							//'FSCommand:PlaySound'
      &pushregister 1
      &pushsdbgm 37							//'m_sfxRollOver'
      &getURL2
     label19:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 22							//'RollOut'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 13							//'Bttn'
      &pushsdbgm 6							//'_visible'
      &not
      &jnz label21      
      &pushzero
      &pushregister 1
      &dcallmp 77							// RollOutCallback()
      &pushsdb 43							//'UpState'
      &pushsdb 78							//'RollOutTrans'
      &pushbyte 2
      &pushregister 1
      &pushsdb 66							//'StartTransition'
      &callMethod
      &not
      &not
      &jnz label20      
      &pushsdb 43							//'UpState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
     label20:
      &pushsdb 72							//'FSCommand:PlaySound'
      &pushregister 1
      &pushsdbgm 39							//'m_sfxRollOut'
      &getURL2
     label21:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 24							//'DragOut'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 13							//'Bttn'
      &pushsdbgm 6							//'_visible'
      &not
      &jnz label22      
      &pushzero
      &pushregister 1
      &dcallmp 79							// DragOutCallback()
      &pushsdb 43							//'UpState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
      &pushsdb 80							//'OutState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
     label22:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 26							//'DragOver'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 13							//'Bttn'
      &pushsdbgm 6							//'_visible'
      &not
      &jnz label23      
      &pushzero
      &pushregister 1
      &dcallmp 81							// DragOverCallback()
      &pushsdb 71							//'DownState'
      &pushone
      &pushregister 1
      &dcallmp 44							// ChangeState()
     label23:
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 82							//'debug'
    &pushsdb 83							//'ButtonClass::'
    &pushone
    &pushsdb 84							//'DebugClass'
    &new
    &setMember
    &pushregister 1
    &pushsdb 42							//'m_state'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 41							//'m_mode'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 61							//'m_nextState'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 56							//'m_curStateMc'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 59							//'m_curTextMc'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 58							//'m_curBttnMc'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 35							//'m_sfxClick'
    &pushsdb 85							//'$ClickButton'
    &setMember
    &pushregister 1
    &pushsdb 37							//'m_sfxRollOver'
    &pushsdb 86							//'$OverButton'
    &setMember
    &pushregister 1
    &pushsdb 39							//'m_sfxRollOut'
    &pushsdb 87							//'$OutButton'
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'ButtonClass'
    &pushsdbgm 32							//'prototype'
    &pushbyte 3
    &pushsdb 88							//'ASSetPropFlags'
    &callFunction
   label24:
    &pop
  &end // of initMovieClip 2

  &defineMovieClip 3 // total frames: 0
  &end // of defineMovieClip 3
  
  &exportAssets
    3 &as '__Packages.AptZombieClass'
  &end // of exportAssets
  
  &initMovieClip 3
    &constants '_global', 'AptZombieClass', '_visible', 'MovieClip', 'prototype', 'AddClass', 's_classes', 'push', 'Object', 'registerClass', 'AddObject', 'AddObject:', 'debug', 'Trace', 's_objects', 'DeleteClass', 'DeleteClass:', 'DeleteFunctions', 'DeleteFunctions:', '_target', '   ', ':', 'movieclip', 'function', 'onUnload', 'length', 'Array', 'AptZombieClass::', 'DebugClass', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'AptZombieClass'
    &not
    &not
    &jnz label9    
    &pushglobalgv
    &pushsdb 1							//'AptZombieClass'
    &function2  () (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushregister 1
      &pushsdb 2							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'AptZombieClass'
    &pushsdbgv 3							//'MovieClip'
    &extends
    &pushregister 0
    &pushsdbgm 4							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 0
    &pushsdb 5							//'AddClass'
    &function2  (r:1='linkageIdentifier', r:2='className') ()
      &pushregister 1
      &pushone
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 6							//'s_classes'
      &dcallmp 7							// push()
      &pushregister 2
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 8							//'Object'
      &dcallmp 9							// registerClass()
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 10							//'AddObject'
    &function2  (r:1='obj') ()
      &pushsdb 11							//'AddObject:'
      &pushregister 1
      &add
      &pushone
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 12							//'debug'
      &dcallmp 13							// Trace()
      &pushregister 1
      &pushone
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 14							//'s_objects'
      &dcallmp 7							// push()
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 15							//'DeleteClass'
    &function2  (r:2='className') (r:1='_global')
      &pushsdb 16							//'DeleteClass:'
      &pushregister 2
      &add
      &pushone
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 12							//'debug'
      &dcallmp 13							// Trace()
      &pushnull
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 8							//'Object'
      &dcallmp 9							// registerClass()
      &pushregister 1
      &pushregister 2
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 17							//'DeleteFunctions'
    &function2  (r:2='obj') ()
      &pushsdb 18							//'DeleteFunctions:'
      &pushregister 2
      &pushsdbgm 19							//'_target'
      &add
      &pushone
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 12							//'debug'
      &dcallmp 13							// Trace()
      &pushregister 2
      &enumerateValue
     label1:
      &setRegister r:0
      &pushnull
      &equals
      &jnz label4      
      &pushregister 0
      &setRegister r:3
      &pop
      &pushregister 2
      &pushregister 3
      &getMember
      &typeof
      &setRegister r:1
      &pop
      &pushsdb 20							//'   '
      &pushregister 3
      &add
      &pushsdb 21							//':'
      &add
      &pushregister 1
      &add
      &pushone
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 12							//'debug'
      &dcallmp 13							// Trace()
      &pushregister 1
      &pushsdb 22							//'movieclip'
      &stringEq
      &not
      &jnz label2      
      &pushtrue
      &pushregister 2
      &pushregister 3
      &getMember
      &pushbyte 2
      &pushsdbgv 1							//'AptZombieClass'
      &dcallmp 17							// DeleteFunctions()
      &jmp label3      
     label2:
      &pushregister 1
      &pushsdb 23							//'function'
      &stringEq
      &not
      &jnz label3      
      &pushregister 2
      &pushregister 3
      &delete
      &pop
     label3:
      &jmp label1      
     label4:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 24							//'onUnload'
    &function2  () ()
      &pushzero
      &setRegister r:1
      &pop
     label5:
      &pushregister 1
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 14							//'s_objects'
      &pushsdbgm 25							//'length'
      &lessThan
      &not
      &jnz label6      
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label5      
     label6:
      &pushzero
      &setRegister r:1
      &pop
     label7:
      &pushregister 1
      &pushsdbgv 1							//'AptZombieClass'
      &pushsdbgm 6							//'s_classes'
      &pushsdbgm 25							//'length'
      &lessThan
      &not
      &jnz label8      
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label7      
     label8:
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 6							//'s_classes'
    &pushzero
    &pushsdb 26							//'Array'
    &new
    &setMember
    &pushregister 0
    &pushsdb 14							//'s_objects'
    &pushzero
    &pushsdb 26							//'Array'
    &new
    &setMember
    &pushregister 0
    &pushsdb 12							//'debug'
    &pushsdb 27							//'AptZombieClass::'
    &pushone
    &pushsdb 28							//'DebugClass'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'AptZombieClass'
    &pushsdbgm 4							//'prototype'
    &pushbyte 3
    &pushsdb 29							//'ASSetPropFlags'
    &callFunction
   label9:
    &pop
  &end // of initMovieClip 3

  &defineMovieClip 4 // total frames: 0
  &end // of defineMovieClip 4
  
  &exportAssets
    4 &as '__Packages.AptApiClass'
  &end // of exportAssets
  
  &initMovieClip 4
    &constants '_global', 'AptApiClass', 'm_codePrefix', 'Apt', '::', 'm_aptActive', 'InGame', '_target', 'RegisterScreen', 'Call', 'prototype', 'extern', 'Boolean', '__get__Active', 'AptCall ', '(', ')', 'debug', 'Trace', 'FSCommand:', 'CallAbs', 'CallAbs ', 'LoadVariables', 'Object', '_loadVariables', 'ConvertObjectMembers', 'LoadVariables:', 'Dump', ',', 'split', 'length', '=', 'Array', 'Number', 'ERROR: AptLoadVariables - Unable to convert : ', 'GetExtern', 'GetExtern(', ')=>', 'SetExtern', 'SetExtern(', 'ToggleExtern', 'PlaySound', 'FSCommand:PlaySound', 'IsCnc3Demo', 'Cnc3PcDemo', '0', 'IsSpecialEdition', '1', 'IsSpecialEditionExe', '============TESTING KANE', 'IsSpectatorClient', 'EnableEngineComponents', 'EnableEngineComponents:', 'FSCommand:EnableComponents', 'DisableEngineComponents', 'DisableEngineComponents:', 'FSCommand:DisableComponents', 'DoTrace', 'AptApiClass::', 'DebugClass', '', 'Active', 'addProperty', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'AptApiClass'
    &not
    &not
    &jnz label25    
    &pushglobalgv
    &pushsdb 1							//'AptApiClass'
    &function2  (r:3='className', r:2='screen') (r:1='this')
      &pushregister 1
      &pushsdb 2							//'m_codePrefix'
      &pushsdb 3							//'Apt'
      &pushregister 3
      &add
      &pushsdb 4							//'::'
      &add
      &setMember
      &pushsdbgv 1							//'AptApiClass'
      &pushsdb 5							//'m_aptActive'
      &pushzero
      &pushsdbgv 1							//'AptApiClass'
      &pushsdb 6							//'InGame'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdbgm 7							//'_target'
      &pushsdb 8							//'RegisterScreen'
      &pushbyte 2
      &pushregister 1
      &dcallmp 9							// Call()
    &end // of function 

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 10							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 0
    &pushsdb 6							//'InGame'
    &function  ('Void'    )    
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 5							//'m_aptActive'
      &pushundef
      &equals
      &not
      &jnz label1      
      &pushsdbgv 1							//'AptApiClass'
      &pushsdb 5							//'m_aptActive'
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushsdbgm 6							//'InGame'
      &toNumber
      &pushone
      &pushsdb 12							//'Boolean'
      &callFunction
      &setMember
     label1:
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 5							//'m_aptActive'
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 13							//'__get__Active'
    &function  (    )    
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 5							//'m_aptActive'
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 9							//'Call'
    &function2  (r:2='func', r:3='params') (r:1='this')
      &pushsdb 14							//'AptCall '
      &pushregister 1
      &pushsdbgm 2							//'m_codePrefix'
      &add
      &pushregister 2
      &add
      &pushsdb 15							//'('
      &add
      &pushregister 3
      &add
      &pushsdb 16							//')'
      &add
      &pushone
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
      &pushsdb 19							//'FSCommand:'
      &pushregister 1
      &pushsdbgm 2							//'m_codePrefix'
      &pushregister 2
      &add
      &concat
      &pushregister 3
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 20							//'CallAbs'
    &function2  (r:1='func', r:2='params') ()
      &pushsdb 16							//')'
      &pushregister 2
      &pushsdb 21							//'CallAbs '
      &pushregister 1
      &add
      &pushsdb 15							//'('
      &add
      &pushbyte 3
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
      &pushsdb 19							//'FSCommand:'
      &pushregister 1
      &concat
      &pushregister 2
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 22							//'LoadVariables'
    &function2  (r:4='varName', r:2='obj', r:3='convertStr') (r:1='this')
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 5							//'m_aptActive'
      &not
      &jnz label2      
      &pushzero
      &pushsdb 23							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 1
      &pushsdbgm 2							//'m_codePrefix'
      &pushregister 4
      &add
      &pushbyte 2
      &pushsdbgv 1							//'AptApiClass'
      &dcallmp 24							// _loadVariables()
     label2:
      &pushregister 3
      &pushundef
      &equals
      &not
      &not
      &jnz label3      
      &pushregister 3
      &pushregister 2
      &pushbyte 2
      &pushregister 1
      &dcallmp 25							// ConvertObjectMembers()
     label3:
      &pushsdb 26							//'LoadVariables:'
      &pushregister 4
      &add
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 27							// Dump()
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 25							//'ConvertObjectMembers'
    &function2  (r:3='obj', r:7='convertStr') ()
      &pushsdb 28							//','
      &pushone
      &pushregister 7
      &pushsdb 29							//'split'
      &callMethod
      &setRegister r:5
      &pop
      &pushregister 5
      &pushsdbgm 30							//'length'
      &setRegister r:6
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label4:
      &pushregister 2
      &pushregister 6
      &lessThan
      &not
      &jnz label12      
      &pushsdb 31							//'='
      &pushone
      &pushregister 5
      &pushregister 2
      &getMember
      &pushsdb 29							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushregister 4
      &pushzero
      &getMember
      &setRegister r:1
      &pop
      &pushregister 4
      &pushone
      &getMember
      &setRegister r:0
      &pushsdb 32							//'Array'
      &strictEquals
      &jnz label5      
      &pushregister 0
      &pushsdb 12							//'Boolean'
      &strictEquals
      &jnz label6      
      &pushregister 0
      &pushsdb 33							//'Number'
      &strictEquals
      &jnz label9      
      &jmp label10      
     label5:
      &pushregister 3
      &pushregister 1
      &pushsdb 28							//','
      &pushone
      &pushregister 3
      &pushregister 1
      &getMember
      &pushsdb 29							//'split'
      &callMethod
      &setMember
      &jmp label11      
     label6:
      &pushregister 3
      &pushregister 1
      &pushregister 3
      &pushregister 1
      &getMember
      &toNumber
      &jnz label7      
      &pushfalse
      &jmp label8      
     label7:
      &pushtrue
     label8:
      &setMember
      &jmp label11      
     label9:
      &pushregister 3
      &pushregister 1
      &pushregister 3
      &pushregister 1
      &getMember
      &toNumber
      &setMember
      &jmp label11      
     label10:
      &pushsdb 34							//'ERROR: AptLoadVariables - Unable to convert : '
      &pushregister 5
      &pushregister 2
      &getMember
      &add
      &trace
      &jmp label11      
     label11:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label4      
     label12:
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 24							//'_loadVariables'
    &function2  (r:2='varName', r:1='obj') ()
      &pushregister 2
      &pushregister 1
      &loadVariables
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 35							//'GetExtern'
    &function2  (r:3='ext', r:4='testDefault') (r:1='this')
      &pushundef
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdbgm 2							//'m_codePrefix'
      &pushregister 3
      &add
      &setRegister r:3
      &pop
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 5							//'m_aptActive'
      &not
      &jnz label13      
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &jmp label16      
     label13:
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushregister 3
      &getMember
      &pushundef
      &equals
      &not
      &jnz label14      
      &pushregister 4
      &setRegister r:2
      &pop
      &jmp label15      
     label14:
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
     label15:
      &pushsdb 36							//'GetExtern('
      &pushregister 3
      &add
      &pushsdb 37							//')=>'
      &add
      &pushregister 2
      &add
      &pushone
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
     label16:
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 38							//'SetExtern'
    &function2  (r:2='ext', r:3='to') (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_codePrefix'
      &pushregister 2
      &add
      &setRegister r:2
      &pop
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushregister 2
      &pushregister 3
      &setMember
      &pushsdb 39							//'SetExtern('
      &pushregister 2
      &add
      &pushsdb 37							//')=>'
      &add
      &pushregister 3
      &add
      &pushone
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 40							//'ToggleExtern'
    &function2  (r:3='ext') (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_codePrefix'
      &pushregister 3
      &add
      &setRegister r:3
      &pop
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushregister 3
      &getMember
      &toNumber
      &not
      &setRegister r:2
      &pop
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushregister 3
      &pushregister 2
      &setMember
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 41							//'PlaySound'
    &function2  (r:1='name') ()
      &pushsdb 42							//'FSCommand:PlaySound'
      &pushregister 1
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 43							//'IsCnc3Demo'
    &function2  (r:1='testDefault') ()
      &pushzero
      &pushsdbgv 1							//'AptApiClass'
      &pushsdb 6							//'InGame'
      &callMethod
      &jnz label17      
      &pushregister 1
      &jmp label18      
     label17:
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushsdbgm 44							//'Cnc3PcDemo'
      &pushsdb 45							//'0'
      &equals
      &not
     label18:
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 46							//'IsSpecialEdition'
    &function2  (r:1='testDefault') ()
      &pushzero
      &pushsdbgv 1							//'AptApiClass'
      &pushsdb 6							//'InGame'
      &callMethod
      &jnz label19      
      &pushregister 1
      &jmp label20      
     label19:
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushsdbgm 46							//'IsSpecialEdition'
      &pushsdb 47							//'1'
      &equals
     label20:
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 48							//'IsSpecialEditionExe'
    &function2  (r:1='testDefault') ()
      &pushsdb 49							//'============TESTING KANE'
      &trace
      &pushzero
      &pushsdbgv 1							//'AptApiClass'
      &pushsdb 6							//'InGame'
      &callMethod
      &jnz label21      
      &pushregister 1
      &jmp label22      
     label21:
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushsdbgm 48							//'IsSpecialEditionExe'
      &pushsdb 47							//'1'
      &equals
     label22:
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 50							//'IsSpectatorClient'
    &function2  (r:1='testDefault') ()
      &pushzero
      &pushsdbgv 1							//'AptApiClass'
      &pushsdb 6							//'InGame'
      &callMethod
      &jnz label23      
      &pushregister 1
      &jmp label24      
     label23:
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 11							//'extern'
      &pushsdbgm 50							//'IsSpectatorClient'
      &pushsdb 45							//'0'
      &equals
      &not
     label24:
      &return
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 51							//'EnableEngineComponents'
    &function2  (r:1='path') ()
      &pushsdb 52							//'EnableEngineComponents:'
      &pushregister 1
      &add
      &pushone
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
      &pushsdb 53							//'FSCommand:EnableComponents'
      &pushregister 1
      &getURL2
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 54							//'DisableEngineComponents'
    &function2  (r:1='path') ()
      &pushsdb 55							//'DisableEngineComponents:'
      &pushregister 1
      &add
      &pushone
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
      &pushsdb 56							//'FSCommand:DisableComponents'
      &pushregister 1
      &getURL2
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 11							//'extern'
    &pushsdb 57							//'DoTrace'
    &pushzero
    &pushsdb 6							//'InGame'
    &pushzero
    &pushsdb 44							//'Cnc3PcDemo'
    &pushzero
    &pushbyte 3
    &initObject
    &setMember
    &pushregister 0
    &pushsdb 17							//'debug'
    &pushsdb 58							//'AptApiClass::'
    &pushone
    &pushsdb 59							//'DebugClass'
    &new
    &setMember
    &pushregister 0
    &pushsdb 5							//'m_aptActive'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 2							//'m_codePrefix'
    &pushsdb 60							//''
    &setMember
    &function  (    )    
    &end // of function 

    &pushregister 1
    &pushsdbgm 13							//'__get__Active'
    &pushsdb 61							//'Active'
    &pushbyte 3
    &pushregister 1
    &pushsdb 62							//'addProperty'
    &callMethod
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'AptApiClass'
    &pushsdbgm 10							//'prototype'
    &pushbyte 3
    &pushsdb 63							//'ASSetPropFlags'
    &callFunction
   label25:
    &pop
  &end // of initMovieClip 4
&end
