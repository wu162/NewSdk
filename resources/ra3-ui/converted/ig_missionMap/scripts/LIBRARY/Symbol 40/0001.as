function Move(newX, newY)
{
   _parent.MoveIndicator(this,newX,newY);
}
function ColorizeDot()
{
   var _loc1_ = new Color(dot);
   _loc1_.setRGB(indicatorColor);
}
function SetColor(newColor)
{
   indicatorColor = newColor;
   ColorizeDot();
}
function SetRevealed(arg)
{
   gotoAndPlay(arg == 0 ? "Shrouded" : "Revealed");
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
   delete SetRevealed;
   delete OnBound;
   delete OnDestroying;
   delete onUnload;
}
if(!initialized)
{
   indicatorColor = 16777215;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
