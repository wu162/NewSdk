function Close()
{
   if(!closing)
   {
      closing = true;
      gotoAndStop("_close");
      play();
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
