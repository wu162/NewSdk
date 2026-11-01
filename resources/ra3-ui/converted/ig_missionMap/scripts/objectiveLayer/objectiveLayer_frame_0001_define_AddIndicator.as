function AddIndicator(category, isBonus)
{
   var _loc3_ = nextIndicatorId;
   nextIndicatorId++;
   var _loc2_ = this.attachMovie("ObjectiveIndicator",String(_loc3_),_loc3_);
   _loc2_.category = category;
   _loc2_.isBonus = isBonus != 0;
   _loc2_.relX = 0;
   _loc2_.relY = 0;
   indicators.push(_loc2_);
   return Pointer.encode(_loc2_);
}
function RemoveIndicator(indicator)
{
   var _loc2_ = indicators.length;
   var _loc1_ = 0;
   while(_loc1_ < _loc2_)
   {
      if(indicator == indicators[_loc1_])
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
function MoveIndicator(indicator, newX, newY)
{
   indicator.relX = newX;
   indicator.relY = newY;
   indicator._x = sizer._width * newX;
   indicator._y = sizer._height * newY;
}
function Resize(newWidth, newHeight)
{
   sizer._width = newWidth;
   sizer._height = newHeight;
   var _loc3_ = indicators.length;
   var _loc2_ = 0;
   var _loc1_;
   while(_loc2_ < _loc3_)
   {
      _loc1_ = indicators[_loc2_];
      _loc1_._x = _loc1_.relX * newWidth;
      _loc1_._y = _loc1_.relY * newHeight;
      _loc2_ = _loc2_ + 1;
   }
}
function OnBound()
{
   if(initialized)
   {
      fscommand(qualifyName(this,"OnInitialized"),"");
   }
}
function OnDestroying()
{
   var _loc3_ = indicators.length;
   var _loc2_ = 0;
   while(_loc2_ < _loc3_)
   {
      indicators[_loc2_].OnDestroying();
      _loc2_ = _loc2_ + 1;
   }
   if(initialized)
   {
      fscommand(qualifyName(this,"OnClipDestroyed"),"");
   }
}
function onUnload()
{
   delete AddIndicator;
   delete RemoveIndicator;
   delete MoveIndicator;
   delete OnBound;
   delete OnDestroying;
   delete onUnload;
}
if(!initialized)
{
   indicators = new Array();
   nextIndicatorId = 0;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
