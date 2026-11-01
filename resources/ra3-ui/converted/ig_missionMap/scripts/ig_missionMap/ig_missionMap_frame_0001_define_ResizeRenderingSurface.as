function ResizeRenderingSurface(surfaceClip, x, y, w, h)
{
   surfaceClip._x = x;
   surfaceClip._y = y;
   surfaceClip._width = w;
   surfaceClip._height = h;
}
function ResizeIndicatorLayer(layerClip, x, y, w, h)
{
   layerClip._x = x;
   layerClip._y = y;
   if(layerClip.initialized)
   {
      layerClip.Resize(w,h);
   }
   else
   {
      layerClip.sizer._width = w;
      layerClip.sizer._height = h;
   }
}
function SetMapAspectRatio(mapAspectRatio)
{
   var _loc7_ = backgroundMC._x;
   var _loc8_ = backgroundMC._y;
   var _loc6_ = backgroundMC._width;
   var _loc5_ = backgroundMC._height;
   var _loc10_ = _loc6_ / _loc5_;
   var _loc4_;
   var _loc3_;
   var _loc1_;
   var _loc2_;
   if(mapAspectRatio >= _loc10_)
   {
      _loc4_ = _loc7_;
      _loc1_ = _loc6_;
      _loc2_ = _loc1_ / mapAspectRatio;
      _loc3_ = _loc8_ + (_loc5_ - _loc2_) * 0.5;
   }
   else
   {
      _loc3_ = _loc8_;
      _loc2_ = _loc5_;
      _loc1_ = _loc2_ * mapAspectRatio;
      _loc4_ = _loc7_ + (_loc6_ - _loc1_) * 0.5;
   }
   ResizeRenderingSurface(terrainSurface,_loc4_,_loc3_,_loc1_,_loc2_);
   ResizeRenderingSurface(shroudSurface,_loc4_,_loc3_,_loc1_,_loc2_);
   ResizeIndicatorLayer(objectiveLayer,_loc4_,_loc3_,_loc1_,_loc2_);
   ResizeIndicatorLayer(structureLayer,_loc4_,_loc3_,_loc1_,_loc2_);
   ResizeIndicatorLayer(hotSpotLayer,_loc4_,_loc3_,_loc1_,_loc2_);
   ResizeIndicatorLayer(startPosLayer,_loc4_,_loc3_,_loc1_,_loc2_);
   ResizeIndicatorLayer(bridgeLayer,_loc4_,_loc3_,_loc1_,_loc2_);
}
function GetObjectiveLayer()
{
   return Pointer.encode(objectiveLayer);
}
function GetStructureLayer()
{
   return Pointer.encode(structureLayer);
}
function GetHotSpotLayer()
{
   return Pointer.encode(hotSpotLayer);
}
function GetBridgeLayer()
{
   return Pointer.encode(bridgeLayer);
}
function GetStartPosLayer()
{
   return Pointer.encode(startPosLayer);
}
function GetTerrainRenderingSurface()
{
   return Pointer.encode(terrainSurface);
}
function GetShroudRenderingSurface()
{
   return Pointer.encode(shroudSurface);
}
function OnBound()
{
   if(initialized)
   {
      fscommand(qualifyName(this,"OnInitialized"),"");
   }
   shroudSurface.OnBound();
   terrainSurface.OnBound();
   objectiveLayer.OnBound();
   structureLayer.OnBound();
   hotSpotLayer.OnBound();
   startPosLayer.OnBound();
   bridgeLayer.OnBound();
}
function OnDestroying()
{
   trace("ig_missionMap OnDestroying()");
   startPosLayer.OnDestroying();
   hotSpotLayer.OnDestroying();
   structureLayer.OnDestroying();
   objectiveLayer.OnDestroying();
   terrainSurface.OnDestroying();
   shroudSurface.OnDestroying();
   bridgeLayer.OnDestroying();
   if(initialized)
   {
      fscommand(qualifyName(this,"OnClipDestroyed"),"");
   }
}
function OnContentLoaded(contentClip)
{
   if(contentClip == hotSpotLayer || contentClip == bridgeLayer)
   {
      contentClip._x = objectiveLayer._x;
      contentClip._y = objectiveLayer._y;
      contentClip.sizer._width = objectiveLayer.sizer._width;
      contentClip.sizer._height = objectiveLayer.sizer._height;
   }
}
function onUnload()
{
   trace("ig_missionMap onUnload()");
   delete ResizeRenderingSurface;
   delete ResizeIndicatorLayer;
   delete SetMapAspectRatio;
   delete GetObjectiveLayer;
   delete GetHotSpotLayer;
   delete GetBridgeLayer;
   delete GetStartPosLayer;
   delete GetTerrainRenderingSurface;
   delete GetShroudRenderingSurface;
   delete OnBound;
   delete OnDestroying;
   delete OnContentLoaded;
   delete onUnload;
}
if(!initialized)
{
   trace("ig_missionMap loaded");
   _parent.OnContentLoaded(this);
   hotSpotLayer.loadMovie("ig_missionMapHotSpotLayer.swf");
   bridgeLayer.loadMovie("ig_RadarBridgeLayer.swf");
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
