function SetVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   panel._visible = _loc1_;
}
function GetModeIndicator()
{
   return Pointer.encode(panel.modeIndicator);
}
function GetTalkIndicator()
{
   return Pointer.encode(panel.talkIndicator);
}
function onUnload()
{
   delete SetVisibility;
   delete GetModeIndicator;
   delete GetTalkIndicator;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
