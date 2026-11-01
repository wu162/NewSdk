function GetMousePosString()
{
   var _loc3_ = _xmouse + _parent._width / 2;
   var _loc2_ = _ymouse + _parent._height / 2;
   return "x=" + String(_loc3_) + "&y=" + String(_loc2_);
}
function onMouseMove()
{
   if(mouseOver)
   {
      fscommand(qualifyName(_parent,"OnMouseMove"),GetMousePosString());
   }
}
function onUnload()
{
   delete onMouseMove;
   delete onUnload;
   delete GetMousePosString;
}
