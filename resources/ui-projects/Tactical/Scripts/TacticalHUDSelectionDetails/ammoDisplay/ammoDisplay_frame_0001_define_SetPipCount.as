function SetPipCount(newPipCount)
{
   var _loc6_;
   var _loc7_;
   var _loc3_;
   var _loc2_;
   var _loc4_;
   if(newPipCount > pipCount)
   {
      if(pipCount > 0)
      {
         _loc7_ = this[String(pipCount - 1)];
         _loc6_ = _loc7_._x + _loc7_._width + 1;
      }
      else
      {
         _loc6_ = 0;
      }
      _loc3_ = pipCount;
      while(_loc3_ < newPipCount)
      {
         _loc2_ = this.attachMovie("AmmoPip",String(_loc3_),_loc3_);
         _loc2_._x = _loc6_;
         _loc2_._y = 0;
         _loc2_.gotoAndPlay(_loc3_ >= currentLoad ? "_empty" : "_filled");
         _loc6_ += _loc2_._width + 1;
         _loc3_ = _loc3_ + 1;
      }
   }
   else if(newPipCount < pipCount)
   {
      _loc3_ = pipCount;
      do
      {
         _loc3_ = _loc3_ - 1;
         _loc4_ = this[String(_loc3_)];
         _loc4_.removeMovieClip();
      }
      while(_loc3_ > newPipCount);
   }
   pipCount = newPipCount;
}
function SetCapacity(ammoCapacity)
{
   var _loc1_ = ammoCapacity <= maxPipCount;
   ammoBar._visible = !_loc1_;
   ammoBarBG._visible = !_loc1_;
   SetPipCount(!_loc1_ ? 0 : ammoCapacity);
   currentCapacity = ammoCapacity;
}
function SetLoad(ammoLoad)
{
   var _loc7_ = currentCapacity <= maxPipCount;
   var _loc5_;
   var _loc2_;
   var _loc3_;
   if(_loc7_)
   {
      if(ammoLoad > currentLoad)
      {
         if(currentLoad < pipCount)
         {
            _loc5_ = Math.min(ammoLoad,pipCount);
            _loc2_ = currentLoad;
            while(_loc2_ < _loc5_)
            {
               _loc3_ = this[String(_loc2_)];
               _loc3_.gotoAndPlay("_filled");
               _loc2_ = _loc2_ + 1;
            }
         }
      }
      else if(ammoLoad < currentLoad)
      {
         if(ammoLoad < pipCount)
         {
            _loc5_ = Math.min(currentLoad,pipCount);
            _loc2_ = ammoLoad;
            while(_loc2_ < _loc5_)
            {
               _loc3_ = this[String(_loc2_)];
               _loc3_.gotoAndPlay("_empty");
               _loc2_ = _loc2_ + 1;
            }
         }
      }
   }
   var _loc6_;
   if(currentCapacity != 0)
   {
      _loc6_ = ammoLoad / currentCapacity * 100;
   }
   else
   {
      _loc6_ = 0;
   }
   ammoBar.gotoAndStop(_loc6_ + 1);
   currentLoad = ammoLoad;
}
function onUnload()
{
   delete SetPipCount;
   delete SetCapacity;
   delete SetLoad;
   delete onUnload;
}
if(!initialized)
{
   maxPipCount = 10;
   pipCount = 0;
   currentCapacity = 0;
   currentLoad = 0;
   SetCapacity(Number(_parent.ammoCapacity));
   SetLoad(Number(_parent.ammoLoad));
   initialized = true;
}
