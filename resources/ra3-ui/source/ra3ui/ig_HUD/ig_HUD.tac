movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\ig_HUD\\ig_HUD.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'FSCommand:', 'OnContentLoaded', 'qualifyName', 'Pointer', 'encode', 'content', 'createEmptyMovieClip', 'loadMovie', 'onUnloading', 'removeMovieClip', 'Compactifying faction observer IDs', 'Before:  nextFactionObserverID = ', 'nextFactionObserverID', ', numFactionObservers = ', 'numFactionObservers', 'Object', 'factionObserverMap', 'factionObserverID', 'After:  nextFactionObserverID = ', 'CompactifyFactionObserverIDs', 'currentFaction', 'subtitlePanel', 'subtitlePanelSite', 'OnSubtitlePanelSiteSet', 'OnSubtitlePanelSiteCleared', 'AttachFactionObserver', 'DetachFactionObserver', 'GetCurrentFaction', 'SetSubtitlePanel', 'SetSubtitlePanelSite', 'LoadContent', 'UnloadContent', 'AttachFactionObserverImpl', 'DetachFactionObserverImpl', 'GetCurrentFactionImpl', 'SetSubtitlePanelImpl', 'SetSubtitlePanelSiteImpl', 'onUnload', '_global', 'scope', 'String', 'name', 'charAt', ':', 'length', 'substr', '_root', 'this', 'bind1DynamicParams', 'bind0', 'initialized', 'Allies', 'OnInitialized', ''  
    &function2 OnContentLoaded (r:2='clip') (r:1='this')
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnContentLoaded'
      &pushregister 1
      &pushbyte 2
      &pushsdb 2							//'qualifyName'
      &callFunction
      &concat
      &pushregister 2
      &pushone
      &pushsdbgv 3							//'Pointer'
      &pushsdb 4							//'encode'
      &callMethod
      &getURL2
    &end // of function OnContentLoaded

    &function2 LoadContent (r:2='url') (r:1='this')
      &pushzero
      &pushsdb 5							//'content'
      &pushbyte 2
      &pushregister 1
      &dcallmp 6							// createEmptyMovieClip()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 5							//'content'
      &dcallmp 7							// loadMovie()
    &end // of function LoadContent

    &function UnloadContent (    )    
      &pushzero
      &pushsdbgv 5							//'content'
      &dcallmp 8							// onUnloading()
      &pushzero
      &pushsdbgv 5							//'content'
      &dcallmp 9							// removeMovieClip()
    &end // of function UnloadContent

    &function2 CompactifyFactionObserverIDs () (r:1='_root')
      &pushsdb 10							//'Compactifying faction observer IDs'
      &trace
      &pushsdb 11							//'Before:  nextFactionObserverID = '
      &pushsdbgv 12							//'nextFactionObserverID'
      &toString
      &add
      &pushsdb 13							//', numFactionObservers = '
      &add
      &pushsdbgv 14							//'numFactionObservers'
      &toString
      &add
      &trace
      &pushzero
      &pushsdb 15							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushzero
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 16							//'factionObserverMap'
      &enumerateValue
     label1:
      &setRegister r:0
      &pushnull
      &equals
      &jnz label4      
      &pushregister 0
      &setRegister r:6
      &pop
      &pushregister 1
      &pushsdbgm 16							//'factionObserverMap'
      &pushregister 6
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label2      
      &pop
      &pushregister 2
      &pushsdbgm 17							//'factionObserverID'
      &pushundef
      &equals
      &not
     label2:
      &not
      &jnz label3      
      &pushregister 4
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdb 17							//'factionObserverID'
      &pushregister 3
      &setMember
      &pushregister 5
      &pushregister 3
      &toString
      &pushregister 2
      &setMember
     label3:
      &jmp label1      
     label4:
      &pushregister 1
      &pushsdb 16							//'factionObserverMap'
      &pushregister 5
      &setMember
      &pushsdb 12							//'nextFactionObserverID'
      &pushregister 4
      &setVariable
      &pushsdb 14							//'numFactionObservers'
      &pushregister 4
      &setVariable
      &pushsdb 18							//'After:  nextFactionObserverID = '
      &pushsdbgv 12							//'nextFactionObserverID'
      &toString
      &add
      &pushsdb 13							//', numFactionObservers = '
      &add
      &pushsdbgv 14							//'numFactionObservers'
      &toString
      &add
      &trace
    &end // of function CompactifyFactionObserverIDs

    &function2 AttachFactionObserverImpl (r:3='observer') (r:1='_root')
      &pushsdbgv 12							//'nextFactionObserverID'
      &pushshort 10000
      &lessThan
      &not
      &dup
      &not
      &jnz label5      
      &pop
      &pushsdbgv 12							//'nextFactionObserverID'
      &pushsdbgv 14							//'numFactionObservers'
      &pushbyte 2
      &multiply
      &lessThan
      &not
     label5:
      &not
      &jnz label6      
      &pushzero
      &dcallfp 19							// CompactifyFactionObserverIDs()
     label6:
      &pushregister 3
      &pushsdbgm 17							//'factionObserverID'
      &pushundef
      &equals
      &not
      &jnz label7      
      &pushsdbgv 12							//'nextFactionObserverID'
      &pushsdb 12							//'nextFactionObserverID'
      &pushsdbgv 12							//'nextFactionObserverID'
      &increment
      &setVariable
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdb 17							//'factionObserverID'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdbgm 16							//'factionObserverMap'
      &pushregister 2
      &toString
      &pushregister 3
      &setMember
      &pushsdb 14							//'numFactionObservers'
      &pushsdbgv 14							//'numFactionObservers'
      &increment
      &setVariable
     label7:
    &end // of function AttachFactionObserverImpl

    &function2 DetachFactionObserverImpl (r:2='observer') (r:1='_root')
      &pushregister 2
      &pushsdbgm 17							//'factionObserverID'
      &pushundef
      &equals
      &not
      &not
      &jnz label8      
      &pushregister 1
      &pushsdbgm 16							//'factionObserverMap'
      &pushregister 2
      &pushsdbgm 17							//'factionObserverID'
      &toString
      &delete
      &pop
      &pushregister 2
      &pushsdb 17							//'factionObserverID'
      &delete
      &pop
      &pushsdb 14							//'numFactionObservers'
      &pushsdbgv 14							//'numFactionObservers'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label8      
      &pushsdb 12							//'nextFactionObserverID'
      &pushzerosv
     label8:
    &end // of function DetachFactionObserverImpl

    &function2 GetCurrentFactionImpl () (r:1='_root')
      &pushregister 1
      &pushsdbgm 20							//'currentFaction'
      &return
    &end // of function GetCurrentFactionImpl

    &function2 SetSubtitlePanelImpl (r:1='panel') ()
      &pushsdb 21							//'subtitlePanel'
      &pushregister 1
      &setVariable
      &pushsdbgv 21							//'subtitlePanel'
      &pushnull
      &equals
      &not
      &dup
      &not
      &jnz label9      
      &pop
      &pushsdbgv 22							//'subtitlePanelSite'
      &pushnull
      &equals
      &not
     label9:
      &not
      &jnz label10      
      &pushsdbgv 22							//'subtitlePanelSite'
      &pushone
      &pushsdbgv 21							//'subtitlePanel'
      &dcallmp 23							// OnSubtitlePanelSiteSet()
     label10:
    &end // of function SetSubtitlePanelImpl

    &function2 SetSubtitlePanelSiteImpl (r:1='panelSite') ()
      &pushregister 1
      &pushsdbgv 22							//'subtitlePanelSite'
      &equals
      &not
      &jnz label11      
      &pushundef
      &return
     label11:
      &pushsdbgv 22							//'subtitlePanelSite'
      &pushnull
      &equals
      &not
      &dup
      &not
      &jnz label12      
      &pop
      &pushsdbgv 21							//'subtitlePanel'
      &pushnull
      &equals
      &not
     label12:
      &not
      &jnz label13      
      &pushzero
      &pushsdbgv 21							//'subtitlePanel'
      &dcallmp 24							// OnSubtitlePanelSiteCleared()
     label13:
      &pushsdb 22							//'subtitlePanelSite'
      &pushregister 1
      &setVariable
      &pushsdbgv 22							//'subtitlePanelSite'
      &pushnull
      &equals
      &not
      &dup
      &not
      &jnz label14      
      &pop
      &pushsdbgv 21							//'subtitlePanel'
      &pushnull
      &equals
      &not
     label14:
      &not
      &jnz label15      
      &pushsdbgv 22							//'subtitlePanelSite'
      &pushone
      &pushsdbgv 21							//'subtitlePanel'
      &dcallmp 23							// OnSubtitlePanelSiteSet()
     label15:
    &end // of function SetSubtitlePanelSiteImpl

    &function2 onUnload () (r:1='_root', r:2='_global')
      &pushregister 1
      &pushsdb 25							//'AttachFactionObserver'
      &delete
      &pop
      &pushregister 1
      &pushsdb 26							//'DetachFactionObserver'
      &delete
      &pop
      &pushregister 1
      &pushsdb 27							//'GetCurrentFaction'
      &delete
      &pop
      &pushregister 1
      &pushsdb 28							//'SetSubtitlePanel'
      &delete
      &pop
      &pushregister 1
      &pushsdb 29							//'SetSubtitlePanelSite'
      &delete
      &pop
      &pushregister 1
      &pushsdb 20							//'currentFaction'
      &delete
      &pop
      &pushregister 1
      &pushsdb 16							//'factionObserverMap'
      &delete
      &pop
      &pushsdb 1							//'OnContentLoaded'
      &delete2
      &pop
      &pushsdb 30							//'LoadContent'
      &delete2
      &pop
      &pushsdb 31							//'UnloadContent'
      &delete2
      &pop
      &pushsdb 19							//'CompactifyFactionObserverIDs'
      &delete2
      &pop
      &pushsdb 32							//'AttachFactionObserverImpl'
      &delete2
      &pop
      &pushsdb 33							//'DetachFactionObserverImpl'
      &delete2
      &pop
      &pushsdb 34							//'GetCurrentFactionImpl'
      &delete2
      &pop
      &pushsdb 35							//'SetSubtitlePanelImpl'
      &delete2
      &pop
      &pushsdb 36							//'SetSubtitlePanelSiteImpl'
      &delete2
      &pop
      &pushregister 2
      &pushsdb 2							//'qualifyName'
      &delete
      &pop
      &pushsdb 37							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushglobalgv
    &pushsdb 2							//'qualifyName'
    &function  ('scope', 'name'    )    
      &pushsdbgv 39							//'scope'
      &pushsdbgv 40							//'String'
      &instanceOf
      &not
      &jnz label16      
      &pushsdb 39							//'scope'
      &pushsdbgv 39							//'scope'
      &getVariable
      &setVariable
     label16:
      &pushzero
      &pushone
      &pushsdbgv 41							//'name'
      &pushsdb 42							//'charAt'
      &callMethod
      &pushsdb 43							//':'
      &equals
      &not
      &jnz label17      
      &pushsdb 41							//'name'
      &pushsdbgv 41							//'name'
      &pushsdbgm 44							//'length'
      &pushone
      &subtract
      &pushone
      &pushbyte 2
      &pushsdbgv 41							//'name'
      &dcallmsv 45							// substr()
     label17:
      &pushsdbgv 39							//'scope'
      &pushone
      &pushsdbgv 3							//'Pointer'
      &pushsdb 4							//'encode'
      &callMethod
      &pushsdb 43							//':'
      &add
      &pushsdbgv 41							//'name'
      &add
      &return
    &end // of function 

    &setMember
    &pushsdbgv 46							//'_root'
    &pushsdb 25							//'AttachFactionObserver'
    &pushsdbgv 32							//'AttachFactionObserverImpl'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 48							//'bind1DynamicParams'
    &callMethod
    &setMember
    &pushsdbgv 46							//'_root'
    &pushsdb 26							//'DetachFactionObserver'
    &pushsdbgv 33							//'DetachFactionObserverImpl'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 48							//'bind1DynamicParams'
    &callMethod
    &setMember
    &pushsdbgv 46							//'_root'
    &pushsdb 27							//'GetCurrentFaction'
    &pushsdbgv 34							//'GetCurrentFactionImpl'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 49							//'bind0'
    &callMethod
    &setMember
    &pushsdbgv 46							//'_root'
    &pushsdb 28							//'SetSubtitlePanel'
    &pushsdbgv 35							//'SetSubtitlePanelImpl'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 48							//'bind1DynamicParams'
    &callMethod
    &setMember
    &pushsdbgv 46							//'_root'
    &pushsdb 29							//'SetSubtitlePanelSite'
    &pushsdbgv 36							//'SetSubtitlePanelSiteImpl'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 48							//'bind1DynamicParams'
    &callMethod
    &setMember
    &pushsdbgv 50							//'initialized'
    &not
    &not
    &jnz label18    
    &pushsdbgv 46							//'_root'
    &pushsdb 20							//'currentFaction'
    &pushsdb 51							//'Allies'
    &setMember
    &pushsdbgv 46							//'_root'
    &pushsdb 16							//'factionObserverMap'
    &pushzero
    &pushsdb 15							//'Object'
    &new
    &setMember
    &pushsdb 12							//'nextFactionObserverID'
    &pushzerosv
    &pushsdb 14							//'numFactionObservers'
    &pushzerosv
    &pushsdb 21							//'subtitlePanel'
    &pushnull
    &setVariable
    &pushsdb 22							//'subtitlePanelSite'
    &pushnull
    &setVariable
    &pushsdb 50							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 0							//'FSCommand:'
    &pushsdb 52							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 2							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 53							//''
    &getURL2
   label18:
    &stop
  &end // of frame 0
&end
