function CreateBeaconIndicator(indicatorType)
{
   indicatorCount++;
   var _loc2_ = nextIndicatorId++;
   var _loc3_ = beacon.attachMovie(indicatorType,String(_loc2_),_loc2_);
   _loc3_.client = this;
   return _loc3_;
}
function CreateMpBeaconIndicator(beaconColor)
{
   var _loc1_ = CreateBeaconIndicator("MpBeaconIndicator");
   _loc1_.beaconColor = beaconColor;
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
}
function CreateAIAssaultBeaconIndicator()
{
   var _loc1_ = CreateBeaconIndicator("AIAssaultBeaconIndicator");
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
}
function CreateAIHoldBeaconIndicator()
{
   var _loc1_ = CreateBeaconIndicator("AIHoldBeaconIndicator");
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
}
function DestroyBeaconIndicator(indicator)
{
   indicator.removeMovieClip();
   if(--indicatorCount == 0)
   {
      nextIndicatorId = 0;
   }
}
function CreateTimeBombFrame()
{
   var _loc1_ = CreateBeaconIndicator("TimeBombFrame");
   return !inGame ? _loc1_ : Pointer.encode(_loc1_);
}
function onUnload()
{
   delete CreateBeaconIndicator;
   delete CreateMpBeaconIndicator;
   delete CreateAIAssaultBeaconIndicator;
   delete CreateAIHoldBeaconIndicator;
   delete DestroyBeaconIndicator;
   delete CreateTimeBombFrame;
   delete onUnload;
}
inGame = extern.InGame;
if(!initialized)
{
   nextIndicatorId = 0;
   indicatorCount = 0;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!inGame)
   {
      CreateMpBeaconIndicator(16711935);
      CreateTimeBombFrame();
   }
}
stop();
