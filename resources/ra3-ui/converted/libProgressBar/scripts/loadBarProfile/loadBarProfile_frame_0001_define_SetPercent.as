function SetPercent(value)
{
   _visible = value > 0;
   trace("Set Percent:" + value);
   targetLevel = value;
   var _loc1_ = barFullSize._width;
   var _loc4_ = _loc1_ / 100;
   var _loc3_ = targetLevel / _loc1_;
   targetSize = _loc1_ * _loc3_ * _loc4_;
   gotoAndPlay(2);
}
SetPercent(_root.GetExtern(_name));
trace("Hello");
