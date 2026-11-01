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
         _loc1_.iconMC._height = newWidth * _loc1_.widthScale;
         _loc1_.iconMC._width = newWidth * _loc1_.lengthScale;
      }
      _loc2_ = _loc2_ + 1;
   }
}
function CreateIndicator(endCapArg)
{
   var _loc2_ = nextIndicatorId++;
   var _loc1_ = attachMovie("BridgeSegmentIndicator",String(_loc2_),_loc2_);
   _loc1_.isEndCap = endCapArg != 0;
   _loc1_.relX = 0;
   _loc1_.relY = 0;
   _loc1_.widthScale = 1;
   _loc1_.lengthScale = 1;
   indicators.push(_loc1_);
   return Pointer.encode(_loc1_);
}
function DestroyIndicator(indicator)
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
function MoveIndicator(indicator, relX, relY)
{
   indicator.relX = relX;
   indicator.relY = relY;
   indicator._x = relX * sizer._width;
   indicator._y = relY * sizer._height;
}
function ScaleIndicator(indicator, widthScale, lengthScale)
{
   lengthScale *= 1.2000000476837158;
   indicator.widthScale = widthScale;
   indicator.lengthScale = lengthScale;
   indicator.iconMC._height = sizer._width * widthScale;
   indicator.iconMC._width = sizer._width * lengthScale;
}
function OnBound()
{
   if(initialized)
   {
      fscommand(_global.qualifyName(this,"OnInitialized"),"");
   }
}
function OnDestroying()
{
   fscommand(_global.qualifyName(this,"OnClipDestroyed"),"");
}
function onUnload()
{
   delete Resize;
   delete CreateIndicator;
   delete DestroyIndicator;
   delete MoveIndicator;
   delete OnBound;
   delete OnDestroying;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   nextIndicatorId = 0;
   indicators = new Array();
   initialized = true;
   fscommand(_global.qualifyName(this,"OnInitialized"),"");
}
stop();
