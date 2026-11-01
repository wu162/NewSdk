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
function AddIndicator(category, bonusArg)
{
   var _loc2_ = nextIndicatorId++;
   var _loc1_ = attachMovie("ObjectiveIndicator",String(_loc2_),_loc2_);
   _loc1_.category = category;
   _loc1_.isBonus = bonusArg != 0;
   _loc1_.relX = 0;
   _loc1_.relY = 0;
   indicators.push(_loc1_);
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
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
function OnDestroyed()
{
   fscommand(qualifyName(this,"OnDestroyed"),"");
}
function onUnload()
{
   delete Resize;
   delete AddIndicator;
   delete RemoveIndicator;
   delete MoveIndicator;
   delete OnDestroyed;
   delete onUnload;
}
if(inGame == undefined)
{
   inGame = extern.InGame;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   nextIndicatorId = 0;
   indicators = new Array();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!inGame)
   {
      MoveIndicator(AddIndicator("Move",0),0.25,0.75);
      MoveIndicator(AddIndicator("Attack",1),0.75,0.75);
   }
}
stop();
