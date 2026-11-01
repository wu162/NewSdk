function SetVisibility(vis)
{
   _visible = vis != 0;
}
function Move(x, y)
{
   _X = x;
   _Y = y;
}
function PlaySelectionEffect()
{
   selectionEffect.gotoAndPlay("_play");
}
function onUnload()
{
   delete SetVisibility;
   delete Move;
   delete PlaySelectionEffect;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
