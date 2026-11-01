function OnCurrentFactionChanged(faction)
{
   gotoAndPlay(faction);
}
function onUnload()
{
   _root.DetachFactionObserver(this);
   delete OnCurrentFactionChanged;
   delete onUnload;
}
if(!initialized)
{
   gotoAndPlay(_root.GetCurrentFaction());
   _root.AttachFactionObserver(this);
   initialized = true;
}
