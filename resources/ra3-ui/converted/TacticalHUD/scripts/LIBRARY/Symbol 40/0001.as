function SetVisibility(visArg)
{
   _visible = visArg != 0;
}
function SetHighlighted(highlightArg)
{
   var _loc2_ = highlightArg != 0;
   if(_loc2_)
   {
      if(highlightClip == undefined)
      {
         highlightClip = _global.CreateHUDGadgetFlash(highlightDummy);
      }
   }
   else
   {
      highlightClip.Close();
      highlightClip = undefined;
   }
}
function onUnload()
{
   highlightClip.Close();
   delete SetVisibility;
   delete SetHighlighted;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
