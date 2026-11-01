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
if(!factionSkinned)
{
   gotoAndPlay(_root.GetCurrentFaction());
   _root.AttachFactionObserver(this);
   factionSkinned = true;
}
