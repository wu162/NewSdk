function SetMenuItemCount(itemCount)
{
   panelBorderBottom._y = panelBorderTop._y + 5;
   var _loc2_;
   var _loc1_;
   var _loc3_;
   if(numItems < itemCount)
   {
      _loc2_ = numItems;
      while(_loc2_ < itemCount)
      {
         _loc1_ = panel.BaseMenuItem.attachMovie("MenuItem",String(_loc2_),_loc2_);
         _loc1_.text.text = "$" + qualifyName(_loc1_,"ItemText");
         _loc1_._y = _loc1_._height * _loc2_;
         panel.DialogBackground._height += _loc1_._height;
         panelBorderBottom._y += _loc1_._height;
         _loc2_ = _loc2_ + 1;
      }
      numItems = itemCount;
   }
   else if(numItems > itemCount)
   {
      _loc2_ = numItems;
      while(_loc2_ > itemCount)
      {
         _loc3_ = panel.BaseMenuItem[String(_loc2_ - 1)];
         panel.DialogBackground._height -= _loc3_._height;
         _loc3_.removeMovieClip();
         _loc2_ = _loc2_ - 1;
      }
      numItems = itemCount;
   }
}
function ClearMenuItems()
{
   var _loc1_;
   while(numItems > 0)
   {
      _loc1_ = panel.BaseMenuItem[String(numItems - 1)];
      panel.DialogBackground._height -= _loc1_._height;
      _loc1_.removeMovieClip();
      numItems--;
   }
}
function GetMenuItem(itemNum)
{
   return Pointer.encode(panel.BaseMenuItem[String(itemNum)]);
}
function Close()
{
   if(!closing)
   {
      closing = true;
      gotoAndStop("_close");
      play();
   }
}
function onUnload()
{
   delete SetMenuItemCount;
   delete ClearItems;
   delete GetItem;
   delete Close;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   closing = false;
   numItems = 0;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
