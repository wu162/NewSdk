function GetNumRowsInColumn(col)
{
   return numRowsInFirstColumn;
}
function GetColumnVerticalOffset(col)
{
   return buttonSlotHeight * (numRowsInFirstColumn - GetNumRowsInColumn(col)) * 0.5;
}
function ComputeSlotPositionFromIndex(index)
{
   var _loc1_ = numRowsInFirstColumn;
   var _loc3_ = 0;
   var _loc2_ = index;
   while(_loc2_ >= _loc1_)
   {
      _loc3_ = _loc3_ + 1;
      _loc2_ -= _loc1_;
   }
   return {row:_loc2_,col:_loc3_};
}
function CreateNewButtonSlot(slotIndex, slotX, slotY)
{
   var _loc2_ = nextButtonSlotId++;
   var _loc5_ = String(_loc2_);
   attachMovie("LoadMovieStage",String(_loc2_),2048 - _loc2_);
   var _loc4_ = this[_loc5_];
   _loc4_._x = slotX;
   _loc4_._y = slotY;
   _loc5_ = "helpSite" + String(_loc2_);
   attachMovie("HelpBoxSite","helpSite" + String(_loc2_),_loc2_ + 65536);
   var _loc3_ = this[_loc5_];
   _loc3_._x = slotX + buttonStageWidth;
   _loc3_._y = slotY;
   _loc3_.vertAlignment = "top";
   _loc3_.horzAlignment = "left";
   _loc4_.helpSite = _loc3_;
   var _loc6_ = {stage:_loc4_,helpSite:_loc3_};
   buttonSlots.splice(slotIndex,0,_loc6_);
   fscommand(qualifyName(this,"OnButtonSlotStageCreated"),"index=" + slotIndex + "&name=" + Pointer.encode(_loc4_));
   return _loc6_;
}
function MoveButtonSlot(slotIndex, slotX, slotY)
{
   var _loc1_ = buttonSlots[slotIndex];
   var _loc3_ = _loc1_.stage;
   _loc3_._x = slotX;
   _loc3_._y = slotY;
   var _loc2_ = _loc1_.helpSite;
   _loc2_._x = slotX + buttonStageWidth;
   _loc2_._y = slotY;
}
function InsertButtonSlot(index)
{
   var _loc7_ = ComputeSlotPositionFromIndex(index);
   var _loc1_ = _loc7_.col;
   var _loc2_ = _loc7_.row;
   var _loc4_ = GetColumnVerticalOffset(_loc1_) + _loc2_ * buttonSlotHeight;
   var _loc5_ = _loc1_ * columnHorizontalOffset;
   CreateNewButtonSlot(index,_loc5_,_loc4_);
   var _loc6_ = GetNumRowsInColumn(_loc1_);
   index = index + 1;
   while(index < buttonSlots.length)
   {
      _loc2_ = _loc2_ + 1;
      if(_loc2_ < _loc6_)
      {
         _loc4_ += buttonSlotHeight;
      }
      else
      {
         _loc1_ = _loc1_ + 1;
         _loc2_ = 0;
         _loc4_ = GetColumnVerticalOffset(_loc1_);
         _loc5_ += columnHorizontalOffset;
         _loc6_ = GetNumRowsInColumn(_loc1_);
      }
      MoveButtonSlot(index,_loc5_,_loc4_);
      index = index + 1;
   }
}
function EraseButtonSlot(index)
{
   var _loc9_ = buttonSlots[index];
   var _loc7_ = _loc9_.stage;
   var _loc8_ = ComputeSlotPositionFromIndex(index);
   var _loc2_ = _loc8_.col;
   var _loc3_ = _loc8_.row;
   var _loc5_ = _loc7_._x;
   var _loc4_ = _loc7_._y;
   buttonSlots.splice(index,1);
   _loc7_.removeMovieClip();
   _loc9_.helpSite.removeMovieClip();
   var _loc6_;
   if(buttonSlots.length > 0)
   {
      if(index < buttonSlots.length)
      {
         _loc6_ = GetNumRowsInColumn(_loc2_);
         while(true)
         {
            MoveButtonSlot(index,_loc5_,_loc4_);
            index = index + 1;
            if(index >= buttonSlots.length)
            {
               break;
            }
            _loc3_ = _loc3_ + 1;
            if(_loc3_ < _loc6_)
            {
               _loc4_ += buttonSlotHeight;
            }
            else
            {
               _loc2_ = _loc2_ + 1;
               _loc3_ = 0;
               _loc4_ = GetColumnVerticalOffset(_loc2_);
               _loc5_ += columnHorizontalOffset;
               _loc6_ = GetNumRowsInColumn(_loc2_);
            }
         }
      }
   }
   else
   {
      nextButtonSlotId = 0;
   }
}
function SetButtonSlotCount(newCount)
{
   var _loc1_;
   var _loc8_;
   var _loc2_;
   var _loc3_;
   var _loc4_;
   var _loc5_;
   var _loc7_;
   if(newCount < buttonSlots.length)
   {
      do
      {
         _loc1_ = buttonSlots.pop();
         _loc1_.stage.removeMovieClip();
         _loc1_.helpSite.removeMovieClip();
      }
      while(buttonSlots.length > newCount);
      if(buttonSlots.length == 0)
      {
         nextButtonSlotId = 0;
      }
   }
   else if(newCount > buttonSlots.length)
   {
      _loc8_ = ComputeSlotPositionFromIndex(buttonSlots.length);
      _loc2_ = _loc8_.col;
      _loc3_ = _loc8_.row;
      _loc4_ = GetColumnVerticalOffset(_loc2_) + _loc3_ * buttonSlotHeight;
      _loc5_ = _loc2_ * columnHorizontalOffset;
      _loc7_ = GetNumRowsInColumn(_loc2_);
      while(true)
      {
         CreateNewButtonSlot(buttonSlots.length,_loc5_,_loc4_);
         if(buttonSlots.length >= newCount)
         {
            break;
         }
         _loc3_ = _loc3_ + 1;
         if(_loc3_ < _loc7_)
         {
            _loc4_ += buttonSlotHeight;
         }
         else
         {
            _loc2_ = _loc2_ + 1;
            _loc3_ = 0;
            _loc4_ = GetColumnVerticalOffset(_loc2_);
            _loc5_ += columnHorizontalOffset;
            _loc7_ = GetNumRowsInColumn(_loc2_);
         }
      }
   }
}
function onUnload()
{
   delete GetNumRowsInColumn;
   delete GetColumnVerticalOffset;
   delete ComputeSlotPositionFromIndex;
   delete CreateNewButtonSlot;
   delete MoveButtonSlot;
   delete InsertButtonSlot;
   delete EraseButtonSlot;
   delete SetButtonSlotCount;
   delete onUnload;
}
if(!initialized)
{
   buttonStageWidth = 62;
   buttonStageHeight = 62;
   buttonSlotWidth = buttonStageWidth - 1;
   buttonSlotHeight = buttonStageHeight - 1;
   columnHorizontalOffset = buttonSlotWidth;
   numRowsInFirstColumn = 6;
   buttonSlots = new Array();
   nextButtonSlotId = 0;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      SetButtonSlotCount(3);
   }
}
stop();
