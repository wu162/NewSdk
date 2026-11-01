function GetFastForwardButton()
{
   return Pointer.encode(ffButtonMC);
}
function SetVisibility(visArg)
{
   _visible = visArg != 0;
}
function onUnload()
{
   delete GetFastForwardButton;
   delete SetVisibility;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   initialized = true;
   fscommand(_global.qualifyName(this,"OnInitialized"),"");
}
