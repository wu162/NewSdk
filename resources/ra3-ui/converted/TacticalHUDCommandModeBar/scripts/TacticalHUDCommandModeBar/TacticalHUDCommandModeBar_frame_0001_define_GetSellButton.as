function GetSellButton()
{
   return Pointer.encode(sellButton);
}
function GetRepairButton()
{
   return Pointer.encode(repairButton);
}
function GetPauseButton()
{
   return Pointer.encode(pauseButton);
}
function onUnload()
{
   delete GetSellButton;
   delete GetRepairButton;
   delete GetPauseButton;
   delete onUnload;
}
if(!initialized)
{
   sellButton.helpSite = helpSite;
   repairButton.helpSite = helpSite;
   pauseButton.helpSite = helpSite;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
