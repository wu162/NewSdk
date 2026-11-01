function Move(x, y, width, height)
{
   x -= horzMargin;
   width += horzMargin * 2;
   if(width < minWidth)
   {
      x -= (minWidth - width) * 0.5;
      width = minWidth;
   }
   y -= vertMargin;
   height += vertMargin * 2;
   if(height < minHeight)
   {
      y -= (minHeight - height) * 0.5;
      height = minHeight;
   }
   this._x = x;
   this._y = y;
   countdownMC._x = (width - countdownWidth) * 0.5;
   tr._x = width;
   right._x = width;
   br._x = width;
   bl._y = height;
   bottom._y = height;
   br._y = height;
   top._width = width - (tl._width + tr._width);
   center._width = width - (left._width + right._width);
   bottom._width = width - (bl._width + br._width);
   left._height = height - (tl._height + bl._height);
   center._height = height - (top._height + bottom._height);
   right._height = height - (tr._height + br._height);
   timesUp._x = width * 0.5 + timesUpOffsetX;
   timesUp._y = height * 0.5 + timesUpOffsetY;
}
function SetVisibility(vis)
{
   _visible = vis != 0;
}
function SetTimeRemaining(seconds)
{
   var _loc2_ = 0;
   while(seconds >= 60)
   {
      _loc2_ = _loc2_ + 1;
      seconds -= 60;
   }
   var _loc3_ = 0;
   while(_loc2_ >= 10)
   {
      _loc3_ = _loc3_ + 1;
      _loc2_ -= 10;
   }
   var _loc4_ = 0;
   while(seconds >= 10)
   {
      _loc4_ = _loc4_ + 1;
      seconds -= 10;
   }
   var _loc5_ = countdownMC.textMC;
   _loc5_.tenMinuteMC.gotoAndStop(_loc3_ + 1);
   _loc5_.minuteMC.gotoAndStop(_loc2_ + 1);
   _loc5_.tenSecondMC.gotoAndStop(_loc4_ + 1);
   _loc5_.secondMC.gotoAndStop(seconds + 1);
}
function ShowTimesUp()
{
   timesUp._visible = true;
}
function onUnload()
{
   delete Move;
   delete SetVisibility;
   delete SetTimeRemaining;
   delete ShowTimesUp;
   delete onUnload;
}
if(!initialized)
{
   horzMargin = 8;
   vertMargin = 8;
   var width = br._x;
   var height = br._y;
   minWidth = width;
   minHeight = height;
   timesUpOffsetX = timesUp._x - width * 0.5;
   timesUpOffsetY = timesUp._y - height * 0.5;
   countdownWidth = countdownMC._width;
   tl._visible = false;
   top._visible = false;
   tr._visible = false;
   right._visible = false;
   br._visible = false;
   bottom._visible = false;
   bl._visible = false;
   left._visible = false;
   timesUp._visible = false;
   _visible = false;
   timesUp.messageTF.text = "$TimeBombMessage&outline";
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      _visible = true;
      Move(100,100,100,200);
      SetTimeRemaining(18);
      ShowTimesUp();
   }
}
stop();
