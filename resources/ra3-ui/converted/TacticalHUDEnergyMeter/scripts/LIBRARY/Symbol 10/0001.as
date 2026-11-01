function Initialize(count)
{
   pipCount = count;
   var _loc3_ = 0;
   var _loc2_ = 0;
   var _loc1_;
   while(_loc2_ < pipCount)
   {
      _loc1_ = attachMovie("RA3PowerBarPip",String(_loc2_),_loc2_);
      _loc1_._x = _loc3_;
      _loc1_._y = 0;
      _loc3_ += _loc1_._width;
      _loc1_.gotoAndPlay("_green");
      _loc2_ = _loc2_ + 1;
   }
}
function CalculateMeterRange(production, consumption)
{
   var _loc1_ = 1000;
   while(_loc1_ < Math.max(production,consumption))
   {
      _loc1_ *= 2;
   }
   return _loc1_;
}
function GetPipIndex(intervals, meterValue, meterRange)
{
   var _loc1_ = Math.floor(meterValue / meterRange * intervals);
   return Math.max(0,Math.min(_loc1_,intervals - 1));
}
function ConfigurePips(production, consumption)
{
   var _loc7_ = CalculateMeterRange(production,consumption);
   var _loc4_ = GetPipIndex(pipCount,production,_loc7_);
   var _loc6_ = GetPipIndex(pipCount,consumption,_loc7_);
   var _loc5_ = Math.max(_loc4_,_loc6_);
   if(_loc4_ == _loc6_ && production < consumption)
   {
      _loc4_ = Math.max(0,_loc4_ - 1);
   }
   var _loc2_ = 0;
   var _loc3_;
   while(_loc2_ < pipCount)
   {
      _loc3_ = this[String(_loc2_)];
      _loc3_._visible = _loc2_ > _loc5_ ? false : true;
      _loc3_.gotoAndPlay(_loc2_ > _loc4_ ? "_red" : "_green");
      _loc2_ = _loc2_ + 1;
   }
   return this[String(_loc6_)]._x;
}
function onUnload()
{
   delete Initialize;
   delete CalculateMeterRange;
   delete GetPipIndex;
   delete ConfigurePips;
   delete onUnload;
}
if(!initialized)
{
   initialized = true;
   _parent.OnPipsLoaded(this);
}
stop();
