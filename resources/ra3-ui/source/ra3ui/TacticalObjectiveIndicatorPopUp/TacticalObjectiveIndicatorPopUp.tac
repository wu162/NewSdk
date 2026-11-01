movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TAC739~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 9, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'Close', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function Close (    )    
      &gotoLabel '_close'
      &play
    &end // of function Close

    &function onUnload (    )    
      &pushsdb 0							//'Close'
      &delete2
      &pop
      &pushsdb 1							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 2							//'initialized'
    &not
    &not
    &jnz label1    
    &pushthisgv
    &pushone
    &pushsdbgv 4							//'_parent'
    &dcallmp 5							// OnContentLoaded()
    &pushsdb 2							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 6							//'FSCommand:'
    &pushsdb 7							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 8							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 9							//''
    &getURL2
   label1:
  &end // of frame 0

  &defineButton 2
  
    &on     &overDownToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnClicked'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 2

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 22 // total frames: 1
  &end // of defineMovieClip 22

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 26 // total frames: 1
  &end // of defineMovieClip 26

  &defineMovieClip 28 // total frames: 1
  &end // of defineMovieClip 28

  &defineMovieClip 29 // total frames: 1

    &placeMovieClip 26 
    
      &onClipEvent &load
        &constants 'myText', '$', 'Description', '_parent', 'qualifyName', '&outline'  
        &pushsdb 0							//'myText'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Description'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &pushsdb 5							//'&outline'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 26

    &placeMovieClip 28 
    
      &onClipEvent &load
        &constants 'myText', '$', 'Title', '_parent', 'qualifyName', '&outline'  
        &pushsdb 0							//'myText'
        &pushsdb 1							//'$'
        &pushsdb 2							//'Title'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &pushsdb 5							//'&outline'
        &add
        &setVariable
      &end
    &end // of placeMovieClip 28
  &end // of defineMovieClip 29

  &frame 4
    &stop
  &end // of frame 4

  &frame 8
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
  &end // of frame 8
&end
