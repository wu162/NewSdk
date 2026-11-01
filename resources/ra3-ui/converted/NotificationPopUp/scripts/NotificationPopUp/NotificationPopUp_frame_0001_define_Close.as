function Close()
{
   if(!closing)
   {
      gotoAndStop("_close");
      play();
      closing = true;
   }
}
function onUnload()
{
   delete Close;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   closing = false;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
