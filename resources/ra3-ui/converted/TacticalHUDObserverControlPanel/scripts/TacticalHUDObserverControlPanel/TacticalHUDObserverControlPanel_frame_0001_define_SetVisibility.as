function SetVisibility(visArg)
{
   var _loc1_ = Number(visArg) != 0;
   panel._visible = _loc1_;
}
function GetPlayerSpinner()
{
   return Pointer.encode(panel.slider.playerSpinner);
}
function GetModeSpinner()
{
   return Pointer.encode(panel.slider.modeSpinner);
}
function onUnload()
{
   delete SetVisibility;
   delete GetPlayerSpinner;
   delete GetModeSpinner;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
