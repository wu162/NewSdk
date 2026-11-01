movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\POPUPF~1\\POPUPF~1.eaf' &compressed // flash 7, total frames: 30, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
    &function2 OnCurrentFactionChanged (r:1='faction') ()
      &pushregister 1
      &gotoAndPlay    &end // of function OnCurrentFactionChanged

    &function2 onUnload () (r:1='this', r:2='_root')
      &pushregister 1
      &pushone
      &pushregister 2
      &dcallmp 0							// DetachFactionObserver()
      &pushsdb 1							//'OnCurrentFactionChanged'
      &delete2
      &pop
      &pushsdb 2							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 3							//'initialized'
    &not
    &not
    &jnz label1    
    &pushzero
    &pushsdbgv 4							//'_root'
    &pushsdb 5							//'GetCurrentFaction'
    &callMethod
    &gotoAndPlay    &pushthisgv
    &pushone
    &pushsdbgv 4							//'_root'
    &dcallmp 7							// AttachFactionObserver()
    &pushsdb 3							//'initialized'
    &pushtrue
    &setVariable
   label1:
  &end // of frame 0

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 10 // total frames: 54

    &frame 53
      &stop
    &end // of frame 53
  &end // of defineMovieClip 10

  &frame 9
    &stop
  &end // of frame 9

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 17 // total frames: 50

    &frame 49
      &stop
    &end // of frame 49
  &end // of defineMovieClip 17

  &frame 19
    &stop
  &end // of frame 19

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 24 // total frames: 40

    &frame 39
      &stop
    &end // of frame 39
  &end // of defineMovieClip 24

  &frame 29
    &stop
  &end // of frame 29
&end
