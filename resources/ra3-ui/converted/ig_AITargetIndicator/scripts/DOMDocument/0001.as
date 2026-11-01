function Move(x, y)
{
   var _loc4_ = _root;
   var _loc3_ = _parent;
   while(_loc3_ != _loc4_)
   {
      x -= _loc3_._x;
      y -= _loc3_._y;
      _loc3_ = _loc3_._parent;
   }
   _X = x;
   _Y = y;
}
function SetDisposition(disposition)
{
   gotoAndPlay(disposition);
}
function onUnload()
{
   delete Move;
   delete SetDisposition;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
