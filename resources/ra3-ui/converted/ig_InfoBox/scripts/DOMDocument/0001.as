function Move(x, y)
{
   var _loc4_ = _root;
   var _loc3_ = _parent;
   while(_loc3_ != _loc4_)
   {
      x -= _loc3_._x;
      y -= _loc3_._y;
      _loc3_ = _loc3_._parent;
   }
   _X = x;
   _Y = y;
}
function ShowImage()
{
   boxMC.imageMC._visible = true;
   LayoutBox();
}
function LayoutBox()
{
   var _loc2_ = messageTopMargin + messageHeight + messageBottomMargin;
   if(boxMC.imageMC._visible)
   {
      _loc2_ += boxMC.imageMC._height;
   }
   var _loc3_ = boxMC.bottomMC._y + boxMC.bottomMC._height;
   var _loc1_ = _loc3_ - _loc2_;
   boxMC.bgMC._y = _loc1_;
   boxMC.bgMC._height = _loc2_;
   boxMC.topMC._y = _loc1_;
   boxMC.middleMC._y = _loc1_ + boxMC.topMC._height;
   boxMC.middleMC._height = boxMC.bottomMC._y - boxMC.middleMC._y;
   boxMC.imageMC._y = _loc1_ + imageTopMargin;
   boxMC.messageTF._y = _loc3_ - messageBottomMargin - messageHeight;
}
function onUnload()
{
   delete Move;
   delete ShowImage;
   delete LayoutBox;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   messageTopMargin = 9;
   messageBottomMargin = 9;
   imageTopMargin = 5;
   boxMC.imageMC._visible = false;
   boxMC.messageTF.text = "$" + qualifyName(this,"Message") + "&outline";
   boxMC.messageTF.autoSize = "left";
   messageHeight = boxMC.messageTF._height;
   LayoutBox();
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
