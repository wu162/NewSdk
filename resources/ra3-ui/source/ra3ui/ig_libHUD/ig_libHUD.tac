movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_LIB~1\\IG_LIB~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 1 // total frames: 1
  &end // of defineMovieClip 1
  
  &exportAssets
    1 &as 'Dummy'
  &end // of exportAssets

  &defineMovieClip 2 // total frames: 1

    &frame 0
      &constants 'FSCommand:', 'OnContentLoaded', 'qualifyName', 'Pointer', 'encode', 'content', 'createEmptyMovieClip', 'loadMovie', 'removeMovieClip', 'LoadContent', 'UnloadContent', 'onUnload', 'initialized', 'OnInitialized', 'this', ''  
      &function2 OnContentLoaded (r:2='clip') (r:1='this')
        &pushsdb 0							//'FSCommand:'
        &pushsdb 1							//'OnContentLoaded'
        &pushregister 1
        &pushbyte 2
        &pushsdb 2							//'qualifyName'
        &callFunction
        &concat
        &pushregister 2
        &pushone
        &pushsdbgv 3							//'Pointer'
        &pushsdb 4							//'encode'
        &callMethod
        &getURL2
      &end // of function OnContentLoaded

      &function2 LoadContent (r:2='url') (r:1='this')
        &pushzero
        &pushsdb 5							//'content'
        &pushbyte 2
        &pushregister 1
        &dcallmp 6							// createEmptyMovieClip()
        &pushregister 2
        &pushone
        &pushregister 1
        &pushsdbgm 5							//'content'
        &dcallmp 7							// loadMovie()
      &end // of function LoadContent

      &function UnloadContent (      )      
        &pushzero
        &pushsdbgv 5							//'content'
        &dcallmp 8							// removeMovieClip()
      &end // of function UnloadContent

      &function onUnload (      )      
        &pushsdb 1							//'OnContentLoaded'
        &delete2
        &pop
        &pushsdb 9							//'LoadContent'
        &delete2
        &pop
        &pushsdb 10							//'UnloadContent'
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
      &pushsdb 12							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 0							//'FSCommand:'
      &pushsdb 13							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 2							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 15							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 2
  
  &exportAssets
    2 &as 'LoadMovieStage'
  &end // of exportAssets

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4
  
  &exportAssets
    4 &as 'RenderImageDisabled'
  &end // of exportAssets

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as 'RenderImage'
  &end // of exportAssets

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as 'AttachMovieClipStageContentPlaceholder'
  &end // of exportAssets
  
  &exportAssets
    8 &as 'AttachMovieClipStageContentPlaceholder'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 1

    &frame 0
      &constants 'contentClip', 'content', 'attachMovie', '_x', 'placeholder', '_y', '_width', '_height', 'Pointer', 'encode', 'removeMovieClip', 'AttachContent', 'DetachContent', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 AttachContent (r:1='contentType') ()
        &pushsdb 0							//'contentClip'
        &pushzero
        &pushsdb 1							//'content'
        &pushregister 1
        &pushbyte 3
        &dcallfsv 2							// attachMovie()
        &pushsdbgv 0							//'contentClip'
        &pushsdb 3							//'_x'
        &pushsdbgv 4							//'placeholder'
        &pushsdbgm 3							//'_x'
        &setMember
        &pushsdbgv 0							//'contentClip'
        &pushsdb 5							//'_y'
        &pushsdbgv 4							//'placeholder'
        &pushsdbgm 5							//'_y'
        &setMember
        &pushsdbgv 0							//'contentClip'
        &pushsdb 6							//'_width'
        &pushsdbgv 4							//'placeholder'
        &pushsdbgm 6							//'_width'
        &setMember
        &pushsdbgv 0							//'contentClip'
        &pushsdb 7							//'_height'
        &pushsdbgv 4							//'placeholder'
        &pushsdbgm 7							//'_height'
        &setMember
        &pushsdbgv 0							//'contentClip'
        &pushone
        &pushsdbgv 8							//'Pointer'
        &pushsdb 9							//'encode'
        &callMethod
        &return
      &end // of function AttachContent

      &function DetachContent (      )      
        &pushsdbgv 0							//'contentClip'
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushzero
        &pushsdbgv 0							//'contentClip'
        &dcallmp 10							// removeMovieClip()
       label1:
      &end // of function DetachContent

      &function onUnload (      )      
        &pushsdb 11							//'AttachContent'
        &delete2
        &pop
        &pushsdb 12							//'DetachContent'
        &delete2
        &pop
        &pushsdb 13							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 14							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 15							//'FSCommand:'
      &pushsdb 16							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 18							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 19							//''
      &getURL2
      &pushsdb 14							//'initialized'
      &pushtrue
      &setVariable
     label2:
    &end // of frame 0

    &placeMovieClip 8 &as 'placeholder'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 8
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as 'AttachMovieClipStage'
  &end // of exportAssets

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11
  
  &exportAssets
    11 &as 'BlackRectangle'
  &end // of exportAssets

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'SimpleRenderingSurfaceContentPlaceholder'
  &end // of exportAssets

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15
  
  &exportAssets
    15 &as 'WhiteRectangle'
  &end // of exportAssets
  
  &exportAssets
    15 &as 'WhiteRectangle'
  &end // of exportAssets

  &defineMovieClip 16 // total frames: 1

    &frame 0
      &constants 'FSCommand:', 'OnClipDestroyed', 'qualifyName', '', 'onUnload', 'initialized', 'initContent', '_type', 'Render', 'this', '_x', 'shape', '_y', '_width', '_height', 'contentClip', 'content', 'SimpleRenderingSurfaceContentPlaceholder', 'attachMovie'  
      &function2 onUnload () (r:1='this')
        &pushsdb 0							//'FSCommand:'
        &pushsdb 1							//'OnClipDestroyed'
        &pushregister 1
        &pushbyte 2
        &pushsdb 2							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 3							//''
        &getURL2
        &pushsdb 4							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 6							//'initContent'
      &pushsdb 7							//'_type'
      &pushsdb 8							//'Render'
      &pushthisgv
      &pushbyte 2
      &pushsdb 2							//'qualifyName'
      &callFunction
      &pushsdb 10							//'_x'
      &pushsdbgv 11							//'shape'
      &pushsdbgm 10							//'_x'
      &pushsdb 12							//'_y'
      &pushsdbgv 11							//'shape'
      &pushsdbgm 12							//'_y'
      &pushsdb 13							//'_width'
      &pushsdbgv 11							//'shape'
      &pushsdbgm 13							//'_width'
      &pushsdb 14							//'_height'
      &pushsdbgv 11							//'shape'
      &pushsdbgm 14							//'_height'
      &pushbyte 5
      &initObject
      &varEquals
      &pushsdb 15							//'contentClip'
      &pushsdbgv 6							//'initContent'
      &pushzero
      &pushsdb 16							//'content'
      &pushsdb 17							//'SimpleRenderingSurfaceContentPlaceholder'
      &pushbyte 4
      &pushsdb 18							//'attachMovie'
      &callFunction
      &varEquals
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
     label1:
    &end // of frame 0

    &placeMovieClip 15 &as 'shape'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 15
  &end // of defineMovieClip 16
  
  &exportAssets
    16 &as 'SimpleRenderingSurface'
  &end // of exportAssets

  &defineMovieClip 19 // total frames: 1

    &frame 0
      &constants '', 'Raise', 'Lower', 'onUnload', 'initialized', 'intialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function Raise (      )      
        &pushsdb 0							//''
        &pushbyte 7
        &pushtrue
        &setProperty
      &end // of function Raise

      &function Lower (      )      
        &pushsdb 0							//''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end // of function Lower

      &function onUnload (      )      
        &pushsdb 1							//'Raise'
        &delete2
        &pop
        &pushsdb 2							//'Lower'
        &delete2
        &pop
        &pushsdb 3							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 4							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 0							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 5							//'intialized'
      &pushtrue
      &setVariable
      &pushsdb 6							//'FSCommand:'
      &pushsdb 7							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as 'InputShield'
  &end // of exportAssets
&end
