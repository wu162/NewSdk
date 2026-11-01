function RegisterMasterResizeListener(mc)
{
   masterResizeListeners.push(mc);
}
function UnregisterMasterResizeListener(mc)
{
   var _loc1_ = 0;
   while(_loc1_ < masterResizeListeners.length)
   {
      if(masterResizeListeners[_loc1_] == mc)
      {
         masterResizeListeners.splice(_loc1_,1);
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
}
function OnMasterResize(mc)
{
   var _loc1_ = 0;
   while(_loc1_ < masterResizeListeners.length)
   {
      masterResizeListeners[_loc1_].OnMasterResize(mc);
      _loc1_ = _loc1_ + 1;
   }
}
function onUnload()
{
   delete masterResizeListeners;
   delete RegisterMasterResizeListener;
   delete UnregisterMasterResizeListener;
   delete OnMasterResize;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   masterResizeListeners = new Array();
}
