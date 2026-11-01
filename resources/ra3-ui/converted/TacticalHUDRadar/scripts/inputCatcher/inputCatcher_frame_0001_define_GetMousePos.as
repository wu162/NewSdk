function GetMousePos()
{
   var _loc7_ = _root;
   var _loc6_ = 0;
   var _loc5_ = 0;
   var _loc9_ = 1;
   var _loc8_ = 1;
   ancestor = _parent._parent;
   var _loc3_;
   var _loc4_;
   while(ancestor != _loc7_)
   {
      _loc3_ = ancestor._xscale * 0.009999999776482582;
      _loc6_ *= _loc3_;
      _loc6_ -= ancestor._x;
      _loc9_ /= _loc3_;
      _loc4_ = ancestor._yscale * 0.009999999776482582;
      _loc5_ *= _loc4_;
      _loc5_ -= ancestor._y;
      _loc8_ /= _loc4_;
      ancestor = ancestor._parent;
   }
   return {x:(_loc7_._xmouse + _loc6_) * _loc9_,y:(_loc7_._ymouse + _loc5_) * _loc8_};
}
function GetMousePosString()
{
   var _loc2_ = GetMousePos();
   var _loc4_ = (_loc2_.x - _parent._parent.mapX) / _parent._parent.mapWidth;
   var _loc3_ = (_loc2_.y - _parent._parent.mapY) / _parent._parent.mapHeight;
   return "x=" + String(_loc4_) + "&y=" + String(_loc3_);
}
function onMouseMove()
{
   var _loc3_;
   var _loc2_;
   if(isMouseOver || isDragging)
   {
      _loc3_ = _parent._parent._xmouse;
      _loc2_ = _parent._parent._ymouse;
      if(_loc3_ != lastXMouse || _loc2_ != lastYMouse)
      {
         lastXMouse = _loc3_;
         lastYMouse = _loc2_;
         fscommand(qualifyName(_parent._parent,"OnMove"),GetMousePosString());
      }
   }
}
function onMouseDown()
{
   if(isMouseOver)
   {
      fscommand(qualifyName(_parent._parent,"OnPress"),GetMousePosString());
      isDragging = true;
   }
}
function onMouseUp()
{
   if(isDragging)
   {
      fscommand(qualifyName(_parent._parent,"OnRelease"),GetMousePosString());
      isDragging = false;
   }
}
function onUnload()
{
   delete GetMousePos;
   delete GetMousePosString;
   delete onMouseMove;
   delete onMouseDown;
   delete onMouseUp;
   delete onUnload;
}
