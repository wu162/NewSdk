function SetTalking(talkingArg)
{
   var _loc1_ = Number(talkingArg) != 0;
   gotoAndPlay(!_loc1_ ? "_off" : "_on");
}
function GetHelpSite()
{
   return Pointer.encode(helpSite);
}
function onUnload()
{
   delete SetTalking;
   delete GetHelpSite;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
