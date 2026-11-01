movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\fe_onlineComponents\\fe_onlineComponents.eaf' // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2
  
  &exportAssets
    2 &as 'background'
  &end // of exportAssets

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14
  
  &exportAssets
    14 &as 'tooltip'
  &end // of exportAssets
  
  &exportAssets
    2 &as 'background'
  &end // of exportAssets

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 24 // total frames: 1
  &end // of defineMovieClip 24

  &defineMovieClip 31 // total frames: 1
  &end // of defineMovieClip 31

  &defineMovieClip 36 // total frames: 1
  &end // of defineMovieClip 36

  &defineMovieClip 44 // total frames: 120

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29

    &frame 39
      &stop
    &end // of frame 39

    &frame 49
      &stop
    &end // of frame 49

    &frame 59
      &stop
    &end // of frame 59

    &frame 86
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'introComplete'
      &callmp
      &stop
    &end // of frame 86

    &frame 119
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'outtroComplete'
      &callmp
      &stop
    &end // of frame 119
  &end // of defineMovieClip 44
  
  &exportAssets
    44 &as 'showHideContentMCSymbol'
  &end // of exportAssets

  &defineMovieClip 46 // total frames: 1
  &end // of defineMovieClip 46
  
  &exportAssets
    46 &as 'Lobby List Frame Long Movie Clip Symbol'
  &end // of exportAssets

  &defineMovieClip 48 // total frames: 1
  &end // of defineMovieClip 48
  
  &exportAssets
    48 &as 'Lobby List Frame Short Movie Clip Symbol'
  &end // of exportAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 49
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 50
  &end // of importAssets

  &defineMovieClip 51 // total frames: 1

    &placeMovieClip 49 &as 'lobbyListTextBox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_global.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 10
        &setVariable
        &pushsdb 3							//'m_vPadding'
        &pushbyte 5
        &setVariable
        &pushsdb 4							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 5							//'m_refFM'
        &pushsdb 6							//'_global.gFM'
        &setVariable
        &pushsdb 7							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 8							//'m_tabIndex'
        &pushzerosv
        &pushsdb 9							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 10							//'m_scrollbarOffset'
        &pushzero
        &initArray
        &setVariable
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushzero
        &pushbyte 10
        &setMember
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushone
        &pushzero
        &setMember
        &pushsdb 11							//'m_BgVisible'
        &pushfalse
        &setVariable
      &end
    &end // of placeMovieClip 49

    &placeMovieClip 50 &as 'uiScrollBar'
    
      &onClipEvent &construct
        &pushs 'm_nLength'
        &pushbyte 100
        &setVariable
        &pushs 'm_nMax'
        &pushbyte 10
        &setVariable
        &pushs 'm_nMin'
        &pushzerosv
        &pushs 'm_nValue'
        &pushzerosv
      &end
    &end // of placeMovieClip 50
  &end // of defineMovieClip 51

  &defineMovieClip 52 // total frames: 1
  &end // of defineMovieClip 52

  &defineMovieClip 53 // total frames: 1
  &end // of defineMovieClip 53
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 54
  &end // of importAssets

  &defineMovieClip 56 // total frames: 1
  &end // of defineMovieClip 56

  &defineMovieClip 58 // total frames: 1
  &end // of defineMovieClip 58

  &defineMovieClip 60 // total frames: 1
  &end // of defineMovieClip 60

  &defineMovieClip 63 // total frames: 1

    &placeMovieClip 54 &as 'showListOfLobbiesButton'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$ShowLobbyList', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_global.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'showHideContentMCSymbol'  
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
        &pushsdb 11							//'$ShowLobbyList'
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//'$ShowLobbyList'
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_global.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushtrue
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushone
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'showHideContentMCSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 54
  &end // of defineMovieClip 63

  &defineMovieClip 65 // total frames: 1

    &placeMovieClip 54 &as 'showListOfLobbiesButton'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$ShowLobbyList', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_global.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'showHideContentMCSymbol'  
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
        &pushsdb 11							//'$ShowLobbyList'
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//'$ShowLobbyList'
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_global.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushtrue
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushone
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'showHideContentMCSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 54
  &end // of defineMovieClip 65

  &defineMovieClip 67 // total frames: 1

    &placeMovieClip 54 &as 'showListOfLobbiesButton'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$ShowLobbyList', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_global.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'showHideContentMCSymbol'  
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
        &pushsdb 11							//'$ShowLobbyList'
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//'$ShowLobbyList'
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_global.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushtrue
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushone
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'showHideContentMCSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 54
  &end // of defineMovieClip 67

  &defineMovieClip 68 // total frames: 1
  &end // of defineMovieClip 68
  
  &exportAssets
    68 &as 'LobbyListComponentSymbol'
  &end // of exportAssets
  
  &exportAssets
    46 &as 'Lobby List Frame Long Movie Clip Symbol'
  &end // of exportAssets
  
  &exportAssets
    48 &as 'Lobby List Frame Short Movie Clip Symbol'
  &end // of exportAssets

  &defineMovieClip 69 // total frames: 1

    &placeMovieClip 49 &as 'ListTextBox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 15
        &setVariable
        &pushsdb 3							//'m_vPadding'
        &pushzerosv
        &pushsdb 4							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 5							//'m_refFM'
        &pushsdb 6							//'_root.gFM'
        &setVariable
        &pushsdb 7							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 8							//'m_tabIndex'
        &pushzerosv
        &pushsdb 9							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 10							//'m_scrollbarOffset'
        &pushzero
        &initArray
        &setVariable
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushzero
        &pushbyte 10
        &setMember
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushone
        &pushzero
        &setMember
        &pushsdb 11							//'m_BgVisible'
        &pushfalse
        &setVariable
        &pushsdb 12							//'m_renderWidth'
        &pushzerosv
        &pushsdb 13							//'m_renderHeight'
        &pushzerosv
        &pushsdb 14							//'m_backgroundMargin'
        &pushbyte 3
        &setVariable
        &pushsdb 15							//'m_useHighlights'
        &pushtrue
        &setVariable
        &pushsdb 16							//'m_alternateBackgroundColors'
        &pushtrue
        &setVariable
        &pushsdb 17							//'m_backgroundColor1'
        &pushlong 2622983
        &setVariable
        &pushsdb 18							//'m_backgroundColor2'
        &pushzerosv
      &end
    &end // of placeMovieClip 49

    &placeMovieClip 50 &as 'uiScrollBar'
    
      &onClipEvent &construct
        &pushs 'm_nLength'
        &pushbyte 100
        &setVariable
        &pushs 'm_nMax'
        &pushbyte 10
        &setVariable
        &pushs 'm_nMin'
        &pushzerosv
        &pushs 'm_nValue'
        &pushzerosv
      &end
    &end // of placeMovieClip 50
  &end // of defineMovieClip 69
  
  &exportAssets
    69 &as 'inviteListComponentSymbol'
  &end // of exportAssets

  &defineMovieClip 70 // total frames: 1

    &placeMovieClip 50 &as 'uiScrollBar'
    
      &onClipEvent &construct
        &pushs 'm_nLength'
        &pushbyte 100
        &setVariable
        &pushs 'm_nMax'
        &pushbyte 10
        &setVariable
        &pushs 'm_nMin'
        &pushzerosv
        &pushs 'm_nValue'
        &pushzerosv
      &end
    &end // of placeMovieClip 50

    &placeMovieClip 49 &as 'gameListTextBox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_global.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 10
        &setVariable
        &pushsdb 3							//'m_vPadding'
        &pushbyte 5
        &setVariable
        &pushsdb 4							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 5							//'m_refFM'
        &pushsdb 6							//'_global.gFM'
        &setVariable
        &pushsdb 7							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 8							//'m_tabIndex'
        &pushzerosv
        &pushsdb 9							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 10							//'m_scrollbarOffset'
        &pushzero
        &initArray
        &setVariable
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushzero
        &pushbyte 10
        &setMember
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushone
        &pushzero
        &setMember
      &end
    &end // of placeMovieClip 49
  &end // of defineMovieClip 70
  
  &exportAssets
    70 &as 'GameListComponentSymbol'
  &end // of exportAssets

  &defineMovieClip 71 // total frames: 1

    &placeMovieClip 49 &as 'playerListTextBox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 15
        &setVariable
        &pushsdb 3							//'m_vPadding'
        &pushzerosv
        &pushsdb 4							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 5							//'m_refFM'
        &pushsdb 6							//'_root.gFM'
        &setVariable
        &pushsdb 7							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 8							//'m_tabIndex'
        &pushzerosv
        &pushsdb 9							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 10							//'m_scrollbarOffset'
        &pushzero
        &initArray
        &setVariable
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushzero
        &pushbyte 10
        &setMember
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushone
        &pushzero
        &setMember
        &pushsdb 11							//'m_BgVisible'
        &pushfalse
        &setVariable
        &pushsdb 12							//'m_renderWidth'
        &pushzerosv
        &pushsdb 13							//'m_renderHeight'
        &pushzerosv
        &pushsdb 14							//'m_backgroundMargin'
        &pushbyte 3
        &setVariable
        &pushsdb 15							//'m_useHighlights'
        &pushtrue
        &setVariable
        &pushsdb 16							//'m_alternateBackgroundColors'
        &pushtrue
        &setVariable
        &pushsdb 17							//'m_backgroundColor1'
        &pushlong 2622983
        &setVariable
        &pushsdb 18							//'m_backgroundColor2'
        &pushzerosv
      &end
    &end // of placeMovieClip 49

    &placeMovieClip 50 &as 'uiScrollBar'
    
      &onClipEvent &construct
        &pushs 'm_nLength'
        &pushbyte 100
        &setVariable
        &pushs 'm_nMax'
        &pushbyte 10
        &setVariable
        &pushs 'm_nMin'
        &pushzerosv
        &pushs 'm_nValue'
        &pushzerosv
      &end
    &end // of placeMovieClip 50
  &end // of defineMovieClip 71
  
  &exportAssets
    71 &as 'PlayerListComponentSymbol'
  &end // of exportAssets

  &defineMovieClip 72 // total frames: 1

    &placeMovieClip 49 &as 'listbox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_global.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 15
        &setVariable
        &pushsdb 3							//'m_vPadding'
        &pushzerosv
        &pushsdb 4							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 5							//'m_refFM'
        &pushsdb 6							//'_global.gFM'
        &setVariable
        &pushsdb 7							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 8							//'m_tabIndex'
        &pushzerosv
        &pushsdb 9							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 10							//'m_scrollbarOffset'
        &pushzero
        &initArray
        &setVariable
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushzero
        &pushbyte 10
        &setMember
        &pushsdbgv 10							//'m_scrollbarOffset'
        &pushone
        &pushzero
        &setMember
        &pushsdb 11							//'m_BgVisible'
        &pushfalse
        &setVariable
        &pushsdb 12							//'m_renderWidth'
        &pushzerosv
        &pushsdb 13							//'m_renderHeight'
        &pushzerosv
        &pushsdb 14							//'m_backgroundMargin'
        &pushbyte 3
        &setVariable
        &pushsdb 15							//'m_useHighlights'
        &pushtrue
        &setVariable
        &pushsdb 16							//'m_alternateBackgroundColors'
        &pushtrue
        &setVariable
        &pushsdb 17							//'m_backgroundColor1'
        &pushlong 2622983
        &setVariable
        &pushsdb 18							//'m_backgroundColor2'
        &pushzerosv
      &end
    &end // of placeMovieClip 49

    &placeMovieClip 50 &as 'scrollbar'
    
      &onClipEvent &construct
        &pushs 'm_nLength'
        &pushbyte 100
        &setVariable
        &pushs 'm_nMax'
        &pushbyte 10
        &setVariable
        &pushs 'm_nMin'
        &pushzerosv
        &pushs 'm_nValue'
        &pushzerosv
      &end
    &end // of placeMovieClip 50
  &end // of defineMovieClip 72
  
  &exportAssets
    72 &as 'BuddyListComponentSymbol'
  &end // of exportAssets

  &defineMovieClip 74 // total frames: 1

    &frame 0
      &pushthisgv
      &pushs '_alpha'
      &pushzero
      &setMember
    &end // of frame 0
  &end // of defineMovieClip 74
  
  &exportAssets
    74 &as 'BuddyStatusIconSymbol'
  &end // of exportAssets
  
  &exportAssets
    76 &as 'LobbyListPanelMask'
  &end // of exportAssets
&end
