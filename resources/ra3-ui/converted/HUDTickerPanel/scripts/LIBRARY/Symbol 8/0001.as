function SetImageSize(widthArg, heightArg)
{
   var _loc1_ = Number(widthArg);
   var _loc4_ = Number(heightArg);
   var _loc3_ = imageRenderer._width;
   var _loc2_ = imageRenderer._height;
   var _loc5_;
   var _loc6_;
   if(_loc1_ * _loc2_ > _loc3_ * _loc4_)
   {
      _loc5_ = _loc3_ * _loc4_ / _loc1_;
      imageRenderer._y += (_loc2_ - _loc5_) * 0.5;
      imageRenderer._height = _loc5_;
   }
   else
   {
      _loc6_ = _loc2_ * _loc1_ / _loc4_;
      imageRenderer._x += (_loc3_ - _loc6_) * 0.5;
      imageRenderer._width = _loc6_;
   }
}
function onUnload()
{
   delete SetImageSize;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
