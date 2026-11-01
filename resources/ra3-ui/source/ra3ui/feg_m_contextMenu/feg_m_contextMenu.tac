movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\feg_m_contextMenu\\feg_m_contextMenu.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 4 // total frames: 19

    &frame 3
      &stop
    &end // of frame 3

    &frame 9
      &gotoLabel '_on'
      &play
    &end // of frame 9

    &frame 18
      &stop
    &end // of frame 18
  &end // of defineMovieClip 4

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7
  
  &exportAssets
    7 &as 'contextMenuEntrySymbol'
  &end // of exportAssets

  &frame 0
    &stop
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_contextMenu'
    &new
    &varEquals
  &end // of frame 0

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 18 // total frames: 0
  &end // of defineMovieClip 18
  
  &exportAssets
    18 &as '__Packages.feg_m_contextMenu'
  &end // of exportAssets
  
  &initMovieClip 18
    &constants '_global', 'feg_m_contextMenu', 'm_screen', 'hideMenu', 'onEnterFrame', 'frameDelayedInit', 'bind0', 'prototype', 'shutdown', 'entryCount', 'm_entryClips', 'setSelectCallback', 'removeMovieClip', 'setNotificationCallback', 'm_callback', 'ERROR in feg_m_contextMenu::setNotificationCallback(), previous callback exists!', 'show', 'populateMenu', 'positionMenuAtCurrentMouseLocation', 'showMenu', 'hide', 'isShowing', 'contextMenu', '_visible', 'doDefaultFriendContextMenu', 'm_defaultHelpersPlayerId', 'Object', 'QueryGameEngine?IsPcGameHost&IS_PLAYER_IGNORED?Player=', '&IS_PLAYER_BUDDY?Player=', '&MP_BUDDYSTATUS?Player=', '&GAME_HAS_OPEN_SLOTS', 'IsPcGameHost', 'IS_PLAYER_BUDDY', 'IS_PLAYER_IGNORED', 'MP_BUDDYSTATUS_IS_ONLINE', 'GAME_HAS_OPEN_SLOTS', '$ContextAddAsFriend', '$ContextRemoveFromFriends', '$ContextIgnore', '$ContextRemoveFromIgnore', '$ContextWhisper', '$ContextInviteToGame', 'Array', 'ENUM', 'CONTEXTMENU_ADD_AS_FRIEND', 'CONTEXTMENU_REMOVE_FROM_FRIENDS', 'CONTEXTMENU_IGNORE', 'CONTEXTMENU_REMOVE_FROM_IGNORE', 'CONTEXTMENU_WHISPER', 'CONTEXTMENU_INVITE_TO_GAME', 'm_defaultHelpersCallback', 'onDefaultFriendContextMenuEntrySelected', 'bind1DynamicParams', 'CONTEXTMENU_INVALID_SELECTION', 'FSCommand:CallGameFunction', '%OnlineAddFriend?Player=', '%OnlineInvitePlayer?Player=', '%OnlineIgnorePlayer?Ignore=1|Player=', '%OnlineRemoveFriend?Player=', '%OnlineIgnorePlayer?Ignore=0|Player=', '*** ERROR: unknown context menu selection, id is ', 'doDefaultPlayerContextMenu', 'onDefaultPlayerContextMenuEntrySelected', 'screen', 'buttonGuard', 'onPress', 'onButtonGuardClicked', 'onContextMenuClicked', 'ENTRY_HEIGHT', 'MAXIMUM_ENTRY_COUNT', 'initEntryClips', '', 'ENTRY_MARGIN_TOP', 'ATTACHMOVIE_BASE_LEVEL', 'entryClip', 'contextMenuEntrySymbol', 'attachMovie', '_x', 'ENTRY_MARGIN_HORIZ', '_y', 'init', 'onEntrySelected', 'getId', 'getAbsoluteMouseLocation', 'length', 'setText', 'setId', 'setEnabled', 'adjustWidth', 'ENTRY_MARGIN_BOTTOM', 'setFrameSize', 'fill', '_width', '_height', 'edge_t', 'edge_b', 'edge_l', 'edge_r', '0', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_contextMenu'
    &not
    &not
    &jnz label54    
    &pushglobalgv
    &pushsdb 1							//'feg_m_contextMenu'
    &function2  (r:3='screen') (r:1='this', r:2='_global')
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdb 2							//'m_screen'
      &pushregister 3
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 3							// hideMenu()
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 4							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 5							//'frameDelayedInit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind0'
      &callMethod
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushregister 1
    &pushsdbgm 7							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 8							//'shutdown'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &setRegister r:3
      &pop
     label1:
      &pushregister 3
      &pushsdbgv 9							//'entryCount'
      &lessThan
      &not
      &jnz label2      
      &pushnull
      &pushone
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 3
      &getMember
      &dcallmp 11							// setSelectCallback()
      &pushzero
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 3
      &getMember
      &dcallmp 12							// removeMovieClip()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label1      
     label2:
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdb 2							//'m_screen'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'feg_m_contextMenu'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 13							//'setNotificationCallback'
    &function2  (r:2='fn') (r:1='this')
      &pushregister 1
      &pushsdbgm 14							//'m_callback'
      &pushnull
      &equals
      &not
      &dup
      &not
      &jnz label3      
      &pop
      &pushregister 2
      &pushnull
      &equals
      &not
     label3:
      &not
      &jnz label4      
      &pushsdb 15							//'ERROR in feg_m_contextMenu::setNotificationCallback(), previous callback exists!'
      &trace
     label4:
      &pushregister 1
      &pushsdb 14							//'m_callback'
      &pushregister 2
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 16							//'show'
    &function2  (r:3='items', r:2='ids', r:4='states') (r:1='this')
      &pushregister 4
      &pushregister 2
      &pushregister 3
      &pushbyte 3
      &pushregister 1
      &dcallmp 17							// populateMenu()
      &pushzero
      &pushregister 1
      &dcallmp 18							// positionMenuAtCurrentMouseLocation()
      &pushzero
      &pushregister 1
      &dcallmp 19							// showMenu()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 20							//'hide'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 3							// hideMenu()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 21							//'isShowing'
    &function  (    )    
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &pushsdbgm 23							//'_visible'
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'doDefaultFriendContextMenu'
    &function2  (r:13='playerId', r:12='callback') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 25							//'m_defaultHelpersPlayerId'
      &pushregister 13
      &setMember
      &pushzero
      &pushsdb 26							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 27							//'QueryGameEngine?IsPcGameHost&IS_PLAYER_IGNORED?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &pushsdb 28							//'&IS_PLAYER_BUDDY?Player='
      &add
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &pushsdb 29							//'&MP_BUDDYSTATUS?Player='
      &add
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &pushsdb 30							//'&GAME_HAS_OPEN_SLOTS'
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 31							//'IsPcGameHost'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:7
      &pop
      &pushregister 3
      &pushsdbgm 32							//'IS_PLAYER_BUDDY'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:4
      &pop
      &pushregister 3
      &pushsdbgm 33							//'IS_PLAYER_IGNORED'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:6
      &pop
      &pushregister 3
      &pushsdbgm 34							//'MP_BUDDYSTATUS_IS_ONLINE'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:5
      &pop
      &pushregister 3
      &pushsdbgm 35							//'GAME_HAS_OPEN_SLOTS'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:9
      &pop
      &pushregister 4
      &jnz label5      
      &pushsdb 36							//'$ContextAddAsFriend'
      &jmp label6      
     label5:
      &pushsdb 37							//'$ContextRemoveFromFriends'
     label6:
      &pushregister 6
      &jnz label7      
      &pushsdb 38							//'$ContextIgnore'
      &jmp label8      
     label7:
      &pushsdb 39							//'$ContextRemoveFromIgnore'
     label8:
      &pushsdb 40							//'$ContextWhisper'
      &pushsdb 41							//'$ContextInviteToGame'
      &pushbyte 4
      &pushsdb 42							//'Array'
      &new
      &setRegister r:10
      &pop
      &pushregister 4
      &jnz label9      
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 44							//'CONTEXTMENU_ADD_AS_FRIEND'
      &jmp label10      
     label9:
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 45							//'CONTEXTMENU_REMOVE_FROM_FRIENDS'
     label10:
      &pushregister 6
      &jnz label11      
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 46							//'CONTEXTMENU_IGNORE'
      &jmp label12      
     label11:
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 47							//'CONTEXTMENU_REMOVE_FROM_IGNORE'
     label12:
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 48							//'CONTEXTMENU_WHISPER'
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 49							//'CONTEXTMENU_INVITE_TO_GAME'
      &pushbyte 4
      &pushsdb 42							//'Array'
      &new
      &setRegister r:8
      &pop
      &pushone
      &pushone
      &pushregister 5
      &jnz label13      
      &pushzero
      &jmp label14      
     label13:
      &pushone
     label14:
      &pushregister 7
      &dup
      &not
      &jnz label15      
      &pop
      &pushregister 5
     label15:
      &dup
      &not
      &jnz label16      
      &pop
      &pushregister 9
     label16:
      &jnz label17      
      &pushzero
      &jmp label18      
     label17:
      &pushone
     label18:
      &pushbyte 4
      &pushsdb 42							//'Array'
      &new
      &setRegister r:11
      &pop
      &pushregister 1
      &pushsdb 50							//'m_defaultHelpersCallback'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 51							//'onDefaultFriendContextMenuEntrySelected'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 52							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &dcallmp 13							// setNotificationCallback()
      &pushregister 11
      &pushregister 8
      &pushregister 10
      &pushbyte 3
      &pushregister 1
      &dcallmp 16							// show()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 51							//'onDefaultFriendContextMenuEntrySelected'
    &function2  (r:3='id') (r:1='this', r:2='_global')
      &pushregister 3
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 53							//'CONTEXTMENU_INVALID_SELECTION'
      &equals
      &not
      &not
      &jnz label26      
      &pushregister 3
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 44							//'CONTEXTMENU_ADD_AS_FRIEND'
      &strictEquals
      &jnz label19      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 49							//'CONTEXTMENU_INVITE_TO_GAME'
      &strictEquals
      &jnz label20      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 48							//'CONTEXTMENU_WHISPER'
      &strictEquals
      &jnz label21      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 46							//'CONTEXTMENU_IGNORE'
      &strictEquals
      &jnz label22      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 45							//'CONTEXTMENU_REMOVE_FROM_FRIENDS'
      &strictEquals
      &jnz label23      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 47							//'CONTEXTMENU_REMOVE_FROM_IGNORE'
      &strictEquals
      &jnz label24      
      &jmp label25      
     label19:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 55							//'%OnlineAddFriend?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label26      
     label20:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 56							//'%OnlineInvitePlayer?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label26      
     label21:
      &jmp label26      
     label22:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 57							//'%OnlineIgnorePlayer?Ignore=1|Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label26      
     label23:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 58							//'%OnlineRemoveFriend?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label26      
     label24:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 59							//'%OnlineIgnorePlayer?Ignore=0|Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label26      
     label25:
      &pushsdb 60							//'*** ERROR: unknown context menu selection, id is '
      &pushregister 3
      &add
      &trace
      &jmp label26      
     label26:
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &pushregister 3
      &pushbyte 2
      &pushregister 1
      &dcallmp 50							// m_defaultHelpersCallback()
      &pushregister 1
      &pushsdb 50							//'m_defaultHelpersCallback'
      &pushnull
      &setMember
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 13							// setNotificationCallback()
      &pushzero
      &pushregister 1
      &dcallmp 20							// hide()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 61							//'doDefaultPlayerContextMenu'
    &function2  (r:12='playerId', r:11='callback') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 25							//'m_defaultHelpersPlayerId'
      &pushregister 12
      &setMember
      &pushzero
      &pushsdb 26							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 27							//'QueryGameEngine?IsPcGameHost&IS_PLAYER_IGNORED?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &pushsdb 28							//'&IS_PLAYER_BUDDY?Player='
      &add
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &pushsdb 30							//'&GAME_HAS_OPEN_SLOTS'
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 31							//'IsPcGameHost'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:6
      &pop
      &pushregister 3
      &pushsdbgm 32							//'IS_PLAYER_BUDDY'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:4
      &pop
      &pushregister 3
      &pushsdbgm 33							//'IS_PLAYER_IGNORED'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:5
      &pop
      &pushregister 3
      &pushsdbgm 35							//'GAME_HAS_OPEN_SLOTS'
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:8
      &pop
      &pushregister 4
      &jnz label27      
      &pushsdb 36							//'$ContextAddAsFriend'
      &jmp label28      
     label27:
      &pushsdb 37							//'$ContextRemoveFromFriends'
     label28:
      &pushregister 5
      &jnz label29      
      &pushsdb 38							//'$ContextIgnore'
      &jmp label30      
     label29:
      &pushsdb 39							//'$ContextRemoveFromIgnore'
     label30:
      &pushsdb 40							//'$ContextWhisper'
      &pushsdb 41							//'$ContextInviteToGame'
      &pushbyte 4
      &pushsdb 42							//'Array'
      &new
      &setRegister r:9
      &pop
      &pushregister 4
      &jnz label31      
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 44							//'CONTEXTMENU_ADD_AS_FRIEND'
      &jmp label32      
     label31:
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 45							//'CONTEXTMENU_REMOVE_FROM_FRIENDS'
     label32:
      &pushregister 5
      &jnz label33      
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 46							//'CONTEXTMENU_IGNORE'
      &jmp label34      
     label33:
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 47							//'CONTEXTMENU_REMOVE_FROM_IGNORE'
     label34:
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 48							//'CONTEXTMENU_WHISPER'
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 49							//'CONTEXTMENU_INVITE_TO_GAME'
      &pushbyte 4
      &pushsdb 42							//'Array'
      &new
      &setRegister r:7
      &pop
      &pushone
      &pushone
      &pushone
      &pushregister 6
      &dup
      &not
      &jnz label35      
      &pop
      &pushregister 8
     label35:
      &jnz label36      
      &pushzero
      &jmp label37      
     label36:
      &pushone
     label37:
      &pushbyte 4
      &pushsdb 42							//'Array'
      &new
      &setRegister r:10
      &pop
      &pushregister 1
      &pushsdb 50							//'m_defaultHelpersCallback'
      &pushregister 11
      &setMember
      &pushregister 1
      &pushsdbgm 62							//'onDefaultPlayerContextMenuEntrySelected'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 52							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 22							//'contextMenu'
      &pushsdbgm 63							//'screen'
      &dcallmp 13							// setNotificationCallback()
      &pushregister 10
      &pushregister 7
      &pushregister 9
      &pushbyte 3
      &pushregister 2
      &pushsdbgm 22							//'contextMenu'
      &pushsdbgm 63							//'screen'
      &dcallmp 16							// show()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 62							//'onDefaultPlayerContextMenuEntrySelected'
    &function2  (r:3='id') (r:1='this', r:2='_global')
      &pushregister 3
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 53							//'CONTEXTMENU_INVALID_SELECTION'
      &equals
      &not
      &not
      &jnz label45      
      &pushregister 3
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 44							//'CONTEXTMENU_ADD_AS_FRIEND'
      &strictEquals
      &jnz label38      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 49							//'CONTEXTMENU_INVITE_TO_GAME'
      &strictEquals
      &jnz label39      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 48							//'CONTEXTMENU_WHISPER'
      &strictEquals
      &jnz label40      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 46							//'CONTEXTMENU_IGNORE'
      &strictEquals
      &jnz label41      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 45							//'CONTEXTMENU_REMOVE_FROM_FRIENDS'
      &strictEquals
      &jnz label42      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 47							//'CONTEXTMENU_REMOVE_FROM_IGNORE'
      &strictEquals
      &jnz label43      
      &jmp label44      
     label38:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 55							//'%OnlineAddFriend?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label45      
     label39:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 56							//'%OnlineInvitePlayer?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label45      
     label40:
      &jmp label45      
     label41:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 57							//'%OnlineIgnorePlayer?Ignore=1|Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label45      
     label42:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 58							//'%OnlineRemoveFriend?Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label45      
     label43:
      &pushsdb 54							//'FSCommand:CallGameFunction'
      &pushsdb 59							//'%OnlineIgnorePlayer?Ignore=0|Player='
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &add
      &getURL2
      &jmp label45      
     label44:
      &pushsdb 60							//'*** ERROR: unknown context menu selection, id is '
      &pushregister 3
      &add
      &trace
      &jmp label45      
     label45:
      &pushregister 1
      &pushsdbgm 25							//'m_defaultHelpersPlayerId'
      &pushregister 3
      &pushbyte 2
      &pushregister 1
      &dcallmp 50							// m_defaultHelpersCallback()
      &pushregister 1
      &pushsdb 50							//'m_defaultHelpersCallback'
      &pushnull
      &setMember
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 13							// setNotificationCallback()
      &pushzero
      &pushregister 1
      &dcallmp 20							// hide()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 5							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 4							//'onEnterFrame'
      &pushnull
      &setMember
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 64							//'buttonGuard'
      &pushsdb 65							//'onPress'
      &pushregister 1
      &pushsdbgm 66							//'onButtonGuardClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &pushsdb 65							//'onPress'
      &pushregister 1
      &pushsdbgm 67							//'onContextMenuClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 68							//'ENTRY_HEIGHT'
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 69							//'MAXIMUM_ENTRY_COUNT'
      &pushbyte 2
      &pushregister 1
      &dcallmp 70							// initEntryClips()
      &pushone
      &pushone
      &initArray
      &pushzero
      &pushone
      &initArray
      &pushsdb 71							//''
      &pushone
      &initArray
      &pushbyte 3
      &pushregister 1
      &dcallmp 17							// populateMenu()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 19							//'showMenu'
    &function  (    )    
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 64							//'buttonGuard'
      &pushsdb 23							//'_visible'
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &pushsdb 23							//'_visible'
      &pushtrue
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 3							//'hideMenu'
    &function  (    )    
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 64							//'buttonGuard'
      &pushsdb 23							//'_visible'
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &pushsdb 23							//'_visible'
      &pushfalse
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 70							//'initEntryClips'
    &function2  (r:6='entryCount', r:7='entryHeight') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 10							//'m_entryClips'
      &pushregister 6
      &pushone
      &pushsdb 42							//'Array'
      &new
      &setMember
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 72							//'ENTRY_MARGIN_TOP'
      &setRegister r:5
      &pop
      &pushzero
      &setRegister r:4
      &pop
     label46:
      &pushregister 4
      &pushregister 6
      &lessThan
      &not
      &jnz label47      
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 73							//'ATTACHMOVIE_BASE_LEVEL'
      &pushregister 4
      &add
      &pushsdb 74							//'entryClip'
      &pushregister 4
      &toString
      &add
      &pushsdb 75							//'contextMenuEntrySymbol'
      &pushbyte 3
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &pushsdb 76							//'attachMovie'
      &callMethod
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 77							//'_x'
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 78							//'ENTRY_MARGIN_HORIZ'
      &setMember
      &pushregister 3
      &pushsdb 79							//'_y'
      &pushregister 5
      &setMember
      &pushzero
      &pushregister 3
      &dcallmp 80							// init()
      &pushregister 1
      &pushsdbgm 81							//'onEntrySelected'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 52							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 11							// setSelectCallback()
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 4
      &pushregister 3
      &setMember
      &pushregister 5
      &pushregister 7
      &add
      &setRegister r:5
      &pop
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &jmp label46      
     label47:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 66							//'onButtonGuardClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 2
      &pushsdbgm 43							//'ENUM'
      &pushsdbgm 53							//'CONTEXTMENU_INVALID_SELECTION'
      &pushone
      &pushregister 1
      &dcallmp 14							// m_callback()
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 13							// setNotificationCallback()
      &pushzero
      &pushregister 1
      &dcallmp 20							// hide()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 67							//'onContextMenuClicked'
    &function  (    )    
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 81							//'onEntrySelected'
    &function2  (r:2='entry') (r:1='this')
      &pushzero
      &pushregister 2
      &pushsdb 82							//'getId'
      &callMethod
      &pushone
      &pushregister 1
      &dcallmp 14							// m_callback()
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 13							// setNotificationCallback()
      &pushzero
      &pushregister 1
      &dcallmp 20							// hide()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 18							//'positionMenuAtCurrentMouseLocation'
    &function2  () (r:1='_global')
      &pushzero
      &pushregister 1
      &pushsdb 83							//'getAbsoluteMouseLocation'
      &callMethod
      &setRegister r:2
      &pop
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &pushsdb 77							//'_x'
      &pushregister 2
      &pushzero
      &getMember
      &setMember
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &pushsdb 79							//'_y'
      &pushregister 2
      &pushone
      &getMember
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 17							//'populateMenu'
    &function2  (r:6='items', r:7='ids', r:8='states') (r:1='this')
      &pushshort 150
      &setRegister r:5
      &pop
      &pushregister 6
      &pushsdbgm 84							//'length'
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 69							//'MAXIMUM_ENTRY_COUNT'
      &greaterThan
      &not
      &jnz label48      
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 69							//'MAXIMUM_ENTRY_COUNT'
      &jmp label49      
     label48:
      &pushregister 6
      &pushsdbgm 84							//'length'
     label49:
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label50:
      &pushregister 2
      &pushregister 4
      &lessThan
      &not
      &jnz label51      
      &pushregister 6
      &pushregister 2
      &getMember
      &pushone
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 2
      &getMember
      &dcallmp 85							// setText()
      &pushregister 7
      &pushregister 2
      &getMember
      &pushone
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 2
      &getMember
      &dcallmp 86							// setId()
      &pushregister 8
      &pushregister 2
      &getMember
      &toNumber
      &pushzero
      &equals
      &not
      &pushone
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 2
      &getMember
      &dcallmp 87							// setEnabled()
      &pushregister 5
      &pushone
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 2
      &getMember
      &dcallmp 88							// adjustWidth()
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 2
      &getMember
      &pushsdb 23							//'_visible'
      &pushtrue
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label50      
     label51:
      &pushregister 4
      &setRegister r:3
      &pop
     label52:
      &pushregister 3
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 69							//'MAXIMUM_ENTRY_COUNT'
      &lessThan
      &not
      &jnz label53      
      &pushsdb 71							//''
      &pushone
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 3
      &getMember
      &dcallmp 85							// setText()
      &pushregister 1
      &pushsdbgm 10							//'m_entryClips'
      &pushregister 3
      &getMember
      &pushsdb 23							//'_visible'
      &pushfalse
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label52      
     label53:
      &pushregister 4
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 68							//'ENTRY_HEIGHT'
      &multiply
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 72							//'ENTRY_MARGIN_TOP'
      &add
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 89							//'ENTRY_MARGIN_BOTTOM'
      &add
      &pushregister 5
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 78							//'ENTRY_MARGIN_HORIZ'
      &pushbyte 2
      &multiply
      &add
      &pushbyte 2
      &pushregister 1
      &dcallmp 90							// setFrameSize()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 90							//'setFrameSize'
    &function2  (r:3='x', r:2='y') ()
      &pushsdbgv 1							//'feg_m_contextMenu'
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 22							//'contextMenu'
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgm 91							//'fill'
      &pushsdb 92							//'_width'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdbgm 91							//'fill'
      &pushsdb 93							//'_height'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdbgm 94							//'edge_t'
      &pushsdb 92							//'_width'
      &pushregister 1
      &pushsdbgm 95							//'edge_b'
      &pushsdb 92							//'_width'
      &pushregister 3
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushregister 1
      &pushsdbgm 95							//'edge_b'
      &pushsdb 79							//'_y'
      &pushregister 2
      &pushregister 1
      &pushsdbgm 95							//'edge_b'
      &pushsdbgm 93							//'_height'
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 96							//'edge_l'
      &pushsdb 93							//'_height'
      &pushregister 1
      &pushsdbgm 97							//'edge_r'
      &pushsdb 93							//'_height'
      &pushregister 2
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushregister 1
      &pushsdbgm 97							//'edge_r'
      &pushsdb 77							//'_x'
      &pushregister 3
      &pushregister 1
      &pushsdbgm 97							//'edge_r'
      &pushsdbgm 92							//'_width'
      &subtract
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 14							//'m_callback'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 50							//'m_defaultHelpersCallback'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 25							//'m_defaultHelpersPlayerId'
    &pushsdb 98							//'0'
    &setMember
    &pushregister 1
    &pushsdb 73							//'ATTACHMOVIE_BASE_LEVEL'
    &pushshort 500
    &setMember
    &pushregister 1
    &pushsdb 69							//'MAXIMUM_ENTRY_COUNT'
    &pushbyte 16
    &setMember
    &pushregister 1
    &pushsdb 68							//'ENTRY_HEIGHT'
    &pushbyte 20
    &setMember
    &pushregister 1
    &pushsdb 78							//'ENTRY_MARGIN_HORIZ'
    &pushbyte 4
    &setMember
    &pushregister 1
    &pushsdb 72							//'ENTRY_MARGIN_TOP'
    &pushbyte 4
    &setMember
    &pushregister 1
    &pushsdb 89							//'ENTRY_MARGIN_BOTTOM'
    &pushbyte 3
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_contextMenu'
    &pushsdbgm 7							//'prototype'
    &pushbyte 3
    &pushsdb 99							//'ASSetPropFlags'
    &callFunction
   label54:
    &pop
  &end // of initMovieClip 18
&end
