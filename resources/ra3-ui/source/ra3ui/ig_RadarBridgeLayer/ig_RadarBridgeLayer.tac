movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\ig_RadarBridgeLayer\\ig_RadarBridgeLayer.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 4 // total frames: 1

    &frame 0
      &constants 'MoveIndicator', '', 'ScaleIndicator', 'Move', 'SetOrientation', 'SetScale', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', '_global', 'qualifyName'  
      &function2 Move (r:4='relX', r:3='relY') (r:1='this', r:2='_parent')
        &pushregister 3
        &pushregister 4
        &pushregister 1
        &pushbyte 3
        &pushregister 2
        &dcallmp 0							// MoveIndicator()
      &end // of function Move

      &function2 SetOrientation (r:1='orient') ()
        &pushsdb 1							//''
        &pushbyte 10
        &pushregister 1
        &pushshort 180
        &multiply
        &pushfloat 3.141592741012573
        &divide
        &setProperty
      &end // of function SetOrientation

      &function2 SetScale (r:4='widthScale', r:3='lengthScale') (r:1='this', r:2='_parent')
        &pushregister 3
        &pushregister 4
        &pushregister 1
        &pushbyte 3
        &pushregister 2
        &dcallmp 2							// ScaleIndicator()
      &end // of function SetScale

      &function onUnload (      )      
        &pushsdb 3							//'Move'
        &delete2
        &pop
        &pushsdb 4							//'SetOrientation'
        &delete2
        &pop
        &pushsdb 5							//'SetScale'
        &delete2
        &pop
        &pushsdb 6							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 7							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 7							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 8							//'FSCommand:'
      &pushsdb 9							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 12							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 1							//''
      &getURL2
     label1:
    &end // of frame 0
  &end // of defineMovieClip 4
  
  &exportAssets
    4 &as 'BridgeSegmentIndicator'
  &end // of exportAssets

  &frame 0
    &constants 'sizer', '_width', '_height', 'indicators', 'length', 'relX', 'relY', '_x', '_y', 'iconMC', 'widthScale', 'lengthScale', 'nextIndicatorId', 'BridgeSegmentIndicator', 'attachMovie', 'isEndCap', 'push', 'Pointer', 'encode', 'splice', 'removeMovieClip', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnClipDestroyed', 'Resize', 'CreateIndicator', 'DestroyIndicator', 'MoveIndicator', 'OnBound', 'OnDestroying', 'onUnload', 'this', '_parent', 'OnContentLoaded', 'Array', '_global'  
    &function2 Resize (r:3='newWidth', r:4='newHeight') ()
      &pushsdbgv 0							//'sizer'
      &pushsdb 1							//'_width'
      &pushregister 3
      &setMember
      &pushsdbgv 0							//'sizer'
      &pushsdb 2							//'_height'
      &pushregister 4
      &setMember
      &pushzero
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label5      
      &pushsdbgv 3							//'indicators'
      &pushregister 2
      &getMember
      &setRegister r:1
      &pop
      &pushregister 1
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label2      
      &pop
      &pushregister 1
      &pushsdbgm 5							//'relX'
      &pushundef
      &equals
      &not
     label2:
      &dup
      &not
      &jnz label3      
      &pop
      &pushregister 1
      &pushsdbgm 6							//'relY'
      &pushundef
      &equals
      &not
     label3:
      &not
      &jnz label4      
      &pushregister 1
      &pushsdb 7							//'_x'
      &pushregister 1
      &pushsdbgm 5							//'relX'
      &pushregister 3
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 8							//'_y'
      &pushregister 1
      &pushsdbgm 6							//'relY'
      &pushregister 4
      &multiply
      &setMember
      &pushregister 1
      &pushsdbgm 9							//'iconMC'
      &pushsdb 2							//'_height'
      &pushregister 3
      &pushregister 1
      &pushsdbgm 10							//'widthScale'
      &multiply
      &setMember
      &pushregister 1
      &pushsdbgm 9							//'iconMC'
      &pushsdb 1							//'_width'
      &pushregister 3
      &pushregister 1
      &pushsdbgm 11							//'lengthScale'
      &multiply
      &setMember
     label4:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label1      
     label5:
    &end // of function Resize

    &function2 CreateIndicator (r:3='endCapArg') ()
      &pushsdbgv 12							//'nextIndicatorId'
      &pushsdb 12							//'nextIndicatorId'
      &pushsdbgv 12							//'nextIndicatorId'
      &increment
      &setVariable
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 2
      &toString
      &pushsdb 13							//'BridgeSegmentIndicator'
      &pushbyte 3
      &pushsdb 14							//'attachMovie'
      &callFunction
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 15							//'isEndCap'
      &pushregister 3
      &pushzero
      &equals
      &not
      &setMember
      &pushregister 1
      &pushsdb 5							//'relX'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 6							//'relY'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 10							//'widthScale'
      &pushone
      &setMember
      &pushregister 1
      &pushsdb 11							//'lengthScale'
      &pushone
      &setMember
      &pushregister 1
      &pushone
      &pushsdbgv 3							//'indicators'
      &dcallmp 16							// push()
      &pushregister 1
      &pushone
      &pushsdbgv 17							//'Pointer'
      &pushsdb 18							//'encode'
      &callMethod
      &return
    &end // of function CreateIndicator

    &function2 DestroyIndicator (r:2='indicator') ()
      &pushzero
      &setRegister r:1
      &pop
     label6:
      &pushregister 1
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label9      
      &pushsdbgv 3							//'indicators'
      &pushregister 1
      &getMember
      &pushregister 2
      &equals
      &not
      &jnz label8      
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 3							//'indicators'
      &dcallmp 19							// splice()
      &pushsdbgv 3							//'indicators'
      &pushsdbgm 4							//'length'
      &pushzero
      &equals
      &not
      &jnz label7      
      &pushsdb 12							//'nextIndicatorId'
      &pushzerosv
     label7:
      &jmp label9      
     label8:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label6      
     label9:
      &pushzero
      &pushregister 2
      &dcallmp 20							// removeMovieClip()
    &end // of function DestroyIndicator

    &function2 MoveIndicator (r:1='indicator', r:3='relX', r:2='relY') ()
      &pushregister 1
      &pushsdb 5							//'relX'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 6							//'relY'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 7							//'_x'
      &pushregister 3
      &pushsdbgv 0							//'sizer'
      &pushsdbgm 1							//'_width'
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 8							//'_y'
      &pushregister 2
      &pushsdbgv 0							//'sizer'
      &pushsdbgm 2							//'_height'
      &multiply
      &setMember
    &end // of function MoveIndicator

    &function2 ScaleIndicator (r:1='indicator', r:3='widthScale', r:2='lengthScale') ()
      &pushregister 2
      &pushfloat 1.200000047683716
      &multiply
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdb 10							//'widthScale'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 11							//'lengthScale'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdbgm 9							//'iconMC'
      &pushsdb 2							//'_height'
      &pushsdbgv 0							//'sizer'
      &pushsdbgm 1							//'_width'
      &pushregister 3
      &multiply
      &setMember
      &pushregister 1
      &pushsdbgm 9							//'iconMC'
      &pushsdb 1							//'_width'
      &pushsdbgv 0							//'sizer'
      &pushsdbgm 1							//'_width'
      &pushregister 2
      &multiply
      &setMember
    &end // of function ScaleIndicator

    &function2 OnBound () (r:1='this', r:2='_global')
      &pushsdbgv 21							//'initialized'
      &not
      &jnz label10      
      &pushsdb 22							//'FSCommand:'
      &pushsdb 23							//'OnInitialized'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 25							//''
      &getURL2
     label10:
    &end // of function OnBound

    &function2 OnDestroying () (r:1='this', r:2='_global')
      &pushsdb 22							//'FSCommand:'
      &pushsdb 26							//'OnClipDestroyed'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'qualifyName'
      &callMethod
      &concat
      &pushsdb 25							//''
      &getURL2
    &end // of function OnDestroying

    &function onUnload (    )    
      &pushsdb 27							//'Resize'
      &delete2
      &pop
      &pushsdb 28							//'CreateIndicator'
      &delete2
      &pop
      &pushsdb 29							//'DestroyIndicator'
      &delete2
      &pop
      &pushsdb 30							//'MoveIndicator'
      &delete2
      &pop
      &pushsdb 31							//'OnBound'
      &delete2
      &pop
      &pushsdb 32							//'OnDestroying'
      &delete2
      &pop
      &pushsdb 33							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 21							//'initialized'
    &not
    &not
    &jnz label11    
    &pushthisgv
    &pushone
    &pushsdbgv 35							//'_parent'
    &dcallmp 36							// OnContentLoaded()
    &pushsdb 12							//'nextIndicatorId'
    &pushzerosv
    &pushsdb 3							//'indicators'
    &pushzero
    &pushsdb 37							//'Array'
    &new
    &setVariable
    &pushsdb 21							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 22							//'FSCommand:'
    &pushsdb 23							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 24							//'qualifyName'
    &callMethod
    &concat
    &pushsdb 25							//''
    &getURL2
   label11:
    &stop
  &end // of frame 0

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &placeMovieClip 6 &as 'sizer'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 6
&end
