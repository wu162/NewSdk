movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\CHATCO~1\\CHATCO~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 1
  &end // of importAssets
  
  &importAssets &from '../../cafe/mouseComponents/std_mouseTextInputField.swf'
    'std_mouseTextInputFieldSymbol' &as 2
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 3
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 4
  &end // of importAssets

  &defineMovieClip 5 // total frames: 1

    &placeMovieClip 1 &as 'uiScrollingTextBox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
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
        &pushsdb 6							//'_root.gFM'
        &setVariable
        &pushsdb 7							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 8							//'m_tabIndex'
        &pushbyte -1
        &setVariable
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
        &pushfalse
        &setVariable
        &pushsdb 16							//'m_alternateBackgroundColors'
        &pushfalse
        &setVariable
        &pushsdb 17							//'m_backgroundColor1'
        &pushzerosv
        &pushsdb 18							//'m_backgroundColor2'
        &pushzerosv
      &end
    &end // of placeMovieClip 1

    &placeMovieClip 2 &as 'chatInputBox'
    
      &onClipEvent &construct
        &pushs 'm_width'
        &pushshort 200
        &setVariable
        &pushs 'm_textAlign'
        &pushfalse
        &setVariable
        &pushs 'm_labelPosition'
        &pushssv 'right align'
        &pushs 'm_label_x'
        &pushzerosv
        &pushs 'm_label_y'
        &pushzerosv
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_iconType'
        &pushssv 'default'
        &pushs 'm_nTruncateType'
        &pushzerosv
        &pushs 'm_label'
        &pushssv ''
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushzerosv
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_password'
        &pushfalse
        &setVariable
        &pushs 'm_charLimit'
        &pushshort 250
        &setVariable
        &pushs 'm_allowHorzScroll'
        &pushtrue
        &setVariable
      &end
    &end // of placeMovieClip 2

    &placeMovieClip 3 &as 'sendButton'
    
      &onClipEvent &construct
        &pushs 'm_width'
        &pushbyte 60
        &setVariable
        &pushs 'm_textAlign'
        &pushfalse
        &setVariable
        &pushs 'm_labelPosition'
        &pushssv 'right align'
        &pushs 'm_label_x'
        &pushzerosv
        &pushs 'm_label_y'
        &pushzerosv
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_iconType'
        &pushssv 'default'
        &pushs 'm_extLabel'
        &pushssv ''
        &pushs 'm_nTruncateType'
        &pushzerosv
        &pushs 'm_label'
        &pushssv 'Send'
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'chatSendButtonSymbol'
      &end
    &end // of placeMovieClip 3

    &placeMovieClip 4 &as 'uiScrollBar'
    
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
    &end // of placeMovieClip 4
  &end // of defineMovieClip 5
  
  &exportAssets
    5 &as 'chatComponentSymbol'
  &end // of exportAssets

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14

  &defineMovieClip 15 // total frames: 48

    &frame 5
      &stop
    &end // of frame 5

    &frame 11
      &stop
    &end // of frame 11

    &frame 17
      &stop
    &end // of frame 17

    &frame 23
      &stop
    &end // of frame 23

    &frame 29
      &stop
    &end // of frame 29

    &frame 35
      &stop
    &end // of frame 35

    &frame 41
      &stop
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'introComplete'
      &callmp
    &end // of frame 41

    &frame 47
      &stop
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'outtroComplete'
      &callmp
    &end // of frame 47
  &end // of defineMovieClip 15
  
  &exportAssets
    15 &as 'chatSendButtonSymbol'
  &end // of exportAssets
&end
