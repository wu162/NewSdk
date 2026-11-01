function SetFaction(faction)
{
   factionLabel = faction;
   image.gotoAndPlay(factionLabel);
}
function onUnload()
{
   delete SetFaction;
   delete onUnload;
}
if(!initialized)
{
   if(factionLabel == undefined)
   {
      factionLabel = "GDI";
   }
   initialized = true;
}
