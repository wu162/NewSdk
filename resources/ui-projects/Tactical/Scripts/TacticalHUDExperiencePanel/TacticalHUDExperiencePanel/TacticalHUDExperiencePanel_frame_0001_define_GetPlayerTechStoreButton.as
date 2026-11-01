function GetPlayerTechStoreButton()
{
   return Pointer.encode(playerTechStoreButton);
}
function GetPlayerExperienceMeter()
{
   return Pointer.encode(playerExperienceMeter);
}
function GetThreatMeter()
{
   return Pointer.encode(threatMeter);
}
function onUnload()
{
   delete GetPlayerTechStoreButton;
   delete GetPlayerExperienceMeter;
   delete GetThreatMeter;
   delete onUnload;
}
if(!initialized)
{
   playerTechStoreButton.helpSite = helpSite;
   threatMeter.helpSite = helpSite;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
