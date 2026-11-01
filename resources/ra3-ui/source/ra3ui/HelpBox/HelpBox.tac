movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\HelpBox\\HelpBox.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 3 // total frames: 1

    &frame 0
      &constants '', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 Resize (r:2='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &setProperty
      &end // of function Resize

      &pushsdbgv 1							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 1							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 2							//'FSCommand:'
      &pushsdb 3							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 5							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0

    &placeMovieClip 2 
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv ''
      &end
    
      &onClipEvent &load
        &pushs '_type'
        &pushsgv '_parent'
        &toString
        &pushs ':Render'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 2
  &end // of defineMovieClip 3
  
  &exportAssets
    3 &as 'RenderingSurface'
  &end // of exportAssets

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5
  
  &exportAssets
    5 &as 'HelpBoxSiteMarker'
  &end // of exportAssets

  &defineMovieClip 6 // total frames: 1

    &frame 0
      &constants 'x', '', 'y', '_xscale', '_yscale', '_x', '_y', '_parent', 'vertAlignment', 'horzAlignment', 'GetHelpBoxPos', 'GetHelpBoxVerticalAlignment', 'SetHelpBoxVerticalAlignment', 'GetHelpBoxHorizontalAlignment', 'SetHelpBoxHorizontalAlignment', 'onUnload'  
      &function2 GetHelpBoxPos () (r:1='_root', r:2='_parent')
        &pushsdb 0							//'x'
        &pushsdb 1							//''
        &pushzero
        &getProperty
        &pushsdb 2							//'y'
        &pushsdb 1							//''
        &pushone
        &getProperty
        &pushbyte 2
        &initObject
        &setRegister r:4
        &pop
        &pushregister 1
        &setRegister r:5
        &pop
        &pushregister 2
        &setRegister r:3
        &pop
       label1:
        &pushregister 3
        &pushregister 5
        &equals
        &not
        &not
        &jnz label2        
        &pushregister 4
        &pushsdb 0							//'x'
        &pushregister 4
        &pushsdbgm 0							//'x'
        &pushregister 3
        &pushsdbgm 3							//'_xscale'
        &pushfloat 0.009999999776483
        &multiply
        &multiply
        &setMember
        &pushregister 4
        &pushsdb 2							//'y'
        &pushregister 4
        &pushsdbgm 2							//'y'
        &pushregister 3
        &pushsdbgm 4							//'_yscale'
        &pushfloat 0.009999999776483
        &multiply
        &multiply
        &setMember
        &pushregister 4
        &pushsdb 0							//'x'
        &pushregister 4
        &pushsdbgm 0							//'x'
        &pushregister 3
        &pushsdbgm 5							//'_x'
        &add
        &setMember
        &pushregister 4
        &pushsdb 2							//'y'
        &pushregister 4
        &pushsdbgm 2							//'y'
        &pushregister 3
        &pushsdbgm 6							//'_y'
        &add
        &setMember
        &pushregister 3
        &pushsdbgm 7							//'_parent'
        &setRegister r:3
        &pop
        &jmp label1        
       label2:
        &pushregister 4
        &return
      &end // of function GetHelpBoxPos

      &function GetHelpBoxVerticalAlignment (      )      
        &pushsdbgv 8							//'vertAlignment'
        &return
      &end // of function GetHelpBoxVerticalAlignment

      &function2 SetHelpBoxVerticalAlignment (r:1='newAlignment') ()
        &pushsdb 8							//'vertAlignment'
        &pushregister 1
        &setVariable
      &end // of function SetHelpBoxVerticalAlignment

      &function GetHelpBoxHorizontalAlignment (      )      
        &pushsdbgv 9							//'horzAlignment'
        &return
      &end // of function GetHelpBoxHorizontalAlignment

      &function2 SetHelpBoxHorizontalAlignment (r:1='newAlignment') ()
        &pushsdb 9							//'horzAlignment'
        &pushregister 1
        &setVariable
      &end // of function SetHelpBoxHorizontalAlignment

      &function onUnload (      )      
        &pushsdb 10							//'GetHelpBoxPos'
        &delete2
        &pop
        &pushsdb 11							//'GetHelpBoxVerticalAlignment'
        &delete2
        &pop
        &pushsdb 12							//'SetHelpBoxVerticalAlignment'
        &delete2
        &pop
        &pushsdb 13							//'GetHelpBoxHorizontalAlignment'
        &delete2
        &pop
        &pushsdb 14							//'SetHelpBoxHorizontalAlignment'
        &delete2
        &pop
        &pushsdb 15							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

    &end // of frame 0

    &placeMovieClip 5 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 5
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as 'HelpBoxSite'
  &end // of exportAssets
  
  &exportAssets
    5 &as 'HelpBoxSiteMarker'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 10 // total frames: 1

    &frame 0
      &constants '', 'Math', 'min', 'titleText', 'field', 'textColor', 'Move', 'SetColor', 'onUnload', 'initialized', 'text', '$', 'Text', 'this', 'qualifyName', '&outline', 'autoSize', 'left', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function2 SetColor (r:1='colorArg') ()
        &pushsdbgv 3							//'titleText'
        &pushsdbgm 4							//'field'
        &pushsdb 5							//'textColor'
        &pushregister 1
        &toNumber
        &setMember
      &end // of function SetColor

      &function onUnload (      )      
        &pushsdb 6							//'Move'
        &delete2
        &pop
        &pushsdb 7							//'SetColor'
        &delete2
        &pop
        &pushsdb 8							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 9							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 3							//'titleText'
      &pushsdb 10							//'text'
      &pushsdb 11							//'$'
      &pushsdb 12							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 14							//'qualifyName'
      &callFunction
      &add
      &pushsdb 15							//'&outline'
      &add
      &setMember
      &pushsdbgv 3							//'titleText'
      &pushsdbgm 4							//'field'
      &pushsdb 16							//'autoSize'
      &pushsdb 17							//'left'
      &setMember
      &pushsdbgv 3							//'titleText'
      &pushsdbgm 4							//'field'
      &pushsdbgm 18							//'_width'
      &pop
      &pushsdb 9							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 19							//'FSCommand:'
      &pushsdb 20							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 14							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 10
  
  &exportAssets
    10 &as 'HelpPlayerName'
  &end // of exportAssets

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 1

    &frame 0
      &constants '', 'Math', 'min', 'Move', 'onUnload', 'initialized', 'titleText', 'text', '$', 'Text', 'this', 'qualifyName', '&outline', 'field', 'autoSize', 'left', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function onUnload (      )      
        &pushsdb 3							//'Move'
        &delete2
        &pop
        &pushsdb 4							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 6							//'titleText'
      &pushsdb 7							//'text'
      &pushsdb 8							//'$'
      &pushsdb 9							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 11							//'qualifyName'
      &callFunction
      &add
      &pushsdb 12							//'&outline'
      &add
      &setMember
      &pushsdbgv 6							//'titleText'
      &pushsdbgm 13							//'field'
      &pushsdb 14							//'autoSize'
      &pushsdb 15							//'left'
      &setMember
      &pushsdbgv 6							//'titleText'
      &pushsdbgm 13							//'field'
      &pushsdbgm 16							//'_width'
      &pop
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 17							//'FSCommand:'
      &pushsdb 18							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 11							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'HelpTitle'
  &end // of exportAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'BlackRectangle' &as 14
  &end // of importAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'WhiteRectangle' &as 15
  &end // of importAssets

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 21

  &defineMovieClip 24 // total frames: 1
  &end // of defineMovieClip 24

  &defineMovieClip 25 // total frames: 20

    &frame 0
      &constants '_noHotKey', 'textMC', 'gotoAndStop', 'amphibiousIcon', '_x', '_width', 'amphibiousIconMargin', 'NotAmphibious', 'Amphibious', '', 'contentStage', 'background', '_parent', 'UnregisterMasterResizeListener', 'HideHotKeyText', 'SetAmphibious', 'Move', 'OnMasterResize', 'onUnload', 'initialized', 'this', 'RegisterMasterResizeListener', 'spacing', 'hotKeyText', 'titleText', 'text', '$', 'Title', 'qualifyName', '&outline', 'field', 'autoSize', 'left', 'HotKey', 'verticalSizer', '_visible', 'FSCommand:', 'OnInitialized'  
      &function HideHotKeyText (      )      
        &pushsdb 0							//'_noHotKey'
        &pushone
        &pushsdbgv 1							//'textMC'
        &dcallmp 2							// gotoAndStop()
        &pushsdbgv 3							//'amphibiousIcon'
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushsdbgv 3							//'amphibiousIcon'
        &pushsdb 4							//'_x'
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 4							//'_x'
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 5							//'_width'
        &add
        &pushsdbgv 6							//'amphibiousIconMargin'
        &add
        &setMember
       label1:
      &end // of function HideHotKeyText

      &function2 SetAmphibious (r:1='val') ()
        &pushregister 1
        &pushzero
        &equals
        &not
        &jnz label2        
        &pushsdb 7							//'NotAmphibious'
        &jmp label3        
       label2:
        &pushsdb 8							//'Amphibious'
       label3:
        &gotoAndPlay        &pushsdbgv 3							//'amphibiousIcon'
        &pushundef
        &equals
        &not
        &not
        &jnz label4        
        &pushsdbgv 3							//'amphibiousIcon'
        &pushsdb 4							//'_x'
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 4							//'_x'
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 5							//'_width'
        &add
        &pushsdbgv 6							//'amphibiousIconMargin'
        &add
        &setMember
       label4:
      &end // of function SetAmphibious

      &function2 Move (r:6='newX', r:5='newY', r:3='newWidth', r:4='newHeight') ()
        &pushsdb 9							//''
        &pushzero
        &pushregister 6
        &setProperty
        &pushsdb 9							//''
        &pushone
        &pushregister 5
        &setProperty
        &pushregister 3
        &pushsdb 9							//''
        &pushbyte 8
        &getProperty
        &lessThan
        &not
        &jnz label8        
        &pushsdb 9							//''
        &pushbyte 8
        &getProperty
        &pushregister 3
        &subtract
        &setRegister r:1
        &pop
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 5							//'_width'
        &setRegister r:2
        &pop
        &pushsdbgv 1							//'textMC'
        &pushsdb 5							//'_width'
        &pushregister 1
        &pushregister 2
        &pushone
        &subtract
        &lessThan
        &jnz label5        
        &pushone
        &jmp label6        
       label5:
        &pushregister 2
        &pushregister 1
        &subtract
       label6:
        &setMember
        &pushsdbgv 3							//'amphibiousIcon'
        &pushundef
        &equals
        &not
        &not
        &jnz label7        
        &pushsdbgv 3							//'amphibiousIcon'
        &pushsdb 4							//'_x'
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 4							//'_x'
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 5							//'_width'
        &add
        &pushsdbgv 6							//'amphibiousIconMargin'
        &add
        &setMember
       label7:
        &pushsdb 9							//''
        &pushbyte 8
        &pushregister 3
        &setProperty
       label8:
        &pushregister 4
        &pushsdb 9							//''
        &pushbyte 9
        &getProperty
        &lessThan
        &not
        &jnz label9        
        &pushsdb 9							//''
        &pushbyte 9
        &pushregister 4
        &setProperty
       label9:
      &end // of function Move

      &function2 OnMasterResize (r:2='mc') ()
        &pushregister 2
        &pushsdbgm 10							//'contentStage'
        &pushsdbgm 4							//'_x'
        &pushsdb 9							//''
        &pushzero
        &getProperty
        &add
        &setRegister r:1
        &pop
        &pushsdbgv 11							//'background'
        &pushsdb 4							//'_x'
        &pushsdbgv 11							//'background'
        &pushsdbgm 4							//'_x'
        &pushregister 1
        &subtract
        &setMember
        &pushsdbgv 11							//'background'
        &pushsdb 5							//'_width'
        &pushregister 2
        &pushsdbgm 11							//'background'
        &pushsdbgm 5							//'_width'
        &pushbyte 100
        &multiply
        &pushsdb 9							//''
        &pushbyte 2
        &getProperty
        &divide
        &setMember
      &end // of function OnMasterResize

      &function2 onUnload () (r:1='this', r:2='_parent')
        &pushregister 1
        &pushone
        &pushregister 2
        &pushsdbgm 12							//'_parent'
        &dcallmp 13							// UnregisterMasterResizeListener()
        &pushsdb 14							//'HideHotKeyText'
        &delete2
        &pop
        &pushsdb 15							//'SetAmphibious'
        &delete2
        &pop
        &pushsdb 16							//'Move'
        &delete2
        &pop
        &pushsdb 17							//'OnMasterResize'
        &delete2
        &pop
        &pushsdb 18							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 19							//'initialized'
      &not
      &not
      &jnz label10      
      &pushsdb 6							//'amphibiousIconMargin'
      &pushbyte 8
      &setVariable
      &pushthisgv
      &pushone
      &pushsdbgv 12							//'_parent'
      &pushsdbgm 12							//'_parent'
      &dcallmp 21							// RegisterMasterResizeListener()
      &pushsdb 22							//'spacing'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 23							//'hotKeyText'
      &pushsdbgm 4							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 24							//'titleText'
      &pushsdbgm 4							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 24							//'titleText'
      &pushsdbgm 5							//'_width'
      &add
      &subtract
      &varEquals
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 24							//'titleText'
      &pushsdb 25							//'text'
      &pushsdb 26							//'$'
      &pushsdb 27							//'Title'
      &pushthisgv
      &pushbyte 2
      &pushsdb 28							//'qualifyName'
      &callFunction
      &add
      &pushsdb 29							//'&outline'
      &add
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 24							//'titleText'
      &pushsdbgm 30							//'field'
      &pushsdb 31							//'autoSize'
      &pushsdb 32							//'left'
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 24							//'titleText'
      &pushsdbgm 30							//'field'
      &pushsdbgm 5							//'_width'
      &pop
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 23							//'hotKeyText'
      &pushsdb 25							//'text'
      &pushsdb 26							//'$'
      &pushsdb 33							//'HotKey'
      &pushthisgv
      &pushbyte 2
      &pushsdb 28							//'qualifyName'
      &callFunction
      &add
      &pushsdb 29							//'&outline'
      &add
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 23							//'hotKeyText'
      &pushsdbgm 30							//'field'
      &pushsdb 31							//'autoSize'
      &pushsdb 32							//'left'
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 23							//'hotKeyText'
      &pushsdbgm 30							//'field'
      &pushsdbgm 5							//'_width'
      &pop
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 23							//'hotKeyText'
      &pushsdb 4							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 24							//'titleText'
      &pushsdbgm 30							//'field'
      &pushsdbgm 4							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 24							//'titleText'
      &pushsdbgm 30							//'field'
      &pushsdbgm 5							//'_width'
      &add
      &pushsdbgv 22							//'spacing'
      &add
      &setMember
      &pushsdbgv 34							//'verticalSizer'
      &pushsdb 35							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 19							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 36							//'FSCommand:'
      &pushsdb 37							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 28							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 9							//''
      &getURL2
     label10:
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &constants 'amphibiousIcon', '_x', 'textMC', '_width', 'amphibiousIconMargin'  
      &pushsdbgv 0							//'amphibiousIcon'
      &pushsdb 1							//'_x'
      &pushsdbgv 2							//'textMC'
      &pushsdbgm 1							//'_x'
      &pushsdbgv 2							//'textMC'
      &pushsdbgm 3							//'_width'
      &add
      &pushsdbgv 4							//'amphibiousIconMargin'
      &add
      &setMember
    &end // of frame 10

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 25
  
  &exportAssets
    25 &as 'UnitTitle'
  &end // of exportAssets

  &defineMovieClip 26 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 26

  &defineMovieClip 29 // total frames: 1
  &end // of defineMovieClip 29

  &defineMovieClip 32 // total frames: 1
  &end // of defineMovieClip 32

  &defineMovieClip 35 // total frames: 1
  &end // of defineMovieClip 35

  &defineMovieClip 36 // total frames: 40

    &frame 0
      &constants '_noHotKey', 'textMC', 'gotoAndStop', '', '_width', 'contentStage', '_x', 'categoryIcon', 'background', '_parent', 'UnregisterMasterResizeListener', 'HideHotKeyText', 'SetUnitAbilityCategory', 'Move', 'OnMasterResize', 'onUnload', 'initialized', 'this', 'RegisterMasterResizeListener', 'spacing', 'hotKeyText', 'titleText', 'text', '$', 'Title', 'qualifyName', '&outline', 'field', 'autoSize', 'left', 'HotKey', 'verticalSizer', '_visible', 'FSCommand:', 'OnInitialized'  
      &function HideHotKeyText (      )      
        &pushsdb 0							//'_noHotKey'
        &pushone
        &pushsdbgv 1							//'textMC'
        &dcallmp 2							// gotoAndStop()
      &end // of function HideHotKeyText

      &function2 SetUnitAbilityCategory (r:1='category') ()
        &pushregister 1
        &gotoAndPlay      &end // of function SetUnitAbilityCategory

      &function2 Move (r:6='newX', r:5='newY', r:3='newWidth', r:4='newHeight') ()
        &pushsdb 3							//''
        &pushzero
        &pushregister 6
        &setProperty
        &pushsdb 3							//''
        &pushone
        &pushregister 5
        &setProperty
        &pushregister 3
        &pushsdb 3							//''
        &pushbyte 8
        &getProperty
        &lessThan
        &not
        &jnz label3        
        &pushsdb 3							//''
        &pushbyte 8
        &getProperty
        &pushregister 3
        &subtract
        &setRegister r:1
        &pop
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 4							//'_width'
        &setRegister r:2
        &pop
        &pushsdbgv 1							//'textMC'
        &pushsdb 4							//'_width'
        &pushregister 1
        &pushregister 2
        &pushone
        &subtract
        &lessThan
        &jnz label1        
        &pushone
        &jmp label2        
       label1:
        &pushregister 2
        &pushregister 1
        &subtract
       label2:
        &setMember
        &pushsdb 3							//''
        &pushbyte 8
        &pushregister 3
        &setProperty
       label3:
        &pushregister 4
        &pushsdb 3							//''
        &pushbyte 9
        &getProperty
        &lessThan
        &not
        &jnz label4        
        &pushsdb 3							//''
        &pushbyte 9
        &pushregister 4
        &setProperty
       label4:
      &end // of function Move

      &function2 OnMasterResize (r:2='mc') ()
        &pushregister 2
        &pushsdbgm 5							//'contentStage'
        &pushsdbgm 6							//'_x'
        &pushsdb 3							//''
        &pushzero
        &getProperty
        &add
        &setRegister r:1
        &pop
        &pushsdbgv 7							//'categoryIcon'
        &pushundef
        &equals
        &not
        &not
        &jnz label5        
        &pushsdbgv 7							//'categoryIcon'
        &pushsdb 6							//'_x'
        &pushsdbgv 7							//'categoryIcon'
        &pushsdbgm 6							//'_x'
        &pushregister 1
        &subtract
        &setMember
        &pushsdbgv 1							//'textMC'
        &pushsdb 6							//'_x'
        &pushsdbgv 1							//'textMC'
        &pushsdbgm 6							//'_x'
        &pushregister 1
        &subtract
        &setMember
       label5:
        &pushsdbgv 8							//'background'
        &pushsdb 6							//'_x'
        &pushsdbgv 8							//'background'
        &pushsdbgm 6							//'_x'
        &pushregister 1
        &subtract
        &setMember
        &pushsdbgv 8							//'background'
        &pushsdb 4							//'_width'
        &pushregister 2
        &pushsdbgm 8							//'background'
        &pushsdbgm 4							//'_width'
        &pushbyte 100
        &multiply
        &pushsdb 3							//''
        &pushbyte 2
        &getProperty
        &divide
        &setMember
      &end // of function OnMasterResize

      &function2 onUnload () (r:1='this', r:2='_parent')
        &pushregister 1
        &pushone
        &pushregister 2
        &pushsdbgm 9							//'_parent'
        &dcallmp 10							// UnregisterMasterResizeListener()
        &pushsdb 11							//'HideHotKeyText'
        &delete2
        &pop
        &pushsdb 12							//'SetUnitAbilityCategory'
        &delete2
        &pop
        &pushsdb 13							//'Move'
        &delete2
        &pop
        &pushsdb 14							//'OnMasterResize'
        &delete2
        &pop
        &pushsdb 15							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 16							//'initialized'
      &not
      &not
      &jnz label6      
      &pushthisgv
      &pushone
      &pushsdbgv 9							//'_parent'
      &pushsdbgm 9							//'_parent'
      &dcallmp 18							// RegisterMasterResizeListener()
      &pushsdb 19							//'spacing'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 20							//'hotKeyText'
      &pushsdbgm 6							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 21							//'titleText'
      &pushsdbgm 6							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 21							//'titleText'
      &pushsdbgm 4							//'_width'
      &add
      &subtract
      &varEquals
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 21							//'titleText'
      &pushsdb 22							//'text'
      &pushsdb 23							//'$'
      &pushsdb 24							//'Title'
      &pushthisgv
      &pushbyte 2
      &pushsdb 25							//'qualifyName'
      &callFunction
      &add
      &pushsdb 26							//'&outline'
      &add
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 21							//'titleText'
      &pushsdbgm 27							//'field'
      &pushsdb 28							//'autoSize'
      &pushsdb 29							//'left'
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 21							//'titleText'
      &pushsdbgm 27							//'field'
      &pushsdbgm 4							//'_width'
      &pop
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 20							//'hotKeyText'
      &pushsdb 22							//'text'
      &pushsdb 23							//'$'
      &pushsdb 30							//'HotKey'
      &pushthisgv
      &pushbyte 2
      &pushsdb 25							//'qualifyName'
      &callFunction
      &add
      &pushsdb 26							//'&outline'
      &add
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 20							//'hotKeyText'
      &pushsdbgm 27							//'field'
      &pushsdb 28							//'autoSize'
      &pushsdb 29							//'left'
      &setMember
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 20							//'hotKeyText'
      &pushsdbgm 27							//'field'
      &pushsdbgm 4							//'_width'
      &pop
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 20							//'hotKeyText'
      &pushsdb 6							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 21							//'titleText'
      &pushsdbgm 27							//'field'
      &pushsdbgm 6							//'_x'
      &pushsdbgv 1							//'textMC'
      &pushsdbgm 21							//'titleText'
      &pushsdbgm 27							//'field'
      &pushsdbgm 4							//'_width'
      &add
      &pushsdbgv 19							//'spacing'
      &add
      &setMember
      &pushsdbgv 31							//'verticalSizer'
      &pushsdb 32							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 16							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 33							//'FSCommand:'
      &pushsdb 34							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 25							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 3							//''
      &getURL2
     label6:
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
  &end // of defineMovieClip 36
  
  &exportAssets
    36 &as 'CommandHelpTitle'
  &end // of exportAssets

  &defineMovieClip 38 // total frames: 1
  &end // of defineMovieClip 38

  &defineMovieClip 39 // total frames: 1

    &frame 0
      &constants '', 'Math', 'min', 'initialized', 'text', '$', 'Text', 'this', 'qualifyName', '&outline', 'field', 'autoSize', 'left', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &pushsdbgv 3							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 4							//'text'
      &pushsdb 4							//'text'
      &pushsdb 5							//'$'
      &pushsdb 6							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &add
      &pushsdb 9							//'&outline'
      &add
      &setMember
      &pushsdbgv 4							//'text'
      &pushsdbgm 10							//'field'
      &pushsdb 11							//'autoSize'
      &pushsdb 12							//'left'
      &setMember
      &pushsdbgv 4							//'text'
      &pushsdbgm 10							//'field'
      &pushsdbgm 13							//'_width'
      &pop
      &pushsdb 14							//'FSCommand:'
      &pushsdb 15							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
      &pushsdb 3							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 39
  
  &exportAssets
    39 &as 'HelpUnitTypeDescription'
  &end // of exportAssets

  &defineMovieClip 41 // total frames: 1
  &end // of defineMovieClip 41

  &defineMovieClip 42 // total frames: 1

    &frame 0
      &constants '', 'Math', 'min', 'text', 'field', 'textColor', 'notAcquiredTextColor', 'acquiredTextColor', 'Move', 'SetAcquired', 'onUnload', 'initialized', '$', 'Text', 'this', 'qualifyName', '&outline', 'autoSize', 'left', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function2 SetAcquired (r:2='acquiredArg') ()
        &pushregister 2
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushsdbgv 3							//'text'
        &pushsdbgm 4							//'field'
        &pushsdb 5							//'textColor'
        &pushregister 1
        &jnz label1        
        &pushsdbgv 6							//'notAcquiredTextColor'
        &jmp label2        
       label1:
        &pushsdbgv 7							//'acquiredTextColor'
       label2:
        &setMember
      &end // of function SetAcquired

      &function onUnload (      )      
        &pushsdb 8							//'Move'
        &delete2
        &pop
        &pushsdb 9							//'SetAcquired'
        &delete2
        &pop
        &pushsdb 10							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 11							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdb 6							//'notAcquiredTextColor'
      &pushlong 16776960
      &setVariable
      &pushsdb 7							//'acquiredTextColor'
      &pushlong 65280
      &setVariable
      &pushsdbgv 3							//'text'
      &pushsdb 3							//'text'
      &pushsdb 12							//'$'
      &pushsdb 13							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 15							//'qualifyName'
      &callFunction
      &add
      &pushsdb 16							//'&outline'
      &add
      &setMember
      &pushsdbgv 3							//'text'
      &pushsdbgm 4							//'field'
      &pushsdb 5							//'textColor'
      &pushsdbgv 6							//'notAcquiredTextColor'
      &setMember
      &pushsdbgv 3							//'text'
      &pushsdbgm 4							//'field'
      &pushsdb 17							//'autoSize'
      &pushsdb 18							//'left'
      &setMember
      &pushsdbgv 3							//'text'
      &pushsdbgm 4							//'field'
      &pushsdbgm 19							//'_width'
      &pop
      &pushsdb 20							//'FSCommand:'
      &pushsdb 21							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 15							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
      &pushsdb 11							//'initialized'
      &pushtrue
      &setVariable
     label3:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 42
  
  &exportAssets
    42 &as 'HelpAcquireHint'
  &end // of exportAssets

  &defineMovieClip 44 // total frames: 1
  &end // of defineMovieClip 44

  &defineMovieClip 45 // total frames: 1

    &frame 0
      &constants '', 'Math', 'min', 'initialized', 'text', '$', 'Text', 'this', 'qualifyName', '&outline', 'field', 'autoSize', 'left', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &pushsdbgv 3							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 4							//'text'
      &pushsdb 4							//'text'
      &pushsdb 5							//'$'
      &pushsdb 6							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &add
      &pushsdb 9							//'&outline'
      &add
      &setMember
      &pushsdbgv 4							//'text'
      &pushsdbgm 10							//'field'
      &pushsdb 11							//'autoSize'
      &pushsdb 12							//'left'
      &setMember
      &pushsdbgv 4							//'text'
      &pushsdbgm 10							//'field'
      &pushsdbgm 13							//'_width'
      &pop
      &pushsdb 14							//'FSCommand:'
      &pushsdb 15							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
      &pushsdb 3							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 45
  
  &exportAssets
    45 &as 'HelpInstructions'
  &end // of exportAssets

  &defineMovieClip 47 // total frames: 1
  &end // of defineMovieClip 47

  &defineMovieClip 49 // total frames: 1

    &frame 0
      &pushs 'text'
      &pushssv '$DESC:Amphibious&outline'
    &end // of frame 0
  &end // of defineMovieClip 49

  &defineMovieClip 50 // total frames: 20

    &frame 0
      &constants '', 'Math', 'min', 'NotAmphibious', 'Amphibious', 'Move', 'SetAmphibious', 'onUnload', 'initialized', 'text', '$', 'Text', 'this', 'qualifyName', '&outline', 'field', 'autoSize', 'left', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function2 SetAmphibious (r:1='val') ()
        &pushregister 1
        &pushzero
        &equals
        &not
        &jnz label1        
        &pushsdb 3							//'NotAmphibious'
        &jmp label2        
       label1:
        &pushsdb 4							//'Amphibious'
       label2:
        &gotoAndPlay      &end // of function SetAmphibious

      &function onUnload (      )      
        &pushsdb 5							//'Move'
        &delete2
        &pop
        &pushsdb 6							//'SetAmphibious'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdbgv 9							//'text'
      &pushsdb 9							//'text'
      &pushsdb 10							//'$'
      &pushsdb 11							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 13							//'qualifyName'
      &callFunction
      &add
      &pushsdb 14							//'&outline'
      &add
      &setMember
      &pushsdbgv 9							//'text'
      &pushsdbgm 15							//'field'
      &pushsdb 16							//'autoSize'
      &pushsdb 17							//'left'
      &setMember
      &pushsdbgv 9							//'text'
      &pushsdbgm 15							//'field'
      &pushsdbgm 18							//'_width'
      &pop
      &pushsdb 19							//'FSCommand:'
      &pushsdb 20							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 13							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
     label3:
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 50
  
  &exportAssets
    50 &as 'UnitDescription'
  &end // of exportAssets

  &defineMovieClip 52 // total frames: 1
  &end // of defineMovieClip 52

  &defineMovieClip 54 // total frames: 1

    &frame 0
      &pushs 'text'
      &pushssv '$UNITABILITYCATEGORY:Instant&outline'
    &end // of frame 0
  &end // of defineMovieClip 54

  &defineMovieClip 56 // total frames: 1

    &frame 0
      &pushs 'text'
      &pushssv '$UNITABILITYCATEGORY:Toggle&outline'
    &end // of frame 0
  &end // of defineMovieClip 56

  &defineMovieClip 58 // total frames: 1

    &frame 0
      &pushs 'text'
      &pushssv '$UNITABILITYCATEGORY:Targeted&outline'
    &end // of frame 0
  &end // of defineMovieClip 58

  &defineMovieClip 59 // total frames: 40

    &frame 0
      &constants '', 'Math', 'min', 'Move', 'SetUnitAbilityCategory', 'onUnload', 'initialized', 'text', '$', 'Text', 'this', 'qualifyName', '&outline', 'field', 'autoSize', 'left', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function2 SetUnitAbilityCategory (r:1='category') ()
        &pushregister 1
        &gotoAndPlay      &end // of function SetUnitAbilityCategory

      &function onUnload (      )      
        &pushsdb 3							//'Move'
        &delete2
        &pop
        &pushsdb 4							//'SetUnitAbilityCategory'
        &delete2
        &pop
        &pushsdb 5							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 6							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 7							//'text'
      &pushsdb 7							//'text'
      &pushsdb 8							//'$'
      &pushsdb 9							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 11							//'qualifyName'
      &callFunction
      &add
      &pushsdb 12							//'&outline'
      &add
      &setMember
      &pushsdbgv 7							//'text'
      &pushsdbgm 13							//'field'
      &pushsdb 14							//'autoSize'
      &pushsdb 15							//'left'
      &setMember
      &pushsdbgv 7							//'text'
      &pushsdbgm 13							//'field'
      &pushsdbgm 16							//'_width'
      &pop
      &pushsdb 17							//'FSCommand:'
      &pushsdb 18							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 11							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
      &pushsdb 6							//'initialized'
      &pushtrue
      &setVariable
     label1:
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
  &end // of defineMovieClip 59
  
  &exportAssets
    59 &as 'CommandHelpDescription'
  &end // of exportAssets

  &defineMovieClip 61 // total frames: 1
  &end // of defineMovieClip 61

  &defineMovieClip 64 // total frames: 1
  &end // of defineMovieClip 64

  &defineMovieClip 67 // total frames: 1
  &end // of defineMovieClip 67

  &defineMovieClip 68 // total frames: 30

    &frame 28
      &gotoFrame 0
      &play
    &end // of frame 28
  &end // of defineMovieClip 68

  &defineMovieClip 69 // total frames: 10

    &frame 0
      &constants 'textClip', 'field', 'textColor', '_cantAfford', '_canAfford', '', 'Math', 'min', 'SetAffordable', 'Move', 'onUnload', 'initialized', 'rightMargin', 'background', '_x', '_width', 'text', '$', 'Text', 'this', 'qualifyName', '&outline', 'autoSize', 'left', 'FSCommand:', 'OnInitialized'  
      &function2 SetAffordable (r:2='valArg') ()
        &pushregister 2
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushsdbgv 0							//'textClip'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &jnz label1        
        &pushlong 16711680
        &jmp label2        
       label1:
        &pushlong 16777215
       label2:
        &setMember
        &pushregister 1
        &jnz label3        
        &pushsdb 3							//'_cantAfford'
        &jmp label4        
       label3:
        &pushsdb 4							//'_canAfford'
       label4:
        &gotoAndStop      &end // of function SetAffordable

      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 5							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 5							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 5							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 5							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 6							//'Math'
        &pushsdb 7							//'min'
        &callMethod
        &setProperty
        &pushsdb 5							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 5							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 6							//'Math'
        &pushsdb 7							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function onUnload (      )      
        &pushsdb 8							//'SetAffordable'
        &delete2
        &pop
        &pushsdb 9							//'Move'
        &delete2
        &pop
        &pushsdb 10							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 11							//'initialized'
      &not
      &not
      &jnz label5      
      &pushsdb 12							//'rightMargin'
      &pushsdbgv 13							//'background'
      &pushsdbgm 14							//'_x'
      &pushsdbgv 13							//'background'
      &pushsdbgm 15							//'_width'
      &add
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 14							//'_x'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 15							//'_width'
      &add
      &subtract
      &varEquals
      &pushsdbgv 0							//'textClip'
      &pushsdb 16							//'text'
      &pushsdb 17							//'$'
      &pushsdb 18							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 20							//'qualifyName'
      &callFunction
      &add
      &pushsdb 21							//'&outline'
      &add
      &setMember
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 1							//'field'
      &pushsdb 22							//'autoSize'
      &pushsdb 23							//'left'
      &setMember
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 1							//'field'
      &pushsdbgm 15							//'_width'
      &pop
      &pushsdbgv 13							//'background'
      &pushsdb 15							//'_width'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 14							//'_x'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 15							//'_width'
      &add
      &pushsdbgv 12							//'rightMargin'
      &add
      &pushsdbgv 13							//'background'
      &pushsdbgm 14							//'_x'
      &subtract
      &setMember
      &pushsdb 24							//'FSCommand:'
      &pushsdb 25							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 20							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 5							//''
      &getURL2
      &pushsdb 11							//'initialized'
      &pushtrue
      &setVariable
     label5:
      &stop
    &end // of frame 0

    &placeMovieClip 14 &as 'background'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 14
  &end // of defineMovieClip 69
  
  &exportAssets
    69 &as 'HelpEnergy'
  &end // of exportAssets

  &defineMovieClip 71 // total frames: 1
  &end // of defineMovieClip 71

  &defineMovieClip 74 // total frames: 1

    &frame 0
    &end // of frame 0
  &end // of defineMovieClip 74

  &defineMovieClip 75 // total frames: 1

    &frame 0
      &constants 'textClip', 'field', 'textColor', 'contentStage', '_x', '', 'background', 'Math', 'min', '_width', '_parent', 'UnregisterMasterResizeListener', 'SetAffordable', 'Move', 'OnMasterResize', 'onUnload', 'initialized', 'this', 'RegisterMasterResizeListener', 'rightMargin', 'text', '$', 'Text', 'qualifyName', '&outline', 'autoSize', 'left', 'FSCommand:', 'OnInitialized'  
      &function2 SetAffordable (r:2='valArg') ()
        &pushregister 2
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushsdbgv 0							//'textClip'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &jnz label1        
        &pushlong 16711680
        &jmp label2        
       label1:
        &pushlong 16777215
       label2:
        &setMember
      &end // of function SetAffordable

      &function2 OnMasterResize (r:2='mc') ()
        &pushregister 2
        &pushsdbgm 3							//'contentStage'
        &pushsdbgm 4							//'_x'
        &pushsdb 5							//''
        &pushzero
        &getProperty
        &add
        &setRegister r:1
        &pop
        &pushsdbgv 6							//'background'
        &pushsdb 4							//'_x'
        &pushzero
        &pushregister 1
        &subtract
        &setMember
      &end // of function OnMasterResize

      &function2 Move (r:3='xArg', r:4='yArg', r:1='widthArg', r:2='heightArg') ()
        &pushsdb 5							//''
        &pushzero
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 5							//''
        &pushone
        &pushregister 4
        &toNumber
        &setProperty
        &pushsdb 5							//''
        &pushbyte 8
        &pushregister 1
        &toNumber
        &pushsdb 5							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 7							//'Math'
        &pushsdb 8							//'min'
        &callMethod
        &setProperty
        &pushsdb 5							//''
        &pushbyte 9
        &pushregister 2
        &toNumber
        &pushsdb 5							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 7							//'Math'
        &pushsdb 8							//'min'
        &callMethod
        &setProperty
        &pushsdbgv 6							//'background'
        &pushsdb 9							//'_width'
        &pushregister 1
        &toNumber
        &pushbyte 5
        &add
        &setMember
      &end // of function Move

      &function2 onUnload () (r:1='this', r:2='_parent')
        &pushregister 1
        &pushone
        &pushregister 2
        &pushsdbgm 10							//'_parent'
        &dcallmp 11							// UnregisterMasterResizeListener()
        &pushsdb 12							//'SetAffordable'
        &delete2
        &pop
        &pushsdb 13							//'Move'
        &delete2
        &pop
        &pushsdb 14							//'OnMasterResize'
        &delete2
        &pop
        &pushsdb 15							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 16							//'initialized'
      &not
      &not
      &jnz label3      
      &pushthisgv
      &pushone
      &pushsdbgv 10							//'_parent'
      &pushsdbgm 10							//'_parent'
      &dcallmp 18							// RegisterMasterResizeListener()
      &pushsdb 19							//'rightMargin'
      &pushsdbgv 6							//'background'
      &pushsdbgm 4							//'_x'
      &pushsdbgv 6							//'background'
      &pushsdbgm 9							//'_width'
      &add
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 4							//'_x'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 9							//'_width'
      &add
      &subtract
      &varEquals
      &pushsdbgv 0							//'textClip'
      &pushsdb 20							//'text'
      &pushsdb 21							//'$'
      &pushsdb 22							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 23							//'qualifyName'
      &callFunction
      &add
      &pushsdb 24							//'&outline'
      &add
      &setMember
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 1							//'field'
      &pushsdb 25							//'autoSize'
      &pushsdb 26							//'left'
      &setMember
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 1							//'field'
      &pushsdbgm 9							//'_width'
      &pop
      &pushsdbgv 6							//'background'
      &pushsdb 9							//'_width'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 4							//'_x'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 9							//'_width'
      &add
      &pushsdbgv 19							//'rightMargin'
      &add
      &pushsdbgv 6							//'background'
      &pushsdbgm 4							//'_x'
      &subtract
      &setMember
      &pushsdb 27							//'FSCommand:'
      &pushsdb 28							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 23							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 5							//''
      &getURL2
      &pushsdb 16							//'initialized'
      &pushtrue
      &setVariable
     label3:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 75
  
  &exportAssets
    75 &as 'HelpCost'
  &end // of exportAssets

  &defineMovieClip 77 // total frames: 1
  &end // of defineMovieClip 77

  &defineMovieClip 80 // total frames: 1

    &frame 0
    &end // of frame 0
  &end // of defineMovieClip 80

  &defineMovieClip 81 // total frames: 1

    &frame 0
      &constants 'textClip', 'field', 'textColor', 'background', '_width', 'contentStage', '', 'Math', 'min', '_parent', 'UnregisterMasterResizeListener', 'SetAdjusted', 'Move', 'OnMasterResize', 'onUnload', 'initialized', 'this', 'RegisterMasterResizeListener', 'rightMargin', '_x', 'text', '$', 'Text', 'qualifyName', '&outline', 'autoSize', 'left', 'FSCommand:', 'OnInitialized'  
      &function2 SetAdjusted (r:2='valArg') ()
        &pushregister 2
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushsdbgv 0							//'textClip'
        &pushsdbgm 1							//'field'
        &pushsdb 2							//'textColor'
        &pushregister 1
        &jnz label1        
        &pushlong 16777215
        &jmp label2        
       label1:
        &pushlong 16711680
       label2:
        &setMember
      &end // of function SetAdjusted

      &function2 OnMasterResize (r:1='mc') ()
        &pushsdbgv 3							//'background'
        &pushsdb 4							//'_width'
        &pushregister 1
        &pushsdbgm 5							//'contentStage'
        &pushsdbgm 4							//'_width'
        &pushsdb 6							//''
        &pushzero
        &getProperty
        &subtract
        &pushbyte 6
        &subtract
        &setMember
      &end // of function OnMasterResize

      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 6							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 6							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 6							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 6							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 7							//'Math'
        &pushsdb 8							//'min'
        &callMethod
        &setProperty
        &pushsdb 6							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 6							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 7							//'Math'
        &pushsdb 8							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function2 onUnload () (r:1='this', r:2='_parent')
        &pushregister 1
        &pushone
        &pushregister 2
        &pushsdbgm 9							//'_parent'
        &dcallmp 10							// UnregisterMasterResizeListener()
        &pushsdb 11							//'SetAdjusted'
        &delete2
        &pop
        &pushsdb 12							//'Move'
        &delete2
        &pop
        &pushsdb 13							//'OnMasterResize'
        &delete2
        &pop
        &pushsdb 14							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 15							//'initialized'
      &not
      &not
      &jnz label3      
      &pushthisgv
      &pushone
      &pushsdbgv 9							//'_parent'
      &pushsdbgm 9							//'_parent'
      &dcallmp 17							// RegisterMasterResizeListener()
      &pushsdb 18							//'rightMargin'
      &pushsdbgv 3							//'background'
      &pushsdbgm 19							//'_x'
      &pushsdbgv 3							//'background'
      &pushsdbgm 4							//'_width'
      &add
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 19							//'_x'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 4							//'_width'
      &add
      &subtract
      &varEquals
      &pushsdbgv 0							//'textClip'
      &pushsdb 20							//'text'
      &pushsdb 21							//'$'
      &pushsdb 22							//'Text'
      &pushthisgv
      &pushbyte 2
      &pushsdb 23							//'qualifyName'
      &callFunction
      &add
      &pushsdb 24							//'&outline'
      &add
      &setMember
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 1							//'field'
      &pushsdb 25							//'autoSize'
      &pushsdb 26							//'left'
      &setMember
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 1							//'field'
      &pushsdbgm 4							//'_width'
      &pop
      &pushsdbgv 3							//'background'
      &pushsdb 4							//'_width'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 19							//'_x'
      &pushsdbgv 0							//'textClip'
      &pushsdbgm 4							//'_width'
      &add
      &pushsdbgv 18							//'rightMargin'
      &add
      &pushsdbgv 3							//'background'
      &pushsdbgm 19							//'_x'
      &subtract
      &setMember
      &pushsdb 27							//'FSCommand:'
      &pushsdb 28							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 23							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 6							//''
      &getURL2
      &pushsdb 15							//'initialized'
      &pushtrue
      &setVariable
     label3:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 81
  
  &exportAssets
    81 &as 'HelpBuildTime'
  &end // of exportAssets

  &defineMovieClip 82 // total frames: 1
  &end // of defineMovieClip 82

  &defineMovieClip 84 // total frames: 1

    &frame 0
      &constants 'ChecklistItem', 'items', 'attachMovie', 'text', '$', 'Item', 'qualifyName', '&outline', 'textClip', 'autoSize', 'left', 'checkbox', '_height', '_y', 'SetItemCount', '_unchecked', '_checked', 'gotoAndPlay', '', 'Math', 'min', 'SetItemChecked', 'Move', 'onUnload', 'initialized', 'headingText', 'Heading', 'this', 'headingClip', '_width', 'FSCommand:', 'OnInitialized'  
      &function2 SetItemCount (r:8='countArg') (r:1='this')
        &pushregister 8
        &toNumber
        &setRegister r:7
        &pop
        &pushzero
        &setRegister r:6
        &pop
        &pushzero
        &setRegister r:3
        &pop
       label1:
        &pushregister 3
        &pushregister 7
        &lessThan
        &not
        &jnz label4        
        &pushregister 3
        &pushregister 3
        &toString
        &pushsdb 0							//'ChecklistItem'
        &pushbyte 3
        &pushsdbgv 1							//'items'
        &pushsdb 2							//'attachMovie'
        &callMethod
        &setRegister r:2
        &pop
        &pushregister 2
        &pushsdb 3							//'text'
        &pushsdb 4							//'$'
        &pushsdb 5							//'Item'
        &pushregister 3
        &toString
        &add
        &pushregister 1
        &pushbyte 2
        &pushsdb 6							//'qualifyName'
        &callFunction
        &add
        &pushsdb 7							//'&outline'
        &add
        &setMember
        &pushregister 2
        &pushsdbgm 8							//'textClip'
        &pushsdb 9							//'autoSize'
        &pushsdb 10							//'left'
        &setMember
        &pushregister 2
        &pushsdbgm 11							//'checkbox'
        &pushsdbgm 12							//'_height'
        &setRegister r:5
        &pop
        &pushregister 2
        &pushsdbgm 8							//'textClip'
        &pushsdbgm 12							//'_height'
        &setRegister r:4
        &pop
        &pushregister 5
        &pushregister 4
        &greaterThan
        &not
        &jnz label2        
        &pushregister 2
        &pushsdbgm 11							//'checkbox'
        &pushsdb 13							//'_y'
        &pushzero
        &setMember
        &pushregister 2
        &pushsdbgm 8							//'textClip'
        &pushsdb 13							//'_y'
        &pushregister 5
        &pushregister 4
        &subtract
        &pushbyte 2
        &divide
        &setMember
        &jmp label3        
       label2:
        &pushregister 2
        &pushsdbgm 11							//'checkbox'
        &pushsdb 13							//'_y'
        &pushregister 4
        &pushregister 5
        &subtract
        &pushbyte 2
        &divide
        &setMember
        &pushregister 2
        &pushsdbgm 8							//'textClip'
        &pushsdb 13							//'_y'
        &pushzero
        &setMember
       label3:
        &pushregister 2
        &pushsdb 13							//'_y'
        &pushregister 6
        &setMember
        &pushregister 6
        &pushregister 2
        &pushsdbgm 12							//'_height'
        &add
        &setRegister r:6
        &pop
        &pushregister 3
        &increment
        &setRegister r:3
        &pop
        &jmp label1        
       label4:
        &pushsdb 14							//'SetItemCount'
        &delete2
        &pop
      &end // of function SetItemCount

      &function2 SetItemChecked (r:5='indexArg', r:4='checkedArg') ()
        &pushregister 5
        &toNumber
        &setRegister r:2
        &pop
        &pushregister 4
        &toNumber
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushsdbgv 1							//'items'
        &pushregister 2
        &toString
        &getMember
        &setRegister r:3
        &pop
        &pushregister 1
        &jnz label5        
        &pushsdb 15							//'_unchecked'
        &jmp label6        
       label5:
        &pushsdb 16							//'_checked'
       label6:
        &pushone
        &pushregister 3
        &pushsdbgm 11							//'checkbox'
        &dcallmp 17							// gotoAndPlay()
      &end // of function SetItemChecked

      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 18							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 18							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 18							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 18							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 19							//'Math'
        &pushsdb 20							//'min'
        &callMethod
        &setProperty
        &pushsdb 18							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 18							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 19							//'Math'
        &pushsdb 20							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function onUnload (      )      
        &pushsdb 14							//'SetItemCount'
        &delete2
        &pop
        &pushsdb 21							//'SetItemChecked'
        &delete2
        &pop
        &pushsdb 22							//'Move'
        &delete2
        &pop
        &pushsdb 23							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 24							//'initialized'
      &not
      &not
      &jnz label7      
      &pushsdb 25							//'headingText'
      &pushsdb 4							//'$'
      &pushsdb 26							//'Heading'
      &pushthisgv
      &pushbyte 2
      &pushsdb 6							//'qualifyName'
      &callFunction
      &add
      &pushsdb 7							//'&outline'
      &add
      &setVariable
      &pushsdbgv 28							//'headingClip'
      &pushsdb 9							//'autoSize'
      &pushsdb 10							//'left'
      &setMember
      &pushsdbgv 28							//'headingClip'
      &pushsdbgm 29							//'_width'
      &pop
      &pushsdb 24							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 30							//'FSCommand:'
      &pushsdb 31							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 6							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 18							//''
      &getURL2
     label7:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 84
  
  &exportAssets
    84 &as 'HelpPrerequisites'
  &end // of exportAssets

  &defineMovieClip 90 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 90

  &defineMovieClip 91 // total frames: 1

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 14 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 14
  &end // of defineMovieClip 91
  
  &exportAssets
    91 &as 'ChecklistItem'
  &end // of exportAssets

  &defineMovieClip 96 // total frames: 1

    &frame 0
      &constants '', 'Math', 'min', 'Move', 'onUnload', 'initialized', 'labelX', 'productionLabelField', '_x', 'margin', 'productionValueField', '_width', 'rightMargin', 'background', 'productionLabelText', '$', 'ProductionLabel', 'this', 'qualifyName', '&outline', 'autoSize', 'left', 'productionValueText', 'ProductionValue', 'consumptionLabelText', 'ConsumptionLabel', 'consumptionLabelField', 'consumptionValueText', 'ConsumptionValue', 'consumptionValueField', 'labelWidth', 'max', 'valueWidth', 'valueX', '_visible', 'FSCommand:', 'OnInitialized'  
      &function2 Move (r:2='xArg', r:3='yArg', r:4='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 3
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 4
        &toNumber
        &pushsdb 0							//''
        &pushbyte 8
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &pushsdb 0							//''
        &pushbyte 9
        &getProperty
        &pushbyte 2
        &pushsdbgv 1							//'Math'
        &pushsdb 2							//'min'
        &callMethod
        &setProperty
      &end // of function Move

      &function onUnload (      )      
        &pushsdb 3							//'Move'
        &delete2
        &pop
        &pushsdb 4							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 6							//'labelX'
      &pushsdbgv 7							//'productionLabelField'
      &pushsdbgm 8							//'_x'
      &varEquals
      &pushsdb 9							//'margin'
      &pushsdbgv 10							//'productionValueField'
      &pushsdbgm 8							//'_x'
      &pushsdbgv 7							//'productionLabelField'
      &pushsdbgm 8							//'_x'
      &pushsdbgv 7							//'productionLabelField'
      &pushsdbgm 11							//'_width'
      &add
      &subtract
      &varEquals
      &pushsdb 12							//'rightMargin'
      &pushsdbgv 13							//'background'
      &pushsdbgm 11							//'_width'
      &pushsdbgv 10							//'productionValueField'
      &pushsdbgm 8							//'_x'
      &pushsdbgv 10							//'productionValueField'
      &pushsdbgm 11							//'_width'
      &add
      &subtract
      &varEquals
      &pushsdb 14							//'productionLabelText'
      &pushsdb 15							//'$'
      &pushsdb 16							//'ProductionLabel'
      &pushthisgv
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &add
      &pushsdb 19							//'&outline'
      &add
      &setVariable
      &pushsdbgv 7							//'productionLabelField'
      &pushsdb 20							//'autoSize'
      &pushsdb 21							//'left'
      &setMember
      &pushsdbgv 7							//'productionLabelField'
      &pushsdbgm 11							//'_width'
      &pop
      &pushsdb 22							//'productionValueText'
      &pushsdb 15							//'$'
      &pushsdb 23							//'ProductionValue'
      &pushthisgv
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &add
      &pushsdb 19							//'&outline'
      &add
      &setVariable
      &pushsdbgv 10							//'productionValueField'
      &pushsdb 20							//'autoSize'
      &pushsdb 21							//'left'
      &setMember
      &pushsdbgv 10							//'productionValueField'
      &pushsdbgm 11							//'_width'
      &pop
      &pushsdb 24							//'consumptionLabelText'
      &pushsdb 15							//'$'
      &pushsdb 25							//'ConsumptionLabel'
      &pushthisgv
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &add
      &pushsdb 19							//'&outline'
      &add
      &setVariable
      &pushsdbgv 26							//'consumptionLabelField'
      &pushsdb 20							//'autoSize'
      &pushsdb 21							//'left'
      &setMember
      &pushsdbgv 26							//'consumptionLabelField'
      &pushsdbgm 11							//'_width'
      &pop
      &pushsdb 27							//'consumptionValueText'
      &pushsdb 15							//'$'
      &pushsdb 28							//'ConsumptionValue'
      &pushthisgv
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &add
      &pushsdb 19							//'&outline'
      &add
      &setVariable
      &pushsdbgv 29							//'consumptionValueField'
      &pushsdb 20							//'autoSize'
      &pushsdb 21							//'left'
      &setMember
      &pushsdbgv 29							//'consumptionValueField'
      &pushsdbgm 11							//'_width'
      &pop
      &pushsdb 30							//'labelWidth'
      &pushsdbgv 26							//'consumptionLabelField'
      &pushsdbgm 11							//'_width'
      &pushsdbgv 7							//'productionLabelField'
      &pushsdbgm 11							//'_width'
      &pushbyte 2
      &pushsdbgv 1							//'Math'
      &pushsdb 31							//'max'
      &callMethod
      &varEquals
      &pushsdbgv 7							//'productionLabelField'
      &pushsdb 8							//'_x'
      &pushsdbgv 6							//'labelX'
      &pushsdbgv 30							//'labelWidth'
      &pushsdbgv 7							//'productionLabelField'
      &pushsdbgm 11							//'_width'
      &subtract
      &add
      &setMember
      &pushsdbgv 26							//'consumptionLabelField'
      &pushsdb 8							//'_x'
      &pushsdbgv 6							//'labelX'
      &pushsdbgv 30							//'labelWidth'
      &pushsdbgv 26							//'consumptionLabelField'
      &pushsdbgm 11							//'_width'
      &subtract
      &add
      &setMember
      &pushsdb 32							//'valueWidth'
      &pushsdbgv 29							//'consumptionValueField'
      &pushsdbgm 11							//'_width'
      &pushsdbgv 10							//'productionValueField'
      &pushsdbgm 11							//'_width'
      &pushbyte 2
      &pushsdbgv 1							//'Math'
      &pushsdb 31							//'max'
      &callMethod
      &varEquals
      &pushsdb 33							//'valueX'
      &pushsdbgv 6							//'labelX'
      &pushsdbgv 30							//'labelWidth'
      &add
      &pushsdbgv 9							//'margin'
      &add
      &varEquals
      &pushsdbgv 10							//'productionValueField'
      &pushsdb 8							//'_x'
      &pushsdbgv 33							//'valueX'
      &pushsdbgv 32							//'valueWidth'
      &pushsdbgv 10							//'productionValueField'
      &pushsdbgm 11							//'_width'
      &subtract
      &add
      &setMember
      &pushsdbgv 29							//'consumptionValueField'
      &pushsdb 8							//'_x'
      &pushsdbgv 33							//'valueX'
      &pushsdbgv 32							//'valueWidth'
      &pushsdbgv 29							//'consumptionValueField'
      &pushsdbgm 11							//'_width'
      &subtract
      &add
      &setMember
      &pushsdbgv 13							//'background'
      &pushsdb 11							//'_width'
      &pushsdbgv 33							//'valueX'
      &pushsdbgv 32							//'valueWidth'
      &add
      &pushsdbgv 12							//'rightMargin'
      &add
      &setMember
      &pushsdbgv 13							//'background'
      &pushsdb 34							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 35							//'FSCommand:'
      &pushsdb 36							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 96
  
  &exportAssets
    96 &as 'HelpEnergyMeter'
  &end // of exportAssets

  &defineMovieClip 98 // total frames: 1
  &end // of defineMovieClip 98

  &defineMovieClip 99 // total frames: 1

    &frame 0
      &constants '', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 Resize (r:2='widthArg', r:1='heightArg') ()
        &pushsdb 0							//''
        &pushbyte 8
        &pushregister 2
        &toNumber
        &setProperty
        &pushsdb 0							//''
        &pushbyte 9
        &pushregister 1
        &toNumber
        &setProperty
      &end // of function Resize

      &pushsdbgv 1							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 2							//'FSCommand:'
      &pushsdb 3							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 5							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
      &pushsdb 1							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0

    &placeMovieClip 98 
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv ''
      &end
    
      &onClipEvent &load
        &pushs '_type'
        &pushs 'Render'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callfsv
      &end
    &end // of placeMovieClip 98
  &end // of defineMovieClip 99
  
  &exportAssets
    99 &as 'ClientRenderedHelp'
  &end // of exportAssets

  &frame 0
    &constants 'box', 'contentStage', 'Pointer', 'encode', 'siteVerticalAlignment', 'top', '_y', 'bottom', 'background', '_height', 'siteHorizontalAlignment', 'left', '_x', 'right', '_width', 'site', 'decode', '_visible', 'contentLeftMargin', 'contentRightMargin', 'contentTopMargin', 'contentBottomMargin', 'frameTop', 'frameBottom', 'frameRight', 'frameLeft', 'curContentWidth', 'curContentHeight', 'GetHelpBoxVerticalAlignment', 'GetHelpBoxHorizontalAlignment', 'OnMasterResize', 'onEnterFrame', 'MoveToSite', 'FSCommand:', 'OnOpen', 'qualifyName', '', 'ancestor', '_xscale', '_yscale', '_parent', 'x', 'y', 'GetHelpBoxPos', 'GlobalToLocal', 'AlignVertically', 'AlignHorizontally', 'displayLeft', 'displayTop', 'displayWidth', 'displayHeight', 'OnClosed', 'GetContentStage', 'Open', 'Close', 'Hide', 'onUnload', 'initialized', 'contentSizer', 'OnInitialized', 'this'  
    &function GetContentStage (    )    
      &pushsdbgv 0							//'box'
      &pushsdbgm 1							//'contentStage'
      &pushone
      &pushsdbgv 2							//'Pointer'
      &pushsdb 3							//'encode'
      &callMethod
      &return
    &end // of function GetContentStage

    &function2 AlignVertically (r:1='siteY') ()
      &pushsdbgv 4							//'siteVerticalAlignment'
      &pushsdb 5							//'top'
      &equals
      &not
      &jnz label1      
      &pushsdbgv 0							//'box'
      &pushsdb 6							//'_y'
      &pushregister 1
      &setMember
      &jmp label3      
     label1:
      &pushsdbgv 4							//'siteVerticalAlignment'
      &pushsdb 7							//'bottom'
      &equals
      &not
      &jnz label2      
      &pushsdbgv 0							//'box'
      &pushsdb 6							//'_y'
      &pushregister 1
      &pushsdbgv 0							//'box'
      &pushsdbgm 8							//'background'
      &pushsdbgm 9							//'_height'
      &subtract
      &setMember
      &jmp label3      
     label2:
      &pushsdbgv 0							//'box'
      &pushsdb 6							//'_y'
      &pushregister 1
      &pushsdbgv 0							//'box'
      &pushsdbgm 8							//'background'
      &pushsdbgm 9							//'_height'
      &pushbyte 2
      &divide
      &subtract
      &setMember
     label3:
    &end // of function AlignVertically

    &function2 AlignHorizontally (r:1='siteX') ()
      &pushsdbgv 10							//'siteHorizontalAlignment'
      &pushsdb 11							//'left'
      &equals
      &not
      &jnz label4      
      &pushsdbgv 0							//'box'
      &pushsdb 12							//'_x'
      &pushregister 1
      &setMember
      &jmp label6      
     label4:
      &pushsdbgv 10							//'siteHorizontalAlignment'
      &pushsdb 13							//'right'
      &equals
      &not
      &jnz label5      
      &pushsdbgv 0							//'box'
      &pushsdb 12							//'_x'
      &pushregister 1
      &pushsdbgv 0							//'box'
      &pushsdbgm 8							//'background'
      &pushsdbgm 14							//'_width'
      &subtract
      &setMember
      &jmp label6      
     label5:
      &pushsdbgv 0							//'box'
      &pushsdb 12							//'_x'
      &pushregister 1
      &pushsdbgv 0							//'box'
      &pushsdbgm 8							//'background'
      &pushsdbgm 14							//'_width'
      &pushbyte 2
      &divide
      &subtract
      &setMember
     label6:
    &end // of function AlignHorizontally

    &function2 Open (r:10='siteRef', r:7='newContentWidth', r:6='newContentHeight') (r:1='this')
      &pushsdb 15							//'site'
      &pushregister 10
      &pushone
      &pushsdbgv 2							//'Pointer'
      &dcallmsv 16							// decode()
      &pushregister 7
      &toNumber
      &setRegister r:7
      &pop
      &pushregister 6
      &toNumber
      &setRegister r:6
      &pop
      &pushsdbgv 0							//'box'
      &pushsdb 17							//'_visible'
      &pushfalse
      &setMember
      &pushregister 7
      &pushsdbgv 18							//'contentLeftMargin'
      &add
      &pushsdbgv 19							//'contentRightMargin'
      &add
      &setRegister r:3
      &pop
      &pushregister 6
      &pushsdbgv 20							//'contentTopMargin'
      &add
      &pushsdbgv 21							//'contentBottomMargin'
      &add
      &setRegister r:2
      &pop
      &pushregister 3
      &setRegister r:4
      &pop
      &pushsdbgv 0							//'box'
      &pushsdbgm 22							//'frameTop'
      &pushsdb 14							//'_width'
      &pushregister 4
      &setMember
      &pushsdbgv 0							//'box'
      &pushsdbgm 23							//'frameBottom'
      &pushsdb 14							//'_width'
      &pushregister 4
      &setMember
      &pushregister 3
      &pushsdbgv 0							//'box'
      &pushsdbgm 24							//'frameRight'
      &pushsdbgm 14							//'_width'
      &subtract
      &setRegister r:8
      &pop
      &pushsdbgv 0							//'box'
      &pushsdbgm 24							//'frameRight'
      &pushsdb 12							//'_x'
      &pushregister 8
      &setMember
      &pushregister 2
      &setRegister r:5
      &pop
      &pushsdbgv 0							//'box'
      &pushsdbgm 25							//'frameLeft'
      &pushsdb 9							//'_height'
      &pushregister 5
      &setMember
      &pushsdbgv 0							//'box'
      &pushsdbgm 24							//'frameRight'
      &pushsdb 9							//'_height'
      &pushregister 5
      &setMember
      &pushregister 2
      &pushsdbgv 0							//'box'
      &pushsdbgm 23							//'frameBottom'
      &pushsdbgm 9							//'_height'
      &subtract
      &setRegister r:9
      &pop
      &pushsdbgv 0							//'box'
      &pushsdbgm 23							//'frameBottom'
      &pushsdb 6							//'_y'
      &pushregister 9
      &setMember
      &pushsdbgv 0							//'box'
      &pushsdbgm 8							//'background'
      &pushsdb 14							//'_width'
      &pushregister 3
      &setMember
      &pushsdbgv 0							//'box'
      &pushsdbgm 8							//'background'
      &pushsdb 9							//'_height'
      &pushregister 2
      &setMember
      &pushsdbgv 0							//'box'
      &pushsdbgm 1							//'contentStage'
      &pushsdb 12							//'_x'
      &pushsdbgv 18							//'contentLeftMargin'
      &setMember
      &pushsdbgv 0							//'box'
      &pushsdbgm 1							//'contentStage'
      &pushsdb 6							//'_y'
      &pushsdbgv 20							//'contentTopMargin'
      &setMember
      &pushsdb 26							//'curContentWidth'
      &pushregister 7
      &setVariable
      &pushsdb 27							//'curContentHeight'
      &pushregister 6
      &setVariable
      &pushsdb 4							//'siteVerticalAlignment'
      &pushzero
      &pushsdbgv 15							//'site'
      &dcallmsv 28							// GetHelpBoxVerticalAlignment()
      &pushsdb 10							//'siteHorizontalAlignment'
      &pushzero
      &pushsdbgv 15							//'site'
      &dcallmsv 29							// GetHelpBoxHorizontalAlignment()
      &pushsdbgv 0							//'box'
      &pushone
      &pushsdbgv 0							//'box'
      &dcallmp 30							// OnMasterResize()
      &pushsdb 31							//'onEnterFrame'
      &function2  () (r:1='this')
        &pushzero
        &dcallfp 32							// MoveToSite()
        &pushsdbgv 0							//'box'
        &pushsdb 17							//'_visible'
        &pushtrue
        &setMember
        &pushsdb 33							//'FSCommand:'
        &pushsdb 34							//'OnOpen'
        &pushregister 1
        &pushbyte 2
        &pushsdb 35							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 36							//''
        &getURL2
        &pushsdb 31							//'onEnterFrame'
        &pushsdbgv 32							//'MoveToSite'
        &setVariable
      &end // of function 

      &setVariable
    &end // of function Open

    &function2 GlobalToLocal (r:10='pos') (r:1='this', r:2='_root')
      &pushregister 2
      &setRegister r:9
      &pop
      &pushzero
      &setRegister r:6
      &pop
      &pushzero
      &setRegister r:5
      &pop
      &pushone
      &setRegister r:8
      &pop
      &pushone
      &setRegister r:7
      &pop
      &pushsdb 37							//'ancestor'
      &pushregister 1
      &setVariable
     label7:
      &pushsdbgv 37							//'ancestor'
      &pushregister 9
      &equals
      &not
      &not
      &jnz label8      
      &pushsdbgv 37							//'ancestor'
      &pushsdbgm 38							//'_xscale'
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:4
      &pop
      &pushregister 6
      &pushregister 4
      &multiply
      &setRegister r:6
      &pop
      &pushregister 6
      &pushsdbgv 37							//'ancestor'
      &pushsdbgm 12							//'_x'
      &subtract
      &setRegister r:6
      &pop
      &pushregister 8
      &pushregister 4
      &divide
      &setRegister r:8
      &pop
      &pushsdbgv 37							//'ancestor'
      &pushsdbgm 39							//'_yscale'
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:3
      &pop
      &pushregister 5
      &pushregister 3
      &multiply
      &setRegister r:5
      &pop
      &pushregister 5
      &pushsdbgv 37							//'ancestor'
      &pushsdbgm 6							//'_y'
      &subtract
      &setRegister r:5
      &pop
      &pushregister 7
      &pushregister 3
      &divide
      &setRegister r:7
      &pop
      &pushsdb 37							//'ancestor'
      &pushsdbgv 37							//'ancestor'
      &pushsdbgm 40							//'_parent'
      &setVariable
      &jmp label7      
     label8:
      &pushregister 10
      &pushsdb 41							//'x'
      &pushregister 10
      &pushsdbgm 41							//'x'
      &pushregister 6
      &add
      &pushregister 8
      &multiply
      &setMember
      &pushregister 10
      &pushsdb 42							//'y'
      &pushregister 10
      &pushsdbgm 42							//'y'
      &pushregister 5
      &add
      &pushregister 7
      &multiply
      &setMember
    &end // of function GlobalToLocal

    &function2 MoveToSite () (r:1='_root')
      &pushsdbgv 15							//'site'
      &pushundef
      &equals
      &not
      &jnz label9      
      &pushundef
      &return
     label9:
      &pushzero
      &pushsdbgv 15							//'site'
      &pushsdb 43							//'GetHelpBoxPos'
      &callMethod
      &setRegister r:3
      &pop
      &pushregister 3
      &pushone
      &dcallfp 44							// GlobalToLocal()
      &pushzero
      &pushsdbgv 15							//'site'
      &pushsdb 28							//'GetHelpBoxVerticalAlignment'
      &callMethod
      &setRegister r:5
      &pop
      &pushzero
      &pushsdbgv 15							//'site'
      &pushsdb 29							//'GetHelpBoxHorizontalAlignment'
      &callMethod
      &setRegister r:8
      &pop
      &pushregister 5
      &pushsdbgv 4							//'siteVerticalAlignment'
      &equals
      &not
      &dup
      &jnz label10      
      &pop
      &pushregister 8
      &pushsdbgv 10							//'siteHorizontalAlignment'
      &equals
      &not
     label10:
      &not
      &jnz label11      
      &pushsdb 4							//'siteVerticalAlignment'
      &pushregister 5
      &setVariable
      &pushsdb 10							//'siteHorizontalAlignment'
      &pushregister 8
      &setVariable
     label11:
      &pushregister 3
      &pushsdbgm 42							//'y'
      &pushone
      &dcallfp 45							// AlignVertically()
      &pushregister 3
      &pushsdbgm 41							//'x'
      &pushone
      &dcallfp 46							// AlignHorizontally()
      &pushregister 1
      &pushsdbgm 47							//'displayLeft'
      &pushundef
      &equals
      &not
      &jnz label12      
      &pushundef
      &return
     label12:
      &pushregister 1
      &pushsdbgm 47							//'displayLeft'
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdbgm 48							//'displayTop'
      &setRegister r:4
      &pop
      &pushregister 2
      &pushregister 1
      &pushsdbgm 49							//'displayWidth'
      &add
      &setRegister r:6
      &pop
      &pushregister 4
      &pushregister 1
      &pushsdbgm 50							//'displayHeight'
      &add
      &setRegister r:7
      &pop
      &pushsdbgv 0							//'box'
      &pushsdbgm 12							//'_x'
      &pushregister 2
      &lessThan
      &not
      &jnz label13      
      &pushsdbgv 0							//'box'
      &pushsdb 12							//'_x'
      &pushregister 2
      &setMember
     label13:
      &pushsdbgv 0							//'box'
      &pushsdbgm 12							//'_x'
      &pushsdbgv 0							//'box'
      &pushsdbgm 14							//'_width'
      &add
      &pushregister 6
      &greaterThan
      &not
      &jnz label14      
      &pushsdbgv 0							//'box'
      &pushsdb 12							//'_x'
      &pushregister 6
      &pushsdbgv 0							//'box'
      &pushsdbgm 14							//'_width'
      &subtract
      &setMember
     label14:
      &pushsdbgv 0							//'box'
      &pushsdbgm 6							//'_y'
      &pushregister 4
      &lessThan
      &not
      &jnz label15      
      &pushsdbgv 0							//'box'
      &pushsdb 6							//'_y'
      &pushregister 4
      &setMember
     label15:
      &pushsdbgv 0							//'box'
      &pushsdbgm 6							//'_y'
      &pushsdbgv 0							//'box'
      &pushsdbgm 9							//'_height'
      &add
      &pushregister 7
      &greaterThan
      &not
      &jnz label16      
      &pushsdbgv 0							//'box'
      &pushsdb 6							//'_y'
      &pushregister 7
      &pushsdbgv 0							//'box'
      &pushsdbgm 9							//'_height'
      &subtract
      &setMember
     label16:
    &end // of function MoveToSite

    &function2 Close () (r:1='this')
      &pushsdb 31							//'onEnterFrame'
      &delete2
      &pop
      &pushsdbgv 0							//'box'
      &pushsdb 17							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 33							//'FSCommand:'
      &pushsdb 51							//'OnClosed'
      &pushregister 1
      &pushbyte 2
      &pushsdb 35							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 36							//''
      &getURL2
    &end // of function Close

    &function Hide (    )    
      &pushsdbgv 0							//'box'
      &pushsdb 17							//'_visible'
      &pushfalse
      &setMember
    &end // of function Hide

    &function onUnload (    )    
      &pushsdb 52							//'GetContentStage'
      &delete2
      &pop
      &pushsdb 53							//'Open'
      &delete2
      &pop
      &pushsdb 31							//'onEnterFrame'
      &delete2
      &pop
      &pushsdb 45							//'AlignVertically'
      &delete2
      &pop
      &pushsdb 46							//'AlignHorizontally'
      &delete2
      &pop
      &pushsdb 44							//'GlobalToLocal'
      &delete2
      &pop
      &pushsdb 32							//'MoveToSite'
      &delete2
      &pop
      &pushsdb 54							//'Close'
      &delete2
      &pop
      &pushsdb 55							//'Hide'
      &delete2
      &pop
      &pushsdb 56							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 57							//'initialized'
    &not
    &not
    &jnz label17    
    &pushsdb 26							//'curContentWidth'
    &pushsdbgv 0							//'box'
    &pushsdbgm 58							//'contentSizer'
    &pushsdbgm 14							//'_width'
    &setVariable
    &pushsdb 27							//'curContentHeight'
    &pushsdbgv 0							//'box'
    &pushsdbgm 58							//'contentSizer'
    &pushsdbgm 9							//'_height'
    &setVariable
    &pushsdb 18							//'contentLeftMargin'
    &pushsdbgv 0							//'box'
    &pushsdbgm 58							//'contentSizer'
    &pushsdbgm 12							//'_x'
    &setVariable
    &pushsdb 19							//'contentRightMargin'
    &pushsdbgv 0							//'box'
    &pushsdbgm 14							//'_width'
    &pushsdbgv 0							//'box'
    &pushsdbgm 58							//'contentSizer'
    &pushsdbgm 12							//'_x'
    &pushsdbgv 26							//'curContentWidth'
    &add
    &subtract
    &setVariable
    &pushsdb 20							//'contentTopMargin'
    &pushsdbgv 0							//'box'
    &pushsdbgm 1							//'contentStage'
    &pushsdbgm 6							//'_y'
    &setVariable
    &pushsdb 21							//'contentBottomMargin'
    &pushsdbgv 0							//'box'
    &pushsdbgm 9							//'_height'
    &pushsdbgv 0							//'box'
    &pushsdbgm 58							//'contentSizer'
    &pushsdbgm 6							//'_y'
    &pushsdbgv 27							//'curContentHeight'
    &add
    &subtract
    &setVariable
    &pushsdb 57							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 33							//'FSCommand:'
    &pushsdb 59							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 35							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 36							//''
    &getURL2
   label17:
    &stop
  &end // of frame 0
  
  &exportAssets
    99 &as 'ClientRenderedHelp'
  &end // of exportAssets

  &defineMovieClip 100 // total frames: 2

    &frame 0
      &stop
    &end // of frame 0
  &end // of defineMovieClip 100

  &defineMovieClip 101 // total frames: 1

    &frame 0
      &constants 'contentClip', 'attachMovie', 'Pointer', 'encode', 'removeMovieClip', 'AttachContent', 'DetachContent', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 AttachContent (r:1='id', r:2='contentType') ()
        &pushregister 1
        &toNumber
        &setRegister r:1
        &pop
        &pushsdb 0							//'contentClip'
        &pushregister 1
        &pushregister 1
        &toString
        &pushregister 2
        &pushbyte 3
        &dcallfsv 1							// attachMovie()
        &pushsdbgv 0							//'contentClip'
        &pushone
        &pushsdbgv 2							//'Pointer'
        &pushsdb 3							//'encode'
        &callMethod
        &return
      &end // of function AttachContent

      &function2 DetachContent (r:3='id') (r:1='this')
        &pushregister 3
        &toNumber
        &setRegister r:3
        &pop
        &pushregister 1
        &pushregister 3
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushregister 2
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushzero
        &pushregister 2
        &dcallmp 4							// removeMovieClip()
       label1:
      &end // of function DetachContent

      &function onUnload (      )      
        &pushsdb 5							//'AttachContent'
        &delete2
        &pop
        &pushsdb 6							//'DetachContent'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 9							//'FSCommand:'
      &pushsdb 10							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 13							//''
      &getURL2
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
     label2:
    &end // of frame 0

    &placeMovieClip 14 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 14
  &end // of defineMovieClip 101

  &defineMovieClip 104 // total frames: 1

    &frame 0
    &end // of frame 0
  &end // of defineMovieClip 104

  &defineMovieClip 106 // total frames: 1

    &frame 0
    &end // of frame 0
  &end // of defineMovieClip 106

  &defineMovieClip 107 // total frames: 1

    &frame 0
      &constants 'masterResizeListeners', 'push', 'length', 'splice', 'OnMasterResize', 'RegisterMasterResizeListener', 'UnregisterMasterResizeListener', 'onUnload', 'initialized', 'Array'  
      &function2 RegisterMasterResizeListener (r:1='mc') ()
        &pushregister 1
        &pushone
        &pushsdbgv 0							//'masterResizeListeners'
        &dcallmp 1							// push()
      &end // of function RegisterMasterResizeListener

      &function2 UnregisterMasterResizeListener (r:2='mc') ()
        &pushzero
        &setRegister r:1
        &pop
       label1:
        &pushregister 1
        &pushsdbgv 0							//'masterResizeListeners'
        &pushsdbgm 2							//'length'
        &lessThan
        &not
        &jnz label3        
        &pushsdbgv 0							//'masterResizeListeners'
        &pushregister 1
        &getMember
        &pushregister 2
        &equals
        &not
        &jnz label2        
        &pushone
        &pushregister 1
        &pushbyte 2
        &pushsdbgv 0							//'masterResizeListeners'
        &dcallmp 3							// splice()
        &jmp label3        
       label2:
        &pushregister 1
        &increment
        &setRegister r:1
        &pop
        &jmp label1        
       label3:
      &end // of function UnregisterMasterResizeListener

      &function2 OnMasterResize (r:2='mc') ()
        &pushzero
        &setRegister r:1
        &pop
       label4:
        &pushregister 1
        &pushsdbgv 0							//'masterResizeListeners'
        &pushsdbgm 2							//'length'
        &lessThan
        &not
        &jnz label5        
        &pushregister 2
        &pushone
        &pushsdbgv 0							//'masterResizeListeners'
        &pushregister 1
        &getMember
        &dcallmp 4							// OnMasterResize()
        &pushregister 1
        &increment
        &setRegister r:1
        &pop
        &jmp label4        
       label5:
      &end // of function OnMasterResize

      &function onUnload (      )      
        &pushsdb 0							//'masterResizeListeners'
        &delete2
        &pop
        &pushsdb 5							//'RegisterMasterResizeListener'
        &delete2
        &pop
        &pushsdb 6							//'UnregisterMasterResizeListener'
        &delete2
        &pop
        &pushsdb 4							//'OnMasterResize'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label6      
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 0							//'masterResizeListeners'
      &pushzero
      &pushsdb 9							//'Array'
      &new
      &setVariable
     label6:
    &end // of frame 0

    &placeMovieClip 14 &as 'contentSizer'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 14
  &end // of defineMovieClip 107

  &placeMovieClip 107 &as 'box'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 107
&end
