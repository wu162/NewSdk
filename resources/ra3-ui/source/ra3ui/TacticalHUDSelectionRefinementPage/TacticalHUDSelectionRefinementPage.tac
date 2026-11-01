movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TAF0D8~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'commandList', 'Pointer', 'encode', 'FSCommand:', 'OnTabRollOver', 'qualifyName', '', 'OnTabRollOut', 'tabHelpSite', 'buttonSlotWidth', 'numColumns', 'numVisibleRows', 'GetCommandListBox', 'GetTabHelpSite', 'OnContentLoaded', 'onUnload', 'initialized', 'this', '_parent', 'OnChildLoaded', 'TacticalHUDCommandListBox.swf', 'loadMovie', 'OnInitialized'  
    &function GetCommandListBox (    )    
      &pushsdbgv 0							//'commandList'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetCommandListBox

    &function2 OnTabRollOver () (r:1='this')
      &pushsdb 3							//'FSCommand:'
      &pushsdb 4							//'OnTabRollOver'
      &pushregister 1
      &pushbyte 2
      &pushsdb 5							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 6							//''
      &getURL2
    &end // of function OnTabRollOver

    &function2 OnTabRollOut () (r:1='this')
      &pushsdb 3							//'FSCommand:'
      &pushsdb 7							//'OnTabRollOut'
      &pushregister 1
      &pushbyte 2
      &pushsdb 5							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 6							//''
      &getURL2
    &end // of function OnTabRollOut

    &function GetTabHelpSite (    )    
      &pushsdbgv 8							//'tabHelpSite'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetTabHelpSite

    &function2 OnContentLoaded (r:1='contentClip') ()
      &pushregister 1
      &pushsdbgv 0							//'commandList'
      &equals
      &not
      &jnz label1      
      &pushsdbgv 0							//'commandList'
      &pushsdb 9							//'buttonSlotWidth'
      &pushbyte 52
      &setMember
      &pushsdbgv 0							//'commandList'
      &pushsdb 10							//'numColumns'
      &pushbyte 4
      &setMember
      &pushsdbgv 0							//'commandList'
      &pushsdb 11							//'numVisibleRows'
      &pushbyte 5
      &setMember
     label1:
    &end // of function OnContentLoaded

    &function onUnload (    )    
      &pushsdb 12							//'GetCommandListBox'
      &delete2
      &pop
      &pushsdb 4							//'OnTabRollOver'
      &delete2
      &pop
      &pushsdb 7							//'OnTabRollOut'
      &delete2
      &pop
      &pushsdb 13							//'GetTabHelpSite'
      &delete2
      &pop
      &pushsdb 14							//'OnContentLoaded'
      &delete2
      &pop
      &pushsdb 15							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 16							//'initialized'
    &not
    &not
    &jnz label2    
    &pushthisgv
    &pushone
    &pushsdbgv 18							//'_parent'
    &dcallmp 19							// OnChildLoaded()
    &pushsdb 20							//'TacticalHUDCommandListBox.swf'
    &pushone
    &pushsdbgv 0							//'commandList'
    &dcallmp 21							// loadMovie()
    &pushsdb 16							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 3							//'FSCommand:'
    &pushsdb 22							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 5							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 6							//''
    &getURL2
   label2:
    &stop
  &end // of frame 0

  &defineMovieClip 1 // total frames: 1
  &end // of defineMovieClip 1
&end
