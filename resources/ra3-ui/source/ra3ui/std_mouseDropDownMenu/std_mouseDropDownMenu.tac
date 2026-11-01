movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\STD_MO~4\\STD_MO~1.eaf' // flash 7, total frames: 1, frame rate: 30 fps, 640x480 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 22

    &frame 0
    &end // of frame 0

    &frame 3
      &stop
    &end // of frame 3

    &frame 6
      &stop
    &end // of frame 6

    &frame 9
      &stop
    &end // of frame 9

    &frame 12
      &stop
    &end // of frame 12

    &frame 15
      &stop
    &end // of frame 15

    &frame 18
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'introComplete'
      &callmp
      &stop
    &end // of frame 18

    &frame 21
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'outtroComplete'
      &callmp
      &stop
    &end // of frame 21
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as 'DropdownButtonContent'
  &end // of exportAssets

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'std_mouseDropdownMenuVisRef'
  &end // of exportAssets

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14
  
  &exportAssets
    14 &as 'std_mouseDropDownMenuSymbol'
  &end // of exportAssets
  
  &exportAssets
    13 &as 'std_mouseDropdownMenuVisRef'
  &end // of exportAssets

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 21
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 22
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 23
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseListboxEntryRenderImageElementSymbol' &as 24
  &end // of importAssets

  &defineMovieClip 25 // total frames: 1

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 21 &as 'listbox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 5
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
        &pushfalse
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
    &end // of placeMovieClip 21

    &placeMovieClip 22 &as 'scrollbar'
    
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
    &end // of placeMovieClip 22

    &placeMovieClip 23 &as 'button'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
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
        &pushs 'm_label'
        &pushssv ''
        &pushs 'm_contentSymbol'
        &pushssv 'DropdownButtonContent'
      &end
    &end // of placeMovieClip 23
  &end // of defineMovieClip 25
  
  &exportAssets
    25 &as 'RenderImageDropdownContentSymbol'
  &end // of exportAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseListboxEntryColorSwatchElementSymbol' &as 26
  &end // of importAssets

  &defineMovieClip 27 // total frames: 1

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 21 &as 'listbox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 5
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
        &pushfalse
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
    &end // of placeMovieClip 21

    &placeMovieClip 22 &as 'scrollbar'
    
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
    &end // of placeMovieClip 22

    &placeMovieClip 23 &as 'button'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
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
        &pushs 'm_label'
        &pushssv ''
        &pushs 'm_contentSymbol'
        &pushssv 'DropdownButtonContent'
      &end
    &end // of placeMovieClip 23
  &end // of defineMovieClip 27
  
  &exportAssets
    27 &as 'ColorSwatchDropdownContentSymbol'
  &end // of exportAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseListboxEntryTextElementSymbol' &as 28
  &end // of importAssets

  &defineMovieClip 29 // total frames: 1

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 21 &as 'listbox'
    
      &onClipEvent &construct
        &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
        &pushsdb 0							//'m_focusDirs'
        &pushsdb 1							//'Up/Down'
        &setVariable
        &pushsdb 2							//'m_numEntries'
        &pushbyte 5
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
        &pushtrue
        &setVariable
        &pushsdb 12							//'m_renderWidth'
        &pushzerosv
        &pushsdb 13							//'m_renderHeight'
        &pushzerosv
        &pushsdb 14							//'m_backgroundMargin'
        &pushbyte 2
        &setVariable
        &pushsdb 15							//'m_useHighlights'
        &pushtrue
        &setVariable
        &pushsdb 16							//'m_alternateBackgroundColors'
        &pushfalse
        &setVariable
        &pushsdb 17							//'m_backgroundColor1'
        &pushzerosv
        &pushsdb 18							//'m_backgroundColor2'
        &pushzerosv
      &end
    &end // of placeMovieClip 21

    &placeMovieClip 22 &as 'scrollbar'
    
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
    &end // of placeMovieClip 22

    &placeMovieClip 23 &as 'button'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
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
        &pushs 'm_label'
        &pushssv ''
        &pushs 'm_contentSymbol'
        &pushssv 'DropdownButtonContent'
      &end
    &end // of placeMovieClip 23
  &end // of defineMovieClip 29
  
  &exportAssets
    29 &as 'TextDropdownMenuContentSymbol'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'DropdownButtonHitRegion'
  &end // of exportAssets
&end
