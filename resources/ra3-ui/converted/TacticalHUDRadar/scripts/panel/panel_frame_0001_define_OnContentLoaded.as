function OnContentLoaded(contentClip)
{
   if(contentClip == objectiveStage)
   {
      _parent.OnObjectiveStageLoaded(contentClip);
   }
   else if(contentClip == coOpAITargetLayer)
   {
      _parent.OnCoOpAITargetLayerLoaded(contentClip);
   }
   else if(contentClip == hotSpotLayer)
   {
      _parent.OnHotSpotLayerLoaded(contentClip);
   }
   else if(contentClip == bridgeLayerMC)
   {
      _parent.OnBridgeLayerLoaded(contentClip);
   }
}
function onUnload()
{
   delete OnContentLoaded;
   delete onUnload;
}
if(!initialized)
{
   objectiveStage.loadMovie("../../common/hud/ig_RadarObjectivesLayer.swf");
   coOpAITargetLayer.loadMovie("../../common/hud/ig_RadarCoOpAITargetLayer.swf");
   hotSpotLayer.loadMovie("ig_missionMapHotSpotLayer.swf");
   bridgeLayerMC.loadMovie("ig_RadarBridgeLayer.swf");
   initialized = true;
}
