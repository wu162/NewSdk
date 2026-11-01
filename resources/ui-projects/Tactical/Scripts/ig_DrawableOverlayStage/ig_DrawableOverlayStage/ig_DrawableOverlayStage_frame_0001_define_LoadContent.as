function LoadContent(overlayId, layerNum, componentStageId, contentType)
{
   var _loc2_ = this[String(layerNum)];
   var _loc3_ = _loc2_.nextContentDepth;
   _loc2_.nextContentDepth = _loc2_.nextContentDepth + 1;
   if(_loc2_.nextContentDepth >= 65536)
   {
      _loc2_.nextContentDepth = 0;
   }
   var _loc4_ = String(overlayId) + "," + String(componentStageId);
   var _loc5_ = _loc2_.createEmptyMovieClip(_loc4_,_loc3_);
   _loc5_.loadMovie(contentType);
}
function UnloadContent(overlayId, layerNum, componentStageId)
{
   var _loc3_ = this[String(layerNum)];
   var _loc2_ = _loc3_[String(overlayId) + "," + String(componentStageId)];
   _loc2_.removeMovieClip();
}
function OnContentLoaded(overlayId, layerNum, componentStageId, contentClip)
{
   fscommand(qualifyName(this,"OnContentLoaded-" + String(overlayId) + "-" + String(layerNum) + "-" + String(componentStageId)),Pointer.encode(contentClip));
}
function onUnload()
{
   delete LoadContent;
   delete UnloadContent;
   delete OnContentLoaded;
   delete onUnload;
}
if(!initialized)
{
   layerCount = 5;
   var i = 0;
   while(i < layerCount)
   {
      var layerClip = this.attachMovie("LayerClip",String(i),i);
      layerClip.nextContentDepth = 0;
      i++;
   }
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
