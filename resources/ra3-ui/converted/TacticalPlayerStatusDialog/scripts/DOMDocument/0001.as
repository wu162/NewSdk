function Close()
{
   if(!closing)
   {
      gotoAndStop("_close");
      play();
      closing = true;
   }
}
function SetStatusLineCount(countArg)
{
   var _loc4_ = Number(countArg);
   var _loc5_;
   var _loc6_;
   var _loc1_;
   var _loc2_;
   var _loc3_;
   if(items.length < _loc4_)
   {
      if(items.length > 0)
      {
         _loc6_ = items[items.length - 1];
         _loc5_ = _loc6_._y + itemHeight;
      }
      else
      {
         _loc5_ = 0;
      }
      do
      {
         _loc1_ = nextItemId++;
         _loc2_ = dialog.table.items.attachMovie("StatusItem",String(_loc1_),_loc1_);
         _loc2_._y = _loc5_;
         _loc5_ += itemHeight;
         items.push(_loc2_);
      }
      while(items.length < _loc4_);
   }
   else if(items.length > _loc4_)
   {
      do
      {
         _loc3_ = items.pop();
         _loc3_.removeMovieClip();
      }
      while(items.length > _loc4_);
      if(items.length == 0)
      {
         nextItemId = 0;
      }
   }
}
function GetStatusLine(indexArg)
{
   var _loc1_ = Number(indexArg);
   var _loc2_ = items[_loc1_];
   return Pointer.encode(_loc2_);
}
function onUnload()
{
   delete SetStatusLineCount;
   delete GetStatusLine;
   delete Close;
   delete onUnload;
}
if(!initialized)
{
   _parent.OnContentLoaded(this);
   itemHeight = 28;
   items = new Array();
   nextItemId = 0;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
   if(!extern.InGame)
   {
      SetStatusLineCount("9");
   }
}
