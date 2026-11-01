movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_DRA~1\\IG_DRA~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 1 // total frames: 1

    &frame 0
      &constants ',', '_name', 'split', '', 'OnContentLoaded', 'onUnload'  
      &function2 OnContentLoaded (r:3='contentClip') (r:1='_parent')
        &pushsdb 0							//','
        &pushone
        &pushregister 3
        &pushsdbgm 1							//'_name'
        &pushsdb 2							//'split'
        &callMethod
        &setRegister r:2
        &pop
        &pushregister 3
        &pushregister 2
        &pushone
        &getMember
        &toNumber
        &pushsdb 3							//''
        &pushbyte 13
        &getProperty
        &toNumber
        &pushregister 2
        &pushzero
        &getMember
        &toNumber
        &pushbyte 4
        &pushregister 1
        &dcallmp 4							// OnContentLoaded()
      &end // of function OnContentLoaded

      &function onUnload (      )      
        &pushsdb 4							//'OnContentLoaded'
        &delete2
        &pop
        &pushsdb 5							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

    &end // of frame 0
  &end // of defineMovieClip 1
  
  &exportAssets
    1 &as 'LayerClip'
  &end // of exportAssets

  &frame 0
    &constants 'nextContentDepth', ',', 'createEmptyMovieClip', 'loadMovie', 'removeMovieClip', 'FSCommand:', 'OnContentLoaded-', '-', 'qualifyName', 'Pointer', 'encode', 'LoadContent', 'UnloadContent', 'OnContentLoaded', 'onUnload', 'initialized', 'layerCount', 'i', 'layerClip', 'LayerClip', 'this', 'attachMovie', 'OnInitialized', ''  
    &function2 LoadContent (r:8='overlayId', r:6='layerNum', r:7='componentStageId', r:9='contentType') (r:1='this')
      &pushregister 1
      &pushregister 6
      &toString
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgm 0							//'nextContentDepth'
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdb 0							//'nextContentDepth'
      &pushregister 2
      &pushsdbgm 0							//'nextContentDepth'
      &increment
      &setMember
      &pushregister 2
      &pushsdbgm 0							//'nextContentDepth'
      &pushlong 65536
      &lessThan
      &not
      &not
      &jnz label1      
      &pushregister 2
      &pushsdb 0							//'nextContentDepth'
      &pushzero
      &setMember
     label1:
      &pushregister 8
      &toString
      &pushsdb 1							//','
      &add
      &pushregister 7
      &toString
      &add
      &setRegister r:4
      &pop
      &pushregister 3
      &pushregister 4
      &pushbyte 2
      &pushregister 2
      &pushsdb 2							//'createEmptyMovieClip'
      &callMethod
      &setRegister r:5
      &pop
      &pushregister 9
      &pushone
      &pushregister 5
      &dcallmp 3							// loadMovie()
    &end // of function LoadContent

    &function2 UnloadContent (r:6='overlayId', r:4='layerNum', r:5='componentStageId') (r:1='this')
      &pushregister 1
      &pushregister 4
      &toString
      &getMember
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 6
      &toString
      &pushsdb 1							//','
      &add
      &pushregister 5
      &toString
      &add
      &getMember
      &setRegister r:2
      &pop
      &pushzero
      &pushregister 2
      &dcallmp 4							// removeMovieClip()
    &end // of function UnloadContent

    &function2 OnContentLoaded (r:5='overlayId', r:3='layerNum', r:4='componentStageId', r:2='contentClip') (r:1='this')
      &pushsdb 5							//'FSCommand:'
      &pushsdb 6							//'OnContentLoaded-'
      &pushregister 5
      &toString
      &add
      &pushsdb 7							//'-'
      &add
      &pushregister 3
      &toString
      &add
      &pushsdb 7							//'-'
      &add
      &pushregister 4
      &toString
      &add
      &pushregister 1
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &concat
      &pushregister 2
      &pushone
      &pushsdbgv 9							//'Pointer'
      &pushsdb 10							//'encode'
      &callMethod
      &getURL2
    &end // of function OnContentLoaded

    &function onUnload (    )    
      &pushsdb 11							//'LoadContent'
      &delete2
      &pop
      &pushsdb 12							//'UnloadContent'
      &delete2
      &pop
      &pushsdb 13							//'OnContentLoaded'
      &delete2
      &pop
      &pushsdb 14							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 15							//'initialized'
    &not
    &not
    &jnz label4    
    &pushsdb 16							//'layerCount'
    &pushbyte 5
    &setVariable
    &pushsdb 17							//'i'
    &pushzero
    &varEquals
   label2:
    &pushsdbgv 17							//'i'
    &pushsdbgv 16							//'layerCount'
    &lessThan
    &not
    &jnz label3    
    &pushsdb 18							//'layerClip'
    &pushsdbgv 17							//'i'
    &pushsdbgv 17							//'i'
    &toString
    &pushsdb 19							//'LayerClip'
    &pushbyte 3
    &pushthisgv
    &pushsdb 21							//'attachMovie'
    &callMethod
    &varEquals
    &pushsdbgv 18							//'layerClip'
    &pushsdb 0							//'nextContentDepth'
    &pushzero
    &setMember
    &pushsdb 17							//'i'
    &pushsdbgv 17							//'i'
    &increment
    &setVariable
    &jmp label2    
   label3:
    &pushsdb 15							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 5							//'FSCommand:'
    &pushsdb 22							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 8							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 23							//''
    &getURL2
   label4:
    &stop
  &end // of frame 0
  
  &exportAssets
    1 &as 'LayerClip'
  &end // of exportAssets

  &defineMovieClip 2 // total frames: 2

    &frame 0
      &stop
    &end // of frame 0
  &end // of defineMovieClip 2

  &placeMovieClip 2 
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2
&end
