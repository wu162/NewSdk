movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TACTIC~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 43, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'panel', 'gotoAndPlay', 'factionLabel', 'imageMC', 'SetEndGameType', 'SetFaction', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'backgroundX', '_root', 'displayLeft', 'backgroundY', 'displayTop', 'ancestor', '_x', '_y', 'background', '_width', 'displayWidth', '_height', 'displayHeight', 'loopCount', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function2 SetEndGameType (r:1='endGameType') ()
      &pushregister 1
      &pushone
      &pushsdbgv 0							//'panel'
      &dcallmp 1							// gotoAndPlay()
    &end // of function SetEndGameType

    &function2 SetFaction (r:1='faction') ()
      &pushsdbgv 0							//'panel'
      &pushsdb 2							//'factionLabel'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushone
      &pushsdbgv 0							//'panel'
      &pushsdbgm 3							//'imageMC'
      &dcallmp 1							// gotoAndPlay()
    &end // of function SetFaction

    &function onUnload (    )    
      &pushsdb 4							//'SetEndGameType'
      &delete2
      &pop
      &pushsdb 5							//'SetFaction'
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
    &pushthisgv
    &pushone
    &pushsdbgv 9							//'_parent'
    &dcallmp 10							// OnContentLoaded()
    &pushsdb 11							//'backgroundX'
    &pushsdbgv 12							//'_root'
    &pushsdbgm 13							//'displayLeft'
    &varEquals
    &pushsdb 14							//'backgroundY'
    &pushsdbgv 12							//'_root'
    &pushsdbgm 15							//'displayTop'
    &varEquals
    &pushsdb 16							//'ancestor'
    &pushthisgv
    &varEquals
   label1:
    &pushsdbgv 16							//'ancestor'
    &pushsdbgv 12							//'_root'
    &equals
    &not
    &not
    &jnz label2    
    &pushsdb 11							//'backgroundX'
    &pushsdbgv 11							//'backgroundX'
    &pushsdbgv 16							//'ancestor'
    &pushsdbgm 17							//'_x'
    &subtract
    &setVariable
    &pushsdb 14							//'backgroundY'
    &pushsdbgv 14							//'backgroundY'
    &pushsdbgv 16							//'ancestor'
    &pushsdbgm 18							//'_y'
    &subtract
    &setVariable
    &pushsdb 16							//'ancestor'
    &pushsdbgv 16							//'ancestor'
    &pushsdbgm 9							//'_parent'
    &setVariable
    &jmp label1    
   label2:
    &pushsdbgv 0							//'panel'
    &pushsdbgm 19							//'background'
    &pushsdb 17							//'_x'
    &pushsdbgv 11							//'backgroundX'
    &setMember
    &pushsdbgv 0							//'panel'
    &pushsdbgm 19							//'background'
    &pushsdb 18							//'_y'
    &pushsdbgv 14							//'backgroundY'
    &setMember
    &pushsdbgv 0							//'panel'
    &pushsdbgm 19							//'background'
    &pushsdb 20							//'_width'
    &pushsdbgv 12							//'_root'
    &pushsdbgm 21							//'displayWidth'
    &setMember
    &pushsdbgv 0							//'panel'
    &pushsdbgm 19							//'background'
    &pushsdb 22							//'_height'
    &pushsdbgv 12							//'_root'
    &pushsdbgm 23							//'displayHeight'
    &setMember
    &pushsdb 24							//'loopCount'
    &pushbyte 7
    &setVariable
    &pushsdb 7							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 25							//'FSCommand:'
    &pushsdb 26							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 27							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 28							//''
    &getURL2
   label3:
  &end // of frame 0

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 9 // total frames: 30

    &frame 0
      &pushsgv '_parent'
      &pushsgm 'factionLabel'
      &gotoAndPlay    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 9

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 20 // total frames: 30

    &frame 0
      &pushsgv '_parent'
      &pushsgm 'factionLabel'
      &gotoAndPlay    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 30

    &placeMovieClip 13 
    
      &onClipEvent &load
        &constants 'text', '$', 'Text', '_parent', 'qualifyName', '&Outline'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Text'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &pushsdb 5							//'&Outline'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 13

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

  &frame 36
    &constants 'loopCount'  
    &pushsdb 0							//'loopCount'
    &pushsdbgv 0							//'loopCount'
    &decrement
    &setRegister r:0
    &setVariable
    &pushregister 0
    &pushzero
    &greaterThan
    &not
    &jnz label1    
    &gotoLabel '_loop'
    &play
   label1:
  &end // of frame 36

  &frame 42
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
  &end // of frame 42
&end
