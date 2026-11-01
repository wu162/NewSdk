movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\NOTIFI~1\\NOTIFI~1.eaf' &compressed // flash 7, total frames: 12, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'closing', 'Close', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function Close (    )    
      &pushsdbgv 0							//'closing'
      &not
      &not
      &jnz label1      
      &gotoLabel '_close'
      &play
      &pushsdb 0							//'closing'
      &pushtrue
      &setVariable
     label1:
    &end // of function Close

    &function onUnload (    )    
      &pushsdb 1							//'Close'
      &delete2
      &pop
      &pushsdb 2							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 3							//'initialized'
    &not
    &not
    &jnz label2    
    &pushthisgv
    &pushone
    &pushsdbgv 5							//'_parent'
    &dcallmp 6							// OnContentLoaded()
    &pushsdb 0							//'closing'
    &pushfalse
    &setVariable
    &pushsdb 3							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 7							//'FSCommand:'
    &pushsdb 8							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 9							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 10							//''
    &getURL2
   label2:
  &end // of frame 0
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'BlackRectangle' &as 1
  &end // of importAssets

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 14 // total frames: 1

    &frame 0
      &constants 'initialized', 'baseMessageHeight', 'messageMC', '_height', 'field', 'text', '$', 'Message', '_parent', 'qualifyName', '&outline', 'autoSize', 'left', 'actualMessageHeight', 'heightIncrease', 'middleMC', 'bottomMC', '_y', 'backgroundMC'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 1							//'baseMessageHeight'
      &pushsdbgv 2							//'messageMC'
      &pushsdbgm 3							//'_height'
      &varEquals
      &pushsdbgv 2							//'messageMC'
      &pushsdbgm 4							//'field'
      &pushsdb 5							//'text'
      &pushsdb 6							//'$'
      &pushsdb 7							//'Message'
      &pushsdbgv 8							//'_parent'
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &add
      &pushsdb 10							//'&outline'
      &add
      &setMember
      &pushsdbgv 2							//'messageMC'
      &pushsdbgm 4							//'field'
      &pushsdb 11							//'autoSize'
      &pushsdb 12							//'left'
      &setMember
      &pushsdbgv 2							//'messageMC'
      &pushsdbgm 4							//'field'
      &pushsdbgm 3							//'_height'
      &pop
      &pushsdb 13							//'actualMessageHeight'
      &pushsdbgv 2							//'messageMC'
      &pushsdbgm 3							//'_height'
      &varEquals
      &pushsdbgv 13							//'actualMessageHeight'
      &pushsdbgv 1							//'baseMessageHeight'
      &greaterThan
      &not
      &jnz label1      
      &pushsdb 14							//'heightIncrease'
      &pushsdbgv 13							//'actualMessageHeight'
      &pushsdbgv 1							//'baseMessageHeight'
      &subtract
      &varEquals
      &pushsdbgv 15							//'middleMC'
      &pushsdb 3							//'_height'
      &pushsdbgv 15							//'middleMC'
      &pushsdbgm 3							//'_height'
      &pushsdbgv 14							//'heightIncrease'
      &add
      &setMember
      &pushsdbgv 16							//'bottomMC'
      &pushsdb 17							//'_y'
      &pushsdbgv 16							//'bottomMC'
      &pushsdbgm 17							//'_y'
      &pushsdbgv 14							//'heightIncrease'
      &add
      &setMember
      &pushsdbgv 18							//'backgroundMC'
      &pushsdb 3							//'_height'
      &pushsdbgv 18							//'backgroundMC'
      &pushsdbgm 3							//'_height'
      &pushsdbgv 14							//'heightIncrease'
      &add
      &setMember
     label1:
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label2:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 14

  &frame 5
    &stop
  &end // of frame 5

  &frame 11
    &stop
    &pushs 'FSCommand:'
    &pushs 'OnClosed'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
  &end // of frame 11
&end
