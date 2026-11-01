function Close()
{
   gotoAndStop("_close");
   play();
}
function onUnload()
{
   delete Close;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
