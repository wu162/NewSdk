movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\INGAME~1\\INGAME~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'panel', '_x', 'panel._x = ', 'Call ', 'CodePrefix', '::', '(', ')', 'FSCommand:', 'Call(NP) ', 'Initialized', 'FSCommand:PlaySound', 'extern', 'InGame', 'InBetaDemo', 'InDreamMachineDemo', 'InitFunc', 'EnableComponents:', 'FSCommand:EnableComponents', 'DisableComponents:', 'FSCommand:DisableComponents', 'GetExtern(', ')=>', 'SetExtern(', '_parent', 'FindAbsolute', '_open', 'gotoAndPlay', '_close', '_reveal', 'mcInGameChat', 'Send', '_disabled', '_root', 'AptInGameChat', 'colorInnerFrameA', '3399FF', 'colorInnerFrameB', 'colorLine', 'colorGadgetBox', 'colorGadgetGameSpeed', 'colorTextDark', '0x57512B', 'colorTextBright', '0x31210F', 'colorSubTextBright', '0xC2F47D', 'colorSubTextDark', '0x57842B', 'colorTitleA', '0xCDE47E', 'tabTextColorDark', '0x123B15', 'tabTextColorBritght', '0x8ac14d', 'OnInitialized', 'GameCode', 'InitialSetup', 'OpenChat'  
    &function2 SetDisplayResolution (r:6='widthArg', r:5='heightArg') ()
      &pushregister 6
      &toNumber
      &setRegister r:1
      &pop
      &pushregister 5
      &toNumber
      &setRegister r:3
      &pop
      &pushshort 1024
      &setRegister r:2
      &pop
      &pushshort 768
      &setRegister r:4
      &pop
      &pushregister 1
      &pushregister 4
      &multiply
      &pushregister 3
      &pushregister 2
      &multiply
      &greaterThan
      &not
      &jnz label1      
      &pushregister 1
      &pushregister 4
      &pushregister 3
      &divide
      &multiply
      &setRegister r:1
      &pop
      &pushsdbgv 0							//'panel'
      &pushsdb 1							//'_x'
      &pushsdbgv 0							//'panel'
      &pushsdbgm 1							//'_x'
      &pushregister 1
      &pushregister 2
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &subtract
      &setMember
      &pushsdb 2							//'panel._x = '
      &pushsdbgv 0							//'panel'
      &pushsdbgm 1							//'_x'
      &toString
      &add
      &trace
     label1:
    &end // of function SetDisplayResolution

    &function2 GameCode (r:2='func', r:3='params') (r:1='_root')
      &pushsdb 3							//'Call '
      &pushregister 1
      &pushsdbgm 4							//'CodePrefix'
      &add
      &pushsdb 5							//'::'
      &add
      &pushregister 2
      &add
      &pushsdb 6							//'('
      &add
      &pushregister 3
      &add
      &pushsdb 7							//')'
      &add
      &trace
      &pushsdb 8							//'FSCommand:'
      &pushregister 1
      &pushsdbgm 4							//'CodePrefix'
      &pushsdb 5							//'::'
      &add
      &pushregister 2
      &add
      &concat
      &pushregister 3
      &getURL2
    &end // of function GameCode

    &function2 GameCodeNoPrefix (r:1='func', r:2='params') ()
      &pushsdb 9							//'Call(NP) '
      &pushregister 1
      &add
      &pushsdb 6							//'('
      &add
      &pushregister 2
      &add
      &pushsdb 7							//')'
      &add
      &trace
      &pushsdb 8							//'FSCommand:'
      &pushregister 1
      &concat
      &pushregister 2
      &getURL2
    &end // of function GameCodeNoPrefix

    &function2 PlaySound (r:2='name') (r:1='_root')
      &pushregister 1
      &pushsdbgm 10							//'Initialized'
      &not
      &not
      &jnz label2      
      &pushundef
      &return
     label2:
      &pushsdb 11							//'FSCommand:PlaySound'
      &pushregister 2
      &getURL2
    &end // of function PlaySound

    &function2 InitialSetup () (r:1='_root', r:2='_global')
      &pushregister 1
      &pushsdb 10							//'Initialized'
      &pushfalse
      &setMember
      &pushsdbgv 12							//'extern'
      &pushsdbgm 13							//'InGame'
      &not
      &jnz label3      
      &pushregister 2
      &pushsdb 13							//'InGame'
      &pushtrue
      &setMember
      &pushregister 2
      &pushsdb 14							//'InBetaDemo'
      &pushundef
      &setMember
      &pushregister 2
      &pushsdb 15							//'InDreamMachineDemo'
      &pushundef
      &setMember
     label3:
      &pushregister 2
      &pushsdbgm 14							//'InBetaDemo'
      &pushundef
      &equals
      &dup
      &not
      &jnz label4      
      &pop
      &pushsdbgv 12							//'extern'
      &pushsdbgm 14							//'InBetaDemo'
     label4:
      &not
      &jnz label5      
      &pushregister 2
      &pushsdb 14							//'InBetaDemo'
      &pushtrue
      &setMember
     label5:
      &pushregister 2
      &pushsdbgm 15							//'InDreamMachineDemo'
      &pushundef
      &equals
      &dup
      &not
      &jnz label6      
      &pop
      &pushsdbgv 12							//'extern'
      &pushsdbgm 15							//'InDreamMachineDemo'
     label6:
      &not
      &jnz label7      
      &pushregister 2
      &pushsdb 15							//'InDreamMachineDemo'
      &pushtrue
      &setMember
     label7:
      &pushzero
      &pushregister 1
      &dcallmp 16							// InitFunc()
      &pushregister 1
      &pushsdb 10							//'Initialized'
      &pushtrue
      &setMember
    &end // of function InitialSetup

    &function2 EnableComponents (r:1='path') ()
      &pushsdb 17							//'EnableComponents:'
      &pushregister 1
      &add
      &trace
      &pushsdb 18							//'FSCommand:EnableComponents'
      &pushregister 1
      &getURL2
    &end // of function EnableComponents

    &function2 DisableComponents (r:1='path') ()
      &pushsdb 19							//'DisableComponents:'
      &pushregister 1
      &add
      &trace
      &pushsdb 20							//'FSCommand:DisableComponents'
      &pushregister 1
      &getURL2
    &end // of function DisableComponents

    &function2 GetExtern (r:4='ext', r:5='testDefault') (r:1='_root', r:2='_global')
      &pushundef
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdbgm 13							//'InGame'
      &not
      &jnz label8      
      &pushsdbgv 12							//'extern'
      &pushregister 4
      &getMember
      &setRegister r:3
      &pop
      &jmp label11      
     label8:
      &pushregister 1
      &pushregister 4
      &getMember
      &pushundef
      &equals
      &not
      &jnz label9      
      &pushregister 5
      &setRegister r:3
      &pop
      &jmp label10      
     label9:
      &pushregister 1
      &pushregister 4
      &getMember
      &setRegister r:3
      &pop
     label10:
      &pushsdb 21							//'GetExtern('
      &pushregister 4
      &add
      &pushsdb 22							//')=>'
      &add
      &pushregister 3
      &add
      &trace
     label11:
      &pushregister 3
      &return
    &end // of function GetExtern

    &function2 SetExtern (r:4='ext', r:3='to') (r:1='_root', r:2='_global')
      &pushregister 2
      &pushsdbgm 13							//'InGame'
      &not
      &jnz label12      
      &pushsdbgv 12							//'extern'
      &pushregister 4
      &pushregister 3
      &setMember
      &jmp label13      
     label12:
      &pushregister 1
      &pushregister 4
      &pushregister 3
      &setMember
      &pushsdb 23							//'SetExtern('
      &pushregister 4
      &add
      &pushsdb 22							//')=>'
      &add
      &pushregister 3
      &add
      &trace
     label13:
    &end // of function SetExtern

    &function2 ToggleExtern (r:4='ext') (r:1='_root', r:2='_global')
      &pushundef
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdbgm 13							//'InGame'
      &not
      &jnz label14      
      &pushsdbgv 12							//'extern'
      &pushregister 4
      &getMember
      &toNumber
      &not
      &setRegister r:3
      &pop
      &pushsdbgv 12							//'extern'
      &pushregister 4
      &pushregister 3
      &setMember
      &jmp label15      
     label14:
      &pushregister 1
      &pushregister 4
      &getMember
      &not
      &setRegister r:3
      &pop
      &pushregister 1
      &pushregister 4
      &pushregister 3
      &setMember
     label15:
      &pushregister 3
      &return
    &end // of function ToggleExtern

    &function StopBackground (    )    
      &getURL 'FSCommand:SetBackground'       'off'      
    &end // of function StopBackground

    &function FadeInBackground (    )    
      &getURL 'FSCommand:SetBackground'       'fadein'      
    &end // of function FadeInBackground

    &function FadeOutBackground (    )    
      &getURL 'FSCommand:SetBackground'       'fadeout'      
    &end // of function FadeOutBackground

    &function2 FindAbsolute (r:2='of', r:1='from') ()
      &pushregister 1
      &pushsdbgm 24							//'_parent'
      &pushundef
      &equals
      &not
      &jnz label16      
      &pushregister 1
      &pushregister 2
      &getMember
      &return
     label16:
      &pushregister 1
      &pushsdbgm 24							//'_parent'
      &pushregister 2
      &pushbyte 2
      &pushsdb 25							//'FindAbsolute'
      &callFunction
      &pushregister 1
      &pushregister 2
      &getMember
      &add
      &return
    &end // of function FindAbsolute

    &function OpenChat (    )    
      &pushsdb 26							//'_open'
      &pushone
      &pushsdbgv 0							//'panel'
      &dcallmp 27							// gotoAndPlay()
    &end // of function OpenChat

    &function CloseChat (    )    
      &pushsdb 28							//'_close'
      &pushone
      &pushsdbgv 0							//'panel'
      &dcallmp 27							// gotoAndPlay()
    &end // of function CloseChat

    &function EnableSendButton (    )    
      &pushsdb 29							//'_reveal'
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 30							//'mcInGameChat'
      &pushsdbgm 31							//'Send'
      &dcallmp 27							// gotoAndPlay()
    &end // of function EnableSendButton

    &function DisableSendButton (    )    
      &pushsdb 32							//'_disabled'
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 30							//'mcInGameChat'
      &pushsdbgm 31							//'Send'
      &dcallmp 27							// gotoAndPlay()
    &end // of function DisableSendButton

    &pushsdbgv 33							//'_root'
    &pushsdb 4							//'CodePrefix'
    &pushsdb 34							//'AptInGameChat'
    &setMember
    &pushsdb 35							//'colorInnerFrameA'
    &pushsdb 36							//'3399FF'
    &setVariable
    &pushsdb 37							//'colorInnerFrameB'
    &pushsdb 36							//'3399FF'
    &setVariable
    &pushsdb 38							//'colorLine'
    &pushsdb 36							//'3399FF'
    &setVariable
    &pushsdb 39							//'colorGadgetBox'
    &pushsdb 36							//'3399FF'
    &setVariable
    &pushsdb 40							//'colorGadgetGameSpeed'
    &pushsdb 36							//'3399FF'
    &setVariable
    &pushsdb 41							//'colorTextDark'
    &pushsdb 42							//'0x57512B'
    &setVariable
    &pushsdb 43							//'colorTextBright'
    &pushsdb 44							//'0x31210F'
    &setVariable
    &pushsdb 45							//'colorSubTextBright'
    &pushsdb 46							//'0xC2F47D'
    &setVariable
    &pushsdb 47							//'colorSubTextDark'
    &pushsdb 48							//'0x57842B'
    &setVariable
    &pushsdb 49							//'colorTitleA'
    &pushsdb 50							//'0xCDE47E'
    &setVariable
    &pushsdb 51							//'tabTextColorDark'
    &pushsdb 52							//'0x123B15'
    &setVariable
    &pushsdb 53							//'tabTextColorBritght'
    &pushsdb 54							//'0x8ac14d'
    &setVariable
    &pushsdbgv 33							//'_root'
    &pushsdbgm 10							//'Initialized'
    &not
    &not
    &jnz label18    
    &pushsdbgv 33							//'_root'
    &pushsdbgm 16							//'InitFunc'
    &pushundef
    &equals
    &not
    &jnz label17    
    &pushsdbgv 33							//'_root'
    &pushsdb 16							//'InitFunc'
    &function  (    )    
      &pushsdb 55							//'OnInitialized'
      &pushone
      &dcallfp 56							// GameCode()
    &end // of function 

    &setMember
   label17:
    &pushzero
    &dcallfp 57							// InitialSetup()
   label18:
    &pushzero
    &dcallfp 58							// OpenChat()
  &end // of frame 0

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineButton 12
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
      &pushs 'Gui_ShellMapMouseOver'
      &pushone
      &pushsgv '_root'
      &pushs 'PlaySound'
      &callmp
    &end
  
    &on     &overDownToOverUp
      &pushthisgv
      &pushsgm 'gameCode'
      &pushone
      &pushsgv '_root'
      &pushs 'GameCode'
      &callmp
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overUpToOverDown
      &pushs 'Gui_ShellMapSelect'
      &pushone
      &pushsgv '_root'
      &pushs 'PlaySound'
      &callmp
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_rollout'
      &play
    &end
  &end // of defineButton 12

  &defineMovieClip 15 // total frames: 50
  &end // of defineMovieClip 15

  &defineMovieClip 16 // total frames: 175

    &placeMovieClip 10 
    
      &onClipEvent &load
        &pushthisgv
        &pushs '_visible'
        &pushzero
        &setMember
      &end
    &end // of placeMovieClip 10

    &frame 17
      &stop
    &end // of frame 17

    &frame 18
      &pushbyte 20
      &pushone
      &pushsgv 'overClip'
      &pushs 'gotoAndPlay'
      &callmp
    &end // of frame 18

    &frame 52
      &stop
    &end // of frame 52

    &frame 64
      &gotoLabel '_up'
      &play
    &end // of frame 64

    &frame 75
      &gotoLabel '_up'
      &play
      &pushzero
      &pushthisgv
      &pushs 'DoButtonCallBack'
      &callmp
    &end // of frame 75

    &frame 87
      &stop
    &end // of frame 87

    &frame 95
      &stop
    &end // of frame 95
  &end // of defineMovieClip 16

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18
  
  &importAssets &from 'GameWindowGadgets.swf'
    'TextEntry' &as 19
  &end // of importAssets

  &defineMovieClip 20 // total frames: 19

    &placeMovieClip 18 
    
      &onClipEvent &load
        &pushthisgv
        &pushs '_visible'
        &pushzero
        &setMember
      &end
    &end // of placeMovieClip 18

    &frame 1
      &pushsgv '_parent'
      &pushsgm 'isBuddyChat'
      &not
      &jnz label1      
      &stop
      &jmp label2      
     label1:
      &gotoLabel '_enabled'
      &play
     label2:
    &end // of frame 1

    &frame 8
      &stop
    &end // of frame 8

    &placeMovieClip 19 &as 'InGameChatEntry'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'TextEntry'
        &pushs '_Init'
        &pushssv 'AptInGameChat::InitGadgets'
        &pushs '_Load'
        &pushssv 'Apt/TextEntry.wnd'
      &end
    &end // of placeMovieClip 19
  &end // of defineMovieClip 20

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 24 // total frames: 1

    &frame 0
      &constants 'Send', 'buttonName', '$Send', 'Cancel', '$Cancel', 'DoButtonCallBack', '_up', 'gotoAndPlay', 'GameCode', '_disabled', 'Close', 'Closing', 'LabelToClose', 'CloseChat', 'Release', '_down', 'param', 'Gui_ShellMapSelect', '_root', 'PlaySound', 'Press', 'gotoAndStop', 'RollOver', '_over', 'Gui_ShellMapMouseOver', 'RollOut', '_rollout', 'Gui_ShellMapMouseOut'  
      &pushsdbgv 0							//'Send'
      &pushsdb 1							//'buttonName'
      &pushsdb 2							//'$Send'
      &setMember
      &pushsdbgv 3							//'Cancel'
      &pushsdb 1							//'buttonName'
      &pushsdb 4							//'$Cancel'
      &setMember
      &pushsdbgv 0							//'Send'
      &pushsdb 5							//'DoButtonCallBack'
      &function2  () (r:1='_root')
        &pushsdb 6							//'_up'
        &pushone
        &pushsdbgv 0							//'Send'
        &dcallmp 7							// gotoAndPlay()
        &pushsdb 0							//'Send'
        &pushone
        &pushregister 1
        &dcallmp 8							// GameCode()
      &end // of function 

      &setMember
      &pushsdbgv 3							//'Cancel'
      &pushsdb 5							//'DoButtonCallBack'
      &function2  () (r:1='_root', r:2='_global')
        &pushsdb 9							//'_disabled'
        &pushone
        &pushsdbgv 3							//'Cancel'
        &dcallmp 7							// gotoAndPlay()
        &pushsdb 10							//'Close'
        &pushone
        &pushregister 1
        &dcallmp 8							// GameCode()
        &pushsdb 11							//'Closing'
        &pushregister 2
        &pushsdbgm 12							//'LabelToClose'
        &add
        &trace
        &pushzero
        &pushregister 1
        &dcallmp 13							// CloseChat()
      &end // of function 

      &setMember
      &pushsdb 14							//'Release'
      &function  ('param'      )      
        &pushsdb 15							//'_down'
        &pushone
        &pushsdbgv 16							//'param'
        &getVariable
        &dcallmp 7							// gotoAndPlay()
        &pushsdb 17							//'Gui_ShellMapSelect'
        &pushone
        &pushsdbgv 18							//'_root'
        &dcallmp 19							// PlaySound()
      &end // of function 

      &setVariable
      &pushsdb 20							//'Press'
      &function  ('param'      )      
        &pushsdb 15							//'_down'
        &pushone
        &pushsdbgv 16							//'param'
        &getVariable
        &dcallmp 21							// gotoAndStop()
      &end // of function 

      &setVariable
      &pushsdb 22							//'RollOver'
      &function  ('param'      )      
        &pushsdb 23							//'_over'
        &pushone
        &pushsdbgv 16							//'param'
        &getVariable
        &dcallmp 7							// gotoAndPlay()
        &pushsdb 24							//'Gui_ShellMapMouseOver'
        &pushone
        &pushsdbgv 18							//'_root'
        &dcallmp 19							// PlaySound()
      &end // of function 

      &setVariable
      &pushsdb 25							//'RollOut'
      &function  ('param'      )      
        &pushsdb 26							//'_rollout'
        &pushone
        &pushsdbgv 16							//'param'
        &getVariable
        &dcallmp 7							// gotoAndPlay()
        &pushsdb 27							//'Gui_ShellMapMouseOut'
        &pushone
        &pushsdbgv 18							//'_root'
        &dcallmp 19							// PlaySound()
      &end // of function 

      &setVariable
      &stop
    &end // of frame 0

    &placeMovieClip 23 
    
      &onClipEvent &load
        &pushs 'text'
        &pushssv '$InGameChatHeading&dropshadow'
      &end
    &end // of placeMovieClip 23
  &end // of defineMovieClip 24

  &defineMovieClip 25 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
      &pushs 'OnClosed'
      &pushone
      &pushsgv '_root'
      &pushs 'GameCode'
      &callmp
    &end // of frame 19
  &end // of defineMovieClip 25
&end
