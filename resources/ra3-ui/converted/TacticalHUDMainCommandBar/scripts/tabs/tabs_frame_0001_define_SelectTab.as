function SelectTab(tabNum)
{
   if(tabNum != selectedTabNum)
   {
      this[String(selectedTabNum)].gotoAndPlay("_up");
      this[String(tabNum)].gotoAndPlay("_selected");
      selectedTabNum = tabNum;
      client.OnTabSelected(selectedTabNum);
   }
   tabOutline.gotoAndPlay("_" + String(tabNum));
}
function GetSelectedTabNum()
{
   return selectedTabNum;
}
function OnClicked(buttonClip)
{
   var _loc1_ = Number(buttonClip._name);
   if(_loc1_ != selectedTabNum)
   {
      SelectTab(_loc1_);
      TabButtonGlow.gotoAndStop("_" + buttonClip._name);
   }
   else
   {
      client.OnSelectedTabClicked(_loc1_);
   }
}
function OnRollOver(buttonClip)
{
   client.OnTabRollOver(Number(buttonClip._name));
}
function OnRollOut(buttonClip)
{
   client.OnTabRollOut(Number(buttonClip._name));
}
function SetTabFlashing(tabNum, flashing)
{
   var _loc2_ = this[String(tabNum)];
   _loc2_.flashClip.gotoAndPlay(!flashing ? "_off" : "_on");
}
function SetTabHighlighted(tabNum, highlighted)
{
   if(highlighted)
   {
      if(tabHighlightClips[tabNum] == null)
      {
         tabHighlightClips[tabNum] = _global.CreateHUDGadgetFlash(this[String(tabNum)]);
      }
   }
   else
   {
      tabHighlightClips[tabNum].Close();
      tabHighlightClips[tabNum] = null;
   }
}
function onUnload()
{
   var _loc1_;
   while(tabHighlightClips.length > 0)
   {
      _loc1_ = tabHighlightClips.pop();
      _loc1_.Close();
   }
   delete SelectTab;
   delete GetSelectedTabNum;
   delete OnClicked;
   delete OnRollOver;
   delete OnRollOut;
   delete SetTabFlashing;
   delete SetTabHighlighted;
   delete onUnload;
}
if(!initialized)
{
   tabCount = _parent.pages.length;
   tabHighlightClips = new Array(tabCount);
   var i = 0;
   while(i < tabCount)
   {
      this[String(i)].iconLabel = _parent.pages[i].tabIcon;
      tabHighlightClips[i] = null;
      i++;
   }
   if(client == undefined)
   {
      client = _parent;
   }
   selectedTabNum = 0;
   this["0"].gotoAndPlay("_selected");
   initialized = true;
   client.OnTabsInitialized(this);
}
