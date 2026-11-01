function GetContentStage()
{
   return Pointer.encode(box.contentStage);
}
function AlignVertically(siteY)
{
   if(siteVerticalAlignment == "top")
   {
      box._y = siteY;
   }
   else if(siteVerticalAlignment == "bottom")
   {
      box._y = siteY - box.background._height;
   }
   else
   {
      box._y = siteY - box.background._height / 2;
   }
}
function AlignHorizontally(siteX)
{
   if(siteHorizontalAlignment == "left")
   {
      box._x = siteX;
   }
   else if(siteHorizontalAlignment == "right")
   {
      box._x = siteX - box.background._width;
   }
   else
   {
      box._x = siteX - box.background._width / 2;
   }
}
function Open(siteRef, newContentWidth, newContentHeight)
{
   site = Pointer.decode(siteRef);
   newContentWidth = Number(newContentWidth);
   newContentHeight = Number(newContentHeight);
   box._visible = false;
   var _loc3_ = newContentWidth + contentLeftMargin + contentRightMargin;
   var _loc2_ = newContentHeight + contentTopMargin + contentBottomMargin;
   var _loc4_ = _loc3_;
   box.frameTop._width = _loc4_;
   box.frameBottom._width = _loc4_;
   var _loc8_ = _loc3_ - box.frameRight._width;
   box.frameRight._x = _loc8_;
   var _loc5_ = _loc2_;
   box.frameLeft._height = _loc5_;
   box.frameRight._height = _loc5_;
   var _loc9_ = _loc2_ - box.frameBottom._height;
   box.frameBottom._y = _loc9_;
   box.background._width = _loc3_;
   box.background._height = _loc2_;
   box.contentStage._x = contentLeftMargin;
   box.contentStage._y = contentTopMargin;
   curContentWidth = newContentWidth;
   curContentHeight = newContentHeight;
   siteVerticalAlignment = site.GetHelpBoxVerticalAlignment();
   siteHorizontalAlignment = site.GetHelpBoxHorizontalAlignment();
   box.OnMasterResize(box);
   onEnterFrame = function()
   {
      MoveToSite();
      box._visible = true;
      fscommand(qualifyName(this,"OnOpen"),"");
      onEnterFrame = MoveToSite;
   };
}
function GlobalToLocal(pos)
{
   var _loc9_ = _root;
   var _loc6_ = 0;
   var _loc5_ = 0;
   var _loc8_ = 1;
   var _loc7_ = 1;
   ancestor = this;
   var _loc4_;
   var _loc3_;
   while(ancestor != _loc9_)
   {
      _loc4_ = ancestor._xscale * 0.009999999776482582;
      _loc6_ *= _loc4_;
      _loc6_ -= ancestor._x;
      _loc8_ /= _loc4_;
      _loc3_ = ancestor._yscale * 0.009999999776482582;
      _loc5_ *= _loc3_;
      _loc5_ -= ancestor._y;
      _loc7_ /= _loc3_;
      ancestor = ancestor._parent;
   }
   pos.x = (pos.x + _loc6_) * _loc8_;
   pos.y = (pos.y + _loc5_) * _loc7_;
}
function MoveToSite()
{
   if(site == undefined)
   {
      return undefined;
   }
   var _loc3_ = site.GetHelpBoxPos();
   GlobalToLocal(_loc3_);
   var _loc5_ = site.GetHelpBoxVerticalAlignment();
   var _loc8_ = site.GetHelpBoxHorizontalAlignment();
   if(_loc5_ != siteVerticalAlignment || _loc8_ != siteHorizontalAlignment)
   {
      siteVerticalAlignment = _loc5_;
      siteHorizontalAlignment = _loc8_;
   }
   AlignVertically(_loc3_.y);
   AlignHorizontally(_loc3_.x);
   if(_root.displayLeft == undefined)
   {
      return undefined;
   }
   var _loc2_ = _root.displayLeft;
   var _loc4_ = _root.displayTop;
   var _loc6_ = _loc2_ + _root.displayWidth;
   var _loc7_ = _loc4_ + _root.displayHeight;
   if(box._x < _loc2_)
   {
      box._x = _loc2_;
   }
   if(box._x + box._width > _loc6_)
   {
      box._x = _loc6_ - box._width;
   }
   if(box._y < _loc4_)
   {
      box._y = _loc4_;
   }
   if(box._y + box._height > _loc7_)
   {
      box._y = _loc7_ - box._height;
   }
}
function Close()
{
   delete onEnterFrame;
   box._visible = false;
   fscommand(qualifyName(this,"OnClosed"),"");
}
function Hide()
{
   box._visible = false;
}
function onUnload()
{
   delete GetContentStage;
   delete Open;
   delete onEnterFrame;
   delete AlignVertically;
   delete AlignHorizontally;
   delete GlobalToLocal;
   delete MoveToSite;
   delete Close;
   delete Hide;
   delete onUnload;
}
if(!initialized)
{
   curContentWidth = box.contentSizer._width;
   curContentHeight = box.contentSizer._height;
   contentLeftMargin = box.contentSizer._x;
   contentRightMargin = box._width - (box.contentSizer._x + curContentWidth);
   contentTopMargin = box.contentStage._y;
   contentBottomMargin = box._height - (box.contentSizer._y + curContentHeight);
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
stop();
