function TrackSite()
{
   var _loc2_ = {x:site._x,y:site._y};
   site._parent.localToGlobal(_loc2_);
   this.globalToLocal(_loc2_);
   _X = _loc2_.x;
   _Y = _loc2_.y - _height;
}
function OnSubtitlePanelSiteSet(panelSite)
{
   site = panelSite;
   onEnterFrame = function()
   {
      TrackSite();
   };
   TrackSite();
}
function OnSubtitlePanelSiteCleared()
{
   delete onEnterFrame;
   site = null;
   _X = baseLeft;
   _Y = baseBottom - _height;
}
function onUnload()
{
   _root.SetSubtitlePanel(null);
   delete TrackSite;
   delete OnSubtitlePanelSiteSet;
   delete OnSubtitlePanelSiteCleared;
   delete onEnterFrame;
   delete onUnload;
}
if(!initialized)
{
   var contentClip = this.attachMovie("SubtitleRenderingSurfaceContentPlaceholder","content",0,{_type:qualifyName(this,"Render")});
   contentClip._y = - contentClip._height;
   site = null;
   baseLeft = _X;
   baseBottom = _Y + _height;
   _root.SetSubtitlePanel(this);
   initialized = true;
}
stop();
