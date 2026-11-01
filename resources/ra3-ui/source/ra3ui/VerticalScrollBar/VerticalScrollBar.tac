movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\VERTIC~1\\VERTIC~1.eaf' &compressed // flash 7, total frames: 2, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 7 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 7
  
  &exportAssets
    7 &as 'VSliderUpButtonPressDynFac'
  &end // of exportAssets

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as 'VSliderUpButtonPressGdi'
  &end // of exportAssets

  &defineMovieClip 15 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 15
  
  &exportAssets
    15 &as 'VSliderDownButtonPressDynFac'
  &end // of exportAssets

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16
  
  &exportAssets
    16 &as 'VSliderDownButtonPressGdi'
  &end // of exportAssets

  &defineMovieClip 23 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 23
  
  &exportAssets
    23 &as 'VSliderDownButtonEnabledDynFac'
  &end // of exportAssets

  &defineMovieClip 24 // total frames: 1
  &end // of defineMovieClip 24
  
  &exportAssets
    24 &as 'VSliderDownButtonEnabledGdi'
  &end // of exportAssets

  &defineMovieClip 31 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 31
  
  &exportAssets
    31 &as 'VSliderUpButtonEnabledDynFac'
  &end // of exportAssets

  &defineMovieClip 32 // total frames: 1
  &end // of defineMovieClip 32
  
  &exportAssets
    32 &as 'VSliderUpButtonEnabledGdi'
  &end // of exportAssets

  &defineMovieClip 39 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 39
  
  &exportAssets
    39 &as 'VSliderDownButtonOverDynFac'
  &end // of exportAssets

  &defineMovieClip 40 // total frames: 1
  &end // of defineMovieClip 40
  
  &exportAssets
    40 &as 'VSliderDownButtonOverGdi'
  &end // of exportAssets

  &defineMovieClip 47 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 47
  
  &exportAssets
    47 &as 'VSliderUpButtonOverDynFac'
  &end // of exportAssets

  &defineMovieClip 48 // total frames: 1
  &end // of defineMovieClip 48
  
  &exportAssets
    48 &as 'VSliderUpButtonOverGdi'
  &end // of exportAssets

  &defineMovieClip 55 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 55
  
  &exportAssets
    55 &as 'VSliderDownButtonDisabledDynFac'
  &end // of exportAssets

  &defineMovieClip 56 // total frames: 1
  &end // of defineMovieClip 56
  
  &exportAssets
    56 &as 'VSliderDownButtonDisabledGdi'
  &end // of exportAssets

  &defineMovieClip 63 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 63
  
  &exportAssets
    63 &as 'VSliderUpButtonDisabledDynFac'
  &end // of exportAssets

  &defineMovieClip 64 // total frames: 1
  &end // of defineMovieClip 64
  
  &exportAssets
    64 &as 'VSliderUpButtonDisabledGdi'
  &end // of exportAssets

  &defineMovieClip 71 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 71
  
  &exportAssets
    71 &as 'VSliderThumbOverTopDynFac'
  &end // of exportAssets

  &defineMovieClip 72 // total frames: 1
  &end // of defineMovieClip 72
  
  &exportAssets
    72 &as 'VSliderThumbOverTopGdi'
  &end // of exportAssets

  &defineMovieClip 79 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 79
  
  &exportAssets
    79 &as 'VSliderThumbOverBottomDynFac'
  &end // of exportAssets

  &defineMovieClip 80 // total frames: 1
  &end // of defineMovieClip 80
  
  &exportAssets
    80 &as 'VSliderThumbOverBottomGdi'
  &end // of exportAssets

  &defineMovieClip 87 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 87
  
  &exportAssets
    87 &as 'VSliderThumbOverMiddleDynFac'
  &end // of exportAssets

  &defineMovieClip 88 // total frames: 1
  &end // of defineMovieClip 88
  
  &exportAssets
    88 &as 'VSliderThumbOverMiddleGdi'
  &end // of exportAssets

  &defineMovieClip 95 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 95
  
  &exportAssets
    95 &as 'VSliderThumbEnabledTopDynFac'
  &end // of exportAssets

  &defineMovieClip 96 // total frames: 1
  &end // of defineMovieClip 96
  
  &exportAssets
    96 &as 'VSliderThumbEnabledTopGdi'
  &end // of exportAssets

  &defineMovieClip 103 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 103
  
  &exportAssets
    103 &as 'VSliderThumbEnabledMiddleDynFac'
  &end // of exportAssets

  &defineMovieClip 104 // total frames: 1
  &end // of defineMovieClip 104
  
  &exportAssets
    104 &as 'VSliderThumbEnabledMiddleGdi'
  &end // of exportAssets

  &defineMovieClip 111 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 111
  
  &exportAssets
    111 &as 'VSliderThumbEnabledBottomDynFac'
  &end // of exportAssets

  &defineMovieClip 112 // total frames: 1
  &end // of defineMovieClip 112
  
  &exportAssets
    112 &as 'VSliderThumbEnabledBottomGdi'
  &end // of exportAssets

  &defineMovieClip 119 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 119
  
  &exportAssets
    119 &as 'VSliderThumbDisabledTopDynFac'
  &end // of exportAssets

  &defineMovieClip 120 // total frames: 1
  &end // of defineMovieClip 120
  
  &exportAssets
    120 &as 'VSliderThumbDisabledTopGdi'
  &end // of exportAssets

  &defineMovieClip 127 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 127
  
  &exportAssets
    127 &as 'VSliderThumbDisabledMiddleDynFac'
  &end // of exportAssets

  &defineMovieClip 128 // total frames: 1
  &end // of defineMovieClip 128
  
  &exportAssets
    128 &as 'VSliderThumbDisabledMiddleGdi'
  &end // of exportAssets

  &defineMovieClip 135 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 135
  
  &exportAssets
    135 &as 'VSliderThumbDisabledBottomDynFac'
  &end // of exportAssets

  &defineMovieClip 136 // total frames: 1
  &end // of defineMovieClip 136
  
  &exportAssets
    136 &as 'VSliderThumbDisabledBottomGdi'
  &end // of exportAssets

  &defineMovieClip 143 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 143
  
  &exportAssets
    143 &as 'VSliderThumbPressTopDynFac'
  &end // of exportAssets

  &defineMovieClip 144 // total frames: 1
  &end // of defineMovieClip 144
  
  &exportAssets
    144 &as 'VSliderThumbPressTopGdi'
  &end // of exportAssets

  &defineMovieClip 151 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 151
  
  &exportAssets
    151 &as 'VSliderThumbPressMiddleDynFac'
  &end // of exportAssets

  &defineMovieClip 152 // total frames: 1
  &end // of defineMovieClip 152
  
  &exportAssets
    152 &as 'VSliderThumbPressMiddleGdi'
  &end // of exportAssets

  &defineMovieClip 159 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 159
  
  &exportAssets
    159 &as 'VSliderThumbPressBottomDynFac'
  &end // of exportAssets

  &defineMovieClip 160 // total frames: 1
  &end // of defineMovieClip 160
  
  &exportAssets
    160 &as 'VSliderThumbPressBottomGdi'
  &end // of exportAssets

  &defineMovieClip 162 // total frames: 1
  &end // of defineMovieClip 162

  &defineMovieClip 164 // total frames: 1
  &end // of defineMovieClip 164

  &defineMovieClip 165 // total frames: 1
  &end // of defineMovieClip 165
  
  &exportAssets
    165 &as 'VSliderBarUpOverGdi'
  &end // of exportAssets

  &defineMovieClip 166 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'initialized', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

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
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'initialized'
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
  &end // of defineMovieClip 166
  
  &exportAssets
    166 &as 'VSliderBarDisabledDynFac'
  &end // of exportAssets

  &defineMovieClip 167 // total frames: 1
  &end // of defineMovieClip 167
  
  &exportAssets
    167 &as 'VSliderBarDisabledGdi'
  &end // of exportAssets

  &frame 0
    &constants 'FSCommand:', 'OnPosChanged', 'qualifyName', 'thumb', '_y', 'bar', '_height', 'pageSize', 'GetCurPos', 'upButton', 'upButtonOffset', 'downButton', 'downButtonOffset', 'barTopMargin', 'barBottomMargin', 'SetHeight', '', 'NotifyPosChanged', 'newPageSize', 'lineSize', '_disabled', '_up', 'gotoAndPlay', 'OnThumbDrag', 'ThumbUp', 'ThumbDown', 'Object', 'x', 'y', 'localToGlobal', '_ymouse', 'SetPageSize', 'SetLineSize', 'SetEnabled', 'SetVisible', 'SetPos', 'OnScrollButtonPress', 'OnSliderBarClicked', 'onUnload', 'param', 'this', '_parent', 'GetScrollBarParam'  
    &function2 OnPosChanged (r:2='pos') (r:1='this')
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnPosChanged'
      &pushregister 1
      &pushbyte 2
      &pushsdb 2							//'qualifyName'
      &callFunction
      &concat
      &pushregister 2
      &getURL2
    &end // of function OnPosChanged

    &function GetCurPos (    )    
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &subtract
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &subtract
      &pushsdbgv 7							//'pageSize'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &multiply
      &add
      &divide
      &return
    &end // of function GetCurPos

    &function2 NotifyPosChanged () ()
      &pushzero
      &pushsdb 8							//'GetCurPos'
      &callFunction
      &setRegister r:1
      &pop
      &pushregister 1
      &pushone
      &dcallfp 1							// OnPosChanged()
    &end // of function NotifyPosChanged

    &function2 SetHeight (r:1='newHeight') ()
      &pushregister 1
      &pushbyte 73
      &lessThan
      &not
      &jnz label1      
      &pushbyte 73
      &setRegister r:1
      &pop
     label1:
      &pushzero
      &pushsdb 8							//'GetCurPos'
      &callFunction
      &setRegister r:4
      &pop
      &pushsdbgv 9							//'upButton'
      &pushsdb 4							//'_y'
      &pushsdbgv 10							//'upButtonOffset'
      &setMember
      &pushsdbgv 11							//'downButton'
      &pushsdb 4							//'_y'
      &pushregister 1
      &pushsdbgv 12							//'downButtonOffset'
      &subtract
      &setMember
      &pushsdbgv 5							//'bar'
      &pushsdb 4							//'_y'
      &pushsdbgv 13							//'barTopMargin'
      &setMember
      &pushsdbgv 5							//'bar'
      &pushsdb 6							//'_height'
      &pushregister 1
      &pushsdbgv 13							//'barTopMargin'
      &pushsdbgv 14							//'barBottomMargin'
      &add
      &subtract
      &setMember
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &pushsdbgv 7							//'pageSize'
      &multiply
      &pushone
      &pushsdbgv 3							//'thumb'
      &dcallmp 15							// SetHeight()
      &pushfalse
      &setRegister r:3
      &pop
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushregister 4
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &multiply
      &add
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &greaterThan
      &not
      &jnz label2      
      &pushtrue
      &setRegister r:3
      &pop
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &subtract
      &setRegister r:2
      &pop
     label2:
      &pushsdbgv 3							//'thumb'
      &pushsdb 4							//'_y'
      &pushregister 2
      &setMember
      &pushsdb 16							//''
      &pushbyte 9
      &pushregister 1
      &setProperty
      &pushregister 3
      &not
      &jnz label3      
      &pushzero
      &dcallfp 17							// NotifyPosChanged()
     label3:
    &end // of function SetHeight

    &function2 SetPageSize (r:1='newPageSize') ()
      &pushregister 1
      &pushzero
      &lessThan
      &not
      &jnz label4      
      &pushzero
      &setRegister r:1
      &pop
      &jmp label5      
     label4:
      &pushregister 1
      &pushone
      &greaterThan
      &not
      &jnz label5      
      &pushone
      &setRegister r:1
      &pop
     label5:
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &pushregister 1
      &multiply
      &pushone
      &pushsdbgv 3							//'thumb'
      &dcallmp 15							// SetHeight()
      &pushsdb 7							//'pageSize'
      &pushregister 1
      &setVariable
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &greaterThan
      &not
      &jnz label6      
      &pushsdbgv 3							//'thumb'
      &pushsdb 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &subtract
      &setMember
      &pushzero
      &dcallfp 17							// NotifyPosChanged()
     label6:
    &end // of function SetPageSize

    &function2 SetLineSize (r:1='newLineSize') ()
      &pushregister 1
      &pushzero
      &lessThan
      &not
      &jnz label7      
      &pushzero
      &setRegister r:1
      &pop
      &jmp label8      
     label7:
      &pushsdbgv 18							//'newPageSize'
      &pushone
      &greaterThan
      &not
      &jnz label8      
      &pushone
      &setRegister r:1
      &pop
     label8:
      &pushsdb 19							//'lineSize'
      &pushregister 1
      &setVariable
    &end // of function SetLineSize

    &function2 SetEnabled (r:2='enable') ()
      &pushregister 2
      &jnz label9      
      &pushsdb 20							//'_disabled'
      &jmp label10      
     label9:
      &pushsdb 21							//'_up'
     label10:
      &setRegister r:1
      &pop
      &pushregister 1
      &pushone
      &pushsdbgv 5							//'bar'
      &dcallmp 22							// gotoAndPlay()
      &pushregister 1
      &pushone
      &pushsdbgv 3							//'thumb'
      &dcallmp 22							// gotoAndPlay()
      &pushregister 1
      &pushone
      &pushsdbgv 9							//'upButton'
      &dcallmp 22							// gotoAndPlay()
      &pushregister 1
      &pushone
      &pushsdbgv 11							//'downButton'
      &dcallmp 22							// gotoAndPlay()
    &end // of function SetEnabled

    &function2 SetVisible (r:1='vis') ()
      &pushsdb 16							//''
      &pushbyte 7
      &pushregister 1
      &setProperty
    &end // of function SetVisible

    &function2 OnThumbDrag ('clip', r:1='newY') ()
      &pushregister 1
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &lessThan
      &not
      &jnz label11      
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &setRegister r:1
      &pop
      &jmp label12      
     label11:
      &pushregister 1
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &greaterThan
      &not
      &jnz label12      
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &subtract
      &setRegister r:1
      &pop
     label12:
      &pushregister 1
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 4							//'_y'
      &equals
      &not
      &not
      &jnz label13      
      &pushsdbgv 3							//'thumb'
      &pushsdb 4							//'_y'
      &pushregister 1
      &setMember
      &pushzero
      &dcallfp 17							// NotifyPosChanged()
     label13:
    &end // of function OnThumbDrag

    &function2 SetPos (r:3='newPos') (r:1='this')
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushregister 3
      &toNumber
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &pushsdbgv 7							//'pageSize'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &multiply
      &subtract
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &add
      &multiply
      &add
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 1
      &pushbyte 2
      &dcallfp 23							// OnThumbDrag()
    &end // of function SetPos

    &function2 ThumbUp (r:2='size') ()
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 4							//'_y'
      &pushregister 2
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &multiply
      &subtract
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &lessThan
      &not
      &jnz label14      
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &setRegister r:1
      &pop
     label14:
      &pushregister 1
      &return
    &end // of function ThumbUp

    &function2 ThumbDown (r:2='size') ()
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 4							//'_y'
      &pushregister 2
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &multiply
      &add
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &greaterThan
      &not
      &jnz label15      
      &pushsdbgv 5							//'bar'
      &pushsdbgm 4							//'_y'
      &pushsdbgv 5							//'bar'
      &pushsdbgm 6							//'_height'
      &add
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 6							//'_height'
      &subtract
      &setRegister r:1
      &pop
     label15:
      &pushregister 1
      &return
    &end // of function ThumbDown

    &function2 OnScrollButtonPress (r:2='scrollButton') ()
      &pushundef
      &setRegister r:1
      &pop
      &pushregister 2
      &pushsdbgv 9							//'upButton'
      &equals
      &not
      &jnz label16      
      &pushsdbgv 19							//'lineSize'
      &pushone
      &pushsdb 24							//'ThumbUp'
      &callFunction
      &setRegister r:1
      &pop
      &jmp label17      
     label16:
      &pushsdbgv 19							//'lineSize'
      &pushone
      &pushsdb 25							//'ThumbDown'
      &callFunction
      &setRegister r:1
      &pop
     label17:
      &pushregister 1
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 4							//'_y'
      &equals
      &not
      &not
      &jnz label18      
      &pushsdbgv 3							//'thumb'
      &pushsdb 4							//'_y'
      &pushregister 1
      &setMember
      &pushzero
      &dcallfp 17							// NotifyPosChanged()
     label18:
    &end // of function OnScrollButtonPress

    &function2 OnSliderBarClicked ('clip') (r:1='_root')
      &pushzero
      &pushsdb 26							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 27							//'x'
      &pushsdb 16							//''
      &pushzero
      &getProperty
      &pushsdb 16							//''
      &pushzero
      &getProperty
      &subtract
      &setMember
      &pushregister 2
      &pushsdb 28							//'y'
      &pushsdb 16							//''
      &pushone
      &getProperty
      &pushsdb 16							//''
      &pushone
      &getProperty
      &subtract
      &setMember
      &pushregister 2
      &pushone
      &pushsdbgv 3							//'thumb'
      &dcallmp 29							// localToGlobal()
      &pushundef
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdbgm 30							//'_ymouse'
      &pushregister 2
      &pushsdbgm 28							//'y'
      &greaterThan
      &not
      &not
      &jnz label19      
      &pushsdbgv 7							//'pageSize'
      &pushone
      &pushsdb 24							//'ThumbUp'
      &callFunction
      &setRegister r:3
      &pop
      &jmp label20      
     label19:
      &pushsdbgv 7							//'pageSize'
      &pushone
      &pushsdb 25							//'ThumbDown'
      &callFunction
      &setRegister r:3
      &pop
     label20:
      &pushregister 3
      &pushsdbgv 3							//'thumb'
      &pushsdbgm 4							//'_y'
      &equals
      &not
      &not
      &jnz label21      
      &pushsdbgv 3							//'thumb'
      &pushsdb 4							//'_y'
      &pushregister 3
      &setMember
      &pushzero
      &dcallfp 17							// NotifyPosChanged()
     label21:
    &end // of function OnSliderBarClicked

    &function onUnload (    )    
      &pushsdb 1							//'OnPosChanged'
      &delete2
      &pop
      &pushsdb 8							//'GetCurPos'
      &delete2
      &pop
      &pushsdb 17							//'NotifyPosChanged'
      &delete2
      &pop
      &pushsdb 15							//'SetHeight'
      &delete2
      &pop
      &pushsdb 31							//'SetPageSize'
      &delete2
      &pop
      &pushsdb 32							//'SetLineSize'
      &delete2
      &pop
      &pushsdb 33							//'SetEnabled'
      &delete2
      &pop
      &pushsdb 34							//'SetVisible'
      &delete2
      &pop
      &pushsdb 23							//'OnThumbDrag'
      &delete2
      &pop
      &pushsdb 35							//'SetPos'
      &delete2
      &pop
      &pushsdb 24							//'ThumbUp'
      &delete2
      &pop
      &pushsdb 25							//'ThumbDown'
      &delete2
      &pop
      &pushsdb 36							//'OnScrollButtonPress'
      &delete2
      &pop
      &pushsdb 37							//'OnSliderBarClicked'
      &delete2
      &pop
      &pushsdb 38							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdb 39							//'param'
    &pushthisgv
    &pushone
    &pushsdbgv 41							//'_parent'
    &dcallmsv 42							// GetScrollBarParam()
  &end // of frame 0

  &defineMovieClip 168 // total frames: 1

    &frame 0
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end // of frame 0
  &end // of defineMovieClip 168

  &defineMovieClip 169 // total frames: 1

    &placeMovieClip 168 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 168
  &end // of defineMovieClip 169

  &defineButton 171
  
    &on     &overUpToOverDown
      &pushzero
      &pushsgv '_parent'
      &pushs 'HandlePress'
      &callmp
    &end
  
    &on     &overDownToOverUp
      &pushzero
      &pushsgv '_parent'
      &pushs 'HandleRelease'
      &callmp
    &end
  
    &on     &outDownToIdle
      &constants '_parent', 'HandleRelease', 'HandleRollOut'  
      &pushzero
      &pushsdbgv 0							//'_parent'
      &dcallmp 1							// HandleRelease()
      &pushzero
      &pushsdbgv 0							//'_parent'
      &dcallmp 2							// HandleRollOut()
    &end
  
    &on     &idleToOverUp
      &pushzero
      &pushsgv '_parent'
      &pushs 'HandleRollOver'
      &callmp
    &end
  
    &on     &overUpToIdle
      &pushzero
      &pushsgv '_parent'
      &pushs 'HandleRollOut'
      &callmp
    &end
  &end // of defineButton 171

  &defineMovieClip 172 // total frames: 1
  &end // of defineMovieClip 172

  &defineMovieClip 173 // total frames: 1

    &placeMovieClip 172 &as 'dummy'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 172
  &end // of defineMovieClip 173

  &defineMovieClip 174 // total frames: 30

    &frame 0
      &constants 'frame', '_height', 'background', 'bttnHeight', 'bttn', 'OnSliderBarClicked', 'SetHeight', 'HandlePress', 'HandleRelease', 'HandleRollOver', 'HandleRollOut', 'onUnload', 'initialized'  
      &function2 SetHeight (r:1='newHeight') ()
        &pushsdbgv 0							//'frame'
        &pushsdb 1							//'_height'
        &pushregister 1
        &setMember
        &pushsdbgv 2							//'background'
        &pushsdb 1							//'_height'
        &pushregister 1
        &setMember
        &pushsdb 3							//'bttnHeight'
        &pushregister 1
        &setVariable
        &pushsdbgv 4							//'bttn'
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushsdbgv 4							//'bttn'
        &pushsdb 1							//'_height'
        &pushsdbgv 3							//'bttnHeight'
        &setMember
       label1:
      &end // of function SetHeight

      &function HandlePress (      )      
      &end // of function HandlePress

      &function2 HandleRelease () (r:1='this', r:2='_parent')
        &pushregister 1
        &pushone
        &pushregister 2
        &dcallmp 5							// OnSliderBarClicked()
      &end // of function HandleRelease

      &function HandleRollOver (      )      
        &gotoLabel '_over'
        &play
      &end // of function HandleRollOver

      &function HandleRollOut (      )      
        &gotoLabel '_up'
        &play
      &end // of function HandleRollOut

      &function onUnload (      )      
        &pushsdb 6							//'SetHeight'
        &delete2
        &pop
        &pushsdb 7							//'HandlePress'
        &delete2
        &pop
        &pushsdb 8							//'HandleRelease'
        &delete2
        &pop
        &pushsdb 9							//'HandleRollOver'
        &delete2
        &pop
        &pushsdb 10							//'HandleRollOut'
        &delete2
        &pop
        &pushsdb 11							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 12							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 12							//'initialized'
      &pushtrue
      &setVariable
     label2:
    &end // of frame 0

    &frame 0
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'disabledImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'upImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 10

    &placeMovieClip 173 &as 'bttn'
    
      &onClipEvent &load
        &constants '_parent', 'bttnHeight', ''  
        &pushsdbgv 0							//'_parent'
        &pushsdbgm 1							//'bttnHeight'
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushsdb 2							//''
        &pushbyte 9
        &pushsdbgv 0							//'_parent'
        &pushsdbgm 1							//'bttnHeight'
        &setProperty
       label1:
      &end
    &end // of placeMovieClip 173

    &frame 19
      &stop
    &end // of frame 19

    &frame 20
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'overImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 20

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 174

  &placeMovieClip 174 &as 'bar'
  
    &onClipEvent &load
      &constants '', 'param', '_parent', 'bar'  
      &pushsdb 0							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 1							//'param'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 1							//'param'
      &pushsdbgm 3							//'bar'
      &setVariable
    &end
  &end // of placeMovieClip 174

  &defineMovieClip 175 // total frames: 1
  &end // of defineMovieClip 175

  &defineMovieClip 176 // total frames: 40

    &frame 0
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'disabledImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'upImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 10

    &frame 19
      &stop
    &end // of frame 19

    &frame 20
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'overImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 20

    &frame 29
      &stop
    &end // of frame 29

    &frame 30
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'downImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 30

    &frame 39
      &stop
    &end // of frame 39
  &end // of defineMovieClip 176

  &defineMovieClip 177 // total frames: 1
  &end // of defineMovieClip 177

  &defineMovieClip 178 // total frames: 40

    &frame 0
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'disabledImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'upImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 10

    &frame 19
      &stop
    &end // of frame 19

    &frame 20
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'overImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 20

    &frame 29
      &stop
    &end // of frame 29

    &frame 30
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'downImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 30

    &frame 39
      &stop
    &end // of frame 39
  &end // of defineMovieClip 178

  &defineMovieClip 179 // total frames: 40

    &frame 0
      &constants 'topPiece', '_height', 'bottomPiece', 'middlePiece', '_y', 'bttnHeight', 'bttn', 'ourMouseListener', 'dragStartY', '', 'dragStartYMouse', '_ymouse', 'createEmptyMovieClip', 'onMouseMove', '_parent', 'HandleMouseMove', 'onMouseUp', 'removeMovieClip', 'OnThumbDrag', 'SetHeight', 'HandlePress', 'HandleRelease', 'HandleRollOver', 'HandleRollOut', 'onUnload'  
      &function2 SetHeight (r:2='newHeight') ()
        &pushsdbgv 0							//'topPiece'
        &pushsdbgm 1							//'_height'
        &pushsdbgv 2							//'bottomPiece'
        &pushsdbgm 1							//'_height'
        &add
        &setRegister r:1
        &pop
        &pushregister 2
        &pushregister 1
        &lessThan
        &not
        &jnz label1        
        &pushregister 1
        &setRegister r:2
        &pop
       label1:
        &pushsdbgv 3							//'middlePiece'
        &pushsdb 1							//'_height'
        &pushregister 2
        &pushregister 1
        &subtract
        &setMember
        &pushsdbgv 2							//'bottomPiece'
        &pushsdb 4							//'_y'
        &pushsdbgv 3							//'middlePiece'
        &pushsdbgm 4							//'_y'
        &pushsdbgv 3							//'middlePiece'
        &pushsdbgm 1							//'_height'
        &add
        &setMember
        &pushsdb 5							//'bttnHeight'
        &pushregister 2
        &setVariable
        &pushsdbgv 6							//'bttn'
        &pushundef
        &equals
        &not
        &not
        &jnz label2        
        &pushsdbgv 6							//'bttn'
        &pushsdb 1							//'_height'
        &pushsdbgv 5							//'bttnHeight'
        &setMember
       label2:
      &end // of function SetHeight

      &function2 HandlePress () (r:1='this', r:2='_root')
        &gotoLabel '_down'
        &play
        &pushsdbgv 7							//'ourMouseListener'
        &pushundef
        &equals
        &not
        &jnz label3        
        &pushsdb 8							//'dragStartY'
        &pushsdb 9							//''
        &pushone
        &getProperty
        &setVariable
        &pushsdb 10							//'dragStartYMouse'
        &pushregister 2
        &pushsdbgm 11							//'_ymouse'
        &setVariable
        &pushzero
        &pushsdb 7							//'ourMouseListener'
        &pushbyte 2
        &pushregister 1
        &dcallmp 12							// createEmptyMovieClip()
        &pushsdbgv 7							//'ourMouseListener'
        &pushsdb 13							//'onMouseMove'
        &function2  () (r:1='this')
          &pushzero
          &pushregister 1
          &pushsdbgm 14							//'_parent'
          &dcallmp 15							// HandleMouseMove()
        &end // of function 

        &setMember
        &pushsdbgv 7							//'ourMouseListener'
        &pushsdb 16							//'onMouseUp'
        &function2  () (r:1='this')
          &pushregister 1
          &pushsdbgm 14							//'_parent'
          &pushsdb 7							//'ourMouseListener'
          &delete
          &pop
          &pushzero
          &pushregister 1
          &dcallmp 17							// removeMovieClip()
        &end // of function 

        &setMember
       label3:
      &end // of function HandlePress

      &function HandleRelease (      )      
        &gotoLabel '_over'
        &play
      &end // of function HandleRelease

      &function HandleRollOver (      )      
        &gotoLabel '_over'
        &play
      &end // of function HandleRollOver

      &function HandleRollOut (      )      
        &gotoLabel '_up'
        &play
      &end // of function HandleRollOut

      &function2 HandleMouseMove () (r:1='this', r:2='_root', r:3='_parent')
        &pushsdbgv 8							//'dragStartY'
        &pushregister 2
        &pushsdbgm 11							//'_ymouse'
        &pushsdbgv 10							//'dragStartYMouse'
        &subtract
        &add
        &pushregister 1
        &pushbyte 2
        &pushregister 3
        &dcallmp 18							// OnThumbDrag()
      &end // of function HandleMouseMove

      &function onUnload (      )      
        &pushsdbgv 7							//'ourMouseListener'
        &pushundef
        &equals
        &not
        &not
        &jnz label4        
        &pushzero
        &pushsdbgv 7							//'ourMouseListener'
        &dcallmp 17							// removeMovieClip()
        &pushsdb 7							//'ourMouseListener'
        &delete2
        &pop
       label4:
        &pushsdb 19							//'SetHeight'
        &delete2
        &pop
        &pushsdb 20							//'HandlePress'
        &delete2
        &pop
        &pushsdb 21							//'HandleRelease'
        &delete2
        &pop
        &pushsdb 22							//'HandleRollOver'
        &delete2
        &pop
        &pushsdb 23							//'HandleRollOut'
        &delete2
        &pop
        &pushsdb 15							//'HandleMouseMove'
        &delete2
        &pop
        &pushsdb 24							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

    &end // of frame 0

    &frame 0
      &constants '_disabled', 'topPiece', 'gotoAndPlay', 'middlePiece', 'bottomPiece'  
      &pushsdb 0							//'_disabled'
      &pushone
      &pushsdbgv 1							//'topPiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_disabled'
      &pushone
      &pushsdbgv 3							//'middlePiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_disabled'
      &pushone
      &pushsdbgv 4							//'bottomPiece'
      &dcallmp 2							// gotoAndPlay()
    &end // of frame 0

    &placeMovieClip 176 &as 'middlePiece'
    
      &onClipEvent &load
        &constants 'param', '_parent', 'middlePiece'  
        &pushsdb 0							//'param'
        &pushsdbgv 1							//'_parent'
        &pushsdbgm 0							//'param'
        &pushsdbgm 2							//'middlePiece'
        &setVariable
      &end
    &end // of placeMovieClip 176

    &placeMovieClip 178 &as 'bottomPiece'
    
      &onClipEvent &load
        &constants 'param', '_parent', 'bottomPiece'  
        &pushsdb 0							//'param'
        &pushsdbgv 1							//'_parent'
        &pushsdbgm 0							//'param'
        &pushsdbgm 2							//'bottomPiece'
        &setVariable
      &end
    &end // of placeMovieClip 178

    &placeMovieClip 178 &as 'topPiece'
    
      &onClipEvent &load
        &constants 'param', '_parent', 'topPiece'  
        &pushsdb 0							//'param'
        &pushsdbgv 1							//'_parent'
        &pushsdbgm 0							//'param'
        &pushsdbgm 2							//'topPiece'
        &setVariable
      &end
    &end // of placeMovieClip 178

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &constants '_up', 'topPiece', 'gotoAndPlay', 'middlePiece', 'bottomPiece'  
      &pushsdb 0							//'_up'
      &pushone
      &pushsdbgv 1							//'topPiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_up'
      &pushone
      &pushsdbgv 3							//'middlePiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_up'
      &pushone
      &pushsdbgv 4							//'bottomPiece'
      &dcallmp 2							// gotoAndPlay()
    &end // of frame 10

    &placeMovieClip 173 &as 'bttn'
    
      &onClipEvent &load
        &constants '_parent', 'bttnHeight', ''  
        &pushsdbgv 0							//'_parent'
        &pushsdbgm 1							//'bttnHeight'
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushsdb 2							//''
        &pushbyte 9
        &pushsdbgv 0							//'_parent'
        &pushsdbgm 1							//'bttnHeight'
        &setProperty
       label1:
      &end
    &end // of placeMovieClip 173

    &frame 19
      &stop
    &end // of frame 19

    &frame 20
      &constants '_over', 'topPiece', 'gotoAndPlay', 'middlePiece', 'bottomPiece'  
      &pushsdb 0							//'_over'
      &pushone
      &pushsdbgv 1							//'topPiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_over'
      &pushone
      &pushsdbgv 3							//'middlePiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_over'
      &pushone
      &pushsdbgv 4							//'bottomPiece'
      &dcallmp 2							// gotoAndPlay()
    &end // of frame 20

    &frame 29
      &stop
    &end // of frame 29

    &frame 30
      &constants '_down', 'topPiece', 'gotoAndPlay', 'middlePiece', 'bottomPiece'  
      &pushsdb 0							//'_down'
      &pushone
      &pushsdbgv 1							//'topPiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_down'
      &pushone
      &pushsdbgv 3							//'middlePiece'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 0							//'_down'
      &pushone
      &pushsdbgv 4							//'bottomPiece'
      &dcallmp 2							// gotoAndPlay()
    &end // of frame 30

    &frame 39
      &stop
    &end // of frame 39
  &end // of defineMovieClip 179

  &placeMovieClip 179 &as 'thumb'
  
    &onClipEvent &load
      &constants '', 'param', '_parent', 'thumb'  
      &pushsdb 0							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 1							//'param'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 1							//'param'
      &pushsdbgm 3							//'thumb'
      &setVariable
    &end
  &end // of placeMovieClip 179

  &defineMovieClip 180 // total frames: 1
  &end // of defineMovieClip 180

  &defineButton 181
  
    &on     &overUpToOverDown
      &pushthisgv
      &pushone
      &pushsgv '_parent'
      &pushs 'OnScrollButtonPress'
      &callmp
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 181

  &defineMovieClip 182 // total frames: 40

    &frame 0
      &constants 'initialized'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label1:
    &end // of frame 0

    &frame 0
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'disabledImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'upImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 10

    &frame 19
      &stop
    &end // of frame 19

    &frame 20
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'overImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 20

    &frame 29
      &stop
    &end // of frame 29

    &frame 30
      &pushzero
      &pushs 'Image'
      &pushsgv 'param'
      &pushsgm 'downImage'
      &pushbyte 3
      &pushsgv 'ImageLoader'
      &pushs 'attachMovie'
      &callmp
    &end // of frame 30

    &frame 39
      &stop
    &end // of frame 39
  &end // of defineMovieClip 182

  &placeMovieClip 182 &as 'upButton'
  
    &onClipEvent &load
      &constants '', 'param', '_parent', 'upButton'  
      &pushsdb 0							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 1							//'param'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 1							//'param'
      &pushsdbgm 3							//'upButton'
      &setVariable
    &end
  &end // of placeMovieClip 182

  &placeMovieClip 182 &as 'downButton'
  
    &onClipEvent &load
      &constants '', 'param', '_parent', 'downButton'  
      &pushsdb 0							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 1							//'param'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 1							//'param'
      &pushsdbgm 3							//'downButton'
      &setVariable
    &end
  &end // of placeMovieClip 182

  &frame 1
    &constants 'initialized', 'upButton', '_visible', 'downButton', 'thumb', 'bar', 'pageSize', 'lineSize', 'upButtonOffset', '_y', 'downButtonOffset', '', 'barTopMargin', 'barBottomMargin', '_height', 'this', '_parent', 'OnScrollBarLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName'  
    &pushsdbgv 0							//'initialized'
    &not
    &not
    &jnz label1    
    &pushsdbgv 1							//'upButton'
    &pushsdb 2							//'_visible'
    &pushtrue
    &setMember
    &pushsdbgv 3							//'downButton'
    &pushsdb 2							//'_visible'
    &pushtrue
    &setMember
    &pushsdbgv 4							//'thumb'
    &pushsdb 2							//'_visible'
    &pushtrue
    &setMember
    &pushsdbgv 5							//'bar'
    &pushsdb 2							//'_visible'
    &pushtrue
    &setMember
    &pushsdb 6							//'pageSize'
    &pushzerosv
    &pushsdb 7							//'lineSize'
    &pushzerosv
    &pushsdb 8							//'upButtonOffset'
    &pushsdbgv 1							//'upButton'
    &pushsdbgm 9							//'_y'
    &setVariable
    &pushsdb 10							//'downButtonOffset'
    &pushsdb 11							//''
    &pushbyte 9
    &getProperty
    &pushsdbgv 3							//'downButton'
    &pushsdbgm 9							//'_y'
    &subtract
    &setVariable
    &pushsdb 12							//'barTopMargin'
    &pushsdbgv 5							//'bar'
    &pushsdbgm 9							//'_y'
    &setVariable
    &pushsdb 13							//'barBottomMargin'
    &pushsdb 11							//''
    &pushbyte 9
    &getProperty
    &pushsdbgv 5							//'bar'
    &pushsdbgm 9							//'_y'
    &pushsdbgv 5							//'bar'
    &pushsdbgm 14							//'_height'
    &add
    &subtract
    &setVariable
    &pushthisgv
    &pushone
    &pushsdbgv 16							//'_parent'
    &dcallmp 17							// OnScrollBarLoaded()
    &pushsdb 0							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 18							//'FSCommand:'
    &pushsdb 19							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 20							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 11							//''
    &getURL2
   label1:
    &stop
  &end // of frame 1
&end
