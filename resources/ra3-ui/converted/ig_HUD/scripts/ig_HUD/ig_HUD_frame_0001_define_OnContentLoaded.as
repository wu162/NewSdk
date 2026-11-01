function OnContentLoaded(clip)
{
   fscommand(qualifyName(this,"OnContentLoaded"),Pointer.encode(clip));
}
function LoadContent(url)
{
   this.createEmptyMovieClip("content",0);
   this.content.loadMovie(url);
}
function UnloadContent()
{
   content.onUnloading();
   content.removeMovieClip();
}
function CompactifyFactionObserverIDs()
{
   trace("Compactifying faction observer IDs");
   trace("Before:  nextFactionObserverID = " + String(nextFactionObserverID) + ", numFactionObservers = " + String(numFactionObservers));
   var _loc5_ = new Object();
   var _loc4_ = 0;
   var _loc2_;
   var _loc3_;
   for(var _loc6_ in _root.factionObserverMap)
   {
      _loc2_ = _root.factionObserverMap[_loc6_];
      if(_loc2_ != undefined && _loc2_.factionObserverID != undefined)
      {
         _loc3_ = _loc4_++;
         _loc2_.factionObserverID = _loc3_;
         _loc5_[String(_loc3_)] = _loc2_;
      }
   }
   _root.factionObserverMap = _loc5_;
   nextFactionObserverID = _loc4_;
   numFactionObservers = _loc4_;
   trace("After:  nextFactionObserverID = " + String(nextFactionObserverID) + ", numFactionObservers = " + String(numFactionObservers));
}
function AttachFactionObserverImpl(observer)
{
   if(nextFactionObserverID >= 10000 && nextFactionObserverID >= numFactionObservers * 2)
   {
      CompactifyFactionObserverIDs();
   }
   var _loc2_;
   if(observer.factionObserverID == undefined)
   {
      _loc2_ = nextFactionObserverID++;
      observer.factionObserverID = _loc2_;
      _root.factionObserverMap[String(_loc2_)] = observer;
      numFactionObservers++;
   }
}
function DetachFactionObserverImpl(observer)
{
   if(observer.factionObserverID != undefined)
   {
      delete _root.factionObserverMap[String(observer.factionObserverID)];
      delete observer.factionObserverID;
      if(--numFactionObservers == 0)
      {
         nextFactionObserverID = 0;
      }
   }
}
function GetCurrentFactionImpl()
{
   return _root.currentFaction;
}
function SetSubtitlePanelImpl(panel)
{
   subtitlePanel = panel;
   if(subtitlePanel != null && subtitlePanelSite != null)
   {
      subtitlePanel.OnSubtitlePanelSiteSet(subtitlePanelSite);
   }
}
function SetSubtitlePanelSiteImpl(panelSite)
{
   if(panelSite == subtitlePanelSite)
   {
      return undefined;
   }
   if(subtitlePanelSite != null && subtitlePanel != null)
   {
      subtitlePanel.OnSubtitlePanelSiteCleared();
   }
   subtitlePanelSite = panelSite;
   if(subtitlePanelSite != null && subtitlePanel != null)
   {
      subtitlePanel.OnSubtitlePanelSiteSet(subtitlePanelSite);
   }
}
function onUnload()
{
   delete _root.AttachFactionObserver;
   delete _root.DetachFactionObserver;
   delete _root.GetCurrentFaction;
   delete _root.SetSubtitlePanel;
   delete _root.SetSubtitlePanelSite;
   delete _root.currentFaction;
   delete _root.factionObserverMap;
   delete OnContentLoaded;
   delete LoadContent;
   delete UnloadContent;
   delete CompactifyFactionObserverIDs;
   delete AttachFactionObserverImpl;
   delete DetachFactionObserverImpl;
   delete GetCurrentFactionImpl;
   delete SetSubtitlePanelImpl;
   delete SetSubtitlePanelSiteImpl;
   delete _global.qualifyName;
   delete onUnload;
}
_global.qualifyName = function(scope, name)
{
   if(scope instanceof String)
   {
      scope = eval(scope);
   }
   if(name.charAt(0) == ":")
   {
      name = name.substr(1,name.length - 1);
   }
   return Pointer.encode(scope) + ":" + name;
};
_root.AttachFactionObserver = _global.bind1DynamicParams(this,AttachFactionObserverImpl);
_root.DetachFactionObserver = _global.bind1DynamicParams(this,DetachFactionObserverImpl);
_root.GetCurrentFaction = _global.bind0(this,GetCurrentFactionImpl);
_root.SetSubtitlePanel = _global.bind1DynamicParams(this,SetSubtitlePanelImpl);
_root.SetSubtitlePanelSite = _global.bind1DynamicParams(this,SetSubtitlePanelSiteImpl);
if(!initialized)
{
   _root.currentFaction = "Allies";
   _root.factionObserverMap = new Object();
   nextFactionObserverID = 0;
   numFactionObservers = 0;
   subtitlePanel = null;
   subtitlePanelSite = null;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
