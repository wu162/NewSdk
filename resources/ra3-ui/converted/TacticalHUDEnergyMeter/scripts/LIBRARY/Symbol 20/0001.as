function OnPipsLoaded(mc)
{
   pipsClip = mc;
   pipsClip.Initialize(_pipCount);
   var _loc3_ = 3;
   var _loc2_ = 3;
   button._x = - _loc3_;
   button._y = - _loc2_;
   button._xscale = _loc3_ * 2 + pipsClip._width;
   button._yscale = _loc2_ * 2 + pipsClip._height;
   if(!extern.InGame)
   {
      _parent.SetValues(1130,1132);
   }
}
function SetValues(production, consumption)
{
   marker._x = pipsClip.ConfigurePips(production,consumption);
   if(consumption <= production)
   {
      SetAlertMode(false);
   }
   else
   {
      SetAlertMode(true);
   }
}
function SetAlertMode(newMode)
{
   if(alertMode != newMode)
   {
      alertMode = newMode;
      if(alertMode == true)
      {
         gotoAndStop("_pulse");
         play();
      }
      else
      {
         gotoAndStop("_steady");
         play();
      }
   }
}
function onUnload()
{
   delete OnPipsLoaded;
   delete SetValues;
   delete SetAlertMode;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   SetAlertMode(false);
}
