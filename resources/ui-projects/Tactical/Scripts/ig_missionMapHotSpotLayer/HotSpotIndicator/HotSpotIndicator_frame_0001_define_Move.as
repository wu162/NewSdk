function Move(newX, newY)
{
   _parent.MoveIndicator(this,newX,newY);
}
function OnBound()
{
   if(initialized)
   {
      fscommand(qualifyName(this,"OnInitialized"),"");
   }
}
function OnDestroying()
{
   if(initialized)
   {
      fscommand(qualifyName(this,"OnClipDestroyed"),"");
   }
}
function onUnload()
{
   delete Move;
   delete OnBound;
   delete OnDestroying;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
