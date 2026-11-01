function SetItemCount(countArg)
{
   var _loc7_ = Number(countArg);
   var _loc6_ = 0;
   var _loc3_ = 0;
   var _loc2_;
   var _loc5_;
   var _loc4_;
   while(_loc3_ < _loc7_)
   {
      _loc2_ = items.attachMovie("ChecklistItem",String(_loc3_),_loc3_);
      _loc2_.text = "$" + qualifyName(this,"Item" + String(_loc3_)) + "&outline";
      _loc2_.textClip.autoSize = "left";
      _loc5_ = _loc2_.checkbox._height;
      _loc4_ = _loc2_.textClip._height;
      if(_loc5_ > _loc4_)
      {
         _loc2_.checkbox._y = 0;
         _loc2_.textClip._y = (_loc5_ - _loc4_) / 2;
      }
      else
      {
         _loc2_.checkbox._y = (_loc4_ - _loc5_) / 2;
         _loc2_.textClip._y = 0;
      }
      _loc2_._y = _loc6_;
      _loc6_ += _loc2_._height;
      _loc3_ = _loc3_ + 1;
   }
   delete SetItemCount;
}
function SetItemChecked(indexArg, checkedArg)
{
   var _loc2_ = Number(indexArg);
   var _loc1_ = Number(checkedArg) != 0;
   var _loc3_ = items[String(_loc2_)];
   _loc3_.checkbox.gotoAndPlay(!_loc1_ ? "_unchecked" : "_checked");
}
function Move(xArg, yArg, widthArg, heightArg)
{
   _X = Number(xArg);
   _Y = Number(yArg);
   _width = Math.min(_width,Number(widthArg));
   _height = Math.min(_height,Number(heightArg));
}
function onUnload()
{
   delete SetItemCount;
   delete SetItemChecked;
   delete Move;
   delete onUnload;
}
if(!initialized)
{
   headingText = "$" + qualifyName(this,"Heading") + "&outline";
   headingClip.autoSize = "left";
   headingClip._width;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
