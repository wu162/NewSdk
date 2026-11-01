movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA4E64~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 16, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'closing', 'Close', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function Close (    )    
      &pushsdbgv 0							//'closing'
      &not
      &not
      &jnz label1      
      &pushsdb 0							//'closing'
      &pushtrue
      &setVariable
      &gotoLabel '_close'
      &play
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

  &defineButton 4
  
    &on     &overUpToOverDown
      &constants 'FSCommand:', 'OnButtonClicked', '_parent', 'qualifyName', '', 'OnCloseClicked'  
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnButtonClicked'
      &pushsdbgv 2							//'_parent'
      &pushbyte 2
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
      &pushsdb 0							//'FSCommand:'
      &pushsdb 5							//'OnCloseClicked'
      &pushsdbgv 2							//'_parent'
      &pushbyte 2
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnButtonRollOver'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &pushs 'FSCommand:'
      &pushs 'OnButtonRollOut'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 4

  &defineButton 5
  
    &on     &overUpToOverDown
      &constants 'FSCommand:', 'OnButtonClicked', '_parent', 'qualifyName', '', 'OnSendClicked'  
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnButtonClicked'
      &pushsdbgv 2							//'_parent'
      &pushbyte 2
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
      &pushsdb 0							//'FSCommand:'
      &pushsdb 5							//'OnSendClicked'
      &pushsdbgv 2							//'_parent'
      &pushbyte 2
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
    &end
  
    &on     &idleToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnButtonRollOver'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  
    &on     &overUpToIdle
      &pushs 'FSCommand:'
      &pushs 'OnButtonRollOut'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushs ''
      &getURL2
    &end
  &end // of defineButton 5

  &defineButton 6
  
    &on     &overUpToOverDown
      &pushs 'FSCommand:'
      &pushs 'OnPress'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushzero
      &pushs 'GetMousePosString'
      &callFunction
      &getURL2
    &end
  
    &on     &overDownToOverUp
      &pushs 'FSCommand:'
      &pushs 'OnRelease'
      &pushsgv '_parent'
      &pushbyte 2
      &pushs 'qualifyName'
      &callFunction
      &concat
      &pushzero
      &pushs 'GetMousePosString'
      &callFunction
      &getURL2
    &end
  
    &on     &idleToOverUp
      &pushs 'mouseOver'
      &pushtrue
      &setVariable
    &end
  
    &on     &overUpToIdle
      &pushs 'mouseOver'
      &pushfalse
      &setVariable
    &end
  &end // of defineButton 6

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 10 // total frames: 1

    &frame 0
      &constants '', '_width', '_height', 'x=', '&y=', 'mouseOver', 'FSCommand:', 'OnMouseMove', 'qualifyName', 'GetMousePosString', 'onMouseMove', 'onUnload'  
      &function2 GetMousePosString () (r:1='_parent')
        &pushsdb 0							//''
        &pushbyte 20
        &getProperty
        &pushregister 1
        &pushsdbgm 1							//'_width'
        &pushbyte 2
        &divide
        &add
        &setRegister r:3
        &pop
        &pushsdb 0							//''
        &pushbyte 21
        &getProperty
        &pushregister 1
        &pushsdbgm 2							//'_height'
        &pushbyte 2
        &divide
        &add
        &setRegister r:2
        &pop
        &pushsdb 3							//'x='
        &pushregister 3
        &toString
        &add
        &pushsdb 4							//'&y='
        &add
        &pushregister 2
        &toString
        &add
        &return
      &end // of function GetMousePosString

      &function2 onMouseMove () (r:1='_parent')
        &pushsdbgv 5							//'mouseOver'
        &not
        &jnz label1        
        &pushsdb 6							//'FSCommand:'
        &pushsdb 7							//'OnMouseMove'
        &pushregister 1
        &pushbyte 2
        &pushsdb 8							//'qualifyName'
        &callFunction
        &concat
        &pushzero
        &pushsdb 9							//'GetMousePosString'
        &callFunction
        &getURL2
       label1:
      &end // of function onMouseMove

      &function onUnload (      )      
        &pushsdb 10							//'onMouseMove'
        &delete2
        &pop
        &pushsdb 11							//'onUnload'
        &delete2
        &pop
        &pushsdb 9							//'GetMousePosString'
        &delete2
        &pop
      &end // of function onUnload

    &end // of frame 0

    &placeMovieClip 9 &as 'messageText'
    
      &onClipEvent &load
        &constants 'text', '$', 'ChatMessage', '_parent', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdb 2							//'ChatMessage'
        &pushsdbgv 3							//'_parent'
        &pushsdbgm 3							//'_parent'
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 9
  &end // of defineMovieClip 10

  &frame 9
    &stop
  &end // of frame 9

  &frame 15
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
  &end // of frame 15
&end
