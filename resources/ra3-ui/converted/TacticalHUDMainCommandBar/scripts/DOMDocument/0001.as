function GetSelectionRefinementPage()
{
   return Pointer.encode(sel);
}
function GetBuildQueueTypePage(pageName)
{
   return Pointer.encode(this[pageName]);
}
function GetBuildQueueTypeTab(pageNameArg)
{
   var _loc2_ = String(pageNameArg);
   var _loc1_ = 0;
   while(_loc1_ < pages.length)
   {
      if(pages[_loc1_].clip._name == _loc2_)
      {
         return Pointer.encode(tabs[String(_loc1_)]);
      }
      _loc1_ = _loc1_ + 1;
   }
   return undefined;
}
function SelectSelectionRefinementPage()
{
   SelectPage(sel);
}
function SelectBuildQueueTypePage(pageName)
{
   SelectPage(this[pageName]);
}
function SetBuildQueueTypeTabFlashing(pageName, flashingArg)
{
   SetTabFlashing(this[pageName],flashingArg != 0);
}
function SetSelectionRefinementTabHighlighted(highlightedArg)
{
   SetTabHighlighted(sel,highlightedArg != 0);
}
function SetBuildQueueTypeTabHighlighted(pageName, highlightedArg)
{
   SetTabHighlighted(this[pageName],highlightedArg != 0);
}
function OnTabSelected(tabNum)
{
   var _loc2_;
   if(tabNum != visiblePageNum)
   {
      pages[visiblePageNum].clip._visible = false;
      _loc2_ = pages[tabNum].clip;
      _loc2_._visible = true;
      visiblePageNum = tabNum;
      if(_loc2_ == sel)
      {
         fscommand(qualifyName(this,"OnSelectionRefinementPageSelected"),"");
      }
      else
      {
         fscommand(qualifyName(this,"OnBuildQueueTypePageSelected"),_loc2_._name);
      }
   }
}
function OnSelectedTabClicked(tabNum)
{
   var _loc2_ = pages[tabNum].clip;
   if(_loc2_ == sel)
   {
      fscommand(qualifyName(this,"OnSelectionRefinementTabClicked"),"");
   }
   else
   {
      fscommand(qualifyName(this,"OnBuildQueueTypeTabClicked"),_loc2_._name);
   }
}
function OnTabRollOver(tabNum)
{
   pages[tabNum].clip.OnTabRollOver();
}
function OnTabRollOut(tabNum)
{
   pages[tabNum].clip.OnTabRollOut();
}
function SelectTab(tabNum)
{
   if(tabsInitialized)
   {
      tabs.SelectTab(tabNum);
   }
   else
   {
      selectedTabNum = tabNum;
   }
}
function SelectPage(pageClip)
{
   var _loc1_ = 0;
   while(_loc1_ < pages.length)
   {
      if(pages[_loc1_].clip == pageClip)
      {
         SelectTab(_loc1_);
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
}
function SetTabFlashing(pageClip, flashing)
{
   var _loc1_ = 0;
   while(_loc1_ < pages.length)
   {
      if(pages[_loc1_].clip == pageClip)
      {
         pages[_loc1_].tabFlashing = flashing;
         if(tabsInitialized)
         {
            tabs.SetTabFlashing(_loc1_,flashing);
         }
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
}
function SetTabHighlighted(pageClip, highlighted)
{
   var _loc1_ = 0;
   while(_loc1_ < pages.length)
   {
      if(pages[_loc1_].clip == pageClip)
      {
         pages[_loc1_].tabHighlighted = highlighted;
         if(tabsInitialized)
         {
            tabs.SetTabHighlighted(_loc1_,highlighted);
         }
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
}
function OnTabsInitialized(clip)
{
   var _loc1_;
   if(!tabsInitialized)
   {
      tabsInitialized = true;
      visiblePageNum = String(tabs.GetSelectedTabNum());
      pages[visiblePageNum].clip._visible = true;
      if(selectedTabNum != undefined)
      {
         tabs.SelectTab(selectedTabNum);
         delete selectedTabNum;
      }
      _loc1_ = 0;
      while(_loc1_ < pages.length)
      {
         if(pages[_loc1_].tabFlashing)
         {
            tabs.SetTabFlashing(_loc1_,true);
         }
         if(pages[_loc1_].tabHighlighted)
         {
            tabs.SetTabHighlighted(_loc1_,true);
         }
         _loc1_ = _loc1_ + 1;
      }
   }
}
function OnChildLoaded(childClip)
{
   var _loc3_ = false;
   if(visiblePageNum != undefined)
   {
      _loc3_ = childClip == pages[visiblePageNum].clip;
   }
   childClip._visible = _loc3_;
   var _loc1_ = 0;
   while(_loc1_ < pages.length)
   {
      if(pages[_loc1_].clip == childClip)
      {
         childClip.tabButton = tabs[String(_loc1_)];
         childClip.tabIcon = pages[_loc1_].tabIcon;
         break;
      }
      _loc1_ = _loc1_ + 1;
   }
   childClip.tabHelpSite = tabHelpSite;
}
function onUnload()
{
   delete GetSelectionRefinementPage;
   delete GetBuildQueueTypePage;
   delete GetBuildQueueTypeTab;
   delete SelectSelectionRefinementPage;
   delete SelectBuildQueueTypePage;
   delete SetBuildQueueTypeTabFlashing;
   delete SetSelectionRefinementTabHighlighted;
   delete SetBuildQueueTypeTabHighlighted;
   delete OnTabSelected;
   delete OnTabRollOver;
   delete OnTabRollOut;
   delete SelectTab;
   delete SelectPage;
   delete SetTabFlashing;
   delete SetTabHighlighted;
   delete OnTabsInitialized;
   delete OnChildLoaded;
   delete onUnload;
}
if(!initialized)
{
   pages = [{clip:sel,tabIcon:"Con",tabFlashing:false,tabHighlighted:false},{clip:mainStructure,tabIcon:"Str",tabFlashing:false,tabHighlighted:false},{clip:otherStructure,tabIcon:"Def",tabFlashing:false,tabHighlighted:false},{clip:infantry,tabIcon:"Inf",tabFlashing:false,tabHighlighted:false},{clip:vehicles,tabIcon:"Veh",tabFlashing:false,tabHighlighted:false},{clip:aircraft,tabIcon:"Air",tabFlashing:false,tabHighlighted:false},{clip:navy,tabIcon:"Nav",tabFlashing:false,tabHighlighted:false}];
   if(tabsInitialized)
   {
      visiblePageNum = String(tabs.GetSelectedTabNum());
      pages[visiblePageNum].clip._visible = true;
   }
   sel.loadMovie("TacticalHUDSelectionRefinementPage.swf");
   mainStructure.loadMovie("TacticalHUDBuildQueuePage.swf");
   otherStructure.loadMovie("TacticalHUDBuildQueuePage.swf");
   infantry.loadMovie("TacticalHUDBuildQueuePage.swf");
   vehicles.loadMovie("TacticalHUDBuildQueuePage.swf");
   aircraft.loadMovie("TacticalHUDBuildQueuePage.swf");
   navy.loadMovie("TacticalHUDBuildQueuePage.swf");
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
