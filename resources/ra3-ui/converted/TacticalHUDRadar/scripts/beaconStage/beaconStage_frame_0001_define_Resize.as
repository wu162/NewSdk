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
function AddMpBeaconIndicator(beaconColor)
{
   var _loc3_ = nextIndicatorId++;
   var _loc2_ = this.attachMovie("MpBeaconIndicator",String(_loc3_),_loc3_);
   _loc2_.beaconColor = beaconColor;
   _loc2_.relX = 0;
   _loc2_.relY = 0;
   indicators.push(_loc2_);
   return !inGame ? _loc2_ : Pointer.encode(_loc2_);
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
function MoveIndicator(indicator, relX, relY)
{
   indicator.relX = relX;
   indicator.relY = relY;
   indicator._x = relX * sizer._width;
   indicator._y = relY * sizer._height;
}
function onUnload()
{
   delete Resize;
   delete AddMpBeaconIndicator;
   delete RemoveIndicator;
   delete MoveIndicator;
   delete onUnload;
}
inGame = extern.InGame;
if(!initialized)
{
   nextIndicatorId = 0;
   indicators = new Array();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!inGame)
   {
      MoveIndicator(AddMpBeaconIndicator(65280),0.8999999761581421,0.8999999761581421);
   }
}
stop();
