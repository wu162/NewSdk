function SetVisibility(vis)
{
   _visible = vis != 0;
}
function SetAvailability(availability)
{
   gotoAndPlay(availability);
}
function SetSelected(selArg)
{
   selected = selArg != 0;
   content.selectedOverlay._visible = selected;
}
function SetDependenceUponPriorRow(dependenceArg)
{
   arrow._visible = dependenceArg != 0;
}
function OnPress()
{
}
function OnRelease()
{
   fscommand(qualifyName(this,"OnClicked"),"");
}
function OnRollOver()
{
   fscommand(qualifyName(this,"OnRollOver"),"");
}
function OnRollOut()
{
   fscommand(qualifyName(this,"OnRollOut"),"");
}
function GetHelpSite()
{
   return Pointer.encode(_parent.helpSite);
}
function onUnload()
{
   delete SetVisibility;
   delete SetAvailability;
   delete SetSelected;
   delete SetDependenceUponPriorRow;
   delete OnPress;
   delete OnRelease;
   delete OnRollOver;
   delete OnRollOut;
   delete GetHelpSite;
   delete onUnload;
}
if(!initialized)
{
   arrow._visible = false;
   selected = false;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
