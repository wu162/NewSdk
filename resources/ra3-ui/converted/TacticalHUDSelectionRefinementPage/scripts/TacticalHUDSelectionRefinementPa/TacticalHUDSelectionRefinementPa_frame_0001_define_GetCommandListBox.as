function GetCommandListBox()
{
   return Pointer.encode(commandList);
}
function OnTabRollOver()
{
   fscommand(qualifyName(this,"OnTabRollOver"),"");
}
function OnTabRollOut()
{
   fscommand(qualifyName(this,"OnTabRollOut"),"");
}
function GetTabHelpSite()
{
   return Pointer.encode(tabHelpSite);
}
function OnContentLoaded(contentClip)
{
   if(contentClip == commandList)
   {
      commandList.buttonSlotWidth = 52;
      commandList.numColumns = 4;
      commandList.numVisibleRows = 5;
   }
}
function onUnload()
{
   delete GetCommandListBox;
   delete OnTabRollOver;
   delete OnTabRollOut;
   delete GetTabHelpSite;
   delete OnContentLoaded;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnChildLoaded(this);
   commandList.loadMovie("TacticalHUDCommandListBox.swf");
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
