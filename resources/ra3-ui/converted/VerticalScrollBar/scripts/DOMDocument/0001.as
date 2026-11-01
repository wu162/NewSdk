function OnPosChanged(pos)
{
   fscommand(qualifyName(this,"OnPosChanged"),pos);
}
function GetCurPos()
{
   return (thumb._y - bar._y) / (bar._height - thumb._height + pageSize * bar._height);
}
function NotifyPosChanged()
{
   var _loc1_ = GetCurPos();
   OnPosChanged(_loc1_);
}
function SetHeight(newHeight)
{
   if(newHeight < 73)
   {
      newHeight = 73;
   }
   var _loc4_ = GetCurPos();
   upButton._y = upButtonOffset;
   downButton._y = newHeight - downButtonOffset;
   bar._y = barTopMargin;
   bar._height = newHeight - (barTopMargin + barBottomMargin);
   thumb.SetHeight(bar._height * pageSize);
   var _loc3_ = false;
   var _loc2_ = bar._y + _loc4_ * bar._height;
   if(_loc2_ + thumb._height > bar._y + bar._height)
   {
      _loc3_ = true;
      _loc2_ = bar._y + bar._height - thumb._height;
   }
   thumb._y = _loc2_;
   _height = newHeight;
   if(_loc3_)
   {
      NotifyPosChanged();
   }
}
function SetPageSize(newPageSize)
{
   if(newPageSize < 0)
   {
      newPageSize = 0;
   }
   else if(newPageSize > 1)
   {
      newPageSize = 1;
   }
   thumb.SetHeight(bar._height * newPageSize);
   pageSize = newPageSize;
   if(thumb._y + thumb._height > bar._y + bar._height)
   {
      thumb._y = bar._y + bar._height - thumb._height;
      NotifyPosChanged();
   }
}
function SetLineSize(newLineSize)
{
   if(newLineSize < 0)
   {
      newLineSize = 0;
   }
   else if(newPageSize > 1)
   {
      newLineSize = 1;
   }
   lineSize = newLineSize;
}
function SetEnabled(enable)
{
   var _loc1_ = !enable ? "_disabled" : "_up";
   bar.gotoAndPlay(_loc1_);
   thumb.gotoAndPlay(_loc1_);
   upButton.gotoAndPlay(_loc1_);
   downButton.gotoAndPlay(_loc1_);
}
function SetVisible(vis)
{
   _visible = vis;
}
function OnThumbDrag(clip, newY)
{
   if(newY < bar._y)
   {
      newY = bar._y;
   }
   else if(newY + thumb._height > bar._y + bar._height)
   {
      newY = bar._y + bar._height - thumb._height;
   }
   if(newY != thumb._y)
   {
      thumb._y = newY;
      NotifyPosChanged();
   }
}
function SetPos(newPos)
{
   var _loc2_ = bar._y + Number(newPos) * (bar._height - pageSize * bar._height + thumb._height);
   OnThumbDrag(this,_loc2_);
}
function ThumbUp(size)
{
   var _loc1_ = thumb._y - size * bar._height;
   if(_loc1_ < bar._y)
   {
      _loc1_ = bar._y;
   }
   return _loc1_;
}
function ThumbDown(size)
{
   var _loc1_ = thumb._y + size * bar._height;
   if(_loc1_ + thumb._height > bar._y + bar._height)
   {
      _loc1_ = bar._y + bar._height - thumb._height;
   }
   return _loc1_;
}
function OnScrollButtonPress(scrollButton)
{
   var _loc1_;
   if(scrollButton == upButton)
   {
      _loc1_ = ThumbUp(lineSize);
   }
   else
   {
      _loc1_ = ThumbDown(lineSize);
   }
   if(_loc1_ != thumb._y)
   {
      thumb._y = _loc1_;
      NotifyPosChanged();
   }
}
function OnSliderBarClicked(clip)
{
   var _loc2_ = new Object();
   _loc2_.x = _X - _X;
   _loc2_.y = _Y - _Y;
   thumb.localToGlobal(_loc2_);
   var _loc3_;
   if(_root._ymouse <= _loc2_.y)
   {
      _loc3_ = ThumbUp(pageSize);
   }
   else
   {
      _loc3_ = ThumbDown(pageSize);
   }
   if(_loc3_ != thumb._y)
   {
      thumb._y = _loc3_;
      NotifyPosChanged();
   }
}
function onUnload()
{
   delete OnPosChanged;
   delete GetCurPos;
   delete NotifyPosChanged;
   delete SetHeight;
   delete SetPageSize;
   delete SetLineSize;
   delete SetEnabled;
   delete SetVisible;
   delete OnThumbDrag;
   delete SetPos;
   delete ThumbUp;
   delete ThumbDown;
   delete OnScrollButtonPress;
   delete OnSliderBarClicked;
   delete onUnload;
}
param = _parent.GetScrollBarParam(this);
