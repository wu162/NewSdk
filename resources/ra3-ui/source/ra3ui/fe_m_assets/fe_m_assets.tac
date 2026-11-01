movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FE_M_A~1\\FE_M_A~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1280x720 px
  
  &exportAssets
    1 &as 'Screen_stripe_upper.png'
  &end // of exportAssets
  
  &exportAssets
    2 &as 'Screen_stripe_lower.png'
  &end // of exportAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 5
  &end // of importAssets
  
  &importAssets &from '../../cafe/mouseComponents/std_mouseTextInputField.swf'
    'std_mouseTextInputFieldSymbol' &as 6
  &end // of importAssets

  &defineMovieClip 11 // total frames: 1

    &placeMovieClip 5 &as 'btnNewCancel'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'None'
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
        &pushssv '$CANCEL'
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5

    &placeMovieClip 5 &as 'btnNewConfirm'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'None'
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
        &pushssv '$ACCEPT'
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 5

    &placeMovieClip 6 &as 'newProfileName'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'top center', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_password', 'm_charLimit', 'm_allowHorzScroll'  
        &pushsdb 0							//'m_width'
        &pushshort 338
        &setVariable
        &pushsdb 1							//'m_textAlign'
        &pushtrue
        &setVariable
        &pushsdb 2							//'m_labelPosition'
        &pushsdb 3							//'top center'
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
        &pushsdb 11							//''
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//''
        &setVariable
        &pushsdb 14							//'m_bEnabled'
        &pushtrue
        &setVariable
        &pushsdb 15							//'m_refFM'
        &pushsdb 16							//'_root.gFM'
        &setVariable
        &pushsdb 17							//'m_initiallySelected'
        &pushfalse
        &setVariable
        &pushsdb 18							//'m_tabIndex'
        &pushbyte -1
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_password'
        &pushfalse
        &setVariable
        &pushsdb 21							//'m_charLimit'
        &pushbyte 16
        &setVariable
        &pushsdb 22							//'m_allowHorzScroll'
        &pushfalse
        &setVariable
      &end
    &end // of placeMovieClip 6
  &end // of defineMovieClip 11
  
  &exportAssets
    11 &as 'newProfileMC'
  &end // of exportAssets
  
  &exportAssets
    2 &as 'Screen_stripe_lower.png'
  &end // of exportAssets

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'lowerScreenBar MC'
  &end // of exportAssets
  
  &exportAssets
    1 &as 'Screen_stripe_upper.png'
  &end // of exportAssets

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15
  
  &exportAssets
    15 &as 'upperScreenTitleBar MC'
  &end // of exportAssets
&end
