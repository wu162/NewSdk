function Calibrate(orangeThresholdArg, redThresholdArg)
{
   orangeThreshold = Number(orangeThresholdArg);
   redThreshold = Number(redThresholdArg);
}
function GetPipIndex(intervals, meterValue, meterRange)
{
   var _loc1_ = Math.floor(meterValue / meterRange * intervals);
   return Math.max(0,Math.min(_loc1_,intervals));
}
function SetValue(valArg)
{
   meterMask._visible = false;
   var _loc3_ = 14;
   var _loc4_ = GetPipIndex(_loc3_,Number(valArg),1);
   var _loc1_ = 0;
   var _loc2_;
   while(_loc1_ < _loc3_)
   {
      _loc2_ = meter[String(_loc1_)];
      _loc2_._visible = _loc1_ < _loc4_;
      _loc1_ = _loc1_ + 1;
   }
}
function SetValueUsingMask(valArg)
{
   var _loc1_ = Number(valArg);
   var _loc2_ = 0.44999998807907104;
   var _loc4_ = 0.8899999856948853;
   var _loc3_;
   if(_loc1_ < orangeThreshold)
   {
      _loc3_ = _loc1_ / orangeThreshold * _loc2_;
   }
   else if(_loc1_ < redThreshold)
   {
      _loc3_ = _loc2_ + (_loc1_ - orangeThreshold) / (redThreshold - orangeThreshold) * (_loc4_ - _loc2_);
   }
   else
   {
      _loc3_ = _loc4_ + (_loc1_ - redThreshold) / (1 - redThreshold) * (1 - _loc4_);
   }
   meterMask._xscale = 100 * _loc3_;
}
function SetVisibility(visArg)
{
   _visible = Number(visArg) != 0;
}
function GetHelpSite()
{
   return Pointer.encode(helpSite);
}
function onUnload()
{
   delete Calibrate;
   delete GetPipIndex;
   delete SetValue;
   delete SetValueUsingMask;
   delete SetVisibility;
   delete GetHelpSite;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      Calibrate("0.33","0.67");
      SetValue("0.9");
   }
}
