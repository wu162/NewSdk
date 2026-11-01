movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\LOADSC~3\\LOADSC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px
  
  &importAssets &from 'ShellContent.swf'
    'ShellContent_HideWideScreen' &as 1
  &end // of importAssets

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 1

    &frame 0
      &constants 'AddSegment', 'Frameupdate', 'OnDone', 'onUnload', '_x', 'vSpacingX', 'vDepth', '_name', '_', 'duplicateMovieClip', 'LoadScreenImage', 'GetPercentLoaded', 'vCurPercent', 'vNumSegments', 'Math', 'round', 'ProgressFull', 'vIntervalId', 'clearInterval', 'setInterval', 'seg', 'ProgressEmpty', 'FrameUpdate'  
      &function onUnload (      )      
        &pushsdb 0							//'AddSegment'
        &delete2
        &pop
        &pushsdb 1							//'Frameupdate'
        &delete2
        &pop
        &pushsdb 2							//'OnDone'
        &delete2
        &pop
        &pushsdb 3							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &function2 AddSegment (r:1='from', r:5='seg') ()
        &pushregister 1
        &pushsdbgm 4							//'_x'
        &pushregister 5
        &pushsdbgv 5							//'vSpacingX'
        &multiply
        &add
        &setRegister r:2
        &pop
        &pushsdb 4							//'_x'
        &pushregister 2
        &pushone
        &initObject
        &pushsdbgv 6							//'vDepth'
        &pushsdb 6							//'vDepth'
        &pushsdbgv 6							//'vDepth'
        &increment
        &setVariable
        &pushregister 1
        &pushsdbgm 7							//'_name'
        &pushsdb 8							//'_'
        &add
        &pushregister 5
        &add
        &pushbyte 3
        &pushregister 1
        &pushsdb 9							//'duplicateMovieClip'
        &callMethod
        &setRegister r:3
        &pop
      &end // of function AddSegment

      &function2 FrameUpdate () (r:1='_root')
        &pushfalse
        &setRegister r:4
        &pop
       label1:
        &pushregister 4
        &not
        &not
        &jnz label6        
        &pushtrue
        &setRegister r:4
        &pop
        &pushzero
        &pushregister 1
        &pushsdbgm 10							//'LoadScreenImage'
        &pushsdb 11							//'GetPercentLoaded'
        &callMethod
        &setRegister r:3
        &pop
        &pushsdbgv 12							//'vCurPercent'
        &pushregister 3
        &lessThan
        &dup
        &not
        &jnz label2        
        &pop
        &pushsdbgv 12							//'vCurPercent'
        &pushbyte 100
        &lessThan
       label2:
        &not
        &jnz label5        
        &pushsdbgv 13							//'vNumSegments'
        &pushsdbgv 12							//'vCurPercent'
        &multiply
        &pushbyte 100
        &divide
        &pushone
        &pushsdbgv 14							//'Math'
        &pushsdb 15							//'round'
        &callMethod
        &setRegister r:5
        &pop
        &pushsdb 12							//'vCurPercent'
        &pushsdbgv 12							//'vCurPercent'
        &increment
        &setVariable
        &pushsdbgv 13							//'vNumSegments'
        &pushsdbgv 12							//'vCurPercent'
        &multiply
        &pushbyte 100
        &divide
        &pushone
        &pushsdbgv 14							//'Math'
        &pushsdb 15							//'round'
        &callMethod
        &setRegister r:2
        &pop
        &pushregister 5
        &pushregister 2
        &equals
        &not
        &not
        &jnz label3        
        &pushregister 2
        &pushsdbgv 16							//'ProgressFull'
        &pushbyte 2
        &dcallfp 0							// AddSegment()
       label3:
        &pushsdbgv 12							//'vCurPercent'
        &pushbyte 100
        &equals
        &not
        &jnz label4        
        &pushsdbgv 17							//'vIntervalId'
        &pushone
        &dcallfp 18							// clearInterval()
        &pushsdb 17							//'vIntervalId'
        &pushshort 300
        &pushsdbgv 2							//'OnDone'
        &pushbyte 2
        &dcallfsv 19							// setInterval()
        &jmp label5        
       label4:
        &pushsdbgv 12							//'vCurPercent'
        &pushregister 3
        &lessThan
        &not
        &setRegister r:4
        &pop
       label5:
        &jmp label1        
       label6:
      &end // of function FrameUpdate

      &function2 OnDone () (r:1='_root')
        &pushsdbgv 17							//'vIntervalId'
        &pushone
        &dcallfp 18							// clearInterval()
        &pushzero
        &pushregister 1
        &pushsdbgm 10							//'LoadScreenImage'
        &dcallmp 2							// OnDone()
      &end // of function OnDone

      &pushsdb 12							//'vCurPercent'
      &pushzero
      &varEquals
      &pushsdb 13							//'vNumSegments'
      &pushbyte 70
      &varEquals
      &pushsdb 5							//'vSpacingX'
      &pushbyte 11
      &varEquals
      &pushsdb 6							//'vDepth'
      &pushbyte 100
      &varEquals
      &pushsdb 20							//'seg'
      &pushone
      &varEquals
     label7:
      &pushsdbgv 20							//'seg'
      &pushsdbgv 13							//'vNumSegments'
      &greaterThan
      &not
      &not
      &jnz label8      
      &pushsdbgv 20							//'seg'
      &pushsdbgv 21							//'ProgressEmpty'
      &pushbyte 2
      &dcallfp 0							// AddSegment()
      &pushsdb 20							//'seg'
      &pushsdbgv 20							//'seg'
      &increment
      &setVariable
      &jmp label7      
     label8:
      &pushsdb 17							//'vIntervalId'
      &pushbyte 100
      &pushsdbgv 22							//'FrameUpdate'
      &pushbyte 2
      &pushsdb 19							//'setInterval'
      &callFunction
      &varEquals
    &end // of frame 0
  &end // of defineMovieClip 11

  &defineMovieClip 15 // total frames: 1

    &frame 0
      &pushs 'vMissionName'
      &pushzero
      &pushsgv '_root'
      &pushsgm 'Campaign'
      &pushs 'GetCurrentMissionName'
      &callMethod
      &varEquals
    &end // of frame 0
  &end // of defineMovieClip 15

  &defineMovieClip 16 // total frames: 1

    &frame 0
      &pushsgv 'LoadingString'
      &pushone
      &pushsgv '_root'
      &pushsgm 'LoadScreenImage'
      &pushs 'RegisterLoadingString'
      &callmp
    &end // of frame 0

    &placeMovieClip 3 &as 'BackgroundImage'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImage'
        &pushs '_imageMap'
        &pushssv 'AptLoadScreenImage'
        &pushs '_mode'
        &pushssv 'SOLID'
      &end
    &end // of placeMovieClip 3
  &end // of defineMovieClip 16

  &defineMovieClip 17 // total frames: 15

    &frame 0
      &stop
    &end // of frame 0

    &frame 14
      &stop
    &end // of frame 14
  &end // of defineMovieClip 17
  
  &exportAssets
    17 &as 'LoadScreenImageClass'
  &end // of exportAssets
  
  &exportAssets
    17 &as 'LoadScreenImageClass'
  &end // of exportAssets

  &defineMovieClip 18 // total frames: 0
  &end // of defineMovieClip 18
  
  &exportAssets
    18 &as '__Packages.DebugClass'
  &end // of exportAssets
  
  &initMovieClip 18
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
  &end // of initMovieClip 18

  &defineMovieClip 19 // total frames: 0
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as '__Packages.LoadScreenImageClass'
  &end // of exportAssets
  
  &initMovieClip 19
    &constants '_global', 'LoadScreenImageClass', 'm_apt', 'm_classRoot', 'AptApiClass', 'm_percentLoaded', 'MovieClip', 'prototype', 'onUnload', 'debug', 'AptZombieClass', 'DeleteFunctions', 'Class', 'DeleteClass', 'OnDone', 'All done', 'OnLoadScreenDone', 'Call', 'RegisterLoadingString', 'm_loadingString', 'GetPercentLoaded', '_SetPercentLoaded', 'LoadScreenImage', 'DebugClass', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'LoadScreenImageClass'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'LoadScreenImageClass'
    &function2  () (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushregister 1
      &pushsdb 2							//'m_apt'
      &pushregister 1
      &pushsdbgv 1							//'LoadScreenImageClass'
      &pushsdbgm 3							//'m_classRoot'
      &pushbyte 2
      &pushsdb 4							//'AptApiClass'
      &new
      &setMember
      &pushregister 1
      &pushsdb 5							//'m_percentLoaded'
      &pushzero
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'LoadScreenImageClass'
    &pushsdbgv 6							//'MovieClip'
    &extends
    &pushregister 0
    &pushsdbgm 7							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 8							//'onUnload'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 2							//'m_apt'
      &delete
      &pop
      &pushsdbgv 1							//'LoadScreenImageClass'
      &pushsdb 9							//'debug'
      &delete
      &pop
      &pushregister 1
      &pushone
      &pushsdbgv 10							//'AptZombieClass'
      &dcallmp 11							// DeleteFunctions()
      &pushsdbgv 1							//'LoadScreenImageClass'
      &pushsdbgm 3							//'m_classRoot'
      &pushsdb 12							//'Class'
      &add
      &pushone
      &pushsdbgv 10							//'AptZombieClass'
      &dcallmp 13							// DeleteClass()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 14							//'OnDone'
    &function2  () (r:1='this')
      &pushsdb 15							//'All done'
      &trace
      &pushsdb 16							//'OnLoadScreenDone'
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_apt'
      &dcallmp 17							// Call()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 18							//'RegisterLoadingString'
    &function2  (r:2='loadingString') (r:1='this')
      &pushregister 1
      &pushsdb 19							//'m_loadingString'
      &pushregister 2
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 20							//'GetPercentLoaded'
    &function2  ('Void') (r:1='this')
      &pushregister 1
      &pushsdbgm 5							//'m_percentLoaded'
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 21							//'_SetPercentLoaded'
    &function2  (r:2='percent') (r:1='this')
      &pushregister 1
      &pushsdb 5							//'m_percentLoaded'
      &pushregister 2
      &toNumber
      &setMember
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 3							//'m_classRoot'
    &pushsdb 22							//'LoadScreenImage'
    &setMember
    &pushregister 1
    &pushsdb 19							//'m_loadingString'
    &pushundef
    &setMember
    &pushregister 0
    &pushsdb 9							//'debug'
    &pushsdbgv 1							//'LoadScreenImageClass'
    &pushsdbgm 3							//'m_classRoot'
    &pushone
    &pushsdb 23							//'DebugClass'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'LoadScreenImageClass'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 24							//'ASSetPropFlags'
    &callFunction
   label1:
    &pop
  &end // of initMovieClip 19

  &defineMovieClip 20 // total frames: 0
  &end // of defineMovieClip 20
  
  &exportAssets
    20 &as '__Packages.AptApiClass'
  &end // of exportAssets
  
  &initMovieClip 20
    &constants '_global', 'AptApiClass', 'm_codePrefix', 'Apt', '::', 'm_aptActive', 'InGame', '_target', 'RegisterScreen', 'Call', 'prototype', 'extern', 'Boolean', '__get__Active', 'AptCall ', '(', ')', 'debug', 'Trace', 'FSCommand:', 'CallAbs', 'CallAbs ', 'LoadVariables', 'Object', '_loadVariables', 'ConvertObjectMembers', 'LoadVariables:', 'Dump', ',', 'split', 'length', '=', 'Array', 'Number', 'ERROR: AptLoadVariables - Unable to convert : ', 'GetExtern', 'GetExtern(', ')=>', 'SetExtern', 'SetExtern(', 'ToggleExtern', 'PlaySound', 'FSCommand:PlaySound', 'IsCnc3Demo', 'Cnc3PcDemo', '0', 'EnableEngineComponents', 'EnableEngineComponents:', 'FSCommand:EnableComponents', 'DisableEngineComponents', 'DisableEngineComponents:', 'FSCommand:DisableComponents', 'DoTrace', 'AptApiClass::', 'DebugClass', '', 'Active', 'addProperty', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'AptApiClass'
    &not
    &not
    &jnz label19    
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
    &pushregister 0
    &pushsdb 46							//'EnableEngineComponents'
    &function2  (r:1='path') ()
      &pushsdb 47							//'EnableEngineComponents:'
      &pushregister 1
      &add
      &pushone
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
      &pushsdb 48							//'FSCommand:EnableComponents'
      &pushregister 1
      &getURL2
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 49							//'DisableEngineComponents'
    &function2  (r:1='path') ()
      &pushsdb 50							//'DisableEngineComponents:'
      &pushregister 1
      &add
      &pushone
      &pushsdbgv 1							//'AptApiClass'
      &pushsdbgm 17							//'debug'
      &dcallmp 18							// Trace()
      &pushsdb 51							//'FSCommand:DisableComponents'
      &pushregister 1
      &getURL2
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 11							//'extern'
    &pushsdb 52							//'DoTrace'
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
    &pushsdb 53							//'AptApiClass::'
    &pushone
    &pushsdb 54							//'DebugClass'
    &new
    &setMember
    &pushregister 0
    &pushsdb 5							//'m_aptActive'
    &pushundef
    &setMember
    &pushregister 1
    &pushsdb 2							//'m_codePrefix'
    &pushsdb 55							//''
    &setMember
    &function  (    )    
    &end // of function 

    &pushregister 1
    &pushsdbgm 13							//'__get__Active'
    &pushsdb 56							//'Active'
    &pushbyte 3
    &pushregister 1
    &pushsdb 57							//'addProperty'
    &callMethod
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'AptApiClass'
    &pushsdbgm 10							//'prototype'
    &pushbyte 3
    &pushsdb 58							//'ASSetPropFlags'
    &callFunction
   label19:
    &pop
  &end // of initMovieClip 20

  &defineMovieClip 21 // total frames: 0
  &end // of defineMovieClip 21
  
  &exportAssets
    21 &as '__Packages.AptZombieClass'
  &end // of exportAssets
  
  &initMovieClip 21
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
  &end // of initMovieClip 21
  
  &initMovieClip 17
    &constants 'LoadScreenImageClass', 'Object', 'registerClass'  
    &pushsdbgv 0							//'LoadScreenImageClass'
    &pushsdb 0							//'LoadScreenImageClass'
    &pushbyte 2
    &pushsdbgv 1							//'Object'
    &dcallmp 2							// registerClass()
  &end // of initMovieClip 17
&end
