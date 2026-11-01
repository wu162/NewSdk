function Resize(newWidth, newHeight)
{
   sizer._width = newWidth;
   sizer._height = newHeight;
   var _loc2_ = 0;
   var _loc1_;
   while(_loc2_ < indicators.length)
   {
      _loc1_ = indicators[_loc2_];
      if(_loc1_ != undefined && _loc1_.relX != undefined && _loc1_.relY != undefined)
      {
         _loc1_._x = _loc1_.relX * newWidth;
         _loc1_._y = _loc1_.relY * newHeight;
      }
      _loc2_ = _loc2_ + 1;
   }
}
function AddIndicator()
{
   var _loc3_ = nextIndicatorId++;
   var _loc2_ = this.attachMovie("AITargetIndicator",String(_loc3_),_loc3_);
   _loc2_.relX = 0;
   _loc2_.relY = 0;
   indicators.push(_loc2_);
   return Pointer.encode(_loc2_);
}
function RemoveIndicator(indicator)
{
   var _loc1_ = 0;
   while(_loc1_ < indicators.length)
   {
      if(indicators[_loc1_] == indicator)
      {
         indicators.splice(_loc1_,1);
         if(indicators.length == 0)
         {
            nextIndicatorId = 0;
         }
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
   indicator.removeMovieClip();
}
function OnDestroyed()
{
   fscommand(qualifyName(this,"OnDestroyed"),"");
}
function onUnload()
{
   delete Resize;
   delete AddIndicator;
   delete RemoveIndicator;
   delete OnDestroyed;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   nextIndicatorId = 0;
   indicators = new Array();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
