function Move(x, y)
{
   if(x < -5000)
   {
      x = -5000;
   }
   else if(x > 5000)
   {
      x = 5000;
   }
   if(y < -5000)
   {
      y = -5000;
   }
   else if(y > 5000)
   {
      y = 5000;
   }
   var _loc6_ = _root;
   var _loc3_ = _parent;
   while(_loc3_ != _loc6_)
   {
      x -= _loc3_._x;
      y -= _loc3_._y;
      _loc3_ = _loc3_._parent;
   }
   _X = x;
   _Y = y;
}
function Close()
{
   gotoAndStop("_close");
   play();
}
function onUnload()
{
   delete Move;
   delete Close;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
