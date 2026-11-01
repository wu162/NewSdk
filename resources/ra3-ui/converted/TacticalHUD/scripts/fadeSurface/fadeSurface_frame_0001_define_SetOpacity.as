function SetOpacity(opacity)
{
   box._alpha = opacity * 100;
   _visible = opacity > 0;
}
function onUnload()
{
   delete SetOpacity;
   delete onUnload;
}
if(!initialized)
{
   _visible = false;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
