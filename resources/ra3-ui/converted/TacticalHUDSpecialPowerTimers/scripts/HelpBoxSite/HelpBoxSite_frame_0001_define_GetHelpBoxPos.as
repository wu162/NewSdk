function GetHelpBoxPos()
{
   var _loc4_ = {x:_X,y:_Y};
   var _loc5_ = _root;
   var _loc3_ = _parent;
   while(_loc3_ != _loc5_)
   {
      _loc4_.x *= _loc3_._xscale * 0.009999999776482582;
      _loc4_.y *= _loc3_._yscale * 0.009999999776482582;
      _loc4_.x += _loc3_._x;
      _loc4_.y += _loc3_._y;
      _loc3_ = _loc3_._parent;
   }
   return _loc4_;
}
function GetHelpBoxVerticalAlignment()
{
   return vertAlignment;
}
function SetHelpBoxVerticalAlignment(newAlignment)
{
   vertAlignment = newAlignment;
}
function GetHelpBoxHorizontalAlignment()
{
   return horzAlignment;
}
function SetHelpBoxHorizontalAlignment(newAlignment)
{
   horzAlignment = newAlignment;
}
function onUnload()
{
   delete GetHelpBoxPos;
   delete GetHelpBoxVerticalAlignment;
   delete SetHelpBoxVerticalAlignment;
   delete GetHelpBoxHorizontalAlignment;
   delete SetHelpBoxHorizontalAlignment;
   delete onUnload;
}
