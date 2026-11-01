function AddOnScreenIndicator(category, bonusArg)
{
   var _loc2_ = nextOnScreenIndicatorId++;
   var _loc1_ = onScreen.attachMovie("OnScreenIndicator",String(_loc2_),_loc2_);
   _loc1_.category = category;
   _loc1_.isBonus = bonusArg != 0;
   onScreenIndicatorCount++;
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
}
function RemoveOnScreenIndicator(indicator)
{
   indicator.removeMovieClip();
   if(--onScreenIndicatorCount == 0)
   {
      nextOnScreenIndicatorId = 0;
   }
}
function AddOffScreenIndicator(category, bonusArg)
{
   var _loc2_ = nextOffScreenIndicatorId++;
   var _loc1_ = offScreen.attachMovie("OffScreenIndicator",String(_loc2_),_loc2_);
   _loc1_.category = category;
   _loc1_.isBonus = bonusArg != 0;
   _loc1_.SetBonus(bonusArg != 0);
   offScreenIndicatorCount++;
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
}
function RemoveOffScreenIndicator(indicator)
{
   indicator.removeMovieClip();
   if(--offScreenIndicatorCount == 0)
   {
      nextOffScreenIndicatorId = 0;
   }
}
function onUnload()
{
   delete AddOnScreenIndicator;
   delete RemoveOnScreenIndicator;
   delete AddOffScreenIndicator;
   delete RemoveOffScreenIndicator;
   delete onUnload;
}
if(inGame == undefined)
{
   inGame = extern.InGame;
}
if(!initialized)
{
   nextOnScreenIndicatorId = 0;
   onScreenIndicatorCount = 0;
   nextOffScreenIndicatorId = 0;
   offScreenIndicatorCount = 0;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!inGame)
   {
      AddOnScreenIndicator("Capture",1);
      AddOffScreenIndicator("Protect",1);
      AddOffScreenIndicator("Capture",0);
   }
}
stop();
