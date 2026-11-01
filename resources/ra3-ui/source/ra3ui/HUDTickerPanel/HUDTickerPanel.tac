movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\HUDTIC~1\\HUDTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 6 // total frames: 1

    &placeMovieClip 5 
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'Text'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 5
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as 'TextContent'
  &end // of exportAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImage' &as 7
  &end // of importAssets

  &defineMovieClip 8 // total frames: 1

    &frame 0
      &constants 'imageRenderer', '_width', '_height', '_y', '_x', 'SetImageSize', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 SetImageSize (r:8='widthArg', r:7='heightArg') ()
        &pushregister 8
        &toNumber
        &setRegister r:1
        &pop
        &pushregister 7
        &toNumber
        &setRegister r:4
        &pop
        &pushsdbgv 0							//'imageRenderer'
        &pushsdbgm 1							//'_width'
        &setRegister r:3
        &pop
        &pushsdbgv 0							//'imageRenderer'
        &pushsdbgm 2							//'_height'
        &setRegister r:2
        &pop
        &pushregister 1
        &pushregister 2
        &multiply
        &pushregister 3
        &pushregister 4
        &multiply
        &greaterThan
        &not
        &jnz label1        
        &pushregister 3
        &pushregister 4
        &multiply
        &pushregister 1
        &divide
        &setRegister r:5
        &pop
        &pushsdbgv 0							//'imageRenderer'
        &pushsdb 3							//'_y'
        &pushsdbgv 0							//'imageRenderer'
        &pushsdbgm 3							//'_y'
        &pushregister 2
        &pushregister 5
        &subtract
        &pushfloat 0.500000000000000
        &multiply
        &add
        &setMember
        &pushsdbgv 0							//'imageRenderer'
        &pushsdb 2							//'_height'
        &pushregister 5
        &setMember
        &jmp label2        
       label1:
        &pushregister 2
        &pushregister 1
        &multiply
        &pushregister 4
        &divide
        &setRegister r:6
        &pop
        &pushsdbgv 0							//'imageRenderer'
        &pushsdb 4							//'_x'
        &pushsdbgv 0							//'imageRenderer'
        &pushsdbgm 4							//'_x'
        &pushregister 3
        &pushregister 6
        &subtract
        &pushfloat 0.500000000000000
        &multiply
        &add
        &setMember
        &pushsdbgv 0							//'imageRenderer'
        &pushsdb 1							//'_width'
        &pushregister 6
        &setMember
       label2:
      &end // of function SetImageSize

      &function onUnload (      )      
        &pushsdb 5							//'SetImageSize'
        &delete2
        &pop
        &pushsdb 6							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 7							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdb 7							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 8							//'FSCommand:'
      &pushsdb 9							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 11							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 12							//''
      &getURL2
     label3:
      &stop
    &end // of frame 0

    &placeMovieClip 7 &as 'imageRenderer'
    
      &onClipEvent &construct
        &constants '_type', 'RenderImage', '_imageMap', '', '_mode'  
        &pushsdb 0							//'_type'
        &pushsdb 1							//'RenderImage'
        &setVariable
        &pushsdb 2							//'_imageMap'
        &pushsdb 3							//''
        &setVariable
        &pushsdb 4							//'_mode'
        &pushsdb 3							//''
        &setVariable
      &end
    
      &onClipEvent &load
        &pushs '_imageMap'
        &pushs 'Image'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callfsv
      &end
    &end // of placeMovieClip 7
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as 'ImageContent'
  &end // of exportAssets

  &frame 0
    &constants '', '_open', 'panel', 'gotoAndPlay', '_close', 'stage', 'Pointer', 'encode', 'Open', 'Close', 'GetContentStage', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', '_visible', 'stop', 'FSCommand:', 'OnInitialized', 'qualifyName', 'extern', 'InGame', 'contentClip', 'content', 'TextContent', 'attachMovie', '_width', 'placeholder', '_height'  
    &function2 SetVisibility (r:2='visArg') ()
      &pushregister 2
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:1
      &pop
      &pushsdb 0							//''
      &pushbyte 7
      &pushregister 1
      &setProperty
    &end // of function SetVisibility

    &function Open (    )    
      &pushsdb 1							//'_open'
      &pushone
      &pushsdbgv 2							//'panel'
      &dcallmp 3							// gotoAndPlay()
    &end // of function Open

    &function Close (    )    
      &pushsdb 4							//'_close'
      &pushone
      &pushsdbgv 2							//'panel'
      &dcallmp 3							// gotoAndPlay()
    &end // of function Close

    &function GetContentStage (    )    
      &pushsdbgv 2							//'panel'
      &pushsdbgm 5							//'stage'
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetContentStage

    &function onUnload (    )    
      &pushsdb 8							//'Open'
      &delete2
      &pop
      &pushsdb 9							//'Close'
      &delete2
      &pop
      &pushsdb 10							//'GetContentStage'
      &delete2
      &pop
      &pushsdb 11							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 12							//'initialized'
    &not
    &not
    &jnz label1    
    &pushthisgv
    &pushone
    &pushsdbgv 14							//'_parent'
    &dcallmp 15							// OnContentLoaded()
    &pushsdbgv 2							//'panel'
    &pushsdbgm 5							//'stage'
    &pushsdb 16							//'_visible'
    &pushfalse
    &setMember
    &pushzero
    &pushsdbgv 2							//'panel'
    &dcallmp 17							// stop()
    &pushsdb 12							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 18							//'FSCommand:'
    &pushsdb 19							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 20							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 0							//''
    &getURL2
    &pushsdbgv 21							//'extern'
    &pushsdbgm 22							//'InGame'
    &not
    &not
    &jnz label1    
    &pushsdb 23							//'contentClip'
    &pushzero
    &pushsdb 24							//'content'
    &pushsdb 25							//'TextContent'
    &pushbyte 3
    &pushsdbgv 2							//'panel'
    &pushsdbgm 5							//'stage'
    &pushsdb 26							//'attachMovie'
    &callMethod
    &varEquals
    &pushsdbgv 23							//'contentClip'
    &pushsdb 27							//'_width'
    &pushsdbgv 2							//'panel'
    &pushsdbgm 5							//'stage'
    &pushsdbgm 28							//'placeholder'
    &pushsdbgm 27							//'_width'
    &setMember
    &pushsdbgv 23							//'contentClip'
    &pushsdb 29							//'_height'
    &pushsdbgv 2							//'panel'
    &pushsdbgm 5							//'stage'
    &pushsdbgm 28							//'placeholder'
    &pushsdbgm 29							//'_height'
    &setMember
    &pushzero
    &dcallfp 8							// Open()
   label1:
    &stop
  &end // of frame 0
  
  &exportAssets
    8 &as 'ImageContent'
  &end // of exportAssets
  
  &exportAssets
    6 &as 'TextContent'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 2

    &frame 0
      &stop
    &end // of frame 0
  &end // of defineMovieClip 9
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'AttachMovieClipStage' &as 10
  &end // of importAssets

  &defineMovieClip 11 // total frames: 47

    &frame 1
      &pushsgv 'stage'
      &pushs '_visible'
      &pushtrue
      &setMember
    &end // of frame 1

    &frame 10
      &stop
      &pushs 'FSCommand:'
      &pushs 'OnOpened'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end // of frame 10

    &frame 46
      &stop
      &pushs 'FSCommand:'
      &pushs 'OnClosed'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end // of frame 46
  &end // of defineMovieClip 11
&end
