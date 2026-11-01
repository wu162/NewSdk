movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TAE82B~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'WhiteRectangle' &as 1
  &end // of importAssets

  &defineMovieClip 2 // total frames: 1

    &placeMovieClip 1 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 1
  &end // of defineMovieClip 2
  
  &exportAssets
    2 &as 'CommandListButtonStage'
  &end // of exportAssets

  &frame 0
    &constants 'buttonStage', '_y', 'visibleListHeight', 'buttonSlots', 'length', 'stage', 'buttonSlotHeight', '_visible', 'nextButtonSlotId', 'LoadMovieStage', 'attachMovie', '_x', 'ShouldButtonSlotBeVisible', 'helpSite', 'HelpBoxSite', 'buttonHelpSite', 'vertAlignment', 'horzAlignment', 'splice', 'FSCommand:', 'OnButtonSlotStageCreated', 'qualifyName', 'index=', '&name=', 'Pointer', 'encode', 'numColumns', 'Math', 'floor', 'buttonSlotWidth', 'CreateNewButtonSlot', 'MoveButtonSlot', 'scrollBar', 'UpdateScrollBarState', 'removeMovieClip', 'pop', 'onEnterFrame', 'GetCurPos', 'ceil', 'round', 'UpdateButtonSlotVisibility', 'SetLineSize', 'numVisibleRows', 'SetPageSize', 'SetEnabled', 'SetVisible', 'scrollBarParam', 'scrollBarPlaceholder', '_height', 'SetHeight', '_xscale', '_yscale', 'OnPosChanged', 'OnScrollBarPosChanged', 'InsertButtonSlot', 'EraseButtonSlot', 'SetButtonSlotCount', 'GetScrollBarParam', 'OnScrollBarLoaded', 'onUnload', 'upButtonParam', 'Object', 'disabledImage', 'VSliderUpButtonDisabledDynFac', 'upImage', 'VSliderUpButtonEnabledDynFac', 'overImage', 'VSliderUpButtonOverDynFac', 'downImage', 'VSliderUpButtonPressDynFac', 'downButtonParam', 'VSliderDownButtonDisabledDynFac', 'VSliderDownButtonEnabledDynFac', 'VSliderDownButtonOverDynFac', 'VSliderDownButtonPressDynFac', 'thumbTop', 'VSliderThumbDisabledTopDynFac', 'VSliderThumbEnabledTopDynFac', 'VSliderThumbOverTopDynFac', 'VSliderThumbPressTopDynFac', 'thumbMiddle', 'VSliderThumbDisabledMiddleDynFac', 'VSliderThumbEnabledMiddleDynFac', 'VSliderThumbOverMiddleDynFac', 'VSliderThumbPressMiddleDynFac', 'thumbBottom', 'VSliderThumbDisabledBottomDynFac', 'VSliderThumbEnabledBottomDynFac', 'VSliderThumbOverBottomDynFac', 'VSliderThumbPressBottomDynFac', 'thumbParam', 'topPiece', 'middlePiece', 'bottomPiece', 'barParam', 'VSliderBarDisabledDynFac', 'VSliderBarUpOverDynFac', 'upButton', 'downButton', 'thumb', 'bar', 'initialized', 'this', '_parent', 'OnContentLoaded', 'buttonStageWidth', 'buttonStageHeight', 'Array', 'VerticalScrollBar.swf', 'loadMovie', 'OnInitialized', ''  
    &function2 UpdateButtonSlotVisibility () ()
      &pushsdbgv 0							//'buttonStage'
      &pushsdbgm 1							//'_y'
      &setRegister r:4
      &pop
      &pushregister 4
      &pushsdbgv 2							//'visibleListHeight'
      &add
      &setRegister r:5
      &pop
      &pushzero
      &setRegister r:1
      &pop
     label1:
      &pushregister 1
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushsdbgv 3							//'buttonSlots'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'stage'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgm 1							//'_y'
      &pushregister 5
      &lessThan
      &dup
      &not
      &jnz label2      
      &pop
      &pushregister 2
      &pushsdbgm 1							//'_y'
      &pushsdbgv 6							//'buttonSlotHeight'
      &add
      &pushregister 4
      &greaterThan
     label2:
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdb 7							//'_visible'
      &pushregister 3
      &setMember
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label1      
     label3:
    &end // of function UpdateButtonSlotVisibility

    &function2 ShouldButtonSlotBeVisible (r:3='slotStage') ()
      &pushzero
      &pushsdbgv 0							//'buttonStage'
      &pushsdbgm 1							//'_y'
      &subtract
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 2							//'visibleListHeight'
      &add
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdbgm 1							//'_y'
      &pushregister 2
      &lessThan
      &dup
      &not
      &jnz label4      
      &pop
      &pushregister 3
      &pushsdbgm 1							//'_y'
      &pushsdbgv 6							//'buttonSlotHeight'
      &add
      &pushregister 1
      &greaterThan
     label4:
      &return
    &end // of function ShouldButtonSlotBeVisible

    &function2 CreateNewButtonSlot (r:6='slotIndex', r:10='slotX', r:9='slotY') (r:1='this')
      &pushsdbgv 8							//'nextButtonSlotId'
      &pushsdb 8							//'nextButtonSlotId'
      &pushsdbgv 8							//'nextButtonSlotId'
      &increment
      &setVariable
      &setRegister r:4
      &pop
      &pushregister 4
      &pushregister 4
      &toString
      &pushsdb 9							//'LoadMovieStage'
      &pushbyte 3
      &pushsdbgv 0							//'buttonStage'
      &pushsdb 10							//'attachMovie'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 11							//'_x'
      &pushregister 10
      &setMember
      &pushregister 2
      &pushsdb 1							//'_y'
      &pushregister 9
      &setMember
      &pushregister 2
      &pushsdb 7							//'_visible'
      &pushregister 2
      &pushone
      &pushsdb 12							//'ShouldButtonSlotBeVisible'
      &callFunction
      &setMember
      &pushregister 4
      &pushlong 65536
      &add
      &pushsdb 13							//'helpSite'
      &pushregister 4
      &toString
      &add
      &pushsdb 14							//'HelpBoxSite'
      &pushbyte 3
      &pushsdbgv 0							//'buttonStage'
      &pushsdb 10							//'attachMovie'
      &callMethod
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 11							//'_x'
      &pushsdbgv 15							//'buttonHelpSite'
      &pushsdbgm 11							//'_x'
      &setMember
      &pushregister 3
      &pushsdb 1							//'_y'
      &pushregister 9
      &setMember
      &pushregister 3
      &pushsdb 16							//'vertAlignment'
      &pushsdbgv 15							//'buttonHelpSite'
      &pushsdbgm 16							//'vertAlignment'
      &setMember
      &pushregister 3
      &pushsdb 17							//'horzAlignment'
      &pushsdbgv 15							//'buttonHelpSite'
      &pushsdbgm 17							//'horzAlignment'
      &setMember
      &pushregister 2
      &pushsdb 13							//'helpSite'
      &pushregister 3
      &setMember
      &pushsdb 5							//'stage'
      &pushregister 2
      &pushsdb 13							//'helpSite'
      &pushregister 3
      &pushbyte 2
      &initObject
      &setRegister r:5
      &pop
      &pushregister 5
      &pushzero
      &pushregister 6
      &pushbyte 3
      &pushsdbgv 3							//'buttonSlots'
      &dcallmp 18							// splice()
      &pushsdb 19							//'FSCommand:'
      &pushsdb 20							//'OnButtonSlotStageCreated'
      &pushregister 1
      &pushbyte 2
      &pushsdb 21							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 22							//'index='
      &pushregister 6
      &add
      &pushsdb 23							//'&name='
      &add
      &pushregister 2
      &pushone
      &pushsdbgv 24							//'Pointer'
      &pushsdb 25							//'encode'
      &callMethod
      &add
      &getURL2
      &pushregister 5
      &return
    &end // of function CreateNewButtonSlot

    &function2 MoveButtonSlot (r:5='slotIndex', r:6='slotX', r:4='slotY') ()
      &pushsdbgv 3							//'buttonSlots'
      &pushregister 5
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgm 5							//'stage'
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 11							//'_x'
      &pushregister 6
      &setMember
      &pushregister 1
      &pushsdb 1							//'_y'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 7							//'_visible'
      &pushregister 1
      &pushone
      &pushsdb 12							//'ShouldButtonSlotBeVisible'
      &callFunction
      &setMember
      &pushregister 2
      &pushsdbgm 13							//'helpSite'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 1							//'_y'
      &pushregister 4
      &setMember
    &end // of function MoveButtonSlot

    &function2 InsertButtonSlot (r:1='index') ()
      &pushregister 1
      &pushsdbgv 26							//'numColumns'
      &divide
      &pushone
      &pushsdbgv 27							//'Math'
      &pushsdb 28							//'floor'
      &callMethod
      &setRegister r:5
      &pop
      &pushregister 1
      &pushsdbgv 26							//'numColumns'
      &modulo
      &setRegister r:3
      &pop
      &pushregister 5
      &pushsdbgv 6							//'buttonSlotHeight'
      &multiply
      &setRegister r:4
      &pop
      &pushregister 3
      &pushsdbgv 29							//'buttonSlotWidth'
      &multiply
      &setRegister r:2
      &pop
      &pushregister 4
      &pushregister 2
      &pushregister 1
      &pushbyte 3
      &dcallfp 30							// CreateNewButtonSlot()
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
     label5:
      &pushregister 1
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label8      
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdbgv 26							//'numColumns'
      &lessThan
      &not
      &jnz label6      
      &pushregister 2
      &pushsdbgv 29							//'buttonSlotWidth'
      &add
      &setRegister r:2
      &pop
      &jmp label7      
     label6:
      &pushzero
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushregister 4
      &pushsdbgv 6							//'buttonSlotHeight'
      &add
      &setRegister r:4
      &pop
     label7:
      &pushregister 4
      &pushregister 2
      &pushregister 1
      &pushbyte 3
      &dcallfp 31							// MoveButtonSlot()
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label5      
     label8:
      &pushsdbgv 32							//'scrollBar'
      &pushone
      &dcallfp 33							// UpdateScrollBarState()
    &end // of function InsertButtonSlot

    &function2 EraseButtonSlot (r:1='index') ()
      &pushsdbgv 3							//'buttonSlots'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'stage'
      &pushsdbgm 11							//'_x'
      &setRegister r:2
      &pop
      &pushsdbgv 3							//'buttonSlots'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'stage'
      &pushsdbgm 1							//'_y'
      &setRegister r:4
      &pop
      &pushzero
      &pushsdbgv 3							//'buttonSlots'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'stage'
      &dcallmp 34							// removeMovieClip()
      &pushzero
      &pushsdbgv 3							//'buttonSlots'
      &pushregister 1
      &getMember
      &pushsdbgm 13							//'helpSite'
      &dcallmp 34							// removeMovieClip()
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 3							//'buttonSlots'
      &dcallmp 18							// splice()
      &pushregister 1
      &pushsdbgv 26							//'numColumns'
      &modulo
      &setRegister r:3
      &pop
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label14      
      &pushregister 1
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label13      
     label9:
      &pushfalse
      &jnz label13      
      &pushregister 4
      &pushregister 2
      &pushregister 1
      &pushbyte 3
      &dcallfp 31							// MoveButtonSlot()
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &not
      &jnz label10      
      &jmp label13      
     label10:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdbgv 26							//'numColumns'
      &lessThan
      &not
      &jnz label11      
      &pushregister 2
      &pushsdbgv 29							//'buttonSlotWidth'
      &add
      &setRegister r:2
      &pop
      &jmp label12      
     label11:
      &pushzero
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushregister 4
      &pushsdbgv 6							//'buttonSlotHeight'
      &add
      &setRegister r:4
      &pop
     label12:
      &jmp label9      
     label13:
      &jmp label15      
     label14:
      &pushsdb 8							//'nextButtonSlotId'
      &pushzerosv
     label15:
      &pushsdbgv 32							//'scrollBar'
      &pushone
      &dcallfp 33							// UpdateScrollBarState()
    &end // of function EraseButtonSlot

    &function2 SetButtonSlotCount (r:5='newCount') ()
      &pushregister 5
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label18      
     label16:
      &pushzero
      &pushsdbgv 3							//'buttonSlots'
      &pushsdb 35							//'pop'
      &callMethod
      &setRegister r:1
      &pop
      &pushzero
      &pushregister 1
      &pushsdbgm 5							//'stage'
      &dcallmp 34							// removeMovieClip()
      &pushzero
      &pushregister 1
      &pushsdbgm 13							//'helpSite'
      &dcallmp 34							// removeMovieClip()
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushregister 5
      &greaterThan
      &jnz label16      
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushzero
      &equals
      &not
      &jnz label17      
      &pushsdb 8							//'nextButtonSlotId'
      &pushzerosv
     label17:
      &jmp label23      
     label18:
      &pushregister 5
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &greaterThan
      &not
      &jnz label23      
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushsdbgv 26							//'numColumns'
      &divide
      &pushone
      &pushsdbgv 27							//'Math'
      &pushsdb 28							//'floor'
      &callMethod
      &setRegister r:6
      &pop
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushsdbgv 26							//'numColumns'
      &modulo
      &setRegister r:2
      &pop
      &pushregister 6
      &pushsdbgv 6							//'buttonSlotHeight'
      &multiply
      &setRegister r:4
      &pop
      &pushregister 2
      &pushsdbgv 29							//'buttonSlotWidth'
      &multiply
      &setRegister r:3
      &pop
     label19:
      &pushfalse
      &jnz label23      
      &pushregister 4
      &pushregister 3
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushbyte 3
      &dcallfp 30							// CreateNewButtonSlot()
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushregister 5
      &lessThan
      &not
      &not
      &jnz label20      
      &jmp label23      
     label20:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgv 26							//'numColumns'
      &lessThan
      &not
      &jnz label21      
      &pushregister 3
      &pushsdbgv 29							//'buttonSlotWidth'
      &add
      &setRegister r:3
      &pop
      &jmp label22      
     label21:
      &pushzero
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:3
      &pop
      &pushregister 4
      &pushsdbgv 6							//'buttonSlotHeight'
      &add
      &setRegister r:4
      &pop
     label22:
      &jmp label19      
     label23:
      &pushsdbgv 32							//'scrollBar'
      &pushone
      &dcallfp 33							// UpdateScrollBarState()
    &end // of function SetButtonSlotCount

    &function2 OnScrollBarPosChanged ('pos') ()
      &pushsdb 36							//'onEnterFrame'
      &function2  () ()
        &pushzero
        &pushsdbgv 32							//'scrollBar'
        &pushsdb 37							//'GetCurPos'
        &callMethod
        &setRegister r:6
        &pop
        &pushsdbgv 3							//'buttonSlots'
        &pushsdbgm 4							//'length'
        &pushsdbgv 26							//'numColumns'
        &divide
        &pushone
        &pushsdbgv 27							//'Math'
        &pushsdb 38							//'ceil'
        &callMethod
        &setRegister r:3
        &pop
        &pushregister 3
        &pushregister 6
        &multiply
        &pushone
        &pushsdbgv 27							//'Math'
        &pushsdb 39							//'round'
        &callMethod
        &setRegister r:4
        &pop
        &pushzero
        &pushregister 4
        &pushsdbgv 6							//'buttonSlotHeight'
        &multiply
        &subtract
        &setRegister r:1
        &pop
        &pushregister 1
        &pushsdbgv 0							//'buttonStage'
        &pushsdbgm 1							//'_y'
        &subtract
        &setRegister r:5
        &pop
        &pushsdbgv 0							//'buttonStage'
        &pushsdb 1							//'_y'
        &pushsdbgv 0							//'buttonStage'
        &pushsdbgm 1							//'_y'
        &pushregister 5
        &pushbyte 2
        &divide
        &add
        &setMember
        &pushregister 1
        &pushsdbgv 0							//'buttonStage'
        &pushsdbgm 1							//'_y'
        &lessThan
        &not
        &jnz label26        
        &pushsdbgv 0							//'buttonStage'
        &pushsdbgm 1							//'_y'
        &pushsdbgv 6							//'buttonSlotHeight'
        &pushbyte 4
        &divide
        &subtract
        &setRegister r:2
        &pop
        &pushregister 1
        &pushregister 2
        &lessThan
        &not
        &jnz label24        
        &pushsdbgv 0							//'buttonStage'
        &pushsdb 1							//'_y'
        &pushregister 2
        &setMember
        &jmp label25        
       label24:
        &pushsdbgv 0							//'buttonStage'
        &pushsdb 1							//'_y'
        &pushregister 1
        &setMember
        &pushsdb 36							//'onEnterFrame'
        &delete2
        &pop
       label25:
        &jmp label28        
       label26:
        &pushsdbgv 0							//'buttonStage'
        &pushsdbgm 1							//'_y'
        &pushsdbgv 6							//'buttonSlotHeight'
        &pushbyte 4
        &divide
        &add
        &setRegister r:2
        &pop
        &pushregister 1
        &pushregister 2
        &greaterThan
        &not
        &jnz label27        
        &pushsdbgv 0							//'buttonStage'
        &pushsdb 1							//'_y'
        &pushregister 2
        &setMember
        &jmp label28        
       label27:
        &pushsdbgv 0							//'buttonStage'
        &pushsdb 1							//'_y'
        &pushregister 1
        &setMember
        &pushsdb 36							//'onEnterFrame'
        &delete2
        &pop
       label28:
        &pushzero
        &dcallfp 40							// UpdateButtonSlotVisibility()
      &end // of function 

      &setVariable
    &end // of function OnScrollBarPosChanged

    &function2 UpdateScrollBarState (r:2='clip') ()
      &pushsdbgv 3							//'buttonSlots'
      &pushsdbgm 4							//'length'
      &pushsdbgv 26							//'numColumns'
      &divide
      &pushone
      &pushsdbgv 27							//'Math'
      &pushsdb 38							//'ceil'
      &callMethod
      &setRegister r:1
      &pop
      &pushregister 1
      &pushone
      &lessThan
      &not
      &jnz label29      
      &pushone
      &setRegister r:1
      &pop
     label29:
      &pushone
      &pushregister 1
      &divide
      &pushone
      &pushregister 2
      &dcallmp 41							// SetLineSize()
      &pushregister 1
      &pushsdbgv 42							//'numVisibleRows'
      &greaterThan
      &not
      &not
      &jnz label30      
      &pushone
      &pushone
      &pushregister 2
      &dcallmp 43							// SetPageSize()
      &pushfalse
      &pushone
      &pushregister 2
      &dcallmp 44							// SetEnabled()
      &pushfalse
      &pushone
      &pushregister 2
      &dcallmp 45							// SetVisible()
      &jmp label31      
     label30:
      &pushsdbgv 42							//'numVisibleRows'
      &pushregister 1
      &divide
      &pushone
      &pushregister 2
      &dcallmp 43							// SetPageSize()
      &pushtrue
      &pushone
      &pushregister 2
      &dcallmp 44							// SetEnabled()
      &pushtrue
      &pushone
      &pushregister 2
      &dcallmp 45							// SetVisible()
     label31:
    &end // of function UpdateScrollBarState

    &function GetScrollBarParam ('clip'    )    
      &pushsdbgv 46							//'scrollBarParam'
      &return
    &end // of function GetScrollBarParam

    &function2 OnScrollBarLoaded (r:1='clip') ()
      &pushtrue
      &pushone
      &pushregister 1
      &dcallmp 44							// SetEnabled()
      &pushsdbgv 47							//'scrollBarPlaceholder'
      &pushsdbgm 48							//'_height'
      &pushone
      &pushregister 1
      &dcallmp 49							// SetHeight()
      &pushregister 1
      &pushsdb 50							//'_xscale'
      &pushbyte 100
      &setMember
      &pushregister 1
      &pushsdb 51							//'_yscale'
      &pushbyte 100
      &setMember
      &pushregister 1
      &pushone
      &dcallfp 33							// UpdateScrollBarState()
      &pushregister 1
      &pushsdb 52							//'OnPosChanged'
      &function2  (r:1='pos') ()
        &pushregister 1
        &pushone
        &dcallfp 53							// OnScrollBarPosChanged()
      &end // of function 

      &setMember
    &end // of function OnScrollBarLoaded

    &function onUnload (    )    
      &pushsdb 40							//'UpdateButtonSlotVisibility'
      &delete2
      &pop
      &pushsdb 12							//'ShouldButtonSlotBeVisible'
      &delete2
      &pop
      &pushsdb 30							//'CreateNewButtonSlot'
      &delete2
      &pop
      &pushsdb 31							//'MoveButtonSlot'
      &delete2
      &pop
      &pushsdb 54							//'InsertButtonSlot'
      &delete2
      &pop
      &pushsdb 55							//'EraseButtonSlot'
      &delete2
      &pop
      &pushsdb 56							//'SetButtonSlotCount'
      &delete2
      &pop
      &pushsdb 53							//'OnScrollBarPosChanged'
      &delete2
      &pop
      &pushsdb 33							//'UpdateScrollBarState'
      &delete2
      &pop
      &pushsdb 57							//'GetScrollBarParam'
      &delete2
      &pop
      &pushsdb 58							//'OnScrollBarLoaded'
      &delete2
      &pop
      &pushsdb 59							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdb 60							//'upButtonParam'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &varEquals
    &pushsdbgv 60							//'upButtonParam'
    &pushsdb 62							//'disabledImage'
    &pushsdb 63							//'VSliderUpButtonDisabledDynFac'
    &setMember
    &pushsdbgv 60							//'upButtonParam'
    &pushsdb 64							//'upImage'
    &pushsdb 65							//'VSliderUpButtonEnabledDynFac'
    &setMember
    &pushsdbgv 60							//'upButtonParam'
    &pushsdb 66							//'overImage'
    &pushsdb 67							//'VSliderUpButtonOverDynFac'
    &setMember
    &pushsdbgv 60							//'upButtonParam'
    &pushsdb 68							//'downImage'
    &pushsdb 69							//'VSliderUpButtonPressDynFac'
    &setMember
    &pushsdb 70							//'downButtonParam'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &varEquals
    &pushsdbgv 70							//'downButtonParam'
    &pushsdb 62							//'disabledImage'
    &pushsdb 71							//'VSliderDownButtonDisabledDynFac'
    &setMember
    &pushsdbgv 70							//'downButtonParam'
    &pushsdb 64							//'upImage'
    &pushsdb 72							//'VSliderDownButtonEnabledDynFac'
    &setMember
    &pushsdbgv 70							//'downButtonParam'
    &pushsdb 66							//'overImage'
    &pushsdb 73							//'VSliderDownButtonOverDynFac'
    &setMember
    &pushsdbgv 70							//'downButtonParam'
    &pushsdb 68							//'downImage'
    &pushsdb 74							//'VSliderDownButtonPressDynFac'
    &setMember
    &pushsdb 75							//'thumbTop'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &varEquals
    &pushsdbgv 75							//'thumbTop'
    &pushsdb 62							//'disabledImage'
    &pushsdb 76							//'VSliderThumbDisabledTopDynFac'
    &setMember
    &pushsdbgv 75							//'thumbTop'
    &pushsdb 64							//'upImage'
    &pushsdb 77							//'VSliderThumbEnabledTopDynFac'
    &setMember
    &pushsdbgv 75							//'thumbTop'
    &pushsdb 66							//'overImage'
    &pushsdb 78							//'VSliderThumbOverTopDynFac'
    &setMember
    &pushsdbgv 75							//'thumbTop'
    &pushsdb 68							//'downImage'
    &pushsdb 79							//'VSliderThumbPressTopDynFac'
    &setMember
    &pushsdb 80							//'thumbMiddle'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &varEquals
    &pushsdbgv 80							//'thumbMiddle'
    &pushsdb 62							//'disabledImage'
    &pushsdb 81							//'VSliderThumbDisabledMiddleDynFac'
    &setMember
    &pushsdbgv 80							//'thumbMiddle'
    &pushsdb 64							//'upImage'
    &pushsdb 82							//'VSliderThumbEnabledMiddleDynFac'
    &setMember
    &pushsdbgv 80							//'thumbMiddle'
    &pushsdb 66							//'overImage'
    &pushsdb 83							//'VSliderThumbOverMiddleDynFac'
    &setMember
    &pushsdbgv 80							//'thumbMiddle'
    &pushsdb 68							//'downImage'
    &pushsdb 84							//'VSliderThumbPressMiddleDynFac'
    &setMember
    &pushsdb 85							//'thumbBottom'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &varEquals
    &pushsdbgv 85							//'thumbBottom'
    &pushsdb 62							//'disabledImage'
    &pushsdb 86							//'VSliderThumbDisabledBottomDynFac'
    &setMember
    &pushsdbgv 85							//'thumbBottom'
    &pushsdb 64							//'upImage'
    &pushsdb 87							//'VSliderThumbEnabledBottomDynFac'
    &setMember
    &pushsdbgv 85							//'thumbBottom'
    &pushsdb 66							//'overImage'
    &pushsdb 88							//'VSliderThumbOverBottomDynFac'
    &setMember
    &pushsdbgv 85							//'thumbBottom'
    &pushsdb 68							//'downImage'
    &pushsdb 89							//'VSliderThumbPressBottomDynFac'
    &setMember
    &pushsdb 90							//'thumbParam'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &varEquals
    &pushsdbgv 90							//'thumbParam'
    &pushsdb 91							//'topPiece'
    &pushsdbgv 75							//'thumbTop'
    &setMember
    &pushsdbgv 90							//'thumbParam'
    &pushsdb 92							//'middlePiece'
    &pushsdbgv 80							//'thumbMiddle'
    &setMember
    &pushsdbgv 90							//'thumbParam'
    &pushsdb 93							//'bottomPiece'
    &pushsdbgv 85							//'thumbBottom'
    &setMember
    &pushsdb 94							//'barParam'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &varEquals
    &pushsdbgv 94							//'barParam'
    &pushsdb 62							//'disabledImage'
    &pushsdb 95							//'VSliderBarDisabledDynFac'
    &setMember
    &pushsdbgv 94							//'barParam'
    &pushsdb 64							//'upImage'
    &pushsdb 96							//'VSliderBarUpOverDynFac'
    &setMember
    &pushsdbgv 94							//'barParam'
    &pushsdb 66							//'overImage'
    &pushsdb 96							//'VSliderBarUpOverDynFac'
    &setMember
    &pushsdb 46							//'scrollBarParam'
    &pushzero
    &pushsdb 61							//'Object'
    &new
    &setVariable
    &pushsdbgv 46							//'scrollBarParam'
    &pushsdb 97							//'upButton'
    &pushsdbgv 60							//'upButtonParam'
    &setMember
    &pushsdbgv 46							//'scrollBarParam'
    &pushsdb 98							//'downButton'
    &pushsdbgv 70							//'downButtonParam'
    &setMember
    &pushsdbgv 46							//'scrollBarParam'
    &pushsdb 99							//'thumb'
    &pushsdbgv 90							//'thumbParam'
    &setMember
    &pushsdbgv 46							//'scrollBarParam'
    &pushsdb 100							//'bar'
    &pushsdbgv 94							//'barParam'
    &setMember
    &pushsdbgv 101							//'initialized'
    &not
    &not
    &jnz label36    
    &pushthisgv
    &pushone
    &pushsdbgv 103							//'_parent'
    &dcallmp 104							// OnContentLoaded()
    &pushsdb 105							//'buttonStageWidth'
    &pushbyte 50
    &setVariable
    &pushsdb 106							//'buttonStageHeight'
    &pushbyte 50
    &setVariable
    &pushsdbgv 29							//'buttonSlotWidth'
    &pushundef
    &equals
    &not
    &jnz label32    
    &pushsdb 29							//'buttonSlotWidth'
    &pushbyte 55
    &setVariable
   label32:
    &pushsdbgv 6							//'buttonSlotHeight'
    &pushundef
    &equals
    &not
    &jnz label33    
    &pushsdb 6							//'buttonSlotHeight'
    &pushbyte 52
    &setVariable
   label33:
    &pushsdbgv 26							//'numColumns'
    &pushundef
    &equals
    &not
    &jnz label34    
    &pushsdb 26							//'numColumns'
    &pushbyte 3
    &setVariable
   label34:
    &pushsdbgv 42							//'numVisibleRows'
    &pushundef
    &equals
    &not
    &jnz label35    
    &pushsdb 42							//'numVisibleRows'
    &pushbyte 3
    &setVariable
   label35:
    &pushsdb 2							//'visibleListHeight'
    &pushsdbgv 42							//'numVisibleRows'
    &pushsdbgv 6							//'buttonSlotHeight'
    &multiply
    &setVariable
    &pushsdb 3							//'buttonSlots'
    &pushzero
    &pushsdb 107							//'Array'
    &new
    &setVariable
    &pushsdb 8							//'nextButtonSlotId'
    &pushzerosv
    &pushsdb 108							//'VerticalScrollBar.swf'
    &pushone
    &pushsdbgv 32							//'scrollBar'
    &dcallmp 109							// loadMovie()
    &pushsdb 19							//'FSCommand:'
    &pushsdb 110							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 21							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 111							//''
    &getURL2
    &pushsdb 101							//'initialized'
    &pushtrue
    &setVariable
   label36:
    &stop
  &end // of frame 0

  &defineMovieClip 3 // total frames: 1

    &placeMovieClip 1 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 1
  &end // of defineMovieClip 3
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 4
  &end // of importAssets

  &placeMovieClip 4 &as 'buttonHelpSite'
  
    &onClipEvent &construct
      &pushs 'vertAlignment'
      &pushssv 'top'
      &pushs 'horzAlignment'
      &pushssv 'right'
    &end
  &end // of placeMovieClip 4
  
  &exportAssets
    2 &as 'CommandListButtonStage'
  &end // of exportAssets
&end
