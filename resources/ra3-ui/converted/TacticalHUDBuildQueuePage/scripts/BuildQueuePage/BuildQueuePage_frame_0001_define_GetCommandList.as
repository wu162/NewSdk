function GetCommandList()
{
   return Pointer.encode(commandList);
}
function GetTabHelpSite()
{
   return Pointer.encode(tabHelpSite);
}
function OnTabPressed()
{
   fscommand(qualifyName(this,"OnTabPressed"),"");
}
function OnTabRollOver()
{
   fscommand(qualifyName(this,"OnTabRollOver"),"");
}
function OnTabRollOut()
{
   fscommand(qualifyName(this,"OnTabRollOut"),"");
}
function onUnload()
{
   delete GetCommandList;
   delete GetTabHelpSite;
   delete OnTabPressed;
   delete OnTabRollOver;
   delete OnTabRollOut;
   delete onUnload;
}
if(!initialized)
{
   disableCommandButtonLoadAnim = true;
   commandList.loadMovie("TacticalHUDCommandListBox.swf");
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
