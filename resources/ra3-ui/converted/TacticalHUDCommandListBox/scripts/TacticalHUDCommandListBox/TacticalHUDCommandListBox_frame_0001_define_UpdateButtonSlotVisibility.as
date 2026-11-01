function UpdateButtonSlotVisibility()
{
   var _loc4_ = buttonStage._y;
   var _loc5_ = _loc4_ + visibleListHeight;
   var _loc1_ = 0;
   var _loc2_;
   var _loc3_;
   while(_loc1_ < buttonSlots.length)
   {
      _loc2_ = buttonSlots[_loc1_].stage;
      _loc3_ = _loc2_._y < _loc5_ && _loc2_._y + buttonSlotHeight > _loc4_;
      _loc2_._visible = _loc3_;
      _loc1_ = _loc1_ + 1;
   }
}
function ShouldButtonSlotBeVisible(slotStage)
{
   var _loc1_ = - buttonStage._y;
   var _loc2_ = _loc1_ + visibleListHeight;
   return slotStage._y < _loc2_ && slotStage._y + buttonSlotHeight > _loc1_;
}
function CreateNewButtonSlot(slotIndex, slotX, slotY)
{
   var _loc4_ = nextButtonSlotId++;
   var _loc2_ = buttonStage.attachMovie("LoadMovieStage",String(_loc4_),_loc4_);
   _loc2_._x = slotX;
   _loc2_._y = slotY;
   _loc2_._visible = ShouldButtonSlotBeVisible(_loc2_);
   var _loc3_ = buttonStage.attachMovie("HelpBoxSite","helpSite" + String(_loc4_),_loc4_ + 65536);
   _loc3_._x = buttonHelpSite._x;
   _loc3_._y = slotY;
   _loc3_.vertAlignment = buttonHelpSite.vertAlignment;
   _loc3_.horzAlignment = buttonHelpSite.horzAlignment;
   _loc2_.helpSite = _loc3_;
   var _loc5_ = {stage:_loc2_,helpSite:_loc3_};
   buttonSlots.splice(slotIndex,0,_loc5_);
   fscommand(qualifyName(this,"OnButtonSlotStageCreated"),"index=" + slotIndex + "&name=" + Pointer.encode(_loc2_));
   return _loc5_;
}
function MoveButtonSlot(slotIndex, slotX, slotY)
{
   var _loc2_ = buttonSlots[slotIndex];
   var _loc1_ = _loc2_.stage;
   _loc1_._x = slotX;
   _loc1_._y = slotY;
   _loc1_._visible = ShouldButtonSlotBeVisible(_loc1_);
   var _loc3_ = _loc2_.helpSite;
   _loc3_._y = slotY;
}
function InsertButtonSlot(index)
{
   var _loc5_ = Math.floor(index / numColumns);
   var _loc3_ = index % numColumns;
   var _loc4_ = _loc5_ * buttonSlotHeight;
   var _loc2_ = _loc3_ * buttonSlotWidth;
   CreateNewButtonSlot(index,_loc2_,_loc4_);
   index = index + 1;
   while(index < buttonSlots.length)
   {
      _loc3_ = _loc3_ + 1;
      if(_loc3_ < numColumns)
      {
         _loc2_ += buttonSlotWidth;
      }
      else
      {
         _loc3_ = 0;
         _loc2_ = 0;
         _loc4_ += buttonSlotHeight;
      }
      MoveButtonSlot(index,_loc2_,_loc4_);
      index = index + 1;
   }
   UpdateScrollBarState(scrollBar);
}
function EraseButtonSlot(index)
{
   var _loc2_ = buttonSlots[index].stage._x;
   var _loc4_ = buttonSlots[index].stage._y;
   buttonSlots[index].stage.removeMovieClip();
   buttonSlots[index].helpSite.removeMovieClip();
   buttonSlots.splice(index,1);
   var _loc3_ = index % numColumns;
   if(buttonSlots.length > 0)
   {
      if(index < buttonSlots.length)
      {
         while(true)
         {
            MoveButtonSlot(index,_loc2_,_loc4_);
            index = index + 1;
            if(index >= buttonSlots.length)
            {
               break;
            }
            _loc3_ = _loc3_ + 1;
            if(_loc3_ < numColumns)
            {
               _loc2_ += buttonSlotWidth;
            }
            else
            {
               _loc3_ = 0;
               _loc2_ = 0;
               _loc4_ += buttonSlotHeight;
            }
         }
      }
   }
   else
   {
      nextButtonSlotId = 0;
   }
   UpdateScrollBarState(scrollBar);
}
function SetButtonSlotCount(newCount)
{
   var _loc1_;
   var _loc6_;
   var _loc2_;
   var _loc4_;
   var _loc3_;
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
      _loc6_ = Math.floor(buttonSlots.length / numColumns);
      _loc2_ = buttonSlots.length % numColumns;
      _loc4_ = _loc6_ * buttonSlotHeight;
      _loc3_ = _loc2_ * buttonSlotWidth;
      while(true)
      {
         CreateNewButtonSlot(buttonSlots.length,_loc3_,_loc4_);
         if(buttonSlots.length >= newCount)
         {
            break;
         }
         _loc2_ = _loc2_ + 1;
         if(_loc2_ < numColumns)
         {
            _loc3_ += buttonSlotWidth;
         }
         else
         {
            _loc2_ = 0;
            _loc3_ = 0;
            _loc4_ += buttonSlotHeight;
         }
      }
   }
   UpdateScrollBarState(scrollBar);
}
function OnScrollBarPosChanged(pos)
{
   onEnterFrame = function()
   {
      var _loc6_ = scrollBar.GetCurPos();
      var _loc3_ = Math.ceil(buttonSlots.length / numColumns);
      var _loc4_ = Math.round(_loc3_ * _loc6_);
      var _loc1_ = - _loc4_ * buttonSlotHeight;
      var _loc5_ = _loc1_ - buttonStage._y;
      buttonStage._y += _loc5_ / 2;
      var _loc2_;
      if(_loc1_ < buttonStage._y)
      {
         _loc2_ = buttonStage._y - buttonSlotHeight / 4;
         if(_loc1_ < _loc2_)
         {
            buttonStage._y = _loc2_;
         }
         else
         {
            buttonStage._y = _loc1_;
            delete onEnterFrame;
         }
      }
      else
      {
         _loc2_ = buttonStage._y + buttonSlotHeight / 4;
         if(_loc1_ > _loc2_)
         {
            buttonStage._y = _loc2_;
         }
         else
         {
            buttonStage._y = _loc1_;
            delete onEnterFrame;
         }
      }
      UpdateButtonSlotVisibility();
   };
}
function UpdateScrollBarState(clip)
{
   var _loc1_ = Math.ceil(buttonSlots.length / numColumns);
   if(_loc1_ < 1)
   {
      _loc1_ = 1;
   }
   clip.SetLineSize(1 / _loc1_);
   if(_loc1_ <= numVisibleRows)
   {
      clip.SetPageSize(1);
      clip.SetEnabled(false);
      clip.SetVisible(false);
   }
   else
   {
      clip.SetPageSize(numVisibleRows / _loc1_);
      clip.SetEnabled(true);
      clip.SetVisible(true);
   }
}
function GetScrollBarParam(clip)
{
   return scrollBarParam;
}
function OnScrollBarLoaded(clip)
{
   clip.SetEnabled(true);
   clip.SetHeight(scrollBarPlaceholder._height);
   clip._xscale = 100;
   clip._yscale = 100;
   UpdateScrollBarState(clip);
   clip.OnPosChanged = function(pos)
   {
      OnScrollBarPosChanged(pos);
   };
}
function onUnload()
{
   delete UpdateButtonSlotVisibility;
   delete ShouldButtonSlotBeVisible;
   delete CreateNewButtonSlot;
   delete MoveButtonSlot;
   delete InsertButtonSlot;
   delete EraseButtonSlot;
   delete SetButtonSlotCount;
   delete OnScrollBarPosChanged;
   delete UpdateScrollBarState;
   delete GetScrollBarParam;
   delete OnScrollBarLoaded;
   delete onUnload;
}
var upButtonParam = new Object();
upButtonParam.disabledImage = "VSliderUpButtonDisabledDynFac";
upButtonParam.upImage = "VSliderUpButtonEnabledDynFac";
upButtonParam.overImage = "VSliderUpButtonOverDynFac";
upButtonParam.downImage = "VSliderUpButtonPressDynFac";
var downButtonParam = new Object();
downButtonParam.disabledImage = "VSliderDownButtonDisabledDynFac";
downButtonParam.upImage = "VSliderDownButtonEnabledDynFac";
downButtonParam.overImage = "VSliderDownButtonOverDynFac";
downButtonParam.downImage = "VSliderDownButtonPressDynFac";
var thumbTop = new Object();
thumbTop.disabledImage = "VSliderThumbDisabledTopDynFac";
thumbTop.upImage = "VSliderThumbEnabledTopDynFac";
thumbTop.overImage = "VSliderThumbOverTopDynFac";
thumbTop.downImage = "VSliderThumbPressTopDynFac";
var thumbMiddle = new Object();
thumbMiddle.disabledImage = "VSliderThumbDisabledMiddleDynFac";
thumbMiddle.upImage = "VSliderThumbEnabledMiddleDynFac";
thumbMiddle.overImage = "VSliderThumbOverMiddleDynFac";
thumbMiddle.downImage = "VSliderThumbPressMiddleDynFac";
var thumbBottom = new Object();
thumbBottom.disabledImage = "VSliderThumbDisabledBottomDynFac";
thumbBottom.upImage = "VSliderThumbEnabledBottomDynFac";
thumbBottom.overImage = "VSliderThumbOverBottomDynFac";
thumbBottom.downImage = "VSliderThumbPressBottomDynFac";
var thumbParam = new Object();
thumbParam.topPiece = thumbTop;
thumbParam.middlePiece = thumbMiddle;
thumbParam.bottomPiece = thumbBottom;
var barParam = new Object();
barParam.disabledImage = "VSliderBarDisabledDynFac";
barParam.upImage = "VSliderBarUpOverDynFac";
barParam.overImage = "VSliderBarUpOverDynFac";
scrollBarParam = new Object();
scrollBarParam.upButton = upButtonParam;
scrollBarParam.downButton = downButtonParam;
scrollBarParam.thumb = thumbParam;
scrollBarParam.bar = barParam;
if(!initialized)
{
   _parent.OnContentLoaded(this);
   buttonStageWidth = 50;
   buttonStageHeight = 50;
   if(buttonSlotWidth == undefined)
   {
      buttonSlotWidth = 55;
   }
   if(buttonSlotHeight == undefined)
   {
      buttonSlotHeight = 52;
   }
   if(numColumns == undefined)
   {
      numColumns = 3;
   }
   if(numVisibleRows == undefined)
   {
      numVisibleRows = 3;
   }
   visibleListHeight = numVisibleRows * buttonSlotHeight;
   buttonSlots = new Array();
   nextButtonSlotId = 0;
   scrollBar.loadMovie("VerticalScrollBar.swf");
   fscommand(qualifyName(this,"OnInitialized"),"");
   initialized = true;
}
stop();
