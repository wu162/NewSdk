function Move(newX, newY)
{
   _parent.MoveIndicator(this,newX,newY);
}
function SetColor(newColor)
{
   var _loc2_ = new Color(this);
   _loc2_.setRGB(newColor);
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
   delete SetColor;
   delete OnBound;
   delete OnDestroying;
   delete onUnload;
}
if(!initialized)
{
   gotoAndPlay(structureType);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
