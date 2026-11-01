function SetPrimary()
{
   arrow.gotoAndPlay("_primary");
}
function SetBonus()
{
   arrow.gotoAndPlay("_bonus");
}
function onUnload()
{
   delete SetPrimary;
   delete SetBonus;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   SetPrimary();
}
