function OnSubTabsInitialized()
{
   subTabsInitialized = true;
   if(initialized)
   {
      fscommand(qualifyName(this,"OnInitialized"),"");
   }
}
function OnSubTabSelected(index)
{
   var _loc2_ = index < 0 ? undefined : pages[index];
   if(_loc2_ != selectedPage)
   {
      if(selectedPage != undefined)
      {
         selectedPage._visible = false;
      }
      if(_loc2_ != undefined)
      {
         _loc2_._visible = true;
      }
      selectedPage = _loc2_;
      fscommand(qualifyName(this,"OnPageSelected"),index);
   }
}
function OnSubTabPressed(index)
{
   pages[index].OnTabPressed();
}
function OnSubTabRollOver(index)
{
   pages[index].OnTabRollOver();
}
function OnSubTabRollOut(index)
{
   pages[index].OnTabRollOut();
}
function SelectPage(indexArg)
{
   var _loc1_ = Number(indexArg);
   subTabs.SelectTab(_loc1_);
   subTabs.MakeTabVisible(_loc1_);
}
function SetTabFlashing(index, flashingArg)
{
   var _loc1_ = flashingArg != 0;
   subTabs.SetTabFlashing(index,_loc1_);
}
function InsertPage(index)
{
   if(pages.length == 0)
   {
      tabButton.gotoAndPlay("_up");
   }
   var _loc2_ = nextPageId++;
   var _loc1_ = attachMovie("BuildQueuePage",String(_loc2_),_loc2_);
   _loc1_._x = pagePos._x;
   _loc1_._y = pagePos._y;
   _loc1_._visible = false;
   _loc1_.tabHelpSite = subTabHelpSite;
   subTabs.InsertTab(index,tabIcon,"$" + qualifyName(_loc1_,"Text"));
   pages.splice(index,0,_loc1_);
   return Pointer.encode(_loc1_);
}
function ErasePage(index)
{
   subTabs.EraseTab(index);
   var _loc2_ = pages[index];
   if(_loc2_ == selectedPage)
   {
      SelectPage(undefined);
   }
   _loc2_.removeMovieClip();
   pages.splice(index,1);
   if(pages.length == 0)
   {
      nextPageId = 0;
      if(_visible)
      {
         _parent.SelectTab(0);
      }
      tabButton.gotoAndPlay("_disabled");
      OnTabRollOut();
   }
}
function OnTabRollOver()
{
   if(!mouseOverTab)
   {
      mouseOverTab = true;
      fscommand(qualifyName(this,"OnTabRollOver"),"");
   }
}
function OnTabRollOut()
{
   if(mouseOverTab)
   {
      mouseOverTab = false;
      fscommand(qualifyName(this,"OnTabRollOut"),"");
   }
}
function OnSubTabClicked(index)
{
   fscommand(qualifyName(this,"OnTabClicked"),String(index));
}
function GetTabHelpSite()
{
   return Pointer.encode(tabHelpSite);
}
function onUnload()
{
   delete OnSubTabsInitialized;
   delete OnSubTabSelected;
   delete OnSubTabPressed;
   delete OnSubTabRollOver;
   delete OnSubTabRollOut;
   delete SelectPage;
   delete SetTabFlashing;
   delete InsertPage;
   delete ErasePage;
   delete OnTabRollOver;
   delete OnTabRollOut;
   delete OnSubTabClicked;
   delete TabHelpSite;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnChildLoaded(this);
   pages = new Array();
   nextPageId = 0;
   mouseOverTab = false;
   initialized = true;
   if(subTabsInitialized)
   {
      fscommand(qualifyName(this,"OnInitialized"),"");
   }
}
stop();
