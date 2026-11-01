function SetProgress(val)
{
   trace("SetProgress( " + String(val) + " )");
   var _loc1_ = String(Math.floor(val * 24) + 1);
   trace("frameNumStr = " + _loc1_);
   gotoAndStop(Number(_loc1_));
}
function onUnload()
{
   delete SetProgress;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
