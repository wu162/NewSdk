function SetValues(productionArg, consumptionArg)
{
   powerBar.SetValues(productionArg,consumptionArg);
}
function SetHighlighted(highlightArg)
{
   var _loc2_ = highlightArg != 0;
   if(_loc2_)
   {
      if(highlightClip == undefined)
      {
         highlightDummy._visible = true;
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
   delete SetValues;
   delete SetHighlighted;
   delete OnPipsLoaded;
   delete onUnload;
}
if(!initialized)
{
   highlightDummy._visible = false;
   initialized = true;
   _parent.OnContentLoaded(this);
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
