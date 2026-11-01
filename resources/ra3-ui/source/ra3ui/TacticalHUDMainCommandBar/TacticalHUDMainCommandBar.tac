movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TA143F~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'sel', 'Pointer', 'encode', 'pages', 'length', 'clip', '_name', 'tabs', 'SelectPage', 'SetTabFlashing', 'SetTabHighlighted', 'visiblePageNum', '_visible', 'FSCommand:', 'OnSelectionRefinementPageSelected', 'qualifyName', '', 'OnBuildQueueTypePageSelected', 'OnSelectionRefinementTabClicked', 'OnBuildQueueTypeTabClicked', 'OnTabRollOver', 'OnTabRollOut', 'tabsInitialized', 'SelectTab', 'selectedTabNum', 'tabFlashing', 'tabHighlighted', 'GetSelectedTabNum', 'tabButton', 'tabIcon', 'tabHelpSite', 'GetSelectionRefinementPage', 'GetBuildQueueTypePage', 'GetBuildQueueTypeTab', 'SelectSelectionRefinementPage', 'SelectBuildQueueTypePage', 'SetBuildQueueTypeTabFlashing', 'SetSelectionRefinementTabHighlighted', 'SetBuildQueueTypeTabHighlighted', 'OnTabSelected', 'OnTabsInitialized', 'OnChildLoaded', 'onUnload', 'initialized', 'navy', 'Nav', 'aircraft', 'Air', 'vehicles', 'Veh', 'infantry', 'Inf', 'otherStructure', 'Def', 'mainStructure', 'Str', 'Con', 'TacticalHUDSelectionRefinementPage.swf', 'loadMovie', 'TacticalHUDBuildQueuePage.swf', 'OnInitialized', 'this'  
    &function GetSelectionRefinementPage (    )    
      &pushsdbgv 0							//'sel'
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetSelectionRefinementPage

    &function2 GetBuildQueueTypePage (r:2='pageName') (r:1='this')
      &pushregister 1
      &pushregister 2
      &getMember
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
    &end // of function GetBuildQueueTypePage

    &function2 GetBuildQueueTypeTab (r:3='pageNameArg') ()
      &pushregister 3
      &toString
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:1
      &pop
     label1:
      &pushregister 1
      &pushsdbgv 3							//'pages'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'clip'
      &pushsdbgm 6							//'_name'
      &pushregister 2
      &equals
      &not
      &jnz label2      
      &pushsdbgv 7							//'tabs'
      &pushregister 1
      &toString
      &getMember
      &pushone
      &pushsdbgv 1							//'Pointer'
      &pushsdb 2							//'encode'
      &callMethod
      &return
     label2:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label1      
     label3:
      &pushundef
      &return
    &end // of function GetBuildQueueTypeTab

    &function SelectSelectionRefinementPage (    )    
      &pushsdbgv 0							//'sel'
      &pushone
      &dcallfp 8							// SelectPage()
    &end // of function SelectSelectionRefinementPage

    &function2 SelectBuildQueueTypePage (r:2='pageName') (r:1='this')
      &pushregister 1
      &pushregister 2
      &getMember
      &pushone
      &dcallfp 8							// SelectPage()
    &end // of function SelectBuildQueueTypePage

    &function2 SetBuildQueueTypeTabFlashing (r:3='pageName', r:2='flashingArg') (r:1='this')
      &pushregister 2
      &pushzero
      &equals
      &not
      &pushregister 1
      &pushregister 3
      &getMember
      &pushbyte 2
      &dcallfp 9							// SetTabFlashing()
    &end // of function SetBuildQueueTypeTabFlashing

    &function2 SetSelectionRefinementTabHighlighted (r:1='highlightedArg') ()
      &pushregister 1
      &pushzero
      &equals
      &not
      &pushsdbgv 0							//'sel'
      &pushbyte 2
      &dcallfp 10							// SetTabHighlighted()
    &end // of function SetSelectionRefinementTabHighlighted

    &function2 SetBuildQueueTypeTabHighlighted (r:3='pageName', r:2='highlightedArg') (r:1='this')
      &pushregister 2
      &pushzero
      &equals
      &not
      &pushregister 1
      &pushregister 3
      &getMember
      &pushbyte 2
      &dcallfp 10							// SetTabHighlighted()
    &end // of function SetBuildQueueTypeTabHighlighted

    &function2 OnTabSelected (r:3='tabNum') (r:1='this')
      &pushregister 3
      &pushsdbgv 11							//'visiblePageNum'
      &equals
      &not
      &not
      &jnz label5      
      &pushsdbgv 3							//'pages'
      &pushsdbgv 11							//'visiblePageNum'
      &getMember
      &pushsdbgm 5							//'clip'
      &pushsdb 12							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 3							//'pages'
      &pushregister 3
      &getMember
      &pushsdbgm 5							//'clip'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 12							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 11							//'visiblePageNum'
      &pushregister 3
      &setVariable
      &pushregister 2
      &pushsdbgv 0							//'sel'
      &equals
      &not
      &jnz label4      
      &pushsdb 13							//'FSCommand:'
      &pushsdb 14							//'OnSelectionRefinementPageSelected'
      &pushregister 1
      &pushbyte 2
      &pushsdb 15							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 16							//''
      &getURL2
      &jmp label5      
     label4:
      &pushsdb 13							//'FSCommand:'
      &pushsdb 17							//'OnBuildQueueTypePageSelected'
      &pushregister 1
      &pushbyte 2
      &pushsdb 15							//'qualifyName'
      &callFunction
      &concat
      &pushregister 2
      &pushsdbgm 6							//'_name'
      &getURL2
     label5:
    &end // of function OnTabSelected

    &function2 OnSelectedTabClicked (r:3='tabNum') (r:1='this')
      &pushsdbgv 3							//'pages'
      &pushregister 3
      &getMember
      &pushsdbgm 5							//'clip'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgv 0							//'sel'
      &equals
      &not
      &jnz label6      
      &pushsdb 13							//'FSCommand:'
      &pushsdb 18							//'OnSelectionRefinementTabClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 15							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 16							//''
      &getURL2
      &jmp label7      
     label6:
      &pushsdb 13							//'FSCommand:'
      &pushsdb 19							//'OnBuildQueueTypeTabClicked'
      &pushregister 1
      &pushbyte 2
      &pushsdb 15							//'qualifyName'
      &callFunction
      &concat
      &pushregister 2
      &pushsdbgm 6							//'_name'
      &getURL2
     label7:
    &end // of function OnSelectedTabClicked

    &function2 OnTabRollOver (r:1='tabNum') ()
      &pushzero
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'clip'
      &dcallmp 20							// OnTabRollOver()
    &end // of function OnTabRollOver

    &function2 OnTabRollOut (r:1='tabNum') ()
      &pushzero
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'clip'
      &dcallmp 21							// OnTabRollOut()
    &end // of function OnTabRollOut

    &function2 SelectTab (r:1='tabNum') ()
      &pushsdbgv 22							//'tabsInitialized'
      &not
      &jnz label8      
      &pushregister 1
      &pushone
      &pushsdbgv 7							//'tabs'
      &dcallmp 23							// SelectTab()
      &jmp label9      
     label8:
      &pushsdb 24							//'selectedTabNum'
      &pushregister 1
      &setVariable
     label9:
    &end // of function SelectTab

    &function2 SelectPage (r:2='pageClip') ()
      &pushzero
      &setRegister r:1
      &pop
     label10:
      &pushregister 1
      &pushsdbgv 3							//'pages'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label12      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'clip'
      &pushregister 2
      &equals
      &not
      &jnz label11      
      &pushregister 1
      &pushone
      &dcallfp 23							// SelectTab()
      &jmp label12      
     label11:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label10      
     label12:
    &end // of function SelectPage

    &function2 SetTabFlashing (r:3='pageClip', r:2='flashing') ()
      &pushzero
      &setRegister r:1
      &pop
     label13:
      &pushregister 1
      &pushsdbgv 3							//'pages'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label16      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'clip'
      &pushregister 3
      &equals
      &not
      &jnz label15      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdb 25							//'tabFlashing'
      &pushregister 2
      &setMember
      &pushsdbgv 22							//'tabsInitialized'
      &not
      &jnz label14      
      &pushregister 2
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 7							//'tabs'
      &dcallmp 9							// SetTabFlashing()
     label14:
      &jmp label16      
     label15:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label13      
     label16:
    &end // of function SetTabFlashing

    &function2 SetTabHighlighted (r:3='pageClip', r:2='highlighted') ()
      &pushzero
      &setRegister r:1
      &pop
     label17:
      &pushregister 1
      &pushsdbgv 3							//'pages'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label20      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'clip'
      &pushregister 3
      &equals
      &not
      &jnz label19      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdb 26							//'tabHighlighted'
      &pushregister 2
      &setMember
      &pushsdbgv 22							//'tabsInitialized'
      &not
      &jnz label18      
      &pushregister 2
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 7							//'tabs'
      &dcallmp 10							// SetTabHighlighted()
     label18:
      &jmp label20      
     label19:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label17      
     label20:
    &end // of function SetTabHighlighted

    &function2 OnTabsInitialized ('clip') ()
      &pushsdbgv 22							//'tabsInitialized'
      &not
      &not
      &jnz label25      
      &pushsdb 22							//'tabsInitialized'
      &pushtrue
      &setVariable
      &pushsdb 11							//'visiblePageNum'
      &pushzero
      &pushsdbgv 7							//'tabs'
      &pushsdb 27							//'GetSelectedTabNum'
      &callMethod
      &toString
      &setVariable
      &pushsdbgv 3							//'pages'
      &pushsdbgv 11							//'visiblePageNum'
      &getMember
      &pushsdbgm 5							//'clip'
      &pushsdb 12							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 24							//'selectedTabNum'
      &pushundef
      &equals
      &not
      &not
      &jnz label21      
      &pushsdbgv 24							//'selectedTabNum'
      &pushone
      &pushsdbgv 7							//'tabs'
      &dcallmp 23							// SelectTab()
      &pushsdb 24							//'selectedTabNum'
      &delete2
      &pop
     label21:
      &pushzero
      &setRegister r:1
      &pop
     label22:
      &pushregister 1
      &pushsdbgv 3							//'pages'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label25      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 25							//'tabFlashing'
      &not
      &jnz label23      
      &pushtrue
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 7							//'tabs'
      &dcallmp 9							// SetTabFlashing()
     label23:
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 26							//'tabHighlighted'
      &not
      &jnz label24      
      &pushtrue
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 7							//'tabs'
      &dcallmp 10							// SetTabHighlighted()
     label24:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label22      
     label25:
    &end // of function OnTabsInitialized

    &function2 OnChildLoaded (r:2='childClip') ()
      &pushfalse
      &setRegister r:3
      &pop
      &pushsdbgv 11							//'visiblePageNum'
      &pushundef
      &equals
      &not
      &not
      &jnz label26      
      &pushregister 2
      &pushsdbgv 3							//'pages'
      &pushsdbgv 11							//'visiblePageNum'
      &getMember
      &pushsdbgm 5							//'clip'
      &equals
      &setRegister r:3
      &pop
     label26:
      &pushregister 2
      &pushsdb 12							//'_visible'
      &pushregister 3
      &setMember
      &pushzero
      &setRegister r:1
      &pop
     label27:
      &pushregister 1
      &pushsdbgv 3							//'pages'
      &pushsdbgm 4							//'length'
      &lessThan
      &not
      &jnz label29      
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 5							//'clip'
      &pushregister 2
      &equals
      &not
      &jnz label28      
      &pushregister 2
      &pushsdb 28							//'tabButton'
      &pushsdbgv 7							//'tabs'
      &pushregister 1
      &toString
      &getMember
      &setMember
      &pushregister 2
      &pushsdb 29							//'tabIcon'
      &pushsdbgv 3							//'pages'
      &pushregister 1
      &getMember
      &pushsdbgm 29							//'tabIcon'
      &setMember
      &jmp label29      
     label28:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label27      
     label29:
      &pushregister 2
      &pushsdb 30							//'tabHelpSite'
      &pushsdbgv 30							//'tabHelpSite'
      &setMember
    &end // of function OnChildLoaded

    &function onUnload (    )    
      &pushsdb 31							//'GetSelectionRefinementPage'
      &delete2
      &pop
      &pushsdb 32							//'GetBuildQueueTypePage'
      &delete2
      &pop
      &pushsdb 33							//'GetBuildQueueTypeTab'
      &delete2
      &pop
      &pushsdb 34							//'SelectSelectionRefinementPage'
      &delete2
      &pop
      &pushsdb 35							//'SelectBuildQueueTypePage'
      &delete2
      &pop
      &pushsdb 36							//'SetBuildQueueTypeTabFlashing'
      &delete2
      &pop
      &pushsdb 37							//'SetSelectionRefinementTabHighlighted'
      &delete2
      &pop
      &pushsdb 38							//'SetBuildQueueTypeTabHighlighted'
      &delete2
      &pop
      &pushsdb 39							//'OnTabSelected'
      &delete2
      &pop
      &pushsdb 20							//'OnTabRollOver'
      &delete2
      &pop
      &pushsdb 21							//'OnTabRollOut'
      &delete2
      &pop
      &pushsdb 23							//'SelectTab'
      &delete2
      &pop
      &pushsdb 8							//'SelectPage'
      &delete2
      &pop
      &pushsdb 9							//'SetTabFlashing'
      &delete2
      &pop
      &pushsdb 10							//'SetTabHighlighted'
      &delete2
      &pop
      &pushsdb 40							//'OnTabsInitialized'
      &delete2
      &pop
      &pushsdb 41							//'OnChildLoaded'
      &delete2
      &pop
      &pushsdb 42							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 43							//'initialized'
    &not
    &not
    &jnz label31    
    &pushsdb 3							//'pages'
    &pushsdb 5							//'clip'
    &pushsdbgv 44							//'navy'
    &pushsdb 29							//'tabIcon'
    &pushsdb 45							//'Nav'
    &pushsdb 25							//'tabFlashing'
    &pushfalse
    &pushsdb 26							//'tabHighlighted'
    &pushfalse
    &pushbyte 4
    &initObject
    &pushsdb 5							//'clip'
    &pushsdbgv 46							//'aircraft'
    &pushsdb 29							//'tabIcon'
    &pushsdb 47							//'Air'
    &pushsdb 25							//'tabFlashing'
    &pushfalse
    &pushsdb 26							//'tabHighlighted'
    &pushfalse
    &pushbyte 4
    &initObject
    &pushsdb 5							//'clip'
    &pushsdbgv 48							//'vehicles'
    &pushsdb 29							//'tabIcon'
    &pushsdb 49							//'Veh'
    &pushsdb 25							//'tabFlashing'
    &pushfalse
    &pushsdb 26							//'tabHighlighted'
    &pushfalse
    &pushbyte 4
    &initObject
    &pushsdb 5							//'clip'
    &pushsdbgv 50							//'infantry'
    &pushsdb 29							//'tabIcon'
    &pushsdb 51							//'Inf'
    &pushsdb 25							//'tabFlashing'
    &pushfalse
    &pushsdb 26							//'tabHighlighted'
    &pushfalse
    &pushbyte 4
    &initObject
    &pushsdb 5							//'clip'
    &pushsdbgv 52							//'otherStructure'
    &pushsdb 29							//'tabIcon'
    &pushsdb 53							//'Def'
    &pushsdb 25							//'tabFlashing'
    &pushfalse
    &pushsdb 26							//'tabHighlighted'
    &pushfalse
    &pushbyte 4
    &initObject
    &pushsdb 5							//'clip'
    &pushsdbgv 54							//'mainStructure'
    &pushsdb 29							//'tabIcon'
    &pushsdb 55							//'Str'
    &pushsdb 25							//'tabFlashing'
    &pushfalse
    &pushsdb 26							//'tabHighlighted'
    &pushfalse
    &pushbyte 4
    &initObject
    &pushsdb 5							//'clip'
    &pushsdbgv 0							//'sel'
    &pushsdb 29							//'tabIcon'
    &pushsdb 56							//'Con'
    &pushsdb 25							//'tabFlashing'
    &pushfalse
    &pushsdb 26							//'tabHighlighted'
    &pushfalse
    &pushbyte 4
    &initObject
    &pushbyte 7
    &initArray
    &setVariable
    &pushsdbgv 22							//'tabsInitialized'
    &not
    &jnz label30    
    &pushsdb 11							//'visiblePageNum'
    &pushzero
    &pushsdbgv 7							//'tabs'
    &pushsdb 27							//'GetSelectedTabNum'
    &callMethod
    &toString
    &setVariable
    &pushsdbgv 3							//'pages'
    &pushsdbgv 11							//'visiblePageNum'
    &getMember
    &pushsdbgm 5							//'clip'
    &pushsdb 12							//'_visible'
    &pushtrue
    &setMember
   label30:
    &pushsdb 57							//'TacticalHUDSelectionRefinementPage.swf'
    &pushone
    &pushsdbgv 0							//'sel'
    &dcallmp 58							// loadMovie()
    &pushsdb 59							//'TacticalHUDBuildQueuePage.swf'
    &pushone
    &pushsdbgv 54							//'mainStructure'
    &dcallmp 58							// loadMovie()
    &pushsdb 59							//'TacticalHUDBuildQueuePage.swf'
    &pushone
    &pushsdbgv 52							//'otherStructure'
    &dcallmp 58							// loadMovie()
    &pushsdb 59							//'TacticalHUDBuildQueuePage.swf'
    &pushone
    &pushsdbgv 50							//'infantry'
    &dcallmp 58							// loadMovie()
    &pushsdb 59							//'TacticalHUDBuildQueuePage.swf'
    &pushone
    &pushsdbgv 48							//'vehicles'
    &dcallmp 58							// loadMovie()
    &pushsdb 59							//'TacticalHUDBuildQueuePage.swf'
    &pushone
    &pushsdbgv 46							//'aircraft'
    &dcallmp 58							// loadMovie()
    &pushsdb 59							//'TacticalHUDBuildQueuePage.swf'
    &pushone
    &pushsdbgv 44							//'navy'
    &dcallmp 58							// loadMovie()
    &pushsdb 43							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 13							//'FSCommand:'
    &pushsdb 60							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 15							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 16							//''
    &getURL2
   label31:
    &stop
  &end // of frame 0

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &placeMovieClip 2 &as 'navy'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'aircraft'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'vehicles'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'infantry'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'otherStructure'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'mainStructure'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2

  &placeMovieClip 2 &as 'sel'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 2
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 3
  &end // of importAssets

  &placeMovieClip 3 &as 'tabHelpSite'
  
    &onClipEvent &construct
      &pushs 'vertAlignment'
      &pushssv 'top'
      &pushs 'horzAlignment'
      &pushssv 'right'
    &end
  &end // of placeMovieClip 3

  &defineMovieClip 10 // total frames: 30

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
  &end // of defineMovieClip 10

  &defineMovieClip 17 // total frames: 30

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
  &end // of defineMovieClip 17

  &defineMovieClip 24 // total frames: 30

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
  &end // of defineMovieClip 24

  &defineMovieClip 31 // total frames: 30

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
  &end // of defineMovieClip 31

  &defineMovieClip 38 // total frames: 30

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
  &end // of defineMovieClip 38

  &defineMovieClip 45 // total frames: 30

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
  &end // of defineMovieClip 45

  &defineMovieClip 52 // total frames: 30

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
  &end // of defineMovieClip 52

  &defineMovieClip 53 // total frames: 35

    &frame 0
      &pushs '_'
      &pushsgv '_parent'
      &pushsgm 'iconLabel'
      &add
      &gotoAndStop    &end // of frame 0
  &end // of defineMovieClip 53

  &defineMovieClip 54 // total frames: 1
  &end // of defineMovieClip 54

  &defineMovieClip 55 // total frames: 35

    &frame 4
      &stop
    &end // of frame 4

    &frame 34
      &gotoLabel '_on'
      &play
    &end // of frame 34
  &end // of defineMovieClip 55

  &defineButton 57
  
    &on     &overUpToOverDown
      &constants '_parent', 'selectedTabNum', '_name', '_down', 'gotoAndPlay'  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &pushsdbgm 1							//'selectedTabNum'
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 2							//'_name'
      &toNumber
      &equals
      &not
      &not
      &jnz label1      
      &pushsdb 3							//'_down'
      &pushone
      &pushsdbgv 0							//'_parent'
      &dcallmp 4							// gotoAndPlay()
     label1:
    &end
  
    &on     &overDownToOverUp
      &constants '_parent', 'selectedTabNum', '_name', '_over', 'gotoAndPlay', 'client', 'OnClicked'  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &pushsdbgm 1							//'selectedTabNum'
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 2							//'_name'
      &toNumber
      &equals
      &not
      &not
      &jnz label1      
      &pushsdb 3							//'_over'
      &pushone
      &pushsdbgv 0							//'_parent'
      &dcallmp 4							// gotoAndPlay()
     label1:
      &pushsdbgv 0							//'_parent'
      &pushone
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 5							//'client'
      &dcallmp 6							// OnClicked()
    &end
  
    &on     &idleToOverUp    ,&outDownToOverDown    ,&idleToOverDown
      &constants '_parent', 'selectedTabNum', '_name', '_over', 'gotoAndPlay', 'client', 'OnRollOver'  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &pushsdbgm 1							//'selectedTabNum'
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 2							//'_name'
      &toNumber
      &equals
      &not
      &not
      &jnz label1      
      &pushsdb 3							//'_over'
      &pushone
      &pushsdbgv 0							//'_parent'
      &dcallmp 4							// gotoAndPlay()
     label1:
      &pushsdbgv 0							//'_parent'
      &pushone
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 5							//'client'
      &dcallmp 6							// OnRollOver()
    &end
  
    &on     &overUpToIdle    ,&overDownToOutDown    ,&overDownToIdle
      &constants '_parent', 'selectedTabNum', '_name', '_up', 'gotoAndPlay', 'client', 'OnRollOut'  
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 0							//'_parent'
      &pushsdbgm 1							//'selectedTabNum'
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 2							//'_name'
      &toNumber
      &equals
      &not
      &not
      &jnz label1      
      &pushsdb 3							//'_up'
      &pushone
      &pushsdbgv 0							//'_parent'
      &dcallmp 4							// gotoAndPlay()
     label1:
      &pushsdbgv 0							//'_parent'
      &pushone
      &pushsdbgv 0							//'_parent'
      &pushsdbgm 5							//'client'
      &dcallmp 6							// OnRollOut()
    &end
  &end // of defineButton 57

  &defineMovieClip 58 // total frames: 1
  &end // of defineMovieClip 58

  &defineMovieClip 63 // total frames: 35

    &frame 0
      &pushs '_'
      &pushsgv '_parent'
      &pushsgm 'iconLabel'
      &add
      &gotoAndStop    &end // of frame 0
  &end // of defineMovieClip 63

  &defineMovieClip 68 // total frames: 30

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
  &end // of defineMovieClip 68

  &defineMovieClip 73 // total frames: 30

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
  &end // of defineMovieClip 73

  &defineMovieClip 78 // total frames: 30

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
  &end // of defineMovieClip 78

  &defineMovieClip 83 // total frames: 30

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
  &end // of defineMovieClip 83

  &defineMovieClip 88 // total frames: 30

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
  &end // of defineMovieClip 88

  &defineMovieClip 93 // total frames: 30

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
  &end // of defineMovieClip 93

  &defineMovieClip 98 // total frames: 30

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
  &end // of defineMovieClip 98

  &defineMovieClip 99 // total frames: 35

    &frame 0
      &pushs '_'
      &pushsgv '_parent'
      &pushsgm 'iconLabel'
      &add
      &gotoAndStop    &end // of frame 0
  &end // of defineMovieClip 99

  &defineMovieClip 100 // total frames: 50

    &frame 0
      &constants 'client', '_parent'  
      &pushsdbgv 0							//'client'
      &pushundef
      &equals
      &not
      &jnz label1      
      &pushsdb 0							//'client'
      &pushsdbgv 1							//'_parent'
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

    &frame 39
      &stop
    &end // of frame 39

    &frame 49
      &stop
    &end // of frame 49
  &end // of defineMovieClip 100

  &defineMovieClip 117 // total frames: 35

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
  &end // of defineMovieClip 117

  &defineMovieClip 118 // total frames: 1

    &frame 0
      &constants 'selectedTabNum', '_up', 'gotoAndPlay', '_selected', 'client', 'OnTabSelected', '_', 'tabOutline', '_name', 'SelectTab', 'TabButtonGlow', 'gotoAndStop', 'OnSelectedTabClicked', 'OnTabRollOver', 'OnTabRollOut', '_off', '_on', 'flashClip', 'tabHighlightClips', 'CreateHUDGadgetFlash', 'Close', 'length', 'pop', 'GetSelectedTabNum', 'OnClicked', 'OnRollOver', 'OnRollOut', 'SetTabFlashing', 'SetTabHighlighted', 'onUnload', 'initialized', 'tabCount', '_parent', 'pages', 'Array', 'i', 'this', 'iconLabel', 'tabIcon', '0', 'OnTabsInitialized'  
      &function2 SelectTab (r:2='tabNum') (r:1='this')
        &pushregister 2
        &pushsdbgv 0							//'selectedTabNum'
        &equals
        &not
        &not
        &jnz label1        
        &pushsdb 1							//'_up'
        &pushone
        &pushregister 1
        &pushsdbgv 0							//'selectedTabNum'
        &toString
        &getMember
        &dcallmp 2							// gotoAndPlay()
        &pushsdb 3							//'_selected'
        &pushone
        &pushregister 1
        &pushregister 2
        &toString
        &getMember
        &dcallmp 2							// gotoAndPlay()
        &pushsdb 0							//'selectedTabNum'
        &pushregister 2
        &setVariable
        &pushsdbgv 0							//'selectedTabNum'
        &pushone
        &pushsdbgv 4							//'client'
        &dcallmp 5							// OnTabSelected()
       label1:
        &pushsdb 6							//'_'
        &pushregister 2
        &toString
        &add
        &pushone
        &pushsdbgv 7							//'tabOutline'
        &dcallmp 2							// gotoAndPlay()
      &end // of function SelectTab

      &function GetSelectedTabNum (      )      
        &pushsdbgv 0							//'selectedTabNum'
        &return
      &end // of function GetSelectedTabNum

      &function2 OnClicked (r:2='buttonClip') ()
        &pushregister 2
        &pushsdbgm 8							//'_name'
        &toNumber
        &setRegister r:1
        &pop
        &pushregister 1
        &pushsdbgv 0							//'selectedTabNum'
        &equals
        &not
        &not
        &jnz label2        
        &pushregister 1
        &pushone
        &dcallfp 9							// SelectTab()
        &pushsdb 6							//'_'
        &pushregister 2
        &pushsdbgm 8							//'_name'
        &add
        &pushone
        &pushsdbgv 10							//'TabButtonGlow'
        &dcallmp 11							// gotoAndStop()
        &jmp label3        
       label2:
        &pushregister 1
        &pushone
        &pushsdbgv 4							//'client'
        &dcallmp 12							// OnSelectedTabClicked()
       label3:
      &end // of function OnClicked

      &function2 OnRollOver (r:1='buttonClip') ()
        &pushregister 1
        &pushsdbgm 8							//'_name'
        &toNumber
        &pushone
        &pushsdbgv 4							//'client'
        &dcallmp 13							// OnTabRollOver()
      &end // of function OnRollOver

      &function2 OnRollOut (r:1='buttonClip') ()
        &pushregister 1
        &pushsdbgm 8							//'_name'
        &toNumber
        &pushone
        &pushsdbgv 4							//'client'
        &dcallmp 14							// OnTabRollOut()
      &end // of function OnRollOut

      &function2 SetTabFlashing (r:3='tabNum', r:4='flashing') (r:1='this')
        &pushregister 1
        &pushregister 3
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushregister 4
        &jnz label4        
        &pushsdb 15							//'_off'
        &jmp label5        
       label4:
        &pushsdb 16							//'_on'
       label5:
        &pushone
        &pushregister 2
        &pushsdbgm 17							//'flashClip'
        &dcallmp 2							// gotoAndPlay()
      &end // of function SetTabFlashing

      &function2 SetTabHighlighted (r:3='tabNum', r:4='highlighted') (r:1='this', r:2='_global')
        &pushregister 4
        &not
        &jnz label7        
        &pushsdbgv 18							//'tabHighlightClips'
        &pushregister 3
        &getMember
        &pushnull
        &equals
        &not
        &jnz label6        
        &pushsdbgv 18							//'tabHighlightClips'
        &pushregister 3
        &pushregister 1
        &pushregister 3
        &toString
        &getMember
        &pushone
        &pushregister 2
        &pushsdb 19							//'CreateHUDGadgetFlash'
        &callMethod
        &setMember
       label6:
        &jmp label8        
       label7:
        &pushzero
        &pushsdbgv 18							//'tabHighlightClips'
        &pushregister 3
        &getMember
        &dcallmp 20							// Close()
        &pushsdbgv 18							//'tabHighlightClips'
        &pushregister 3
        &pushnull
        &setMember
       label8:
      &end // of function SetTabHighlighted

      &function2 onUnload () ()
       label9:
        &pushsdbgv 18							//'tabHighlightClips'
        &pushsdbgm 21							//'length'
        &pushzero
        &greaterThan
        &not
        &jnz label10        
        &pushzero
        &pushsdbgv 18							//'tabHighlightClips'
        &pushsdb 22							//'pop'
        &callMethod
        &setRegister r:1
        &pop
        &pushzero
        &pushregister 1
        &dcallmp 20							// Close()
        &jmp label9        
       label10:
        &pushsdb 9							//'SelectTab'
        &delete2
        &pop
        &pushsdb 23							//'GetSelectedTabNum'
        &delete2
        &pop
        &pushsdb 24							//'OnClicked'
        &delete2
        &pop
        &pushsdb 25							//'OnRollOver'
        &delete2
        &pop
        &pushsdb 26							//'OnRollOut'
        &delete2
        &pop
        &pushsdb 27							//'SetTabFlashing'
        &delete2
        &pop
        &pushsdb 28							//'SetTabHighlighted'
        &delete2
        &pop
        &pushsdb 29							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 30							//'initialized'
      &not
      &not
      &jnz label14      
      &pushsdb 31							//'tabCount'
      &pushsdbgv 32							//'_parent'
      &pushsdbgm 33							//'pages'
      &pushsdbgm 21							//'length'
      &setVariable
      &pushsdb 18							//'tabHighlightClips'
      &pushsdbgv 31							//'tabCount'
      &pushone
      &pushsdb 34							//'Array'
      &new
      &setVariable
      &pushsdb 35							//'i'
      &pushzero
      &varEquals
     label11:
      &pushsdbgv 35							//'i'
      &pushsdbgv 31							//'tabCount'
      &lessThan
      &not
      &jnz label12      
      &pushthisgv
      &pushsdbgv 35							//'i'
      &toString
      &getMember
      &pushsdb 37							//'iconLabel'
      &pushsdbgv 32							//'_parent'
      &pushsdbgm 33							//'pages'
      &pushsdbgv 35							//'i'
      &getMember
      &pushsdbgm 38							//'tabIcon'
      &setMember
      &pushsdbgv 18							//'tabHighlightClips'
      &pushsdbgv 35							//'i'
      &pushnull
      &setMember
      &pushsdb 35							//'i'
      &pushsdbgv 35							//'i'
      &increment
      &setVariable
      &jmp label11      
     label12:
      &pushsdbgv 4							//'client'
      &pushundef
      &equals
      &not
      &jnz label13      
      &pushsdb 4							//'client'
      &pushsdbgv 32							//'_parent'
      &setVariable
     label13:
      &pushsdb 0							//'selectedTabNum'
      &pushzerosv
      &pushsdb 3							//'_selected'
      &pushone
      &pushthisgv
      &pushsdbgm 39							//'0'
      &dcallmp 2							// gotoAndPlay()
      &pushsdb 30							//'initialized'
      &pushtrue
      &setVariable
      &pushthisgv
      &pushone
      &pushsdbgv 4							//'client'
      &dcallmp 40							// OnTabsInitialized()
     label14:
    &end // of frame 0
  &end // of defineMovieClip 118
&end
