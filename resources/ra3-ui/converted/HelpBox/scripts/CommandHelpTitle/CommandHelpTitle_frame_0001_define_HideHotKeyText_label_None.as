function HideHotKeyText()
{
   textMC.gotoAndStop("_noHotKey");
}
function SetUnitAbilityCategory(category)
{
   gotoAndPlay(category);
}
function Move(newX, newY, newWidth, newHeight)
{
   _X = newX;
   _Y = newY;
   var _loc1_;
   var _loc2_;
   if(newWidth < _width)
   {
      _loc1_ = _width - newWidth;
      _loc2_ = textMC._width;
      textMC._width = _loc1_ >= _loc2_ - 1 ? 1 : _loc2_ - _loc1_;
      _width = newWidth;
   }
   if(newHeight < _height)
   {
      _height = newHeight;
   }
}
function OnMasterResize(mc)
{
   var _loc1_ = mc.contentStage._x + _X;
   if(categoryIcon != undefined)
   {
      categoryIcon._x -= _loc1_;
      textMC._x -= _loc1_;
   }
   background._x -= _loc1_;
   background._width = mc.background._width * 100 / _xscale;
}
function onUnload()
{
   _parent._parent.UnregisterMasterResizeListener(this);
   delete HideHotKeyText;
   delete SetUnitAbilityCategory;
   delete Move;
   delete OnMasterResize;
   delete onUnload;
}
if(!initialized)
{
   _parent._parent.RegisterMasterResizeListener(this);
   var spacing = textMC.hotKeyText._x - (textMC.titleText._x + textMC.titleText._width);
   textMC.titleText.text = "$" + qualifyName(this,"Title") + "&outline";
   textMC.titleText.field.autoSize = "left";
   textMC.titleText.field._width;
   textMC.hotKeyText.text = "$" + qualifyName(this,"HotKey") + "&outline";
   textMC.hotKeyText.field.autoSize = "left";
   textMC.hotKeyText.field._width;
   textMC.hotKeyText._x = textMC.titleText.field._x + textMC.titleText.field._width + spacing;
   verticalSizer._visible = false;
   initialized = true;
   fscommand(qualifyName(this,"OnInitialized"),"");
}
