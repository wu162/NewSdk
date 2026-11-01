movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\YESNOD~1\\YESNOD~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'closing', '_close', 'popup', 'gotoAndPlay', 'Close', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', '_root', 'displayLeft', 'backdropX', 'backdropY', 'displayTop', 'backdropWidth', 'displayWidth', 'backdropHeight', 'displayHeight', 'clipPath', 'Array', 'clip', 'push', 'length', 'pop', '_x', '_y', 'invScaleX', '_xscale', 'invScaleY', '_yscale', 'backdrop', '_width', '_height', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function Close (    )    
      &pushsdbgv 0							//'closing'
      &not
      &not
      &jnz label1      
      &pushsdb 0							//'closing'
      &pushtrue
      &setVariable
      &pushsdb 1							//'_close'
      &pushone
      &pushsdbgv 2							//'popup'
      &dcallmp 3							// gotoAndPlay()
     label1:
    &end // of function Close

    &function onUnload (    )    
      &pushsdb 4							//'Close'
      &delete2
      &pop
      &pushsdb 5							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 6							//'initialized'
    &not
    &not
    &jnz label6    
    &pushthisgv
    &pushone
    &pushsdbgv 8							//'_parent'
    &dcallmp 9							// OnContentLoaded()
    &pushsdb 0							//'closing'
    &pushfalse
    &setVariable
    &pushsdbgv 10							//'_root'
    &pushsdbgm 11							//'displayLeft'
    &pushundef
    &equals
    &not
    &not
    &jnz label5    
    &pushsdb 12							//'backdropX'
    &pushsdbgv 10							//'_root'
    &pushsdbgm 11							//'displayLeft'
    &varEquals
    &pushsdb 13							//'backdropY'
    &pushsdbgv 10							//'_root'
    &pushsdbgm 14							//'displayTop'
    &varEquals
    &pushsdb 15							//'backdropWidth'
    &pushsdbgv 10							//'_root'
    &pushsdbgm 16							//'displayWidth'
    &varEquals
    &pushsdb 17							//'backdropHeight'
    &pushsdbgv 10							//'_root'
    &pushsdbgm 18							//'displayHeight'
    &varEquals
    &pushsdb 19							//'clipPath'
    &pushzero
    &pushsdb 20							//'Array'
    &new
    &varEquals
    &pushsdb 21							//'clip'
    &pushthisgv
    &setVariable
   label2:
    &pushsdbgv 21							//'clip'
    &pushsdbgv 10							//'_root'
    &equals
    &not
    &not
    &jnz label3    
    &pushsdbgv 21							//'clip'
    &pushone
    &pushsdbgv 19							//'clipPath'
    &dcallmp 22							// push()
    &pushsdb 21							//'clip'
    &pushsdbgv 21							//'clip'
    &pushsdbgm 8							//'_parent'
    &setVariable
    &jmp label2    
   label3:
    &pushsdbgv 19							//'clipPath'
    &pushsdbgm 23							//'length'
    &pushzero
    &greaterThan
    &not
    &jnz label4    
    &pushsdb 21							//'clip'
    &pushzero
    &pushsdbgv 19							//'clipPath'
    &pushsdb 24							//'pop'
    &callMethod
    &varEquals
    &pushsdb 12							//'backdropX'
    &pushsdbgv 12							//'backdropX'
    &pushsdbgv 21							//'clip'
    &pushsdbgm 25							//'_x'
    &subtract
    &setVariable
    &pushsdb 13							//'backdropY'
    &pushsdbgv 13							//'backdropY'
    &pushsdbgv 21							//'clip'
    &pushsdbgm 26							//'_y'
    &subtract
    &setVariable
    &pushsdb 27							//'invScaleX'
    &pushbyte 100
    &pushsdbgv 21							//'clip'
    &pushsdbgm 28							//'_xscale'
    &divide
    &varEquals
    &pushsdb 29							//'invScaleY'
    &pushbyte 100
    &pushsdbgv 21							//'clip'
    &pushsdbgm 30							//'_yscale'
    &divide
    &varEquals
    &pushsdb 12							//'backdropX'
    &pushsdbgv 12							//'backdropX'
    &pushsdbgv 27							//'invScaleX'
    &multiply
    &setVariable
    &pushsdb 13							//'backdropY'
    &pushsdbgv 13							//'backdropY'
    &pushsdbgv 29							//'invScaleY'
    &multiply
    &setVariable
    &pushsdb 15							//'backdropWidth'
    &pushsdbgv 15							//'backdropWidth'
    &pushsdbgv 27							//'invScaleX'
    &multiply
    &setVariable
    &pushsdb 17							//'backdropHeight'
    &pushsdbgv 17							//'backdropHeight'
    &pushsdbgv 29							//'invScaleY'
    &multiply
    &setVariable
    &jmp label3    
   label4:
    &pushsdbgv 31							//'backdrop'
    &pushsdb 25							//'_x'
    &pushsdbgv 12							//'backdropX'
    &setMember
    &pushsdbgv 31							//'backdrop'
    &pushsdb 26							//'_y'
    &pushsdbgv 13							//'backdropY'
    &setMember
    &pushsdbgv 31							//'backdrop'
    &pushsdb 32							//'_width'
    &pushsdbgv 15							//'backdropWidth'
    &setMember
    &pushsdbgv 31							//'backdrop'
    &pushsdb 33							//'_height'
    &pushsdbgv 17							//'backdropHeight'
    &setMember
   label5:
    &pushsdb 6							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 34							//'FSCommand:'
    &pushsdb 35							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 36							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 37							//''
    &getURL2
   label6:
  &end // of frame 0

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 5 // total frames: 20

    &frame 0
      &stop
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 5

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineMovieClip 16 // total frames: 8

    &frame 7
      &stop
    &end // of frame 7
  &end // of defineMovieClip 16

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 25 // total frames: 1
  &end // of defineMovieClip 25

  &defineButton 27
  
    &on     &idleToOverUp
      &constants 'FSCommand:', 'OnButtonRollOver', '_parent', 'qualifyName', ''  
      &gotoLabel '_over'
      &play
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnButtonRollOver'
      &pushsdbgv 2							//'_parent'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 2							//'_parent'
      &pushbyte 3
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
    &end
  
    &on     &overDownToOverUp
      &constants 'FSCommand:', 'OnButtonClicked', '_parent', 'qualifyName', '', 'this', 'OnRelease', 'child', '_exit', 'gotoAndPlay'  
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnButtonClicked'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 2							//'_parent'
      &pushsdbgm 2							//'_parent'
      &pushbyte 2
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
      &pushthisgv
      &pushone
      &pushsdbgv 2							//'_parent'
      &dcallmp 6							// OnRelease()
      &gotoLabel '_down'
      &play
      &pushsdbgv 2							//'_parent'
      &enumerateValue
     label1:
      &setRegister r:0
      &pushnull
      &equals
      &jnz label3      
      &pushsdb 7							//'child'
      &pushregister 0
      &setVariable
      &pushsdb 4							//''
      &pushbyte 13
      &getProperty
      &pushsdbgv 7							//'child'
      &equals
      &not
      &not
      &jnz label2      
      &pushsdb 8							//'_exit'
      &pushone
      &pushsdbgv 2							//'_parent'
      &pushsdbgv 7							//'child'
      &getMember
      &dcallmp 9							// gotoAndPlay()
     label2:
      &jmp label1      
     label3:
    &end
  
    &on     &overUpToIdle    ,&overDownToOutDown    ,&overDownToIdle
      &constants 'FSCommand:', 'OnButtonRollOut', '_parent', 'qualifyName', ''  
      &gotoLabel '_rollout'
      &play
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnButtonRollOut'
      &pushsdbgv 2							//'_parent'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 2							//'_parent'
      &pushbyte 3
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
    &end
  &end // of defineButton 27

  &defineMovieClip 33 // total frames: 1
  &end // of defineMovieClip 33

  &defineMovieClip 34 // total frames: 55
  &end // of defineMovieClip 34

  &defineMovieClip 66 // total frames: 52
  &end // of defineMovieClip 66

  &defineMovieClip 69 // total frames: 1
  &end // of defineMovieClip 69

  &defineMovieClip 72 // total frames: 1
  &end // of defineMovieClip 72

  &defineMovieClip 76 // total frames: 1
  &end // of defineMovieClip 76

  &defineMovieClip 79 // total frames: 1
  &end // of defineMovieClip 79

  &defineMovieClip 81 // total frames: 1
  &end // of defineMovieClip 81

  &defineMovieClip 82 // total frames: 1
  &end // of defineMovieClip 82

  &defineMovieClip 83 // total frames: 48

    &frame 8
      &stop
    &end // of frame 8

    &frame 18
      &stop
    &end // of frame 18

    &frame 28
      &stop
    &end // of frame 28

    &frame 36
      &gotoLabel '_up'
      &play
    &end // of frame 36

    &frame 44
      &stop
    &end // of frame 44
  &end // of defineMovieClip 83

  &defineMovieClip 84 // total frames: 6

    &frame 0
      &constants 'Yes', 'FSCommand:', 'OnYesClicked', '_parent', 'qualifyName', '', 'No', 'OnNoClicked', '_close', 'gotoAndPlay', 'OnRelease', 'onUnload'  
      &function2 OnRelease (r:2='buttonClip') (r:1='_parent')
        &pushregister 2
        &pushsdbgv 0							//'Yes'
        &equals
        &not
        &jnz label1        
        &pushsdb 1							//'FSCommand:'
        &pushsdb 2							//'OnYesClicked'
        &pushregister 1
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 5							//''
        &getURL2
        &jmp label2        
       label1:
        &pushregister 2
        &pushsdbgv 6							//'No'
        &equals
        &not
        &jnz label2        
        &pushsdb 1							//'FSCommand:'
        &pushsdb 7							//'OnNoClicked'
        &pushregister 1
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 5							//''
        &getURL2
       label2:
        &pushsdb 8							//'_close'
        &pushone
        &pushregister 1
        &dcallmp 9							// gotoAndPlay()
      &end // of function OnRelease

      &function onUnload (      )      
        &pushsdb 10							//'OnRelease'
        &delete2
        &pop
        &pushsdb 11							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

    &end // of frame 0

    &placeMovieClip 83 &as 'Yes'
    
      &onClipEvent &load
        &constants 'this', 'buttonName', '$', 'YesButtonLabel', '_parent', 'qualifyName'  
        &pushthisgv
        &pushsdb 1							//'buttonName'
        &pushsdb 2							//'$'
        &pushsdb 3							//'YesButtonLabel'
        &pushsdbgv 4							//'_parent'
        &pushsdbgm 4							//'_parent'
        &pushsdbgm 4							//'_parent'
        &pushbyte 2
        &pushsdb 5							//'qualifyName'
        &callFunction
        &add
        &setMember
      &end
    &end // of placeMovieClip 83

    &placeMovieClip 83 &as 'No'
    
      &onClipEvent &load
        &constants 'this', 'buttonName', '$', 'NoButtonLabel', '_parent', 'qualifyName'  
        &pushthisgv
        &pushsdb 1							//'buttonName'
        &pushsdb 2							//'$'
        &pushsdb 3							//'NoButtonLabel'
        &pushsdbgv 4							//'_parent'
        &pushsdbgm 4							//'_parent'
        &pushsdbgm 4							//'_parent'
        &pushbyte 2
        &pushsdb 5							//'qualifyName'
        &callFunction
        &add
        &setMember
      &end
    &end // of placeMovieClip 83

    &frame 5
      &stop
    &end // of frame 5
  &end // of defineMovieClip 84

  &defineMovieClip 86 // total frames: 1
  &end // of defineMovieClip 86

  &defineMovieClip 88 // total frames: 1
  &end // of defineMovieClip 88

  &defineMovieClip 89 // total frames: 25

    &frame 1
      &pushs '_open'
      &pushone
      &pushsgv '_parent'
      &pushsgm 'backdrop'
      &pushs 'gotoAndPlay'
      &callmp
    &end // of frame 1

    &frame 7
      &constants 'txtTitle', 'stringName', '$', 'Title', '_parent', 'qualifyName', 'txtMessage', 'Prompt'  
      &pushsdbgv 0							//'txtTitle'
      &pushsdb 1							//'stringName'
      &pushsdb 2							//'$'
      &pushsdb 3							//'Title'
      &pushsdbgv 4							//'_parent'
      &pushbyte 2
      &pushsdb 5							//'qualifyName'
      &callFunction
      &add
      &setMember
      &pushsdbgv 6							//'txtMessage'
      &pushsdb 1							//'stringName'
      &pushsdb 2							//'$'
      &pushsdb 7							//'Prompt'
      &pushsdbgv 4							//'_parent'
      &pushbyte 2
      &pushsdb 5							//'qualifyName'
      &callFunction
      &add
      &setMember
    &end // of frame 7

    &frame 12
      &stop
    &end // of frame 12

    &frame 13
      &pushs '_close'
      &pushone
      &pushsgv '_parent'
      &pushsgm 'backdrop'
      &pushs 'gotoAndPlay'
      &callmp
    &end // of frame 13

    &frame 24
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
    &end // of frame 24
  &end // of defineMovieClip 89
&end
