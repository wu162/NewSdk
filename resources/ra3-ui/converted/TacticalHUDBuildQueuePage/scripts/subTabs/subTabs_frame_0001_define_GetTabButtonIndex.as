function GetTabButtonIndex(tabButton)
{
   if(tabButton == undefined)
   {
      return -1;
   }
   var _loc1_ = 0;
   while(_loc1_ < tabButtons.length)
   {
      if(tabButton == tabButtons[_loc1_])
      {
         return _loc1_;
      }
      _loc1_ = _loc1_ + 1;
   }
   return -1;
}
function SelectTabButton(tabButton)
{
   if(tabButton != selectedTabButton)
   {
      if(selectedTabButton != undefined)
      {
         selectedTabButton.gotoAndPlay("_up");
      }
      if(tabButton != undefined)
      {
         tabButton.gotoAndPlay("_selected");
      }
      selectedTabButton = tabButton;
      client.OnSubTabSelected(GetTabButtonIndex(selectedTabButton));
      OutlineSelectedTab();
   }
}
function OutlineSelectedTab()
{
   var _loc1_ = GetTabButtonIndex(selectedTabButton) - tabScrollPos;
   if(_loc1_ >= 0 && _loc1_ < numVisibleTabs)
   {
      tabOutline.gotoAndPlay("_" + String(_loc1_));
   }
   else
   {
      tabOutline.gotoAndPlay("_none");
   }
}
function SelectTab(index)
{
   SelectTabButton(tabButtons[index]);
}
function SetTabFlashing(index, flashing)
{
   tabButtons[index].flashClip.gotoAndPlay(!flashing ? "_off" : "_on");
}
function GetSelectedTabNum()
{
   return GetTabButtonIndex(selectedTabButton);
}
function OnPressed(buttonClip)
{
   client.OnSubTabPressed(GetTabButtonIndex(buttonClip));
}
function OnClicked(buttonClip)
{
   client.OnSubTabClicked(GetTabButtonIndex(buttonClip));
   SelectTabButton(buttonClip);
}
function OnRollOver(buttonClip)
{
   client.OnSubTabRollOver(GetTabButtonIndex(buttonClip));
}
function OnRollOut(buttonClip)
{
   client.OnSubTabRollOut(GetTabButtonIndex(buttonClip));
}
function ShouldTabButtonBeVisible(tabButton)
{
   var _loc2_ = - tabSlotWidth - (buttons._x - buttonStageLeft);
   var _loc1_ = tabSlotWidth * numVisibleTabs - (buttons._x - buttonStageLeft);
   return tabButton._x > _loc2_ && tabButton._x < _loc1_;
}
function UpdateTabButtonVisibility()
{
   var _loc1_ = 0;
   while(_loc1_ < tabButtons.length)
   {
      tabButtons[_loc1_]._visible = ShouldTabButtonBeVisible(tabButtons[_loc1_]);
      _loc1_ = _loc1_ + 1;
   }
   OutlineSelectedTab();
}
function ScrollTo(index)
{
   buttons._x = buttonStageLeft - index * tabSlotWidth;
   tabScrollPos = index;
   UpdateTabButtonVisibility();
   SetSpinnerButtonEnabled(spinLeftButton,CanScrollLeft());
   SetSpinnerButtonEnabled(spinRightButton,CanScrollRight());
}
function InsertTab(index, iconLabel, tabText)
{
   var _loc4_ = index * tabSlotWidth;
   var _loc6_ = nextTabButtonId++;
   var _loc5_ = buttons.attachMovie("TabButton",String(_loc6_),_loc6_);
   _loc5_._x = _loc4_;
   _loc5_._visible = ShouldTabButtonBeVisible(_loc5_);
   _loc5_.iconLabel = iconLabel;
   _loc5_.text = tabText;
   _loc5_.client = this;
   tabButtons.splice(index,0,_loc5_);
   var _loc2_;
   while(true)
   {
      index = index + 1;
      if(index >= tabButtons.length)
      {
         break;
      }
      _loc4_ += tabSlotWidth;
      _loc2_ = tabButtons[index];
      _loc2_._x = _loc4_;
      _loc2_._visible = ShouldTabButtonBeVisible(_loc2_);
   }
   SetSpinnerButtonEnabled(spinRightButton,CanScrollRight());
}
function EraseTab(index)
{
   var _loc5_ = tabButtons[index];
   var _loc3_ = _loc5_._x;
   if(selectedTabButton == _loc5_)
   {
      SelectTabButton(undefined);
   }
   _loc5_.removeMovieClip();
   tabButtons.splice(index,1);
   var _loc1_;
   if(index < tabButtons.length)
   {
      while(true)
      {
         _loc1_ = tabButtons[index];
         _loc1_._x = _loc3_;
         _loc1_._visible = ShouldTabButtonBeVisible(_loc1_);
         index = index + 1;
         if(index >= tabButtons.length)
         {
            break;
         }
         _loc3_ += tabSlotWidth;
      }
   }
   if(tabButtons.length == 0)
   {
      nextTabButtonId = 0;
   }
   var _loc4_ = tabScrollPos;
   if(_loc4_ > tabButtons.length - numVisibleTabs)
   {
      _loc4_ = tabButtons.length - numVisibleTabs;
   }
   if(_loc4_ < 0)
   {
      _loc4_ = 0;
   }
   if(_loc4_ != tabScrollPos)
   {
      ScrollTo(_loc4_);
   }
}
function CanScrollLeft()
{
   return tabScrollPos > 0;
}
function CanScrollRight()
{
   return tabScrollPos < tabButtons.length - numVisibleTabs;
}
function SetSpinnerButtonEnabled(buttonClip, enable)
{
   if(buttonClip.initialized)
   {
      buttonClip.SetEnabled(enable);
   }
   else
   {
      buttonClip.isEnabled = enable;
   }
}
function OnSpinnerButtonClicked(buttonClip)
{
   if(buttonClip == spinLeftButton)
   {
      if(CanScrollLeft())
      {
         ScrollTo(tabScrollPos - 1);
      }
   }
   else if(CanScrollRight())
   {
      ScrollTo(tabScrollPos + 1);
   }
}
function MakeTabVisible(index)
{
   if(index < tabScrollPos)
   {
      ScrollTo(index);
   }
   else if(index >= tabScrollPos + numVisibleTabs)
   {
      ScrollTo(index - numVisibleTabs + 1);
   }
}
function onUnload()
{
   delete GetTabButtonIndex;
   delete SelectTabButton;
   delete OutlineSelectedTab;
   delete SelectTab;
   delete SetTabFlashing;
   delete GetSelectedTabNum;
   delete OnPressed;
   delete OnClicked;
   delete OnRollOver;
   delete OnRollOut;
   delete ShouldTabButtonBeVisible;
   delete UpdateTabButtonVisibility;
   delete ScrollTo;
   delete InsertTab;
   delete EraseTab;
   delete CanScrollLeft;
   delete CanScrollRight;
   delete SetSpinnerButtonEnabled;
   delete OnSpinnerButtonClicked;
   delete MakeTabVisible;
   delete onUnload;
}
if(!initialized)
{
   if(!extern.InGame)
   {
      _root.GetCurrentFaction = function()
      {
         return "Alien";
      };
   }
   tabSlotWidth = 29;
   numVisibleTabs = 5;
   buttonStageLeft = buttons._x;
   tabScrollPos = 0;
   if(client == undefined)
   {
      client = _parent;
   }
   tabButtons = new Array();
   nextTabButtonId = 0;
   SetSpinnerButtonEnabled(spinLeftButton,false);
   SetSpinnerButtonEnabled(spinRightButton,false);
   initialized = true;
   client.OnSubTabsInitialized(this);
}
