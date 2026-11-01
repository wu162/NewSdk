function SetPlayerColor(colorArg)
{
   var _loc1_ = Number(colorArg);
   clanText.field.textColor = _loc1_;
   nameText.field.textColor = _loc1_;
   factionText.field.textColor = _loc1_;
   teamText.field.textColor = _loc1_;
   statusText.field.textColor = _loc1_;
}
function GetMuteCheckbox()
{
   return Pointer.encode(muteCheckbox);
}
function onUnload()
{
   delete SetPlayerColor;
   delete GetMuteCheckbox;
   delete onUnload;
}
if(!initialized)
{
   muteCheckbox.loadMovie("InGameCheckbox.swf");
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
