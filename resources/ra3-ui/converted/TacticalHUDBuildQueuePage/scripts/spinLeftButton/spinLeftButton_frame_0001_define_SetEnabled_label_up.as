function SetEnabled(enable)
{
   if(enable != isEnabled)
   {
      if(enable)
      {
         gotoAndStop("_up");
         play();
      }
      else
      {
         gotoAndStop("_disabled");
         play();
      }
      isEnabled = enable;
   }
}
function onUnload()
{
   delete SetEnabled;
   delete onUnload;
}
if(!initialized)
{
   if(!isEnabled)
   {
      gotoAndStop("_disabled");
      play();
   }
   initialized = true;
}
