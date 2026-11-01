function Open()
{
   coverMC.gotoAndPlay("_opening");
   panel._visible = true;
}
function Close()
{
   inputShield._visible = true;
   isCoverClosed = true;
   isMapAspectRatioChanging = false;
   coverMC.gotoAndPlay("_closing");
}
function GetViewBoxSurface()
{
   return Pointer.encode(panel.viewBoxSurface);
}
function GetPingStage()
{
   return Pointer.encode(panel.pingSurface);
}
function GetTelestratorSurface()
{
   return Pointer.encode(panel.telestratorSurface);
}
function GetObjectiveStage()
{
   return Pointer.encode(panel.objectiveStage);
}
function GetMovieSurface()
{
   return Pointer.encode(movieSurface);
}
function GetBuildQueueStage()
{
   return Pointer.encode(panel.buildQueueStage);
}
function GetBeaconStage()
{
   return Pointer.encode(panel.beaconStage);
}
function GetObjectsSurface()
{
   return Pointer.encode(panel.objectsSurface);
}
function GetTerrainSurface()
{
   return Pointer.encode(panel.terrainSurface);
}
function GetCoOpAITargetLayer()
{
   return Pointer.encode(panel.coOpAITargetLayer);
}
function GetShroudSurface()
{
   return Pointer.encode(panel.shroudSurface);
}
function GetHotSpotLayer()
{
   return Pointer.encode(panel.hotSpotLayer);
}
function GetBridgeLayer()
{
   return Pointer.encode(panel.bridgeLayerMC);
}
function GetMapWidth()
{
   return String(mapWidth);
}
function GetMapHeight()
{
   return String(mapHeight);
}
function MoveToMapArea(clip)
{
   clip._x = mapX;
   clip._y = mapY;
   clip._width = mapWidth;
   clip._height = mapHeight;
}
function MoveToMapAreaWithoutScaling(clip)
{
   clip._x = mapX;
   clip._y = mapY;
   if(clip.initialized)
   {
      clip.Resize(mapWidth,mapHeight);
   }
   else
   {
      clip.sizer._width = mapWidth;
      clip.sizer._height = mapHeight;
   }
   clip._width = mapWidth;
   clip._height = mapHeight;
}
function SetMapAspectRatio(newRatio)
{
   if(newRatio == mapAspectRatio)
   {
      return undefined;
   }
   if(newRatio > backAspectRatio)
   {
      mapWidth = backWidth;
      mapHeight = mapWidth / newRatio;
      mapX = backX;
      mapY = backY + (backHeight - mapHeight) / 2;
   }
   else
   {
      mapHeight = backHeight;
      mapWidth = mapHeight * newRatio;
      mapY = backY;
      mapX = backX + (backWidth - mapWidth) / 2;
   }
   mapAspectRatio = newRatio;
   MoveToMapArea(panel.viewBoxSurface);
   MoveToMapArea(panel.shroudSurface);
   MoveToMapArea(panel.objectsSurface);
   MoveToMapArea(panel.taintSurface);
   MoveToMapArea(panel.terrainSurface);
   MoveToMapArea(panel.telestratorSurface);
   MoveToMapArea(panel.inputCatcher);
   MoveToMapAreaWithoutScaling(panel.pingSurface);
   trace("##################******************* pingSurface is " + panel.pingSurface + ", pingSurface._x is " + panel.pingSurface._x + ", pingSurface._y is " + panel.pingSurface._y);
   MoveToMapAreaWithoutScaling(panel.objectiveStage);
   MoveToMapAreaWithoutScaling(panel.beaconStage);
   MoveToMapAreaWithoutScaling(panel.buildQueueStage);
   MoveToMapAreaWithoutScaling(panel.coOpAITargetLayer);
   MoveToMapAreaWithoutScaling(panel.hotSpotLayer);
   MoveToMapAreaWithoutScaling(panel.bridgeLayer);
   if(!isCoverClosed)
   {
      isMapAspectRatioChanging = true;
      inputShield._visible = true;
      coverMC.gotoAndPlay("_closing");
   }
}
function SetJammedMessageVisibility(vis)
{
   jammedMessage._visible = vis != 0;
}
function SetHighlighted(highlightArg)
{
   var _loc2_ = highlightArg != 0;
   if(_loc2_)
   {
      if(highlightClip == undefined)
      {
         highlightClip = _global.CreateHUDGadgetFlash(panel.back);
      }
   }
   else
   {
      highlightClip.Close();
      highlightClip = undefined;
   }
}
function OnObjectiveStageLoaded(objectiveStage)
{
   MoveToMapAreaWithoutScaling(objectiveStage);
}
function OnCoOpAITargetLayerLoaded(coOpAITargetLayer)
{
   MoveToMapAreaWithoutScaling(coOpAITargetLayer);
}
function OnHotSpotLayerLoaded(hotSpotLayer)
{
   MoveToMapAreaWithoutScaling(hotSpotLayer);
}
function OnBridgeLayerLoaded(bridgeLayer)
{
   MoveToMapAreaWithoutScaling(bridgeLayer);
}
function OnCoverClipOpened()
{
   inputShield._visible = false;
   if(isMapAspectRatioChanging)
   {
      isMapAspectRatioChanging = false;
   }
   else
   {
      fscommand(qualifyName(this,"OnOpened"),"");
      isCoverClosed = false;
   }
}
function OnCoverClipClosed()
{
   if(isMapAspectRatioChanging)
   {
      coverMC.gotoAndPlay("_opening");
   }
   else
   {
      fscommand(qualifyName(this,"OnClosed"),"");
      panel._visible = false;
   }
}
function onUnload()
{
   highlightClip.Close();
   delete Open;
   delete Close;
   delete GetViewBoxSurface;
   delete GetPingStage;
   delete GetTelestratorSurface;
   delete GetObjectiveStage;
   delete GetMovieSurface;
   delete GetBuildQueueStage;
   delete GetBeaconStage;
   delete GetObjectsSurface;
   delete GetTerrainSurface;
   delete GetCoOpAITargetLayer;
   delete GetShroudSurface;
   delete GetHotSpotLayer;
   delete GetBridgeLayer;
   delete GetMapWidth;
   delete GetMapHeight;
   delete MoveToMapArea;
   delete MoveToMapAreaWithoutScaling;
   delete SetMapAspectRatio;
   delete SetJammedMessageVisibility;
   delete SetHighlighted;
   delete OnObjectiveStageLoaded;
   delete OnCoOpAITargetLayerLoaded;
   delete OnHotSpotLayerLoaded;
   delete OnBridgeLayerLoaded;
   delete OnCoverClipOpened;
   delete OnCoverClipClosed;
   delete coverMC.onOpened;
   delete coverMC.onClosed;
   delete onUnload;
}
if(!initialized)
{
   backX = panel.back._x;
   backY = panel.back._y;
   backWidth = panel.back._width;
   backHeight = panel.back._height;
   backAspectRatio = backWidth / backHeight;
   mapX = backX;
   mapY = backY;
   mapWidth = backWidth;
   mapHeight = backHeight;
   mapAspectRatio = backAspectRatio;
   isCoverClosed = true;
   isMapAspectRatioChanging = false;
   jammedMessage._visible = false;
   panel._visible = false;
   coverMC.onOpened = _global.bind0(this,OnCoverClipOpened);
   coverMC.onClosed = _global.bind0(this,OnCoverClipClosed);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      SetMapAspectRatio(0.5);
      Close();
   }
}
