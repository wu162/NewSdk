movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TAE42E~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 15, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 1 // total frames: 1
  &end // of defineMovieClip 1

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 1

    &frame 0
      &constants 'clanText', 'field', 'textColor', 'nameText', 'factionText', 'teamText', 'statusText', 'muteCheckbox', 'Pointer', 'encode', 'SetPlayerColor', 'GetMuteCheckbox', 'onUnload', 'initialized', 'InGameCheckbox.swf', 'loadMovie', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 SetPlayerColor (r:2='colorArg') ()
        &pushregister 2
        &toNumber
        &setRegister r:1
        &pop
        &pushsdbgv 0							//'clanText'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &setMember
        &pushsdbgv 3							//'nameText'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &setMember
        &pushsdbgv 4							//'factionText'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &setMember
        &pushsdbgv 5							//'teamText'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &setMember
        &pushsdbgv 6							//'statusText'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &setMember
      &end // of function SetPlayerColor

      &function GetMuteCheckbox (      )      
        &pushsdbgv 7							//'muteCheckbox'
        &pushone
        &pushsdbgv 8							//'Pointer'
        &pushsdb 9							//'encode'
        &callMethod
        &return
      &end // of function GetMuteCheckbox

      &function onUnload (      )      
        &pushsdb 10							//'SetPlayerColor'
        &delete2
        &pop
        &pushsdb 11							//'GetMuteCheckbox'
        &delete2
        &pop
        &pushsdb 12							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 13							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 14							//'InGameCheckbox.swf'
      &pushone
      &pushsdbgv 7							//'muteCheckbox'
      &dcallmp 15							// loadMovie()
      &pushsdb 13							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 16							//'FSCommand:'
      &pushsdb 17							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 19							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 20							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0

    &placeMovieClip 4 &as 'statusText'
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'Status'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&dropshadow'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 4

    &placeMovieClip 6 &as 'teamText'
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'TeamName'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&dropshadow'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 6

    &placeMovieClip 8 &as 'factionText'
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'FactionName'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&dropshadow'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 8

    &placeMovieClip 10 &as 'nameText'
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'PlayerName'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&dropshadow'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 10

    &placeMovieClip 12 &as 'clanText'
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'ClanId'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &pushs '&dropshadow'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 12
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'StatusItem'
  &end // of exportAssets

  &frame 0
    &constants 'closing', 'items', 'length', '_y', 'itemHeight', 'nextItemId', 'StatusItem', 'dialog', 'table', 'attachMovie', 'push', 'pop', 'removeMovieClip', 'Pointer', 'encode', 'SetStatusLineCount', 'GetStatusLine', 'Close', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'Array', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'extern', 'InGame', '9'  
    &function Close (    )    
      &pushsdbgv 0							//'closing'
      &not
      &not
      &jnz label1      
      &gotoLabel '_close'
      &play
      &pushsdb 0							//'closing'
      &pushtrue
      &setVariable
     label1:
    &end // of function Close

    &function2 SetStatusLineCount (r:7='countArg') ()
      &pushregister 7
      &toNumber
      &setRegister r:4
      &pop
      &pushsdbgv 1							//'items'
      &pushsdbgm 2							//'length'
      &pushregister 4
      &lessThan
      &not
      &jnz label4      
      &pushundef
      &setRegister r:5
      &pop
      &pushsdbgv 1							//'items'
      &pushsdbgm 2							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label2      
      &pushsdbgv 1							//'items'
      &pushsdbgv 1							//'items'
      &pushsdbgm 2							//'length'
      &pushone
      &subtract
      &getMember
      &setRegister r:6
      &pop
      &pushregister 6
      &pushsdbgm 3							//'_y'
      &pushsdbgv 4							//'itemHeight'
      &add
      &setRegister r:5
      &pop
      &jmp label3      
     label2:
      &pushzero
      &setRegister r:5
      &pop
     label3:
      &pushsdbgv 5							//'nextItemId'
      &pushsdb 5							//'nextItemId'
      &pushsdbgv 5							//'nextItemId'
      &increment
      &setVariable
      &setRegister r:1
      &pop
      &pushregister 1
      &pushregister 1
      &toString
      &pushsdb 6							//'StatusItem'
      &pushbyte 3
      &pushsdbgv 7							//'dialog'
      &pushsdbgm 8							//'table'
      &pushsdbgm 1							//'items'
      &pushsdb 9							//'attachMovie'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 3							//'_y'
      &pushregister 5
      &setMember
      &pushregister 5
      &pushsdbgv 4							//'itemHeight'
      &add
      &setRegister r:5
      &pop
      &pushregister 2
      &pushone
      &pushsdbgv 1							//'items'
      &dcallmp 10							// push()
      &pushsdbgv 1							//'items'
      &pushsdbgm 2							//'length'
      &pushregister 4
      &lessThan
      &jnz label3      
      &jmp label6      
     label4:
      &pushsdbgv 1							//'items'
      &pushsdbgm 2							//'length'
      &pushregister 4
      &greaterThan
      &not
      &jnz label6      
     label5:
      &pushzero
      &pushsdbgv 1							//'items'
      &pushsdb 11							//'pop'
      &callMethod
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &dcallmp 12							// removeMovieClip()
      &pushsdbgv 1							//'items'
      &pushsdbgm 2							//'length'
      &pushregister 4
      &greaterThan
      &jnz label5      
      &pushsdbgv 1							//'items'
      &pushsdbgm 2							//'length'
      &pushzero
      &equals
      &not
      &jnz label6      
      &pushsdb 5							//'nextItemId'
      &pushzerosv
     label6:
    &end // of function SetStatusLineCount

    &function2 GetStatusLine (r:3='indexArg') ()
      &pushregister 3
      &toNumber
      &setRegister r:1
      &pop
      &pushsdbgv 1							//'items'
      &pushregister 1
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushone
      &pushsdbgv 13							//'Pointer'
      &pushsdb 14							//'encode'
      &callMethod
      &return
    &end // of function GetStatusLine

    &function onUnload (    )    
      &pushsdb 15							//'SetStatusLineCount'
      &delete2
      &pop
      &pushsdb 16							//'GetStatusLine'
      &delete2
      &pop
      &pushsdb 17							//'Close'
      &delete2
      &pop
      &pushsdb 18							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 19							//'initialized'
    &not
    &not
    &jnz label7    
    &pushthisgv
    &pushone
    &pushsdbgv 21							//'_parent'
    &dcallmp 22							// OnContentLoaded()
    &pushsdb 4							//'itemHeight'
    &pushbyte 28
    &setVariable
    &pushsdb 1							//'items'
    &pushzero
    &pushsdb 23							//'Array'
    &new
    &setVariable
    &pushsdb 5							//'nextItemId'
    &pushzerosv
    &pushsdb 19							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 24							//'FSCommand:'
    &pushsdb 25							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 26							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 27							//''
    &getURL2
    &pushsdbgv 28							//'extern'
    &pushsdbgm 29							//'InGame'
    &not
    &not
    &jnz label7    
    &pushsdb 30							//'9'
    &pushone
    &dcallfp 15							// SetStatusLineCount()
   label7:
  &end // of frame 0
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'BlackRectangle' &as 14
  &end // of importAssets

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17
  
  &importAssets &from 'ShellButtons.swf'
    'ShellButtons Small' &as 18
  &end // of importAssets

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 22 // total frames: 1
  &end // of defineMovieClip 22

  &defineMovieClip 24 // total frames: 1
  &end // of defineMovieClip 24

  &defineMovieClip 26 // total frames: 1
  &end // of defineMovieClip 26

  &defineMovieClip 28 // total frames: 1
  &end // of defineMovieClip 28

  &defineMovieClip 29 // total frames: 1

    &placeMovieClip 20 
    
      &onClipEvent &load
        &constants 'text', '$', 'Mute', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Mute'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 20

    &placeMovieClip 22 
    
      &onClipEvent &load
        &constants 'text', '$', 'Status', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Status'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 22

    &placeMovieClip 24 
    
      &onClipEvent &load
        &constants 'text', '$', 'Team', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Team'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 24

    &placeMovieClip 26 
    
      &onClipEvent &load
        &constants 'text', '$', 'Faction', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Faction'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 26

    &placeMovieClip 28 
    
      &onClipEvent &load
        &constants 'text', '$', 'PlayerName', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'PlayerName'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 28
  &end // of defineMovieClip 29

  &defineMovieClip 32 // total frames: 1
  &end // of defineMovieClip 32

  &defineMovieClip 35 // total frames: 1
  &end // of defineMovieClip 35

  &defineMovieClip 36 // total frames: 1
  &end // of defineMovieClip 36

  &defineMovieClip 40 // total frames: 1
  &end // of defineMovieClip 40

  &defineMovieClip 43 // total frames: 1

    &frame 0
      &constants 'Close', 'OnCloseButtonClicked', 'onUnload', 'closeButton', 'PressCallback'  
      &function2 OnCloseButtonClicked () (r:1='_parent')
        &pushzero
        &pushregister 1
        &dcallmp 0							// Close()
      &end // of function OnCloseButtonClicked

      &function onUnload (      )      
        &pushsdb 1							//'OnCloseButtonClicked'
        &delete2
        &pop
        &pushsdb 2							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 3							//'closeButton'
      &pushsdb 4							//'PressCallback'
      &pushsdbgv 1							//'OnCloseButtonClicked'
      &setMember
    &end // of frame 0

    &placeMovieClip 14 
    
      &onClipEvent &load
        &constants '_root', 'displayLeft', 'left', 'top', 'displayTop', 'ancestor', '_parent', '_x', '_y', '', 'displayWidth', 'displayHeight'  
        &pushsdbgv 0							//'_root'
        &pushsdbgm 1							//'displayLeft'
        &pushundef
        &equals
        &not
        &not
        &jnz label3        
        &pushsdb 2							//'left'
        &pushsdbgv 0							//'_root'
        &pushsdbgm 1							//'displayLeft'
        &varEquals
        &pushsdb 3							//'top'
        &pushsdbgv 0							//'_root'
        &pushsdbgm 4							//'displayTop'
        &varEquals
        &pushsdb 5							//'ancestor'
        &pushsdbgv 6							//'_parent'
        &varEquals
       label1:
        &pushsdbgv 5							//'ancestor'
        &pushsdbgv 0							//'_root'
        &equals
        &not
        &not
        &jnz label2        
        &pushsdb 2							//'left'
        &pushsdbgv 2							//'left'
        &pushsdbgv 5							//'ancestor'
        &pushsdbgm 7							//'_x'
        &subtract
        &setVariable
        &pushsdb 3							//'top'
        &pushsdbgv 3							//'top'
        &pushsdbgv 5							//'ancestor'
        &pushsdbgm 8							//'_y'
        &subtract
        &setVariable
        &pushsdb 5							//'ancestor'
        &pushsdbgv 5							//'ancestor'
        &pushsdbgm 6							//'_parent'
        &setVariable
        &jmp label1        
       label2:
        &pushsdb 9							//''
        &pushzero
        &pushsdbgv 2							//'left'
        &setProperty
        &pushsdb 9							//''
        &pushone
        &pushsdbgv 3							//'top'
        &setProperty
        &pushsdb 9							//''
        &pushbyte 8
        &pushsdbgv 0							//'_root'
        &pushsdbgm 10							//'displayWidth'
        &setProperty
        &pushsdb 9							//''
        &pushbyte 9
        &pushsdbgv 0							//'_root'
        &pushsdbgm 11							//'displayHeight'
        &setProperty
       label3:
      &end
    &end // of placeMovieClip 14

    &placeMovieClip 18 &as 'closeButton'
    
      &onClipEvent &construct
        &pushs 'vStartMode'
        &pushssv ''
      &end
    
      &onClipEvent &load
        &constants '$', 'CloseButtonLabel', '_parent', 'qualifyName', 'SetText'  
        &pushsdb 0							//'$'
        &pushsdb 1							//'CloseButtonLabel'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &pushsdb 3							//'qualifyName'
        &callFunction
        &add
        &pushone
        &dcallfp 4							// SetText()
      &end
    &end // of placeMovieClip 18

    &placeMovieClip 40 
    
      &onClipEvent &load
        &constants 'text', '$', 'Title', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Title'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 40
  &end // of defineMovieClip 43

  &defineMovieClip 46 // total frames: 1
  &end // of defineMovieClip 46

  &frame 11
    &stop
  &end // of frame 11

  &frame 14
    &stop
    &pushs 'FSCommand:'
    &pushs 'OnClosed'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
  &end // of frame 14
&end
