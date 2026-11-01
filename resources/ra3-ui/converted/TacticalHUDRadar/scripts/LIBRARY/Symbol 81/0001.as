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
function AddIndicator(idArg, imageLabelArg)
{
   var _loc3_ = Number(idArg);
   var _loc4_ = String(imageLabelArg);
   var _loc2_ = this[String(_loc3_)];
   if(_loc2_ != undefined)
   {
      DestroyIndicator(_loc2_);
   }
   _loc2_ = attachMovie("BuildQueueIndicator",String(_loc3_),_loc3_);
   _loc2_.imageLabel = _loc4_;
   _loc2_.relX = 0;
   _loc2_.relY = 0;
   indicators.push(_loc2_);
}
function RemoveIndicator(idArg)
{
   var _loc3_ = Number(idArg);
   var _loc2_ = this[String(_loc3_)];
   DestroyIndicator(_loc2_);
}
function MoveIndicator(idArg, relXArg, relYArg)
{
   var _loc5_ = Number(idArg);
   var _loc4_ = Number(relXArg);
   var _loc3_ = Number(relYArg);
   var _loc2_ = this[String(_loc5_)];
   _loc2_.relX = _loc4_;
   _loc2_.relY = _loc3_;
   _loc2_._x = _loc4_ * sizer._width;
   _loc2_._y = _loc3_ * sizer._height;
}
function DestroyIndicator(indicator)
{
   var _loc1_ = 0;
   while(_loc1_ < indicators.length)
   {
      if(indicators[_loc1_] == indicator)
      {
         indicators.splice(_loc1_,1);
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
   indicator.removeMovieClip();
}
function onUnload()
{
   delete Resize;
   delete AddIndicator;
   delete RemoveIndicator;
   delete MoveIndicator;
   delete DestroyIndicator;
   delete onUnload;
}
if(!initialized)
{
   indicators = new Array();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      AddIndicator(0,"Def");
      MoveIndicator(0,0.25,0.5);
      AddIndicator(1,"Air");
      MoveIndicator(1,0.75,0.5);
   }
}
stop();
