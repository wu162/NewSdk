movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TADDB0~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'numRowsInFirstColumn', 'buttonSlotHeight', 'GetNumRowsInColumn', 'row', 'col', 'nextButtonSlotId', 'LoadMovieStage', 'attachMovie', '_x', '_y', 'helpSite', 'HelpBoxSite', 'buttonStageWidth', 'vertAlignment', 'top', 'horzAlignment', 'left', 'stage', 'buttonSlots', 'splice', 'FSCommand:', 'OnButtonSlotStageCreated', 'qualifyName', 'index=', '&name=', 'Pointer', 'encode', 'ComputeSlotPositionFromIndex', 'GetColumnVerticalOffset', 'columnHorizontalOffset', 'CreateNewButtonSlot', 'length', 'MoveButtonSlot', 'removeMovieClip', 'pop', 'InsertButtonSlot', 'EraseButtonSlot', 'SetButtonSlotCount', 'onUnload', 'initialized', 'buttonStageHeight', 'buttonSlotWidth', 'Array', 'OnInitialized', 'this', '', 'extern', 'InGame'  
    &function GetNumRowsInColumn ('col'    )    
      &pushsdbgv 0							//'numRowsInFirstColumn'
      &return
    &end // of function GetNumRowsInColumn

    &function2 GetColumnVerticalOffset (r:1='col') ()
      &pushsdbgv 1							//'buttonSlotHeight'
      &pushsdbgv 0							//'numRowsInFirstColumn'
      &pushregister 1
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &subtract
      &multiply
      &pushfloat 0.500000000000000
      &multiply
      &return
    &end // of function GetColumnVerticalOffset

    &function2 ComputeSlotPositionFromIndex (r:4='index') ()
      &pushsdbgv 0							//'numRowsInFirstColumn'
      &setRegister r:1
      &pop
      &pushzero
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushregister 1
      &lessThan
      &not
      &not
      &jnz label2      
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 2
      &pushregister 1
      &subtract
      &setRegister r:2
      &pop
      &jmp label1      
     label2:
      &pushsdb 3							//'row'
      &pushregister 2
      &pushsdb 4							//'col'
      &pushregister 3
      &pushbyte 2
      &initObject
      &return
    &end // of function ComputeSlotPositionFromIndex

    &function2 CreateNewButtonSlot (r:7='slotIndex', r:11='slotX', r:10='slotY') (r:1='this')
      &pushsdbgv 5							//'nextButtonSlotId'
      &pushsdb 5							//'nextButtonSlotId'
      &pushsdbgv 5							//'nextButtonSlotId'
      &increment
      &setVariable
      &setRegister r:2
      &pop
      &pushregister 2
      &toString
      &setRegister r:5
      &pop
      &pushshort 2048
      &pushregister 2
      &subtract
      &pushregister 2
      &toString
      &pushsdb 6							//'LoadMovieStage'
      &pushbyte 3
      &dcallfp 7							// attachMovie()
      &pushregister 1
      &pushregister 5
      &getMember
      &setRegister r:4
      &pop
      &pushregister 4
      &pushsdb 8							//'_x'
      &pushregister 11
      &setMember
      &pushregister 4
      &pushsdb 9							//'_y'
      &pushregister 10
      &setMember
      &pushsdb 10							//'helpSite'
      &pushregister 2
      &toString
      &add
      &setRegister r:5
      &pop
      &pushregister 2
      &pushlong 65536
      &add
      &pushsdb 10							//'helpSite'
      &pushregister 2
      &toString
      &add
      &pushsdb 11							//'HelpBoxSite'
      &pushbyte 3
      &dcallfp 7							// attachMovie()
      &pushregister 1
      &pushregister 5
      &getMember
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 8							//'_x'
      &pushregister 11
      &pushsdbgv 12							//'buttonStageWidth'
      &add
      &setMember
      &pushregister 3
      &pushsdb 9							//'_y'
      &pushregister 10
      &setMember
      &pushregister 3
      &pushsdb 13							//'vertAlignment'
      &pushsdb 14							//'top'
      &setMember
      &pushregister 3
      &pushsdb 15							//'horzAlignment'
      &pushsdb 16							//'left'
      &setMember
      &pushregister 4
      &pushsdb 10							//'helpSite'
      &pushregister 3
      &setMember
      &pushsdb 17							//'stage'
      &pushregister 4
      &pushsdb 10							//'helpSite'
      &pushregister 3
      &pushbyte 2
      &initObject
      &setRegister r:6
      &pop
      &pushregister 6
      &pushzero
      &pushregister 7
      &pushbyte 3
      &pushsdbgv 18							//'buttonSlots'
      &dcallmp 19							// splice()
      &pushsdb 20							//'FSCommand:'
      &pushsdb 21							//'OnButtonSlotStageCreated'
      &pushregister 1
      &pushbyte 2
      &pushsdb 22							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 23							//'index='
      &pushregister 7
      &add
      &pushsdb 24							//'&name='
      &add
      &pushregister 4
      &pushone
      &pushsdbgv 25							//'Pointer'
      &pushsdb 26							//'encode'
      &callMethod
      &add
      &getURL2
      &pushregister 6
      &return
    &end // of function CreateNewButtonSlot

    &function2 MoveButtonSlot (r:6='slotIndex', r:5='slotX', r:4='slotY') ()
      &pushsdbgv 18							//'buttonSlots'
      &pushregister 6
      &getMember
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgm 17							//'stage'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 8							//'_x'
      &pushregister 5
      &setMember
      &pushregister 3
      &pushsdb 9							//'_y'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'helpSite'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 8							//'_x'
      &pushregister 5
      &pushsdbgv 12							//'buttonStageWidth'
      &add
      &setMember
      &pushregister 2
      &pushsdb 9							//'_y'
      &pushregister 4
      &setMember
    &end // of function MoveButtonSlot

    &function2 InsertButtonSlot (r:3='index') ()
      &pushregister 3
      &pushone
      &pushsdb 27							//'ComputeSlotPositionFromIndex'
      &callFunction
      &setRegister r:7
      &pop
      &pushregister 7
      &pushsdbgm 4							//'col'
      &setRegister r:1
      &pop
      &pushregister 7
      &pushsdbgm 3							//'row'
      &setRegister r:2
      &pop
      &pushregister 1
      &pushone
      &pushsdb 28							//'GetColumnVerticalOffset'
      &callFunction
      &pushregister 2
      &pushsdbgv 1							//'buttonSlotHeight'
      &multiply
      &add
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgv 29							//'columnHorizontalOffset'
      &multiply
      &setRegister r:5
      &pop
      &pushregister 4
      &pushregister 5
      &pushregister 3
      &pushbyte 3
      &dcallfp 30							// CreateNewButtonSlot()
      &pushregister 1
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:6
      &pop
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
     label3:
      &pushregister 3
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &lessThan
      &not
      &jnz label6      
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 6
      &lessThan
      &not
      &jnz label4      
      &pushregister 4
      &pushsdbgv 1							//'buttonSlotHeight'
      &add
      &setRegister r:4
      &pop
      &jmp label5      
     label4:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushregister 1
      &pushone
      &pushsdb 28							//'GetColumnVerticalOffset'
      &callFunction
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgv 29							//'columnHorizontalOffset'
      &add
      &setRegister r:5
      &pop
      &pushregister 1
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:6
      &pop
     label5:
      &pushregister 4
      &pushregister 5
      &pushregister 3
      &pushbyte 3
      &dcallfp 32							// MoveButtonSlot()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label3      
     label6:
    &end // of function InsertButtonSlot

    &function2 EraseButtonSlot (r:1='index') ()
      &pushsdbgv 18							//'buttonSlots'
      &pushregister 1
      &getMember
      &setRegister r:9
      &pop
      &pushregister 9
      &pushsdbgm 17							//'stage'
      &setRegister r:7
      &pop
      &pushregister 1
      &pushone
      &pushsdb 27							//'ComputeSlotPositionFromIndex'
      &callFunction
      &setRegister r:8
      &pop
      &pushregister 8
      &pushsdbgm 4							//'col'
      &setRegister r:2
      &pop
      &pushregister 8
      &pushsdbgm 3							//'row'
      &setRegister r:3
      &pop
      &pushregister 7
      &pushsdbgm 8							//'_x'
      &setRegister r:5
      &pop
      &pushregister 7
      &pushsdbgm 9							//'_y'
      &setRegister r:4
      &pop
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 18							//'buttonSlots'
      &dcallmp 19							// splice()
      &pushzero
      &pushregister 7
      &dcallmp 33							// removeMovieClip()
      &pushzero
      &pushregister 9
      &pushsdbgm 10							//'helpSite'
      &dcallmp 33							// removeMovieClip()
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label12      
      &pushregister 1
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &lessThan
      &not
      &jnz label11      
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:6
      &pop
     label7:
      &pushfalse
      &jnz label11      
      &pushregister 4
      &pushregister 5
      &pushregister 1
      &pushbyte 3
      &dcallfp 32							// MoveButtonSlot()
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &lessThan
      &not
      &not
      &jnz label8      
      &jmp label11      
     label8:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 6
      &lessThan
      &not
      &jnz label9      
      &pushregister 4
      &pushsdbgv 1							//'buttonSlotHeight'
      &add
      &setRegister r:4
      &pop
      &jmp label10      
     label9:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:3
      &pop
      &pushregister 2
      &pushone
      &pushsdb 28							//'GetColumnVerticalOffset'
      &callFunction
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgv 29							//'columnHorizontalOffset'
      &add
      &setRegister r:5
      &pop
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:6
      &pop
     label10:
      &jmp label7      
     label11:
      &jmp label13      
     label12:
      &pushsdb 5							//'nextButtonSlotId'
      &pushzerosv
     label13:
    &end // of function EraseButtonSlot

    &function2 SetButtonSlotCount (r:6='newCount') ()
      &pushregister 6
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &lessThan
      &not
      &jnz label16      
     label14:
      &pushzero
      &pushsdbgv 18							//'buttonSlots'
      &pushsdb 34							//'pop'
      &callMethod
      &setRegister r:1
      &pop
      &pushzero
      &pushregister 1
      &pushsdbgm 17							//'stage'
      &dcallmp 33							// removeMovieClip()
      &pushzero
      &pushregister 1
      &pushsdbgm 10							//'helpSite'
      &dcallmp 33							// removeMovieClip()
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &pushregister 6
      &greaterThan
      &jnz label14      
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &pushzero
      &equals
      &not
      &jnz label15      
      &pushsdb 5							//'nextButtonSlotId'
      &pushzerosv
     label15:
      &jmp label21      
     label16:
      &pushregister 6
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &greaterThan
      &not
      &jnz label21      
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &pushone
      &pushsdb 27							//'ComputeSlotPositionFromIndex'
      &callFunction
      &setRegister r:8
      &pop
      &pushregister 8
      &pushsdbgm 4							//'col'
      &setRegister r:2
      &pop
      &pushregister 8
      &pushsdbgm 3							//'row'
      &setRegister r:3
      &pop
      &pushregister 2
      &pushone
      &pushsdb 28							//'GetColumnVerticalOffset'
      &callFunction
      &pushregister 3
      &pushsdbgv 1							//'buttonSlotHeight'
      &multiply
      &add
      &setRegister r:4
      &pop
      &pushregister 2
      &pushsdbgv 29							//'columnHorizontalOffset'
      &multiply
      &setRegister r:5
      &pop
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:7
      &pop
     label17:
      &pushfalse
      &jnz label21      
      &pushregister 4
      &pushregister 5
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &pushbyte 3
      &dcallfp 30							// CreateNewButtonSlot()
      &pushsdbgv 18							//'buttonSlots'
      &pushsdbgm 31							//'length'
      &pushregister 6
      &lessThan
      &not
      &not
      &jnz label18      
      &jmp label21      
     label18:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 7
      &lessThan
      &not
      &jnz label19      
      &pushregister 4
      &pushsdbgv 1							//'buttonSlotHeight'
      &add
      &setRegister r:4
      &pop
      &jmp label20      
     label19:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:3
      &pop
      &pushregister 2
      &pushone
      &pushsdb 28							//'GetColumnVerticalOffset'
      &callFunction
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgv 29							//'columnHorizontalOffset'
      &add
      &setRegister r:5
      &pop
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:7
      &pop
     label20:
      &jmp label17      
     label21:
    &end // of function SetButtonSlotCount

    &function onUnload (    )    
      &pushsdb 2							//'GetNumRowsInColumn'
      &delete2
      &pop
      &pushsdb 28							//'GetColumnVerticalOffset'
      &delete2
      &pop
      &pushsdb 27							//'ComputeSlotPositionFromIndex'
      &delete2
      &pop
      &pushsdb 30							//'CreateNewButtonSlot'
      &delete2
      &pop
      &pushsdb 32							//'MoveButtonSlot'
      &delete2
      &pop
      &pushsdb 35							//'InsertButtonSlot'
      &delete2
      &pop
      &pushsdb 36							//'EraseButtonSlot'
      &delete2
      &pop
      &pushsdb 37							//'SetButtonSlotCount'
      &delete2
      &pop
      &pushsdb 38							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 39							//'initialized'
    &not
    &not
    &jnz label22    
    &pushsdb 12							//'buttonStageWidth'
    &pushbyte 62
    &setVariable
    &pushsdb 40							//'buttonStageHeight'
    &pushbyte 62
    &setVariable
    &pushsdb 41							//'buttonSlotWidth'
    &pushsdbgv 12							//'buttonStageWidth'
    &pushone
    &subtract
    &setVariable
    &pushsdb 1							//'buttonSlotHeight'
    &pushsdbgv 40							//'buttonStageHeight'
    &pushone
    &subtract
    &setVariable
    &pushsdb 29							//'columnHorizontalOffset'
    &pushsdbgv 41							//'buttonSlotWidth'
    &setVariable
    &pushsdb 0							//'numRowsInFirstColumn'
    &pushbyte 6
    &setVariable
    &pushsdb 18							//'buttonSlots'
    &pushzero
    &pushsdb 42							//'Array'
    &new
    &setVariable
    &pushsdb 5							//'nextButtonSlotId'
    &pushzerosv
    &pushsdb 39							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 20							//'FSCommand:'
    &pushsdb 43							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 22							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 45							//''
    &getURL2
    &pushsdbgv 46							//'extern'
    &pushsdbgm 47							//'InGame'
    &not
    &not
    &jnz label22    
    &pushbyte 3
    &pushone
    &dcallfp 37							// SetButtonSlotCount()
   label22:
    &stop
  &end // of frame 0
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'LoadMovieStage' &as 1
  &end // of importAssets
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 2
  &end // of importAssets

  &defineMovieClip 3 // total frames: 2

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 2 
    
      &onClipEvent &construct
        &constants 'vertAlignment', 'center', 'horzAlignment'  
        &pushsdb 0							//'vertAlignment'
        &pushsdb 1							//'center'
        &setVariable
        &pushsdb 2							//'horzAlignment'
        &pushsdb 1							//'center'
        &setVariable
      &end
    &end // of placeMovieClip 2
  &end // of defineMovieClip 3
&end
