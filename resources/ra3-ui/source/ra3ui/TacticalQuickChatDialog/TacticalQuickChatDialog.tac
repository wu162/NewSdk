movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TacticalQuickChatDialog\\TacticalQuickChatDialog.eaf' &compressed // flash 7, total frames: 16, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 4 // total frames: 2

    &frame 0
      &constants 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushthisgv
      &pushone
      &pushsdbgv 2							//'_parent'
      &dcallmp 3							// OnContentLoaded()
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 4							//'FSCommand:'
      &pushsdb 5							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 6							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 7							//''
      &getURL2
     label1:
    &end // of frame 0

    &placeMovieClip 3 &as 'text'
    &end // of placeMovieClip 3

    &frame 1
      &stop
    &end // of frame 1
  &end // of defineMovieClip 4
  
  &exportAssets
    4 &as 'MenuItem'
  &end // of exportAssets

  &frame 0
    &constants 'panelBorderBottom', '_y', 'panelBorderTop', 'numItems', 'MenuItem', 'panel', 'BaseMenuItem', 'attachMovie', 'text', '$', 'ItemText', 'qualifyName', '_height', 'DialogBackground', 'removeMovieClip', 'Pointer', 'encode', 'closing', 'SetMenuItemCount', 'ClearItems', 'GetItem', 'Close', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', ''  
    &function2 SetMenuItemCount (r:4='itemCount') ()
      &pushsdbgv 0							//'panelBorderBottom'
      &pushsdb 1							//'_y'
      &pushsdbgv 2							//'panelBorderTop'
      &pushsdbgm 1							//'_y'
      &pushbyte 5
      &add
      &setMember
      &pushsdbgv 3							//'numItems'
      &pushregister 4
      &lessThan
      &not
      &jnz label3      
      &pushsdbgv 3							//'numItems'
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushregister 4
      &lessThan
      &not
      &jnz label2      
      &pushregister 2
      &pushregister 2
      &toString
      &pushsdb 4							//'MenuItem'
      &pushbyte 3
      &pushsdbgv 5							//'panel'
      &pushsdbgm 6							//'BaseMenuItem'
      &pushsdb 7							//'attachMovie'
      &callMethod
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgm 8							//'text'
      &pushsdb 8							//'text'
      &pushsdb 9							//'$'
      &pushsdb 10							//'ItemText'
      &pushregister 1
      &pushbyte 2
      &pushsdb 11							//'qualifyName'
      &callFunction
      &add
      &setMember
      &pushregister 1
      &pushsdb 1							//'_y'
      &pushregister 1
      &pushsdbgm 12							//'_height'
      &pushregister 2
      &multiply
      &setMember
      &pushsdbgv 5							//'panel'
      &pushsdbgm 13							//'DialogBackground'
      &pushsdb 12							//'_height'
      &pushsdbgv 5							//'panel'
      &pushsdbgm 13							//'DialogBackground'
      &pushsdbgm 12							//'_height'
      &pushregister 1
      &pushsdbgm 12							//'_height'
      &add
      &setMember
      &pushsdbgv 0							//'panelBorderBottom'
      &pushsdb 1							//'_y'
      &pushsdbgv 0							//'panelBorderBottom'
      &pushsdbgm 1							//'_y'
      &pushregister 1
      &pushsdbgm 12							//'_height'
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label1      
     label2:
      &pushsdb 3							//'numItems'
      &pushregister 4
      &setVariable
      &jmp label6      
     label3:
      &pushsdbgv 3							//'numItems'
      &pushregister 4
      &greaterThan
      &not
      &jnz label6      
      &pushsdbgv 3							//'numItems'
      &setRegister r:2
      &pop
     label4:
      &pushregister 2
      &pushregister 4
      &greaterThan
      &not
      &jnz label5      
      &pushsdbgv 5							//'panel'
      &pushsdbgm 6							//'BaseMenuItem'
      &pushregister 2
      &pushone
      &subtract
      &toString
      &getMember
      &setRegister r:3
      &pop
      &pushsdbgv 5							//'panel'
      &pushsdbgm 13							//'DialogBackground'
      &pushsdb 12							//'_height'
      &pushsdbgv 5							//'panel'
      &pushsdbgm 13							//'DialogBackground'
      &pushsdbgm 12							//'_height'
      &pushregister 3
      &pushsdbgm 12							//'_height'
      &subtract
      &setMember
      &pushzero
      &pushregister 3
      &dcallmp 14							// removeMovieClip()
      &pushregister 2
      &decrement
      &setRegister r:2
      &pop
      &jmp label4      
     label5:
      &pushsdb 3							//'numItems'
      &pushregister 4
      &setVariable
     label6:
    &end // of function SetMenuItemCount

    &function2 ClearMenuItems () ()
     label7:
      &pushsdbgv 3							//'numItems'
      &pushzero
      &greaterThan
      &not
      &jnz label8      
      &pushsdbgv 5							//'panel'
      &pushsdbgm 6							//'BaseMenuItem'
      &pushsdbgv 3							//'numItems'
      &pushone
      &subtract
      &toString
      &getMember
      &setRegister r:1
      &pop
      &pushsdbgv 5							//'panel'
      &pushsdbgm 13							//'DialogBackground'
      &pushsdb 12							//'_height'
      &pushsdbgv 5							//'panel'
      &pushsdbgm 13							//'DialogBackground'
      &pushsdbgm 12							//'_height'
      &pushregister 1
      &pushsdbgm 12							//'_height'
      &subtract
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 14							// removeMovieClip()
      &pushsdb 3							//'numItems'
      &pushsdbgv 3							//'numItems'
      &decrement
      &setVariable
      &jmp label7      
     label8:
    &end // of function ClearMenuItems

    &function2 GetMenuItem (r:1='itemNum') ()
      &pushsdbgv 5							//'panel'
      &pushsdbgm 6							//'BaseMenuItem'
      &pushregister 1
      &toString
      &getMember
      &pushone
      &pushsdbgv 15							//'Pointer'
      &pushsdb 16							//'encode'
      &callMethod
      &return
    &end // of function GetMenuItem

    &function Close (    )    
      &pushsdbgv 17							//'closing'
      &not
      &not
      &jnz label9      
      &pushsdb 17							//'closing'
      &pushtrue
      &setVariable
      &gotoLabel '_close'
      &play
     label9:
    &end // of function Close

    &function onUnload (    )    
      &pushsdb 18							//'SetMenuItemCount'
      &delete2
      &pop
      &pushsdb 19							//'ClearItems'
      &delete2
      &pop
      &pushsdb 20							//'GetItem'
      &delete2
      &pop
      &pushsdb 21							//'Close'
      &delete2
      &pop
      &pushsdb 22							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 23							//'initialized'
    &not
    &not
    &jnz label10    
    &pushthisgv
    &pushone
    &pushsdbgv 25							//'_parent'
    &dcallmp 26							// OnContentLoaded()
    &pushsdb 17							//'closing'
    &pushfalse
    &setVariable
    &pushsdb 3							//'numItems'
    &pushzerosv
    &pushsdb 23							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 27							//'FSCommand:'
    &pushsdb 28							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 11							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 29							//''
    &getURL2
   label10:
  &end // of frame 0

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14

  &frame 9
    &stop
  &end // of frame 9

  &frame 15
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
  &end // of frame 15
&end
