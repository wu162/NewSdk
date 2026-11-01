movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IGM_PL~1\\IGM_PL~1.eaf' &compressed // flash 7, total frames: 15, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'obj', 'fn', 'call', 'closing', 'dialog', 'rows', 'Pointer', 'encode', 'Math', 'round', 'ProgressMeter', 'gotoAndStop', '_', 'techPoints', 'gotoAndPlay', 'FSCommand:', 'OnConfirmClicked', 'qualifyName', '', 'extern', 'InGame', 'Close', 'OnCancelClicked', 'OnClosed', 'helpBox', 'BindFunction', 'GetPurchaseButton', 'SetProgressToNextRank', 'SetNumAvailablePoints', 'OnConfirm', 'OnCancel', 'GetHelpBox', 'confirmButton', 'OnClicked', 'cancelButton', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'HelpBox.swf', 'loadMovie', 'OnInitialized', '_visible', '_global', ':'  
    &function BindFunction ('fn', 'obj'    )    
      &function2  (r:1='buttonClip') ()
        &pushregister 1
        &pushsdbgv 0							//'obj'
        &pushbyte 2
        &pushsdbgv 1							//'fn'
        &dcallmp 2							// call()
      &end // of function 

      &return
    &end // of function BindFunction

    &function Close (    )    
      &pushsdbgv 3							//'closing'
      &not
      &not
      &jnz label1      
      &gotoLabel '_close'
      &play
      &pushsdb 3							//'closing'
      &pushtrue
      &setVariable
     label1:
    &end // of function Close

    &function2 GetPurchaseButton (r:1='row', r:2='col') ()
      &pushsdbgv 4							//'dialog'
      &pushsdbgm 5							//'rows'
      &pushregister 1
      &toString
      &getMember
      &pushregister 2
      &toString
      &getMember
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetPurchaseButton

    &function2 SetProgressToNextRank (r:2='progress') ()
      &pushregister 2
      &pushbyte 25
      &multiply
      &pushone
      &pushsdbgv 8							//'Math'
      &pushsdb 9							//'round'
      &callMethod
      &pushone
      &add
      &setRegister r:1
      &pop
      &pushregister 1
      &pushone
      &pushsdbgv 4							//'dialog'
      &pushsdbgm 10							//'ProgressMeter'
      &dcallmp 11							// gotoAndStop()
    &end // of function SetProgressToNextRank

    &function2 SetNumAvailablePoints (r:1='points') ()
      &pushsdb 12							//'_'
      &pushregister 1
      &toString
      &add
      &pushone
      &pushsdbgv 4							//'dialog'
      &pushsdbgm 13							//'techPoints'
      &dcallmp 14							// gotoAndPlay()
    &end // of function SetNumAvailablePoints

    &function2 OnConfirm () (r:1='this')
      &pushsdb 15							//'FSCommand:'
      &pushsdb 16							//'OnConfirmClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 17							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 18							//''
      &getURL2
      &pushsdbgv 19							//'extern'
      &pushsdbgm 20							//'InGame'
      &not
      &not
      &jnz label2      
      &pushzero
      &dcallfp 21							// Close()
     label2:
    &end // of function OnConfirm

    &function2 OnCancel () (r:1='this')
      &pushsdb 15							//'FSCommand:'
      &pushsdb 22							//'OnCancelClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 17							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 18							//''
      &getURL2
      &pushsdbgv 19							//'extern'
      &pushsdbgm 20							//'InGame'
      &not
      &not
      &jnz label3      
      &pushzero
      &dcallfp 21							// Close()
     label3:
    &end // of function OnCancel

    &function2 OnClosed () (r:1='this')
      &pushsdb 15							//'FSCommand:'
      &pushsdb 23							//'OnClosed'
      &pushregister 1
      &pushbyte 2
      &pushsdb 17							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 18							//''
      &getURL2
    &end // of function OnClosed

    &function GetHelpBox (    )    
      &pushsdbgv 24							//'helpBox'
      &pushone
      &pushsdbgv 6							//'Pointer'
      &pushsdb 7							//'encode'
      &callMethod
      &return
    &end // of function GetHelpBox

    &function onUnload (    )    
      &pushsdb 25							//'BindFunction'
      &delete2
      &pop
      &pushsdb 21							//'Close'
      &delete2
      &pop
      &pushsdb 26							//'GetPurchaseButton'
      &delete2
      &pop
      &pushsdb 27							//'SetProgressToNextRank'
      &delete2
      &pop
      &pushsdb 28							//'SetNumAvailablePoints'
      &delete2
      &pop
      &pushsdb 29							//'OnConfirm'
      &delete2
      &pop
      &pushsdb 30							//'OnCancel'
      &delete2
      &pop
      &pushsdb 23							//'OnClosed'
      &delete2
      &pop
      &pushsdb 31							//'GetHelpBox'
      &delete2
      &pop
      &pushsdbgv 4							//'dialog'
      &pushsdbgm 32							//'confirmButton'
      &pushsdb 33							//'OnClicked'
      &delete
      &pop
      &pushsdbgv 4							//'dialog'
      &pushsdbgm 34							//'cancelButton'
      &pushsdb 33							//'OnClicked'
      &delete
      &pop
      &pushsdb 35							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 36							//'initialized'
    &not
    &not
    &jnz label4    
    &pushthisgv
    &pushone
    &pushsdbgv 38							//'_parent'
    &dcallmp 39							// OnContentLoaded()
    &pushsdb 40							//'HelpBox.swf'
    &pushone
    &pushsdbgv 24							//'helpBox'
    &dcallmp 41							// loadMovie()
    &pushsdbgv 4							//'dialog'
    &pushsdbgm 32							//'confirmButton'
    &pushsdb 33							//'OnClicked'
    &pushthisgv
    &pushsdbgv 29							//'OnConfirm'
    &pushbyte 2
    &pushsdb 25							//'BindFunction'
    &callFunction
    &setMember
    &pushsdbgv 4							//'dialog'
    &pushsdbgm 34							//'cancelButton'
    &pushsdb 33							//'OnClicked'
    &pushthisgv
    &pushsdbgv 30							//'OnCancel'
    &pushbyte 2
    &pushsdb 25							//'BindFunction'
    &callFunction
    &setMember
    &pushsdb 36							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 15							//'FSCommand:'
    &pushsdb 42							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 17							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 18							//''
    &getURL2
   label4:
    &pushsdbgv 4							//'dialog'
    &pushsdb 43							//'_visible'
    &pushfalse
    &setMember
    &pushsdbgv 19							//'extern'
    &pushsdbgm 20							//'InGame'
    &not
    &not
    &jnz label5    
    &pushglobalgv
    &pushsdb 17							//'qualifyName'
    &function2  (r:1='clip', r:2='label') ()
      &pushregister 1
      &toString
      &pushsdb 45							//':'
      &add
      &pushregister 2
      &add
      &return
    &end // of function 

    &setMember
   label5:
  &end // of frame 0

  &defineMovieClip 7 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
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

      &pushsdbgv 3							//'factionSkinned'
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
      &pushsdb 3							//'factionSkinned'
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

  &defineButton 9
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &gotoLabel '_over'
      &play
      &pushthisgv
      &pushone
      &pushs 'OnClicked'
      &callfp
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 9

  &defineMovieClip 16 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
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

      &pushsdbgv 3							//'factionSkinned'
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
      &pushsdb 3							//'factionSkinned'
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
  &end // of defineMovieClip 16

  &defineMovieClip 21 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
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

      &pushsdbgv 3							//'factionSkinned'
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
      &pushsdb 3							//'factionSkinned'
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
  &end // of defineMovieClip 21

  &defineMovieClip 28 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
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

      &pushsdbgv 3							//'factionSkinned'
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
      &pushsdb 3							//'factionSkinned'
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
  &end // of defineMovieClip 28

  &defineMovieClip 35 // total frames: 30

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
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

      &pushsdbgv 3							//'factionSkinned'
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
      &pushsdb 3							//'factionSkinned'
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
  &end // of defineMovieClip 35

  &defineMovieClip 36 // total frames: 30

    &placeMovieClip 21 
    
      &onClipEvent &load
        &constants 'text', '$', '_parent', 'label', 'ButtonLabel', 'qualifyName'  
        &pushsdb 0							//'text'
        &pushsdb 1							//'$'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 3							//'label'
        &pushsdb 4							//'ButtonLabel'
        &add
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &pushsdb 5							//'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 21

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 36

  &defineMovieClip 44 // total frames: 29

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
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

      &pushsdbgv 3							//'factionSkinned'
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
      &pushsdb 3							//'factionSkinned'
      &pushtrue
      &setVariable
     label1:
    &end // of frame 0

    &frame 8
      &stop
    &end // of frame 8

    &frame 18
      &stop
    &end // of frame 18

    &frame 28
      &stop
    &end // of frame 28
  &end // of defineMovieClip 44

  &defineMovieClip 47 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 47

  &defineButton 49
  
    &on     &overUpToOverDown
      &pushzero
      &pushs 'OnPress'
      &callfp
    &end
  
    &on     &overDownToOverUp
      &pushzero
      &pushs 'OnRelease'
      &callfp
    &end
  
    &on     &idleToOverUp
      &pushzero
      &pushs 'OnRollOver'
      &callfp
    &end
  
    &on     &overUpToIdle
      &pushzero
      &pushs 'OnRollOut'
      &callfp
    &end
  &end // of defineButton 49
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'BlackRectangle' &as 50
  &end // of importAssets

  &defineMovieClip 55 // total frames: 20

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 55
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImage' &as 56
  &end // of importAssets

  &defineMovieClip 59 // total frames: 1
  &end // of defineMovieClip 59

  &defineMovieClip 60 // total frames: 2

    &placeMovieClip 56 
    
      &onClipEvent &construct
        &constants '_type', 'RenderImage', '_imageMap', '', '_mode'  
        &pushsdb 0							//'_type'
        &pushsdb 1							//'RenderImage'
        &setVariable
        &pushsdb 2							//'_imageMap'
        &pushsdb 3							//''
        &setVariable
        &pushsdb 4							//'_mode'
        &pushsdb 3							//''
        &setVariable
      &end
    
      &onClipEvent &load
        &constants '_imageMap', 'Image', '_parent', 'qualifyName'  
        &pushsdb 0							//'_imageMap'
        &pushsdb 1							//'Image'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &dcallfsv 3							// qualifyName()
      &end
    &end // of placeMovieClip 56

    &placeMovieClip 59 &as 'selectedOverlay'
    
      &onClipEvent &load
        &constants '', '_parent', 'selected'  
        &pushsdb 0							//''
        &pushbyte 7
        &pushsdbgv 1							//'_parent'
        &pushsdbgm 1							//'_parent'
        &pushsdbgm 2							//'selected'
        &setProperty
      &end
    &end // of placeMovieClip 59
  &end // of defineMovieClip 60
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'RenderImageDisabled' &as 61
  &end // of importAssets

  &defineMovieClip 63 // total frames: 2

    &placeMovieClip 61 
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImageDisabled'
        &pushs '_imageMap'
        &pushssv ''
      &end
    
      &onClipEvent &load
        &constants '_imageMap', 'Image', '_parent', 'qualifyName'  
        &pushsdb 0							//'_imageMap'
        &pushsdb 1							//'Image'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &dcallfsv 3							// qualifyName()
      &end
    &end // of placeMovieClip 61
  &end // of defineMovieClip 63

  &defineMovieClip 66 // total frames: 1
  &end // of defineMovieClip 66

  &defineMovieClip 67 // total frames: 2

    &placeMovieClip 56 
    
      &onClipEvent &construct
        &constants '_type', 'RenderImage', '_imageMap', '', '_mode'  
        &pushsdb 0							//'_type'
        &pushsdb 1							//'RenderImage'
        &setVariable
        &pushsdb 2							//'_imageMap'
        &pushsdb 3							//''
        &setVariable
        &pushsdb 4							//'_mode'
        &pushsdb 3							//''
        &setVariable
      &end
    
      &onClipEvent &load
        &constants '_imageMap', 'Image', '_parent', 'qualifyName'  
        &pushsdb 0							//'_imageMap'
        &pushsdb 1							//'Image'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &dcallfsv 3							// qualifyName()
      &end
    &end // of placeMovieClip 56
  &end // of defineMovieClip 67

  &defineMovieClip 68 // total frames: 30

    &frame 0
      &constants '', 'selected', 'content', 'selectedOverlay', '_visible', 'arrow', 'FSCommand:', 'OnClicked', 'qualifyName', 'OnRollOver', 'OnRollOut', 'helpSite', 'Pointer', 'encode', 'SetVisibility', 'SetAvailability', 'SetSelected', 'SetDependenceUponPriorRow', 'OnPress', 'OnRelease', 'GetHelpSite', 'onUnload', 'initialized', 'OnInitialized', 'this'  
      &function2 SetVisibility (r:1='vis') ()
        &pushsdb 0							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 SetAvailability (r:1='availability') ()
        &pushregister 1
        &gotoAndPlay      &end // of function SetAvailability

      &function2 SetSelected (r:1='selArg') ()
        &pushsdb 1							//'selected'
        &pushregister 1
        &pushzero
        &equals
        &not
        &setVariable
        &pushsdbgv 2							//'content'
        &pushsdbgm 3							//'selectedOverlay'
        &pushsdb 4							//'_visible'
        &pushsdbgv 1							//'selected'
        &setMember
      &end // of function SetSelected

      &function2 SetDependenceUponPriorRow (r:1='dependenceArg') ()
        &pushsdbgv 5							//'arrow'
        &pushsdb 4							//'_visible'
        &pushregister 1
        &pushzero
        &equals
        &not
        &setMember
      &end // of function SetDependenceUponPriorRow

      &function OnPress (      )      
      &end // of function OnPress

      &function2 OnRelease () (r:1='this')
        &pushsdb 6							//'FSCommand:'
        &pushsdb 7							//'OnClicked'
        &pushregister 1
        &pushbyte 2
        &pushsdb 8							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 0							//''
        &getURL2
      &end // of function OnRelease

      &function2 OnRollOver () (r:1='this')
        &pushsdb 6							//'FSCommand:'
        &pushsdb 9							//'OnRollOver'
        &pushregister 1
        &pushbyte 2
        &pushsdb 8							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 0							//''
        &getURL2
      &end // of function OnRollOver

      &function2 OnRollOut () (r:1='this')
        &pushsdb 6							//'FSCommand:'
        &pushsdb 10							//'OnRollOut'
        &pushregister 1
        &pushbyte 2
        &pushsdb 8							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 0							//''
        &getURL2
      &end // of function OnRollOut

      &function2 GetHelpSite () (r:1='_parent')
        &pushregister 1
        &pushsdbgm 11							//'helpSite'
        &pushone
        &pushsdbgv 12							//'Pointer'
        &pushsdb 13							//'encode'
        &callMethod
        &return
      &end // of function GetHelpSite

      &function onUnload (      )      
        &pushsdb 14							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 15							//'SetAvailability'
        &delete2
        &pop
        &pushsdb 16							//'SetSelected'
        &delete2
        &pop
        &pushsdb 17							//'SetDependenceUponPriorRow'
        &delete2
        &pop
        &pushsdb 18							//'OnPress'
        &delete2
        &pop
        &pushsdb 19							//'OnRelease'
        &delete2
        &pop
        &pushsdb 9							//'OnRollOver'
        &delete2
        &pop
        &pushsdb 10							//'OnRollOut'
        &delete2
        &pop
        &pushsdb 20							//'GetHelpSite'
        &delete2
        &pop
        &pushsdb 21							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 22							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdbgv 5							//'arrow'
      &pushsdb 4							//'_visible'
      &pushfalse
      &setMember
      &pushsdb 1							//'selected'
      &pushfalse
      &setVariable
      &pushsdb 22							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 6							//'FSCommand:'
      &pushsdb 23							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
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
  &end // of defineMovieClip 68
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 69
  &end // of importAssets

  &defineMovieClip 70 // total frames: 1

    &placeMovieClip 69 &as 'helpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'left'
      &end
    &end // of placeMovieClip 69
  &end // of defineMovieClip 70

  &defineMovieClip 71 // total frames: 1
  &end // of defineMovieClip 71

  &defineMovieClip 120 // total frames: 26

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

    &frame 10
      &stop
    &end // of frame 10

    &frame 11
      &stop
    &end // of frame 11

    &frame 12
      &stop
    &end // of frame 12

    &frame 13
      &stop
    &end // of frame 13

    &frame 14
      &stop
    &end // of frame 14

    &frame 15
      &stop
    &end // of frame 15

    &frame 16
      &stop
    &end // of frame 16

    &frame 17
      &stop
    &end // of frame 17

    &frame 18
      &stop
    &end // of frame 18

    &frame 19
      &stop
    &end // of frame 19

    &frame 20
      &stop
    &end // of frame 20

    &frame 21
      &stop
    &end // of frame 21

    &frame 22
      &stop
    &end // of frame 22

    &frame 23
      &stop
    &end // of frame 23

    &frame 24
      &stop
    &end // of frame 24

    &frame 25
      &stop
    &end // of frame 25
  &end // of defineMovieClip 120

  &defineMovieClip 143 // total frames: 55

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19

    &frame 24
      &stop
    &end // of frame 24

    &frame 29
      &stop
    &end // of frame 29

    &frame 34
      &stop
    &end // of frame 34

    &frame 39
      &stop
    &end // of frame 39

    &frame 44
      &stop
    &end // of frame 44

    &frame 49
      &stop
    &end // of frame 49

    &frame 54
      &stop
    &end // of frame 54
  &end // of defineMovieClip 143

  &defineMovieClip 144 // total frames: 1

    &placeMovieClip 36 &as 'cancelButton'
    
      &onClipEvent &construct
        &pushs 'label'
        &pushssv 'Cancel'
      &end
    &end // of placeMovieClip 36

    &placeMovieClip 36 &as 'confirmButton'
    
      &onClipEvent &construct
        &pushs 'label'
        &pushssv 'Confirm'
      &end
    &end // of placeMovieClip 36

    &placeMovieClip 143 &as 'techPoints'
    
      &onClipEvent &load
        &constants '_', 'NumAvailablePoints', '_parent', 'qualifyName'  
        &pushsdb 0							//'_'
        &pushsdb 1							//'NumAvailablePoints'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &pushsdb 3							//'qualifyName'
        &callFunction
        &add
        &gotoAndStop      &end
    &end // of placeMovieClip 143
  &end // of defineMovieClip 144

  &defineMovieClip 145 // total frames: 1
  &end // of defineMovieClip 145

  &frame 3
    &pushsgv 'dialog'
    &pushs '_visible'
    &pushtrue
    &setMember
  &end // of frame 3

  &frame 11
    &stop
  &end // of frame 11

  &frame 14
    &stop
    &pushzero
    &pushs 'OnClosed'
    &callfp
  &end // of frame 14
&end
