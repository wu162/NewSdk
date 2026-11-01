movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IGM_UN~1\\IGM_UN~1.eaf' &compressed // flash 7, total frames: 15, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 9 // total frames: 120

    &frame 0
    &end // of frame 0

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
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as 'AudioButtonContent'
  &end // of exportAssets

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'SimpleRenderingSurface' &as 14
  &end // of importAssets

  &defineMovieClip 21 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 21
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 24
  &end // of importAssets

  &defineMovieClip 25 // total frames: 1

    &placeMovieClip 24 &as 'audioButtonMC'
    
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
        &pushone
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_label'
        &pushssv ''
        &pushs 'm_contentSymbol'
        &pushssv 'AudioButtonContent'
      &end
    &end // of placeMovieClip 24

    &placeMovieClip 24 &as 'closeButtonMC'
    
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
        &pushzerosv
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_label'
        &pushssv '$Close'
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 24
  &end // of defineMovieClip 25

  &defineMovieClip 26 // total frames: 0
  &end // of defineMovieClip 26
  
  &exportAssets
    26 &as '__Packages.igm_UnitIntroDialog'
  &end // of exportAssets
  
  &initMovieClip 26
    &constants '_global', 'igm_UnitIntroDialog', 'igm_UnitIntroDialog::igm_UnitIntroDialog() clip = ', 'extern', 'InGame', 'qualifyName', ':', 'm_clip', '_parent', 'OnContentLoaded', 'Close', 'close', 'bind0', 'GetMovieSurface', 'getMovieSurface', 'SetFaction', 'setFaction', 'bind1DynamicParams', 'OnUnload', 'onUnload', 'initCloseButton', 'initAudioButton', 'initBackground', 'dialogMC', 'titleTF', 'text', '$', 'Title', '&outline', 'FSCommand:', 'OnInitialized', '', 'prototype', 'onClosed', 'igm_UnitIntroDialog::onClosed()', 'OnClosed', 'igm_UnitIntroDialog::close()', 'm_closing', '_close', 'gotoAndPlay', 'igm_UnitIntroDialog::getMovieSurface()', 'movieSurfaceMC', 'Pointer', 'encode', 'igm_UnitIntroDialog::setFaction( ', ' )', 'flagMC', 'igm_UnitIntroDialog::onUnload()', 'onCloseButtonClicked', 'igm_UnitIntroDialog::onCloseButtonClicked()', 'OnCloseButtonClicked', 'igm_UnitIntroDialog::initCloseButton()', 'closeButtonMC', '_visible', 'enable', 'setOnMouseUpFunction', 'onAudioButtonClicked', 'igm_UnitIntroDialog::onAudioButtonClicked()', 'OnAudioButtonClicked', 'igm_UnitIntroDialog::initAudioButton()', 'audioButtonMC', 'igm_UnitIntroDialog::initBackground()', '_root', 'backgroundMC', 'displayLeft', 'displayTop', '_x', '_y', '_width', 'displayWidth', '_height', 'displayHeight', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'igm_UnitIntroDialog'
    &not
    &not
    &jnz label6    
    &pushglobalgv
    &pushsdb 1							//'igm_UnitIntroDialog'
    &function2  (r:3='clip') (r:1='this', r:2='_global')
      &pushsdb 2							//'igm_UnitIntroDialog::igm_UnitIntroDialog() clip = '
      &pushregister 3
      &add
      &trace
      &pushregister 2
      &pushsdbgm 3							//'extern'
      &pushsdbgm 4							//'InGame'
      &not
      &dup
      &not
      &jnz label1      
      &pop
      &pushregister 2
      &pushsdbgm 5							//'qualifyName'
      &pushundef
      &equals
     label1:
      &not
      &jnz label2      
      &pushregister 2
      &pushsdb 5							//'qualifyName'
      &function2  (r:2='scope', r:1='name') ()
        &pushregister 2
        &toString
        &pushsdb 6							//':'
        &add
        &pushregister 1
        &add
        &return
      &end // of function 

      &setMember
     label2:
      &pushregister 1
      &pushsdb 7							//'m_clip'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 8							//'_parent'
      &dcallmp 9							// OnContentLoaded()
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 10							//'Close'
      &pushregister 1
      &pushsdbgm 11							//'close'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 13							//'GetMovieSurface'
      &pushregister 1
      &pushsdbgm 14							//'getMovieSurface'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 15							//'SetFaction'
      &pushregister 1
      &pushsdbgm 16							//'setFaction'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 18							//'OnUnload'
      &pushregister 1
      &pushsdbgm 19							//'onUnload'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 20							// initCloseButton()
      &pushzero
      &pushregister 1
      &dcallmp 21							// initAudioButton()
      &pushzero
      &pushregister 1
      &dcallmp 22							// initBackground()
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 23							//'dialogMC'
      &pushsdbgm 24							//'titleTF'
      &pushsdb 25							//'text'
      &pushsdb 26							//'$'
      &pushsdb 27							//'Title'
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'qualifyName'
      &callMethod
      &add
      &pushsdb 28							//'&outline'
      &add
      &setMember
      &pushsdb 29							//'FSCommand:'
      &pushsdb 30							//'OnInitialized'
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 31							//''
      &getURL2
    &end // of function 

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 32							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 33							//'onClosed'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 34							//'igm_UnitIntroDialog::onClosed()'
      &trace
      &pushsdb 29							//'FSCommand:'
      &pushsdb 35							//'OnClosed'
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 31							//''
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 11							//'close'
    &function2  () (r:1='this')
      &pushsdb 36							//'igm_UnitIntroDialog::close()'
      &trace
      &pushregister 1
      &pushsdbgm 37							//'m_closing'
      &not
      &not
      &jnz label3      
      &pushsdb 38							//'_close'
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &dcallmp 39							// gotoAndPlay()
      &pushregister 1
      &pushsdb 37							//'m_closing'
      &pushtrue
      &setMember
     label3:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 14							//'getMovieSurface'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 40							//'igm_UnitIntroDialog::getMovieSurface()'
      &trace
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 23							//'dialogMC'
      &pushsdbgm 41							//'movieSurfaceMC'
      &pushone
      &pushregister 2
      &pushsdbgm 42							//'Pointer'
      &pushsdb 43							//'encode'
      &callMethod
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 16							//'setFaction'
    &function2  (r:2='faction') (r:1='this')
      &pushsdb 44							//'igm_UnitIntroDialog::setFaction( '
      &pushregister 2
      &add
      &pushsdb 45							//' )'
      &add
      &trace
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 23							//'dialogMC'
      &pushsdbgm 46							//'flagMC'
      &dcallmp 39							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 19							//'onUnload'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 47							//'igm_UnitIntroDialog::onUnload()'
      &trace
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 10							//'Close'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 13							//'GetMovieSurface'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 15							//'SetFaction'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdb 18							//'OnUnload'
      &delete
      &pop
      &pushregister 2
      &pushsdb 1							//'igm_UnitIntroDialog'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 48							//'onCloseButtonClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 49							//'igm_UnitIntroDialog::onCloseButtonClicked()'
      &trace
      &pushsdb 29							//'FSCommand:'
      &pushsdb 50							//'OnCloseButtonClicked'
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 31							//''
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 20							//'initCloseButton'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 51							//'igm_UnitIntroDialog::initCloseButton()'
      &trace
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 23							//'dialogMC'
      &pushsdbgm 52							//'closeButtonMC'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 53							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 3
      &dcallmp 54							// enable()
      &pushregister 1
      &pushsdbgm 48							//'onCloseButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 55							// setOnMouseUpFunction()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 56							//'onAudioButtonClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 57							//'igm_UnitIntroDialog::onAudioButtonClicked()'
      &trace
      &pushsdb 29							//'FSCommand:'
      &pushsdb 58							//'OnAudioButtonClicked'
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 31							//''
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 21							//'initAudioButton'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 59							//'igm_UnitIntroDialog::initAudioButton()'
      &trace
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 23							//'dialogMC'
      &pushsdbgm 60							//'audioButtonMC'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 53							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 3
      &dcallmp 54							// enable()
      &pushregister 1
      &pushsdbgm 56							//'onAudioButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 12							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 55							// setOnMouseUpFunction()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 22							//'initBackground'
    &function2  () (r:1='this')
      &pushsdb 61							//'igm_UnitIntroDialog::initBackground()'
      &trace
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 62							//'_root'
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdbgm 7							//'m_clip'
      &pushsdbgm 23							//'dialogMC'
      &pushsdbgm 63							//'backgroundMC'
      &setRegister r:4
      &pop
      &pushregister 3
      &pushsdbgm 64							//'displayLeft'
      &setRegister r:6
      &pop
      &pushregister 3
      &pushsdbgm 65							//'displayTop'
      &setRegister r:5
      &pop
      &pushregister 4
      &setRegister r:2
      &pop
     label4:
      &pushregister 2
      &pushregister 3
      &equals
      &not
      &not
      &jnz label5      
      &pushregister 6
      &pushregister 2
      &pushsdbgm 66							//'_x'
      &subtract
      &setRegister r:6
      &pop
      &pushregister 5
      &pushregister 2
      &pushsdbgm 67							//'_y'
      &subtract
      &setRegister r:5
      &pop
      &pushregister 2
      &pushsdbgm 8							//'_parent'
      &setRegister r:2
      &pop
      &jmp label4      
     label5:
      &pushregister 4
      &pushsdb 66							//'_x'
      &pushregister 6
      &setMember
      &pushregister 4
      &pushsdb 67							//'_y'
      &pushregister 5
      &setMember
      &pushregister 4
      &pushsdb 68							//'_width'
      &pushregister 3
      &pushsdbgm 69							//'displayWidth'
      &setMember
      &pushregister 4
      &pushsdb 70							//'_height'
      &pushregister 3
      &pushsdbgm 71							//'displayHeight'
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 37							//'m_closing'
    &pushfalse
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'igm_UnitIntroDialog'
    &pushsdbgm 32							//'prototype'
    &pushbyte 3
    &pushsdb 72							//'ASSetPropFlags'
    &callFunction
   label6:
    &pop
  &end // of initMovieClip 26

  &frame 1
    &constants 'controller', 'this', 'igm_UnitIntroDialog'  
    &pushsdbgv 0							//'controller'
    &pushundef
    &equals
    &not
    &jnz label1    
    &pushsdb 0							//'controller'
    &pushthisgv
    &pushone
    &pushsdb 2							//'igm_UnitIntroDialog'
    &new
    &setVariable
   label1:
  &end // of frame 1

  &frame 11
    &stop
  &end // of frame 11

  &frame 14
    &stop
    &pushzero
    &pushsgv 'controller'
    &pushs 'onClosed'
    &callmp
  &end // of frame 14
&end
