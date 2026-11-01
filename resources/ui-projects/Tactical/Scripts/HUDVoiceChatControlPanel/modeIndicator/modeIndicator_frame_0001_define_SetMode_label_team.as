function SetMode(modeLabelArg)
{
   var _loc1_ = String(modeLabelArg);
   gotoAndPlay(_loc1_);
}
function GetHelpSite()
{
   return Pointer.encode(helpSite);
}
function SetVisibility(visArg)
{
   _visible = Number(visArg) != 0;
}
function onUnload()
{
   delete SetMode;
   delete GetHelpSite;
   delete SetVisibility;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
