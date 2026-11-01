function OnContentLoaded(contentClip)
{
   var _loc2_ = contentClip._name.split(",");
   _parent.OnContentLoaded(Number(_loc2_[0]),Number(_name),Number(_loc2_[1]),contentClip);
}
function onUnload()
{
   delete OnContentLoaded;
   delete onUnload;
}
