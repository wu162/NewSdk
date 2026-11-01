movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TACTIC~3\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 1 // total frames: 1
  &end // of defineMovieClip 1

  &defineButton 3
  
    &on     &overDownToOverUp
      &constants 'client', 'OnClicked'  
      &pushsdbgv 0							//'client'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushzero
      &pushsdbgv 0							//'client'
      &dcallmp 1							// OnClicked()
     label1:
    &end
  &end // of defineButton 3

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &defineMovieClip 7 // total frames: 1

    &frame 0
      &constants '', 'messageClip', 'removeMovieClip', 'MpBeaconMessage', 'attachMovie', '_x', 'messagePos', '_y', 'FSCommand:', 'OnClicked', 'qualifyName', 'SetVisibility', 'Move', 'Destroy', 'CreateMessage', 'RemoveMessage', 'onUnload', 'initialized', 'arrow', 'client', 'this', 'beaconColor', 'colorizer', 'inner', 'Color', 'setRGB', 'OnInitialized'  
      &function2 SetVisibility (r:1='vis') ()
        &pushsdb 0							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 Move (r:2='x', r:1='y') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 1
        &setProperty
      &end // of function Move

      &function2 CreateMessage () (r:1='this')
        &pushzero
        &pushsdbgv 1							//'messageClip'
        &dcallmp 2							// removeMovieClip()
        &pushzero
        &pushsdb 1							//'messageClip'
        &pushsdb 3							//'MpBeaconMessage'
        &pushbyte 3
        &pushregister 1
        &dcallmp 4							// attachMovie()
        &pushsdbgv 1							//'messageClip'
        &pushsdb 5							//'_x'
        &pushsdbgv 6							//'messagePos'
        &pushsdbgm 5							//'_x'
        &setMember
        &pushsdbgv 1							//'messageClip'
        &pushsdb 7							//'_y'
        &pushsdbgv 6							//'messagePos'
        &pushsdbgm 7							//'_y'
        &setMember
      &end // of function CreateMessage

      &function RemoveMessage (      )      
        &pushzero
        &pushsdbgv 1							//'messageClip'
        &dcallmp 2							// removeMovieClip()
      &end // of function RemoveMessage

      &function2 OnClicked () (r:1='this')
        &pushsdb 8							//'FSCommand:'
        &pushsdb 9							//'OnClicked'
        &pushregister 1
        &pushbyte 2
        &pushsdb 10							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 0							//''
        &getURL2
      &end // of function OnClicked

      &function onUnload (      )      
        &pushsdb 11							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 12							//'Move'
        &delete2
        &pop
        &pushsdb 13							//'Destroy'
        &delete2
        &pop
        &pushsdb 14							//'CreateMessage'
        &delete2
        &pop
        &pushsdb 15							//'RemoveMessage'
        &delete2
        &pop
        &pushsdb 9							//'OnClicked'
        &delete2
        &pop
        &pushsdb 16							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 17							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdbgv 18							//'arrow'
      &pushsdb 19							//'client'
      &pushthisgv
      &setMember
      &pushsdbgv 21							//'beaconColor'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushsdb 22							//'colorizer'
      &pushsdbgv 18							//'arrow'
      &pushsdbgm 23							//'inner'
      &pushone
      &pushsdb 24							//'Color'
      &new
      &varEquals
      &pushsdbgv 21							//'beaconColor'
      &pushone
      &pushsdbgv 22							//'colorizer'
      &dcallmp 25							// setRGB()
     label1:
      &pushsdb 17							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 8							//'FSCommand:'
      &pushsdb 26							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 10							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label2:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 7
  
  &exportAssets
    7 &as 'MpBeaconIndicator'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 1

    &frame 0
      &constants 'initialized', '_parent', 'beaconColor', 'colorizer1', 'cornerUL', 'Color', 'setRGB', 'colorizer2', 'cornerUR', 'colorizer3', 'cornerLL', 'colorizer4', 'cornerLR', 'colorizer5', 'edgeT', 'colorizer6', 'edgeB', 'colorizer7', 'edgeL', 'colorizer8', 'edgeR', 'inset', 'clipWidth', 'messageText', '_width', 'clipHeight', '_height', 'this', '_x', '_y', 'background', '_xscale', '_yscale', 'cornerSize'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label4      
      &pushsdbgv 1							//'_parent'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label1      
      &pop
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushundef
      &equals
      &not
     label1:
      &dup
      &not
      &jnz label2      
      &pop
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushundef
      &equals
      &not
     label2:
      &not
      &jnz label3      
      &pushsdb 3							//'colorizer1'
      &pushsdbgv 4							//'cornerUL'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 3							//'colorizer1'
      &dcallmp 6							// setRGB()
      &pushsdb 7							//'colorizer2'
      &pushsdbgv 8							//'cornerUR'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 7							//'colorizer2'
      &dcallmp 6							// setRGB()
      &pushsdb 9							//'colorizer3'
      &pushsdbgv 10							//'cornerLL'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 9							//'colorizer3'
      &dcallmp 6							// setRGB()
      &pushsdb 11							//'colorizer4'
      &pushsdbgv 12							//'cornerLR'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 11							//'colorizer4'
      &dcallmp 6							// setRGB()
      &pushsdb 13							//'colorizer5'
      &pushsdbgv 14							//'edgeT'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 13							//'colorizer5'
      &dcallmp 6							// setRGB()
      &pushsdb 15							//'colorizer6'
      &pushsdbgv 16							//'edgeB'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 15							//'colorizer6'
      &dcallmp 6							// setRGB()
      &pushsdb 17							//'colorizer7'
      &pushsdbgv 18							//'edgeL'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 17							//'colorizer7'
      &dcallmp 6							// setRGB()
      &pushsdb 19							//'colorizer8'
      &pushsdbgv 20							//'edgeR'
      &pushone
      &pushsdb 5							//'Color'
      &new
      &varEquals
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 1							//'_parent'
      &pushsdbgm 2							//'beaconColor'
      &pushone
      &pushsdbgv 19							//'colorizer8'
      &dcallmp 6							// setRGB()
     label3:
      &pushsdb 21							//'inset'
      &pushbyte 2
      &varEquals
      &pushsdb 22							//'clipWidth'
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 23							//'messageText'
      &pushsdbgm 24							//'_width'
      &pushbyte 25
      &add
      &varEquals
      &pushsdb 25							//'clipHeight'
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 23							//'messageText'
      &pushsdbgm 26							//'_height'
      &pushbyte 20
      &add
      &varEquals
      &pushthisgv
      &pushsdb 28							//'_x'
      &pushzero
      &pushsdbgv 22							//'clipWidth'
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &setMember
      &pushthisgv
      &pushsdb 29							//'_y'
      &pushsdbgv 1							//'_parent'
      &pushsdbgm 23							//'messageText'
      &pushsdbgm 29							//'_y'
      &pushbyte 11
      &subtract
      &setMember
      &pushsdbgv 30							//'background'
      &pushsdb 31							//'_xscale'
      &pushsdbgv 22							//'clipWidth'
      &setMember
      &pushsdbgv 30							//'background'
      &pushsdb 32							//'_yscale'
      &pushsdbgv 25							//'clipHeight'
      &setMember
      &pushsdbgv 4							//'cornerUL'
      &pushsdb 28							//'_x'
      &pushsdbgv 21							//'inset'
      &setMember
      &pushsdbgv 4							//'cornerUL'
      &pushsdb 29							//'_y'
      &pushsdbgv 21							//'inset'
      &setMember
      &pushsdbgv 8							//'cornerUR'
      &pushsdb 28							//'_x'
      &pushsdbgv 30							//'background'
      &pushsdbgm 24							//'_width'
      &pushsdbgv 21							//'inset'
      &subtract
      &setMember
      &pushsdbgv 8							//'cornerUR'
      &pushsdb 29							//'_y'
      &pushsdbgv 21							//'inset'
      &setMember
      &pushsdbgv 10							//'cornerLL'
      &pushsdb 28							//'_x'
      &pushsdbgv 21							//'inset'
      &setMember
      &pushsdbgv 10							//'cornerLL'
      &pushsdb 29							//'_y'
      &pushsdbgv 30							//'background'
      &pushsdbgm 26							//'_height'
      &pushsdbgv 21							//'inset'
      &subtract
      &setMember
      &pushsdbgv 12							//'cornerLR'
      &pushsdb 28							//'_x'
      &pushsdbgv 30							//'background'
      &pushsdbgm 24							//'_width'
      &pushsdbgv 21							//'inset'
      &subtract
      &setMember
      &pushsdbgv 12							//'cornerLR'
      &pushsdb 29							//'_y'
      &pushsdbgv 30							//'background'
      &pushsdbgm 26							//'_height'
      &pushsdbgv 21							//'inset'
      &subtract
      &setMember
      &pushsdb 33							//'cornerSize'
      &pushsdbgv 4							//'cornerUL'
      &pushsdbgm 24							//'_width'
      &varEquals
      &pushsdbgv 14							//'edgeT'
      &pushsdb 28							//'_x'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &pushbyte 2
      &multiply
      &add
      &setMember
      &pushsdbgv 14							//'edgeT'
      &pushsdb 29							//'_y'
      &pushsdbgv 21							//'inset'
      &setMember
      &pushsdbgv 14							//'edgeT'
      &pushsdb 31							//'_xscale'
      &pushsdbgv 22							//'clipWidth'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &add
      &pushsdbgv 21							//'inset'
      &add
      &pushbyte 2
      &multiply
      &subtract
      &setMember
      &pushsdbgv 14							//'edgeT'
      &pushsdb 32							//'_yscale'
      &pushone
      &setMember
      &pushsdbgv 16							//'edgeB'
      &pushsdb 28							//'_x'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &pushbyte 2
      &multiply
      &add
      &setMember
      &pushsdbgv 16							//'edgeB'
      &pushsdb 29							//'_y'
      &pushsdbgv 30							//'background'
      &pushsdbgm 26							//'_height'
      &pushsdbgv 21							//'inset'
      &subtract
      &pushsdbgv 16							//'edgeB'
      &pushsdbgm 26							//'_height'
      &subtract
      &setMember
      &pushsdbgv 16							//'edgeB'
      &pushsdb 31							//'_xscale'
      &pushsdbgv 22							//'clipWidth'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &add
      &pushsdbgv 21							//'inset'
      &add
      &pushbyte 2
      &multiply
      &subtract
      &setMember
      &pushsdbgv 16							//'edgeB'
      &pushsdb 32							//'_yscale'
      &pushone
      &setMember
      &pushsdbgv 18							//'edgeL'
      &pushsdb 28							//'_x'
      &pushsdbgv 21							//'inset'
      &setMember
      &pushsdbgv 18							//'edgeL'
      &pushsdb 29							//'_y'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &pushbyte 2
      &multiply
      &add
      &setMember
      &pushsdbgv 18							//'edgeL'
      &pushsdb 31							//'_xscale'
      &pushone
      &setMember
      &pushsdbgv 18							//'edgeL'
      &pushsdb 32							//'_yscale'
      &pushsdbgv 25							//'clipHeight'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &add
      &pushsdbgv 21							//'inset'
      &add
      &pushbyte 2
      &multiply
      &subtract
      &setMember
      &pushsdbgv 20							//'edgeR'
      &pushsdb 28							//'_x'
      &pushsdbgv 30							//'background'
      &pushsdbgm 24							//'_width'
      &pushsdbgv 21							//'inset'
      &subtract
      &pushsdbgv 20							//'edgeR'
      &pushsdbgm 24							//'_width'
      &subtract
      &setMember
      &pushsdbgv 20							//'edgeR'
      &pushsdb 29							//'_y'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &pushbyte 2
      &multiply
      &add
      &setMember
      &pushsdbgv 20							//'edgeR'
      &pushsdb 31							//'_xscale'
      &pushone
      &setMember
      &pushsdbgv 20							//'edgeR'
      &pushsdb 32							//'_yscale'
      &pushsdbgv 25							//'clipHeight'
      &pushsdbgv 33							//'cornerSize'
      &pushsdbgv 21							//'inset'
      &add
      &pushsdbgv 21							//'inset'
      &add
      &pushbyte 2
      &multiply
      &subtract
      &setMember
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label4:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 20

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 24 // total frames: 1

    &frame 0
      &constants 'initialized', 'messageText', 'text', '$', 'Message', '_parent', 'qualifyName', '&outline', 'field', 'autoSize', 'left', '_width', '_x'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 1							//'messageText'
      &pushsdb 2							//'text'
      &pushsdb 3							//'$'
      &pushsdb 4							//'Message'
      &pushsdbgv 5							//'_parent'
      &pushbyte 2
      &pushsdb 6							//'qualifyName'
      &callFunction
      &add
      &pushsdb 7							//'&outline'
      &add
      &setMember
      &pushsdbgv 1							//'messageText'
      &pushsdbgm 8							//'field'
      &pushsdb 9							//'autoSize'
      &pushsdb 10							//'left'
      &setMember
      &pushsdbgv 1							//'messageText'
      &pushsdbgm 8							//'field'
      &pushsdbgm 11							//'_width'
      &pop
      &pushsdbgv 1							//'messageText'
      &pushsdb 12							//'_x'
      &pushzero
      &pushsdbgv 1							//'messageText'
      &pushsdbgm 11							//'_width'
      &pushfloat 0.500000000000000
      &multiply
      &subtract
      &setMember
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 24
  
  &exportAssets
    24 &as 'MpBeaconMessage'
  &end // of exportAssets

  &defineButton 25
  
    &on     &overDownToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnClicked'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnRollOver'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &pushs 'FSCommand:'
      &pushs 'OnRollOut'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 25

  &defineMovieClip 30 // total frames: 1
  &end // of defineMovieClip 30

  &defineMovieClip 31 // total frames: 15

    &frame 0
      &stop
    &end // of frame 0

    &frame 14
      &gotoLabel '_stop'
    &end // of frame 14
  &end // of defineMovieClip 31

  &defineMovieClip 32 // total frames: 1

    &frame 0
      &constants '', '_play', 'selectionEffect', 'gotoAndPlay', 'SetVisibility', 'Move', 'PlaySelectionEffect', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 SetVisibility (r:1='vis') ()
        &pushsdb 0							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 Move (r:2='x', r:1='y') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 1
        &setProperty
      &end // of function Move

      &function PlaySelectionEffect (      )      
        &pushsdb 1							//'_play'
        &pushone
        &pushsdbgv 2							//'selectionEffect'
        &dcallmp 3							// gotoAndPlay()
      &end // of function PlaySelectionEffect

      &function onUnload (      )      
        &pushsdb 4							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 5							//'Move'
        &delete2
        &pop
        &pushsdb 6							//'PlaySelectionEffect'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 9							//'FSCommand:'
      &pushsdb 10							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 32
  
  &exportAssets
    32 &as 'AIHoldBeaconIndicator'
  &end // of exportAssets

  &defineButton 34
  
    &on     &overDownToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnClicked'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnRollOver'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &pushs 'FSCommand:'
      &pushs 'OnRollOut'
      &pushthisgv
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 34

  &defineMovieClip 37 // total frames: 1

    &frame 0
      &constants '', '_play', 'selectionEffect', 'gotoAndPlay', 'SetVisibility', 'Move', 'PlaySelectionEffect', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 SetVisibility (r:1='vis') ()
        &pushsdb 0							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 Move (r:2='x', r:1='y') ()
        &pushsdb 0							//''
        &pushzero
        &pushregister 2
        &setProperty
        &pushsdb 0							//''
        &pushone
        &pushregister 1
        &setProperty
      &end // of function Move

      &function PlaySelectionEffect (      )      
        &pushsdb 1							//'_play'
        &pushone
        &pushsdbgv 2							//'selectionEffect'
        &dcallmp 3							// gotoAndPlay()
      &end // of function PlaySelectionEffect

      &function onUnload (      )      
        &pushsdb 4							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 5							//'Move'
        &delete2
        &pop
        &pushsdb 6							//'PlaySelectionEffect'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 9							//'FSCommand:'
      &pushsdb 10							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 37
  
  &exportAssets
    37 &as 'AIAssaultBeaconIndicator'
  &end // of exportAssets

  &defineMovieClip 39 // total frames: 1
  &end // of defineMovieClip 39

  &defineMovieClip 40 // total frames: 1
  &end // of defineMovieClip 40

  &defineMovieClip 43 // total frames: 1
  &end // of defineMovieClip 43

  &defineMovieClip 45 // total frames: 1
  &end // of defineMovieClip 45

  &defineMovieClip 48 // total frames: 1
  &end // of defineMovieClip 48

  &defineMovieClip 50 // total frames: 1

    &placeMovieClip 40 &as 'background'
    &end // of placeMovieClip 40
  &end // of defineMovieClip 50

  &defineMovieClip 55 // total frames: 1
  &end // of defineMovieClip 55

  &defineMovieClip 76 // total frames: 10

    &frame 0
      &stop
    &end // of frame 0

    &frame 1
      &stop
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2

    &frame 3
      &stop
    &end // of frame 3

    &frame 4
      &stop
    &end // of frame 4

    &frame 5
      &stop
    &end // of frame 5

    &frame 6
      &stop
    &end // of frame 6

    &frame 7
      &stop
    &end // of frame 7

    &frame 8
      &stop
    &end // of frame 8

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 76

  &defineMovieClip 79 // total frames: 1
  &end // of defineMovieClip 79

  &defineMovieClip 80 // total frames: 1
  &end // of defineMovieClip 80

  &defineMovieClip 81 // total frames: 1

    &frame 0
      &constants 'horzMargin', 'minWidth', 'vertMargin', 'minHeight', '_x', '_y', 'countdownMC', 'countdownWidth', 'tr', 'right', 'br', 'bl', 'bottom', 'top', '_width', 'tl', 'center', 'left', '_height', 'timesUp', 'timesUpOffsetX', 'timesUpOffsetY', '', 'textMC', 'tenMinuteMC', 'gotoAndStop', 'minuteMC', 'tenSecondMC', 'secondMC', '_visible', 'Move', 'SetVisibility', 'SetTimeRemaining', 'ShowTimesUp', 'onUnload', 'initialized', 'width', 'height', 'messageTF', 'text', '$TimeBombMessage&outline', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', 'extern', 'InGame'  
      &function2 Move (r:5='x', r:4='y', r:2='width', r:3='height') (r:1='this')
        &pushregister 5
        &pushsdbgv 0							//'horzMargin'
        &subtract
        &setRegister r:5
        &pop
        &pushregister 2
        &pushsdbgv 0							//'horzMargin'
        &pushbyte 2
        &multiply
        &add
        &setRegister r:2
        &pop
        &pushregister 2
        &pushsdbgv 1							//'minWidth'
        &lessThan
        &not
        &jnz label1        
        &pushregister 5
        &pushsdbgv 1							//'minWidth'
        &pushregister 2
        &subtract
        &pushfloat 0.500000000000000
        &multiply
        &subtract
        &setRegister r:5
        &pop
        &pushsdbgv 1							//'minWidth'
        &setRegister r:2
        &pop
       label1:
        &pushregister 4
        &pushsdbgv 2							//'vertMargin'
        &subtract
        &setRegister r:4
        &pop
        &pushregister 3
        &pushsdbgv 2							//'vertMargin'
        &pushbyte 2
        &multiply
        &add
        &setRegister r:3
        &pop
        &pushregister 3
        &pushsdbgv 3							//'minHeight'
        &lessThan
        &not
        &jnz label2        
        &pushregister 4
        &pushsdbgv 3							//'minHeight'
        &pushregister 3
        &subtract
        &pushfloat 0.500000000000000
        &multiply
        &subtract
        &setRegister r:4
        &pop
        &pushsdbgv 3							//'minHeight'
        &setRegister r:3
        &pop
       label2:
        &pushregister 1
        &pushsdb 4							//'_x'
        &pushregister 5
        &setMember
        &pushregister 1
        &pushsdb 5							//'_y'
        &pushregister 4
        &setMember
        &pushsdbgv 6							//'countdownMC'
        &pushsdb 4							//'_x'
        &pushregister 2
        &pushsdbgv 7							//'countdownWidth'
        &subtract
        &pushfloat 0.500000000000000
        &multiply
        &setMember
        &pushsdbgv 8							//'tr'
        &pushsdb 4							//'_x'
        &pushregister 2
        &setMember
        &pushsdbgv 9							//'right'
        &pushsdb 4							//'_x'
        &pushregister 2
        &setMember
        &pushsdbgv 10							//'br'
        &pushsdb 4							//'_x'
        &pushregister 2
        &setMember
        &pushsdbgv 11							//'bl'
        &pushsdb 5							//'_y'
        &pushregister 3
        &setMember
        &pushsdbgv 12							//'bottom'
        &pushsdb 5							//'_y'
        &pushregister 3
        &setMember
        &pushsdbgv 10							//'br'
        &pushsdb 5							//'_y'
        &pushregister 3
        &setMember
        &pushsdbgv 13							//'top'
        &pushsdb 14							//'_width'
        &pushregister 2
        &pushsdbgv 15							//'tl'
        &pushsdbgm 14							//'_width'
        &pushsdbgv 8							//'tr'
        &pushsdbgm 14							//'_width'
        &add
        &subtract
        &setMember
        &pushsdbgv 16							//'center'
        &pushsdb 14							//'_width'
        &pushregister 2
        &pushsdbgv 17							//'left'
        &pushsdbgm 14							//'_width'
        &pushsdbgv 9							//'right'
        &pushsdbgm 14							//'_width'
        &add
        &subtract
        &setMember
        &pushsdbgv 12							//'bottom'
        &pushsdb 14							//'_width'
        &pushregister 2
        &pushsdbgv 11							//'bl'
        &pushsdbgm 14							//'_width'
        &pushsdbgv 10							//'br'
        &pushsdbgm 14							//'_width'
        &add
        &subtract
        &setMember
        &pushsdbgv 17							//'left'
        &pushsdb 18							//'_height'
        &pushregister 3
        &pushsdbgv 15							//'tl'
        &pushsdbgm 18							//'_height'
        &pushsdbgv 11							//'bl'
        &pushsdbgm 18							//'_height'
        &add
        &subtract
        &setMember
        &pushsdbgv 16							//'center'
        &pushsdb 18							//'_height'
        &pushregister 3
        &pushsdbgv 13							//'top'
        &pushsdbgm 18							//'_height'
        &pushsdbgv 12							//'bottom'
        &pushsdbgm 18							//'_height'
        &add
        &subtract
        &setMember
        &pushsdbgv 9							//'right'
        &pushsdb 18							//'_height'
        &pushregister 3
        &pushsdbgv 8							//'tr'
        &pushsdbgm 18							//'_height'
        &pushsdbgv 10							//'br'
        &pushsdbgm 18							//'_height'
        &add
        &subtract
        &setMember
        &pushsdbgv 19							//'timesUp'
        &pushsdb 4							//'_x'
        &pushregister 2
        &pushfloat 0.500000000000000
        &multiply
        &pushsdbgv 20							//'timesUpOffsetX'
        &add
        &setMember
        &pushsdbgv 19							//'timesUp'
        &pushsdb 5							//'_y'
        &pushregister 3
        &pushfloat 0.500000000000000
        &multiply
        &pushsdbgv 21							//'timesUpOffsetY'
        &add
        &setMember
      &end // of function Move

      &function2 SetVisibility (r:1='vis') ()
        &pushsdb 22							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 SetTimeRemaining (r:1='seconds') ()
        &pushzero
        &setRegister r:2
        &pop
       label3:
        &pushregister 1
        &pushbyte 60
        &lessThan
        &not
        &not
        &jnz label4        
        &pushregister 2
        &increment
        &setRegister r:2
        &pop
        &pushregister 1
        &pushbyte 60
        &subtract
        &setRegister r:1
        &pop
        &jmp label3        
       label4:
        &pushzero
        &setRegister r:3
        &pop
       label5:
        &pushregister 2
        &pushbyte 10
        &lessThan
        &not
        &not
        &jnz label6        
        &pushregister 3
        &increment
        &setRegister r:3
        &pop
        &pushregister 2
        &pushbyte 10
        &subtract
        &setRegister r:2
        &pop
        &jmp label5        
       label6:
        &pushzero
        &setRegister r:4
        &pop
       label7:
        &pushregister 1
        &pushbyte 10
        &lessThan
        &not
        &not
        &jnz label8        
        &pushregister 4
        &increment
        &setRegister r:4
        &pop
        &pushregister 1
        &pushbyte 10
        &subtract
        &setRegister r:1
        &pop
        &jmp label7        
       label8:
        &pushsdbgv 6							//'countdownMC'
        &pushsdbgm 23							//'textMC'
        &setRegister r:5
        &pop
        &pushregister 3
        &pushone
        &add
        &pushone
        &pushregister 5
        &pushsdbgm 24							//'tenMinuteMC'
        &dcallmp 25							// gotoAndStop()
        &pushregister 2
        &pushone
        &add
        &pushone
        &pushregister 5
        &pushsdbgm 26							//'minuteMC'
        &dcallmp 25							// gotoAndStop()
        &pushregister 4
        &pushone
        &add
        &pushone
        &pushregister 5
        &pushsdbgm 27							//'tenSecondMC'
        &dcallmp 25							// gotoAndStop()
        &pushregister 1
        &pushone
        &add
        &pushone
        &pushregister 5
        &pushsdbgm 28							//'secondMC'
        &dcallmp 25							// gotoAndStop()
      &end // of function SetTimeRemaining

      &function ShowTimesUp (      )      
        &pushsdbgv 19							//'timesUp'
        &pushsdb 29							//'_visible'
        &pushtrue
        &setMember
      &end // of function ShowTimesUp

      &function onUnload (      )      
        &pushsdb 30							//'Move'
        &delete2
        &pop
        &pushsdb 31							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 32							//'SetTimeRemaining'
        &delete2
        &pop
        &pushsdb 33							//'ShowTimesUp'
        &delete2
        &pop
        &pushsdb 34							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 35							//'initialized'
      &not
      &not
      &jnz label9      
      &pushsdb 0							//'horzMargin'
      &pushbyte 8
      &setVariable
      &pushsdb 2							//'vertMargin'
      &pushbyte 8
      &setVariable
      &pushsdb 36							//'width'
      &pushsdbgv 10							//'br'
      &pushsdbgm 4							//'_x'
      &varEquals
      &pushsdb 37							//'height'
      &pushsdbgv 10							//'br'
      &pushsdbgm 5							//'_y'
      &varEquals
      &pushsdb 1							//'minWidth'
      &pushsdbgv 36							//'width'
      &setVariable
      &pushsdb 3							//'minHeight'
      &pushsdbgv 37							//'height'
      &setVariable
      &pushsdb 20							//'timesUpOffsetX'
      &pushsdbgv 19							//'timesUp'
      &pushsdbgm 4							//'_x'
      &pushsdbgv 36							//'width'
      &pushfloat 0.500000000000000
      &multiply
      &subtract
      &setVariable
      &pushsdb 21							//'timesUpOffsetY'
      &pushsdbgv 19							//'timesUp'
      &pushsdbgm 5							//'_y'
      &pushsdbgv 37							//'height'
      &pushfloat 0.500000000000000
      &multiply
      &subtract
      &setVariable
      &pushsdb 7							//'countdownWidth'
      &pushsdbgv 6							//'countdownMC'
      &pushsdbgm 14							//'_width'
      &setVariable
      &pushsdbgv 15							//'tl'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 13							//'top'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 8							//'tr'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 9							//'right'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 10							//'br'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 12							//'bottom'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 11							//'bl'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 17							//'left'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 19							//'timesUp'
      &pushsdb 29							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 22							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdbgv 19							//'timesUp'
      &pushsdbgm 38							//'messageTF'
      &pushsdb 39							//'text'
      &pushsdb 40							//'$TimeBombMessage&outline'
      &setMember
      &pushsdb 35							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 41							//'FSCommand:'
      &pushsdb 42							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 44							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 22							//''
      &getURL2
      &pushsdbgv 45							//'extern'
      &pushsdbgm 46							//'InGame'
      &not
      &not
      &jnz label9      
      &pushsdb 22							//''
      &pushbyte 7
      &pushtrue
      &setProperty
      &pushshort 200
      &pushbyte 100
      &pushbyte 100
      &pushbyte 100
      &pushbyte 4
      &dcallfp 30							// Move()
      &pushbyte 18
      &pushone
      &dcallfp 32							// SetTimeRemaining()
      &pushzero
      &dcallfp 33							// ShowTimesUp()
     label9:
      &stop
    &end // of frame 0

    &placeMovieClip 39 &as 'center'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 39

    &placeMovieClip 43 &as 'left'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 43

    &placeMovieClip 48 &as 'bl'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 48

    &placeMovieClip 50 &as 'bottom'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 50

    &placeMovieClip 48 &as 'br'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 48

    &placeMovieClip 43 &as 'right'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 43

    &placeMovieClip 48 &as 'tr'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 48

    &placeMovieClip 50 &as 'top'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 50

    &placeMovieClip 48 &as 'tl'
    
      &onClipEvent &load
        &pushsgv 'background'
        &pushs '_visible'
        &pushfalse
        &setMember
      &end
    &end // of placeMovieClip 48
  &end // of defineMovieClip 81
  
  &exportAssets
    81 &as 'TimeBombFrame'
  &end // of exportAssets

  &frame 0
    &constants 'indicatorCount', 'nextIndicatorId', 'beacon', 'attachMovie', 'client', 'MpBeaconIndicator', 'CreateBeaconIndicator', 'beaconColor', 'inGame', 'Pointer', 'encode', 'AIAssaultBeaconIndicator', 'AIHoldBeaconIndicator', 'removeMovieClip', 'TimeBombFrame', 'CreateMpBeaconIndicator', 'CreateAIAssaultBeaconIndicator', 'CreateAIHoldBeaconIndicator', 'DestroyBeaconIndicator', 'CreateTimeBombFrame', 'onUnload', 'extern', 'InGame', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
    &function2 CreateBeaconIndicator (r:4='indicatorType') (r:1='this')
      &pushsdb 0							//'indicatorCount'
      &pushsdbgv 0							//'indicatorCount'
      &increment
      &setVariable
      &pushsdbgv 1							//'nextIndicatorId'
      &pushsdb 1							//'nextIndicatorId'
      &pushsdbgv 1							//'nextIndicatorId'
      &increment
      &setVariable
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 2
      &toString
      &pushregister 4
      &pushbyte 3
      &pushsdbgv 2							//'beacon'
      &pushsdb 3							//'attachMovie'
      &callMethod
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 4							//'client'
      &pushregister 1
      &setMember
      &pushregister 3
      &return
    &end // of function CreateBeaconIndicator

    &function2 CreateMpBeaconIndicator (r:2='beaconColor') ()
      &pushsdb 5							//'MpBeaconIndicator'
      &pushone
      &pushsdb 6							//'CreateBeaconIndicator'
      &callFunction
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 7							//'beaconColor'
      &pushregister 2
      &setMember
      &pushsdbgv 8							//'inGame'
      &jnz label1      
      &pushregister 1
      &jmp label2      
     label1:
      &pushregister 1
      &pushone
      &pushsdbgv 9							//'Pointer'
      &pushsdb 10							//'encode'
      &callMethod
     label2:
      &return
    &end // of function CreateMpBeaconIndicator

    &function2 CreateAIAssaultBeaconIndicator () ()
      &pushsdb 11							//'AIAssaultBeaconIndicator'
      &pushone
      &pushsdb 6							//'CreateBeaconIndicator'
      &callFunction
      &setRegister r:1
      &pop
      &pushsdbgv 8							//'inGame'
      &jnz label3      
      &pushregister 1
      &jmp label4      
     label3:
      &pushregister 1
      &pushone
      &pushsdbgv 9							//'Pointer'
      &pushsdb 10							//'encode'
      &callMethod
     label4:
      &return
    &end // of function CreateAIAssaultBeaconIndicator

    &function2 CreateAIHoldBeaconIndicator () ()
      &pushsdb 12							//'AIHoldBeaconIndicator'
      &pushone
      &pushsdb 6							//'CreateBeaconIndicator'
      &callFunction
      &setRegister r:1
      &pop
      &pushsdbgv 8							//'inGame'
      &jnz label5      
      &pushregister 1
      &jmp label6      
     label5:
      &pushregister 1
      &pushone
      &pushsdbgv 9							//'Pointer'
      &pushsdb 10							//'encode'
      &callMethod
     label6:
      &return
    &end // of function CreateAIHoldBeaconIndicator

    &function2 DestroyBeaconIndicator (r:1='indicator') ()
      &pushzero
      &pushregister 1
      &dcallmp 13							// removeMovieClip()
      &pushsdb 0							//'indicatorCount'
      &pushsdbgv 0							//'indicatorCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label7      
      &pushsdb 1							//'nextIndicatorId'
      &pushzerosv
     label7:
    &end // of function DestroyBeaconIndicator

    &function2 CreateTimeBombFrame () ()
      &pushsdb 14							//'TimeBombFrame'
      &pushone
      &pushsdb 6							//'CreateBeaconIndicator'
      &callFunction
      &setRegister r:1
      &pop
      &pushsdbgv 8							//'inGame'
      &jnz label8      
      &pushregister 1
      &jmp label9      
     label8:
      &pushregister 1
      &pushone
      &pushsdbgv 9							//'Pointer'
      &pushsdb 10							//'encode'
      &callMethod
     label9:
      &return
    &end // of function CreateTimeBombFrame

    &function onUnload (    )    
      &pushsdb 6							//'CreateBeaconIndicator'
      &delete2
      &pop
      &pushsdb 15							//'CreateMpBeaconIndicator'
      &delete2
      &pop
      &pushsdb 16							//'CreateAIAssaultBeaconIndicator'
      &delete2
      &pop
      &pushsdb 17							//'CreateAIHoldBeaconIndicator'
      &delete2
      &pop
      &pushsdb 18							//'DestroyBeaconIndicator'
      &delete2
      &pop
      &pushsdb 19							//'CreateTimeBombFrame'
      &delete2
      &pop
      &pushsdb 20							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdb 8							//'inGame'
    &pushsdbgv 21							//'extern'
    &pushsdbgm 22							//'InGame'
    &setVariable
    &pushsdbgv 23							//'initialized'
    &not
    &not
    &jnz label10    
    &pushsdb 1							//'nextIndicatorId'
    &pushzerosv
    &pushsdb 0							//'indicatorCount'
    &pushzerosv
    &pushsdb 23							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 24							//'FSCommand:'
    &pushsdb 25							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 27							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 28							//''
    &getURL2
    &pushsdbgv 8							//'inGame'
    &not
    &not
    &jnz label10    
    &pushlong 16711935
    &pushone
    &dcallfp 15							// CreateMpBeaconIndicator()
    &pushzero
    &dcallfp 19							// CreateTimeBombFrame()
   label10:
    &stop
  &end // of frame 0
&end
